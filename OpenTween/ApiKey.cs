// OpenTween - Client of Twitter
// Copyright (c) 2022 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
// All rights reserved.
//
// This file is part of OpenTween.
//
// This program is free software; you can redistribute it and/or modify it
// under the terms of the GNU General public License as published by the Free
// Software Foundation; either version 3 of the License, or (at your option)
// any later version.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General public License
// for more details.
//
// You should have received a copy of the GNU General public License along
// with this program. If not, see <http://www.gnu.org/licenses/>, or write to
// the Free Software Foundation, Inc., 51 Franklin Street - Fifth Floor,
// Boston, MA 02110-1301, USA.

#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OpenTween
{
    public class ApiKey
    {
        private static readonly string EncryptionPrefix = "%e%";
        private static readonly int SaltSize = 16;
        private static readonly int KeySize = 32;
        private static readonly int BlockSize = 16;
        private static readonly int IterationCount = 10_000;
        private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA256;

        private readonly string rawKey;
        private readonly Lazy<string> decryptLazy;

        /// <summary>
        /// 平文の API キー
        /// </summary>
        /// <exception cref="ApiKeyDecryptException" />
        public string Value => this.decryptLazy.Value;

        private ApiKey(string password, string rawKey)
        {
            this.rawKey = rawKey;
            this.decryptLazy = new Lazy<string>(
                () => Decrypt(password, this.rawKey)
            );
        }

        /// <summary>
        /// 平文の API キーを返します
        /// </summary>
        /// <returns>
        /// 成功した場合は true、暗号化された API キーの復号に失敗した場合は false を返します
        /// </returns>
        public bool TryGetValue([NotNullWhen(true)]out string? output)
        {
            try
            {
                output = this.Value;
                return true;
            }
            catch (ApiKeyDecryptException)
            {
                output = null;
                return false;
            }
        }

        /// <summary>
        /// <see cref="ApiKey"/> インスタンスを作成します
        /// </summary>
        public static ApiKey Create(string rawKey)
            => Create(ApplicationSettings.EncryptionPassword, rawKey);

        /// <summary>
        /// <see cref="ApiKey"/> インスタンスを作成します
        /// </summary>
        public static ApiKey Create(string password, string rawKey)
            => new(password, rawKey);

        /// <summary>
        /// 指定された文字列を暗号化して返します
        /// </summary>
        public static string Encrypt(string password, string plainText)
        {
            var salt = GenerateSalt();
            var (encryptionKey, iv, macKey) = GenerateKeyAndIV(password, salt);

            using var aes = CreateAes();
            aes.Key = encryptionKey;
            aes.IV = iv;

            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            using var encryptor = aes.CreateEncryptor();
            using var memoryStream = new MemoryStream(plainBytes);
            using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Read);

            using var cipherStream = new MemoryStream(capacity: plainBytes.Length + BlockSize + SaltSize);
            cryptoStream.CopyTo(cipherStream);

            var cipherBytes = cipherStream.ToArray();
            var macBytes = GenerateMAC(plainBytes, macKey);
            var saltText = Convert.ToBase64String(salt);
            var cipherText = Convert.ToBase64String(cipherBytes);
            var macText = Convert.ToBase64String(macBytes);

            return $"{EncryptionPrefix}{saltText}%{cipherText}%{macText}";
        }

        /// <summary>
        /// 暗号化された文字列を復号します
        /// </summary>
        /// <exception cref="ApiKeyDecryptException" />
        public static string Decrypt(string password, string encryptedText)
        {
            // 先頭が "%e%" から始まっていない場合、APIキーは平文のまま書かれているものとして扱う
            if (!encryptedText.StartsWith(EncryptionPrefix))
                return encryptedText;

            var splitted = encryptedText.Split('%');
            if (splitted.Length != 5)
                throw new ApiKeyDecryptException("暗号文のフォーマットが不正です");

            byte[] salt, cipherBytes, macBytes;
            try
            {
                // e.g. "%e%...salt...%...cipher...%...mac..."
                salt = Convert.FromBase64String(splitted[2]);
                cipherBytes = Convert.FromBase64String(splitted[3]);
                macBytes = Convert.FromBase64String(splitted[4]);
            }
            catch (FormatException ex)
            {
                throw new ApiKeyDecryptException("不正な Base64 フォーマットです", ex);
            }

            var (encryptionKey, iv, macKey) = GenerateKeyAndIV(password, salt);
            using var aes = CreateAes();
            aes.Key = encryptionKey;
            aes.IV = iv;

            byte[] decryptedBytes;
            try
            {
                using var decryptor = aes.CreateDecryptor();
                using var memoryStream = new MemoryStream(cipherBytes);
                using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

                using var decryptedStream = new MemoryStream(capacity: cipherBytes.Length);
                cryptoStream.CopyTo(decryptedStream);

                decryptedBytes = decryptedStream.ToArray();
            }
            catch (CryptographicException ex)
            {
                throw new ApiKeyDecryptException("API キーの復号に失敗しました", ex);
            }

            var macBytesExpected = GenerateMAC(decryptedBytes, macKey);

            var isValid = macBytes.Length == macBytesExpected.Length &&
                Enumerable.Zip(macBytes, macBytesExpected, (x, y) => x == y).All(x => x);
            if (!isValid)
                throw new ApiKeyDecryptException("ダイジェストが一致しません");

            return Encoding.UTF8.GetString(decryptedBytes);
        }

        private static byte[] GenerateSalt()
        {
            using var random = new RNGCryptoServiceProvider();
            var salt = new byte[SaltSize];
            random.GetBytes(salt);
            return salt;
        }

        private static (byte[] EncryptionKey, byte[] IV, byte[] MacKey) GenerateKeyAndIV(string password, byte[] salt)
        {
            using var generator = new Rfc2898DeriveBytes(password, salt, IterationCount, HashAlgorithm);
            var encryptionKey = generator.GetBytes(KeySize);
            var iv = generator.GetBytes(BlockSize);
            var macKey = generator.GetBytes(KeySize);
            return (encryptionKey, iv, macKey);
        }

        private static byte[] GenerateMAC(byte[] source, byte[] key)
        {
            using var hmac = new HMACSHA256(key);
            return hmac.ComputeHash(source);
        }

        private static Aes CreateAes()
        {
            var aes = Aes.Create();
            aes.KeySize = KeySize * 8;
            aes.BlockSize = BlockSize * 8;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            return aes;
        }
    }

    public static class ApiKeyExtensions
    {
#pragma warning disable SA1141
        public static bool TryGetValue(this ValueTuple<ApiKey, ApiKey> apiKeys, out ValueTuple<string, string> decryptedKeys)
#pragma warning restore SA1141
        {
            var (apiKey1, apiKey2) = apiKeys;
            if (apiKey1.TryGetValue(out var decrypted1) && apiKey2.TryGetValue(out var decrypted2))
            {
                decryptedKeys = (decrypted1, decrypted2);
                return true;
            }

            decryptedKeys = ("", "");
            return false;
        }
    }

    [Serializable]
    public class ApiKeyDecryptException : Exception
    {
        public ApiKeyDecryptException()
        {
        }

        public ApiKeyDecryptException(string message)
            : base(message)
        {
        }

        public ApiKeyDecryptException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ApiKeyDecryptException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
