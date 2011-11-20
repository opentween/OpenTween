// Tween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2011      Egtra (@egtra) <http://dev.activebasic.com/egtra/>
// All rights reserved.
// 
// This file is part of Tween.
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

namespace Tween
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
    }

    public class ApiInfo : ApiInfoBase
    {
        public int MaxCount;
        public int RemainCount;
        public DateTime ResetTime;
        public int ResetTimeInSeconds;
        public int UsingCount;

        public ApiInfo()
        {
            this.MaxCount = _MaxCount;
            this.RemainCount = _RemainCount;
            this.ResetTime = _ResetTime;
            this.ResetTimeInSeconds = _ResetTimeInSeconds;
            this.UsingCount = _UsingCount;
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
            _MaxCount = -1;
            _RemainCount = -1;
            _ResetTime = new DateTime();
            _ResetTimeInSeconds = -1;
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

        public void ParseHttpHeaders(Dictionary<string, string> headers)
        {
            _MaxCount = MaxCountFromHttpHeader;
            _RemainCount = RemainCountFromHttpHeader;
            _ResetTime = ResetTimeFromHttpHeader;
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
