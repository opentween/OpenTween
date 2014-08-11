using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenTween
{
    public partial class EditNgDialog : OTBaseForm
    {
        private TabClass _tabClass = null;
        public TabClass TabClass
        {
            get
            {
                return this._tabClass;
            }
            set
            {
                this._tabClass = value;
                if (value != null)
                {
                    this.dataGridViewScreenName.DataSource = createDataTableFromList(value.SearchNgScreenNames);
                    this.dataGridViewWord.DataSource = createDataTableFromList(value.SearchNgWords);
                    this.dataGridViewSource.DataSource = createDataTableFromList(value.SearchNgSources);
                }
            }
        }

        public EditNgDialog()
        {
            InitializeComponent();

            this.TabClass = null;
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            if (this.TabClass != null)
            {
                this.TabClass.SearchNgScreenNames = createListFromDataTable((DataTable)this.dataGridViewScreenName.DataSource);
                this.TabClass.SearchNgWords = createListFromDataTable((DataTable)this.dataGridViewWord.DataSource);
                this.TabClass.SearchNgSources = createListFromDataTable((DataTable)this.dataGridViewSource.DataSource);

                this.DialogResult = DialogResult.OK;
            }
            else
            {
                this.DialogResult = DialogResult.Cancel;
            }
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private DataTable createDataTableFromList(List<string> list)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("text");

            if (list != null)
            {
                list.ForEach(delegate(string text)
                {
                    dt.Rows.Add(text);
                });
            }

            return dt;
        }

        private List<string> createListFromDataTable(DataTable dt)
        {
            List<string> list = new List<string>();

            foreach (DataRow row in dt.Rows)
            {
                // 重複チェック
                if (!list.Contains((string)row[0]))
                {
                    list.Add((string)row[0]);
                }
            }

            return list;
        }
    }
}
