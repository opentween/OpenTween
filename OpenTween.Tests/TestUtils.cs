// OpenTween - Client of Twitter
// Copyright (c) 2012 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xunit;

namespace OpenTween
{
    internal static class TestUtils
    {
        public static void CheckDeepCloning(object obj, object cloneObj)
        {
            Assert.Equal(obj, cloneObj);
            Assert.NotSame(obj, cloneObj);

            foreach (var field in obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var objValue = field.GetValue(obj);
                var cloneValue = field.GetValue(cloneObj);

                Assert.Equal(objValue, cloneValue);
                if (objValue == null && cloneValue == null) continue;
                if (field.FieldType.IsValueType || field.FieldType == typeof(string)) continue;

                Assert.NotSame(objValue, cloneValue);
            }
        }

        public static async Task NotRaisesAsync<T>(Action<EventHandler<T>> attach, Action<EventHandler<T>> detach, Func<Task> testCode)
            where T : EventArgs
        {
            T? raisedEvent = null;

            void Handler(object s, T e)
                => raisedEvent = e;

            try
            {
                attach(Handler);
                await testCode().ConfigureAwait(false);

                if (raisedEvent != null)
                    throw new Xunit.Sdk.RaisesException(typeof(void), raisedEvent.GetType());
            }
            finally
            {
                detach(Handler);
            }
        }

        public static void NotPropertyChanged(INotifyPropertyChanged @object, string propertyName, Action testCode)
        {
            void Handler(object s, PropertyChangedEventArgs e)
            {
                if (s == @object && e.PropertyName == propertyName)
                    throw new Xunit.Sdk.PropertyChangedException(propertyName);
            }

            try
            {
                @object.PropertyChanged += Handler;
                testCode();
            }
            finally
            {
                @object.PropertyChanged -= Handler;
            }
        }

        public static MemoryImage CreateDummyImage()
        {
            using var bitmap = new Bitmap(100, 100);
            using var stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Png);
            stream.Position = 0;

            return MemoryImage.CopyFromStream(stream);
        }

        public static MemoryImageMediaItem CreateDummyMediaItem()
            => new(CreateDummyImage());

        public static void FireEvent<T>(T control, string eventName)
            where T : Control
            => TestUtils.FireEvent(control, eventName, EventArgs.Empty);

        public static void FireEvent<T>(T control, string eventName, EventArgs e)
            where T : Control
        {
            var methodName = "On" + eventName;
            var method = typeof(T).GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);

            method.Invoke(control, new[] { e });
        }

        public static void Validate<T>(T control)
            where T : Control
        {
            var cancelEventArgs = new CancelEventArgs();
            TestUtils.FireEvent(control, "Validating", cancelEventArgs);

            if (cancelEventArgs.Cancel)
                return;

            TestUtils.FireEvent(control, "Validated");
        }

        public static IDisposable FreezeTime(DateTimeUtc datetime)
        {
            DateTimeUtc.UseFakeNow = true;
            DateTimeUtc.FakeNow = datetime;
            DateTimeUtc.FakeNowDrift = TimeSpan.Zero;
            return new RestoreFreezedTime();
        }

        public static void DriftTime(TimeSpan drift)
            => DateTimeUtc.FakeNowDrift += drift;

        private sealed class RestoreFreezedTime : IDisposable
        {
            public void Dispose()
                => DateTimeUtc.UseFakeNow = false;
        }

        public static DateTimeUtc LocalTime(int year, int month, int day, int hour, int minute, int second)
            => new(new DateTimeOffset(year, month, day, hour, minute, second, TimeZoneInfo.Local.BaseUtcOffset));
    }
}

#pragma warning disable SA1403

namespace OpenTween.Setting
{
    public class SettingManagerTest
    {
        public static SettingCommon Common
        {
            get => SettingManager.Instance.Common;
            set => SettingManager.Instance.Common = value;
        }

        public static SettingLocal Local
        {
            get => SettingManager.Instance.Local;
            set => SettingManager.Instance.Local = value;
        }

        public static SettingTabs Tabs
        {
            get => SettingManager.Instance.Tabs;
            set => SettingManager.Instance.Tabs = value;
        }

        public static SettingAtIdList AtIdList
        {
            get => SettingManager.Instance.AtIdList;
            set => SettingManager.Instance.AtIdList = value;
        }
    }
}
