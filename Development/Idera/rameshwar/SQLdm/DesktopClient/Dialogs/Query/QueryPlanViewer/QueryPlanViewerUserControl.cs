using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Dialogs.Query.QueryPlanViewer
{
    public partial class QueryPlanViewerUserControl : UserControl
    {
        public QueryPlanViewerUserControl()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        public void FillText(string plan)
        {
            this.textBox1.Text = plan;
        }
    }
}
