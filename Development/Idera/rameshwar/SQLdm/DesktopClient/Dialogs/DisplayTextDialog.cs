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
    public partial class DisplayTextDialog : Form
    {
        #region Constructors

        public DisplayTextDialog(string caption, string text, bool readOnly)
        {
            InitializeComponent();

            Text = caption;

            textBox.Text = text;
            textBox.ReadOnly = readOnly;
            textBox.SelectionStart = 0;
            textBox.SelectionLength = 0;
            copySelectedButton.Enabled = false;
            AdaptFontSize();
        }

        #endregion

        #region Events

        private void copyAllButton_Click(object sender, EventArgs e)
        {
            DataObject dataObject = new DataObject();
            dataObject.SetText(textBox.Text);
            Clipboard.SetDataObject(dataObject, true);
        }

        private void copySelectedButton_Click(object sender, EventArgs e)
        {
            DataObject dataObject = new DataObject();
            dataObject.SetText(textBox.SelectedText);
            Clipboard.SetDataObject(dataObject, true);
        }

        private void textBox_MouseUp(object sender, MouseEventArgs e)
        {
            copySelectedButton.Enabled = textBox.SelectionLength > 0;
        }

        private void textBox_KeyUp(object sender, KeyEventArgs e)
        {
            copySelectedButton.Enabled = textBox.SelectionLength > 0;
        }

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }

        #endregion
    }
}