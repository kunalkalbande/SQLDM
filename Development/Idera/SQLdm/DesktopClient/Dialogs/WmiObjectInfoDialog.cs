using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Idera.SQLdm.DesktopClient.Helpers;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    public partial class WmiObjectInfoDialog : BaseDialog
    {
        public WmiObjectInfoDialog(string objectName, string objectDesc, string counterName, string counterDesc)
        {
            this.DialogHeader = "Wmi Object Info";
            InitializeComponent();
            this.AdaptFontSize();

            this.objectNameLabel.Text = objectName;
            this.objectDescriptionLabel.Text = objectDesc;
            if (!String.IsNullOrEmpty(counterName))
            {
                this.counterNameLabel.Text = counterName;
                this.counterDescriptionLabel.Text = counterDesc;
            } else
            {
                this.Height -= 91 ;
                splitContainer1.Panel2Collapsed = true;                
            }
        }

		/// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Dispose();
        }
    }
}