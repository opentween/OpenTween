// OpenTween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2011      kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
// All rights reserved.
// 
// This file is part of OpenTween.
// 
// This program is free software; you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the Free
// Software Foundation; either version 3 of the License, or (at your option)
// any later version.
// 
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License
// for more details. 
// 
// You should have received a copy of the GNU General Public License along
// with this program. If not, see <http://www.gnu.org/licenses/>, or write to
// the Free Software Foundation, Inc., 51 Franklin Street - Fifth Floor,
// Boston, MA 02110-1301, USA.

using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Drawing;
using OpenTween.Connection;
using System.Net.Cache;

///<summary>
///HttpWebRequest,HttpWebResponseを使用した基本的な通信機能を提供する
///</summary>
///<remarks>
///プロキシ情報などを設定するため、使用前に静的メソッドInitializeConnectionを呼び出すこと。
///通信方式によって必要になるHTTPヘッダの付加などは、派生クラスで行う。
///</remarks>
namespace OpenTween
{
    public class HttpConnection
    {
        /// <summary>
        /// キャッシュを有効にするか否か
        /// </summary>
        public bool CacheEnabled { get; set; } = true;

        /// <summary>
        /// リクエスト間で Cookie を保持するか否か
        /// </summary>
        public bool UseCookie { get; set; }

        /// <summary>
        /// クッキー保存用コンテナ
        /// </summary>
        private CookieContainer cookieContainer = new CookieContainer();

        protected const string PostMethod = "POST";
        protected const string GetMethod = "GET";
        protected const string HeadMethod = "HEAD";

        ///<summary>
        ///HttpWebRequestオブジェクトを取得する。パラメータはGET/HEAD/DELETEではクエリに、POST/PUTではエンティティボディに変換される。
        ///</summary>
        ///<remarks>
        ///追加で必要となるHTTPヘッダや通信オプションは呼び出し元で付加すること
        ///（Timeout,AutomaticDecompression,AllowAutoRedirect,UserAgent,ContentType,Accept,HttpRequestHeader.Authorization,カスタムヘッダ）
        ///POST/PUTでクエリが必要な場合は、requestUriに含めること。
        ///</remarks>
        ///<param name="method">HTTP通信メソッド（GET/HEAD/POST/PUT/DELETE）</param>
        ///<param name="requestUri">通信先URI</param>
        ///<param name="param">GET時のクエリ、またはPOST時のエンティティボディ</param>
        ///<param name="gzip">Accept-Encodingヘッダにgzipを付加するかどうかを表す真偽値</param>
        ///<returns>引数で指定された内容を反映したHttpWebRequestオブジェクト</returns>
        protected HttpWebRequest CreateRequest(string method,
                                               Uri requestUri,
                                               Dictionary<string, string> param,
                                               bool gzip = false)
        {
            Networking.CheckInitialized();

            //GETメソッドの場合はクエリとurlを結合
            UriBuilder ub = new UriBuilder(requestUri.AbsoluteUri);
            if (param != null && (method == "GET" || method == "DELETE" || method == "HEAD"))
            {
                ub.Query = MyCommon.BuildQueryString(param);
            }

            HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(ub.Uri);

            webReq.ReadWriteTimeout = 90 * 1000; //Streamの読み込みは90秒でタイムアウト（デフォルト5分）

            //プロキシ設定
            if (Networking.ProxyType != ProxyType.IE) webReq.Proxy = Networking.Proxy;

            if (gzip)
            {
                // Accept-Encodingヘッダを付加
                webReq.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            }

            webReq.Method = method;
            if (method == "POST" || method == "PUT")
            {
                webReq.ContentType = "application/x-www-form-urlencoded";
                //POST/PUTメソッドの場合は、ボディデータとしてクエリ構成して書き込み
                using (StreamWriter writer = new StreamWriter(webReq.GetRequestStream()))
                {
                    writer.Write(MyCommon.BuildQueryString(param));
                }
            }
            //cookie設定
            if (this.UseCookie) webReq.CookieContainer = this.cookieContainer;
            //タイムアウト設定
            webReq.Timeout = this.InstanceTimeout ?? (int)Networking.DefaultTimeout.TotalMilliseconds;

            webReq.UserAgent = Networking.GetUserAgentString();

            // KeepAlive無効なサーバー(Twitter等)に使用すると、タイムアウト後にWebExceptionが発生する場合あり
            webReq.KeepAlive = false;

            if (!this.CacheEnabled)
                webReq.CachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);

            return webReq;
        }

        ///<summary>
        ///HttpWebRequestオブジェクトを取得する。multipartでのバイナリアップロード用。
        ///</summary>
        ///<remarks>
        ///methodにはPOST/PUTのみ指定可能
        ///</remarks>
        ///<param name="method">HTTP通信メソッド（POST/PUT）</param>
        ///<param name="requestUri">通信先URI</param>
        ///<param name="param">form-dataで指定する名前と文字列のディクショナリ</param>
        ///<param name="binaryFileInfo">form-dataで指定する名前とバイナリファイル情報のリスト</param>
        ///<returns>引数で指定された内容を反映したHttpWebRequestオブジェクト</returns>
        protected HttpWebRequest CreateRequest(string method,
                                               Uri requestUri,
                                               Dictionary<string, string> param,
                                               List<KeyValuePair<String, IMediaItem>> binaryFileInfo)
        {
            Networking.CheckInitialized();

            //methodはPOST,PUTのみ許可
            UriBuilder ub = new UriBuilder(requestUri.AbsoluteUri);
            if (method == "GET" || method == "DELETE" || method == "HEAD")
                throw new ArgumentException("Method must be POST or PUT", nameof(method));
            if ((param == null || param.Count == 0) && (binaryFileInfo == null || binaryFileInfo.Count == 0))
                throw new ArgumentException("Data is empty");

            HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(ub.Uri);

            //プロキシ設定
            if (Networking.ProxyType != ProxyType.IE) webReq.Proxy = Networking.Proxy;

            webReq.Method = method;
            if (method == "POST" || method == "PUT")
            {
                string boundary = System.Environment.TickCount.ToString();
                webReq.ContentType = "multipart/form-data; boundary=" + boundary;
                using (Stream reqStream = webReq.GetRequestStream())
                {
                    //POST送信する文字データを作成
                    if (param != null)
                    {
                        string postData = "";
                        foreach (KeyValuePair<string, string> kvp in param)
                        {
                            postData += "--" + boundary + "\r\n" +
                                    "Content-Disposition: form-data; name=\"" + kvp.Key + "\"" +
                                    "\r\n\r\n" + kvp.Value + "\r\n";
                        }
                        byte[] postBytes = Encoding.UTF8.GetBytes(postData);
                        reqStream.Write(postBytes, 0, postBytes.Length);
                    }
                    //POST送信するバイナリデータを作成
                    if (binaryFileInfo != null)
                    {
                        foreach (KeyValuePair<string, IMediaItem> kvp in binaryFileInfo)
                        {
                            string postData = "";
                            byte[] crlfByte = Encoding.UTF8.GetBytes("\r\n");
                            //コンテンツタイプの指定
                            string mime = "";
                            switch (kvp.Value.Extension.ToLowerInvariant())
                            {
                                case ".jpg":
                                case ".jpeg":
                                case ".jpe":
                                    mime = "image/jpeg";
                                    break;
                                case ".gif":
                                    mime = "image/gif";
                                    break;
                                case ".png":
                                    mime = "image/png";
                                    break;
                                case ".tiff":
                                case ".tif":
                                    mime = "image/tiff";
                                    break;
                                case ".bmp":
                                    mime = "image/x-bmp";
                                    break;
                                case ".avi":
                                    mime = "video/avi";
                                    break;
                                case ".wmv":
                                    mime = "video/x-ms-wmv";
                                    break;
                                case ".flv":
                                    mime = "video/x-flv";
                                    break;
                                case ".m4v":
                                    mime = "video/x-m4v";
                                    break;
                                case ".mov":
                                    mime = "video/quicktime";
                                    break;
                                case ".mp4":
                                    mime = "video/3gpp";
                                    break;
                                case ".rm":
                                    mime = "application/vnd.rn-realmedia";
                                    break;
                                case ".mpeg":
                                case ".mpg":
                                    mime = "video/mpeg";
                                    break;
                                case ".3gp":
                                    mime = "movie/3gp";
                                    break;
                                case ".3g2":
                                    mime = "video/3gpp2";
                                    break;
                                default:
                                    mime = "application/octet-stream\r\nContent-Transfer-Encoding: binary";
                                    break;
                            }
                            postData = "--" + boundary + "\r\n" +
                                "Content-Disposition: form-data; name=\"" + kvp.Key + "\"; filename=\"" +
                                kvp.Value.Name + "\"\r\n" +
                                "Content-Type: " + mime + "\r\n\r\n";
                            byte[] postBytes = Encoding.UTF8.GetBytes(postData);
                            reqStream.Write(postBytes, 0, postBytes.Length);
                            //ファイルを読み出してHTTPのストリームに書き込み
                            kvp.Value.CopyTo(reqStream);
                            reqStream.Write(crlfByte, 0, crlfByte.Length);
                        }
                    }
                    //終端
                    byte[] endBytes = Encoding.UTF8.GetBytes("--" + boundary + "--\r\n");
                    reqStream.Write(endBytes, 0, endBytes.Length);
                }
            }
            //cookie設定
            if (this.UseCookie) webReq.CookieContainer = this.cookieContainer;
            //タイムアウト設定
            webReq.Timeout = this.InstanceTimeout ?? (int)Networking.DefaultTimeout.TotalMilliseconds;

            // KeepAlive無効なサーバー(Twitter等)に使用すると、タイムアウト後にWebExceptionが発生する場合あり
            webReq.KeepAlive = false;

            return webReq;
        }

        ///<summary>
        ///HTTPの応答を処理し、引数で指定されたストリームに書き込み
        ///</summary>
        ///<remarks>
        ///リダイレクト応答の場合（AllowAutoRedirect=Falseの場合のみ）は、headerInfoインスタンスがあればLocationを追加してリダイレクト先を返却
        ///WebExceptionはハンドルしていないので、呼び出し元でキャッチすること
        ///gzipファイルのダウンロードを想定しているため、他形式の場合は伸張時に問題が発生する可能性があります。
        ///</remarks>
        ///<param name="webRequest">HTTP通信リクエストオブジェクト</param>
        ///<param name="contentStream">[OUT]HTTP応答のボディストリームのコピー先</param>
        ///<param name="headerInfo">[IN/OUT]HTTP応答のヘッダ情報。ヘッダ名をキーにして空データのコレクションを渡すことで、該当ヘッダの値をデータに設定して戻す</param>
        ///<returns>HTTP応答のステータスコード</returns>
        protected HttpStatusCode GetResponse(HttpWebRequest webRequest,
                                             Stream contentStream,
                                             Dictionary<string, string> headerInfo)
        {
            try
            {
                using (HttpWebResponse webRes = (HttpWebResponse)webRequest.GetResponse())
                {
                    HttpStatusCode statusCode = webRes.StatusCode;
                    //cookie保持
                    if (this.UseCookie) this.FixCookies(webRes.Cookies);
                    //リダイレクト応答の場合は、リダイレクト先を設定
                    GetHeaderInfo(webRes, headerInfo);
                    //応答のストリームをコピーして戻す
                    if (webRes.ContentLength > 0)
                    {
                        //gzipなら応答ストリームの内容は伸張済み。それ以外なら伸張する。
                        if (webRes.ContentEncoding == "gzip" || webRes.ContentEncoding == "deflate")
                        {
                            using (Stream stream = webRes.GetResponseStream())
                            {
                                stream?.CopyTo(contentStream);
                            }
                        }
                        else
                        {
                            using (Stream stream = new GZipStream(webRes.GetResponseStream(), CompressionMode.Decompress))
                            {
                                stream?.CopyTo(contentStream);
                            }
                        }
                    }
                    return statusCode;
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse res = (HttpWebResponse)ex.Response;
                    GetHeaderInfo(res, headerInfo);
                    return res.StatusCode;
                }
                throw;
            }
        }

        ///<summary>
        ///HTTPの応答を処理し、応答ボディデータをテキストとして返却する
        ///</summary>
        ///<remarks>
        ///リダイレクト応答の場合（AllowAutoRedirect=Falseの場合のみ）は、headerInfoインスタンスがあればLocationを追加してリダイレクト先を返却
        ///WebExceptionはハンドルしていないので、呼び出し元でキャッチすること
        ///テキストの文字コードはUTF-8を前提として、エンコードはしていません
        ///</remarks>
        ///<param name="webRequest">HTTP通信リクエストオブジェクト</param>
        ///<param name="contentText">[OUT]HTTP応答のボディデータ</param>
        ///<param name="headerInfo">[IN/OUT]HTTP応答のヘッダ情報。ヘッダ名をキーにして空データのコレクションを渡すことで、該当ヘッダの値をデータに設定して戻す</param>
        ///<returns>HTTP応答のステータスコード</returns>
        protected HttpStatusCode GetResponse(HttpWebRequest webRequest,
                                             out string contentText,
                                             Dictionary<string, string> headerInfo)
        {
            try
            {
                using (HttpWebResponse webRes = (HttpWebResponse)webRequest.GetResponse())
                {
                    HttpStatusCode statusCode = webRes.StatusCode;
                    //cookie保持
                    if (this.UseCookie) this.FixCookies(webRes.Cookies);
                    //リダイレクト応答の場合は、リダイレクト先を設定
                    GetHeaderInfo(webRes, headerInfo);
                    //応答のストリームをテキストに書き出し
                    using (StreamReader sr = new StreamReader(webRes.GetResponseStream()))
                    {
                        contentText = sr.ReadToEnd();
                    }
                    return statusCode;
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse res = (HttpWebResponse)ex.Response;
                    GetHeaderInfo(res, headerInfo);
                    using (StreamReader sr = new StreamReader(res.GetResponseStream()))
                    {
                        contentText = sr.ReadToEnd();
                    }
                    return res.StatusCode;
                }
                throw;
            }
        }

        ///<summary>
        ///HTTPの応答を処理します。応答ボディデータが不要な用途向け。
        ///</summary>
        ///<remarks>
        ///リダイレクト応答の場合（AllowAutoRedirect=Falseの場合のみ）は、headerInfoインスタンスがあればLocationを追加してリダイレクト先を返却
        ///WebExceptionはハンドルしていないので、呼び出し元でキャッチすること
        ///</remarks>
        ///<param name="webRequest">HTTP通信リクエストオブジェクト</param>
        ///<param name="headerInfo">[IN/OUT]HTTP応答のヘッダ情報。ヘッダ名をキーにして空データのコレクションを渡すことで、該当ヘッダの値をデータに設定して戻す</param>
        ///<returns>HTTP応答のステータスコード</returns>
        protected HttpStatusCode GetResponse(HttpWebRequest webRequest,
                                             Dictionary<string, string> headerInfo)
        {
            try
            {
                using (HttpWebResponse webRes = (HttpWebResponse)webRequest.GetResponse())
                {
                    HttpStatusCode statusCode = webRes.StatusCode;
                    //cookie保持
                    if (this.UseCookie) this.FixCookies(webRes.Cookies);
                    //リダイレクト応答の場合は、リダイレクト先を設定
                    GetHeaderInfo(webRes, headerInfo);
                    return statusCode;
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse res = (HttpWebResponse)ex.Response;
                    GetHeaderInfo(res, headerInfo);
                    return res.StatusCode;
                }
                throw;
            }
        }

        ///<summary>
        ///HTTPの応答を処理し、応答ボディデータをBitmapとして返却します
        ///</summary>
        ///<remarks>
        ///リダイレクト応答の場合（AllowAutoRedirect=Falseの場合のみ）は、headerInfoインスタンスがあればLocationを追加してリダイレクト先を返却
        ///WebExceptionはハンドルしていないので、呼び出し元でキャッチすること
        ///</remarks>
        ///<param name="webRequest">HTTP通信リクエストオブジェクト</param>
        ///<param name="contentBitmap">[OUT]HTTP応答のボディデータを書き込むBitmap</param>
        ///<param name="headerInfo">[IN/OUT]HTTP応答のヘッダ情報。ヘッダ名をキーにして空データのコレクションを渡すことで、該当ヘッダの値をデータに設定して戻す</param>
        ///<returns>HTTP応答のステータスコード</returns>
        protected HttpStatusCode GetResponse(HttpWebRequest webRequest,
                                             out Bitmap contentBitmap,
                                             Dictionary<string, string> headerInfo)
        {
            try
            {
                using (HttpWebResponse webRes = (HttpWebResponse)webRequest.GetResponse())
                {
                    HttpStatusCode statusCode = webRes.StatusCode;
                    //cookie保持
                    if (this.UseCookie) this.FixCookies(webRes.Cookies);
                    //リダイレクト応答の場合は、リダイレクト先を設定
                    GetHeaderInfo(webRes, headerInfo);
                    //応答のストリームをBitmapにして戻す
                    //if (webRes.ContentLength > 0) contentBitmap = new Bitmap(webRes.GetResponseStream());
                    contentBitmap = new Bitmap(webRes.GetResponseStream());
                    return statusCode;
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse res = (HttpWebResponse)ex.Response;
                    GetHeaderInfo(res, headerInfo);
                    contentBitmap = null;
                    return res.StatusCode;
                }
                throw;
            }
        }

        /// <summary>
        /// ホスト名なしのドメインはドメイン名から先頭のドットを除去しないと再利用されないため修正して追加する
        /// </summary>
        private void FixCookies(CookieCollection cookieCollection)
        {
            foreach (Cookie ck in cookieCollection)
            {
                if (ck.Domain.StartsWith(".", StringComparison.Ordinal))
                {
                    ck.Domain = ck.Domain.Substring(1);
                    cookieContainer.Add(ck);
                }
            }
        }

        ///<summary>
        ///headerInfoのキー情報で指定されたHTTPヘッダ情報を取得・格納する。redirect応答時はLocationヘッダの内容を追記する
        ///</summary>
        ///<param name="webResponse">HTTP応答</param>
        ///<param name="headerInfo">[IN/OUT]キーにヘッダ名を指定したデータ空のコレクション。取得した値をデータにセットして戻す</param>
        private void GetHeaderInfo(HttpWebResponse webResponse,
                                   Dictionary<string, string> headerInfo)
        {
            if (headerInfo == null) return;

            if (headerInfo.Count > 0)
            {
                var headers = webResponse.Headers;
                var dictKeys = new string[headerInfo.Count];
                headerInfo.Keys.CopyTo(dictKeys, 0);

                foreach (var key in dictKeys)
                {
                    var value = headers[key];
                    headerInfo[key] = value ?? "";
                }
            }

            HttpStatusCode statusCode = webResponse.StatusCode;
            if (statusCode == HttpStatusCode.MovedPermanently ||
                statusCode == HttpStatusCode.Found ||
                statusCode == HttpStatusCode.SeeOther ||
                statusCode == HttpStatusCode.TemporaryRedirect)
            {
                if (webResponse.Headers["Location"] != null)
                {
                    headerInfo["Location"] = webResponse.Headers["Location"];
                }
            }
        }

        ///<summary>
        ///クエリ形式（key1=value1&amp;key2=value2&amp;...）の文字列をkey-valueコレクションに詰め直し
        ///</summary>
        ///<param name="queryString">クエリ文字列</param>
        ///<returns>key-valueのコレクション</returns>
        protected NameValueCollection ParseQueryString(string queryString)
        {
            NameValueCollection query = new NameValueCollection();
            string[] parts = queryString.Split('&');
            foreach (string part in parts)
            {
                int index = part.IndexOf('=');
                if (index == -1)
                    query.Add(Uri.UnescapeDataString(part), "");
                else
                    query.Add(Uri.UnescapeDataString(part.Substring(0, index)), Uri.UnescapeDataString(part.Substring(index + 1)));
            }
            return query;
        }

        #region "InstanceTimeout"
        ///<summary>
        ///通信タイムアウト時間（ms）
        ///</summary>
        private int? _timeout = null;

        ///<summary>
        ///通信タイムアウト時間（ms）。10～120秒の範囲で指定。範囲外は20秒とする
        ///</summary>
        protected int? InstanceTimeout
        {
            get { return _timeout; }
            set
            {
                const int TimeoutMinValue = 10000;
                const int TimeoutMaxValue = 120000;
                if (value < TimeoutMinValue || value > TimeoutMaxValue)
                    throw new ArgumentOutOfRangeException(nameof(value), "Set " + TimeoutMinValue + "-" + TimeoutMaxValue + ": Value=" + value);
                else
                    _timeout = value;
            }
        }
        #endregion
    }
}