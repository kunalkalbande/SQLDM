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
    using Idera.SQLdm.Common.Snapshots;

    public partial class FileSizeDialog : Form
    {
        private FileSize fileSize;

        public FileSizeDialog()
        {
            FileSize = new FileSize();
            InitializeComponent();
            AdaptFontSize();
        }

        private void internal_SetFileSize(FileSize value)
        {
            if (fileSize == null)
            {
                fileSize = new FileSize();
            }

            value.Kilobytes = value.Kilobytes;

            // set a default value if necessary
            if (fileSize.Kilobytes == null || fileSize.Kilobytes.Value == 0)
                fileSize.Pages = 25;

            if (numericUpDown1 != null && numericUpDown1.Created)
                numericUpDown1.Value = fileSize.Kilobytes.Value; 
        }

        public FileSize FileSize
        {
            get
            {
                return fileSize;
            }
            set
            {
                internal_SetFileSize(value);
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            fileSize.Kilobytes = numericUpDown1.Value;
        }

        private void FileSizeDialog_Load(object sender, EventArgs e)
        {
            numericUpDown1.Value = fileSize.Kilobytes.Value;
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