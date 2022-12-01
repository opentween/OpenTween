﻿// OpenTween - Client of Twitter
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
using System.Text;
using System.Threading.Tasks;
using OpenTween.Connection;

namespace OpenTween
{
    public class PostStatusParams
    {
        public string Text { get; set; } = "";

        public long? InReplyToStatusId { get; set; }

        public IReadOnlyList<long> MediaIds { get; set; } = Array.Empty<long>();

        public bool AutoPopulateReplyMetadata { get; set; }

        public IReadOnlyList<long> ExcludeReplyUserIds { get; set; } = Array.Empty<long>();

        public string? AttachmentUrl { get; set; }

        public bool PostToMastodon { get; set; } = false;
    }
}
