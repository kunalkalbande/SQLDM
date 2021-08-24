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
    public partial class SQLAUTHDM : Form
    {
        RepositoryDetailsDM backScreen;
        bool isSQLAuthForManagementService = false;
        public SQLAUTHDM(RepositoryDetailsDM backScreenObj, bool isSQLAuthForManagementSvc)
        {
            InitializeComponent();
            AcceptButton = buttonOk;
            CancelButton = buttonCancel;
            backScreen = backScreenObj;
            isSQLAuthForManagementService = isSQLAuthForManagementSvc;
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
            
            backScreen.StoreSQLAuth(textBoxUsername.Text, textBoxPassword.Text, isSQLAuthForManagementService);
            this.DialogResult = DialogResult.OK;
            this.Hide();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Hide();
            backScreen.disableCheckBox(isSQLAuthForManagementService);
            backScreen.Show();
        }

        private void SQLAUTH2_Load(object sender, EventArgs e)
        {
            if (isSQLAuthForManagementService)
            {
                textBoxUsername.Text = properties.SQLUsername_DM_Service;
                textBoxPassword.Text = properties.SQLPassword_DM_Service;
            }
            else
            {
                textBoxUsername.Text = properties.SQLUsername_DM_Repo;
                textBoxPassword.Text = properties.SQLPassword_DM_Repo;
            }
        }
    }
}
