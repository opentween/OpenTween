// OpenTween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2014      kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace OpenTween.Setting.Panel
{
    public partial class NotifyPanel : SettingPanelBase
    {
        public NotifyPanel()
        {
            InitializeComponent();
        }

        public void LoadConfig(SettingCommon settingCommon)
        {
            this.ApplyEventNotifyFlag(settingCommon.EventNotifyEnabled, settingCommon.EventNotifyFlag, settingCommon.IsMyEventNotifyFlag);
            this.CheckForceEventNotify.Checked = settingCommon.ForceEventNotify;
            this.CheckFavEventUnread.Checked = settingCommon.FavEventUnread;

            this.SoundFileListup();

            var soundFile = settingCommon.EventSoundFile ?? "";
            var soundFileIdx = this.ComboBoxEventNotifySound.Items.IndexOf(soundFile);
            if (soundFileIdx != -1)
                this.ComboBoxEventNotifySound.SelectedIndex = soundFileIdx;

            this.IsRemoveSameFavEventCheckBox.Checked = settingCommon.IsRemoveSameEvent;
        }

        public void SaveConfig(SettingCommon settingCommon)
        {
            settingCommon.EventNotifyEnabled = this.CheckEventNotify.Checked;
            this.GetEventNotifyFlag(ref settingCommon.EventNotifyFlag, ref settingCommon.IsMyEventNotifyFlag);
            settingCommon.ForceEventNotify = this.CheckForceEventNotify.Checked;
            settingCommon.FavEventUnread = this.CheckFavEventUnread.Checked;
            settingCommon.EventSoundFile = (string)this.ComboBoxEventNotifySound.SelectedItem;
            settingCommon.IsRemoveSameEvent = this.IsRemoveSameFavEventCheckBox.Checked;
        }

        private void SoundFileListup()
        {
            this.ComboBoxEventNotifySound.Items.Clear();
            this.ComboBoxEventNotifySound.Items.Add("");
            DirectoryInfo oDir = new DirectoryInfo(Application.StartupPath + Path.DirectorySeparatorChar);
            if (Directory.Exists(Path.Combine(Application.StartupPath, "Sounds")))
            {
                oDir = oDir.GetDirectories("Sounds")[0];
            }
            foreach (FileInfo oFile in oDir.GetFiles("*.wav"))
            {
                this.ComboBoxEventNotifySound.Items.Add(oFile.Name);
            }
        }

        private class EventCheckboxTblElement
        {
            public CheckBox CheckBox;
            public MyCommon.EVENTTYPE Type;
        }

        private EventCheckboxTblElement[] GetEventCheckboxTable()
        {
            return new[]
            {
                new EventCheckboxTblElement
                {
                    CheckBox = this.CheckFavoritesEvent,
                    Type = MyCommon.EVENTTYPE.Favorite,
                },
                new EventCheckboxTblElement
                {
                    CheckBox = this.CheckUnfavoritesEvent,
                    Type = MyCommon.EVENTTYPE.Unfavorite,
                },
                new EventCheckboxTblElement
                {
                    CheckBox = this.CheckFollowEvent,
                    Type = MyCommon.EVENTTYPE.Follow,
                },
                new EventCheckboxTblElement
                {
                    CheckBox = this.CheckListMemberAddedEvent,
                    Type = MyCommon.EVENTTYPE.ListMemberAdded,
                },
                new EventCheckboxTblElement
                {
                    CheckBox = this.CheckListMemberRemovedEvent,
                    Type = MyCommon.EVENTTYPE.ListMemberRemoved,
                },
                new EventCheckboxTblElement
                {
                    CheckBox = this.CheckBlockEvent,
                    Type = MyCommon.EVENTTYPE.Block,
                },
                new EventCheckboxTblElement
                {
                    CheckBox = this.CheckUserUpdateEvent,
                    Type = MyCommon.EVENTTYPE.UserUpdate,
                },
                new EventCheckboxTblElement
                {
                    CheckBox = this.CheckListCreatedEvent,
                    Type = MyCommon.EVENTTYPE.ListCreated,
                },
            };
        }

        private void GetEventNotifyFlag(ref MyCommon.EVENTTYPE eventnotifyflag, ref MyCommon.EVENTTYPE isMyeventnotifyflag)
        {
            MyCommon.EVENTTYPE evt = MyCommon.EVENTTYPE.None;
            MyCommon.EVENTTYPE myevt = MyCommon.EVENTTYPE.None;

            foreach (EventCheckboxTblElement tbl in GetEventCheckboxTable())
            {
                switch (tbl.CheckBox.CheckState)
                {
                    case CheckState.Checked:
                        evt = evt | tbl.Type;
                        myevt = myevt | tbl.Type;
                        break;
                    case CheckState.Indeterminate:
                        evt = evt | tbl.Type;
                        break;
                    case CheckState.Unchecked:
                        break;
                }
            }
            eventnotifyflag = evt;
            isMyeventnotifyflag = myevt;
        }

        private void ApplyEventNotifyFlag(bool rootEnabled, MyCommon.EVENTTYPE eventnotifyflag, MyCommon.EVENTTYPE isMyeventnotifyflag)
        {
            MyCommon.EVENTTYPE evt = eventnotifyflag;
            MyCommon.EVENTTYPE myevt = isMyeventnotifyflag;

            this.CheckEventNotify.Checked = rootEnabled;

            foreach (EventCheckboxTblElement tbl in GetEventCheckboxTable())
            {
                if ((evt & tbl.Type) != 0)
                {
                    if ((myevt & tbl.Type) != 0)
                    {
                        tbl.CheckBox.CheckState = CheckState.Checked;
                    }
                    else
                    {
                        tbl.CheckBox.CheckState = CheckState.Indeterminate;
                    }
                }
                else
                {
                    tbl.CheckBox.CheckState = CheckState.Unchecked;
                }
                tbl.CheckBox.Enabled = rootEnabled;
            }
        }

        private void CheckEventNotify_CheckedChanged(object sender, EventArgs e)
        {
            foreach (var tbl in this.GetEventCheckboxTable())
            {
                tbl.CheckBox.Enabled = this.CheckEventNotify.Checked;
            }
        }
    }
}
