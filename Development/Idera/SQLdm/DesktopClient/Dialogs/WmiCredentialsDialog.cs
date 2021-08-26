using System;
using System.Windows.Forms;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.DesktopClient.Helpers;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    public partial class WmiCredentialsDialog : BaseDialog
    {
        private readonly WmiConfiguration wmiConfiguration;
        public WmiCredentialsDialog(WmiConfiguration wmiConfiguration)
        {
            this.DialogHeader = "WMI Credentials";
            InitializeComponent();
            this.AdaptFontSize();
            this.wmiConfiguration = wmiConfiguration;
        }

		/// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            wmiConfiguration.OleAutomationDisabled = true;
            wmiConfiguration.DirectWmiEnabled = true;
            wmiConfiguration.DirectWmiConnectAsCollectionService = false;
            wmiConfiguration.DirectWmiUserName = txtHostUser.Text;
            wmiConfiguration.DirectWmiPassword = txtHostPassword.Text;
        }

        private void OnTextChanged(object sender, EventArgs e)
        {
            btnOK.Enabled = !String.IsNullOrEmpty(txtHostUser.Text) && !String.IsNullOrEmpty(txtHostPassword.Text);
        }
    }
}
