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
    public partial class FailureIsCriticalOptionDialog : BaseDialog
    {
        public FailureIsCriticalOptionDialog()
        {
            this.DialogHeader = "Generate Alert";
            InitializeComponent();

            this.AdaptFontSize();
        }

		/// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }

        public bool GenerateAlert
        {
            get { return generateAlertButton.Checked; }
            set
            {
                generateAlertButton.Checked = value;
                skipAlertButton.Checked = !value;
            }
        }
    }
}