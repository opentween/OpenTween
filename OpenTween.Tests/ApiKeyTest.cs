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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenTween
{
    public class ApiKeyTest
    {
        [Fact]
        public void Encrypt_FormatTest()
        {
            var password = "hoge";
            var plainText = "aaaaaaa";
            var encrypted = ApiKey.Encrypt(password, plainText);
            Assert.StartsWith("%e%", encrypted);
            Assert.Equal(5, encrypted.Split('%').Length);
        }

        [Fact]
        public void Encrypt_NonceTest()
        {
            // 同じ平文に対する暗号文を繰り返し生成しても出力は毎回変化する
            var password = "hoge";
            var plainText = "aaaaaaa";
            var encrypted1 = ApiKey.Encrypt(password, plainText);
            var encrypted2 = ApiKey.Encrypt(password, plainText);
            Assert.NotEqual(encrypted1, encrypted2);
        }

        [Fact]
        public void Decrypt_Test()
        {
            var password = "password";
            var encrypted = "%e%m6EH2dECH7HWT9SFE0SK4Q==%mAAWPhPALf48s32s/yQarg==%zoCs8crMqZN6Nfj8ALkl2R3kbD/FORecuepU1LJ3CK0=";
            var decrypted = ApiKey.Decrypt(password, encrypted);
            Assert.Equal("hogehoge", decrypted);
        }

        [Fact]
        public void Decrypt_PlainTextTest()
        {
            // %e% から始まっていない文字列は平文として何もせずに返す
            var password = "password";
            var plainText = "plaintext";
            var decrypted = ApiKey.Decrypt(password, plainText);
            Assert.Equal("plaintext", decrypted);
        }

        [Fact]
        public void Decrypt_InvalidFormatTest()
        {
            var password = "password";
            var encrypted = "%e%INVALID_FORMAT";
            Assert.Throws<ApiKeyDecryptException>(() => ApiKey.Decrypt(password, encrypted));
        }

        [Fact]
        public void Decrypt_InvalidBase64Test()
        {
            var password = "password";
            var encrypted = "%e%!!!!!!!!!!%!!!!!!!!!!%!!!!!!!!!!";
            Assert.Throws<ApiKeyDecryptException>(() => ApiKey.Decrypt(password, encrypted));
        }

        [Fact]
        public void Decrypt_InvalidHMACTest()
        {
            var password = "password";
            var encrypted = "%e%m6EH2dECH7HWT9SFE0SK4Q==%mAAWPhPALf48s32s/yQarg==%AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=";
            Assert.Throws<ApiKeyDecryptException>(() => ApiKey.Decrypt(password, encrypted));
        }

        [Fact]
        public void Decrypt_InvalidHMACLengthTest()
        {
            // HMAC が途中まで一致しているが長さが足りない場合
            var password = "password";
            var encrypted = "%e%m6EH2dECH7HWT9SFE0SK4Q==%mAAWPhPALf48s32s/yQarg==%zoCs8";
            Assert.Throws<ApiKeyDecryptException>(() => ApiKey.Decrypt(password, encrypted));
        }

        [Fact]
        public void EncryptAndDecrypt_Test()
        {
            var password = "hoge";
            var plainText = "aaaaaaa";
            var encrypted = ApiKey.Encrypt(password, plainText);
            Assert.Equal(plainText, ApiKey.Decrypt(password, encrypted));
        }
    }
}
