using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    public partial class ReportInstructionsControl : UserControl
    {
        public ReportInstructionsControl()
        {
            InitializeComponent();
        }

        public string ReportTitle
        {
            get { return reportTitleLabel.Text; }
            set { reportTitleLabel.Text = value; }
        }

        public string ReportDescription
        {
            get { return reportDescriptionLabel.Text; }
            set { reportDescriptionLabel.Text = value; }
        }
        [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public string ReportInstructions
        {
            get { return reportInstructionsLabel.Text; }
            set { reportInstructionsLabel.Text = value; }
        }
    }
}
