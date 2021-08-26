using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win;
using BBS.TracerX;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Resources = Idera.SQLdm.DesktopClient.Properties.Resources;
using Settings = Idera.SQLdm.DesktopClient.Properties.Settings;
using Idera.SQLdm.Common.Events.AzureMonitor;
using Idera.SQLdm.Common.Events.AzureMonitor.Interfaces;

namespace Idera.SQLdm.DesktopClient.Dialogs.AzureConfigurations
{
    public partial class AzureProfilesConfiguration : Form
    {
        private readonly bool isReadOnly;
        private static readonly Logger Log = Logger.GetLogger("AzureProfilesConfiguration");
        private const string AttemptingToConnectTo = "Attempting to connect to {0}...";
        private readonly List<IAzureApplicationProfile> azureTestList;
        private Dictionary<int, List<IAzureProfile>> profilesMap;
        private Dictionary<string, IAzureApplicationProfile> applicationProfilesMap;
        private List<IAzureProfile> profilesRaw;
        private List<IAzureApplicationProfile> applicationProfilesRaw;
        private const string ErrorConnectingUsingTheAzureApplication = "Error connecting using the azure application {0}:\r\n{1}";

        private long? appProfileGridCellSelectionId;
        private bool appProfileGridCellSelection;
        private long? profileGridCellSelectionId;
        private bool profileGridCellSelection;

        #region Constructor
        public AzureProfilesConfiguration(bool isReadOnly = false)
        {
            this.isReadOnly = isReadOnly;
            azureTestList = new List<IAzureApplicationProfile>();
            profileGridCellSelection = false;
            InitializeComponent();
            InitializeProgressBar();
            azureTestStatusLabel.Text = string.Empty;
            // using the same images here as in the manage servers dialog
            var statusValueList = appProfileGrid.DisplayLayout.ValueLists["statusValueList"];
            var listItem = new ValueListItem(AzureProfileTestStatus.Unknown)
            {
                Appearance = { Image = Resources.TestConnectionUnknown }
            };
            statusValueList.ValueListItems.Add(listItem);
            listItem = new ValueListItem(AzureProfileTestStatus.Testing)
            {
                Appearance = { Image = Resources.ToolbarRefresh }
            };
            statusValueList.ValueListItems.Add(listItem);

            listItem = new ValueListItem(AzureProfileTestStatus.Success)
            {
                Appearance = { Image = Resources.TestConnectionSuccess }
            };
            statusValueList.ValueListItems.Add(listItem);

            listItem = new ValueListItem(AzureProfileTestStatus.Failed)
            {
                Appearance = { Image = Resources.TestConnectionFailed }
            };
            statusValueList.ValueListItems.Add(listItem);

            appProfileGrid.DrawFilter = new HideFocusRectangleDrawFilter();
            azureProfileGrid.DrawFilter = new HideFocusRectangleDrawFilter();
        }

        private void InitializeProgressBar()
        {
            // Initialize the progress bar
            // 
            // statusProgressBar
            // 
            this.statusProgressBar = new Controls.InfiniteProgressBar
            {
                Color1 = System.Drawing.Color.FromArgb(255, 135,
                    45),
                Color2 = System.Drawing.Color.White,
                Dock = DockStyle.Top,
                Location = new System.Drawing.Point(0, 0),
                Name = "statusProgressBar",
                Size = new System.Drawing.Size(565, 3),
                Speed = 15D,
                Step = 5F,
                TabIndex = 9
            };
            this.Controls.Add(this.statusProgressBar);
        }

        #endregion


        private void AzureProfilesConfig_Load(object sender, EventArgs e)
        {
            SetupDataSource();
            appProfileGrid.DataSource = applicationProfilesRaw;
            azureProfileGrid.DataSource = profilesRaw;
            UpdateUiControls();
        }

        /// <summary>
        /// Display the empty list label, if the vcGrid does not contains elements to show.
        /// </summary>
        private void UpdateUiControls()
        {
            // Enable button on selected rows
            var isAppProfileGridRowSelected = appProfileGrid.Selected.Rows.Count > 0;
            var isProfileGridRowSelected = azureProfileGrid.Selected.Rows.Count > 0;
            btnViewResources.Enabled = azureProfileGrid.Rows.Any();

            // Enable button on selected rows
            btnTestAppProfile.Enabled = isAppProfileGridRowSelected;
            var isSqlAdmin = ApplicationModel.Default.UserToken.IsSQLdmAdministrator && !isReadOnly;
            btnAddAppProfile.Visible = btnDeleteAppProfile.Visible =
                btnEditAppProfile.Visible = isSqlAdmin;
            buttonEditProfile.Enabled =
                buttonDeleteProfile.Enabled = isSqlAdmin && isProfileGridRowSelected;

            buttonAddProfile.Visible = buttonEditProfile.Visible =
                buttonDeleteProfile.Visible = isSqlAdmin;
            btnDeleteAppProfile.Enabled =
                btnEditAppProfile.Enabled = isSqlAdmin && isAppProfileGridRowSelected;

            if (appProfileGrid.Rows.Count > 0)
            {
                lblAzureAppProfileNoInstances.Visible = false;
                lblAzureAppProfileNoInstances.SendToBack();
            }
            else
            {
                lblAzureAppProfileNoInstances.Visible = true;
                lblAzureAppProfileNoInstances.BringToFront();
            }

            if (azureProfileGrid.Rows.Count > 0)
            {
                lblNoLinkedAzureProfile.Visible = false;
                lblNoLinkedAzureProfile.SendToBack();
            }
            else
            {
                lblNoLinkedAzureProfile.Visible = true;
                lblNoLinkedAzureProfile.BringToFront();
            }
        }

        private void SetupDataSource()
        {
            // Get List of azure linked profiles
            profilesRaw =
                RepositoryHelper.GetAzureProfiles(Settings.Default.ActiveRepositoryConnection.ConnectionInfo) ??
                new List<IAzureProfile>();
            profilesMap = profilesRaw.GroupBy(p => p.SqlServerId)
                .ToDictionary(p => p.Key, p => p.ToList());

            // Get List of azure application profiles
            applicationProfilesRaw = RepositoryHelper.GetAzureApplicationProfiles(Settings.Default
                                         .ActiveRepositoryConnection
                                         .ConnectionInfo) ??
                                     new List<IAzureApplicationProfile>();
            applicationProfilesMap = applicationProfilesRaw.ToDictionary(ap => ap.Name,
                ap => ap);

            appProfileGrid.DataSource = applicationProfilesRaw;
            azureProfileGrid.DataSource = profilesRaw;
        }

        private void btnAddAppProfile_Click(object sender, EventArgs e)
        {
            var azureAppProfile = new AddEditAzureAppProfile(null, applicationProfilesMap);

            var result = azureAppProfile.ShowDialog(this);

            // Add the new app profile to the list
            if (DialogResult.OK == result)
            {
                SetupDataSource();
            }

            UpdateUiControls();
        }

        private void btnEditAppProfile_Click(object sender, EventArgs e)
        {
            // get the app profile being edited
            var applicationProfile = (AzureApplicationProfile)appProfileGrid.Selected.Rows[0].ListObject;
            if (applicationProfile == null)
            {
                return;
            }
            var azureAppProfile = new AddEditAzureAppProfile(applicationProfile, applicationProfilesMap);

            // save the new information and update the relevant controls
            //SQLdm 30820 Test Connection of Azure App Profile is successful even after deletion of subscription and application
            var returnedRes = azureAppProfile.ShowDialog(this);
            if(returnedRes == DialogResult.Cancel || returnedRes == DialogResult.OK)
            {
                SetupDataSource();
            }
        }

        private void btnDeleteAppProfile_Click(object sender, EventArgs e)
        {
            var applicationProfile = (AzureApplicationProfile)appProfileGrid.Selected.Rows[0].ListObject;
            if (applicationProfile == null)
            {
                return;
            }

            var message = new StringBuilder();
            message.AppendFormat("Deleting Azure Application Profile {0} will remove links to all associated SQL Server instances.", applicationProfile.Name);
            message.Append("\r\n\r\nAre you sure you want to delete this Azure Application Profile?");

            if (DialogResult.Yes == ApplicationMessageBox.ShowQuestion(this, message.ToString(),
                    Microsoft.SqlServer.MessageBox.ExceptionMessageBoxButtons.YesNo))
            {
                RepositoryHelper.DeleteAzureApplicationProfile(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, applicationProfile.Id);
                SetupDataSource();

            }
        }

        private void buttonAddProfile_Click(object sender, EventArgs e)
        {
            var azureProfile = new AddEditAzureProfile(null, profilesMap);

            var result = azureProfile.ShowDialog(this);

            // Add the new vCenter server to the list
            if (DialogResult.OK == result)
            {
                SetupDataSource();
            }
            UpdateUiControls();
        }

        private void buttonEditProfile_Click(object sender, EventArgs e)
        {
            var profile = (AzureProfile)azureProfileGrid.Selected.Rows[0].ListObject;
            if (profile == null)
            {
                return;
            }
            var azureProfile = new AddEditAzureProfile(profile, profilesMap);

            var result = azureProfile.ShowDialog(this);

            // Add the new vCenter server to the list
            if (DialogResult.OK == result)
            {
                SetupDataSource();
            }
        }

        private void buttonDeleteProfile_Click(object sender, EventArgs e)
        {
            var profile = (AzureProfile)azureProfileGrid.Selected.Rows[0].ListObject;
            if (profile == null)
            {
                return;
            }

            var message = new StringBuilder();
            message.AppendFormat("Deleting the Azure Profile will remove the Azure application profile link to the SQL Server instance.");
            message.Append("\r\n\r\nAre you sure you want to delete this Azure Profile?");

            if (DialogResult.Yes == ApplicationMessageBox.ShowQuestion(this, message.ToString(),
                    Microsoft.SqlServer.MessageBox.ExceptionMessageBoxButtons.YesNo))
            {
                RepositoryHelper.DeleteAzureProfile(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, profile.Id);
                SetupDataSource();
            }
        }

        private void btnViewResources_Click(object sender, EventArgs e)
        {
            var profile = azureProfileGrid.Selected.Rows.Count > 0
                ? (AzureProfile)azureProfileGrid.Selected.Rows[0].ListObject
                : null;
            var azureResourcesDialog = new AzureResourcesConfiguration(profile, profilesMap);

            var result = azureResourcesDialog.ShowDialog(this);

            if (DialogResult.OK == result)
            {
            }
        }

        private void azureProfileGrid_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            int sqlServerId;
            if (int.TryParse(e.Row.GetCellValue("SqlServerId").ToString(), out sqlServerId) &&
                ApplicationModel.Default.AllInstances.ContainsKey(sqlServerId))
            {
                e.Row.Cells["azureServer"].Value = ApplicationModel.Default.AllInstances[sqlServerId].DisplayInstanceName;
            }
        }



        private void btnTestAppProfile_Click(object sender, EventArgs e)
        {
            if (appProfileGrid.Selected.Rows.Count <= 0)
            {
                return;
            }
            var selectedRow = appProfileGrid.Selected.Rows[0];
            var applicationProfile = (AzureApplicationProfile)selectedRow.ListObject;

            // Prevent accidental testing on multiple clicks
            if (applicationProfile.TestStatus == AzureProfileTestStatus.Testing)
            {
                return;
            }

            applicationProfile.TestStatus = AzureProfileTestStatus.Testing;
            appProfileGrid.Rows.Refresh(RefreshRow.ReloadData);

            // Run a parallel Worker operation with a queue to perform the connection testing
            if (azureProfileWorker.IsBusy)
            {
                azureTestList.Add(applicationProfile);
            }
            else
            {
                UpdateStatusProgressBar(true);
                azureTestStatusLabel.Text = string.Format(AttemptingToConnectTo, applicationProfile.Name);
                azureProfileWorker.RunWorkerAsync(applicationProfile);
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

        private void azureProfileWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
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

        private void azureProfileWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled || this.IsDisposed)
            {
                azureTestStatusLabel.Text = string.Empty;
                UpdateStatusProgressBar(false);
            }
            else
            {
                var result = (Tuple<IAzureApplicationProfile, List<IAzureResource>, Exception>)e.Result;
                var appProfile = result.Item1;
                var exception = result.Item3;

                if (exception != null)
                {
                    var unableToConnectToTheAzureServer = "Unable to connect to the azure server.";
                    azureTestStatusLabel.Text =
                        string.Format(ErrorConnectingUsingTheAzureApplication, appProfile.Name,
                            unableToConnectToTheAzureServer);
                    appProfile.TestStatus = AzureProfileTestStatus.Failed;
                    ApplicationMessageBox.ShowError(this, unableToConnectToTheAzureServer, exception);
                }
                else
                {
                    appProfile.TestStatus = AzureProfileTestStatus.Success;
                    azureTestStatusLabel.Text = string.Empty;
                }

                if (this.IsDisposed)
                {
                    return;
                }
                if (azureTestList.Count > 0)
                {
                    var workItem = azureTestList[0];
                    azureTestList.RemoveAt(0);
                    azureTestStatusLabel.Text = string.Format(AttemptingToConnectTo, workItem.Name);
                    azureProfileWorker.RunWorkerAsync(workItem);
                }
                else
                {
                    UpdateStatusProgressBar(false);
                }
                appProfileGrid.Rows.Refresh(RefreshRow.ReloadData);
            }
        }

        private void appProfileGrid_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            UpdateUiControls();
        }

        private void azureProfileGrid_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            UpdateUiControls();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void appProfileGrid_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            if (btnEditAppProfile.Enabled)
            {
                btnEditAppProfile_Click(sender, e);
            }
        }

        private void azureProfileGrid_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            if (buttonEditProfile.Enabled)
            {
                buttonEditProfile_Click(sender, e);
            }
        }

        private void appProfileGrid_ClickCell(object sender, ClickCellEventArgs e)
        {
            var appProfile = e.Cell.Row.ListObject as IAzureApplicationProfile;
            if (appProfile == null)
            {
                return;
            }

            if (appProfileGridCellSelectionId == null || appProfile.Id != appProfileGridCellSelectionId)
            {
                appProfileGridCellSelectionId = appProfile.Id;
                appProfileGridCellSelection = e.Cell.Row.Selected;
                return;
            }

            if (appProfileGridCellSelection == e.Cell.Row.Selected)
            {
                e.Cell.Row.Selected = !e.Cell.Row.Selected;
            }
            appProfileGridCellSelection = e.Cell.Row.Selected;
        }

        private void azureProfileGrid_ClickCell(object sender, ClickCellEventArgs e)
        {
            var profile = e.Cell.Row.ListObject as IAzureProfile;
            if (profile == null)
            {
                return;
            }

            if (profileGridCellSelectionId == null || profile.Id != profileGridCellSelectionId)
            {
                profileGridCellSelectionId = profile.Id;
                profileGridCellSelection = e.Cell.Row.Selected;
                return;
            }

            if (profileGridCellSelection == e.Cell.Row.Selected)
            {
                e.Cell.Row.Selected = !e.Cell.Row.Selected;
            }
            profileGridCellSelection = e.Cell.Row.Selected;
        }
    }
}