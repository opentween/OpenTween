// OpenTween - Client of Twitter
// Copyright (c) 2022 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
// with this program. If not, see <http://www.gnu.org/licenses/>, or write to
// the Free Software Foundation, Inc., 51 Franklin Street - Fifth Floor,
// Boston, MA 02110-1301, USA.

#nullable enable

using System;
using OpenTween.Setting;

namespace OpenTween
{
    public sealed class ApplicationContainer : IDisposable
    {
        public bool IsDisposed { get; private set; } = false;

        public SettingManager Settings { get; } = SettingManager.Instance;

        public CultureService CultureService
            => this.cultureServiceLazy.Value;

        public TweenMain MainForm
            => this.mainFormLazy.Value;

        private readonly Lazy<CultureService> cultureServiceLazy;
        private readonly Lazy<TweenMain> mainFormLazy;

        public ApplicationContainer()
        {
            this.cultureServiceLazy = new(this.CreateCultureService);
            this.mainFormLazy = new(this.CreateTweenMain);
        }

        private CultureService CreateCultureService()
            => new(this.Settings.Common);

        private TweenMain CreateTweenMain()
            => new();

        public void Dispose()
        {
            if (this.IsDisposed)
                return;

            this.IsDisposed = true;
            this.MainForm.Dispose();
        }
    }
}
