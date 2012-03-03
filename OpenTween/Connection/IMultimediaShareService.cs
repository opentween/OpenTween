// OpenTween - Client of Twitter
// Copyright (c) 2011      kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2011      Egtra (@egtra) <http://dev.activebasic.com/egtra/>
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

namespace OpenTween
{
    public interface IMultimediaShareService
    {
        string Upload(ref string filePath,
                        ref string message,
                        long reply_to);
        bool CheckValidExtension(string ext) ;
        string GetFileOpenDialogFilter();
        MyCommon.UploadFileType GetFileType(string ext);
        bool IsSupportedFileType(MyCommon.UploadFileType type);
        bool CheckValidFilesize(string ext, long fileSize);
        bool Configuration(string key, object value);
    }
}
