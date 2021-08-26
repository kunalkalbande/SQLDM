using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BBS.TracerX;
using Idera.Newsfeed.Plugins.UI.Dialogs;
using Idera.SQLdm.Common.Events.AzureMonitor;
using Idera.SQLdm.Common.Events.AzureMonitor.Interfaces;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;

namespace Idera.SQLdm.DesktopClient.Dialogs.AzureConfigurations
{
    public partial class AddEditAzureAppProfile : BaseDialog
    {
        private static readonly Logger Log = Logger.GetLogger("AzureProfilesConfiguration");
        private readonly IAzureApplicationProfile inputAppProfile;

        private readonly Dictionary<string, IAzureApplicationProfile> appProfilesMap;

        //private readonly IAzureApplicationProfile _workingApplicationProfile;
        private List<IAzureSubscription> subscriptions;
        private List<IAzureApplication> applications;

        public AddEditAzureAppProfile(IAzureApplicationProfile applicationProfile, Dictionary<string, IAzureApplicationProfile> appProfilesMap)
        {

            inputAppProfile = applicationProfile;
            this.appProfilesMap = appProfilesMap;
            InitializeComponent();
        }

        private void AddEditAzureAppProfile_Load(object sender, EventArgs e)
        {
            // Load applications and subscriptions
            subscriptions = RepositoryHelper.GetAzureSubscriptions(Settings.Default.ActiveRepositoryConnection.ConnectionInfo) ??
                            new List<IAzureSubscription>();
            applications = RepositoryHelper.GetAzureApplications(Settings.Default.ActiveRepositoryConnection.ConnectionInfo) ??
                           new List<IAzureApplication>();

            subscriptionComboBox.DataSource = subscriptions;
            subscriptionComboBox.DisplayMember = "SubscriptionId";

            comboBoxApp.DataSource = applications;
            comboBoxApp.DisplayMember = "Name";

            if (inputAppProfile != null)
            {
                // Set data sources for subscription and application
                subscriptionComboBox.SelectedItem =
                    subscriptions.FirstOrDefault(s =>
                        s.SubscriptionId == inputAppProfile.Subscription.SubscriptionId) ??
                    subscriptionComboBox.SelectedItem;
                comboBoxApp.SelectedItem = applications.FirstOrDefault(a =>
                                               a.Name == inputAppProfile.Application.Name) ??
                                           comboBoxApp.SelectedItem;
                appProfileBindingSource.DataSource = inputAppProfile;
            }

            nameTextBox.Focus();
            UpdateUiControls();
        }

        private void btnAddSubs_Click(object sender, EventArgs e)
        {
            var subscriptionDialog = new AddEditAzureSubscription(null, subscriptions);

            var result = subscriptionDialog.ShowDialog(this);

            // Add the new vCenter server to the list
            if (DialogResult.OK == result)
            {
                LoadSubscriptionValues();
            }

            UpdateUiControls();
        }

        private void LoadSubscriptionValues()
        {
            subscriptions =
                RepositoryHelper.GetAzureSubscriptions(Settings.Default.ActiveRepositoryConnection.ConnectionInfo) ??
                new List<IAzureSubscription>();
                subscriptionComboBox.DataSource = subscriptions;
                subscriptionComboBox.DisplayMember = "SubscriptionId";

            if (subscriptions.Count == 0)
            {
                subscriptionComboBox.Text = string.Empty;
            }
            // SQLDM - 30699 Edit and Delete buttons are not disabled in Azure Application Profile when no data is there.
            UpdateUiControls();
        }
        private void LoadApplicationValues()
        {
            applications =
                RepositoryHelper.GetAzureApplications(Settings.Default.ActiveRepositoryConnection.ConnectionInfo) ??
                new List<IAzureApplication>();
            comboBoxApp.DataSource = applications;
            comboBoxApp.DisplayMember = "Name";
            if(applications.Count == 0)
            {
                comboBoxApp.Text = string.Empty;
            }
            // SQLDM - 30699 Edit and Delete buttons are not disabled in Azure Application Profile when no data is there.
            UpdateUiControls();
        }
        private void btnEditSubs_Click(object sender, EventArgs e)
        {
            // get the app profile being edited
            var subscription = (AzureSubscription) subscriptionComboBox.SelectedValue;
            if (subscription == null)
            {
                return;
            }
            var subscriptionDialog = new AddEditAzureSubscription(subscription, subscriptions);

            var result = subscriptionDialog.ShowDialog(this);

            // Add the new vCenter server to the list
            if (DialogResult.OK == result)
            {
                LoadSubscriptionValues();
                var activeSubscription = subscriptions.FirstOrDefault(s => s.Id == subscription.Id);
                if(activeSubscription != null)
                {
                    subscriptionComboBox.SelectedItem = activeSubscription;
                }
            }
        }
        private void btnDeleteSubs_Click(object sender, EventArgs e)
        {
            // get the app profile being edited
            var subscription = (AzureSubscription)subscriptionComboBox.SelectedValue;
            var message = new StringBuilder();
            message.AppendFormat("Deleting Azure Subscription {0} will remove all the associated profiles.", subscription.SubscriptionId);
            message.Append("\r\n\r\nAre you sure you want to delete this Azure Subscription?");

            if (DialogResult.Yes == ApplicationMessageBox.ShowQuestion(this, message.ToString(),
                    Microsoft.SqlServer.MessageBox.ExceptionMessageBoxButtons.YesNo))
            {
                if (String.IsNullOrEmpty(subscriptionComboBox.Text))
                    return;
                String subscriptionId = subscriptionComboBox.Text;
              RepositoryHelper.DeleteAzureSubscriptions(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, subscriptionId);
            }

            LoadSubscriptionValues();
        }

        private void buttonAddApp_Click(object sender, EventArgs e)
        {
            var application = new AzureApplication();
            var applicationDialog = new AddEditAzureApplication(application, applications);

            var result = applicationDialog.ShowDialog(this);

           
            // Add the new vCenter server to the list
            if (DialogResult.OK == result)
            {
                LoadApplicationValues();

            }
        }

        private void buttonEditApp_Click(object sender, EventArgs e)
        {
            var application = (AzureApplication)comboBoxApp.SelectedValue;
            if (application == null)
            {
                return;
            }
            var applicationDialog = new AddEditAzureApplication(application, applications);

            var result = applicationDialog.ShowDialog(this);

            // Add the new vCenter server to the list
            if (DialogResult.OK == result)
            { 
                LoadApplicationValues();
            }
        }

        private void buttonDelApp_Click(object sender, EventArgs e)
        {
            // get the app profile being edited
            var application = (AzureApplication)comboBoxApp.SelectedValue;
            var message = new StringBuilder();
            message.AppendFormat("Deleting Azure application {0} will remove all the associated profiles.", application.Name);
            message.Append("\r\n\r\nAre you sure you want to delete this Azure Application?");

            if (DialogResult.Yes == ApplicationMessageBox.ShowQuestion(this, message.ToString(),
                    Microsoft.SqlServer.MessageBox.ExceptionMessageBoxButtons.YesNo))
            {
                RepositoryHelper.DeleteAzureApplication(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, application.Id);
                LoadApplicationValues();
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            var name = nameTextBox.Text;
            var desc = descriptionTextBox.Text;
            var azureAppProfileId = inputAppProfile != null ? inputAppProfile.Id : (long?)null;
            var subscription = (IAzureSubscription)(subscriptionComboBox.SelectedValue);
            var application = (IAzureApplication)comboBoxApp.SelectedValue;

            long newAzureAppProfileId;
            RepositoryHelper.InsertUpdateAzureAppProfile(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                name, desc,
                subscription.Id, application.Id, azureAppProfileId, out newAzureAppProfileId);

            // Load resources
            try
            {
                var appProfile = new AzureApplicationProfile
                {
                    Name = name,
                    Application = application,
                    Description = desc,
                    Id = newAzureAppProfileId,
                    Subscription = subscription
                };
                var configuration = new MonitorManagementConfiguration
                {
                    Profile = new AzureProfile
                    {
                        ApplicationProfile = appProfile
                    },
                    MonitorParameters = new AzureMonitorParameters()
                };

                var connectionInfo = Settings.Default.ActiveRepositoryConnection.ConnectionInfo;
                var managementService = ManagementServiceHelper.GetDefaultService(connectionInfo);
                var resources = managementService.GetAzureApplicationResources(configuration);
                RepositoryHelper.UpdateAzureResources(connectionInfo, appProfile, resources);
            }
            catch (Exception exception)
            {
                Log.Error("Unable to connect to the remote server.", exception);
            }
        }

        private void azureAppProfileValidator_Validating(object sender, Infragistics.Win.Misc.ValidatingEventArgs e)
        {
            if (e.ValidationSettings.ValidationGroup == null)
                return;

            var value = e.Value == null ? null : e.Value.ToString().Trim();

            switch (e.ValidationSettings.ValidationGroup.Key)
            {
                case "appProfileNameValidationGroup":
                    if (string.IsNullOrEmpty(value))
                    {
                        e.IsValid = false;
                        break;
                    }

                    if (appProfilesMap.ContainsKey(value) &&
                        (inputAppProfile == null || inputAppProfile.Name != value))
                    {
                        e.IsValid = false;
                        e.ValidationSettings.NotificationSettings.Text =
                            "The application profile name must be unique. This application profile name is already present.";
                        break;
                    }
                    e.IsValid = true;
                    break;
                case "appProfileDetailsValidationGroup":
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

        private void nameTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateUiControls();
        }

        private void UpdateUiControls()
        {
            btnEditSubs.Enabled = btnDeleteSubs.Enabled = subscriptionComboBox.Items.Count > 0;
            buttonEditApp.Enabled = buttonDelApp.Enabled = comboBoxApp.Items.Count > 0;
            this.btnOK.Enabled = this.azureAppProfileValidator.Validate(true, false).IsValid;
        }

        private void subscriptionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateUiControls();
        }

        private void comboBoxApp_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateUiControls();
        }
    }
}
