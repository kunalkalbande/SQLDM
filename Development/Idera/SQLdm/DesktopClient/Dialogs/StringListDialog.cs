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
    public partial class StringListDialog : BaseDialog
    {
        public StringListDialog()
        {
            this.DialogHeader = "StringListDialog";
            InitializeComponent();
            AdaptFontSize();
        }

        public IEnumerable<string> Strings
        {
            get { return textBox1.Lines; }
            set
            {
                string[] lines = null;
                if (value is string[])
                    lines = (string[]) value;
                else 
                if (value is IList<string>)
                    lines = ((List<string>) value).ToArray();
                else
                    lines = new List<string>(value).ToArray();

                textBox1.Lines = lines;
            }
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