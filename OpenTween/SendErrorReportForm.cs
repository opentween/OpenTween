// OpenTween - Client of Twitter
// Copyright (c) 2015 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTween.Api.DataModel;

namespace OpenTween
{
    public partial class SendErrorReportForm : OTBaseForm
    {
        public ErrorReport ErrorReport
        {
            get { return this._errorReport; }
            set
            {
                this._errorReport = value;
                this.bindingSource.DataSource = value;
            }
        }
        private ErrorReport _errorReport;

        public SendErrorReportForm()
        {
            this.InitializeComponent();
        }

        private void SendErrorReportForm_Shown(object sender, EventArgs e)
        {
            this.pictureBoxIcon.Image = SystemIcons.Error.ToBitmap();
            this.textBoxErrorReport.DeselectAll();
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            this.ErrorReport.Reset();
        }

        private async void buttonSendByMail_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            await this.ErrorReport.SendByMailAsync();
        }

        private async void buttonSendByDM_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;

            try
            {
                await this.ErrorReport.SendByDmAsync();

                MessageBox.Show(Properties.Resources.SendErrorReport_DmSendCompleted, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (WebApiException ex)
            {
                var message = Properties.Resources.SendErrorReport_DmSendError + Environment.NewLine + ex.Message;
                MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonNotSend_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }

    public class ErrorReport : NotifyPropertyChangedBase
    {
        public string ReportText
        {
            get { return this._reportText; }
            set
            {
                this.SetProperty(ref this._reportText, value);
                this.UpdateEncodedReport();
            }
        }
        private string _reportText;

        public bool AnonymousReport
        {
            get { return this._anonymousReport; }
            set
            {
                this.SetProperty(ref this._anonymousReport, value);
                this.UpdateEncodedReport();
            }
        }
        private bool _anonymousReport = true;

        public bool CanSendByDM
        {
            get { return this._canSendByDm; }
            private set { this.SetProperty(ref this._canSendByDm, value); }
        }
        private bool _canSendByDm;

        public string EncodedReportForDM
        {
            get { return this._encodedReportForDM; }
            private set { this.SetProperty(ref this._encodedReportForDM, value); }
        }
        private string _encodedReportForDM;

        private readonly Twitter tw;
        private readonly string originalReportText;

        public ErrorReport(string reportText)
            : this(null, reportText)
        {
        }

        public ErrorReport(Twitter tw, string reportText)
        {
            this.tw = tw;
            this.originalReportText = reportText;

            this.Reset();
        }

        public void Reset()
        {
            this.ReportText = this.originalReportText;
        }

        public async Task SendByMailAsync()
        {
            var toAddress = ApplicationSettings.FeedbackEmailAddress;
            var subject = $"{Application.ProductName} {MyCommon.GetReadableVersion()} エラーログ";
            var body = this.ReportText;

            var mailto = $"mailto:{Uri.EscapeDataString(toAddress)}?subject={Uri.EscapeDataString(subject)}&body={Uri.EscapeDataString(body)}";
            await Task.Run(() => Process.Start(mailto));
        }

        public async Task SendByDmAsync()
        {
            await Task.Run(() => this.tw.SendDirectMessage(this.EncodedReportForDM));
        }

        private void UpdateEncodedReport()
        {
            if (!this.CheckDmAvailable())
            {
                this.CanSendByDM = false;
                return;
            }

            var body = $"Anonymous: {this.AnonymousReport}" + Environment.NewLine + this.ReportText;
            var originalBytes = Encoding.UTF8.GetBytes(body);

            using (var outputStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(outputStream, CompressionMode.Compress, leaveOpen: true))
                {
                    gzipStream.Write(originalBytes, 0, originalBytes.Length);
                }

                var encodedReport = Convert.ToBase64String(outputStream.ToArray());
                var destScreenName = ApplicationSettings.FeedbackTwitterName.Substring(1);
                this.EncodedReportForDM = $"D {destScreenName} ErrorReport: {encodedReport}";
            }

            this.CanSendByDM = this.tw.GetTextLengthRemain(this.EncodedReportForDM) >= 0;
        }

        private bool CheckDmAvailable()
        {
            if (!ApplicationSettings.AllowSendErrorReportByDM)
                return false;

            if (this.tw == null || !this.tw.AccessLevel.HasFlag(TwitterApiAccessLevel.DirectMessage))
                return false;

            if (Twitter.AccountState != MyCommon.ACCOUNT_STATE.Valid)
                return false;

            return true;
        }
    }
}
