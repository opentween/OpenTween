// OpenTween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2011      Egtra (@egtra) <http://dev.activebasic.com/egtra/>
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
// with this program. if not, see <http://www.gnu.org/licenses/>, or write to
// the Free Software Foundation, Inc., 51 Franklin Street - Fifth Floor,
// Boston, MA 02110-1301, USA.

using System;
using System.Collections.Generic;

namespace OpenTween
{
    public class ApiInformationChangedEventArgs : EventArgs
    {
        public ApiInfo ApiInfo = new ApiInfo();
    }

    public abstract class ApiInfoBase
    {
        protected static int _MaxCount = -1;
        protected static int _RemainCount = -1;
        protected static DateTime _ResetTime = new DateTime();
        protected static int _ResetTimeInSeconds = -1;
        protected static int _UsingCount = -1;
        protected static ApiAccessLevel _AccessLevel = ApiAccessLevel.None;
        protected static int _MediaMaxCount = -1;
        protected static DateTime _MediaResetTime = new DateTime();
        protected static int _MediaRemainCount = -1;
    }

    public enum ApiAccessLevel
    {
        None,
        Unknown,
        Read,
        ReadWrite,
        ReadWriteAndDirectMessage,
    }

    public class ApiInfo : ApiInfoBase
    {
        public int MaxCount;
        public int RemainCount;
        public DateTime ResetTime;
        public int ResetTimeInSeconds;
        public int UsingCount;
        public ApiAccessLevel AccessLevel;
        public int MediaMaxCount;
        public int MediaRemainCount;
        public DateTime MediaResetTime;

        public ApiInfo()
        {
            this.MaxCount = _MaxCount;
            this.RemainCount = _RemainCount;
            this.ResetTime = _ResetTime;
            this.ResetTimeInSeconds = _ResetTimeInSeconds;
            this.UsingCount = _UsingCount;
            this.AccessLevel = _AccessLevel;
            this.MediaMaxCount = _MediaMaxCount;
            this.MediaRemainCount = _MediaRemainCount;
            this.MediaResetTime = _MediaResetTime;
        }
    }

    public class ApiInformation : ApiInfoBase
    {
        //private ReadOnly _lockobj As new Object 更新時にロックが必要かどうかは様子見

        public Dictionary<string, string> HttpHeaders = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);

        public void Initialize()
        {
            if (HttpHeaders.ContainsKey("X-RateLimit-Remaining"))
            {
                HttpHeaders["X-RateLimit-Remaining"] = "-1";
            }
            else
            {
                HttpHeaders.Add("X-RateLimit-Remaining", "-1");
            }

            if (HttpHeaders.ContainsKey("X-RateLimit-Limit"))
            {
                HttpHeaders["X-RateLimit-Limit"] = "-1";
            }
            else
            {
                HttpHeaders.Add("X-RateLimit-Limit", "-1");
            }

            if (HttpHeaders.ContainsKey("X-RateLimit-Reset"))
            {
                HttpHeaders["X-RateLimit-Reset"] = "-1";
            }
            else
            {
                HttpHeaders.Add("X-RateLimit-Reset", "-1");
            }

            if (HttpHeaders.ContainsKey("X-Access-Level"))
            {
                HttpHeaders["X-Access-Level"] = "read-write-directmessages";
            }
            else
            {
                HttpHeaders.Add("X-Access-Level", "read-write-directmessages");
            }

            if (HttpHeaders.ContainsKey("X-MediaRateLimit-Remaining"))
            {
                HttpHeaders["X-MediaRateLimit-Remaining"] = "-1";
            }
            else
            {
                HttpHeaders.Add("X-MediaRateLimit-Remaining", "-1");
            }

            if (HttpHeaders.ContainsKey("X-MediaRateLimit-Limit"))
            {
                HttpHeaders["X-MediaRateLimit-Limit"] = "-1";
            }
            else
            {
                HttpHeaders.Add("X-MediaRateLimit-Limit", "-1");
            }

            if (HttpHeaders.ContainsKey("X-MediaRateLimit-Reset"))
            {
                HttpHeaders["X-MediaRateLimit-Reset"] = "-1";
            }
            else
            {
                HttpHeaders.Add("X-MediaRateLimit-Reset", "-1");
            }

            _MaxCount = -1;
            _RemainCount = -1;
            _ResetTime = new DateTime();
            _ResetTimeInSeconds = -1;
            AccessLevel = ApiAccessLevel.None;
            _MediaMaxCount = -1;
            _MediaRemainCount = -1;
            _MediaResetTime = new DateTime();

            //_UsingCount = -1
            var arg = new ApiInformationChangedEventArgs();
            var changed = Changed;
            if (changed != null)
            {
                changed(this, arg);
            }
        }

        public DateTime ConvertResetTimeInSecondsToResetTime(int seconds)
        {
            if (seconds >= 0)
            {
                return System.TimeZone.CurrentTimeZone.ToLocalTime((new DateTime(1970, 1, 1, 0, 0, 0)).AddSeconds(seconds));
            }
            else
            {
                return new DateTime();
            }
        }

        public event EventHandler<ApiInformationChangedEventArgs> Changed;

        private void Raise_Changed()
        {
            var arg = new ApiInformationChangedEventArgs();
            var changed = Changed;
            if (changed != null)
            {
                changed(this, arg);
            }
            _MaxCount = arg.ApiInfo.MaxCount;
            _RemainCount = arg.ApiInfo.RemainCount;
            _ResetTime = arg.ApiInfo.ResetTime;
            _ResetTimeInSeconds = arg.ApiInfo.ResetTimeInSeconds;
            //_UsingCount = arg.ApiInfo.UsingCount
        }

        public int MaxCount
        {
            get
            {
                return _MaxCount;
            }
            set
            {
                if (_MaxCount != value)
                {
                    _MaxCount = value;
                    Raise_Changed();
                }
            }
        }

        public int RemainCount
        {
            get
            {
                return _RemainCount;
            }
            set
            {
                if (_RemainCount != value)
                {
                    _RemainCount = value;
                    Raise_Changed();
                }
            }
        }

        public DateTime ResetTime
        {
            get
            {
                return _ResetTime;
            }
            set
            {
                if (_ResetTime != value)
                {
                    _ResetTime = value;
                    Raise_Changed();
                }
            }
        }

        public int MediaMaxCount
        {
            get
            {
                return _MediaMaxCount;
            }
            set
            {
                if (_MediaMaxCount != value)
                {
                    _MediaMaxCount = value;
                    Raise_Changed();
                }
            }
        }

        public int MediaRemainCount
        {
            get
            {
                return _MediaRemainCount;
            }
            set
            {
                if (_MediaRemainCount != value)
                {
                    _MediaRemainCount = value;
                    Raise_Changed();
                }
            }
        }

        public DateTime MediaResetTime
        {
            get
            {
                return _MediaResetTime;
            }
            set
            {
                if (_MediaResetTime != value)
                {
                    _MediaResetTime = value;
                    Raise_Changed();
                }
            }
        }

        public int ResetTimeInSeconds
        {
            get
            {
                return _ResetTimeInSeconds;
            }
            set
            {
                if (_ResetTimeInSeconds != value)
                {
                    _ResetTimeInSeconds = value;
                    Raise_Changed();
                }
            }
        }

        public int UsingCount
        {
            get
            {
                return _UsingCount;
            }
            set
            {
                if (_UsingCount != value)
                {
                    _UsingCount = value;
                    Raise_Changed();
                }
            }
        }

        public ApiAccessLevel AccessLevel
        {
            get
            {
                return _AccessLevel;
            }
            private set
            {
                if (_AccessLevel != value)
                {
                    _AccessLevel = value;
                    Raise_Changed();
                }
            }
        }

        public bool IsReadPermission
        {
            get
            {
                return AccessLevel == ApiAccessLevel.Read ||
                    AccessLevel == ApiAccessLevel.ReadWrite ||
                    AccessLevel == ApiAccessLevel.ReadWriteAndDirectMessage;
            }
        }

        public bool IsWritePermission
        {
            get
            {
                return AccessLevel == ApiAccessLevel.ReadWrite ||
                    AccessLevel == ApiAccessLevel.ReadWriteAndDirectMessage;
            }
        }

        public bool IsDirectMessagePermission
        {
            get
            {
                return AccessLevel == ApiAccessLevel.ReadWriteAndDirectMessage;
            }
        }

        private int RemainCountFromHttpHeader
        {
            get
            {
                int result = 0;
                if (string.IsNullOrEmpty(HttpHeaders["X-RateLimit-Remaining"])) return -1;
                if (int.TryParse(HttpHeaders["X-RateLimit-Remaining"], out result))
                {
                    return result;
                }
                return -1;
            }
        }

        private int MaxCountFromHttpHeader
        {
            get
            {
                int result;
                if (string.IsNullOrEmpty(HttpHeaders["X-RateLimit-Limit"])) return -1;
                if (int.TryParse(HttpHeaders["X-RateLimit-Limit"], out result))
                {
                    return result;
                }
                return -1;
            }
        }

        private DateTime ResetTimeFromHttpHeader
        {
            get
            {
                int i;
                if (int.TryParse(HttpHeaders["X-RateLimit-Reset"], out i))
                {
                    if (i >= 0)
                    {
                        return TimeZone.CurrentTimeZone.ToLocalTime((new DateTime(1970, 1, 1, 0, 0, 0)).AddSeconds(i));
                    }
                    else
                    {
                        return new DateTime();
                    }
                }
                else
                {
                    return new DateTime();
                }
            }
        }

        private int MediaRemainCountFromHttpHeader
        {
            get
            {
                int result = 0;
                if (string.IsNullOrEmpty(HttpHeaders["X-MediaRateLimit-Remaining"])) return -1;
                if (int.TryParse(HttpHeaders["X-MediaRateLimit-Remaining"], out result))
                {
                    return result;
                }
                return -1;
            }
        }

        private int MediaMaxCountFromHttpHeader
        {
            get
            {
                int result = 0;
                if (string.IsNullOrEmpty(HttpHeaders["X-MediaRateLimit-Limit"])) return -1;
                if (int.TryParse(HttpHeaders["X-MediaRateLimit-Limit"], out result))
                {
                    return result;
                }
                return -1;
            }
        }

        private DateTime MediaResetTimeFromHttpHeader
        {
            get
            {
                int i;
                if (int.TryParse(HttpHeaders["X-MediaRateLimit-Reset"], out i))
                {
                    if (i >= 0)
                    {
                        return TimeZone.CurrentTimeZone.ToLocalTime((new DateTime(1970, 1, 1, 0, 0, 0)).AddSeconds(i));
                    }
                    else
                    {
                        return new DateTime();
                    }
                }
                else
                {
                    return new DateTime();
                }
            }
        }

        private ApiAccessLevel ApiAccessLevelFromHttpHeader
        {
            get
            {
                switch (HttpHeaders["X-Access-Level"])
                {
                case "read":
                    return ApiAccessLevel.Read;
                case "read-write":
                    return ApiAccessLevel.ReadWrite;
                case "read-write-directmessages":
                case "read-write-privatemessages":
                    return ApiAccessLevel.ReadWriteAndDirectMessage;
                default:
                    MyCommon.TraceOut("Unknown ApiAccessLevel:" + HttpHeaders["X-Access-Level"]);
                    return ApiAccessLevel.ReadWriteAndDirectMessage;     //未知のアクセスレベルの場合Read/Write/Dmと仮定して処理継続;
                }
            }
        }

        public void ParseHttpHeaders(Dictionary<string, string> headers)
        {
            int tmp;
            DateTime tmpd;
            tmp = MaxCountFromHttpHeader;
            if (tmp != -1)
            {
                _MaxCount = tmp;
            }
            tmp = RemainCountFromHttpHeader;
            if (tmp != -1)
            {
                _RemainCount = tmp;
            }
            tmpd = ResetTimeFromHttpHeader;
            if (tmpd != new DateTime())
            {
                _ResetTime = tmpd;
            }

            tmp = MediaMaxCountFromHttpHeader;
            if (tmp != -1)
            {
                _MediaMaxCount = tmp;
            }
            tmp = MediaRemainCountFromHttpHeader;
            if (tmp != -1)
            {
                _MediaRemainCount = tmp;
            }
            tmpd = MediaResetTimeFromHttpHeader;
            if (tmpd != new DateTime())
            {
                _MediaResetTime = tmpd;
            }

            AccessLevel = ApiAccessLevelFromHttpHeader;
            Raise_Changed();
        }

        public void WriteBackEventArgs(ApiInformationChangedEventArgs arg)
        {
            _MaxCount = arg.ApiInfo.MaxCount;
            _RemainCount = arg.ApiInfo.RemainCount;
            _ResetTime = arg.ApiInfo.ResetTime;
            _ResetTimeInSeconds = arg.ApiInfo.ResetTimeInSeconds;
            _UsingCount = arg.ApiInfo.UsingCount;
            Raise_Changed();
        }
    }
}
