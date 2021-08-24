using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SQLdmCWFInstaller
{
    public partial class MessagePopupDialog : Form
    {
        /// <summary>
        /// /// SQLdm 9.0 (Ankit Srivastava) - CWF Installer Wrapper - Created dialog for displaying message
        /// </summary> 
        /// <param name="caption"></param>
        /// <param name="message"></param>
        public MessagePopupDialog(string caption,string message,string okText,string cancelText)
        {
            InitializeComponent();

            this.Text = caption;
            lblMessage.Text = message;
            btnOk.Text = okText;
            btnCancel.Text = cancelText;
        }

        /// <summary>
        /// This method opens the Popup form and positions it according to the parent
        /// </summary>
        /// <param name="parent"></param>
        internal void OpenForm(Form parent)
        {
            this.StartPosition = FormStartPosition.CenterParent;            
            this.ShowDialog();
        }

        /// <summary>
        /// This method is called on OK button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// This method is called on Cancel button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
