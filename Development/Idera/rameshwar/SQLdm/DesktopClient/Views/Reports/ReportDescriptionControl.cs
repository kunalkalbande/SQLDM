using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Views.Reports
{
    public partial class ReportDescriptionControl : UserControl
    {
        public ReportDescriptionControl()
        {
            InitializeComponent();
        }

        public string TitleText
        {
            get { return headerLabel.Text; }
            set { headerLabel.Text = value; }
        }

        public string DescriptionText
        {
            get { return reportDescriptionLabel.Text; }
            set { reportDescriptionLabel.Text = value; }
        }

        public string GettingStartedText
        {
            get { return gettingStartedLabel.Text; }
            set { gettingStartedLabel.Text = value; }
        }
    }
}