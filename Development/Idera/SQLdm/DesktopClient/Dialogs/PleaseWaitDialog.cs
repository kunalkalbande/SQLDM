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
    public partial class PleaseWaitDialog : BaseDialog
    {
        private volatile Boolean closeForm;

        public delegate void SetMessageDelegate(string msg);
        public delegate void CloseFormDelegate(bool close);

        public PleaseWaitDialog()
        {
            InitializeComponent();

            message.Text = "Please wait...";
            AdaptFontSize();
        }

        public PleaseWaitDialog(string stsMsg)
        {
            this.DialogHeader = "Please Wait";
            InitializeComponent();

            message.Text = stsMsg;
            AdaptFontSize();
        }

        private void PleaseWaitDialog_Load(object sender, EventArgs e)
        {
            closeForm = false;
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (closeForm)
                Close();
        }

        public void SetMessage(string msg)
        {
            if (this.InvokeRequired)
            {
                SetMessageDelegate setMessageMethod = new SetMessageDelegate(this.SetMessage);
                this.Invoke(setMessageMethod, new object[] { msg });
            }
            else
                message.Text = msg;
        }

        public void CloseForm(bool close)
        {
            if (this.InvokeRequired)
            {
                CloseFormDelegate closeFormMethod = new CloseFormDelegate(this.CloseForm);
                this.Invoke(closeFormMethod, new object[] { close });
            }
            else
                closeForm = close;
        }

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }
    }
}
