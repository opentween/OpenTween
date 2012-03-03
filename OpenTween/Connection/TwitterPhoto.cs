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

using IMultimediaShareService = OpenTween.IMultimediaShareService;
using Array = System.Array;
using Convert = System.Convert;
using Exception = System.Exception;
using UploadFileType = OpenTween.MyCommon.UploadFileType;
using MyCommon = OpenTween.MyCommon;
using FileInfo = System.IO.FileInfo;
using NotSupportedException = System.NotSupportedException;

namespace OpenTween
{
	public class TwitterPhoto : IMultimediaShareService
	{
		private string[] pictureExt = new string[] { ".jpg", ".jpeg", ".gif", ".png" };

		private const long MaxfilesizeDefault = 3145728;

		// help/configurationにより取得されコンストラクタへ渡される
		private long _MaxFileSize = 3145728;

		private Twitter tw;

		public bool CheckValidExtension( string ext )
		{
			if ( Array.IndexOf( this.pictureExt, ext.ToLower() ) > -1 )
				return true;

			return false;
		}

		public bool CheckValidFilesize( string ext, long fileSize )
		{
			if ( this.CheckValidExtension( ext ) )
				return fileSize <= this._MaxFileSize;

			return false;
		}

		public bool Configuration( string key, object value )
		{
			if ( key == "MaxUploadFilesize" )
			{
				long val;
				try
				{
					val = Convert.ToInt64( value );
					if ( val > 0 )
						this._MaxFileSize = val;
					else
					this._MaxFileSize = TwitterPhoto.MaxfilesizeDefault;
				}
				catch ( Exception )
				{
					this._MaxFileSize = TwitterPhoto.MaxfilesizeDefault;
					return false; // error
				}
				return true; // 正常に設定終了
			}
			return true; // 設定項目がない場合はとりあえずエラー扱いにしない
		}

		public string GetFileOpenDialogFilter()
		{
			return "Image Files(*.gif;*.jpg;*.jpeg;*.png)|*.gif;*.jpg;*.jpeg;*.png";
		}

		public UploadFileType GetFileType( string ext )
		{
			if ( this.CheckValidExtension( ext ) )
				return UploadFileType.Picture;

			return UploadFileType.Invalid;
		}

		public bool IsSupportedFileType( UploadFileType type )
		{
			return type.Equals( UploadFileType.Picture );
		}

		public string Upload( ref string filePath, ref string message, long reply_to )
		{
			if ( string.IsNullOrEmpty( filePath ) )
				return "Err:File isn't specified.";

			if ( string.IsNullOrEmpty( message ) )
				message =  "";

			FileInfo mediaFile;
			try
			{
				mediaFile = new FileInfo( filePath );
			}
			catch ( NotSupportedException ex )
			{
				return "Err:" + ex.Message;
			}

			if ( !mediaFile.Exists )
				return "Err:File isn't exists.";

			if ( MyCommon.IsAnimatedGif( filePath ) )
				return "Err:Don't support animatedGIF.";

			return tw.PostStatusWithMedia( message, reply_to, mediaFile );
		}

		public TwitterPhoto( Twitter twitter )
		{
			this.tw = twitter;
		}
	}
}
