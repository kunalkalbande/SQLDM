using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Idera.SQLdm.Common.Events.AzureMonitor;
using Idera.SQLdm.Common.Events.AzureMonitor.Interfaces;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;
using Idera.SQLdm.Common.Data;
using Idera.SQLdm.DesktopClient.Dialogs.AzureConfigurations;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    public partial class AzureDiscoveryDialog : Form
    {
        #region private properties
        private readonly String _loginName;
        private readonly String _password;
        private readonly string _azureApplicationName;
        private TextItem<IAzureApplicationProfile> selectedProfile;
        private readonly String SELECT_A_PROFILE = "Select a Profile";
        #endregion

        #region Constructor
        public AzureDiscoveryDialog(String LoginName, String Password, string azureApplicationName)
        {
            _loginName = LoginName;
            _password = Password;
            _azureApplicationName = azureApplicationName;
            InitializeComponent();
            textBoxPassword.Text = _password;
            textBoxLoginName.Text = _loginName;
            BindAzureProfiles();
        }
        #endregion

        private void btnOK_Click(object sender, EventArgs e)
        {
            btnOK.Enabled = false;
        }

        private void AzureDiscoveryDialog_Load(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void UpdateControls()
        {
            var selectedItem = azureProfileComboBox.SelectedItem as TextItem<IAzureApplicationProfile>;
            btnOK.Enabled = !((selectedItem == null) && azureProfileComboBox.SelectedItem.Equals(SELECT_A_PROFILE));
        }

        private void azureProfileComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateControls();
            selectedProfile = azureProfileComboBox.SelectedItem as TextItem<IAzureApplicationProfile>;

        }

        private void addAzureProfileButton_Click(object sender, EventArgs e)
        {
            var AzureDiscoveryDialog = new AzureProfilesConfiguration(false);
            AzureDiscoveryDialog.ShowDialog(this);
            BindAzureProfiles(); 
        }

        private void BindAzureProfiles()
        {
        	// SQLDM-30785 Not able to select an Azure Profile in add server wizard.
            var azureProfiles = RepositoryHelper.GetAzureApplicationProfiles(Settings.Default.ActiveRepositoryConnection.ConnectionInfo).ToList();
            azureProfileComboBox.Items.Clear();
            azureProfiles.Sort(
                (p1, p2) => string.CompareOrdinal(p1.Name, p2.Name));
            foreach (var azureProfile in azureProfiles)
            {
                var item = new TextItem<IAzureApplicationProfile>
                {
                    Text = azureProfile.Name,
                    Value = azureProfile
                };
                azureProfileComboBox.Items.Add(item);
            }

            if (selectedProfile == null)
            {
                var index = azureProfileComboBox.Items.IndexOf(SELECT_A_PROFILE);
                if (index == -1)
                {
                    azureProfileComboBox.Items.Insert(0, SELECT_A_PROFILE);
                    azureProfileComboBox.SelectedIndex = 0;
                }
                else
                {
                    azureProfileComboBox.SelectedIndex = index;
                }

                if (!string.IsNullOrEmpty(_azureApplicationName))
                {
                    foreach (var item in azureProfileComboBox.Items)
                    {
                        if (!(item is TextItem<IAzureApplicationProfile>))
                        {
                            continue;
                        }
                        var appProfile = (TextItem<IAzureApplicationProfile>) item;
                        if (!_azureApplicationName.Equals(appProfile.Text))
                        {
                            continue;
                        }
                        azureProfileComboBox.SelectedItem = item;
                        break;
                    }
                }
            }
            else
            {
                foreach (var item in azureProfileComboBox.Items)
                {
                    if (item.ToString().Equals(SELECT_A_PROFILE) || !item.ToString()
                            .Equals(selectedProfile.Text, StringComparison.InvariantCultureIgnoreCase))
                    {
                        continue;
                    }

                    azureProfileComboBox.SelectedItem = item;
                    break;
                }
            }
            UpdateControls();
        }

        public TextItem<IAzureApplicationProfile> getSelectedProfile()
        {
            return azureProfileComboBox.SelectedItem as TextItem<IAzureApplicationProfile>;
        }


    }
}
