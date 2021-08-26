using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Idera.SQLdm.Common.Events.AzureMonitor;
using Idera.SQLdm.Common.Events.AzureMonitor.Interfaces;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;

namespace Idera.SQLdm.DesktopClient.Dialogs.AzureConfigurations
{
    public partial class AddEditAzureApplication : BaseDialog
    {
        #region private properties
        private readonly AzureApplication azureApplication;
        private readonly List<IAzureApplication> applications;
        #endregion

        #region Constructor
        public AddEditAzureApplication(AzureApplication application, List<IAzureApplication> applications)
        {
            InitializeComponent();
            this.applications = applications;
            azureApplication = application;
            SetDataInText();
        }
        #endregion

        #region control methods
        private void btnOK_Click(object sender, EventArgs e)
        {
            GetDataFromText();
            long tagId = RepositoryHelper.AddOrUpdateAzureApplication(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ConnectionString, azureApplication);
            azureApplication.Id = tagId;
            btnOK.Enabled = false;
        }
        #endregion

        #region private methods
        private void SetDataInText()
        {
            textBoxClientId.Text = azureApplication.ClientId;
            textBoxDescription.Text = azureApplication.Description;
            textBoxName.Text = azureApplication.Name;
            textBoxSecret.Text = azureApplication.Secret;
            textBoxTenantId.Text = azureApplication.TenantId;
            btnOK.Enabled = true;
        }
        private void GetDataFromText()
        {
            azureApplication.TenantId = textBoxTenantId.Text;
            azureApplication.Secret = textBoxSecret.Text;
            azureApplication.Name = textBoxName.Text;
            azureApplication.ClientId = textBoxClientId.Text;
            azureApplication.Description = textBoxDescription.Text;
        }
        #endregion

        private void azureApplicationValidator_Validating(object sender, Infragistics.Win.Misc.ValidatingEventArgs e)
        {
            if (e.ValidationSettings.ValidationGroup == null)
                return;

            var value = e.Value == null ? null : e.Value.ToString().Trim();

            switch (e.ValidationSettings.ValidationGroup.Key)
            {
                case "applicationNameValidationGroup":
                    if (string.IsNullOrEmpty(value))
                    {
                        e.IsValid = false;
                        break;
                    }

                    var applicationName = textBoxName.Text;
                    if (applicationName != null && applications.Any(app => app.Name == applicationName) &&
                        (azureApplication == null || azureApplication.Name != applicationName))
                    {
                        e.IsValid = false;
                        e.ValidationSettings.NotificationSettings.Text =
                            "The application name must be unique. This application name is already present.";
                        break;
                    }

                    e.IsValid = true;
                    break;
                case "applicationDetailsValidationGroup":
                    if (string.IsNullOrEmpty(value))
                    {
                        e.IsValid = false;
                        break;
                    }
                    e.IsValid = true;
                    break;
                default:
                    e.IsValid = true;
                    break;
            }
        }

        private void textBoxName_TextChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void textBoxTenantId_TextChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void textBoxClientId_TextChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void textBoxSecret_TextChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void AddEditAzureApplication_Load(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void UpdateControls()
        {
            btnOK.Enabled = azureApplicationValidator.Validate(true, false).IsValid;
        }
    }
}
