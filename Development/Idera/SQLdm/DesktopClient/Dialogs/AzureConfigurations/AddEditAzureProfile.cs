using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Idera.SQLdm.Common.Events.AzureMonitor;
using Idera.SQLdm.Common.Events.AzureMonitor.Interfaces;
using Idera.SQLdm.DesktopClient.Properties;
using Idera.SQLdm.DesktopClient.Helpers;
using System.Linq;

namespace Idera.SQLdm.DesktopClient.Dialogs.AzureConfigurations
{
    public partial class AddEditAzureProfile : Form
    {
        private readonly Dictionary<int, List<IAzureProfile>> profilesMap;
        private readonly AzureProfile azureProfile;
        private Dictionary<int, EntityData> sqlServers;
        private Dictionary<long, EntityData> appProfiles;

        public AddEditAzureProfile(AzureProfile profile, Dictionary<int, List<IAzureProfile>> profilesMap)
        {
            this.profilesMap = profilesMap;
            azureProfile = profile;
            InitializeComponent();
        }

        private void AddEditAzureProfile_Load(object sender, EventArgs e)
        {
            LoadDataSource();
            if (azureProfile == null)
            {
                return;
            }
            descriptionTextBox.Text = azureProfile.Description;
            if (sqlServers.ContainsKey(azureProfile.SqlServerId))
            {
                azureServerComboBox.SelectedItem = sqlServers[azureProfile.SqlServerId];
            }
            if (azureProfile.ApplicationProfile != null && appProfiles.ContainsKey(azureProfile.ApplicationProfile.Id))
            {
                comboBoxAppProfile.SelectedItem = appProfiles[azureProfile.ApplicationProfile.Id];
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            var inputProfile = new AzureProfile
            {
                Id = azureProfile != null ? azureProfile.Id : 0,
                Description = descriptionTextBox.Text,
                SqlServerId = (int)((EntityData)azureServerComboBox.SelectedItem).Id,
                ApplicationProfile = new AzureApplicationProfile
                {
                    Id = ((EntityData)comboBoxAppProfile.SelectedItem).Id
                }
            };
            RepositoryHelper.AddUpdateAzureLinkedProfile(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ConnectionString, inputProfile);
            btnOK.Enabled = false;
        }
        private void comboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            btnOK.Enabled = profileValidator.Validate(true, false).IsValid;
        }

        #region Private user defined methods and classes
        public class EntityData
        {
            private readonly long id;
            private readonly string name;

            public long Id
            {
                get { return id; }
            }

            public string Name
            {
                get { return name; }
            }

            public EntityData(long id, string name)
            {
                this.id = id;
                this.name = name;
            }
        }

        private void LoadDataSource()
        {
            // Load only azure sql servers
            sqlServers = ApplicationModel.Default.ActiveInstances
                .Where(s => s.Instance.CloudProviderId == Common.Constants.MicrosoftAzureId || s.Instance.CloudProviderId == Common.Constants.MicrosoftAzureManagedInstanceId)
                .OrderBy(s => s.DisplayInstanceName).ToDictionary(s => s.Id,
                    s => new EntityData(s.Id, s.DisplayInstanceName));

            // Load application profiles
            appProfiles = RepositoryHelper
                .GetAzureApplicationProfile(Settings.Default.ActiveRepositoryConnection.ConnectionInfo)
                .OrderBy(p => p.Value)
                .ToDictionary(d => d.Key, d => new EntityData(d.Key, d.Value));

            azureServerComboBox.DataSource = sqlServers.Values.ToList();
            azureServerComboBox.DisplayMember = "Name";

            comboBoxAppProfile.DataSource = appProfiles.Values.ToList();
            comboBoxAppProfile.DisplayMember = "Name";
        }

        #endregion

        private void profileValidator_Validating(object sender, Infragistics.Win.Misc.ValidatingEventArgs e)
        {
            if (e.ValidationSettings.ValidationGroup == null)
                return;

            switch (e.ValidationSettings.ValidationGroup.Key)
            {
                case "profileValidationGroup":
                    if (azureServerComboBox.SelectedValue == null ||
                        comboBoxAppProfile.SelectedValue == null)
                    {
                        e.IsValid = false;
                        break;
                    }

                    var sqlServerId = Convert.ToInt32(((EntityData) azureServerComboBox.SelectedValue).Id);
                    var appProfileId = ((EntityData)comboBoxAppProfile.SelectedValue).Id;

                    if (profilesMap.ContainsKey(sqlServerId) && profilesMap[sqlServerId].Any(p => p.ApplicationProfile.Id == appProfileId) &&
                            (azureProfile == null || azureProfile.ApplicationProfile == null || 
                             !(azureProfile.SqlServerId == sqlServerId && azureProfile.ApplicationProfile.Id == appProfileId)))
                    {
                        e.IsValid = false;
                        e.ValidationSettings.NotificationSettings.Text =
                            "The application profile name must be unique. This application profile name is already present.";
                        break;
                    }
                    e.IsValid = true;
                    break;
                default:
                    e.IsValid = true;
                    break;
            }
        }
    }
}
