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
using HttpStatusCode = System.Net.HttpStatusCode;
using FileInfo = System.IO.FileInfo;
using ArgumentException = System.ArgumentException;
using Array = System.Array;
using Encoding = System.Text.Encoding;
using BitConverter = System.BitConverter;
using System.Collections.Generic; // for Dictionary<TKey, TValue>, List<T>, KeyValuePair<TKey, TValue>
using HttpWebRequest = System.Net.HttpWebRequest;
using Uri = System.Uri;
using UploadFileType = OpenTween.MyCommon.UploadFileType;
using MD5CryptoServiceProvider = System.Security.Cryptography.MD5CryptoServiceProvider;

namespace OpenTween
{
	public class TwitVideo : HttpConnection
	{
		private string[] multimediaExt = new string[] { ".avi", ".wmv", ".flv", ".m4v", ".mov", ".mp4", ".rm", ".mpeg", ".mpg", ".3gp", ".3g2" };

		private string[] pictureExt = new string[] { ".jpg", ".jpeg", ".gif", ".png" };

		private const long MaxPictureFileSize = 10 * 1024 * 1024;

		private const long MaxMultiMediaFileSize = 20 * 1024 * 1024;

		public HttpStatusCode Upload( FileInfo mediaFile, string message, string keyword, string username, string twitter_id, ref string content )
		{
			// Message必須
			if ( string.IsNullOrEmpty( message ) )
				throw new ArgumentException( "'Message' is required." );

			// Check filetype and size
			if ( Array.IndexOf( multimediaExt, mediaFile.Extension.ToLower() ) > -1 )
			{
                if ( mediaFile.Length > TwitVideo.MaxMultiMediaFileSize )
					throw new ArgumentException( "File is too large." );
			}
			else if ( Array.IndexOf( pictureExt, mediaFile.Extension.ToLower() ) > -1 )
			{
                if ( mediaFile.Length > TwitVideo.MaxPictureFileSize )
					throw new ArgumentException( "File is too large." );
			}
			else
			{
				throw new ArgumentException( "Service don't support this filetype." );
			}

			// Endpoint(URI+Token)
			const string URLBASE = "http://api.twitvideo.jp/oauth/upload/";
            byte[] data = Encoding.ASCII.GetBytes( ApplicationSettings.TwitVideoConsumerKey.Substring(0, 9) + username );

            byte[] bHash;
            using (MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider())
            {
    	        bHash = md5provider.ComputeHash( data );
            }

			string url = URLBASE + BitConverter.ToString( bHash ).ToLower().Replace( "-", "" );
			// Parameters
			Dictionary< string, string > param = new Dictionary< string, string >();
			param.Add( "username", username );
			if ( !string.IsNullOrEmpty( twitter_id ) )
				param.Add( "twitter_id", twitter_id );
			if ( !string.IsNullOrEmpty( keyword ) )
				param.Add( "keyword", keyword );
			param.Add( "type", "xml" );
			param.Add( "message", message );
			List< KeyValuePair< string, FileInfo > > binary = new List< KeyValuePair< string, FileInfo > >();
			binary.Add( new KeyValuePair< string, FileInfo >( "media", mediaFile ) );
			this.InstanceTimeout = 60000; // タイムアウト60秒

			HttpWebRequest req = this.CreateRequest( HttpConnection.PostMethod, new Uri( url ), param, binary, false );
			return this.GetResponse( req, out content, null, false );
		}

		public bool CheckValidExtension( string ext )
		{
			if ( Array.IndexOf( this.pictureExt, ext.ToLower() ) > -1 )
				return true;

			if ( Array.IndexOf( this.multimediaExt, ext.ToLower() ) > -1 )
				return true;

			return false;
		}

		UploadFileType GetFileType( string ext )
		{
			if ( Array.IndexOf( this.pictureExt, ext.ToLower() ) > -1 )
				return UploadFileType.Picture;

			if ( Array.IndexOf( this.multimediaExt, ext.ToLower() ) > -1 )
				return UploadFileType.MultiMedia;

			return UploadFileType.Invalid;
		}

		bool IsSupportedFileType( UploadFileType type )
		{
			return type.Equals( UploadFileType.Picture ) || type.Equals( UploadFileType.MultiMedia );
		}

		long GetMaxFileSize( string ext )
		{
			if ( Array.IndexOf( this.pictureExt, ext.ToLower() ) > -1 )
                return TwitVideo.MaxPictureFileSize;

			if ( Array.IndexOf( this.multimediaExt, ext.ToLower() ) > -1 )
                return TwitVideo.MaxMultiMediaFileSize;

			return -1;
		}

		string GetFileOpenDialogFilter()
		{
			return "Image Files(*.gif;*.jpg;*.jpeg;*.png)|*.gif;*.jpg;*.jpeg;*.png|"
			+ "Movie Files(*.avi;*.wmv;*.flv;*.m4v;*.mov;*.mp4;*.rm;*.mpeg;*.mpg;*.3gp;*.3g2)|*.avi;*.wmv;*.flv;*.m4v;*.mov;*.mp4;*.rm;*.mpeg;*.mpg;*.3gp;*.3g2";
		}
	}
}
