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

using HttpConnectionOAuth = OpenTween.HttpConnectionOAuth;
using OAuthUtility = OpenTween.Connection.OAuthUtility;
using Uri = System.Uri;
using HttpWebRequest = System.Net.HttpWebRequest;
using System.Collections.Generic; // for Dictionary<TKey, TValue>, KeyValuePair<TKey, TValue>
using HttpConnection = OpenTween.HttpConnection;
using StringBuilder = System.Text.StringBuilder;

namespace OpenTween
{
	public class HttpConnectionOAuthEcho : HttpConnectionOAuth
	{
		public Uri Realm { get; set; }
		public Uri ServiceProvider { get; set; }

		protected override void AppendOAuthInfo( HttpWebRequest webRequest, Dictionary< string, string > query, string token, string tokenSecret )
		{
			var realm = this.Realm.Scheme + "://" + this.Realm.Host + this.Realm.AbsolutePath;

			var credential = OAuthUtility.CreateAuthorization( HttpConnection.GetMethod, this.ServiceProvider, query,
				this.consumerKey, this.consumerSecret, token, tokenSecret, realm );

			webRequest.Headers.Add( "X-Verify-Credentials-Authorization", credential );
			webRequest.Headers.Add( "X-Auth-Service-Provider", string.Format("{0}://{1}{2}", this.ServiceProvider.Scheme, this.ServiceProvider.Host, this.ServiceProvider.AbsolutePath));
		}

		public HttpConnectionOAuthEcho( Uri realm, Uri serviceProvider )
		{
			this.Realm = realm;
			this.ServiceProvider = serviceProvider;
		}
	}
}
