using CWFInstallerService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Installer_form_application
{
    public partial class SQLAuthID : Form
    {
        RepositoryDetails backScreen;
        public SQLAuthID(RepositoryDetails backScreenObj)
        {
            InitializeComponent();
            AcceptButton = buttonOk;
            CancelButton = buttonCancel;
            backScreen = backScreenObj;
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            try
            {
                Validator.validateSqlAuthCredentials(textBoxUsername.Text, textBoxPassword.Text);
            }
            catch (CWFBaseException ex)
            {
                MessageBox.Show(ex.ErrorCode + " - " + ex.ErrorMessage, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Show();
                return;
            }
            
            backScreen.StoreSQLAuth(textBoxUsername.Text, textBoxPassword.Text);
            this.DialogResult = DialogResult.OK;
            this.Hide();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Hide();
            //backScreen.disableCheckBox();
            //backScreen.Show();
        }

        private void SQLAuthID_Load(object sender, EventArgs e)
        {
            textBoxUsername.Text = properties.SQLUsernameID;
            textBoxPassword.Text = properties.SQLPasswordID;
        }
    }
}
