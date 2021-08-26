using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using BBS.TracerX;
using Idera.SQLdm.DesktopClient.Objects;
using Microsoft.SqlServer.MessageBox;
using Settings = Idera.SQLdm.DesktopClient.Properties.Settings;
using Idera.SQLdm.Common;
using System.ComponentModel;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win;
using Amazon;
using System.Linq;
using Idera.SQLdm.Common.Data;
using Idera.SQLdm.Common.Events.AzureMonitor.Interfaces;
using Idera.SQLdm.Common.Events.AzureMonitor;
using Infragistics.Windows.Themes;
using Idera.SQLdm.DesktopClient.Controls;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    internal partial class AddServersWizard : Form
    {
        private const string AdhocInstructionText = "< Type semicolon separated names >";
        // SQLdm 10.3 (Varun Chopra) SQLDM-28770 - Support OS Collection for mixed instances with Linux
        private const string WMICONFIG = "WMI Configuration";
        private const string WMICONFIGFORNONLINUX = "WMI Configuration for Non Linux Instances";
        private const string AMAZONRDS = "Amazon RDS";
        private const string MICROSOFTAZURE = "Microsoft Azure";
        private const string LINUX = "Linux";
        private const string CLOUDPROVIDER = "Server Type";
        private const string CLOUDRDSREGION = "AWS RDS Region";
        private const string DISABLEOSWARNINGMESSAGE = "You have selected to disable collection of Operating System statistics.  Display of certain statistics will be unavailable and alerting for OS based metrics will not function.";
        private const string DISABLEOSNONLINUXWARNINGMESSAGE = "You have selected to disable collection of Operating System statistics.  Display of certain statistics will be unavailable and alerting for OS based metrics will not function for non linux instances.";
        private static readonly BBS.TracerX.Logger Log = BBS.TracerX.Logger.GetLogger("AddServersWizard");

        private readonly bool commitServers = false;
        private int existingInstanceCount;
        private readonly IDictionary<string, object> existingInstances;
        private TextItem<IAzureApplicationProfile> selectedProfile;
        private string sqlLoginName = String.Empty;
        private String sqlLoginPwd = String.Empty;
        private readonly List<MonitoredSqlServerConfiguration> addedInstances = new List<MonitoredSqlServerConfiguration>();
        private readonly string addedServersFormat;
        private readonly string availableLicensesFormat;
        private LicenseSummary license = null;
        private AdvancedQueryMonitorConfiguration advancedQueryFilterConfiguration = new AdvancedQueryMonitorConfiguration(true);
        private readonly Dictionary<int, CheckboxListItem> availableTagsLookupTable = new Dictionary<int, CheckboxListItem>();
        private bool addTagInProgress = false;
        private DataTable DataTableForCloudProvidersGrid = null;
        // SQLdm 10.3 (Varun Chopra) Linux Support Message
        private const string NOT_CLOUD_PROVIDER_STRING = "Not a cloud or Linux instance";//SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: Drop down label when the monitored instance is not on any cloud server
        private const string WINDOWS_PROVIDER_STRING = "Windows";
        private string AZURE_PROVIDER_STRING =  string.Empty;
        private readonly Dictionary<string, AzureSqlModel> _azureResourceMap;
        private DataTable availableSQLInstancesCache = null; // SQLdm 11 Cache the instances collected by worker
        private AuthOption? prevSelectedAuthOption = null;
        private long? prevSelectedAzureProfileId = null;
        //SQLdm11 selectedInstances display 
        enum AuthOption
        {
            WINDOWS,
            SQL,
            AZURESQL
        }

        public AddServersWizard(bool commitServers, IDictionary<string, object> existingInstances)
        {
            _azureResourceMap = new Dictionary<string, AzureSqlModel>();
            this.commitServers = commitServers;
            this.existingInstances = existingInstances;
            InitializeComponent();
            SetGridTheme();
            SetLoadingBackColor();
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
            this.topPlanComboBox.Items.Add(0, Duration);

            this.topPlanComboBox.Items.Add(1, LogicalDiskReads);
            this.topPlanComboBox.Items.Add(2, CpuUsage);
            this.topPlanComboBox.Items.Add(3, PhysicalWrites);
            this.topPlanComboBox.Value = 0;
            Icon = Properties.Resources.ServersIcon;

            availableLicensesFormat = availableLicensesLabel.Text;
            addedServersFormat = addedInstancesLabel.Text;
            availableLicensesLabel.Text = string.Format(availableLicensesFormat, "( " + Idera.SQLdm.Common.Constants.LOADING + " )");
            addedInstancesLabel.Text = string.Format(addedServersFormat, 0);
            adhocInstancesTextBox.Text = AdhocInstructionText;

            loadingServersProgressControl.Active = true;
            loadAvailableServersWorker.RunWorkerAsync();

            LoadAvailableTemplates();

            baselineConfiguration1.ChangeBaselineTittleControlStyle();
            baselineConfiguration1.BackColor = SystemColors.Control;
            AdaptFontSize();
            this.baselineConfiguration1.SetVisibleBaseline(false);
            HideNonQuery();
            if (this.wizardFramework.SelectedPage == this.baselineConfiguration) this.HelpButton = false; //SQLdm9.0: not showing the help link in baseline page.
            //Saurabh - SQLDM-30848 - UX-Modernization, PRD 4.2
            if (AutoScaleSizeHelper.isScalingRequired)
            {
                ScaleControlsAsPerResolution();
            }
          
        }

        void OnCurrentThemeChanged(object sender, EventArgs e)
        {
            SetGridTheme();
            SetLoadingBackColor();
        }

        private void SetGridTheme()
        {
            // Update UltraGrid Theme
            var themeManager = new GridThemeManager();
            themeManager.updateGridTheme(this.grdCloudProviders);
        }

        private void SetLoadingBackColor()
        {
            if (Settings.Default.ColorScheme == "Dark")
            {
                this.loadingServersProgressControl.BackColor = ColorTranslator.FromHtml(DarkThemeColorConstants.BackColor);
            }
            else
            {
                this.loadingServersProgressControl.BackColor = System.Drawing.Color.White;
            }
        }

        public void HideNonQuery()
        {
            thresholdWarningLabel.AutoSize = true;

            //groupBox5.Size = new Size(510, 256);
            groupBox5.Size = new Size(510, 289);  //SQLdm 10.4 (Ruchika Salwan) -- position changed to fit new UI changes
            groupBox6.Visible = false;

            this.featuresGroupBox.Controls.Remove(queryMonitorAdvancedOptionsButton);

            queryMonitorAdvancedOptionsButton.Location = new Point(10, 10);
            groupBox5.Controls.Add(queryMonitorAdvancedOptionsButton);

            queryMonitorAdvancedOptionsButton.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
            queryMonitorAdvancedOptionsButton.AutoSize = true;

            int x, y;
            x = (groupBox5.Size.Width - queryMonitorAdvancedOptionsButton.Size.Width) - 5;
            y = (groupBox5.Size.Height - queryMonitorAdvancedOptionsButton.Size.Height) - 15;

            queryMonitorAdvancedOptionsButton.Location = new Point(x, y); //SQLdm 10.4 (Ruchika Salwan) -- y axis changed to fit new UI changes
            queryMonitorAdvancedOptionsButton.BringToFront();
        }

        public IList<MonitoredSqlServerConfiguration> AddedInstances
        {
            get { return addedInstances; }
        }

        //START: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: Adding another page for choosing the cloud provider for all the added instances
        private void chooseCloudProviderPage_BeforeDisplay(object sender, EventArgs e)
        {
            Dictionary<string, int> cloudProvidersDetail = RepositoryHelper.GetCloudProviders(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            Infragistics.Win.ValueList valueList1 = new Controls.CustomControls.CustomValueList(74068277);
            valueList1.Key = CLOUDPROVIDER;
            valueList1.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            foreach (string cloudProviderName in cloudProvidersDetail.Keys)
            {
                object temp = new object();
                temp = cloudProviderName;
                valueList1.ValueListItems.Add(temp);
            }
            // Get AWS regions endpoints
            Infragistics.Win.ValueList valueListRegions = new Controls.CustomControls.CustomValueList(74068277);
            valueListRegions.Key = CLOUDRDSREGION;
            valueListRegions.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            foreach (string rdsRegion in RegionEndpoint.EnumerableAllRegions.Select(item => item.DisplayName).ToList())
            {
                object temp = new object();
                temp = rdsRegion;
                valueListRegions.ValueListItems.Add(temp);
            }
            // valueList1.SelectedIndex = 3;
            //valueList1.SortStyle = ValueListSortStyle.AscendingByValue;
            // Get AWS regions endpoints
            this.grdCloudProviders.DisplayLayout.ValueLists.Clear();
            //Adding Default selection Data in list
            this.grdCloudProviders.DisplayLayout.ValueLists.Add(WINDOWS_PROVIDER_STRING);
            this.grdCloudProviders.DisplayLayout.ValueLists.Add(valueList1);
            this.grdCloudProviders.DisplayLayout.ValueLists.Add(valueListRegions);

            DataTableForCloudProvidersGrid = new DataTable("DataTableForCloudProvidersGrid");

            DataTableForCloudProvidersGrid.Columns.Add("Server Name", typeof(string));
            DataTableForCloudProvidersGrid.Columns.Add(CLOUDPROVIDER, typeof(string));
            DataTableForCloudProvidersGrid.Columns.Add(AWSRDSAccessKey, typeof(string));
            DataTableForCloudProvidersGrid.Columns.Add(AWSRDSSecretKey, typeof(string));
            DataTableForCloudProvidersGrid.Columns.Add(CLOUDRDSREGION, typeof(string));

            grdCloudProviders.DisplayLayout.Override.AllowUpdate = DefaultableBoolean.True;
            grdCloudProviders.DataSource = DataTableForCloudProvidersGrid;

            grdCloudProviders.DisplayLayout.Bands[0].Columns["Server Name"].CellActivation = Activation.NoEdit;
            grdCloudProviders.DisplayLayout.Bands[0].Columns[AWSRDSAccessKey].CellActivation = Activation.NoEdit;
            grdCloudProviders.DisplayLayout.Bands[0].Columns[AWSRDSSecretKey].CellActivation = Activation.NoEdit;
            grdCloudProviders.DisplayLayout.Bands[0].Columns[CLOUDRDSREGION].CellActivation = Activation.NoEdit;

            grdCloudProviders.DisplayLayout.Bands[0].Columns["Server Name"].SortIndicator = SortIndicator.Disabled;
            grdCloudProviders.DisplayLayout.Bands[0].Columns[CLOUDPROVIDER].SortIndicator = SortIndicator.Disabled;
            grdCloudProviders.DisplayLayout.Bands[0].Columns[AWSRDSAccessKey].SortIndicator = SortIndicator.Disabled;
            grdCloudProviders.DisplayLayout.Bands[0].Columns[AWSRDSSecretKey].SortIndicator = SortIndicator.Disabled;
            grdCloudProviders.DisplayLayout.Bands[0].Columns[CLOUDRDSREGION].SortIndicator = SortIndicator.Disabled;
            if (AutoScaleSizeHelper.isScalingRequired)
            {
                if (AutoScaleSizeHelper.isLargeSize)
                {
                    grdCloudProviders.DisplayLayout.Bands[0].Columns["Server Name"].Width = 150;
                    grdCloudProviders.DisplayLayout.Bands[0].Columns[CLOUDPROVIDER].Width = 300;
                    grdCloudProviders.DisplayLayout.Bands[0].Columns[AWSRDSAccessKey].Width = 200;
                    grdCloudProviders.DisplayLayout.Bands[0].Columns[AWSRDSSecretKey].Width = 300;
                    grdCloudProviders.DisplayLayout.Bands[0].Columns[CLOUDRDSREGION].Width = 200;
                }
                if (AutoScaleSizeHelper.isXLargeSize)
                {
                    grdCloudProviders.DisplayLayout.Bands[0].Columns["Server Name"].Width = 200;
                    grdCloudProviders.DisplayLayout.Bands[0].Columns[CLOUDPROVIDER].Width = 400;
                    grdCloudProviders.DisplayLayout.Bands[0].Columns[AWSRDSAccessKey].Width = 250;
                    grdCloudProviders.DisplayLayout.Bands[0].Columns[AWSRDSSecretKey].Width = 350;
                    grdCloudProviders.DisplayLayout.Bands[0].Columns[CLOUDRDSREGION].Width = 250;
                }
                if (AutoScaleSizeHelper.isXXLargeSize)
                {
                    grdCloudProviders.DisplayLayout.Bands[0].Columns["Server Name"].Width = 250;
                    grdCloudProviders.DisplayLayout.Bands[0].Columns[CLOUDPROVIDER].Width = 500;
                    grdCloudProviders.DisplayLayout.Bands[0].Columns[AWSRDSAccessKey].Width = 300;
                    grdCloudProviders.DisplayLayout.Bands[0].Columns[AWSRDSSecretKey].Width = 400;
                    grdCloudProviders.DisplayLayout.Bands[0].Columns[CLOUDRDSREGION].Width = 300;
                }
            }
            else
            {
                grdCloudProviders.DisplayLayout.Bands[0].Columns["Server Name"].Width = 100;
                grdCloudProviders.DisplayLayout.Bands[0].Columns[CLOUDPROVIDER].Width = 210;
                grdCloudProviders.DisplayLayout.Bands[0].Columns[AWSRDSAccessKey].Width = 150;
                grdCloudProviders.DisplayLayout.Bands[0].Columns[AWSRDSSecretKey].Width = 250;
                grdCloudProviders.DisplayLayout.Bands[0].Columns[CLOUDRDSREGION].Width = 150;
            }

            DataTableForCloudProvidersGrid.Clear();

            DataTableForCloudProvidersGrid.PrimaryKey = new DataColumn[] { DataTableForCloudProvidersGrid.Columns[0] };

            grdCloudProviders.DisplayLayout.Bands[0].Columns[CLOUDPROVIDER].ValueList =
                grdCloudProviders.DisplayLayout.ValueLists[CLOUDPROVIDER];

            grdCloudProviders.DisplayLayout.Bands[0].Columns[CLOUDPROVIDER].Style =
                Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;

            grdCloudProviders.DisplayLayout.Bands[0].Columns[CLOUDRDSREGION].ValueList =
                grdCloudProviders.DisplayLayout.ValueLists[CLOUDRDSREGION];

            grdCloudProviders.DisplayLayout.Bands[0].Columns[CLOUDRDSREGION].Style =
                Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;


            foreach (ListViewItem item in addedInstancesListBox.Items)
            {
                object[] rowobject = new object[2];

                rowobject[0] = item.Tag;
                var tagName = (string)item.Tag;
                if (useSqlAunthenticationForAzureCheckBox.Checked && selectedAzureProfileNameTextBox.Text.Length > 0)
                {
                    string azureCloud = null;
                    if (_azureResourceMap.ContainsKey(tagName))
                    {
                        if ("Microsoft.Sql/servers".Equals(_azureResourceMap[tagName].Type,
                            StringComparison.InvariantCultureIgnoreCase))
                        {
                            azureCloud =
                                cloudProvidersDetail.FirstOrDefault(c => c.Value == Constants.MicrosoftAzureId).Key;
                            AZURE_PROVIDER_STRING = !string.IsNullOrEmpty(azureCloud) ? azureCloud : "Microsoft Azure SQL Database (AzureDB)";
                        } else if ("Microsoft.Sql/managedInstances".Equals(_azureResourceMap[tagName].Type,
                            StringComparison.InvariantCultureIgnoreCase))
                        {
                            azureCloud =
                                cloudProvidersDetail.FirstOrDefault(c => c.Value == Constants.MicrosoftAzureManagedInstanceId).Key;
                            AZURE_PROVIDER_STRING = !string.IsNullOrEmpty(azureCloud) ? azureCloud : "Microsoft Azure SQL Managed Instance";
                        }
                    }
                    else
                    {
                        azureCloud =
                            cloudProvidersDetail.FirstOrDefault(c => c.Value == Constants.MicrosoftAzureId).Key;
                        AZURE_PROVIDER_STRING = !string.IsNullOrEmpty(azureCloud)
                            ? azureCloud
                            : "Microsoft Azure SQL Database (AzureDB)";
                    }

                    rowobject[1] = AZURE_PROVIDER_STRING;
                }
                else
                {
                    rowobject[1] = WINDOWS_PROVIDER_STRING;
                }
                DataTableForCloudProvidersGrid.LoadDataRow(rowobject, true);
            }

            DataTableForCloudProvidersGrid.EndLoadData();
        }

        //Help click page
        private void chooseCloudProviderPage_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            if (hlpevent != null)
                hlpevent.Handled = true;
            //Srishti 10.0 -- TO Do change help link once recieved from idera
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.AddServersWizard_ServerTypeSelection);
        }
        //END: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: Adding another page for choosing the cloud provider for all the added instances

        //START: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: Adding another page for choosing the cloud provider for all the added instances
        private void grdCloudProviders_KeyPress(object sender, KeyPressEventArgs e)
        {
            UltraGrid grid = sender as UltraGrid;
            UltraGridCell activeCell = grid == null ? null : grid.ActiveCell;

            // if there is an active cell, its not in edit mode and can enter edit mode
            if (null != activeCell && false == activeCell.IsInEditMode && activeCell.CanEnterEditMode)
            {
                // if the character is not a control character
                if (char.IsControl(e.KeyChar) == false)
                {
                    // try to put cell into edit mode
                    grid.PerformAction(UltraGridAction.EnterEditMode);

                    // if this cell is still active and it is in edit mode...
                    if (grid.ActiveCell == activeCell && activeCell.IsInEditMode)
                    {
                        // get its editor
                        EmbeddableEditorBase editor = this.grdCloudProviders.ActiveCell.EditorResolved;

                        // if the editor supports selectable text
                        if (editor.SupportsSelectableText)
                        {
                            // select all the text so it can be replaced
                            editor.SelectionStart = 0;
                            editor.SelectionLength = editor.TextLength;

                            if (editor is EditorWithMask)
                            {
                                // just clear the selected text and let the grid
                                // forward the keypress to the editor
                                editor.SelectedText = string.Empty;
                            }
                            else
                            {
                                // then replace the selected text with the character
                                editor.SelectedText = new string(e.KeyChar, 1);

                                // mark the event as handled so the grid doesn't process it
                                e.Handled = true;
                            }
                        }
                    }
                }
            }
        }
        //END: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: Adding another page for choosing the cloud provider for all the added instances

        //START: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: Adding another page for choosing the cloud provider for all the added instances
        private void grdCloudProviders_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                UIElement selectedElement =
                    ((UltraGrid)sender).DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));
                object contextObject = selectedElement.GetContext(typeof(System.Windows.Forms.ColumnHeader));

                if (contextObject is System.Windows.Forms.ColumnHeader)
                {
                    System.Windows.Forms.ColumnHeader columnHeader = contextObject as System.Windows.Forms.ColumnHeader;
                }
                else
                {
                    contextObject = selectedElement.GetAncestor(typeof(RowUIElement));

                    if (contextObject is RowUIElement)
                    {
                        RowUIElement row = contextObject as RowUIElement;
                        row.Row.Selected = true;
                    }
                    else
                    {
                        //toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "gridContextMenu");
                    }
                }
            }
            //else
            //{
            //    UIElement selectedElement =
            //        ((UltraGrid)sender).DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));
            //    object contextObject = selectedElement.GetContext(typeof(UltraGridCell));

            //    if (contextObject is UltraGridCell)
            //    {
            //        UltraGridCell cell = contextObject as UltraGridCell;

            //        if (cell.Column.Key == "MoveUp")
            //        {
            //            //is there a row below?
            //            if (cell.Row.Index > 0)
            //            {
            //                int thisRowIndex = cell.Row.Index;
            //                int prevRowIndex = thisRowIndex - 1;

            //                object[] prevRowObject = new object[7];

            //                //save the next row
            //                prevRowObject[0] = thisRowIndex;
            //                prevRowObject[1] = grdCloudProviders.Rows[prevRowIndex].Cells["Graph Title"].Value.ToString();
            //                prevRowObject[2] = grdCloudProviders.Rows[prevRowIndex].Cells["Counter"].Value.ToString();
            //                prevRowObject[3] = grdCloudProviders.Rows[prevRowIndex].Cells["Aggregation"].Value.ToString();
            //                prevRowObject[4] = int.Parse(grdCloudProviders.Rows[prevRowIndex].Cells["Source"].Value.ToString());
            //                prevRowObject[5] = MoveUp;
            //                prevRowObject[6] = MoveDown;

            //                object[] thisRowObject = new object[7];

            //                //save the next row
            //                thisRowObject[0] = prevRowIndex;
            //                thisRowObject[1] = grdSelectedCounters.Rows[thisRowIndex].Cells["Graph Title"].Value.ToString();
            //                thisRowObject[2] = grdSelectedCounters.Rows[thisRowIndex].Cells["Counter"].Value.ToString();
            //                thisRowObject[3] = grdSelectedCounters.Rows[thisRowIndex].Cells["Aggregation"].Value.ToString();
            //                thisRowObject[4] = int.Parse(grdSelectedCounters.Rows[thisRowIndex].Cells["Source"].Value.ToString());
            //                thisRowObject[5] = MoveUp;
            //                thisRowObject[6] = MoveDown;


            //                AddedCounters.BeginLoadData();

            //                //insert the row with this rows old graph number
            //                AddedCounters.LoadDataRow(thisRowObject, true);
            //                AddedCounters.LoadDataRow(prevRowObject, true);


            //                AddedCounters.EndLoadData();

            //                AddedCounters.DefaultView.Sort = "GraphNumber asc";

            //                grdSelectedCounters.Refresh();
            //                setTooltipsOnArrows(grdSelectedCounters);

            //            }
            //        }

            //        if (cell.Column.Key == "MoveDown")
            //        {
            //            //is there a row below?
            //            if (cell.Row.Index < grdSelectedCounters.Rows.Count - 1)
            //            {
            //                int thisRowIndex = cell.Row.Index;
            //                int nextRowIndex = thisRowIndex + 1;

            //                object[] nextRowObject = new object[7];

            //                //save the next row
            //                nextRowObject[0] = thisRowIndex;
            //                nextRowObject[1] = grdSelectedCounters.Rows[nextRowIndex].Cells["Graph Title"].Value.ToString();
            //                nextRowObject[2] = grdSelectedCounters.Rows[nextRowIndex].Cells["Counter"].Value.ToString();
            //                nextRowObject[3] = grdSelectedCounters.Rows[nextRowIndex].Cells["Aggregation"].Value.ToString();
            //                nextRowObject[4] = int.Parse(grdSelectedCounters.Rows[nextRowIndex].Cells["Source"].Value.ToString());
            //                nextRowObject[5] = MoveUp;
            //                nextRowObject[6] = MoveDown;

            //                object[] thisRowObject = new object[7];

            //                //save the next row
            //                thisRowObject[0] = nextRowIndex;
            //                thisRowObject[1] = grdSelectedCounters.Rows[thisRowIndex].Cells["Graph Title"].Value.ToString();
            //                thisRowObject[2] = grdSelectedCounters.Rows[thisRowIndex].Cells["Counter"].Value.ToString();
            //                thisRowObject[3] = grdSelectedCounters.Rows[thisRowIndex].Cells["Aggregation"].Value.ToString();
            //                thisRowObject[4] = int.Parse(grdSelectedCounters.Rows[thisRowIndex].Cells["Source"].Value.ToString());
            //                thisRowObject[5] = MoveUp;
            //                thisRowObject[6] = MoveDown;


            //                AddedCounters.BeginLoadData();

            //                //insert the row with this rows old graph number
            //                AddedCounters.LoadDataRow(thisRowObject, true);
            //                AddedCounters.LoadDataRow(nextRowObject, true);
            //                AddedCounters.EndLoadData();

            //                AddedCounters.DefaultView.Sort = "GraphNumber asc";

            //                grdSelectedCounters.Refresh();
            //                setTooltipsOnArrows(grdSelectedCounters);
            //            }
            //        }
            //    }
            //}
        }
        //END: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: Adding another page for choosing the cloud provider for all the added instances

        private void useWindowsAuthenticationRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateConfigureAuthenticationPage();
        }

        private void useSqlServerAuthenticationRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateConfigureAuthenticationPage();
        }

        private void useSqlAunthenticationForAzureCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            var enableDiscoveryChecked = useSqlAunthenticationForAzureCheckBox.Checked;
            if (enableDiscoveryChecked)
            {
                sqlLoginName = loginNameTextbox.Text;
                sqlLoginPwd = passwordTextbox.Text;
                if (loginNameTextbox.Text.Length > 0)
                {
                    addAzureServerButton.Enabled = true;
                }
            }

            addAzureServerButton.Enabled = enableDiscoveryChecked;
            selectedAzureProfileLbl.Visible = enableDiscoveryChecked;
            selectedAzureProfileNameTextBox.Visible = enableDiscoveryChecked;
            // SQLDM-30822 'Next' button is disabled when switched between SQL authentication and Windows
            UpdateConfigureAuthenticationPage();
            if (enableDiscoveryChecked)
            {
                if (selectedAzureProfileNameTextBox.Text.ToString().Length > 0)
                    configureAuthenticationPage.AllowMoveNext = true;
                else
                    configureAuthenticationPage.AllowMoveNext = false;
            } else
            {
                configureAuthenticationPage.AllowMoveNext = true;
            }
        }

        private void addAzureServerButton_Click(object sender, EventArgs e)
        {
        	// Ensure the current selected profile is selected
            // SQLDM - 30823 Credentials not captured in profile config when check box selected first
            sqlLoginName = loginNameTextbox.Text;
            sqlLoginPwd = passwordTextbox.Text;
            var azureDiscoveryDialog = new AzureDiscoveryDialog(sqlLoginName, sqlLoginPwd, selectedProfile != null ? selectedProfile.Text : null);
            var result = azureDiscoveryDialog.ShowDialog(this);
            if (DialogResult.OK == result)
            {
                selectedProfile = azureDiscoveryDialog.getSelectedProfile();
            }
            if (selectedProfile != null && useSqlAunthenticationForAzureCheckBox.Checked)
            {
                selectedAzureProfileNameTextBox.Text = selectedProfile.Text;
            }
            UpdateConfigureAuthenticationPage();
			if (selectedAzureProfileNameTextBox.Text.ToString().Length > 0)
                configureAuthenticationPage.AllowMoveNext = true;
            else
                configureAuthenticationPage.AllowMoveNext = false;
        } 

        private void loadAvailableServersWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (System.Threading.Thread.CurrentThread.Name == null) System.Threading.Thread.CurrentThread.Name = "LoadAvailableServersWorker";
            // First get the license information since we can do this 
            // without the management service or collection service.
            license = RepositoryHelper.GetLicenseKeys(null);

            if (!loadAvailableServersWorker.CancellationPending)
            {
                //[START]SQLdm 9.0 (Ankit Srivastava)- Resolved defect DE44130 - decreased the default timeout to 20
                var connInfo = Settings.Default.ActiveRepositoryConnection.ConnectionInfo;
                connInfo.ConnectionTimeout = 20;
                IManagementService managementService =
                        ManagementServiceHelper.GetDefaultService(connInfo);
                //[END]SQLdm 9.0 (Ankit Srivastava)- Resolved defect DE44130 - decreased the default timeout to 20

                // This requires both the MS and CS to be up or it throws an exception.
                e.Result = managementService.GetAvailableSqlServerInstances();
            }

            if (!loadAvailableServersWorker.CancellationPending)
            {
                ApplicationModel.Default.RefreshTags();
            }

            e.Cancel = loadAvailableServersWorker.CancellationPending;
        }

        private void loadAvailableServersWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                if (license == null)
                {
                    availableLicensesLabel.Text = string.Format(availableLicensesFormat, "Unknown");
                }
                else if (license.IsUnlimited)
                {
                    availableLicensesLabel.Text = string.Format(availableLicensesFormat, "Unlimited");
                }
                else
                {
                    if (existingInstances == null)
                    {
                        existingInstanceCount = license.MonitoredServers;
                    }
                    else
                    {
                        existingInstanceCount = existingInstances.Count;
                    }

                    availableLicensesLabel.Text = string.Format(availableLicensesFormat, license.LicensedServers - existingInstanceCount);
                }

                loadingServersProgressControl.Active = false;
                loadingServersProgressControl.Hide();

                if (e.Error != null)
                {
                    Log.Error("Unable to retrieve available SQL Server instances from the management service.", e.Error);
                    availableInstancesListBox.Items.Clear();
                    availableInstancesStatusLabel.Show();
                }
                else
                {
                    availableSQLInstancesCache = e.Result as DataTable;
                    ListAvailableInstances(e.Result as DataTable);
                    LoadAvailableTags();

                }
            }
        }

        private void UpdateSelectedInstancesPage()
        {
            UpdateSelectedInstancesPage(false);
        }

        private void UpdateSelectedInstancesPage(bool ignoreAdhocInstanceTextBox)
        {
            addedInstancesLabel.Text = string.Format(addedServersFormat, addedInstancesListBox.Items.Count);

            selectInstancesPage.AllowMoveNext = addedInstancesListBox.Items.Count > 0 && license != null;
            removeInstancesButton.Enabled = addedInstancesListBox.SelectedItems.Count > 0;
            addInstancesButton.Enabled = availableInstancesListBox.SelectedItems.Count > 0 ||
                                         (adhocInstancesTextBox.Text.Trim().Length > 0 &&
                                         string.CompareOrdinal(AdhocInstructionText, adhocInstancesTextBox.Text.Trim()) != 0);

            if (addInstancesButton.Enabled)
            {
                this.AcceptButton = addInstancesButton;
            }
            else if (removeInstancesButton.Enabled)
            {
                this.AcceptButton = removeInstancesButton;
            }

            if (!ignoreAdhocInstanceTextBox)
            {
                if (adhocInstancesTextBox.Text.Trim().Length == 0)
                {
                    adhocInstancesTextBox.ForeColor = SystemColors.GrayText;
                    adhocInstancesTextBox.Text = AdhocInstructionText;
                }
            }
        }

        private void UpdateConfigureAuthenticationPage()
        {
            loginNameTextbox.Enabled = useSqlServerAuthenticationRadioButton.Checked;
            passwordTextbox.Enabled = useSqlServerAuthenticationRadioButton.Checked;
            // SQLDM-30789 - 'Azure Discovery Instance' should be disabled for Windows Authentication
            useSqlAunthenticationForAzureCheckBox.Enabled = useSqlServerAuthenticationRadioButton.Checked;
            addAzureServerButton.Enabled = useSqlAunthenticationForAzureCheckBox.Checked && useSqlServerAuthenticationRadioButton.Checked;

            if (useWindowsAuthenticationRadioButton.Checked || loginNameTextbox.Text.Trim().Length > 0)
            {
                configureAuthenticationPage.AllowMoveNext = true;
            }
            else
            {
                configureAuthenticationPage.AllowMoveNext = false;
            }
            // SQLDM-30822 'Next' button is disabled when switched between SQL authentication and Windows
            if (useSqlServerAuthenticationRadioButton.Checked && loginNameTextbox.Text.Trim().Length > 0 && useSqlAunthenticationForAzureCheckBox.Checked)
            {
                if (selectedAzureProfileNameTextBox.Text.ToString().Length > 0)
                    configureAuthenticationPage.AllowMoveNext = true;
                else
                    configureAuthenticationPage.AllowMoveNext = false;
            }
        }

        private void ListAvailableInstances(DataTable availableInstances)
        {
            // SQLdm 11 Azure list being replaced with SQL
            if ((useSqlServerAuthenticationRadioButton.Checked) && (useSqlAunthenticationForAzureCheckBox.Checked) && (selectedAzureProfileNameTextBox.Text.ToString().Length > 0))
                return;
            availableInstancesListBox.Items.Clear();

            if (availableInstances == null || availableInstances.Rows.Count == 0)
            {
                availableInstancesStatusLabel.Show();
            }
            else
            {
                availableInstancesStatusLabel.Hide();
                availableInstancesListBox.Items.Clear();

                foreach (DataRow instance in availableInstances.Rows)
                {
                    string instanceName = instance[0].ToString();

                    if (existingInstances == null || !existingInstances.ContainsKey(instanceName))
                    {
                        if (GetAddedInstanceIndex(instanceName) == -1)
                        {
                            string instanceVersion = instance[4].ToString();
                            string itemText = instanceVersion.Length > 0
                                                  ? instanceName + "  (" + instanceVersion + ")"
                                                  : instanceName;

                            ListViewItem instanceItem = new ListViewItem(itemText);
                            instanceItem.Tag = instanceName;
                            //SQLdm 11 selection and available contain duplicates
                            var commonInstancesCollection = addedInstancesListBox.Items.Cast<ListViewItem>().Where(listItem => listItem.Tag.ToString() == instanceItem.Tag.ToString());
                            if (commonInstancesCollection.Count() == 0)    
                                availableInstancesListBox.Items.Add(instanceItem);
                        }
                    }
                }
            }
        }

        private void LoadAvailableTags()
        {
            if (ApplicationModel.Default.LocalTags.Count > 0)
            {
                availableTagsStatusLabel.Hide();
                availableTagsListBox.Show();
                availableTagsListBox.BeginUpdate();

                //SQLDM 10.1 (Srishti Purohit)
                //removing global tags as they can not be edited from SQLdm console
                foreach (Tag tag in ApplicationModel.Default.LocalTags)
                {
                    CheckboxListItem newItem = new CheckboxListItem(tag.Name, tag.Id);
                    availableTagsListBox.Items.Add(newItem);
                    availableTagsLookupTable.Add(tag.Id, newItem);
                }

                availableTagsListBox.EndUpdate();
            }
            else
            {
                availableTagsListBox.Hide();
                availableTagsStatusLabel.ForeColor = SystemColors.GrayText;
                availableTagsStatusLabel.Text = "Click the Add Tag button to create a tag.";
                availableTagsStatusLabel.Show();
            }

            ApplicationModel.Default.LocalTags.Changed += Tags_Changed;
        }

        private void LoadAvailableTemplates()
        {
            //[START]SQLdm 9.0 (Ankit Srivastava)- Resolved defect DE44130 - decreased the default timeout to 20
            var connInfo = Settings.Default.ActiveRepositoryConnection.ConnectionInfo;
            connInfo.ConnectionTimeout = 20;
            foreach (AlertTemplate template in RepositoryHelper.GetAlertTemplateList(connInfo))
            //[END]SQLdm 9.0 (Ankit Srivastava)- Resolved defect DE44130 - decreased the default timeout to 20
            {
                cboAlertTemplates.Items.Add(template.TemplateID, template.Name);
            }
            cboAlertTemplates.SelectedIndex = 0;
        }

        private void Tags_Changed(object sender, TagCollectionChangedEventArgs e)
        {
            availableTagsListBox.BeginUpdate();

            switch (e.ChangeType)
            {
                case KeyedCollectionChangeType.Added:
                    foreach (Tag tag in e.Tags.Values)
                    {
                        CheckboxListItem newItem = new CheckboxListItem(tag.Name, tag.Id);
                        availableTagsListBox.Items.Add(newItem, addTagInProgress);
                        availableTagsLookupTable.Add(tag.Id, newItem);
                    }
                    addTagInProgress = false;
                    break;
                case KeyedCollectionChangeType.Removed:
                    foreach (Tag tag in e.Tags.Values)
                    {
                        CheckboxListItem existingItem;
                        if (availableTagsLookupTable.TryGetValue(tag.Id, out existingItem))
                        {
                            availableTagsListBox.Items.Remove(existingItem);
                            availableTagsLookupTable.Remove(tag.Id);
                        }
                    }
                    break;
                case KeyedCollectionChangeType.Replaced:
                    foreach (Tag tag in e.Tags.Values)
                    {
                        CheckboxListItem existingItem;
                        if (availableTagsLookupTable.TryGetValue(tag.Id, out existingItem))
                        {
                            existingItem.Text = tag.Name;
                        }
                    }
                    break;
                case KeyedCollectionChangeType.Cleared:
                    availableTagsListBox.Items.Clear();
                    availableTagsLookupTable.Clear();
                    break;
            }

            availableTagsListBox.EndUpdate();

            if (availableTagsListBox.Items.Count == 0)
            {
                availableTagsStatusLabel.ForeColor = SystemColors.GrayText;
                availableTagsStatusLabel.Text = "Click the Add Tag button to create a tag.";
                availableTagsStatusLabel.Show();
            }
            else
            {
                availableTagsStatusLabel.Hide();
            }
        }
        private void setSelectionCache()
        {
            if (useWindowsAuthenticationRadioButton.Checked)
                prevSelectedAuthOption = AuthOption.WINDOWS;
            else if (useSqlServerAuthenticationRadioButton.Checked && useSqlAunthenticationForAzureCheckBox.Checked)
            {
                prevSelectedAzureProfileId = selectedProfile.Value.Id;
                prevSelectedAuthOption = AuthOption.AZURESQL;
            }
            else
                prevSelectedAuthOption = AuthOption.SQL;
            // 
        }
        private void selectInstancesPage_BeforeDisplay(object sender, EventArgs e)
        {
            if(prevSelectedAuthOption == null)
            {
                setSelectionCache();
                addedInstancesListBox.Items.Clear();
            }
            else
            {
                if(prevSelectedAuthOption == AuthOption.WINDOWS && !useWindowsAuthenticationRadioButton.Checked)
                {
                    setSelectionCache();
                    addedInstancesListBox.Items.Clear();
                }
                else if(prevSelectedAuthOption == AuthOption.AZURESQL && !(useSqlServerAuthenticationRadioButton.Checked && useSqlAunthenticationForAzureCheckBox.Checked))
                {
                    setSelectionCache();
                    addedInstancesListBox.Items.Clear();
                }
                else if(prevSelectedAuthOption == AuthOption.SQL && !useSqlServerAuthenticationRadioButton.Checked)
                {
                    setSelectionCache();
                    addedInstancesListBox.Items.Clear();
                }
                else if(prevSelectedAuthOption == AuthOption.AZURESQL) // Previous is azure nd this time also azure
                {
                    if (prevSelectedAzureProfileId == null)
                        addedInstancesListBox.Items.Clear();
                    else if (prevSelectedAzureProfileId != selectedProfile.Value.Id)
                    {
                        prevSelectedAzureProfileId = selectedProfile.Value.Id;
                        addedInstancesListBox.Items.Clear();
                    }
                }
            }
            // SQLDM-30824 Azure instances are discovered when we switched between SQL to windows auth
            if ((useSqlServerAuthenticationRadioButton.Checked) &&(useSqlAunthenticationForAzureCheckBox.Checked) && (selectedAzureProfileNameTextBox.Text.ToString().Length > 0))
            {                
                BindAzureResources(availableInstancesListBox);
        }
            // SQLdm11 If thread is inactive, then show the cached result else thread will itself update the view.
            else if (!loadAvailableServersWorker.IsBusy) 
            {
                ListAvailableInstances(availableSQLInstancesCache);
            }
            UpdateSelectedInstancesPage();
            selectInstancesPage.AllowMoveNext = addedInstancesListBox.Items.Count > 0 && license != null;
        }

        private void addInstancesButton_Click(object sender, EventArgs e)
        {
            AddAdhocInstances();
            AddSelectedInstances();
            adhocInstancesTextBox.Focus();
            adhocInstancesTextBox_Enter(adhocInstancesTextBox, e);
        }

        private void AddAdhocInstances()
        {
            string adhocInstancesText = adhocInstancesTextBox.Text.Trim();

            if (adhocInstancesText.Length != 0 &&
                string.CompareOrdinal(adhocInstancesText, AdhocInstructionText) != 0)
            {
                SortedList<string, string> alreadyMonitoredInstances = new SortedList<string, string>();
                string[] adhocInstances = adhocInstancesText.Split(new char[] { ';' });

                foreach (string adhocInstance in adhocInstances)
                {
                    string trimmedInstanceName = adhocInstance.Trim();

                    if (trimmedInstanceName.Length > 0)
                    {
                        // Check to see if the local instance is being referenced and resolve
                        // it to the machine name
                        if (string.CompareOrdinal(trimmedInstanceName, ".") == 0 ||
                            string.Compare(trimmedInstanceName, "(local)", true) == 0)
                        {
                            trimmedInstanceName = Environment.MachineName;
                        }

                        string upperCaseInstanceName = trimmedInstanceName.ToUpper();

                        if (existingInstances != null && existingInstances.ContainsKey(upperCaseInstanceName))
                        {
                            if (!alreadyMonitoredInstances.ContainsKey(trimmedInstanceName))
                                alreadyMonitoredInstances.Add(trimmedInstanceName, trimmedInstanceName);
                            else
                                alreadyMonitoredInstances[trimmedInstanceName] = trimmedInstanceName;
                        }
                        else
                        {
                            // TODO: We should probably validate instance name

                            if (GetAddedInstanceIndex(trimmedInstanceName) == -1)
                            {
                                int existingAvailableInstanceIndex = GetAvailableInstanceIndex(trimmedInstanceName);

                                if (existingAvailableInstanceIndex != -1)
                                {
                                    availableInstancesListBox.Items.RemoveAt(existingAvailableInstanceIndex);
                                }

                                ListViewItem newInstanceItem = new ListViewItem(trimmedInstanceName);
                                newInstanceItem.Tag = trimmedInstanceName;
                                addedInstancesListBox.Items.Add(newInstanceItem);
                            }
                        }
                    }
                }

                if (alreadyMonitoredInstances.Count > 0)
                {
                    StringBuilder informationalMessage =
                        new StringBuilder(
                            "The following instances were not added because they are already actively monitored by SQL Diagnostic Manager:\r\n\r\n");

                    for (int i = 0; i < alreadyMonitoredInstances.Count; i++)
                    {
                        informationalMessage.Append(alreadyMonitoredInstances.Keys[i]);
                        informationalMessage.Append("\r\n");
                    }

                    ApplicationMessageBox.ShowInfo(this, informationalMessage.ToString());
                }
            }

            adhocInstancesTextBox.Clear();
            UpdateSelectedInstancesPage();
        }

        private int GetAvailableInstanceIndex(string instanceName)
        {
            if (availableInstancesListBox.Items.Count > 0)
            {
                for (int index = 0; index < availableInstancesListBox.Items.Count; index++)
                {
                    ListViewItem existingInstance = availableInstancesListBox.Items[index] as ListViewItem;

                    if (existingInstance != null && string.Compare(existingInstance.Tag as string, instanceName, true) == 0)
                    {
                        return index;
                    }
                }
            }

            return -1;
        }

        private int GetAddedInstanceIndex(string instanceName)
        {
            if (addedInstancesListBox.Items.Count > 0)
            {
                for (int index = 0; index < addedInstancesListBox.Items.Count; index++)
                {
                    ListViewItem existingInstance = addedInstancesListBox.Items[index] as ListViewItem;

                    if (existingInstance != null && string.Compare(existingInstance.Tag as string, instanceName, true) == 0)
                    {
                        return index;
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// Add selected instances to the Added list and remove them from the Available list
        /// </summary>
        private void AddSelectedInstances()
        {
            List<int> addedIndices = new List<int>();

            foreach (int selectedIndex in availableInstancesListBox.SelectedIndices)
            {
                ListViewItem selectedInstance = availableInstancesListBox.Items[selectedIndex] as ListViewItem;

                if (selectedInstance != null)
                {
                    addedIndices.Insert(0, selectedIndex);
                    addedInstancesListBox.Items.Add(selectedInstance);
                }
            }

            foreach (int addedIndex in addedIndices)
            {
                availableInstancesListBox.Items.RemoveAt(addedIndex);
            }
            if(availableInstancesListBox != null && availableInstancesListBox.Items.Count > 0)
            {
                availableInstancesStatusLabel.Hide();
            }
            else
            {
                availableInstancesStatusLabel.Show();
            }
            //UpdateSelectedInstancesPage();
        }

        private void availableInstancesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSelectedInstancesPage();
        }

        private void addedInstancesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSelectedInstancesPage();
        }

        private void removeInstancesButton_Click(object sender, EventArgs e)
        {
            RemoveSelectedInstances();
        }

        private void RemoveSelectedInstances()
        {
            List<int> removedIndices = new List<int>();

            foreach (int selectedIndex in addedInstancesListBox.SelectedIndices)
            {
                ListViewItem selectedInstance = addedInstancesListBox.Items[selectedIndex] as ListViewItem;

                if (selectedInstance != null)
                {
                    removedIndices.Insert(0, selectedIndex);
                    availableInstancesListBox.Items.Add(selectedInstance);
                }
            }

            foreach (int removedIndex in removedIndices)
            {
                addedInstancesListBox.Items.RemoveAt(removedIndex);
            }
            if (availableInstancesListBox != null && availableInstancesListBox.Items.Count > 0)
            {
                availableInstancesStatusLabel.Hide();
            }
            else
            {
                availableInstancesStatusLabel.Show();
            }

            //UpdateSelectedInstancesPage();
        }

        private void addedInstancesListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            RemoveSelectedInstances();
        }

        private void availableInstancesListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            AddSelectedInstances();
        }

        private void wizardFramework_Cancel(object sender, EventArgs e)
        {
            loadAvailableServersWorker.CancelAsync();
        }

        private void adhocInstancesTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateSelectedInstancesPage(true);
        }

        private void loginNameTextbox_TextChanged(object sender, EventArgs e)
        {
            UpdateConfigureAuthenticationPage();
            useSqlAunthenticationForAzureCheckBox.Enabled = useSqlServerAuthenticationRadioButton.Checked;
        }

        private void enableQueryMonitorTraceCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            UpdateConfigureCollectionPage();
        }

        //START - SQLdm 10.4 (Nikhil Bansal) - Enable top plan only if the SQL Server Version is 2008 or above i.e. Query Monitor Data is Collected using Extended Events
        private void aboveAnd2008CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            bool enabled = aboveAnd2008CheckBox.Enabled && aboveAnd2008CheckBox.Checked;

            this.topPlanLabel.Enabled = enabled;
            this.topPlanSpinner.Enabled = enabled;
            this.topPlanSuffixLabel.Enabled = enabled;
            this.topPlanComboBox.Enabled = enabled;
            this.topPlanTableLayoutPanel.Enabled = enabled;
        }
        //END - SQLdm 10.4 (Nikhil Bansal) - Enable top plan only if the SQL Server Version is 2008 or above i.e. Query Monitor Data is Collected using Extended Events

        private void UpdateConfigureCollectionPage()
        {
            //SQLdm 9.0 (Ankit Srivastava) -- Query Monitoring Improvement -- Add server wizard  -- added two new checkbox entries- start
            belowAnd2005CheckBox.Enabled = enableQueryMonitorTraceCheckBox.Checked;
            aboveAnd2008CheckBox.Enabled = enableQueryMonitorTraceCheckBox.Checked;
            //SQLdm 9.0 (Ankit Srivastava) -- Query Monitoring Improvement -- Add server wizard  -- added two new checkbox entries- end
            poorlyQueriesLabel.Enabled = enableQueryMonitorTraceCheckBox.Checked;
            captureSqlBatchesCheckBox.Enabled = enableQueryMonitorTraceCheckBox.Checked;
            captureSqlStatementsCheckBox.Enabled = enableQueryMonitorTraceCheckBox.Checked;
            captureStoredProceduresCheckBox.Enabled = enableQueryMonitorTraceCheckBox.Checked;

            thresholdWarningLabel.Enabled = enableQueryMonitorTraceCheckBox.Checked;
            durationThresholdLabel.Enabled = enableQueryMonitorTraceCheckBox.Checked;
            durationThresholdSpinner.Enabled = enableQueryMonitorTraceCheckBox.Checked;
            cpuThresholdLabel.Enabled = enableQueryMonitorTraceCheckBox.Checked;
            cpuThresholdSpinner.Enabled = enableQueryMonitorTraceCheckBox.Checked;
            logicalReadsThresholdLabel.Enabled = enableQueryMonitorTraceCheckBox.Checked;
            logicalReadsThresholdSpinner.Enabled = enableQueryMonitorTraceCheckBox.Checked;
            physicalWritesThresholdLabel.Enabled = enableQueryMonitorTraceCheckBox.Checked;
            physicalWritesThresholdSpinner.Enabled = enableQueryMonitorTraceCheckBox.Checked;
            queryMonitorAdvancedOptionsButton.Enabled = enableQueryMonitorTraceCheckBox.Checked;

            //Enable top plan only if the SQL Server Version is 2008 or above i.e. Query Monitor Data is Collected using Extended Events
            bool enabled = enableQueryMonitorTraceCheckBox.Checked && aboveAnd2008CheckBox.Enabled && aboveAnd2008CheckBox.Checked;

            //SQLdm 10.4 (Ruchika Salwan): Enabling Top Plan
            this.topPlanLabel.Enabled = enabled;
            this.topPlanSpinner.Enabled = enabled;
            this.topPlanSuffixLabel.Enabled = enabled;
            this.topPlanComboBox.Enabled = enabled;
            this.topPlanTableLayoutPanel.Enabled = enabled;
            //SQLdm 10.4 (Ruchika Salwan): Enabling Top Plan

            enabled = chkEnableActivityMonitor.Checked;

            chkCaptureAutoGrow.Enabled = enabled;
            captureDeadlocksCheckBox.Enabled = enabled;
            chkCaptureBlocking.Enabled = enabled;

            lblConfigureBlockedProcessThreshold.Enabled = enabled && chkCaptureBlocking.Checked;
            lblBlockedProcessesSpinner.Enabled = enabled && chkCaptureBlocking.Checked;
            spnBlockedProcessThreshold.Enabled = enabled && chkCaptureBlocking.Checked;
        }

        private void wizardFramework_Finish(object sender, EventArgs e)
        {
            bool allowFinish = true;

            if (chkEnableActivityMonitor.Checked)
            {
                if (!chkCaptureAutoGrow.Checked &&
                    !chkCaptureBlocking.Checked &&
                    !chkCaptureAutoGrow.Checked)
                {
                    ApplicationMessageBox.ShowInfo(this,
                                                   "Please specify at least one event type for the Activity Monitor Trace(non-query) to capture.");
                    allowFinish = false;
                }
            }

            if (enableQueryMonitorTraceCheckBox.Checked)
            {

                if (!captureSqlBatchesCheckBox.Checked &&
                    !captureSqlStatementsCheckBox.Checked &&
                    !captureStoredProceduresCheckBox.Checked)
                {
                    ApplicationMessageBox.ShowInfo(this,
                                                   "Please specify at least one event type for the Activity Monitor Trace(queries) to capture.");
                    allowFinish = false;
                }
                else if (durationThresholdSpinner.Value < 500)
                {
                    if (
                        ApplicationMessageBox.ShowWarning(this,
                                                          "A low duration threshold has been specified for the Query Monitor Trace.  A duration threshold greater than 500 ms is recommended to reduce the performance impact of the trace on the monitored SQL Server.  This is especially important on SQL Server 2000 instances, where CPU performance is significantly impacted.  Do you wish to continue with a low duration threshold?",
                                                          ExceptionMessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        allowFinish = false;
                    }
                } // This code will never be reached.
                else if (durationThresholdSpinner.Value == 0 && cpuThresholdSpinner.Value == 0 &&
                         logicalReadsThresholdSpinner.Value == 0 && physicalWritesThresholdSpinner.Value == 0)
                {
                    if (
                        ApplicationMessageBox.ShowWarning(this,
                                                          "No threshold filters have been specified for the Query Monitor Trace. Thresholds are recommended to reduce the performance impact of the trace on the monitored SQL Server. Do you wish to continue without specifying thresholds?",
                                                          ExceptionMessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        allowFinish = false;
                    }
                }
            }

            if (allowFinish)
            {
                BuildAddedInstances();

                if (commitServers)
                {
                    try
                    {
                        ApplicationModel.Default.AddMonitoredSqlServers(addedInstances);
                    }
                    catch (Exception exception)
                    {
                        ApplicationMessageBox.ShowError(this,
                                                        "An error occurred while adding the selected SQL Server instances. Please resolve the following error and try again.",
                                                        exception);
                    }
                }
            }
            else
            {
                DialogResult = DialogResult.None;
            }
        }

        private void BuildAddedInstances()
        {
            addedInstances.Clear();
            int idxForCloudProviderDataTable = 0;
            Dictionary<string, int> cloudProvidersDetail = RepositoryHelper.GetCloudProviders(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);//SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: Get the id and name of all the supported cloud providers
            foreach (ListViewItem selectedInstance in addedInstancesListBox.Items)
            {
                MonitoredSqlServerConfiguration addedInstance = new MonitoredSqlServerConfiguration(selectedInstance.Tag as string);
                addedInstance.ConnectionInfo.ConnectionTimeout = 20;//SQLdm 9.0 (Ankit Srivastava)- Resolved defect DE44130 - decreased the default timeout to 20

                addedInstance.ConnectionInfo.UseIntegratedSecurity = useWindowsAuthenticationRadioButton.Checked;

                if (!addedInstance.ConnectionInfo.UseIntegratedSecurity)
                {
                    addedInstance.ConnectionInfo.UserName = loginNameTextbox.Text;
                    addedInstance.ConnectionInfo.Password = passwordTextbox.Text;
                }
                addedInstance.ConnectionInfo.EncryptData = encryptDataCheckbox.Checked;
                if (addedInstance.ConnectionInfo.EncryptData)
                    addedInstance.ConnectionInfo.TrustServerCertificate = trustServerCertificateCheckbox.Checked;

                addedInstance.ScheduledCollectionInterval = TimeSpan.FromMinutes(Convert.ToInt32(collectionIntervalSpinner.Value));


                //SQLdm 9.0 (Ankit Srivastava) -- Query Monitoring Improvement -- Add server wizard - set the values according to the version info -start
                //SQLdm 10.1 (Barkha Khatri) getting user permissions along with server version
                MonitoredSqlServerMixin serverVersionAndPermission = RepositoryHelper.GetServerVersionAndPermission(addedInstance.ConnectionInfo);
                if (serverVersionAndPermission == null)
                {
                    Log.Error("after calling RepositoryHelper.GetServerVersionAndPermission, serverVersionAndPermission object found null. Initializing object with default value.");
                    serverVersionAndPermission = new MonitoredSqlServerMixin();
                }
                addedInstance.IsUserSysAdmin = serverVersionAndPermission.IsUserSysAdmin;

                Log.InfoFormat("BuildAddedInstances -- This is the server version found -{0}: And This is the server user sys admin value -{1}:", serverVersionAndPermission.ServerVersion, serverVersionAndPermission.IsUserSysAdmin);
                bool isMonitoringEnabled = enableQueryMonitorTraceCheckBox.Checked && serverVersionAndPermission.ServerVersion != null ?
                    (serverVersionAndPermission.ServerVersion.Major >= 10 ? aboveAnd2008CheckBox.Checked : belowAnd2005CheckBox.Checked) : false;
                bool isTraceMonitoringEnabled = serverVersionAndPermission.ServerVersion != null ? (serverVersionAndPermission.ServerVersion.Major >= 10 ? false : true) : false;
                // SQLdm 10.4 (Varun Chopra) Query Monitor using Query Store
                // SQLDM-29633 (Varun Chopra) Query Monitor default option
                bool isQueryStoreMonitoringEnabled = false;
                //SQLDM-28829. Remove queryPlanCollection as default selection. Estimated query plan is selected as default option. 
                //bool queryPlanCollection = serverVersionAndPermission.ServerVersion != null ? (serverVersionAndPermission.ServerVersion.Major >= 10 ? (isTraceMonitoringEnabled ? false : true) : false) : false;//SQLdm 9.1 (Ankit Srivastava) -- Fixed Open functional isssue--changed default value according to the implementaion
                //SQLdm 9.0 (Ankit Srivastava) -- Query Monitoring Improvement -- Add server wizard - set the values according to the version info -end

                //START SQLdm 9.1 (Ankit Srivastava) -- Activity Monitoring with Extended Events -- Add server wizard - set the values according to the version info
                bool is¡ctivityMonitoringEnabled = chkEnableActivityMonitor.Checked && serverVersionAndPermission.ServerVersion != null ?
                    (serverVersionAndPermission.ServerVersion.Major > 10 ? true : false) : false;
                bool isTraceActivityMonitoringEnabled = serverVersionAndPermission.ServerVersion != null ? (serverVersionAndPermission.ServerVersion.Major > 10 ? false : true) : true;//SQLdm 9.1 (Ankit Srivastava) -Resolved rally defect DE44467
                //END SQLdm 9.1 (Ankit Srivastava) -- Activity Monitoring with Extended Events -- Add server wizard - set the values according to the version info
                Log.InfoFormat("BuildAddedInstances -- This is the server version found -{0} hence is¡ctivityMonitoringEnabled={1} and isTraceActivityMonitoringEnabled={2} "
                                                , serverVersionAndPermission.ServerVersion, is¡ctivityMonitoringEnabled, isTraceActivityMonitoringEnabled);
                //SQLdm 10.4 (Ruchika Salwan): Adding Top Plan Parameters

                string providerName12 = grdCloudProviders.Rows[idxForCloudProviderDataTable].Cells[CLOUDPROVIDER].Value.ToString();

                if (providerName12.Trim().ToLower().Contains("rds"))
                {
                    addedInstance.AwsAccessKey = grdCloudProviders.Rows[idxForCloudProviderDataTable].Cells["AWS RDS Access Key"].Value.ToString();
                    addedInstance.AwsSecretKey = grdCloudProviders.Rows[idxForCloudProviderDataTable].Cells["AWS RDS Secret Key"].Value.ToString();
                    //addedInstance.AwsRegionEndpoint = grdCloudProviders.Rows[idxForCloudProviderDataTable].Cells[CLOUDRDSREGION].Value.ToString();
                    string selectedAWSRegion = grdCloudProviders.Rows[idxForCloudProviderDataTable].Cells[CLOUDRDSREGION].Value.ToString();
                    addedInstance.AwsRegionEndpoint = RegionEndpoint.EnumerableAllRegions.Where(item => item.DisplayName == selectedAWSRegion).Select(item => item.SystemName).FirstOrDefault();
                }

                if (providerName12.Trim().ToLower().Contains("rds") && enableQueryMonitorTraceCheckBox.Checked == true)
                {
                    isMonitoringEnabled = true;
                    isTraceMonitoringEnabled = true;
                }
                else
                if (providerName12.Trim().ToLower().Contains("azure") && (enableQueryMonitorTraceCheckBox.Checked == true || enableQueryMonitorTraceCheckBox.Checked == false))
                {
                    isMonitoringEnabled = false;
                    isTraceMonitoringEnabled = false;

                }
                else
                if (providerName12.Trim().ToLower().Contains("linux") && enableQueryMonitorTraceCheckBox.Checked == true)
                {
                    isMonitoringEnabled = true;
                    isTraceMonitoringEnabled = false;

                }
                else if (string.IsNullOrWhiteSpace(providerName12) && enableQueryMonitorTraceCheckBox.Checked == true)
                {
                    isMonitoringEnabled = true;
                    isTraceMonitoringEnabled = false;

                }

                addedInstance.QueryMonitorConfiguration = new QueryMonitorConfiguration(
                    isMonitoringEnabled, //SQLdm 9.0 (Ankit Srivastava) -- Query Monitoring Improvement -- Add server wizard - changed the textbox.checked to the new variable
                    captureSqlBatchesCheckBox.Checked,
                    captureSqlStatementsCheckBox.Checked,
                    captureStoredProceduresCheckBox.Checked,
                    TimeSpan.FromMilliseconds(Convert.ToInt32(durationThresholdSpinner.Value)),
                    TimeSpan.FromMilliseconds(Convert.ToInt32(cpuThresholdSpinner.Value)),
                    Convert.ToInt32(logicalReadsThresholdSpinner.Value),
                    Convert.ToInt32(physicalWritesThresholdSpinner.Value),
                    new FileSize(1024), 2, 1000, advancedQueryFilterConfiguration,
                    //SQLdm 9.0 (Ankit Srivastava) -- Query Monitoring Improvement -- Add server wizard  passed default values of  the traceenabled,collectQueryPlans to the constructor -start
                    isTraceMonitoringEnabled,
                    false, true,//SQLDM - 28829
                    Convert.ToInt32(this.topPlanSpinner.Value),
                    Convert.ToInt32(this.topPlanComboBox.Value),
                    isQueryStoreMonitoringEnabled);
                //SQLdm 9.0 (Ankit Srivastava) -- Query Monitoring Improvement -- Add server wizard  passed default values of  the traceenabled,collectQueryPlans to the constructor - end

                addedInstance.ActivityMonitorConfiguration = new ActivityMonitorConfiguration(
                    is¡ctivityMonitoringEnabled,
                    captureDeadlocksCheckBox.Checked,
                    chkCaptureBlocking.Checked,
                    chkCaptureAutoGrow.Checked,
                    (int)spnBlockedProcessThreshold.Value,
                    new FileSize(1024), 2, 1000,
                    advancedQueryFilterConfiguration,
                    isTraceActivityMonitoringEnabled);//SQLdm 9.1 (Ankit Srivastava) -- Activity Monitoring with Extended Events -- Add server wizard  passed default value of  the traceMonitoringEnabled

                addedInstance.ActiveWaitsConfiguration = new ActiveWaitsConfiguration(-1);

                addedInstance.Tags.AddRange(GetCheckedTagIds());

                addedInstance.AlertTemplateID = (int)cboAlertTemplates.SelectedItem.DataValue;

                addedInstance.BaselineConfiguration = baselineConfiguration1.BaselineConfig;

                //START: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: Added a field for signifying the cloud provider id
                string providerName = grdCloudProviders.Rows[idxForCloudProviderDataTable].Cells[CLOUDPROVIDER].Value.ToString();
                if (cloudProvidersDetail.ContainsKey(providerName))
                    addedInstance.CloudProviderId = cloudProvidersDetail[providerName];
                //END: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: Added a field for signifying the cloud provider id
                var wmi = addedInstance.WmiConfig;
                // SQLdm 10.3 (Varun Chopra) SQLDM-28770 - Support OS Collection for mixed instances with Linux
                if (providerName.Trim().ToLower().Contains("rds") || providerName.Trim().ToLower().Contains("azure"))
                {
                    wmi.OptionWmiNoneDisabled = !optionWmiNone.Checked;
                    wmi.OleAutomationDisabled = true;
                    wmi.DirectWmiEnabled = false;
                    wmi.DirectWmiConnectAsCollectionService = optionWmiCSCreds.Checked;
                    wmi.DirectWmiUserName = directWmiUserName.Text;
                    wmi.DirectWmiPassword = directWmiPassword.Text;
                }
                else
                if (providerName.Trim().ToLower().Contains("linux"))
                {
                    // Disable OS Collection for Linux Instances
                    wmi.OleAutomationDisabled = true;
                    wmi.DirectWmiEnabled = false;
                    wmi.DirectWmiConnectAsCollectionService = optionWmiCSCreds.Checked;
                    wmi.DirectWmiUserName = string.Empty;
                    wmi.DirectWmiPassword = string.Empty;
                }
                else
                {
                    wmi.OleAutomationDisabled = !optionWmiOleAutomation.Checked;
                    wmi.DirectWmiEnabled = optionWmiDirect.Checked;
                    wmi.DirectWmiConnectAsCollectionService = optionWmiCSCreds.Checked;
                    wmi.DirectWmiUserName = directWmiUserName.Text;
                    wmi.DirectWmiPassword = directWmiPassword.Text;
                }
                addedInstances.Add(addedInstance);
                idxForCloudProviderDataTable++;
            }
        }

        private IList<int> GetCheckedTagIds()
        {
            List<int> ids = new List<int>();

            foreach (CheckboxListItem listItem in availableTagsListBox.CheckedItems)
            {
                ids.Add((int)listItem.Tag);
            }

            return ids;
        }

        private void adhocInstancesTextBox_Enter(object sender, EventArgs e)
        {
            if (string.CompareOrdinal(AdhocInstructionText, adhocInstancesTextBox.Text) == 0)
            {
                adhocInstancesTextBox.Clear();
            }

            adhocInstancesTextBox.ForeColor = SystemColors.WindowText;
        }

        private void AddServersWizard_HelpButtonClicked(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (e != null) e.Cancel = true;
            if (this.wizardFramework.SelectedPage == this.welcomePage) welcomePage_HelpRequested(null, null);
            if (this.wizardFramework.SelectedPage == this.configureAuthenticationPage) configureAuthenticationPage_HelpRequested(null, null);
            if (this.wizardFramework.SelectedPage == this.selectInstancesPage) selectInstancesPage_HelpRequested(null, null);
            if (this.wizardFramework.SelectedPage == this.configureCollectionPage) configureCollectionPage_HelpRequested(null, null);
            if (this.wizardFramework.SelectedPage == this.selectTagsPage) selectTagsPage_HelpRequested(null, null);
            if (this.wizardFramework.SelectedPage == this.baselineConfiguration) baselineConfiguration_HelpRequested(null, null);
            if (this.wizardFramework.SelectedPage == this.wmiConfigPage) wmiConfigPage_HelpRequested(null, null);
            if (this.wizardFramework.SelectedPage == this.chooseCloudProviderPage) chooseCloudProviderPage_HelpRequested(null, null);
        }

        private void AddServersWizard_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            if (this.wizardFramework.SelectedPage == this.welcomePage) welcomePage_HelpRequested(sender, hlpevent);
            if (this.wizardFramework.SelectedPage == this.configureAuthenticationPage) configureAuthenticationPage_HelpRequested(sender, hlpevent);
            if (this.wizardFramework.SelectedPage == this.selectInstancesPage) selectInstancesPage_HelpRequested(sender, hlpevent);
            if (this.wizardFramework.SelectedPage == this.configureCollectionPage) configureCollectionPage_HelpRequested(sender, hlpevent);
            if (this.wizardFramework.SelectedPage == this.selectTagsPage) selectTagsPage_HelpRequested(sender, hlpevent);
            if (this.wizardFramework.SelectedPage == this.baselineConfiguration) baselineConfiguration_HelpRequested(null, null);
            if (this.wizardFramework.SelectedPage == this.wmiConfigPage) wmiConfigPage_HelpRequested(sender, hlpevent);
            if (this.wizardFramework.SelectedPage == this.chooseCloudProviderPage) chooseCloudProviderPage_HelpRequested(null, null);
        }

        private void welcomePage_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            if (hlpevent != null) hlpevent.Handled = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.AddServersWizard);
        }

        private void configureAuthenticationPage_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            if (hlpevent != null) hlpevent.Handled = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.AddServersWizardConfig);
        }

        private void selectInstancesPage_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            if (hlpevent != null) hlpevent.Handled = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.AddServersWizardSelect);
        }

        private void configureCollectionPage_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            if (hlpevent != null) hlpevent.Handled = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.AddServersWizardCollect);
        }

        private void selectTagsPage_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            if (hlpevent != null) hlpevent.Handled = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.TagsAddServersSelectTags);
        }

        private void wmiConfigPage_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            if (hlpevent != null) hlpevent.Handled = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.AddServersWizardWMI);
        }

        private void selectInstancesPage_BeforeMoveNext(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (license != null && !license.IsUnlimited)
            {
                if ((license.Expiration <= DateTime.Now) || (addedInstancesListBox.Items.Count + existingInstanceCount > license.LicensedServers))
                {
                    string msg;
                    if (license.Expiration <= DateTime.Now)
                    {
                        if (license.Expiration > DateTime.MinValue)
                        {
                            msg = String.Format("The SQLDM license expired as of {0}", license.Expiration);
                        }
                        else
                        {
                            msg = String.Format("No valid SQLDM license is available.");
                        }
                    }
                    else
                    {
                        if (existingInstanceCount == license.LicensedServers)
                        {
                            msg = "The license does not allow any additional servers to be monitored.";
                        }
                        else
                        {
                            if (existingInstanceCount >= license.LicensedServers)
                            {
                                msg =
                                   string.Format(
                                       "You are currently monitoring more servers than the license allows.  Please reduce your server count to {0}.",
                                       license.LicensedServers);
                            }
                            else
                            {
                                msg =
                                    string.Format(
                                        "You have selected more servers than the license allows.  Please select no more than {0}.",
                                        license.LicensedServers - existingInstanceCount);
                            }
                        }
                    }
                    ApplicationMessageBox.ShowError(this, msg);
                    e.Cancel = true;
                }
            }
        }

        private void encryptDataCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            trustServerCertificateCheckbox.Enabled = encryptDataCheckbox.Checked;
        }

        private void hideWelcomePageCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.HideAddServersWizardWelcomePage = hideWelcomePageCheckBox.Checked;
        }

        private void AddServersWizard_Load(object sender, EventArgs e)
        {
            hideWelcomePageCheckBox.Checked = Settings.Default.HideAddServersWizardWelcomePage;

            if (Settings.Default.HideAddServersWizardWelcomePage)
            {
                wizardFramework.GoNext();
            }
        }

        private void queryMonitorAdvancedOptionsButton_Click(object sender, EventArgs e)
        {
            AdvancedQueryFilterConfigurationDialog dialog =
                new AdvancedQueryFilterConfigurationDialog(advancedQueryFilterConfiguration, "Query Monitor");
            dialog.ShowDialog(this);
        }

        private void addTagButton_Click(object sender, EventArgs e)
        {
            addTagInProgress = true;

            AddTagDialog dialog = new AddTagDialog();

            if (dialog.ShowDialog(this) != DialogResult.OK)
            {
                addTagInProgress = false;
            }
        }
            private void configureAuthenticationPage_BeforeDisplay(object sender, EventArgs e)
        {
        	// SQLDM-30786 - Add server wizard - Azure Discovery Settings Tab gets disabled on back button.
            // SQLDM-30789 - 'Azure Discovery Instance' should be disabled for Windows Authentication
            useSqlAunthenticationForAzureCheckBox.Enabled = useSqlServerAuthenticationRadioButton.Checked;
            addAzureServerButton.Enabled = useSqlAunthenticationForAzureCheckBox.Checked && useSqlServerAuthenticationRadioButton.Checked;
            if (useSqlAunthenticationForAzureCheckBox.Checked)
            {
                if (selectedAzureProfileNameTextBox.Text.ToString().Length > 0)
                    configureAuthenticationPage.AllowMoveNext = true;
                else
                    configureAuthenticationPage.AllowMoveNext = false;
            }
            else
            {
                configureAuthenticationPage.AllowMoveNext = true;
            }
        }

        private void configureCollectionPage_BeforeDisplay(object sender, EventArgs e)
        {

        }

        private void selectTagsPage_BeforeDisplay(object sender, EventArgs e)
        {

        }

        private void baselineConfiguration_BeforeDisplay(object sender, EventArgs e)
        {
            // init?
            baselineConfiguration1.IsMultiEdit = true;
            baselineConfiguration1.Init();
        }

        private void baselineConfiguration_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            if (hlpevent != null)
                hlpevent.Handled = true;

            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.AddServersWizardBaseline);
        }

        private void baselineConfiguration_BeforeMoveNext(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (baselineConfiguration1.CheckForErrors())
            {
                ApplicationMessageBox.ShowError(this, baselineConfiguration1.ErrorMessage);
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Returns true if Linux Server Present
        /// </summary>
        /// <param name="nonLinuxPresent">Set if any Non Linux Server Present in List</param>
        /// <returns>Returns true if Linux Server Present</returns>
        /// <remarks>
        /// SQLdm 10.3 (Varun Chopra) SQLDM-28770 - Support OS Collection for mixed instances with Linux
        /// </remarks>
        private bool CheckForLinuxServers(out bool nonLinuxPresent, out bool ischeckNext)
        {
            nonLinuxPresent = false;
            ischeckNext = false;
            bool linuxEnabled = false;
            int count = 0;
            foreach (var row in grdCloudProviders.Rows)
            {
                foreach (var row1 in grdCloudProviders.Rows)
                {
                    if (string.Equals(NOT_CLOUD_PROVIDER_STRING, (string)row1.Cells[CLOUDPROVIDER].Value, StringComparison.InvariantCultureIgnoreCase) && count == 0)
                    {
                        MessageBox.Show("Please select 'Server Type' for all the added instances.");
                        ischeckNext = true;
                        count++;
                        break;
                    }
                }
                if (string.Equals(LINUX, (string)row.Cells[CLOUDPROVIDER].Value, StringComparison.InvariantCultureIgnoreCase))
                {
                    linuxEnabled = true;
                }
                else
                {
                    nonLinuxPresent = true;
                }
                if (linuxEnabled && nonLinuxPresent)
                {
                    break;
                }
            }
            return linuxEnabled;
        }

        // SQLdm 10.3 (Varun Chopra) Linux Support UI
        private void chooseCloudProviderPage_BeforeMoveNext(object sender, CancelEventArgs e)
        {
            // SQLdm 10.3 (Varun Chopra) SQLDM-28770 - Support OS Collection for mixed instances with Linux
            var wmiConfigText = WMICONFIG;
            bool nonLinuxPresent;
            bool ischeckNext;
            bool linuxEnabled = CheckForLinuxServers(out nonLinuxPresent, out ischeckNext);
            if (ischeckNext)
            {
                e.Cancel = true;
            }
            // SQLdm 10.3 (Varun Chopra) Linux Support - Disable OS Metrics UI
            if (linuxEnabled && !nonLinuxPresent)
            {
                if (!optionWmiNone.Checked)
                    optionWmiNone.Checked = true;
                if (optionWmiDirect.Enabled)
                    optionWmiDirect.Enabled = false;
                if (optionWmiOleAutomation.Enabled)
                    optionWmiOleAutomation.Enabled = false;
                if (optionWmiCSCreds.Enabled)
                    optionWmiCSCreds.Enabled = false;
            }
            else
            {
                if (linuxEnabled)
                {
                    // SQLdm 10.3 (Varun Chopra) SQLDM-28770 - Support OS Collection for mixed instances with Linux
                    wmiConfigText = WMICONFIGFORNONLINUX;
                }
                if (!optionWmiDirect.Enabled)
                    optionWmiDirect.Enabled = true;
                if (!optionWmiOleAutomation.Enabled)
                    optionWmiOleAutomation.Enabled = true;
                if (!optionWmiCSCreds.Enabled)
                    optionWmiCSCreds.Enabled = true;
            }
            // Set WMI Configuration Group box text based on linux instance is present
            groupBox4.Text = wmiConfigText;
        }

        private void optionWmiChanged(object sender, EventArgs e)
        {
            var directEnabled = optionWmiDirect.Checked;
            optionWmiCSCreds.Enabled = directEnabled;

            if (directEnabled)
                directEnabled = !optionWmiCSCreds.Checked;

            directWmiPassword.Enabled = directWmiUserName.Enabled = directEnabled;

            bool allowMoveNext = true;
            if (directEnabled && !optionWmiCSCreds.Checked)
            {
                if (String.IsNullOrEmpty(directWmiPassword.Text) || String.IsNullOrEmpty(directWmiPassword.Text))
                    allowMoveNext = false;
            }
            wmiConfigPage.AllowMoveNext = allowMoveNext;
        }

        private void wmiConfigPage_BeforeDisplay(object sender, EventArgs e)
        {
            optionWmiChanged(this, EventArgs.Empty);
        }

        //[START] SQLdm 9.0 (Ankit Srivastava) -- Resolved defect DE44129 -- Add server wizard - New method to check if user has not chosen any of the SQL server versions.
        private void configureCollectionPage_BeforeMoveNext(object sender, CancelEventArgs e)
        {

            if (!belowAnd2005CheckBox.Checked &&
                !aboveAnd2008CheckBox.Checked)
            {
                ApplicationMessageBox.ShowInfo(this,
                                               "Please specify at least one type of SQL Server version to monitor the queries.");
                e.Cancel = true;

            }
            else
            {
                e.Cancel = false;
            }
        }
        //[END] SQLdm 9.0 (Ankit Srivastava) --  Resolved defect DE44129 -- Add server wizard - New method to check if user has not chosen any of the SQL server versions.

        private void wmiConfigPage_BeforeMoveNext(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // SQLdm 10.3 (Varun Chopra) SQLDM-28770 - Support OS Collection for mixed instances with Linux
            bool nonLinuxPresent;
            bool ischeckNext;
            // SQLdm 10.3 (Varun Chopra) Linux Support - Disable OS Metrics
            bool linuxEnabled = CheckForLinuxServers(out nonLinuxPresent, out ischeckNext);
            if (ischeckNext)
            {
                e.Cancel = true;
            }
            if (optionWmiNone.Checked)
            {
                var message = DISABLEOSWARNINGMESSAGE;
                if (linuxEnabled)
                {
                    if (!nonLinuxPresent)
                    {
                        return;
                    }
                    else
                    {
                        // Updated message to denote disable os for non linux instances only since its not applicable on linux instances
                        message = DISABLEOSNONLINUXWARNINGMESSAGE;
                    }
                }
                if (ApplicationMessageBox.ShowWarning(this, message, ExceptionMessageBoxButtons.OKCancel) == DialogResult.Cancel)
                    e.Cancel = true;
            }
            else if (optionWmiDirect.Checked && !optionWmiCSCreds.Checked)
            {
                if (string.IsNullOrEmpty(directWmiUserName.Text) || string.IsNullOrEmpty(directWmiPassword.Text))
                {
                    ApplicationMessageBox.ShowError(this, "A user and password are required when electing to not use the SQLDM Collection Service account when establishing a direct WMI connection.");
                    e.Cancel = true;
                }
            }
        }

        private void chkEnableActivityMonitor_CheckedChanged(object sender, EventArgs e)
        {
            UpdateConfigureCollectionPage();
        }

        private void chkCaptureBlocking_CheckedChanged(object sender, EventArgs e)
        {
            UpdateConfigureCollectionPage();
        }

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
            //AutoScaleFontResolutionHelper.Default.AutoScaleControl(this, AutoScaleFontResolutionHelper.ControlType.Container);
        }

        //Saurabh - SQLDM-30848 - UX-Modernization, PRD 4.2
        protected void ScaleControlsAsPerResolution()
        {
            scaleControls();
            scaleFonts();

            if (AutoScaleSizeHelper.isLargeSize)
            {
                //welcome page.
                this.ClientSize = new System.Drawing.Size(1000, 1000);
                this.welcomePage.Width += 200;
                this.introductoryTextLabel1.Width += 200;
                this.introductoryTextLabel2.Width += 200;
                //Configuration Page
                this.informationBox1.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Top;
                this.informationBox1.Height += 20;
                this.configureAuthenticationPage.Scale(new SizeF(1.0F, 1.33F));
                this.addAzureServerButton.Height -= 10;
                //Instances Page
                this.addedInstancesListBox.Scale(new SizeF(1.00F, 0.70F));
                this.addedInstancesListBox.Location = new Point(this.addedInstancesListBox.Location.X + 200, this.addedInstancesListBox.Location.Y);
                this.removeInstancesButton.Scale(new SizeF(0.75F, 0.75F));
                this.removeInstancesButton.Location = new Point(this.removeInstancesButton.Location.X + 180, this.removeInstancesButton.Location.Y + 60);
                this.addInstancesButton.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                this.removeInstancesButton.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                this.addInstancesButton.Scale(new SizeF(0.75F, 0.75F));
                this.addInstancesButton.Location = new Point(this.addInstancesButton.Location.X + 180, this.addInstancesButton.Location.Y + 40);
                this.availableInstancesListBox.Scale(new SizeF(1.00F, 0.70F));
                this.availableInstancesStatusLabel.Scale(new SizeF(1.00F, 0.70F));
                this.availableInstancesStatusLabel.Location = new Point(this.availableInstancesStatusLabel.Location.X, this.availableInstancesStatusLabel.Location.Y + 50);
                this.availableInstancesStatusLabel.Size = new Size(this.availableInstancesStatusLabel.Width, this.availableInstancesStatusLabel.Height - 30);
                this.loadingServersProgressControl.Scale(new SizeF(1.00F, 0.70F));
                this.availableLicensesLabel.Location = new Point(this.availableLicensesLabel.Location.X + 200, this.availableLicensesLabel.Location.Y - 50);
                this.availableInstancesLabel.Location = new Point(this.availableInstancesLabel.Location.X, this.availableInstancesLabel.Location.Y - 50);
                this.availableInstancesStatusLabel.Location = new Point(this.availableInstancesStatusLabel.Location.X, this.availableInstancesStatusLabel.Location.Y - 50);
                this.addedInstancesLabel.Location = new Point(this.addedInstancesLabel.Location.X + 200, this.addedInstancesLabel.Location.Y - 50);
                this.adhocInstancesTextBox.Location = new Point(this.adhocInstancesTextBox.Location.X, this.adhocInstancesTextBox.Location.Y - 50);
                this.adhocInstancesTextBox.Width += 30;
                this.addedInstancesListBox.Width += 30;
                this.addedInstancesLabel.Width += 30;
                this.availableInstancesListBox.Width += 30;
                this.availableInstancesLabel.Width += 30;
                this.availableInstancesStatusLabel.Width += 30;
                this.addInstancesButton.Width = this.removeInstancesButton.Width;
                //configuration collection page
                this.configureCollectionPage.Size = new System.Drawing.Size(900, 900);
                this.featuresGroupBox.Scale(new SizeF(1.75F, 1.5F));
                this.dataCollectionGroupBox.Scale(new SizeF(1.75F, 1.5F));
                this.topPlanComboBox.Scale(new SizeF(0.55F, 1.0F));
                this.topPlanComboBox.Width += 20;
                this.queryMonitorAdvancedOptionsButton.Scale(new SizeF(0.5F, 0.5F));
                this.queryMonitorAdvancedOptionsButton.Size = new Size(this.queryMonitorAdvancedOptionsButton.Width - 20, this.queryMonitorAdvancedOptionsButton.Height - 10);
                this.queryMonitorAdvancedOptionsButton.Location = new Point(this.queryMonitorAdvancedOptionsButton.Location.X + 250, this.queryMonitorAdvancedOptionsButton.Location.Y + 195);
                this.selectedAzureProfileNameTextBox.Location = new Point(this.selectedAzureProfileNameTextBox.Location.X + 20, this.selectedAzureProfileNameTextBox.Location.Y + 20);
                this.selectedAzureProfileLbl.Location = new Point(this.selectedAzureProfileLbl.Location.X, this.selectedAzureProfileLbl.Location.Y + 20);
                //OS Metric Page(WMI collection)
                this.wmiConfigPage.Scale(new SizeF(1.0F, 1.33F));
                //tag page
                this.groupBox3.Scale(new SizeF(1.2F, 1.2F));
                this.groupBox2.Scale(new SizeF(1.2F, 1.2F));
                this.addTagButton.Scale(new SizeF(0.75F, 0.5F));
                this.addTagButton.Location = new Point(this.addTagButton.Location.X + 150, this.addTagButton.Location.Y + 150);
                //Cloud provider
                this.grdCloudProviders.Width += 290;
                return;
            }
            if (AutoScaleSizeHelper.isXLargeSize)
            {
                //welcome page.
                this.ClientSize = new System.Drawing.Size(1000, 1000);
                this.welcomePage.Size = new System.Drawing.Size(550, 950);
                this.introductoryTextLabel1.Size = new System.Drawing.Size(437, 95);
                this.introductoryTextLabel2.Location = new System.Drawing.Point(20, 95);
                this.introductoryTextLabel2.Size = new System.Drawing.Size(500, 600);
                this.introductoryTextLabel2.MaximumSize = new System.Drawing.Size(500, 600);
                //Configuration Page
                this.informationBox1.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Top;
                this.informationBox1.Height += 20;
                this.configureAuthenticationPage.Scale(new SizeF(1.0F, 1.33F));
                this.addAzureServerButton.Height -= 10;
                //instances page
                this.addedInstancesListBox.Scale(new SizeF(1.00F, 0.70F));
                this.addedInstancesListBox.Location = new Point(this.addedInstancesListBox.Location.X + 200, this.addedInstancesListBox.Location.Y);
                this.removeInstancesButton.Scale(new SizeF(0.75F, 0.75F));
                this.removeInstancesButton.Location = new Point(this.removeInstancesButton.Location.X + 180, this.removeInstancesButton.Location.Y + 60);
                this.addInstancesButton.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                this.removeInstancesButton.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                this.addInstancesButton.Scale(new SizeF(0.75F, 0.75F));
                this.addInstancesButton.Location = new Point(this.addInstancesButton.Location.X + 180, this.addInstancesButton.Location.Y + 40);
                this.availableInstancesListBox.Scale(new SizeF(1.00F, 0.70F));
                this.availableInstancesStatusLabel.Scale(new SizeF(1.00F, 0.70F));
                this.availableInstancesStatusLabel.Location = new Point(this.availableInstancesStatusLabel.Location.X, this.availableInstancesStatusLabel.Location.Y + 50);
                this.availableInstancesStatusLabel.Size = new Size(this.availableInstancesStatusLabel.Width, this.availableInstancesStatusLabel.Height - 30);
                this.loadingServersProgressControl.Scale(new SizeF(1.00F, 0.70F));
                this.availableLicensesLabel.Location = new Point(this.availableLicensesLabel.Location.X+ 200, this.availableLicensesLabel.Location.Y - 50);
                this.availableInstancesLabel.Location = new Point(this.availableInstancesLabel.Location.X, this.availableInstancesLabel.Location.Y - 50);
                this.availableInstancesStatusLabel.Location = new Point(this.availableInstancesStatusLabel.Location.X, this.availableInstancesStatusLabel.Location.Y - 50);
                this.addedInstancesLabel.Location = new Point(this.addedInstancesLabel.Location.X + 200, this.addedInstancesLabel.Location.Y - 50);
                this.adhocInstancesTextBox.Location = new Point(this.adhocInstancesTextBox.Location.X, this.adhocInstancesTextBox.Location.Y - 50);
                this.adhocInstancesTextBox.Width += 30;
                this.addedInstancesListBox.Width += 30;
                this.addedInstancesLabel.Width += 30;
                this.availableInstancesListBox.Width += 30;
                this.availableInstancesLabel.Width += 30;
                this.availableInstancesStatusLabel.Width += 30;
                this.addInstancesButton.Width = this.removeInstancesButton.Width;
                //configuration collection page
                this.configureCollectionPage.Size = new System.Drawing.Size(900, 900);
                this.featuresGroupBox.Scale(new SizeF(1.75F, 1.5F));
                this.dataCollectionGroupBox.Scale(new SizeF(1.75F, 1.5F));
                this.topPlanComboBox.Scale(new SizeF(0.55F, 1.0F));
                this.topPlanComboBox.Width += 20;
                this.queryMonitorAdvancedOptionsButton.Scale(new SizeF(0.5F, 0.5F));
                this.queryMonitorAdvancedOptionsButton.Size = new Size(this.queryMonitorAdvancedOptionsButton.Width - 20, this.queryMonitorAdvancedOptionsButton.Height - 10);
                this.queryMonitorAdvancedOptionsButton.Location = new Point(this.queryMonitorAdvancedOptionsButton.Location.X + 280, this.queryMonitorAdvancedOptionsButton.Location.Y + 210);
                this.selectedAzureProfileNameTextBox.Location = new Point(this.selectedAzureProfileNameTextBox.Location.X + 20, this.selectedAzureProfileNameTextBox.Location.Y + 20);
                this.selectedAzureProfileLbl.Location = new Point(this.selectedAzureProfileLbl.Location.X, this.selectedAzureProfileLbl.Location.Y + 20);
                //OS Metric Page(WMI collection)
                this.wmiConfigPage.Scale(new SizeF(1.0F, 1.33F));
                //tag page
                this.groupBox3.Scale(new SizeF(1.2F, 1.2F));
                this.groupBox2.Scale(new SizeF(1.2F, 1.2F));
                this.addTagButton.Scale(new SizeF(0.75F, 0.5F));
                this.addTagButton.Location = new Point(this.addTagButton.Location.X + 150, this.addTagButton.Location.Y + 200);
                //Cloud Provider Page
                //grdCloudProviders.DisplayLayout.Bands[0].Columns["Server Name"].Width = 200;
                //grdCloudProviders.DisplayLayout.Bands[0].Columns[CLOUDPROVIDER].Width = 200;
                //grdCloudProviders.DisplayLayout.Bands[0].Columns[AWSRDSAccessKey].Width = 150;
                //grdCloudProviders.DisplayLayout.Bands[0].Columns[AWSRDSSecretKey].Width = 250;
                //grdCloudProviders.DisplayLayout.Bands[0].Columns[CLOUDRDSREGION].Width = 150;
                this.grdCloudProviders.Width += 590;
                return;
            }
            if (AutoScaleSizeHelper.isXXLargeSize)
            {
                //welcome page.
                this.ClientSize = new System.Drawing.Size(1000, 1100);
                this.welcomePage.Size = new System.Drawing.Size(650, 950);
                this.introductoryTextLabel1.Size = new System.Drawing.Size(500, 95);
                this.introductoryTextLabel2.Location = new System.Drawing.Point(20, 95);
                this.introductoryTextLabel2.Size = new System.Drawing.Size(550, 600);
                this.introductoryTextLabel2.MaximumSize = new System.Drawing.Size(550, 600);
                //Configuration Page
                this.informationBox1.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Top;
                this.informationBox1.Height += 20;
                this.configureAuthenticationPage.Scale(new SizeF(1.0F, 1.33F));
                this.addAzureServerButton.Height -= 10;
                //instances page
                this.addedInstancesListBox.Scale(new SizeF(1.00F, 0.70F));
                this.addedInstancesListBox.Location = new Point(this.addedInstancesListBox.Location.X + 200, this.addedInstancesListBox.Location.Y);
                this.removeInstancesButton.Scale(new SizeF(0.75F, 0.75F));
                this.removeInstancesButton.Location = new Point(this.removeInstancesButton.Location.X + 180, this.removeInstancesButton.Location.Y + 60);
                this.addInstancesButton.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                this.removeInstancesButton.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                this.addInstancesButton.Scale(new SizeF(0.75F, 0.75F));
                this.addInstancesButton.Location = new Point(this.addInstancesButton.Location.X + 180, this.addInstancesButton.Location.Y + 40);
                this.availableInstancesListBox.Scale(new SizeF(1.00F, 0.70F));
                this.availableInstancesStatusLabel.Scale(new SizeF(1.00F, 0.70F));
                this.availableInstancesStatusLabel.Location = new Point(this.availableInstancesStatusLabel.Location.X, this.availableInstancesStatusLabel.Location.Y + 50);
                this.availableInstancesStatusLabel.Size = new Size(this.availableInstancesStatusLabel.Width, this.availableInstancesStatusLabel.Height - 30);
                this.loadingServersProgressControl.Scale(new SizeF(1.00F, 0.70F));
                this.availableLicensesLabel.Location = new Point(this.availableLicensesLabel.Location.X + 200, this.availableLicensesLabel.Location.Y - 50);
                this.availableInstancesLabel.Location = new Point(this.availableInstancesLabel.Location.X, this.availableInstancesLabel.Location.Y -50);
                this.availableInstancesStatusLabel.Location = new Point(this.availableInstancesStatusLabel.Location.X, this.availableInstancesStatusLabel.Location.Y -50);
                this.addedInstancesLabel.Location = new Point(this.addedInstancesLabel.Location.X + 200, this.addedInstancesLabel.Location.Y - 50);
                this.adhocInstancesTextBox.Location = new Point(this.adhocInstancesTextBox.Location.X, this.adhocInstancesTextBox.Location.Y -50);
                this.adhocInstancesTextBox.Width += 30;
                this.addedInstancesListBox.Width += 30;
                this.addedInstancesLabel.Width += 30;
                this.availableInstancesListBox.Width += 30;
                this.availableInstancesLabel.Width += 30;
                this.availableInstancesStatusLabel.Width += 30;
                this.addInstancesButton.Width = this.removeInstancesButton.Width;
                //configuration collection page
                this.configureCollectionPage.Size = new System.Drawing.Size(900, 900);
                this.featuresGroupBox.Scale(new SizeF(1.45F, 1.45F));
                this.dataCollectionGroupBox.Scale(new SizeF(1.45F, 1.45F));
                this.topPlanComboBox.Scale(new SizeF(0.75F, 1.0F));
                this.topPlanComboBox.Width -= 20;
                this.queryMonitorAdvancedOptionsButton.Scale(new SizeF(0.5F, 0.5F));
                this.queryMonitorAdvancedOptionsButton.Size = new Size(this.queryMonitorAdvancedOptionsButton.Width - 20, this.queryMonitorAdvancedOptionsButton.Height - 10);
                this.queryMonitorAdvancedOptionsButton.Location = new Point(this.queryMonitorAdvancedOptionsButton.Location.X + 370, this.queryMonitorAdvancedOptionsButton.Location.Y + 245);
                this.selectedAzureProfileNameTextBox.Location = new Point(this.selectedAzureProfileNameTextBox.Location.X + 20, this.selectedAzureProfileNameTextBox.Location.Y + 20);
                this.selectedAzureProfileLbl.Location = new Point(this.selectedAzureProfileLbl.Location.X, this.selectedAzureProfileLbl.Location.Y + 20);
                //OS Metric Page(WMI collection)
                this.wmiConfigPage.Scale(new SizeF(1.0F, 1.33F));
                //tag page
                this.groupBox3.Scale(new SizeF(1.2F, 1.2F));
                this.groupBox2.Scale(new SizeF(1.2F, 1.2F));
                this.addTagButton.Scale(new SizeF(0.75F, 0.5F));
                this.addTagButton.Location = new Point(this.addTagButton.Location.X + 150, this.addTagButton.Location.Y + 200);
                //Cloud Provider
                this.grdCloudProviders.Width += 890;
                return;
            }
        }

        private void scaleControls()
        {
           AutoScaleSizeHelper.Default.AutoScaleControl(this.configureAuthenticationPage, AutoScaleSizeHelper.ControlType.Form);
           AutoScaleSizeHelper.Default.AutoScaleControl(this.selectInstancesPage, AutoScaleSizeHelper.ControlType.Form);
           AutoScaleSizeHelper.Default.AutoScaleControl(this.chooseCloudProviderPage, AutoScaleSizeHelper.ControlType.Form);
           AutoScaleSizeHelper.Default.AutoScaleControl(this.configureCollectionPage, AutoScaleSizeHelper.ControlType.Form);
           AutoScaleSizeHelper.Default.AutoScaleControl(this.wmiConfigPage, AutoScaleSizeHelper.ControlType.Form);
           AutoScaleSizeHelper.Default.AutoScaleControl(this.selectTagsPage, AutoScaleSizeHelper.ControlType.Form);
        }

        private void scaleFonts()
        {
            AutoScaleFontResolutionHelper.Default.AutoScaleFont(this.welcomePage, AutoScaleFontHelper.ControlType.Container);
            AutoScaleFontResolutionHelper.Default.AutoScaleFont(this.configureAuthenticationPage, AutoScaleFontHelper.ControlType.Container);
            AutoScaleFontResolutionHelper.Default.AutoScaleFont(this.selectInstancesPage, AutoScaleFontHelper.ControlType.Container);
            AutoScaleFontResolutionHelper.Default.AutoScaleFont(this.chooseCloudProviderPage, AutoScaleFontHelper.ControlType.Container);
            AutoScaleFontResolutionHelper.Default.AutoScaleFont(this.configureCollectionPage, AutoScaleFontHelper.ControlType.Container);
            AutoScaleFontResolutionHelper.Default.AutoScaleFont(this.wmiConfigPage, AutoScaleFontHelper.ControlType.Container);
            AutoScaleFontResolutionHelper.Default.AutoScaleFont(this.selectTagsPage, AutoScaleFontHelper.ControlType.Container);
        }
        //Saurabh - SQLDM-30848 - UX-Modernization, PRD 4.2

        #region "Events - Configure SQL diagnostic manager"

        private void CollectionIntervalSpinner_TextChanged(object sender, EventArgs e)
        {
            EnableNextPageOfConfigureCollection();
        }

        private void SpnBlockedProcessThreshold_TextChanged(object sender, EventArgs e)
        {
            EnableNextPageOfConfigureCollection();
        }

        private void DurationThresholdSpinner_TextChanged(object sender, EventArgs e)
        {
            EnableNextPageOfConfigureCollection();
        }

        private void CPUThresholdSpinner_TextChanged(object sender, EventArgs e)
        {
            EnableNextPageOfConfigureCollection();
        }

        private void LogicalReadsThresholdSpinner_TextChanged(object sender, EventArgs e)
        {
            EnableNextPageOfConfigureCollection();
        }

        private void PhysicalWritesThresholdSpinner_TextChanged(object sender, EventArgs e)
        {
            EnableNextPageOfConfigureCollection();
        }

        private void TopPlanSpinner_TextChanged(object sender, EventArgs e)
        {
            EnableNextPageOfConfigureCollection();
        }

        /// <summary>
        /// Verifies the text in all spinners in 'Configure SQL diagnostic manager Collection' view,
        /// if these are null or empty to enable the 'Next' button.
        /// </summary>
        private void EnableNextPageOfConfigureCollection()
        {
            bool isEnableNextButton = !string.IsNullOrEmpty(collectionIntervalSpinner.Text) &&
                                      !string.IsNullOrEmpty(spnBlockedProcessThreshold.Text) &&
                                      !string.IsNullOrEmpty(durationThresholdSpinner.Text) &&
                                      !string.IsNullOrEmpty(cpuThresholdSpinner.Text) &&
                                      !string.IsNullOrEmpty(topPlanSpinner.Text) &&
                                      !string.IsNullOrEmpty(logicalReadsThresholdSpinner.Text) &&
                                      !string.IsNullOrEmpty(physicalWritesThresholdSpinner.Text);

            configureCollectionPage.AllowMoveNext = isEnableNextButton;
        }

        #endregion

        #region Azure

        private void BindAzureResources(ListBox availableInstancesListBox)
        {
            var profileItem = selectedProfile;
            if (profileItem != null)
            {
                var selectedProfileValue = profileItem.Value;
                var configuration = new MonitorManagementConfiguration
                {
                    Profile = new AzureProfile
                    {
                        ApplicationProfile = selectedProfileValue
                    },
                    MonitorParameters = new AzureMonitorParameters()
                };

                var connectionInfo = Settings.Default.ActiveRepositoryConnection.ConnectionInfo;
                var managementService = ManagementServiceHelper.GetDefaultService(connectionInfo);
                var filteredResources = managementService.GetFilteredAzureApplicationResources(configuration);
                _azureResourceMap.Clear();
                if (filteredResources != null)
                {
                    // SQLDM-30796 Manage Server - Registered Azure instances still appear in available servers section.
                    if (existingInstances == null)
                    {
                        existingInstanceCount = license.MonitoredServers;
                    }
                    else
                    {
                        existingInstanceCount = existingInstances.Count;
                    }


                    availableInstancesStatusLabel.Hide();
                    availableInstancesListBox.Items.Clear();
                    foreach (var resource in filteredResources)
                    {
                        ListViewItem instanceItem = new ListViewItem();
                        if (resource.Type.Equals("Microsoft.Sql/managedInstances"))
                        {
                            if (resource.FullyQualifiedName.Contains(resource.Name))
                            {
                                string tagName = resource.FullyQualifiedName;
                                tagName = tagName.Insert(resource.Name.Length, ".public");
                                tagName += ",3342";
                                instanceItem = new ListViewItem(tagName);
                                instanceItem.Tag = tagName;
                                if (!_azureResourceMap.ContainsKey(tagName))
                                {
                                    _azureResourceMap.Add(tagName, resource);
                                }
                                else
                                {
                                    _azureResourceMap[tagName] = resource;
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            string tagName = resource.FullyQualifiedName.Trim();
                            instanceItem = new ListViewItem(tagName) {Tag = tagName};
                            if (!_azureResourceMap.ContainsKey(tagName))
                            {
                                _azureResourceMap.Add(tagName, resource);
                            }
                            else
                            {
                                _azureResourceMap[tagName] = resource;
                            }
                        }
                        // SQLDM-30796 Manage Server - Registered Azure instances still appear in available servers section.
                        if (existingInstances != null && existingInstances.ContainsKey(instanceItem.Tag.ToString().ToUpperInvariant()))
                        {
                            continue;
                        }
                        //SQLdm 11 selection and available contain duplicates
                        var commonInstancesCollection = addedInstancesListBox.Items.Cast<ListViewItem>().Where(listItem => listItem.Tag.ToString() == instanceItem.Tag.ToString());                                   
                        if(commonInstancesCollection.Count() == 0)   
                            availableInstancesListBox.Items.Add(instanceItem);
                    }
                }
            }
        }


        #endregion
    }
}