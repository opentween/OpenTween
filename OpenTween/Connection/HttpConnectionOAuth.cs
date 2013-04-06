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
		/// OAuth署名のoauth_timestamp算出用基準日付（1970/1/1 00:00:00）
		/// </summary>
		private static readonly DateTime UnixEpoch = new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Unspecified );

		/// <summary>
		/// OAuth署名のoauth_nonce算出用乱数クラス
		/// </summary>
		private static readonly Random NonceRandom = new Random();

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
		private string consumerKey;

		/// <summary>
		/// OAuthの署名作成用秘密コンシューマーデータ
		/// </summary>
		protected string consumerSecret;

		/// <summary>
		/// 認証成功時の応答でユーザー情報を取得する場合のキー。設定しない場合は、AuthUsernameもブランクのままとなる
		/// </summary>
		private string userIdentKey = "";

		/// <summary>
		/// 認証成功時の応答でユーザーID情報を取得する場合のキー。設定しない場合は、AuthUserIdもブランクのままとなる
		/// </summary>
		private string userIdIdentKey = "";

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
		/// <param name="callback">処理終了直前に呼ばれるコールバック関数のデリゲート 不要な場合はNothingを渡すこと</param>
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

			HttpWebRequest webReq = this.CreateRequest( method, requestUri, param, false );
			// OAuth認証ヘッダを付加
			this.AppendOAuthInfo( webReq, param, token, tokenSecret );

			HttpStatusCode code;
			if ( content == null )
				code = this.GetResponse( webReq, headerInfo, false );
			else
				code = this.GetResponse( webReq, out content, headerInfo, false );

			if ( callback != null )
			{
				StackFrame frame = new StackFrame( 1 );
				callback( frame.GetMethod().Name, ref code, ref content );
			}
			return code;
		}

		/// <summary>
		/// バイナリアップロード
		/// </summary>
		public HttpStatusCode GetContent( string method,
		                                  Uri requestUri,
		                                  Dictionary< string, string > param,
		                                  List< KeyValuePair< string, FileInfo > > binary, 
		                                  ref string content,
		                                  Dictionary< string, string > headerInfo,
		                                  CallbackDelegate callback )
		{
			// 認証済かチェック
			if ( string.IsNullOrEmpty( token ) )
				return HttpStatusCode.Unauthorized;

			HttpWebRequest webReq = this.CreateRequest( method, requestUri, param, binary, false );
			// OAuth認証ヘッダを付加
			this.AppendOAuthInfo( webReq, null, token, tokenSecret );

			HttpStatusCode code;
			if ( content == null )
				code = this.GetResponse( webReq, headerInfo, false );
			else
				code = this.GetResponse( webReq, out content, headerInfo, false );

			if ( callback != null )
			{
				StackFrame frame = new StackFrame( 1 );
				callback( frame.GetMethod().Name, ref code, ref content );
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
			this.streamReq = this.CreateRequest( method, requestUri, param, false );
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

		#region "認証処理"
		/// <summary>
		/// OAuth認証の開始要求（リクエストトークン取得）。PIN入力用の前段
		/// </summary>
		/// <remarks>
		/// 呼び出し元では戻されたurlをブラウザで開き、認証完了後PIN入力を受け付けて、リクエストトークンと共にAuthenticatePinFlowを呼び出す
		/// </remarks>
		/// <param name="requestTokenUrl">リクエストトークンの取得先URL</param>
		/// <param name="requestUri">ブラウザで開く認証用URLのベース</param>
		/// <param name="requestToken">[OUT]認証要求で戻されるリクエストトークン。使い捨て</param>
		/// <param name="authUri">[OUT]requestUriを元に生成された認証用URL。通常はリクエストトークンをクエリとして付加したUri</param>
		/// <returns>取得結果真偽値</returns>
		public bool AuthenticatePinFlowRequest( string requestTokenUrl, string authorizeUrl, ref string requestToken, ref Uri authUri )
		{
			// PIN-based flow
            authUri = this.GetAuthenticatePageUri( requestTokenUrl, authorizeUrl, ref requestToken );
			if ( authUri == null )
				return false;
			return true;
		}

		/// <summary>
		/// OAuth認証のアクセストークン取得。PIN入力用の後段
		/// </summary>
		/// <remarks>
		/// 事前にAuthenticatePinFlowRequestを呼んで、ブラウザで認証後に表示されるPINを入力してもらい、その値とともに呼び出すこと
		/// </remarks>
		/// <param name="accessTokenUrl">アクセストークンの取得先URL</param>
		/// <param name="requestUri">AuthenticatePinFlowRequestで取得したリクエストトークン</param>
		/// <param name="pinCode">Webで認証後に表示されるPINコード</param>
		/// <returns>取得結果真偽値</returns>
		public HttpStatusCode AuthenticatePinFlow( string accessTokenUrl, string requestToken, string pinCode )
		{
			// PIN-based flow
			if ( string.IsNullOrEmpty( requestToken ) )
				throw new Exception( "Sequence error.(requestToken is blank)" );

			// アクセストークン取得
			string content = "";
			NameValueCollection accessTokenData;
			HttpStatusCode httpCode = this.GetOAuthToken( new Uri( accessTokenUrl ), pinCode, requestToken, null, ref content );
			if ( httpCode != HttpStatusCode.OK )
				return httpCode;
			accessTokenData = base.ParseQueryString( content );

			if ( accessTokenData != null )
			{
				this.token = accessTokenData[ "oauth_token" ];
				this.tokenSecret = accessTokenData[ "oauth_token_secret" ];

				// サービスごとの独自拡張対応
				if ( !string.IsNullOrEmpty(this.userIdentKey) )
					this.authorizedUsername = accessTokenData[ this.userIdentKey ];
				else
					this.authorizedUsername = "";

				if ( !string.IsNullOrEmpty(this.userIdIdentKey) )
				{
					try
					{
						this.authorizedUserId = Convert.ToInt64( accessTokenData[ this.userIdIdentKey ] );
					}
					catch ( Exception )
					{
						this.authorizedUserId = 0;
					}
				}
				else
				{
					this.authorizedUserId = 0;
				}

				if ( string.IsNullOrEmpty(token) )
					throw new InvalidDataException( "Token is null." );
				return HttpStatusCode.OK;
			}
			else
			{
                throw new InvalidDataException( "Return value is null." );
			}
		}

        public HttpStatusCode Authenticate(Uri accessTokenUrl, string username, string password, ref string content)
        {
            return this.AuthenticateXAuth(accessTokenUrl, username, password, ref content);
        }

		/// <summary>
		/// OAuth認証のアクセストークン取得。xAuth方式
		/// </summary>
		/// <param name="accessTokenUrl">アクセストークンの取得先URL</param>
		/// <param name="username">認証用ユーザー名</param>
		/// <param name="password">認証用パスワード</param>
		/// <returns>取得結果真偽値</returns>
		public HttpStatusCode AuthenticateXAuth( Uri accessTokenUrl, string username, string password, ref string content )
		{
			// ユーザー・パスワードチェック
			if ( string.IsNullOrEmpty( username ) || string.IsNullOrEmpty( password ) )
				throw new Exception( "Sequence error.(username or password is blank)" );

			// xAuthの拡張パラメータ設定
			Dictionary< string, string > parameter = new Dictionary< string, string >();
			parameter.Add( "x_auth_mode", "client_auth" );
			parameter.Add( "x_auth_username", username );
			parameter.Add( "x_auth_password", password );

			// アクセストークン取得
			HttpStatusCode httpCode = this.GetOAuthToken( accessTokenUrl, "", "", parameter, ref content );
			if ( httpCode != HttpStatusCode.OK )
				return httpCode;
			NameValueCollection accessTokenData = base.ParseQueryString( content );

			if ( accessTokenData != null )
			{
				this.token = accessTokenData[ "oauth_token" ];
				this.tokenSecret = accessTokenData[ "oauth_token_secret" ];

				// サービスごとの独自拡張対応
				if ( !string.IsNullOrEmpty(this.userIdentKey) )
					this.authorizedUsername = accessTokenData[ this.userIdentKey ];
				else
					this.authorizedUsername = "";

				if ( !string.IsNullOrEmpty(this.userIdIdentKey) )
				{
					try
					{
                        this.authorizedUserId = Convert.ToInt64( accessTokenData[ this.userIdIdentKey ] );
					}
					catch ( Exception )
					{
						this.authorizedUserId = 0;
					}
				}
				else
				{
					this.authorizedUserId = 0;
				}

				if ( string.IsNullOrEmpty(token) )
					throw new InvalidDataException( "Token is null." );
				return HttpStatusCode.OK;
			}
			else
			{
				throw new InvalidDataException( "Return value is null." );
			}
		}

		/// <summary>
		/// OAuth認証のリクエストトークン取得。リクエストトークンと組み合わせた認証用のUriも生成する
		/// </summary>
		/// <param name="accessTokenUrl">リクエストトークンの取得先URL</param>
		/// <param name="authorizeUrl">ブラウザで開く認証用URLのベース</param>
		/// <param name="requestToken">[OUT]取得したリクエストトークン</param>
		/// <returns>取得結果真偽値</returns>
		private Uri GetAuthenticatePageUri( string requestTokenUrl, string authorizeUrl, ref string requestToken )
		{
			const string tokenKey = "oauth_token";

			// リクエストトークン取得
			string content = "";
			NameValueCollection reqTokenData;
			if ( this.GetOAuthToken( new Uri( requestTokenUrl ), "", "", null, ref content, callbackUrl: "oob" ) != HttpStatusCode.OK )
				return null;
			reqTokenData = base.ParseQueryString( content );

			if ( reqTokenData != null )
			{
				requestToken = reqTokenData[ tokenKey ];
				// Uri生成
				UriBuilder ub = new UriBuilder( authorizeUrl );
				ub.Query = string.Format( "{0}={1}", tokenKey, requestToken );
				return ub.Uri;
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// OAuth認証のトークン取得共通処理
		/// </summary>
		/// <param name="requestUri">各種トークンの取得先URL</param>
		/// <param name="pinCode">PINフロー時のアクセストークン取得時に設定。それ以外は空文字列</param>
		/// <param name="requestToken">PINフロー時のリクエストトークン取得時に設定。それ以外は空文字列</param>
		/// <param name="parameter">追加パラメータ。xAuthで使用</param>
		/// <returns>取得結果のデータ。正しく取得出来なかった場合はNothing</returns>
		private HttpStatusCode GetOAuthToken( Uri requestUri, string pinCode, string requestToken, Dictionary< string , string > parameter, ref string content, string callbackUrl = null )
		{
			HttpWebRequest webReq = null;
			// HTTPリクエスト生成。PINコードもパラメータも未指定の場合はGETメソッドで通信。それ以外はPOST
			if ( string.IsNullOrEmpty( pinCode ) && parameter != null )
				webReq = this.CreateRequest( "GET", requestUri, null, false );
			else
				webReq = this.CreateRequest( "POST", requestUri, parameter, false ); // ボディに追加パラメータ書き込み

			// OAuth関連パラメータ準備。追加パラメータがあれば追加
			Dictionary< string, string > query = new Dictionary< string, string >();
			if ( parameter != null )
				foreach ( KeyValuePair< string, string > kvp in parameter )
					query.Add( kvp.Key, kvp.Value );

			// PINコードが指定されていればパラメータに追加
			if ( ! string.IsNullOrEmpty( pinCode ) )
				query.Add( "oauth_verifier", pinCode );

			// コールバックURLが指定されていればパラメータに追加
			if (!string.IsNullOrEmpty(callbackUrl))
				query.Add("oauth_callback", callbackUrl);

			// OAuth関連情報をHTTPリクエストに追加
			this.AppendOAuthInfo( webReq, query, requestToken, "" );

			// HTTP応答取得
			Dictionary< string, string > header = new Dictionary< string, string >() { { "Date", "" } };
			HttpStatusCode responseCode = this.GetResponse( webReq, out content, header, false );
			if ( responseCode == HttpStatusCode.OK )
				return responseCode;

			if ( !string.IsNullOrEmpty( header[ "Date" ] ) )
				content += Environment.NewLine + "Check the Date & Time of this computer." + Environment.NewLine
				+ "Server:" + DateTime.Parse( header[ "Date" ] ).ToString() + "  PC:" + DateTime.Now.ToString();
			return responseCode;
		}
		#endregion // 認証処理

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
			// OAuth共通情報取得
			Dictionary< string, string > parameter = this.GetOAuthParameter( token );
			// OAuth共通情報にquery情報を追加
			if ( query != null )
				foreach ( KeyValuePair< string, string > item in query )
					parameter.Add( item.Key, item.Value );
			// 署名の作成・追加
			parameter.Add( "oauth_signature", this.CreateSignature( tokenSecret, webRequest.Method, webRequest.RequestUri, parameter ) );
			// HTTPリクエストのヘッダに追加
			StringBuilder sb = new StringBuilder( "OAuth " );
			foreach ( KeyValuePair< string, string > item in parameter )
				// 各種情報のうち、oauth_で始まる情報のみ、ヘッダに追加する。各情報はカンマ区切り、データはダブルクォーテーションで括る
				if ( item.Key.StartsWith("oauth_") )
					sb.AppendFormat( "{0}=\"{1}\",", item.Key, this.UrlEncode( item.Value ) );
			webRequest.Headers.Add( HttpRequestHeader.Authorization, sb.ToString() );
		}

		/// <summary>
		/// OAuthで使用する共通情報を取得する
		/// </summary>
		/// <param name="token">アクセストークン、もしくはリクエストトークン。未取得なら空文字列</param>
		/// <returns>OAuth情報のディクショナリ</returns>
		protected Dictionary< string, string > GetOAuthParameter( string token )
		{
			Dictionary< string, string > parameter = new Dictionary< string, string >();
			parameter.Add( "oauth_consumer_key", this.consumerKey );
			parameter.Add( "oauth_signature_method", "HMAC-SHA1" );
			parameter.Add( "oauth_timestamp", Convert.ToInt64( ( DateTime.UtcNow - HttpConnectionOAuth.UnixEpoch ).TotalSeconds ).ToString() ); // epoch秒
			parameter.Add( "oauth_nonce", HttpConnectionOAuth.NonceRandom.Next( 123400, 9999999 ).ToString() );
			parameter.Add( "oauth_version", "1.0" );
			if ( !string.IsNullOrEmpty( token ) )
				parameter.Add( "oauth_token", token ); // トークンがあれば追加
			return parameter;
		}

		/// <summary>
		/// OAuth認証ヘッダの署名作成
		/// </summary>
		/// <param name="tokenSecret">アクセストークン秘密鍵</param>
		/// <param name="method">HTTPメソッド文字列</param>
		/// <param name="uri">アクセス先Uri</param>
		/// <param name="parameter">クエリ、もしくはPOSTデータ</param>
		/// <returns>署名文字列</returns>
		protected virtual string CreateSignature( string tokenSecret, string method, Uri uri, Dictionary< string, string > parameter )
		{
			// パラメタをソート済みディクショナリに詰替（OAuthの仕様）
			SortedDictionary< string, string > sorted = new SortedDictionary< string, string >( parameter );
			// URLエンコード済みのクエリ形式文字列に変換
			string paramString = this.CreateQueryString( sorted );
			// アクセス先URLの整形
			string url = string.Format( "{0}://{1}{2}", uri.Scheme, uri.Host, uri.AbsolutePath );
			// 署名のベース文字列生成（&区切り）。クエリ形式文字列は再エンコードする
			string signatureBase = string.Format( "{0}&{1}&{2}", method, this.UrlEncode( url ), this.UrlEncode( paramString ) );
			// 署名鍵の文字列をコンシューマー秘密鍵とアクセストークン秘密鍵から生成（&区切り。アクセストークン秘密鍵なくても&残すこと）
			string key = this.UrlEncode( this.consumerSecret ) + "&";
			if ( !string.IsNullOrEmpty( tokenSecret ) )
				key += this.UrlEncode( tokenSecret );
			// 鍵生成＆署名生成
			using ( HMACSHA1 hmac = new HMACSHA1( Encoding.ASCII.GetBytes( key ) ) )
			{
				byte[] hash = hmac.ComputeHash( Encoding.ASCII.GetBytes( signatureBase ) );
				return Convert.ToBase64String( hash );
			}
		}
		#endregion // OAuth認証用ヘッダ作成・付加処理

		/// <summary>
		/// 初期化。各種トークンの設定とユーザー識別情報設定
		/// </summary>
		/// <param name="consumerKey">コンシューマー鍵</param>
		/// <param name="consumerSecret">コンシューマー秘密鍵</param>
		/// <param name="accessToken">アクセストークン</param>
		/// <param name="accessTokenSecret">アクセストークン秘密鍵</param>
		/// <param name="userIdentifier">アクセストークン取得時に得られるユーザー識別情報。不要なら空文字列</param>
		public void Initialize( string consumerKey, string consumerSecret,
		                        string accessToken, string accessTokenSecret,
		                        string userIdentifier, string userIdIdentifier )
		{
			this.consumerKey = consumerKey;
			this.consumerSecret = consumerSecret;
			this.token = accessToken;
			this.tokenSecret = accessTokenSecret;
			this.userIdentKey = userIdentifier;
			this.userIdIdentKey = userIdIdentifier;
		}

		/// <summary>
		/// 初期化。各種トークンの設定とユーザー識別情報設定
		/// </summary>
		/// <param name="consumerKey">コンシューマー鍵</param>
		/// <param name="consumerSecret">コンシューマー秘密鍵</param>
		/// <param name="accessToken">アクセストークン</param>
		/// <param name="accessTokenSecret">アクセストークン秘密鍵</param>
		/// <param name="username">認証済みユーザー名</param>
		/// <param name="userIdentifier">アクセストークン取得時に得られるユーザー識別情報。不要なら空文字列</param>
		public void Initialize( string consumerKey, string consumerSecret,
		                        string accessToken, string accessTokenSecret,
		                        string username, long userId,
		                        string userIdentifier, string userIdIdentifier )
		{
			this.Initialize( consumerKey, consumerSecret, accessToken, accessTokenSecret, userIdentifier, userIdIdentifier );
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
