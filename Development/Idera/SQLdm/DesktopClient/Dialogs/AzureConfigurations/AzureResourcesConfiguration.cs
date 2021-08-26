using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using BBS.TracerX;
using Idera.SQLdm.Common.Events.AzureMonitor;
using Idera.SQLdm.Common.Events.AzureMonitor.Interfaces;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Controls;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;

namespace Idera.SQLdm.DesktopClient.Dialogs.AzureConfigurations
{
    public partial class AzureResourcesConfiguration : Form
    {
        private static readonly Logger Log = Logger.GetLogger("AzureResourcesConfiguration");
        private const string ResourceDetailsformat = "{0} - {1}";

        private readonly AzureProfile inputProfile;
        private readonly Dictionary<int, List<IAzureProfile>> profiles;
        private Dictionary<int, MonitoredSqlServer> servers;
        private AzureApplicationProfile loadResourceAppProfile;
        private const string ErrorConnectingFmt = "Error connecting using the azure application {0}:\r\n{1}";
        private const string ConnectToAzureFmt = "Connecting to the Azure Application {0}";

        public AzureResourcesConfiguration(AzureProfile profile, Dictionary<int, List<IAzureProfile>> profiles)
        {
            inputProfile = profile;
            this.profiles = profiles ?? new Dictionary<int, List<IAzureProfile>>();
            this.statusProgressBar = new InfiniteProgressBar();
            InitializeComponent();
            InitializeProgressBar();
        }
        private void InitializeProgressBar()
        {
            // Initialize the progress bar
            // 
            // statusProgressBar
            // 
            this.statusProgressBar = new InfiniteProgressBar
            {
                Color1 = System.Drawing.Color.FromArgb(255, 135,
                    45),
                Color2 = System.Drawing.Color.White,
                Dock = DockStyle.Top,
                Location = new System.Drawing.Point(0, 60),
                Name = "statusProgressBar",
                Size = new System.Drawing.Size(532, 3),
                Speed = 15D,
                Step = 5F,
                TabIndex = 9
            };
            this.Controls.Add(this.statusProgressBar);
        }
        private void AzureResourcesConfiguration_Load(object sender, EventArgs e)
        {
            updateStatusLabel.Text = string.Empty;
            // Populate the servers combobox
            servers = profiles.Select(p => p.Key)
                .Where(k => ApplicationModel.Default.AllInstances.ContainsKey(k))
                .Select(id => ApplicationModel.Default.AllInstances[id]).ToDictionary(s => s.Id, s => s);
            serverComboBox.DataSource = servers.Values.ToList();

            if (inputProfile != null && servers.ContainsKey(inputProfile.SqlServerId))
            {
                serverComboBox.SelectedItem = servers[inputProfile.SqlServerId];
                applicationProfileComboBox.SelectedItem = inputProfile.ApplicationProfile;
            }

            var appProfile = (AzureApplicationProfile)applicationProfileComboBox.SelectedItem;
            if (appProfile != null)
            {
                UpdateDataSource(appProfile.Resources);
            }

            UpdateUiControls();
        }

        private void UpdateDataSource(List<IAzureResource> resources)
        {
            azureResourcesGrid.DataSource = resources;
            labelDetails.Text = string.Format(ResourceDetailsformat, serverComboBox.Text, applicationProfileComboBox.Text);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void serverComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var box = sender as ComboBox;
            var selectedValue = box != null ? box.SelectedValue : null;
            if (!(selectedValue is int))
            {
                return;
            }
            
            var sqlServerId = (int)selectedValue;
            if (profiles.ContainsKey(sqlServerId))
            {
                applicationProfileComboBox.DataSource = profiles[sqlServerId].Select(p => p.ApplicationProfile).ToList();
            }
            UpdateUiControls();
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            updateStatusLabel.Text = string.Empty;
            var appProfile = (AzureApplicationProfile) applicationProfileComboBox.SelectedItem;
            if (appProfile != null)
            {
                UpdateDataSource(appProfile.Resources);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            var appProfile = (AzureApplicationProfile) applicationProfileComboBox.SelectedItem;
            if (appProfile == null)
            {
                return;
            }
            
            // Run a parallel Worker operation with a queue to perform the connection testing
            if (loadResourcesWorker.IsBusy)
            {
                loadResourceAppProfile = appProfile;
            }
            else
            {
                UpdateStatusProgressBar(true);
                updateStatusLabel.Text = string.Format(ConnectToAzureFmt, appProfile.Name);
                loadResourcesWorker.RunWorkerAsync(appProfile);
            }
        }

        private void UpdateStatusProgressBar(bool running)
        {
            if (running)
            {
                statusProgressBar.Start();
            }
            else
            {
                statusProgressBar.Stop();
            }
        }

        private void loadResourcesWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            var appProfile = e.Argument as IAzureApplicationProfile;
            if (appProfile == null)
            {
                Log.Error("azureProfileWorker_DoWork: Invalid Argument, expected IAzureApplicationProfile");

                e.Result = new Tuple<IAzureApplicationProfile, List<IAzureResource>, Exception>(
                    new AzureApplicationProfile(), null,
                    new Exception("Unexpected input to connect to the azure server."));
            }

            try
            {
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
                e.Result = new Tuple<IAzureApplicationProfile, List<IAzureResource>, Exception>(appProfile, managementService.GetAzureApplicationResources(configuration), null);
            }
            catch (Exception exception)
            {
                Log.Error("Unable to connect to the azure server.", exception);
                e.Result = new Tuple<IAzureApplicationProfile, List<IAzureResource>, Exception>(appProfile, null, exception);
            }
        }

        private void UpdateUiControls()
        {
            if (azureResourcesGrid.Rows.Count > 0)
            {
                lblNoResources.Visible = false;
                lblNoResources.SendToBack();
            }
            else
            {
                lblNoResources.Visible = true;
                lblNoResources.BringToFront();
            }
        }

        private void loadResourcesWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled || this.IsDisposed)
            {
                updateStatusLabel.Text = string.Empty;
                UpdateStatusProgressBar(false);
            }
            else
            {
                var result = (Tuple<IAzureApplicationProfile, List<IAzureResource>, Exception>)e.Result;
                var appProfile = result.Item1;
                var exception = result.Item3;
                var azureResources = result.Item2;
                if (exception != null)
                {
                    var unableToConnectToTheAzureServer = "Unable to connect to the azure server.";
                    updateStatusLabel.Text =
                        string.Format(ErrorConnectingFmt, appProfile.Name,
                            unableToConnectToTheAzureServer);
                    ApplicationMessageBox.ShowError(this, unableToConnectToTheAzureServer, exception);
                }
                else
                {
                    RepositoryHelper.UpdateAzureResources(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                        appProfile, azureResources);
                    UpdateDataSource(azureResources);
                    appProfile.Resources = azureResources;
                    foreach (var profile in profiles.SelectMany(serverProfilePair =>
                        serverProfilePair.Value.Where(profile => profile.ApplicationProfile.Id == appProfile.Id)))
                    {
                        // Update resources in all the application profiles
                        profile.ApplicationProfile.Resources = azureResources;
                    }
                    updateStatusLabel.Text = string.Empty;
                }

                if (this.IsDisposed)
                {
                    return;
                }
                if (loadResourceAppProfile != appProfile && loadResourceAppProfile != null)
                {
                    var workItem = loadResourceAppProfile;
                    updateStatusLabel.Text = string.Format(ConnectToAzureFmt, workItem.Name);
                    loadResourcesWorker.RunWorkerAsync(workItem);
                }
                else
                {
                    UpdateStatusProgressBar(false);
                }
            }
        }

        private void applicationProfileComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateUiControls();
        }
    }
}
