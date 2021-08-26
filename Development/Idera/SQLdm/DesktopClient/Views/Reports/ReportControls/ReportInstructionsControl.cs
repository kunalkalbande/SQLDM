using Idera.SQLdm.DesktopClient.Properties;
using Infragistics.Windows.Themes;
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    public partial class ReportInstructionsControl : UserControl
    {
        public ReportInstructionsControl()
        {
            InitializeComponent();
            SetThemeImages(); //DarkTheme 4.12 Icons Switching : Babita Manral
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);  //DarkTheme 4.12 Icons Switching : Babita Manral
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
       // DarkTheme 4.12 Icons Switching : Babita Manral
        void OnCurrentThemeChanged(object sender, EventArgs e)
        {
            SetThemeImages();
        }
        //DarkTheme 4.12 Icons Switching : Babita Manral
        private void SetThemeImages()
        {
            if (Settings.Default.ColorScheme == "Dark")
            {
                this.pictureBox1.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.darkTheme_SQLdmLogoHeader;
                this.pictureBox1.BackColor = ColorTranslator.FromHtml(DarkThemeColorConstants.ReportColor);
            }
            else
            {
                this.pictureBox1.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.SQLdmLogoHeader;
                this.pictureBox1.BackColor = Color.Transparent;
            }
        }
    }
}
