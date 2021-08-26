using System.Data;
using System.Windows.Forms;
using Idera.SQLdm.DesktopClient.Helpers;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data.SqlClient;
    using System.Drawing;
    using ChartFX.WinForms.Gauge;
    using ChartFX.WinForms.Gauge.HitDetection;
    using Common;
    using Idera.SQLdm.Common.Configuration;
    using Idera.SQLdm.Common.Data;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.Thresholds;
    using Idera.SQLdm.Common.UI.Dialogs;
    using Infragistics.Win;
    using Infragistics.Win.UltraWinGrid;
    using Properties;

    public partial class AlertRecommendationsDialog : Form
    {
        private readonly string AlertConfigurationGroupBoxText;

        private readonly int instanceId;
        private readonly AlertConfiguration alertConfiguration;
        private readonly Dictionary<int, AlertBaselineEntry> alertBaselineEntryMap;
        private bool currentChanging;
        private bool gaugeChanging;
        private bool checkChanging;
        
        private Indicator warningIndicator;
        private Indicator criticalIndicator;
        private Section okSection;
        private Section warningSection;
        private Section criticalSection;
        private LinearStrip refRangeStrip;
        
        public AlertRecommendationsDialog(AlertConfiguration alertConfig, IEnumerable<BaselineItemData> baselineItems)
        {
            this.instanceId = alertConfig.InstanceID;
            this.alertConfiguration = alertConfig;
            this.alertBaselineEntryMap = new Dictionary<int, AlertBaselineEntry>();

            InitializeComponent();

            // Perform auto resize column.
            recommendationsGrid.DisplayLayout.Bands[0].Columns["Selected"].PerformAutoResize();

            recommendationsGrid.DrawFilter = new HideFocusRectangleDrawFilter();
            AlertConfigurationGroupBoxText = alertConfigurationGroupBox.Text;
            
            if (ApplicationModel.Default.ActiveInstances.Contains(instanceId))
            {
                string instanceName = ApplicationModel.Default.ActiveInstances[instanceId].InstanceName;
                Text = string.Format(Text, instanceName);
            }
            RefreshSuggestions(baselineItems);
            AdaptFontSize();
        }

        private void RefreshSuggestions(IEnumerable<BaselineItemData> baselineItems)
        {
            Dictionary<int, AlertBaselineEntry> syncMap =
                new Dictionary<int, AlertBaselineEntry>(alertBaselineEntryMap);

            AlertBaselineEntry entry;
            AlertConfigurationItem alertConfigItem;
            if (baselineItems != null)
            {
                foreach (BaselineItemData item in baselineItems)
                {
                    BaselineMetaDataItem metaData = item.GetMetaData();
                    if (metaData.MetricId.HasValue)
                    {
                        int metricId = metaData.MetricId.Value;
                        if (alertBaselineEntryMap.TryGetValue(metricId, out entry))
                        {
                            entry.BaselineItem = item;
                            // remove from sync map
                            if (syncMap.ContainsKey(metricId))
                                syncMap.Remove(metricId);
                        } else
                        {
                            alertConfigItem = alertConfiguration[metricId, String.Empty];
                            if (alertConfigItem != null)
                            {
                                // new entry - add to map and data source
                                entry = new AlertBaselineEntry(alertConfigItem, item);
                                alertBaselineEntryMap.Add(metricId, entry);
                                gridBindingSource.Add(entry);
                            }
                        }
                    }
                }
            }
            // everything left in syncMap no longer exists
            foreach(AlertBaselineEntry item in syncMap.Values)
            {
                // remove from binding source
                gridBindingSource.Remove(item);
                // remove from map
                alertBaselineEntryMap.Add(item.AlertItem.MetricID, item);
            }
        }

        private void configureBaselineButton_Click(object sender, System.EventArgs e)
        {            
            using (MonitoredSqlServerInstancePropertiesDialog dialog = new MonitoredSqlServerInstancePropertiesDialog(instanceId))
            {
                dialog.SelectedPropertyPage = MonitoredSqlServerInstancePropertiesDialogPropertyPages.Baseline;

                if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    // update the suggestions using the new configuration
                    if (!refreshSuggestionsWorker.IsBusy)
                        refreshSuggestionsWorker.RunWorkerAsync();
                }
            }
        }

        private void alertRecommendationOptionsButton_Click(object sender, System.EventArgs e)
        {
            AlertRecommendationOptionsDialog dialog = new AlertRecommendationOptionsDialog();

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                CalculateSuggestions();
            }
        }

        private void CalculateSuggestions()
        {    
            foreach (AlertBaselineEntry entry in gridBindingSource)
            {
                entry.CalculateSuggestions();
            }

            RefreshRows();
        }

        private void recommendationsGrid_MouseClick(object sender, MouseEventArgs e)
        {
            UltraGrid grid = sender as UltraGrid;
            if (e.Button == MouseButtons.Left)
            {
                UIElement selectedElement = grid.DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));

//                // see if the user clicked the ... button in the value column of the alert 
//                // configuration grid.  This cell is set as non-editable so we have to look
//                // for the mouse click in order to show the dialog.
//                if (grid == alertConfigurationGrid)
//                {
//                    object seco = selectedElement.GetContext();
//                    // only the Value column has an EditorButton
//                    if (seco is EditorButton)
//                    {
//                        object element = selectedElement.GetAncestor(typeof(CellUIElement)) as CellUIElement;
//                        if (element != null)
//                        {
//                            EditCell(((CellUIElement)element).Cell);
//                        }
//                        return;
//                    }
//                }

                if (!(selectedElement is CheckIndicatorUIElement || selectedElement is ImageUIElement
                    || selectedElement is EditorWithTextDisplayTextUIElement))
                    return;

                // logic to handle toggling a checkbox in a non-editable (no cell selection) column
                object contextObject = selectedElement.GetContext();
                if (contextObject is Infragistics.Win.UltraWinGrid.UltraGridColumn)
                {
                    if (((UltraGridColumn)contextObject).Key == "Selected")
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
                            selectedRow.Refresh(RefreshRow.RefreshDisplay);
                            RefreshCurrentRow();
                            UpdateApplyAll();
                        }
                    }
                }
            }
        }

        private void AlertRecommendationsDialog_Load(object sender, System.EventArgs e)
        {
            UltraGridRow[] rows = recommendationsGrid.Rows.GetAllNonGroupByRows();
            if (rows.Length > 0)
            {
                rows[0].Activate();
                rows[0].Selected = true;
            }
            refRangeStrip = alertConfigurationGauge.MainScale.Stripes[0];
            UpdateApplyAll();
        }

        private void alertConfigurationGauge_ValueChanged(object sender, ChartFX.WinForms.Gauge.IndicatorEventArgs e)
        {
            if (gaugeChanging)
                return;

            AlertBaselineEntry abe = gridBindingSource.Current as AlertBaselineEntry;
            if (abe == null)
                return;

            AlertConfigurationItem item = abe.AlertItem;

            if (!currentChanging)
            {
                IndicatorCollection indicators = alertConfigurationGauge.MainScale.Indicators;
                double indicatorValue = Math.Round(e.Indicator.ValueDisplayed);

                // compensate for conversion issues going between double and long
                if (e.Indicator == warningIndicator)
                {
                    abe.GaugeWarningValue = indicatorValue;
                    if (item.GetMetaData().ComparisonType == ComparisonType.GE)
                    {
                        if (indicatorValue > criticalIndicator.ValueDisplayed)
                            abe.GaugeCriticalValue = indicatorValue;
                    }
                    else
                    {
                        if (indicatorValue < criticalIndicator.ValueDisplayed)
                            abe.GaugeCriticalValue = indicatorValue;
                    }
                }
                else if (e.Indicator == criticalIndicator)
                {
                    abe.GaugeCriticalValue = indicatorValue;
                    if (item.GetMetaData().ComparisonType == ComparisonType.GE)
                    {
                        if (indicatorValue < warningIndicator.ValueDisplayed)
                            abe.GaugeWarningValue = indicatorValue;
                    }
                    else
                    {
                        if (indicatorValue > warningIndicator.ValueDisplayed)
                            abe.GaugeWarningValue = indicatorValue;
                    }
                }

                // see if the grids active cell is currently in edit mode
                UltraGridCell activeCell = recommendationsGrid.ActiveCell;
                if (activeCell != null && activeCell.IsInEditMode)
                {
                    EmbeddableEditorBase editor = activeCell.EditorResolved;
                    // editor should sync its value with the grid
                    if (editor != null)
                        editor.Value = activeCell.Value;
                }
                recommendationsGrid.DisplayLayout.Rows.Refresh(RefreshRow.RefreshDisplay);
                ConfigureGauge(abe, false);
            }
        }

        private void ConfigureGauge(AlertBaselineEntry entry, bool adjustMaxValue)
        {   
            BaselineItemData baselineItem = entry.BaselineItem;
            AlertConfigurationItem item = entry.AlertItem;

            MetricDefinition metaData = item.GetMetaData();
            ThresholdOptions options = metaData.Options;

            Threshold warningThreshold = item.ThresholdEntry.WarningThreshold;
            Threshold criticalThreshold = item.ThresholdEntry.CriticalThreshold;

            SectionCollection sections = alertConfigurationGauge.MainScale.Sections;

            alertConfigurationGroupBox.Text = (entry.Selected ? "New " : "Current ") +
                                              string.Format(AlertConfigurationGroupBoxText, item.Name);
            
            // only enable indicators (triangles) for enabled thresholds
            alertConfigurationGauge.MainScale.Indicators[0].Visible = warningThreshold.Enabled;
            alertConfigurationGauge.MainScale.Indicators[1].Visible = criticalThreshold.Enabled;
            // only enable sections (colored background of gauge) for enabled thresholds (Green always shows!)
            alertConfigurationGauge.MainScale.Sections[1].Visible = warningThreshold.Enabled;
            alertConfigurationGauge.MainScale.Sections[2].Visible = criticalThreshold.Enabled;

            // set the alpha level based on selection
            int alpha = entry.Selected ? 255 : 128;

            // force in the section colors
            okSection.Color = Color.FromArgb(alpha, Color.Green);
            warningSection.Color = Color.FromArgb(alpha, Color.Gold);
            criticalSection.Color = Color.FromArgb(alpha, Color.Red);
            // force in the indicator colors
            warningIndicator.Color = Color.FromArgb(alpha, Color.Gold);
            criticalIndicator.Color = Color.FromArgb(alpha, Color.Red);
            // indicators can be dragged if entry is selected
            warningIndicator.Draggable = entry.Selected;
            criticalIndicator.Draggable = entry.Selected;

            long rs1 = Convert.ToInt64(entry.GaugeWarningValue);
            long rs2 = Convert.ToInt64(entry.GaugeCriticalValue);

            decimal gaugeMax = metaData.MaxValue;
            if (adjustMaxValue)
            {
                if ((options & ThresholdOptions.CalculateMaxValue) == ThresholdOptions.CalculateMaxValue)
                {
                    if (warningThreshold.Enabled || criticalThreshold.Enabled)
                    {
                        long maxValue = criticalThreshold.Enabled ? rs2 : rs1;
                        if (metaData.ComparisonType == ComparisonType.LE)
                            maxValue = warningThreshold.Enabled ? rs1 : rs2;

                        // make sure auto adjust items will show the entire reference range 
                        if (baselineItem.ReferenceRangeEnd.HasValue && baselineItem.ReferenceRangeEnd.Value > maxValue)
                            maxValue = Convert.ToInt64(baselineItem.ReferenceRangeEnd.Value);
                        
                        gaugeMax = metaData.GetVisualUpperBound(maxValue);
                    }

                }
            }
            else
                gaugeMax = Convert.ToDecimal(alertConfigurationGauge.MainScale.Max);

            if (gaugeMax == 0m)
                gaugeMax = 100m;

            alertConfigurationGauge.MainScale.Max = Convert.ToDouble(gaugeMax);
            alertConfigurationGauge.MainScale.Min = Convert.ToDouble(metaData.MinValue);

            if (metaData.ComparisonType == ComparisonType.GE)
            {
                double nextBoundary = alertConfigurationGauge.MainScale.Max;
                if (criticalThreshold.Enabled)
                {
                    criticalSection.Max = nextBoundary;
                    criticalSection.Min = rs2;
                    nextBoundary = rs2;
                }

                if (warningThreshold.Enabled)
                {
                    warningSection.Max = nextBoundary;
                    warningSection.Min = rs1;
                    nextBoundary = rs1;
                }
                okSection.Min = alertConfigurationGauge.MainScale.Min;
                okSection.Max = nextBoundary;
            }
            else
            {
                double nextBoundary = alertConfigurationGauge.MainScale.Min;
                if (criticalThreshold.Enabled)
                {
                    criticalSection.Min = nextBoundary;
                    criticalSection.Max = rs2;
                    nextBoundary = rs2;
                }
                if (warningThreshold.Enabled)
                {
                    warningSection.Max = rs1;
                    warningSection.Min = nextBoundary;
                    nextBoundary = rs1;
                }
                okSection.Min = nextBoundary;
                okSection.Max = alertConfigurationGauge.MainScale.Max;
            }
            try
            {
                gaugeChanging = true;
                warningIndicator.Value = rs1;
                criticalIndicator.Value = rs2;
            }
            finally
            {
                gaugeChanging = false;
            }

            refRangeStrip = alertConfigurationGauge.MainScale.Stripes[0];
            if (baselineItem.ReferenceRangeStart.HasValue &&
                baselineItem.ReferenceRangeEnd.HasValue)
            {
                refRangeStrip.Min = Convert.ToDouble(baselineItem.ReferenceRangeStart.Value);
                refRangeStrip.Max = Convert.ToDouble(baselineItem.ReferenceRangeEnd.Value);
                refRangeStrip.Visible = true;
            } else
                refRangeStrip.Visible = false;

            refRangeStrip.Color = Color.FromArgb(alpha, Color.Blue);
        }


        #region AlertBaselineEntry Class

        public class AlertBaselineEntry
        {
            private readonly AlertConfigurationItem alertItem;

            private BaselineItemData baselineItem;
            private bool selected = false;

            // our suggestions
            private decimal? suggestedWarning;
            private decimal? suggestedCritical;
            // manual changes
            private decimal? tweakedWarning;
            private decimal? tweakedCritical;

            public AlertBaselineEntry(AlertConfigurationItem alertItem, BaselineItemData baselineItem)
            {
                if (alertItem == null)
                    throw new ArgumentNullException("alertItem");

                this.alertItem = alertItem;
                this.baselineItem = baselineItem;

                CalculateSuggestions();
            }

            public bool Selected
            {   
                get { return selected; }
                set 
                {
                    if (selected != value)
                    {
                        selected = value;
                        // any time selected changes to true - kill tweaks
                        tweakedWarning = tweakedCritical = null;
                    }
                }
            }

            public bool Tweaked
            {   // can be selected or tweaked but not both
                get { return !Selected && tweakedWarning.HasValue || tweakedCritical.HasValue; }
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
                get { return alertItem.RangeStart2; }
                set { alertItem.FlattenedThresholds[2].Value = value; }
            }

            public object CriticalThreshold
            {
                get { return alertItem.RangeStart3; }
                set { alertItem.FlattenedThresholds[3].Value = value; }
            }

            public object GaugeWarningValue
            {
                get
                {
                    if (selected)
                    {   // if tweaked - show tweaked
                        if (tweakedWarning.HasValue)
                            return tweakedWarning;
                        // not tweaked - show suggestion
                        return SuggestedWarning;
                    }
                    // not selected - show current
                    return WarningThreshold;
                }
                set
                {
                    // keep track of tweaked value
                    tweakedWarning = Convert.ToDecimal(value);
                }
            }

            public object GaugeCriticalValue
            {
                get
                {
                    if (selected)
                    {   // if tweaked - show tweaked
                        if (tweakedCritical.HasValue)
                            return tweakedCritical;
                        // not tweaked - show suggestion
                        return SuggestedCritical;
                    }
                    //not selected - show current
                    return CriticalThreshold;
                }
                set
                {   
                    // keep track of tweaked value
                    tweakedCritical = Convert.ToDecimal(value);
                }
            }

            public object SuggestedWarning
            {
                get
                {                     
                    return suggestedWarning;
                }
            }

            public object SuggestedCritical
            {
                get
                {
                    return suggestedCritical;
                }
            }

            internal AlertConfigurationItem AlertItem
            {
                get { return alertItem; }
            }

            internal BaselineItemData BaselineItem
            {
                get { return baselineItem; }
                set
                {
                    if (value == null)
                        throw new ArgumentNullException("BaselineItem");

                    baselineItem = value;
                    CalculateSuggestions();
                }
            }

            internal void ApplyChange()
            {
                if (Selected)
                {
                    WarningThreshold = GaugeWarningValue;
                    CriticalThreshold = GaugeCriticalValue;
                }
            }

            internal void CalculateSuggestions()
            {
                int warningPercent = Settings.Default.AlertRecommendationWarningThesholdPercentage;
                int criticalPercent = Settings.Default.AlertRecommendationCriticalThesholdPercentage;
                decimal? referenceRangeValue = baselineItem.ReferenceRangeEnd;

                MetricDefinition metricMetaData = alertItem.GetMetaData();

                if (metricMetaData.ComparisonType == ComparisonType.LE)
                {
                    warningPercent = warningPercent * -1;
                    criticalPercent = criticalPercent * -1;
                    referenceRangeValue = baselineItem.ReferenceRangeStart;
                }

                long ceiling = metricMetaData.MaxValue;
                long floor = metricMetaData.MinValue;

                if (referenceRangeValue == null)
                    suggestedWarning = Convert.ToDecimal(WarningThreshold);
                else
                    suggestedWarning = Calc(referenceRangeValue.Value, warningPercent, floor, ceiling);

                if (referenceRangeValue == null)
                    suggestedCritical = Convert.ToDecimal(CriticalThreshold);
                else
                    suggestedCritical = Calc(referenceRangeValue.Value, criticalPercent, floor, ceiling);

                if (metricMetaData.ComparisonType == ComparisonType.LE)
                {
                    if (suggestedWarning == suggestedCritical)
                    {
                        if (suggestedCritical > floor)
                            suggestedCritical--;
                        else
                            suggestedWarning++;
                    }
                    if (suggestedWarning < suggestedCritical)
                    {
                        SwapSuggestion();
                    } 
                } else
                {
                    if (suggestedWarning == suggestedCritical)
                    {
                        if (suggestedCritical < ceiling)
                            suggestedCritical++;
                        else
                            suggestedWarning--;
                    }
                    if (suggestedWarning > suggestedCritical)
                    {
                        SwapSuggestion();
                    } 
                }
            }

            private void SwapSuggestion()
            {
                decimal? temp = suggestedWarning;
                suggestedWarning = suggestedCritical;
                suggestedCritical = temp;
            }

            internal static decimal Calc(decimal referenceRangeValue, int adjustment, long lowerLimit, long upperLimit)
            {
                decimal result = Math.Round(referenceRangeValue * (1m + adjustment / 100m), 0);

                if (result > upperLimit)
                    result = upperLimit;
                if (result < lowerLimit)
                    result = lowerLimit;

                return result;
            }
        }
        #endregion

        private void recommendationsGrid_InitializeRow(object sender, InitializeRowEventArgs e)
        {

        }

        private void gridBindingSource_CurrentChanged(object sender, EventArgs e)
        {
            try
            {
                currentChanging = true;

                AlertBaselineEntry current = gridBindingSource.Current as AlertBaselineEntry;
                if (current == null)
                    return;

                // whenever the current row changes reconfigure the grid to show the correct columns
//                UltraGridColumn valueColumn = alertConfigurationGrid.DisplayLayout.Bands[0].Columns["Value"];
//                UltraGridColumn startColumn = alertConfigurationGrid.DisplayLayout.Bands[0].Columns["RangeStart"];
//                UltraGridColumn endColumn = alertConfigurationGrid.DisplayLayout.Bands[0].Columns["RangeEnd"];
//                UltraGridColumn enabledColumn = alertConfigurationGrid.DisplayLayout.Bands[0].Columns["Enabled"];

                AlertConfigurationItem item = current.AlertItem;
                MetricDefinition metaData = item.GetMetaData();
                ThresholdOptions options = metaData.Options;
                System.Type valueType = metaData.ValueType;
                
                if (metaData.IsConfigurable)
                {
                    // hide value and show Range start and end
//                    startColumn.Hidden = false;
//                    endColumn.Hidden = false;
//                    valueColumn.Hidden = true;

                    alertConfigurationGauge.Parent.Visible = true;

                    warningSection = alertConfigurationGauge.MainScale.Sections[1];
                    if (metaData.ComparisonType == ComparisonType.GE)
                    {
                        okSection = alertConfigurationGauge.MainScale.Sections[0];
                        criticalSection = alertConfigurationGauge.MainScale.Sections[2];
                        warningIndicator = alertConfigurationGauge.MainScale.Indicators[0];
                        criticalIndicator = alertConfigurationGauge.MainScale.Indicators[1];
                    }
                    else
                    {
                        okSection = alertConfigurationGauge.MainScale.Sections[2];
                        criticalSection = alertConfigurationGauge.MainScale.Sections[0];
                        warningIndicator = alertConfigurationGauge.MainScale.Indicators[1];
                        criticalIndicator = alertConfigurationGauge.MainScale.Indicators[0];
                    }

                    // calc the high end of the gauge
                    decimal gaugeMax = metaData.MaxValue;
                    if ((options & ThresholdOptions.CalculateMaxValue) == ThresholdOptions.CalculateMaxValue)
                    {
                        long criticalThreshold = (long) Convert.ChangeType(item.RangeStart2, typeof (long));
                        gaugeMax = metaData.GetVisualUpperBound(criticalThreshold);
                    }
                    alertConfigurationGauge.MainScale.Max = Convert.ToDouble(gaugeMax);
                    alertConfigurationGauge.MainScale.Sections[2].Max = alertConfigurationGauge.MainScale.Max;
                    // set the indicators to match the threshold values
                    warningIndicator.Value = item.RangeStart1;
                    criticalIndicator.Value = item.RangeStart2;

//                    startColumn.MinValue = metaData.MinValue;
//                    startColumn.MaxValue = metaData.MaxValue;

                    ConfigureGauge(current, true);
                }
//                    if ((options & ThresholdOptions.MutuallyExclusive) == ThresholdOptions.MutuallyExclusive)
//                        enabledColumn.ValueList = alertConfigurationGrid.DisplayLayout.ValueLists["RadioButtons"];
//                    else
//                        enabledColumn.ValueList = alertConfigurationGrid.DisplayLayout.ValueLists["CheckBoxes"];

//                    advancedPanel.Visible = item.MetricID != (int)Metric.IndexRowHits &&
//                                            item.MetricID != (int)Metric.FullTextRefreshHours;
//                    editButton.Visible = !readOnly && !valueColumn.Hidden;
//                UpdateInformationLabel(item);
            }
            finally
            {
                currentChanging = false;
            }

        }

        private void refreshSuggestionsWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            SqlConnectionInfo connectionInfo = Settings.Default.ActiveRepositoryConnection.ConnectionInfo;
            using (SqlConnection connection = connectionInfo.GetConnection(Idera.SQLdm.Common.Constants.DesktopClientConnectionStringApplicationName))
            {
                connection.Open();
                
                List<BaselineItemData> baseline = BaselineHelpers.GetBaseline(connection, alertConfiguration.InstanceID, true);
                e.Result = BaselineHelpers.FilterRecommendations(alertConfiguration, baseline);
            }
        }

        private void refreshSuggestionsWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                ApplicationMessageBox.ShowError(this, e.Error);
                return;  
            }
            if (e.Cancelled)
                return;

            // update the list of suggestions 
            RefreshSuggestions(e.Result as List<BaselineItemData>);
            RefreshRows();
        }

        private void RefreshRows()
        {
            // sync selected row in grid and gauge
            RefreshCurrentRow();
            // make sure all rows get refreshed in the grid
            recommendationsGrid.Rows.Refresh(RefreshRow.RefreshDisplay);

            UpdateApplyAll();
        }

        private void RefreshCurrentRow()
        {
            gridBindingSource.ResetCurrentItem();
            ConfigureGauge((AlertBaselineEntry)gridBindingSource.Current, true);
        }

        private void applyAllCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (checkChanging)
                return;

            bool checkState = applyAllCheckBox.Checked;
            bool changed = false;

            foreach (UltraGridRow row in recommendationsGrid.Rows.GetAllNonGroupByRows())
            {
                AlertBaselineEntry abe = row.ListObject as AlertBaselineEntry;
                if (abe != null)
                {
                    if (abe.Selected != checkState)
                    {
                        abe.Selected = checkState;
                        changed = true;
                    }
                }
            }
            if (changed)
                RefreshRows();
        }

        private void UpdateApplyAll()
        {
            bool checkState = true;
            foreach (UltraGridRow row in recommendationsGrid.Rows.GetAllNonGroupByRows())
            {
                AlertBaselineEntry abe = row.ListObject as AlertBaselineEntry;
                if (abe != null)
                {
                    if (!abe.Selected)
                    {
                        checkState = false;
                    }
                }
            }
            try
            {
                checkChanging = true;
                applyAllCheckBox.Checked = checkState;
            } finally
            {
                checkChanging = false;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            foreach (AlertBaselineEntry entry in gridBindingSource)
            {
                entry.ApplyChange();
            }
        }

        private const string BASELINE_TOOLTIP_TEMPLATE =
            "Baseline Statistics\nRange: {0} - {1}\nAverage: {2}\nMaximum: {3}\nStdev: {4}\nCount: {5}";

        private void alertConfigurationGauge_GetTip(object sender, GetTipEventArgs e)
        {
            string tooltip = String.Empty;

            AlertBaselineEntry current = gridBindingSource.Current as AlertBaselineEntry;
            if (current == null)
                return;

            BaselineItemData baselineItem = current.BaselineItem;
            IHitTestTarget target = e.FoundTargets.TopMostNoneEmptyTarget;
            if (target != null && target is LinearStrip)
            {
                tooltip = String.Format(BASELINE_TOOLTIP_TEMPLATE,
                            ((LinearStrip)target).Min,
                            ((LinearStrip)target).Max,
                            baselineItem.Average,
                            baselineItem.Maximum,
                            baselineItem.Deviation,
                            baselineItem.Count);

            }
            e.SelectedTarget.ToolTip = tooltip;
        }

        private void alertConfigurationGauge_MouseDown(object sender, MouseEventArgs e)
        {
            HitTestTargetStack hit = alertConfigurationGauge.HitTest(e.Location);
            IHitTestTarget target = hit.TopMostNoneEmptyTarget;
            if (target != null && target is LinearStrip)
            {
                Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.AlertsConfigurationBlueBar);
            }
        }

        private void AlertRecommendationsDialog_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            if (e != null) e.Cancel = true;
            ApplicationHelper.ShowHelpTopic(HelpTopics.AlertRecommendationsDialog);
        }

        private void AlertRecommendationsDialog_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            if (hlpevent != null) hlpevent.Handled = true;
            ApplicationHelper.ShowHelpTopic(HelpTopics.AlertRecommendationsDialog);
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