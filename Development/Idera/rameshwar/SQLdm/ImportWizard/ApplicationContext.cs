using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;

using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.ImportWizard.Properties;

namespace Idera.SQLdm.ImportWizard
{
    internal class ApplicationContext : System.Windows.Forms.ApplicationContext
    {
        #region ctors
        public ApplicationContext(
                string hostIn,
                string dbIn,
                string userIn,
                string pwdIn
            )
        {
            // Set the main form to the Wizard.
            MainForm = (Form)new WizardMainForm(hostIn, dbIn, userIn, pwdIn);
        }
        #endregion

        #region methods

        protected override void OnMainFormClosed(object sender, EventArgs e)
        {
            // Process based on who the sender is.
            if (sender is WizardMainForm)
            {
                WizardMainForm wmf = sender as WizardMainForm;
                if (wmf.DialogResult == DialogResult.OK)
                {
                    MainForm = (Form)new ImportStatusForm(wmf.RepositoryInfo, wmf.SelectedImportDate,
                                                                wmf.SelectedServers);
                    MainForm.Show();
                }
                else
                {
                    base.OnMainFormClosed(sender, e);
                }
            }
            else
            {
                base.OnMainFormClosed(sender, e);
            }
            Settings.Default.Save();
        }

        #endregion
    }
}
