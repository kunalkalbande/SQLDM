using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Views.Servers.ServerGroup
{
    public partial class ServerGroupHeatmapView : UserControl
    {
        public ServerGroupHeatmapView()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            string appDir = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            string heatmapFile = appDir + @"\Views\Servers\ServerGroup\HeatmapSupportFiles\Heatmap.html";
            this.webBrowser1.Navigate("file:///" + heatmapFile);
            base.OnLoad(e);
        }
    }
}
