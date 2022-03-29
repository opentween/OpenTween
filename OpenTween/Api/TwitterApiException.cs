// OpenTween - Client of Twitter
// Copyright (c) 2016 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using OpenTween.Api.DataModel;

namespace OpenTween.Api
{
    [Serializable]
    public class TwitterApiException : WebApiException
    {
        public HttpStatusCode StatusCode { get; }

        public TwitterError? ErrorResponse { get; }

        public TwitterErrorItem[] Errors
            => this.ErrorResponse != null ? this.ErrorResponse.Errors : Array.Empty<TwitterErrorItem>();

        public TwitterApiException()
        {
        }

        public TwitterApiException(string message)
            : base(message)
        {
        }

        public TwitterApiException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public TwitterApiException(HttpStatusCode statusCode, string responseText)
            : base(statusCode.ToString(), responseText)
        {
            this.StatusCode = statusCode;
        }

        public TwitterApiException(HttpStatusCode statusCode, TwitterError error, string responseText)
            : base(FormatTwitterError(error), responseText)
        {
            this.StatusCode = statusCode;
            this.ErrorResponse = error;
        }

        protected TwitterApiException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.StatusCode = (HttpStatusCode)info.GetValue("StatusCode", typeof(HttpStatusCode));
            this.ErrorResponse = (TwitterError)info.GetValue("ErrorResponse", typeof(TwitterError));
        }

        private TwitterApiException(string message, string responseText, Exception innerException)
            : base(message, responseText, innerException)
        {
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("StatusCode", this.StatusCode);
            info.AddValue("ErrorResponse", this.ErrorResponse);
        }

        public static TwitterApiException CreateFromException(HttpRequestException ex)
            => new(ex.InnerException?.Message ?? ex.Message, ex);

        public static TwitterApiException CreateFromException(OperationCanceledException ex)
            => new("Timeout", ex);

        public static TwitterApiException CreateFromException(SerializationException ex, string responseText)
            => new("Invalid JSON", responseText, ex);

        private static string FormatTwitterError(TwitterError error)
            => string.Join(",", error.Errors.Select(x => x.ToString()));
    }
}
