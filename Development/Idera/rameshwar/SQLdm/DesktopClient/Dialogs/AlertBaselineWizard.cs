namespace Idera.SQLdm.DesktopClient.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data.SqlClient;
    using System.Drawing;
    using System.Windows.Forms;
    using Helpers;
    using Idera.SQLdm.Common.Configuration;
    using Idera.SQLdm.Common.Data;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.UI.Dialogs;
    using Infragistics.Win;
    using Infragistics.Win.UltraWinGrid;
    using Properties;
    using Wintellect.PowerCollections;

    public partial class AlertBaselineWizard : Form
    {
        private readonly AlertConfiguration alertConfig;
        private readonly SqlConnectionInfo connectionInfo;
        private Dictionary<int,BaselineItemData> baselineData;
        private DateTime? startTime;
        private DateTime? endTime;

        public AlertBaselineWizard(AlertConfiguration config)
        {
            InitializeComponent();
            contentStackPanel.ActiveControl = wizardSelectionContentPanel;
            alertsGrid.DrawFilter = new HideFocusRectangleDrawFilter();
            alertConfig = config;

            connectionInfo = Settings.Default.ActiveRepositoryConnection.ConnectionInfo;
            AdaptFontSize();
        }

        private void configPage_BeforeDisplay(object sender, EventArgs e)
        {
            if (endTime == null)
            {
                endTime = DateTime.Now.Date;
                endTime = endTime.Value.Date + TimeSpan.FromHours(endTime.Value.TimeOfDay.Hours);
                startTime = endTime.Value - TimeSpan.FromDays(14);
            }
            if (startTime == null || startTime.Value > endTime.Value)
            {
                startTime = endTime.Value - TimeSpan.FromDays(14);
            }
        }

        private void metricSelectionPage_BeforeDisplay(object sender, EventArgs e)
        {
            AlertBaselineEntry.WarningPercent = warningPercentageEditor.Value;
            AlertBaselineEntry.CriticalPercent = criticalPercentageEditor.Value;

            alertConfigBindingSource.Clear();
            foreach (BaselineItemData baselineItem in baselineData.Values)
            {
                BaselineMetaDataItem metaData = baselineItem.GetMetaData();
                // skip items that do not have a metric id - they are not alertable
                if (!metaData.MetricId.HasValue)
                    continue;
                AlertConfigurationItem alertItem = alertConfig[metaData.MetricId.Value, String.Empty]; // Will need to update this for multi-Thresholds at some point
                if (alertItem == null)
                    continue;
                MetricDefinition mmd = alertItem.GetMetaData();
                if (mmd.IsConfigurable && (mmd.Options & ThresholdOptions.NumericValue) == ThresholdOptions.NumericValue)
                {
                    AlertBaselineEntry entry = new AlertBaselineEntry(alertItem, baselineItem);
                    alertConfigBindingSource.Add(entry);
                }
            }
            UpdateCheckAllButton();
        }

        private void AlertBaselineWizard_Load(object sender, EventArgs e)
        {
            // always start from the intro page (unless is has been disabled)
            this.wizard1.SelectedPage = this.introductionPage;

            ColumnsCollection ugcc = alertsGrid.DisplayLayout.Bands[0].Columns;
            ugcc["Category"].CellClickAction = CellClickAction.RowSelect;
            ugcc["SuggestedCritical"].CellClickAction = CellClickAction.RowSelect;
            ugcc["SuggestedWarning"].CellClickAction = CellClickAction.RowSelect;
            ugcc["Selected"].CellClickAction = CellClickAction.RowSelect;
            ugcc["Name"].CellClickAction = CellClickAction.RowSelect;
            ugcc["ReferenceRange"].CellClickAction = CellClickAction.RowSelect;

            ugcc["WarningThreshold"].CellClickAction = CellClickAction.EditAndSelectText;
            ugcc["CriticalThreshold"].CellClickAction = CellClickAction.EditAndSelectText;

            try
            {
                using (
                    SqlConnection connection =
                        connectionInfo.GetConnection(
                            Idera.SQLdm.Common.Constants.DesktopClientConnectionStringApplicationName))
                {
                    connection.Open();
  //                  List<BaselineItemData> baseline = BaselineHelpers.GetBaseline(connection, alertConfig.InstanceID, true, out startTime, out endTime);
  //                  baselineData = BaselineHelpers.ToDictionary(baseline);
                }
            } catch (Exception ex)
            {
                ApplicationMessageBox.ShowError(this, "Unable to retrieve the baseline data", ex);
            }

            DataChanged();
        }


        private void alertsGrid_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            AlertBaselineEntry entry = e.Row.ListObject as AlertBaselineEntry;
            if (entry != null)
            {
                Image image = entry.Selected
                                  ? Properties.Resources.RibbonCheckboxChecked
                                  : Properties.Resources.RibbonCheckboxUnchecked;

                e.Row.Cells["Name"].Appearance.Image = image;
            }
            e.Row.Cells["WarningThreshold"].Activation = Activation.AllowEdit;
            e.Row.Cells["CriticalThreshold"].Activation = Activation.AllowEdit;
        }

        private void alertsGrid_MouseClick(object sender, MouseEventArgs e)
        {
            UltraGrid grid = sender as UltraGrid;
            if (e.Button == MouseButtons.Left)
            {
                UIElement selectedElement = grid.DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));

                if (!(selectedElement is CheckIndicatorUIElement || selectedElement is ImageUIElement
                    || selectedElement is EditorWithTextDisplayTextUIElement))
                    return;

                // logic to handle toggling a checkbox in a non-editable (no cell selection) column
                object contextObject = selectedElement.GetContext();
                if (contextObject is Infragistics.Win.UltraWinGrid.UltraGridColumn)
                {
                    if (((UltraGridColumn)contextObject).Key == "Name")
                    {
                        UltraGridRow selectedRow = selectedElement.SelectableItem as UltraGridRow;

                        if (selectedRow != null)
                        {
                            bool newValue = true;
                            CurrencyManager cm =
                                ((ICurrencyManagerProvider)grid.DataSource).GetRelatedCurrencyManager(grid.DataMember);
                            PropertyDescriptor descriptor = cm.GetItemProperties()["Selected"];
                            object value = descriptor.GetValue(selectedRow.ListObject);
                            if (value is bool)
                            {
                                newValue = !((bool)value);
                            }
                            descriptor.SetValue(selectedRow.ListObject, newValue);
                            UpdateCheckAllButton();
                        }
                    }
                }
            }
        }

        private void baselineWizardButton_Click(object sender, EventArgs e)
        {
//            DateTime? start = startTime;
//            DateTime? end = endTime;
//            List<BaselineItemData> baseline;
//
//            if (BaselineWizard.Show(this,alertConfig.InstanceID,ref start,ref end,out baseline) == DialogResult.OK)
//            {
//                startTime = start;
//                endTime = end;
//                baselineData = BaselineHelpers.ToDictionary(baseline);
//
//                DataChanged();
//            }
        }

        private void DataChanged()
        {
            if (startTime.HasValue && endTime.HasValue 
                && baselineData != null
                && startTime.Value < endTime.Value)
            {
                showAlertBaselineWizardButton.Enabled = true;
            }
            else
            {
                showAlertBaselineWizardButton.Enabled = false;
            }
        }


        private void checkAllButton_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            bool value = true;
            if (button != null && button.Tag is bool)
                value = (bool) button.Tag;

            foreach (UltraGridRow row in alertsGrid.Rows.GetAllNonGroupByRows())
            {
                AlertBaselineEntry entry = row.ListObject as AlertBaselineEntry;
                if (entry != null)
                {
                    if (entry.Selected != value)
                        entry.Selected = value;

                    Image image = entry.Selected
                      ? Properties.Resources.RibbonCheckboxChecked
                      : Properties.Resources.RibbonCheckboxUnchecked;

                    row.Cells["Name"].Appearance.Image = image;
                }
            }

            checkAllButton.Text = value ? "Uncheck All" : "Check All";
            checkAllButton.Tag = !value;

            alertsGrid.Rows.Refresh(RefreshRow.RefreshDisplay);
        }

        private void UpdateCheckAllButton()
        {
            bool allChecked = true;
            foreach (UltraGridRow row in alertsGrid.Rows.GetAllNonGroupByRows())
            {
                AlertBaselineEntry entry = row.ListObject as AlertBaselineEntry;
                if (entry != null)
                {
                    if (!entry.Selected)
                    {
                        allChecked = false;
                        break;
                    }
                }
            }
            if (allChecked)
            {
                checkAllButton.Text = "Uncheck All";
                checkAllButton.Tag = false;
            }
            else
            {
                checkAllButton.Text = "Check All";
                checkAllButton.Tag = true;
            }
        }

        private void wizard1_Finish(object sender, EventArgs e)
        {
            foreach (UltraGridRow row in alertsGrid.Rows.GetAllNonGroupByRows())
            {
                AlertBaselineEntry entry = row.ListObject as AlertBaselineEntry;
                if (entry != null && entry.Selected)
                {
                    object sw = entry.SuggestedWarning;
                    object sc = entry.SuggestedCritical;
                    if (sw != null)
                    {
                        if (sc == null || sw.Equals(sc))
                            continue;

                        entry.WarningThreshold = sc;
                    }
                    if (sc != null)
                    {
                        entry.CriticalThreshold = entry.SuggestedCritical;
                    }
                }
            }
        }

        public class AlertBaselineEntry
        {
            public static decimal WarningPercent = 75;
            public static decimal CriticalPercent = 90;

            private AlertConfigurationItem alertItem;
            private BaselineItemData baselineItem;
            private bool selected = true;

            public AlertBaselineEntry(AlertConfigurationItem alertItem, BaselineItemData baselineItem)
            {
                this.alertItem = alertItem;
                this.baselineItem = baselineItem;
            }

            public bool Selected
            {
                get { return selected; }
                set { selected = value; }
            }

            public string Name
            {
                get { return alertItem.Name; }
            }

            public string Category
            {
                get { return alertItem.Category; }
            }

            public string ReferenceRange
            {
                get
                {
                    if (baselineItem == null || baselineItem.Average == null || baselineItem.Deviation == null)
                        return null;

                    return baselineItem.GetDisplayString();
                }
            }

            public object WarningThreshold
            {
                get { return alertItem.RangeStart1; }
                set { alertItem.FlattenedThresholds[1].Value = value; }
            }

            public object CriticalThreshold
            {
                get { return alertItem.RangeStart2; }
                set { alertItem.FlattenedThresholds[2].Value = value; }
            }

            public object SuggestedWarning
            {
                get
                {
                    decimal? end = baselineItem.ReferenceRangeEnd;
                    if (end == null)
                        return WarningThreshold;
                    return Math.Round(end.Value * WarningPercent / 100m, 0);
                }

                //                get { return alertItem.GetSuggestedValue(ThresholdItemType.Warning); }
                //                set { alertItem.SetSuggestedWarningThreshold(value); }
            }

            public object SuggestedCritical
            {
                get
                {
                    decimal? end = baselineItem.ReferenceRangeEnd;
                    if (end == null)
                        return CriticalThreshold;
                    return Math.Round(end.Value * CriticalPercent / 100m, 0);
                }

                //                get { return alertItem.GetSuggestedValue(ThresholdItemType.Warning); }
                //                set { alertItem.SetSuggestedCriticalThreshold(value); }
            }

            internal AlertConfigurationItem AlertItem
            {
                get { return alertItem; }
            }

            internal BaselineItemData BaselineItem
            {
                get { return baselineItem; }
            }
        }

        private void showBaselineWizardButton_MouseClick(object sender, MouseEventArgs e)
        {
            DateTime? start = startTime;
            DateTime? end = endTime;
            List<BaselineItemData> baseline;

            if (BaselineWizard.Show(this, alertConfig.InstanceID, ref start, ref end, out baseline) == DialogResult.OK)
            {
                startTime = start;
                endTime = end;
                baselineData = BaselineHelpers.ToDictionary(baseline);

                DataChanged();
            }
        }

        private void showAlertBaselineWizardButton_MouseClick(object sender, MouseEventArgs e)
        {
            configPage_BeforeDisplay(wizard1, EventArgs.Empty);
            wizard1.SelectedPage = configPage;
            contentStackPanel.ActiveControl = wizard1;
        }

        private void configPage_BeforeMoveBack(object sender, CancelEventArgs e)
        {
            contentStackPanel.ActiveControl = wizardSelectionContentPanel;
        }

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }
    }
}