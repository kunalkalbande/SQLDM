using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Events;
using Idera.SQLdm.Common.Thresholds;
using Idera.SQLdm.Common.Data;
using Idera.SQLdm.Common.Objects.ApplicationSecurity;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.Misc;
using ChartFX.WinForms.Gauge;
using System.Windows.Forms;
using Appearance = Infragistics.Win.Appearance;
using ButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle;
using ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle;
using Infragistics.Windows.Themes;
using Idera.SQLdm.DesktopClient.Controls;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    public partial class instanceThresholdDialog : BaseDialog
    {
        private AlertConfigurationItem item = null;
        private InstanceAction action;
        private InstanceType instanceType;
        private bool alertTemplate;
        private List<string> existingInstances; 
        private EditorWithText textEditor;

        private Indicator infoIndicator;
        private Indicator warningIndicator;
        private Indicator criticalIndicator;
        private Section okSection;
        private Section warningSection;
        private Section criticalSection;
        private Section infoSection;
        readonly Threshold oldInfoThreshold = null;
        readonly Threshold oldWarningThreshold = null;
        readonly Threshold oldCriticalThreshold = null;
        private readonly object oldAdvancedSettings = null;
        readonly string oldConfiguredAlertValue = String.Empty; //SQLdm 8.6 -(Ankit Srivastava) : Preferred Node Feature -- old property ConfigAlertValue 

        private bool gaugeChanging;
        private bool currentChanging = false;
        private bool retrieveInstanceRunning = false;
        private PermissionType permissionType = PermissionType.View;
        Type checktype;

        public instanceThresholdDialog(AlertConfigurationItem item, InstanceAction iAction, bool alertTemplate, List<string> existingInstances, PermissionType permission)
        {
            InitializeComponent();
            this.existingInstances = existingInstances;
            this.item = item;
            action =  iAction;
            this.alertTemplate = alertTemplate;
            instanceType = item.InstanceType;
            this.permissionType = permission;

            instanceConfigurationGrid.DrawFilter = new HideFocusRectangleDrawFilter();

            configBindSource.DataSource = item;

            //SQLdm 8.6 (Ankit Srivastava) -Preferred Node feature - Setting the visibility of replica instance name text box and label 
            if (item.MetricID == (int)Metric.PreferredNodeUnavailability)
            {
                replicaNameTextBox.Visible = label2.Visible = true;
                alertConfigurationGroupBox.Visible = false;
            }
            else
            {
                replicaNameTextBox.Visible = label2.Visible = false;
                alertConfigurationGroupBox.Visible = true;
            }

            if (action == InstanceAction.Add) 
            {
                currentChanging = true;
            }
            if (action  == InstanceAction.Edit)
            {
                currentChanging = true;



                oldInfoThreshold = new Threshold(item.ThresholdEntry.InfoThreshold.Enabled,
                                                           item.ThresholdEntry.InfoThreshold.Op,                                        
                                                           (IComparable)item.ThresholdEntry.InfoThreshold.Value);
                oldWarningThreshold = new Threshold(item.ThresholdEntry.WarningThreshold.Enabled,
                                                              item.ThresholdEntry.WarningThreshold.Op,
                                                              (IComparable)item.ThresholdEntry.WarningThreshold.Value);
                oldCriticalThreshold = new Threshold(item.ThresholdEntry.CriticalThreshold.Enabled,
                                                               item.ThresholdEntry.CriticalThreshold.Op,
                                                               (IComparable)item.ThresholdEntry.CriticalThreshold.Value);
                oldConfiguredAlertValue = item.ConfiguredAlertValue; //SQLdm 8.6 -(Ankit Srivastava) : Preferred Node Feature -- saving old property ConfigAlertValue into the vairable
                oldAdvancedSettings = item.ThresholdEntry.Data == null ? null : DeepClone(item.ThresholdEntry.Data);
            }
            AdaptFontSize();
            SetGridTheme();
            SetPropertiesTheme();
            updateLinearScaleFontAsPerTheme(this.linearScale1);
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
        }

        void OnCurrentThemeChanged(object sender, EventArgs e)
        {
            SetGridTheme();
            SetPropertiesTheme();
            updateLinearScaleFontAsPerTheme(this.linearScale1);
        }
        public void updateLinearScaleFontAsPerTheme(LinearScale linearscale)
        {
            ThemeSetter ts = new ThemeSetter();
            ts.SetLinearScale(linearscale);
        }

        void SetPropertiesTheme()
        {
            var propertiesThemeManager = new Controls.PropertiesThemeManager();
            propertiesThemeManager.UpdatePropertyTheme(office2007PropertyPage1);
        }

        private void SetGridTheme()
        {
            // Update UltraGrid Theme
            var themeManager = new GridThemeManager();
            themeManager.updateGridTheme(this.instanceConfigurationGrid);
        }

        private void instanceThresholdDialog_Load(object sender, EventArgs e)
        {
            if (action == InstanceAction.Add && !alertTemplate)
                retrieveInstancesWorker.RunWorkerAsync();
            else if (action == InstanceAction.Add && alertTemplate)
                instanceSelector.Text = String.Empty;
            else
                instanceSelector.Text = item.IsDefaultThreshold ? MetricThresholdEntry.DEFAULT_THRESHOLD_NAME : item.MetricInstance;

            this.Text = string.Format(this.Text, item.InstanceType.ToString());

            //changed for the context based toop tip by Gaurav Karwal for SQLdm 8.6
            this.ultraValidator1.GetValidationSettings(this.instanceSelector).NotificationSettings.Text = string.Format("You must select or specify a unique {0} name.", item.InstanceType.ToString());

            label1.Text = item.InstanceType == InstanceType.Disk ? "Disk / Mount Point" : item.InstanceType.ToString();
            office2007PropertyPage1.Text = string.Format(office2007PropertyPage1.Text, item.MetricDescription.Name);
            //SQLdm 8.6 (Ankit Srivastava) -Preferred Node feature - Setting the text of replica instance name text box with the exisiting value 
            replicaNameTextBox.Text = item.ThresholdEntry.Data == null ? String.Empty : (new AdvancedAlertConfigurationSettings(MetricDefinition.GetMetric(item.MetricID), item.ThresholdEntry.Data)).ReplicaInstanceName; 

            textEditor = new EditorWithText();

            // these options don't retain the value set in the designer.  Setting them causes the row
            // to be selected when clicked rather than the default which is cell selection.
            UltraGridColumn column = instanceConfigurationGrid.DisplayLayout.Bands[0].Columns["Enabled"];
            column.CellClickAction = CellClickAction.RowSelect;
            column.Editor = textEditor;
            column.CellAppearance.ImageHAlign = HAlign.Center;
            column = instanceConfigurationGrid.DisplayLayout.Bands[0].Columns["ThresholdItemType"];
            column.CellClickAction = CellClickAction.RowSelect;
            column = instanceConfigurationGrid.DisplayLayout.Bands[0].Columns["RangeEnd"];
            column.CellClickAction = CellClickAction.RowSelect;
            column = instanceConfigurationGrid.DisplayLayout.Bands[0].Columns["Value"];
            column.CellClickAction = CellClickAction.RowSelect;
            // set range start as editable
            column = instanceConfigurationGrid.DisplayLayout.Bands[0].Columns["RangeStart"];
            column.CellClickAction = CellClickAction.EditAndSelectText;

            MetricDefinition metaData = item.GetMetaData();
            ThresholdOptions options = metaData.Options;
            UltraGridColumn valueColumn = instanceConfigurationGrid.DisplayLayout.Bands[0].Columns["Value"];
            UltraGridColumn startColumn = instanceConfigurationGrid.DisplayLayout.Bands[0].Columns["RangeStart"];
            UltraGridColumn endColumn = instanceConfigurationGrid.DisplayLayout.Bands[0].Columns["RangeEnd"];
            UltraGridColumn enabledColumn = instanceConfigurationGrid.DisplayLayout.Bands[0].Columns["Enabled"];

            //SQLdm 8.6 (Ankit Srivastava) -Preferred Node feature -Added the event for saving the replica instance name
            if ((options & ThresholdOptions.MutuallyExclusive) == ThresholdOptions.MutuallyExclusive)
            {
                valueColumn.Hidden = true;
                startColumn.Hidden = true;
                endColumn.Hidden = true;
                enabledColumn.ValueList = instanceConfigurationGrid.DisplayLayout.ValueLists["RadioButtons"];
            }
            else
            {
                valueColumn.Hidden = false;
                startColumn.Hidden = false;
                endColumn.Hidden = false;
                enabledColumn.ValueList = instanceConfigurationGrid.DisplayLayout.ValueLists["CheckBoxes"];
            }
                
   

            if (metaData.ComparisonType == ComparisonType.GE)
            {
                okSection = alertConfigurationGauge.MainScale.Sections[0];
                infoSection = alertConfigurationGauge.MainScale.Sections[1];
                warningSection = alertConfigurationGauge.MainScale.Sections[2];
                criticalSection = alertConfigurationGauge.MainScale.Sections[3];
                infoIndicator = alertConfigurationGauge.MainScale.Indicators[0];
                warningIndicator = alertConfigurationGauge.MainScale.Indicators[1];
                criticalIndicator = alertConfigurationGauge.MainScale.Indicators[2];
            }
            else
            {
                okSection = alertConfigurationGauge.MainScale.Sections[3];
                infoSection = alertConfigurationGauge.MainScale.Sections[2];
                warningSection = alertConfigurationGauge.MainScale.Sections[1];
                criticalSection = alertConfigurationGauge.MainScale.Sections[0];
                infoIndicator = alertConfigurationGauge.MainScale.Indicators[2];
                warningIndicator = alertConfigurationGauge.MainScale.Indicators[1];
                criticalIndicator = alertConfigurationGauge.MainScale.Indicators[0];
            }

            decimal gaugeMax = metaData.MaxValue;
            if ((options & ThresholdOptions.CalculateMaxValue) == ThresholdOptions.CalculateMaxValue)
            {
                long criticalThreshold = (long)Convert.ChangeType(item.RangeStart3, typeof(long));
                gaugeMax = metaData.GetVisualUpperBound(criticalThreshold);
            }

            alertConfigurationGauge.MainScale.Max = Convert.ToDouble(gaugeMax);
            alertConfigurationGauge.MainScale.Sections[3].Max = alertConfigurationGauge.MainScale.Max;
            // set the indicators to match the threshold values
            infoIndicator.Value = item.RangeStart1;
            warningIndicator.Value = item.RangeStart2;
            criticalIndicator.Value = item.RangeStart3;

            startColumn.MinValue = metaData.MinValue;
            startColumn.MaxValue = metaData.MaxValue;

            currentChanging = false;                

            if (action == InstanceAction.Add)
            {
                instanceSelector.Textbox.Focus();
            }

            instanceSelector.DropDownButtonDisplayStyle = alertTemplate ? ButtonDisplayStyle.Never : ButtonDisplayStyle.Always;

            ConfigureGauge(item, true);

            UpdateControls();

        }

        private void ConfigureGauge(AlertConfigurationItem item, bool adjustMaxValue)
        {
            MetricDefinition metaData = item.GetMetaData();
            ThresholdOptions options = metaData.Options;

            Threshold infoThreshold = null;
            Threshold warningThreshold = null;
            Threshold criticalThreshold = null;

            infoThreshold = item.ThresholdEntry.InfoThreshold;
            warningThreshold = item.ThresholdEntry.WarningThreshold;
            criticalThreshold = item.ThresholdEntry.CriticalThreshold;

            SectionCollection sections = alertConfigurationGauge.MainScale.Sections;

            if (metaData.ComparisonType == ComparisonType.GE)
            {
                // only enable indicators (triangles) for enabled thresholds
                alertConfigurationGauge.MainScale.Indicators[0].Visible = infoThreshold.Enabled;
                alertConfigurationGauge.MainScale.Indicators[1].Visible = warningThreshold.Enabled;
                alertConfigurationGauge.MainScale.Indicators[2].Visible = criticalThreshold.Enabled;
                // only enable sections (colored background of gauge) for enabled thresholds (Green always shows!)
                alertConfigurationGauge.MainScale.Sections[1].Visible = infoThreshold.Enabled;
                alertConfigurationGauge.MainScale.Sections[2].Visible = warningThreshold.Enabled;
                alertConfigurationGauge.MainScale.Sections[3].Visible = criticalThreshold.Enabled;
            }
            else
            {
                // only enable indicators (triangles) for enabled thresholds
                alertConfigurationGauge.MainScale.Indicators[2].Visible = infoThreshold.Enabled;
                alertConfigurationGauge.MainScale.Indicators[1].Visible = warningThreshold.Enabled;
                alertConfigurationGauge.MainScale.Indicators[0].Visible = criticalThreshold.Enabled;
                // only enable sections (colored background of gauge) for enabled thresholds (Green always shows!)
                alertConfigurationGauge.MainScale.Sections[2].Visible = infoThreshold.Enabled;
                alertConfigurationGauge.MainScale.Sections[1].Visible = warningThreshold.Enabled;
                alertConfigurationGauge.MainScale.Sections[0].Visible = criticalThreshold.Enabled;
            }

            // force in the section colors
            okSection.Color = Color.Green;
            infoSection.Color = Color.Blue;
            warningSection.Color = Color.Gold;
            criticalSection.Color = Color.Red;

            infoIndicator.Color = Color.Blue;
            warningIndicator.Color = Color.Gold;
            criticalIndicator.Color = Color.Red;

            okSection.Visible = true;
            infoSection.Visible = infoThreshold.Enabled;
            warningSection.Visible = warningThreshold.Enabled;
            criticalSection.Visible = criticalThreshold.Enabled;

            long rs1 = Convert.ToInt64(item.RangeStart1);
            long rs2 = Convert.ToInt64(item.RangeStart2);
            long rs3 = Convert.ToInt64(item.RangeStart3);

            decimal gaugeMax = metaData.MaxValue;
            if (adjustMaxValue)
            {
                if ((options & ThresholdOptions.CalculateMaxValue) == ThresholdOptions.CalculateMaxValue)
                {
                    if (infoThreshold.Enabled || warningThreshold.Enabled || criticalThreshold.Enabled)
                    {
                        BaselineItemData baselineItem;

                        long maxValue = criticalThreshold.Enabled ? rs3 : warningThreshold.Enabled ? rs2 : rs1;
                        if (metaData.ComparisonType == ComparisonType.LE)
                            maxValue = infoThreshold.Enabled ? rs1 : warningThreshold.Enabled ? rs2 : rs3;

                        //// see if there is a baseline item for this sucker
                        //if (baselineData.TryGetValue(item.MetricID, out baselineItem))
                        //{
                        //    // make sure auto adjust items will show the entire reference range 
                        //    if (baselineItem.ReferenceRangeEnd.HasValue && baselineItem.ReferenceRangeEnd.Value > maxValue)
                        //        maxValue = Convert.ToInt64(baselineItem.ReferenceRangeEnd.Value);
                        //}

                        gaugeMax = metaData.GetVisualUpperBound(maxValue);
                    }
                }
            }
            else
                gaugeMax = Convert.ToDecimal(alertConfigurationGauge.MainScale.Max);

            if (gaugeMax == 0m)
                gaugeMax = 100m;

            alertConfigurationGauge.MainScale.Max = Convert.ToDouble(gaugeMax);
            alertConfigurationGauge.MainScale.Min = metaData.MinValue;

            if (metaData.ComparisonType == ComparisonType.GE)
            {
                double nextBoundary = alertConfigurationGauge.MainScale.Max;
                if (criticalThreshold.Enabled)
                {
                    criticalSection.Max = nextBoundary;
                    criticalSection.Min = rs3;
                    nextBoundary = rs3;
                }
                if (warningThreshold.Enabled)
                {
                    warningSection.Max = nextBoundary;
                    warningSection.Min = rs2;
                    nextBoundary = rs2;
                }
                if (infoThreshold.Enabled)
                {
                    infoSection.Max = nextBoundary;
                    infoSection.Min = rs1;
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
                    criticalSection.Max = rs3;
                    nextBoundary = rs3;
                }
                if (warningThreshold.Enabled)
                {
                    warningSection.Max = rs2;
                    warningSection.Min = nextBoundary;
                    nextBoundary = rs2;
                }
                if (infoThreshold.Enabled)
                {
                    infoSection.Max = rs1;
                    infoSection.Min = nextBoundary;
                    nextBoundary = rs1;
                }
                okSection.Min = nextBoundary;
                okSection.Max = alertConfigurationGauge.MainScale.Max;
            }
            try
            {
                gaugeChanging = true;
                infoIndicator.Value = rs1;
                warningIndicator.Value = rs2;
                criticalIndicator.Value = rs3;
            }
            finally
            {
                gaugeChanging = false;
            }
        }

        private void alertConfigurationGauge_ValueChanged(object sender, ChartFX.WinForms.Gauge.IndicatorEventArgs e)
        {
            if (gaugeChanging)
                return;

            if (item == null)
                return;

            IndicatorCollection indicators = alertConfigurationGauge.MainScale.Indicators;
            double indicatorValue = e.Indicator.ValueDisplayed;

            if (!currentChanging)
            {
                // compensate for conversion issues going between double and long
                if (e.Indicator == infoIndicator)
                {
                    item.RangeStart1 = indicatorValue;
                    if (item.GetMetaData().ComparisonType == ComparisonType.GE)
                    {
                        if (indicatorValue > warningIndicator.ValueDisplayed)
                            item.RangeStart2 = indicatorValue;
                        if (indicatorValue > criticalIndicator.ValueDisplayed)
                            item.RangeStart3 = indicatorValue;
                    }
                    else
                    {
                        if (indicatorValue < warningIndicator.ValueDisplayed)
                            item.RangeStart2 = indicatorValue;
                        if (indicatorValue < criticalIndicator.ValueDisplayed)
                            item.RangeStart3 = indicatorValue;
                    }
                }
                else if (e.Indicator == warningIndicator)
                {
                    item.RangeStart2 = indicatorValue;
                    if (item.GetMetaData().ComparisonType == ComparisonType.GE)
                    {
                        if (indicatorValue < infoIndicator.ValueDisplayed)
                            item.RangeStart1 = indicatorValue;
                        if (indicatorValue > criticalIndicator.ValueDisplayed)
                            item.RangeStart3 = indicatorValue;
                    }
                    else
                    {
                        if (indicatorValue > infoIndicator.ValueDisplayed)
                            item.RangeStart1 = indicatorValue;
                        if (indicatorValue < criticalIndicator.ValueDisplayed)
                            item.RangeStart3 = indicatorValue;
                    }
                }
                else if (e.Indicator == criticalIndicator)
                {
                    item.RangeStart3 = indicatorValue;
                    if (item.GetMetaData().ComparisonType == ComparisonType.GE)
                    {
                        if (indicatorValue < infoIndicator.ValueDisplayed)
                            item.RangeStart1 = indicatorValue;
                        if (indicatorValue < warningIndicator.ValueDisplayed)
                            item.RangeStart2 = indicatorValue;
                    }
                    else
                    {
                        if (indicatorValue > infoIndicator.ValueDisplayed)
                            item.RangeStart1 = indicatorValue;
                        if (indicatorValue > warningIndicator.ValueDisplayed)
                            item.RangeStart2 = indicatorValue;
                    }
                }


                // see if the grids active cell is currently in edit mode
                UltraGridCell activeCell = instanceConfigurationGrid.ActiveCell;
                if (activeCell != null && activeCell.IsInEditMode)
                {
                    EmbeddableEditorBase editor = activeCell.EditorResolved;
                    // editor should sync its value with the grid
                    if (editor != null)
                        editor.Value = activeCell.Value;
                }
                instanceConfigurationGrid.DisplayLayout.Rows.Refresh(RefreshRow.RefreshDisplay);
                ConfigureGauge(item, false);
                //               RefilterRecommendations();
            }
        }

        private void instanceConfigurationGrid_BeforeCellActivate(object sender, CancelableCellEventArgs e)
        {
            // if the column is not RangeStart then cancel the activation and select the row
            if (e.Cell.Column.Key != "RangeStart")
            {
                e.Cancel = true;
                e.Cell.Row.Selected = true;
                e.Cell.Row.Activate();
            }
        }

        private void instanceConfigurationGrid_AfterCellUpdate(object sender, CellEventArgs e)
        {
            // update the values of the alertConfigurationGauge
            MetricDefinition metaData = item.GetMetaData();

            long rs1 = Convert.ToInt64(item.RangeStart1); // infoThreshold starting Range
            long rs2 = Convert.ToInt64(item.RangeStart2); // warningThreshold starting Range
            long rs3 = Convert.ToInt64(item.RangeStart3); // criticalThreshold starting Range

            int rowIndex = e.Cell.Row.Index;

            if (metaData.ComparisonType == ComparisonType.GE)
            {
                switch (rowIndex)
                {
                    case 1:   // Info Start Range was changed
                        if (rs1 > rs3)
                            item.RangeStart3 = item.RangeStart2 = rs1;
                        else if (rs1 > rs2)
                            item.RangeStart2 = rs1;
                        break;
                    case 2:  // Warning start range was changed
                        if (rs2 < rs1)
                            item.RangeStart1 = rs2;
                        else if (rs2 > rs3)
                            item.RangeStart3 = rs2;
                        break;
                    case 3:
                        if (rs3 < rs1)
                            item.RangeStart1 = item.RangeStart2 = rs3;
                        else if (rs3 < rs2)
                            item.RangeStart2 = rs3;
                        break;
                    default:
                        break;
                }
            }
            if (metaData.ComparisonType == ComparisonType.LE)
            {
                switch (rowIndex)
                {
                    case 1:   // Info Start Range was changed
                        if (rs1 < rs3)
                            item.RangeStart3 = item.RangeStart2 = rs1;
                        else if (rs1 < rs2)
                            item.RangeStart2 = rs1;
                        break;
                    case 2:  // Warning start range was changed
                        if (rs2 > rs1)
                            item.RangeStart1 = rs2;
                        else if (rs2 < rs3)
                            item.RangeStart3 = rs2;
                        break;
                    case 3:
                        if (rs3 > rs1)
                            item.RangeStart1 = item.RangeStart2 = rs3;
                        else if (rs3 > rs2)
                            item.RangeStart2 = rs3;
                        break;
                    default:
                        break;
                }
            }

            ConfigureGauge(item, (metaData.Options & ThresholdOptions.CalculateMaxValue) == ThresholdOptions.CalculateMaxValue);

            instanceConfigurationGrid.DisplayLayout.Rows.Refresh(RefreshRow.ReloadData);
        }

        private static ImageBackgroundStretchMargins BG_Stretch_Margins = new ImageBackgroundStretchMargins(1, 1, 1, 1);
        private void instanceConfigurationGrid_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            FlattenedThreshold threshold = e.Row.ListObject as FlattenedThreshold;

            // set activation for editable fields based on meta data for the row
            if (!e.Row.Cells["RangeStart"].Column.Hidden)
            {
                Activation activation = threshold.Enabled && threshold.IsEditable
                                            ? Activation.AllowEdit
                                            : Activation.NoEdit;

                UltraGridCell rangeStartCell = e.Row.Cells["RangeStart"];
                rangeStartCell.Activation =  activation;

                if (activation == Activation.AllowEdit)
                {
                    var appearance = rangeStartCell.Appearance;
                    appearance.ImageBackground = DesktopClient.Properties.Resources.CellBorder;
                    appearance.ImageBackgroundStretchMargins = BG_Stretch_Margins;
                    appearance.ImageBackgroundStyle = ImageBackgroundStyle.Stretched;
                }
            }
            if (threshold.ThresholdItemType == ThresholdItemType.OK)
            {
                UltraGridCell cell = e.Row.Cells["Enabled"];
                cell.Hidden = true;
            }
            MetricDefinition metaData = threshold.GetConfigurationItem().GetMetaData();
            if (threshold.IsNumeric)
            {
                Type valueType = metaData.ValueType;
                ColumnStyle style = ColumnStyle.IntegerNonNegative;
                if (valueType == typeof(double) || valueType == typeof(float))
                    style = ColumnStyle.DoubleNonNegative;
                e.Row.Cells["RangeStart"].Style = style;
                e.Row.Cells["RangeStart"].Column.MaskInput = (metaData.IsCustom) ? "999999999999999" : null;

                e.Row.Cells["RangeStart"].Hidden = !threshold.Enabled;
                e.Row.Cells["RangeEnd"].Hidden = !threshold.Enabled;
            }
            checktype = checktype = metaData.ValueType;
        }

        private void retrieveInstancesWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            retrieveInstanceRunning = true;

            try
            {
                IManagementService defaultManagementService =
                    ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                if (item.ThresholdEntry.MetricInstanceType == InstanceType.Database)
                {

                    IDictionary<string,bool> result ;
                    if (item.ThresholdEntry.MetricID == (int)Metric.PreferredNodeUnavailability) //SQLdm 8.6 (Ankit Srivastava )-- Preferred Node Feature -- fixed defect DE43675
                        result = defaultManagementService.GetDatabases(item.ThresholdEntry.MonitoredServerID, false, true);
                    else
                        result = defaultManagementService.GetDatabases(item.ThresholdEntry.MonitoredServerID, true, true);

                    foreach (var instance in existingInstances.Where(result.ContainsKey))
                    {
                        result.Remove(instance);
                    }

                    e.Result = result;
                }
                else if (item.ThresholdEntry.MetricInstanceType == InstanceType.Job)
                {
                    try
                    {
                        if ((int)Metric.LongJobsMinutes == item.ThresholdEntry.MetricID || (int)Metric.LongJobs == item.ThresholdEntry.MetricID)
                        {
                            List<string> job = defaultManagementService.GetAgentJobNames(item.ThresholdEntry.MonitoredServerID).ToList();

                            if (job != null && job.Count > 0)
                            {
                                foreach (var instance in existingInstances.Where(job.Contains))
                                {
                                    job.Remove(instance);
                                }
                                e.Result = job;
                            }
                        }
                    }
                    catch (Exception ex)
                    {                        
                    }
                }
                else if (item.ThresholdEntry.MetricInstanceType == InstanceType.Disk)
                {
                    var disks = defaultManagementService.GetDisks(item.ThresholdEntry.MonitoredServerID);

                    foreach (var instance in existingInstances.Where(disks.Contains))
                    {
                        disks.Remove(instance);
                    }

                    e.Result = disks;
                }
            }
            catch (Exception err)
            {
            }
        }

        private void retrieveInstancesWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Error != null)
                {
                    ApplicationMessageBox.ShowError(this, e.Error);
                }
                else
                {
                    if (e.Result is Dictionary<string, bool>)
                    {
                        IDictionary<string, bool> databases = (IDictionary<string, bool>)e.Result;
                        if (databases.Count != 0)
                        {
                            instanceSelector.DataSource = databases.Keys.ToArray();
                        }
                    }
                    else if (e.Result is List<string>)
                    {
                        List<string> disks = e.Result as List<string>;

                        if (disks.Count != 0)
                        {
                            instanceSelector.DataSource = disks.ToArray();
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                retrieveInstanceRunning = false;
            }
        }

        private void instanceConfigurationGrid_ClickCell(object sender, ClickCellEventArgs e)
        {
            try
            {
                if (e.Cell.Column.Key == "Enabled" && permissionType >= PermissionType.Modify)
                {
                    //SQLdm 8.6 (Ankit Srivastava )-- Preferred Node Feature -- fixed defect DE43672
                    if (e.Cell.Column.ValueList != instanceConfigurationGrid.DisplayLayout.ValueLists["RadioButtons"] )
                    {
                            e.Cell.Value = !((bool)(e.Cell.Value));
                    }
                    else //SQLdm 8.6 (Ankit Srivastava )-- Preferred Node Feature -- fixed defect DE43684
                    {
                        e.Cell.Value = true;
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (action == InstanceAction.Add)
                item.IsThresholdNew = true;
            else if (action == InstanceAction.Edit)
                item.IsThresholdChanged = true;
            else if (action == InstanceAction.Delete)
                item.IsThresholdDeleted = true;
            item.ConfiguredAlertValue = item.ThresholdEntry.WarningThreshold.Enabled ? "Warning" : (item.ThresholdEntry.InfoThreshold.Enabled ? "Informational" : "Critical"); //SQLdm 8.6 -(Ankit Srivastava) : Preferred Node Feature -- setting the property ConfigAlertValue accordingly
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (action == InstanceAction.Edit)
            {
                item.ThresholdEntry.InfoThreshold.Enabled = oldInfoThreshold.Enabled;
                item.ThresholdEntry.InfoThreshold.Value = oldInfoThreshold.Value;
                item.ThresholdEntry.WarningThreshold.Enabled = oldWarningThreshold.Enabled;
                item.ThresholdEntry.WarningThreshold.Value = oldWarningThreshold.Value;
                item.ThresholdEntry.CriticalThreshold.Enabled = oldCriticalThreshold.Enabled;
                item.ThresholdEntry.CriticalThreshold.Value = oldCriticalThreshold.Value;
                item.ConfiguredAlertValue = oldConfiguredAlertValue; //SQLdm 8.6 -(Ankit Srivastava) : Preferred Node Feature -- setting the property ConfigAlertValue as the older one if edit cancelled.

                item.ThresholdEntry.Data = oldAdvancedSettings == null ? null : DeepClone(oldAdvancedSettings);
            }
            this.DialogResult = DialogResult.Cancel;
        }

        private bool UltraDropDownHasVerticalScrollbar(UltraCombo uiControl)
        {
            UltraGridUIElement mainElem = uiControl.DisplayLayout.UIElement;

            if (mainElem != null)
                return mainElem.GetDescendant(typeof(RowScrollbarUIElement)) != null;

            return false;
        }

        private void instanceSelector_AfterDropDown(object sender, EventArgs e)
        {
            bool verticalScrollBarPresent = UltraDropDownHasVerticalScrollbar(instanceSelector);
            int gap = (verticalScrollBarPresent ? SystemInformation.VerticalScrollBarWidth : 0);
            instanceSelector.DropDownWidth = instanceSelector.Width;
            instanceSelector.Rows.Band.Columns[0].Width = instanceSelector.DropDownWidth - gap - 2;
        }

        private void instanceSelector_TextChanged(object sender, EventArgs e)
        {
            try
            {
                item.ThresholdEntry.MetricInstanceName = instanceSelector.Textbox.Text == String.Empty ? instanceSelector.Value.ToString() : instanceSelector.Textbox.Text;
            }
            catch (Exception)
            {
                item.ThresholdEntry.MetricInstanceName = string.Empty;
            }
            finally
            {
                UpdateControls();
            }
        }

        //SQLdm 8.6 (Ankit Srivastava) -Preferred Node feature -Added the event for saving the replica instance name
        private void replicaNameTextBox_TextChanged(object sender, EventArgs e)
      {
            // Get the AdvancedSettings from the ACI... create a new one if it hasn't been set
            AdvancedAlertConfigurationSettings settings =
                item.ThresholdEntry.Data as AdvancedAlertConfigurationSettings ??
                new AdvancedAlertConfigurationSettings(MetricDefinition.GetMetric(item.MetricID), item.ThresholdEntry.Data);
            try
            {
                if (!String.IsNullOrWhiteSpace(replicaNameTextBox.Text))
                    settings.ReplicaInstanceName = replicaNameTextBox.Text;
                else
                    settings.ReplicaInstanceName = string.Empty;
            }
            catch (Exception)
            {
                settings.ReplicaInstanceName = string.Empty;
            }
            finally
            {
                item.SetData(settings);
                UpdateControls();
            }
        }
  

        private void UpdateControls()
        {
            switch (action)
            {
                case InstanceAction.Add:
                    instanceSelector.Enabled = true;
                    btnAdvanced.Enabled = !String.IsNullOrEmpty(instanceSelector.Text) && (replicaNameTextBox.Visible ? !String.IsNullOrWhiteSpace(replicaNameTextBox.Text) : true); //SQLdm 8.6 (Ankit Srivastava) -Preferred Node feature - added replicatextbox too for the button enable disable
                    break;
                case InstanceAction.Edit:
                    instanceSelector.Enabled = false;
                    btnAdvanced.Enabled = true;
                    break;
                case InstanceAction.Delete:
                    instanceSelector.Enabled = false;
                    btnAdvanced.Enabled = false;
                    break;
            }
            btnOK.Enabled = (action != InstanceAction.Add || ultraValidator1.Validate(true, false).IsValid) && (replicaNameTextBox.Visible ? !String.IsNullOrWhiteSpace(replicaNameTextBox.Text) : true); //SQLdm 8.6 (Ankit Srivastava) -Preferred Node feature - added replicatextbox too for the button enable disable
        }

        private void btnAdvanced_Click(object sender, EventArgs e)
        {
            // Get the AdvancedSettings from the ACI... create a new one if it hasn't been set
            AdvancedAlertConfigurationSettings settings =
                item.ThresholdEntry.Data as AdvancedAlertConfigurationSettings ??
                new AdvancedAlertConfigurationSettings(MetricDefinition.GetMetric(item.MetricID), item.ThresholdEntry.Data);

            AdvancedAlertConfigurationDialog aacd = new AdvancedAlertConfigurationDialog(alertTemplate ? 0 : item.ThresholdEntry.MonitoredServerID,
                                                                                         item.Name,settings,
                                                                                         alertTemplate, 
                                                                                         item.IsDefaultThreshold,
                                                                                         item.MetricInstance, checktype);  // SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mountpoint Monitoring Improvements --pass threshold instance name (database name) for per database filegroup filters 

            if (aacd.ShowDialog(this) == DialogResult.OK)
            {
                item.SetData(settings);
            }
        }

        private void ultraValidator1_Validating(object sender, Infragistics.Win.Misc.ValidatingEventArgs e)
        {
            string value = instanceSelector.Text.Trim();

            if (String.IsNullOrEmpty(value))
            {
                e.IsValid = false;
            }
            else
            {
                e.IsValid = true;
                foreach (var instance in existingInstances.Where(instance => instance.ToLower() == value.ToLower()))
                {
                    e.IsValid = false;
                    break;
                }
            }
        }

        private void instanceSelector_Enter(object sender, EventArgs e)
        {
            UltraCombo combo = (UltraCombo) sender;
            combo.Textbox.MaxLength = 255;
        }

        private void instanceThresholdDialog_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            if (hlpevent != null) hlpevent.Handled = true;
            ShowHelp();
        }

        private void instanceThresholdDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (retrieveInstanceRunning)
                retrieveInstancesWorker.CancelAsync();
        }

        public static T DeepClone<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }

        public void ShowHelp()
        {
            switch (instanceType)
            {
                case InstanceType.Disk:
                    Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.InstanceThresholdsDisk);
                    break;
                default:
                    Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.InstanceThresholdsDatabase);
                    break;
            }
        }

        private void instanceThresholdDialog_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            ShowHelp();
        }

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }

        private void instanceSelector_BeforeDropDown(object sender, CancelEventArgs e)
        {
            if (instanceSelector.Rows.Count == 0)
            {
                string errorMessage = string.Format("An error occurred while adding a database or job.{0}{0}Additional information:{0}\t{1}{0}\t{2}{0}\t{3}{0}\t{4}",
                                                    Environment.NewLine,
                                                    "    Unable to connect to server or invalid credentials entered.",
                                                    "    To troubleshoot:",
                                                    "    1. Verify that server is turned on or that it can accept connections from outside.",
                                                    "    2. Verify that the correct authentication credentials are entered.");

                ApplicationMessageBox.ShowError(ParentForm, errorMessage);
                e.Cancel = true;
            }
        }
    }
}
