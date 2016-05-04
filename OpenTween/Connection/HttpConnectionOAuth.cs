// OpenTween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2011      spinor (@tplantd) <http://d.hatena.ne.jp/spinor/>
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

using HttpConnection = OpenTween.HttpConnection;
using IHttpConnection = OpenTween.IHttpConnection;
using OAuthUtility = OpenTween.Connection.OAuthUtility;
using DateTime = System.DateTime;
using DateTimeKind = System.DateTimeKind;
using Random = System.Random;
using HttpWebRequest = System.Net.HttpWebRequest;
using HttpStatusCode = System.Net.HttpStatusCode;
using Uri = System.Uri;
using System.Collections.Generic; // for Dictionary<TKey, TValue>, List<T>, KeyValuePair<TKey, TValue>, SortedDictionary<TKey, TValue>
using CallbackDelegate = OpenTween.CallbackDelegate;
using StackFrame = System.Diagnostics.StackFrame;
using FileInfo = System.IO.FileInfo;
using Stream = System.IO.Stream;
using HttpWebResponse = System.Net.HttpWebResponse;
using WebException = System.Net.WebException;
using WebExceptionStatus = System.Net.WebExceptionStatus;
using Exception = System.Exception;
using NameValueCollection = System.Collections.Specialized.NameValueCollection;
using Convert = System.Convert;
using InvalidDataException = System.IO.InvalidDataException;
using UriBuilder = System.UriBuilder;
using Environment = System.Environment;
using StringBuilder = System.Text.StringBuilder;
using HttpRequestHeader = System.Net.HttpRequestHeader;
using HMACSHA1 = System.Security.Cryptography.HMACSHA1;
using Encoding = System.Text.Encoding;
using System;

namespace OpenTween
{
	/// <summary>
	/// OAuth認証を使用するHTTP通信。HMAC-SHA1固定
	/// </summary>
	/// <remarks>
	/// 使用前に認証情報を設定する。認証確認を伴う場合はAuthenticate系のメソッドを、認証不要な場合はInitializeを呼ぶこと。
	/// </remarks>
	abstract public class HttpConnectionOAuth : HttpConnection, IHttpConnection
	{
		/// <summary>
		/// OAuthのアクセストークン。永続化可能（ユーザー取り消しの可能性はある）。
		/// </summary>
		private string token = "";

		/// <summary>
		/// OAuthの署名作成用秘密アクセストークン。永続化可能（ユーザー取り消しの可能性はある）。
		/// </summary>
		private string tokenSecret = "";

		/// <summary>
		/// OAuthのコンシューマー鍵
		/// </summary>
		protected string consumerKey;

		/// <summary>
		/// OAuthの署名作成用秘密コンシューマーデータ
		/// </summary>
		protected string consumerSecret;

		/// <summary>
		/// 認証完了時の応答からuserIdentKey情報に基づいて取得するユーザー情報
		/// </summary>
		private string authorizedUsername = "";

		/// <summary>
		/// 認証完了時の応答からuserIdentKey情報に基づいて取得するユーザー情報
		/// </summary>
		private long authorizedUserId;

		/// <summary>
		/// Stream用のHttpWebRequest
		/// </summary>
		private HttpWebRequest streamReq = null;

		/// <summary>
		/// OAuth認証で指定のURLとHTTP通信を行い、結果を返す
		/// </summary>
		/// <param name="method">HTTP通信メソッド（GET/HEAD/POST/PUT/DELETE）</param>
		/// <param name="requestUri">通信先URI</param>
		/// <param name="param">GET時のクエリ、またはPOST時のエンティティボディ</param>
		/// <param name="content">[OUT]HTTP応答のボディデータ</param>
		/// <param name="headerInfo">[IN/OUT]HTTP応答のヘッダ情報。必要なヘッダ名を事前に設定しておくこと</param>
		/// <param name="callback">処理終了直前に呼ばれるコールバック関数のデリゲート 不要な場合はnullを渡すこと</param>
		/// <returns>HTTP応答のステータスコード</returns>
		public HttpStatusCode GetContent( string method,
		                                  Uri requestUri,
		                                  Dictionary< string, string > param,
		                                  ref string content,
		                                  Dictionary< string, string > headerInfo,
		                                  CallbackDelegate callback )
		{
			// 認証済かチェック
			if ( string.IsNullOrEmpty( token ) )
				return HttpStatusCode.Unauthorized;

			HttpWebRequest webReq = this.CreateRequest( method, requestUri, param, gzip: true );
			// OAuth認証ヘッダを付加
			this.AppendOAuthInfo( webReq, param, token, tokenSecret );

			HttpStatusCode code;
			if ( content == null )
				code = this.GetResponse( webReq, headerInfo );
			else
				code = this.GetResponse( webReq, out content, headerInfo );

			if ( callback != null )
			{
				StackFrame frame = new StackFrame( 1 );
				callback( frame.GetMethod().Name, code, headerInfo, content );
			}
			return code;
		}

		/// <summary>
		/// バイナリアップロード
		/// </summary>
		public HttpStatusCode GetContent( string method,
		                                  Uri requestUri,
		                                  Dictionary< string, string > param,
		                                  List< KeyValuePair< string, IMediaItem > > binary, 
		                                  ref string content,
		                                  Dictionary< string, string > headerInfo,
		                                  CallbackDelegate callback )
		{
			// 認証済かチェック
			if ( string.IsNullOrEmpty( token ) )
				return HttpStatusCode.Unauthorized;

			HttpWebRequest webReq = this.CreateRequest( method, requestUri, param, binary );
			// OAuth認証ヘッダを付加
			this.AppendOAuthInfo( webReq, null, token, tokenSecret );

			HttpStatusCode code;
			if ( content == null )
				code = this.GetResponse( webReq, headerInfo );
			else
				code = this.GetResponse( webReq, out content, headerInfo );

			if ( callback != null )
			{
				StackFrame frame = new StackFrame( 1 );
				callback( frame.GetMethod().Name, code, headerInfo, content );
			}
			return code;
		}

		/// <summary>
		/// OAuth認証で指定のURLとHTTP通信を行い、ストリームを返す
		/// </summary>
		/// <param name="method">HTTP通信メソッド（GET/HEAD/POST/PUT/DELETE）</param>
		/// <param name="requestUri">通信先URI</param>
		/// <param name="param">GET時のクエリ、またはPOST時のエンティティボディ</param>
		/// <param name="content">[OUT]HTTP応答のボディストリーム</param>
		/// <returns>HTTP応答のステータスコード</returns>
		public HttpStatusCode GetContent( string method,
		                                  Uri requestUri,
		                                  Dictionary< string, string > param,
		                                  ref Stream content,
		                                  string userAgent )
		{
			// 認証済かチェック
			if ( string.IsNullOrEmpty( token ) )
				return HttpStatusCode.Unauthorized;

			this.RequestAbort();
			this.streamReq = this.CreateRequest( method, requestUri, param );
			// User-Agent指定がある場合は付加
			if ( !string.IsNullOrEmpty( userAgent ) )
				this.streamReq.UserAgent = userAgent;

			// OAuth認証ヘッダを付加
			this.AppendOAuthInfo( this.streamReq, param, token, tokenSecret );

			try
			{
				HttpWebResponse webRes = (HttpWebResponse)this.streamReq.GetResponse();
				content = webRes.GetResponseStream();
				return webRes.StatusCode;
			}
			catch ( WebException ex )
			{
				if ( ex.Status == WebExceptionStatus.ProtocolError )
				{
					HttpWebResponse res = (HttpWebResponse)ex.Response;
					return res.StatusCode;
				}
				throw;
			}
		}

		public void RequestAbort()
		{
			try
			{
				if ( this.streamReq != null )
				{
					this.streamReq.Abort();
					this.streamReq = null;
				}
			}
			catch ( Exception ) {}
		}

		#region "OAuth認証用ヘッダ作成・付加処理"
		/// <summary>
		/// HTTPリクエストにOAuth関連ヘッダを追加
		/// </summary>
		/// <param name="webRequest">追加対象のHTTPリクエスト</param>
		/// <param name="query">OAuth追加情報＋クエリ or POSTデータ</param>
		/// <param name="token">アクセストークン、もしくはリクエストトークン。未取得なら空文字列</param>
		/// <param name="tokenSecret">アクセストークンシークレット。認証処理では空文字列</param>
		protected virtual void AppendOAuthInfo( HttpWebRequest webRequest, Dictionary< string, string > query, string token, string tokenSecret )
		{
			var credential = OAuthUtility.CreateAuthorization( webRequest.Method, webRequest.RequestUri, query,
				this.consumerKey, this.consumerSecret, token, tokenSecret );

			webRequest.Headers.Add( HttpRequestHeader.Authorization, credential );
		}
		#endregion // OAuth認証用ヘッダ作成・付加処理

		/// <summary>
		/// 初期化。各種トークンの設定とユーザー識別情報設定
		/// </summary>
		/// <param name="consumerKey">コンシューマー鍵</param>
		/// <param name="consumerSecret">コンシューマー秘密鍵</param>
		/// <param name="accessToken">アクセストークン</param>
		/// <param name="accessTokenSecret">アクセストークン秘密鍵</param>
		public void Initialize( string consumerKey, string consumerSecret,
		                        string accessToken, string accessTokenSecret )
		{
			this.consumerKey = consumerKey;
			this.consumerSecret = consumerSecret;
			this.token = accessToken;
			this.tokenSecret = accessTokenSecret;
		}

		/// <summary>
		/// 初期化。各種トークンの設定とユーザー識別情報設定
		/// </summary>
		/// <param name="consumerKey">コンシューマー鍵</param>
		/// <param name="consumerSecret">コンシューマー秘密鍵</param>
		/// <param name="accessToken">アクセストークン</param>
		/// <param name="accessTokenSecret">アクセストークン秘密鍵</param>
		/// <param name="username">認証済みユーザー名</param>
		public void Initialize( string consumerKey, string consumerSecret,
		                        string accessToken, string accessTokenSecret,
		                        string username, long userId )
		{
			this.Initialize( consumerKey, consumerSecret, accessToken, accessTokenSecret );
			this.authorizedUsername = username;
			this.authorizedUserId = userId;
		}

		/// <summary>
		/// アクセストークン
		/// </summary>
		public string AccessToken
		{
			get { return this.token; }
		}

		/// <summary>
		/// アクセストークン秘密鍵
		/// </summary>
		public string AccessTokenSecret
		{
			get { return this.tokenSecret; }
		}

		/// <summary>
		/// 認証済みユーザー名
		/// </summary>
		public string AuthUsername
		{
			get { return this.authorizedUsername; }
		}

		/// <summary>
		/// 認証済みユーザーId
		/// </summary>
		public long AuthUserId
		{
			get { return this.authorizedUserId; }
			set { this.authorizedUserId = value; }
		}
	}
}
