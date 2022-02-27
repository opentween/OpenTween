// OpenTween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2011      kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace OpenTween
{
    public class GrowlHelper
    {
        private Assembly? connector = null;
        private Assembly? core = null;

        private object? growlNTreply;
        private object? growlNTdm;
        private object? growlNTnew;
        private object? growlApp;

        private object? targetConnector;
        private bool initialized = false;

        public class NotifyCallbackEventArgs : EventArgs
        {
            public long StatusId { get; set; }
            public NotifyType NotifyType { get; set; }
            public NotifyCallbackEventArgs(NotifyType notifyType, string statusId)
            {
                if (statusId.Length > 1)
                {
                    this.StatusId = long.Parse(statusId);
                    this.NotifyType = notifyType;
                }
            }
        }

        public event EventHandler<NotifyCallbackEventArgs>? NotifyClicked;

        public string AppName { get; }

        public enum NotifyType
        {
            Reply = 0,
            DirectMessage = 1,
            Notify = 2,
        }

        public GrowlHelper(string appName)
            => this.AppName = appName;

        public bool IsAvailable
        {
            get
            {
                if (this.connector == null || this.core == null || !this.initialized)
                    return false;
                else
                    return true;
            }
        }

        private byte[] IconToByteArray(string filename)
        {
            using var ic = new Icon(filename);
            return this.IconToByteArray(ic);
        }

        private byte[] IconToByteArray(Icon icondata)
        {
            using var ms = new MemoryStream();
            using var ic = new Icon(icondata, 48, 48);
            ic.ToBitmap().Save(ms, ImageFormat.Png);
            return ms.ToArray();
        }

        public static bool IsDllExists
        {
            get
            {
                var dir = Application.StartupPath;
                var connectorPath = Path.Combine(dir, "Growl.Connector.dll");
                var corePath = Path.Combine(dir, "Growl.CoreLibrary.dll");
                if (File.Exists(connectorPath) && File.Exists(corePath))
                    return true;
                else
                    return false;
            }
        }

        public bool RegisterGrowl()
        {
            this.initialized = false;
            var dir = Application.StartupPath;
            var connectorPath = Path.Combine(dir, "Growl.Connector.dll");
            var corePath = Path.Combine(dir, "Growl.CoreLibrary.dll");

            try
            {
                if (!IsDllExists) return false;
                this.connector = Assembly.LoadFile(connectorPath);
                this.core = Assembly.LoadFile(corePath);
            }
            catch (Exception)
            {
                return false;
            }

            try
            {
                this.targetConnector = this.connector.CreateInstance("Growl.Connector.GrowlConnector");
                var t = this.connector.GetType("Growl.Connector.NotificationType");

                this.growlNTreply = t.InvokeMember(
                    null,
                    BindingFlags.CreateInstance,
                    null,
                    null,
                    new object[] { "REPLY", "Reply" },
                    CultureInfo.InvariantCulture);

                this.growlNTdm = t.InvokeMember(null,
                    BindingFlags.CreateInstance,
                    null,
                    null,
                    new object[] { "DIRECT_MESSAGE", "DirectMessage" },
                    CultureInfo.InvariantCulture);

                this.growlNTnew = t.InvokeMember(
                    null,
                    BindingFlags.CreateInstance,
                    null,
                    null,
                    new object[] { "NOTIFY", "新着通知" },
                    CultureInfo.InvariantCulture);

                var encryptType =
                        this.connector.GetType("Growl.Connector.Cryptography+SymmetricAlgorithmType").InvokeMember(
                            "PlainText", BindingFlags.GetField, null, null, null, CultureInfo.InvariantCulture);
                this.targetConnector.GetType().InvokeMember("EncryptionAlgorithm", BindingFlags.SetProperty, null, this.targetConnector, new object[] { encryptType }, CultureInfo.InvariantCulture);

                this.growlApp = this.connector.CreateInstance(
                    "Growl.Connector.Application", false, BindingFlags.Default, null, new object[] { this.AppName }, null, null);

                if (File.Exists(Path.Combine(Application.StartupPath, "Icons\\Tween.png")))
                {
                    // Icons\Tween.pngを使用
                    var ci = this.core.GetType("Growl.CoreLibrary.Resource").GetConstructor(
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        null,
                        new Type[] { typeof(string) },
                        null);

                    var data = ci.Invoke(new object[] { Path.Combine(Application.StartupPath, "Icons\\Tween.png") });
                    var pi = this.growlApp.GetType().GetProperty("Icon");
                    pi.SetValue(this.growlApp, data, null);
                }
                else if (File.Exists(Path.Combine(Application.StartupPath, "Icons\\MIcon.ico")))
                {
                    // アイコンセットにMIcon.icoが存在する場合それを使用
                    var cibd = this.core.GetType("Growl.CoreLibrary.BinaryData").GetConstructor(
                        BindingFlags.Public | BindingFlags.Instance,
                        null,
                        new Type[] { typeof(byte[]) },
                        null);
                    var bdata = cibd.Invoke(
                        new object[] { this.IconToByteArray(Path.Combine(Application.StartupPath, "Icons\\MIcon.ico")) });

                    var ciRes = this.core.GetType("Growl.CoreLibrary.Resource").GetConstructor(
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        null,
                        new Type[] { bdata.GetType() },
                        null);

                    var data = ciRes.Invoke(new object[] { bdata });
                    var pi = this.growlApp.GetType().GetProperty("Icon");
                    pi.SetValue(this.growlApp, data, null);
                }
                else
                {
                    // 内蔵アイコンリソースを使用
                    var cibd = this.core.GetType("Growl.CoreLibrary.BinaryData").GetConstructor(
                        BindingFlags.Public | BindingFlags.Instance,
                        null,
                        new Type[] { typeof(byte[]) },
                        null);
                    var bdata = cibd.Invoke(
                        new object[] { this.IconToByteArray(Properties.Resources.MIcon) });

                    var ciRes = this.core.GetType("Growl.CoreLibrary.Resource").GetConstructor(
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        null,
                        new Type[] { bdata.GetType() },
                        null);

                    var data = ciRes.Invoke(new object[] { bdata });
                    var pi = this.growlApp.GetType().GetProperty("Icon");
                    pi.SetValue(this.growlApp, data, null);
                }

                var mi = this.targetConnector.GetType().GetMethod("Register", new Type[] { this.growlApp.GetType(), this.connector.GetType("Growl.Connector.NotificationType[]") });

                t = this.connector.GetType("Growl.Connector.NotificationType");

                var arglist = new ArrayList
                {
                    this.growlNTreply,
                    this.growlNTdm,
                    this.growlNTnew,
                };

                mi.Invoke(this.targetConnector, new object[] { this.growlApp, arglist.ToArray(t) });

                // コールバックメソッドの登録
                var tGrowlConnector = this.connector.GetType("Growl.Connector.GrowlConnector");
                var evNotificationCallback = tGrowlConnector.GetEvent("NotificationCallback");
                var tDelegate = evNotificationCallback.EventHandlerType;
                var miHandler = typeof(GrowlHelper).GetMethod("GrowlCallbackHandler", BindingFlags.NonPublic | BindingFlags.Instance);
                var d = Delegate.CreateDelegate(tDelegate, this, miHandler);
                var miAddHandler = evNotificationCallback.GetAddMethod();
                object[] addHandlerArgs = { d };
                miAddHandler.Invoke(this.targetConnector, addHandlerArgs);

                this.initialized = true;
            }
            catch (Exception)
            {
                this.initialized = false;
                return false;
            }

            return true;
        }

        public void Notify(NotifyType notificationType, string id, string title, string text, Image? icon = null, string url = "")
        {
            if (!this.initialized) return;

            var notificationName = notificationType switch
            {
                NotifyType.Reply => "REPLY",
                NotifyType.DirectMessage => "DIRECT_MESSAGE",
                NotifyType.Notify => "NOTIFY",
                _ => "",
            };

            object? n;
            if (icon != null || !MyCommon.IsNullOrEmpty(url))
            {
                var gCore = this.core!.GetType("Growl.CoreLibrary.Resource");
                object? res;
                if (icon != null)
                {
                    res = gCore.InvokeMember("op_Implicit",
                                             BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod,
                                             null,
                                             null,
                                             new object[] { icon },
                                             CultureInfo.InvariantCulture);
                }
                else
                {
                    res = gCore.InvokeMember("op_Implicit",
                                             BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod,
                                             null,
                                             null,
                                             new object[] { url },
                                             CultureInfo.InvariantCulture);
                }
                var priority =
                        this.connector!.GetType("Growl.Connector.Priority").InvokeMember(
                            "Normal", BindingFlags.GetField, null, null, null, CultureInfo.InvariantCulture);
                n = this.connector!.GetType("Growl.Connector.Notification").InvokeMember(
                        "Notification",
                        BindingFlags.CreateInstance,
                        null,
                        this.connector,
                        new object[]
                        {
                            this.AppName,
                            notificationName,
                            id,
                            title,
                            text,
                            res,
                            false,
                            priority,
                            "aaa",
                        },
                        CultureInfo.InvariantCulture);
            }
            else
            {
                n = this.connector!.GetType("Growl.Connector.Notification").InvokeMember(
                        "Notification",
                        BindingFlags.CreateInstance,
                        null,
                        this.connector,
                        new object[]
                        {
                            this.AppName,
                            notificationName,
                            id,
                            title,
                            text,
                        },
                        CultureInfo.InvariantCulture);
            }
            var cc = this.connector.GetType("Growl.Connector.CallbackContext").InvokeMember(
                null,
                BindingFlags.CreateInstance,
                null,
                this.connector,
                new object[] { "some fake information", notificationName },
                CultureInfo.InvariantCulture);
            this.targetConnector!.GetType().InvokeMember("Notify", BindingFlags.InvokeMethod, null, this.targetConnector, new object[] { n, cc }, CultureInfo.InvariantCulture);
        }

        private void GrowlCallbackHandler(object response, object callbackData, object state)
        {
            try
            {
                // 定数取得
                var vCLICK =
                this.core!.GetType("Growl.CoreLibrary.CallbackResult").GetField(
                    "CLICK",
                    BindingFlags.Public | BindingFlags.Static).GetRawConstantValue();
                // 実際の値
                var vResult = callbackData.GetType().GetProperty(
                            "Result",
                            BindingFlags.Public | BindingFlags.Instance).GetGetMethod().Invoke(callbackData, null);
                vResult = (int)vResult;
                var notifyId = (string)callbackData.GetType().GetProperty("NotificationID").GetGetMethod().Invoke(callbackData, null);
                var notifyName = (string)callbackData.GetType().GetProperty("Type").GetGetMethod().Invoke(callbackData, null);
                if (vCLICK.Equals(vResult))
                {
                    NotifyType nt;
                    switch (notifyName)
                    {
                        case "REPLY":
                            nt = NotifyType.Reply;
                            break;
                        case "DIRECT_MESSAGE":
                            nt = NotifyType.DirectMessage;
                            break;
                        case "NOTIFY":
                            nt = NotifyType.Notify;
                            break;
                        default:
                            return;
                    }

                    this.NotifyClicked?.Invoke(this, new NotifyCallbackEventArgs(nt, notifyId));
                }
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}
