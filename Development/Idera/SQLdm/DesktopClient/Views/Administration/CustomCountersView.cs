using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Views.Administration
{
    using System.Threading;
    using Common;
    using Controls;
    using Dialogs;
    using Helpers;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.Services;
    using Idera.SQLdm.Common.Thresholds;
    using Idera.SQLdm.Common.UI.Dialogs;
    using Infragistics.Win;
    using Infragistics.Win.UltraWinGrid;
    using Infragistics.Win.UltraWinToolbars;
    using Properties;
    using Wintellect.PowerCollections;
    using System.IO;
    using Infragistics.Windows.Themes;

    internal partial class CustomCountersView : View
    {
        // START: SQLdm 9.0 (Abhishek Joshi) --DE40831 -removed the duplicate occurrence of the word 'data' from the confirmation window message of custom counter deletion
        private const string DELETE_COUNTER_INFO_MONITORED =
            "The listed instances are currently monitoring the '{0}' counter.  Deleting the counter will remove it from all servers monitoring the counter and delete all previously collected data for the counter.  Click OK to delete the custom counter anyway.";
        // END: SQLdm 9.0 (Abhishek Joshi) --DE40831
        private const string DELETE_COUNTER_INFO_NOT_MONITORED =
            "The are no instances currently monitoring the '{0}' counter.  Deleting the counter will delete all previously collected data for the counter.  Click OK to delete the custom counter.";

        private const string ENABLE_COUNTER_INFO_MONITORED =
            "The listed instances will begin monitoring the '{0}' counter.  Click OK to enable the custom counter";
        private const string ENABLE_COUNTER_INFO_NOT_MONITORED =
            "There are no instances configured to monitor the {0}' counter.  Click OK to enable the custom counter";

        private const string DISABLE_COUNTER_INFO_MONITORED =
            "The listed instances are currently monitoring the '{0}' counter.  Disabling the counter will prevent further collection of this counter on the listed instances.  Click OK to disable the custom counter.";
        private const string DISABLE_COUNTER_INFO_NOT_MONITORED =
            "The are no instances currently monitoring the '{0}' counter.  Click OK to disable the custom counter.";
        
        private Dictionary<int,CustomCounter> counterMap = new Dictionary<int, CustomCounter>();
        private UltraGridColumn selectedColumn = null;

        public CustomCountersView()
        {
            InitializeComponent();
            counterGrid.DrawFilter = new HideFocusRectangleDrawFilter();
            this.AdaptFontSize();
            if(AutoScaleSizeHelper.isScalingRequired)
                ScaleControlsAsPerResolution();
            AutoScaleFontHelper.Default.AutoScaleControl(this.panel3, AutoScaleFontHelper.ControlType.Container);
            SetGridTheme();
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
        }

        private void ScaleControlsAsPerResolution()
        {
            this.panel2.Size = new System.Drawing.Size(290, 466);
            this.introductoryTextLabel2.Size = new System.Drawing.Size(650, 333);
            this.panel3.Size = new Size(this.panel3.Size.Width, this.panel3.Size.Height + 200);
            this.addCustomCounterLinkLabel.Location = new System.Drawing.Point(152, 390);
            this.addCustomCounterLinkLabel.Size = new Size(this.addCustomCounterLinkLabel.Size.Width + 50, this.addCustomCounterLinkLabel.Size.Height);
            this.communitySiteLinkLabel.Location = new Point(this.communitySiteLinkLabel.Location.X + 50, this.communitySiteLinkLabel.Location.Y + 100);
            this.communitySiteLabel1.Location = new Point(this.communitySiteLabel1.Location.X, this.communitySiteLabel1.Location.Y + 100);
            this.communitySiteLabel2.Location = new Point(this.communitySiteLabel2.Location.X + 100, this.communitySiteLabel2.Location.Y + 100);
            this.communitySiteLinkLabel.Size = new Size(this.communitySiteLinkLabel.Size.Width + 50, this.communitySiteLinkLabel.Size.Height);
            this.communitySiteLabel1.Size = new Size(this.communitySiteLabel1.Size.Width + 50, this.communitySiteLabel1.Size.Height);
            this.communitySiteLabel2.Size = new Size(this.communitySiteLabel2.Size.Width + 50, this.communitySiteLabel2.Size.Height);
            if (AutoScaleSizeHelper.isXLargeSize)
            {
                this.addCustomCounterLinkLabel.Location = new System.Drawing.Point(152, 550);
                this.communitySiteLabel1.Location = new Point(this.communitySiteLabel1.Location.X, this.communitySiteLabel1.Location.Y + 200);
                this.communitySiteLinkLabel.Location = new Point(this.communitySiteLinkLabel.Location.X + 50, this.communitySiteLinkLabel.Location.Y + 200);
                this.communitySiteLinkLabel.Size = new Size(this.communitySiteLinkLabel.Size.Width + 50, this.communitySiteLinkLabel.Size.Height);
                this.communitySiteLabel2.Location = new Point(this.communitySiteLabel2.Location.X + 100, this.communitySiteLabel2.Location.Y + 210);
            }
            else if (AutoScaleSizeHelper.isXXLargeSize)
            {
                this.addCustomCounterLinkLabel.Location = new System.Drawing.Point(152, 550);
                this.communitySiteLabel1.Location = new Point(this.communitySiteLabel1.Location.X, this.communitySiteLabel1.Location.Y + 200);
                this.communitySiteLinkLabel.Location = new Point(this.communitySiteLinkLabel.Location.X + 50, this.communitySiteLinkLabel.Location.Y + 200);
                this.communitySiteLinkLabel.Size = new Size(this.communitySiteLinkLabel.Size.Width + 50, this.communitySiteLinkLabel.Size.Height);
                this.communitySiteLabel2.Location = new Point(this.communitySiteLabel2.Location.X + 100, this.communitySiteLabel2.Location.Y + 210);
            }
        }

        private void ultraToolbarsManager1_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "createCounterButton":
                    ShowCreateCounterWizard();
                    break;
                case "changeCounterButton":
                    ShowChangeWizard();
                    break;
                case "deleteCounterButton":
                    DeleteSelectedCounter();
                    break;
                case "enableDisableCounterButton":
                    ChangeCounterStatus();
                    break;
                case "testCounterButton":
                    ShowTestCustomCounterDialog();
                    break;
                case "viewAssociatedServersButton":
                    ShowAssociatedServersDialog();
                    break;
                case "sortAscendingButton":
                    SortSelectedColumnAscending();
                    break;
                case "sortDescendingButton":
                    SortSelectedColumnDescending();
                    break;
                case "toggleGroupByBoxButton":
                    ToggleGroupByBox();
                    break;
                case "groupByThisColumnButton":
                    GroupBySelectedColumn(((StateButtonTool)e.Tool).Checked);
                    break;
                case "removeThisColumnButton":
                    RemoveSelectedColumn();
                    break;
                case "columnChooserButton":
                    ShowColumnChooser();
                    break;
                case "collapseAllGroupsButton":
                    CollapseAllGroups();
                    break;
                case "expandAllGroupsButton":
                    ExpandAllGroups();
                    break;
                //start:SQLdm 9.1 (Vineet Kumar) (Community Integration) -- UI changes for Custom counter import/export
                case "importCounterButton":
                    ShowImportCustomCounterDialogue();
                    break;
                case "exportCounterButton":
                    ExportCustomCounter();
                    break;
                //End : SQLdm 9.1 (Vineet Kumar) (Community Integration) -- UI changes for Custom counter import/export
            }
        }

        /// <summary>
        /// SQLdm 9.1 (Vineet Kumar) (Community Integration) -- Export method for custim counter
        /// 
        /// </summary>
        private void ExportCustomCounter()
        {
            string selectedDirectory = null;
            try
            {
                using (var sfd = new FolderBrowserDialog())
                {
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        selectedDirectory = sfd.SelectedPath;
                        for (int i = 0; i < counterGrid.Selected.Rows.Count; i++)
                        {
                            CustomCounter counter = counterGrid.Selected.Rows[i].ListObject as CustomCounter;
                            string xml = Idera.SQLdm.DesktopClient.Helpers.CustomCounterHelper.SerializeCustomCounter(counter);
                            string fileName = counter.GetMetricDescription().Name + ".xml";
                            string fullFileName = Path.Combine(selectedDirectory, fileName);
                            File.WriteAllText(fullFileName, xml);
                        }
                        ApplicationMessageBox.ShowInfo(this, "Selected custom counters exported to selected directory");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Export custom counters operation failed : Error : " + ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                ApplicationMessageBox.ShowError(this, "Export operation failed : ", ex);
            }
        }

        /// <summary>
        /// SQLdm 9.1 (Vineet Kumar) (Community Integration) -- Import method for custom counter
        /// </summary>
        private void ShowImportCustomCounterDialogue()
        {
            List<Triple<MetricDefinition, MetricDescription, CustomCounterDefinition>> newCounters = null;
            try
            {
                if (CustomCounterImportWizard.ImportNewCounter(this, out newCounters) == DialogResult.OK && newCounters != null)
                {
                    foreach (var newCounter in newCounters)
                    {
                        int metricID = newCounter.First.MetricID;

                        CustomCounter wrapper =
                            new CustomCounter(newCounter.First, newCounter.Second, newCounter.Third);

                        if (!counterMap.ContainsKey(metricID))
                        {
                            counterMap.Add(metricID, wrapper);
                            counterBindingSource.Add(wrapper);
                            counterGrid.Visible = (counterBindingSource.Count > 0);
                        }

                        string question = "Would you like to link your counter to monitored SQL Server instances now?";
                        if (newCounter.Third.MetricType != MetricType.AzureCounter && ApplicationMessageBox.ShowQuestion(this, question, Microsoft.SqlServer.MessageBox.ExceptionMessageBoxButtons.YesNo, Microsoft.SqlServer.MessageBox.ExceptionMessageBoxDefaultButton.Button1) == DialogResult.Yes)
                        {
                            ThreadPool.QueueUserWorkItem((WaitCallback)delegate(object arg) { ApplicationModel.Default.RefreshMetricMetaData(); });
                            using (ApplyCustomCounterToServersDialog acctsd = new ApplyCustomCounterToServersDialog(metricID))
                            {
                                acctsd.ShowDialog(this);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error occurred while importing custom counters : " + ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        private void ChangeCounterStatus()
        {
            if (counterGrid.Selected.Rows.Count == 1)
            {
                CustomCounter counter = counterGrid.Selected.Rows[0].ListObject as CustomCounter;
                CustomCounterDefinition ccd = counter.GetCustomCounterDefinition();
                try
                {
                    Pair<IList<int>, IList<int>> result =
                        RepositoryHelper.GetCustomCounterTagsAndServers(
                            Settings.Default.ActiveRepositoryConnection.ConnectionInfo, counter.MetricID);

                    using (AffectedServersConfirmationDialog confirm = new AffectedServersConfirmationDialog())
                    {

                        if (ccd.IsEnabled)
                        {
                            confirm.Text = "Disable Custom Counter";
                            confirm.InfoText = result.First.Count > 0 || result.Second.Count > 0
                                                   ?
                                                       String.Format(DISABLE_COUNTER_INFO_MONITORED, counter.Name)
                                                   :
                                                       String.Format(DISABLE_COUNTER_INFO_NOT_MONITORED, counter.Name);
                        }
                        else
                        {
                            confirm.Text = "Enable Custom Counter";
                            confirm.InfoText = result.First.Count > 0 || result.Second.Count > 0
                                                   ?
                                                       String.Format(ENABLE_COUNTER_INFO_MONITORED, counter.Name)
                                                   :
                                                       String.Format(ENABLE_COUNTER_INFO_NOT_MONITORED, counter.Name);
                        }
                        confirm.SetTagsAndInstances(result.First, result.Second);
                        if (confirm.ShowDialog(this) == DialogResult.OK)
                        {
                            ChangeCounterStatus(counter);
                        }
                    }
                }
                catch (Exception e)
                {
                    ApplicationMessageBox.ShowError(
                        this,
                        String.Format("Error trying to {1} the '{0}' custom counter.", counter.Name,
                                      ccd.IsEnabled ? "enable" : "disable"),
                        e);
                }
            }
        }

        private void ChangeCounterStatus(CustomCounter counter)
        {
            IManagementService managementService = ManagementServiceHelper.GetDefaultService();

            CustomCounterDefinition ccd = counter.GetCustomCounterDefinition();
            managementService.UpdateCustomCounterStatus(counter.MetricID, !ccd.IsEnabled);
            ccd.IsEnabled = !ccd.IsEnabled;
            // force a refresh of the data
            counterGrid.DisplayLayout.Rows.Refresh(RefreshRow.ReloadData);
        }


        private void DeleteSelectedCounter()
        {
            if (counterGrid.Selected.Rows.Count == 1)
            {
                CustomCounter counter = counterGrid.Selected.Rows[0].ListObject as CustomCounter;
                try
                {
                    Pair<IList<int>, IList<int>> result =
                        RepositoryHelper.GetCustomCounterTagsAndServers(
                            Settings.Default.ActiveRepositoryConnection.ConnectionInfo, counter.MetricID);

                    using (AffectedServersConfirmationDialog confirm = new AffectedServersConfirmationDialog())
                    {
                        confirm.InfoText = result.First.Count > 0 || result.Second.Count > 0
                                               ?
                                                   String.Format(DELETE_COUNTER_INFO_MONITORED, counter.Name)
                                               :
                                                   String.Format(DELETE_COUNTER_INFO_NOT_MONITORED, counter.Name);
                        confirm.HelpLink = HelpTopics.CustomCountersDelete;
                        confirm.Text = "Delete Custom Counter";
                        confirm.SetTagsAndInstances(result.First, result.Second);

                        if (confirm.ShowDialog(this) == DialogResult.OK)
                        {
                            DeleteCounter(counter);
                        }
                    }
                }
                catch (Exception e)
                {
                    ApplicationMessageBox.ShowError(
                        this,
                        String.Format("Error trying to delete the '{0}' custom counter.", counter.Name),
                        e);
                }
            }
        }

        private void DeleteCounter(CustomCounter counter)
        {
           IManagementService managementService = ManagementServiceHelper.GetDefaultService();

            managementService.DeleteCustomCounter(counter.MetricID);
            if (counterMap.ContainsKey(counter.MetricID))
            {
                // remove counter from the grid
                counterMap.Remove(counter.MetricID);
                counterBindingSource.Remove(counter);
                counterGrid.Visible = (counterBindingSource.Count > 0);
                UpdateToolbarItems();
            }
        }

        private void ShowCreateCounterWizard()
        {
            Triple<MetricDefinition, MetricDescription, CustomCounterDefinition>? newCounter = null;
            if (CustomCounterWizard.CreateNewCounter(this, out newCounter) == DialogResult.OK && newCounter != null)
            {
                int metricID = newCounter.Value.First.MetricID;

                CustomCounter wrapper =
                    new CustomCounter(newCounter.Value.First, newCounter.Value.Second, newCounter.Value.Third);

                if (!counterMap.ContainsKey(metricID))
                {
                    counterMap.Add(metricID, wrapper);
                    counterBindingSource.Add(wrapper);
                    counterGrid.Visible = (counterBindingSource.Count > 0);
                }

                string question = "Would you like to link your counter to monitored SQL Server instances now?";

                if (newCounter.Value.Third.MetricType == MetricType.AzureCounter)
                {
                    var serverDetails = ApplicationModel.Default.ActiveInstances.FirstOrDefault(i =>
                        i.InstanceName == newCounter.Value.Third.InstanceName);
                    if (serverDetails != null)
                    {
                        ApplicationModel.Default.AddCustomCounterToMonitoredServers(newCounter.Value.First.MetricID,
                            new List<int>(), new List<int>
                            {
                                serverDetails.Id
                            });
                    }
                }
                else if (ApplicationMessageBox.ShowQuestion(
                        this, question, Microsoft.SqlServer.MessageBox.ExceptionMessageBoxButtons.YesNo,
                        Microsoft.SqlServer.MessageBox.ExceptionMessageBoxDefaultButton.Button1) == DialogResult.Yes)
                {
                    ThreadPool.QueueUserWorkItem((WaitCallback)delegate(object arg) { ApplicationModel.Default. RefreshMetricMetaData(); });
                    using (ApplyCustomCounterToServersDialog acctsd = new ApplyCustomCounterToServersDialog(metricID))
                    {
                        acctsd.ShowDialog(this);
                    }
                }
            }
        }

        private void toolbarsManager_BeforeToolDropdown(object sender, Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventArgs e)
        {
//            switch (e.Tool.Key)
//            {
//                case "chartContextMenu":
//                    ((StateButtonTool)((PopupMenuTool)e.Tool).Tools["toggleChartToolbarButton"]).InitializeChecked(chart.ToolBar.Visible);
//                    break;
//            }

            if (e.Tool.Key == "metricContextMenu" || e.Tool.Key == "GridRowMenu")
            {
                bool isGrouped = counterGrid.Rows.Count > 0 && counterGrid.Rows[0].IsGroupByRow;
                ((PopupMenuTool) e.Tool).Tools["collapseAllGroupsButton"].SharedProps.Enabled = isGrouped;
                ((PopupMenuTool)e.Tool).Tools["expandAllGroupsButton"].SharedProps.Enabled = isGrouped;
            }

            if (e.Tool.Key == "columnContextMenu")
            {
                int minCantForEnable = UltraGridHelper.GetNotHiddenColumns(counterGrid);
                bool enableTool = minCantForEnable > 1 ? true : false;

                ((PopupMenuTool)e.Tool).Tools["removeThisColumnButton"].SharedProps.Enabled = enableTool;
            }

        }

        private void SortSelectedColumnAscending()
        {
            if (selectedColumn != null)
            {
                counterGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                counterGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, false);
            }
        }

        private void SortSelectedColumnDescending()
        {
            if (selectedColumn != null)
            {
                counterGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                counterGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, true, false);
            }
        }

        private void ToggleGroupByBox()
        {
            counterGrid.DisplayLayout.GroupByBox.Hidden = !counterGrid.DisplayLayout.GroupByBox.Hidden;
            UpdateToggleGroupByBoxButton();
        }

        private void UpdateToggleGroupByBoxButton()
        {
            if (counterGrid.DisplayLayout.GroupByBox.Hidden)
            {
                toolbarsManager.Tools["toggleGroupByBoxButton"].SharedProps.AppearancesSmall.
                    Appearance.Image = Properties.Resources.RibbonCheckboxUnchecked;
            }
            else
            {
                toolbarsManager.Tools["toggleGroupByBoxButton"].SharedProps.AppearancesSmall.
                    Appearance.Image = Properties.Resources.RibbonCheckboxChecked;
            }
        }

        private void GroupBySelectedColumn(bool GroupBy)
        {
            if (selectedColumn != null)
            {
                if (GroupBy)
                {
                    counterGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, true);
                }
                else
                {
                    counterGrid.DisplayLayout.Bands[0].SortedColumns.Remove(selectedColumn);
                }
            }
        }

        private void RemoveSelectedColumn()
        {
            if (selectedColumn != null)
            {
                selectedColumn.Hidden = true;
            }
        }

        private void CollapseAllGroups()
        {
            counterGrid.Rows.CollapseAll(true);
        }

        private void ExpandAllGroups()
        {
            counterGrid.Rows.ExpandAll(true);
        }

        private void ShowColumnChooser()
        {
            SimpleUltraGridColumnChooserDialog dialog = new SimpleUltraGridColumnChooserDialog(counterGrid);
            dialog.Show(this);
        }


        private void ShowTestCustomCounterDialog()
        {
            if (counterGrid.Selected.Rows.Count == 1)
            {
                CustomCounter counter = (CustomCounter)counterGrid.Selected.Rows[0].ListObject;
                using (TestCustomCounterDialog acctsd = new TestCustomCounterDialog(counter.GetCustomCounterDefinition(), counter.GetMetricDescription()))
                {
                    acctsd.ShowDialog(this);
                }
            }
        }

        private void ShowAssociatedServersDialog()
        {
            if (counterGrid.Selected.Rows.Count == 1)
            {
                CustomCounter counter = counterGrid.Selected.Rows[0].ListObject as CustomCounter;
                ApplyCustomCounterToServersDialog acctsd = null;
                try
                {
                    acctsd = new ApplyCustomCounterToServersDialog(counter.MetricID);
                    acctsd.ShowDialog(this);
                }
                catch (Exception e)
                {
                    if (acctsd != null)
                    {
                        acctsd.Dispose();
                        acctsd = null;
                    }
                    ApplicationMessageBox.ShowError(this, "Error attempting to link custom counters.  Please resolve the error and try again.", e);
                }
                finally
                {
                    if (acctsd != null)
                        acctsd.Dispose();
                }
            }
        }

        void ShowChangeWizard()
        {
            if (counterGrid.Selected.Rows.Count == 1)
            {
                Triple<MetricDefinition, MetricDescription, CustomCounterDefinition>? changedCounter = null;
                CustomCounter counter = counterGrid.Selected.Rows[0].ListObject as CustomCounter;
                
                if (CustomCounterWizard.ChangeCounter(
                    this.ParentForm,
                    counter.GetMetricDefinition(),
                    counter.GetMetricDescription(),
                    counter.GetCustomCounterDefinition(), 
                    out changedCounter) == DialogResult.OK && changedCounter.HasValue)
                {
                    // update the metadata stored in the application model
                    ThreadPool.QueueUserWorkItem(
                        (WaitCallback)delegate(object arg)
                            {
                                ApplicationModel.Default.RefreshMetricMetaData();
                            });

                    // update the list object
                    counter.UpdateCounter(changedCounter.Value.First, 
                                          changedCounter.Value.Second,
                                          changedCounter.Value.Third);

                    if (changedCounter.Value.Third.MetricType == MetricType.AzureCounter)
                    {
                        var serverDetails = ApplicationModel.Default.ActiveInstances.FirstOrDefault(i =>
                            i.InstanceName == changedCounter.Value.Third.InstanceName);
                        if (serverDetails != null)
                        {
                            ApplicationModel.Default.AddCustomCounterToMonitoredServers(changedCounter.Value.First.MetricID,
                                new List<int>(), new List<int>
                                {
                                    serverDetails.Id
                                });
                        }
                    }
                    // refresh the item in the grid
                    int itemIndex = counterBindingSource.IndexOf(counter);
                    if (itemIndex >= 0)
                        counterBindingSource.ResetItem(itemIndex);
                }
            }
        }

        public override object DoRefreshWork()
        {
            MetricDefinitions metrics = new MetricDefinitions(true, false, true);
            metrics.Load(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ConnectionString);
            return metrics;
        }

        public override void HandleBackgroundWorkerError(Exception e)
        {
            base.HandleBackgroundWorkerError(e);
            if (counterBindingSource.Count > 0)
            {
                counterGrid.Visible = true;
                errorLoadingCountersLinkLabel.Visible = false;
            }
            else
            {
                counterGrid.Visible = false;
                errorLoadingCountersLinkLabel.Tag = e;
                errorLoadingCountersLinkLabel.Visible = true;
            }
        }

        public override void UpdateData(object data)
        {
            CustomCounter counter;
            MetricDescription? description;

            Set<int> currentKeySet = new Set<int>(counterMap.Keys);
            MetricDefinitions metrics = data as MetricDefinitions;
            if (metrics != null)
            {
                foreach (int metricID in metrics.GetCounterDefinitionKeys())
                {
                    description = metrics.GetMetricDescription(metricID);
                    if (description.HasValue)
                    {
                        if (counterMap.TryGetValue(metricID, out counter))
                        {
                            // update the current data object
                            counter.SetData(metrics.GetMetricDefinition(metricID), description.Value,
                                            metrics.GetCounterDefinition(metricID));
                            currentKeySet.Remove(metricID);
                            int itemIndex = counterBindingSource.IndexOf(counter);
                            if (itemIndex >= 0)
                                counterBindingSource.ResetItem(itemIndex);
                        }
                        else
                        {
                            // add new counter to the grid
                            counter = new CustomCounter(metrics.GetMetricDefinition(metricID), description.Value,
                                                metrics.GetCounterDefinition(metricID));
                            counterMap.Add(metricID, counter);
                            counterBindingSource.Add(counter);
                        }
                    }
                }
                // remove counters that no longer exist
                foreach (int metricID in currentKeySet)
                {
                    if (counterMap.TryGetValue(metricID, out counter))
                    {
                        counterMap.Remove(metricID);
                        counterBindingSource.Remove(counter);
                    }
                }
            }

            counterGrid.Visible = (counterBindingSource.Count > 0);
            errorLoadingCountersLinkLabel.Visible = false;

            UpdateToolbarItems();
            ApplicationController.Default.OnRefreshActiveViewCompleted(new RefreshActiveViewCompletedEventArgs(DateTime.Now));
        }

        private void counterGrid_AfterSelectChange(object sender, Infragistics.Win.UltraWinGrid.AfterSelectChangeEventArgs e)
        {
            UpdateToolbarItems();
        }

        //Saurabh - SQLDM-30848 - UX-Modernization, PRD 4.2
        private void counterGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            if (AutoScaleSizeHelper.isScalingRequired)
                AutoScaleSizeHelper.Default.AutoScaleControl(this.counterGrid, AutoScaleSizeHelper.ControlType.UltraGridCheckbox);
        }

        private void UpdateToolbarItems() 
        {
            bool singleSelected = counterGrid.Selected.Rows.Count == 1;
            bool anySelected = counterGrid.Selected.Rows.Count > 0;

            var isAzureMetric = false;
            if (singleSelected)
            {
                CustomCounter counter = (CustomCounter)counterGrid.Selected.Rows[0].ListObject;
                isAzureMetric = counter.GetCustomCounterDefinition().MetricType == MetricType.AzureCounter;
            }

            toolbarsManager.Tools["changeCounterButton"].SharedProps.Enabled = singleSelected;
            toolbarsManager.Tools["deleteCounterButton"].SharedProps.Enabled = singleSelected;

            toolbarsManager.Tools["exportCounterButton"].SharedProps.Enabled = anySelected;//SQLdm 9.1 (Vineet Kumar) (Community Integration) -- UI changes for Custom counter import/export

            toolbarsManager.Tools["testCounterButton"].SharedProps.Enabled = singleSelected;
            toolbarsManager.Tools["viewAssociatedServersButton"].SharedProps.Enabled = singleSelected && !isAzureMetric;
            
            // update visibility and caption for the enable/disble counter menu option
            toolbarsManager.Tools["enableDisableCounterButton"].SharedProps.Visible = singleSelected;
            if (singleSelected)
            {
                CustomCounter counter = (CustomCounter)counterGrid.Selected.Rows[0].ListObject;
                toolbarsManager.Tools["enableDisableCounterButton"].SharedProps.Caption =
                    counter.GetCustomCounterDefinition().IsEnabled ? "Disable" : "Enable";
            }
        }

        private void CustomCountersView_Load(object sender, EventArgs e)
        {
            UpdateToggleGroupByBoxButton();
            counterGrid.Visible = (counterBindingSource.Count > 0);
        }

        private void counterGrid_MouseDown(object sender, MouseEventArgs e)
        {
            // set the grid context menu based on where the mouse clicked 
            if (e.Button == MouseButtons.Right)
            {
                UIElement selectedElement =
                    ((UltraGrid) sender).DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));
                object contextObject = selectedElement.GetContext(typeof (Infragistics.Win.UltraWinGrid.ColumnHeader));

                if (contextObject is Infragistics.Win.UltraWinGrid.ColumnHeader)
                {
                    Infragistics.Win.UltraWinGrid.ColumnHeader columnHeader =
                        contextObject as Infragistics.Win.UltraWinGrid.ColumnHeader;
                    selectedColumn = columnHeader.Column;
                    ((StateButtonTool) toolbarsManager.Tools["groupByThisColumnButton"]).Checked =
                        selectedColumn.IsGroupByColumn;
                    toolbarsManager.SetContextMenuUltra((UltraGrid)sender, "columnContextMenu");
                }
                else
                {
                    UltraGridRow row = ((UltraGridRow)selectedElement.SelectableItem);
                    if (row != null && row.IsDataRow)
                    {
                        toolbarsManager.SetContextMenuUltra((UltraGrid)sender, "GridRowMenu");
                        counterGrid.Selected.Rows.Clear();
                        row.Activate();
                        row.Selected = true;
                    } else
                    {
                        toolbarsManager.SetContextMenuUltra((UltraGrid)sender, null);
                    }
                }
            } 
        }

        private void addCustomCounterLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowCreateCounterWizard();
        }

        private void counterGrid_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            if (e.Row.IsDataRow)
            {
                ShowChangeWizard();
            }
        }

        public override void ShowHelp()
        {
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(Idera.SQLdm.Common.HelpTopics.CustomCountersAdministration);       
        }

        private void errorLoadingCountersLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ApplicationMessageBox.ShowError(this,
                                            "Error loading the list of custom counters.  Please resolve the error and refresh the administration view.",
                                            (Exception) errorLoadingCountersLinkLabel.Tag);
        }

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }

        void OnCurrentThemeChanged(object sender, EventArgs e)
        {
            SetGridTheme();
            SetBackgroundImage();
        }

        private void SetGridTheme()
        {
            // Update UltraGrid Theme
            var themeManager = new GridThemeManager();
            themeManager.updateGridTheme(this.counterGrid);
        }

        private void SetBackgroundImage()
        {
            if (Properties.Settings.Default.ColorScheme == "Dark")
            {
                this.panel2.BackgroundImage = null;
            }
            else
            {
                this.panel2.BackgroundImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.CustomCounterWizardWelcomePageMarginImage;
            }
        }
    }

    public class CustomCounter
    {
       
        private MetricDefinition metricDefinition;
        private MetricDescription metricDescription;
        private CustomCounterDefinition counterDefinition;
        public CustomCounter(MetricDefinition metricDefinition, MetricDescription metricDescription, CustomCounterDefinition counterDefinition)
        {
            this.metricDescription = metricDescription;
            this.metricDefinition = metricDefinition;
            this.counterDefinition = counterDefinition;
        }

        public void SetData(MetricDefinition metricDefinition, MetricDescription metricDescription, CustomCounterDefinition counterDefinition)
        {
            this.metricDescription = metricDescription;
            this.metricDefinition = metricDefinition;
            this.counterDefinition = counterDefinition;
        }

        public MetricDefinition GetMetricDefinition()
        {
            return metricDefinition;
        }
        public MetricDescription GetMetricDescription()
        {
            return metricDescription;
        }
        public CustomCounterDefinition GetCustomCounterDefinition()
        {
            return counterDefinition;
        }

        public int MetricID
        {
            get { return metricDefinition.MetricID; }
        }
        public string Category
        {
            get { return metricDescription.Category; }
        }
        public String Enabled
        {
            get { return counterDefinition.IsEnabled ? "Yes" : "No"; }
        }
        public string Name
        {
            get { return metricDescription.Name; }
        }
        public string Description
        {
            get { return metricDescription.Description; }
        }
        public long DefaultInfoThreshold
        {
            get { return metricDefinition.DefaultInfoThresholdValue; }
        }
        public long DefaultWarningThreshold
        {
            get { return metricDefinition.DefaultWarningThresholdValue; }
        }
        public long DefaultCriticalThreshold
        {
            get { return metricDefinition.DefaultCriticalThresholdValue; }
        }
        public double Scale
        {
            get { return counterDefinition.Scale; }
        }
        public string CalculationType
        {
            get
            {
                return ApplicationHelper.GetEnumDescription(counterDefinition.CalculationType).ToString(); 
            }
        }
        public bool AlertEnabledByDefault
        {
            get { return metricDefinition.AlertEnabledByDefault; }
        }
        public string AlertComparison
        {
            get
            {
                return ApplicationHelper.GetEnumDescription(metricDefinition.ComparisonType).ToString();
            }
        }
        public string CounterType
        {
            get
            {
                return ApplicationHelper.GetEnumDescription(counterDefinition.MetricType).ToString();
            }
        }
        public string ObjectName
        {
            get { return counterDefinition.ObjectName; }
        }
        public string CounterName
        {
            get { return counterDefinition.CounterName; }
        }
        public string InstanceName
        {
            get { return counterDefinition.InstanceName; }
        }
        public string SQLBatch
        {
            // Ensure that the SQL Statement is returned null for Azure Custom Counter
            get
            { 
                return counterDefinition.MetricType != MetricType.AzureCounter ? counterDefinition.SqlStatement : null;
            }
        }

        internal void UpdateCounter(MetricDefinition metricDefinition, MetricDescription metricDescription, CustomCounterDefinition counterDefinition)
        {
            this.metricDefinition = metricDefinition;
            this.metricDescription = metricDescription;
            this.counterDefinition = counterDefinition;
        }
    }

}

