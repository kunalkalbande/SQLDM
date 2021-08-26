using Idera.SQLdm.DesktopClient.Properties;
using System.Drawing;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class AlertConfigurationDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                if (textEditor != null)
                    textEditor.Dispose();

                if(components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinEditors.EditorButton editorButton1 = new Infragistics.Win.UltraWinEditors.EditorButton();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance20 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand2 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn45 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("MetricID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn46 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ThresholdEnabled");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn47 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("MetricInstance");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn48 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("RangeStart1");
            Infragistics.Win.Appearance appearance21 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn49 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("RangeStart2");
            Infragistics.Win.Appearance appearance42 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn50 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("RangeStart3");
            Infragistics.Win.Appearance appearance48 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn51 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("MetricDescription");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn52 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ThresholdEntry");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn53 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("InstanceType");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn54 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsDefaultThreshold");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn55 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsMultiInstance");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn56 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Name");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn57 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Category");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn58 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Description");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn59 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Enabled");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn60 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Comments");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn61 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsChanged");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn62 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsThresholdChanged");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn63 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsThresholdNew");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn64 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsThresholdDeleted");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn88 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FlattenedThresholds");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn98 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ConfiguredAlertValue"); //SQLdm 8.6 -(Ankit Srivastava) : Preferred Node Feature -- Added new column for ConfigAlertValue
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn100 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsMetricInfoChanged");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn23 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsInfoEnabled");
            Infragistics.Win.Appearance appearance49 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn24 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsWarningEnabled");
            Infragistics.Win.Appearance appearance50 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn25 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsCriticalEnabled");
            Infragistics.Win.Appearance appearance51 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn87 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ItemExists", 0);
            Infragistics.Win.Appearance appearance25 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand3 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FlattenedThresholds", -1);
            //10.0 Srishti purohit // For baseline alert
            //Making copy of State grid to give baseline alert functioanlity
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBandForBaselineAlertGrid = new Infragistics.Win.UltraWinGrid.UltraGridBand("FlattenedThresholdsBaseline", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn85 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ThresholdItemType", -1, 10018657);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn86 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsEditable");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn89 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsNumeric");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn90 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsMultiValued");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn91 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsMutuallyExclusive");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn92 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsContained");
            
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn93 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Enabled", -1, 5686891);
            Infragistics.Win.Appearance appearance24 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance26 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn94 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("RangeStart");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn95 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("RangeEnd");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn96 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Value");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn97 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("SuggestedValue");
            //10.0 SQLdm Srishti Purohit -- to hide property added to differentiat alert type (normal or baseline)
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumnIsBaselineTypeAlert = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsBaselineTypeAlert");
            
            Infragistics.Win.Appearance appearance28 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance29 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance30 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance31 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance32 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance33 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance34 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance35 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance36 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance37 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList2 = new Controls.CustomControls.CustomValueList(10018657);
            Infragistics.Win.ValueListItem valueListItem3 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem4 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem5 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueList valueList3 = new Controls.CustomControls.CustomValueList(5632719);
            Infragistics.Win.ValueListItem valueListItem6 = new Infragistics.Win.ValueListItem();

            //10.0 Srishti purohit // For baseline alert
            //Making copy of State grid to give baseline alert functioanlity
            Infragistics.Win.ValueList valueListBaselineAlertState = new Controls.CustomControls.CustomValueList(10018657);
            Infragistics.Win.ValueList valueListBaselineRadioButton = new Controls.CustomControls.CustomValueList(5632719);
            Infragistics.Win.ValueList valueListBaselineCheckBox = new Controls.CustomControls.CustomValueList(5686891);
            Infragistics.Win.ValueListItem valueListItemOK = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItemWarning = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItemCritical = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItemRadio1 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItemRadio2 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItemCheck1 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItemCheck2 = new Infragistics.Win.ValueListItem();

            Infragistics.Win.Appearance appearance38 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueListItem valueListItem7 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance39 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList4 = new Controls.CustomControls.CustomValueList(5686891);
            Infragistics.Win.ValueListItem valueListItem8 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance40 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueListItem valueListItem9 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance41 = new Infragistics.Win.Appearance();
             linearScale1 = new ChartFX.WinForms.Gauge.LinearScale();
            ChartFX.WinForms.Gauge.Marker marker1 = new ChartFX.WinForms.Gauge.Marker();
            ChartFX.WinForms.Gauge.Marker marker2 = new ChartFX.WinForms.Gauge.Marker();
            ChartFX.WinForms.Gauge.Marker marker3 = new ChartFX.WinForms.Gauge.Marker();
            ChartFX.WinForms.Gauge.Section section1 = new ChartFX.WinForms.Gauge.Section();
            ChartFX.WinForms.Gauge.Section section2 = new ChartFX.WinForms.Gauge.Section();
            ChartFX.WinForms.Gauge.Section section3 = new ChartFX.WinForms.Gauge.Section();
            ChartFX.WinForms.Gauge.Section section4 = new ChartFX.WinForms.Gauge.Section();
            ChartFX.WinForms.Gauge.LinearStrip linearStrip1 = new ChartFX.WinForms.Gauge.LinearStrip();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AlertConfigurationDialog));
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("ItemList", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn40 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("MetricDescription");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn41 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ThresholdEntry");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn42 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("MetricID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn43 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("MetricInstance");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn44 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("InstanceType");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn65 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsDefaultThreshold");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn101 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Rank");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn66 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsInfoEnabled");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn67 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsWarningEnabled");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn68 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsCriticalEnabled");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn69 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsMultiInstance");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn70 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Name", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, false);
            Infragistics.Win.Appearance appearance22 = new Infragistics.Win.Appearance();
            Infragistics.Win.ConditionValueAppearance conditionValueAppearance1 = new Infragistics.Win.ConditionValueAppearance(new Infragistics.Win.ICondition[] {
            ((Infragistics.Win.ICondition)(new Infragistics.Win.FormulaCondition("[IsChanged]")))}, new Infragistics.Win.Appearance[] {
            appearance22});
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn71 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Category");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn72 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Description");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn73 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Enabled", -1, 8767172);
            Infragistics.Win.Appearance appearance23 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn74 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ThresholdEnabled");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumnIsBaselineEnabled = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsBaselineEnabled");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumnRangeStartBaseline1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("RangeStartBaselineInfo1");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumnRangeStartBaseline2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("RangeStartBaselineWarning2");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumnRangeStartBaseline3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("RangeStartBaselineCritical3");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumnFlattenedThresholdsBaseline = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FlattenedThresholdsBaseline");

            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn75 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Comments");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn76 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsChanged");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn77 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsMetricInfoChanged");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn78 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsThresholdChanged");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn79 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsThresholdNew");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn80 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsThresholdDeleted");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn81 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("RangeStart1");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn82 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("RangeStart2");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn83 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("RangeStart3");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn84 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FlattenedThresholds");
            Infragistics.Win.UltraWinGrid.RowLayout rowLayout1 = new Infragistics.Win.UltraWinGrid.RowLayout("Another layout");
            Infragistics.Win.UltraWinGrid.RowLayoutColumnInfo rowLayoutColumnInfo1 = new Infragistics.Win.UltraWinGrid.RowLayoutColumnInfo(Infragistics.Win.UltraWinGrid.RowLayoutColumnInfoContext.Column, "MetricDescription", -1, Infragistics.Win.DefaultableBoolean.True);
            Infragistics.Win.UltraWinGrid.RowLayoutColumnInfo rowLayoutColumnInfo2 = new Infragistics.Win.UltraWinGrid.RowLayoutColumnInfo(Infragistics.Win.UltraWinGrid.RowLayoutColumnInfoContext.Column, "ThresholdEntry", -1, Infragistics.Win.DefaultableBoolean.True);
            Infragistics.Win.UltraWinGrid.RowLayoutColumnInfo rowLayoutColumnInfo3 = new Infragistics.Win.UltraWinGrid.RowLayoutColumnInfo(Infragistics.Win.UltraWinGrid.RowLayoutColumnInfoContext.Column, "MetricID", -1, Infragistics.Win.DefaultableBoolean.True);
            Infragistics.Win.UltraWinGrid.RowLayoutColumnInfo rowLayoutColumnInfo4 = new Infragistics.Win.UltraWinGrid.RowLayoutColumnInfo(Infragistics.Win.UltraWinGrid.RowLayoutColumnInfoContext.Column, "MetricInstance", -1, Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinGrid.RowLayoutColumnInfo rowLayoutColumnInfo5 = new Infragistics.Win.UltraWinGrid.RowLayoutColumnInfo(Infragistics.Win.UltraWinGrid.RowLayoutColumnInfoContext.Column, "InstanceType", -1, Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinGrid.RowLayoutColumnInfo rowLayoutColumnInfo6 = new Infragistics.Win.UltraWinGrid.RowLayoutColumnInfo(Infragistics.Win.UltraWinGrid.RowLayoutColumnInfoContext.Column, "IsDefaultThreshold", -1, Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinGrid.RowLayoutColumnInfo rowLayoutColumnInfo7 = new Infragistics.Win.UltraWinGrid.RowLayoutColumnInfo(Infragistics.Win.UltraWinGrid.RowLayoutColumnInfoContext.Column, "IsInfoEnabled", -1, Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinGrid.RowLayoutColumnInfo rowLayoutColumnInfo8 = new Infragistics.Win.UltraWinGrid.RowLayoutColumnInfo(Infragistics.Win.UltraWinGrid.RowLayoutColumnInfoContext.Column, "IsWarningEnabled", -1, Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinGrid.RowLayoutColumnInfo rowLayoutColumnInfo9 = new Infragistics.Win.UltraWinGrid.RowLayoutColumnInfo(Infragistics.Win.UltraWinGrid.RowLayoutColumnInfoContext.Column, "IsCriticalEnabled", -1, Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinGrid.RowLayoutColumnInfo rowLayoutColumnInfo10 = new Infragistics.Win.UltraWinGrid.RowLayoutColumnInfo(Infragistics.Win.UltraWinGrid.RowLayoutColumnInfoContext.Column, "IsMultiInstance", -1, Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinGrid.RowLayoutColumnInfo rowLayoutColumnInfo11 = new Infragistics.Win.UltraWinGrid.RowLayoutColumnInfo(Infragistics.Win.UltraWinGrid.RowLayoutColumnInfoContext.Column, "Name", -1, Infragistics.Win.DefaultableBoolean.False);
            Infragistics.Win.UltraWinGrid.RowLayoutColumnInfo rowLayoutColumnInfo12 = new Infragistics.Win.UltraWinGrid.RowLayoutColumnInfo(Infragistics.Win.UltraWinGrid.RowLayoutColumnInfoContext.Column, "Category", -1, Infragistics.Win.DefaultableBoolean.True);
            Infragistics.Win.UltraWinGrid.RowLayoutColumnInfo rowLayoutColumnInfo13 = new Infragistics.Win.UltraWinGrid.RowLayoutColumnInfo(Infragistics.Win.UltraWinGrid.RowLayoutColumnInfoContext.Column, "Description", -1, Infragistics.Win.DefaultableBoolean.True);
            Infragistics.Win.UltraWinGrid.RowLayoutColumnInfo rowLayoutColumnInfo14 = new Infragistics.Win.UltraWinGrid.RowLayoutColumnInfo(Infragistics.Win.UltraWinGrid.RowLayoutColumnInfoContext.Column, "Enabled", -1, Infragistics.Win.DefaultableBoolean.False);
            Infragistics.Win.UltraWinGrid.RowLayoutColumnInfo rowLayoutColumnInfo15 = new Infragistics.Win.UltraWinGrid.RowLayoutColumnInfo(Infragistics.Win.UltraWinGrid.RowLayoutColumnInfoContext.Column, "ThresholdEnabled", -1, Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinGrid.RowLayoutColumnInfo rowLayoutColumnInfo16 = new Infragistics.Win.UltraWinGrid.RowLayoutColumnInfo(Infragistics.Win.UltraWinGrid.RowLayoutColumnInfoContext.Column, "Comments", -1, Infragistics.Win.DefaultableBoolean.True);
            Infragistics.Win.UltraWinGrid.RowLayoutColumnInfo rowLayoutColumnInfo17 = new Infragistics.Win.UltraWinGrid.RowLayoutColumnInfo(Infragistics.Win.UltraWinGrid.RowLayoutColumnInfoContext.Column, "IsChanged", -1, Infragistics.Win.DefaultableBoolean.True);
            Infragistics.Win.UltraWinGrid.RowLayoutColumnInfo rowLayoutColumnInfo18 = new Infragistics.Win.UltraWinGrid.RowLayoutColumnInfo(Infragistics.Win.UltraWinGrid.RowLayoutColumnInfoContext.Column, "IsMetricInfoChanged", -1, Infragistics.Win.DefaultableBoolean.True);
            Infragistics.Win.UltraWinGrid.RowLayoutColumnInfo rowLayoutColumnInfo19 = new Infragistics.Win.UltraWinGrid.RowLayoutColumnInfo(Infragistics.Win.UltraWinGrid.RowLayoutColumnInfoContext.Column, "IsThresholdChanged", -1, Infragistics.Win.DefaultableBoolean.True);
            Infragistics.Win.UltraWinGrid.RowLayoutColumnInfo rowLayoutColumnInfo20 = new Infragistics.Win.UltraWinGrid.RowLayoutColumnInfo(Infragistics.Win.UltraWinGrid.RowLayoutColumnInfoContext.Column, "IsThresholdNew", -1, Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinGrid.RowLayoutColumnInfo rowLayoutColumnInfo21 = new Infragistics.Win.UltraWinGrid.RowLayoutColumnInfo(Infragistics.Win.UltraWinGrid.RowLayoutColumnInfoContext.Column, "IsThresholdDeleted", -1, Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinGrid.RowLayoutColumnInfo rowLayoutColumnInfo22 = new Infragistics.Win.UltraWinGrid.RowLayoutColumnInfo(Infragistics.Win.UltraWinGrid.RowLayoutColumnInfoContext.Column, "RangeStart1", -1, Infragistics.Win.DefaultableBoolean.True);
            Infragistics.Win.UltraWinGrid.RowLayoutColumnInfo rowLayoutColumnInfo23 = new Infragistics.Win.UltraWinGrid.RowLayoutColumnInfo(Infragistics.Win.UltraWinGrid.RowLayoutColumnInfoContext.Column, "RangeStart2", -1, Infragistics.Win.DefaultableBoolean.True);
            Infragistics.Win.UltraWinGrid.RowLayoutColumnInfo rowLayoutColumnInfo24 = new Infragistics.Win.UltraWinGrid.RowLayoutColumnInfo(Infragistics.Win.UltraWinGrid.RowLayoutColumnInfoContext.Column, "RangeStart3", -1, Infragistics.Win.DefaultableBoolean.True);
            Infragistics.Win.UltraWinGrid.RowLayoutColumnInfo rowLayoutColumnInfo25 = new Infragistics.Win.UltraWinGrid.RowLayoutColumnInfo(Infragistics.Win.UltraWinGrid.RowLayoutColumnInfoContext.Column, "FlattenedThresholds", -1, Infragistics.Win.DefaultableBoolean.True);
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList1 = new Controls.CustomControls.CustomValueList(8767172);
            Infragistics.Win.ValueListItem valueListItem1 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueListItem valueListItem2 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            appearance18 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraTab();
            this.dialogEditor = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraTabPageControl1 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.instanceConfigPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.instanceGridPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.instanceConfigGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.ultraCalcManager1 = new Infragistics.Win.UltraWinCalcManager.UltraCalcManager(this.components);
            this.instanceButtonPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.btnDeleteInstance = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.btnEditInstance = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.btnAddInstance = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.configurationLayoutPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.configurationGridPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.alertConfigurationGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            //10.0 Srishti purohit // For baseline alert
            //Making copy of State grid to give baseline alert functioanlity
            this.baselineAlertConfigurationGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.configBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.informationLabelPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.informationLabelPictureBox = new System.Windows.Forms.PictureBox();
            this.informationLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.configurationGaugePanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.alertConfigurationGauge = new ChartFX.WinForms.Gauge.HorizontalGauge();
            this.advancedPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();

            //10.0 srishti purohit -- for baseline alert modifications
            //this.isBaselineEnabledCheckBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.advancedButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.editButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.panel2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.categoryTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.metricNameDescriptionLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.RankLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.descriptionTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.metricNameTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.RankTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.label2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.ultraTabPageControl2 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.commentsTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.panel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.commentsInformationBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox();
            this.label3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.spacelabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.cancelButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.applyButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.okButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.splitContainer = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomSplitContainer();
            this.alertsGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.checkBox1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox(); //CustomCheckbox 4.12 DarkTheme  Babita Manral 
            this.alertTabControl = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraTabControl(true);
            this.ultraTabSharedControlsPage1 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.applyTemplateButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.createTemplateButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.panel3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.alertRecommendationsPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.alertRecommendationsWarningImage = new System.Windows.Forms.PictureBox();
            this.alertRecommendationsLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.alertRecommendationsBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.configureBaselineButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.AlertConfigurationDialog_Fill_Panel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.existingDatabasesBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.dialogEditor)).BeginInit();
            this.ultraTabPageControl1.SuspendLayout();
            this.instanceConfigPanel.SuspendLayout();
            this.instanceGridPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.instanceConfigGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraCalcManager1)).BeginInit();
            this.instanceButtonPanel.SuspendLayout();
            this.configurationLayoutPanel.SuspendLayout();
            this.configurationGridPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.alertConfigurationGrid)).BeginInit();
            //10.0 Srishti purohit // For baseline alert
            //Making copy of State grid to give baseline alert functioanlity
            ((System.ComponentModel.ISupportInitialize)(this.baselineAlertConfigurationGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.configBindingSource)).BeginInit();
            this.informationLabelPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.informationLabelPictureBox)).BeginInit();
            this.configurationGaugePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.alertConfigurationGauge)).BeginInit();
            this.advancedPanel.SuspendLayout();
            this.panel2.SuspendLayout();
            this.ultraTabPageControl2.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.alertsGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.alertTabControl)).BeginInit();
            this.alertTabControl.SuspendLayout();
            this.panel3.SuspendLayout();
            this.alertRecommendationsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.alertRecommendationsWarningImage)).BeginInit();
            this.AlertConfigurationDialog_Fill_Panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // dialogEditor
            // 
            this.dialogEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            appearance1.BackColor = System.Drawing.Color.Transparent;
            this.dialogEditor.Appearance = appearance1;
            this.dialogEditor.BackColor = System.Drawing.Color.Transparent;
            appearance2.TextHAlignAsString = "Center";
            editorButton1.Appearance = appearance2;
            editorButton1.Text = "...";
            this.dialogEditor.ButtonsRight.Add(editorButton1);
            this.dialogEditor.Location = new System.Drawing.Point(237, 537);
            this.dialogEditor.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dialogEditor.Name = "dialogEditor";
            this.dialogEditor.ReadOnly = true;
            this.dialogEditor.Size = new System.Drawing.Size(73, 24);
            this.dialogEditor.TabIndex = 5;
            this.dialogEditor.Visible = false;
            this.dialogEditor.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.dialogEditor_EditorButtonClick);
            // 
            // ultraTabPageControl1
            // 
            this.ultraTabPageControl1.Controls.Add(this.instanceConfigPanel);
            this.ultraTabPageControl1.Controls.Add(this.configurationLayoutPanel);
            this.ultraTabPageControl1.Controls.Add(this.panel2);
            this.ultraTabPageControl1.Location = new System.Drawing.Point(1, 23);
            this.ultraTabPageControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ultraTabPageControl1.Name = "ultraTabPageControl1";
            this.ultraTabPageControl1.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.ultraTabPageControl1.Size = new System.Drawing.Size(531, 460);
            // 
            // instanceConfigPanel
            // 
            this.instanceConfigPanel.Controls.Add(this.instanceGridPanel);
            this.instanceConfigPanel.Controls.Add(this.instanceButtonPanel);
            this.instanceConfigPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.instanceConfigPanel.Location = new System.Drawing.Point(7, 150);
            this.instanceConfigPanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.instanceConfigPanel.Name = "instanceConfigPanel";
            this.instanceConfigPanel.Size = new System.Drawing.Size(517, 304);
            this.instanceConfigPanel.TabIndex = 20;
            this.instanceConfigPanel.Visible = false;
            // 
            // instanceGridPanel
            // 
            this.instanceGridPanel.Controls.Add(this.instanceConfigGrid);
            this.instanceGridPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.instanceGridPanel.Location = new System.Drawing.Point(0, 0);
            this.instanceGridPanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.instanceGridPanel.Name = "instanceGridPanel";
            this.instanceGridPanel.Size = new System.Drawing.Size(517, 252);
            this.instanceGridPanel.TabIndex = 3;
            // 
            // instanceConfigGrid
            // 
            this.instanceConfigGrid.CalcManager = this.ultraCalcManager1;
            appearance20.BackColor = System.Drawing.SystemColors.Window;
            appearance20.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.instanceConfigGrid.DisplayLayout.Appearance = appearance20;
            this.instanceConfigGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            ultraGridColumn45.Header.VisiblePosition = 0;
            ultraGridColumn45.Hidden = true;
            ultraGridColumn45.Width = 44;
            ultraGridColumn46.Header.Caption = "";
            ultraGridColumn46.Header.VisiblePosition = 1;
            ultraGridColumn46.MaxWidth = 24;
            ultraGridColumn46.MinLength = 24;
            ultraGridColumn46.MinWidth = 24;
            ultraGridColumn46.Width = 24;
            ultraGridColumn47.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            ultraGridColumn47.Header.Caption = "Instance";
            ultraGridColumn47.Header.VisiblePosition = 2;
            ultraGridColumn47.Width = 343;
            ultraGridColumn48.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            appearance21.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Information32x32;
            appearance21.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance21.ImageVAlign = Infragistics.Win.VAlign.Middle;
            ultraGridColumn48.Header.Appearance = appearance21;
            ultraGridColumn48.Header.Caption = "";
            ultraGridColumn48.Header.VisiblePosition = 3;
            ultraGridColumn48.MaxWidth = 50;
            ultraGridColumn48.MinWidth = 45;
            ultraGridColumn48.Width = 50;
            ultraGridColumn49.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            appearance42.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Warning32x32;
            appearance42.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance42.ImageVAlign = Infragistics.Win.VAlign.Middle;
            ultraGridColumn49.Header.Appearance = appearance42;
            ultraGridColumn49.Header.Caption = "";
            ultraGridColumn49.Header.VisiblePosition = 4;
            ultraGridColumn49.MaxWidth = 50;
            ultraGridColumn49.MinWidth = 45;
            ultraGridColumn49.Width = 50;
            ultraGridColumn50.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            appearance48.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Critical32x32;
            appearance48.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance48.ImageVAlign = Infragistics.Win.VAlign.Middle;
            ultraGridColumn50.Header.Appearance = appearance48;
            ultraGridColumn50.Header.Caption = "";
            ultraGridColumn50.Header.VisiblePosition = 5;
            ultraGridColumn50.MaxWidth = 50;
            ultraGridColumn50.MinWidth = 45;
            ultraGridColumn50.Width = 50;
            ultraGridColumn51.Header.VisiblePosition = 6;
            ultraGridColumn51.Hidden = true;
            ultraGridColumn51.Width = 15;
            ultraGridColumn52.Header.VisiblePosition = 7;
            ultraGridColumn52.Hidden = true;
            ultraGridColumn52.Width = 31;
            ultraGridColumn53.Header.VisiblePosition = 8;
            ultraGridColumn53.Hidden = true;
            ultraGridColumn53.Width = 31;
            ultraGridColumn54.Header.VisiblePosition = 9;
            ultraGridColumn54.Hidden = true;
            ultraGridColumn54.Width = 34;
            ultraGridColumn55.Header.VisiblePosition = 10;
            ultraGridColumn55.Hidden = true;
            ultraGridColumn55.Width = 27;
            ultraGridColumn56.Header.VisiblePosition = 11;
            ultraGridColumn56.Hidden = true;
            ultraGridColumn56.Width = 33;
            ultraGridColumn57.Header.VisiblePosition = 12;
            ultraGridColumn57.Hidden = true;
            ultraGridColumn57.Width = 31;
            ultraGridColumn58.Header.VisiblePosition = 13;
            ultraGridColumn58.Hidden = true;
            ultraGridColumn58.Width = 33;
            ultraGridColumn59.Header.VisiblePosition = 14;
            ultraGridColumn59.Hidden = true;
            ultraGridColumn59.Width = 37;
            ultraGridColumn60.Header.VisiblePosition = 15;
            ultraGridColumn60.Hidden = true;
            ultraGridColumn60.Width = 36;
            ultraGridColumn61.Header.VisiblePosition = 16;
            ultraGridColumn61.Hidden = true;
            ultraGridColumn61.Width = 38;
            ultraGridColumn62.Header.VisiblePosition = 17;
            ultraGridColumn62.Hidden = true;
            ultraGridColumn62.Width = 55;
            ultraGridColumn63.Header.VisiblePosition = 18;
            ultraGridColumn63.Hidden = true;
            ultraGridColumn63.Width = 49;
            ultraGridColumn64.Header.VisiblePosition = 19;
            ultraGridColumn64.Hidden = true;
            ultraGridColumn64.Width = 67;
            ultraGridColumn88.Header.VisiblePosition = 20;
            ultraGridColumn88.Hidden = true;
            ultraGridColumn88.Width = 68;
            //SQLdm 8.6 -(Ankit Srivastava) : Preferred Node Feature -- Setting properties for the newly added column
            ultraGridColumn98.Header.VisiblePosition = 26;
            ultraGridColumn98.Hidden = true;
            ultraGridColumn98.Header.Caption = "State"; //SQLdm 8.6 -(Ankit Srivastava) : Preferred Node Feature -- Fixed defect DE43150
            ultraGridColumn98.MinWidth = 80;
            ultraGridColumn98.MaxWidth = 120;

            ultraGridColumn100.Header.VisiblePosition = 21;
            ultraGridColumn100.Hidden = true;
            ultraGridColumn100.Width = 87;
            appearance49.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Information32x32;
            appearance49.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance49.ImageVAlign = Infragistics.Win.VAlign.Middle;
            ultraGridColumn23.Header.Appearance = appearance49;
            ultraGridColumn23.Header.Caption = "";
            ultraGridColumn23.Header.VisiblePosition = 22;
            ultraGridColumn23.Hidden = true;
            ultraGridColumn23.MaxWidth = 50;
            ultraGridColumn23.MinWidth = 45;
            ultraGridColumn23.Width = 50;
            appearance50.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Warning32x32;
            appearance50.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance50.ImageVAlign = Infragistics.Win.VAlign.Middle;
            ultraGridColumn24.Header.Appearance = appearance50;
            ultraGridColumn24.Header.Caption = "";
            ultraGridColumn24.Header.VisiblePosition = 24;
            ultraGridColumn24.Hidden = true;
            ultraGridColumn24.MaxWidth = 50;
            ultraGridColumn24.MinWidth = 45;
            ultraGridColumn24.Width = 50;
            appearance51.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Critical32x32;
            appearance51.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance51.ImageVAlign = Infragistics.Win.VAlign.Middle;
            ultraGridColumn25.Header.Appearance = appearance51;
            ultraGridColumn25.Header.Caption = "";
            ultraGridColumn25.Header.VisiblePosition = 25;
            ultraGridColumn25.Hidden = true;
            ultraGridColumn25.MaxWidth = 50;
            ultraGridColumn25.MinWidth = 45;
            ultraGridColumn25.Width = 50;
            ultraGridColumn87.DataType = typeof(bool);
            ultraGridColumn87.DefaultCellValue = true;
            ultraGridColumn87.Header.VisiblePosition = 23;
            ultraGridColumn87.Hidden = true;
            ultraGridColumn87.Width = 53;
            ultraGridColumnIsBaselineEnabled.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            //ultraGridColumnIsBaselineEnabled.Header.VisiblePosition = 22;
            ultraGridColumnIsBaselineEnabled.Hidden = true;
            ultraGridColumnRangeStartBaseline1.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumnRangeStartBaseline1.Hidden = true;
            ultraGridColumnRangeStartBaseline2.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumnRangeStartBaseline2.Hidden = true;
            ultraGridColumnRangeStartBaseline3.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumnRangeStartBaseline3.Hidden = true;
            ultraGridColumnFlattenedThresholdsBaseline.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumnFlattenedThresholdsBaseline.Hidden = true;
            ultraGridBand2.Columns.AddRange(new object[] {
            ultraGridColumn45,
            ultraGridColumn46,
            ultraGridColumn47,
            ultraGridColumn48,
            ultraGridColumn49,
            ultraGridColumn50,
            ultraGridColumn51,
            ultraGridColumn52,
            ultraGridColumn53,
            ultraGridColumn54,
            ultraGridColumn55,
            ultraGridColumn56,
            ultraGridColumn57,
            ultraGridColumn58,
            ultraGridColumn59,
            ultraGridColumn60,
            ultraGridColumn61,
            ultraGridColumn62,
            ultraGridColumn63,
            ultraGridColumn64,
            ultraGridColumn88,
            ultraGridColumn100,
            ultraGridColumn23,
            ultraGridColumn24,
            ultraGridColumn25,
            ultraGridColumn87,
            ultraGridColumn98,
            ultraGridColumnIsBaselineEnabled,
            ultraGridColumnRangeStartBaseline1,
            ultraGridColumnRangeStartBaseline2,
            ultraGridColumnRangeStartBaseline3,
            ultraGridColumnFlattenedThresholdsBaseline}); //SQLdm 8.6 -(Ankit Srivastava) : Preferred Node Feature -- Added new column ConfigAlertValue to the gridband
            ultraGridBand2.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.None;
            ultraGridBand2.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.None;
            ultraGridBand2.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            ultraGridBand2.Override.RowEditTemplateUIType = Infragistics.Win.UltraWinGrid.RowEditTemplateUIType.None;
            ultraGridBand2.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
            ultraGridBand2.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            ultraGridBand2.Override.SelectTypeGroupByRow = Infragistics.Win.UltraWinGrid.SelectType.None;
            ultraGridBand2.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            this.instanceConfigGrid.DisplayLayout.BandsSerializer.Add(ultraGridBand2);
            this.instanceConfigGrid.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.instanceConfigGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            this.instanceConfigGrid.DisplayLayout.MaxRowScrollRegions = 1;
            this.instanceConfigGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.None;
            this.instanceConfigGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.None;
            this.instanceConfigGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.instanceConfigGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.instanceConfigGrid.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.instanceConfigGrid.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.instanceConfigGrid.DisplayLayout.Override.SelectTypeGroupByRow = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.instanceConfigGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            this.instanceConfigGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.instanceConfigGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.instanceConfigGrid.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.instanceConfigGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.instanceConfigGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.instanceConfigGrid.Location = new System.Drawing.Point(0, 0);
            this.instanceConfigGrid.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.instanceConfigGrid.Name = "instanceConfigGrid";
            this.instanceConfigGrid.Size = new System.Drawing.Size(517, 252);
            this.instanceConfigGrid.TabIndex = 0;
            this.instanceConfigGrid.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(this.instanceConfigGrid_InitializeRow);
            this.instanceConfigGrid.AfterSelectChange += new Infragistics.Win.UltraWinGrid.AfterSelectChangeEventHandler(this.instanceConfigGrid_AfterSelectChange);
            this.instanceConfigGrid.ClickCell += new Infragistics.Win.UltraWinGrid.ClickCellEventHandler(this.instanceConfigGrid_ClickCell);
            this.instanceConfigGrid.DoubleClickCell += new Infragistics.Win.UltraWinGrid.DoubleClickCellEventHandler(this.instanceConfigGrid_DoubleClickCell);
            // 
            // ultraCalcManager1
            // 
            this.ultraCalcManager1.ContainingControl = this;
            // 
            // instanceButtonPanel
            // 
            this.instanceButtonPanel.AutoSize = true;
            this.instanceButtonPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.instanceButtonPanel.Controls.Add(this.btnDeleteInstance);
            this.instanceButtonPanel.Controls.Add(this.btnEditInstance);
            this.instanceButtonPanel.Controls.Add(this.btnAddInstance);
            this.instanceButtonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.instanceButtonPanel.Location = new System.Drawing.Point(0, 252);
            this.instanceButtonPanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.instanceButtonPanel.MinimumSize = new System.Drawing.Size(0, 48);
            this.instanceButtonPanel.Name = "instanceButtonPanel";
            this.instanceButtonPanel.Padding = new System.Windows.Forms.Padding(11, 10, 11, 10);
            this.instanceButtonPanel.Size = new System.Drawing.Size(517, 52);
            this.instanceButtonPanel.TabIndex = 1;
            // 
            // btnDeleteInstance
            // 
            this.btnDeleteInstance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteInstance.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnDeleteInstance.Location = new System.Drawing.Point(421, 10);
            this.btnDeleteInstance.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnDeleteInstance.Name = "btnDeleteInstance";
            this.btnDeleteInstance.Size = new System.Drawing.Size(85, 28);
            this.btnDeleteInstance.TabIndex = 4;
            this.btnDeleteInstance.Text = "Delete";
            this.btnDeleteInstance.UseVisualStyleBackColor = false;
            this.btnDeleteInstance.Click += new System.EventHandler(this.btnDeleteInstance_Click);
            // 
            // btnEditInstance
            // 
            this.btnEditInstance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEditInstance.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnEditInstance.Location = new System.Drawing.Point(98, 10);
            this.btnEditInstance.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.btnEditInstance.Name = "btnEditInstance";
            this.btnEditInstance.Size = new System.Drawing.Size(85, 28);
            this.btnEditInstance.TabIndex = 3;
            this.btnEditInstance.Text = "Edit";
            this.btnEditInstance.UseVisualStyleBackColor = false;
            this.btnEditInstance.Click += new System.EventHandler(this.btnEditInstance_Click);
            // 
            // btnAddInstance
            // 
            this.btnAddInstance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddInstance.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnAddInstance.Location = new System.Drawing.Point(11, 10);
            this.btnAddInstance.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.btnAddInstance.Name = "btnAddInstance";
            this.btnAddInstance.Size = new System.Drawing.Size(85, 28);
            this.btnAddInstance.TabIndex = 2;
            this.btnAddInstance.Text = "Add";
            this.btnAddInstance.UseVisualStyleBackColor = false;
            this.btnAddInstance.Click += new System.EventHandler(this.btnAddInstance_Click);
            // 
            // configurationLayoutPanel
            // 
            this.configurationLayoutPanel.Controls.Add(this.configurationGridPanel);
            this.configurationLayoutPanel.Controls.Add(this.configurationGaugePanel);
            this.configurationLayoutPanel.Controls.Add(this.advancedPanel);
            this.configurationLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.configurationLayoutPanel.Location = new System.Drawing.Point(7, 150);
            this.configurationLayoutPanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.configurationLayoutPanel.Name = "configurationLayoutPanel";
            this.configurationLayoutPanel.Size = new System.Drawing.Size(517, 304);
            this.configurationLayoutPanel.TabIndex = 17;
            // 
            // configurationGridPanel
            // 
            this.configurationGridPanel.AutoSize = true;
            this.configurationGridPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.configurationGridPanel.Controls.Add(this.alertConfigurationGrid);
            //10.0 Srishti purohit // For baseline alert
            //Making copy of State grid to give baseline alert functioanlity
            this.configurationGridPanel.Controls.Add(this.baselineAlertConfigurationGrid);
            this.configurationGridPanel.Controls.Add(this.informationLabelPanel);
            this.configurationGridPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.configurationGridPanel.Location = new System.Drawing.Point(0, 105);
            this.configurationGridPanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.configurationGridPanel.Name = "configurationGridPanel";
            this.configurationGridPanel.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.configurationGridPanel.Size = new System.Drawing.Size(517, 151);
            this.configurationGridPanel.TabIndex = 1;
            // 
            // alertConfigurationGrid
            // 
            this.alertConfigurationGrid.CalcManager = this.ultraCalcManager1;
            this.alertConfigurationGrid.DataMember = "FlattenedThresholds";
            this.alertConfigurationGrid.DataSource = this.configBindingSource;
            appearance25.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            appearance25.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.alertConfigurationGrid.DisplayLayout.Appearance = appearance25;
            this.alertConfigurationGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            ultraGridColumn85.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn85.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            ultraGridColumn85.Header.Caption = "State";
            ultraGridColumn85.Header.VisiblePosition = 2;
            ultraGridColumn85.SortIndicator = Infragistics.Win.UltraWinGrid.SortIndicator.Disabled;
            ultraGridColumn85.Width = 82;
            ultraGridColumn86.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn86.Header.VisiblePosition = 3;
            ultraGridColumn86.Hidden = true;
            ultraGridColumn89.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn89.Header.VisiblePosition = 9;
            ultraGridColumn89.Hidden = true;
            ultraGridColumn90.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn90.Header.VisiblePosition = 10;
            ultraGridColumn90.Hidden = true;
            ultraGridColumn91.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn91.Header.VisiblePosition = 0;
            ultraGridColumn91.Hidden = true;
            ultraGridColumn92.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn92.Header.VisiblePosition = 5;
            ultraGridColumn92.Hidden = true;
            ultraGridColumn93.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn93.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            appearance24.TextHAlignAsString = "Center";
            ultraGridColumn93.CellAppearance = appearance24;
            appearance26.ImageHAlign = Infragistics.Win.HAlign.Center;
            ultraGridColumn93.Header.Appearance = appearance26;
            ultraGridColumn93.Header.Caption = "";
            ultraGridColumn93.Header.ToolTipText = "Enabled";
            ultraGridColumn93.Header.VisiblePosition = 1;
            ultraGridColumn93.MaxWidth = 24;
            ultraGridColumn93.MinWidth = 24;
            ultraGridColumn93.SortIndicator = Infragistics.Win.UltraWinGrid.SortIndicator.Disabled;
            ultraGridColumn93.Width = 24;
            ultraGridColumn94.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn94.Format = "F0";
            ultraGridColumn94.Header.Caption = "Start";
            ultraGridColumn94.Header.VisiblePosition = 7;
            ultraGridColumn94.InvalidValueBehavior = Infragistics.Win.UltraWinGrid.InvalidValueBehavior.RevertValueAndRetainFocus;
            ultraGridColumn94.PromptChar = ' ';
            ultraGridColumn94.SortIndicator = Infragistics.Win.UltraWinGrid.SortIndicator.Disabled;
            ultraGridColumn94.Width = 101;
            ultraGridColumn95.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn95.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            ultraGridColumn95.Format = "F0";
            ultraGridColumn95.Header.Caption = "End";
            ultraGridColumn95.Header.VisiblePosition = 8;
            ultraGridColumn95.SortIndicator = Infragistics.Win.UltraWinGrid.SortIndicator.Disabled;
            ultraGridColumn96.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn96.EditorComponent = this.dialogEditor;
            ultraGridColumn96.Header.VisiblePosition = 4;
            ultraGridColumn96.Hidden = true;
            ultraGridColumn96.SortIndicator = Infragistics.Win.UltraWinGrid.SortIndicator.Disabled;
            ultraGridColumn97.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn97.Header.VisiblePosition = 6;
            ultraGridColumn97.Hidden = true;
            ultraGridColumnIsBaselineTypeAlert.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumnIsBaselineTypeAlert.Header.VisiblePosition = 6;
            ultraGridColumnIsBaselineTypeAlert.Hidden = true;
            ultraGridBand3.Columns.AddRange(new object[] {
            ultraGridColumn85,
            ultraGridColumn86,
            ultraGridColumn89,
            ultraGridColumn90,
            ultraGridColumn91,
            ultraGridColumn92,
            ultraGridColumn93,
            ultraGridColumn94,
            ultraGridColumn95,
            ultraGridColumn96,
            ultraGridColumn97,
            ultraGridColumnIsBaselineTypeAlert});
            this.alertConfigurationGrid.DisplayLayout.BandsSerializer.Add(ultraGridBand3);
            this.alertConfigurationGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance28.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(239)))), ((int)(((byte)(255)))));
            appearance28.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance28.BackGradientStyle = Infragistics.Win.GradientStyle.None;
            appearance28.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(147)))), ((int)(((byte)(207)))));
            this.alertConfigurationGrid.DisplayLayout.GroupByBox.Appearance = appearance28;
            appearance29.ForeColor = System.Drawing.SystemColors.GrayText;
            this.alertConfigurationGrid.DisplayLayout.GroupByBox.BandLabelAppearance = appearance29;
            this.alertConfigurationGrid.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.alertConfigurationGrid.DisplayLayout.GroupByBox.Hidden = true;
            appearance30.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance30.BackColor2 = System.Drawing.SystemColors.Control;
            appearance30.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance30.ForeColor = System.Drawing.SystemColors.GrayText;
            this.alertConfigurationGrid.DisplayLayout.GroupByBox.PromptAppearance = appearance30;
            this.alertConfigurationGrid.DisplayLayout.MaxColScrollRegions = 1;
            this.alertConfigurationGrid.DisplayLayout.MaxRowScrollRegions = 1;
            this.alertConfigurationGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.alertConfigurationGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.alertConfigurationGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
            this.alertConfigurationGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.None;
            this.alertConfigurationGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.None;
            appearance31.BackColor = System.Drawing.SystemColors.Window;
            this.alertConfigurationGrid.DisplayLayout.Override.CardAreaAppearance = appearance31;
            appearance32.BorderColor = System.Drawing.Color.Silver;
            appearance32.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.alertConfigurationGrid.DisplayLayout.Override.CellAppearance = appearance32;
            appearance33.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance33.ImageVAlign = Infragistics.Win.VAlign.Middle;
            this.alertConfigurationGrid.DisplayLayout.Override.CellButtonAppearance = appearance33;
            this.alertConfigurationGrid.DisplayLayout.Override.CellPadding = 0;
            this.alertConfigurationGrid.DisplayLayout.Override.FixedHeaderIndicator = Infragistics.Win.UltraWinGrid.FixedHeaderIndicator.None;
            this.alertConfigurationGrid.DisplayLayout.Override.GroupByColumnsHidden = Infragistics.Win.DefaultableBoolean.False;
            appearance34.BackColor = System.Drawing.SystemColors.Control;
            appearance34.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance34.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance34.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance34.BorderColor = System.Drawing.SystemColors.Window;
            this.alertConfigurationGrid.DisplayLayout.Override.GroupByRowAppearance = appearance34;
            this.alertConfigurationGrid.DisplayLayout.Override.GroupByRowInitialExpansionState = Infragistics.Win.UltraWinGrid.GroupByRowInitialExpansionState.Expanded;
            appearance35.TextHAlignAsString = "Left";
            this.alertConfigurationGrid.DisplayLayout.Override.HeaderAppearance = appearance35;
            this.alertConfigurationGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortSingle;
            appearance36.BackColor = System.Drawing.SystemColors.Window;
            appearance36.BorderColor = System.Drawing.Color.Silver;
            this.alertConfigurationGrid.DisplayLayout.Override.RowAppearance = appearance36;
            this.alertConfigurationGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.alertConfigurationGrid.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.Single;
            this.alertConfigurationGrid.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.alertConfigurationGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            appearance37.BackColor = System.Drawing.SystemColors.ControlLight;
            this.alertConfigurationGrid.DisplayLayout.Override.TemplateAddRowAppearance = appearance37;
            this.alertConfigurationGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.alertConfigurationGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.alertConfigurationGrid.DisplayLayout.UseFixedHeaders = true;
            valueList2.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.DisplayTextAndPicture;
            valueList2.Key = "ThresholdTypeItems";
            valueList2.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            valueListItem3.DataValue = 0;
            valueListItem3.DisplayText = "OK";
            valueListItem4.DataValue = 1;
            valueListItem4.DisplayText = "Warning";
            valueListItem5.DataValue = 2;
            valueListItem5.DisplayText = "Critical";
            valueList2.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem3,
            valueListItem4,
            valueListItem5});
            valueList3.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.Picture;
            valueList3.Key = "RadioButtons";
            valueList3.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            valueList3.ScaleItemImage = Infragistics.Win.ScaleImage.Never;
            appearance38.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RibbonRadioButtonUnchecked;
            appearance38.ImageHAlign = Infragistics.Win.HAlign.Center;
            valueListItem6.Appearance = appearance38;
            valueListItem6.DataValue = false;
            appearance39.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RibbonRadioButtonChecked;
            appearance39.ImageHAlign = Infragistics.Win.HAlign.Center;
            valueListItem7.Appearance = appearance39;
            valueListItem7.DataValue = true;
            valueList3.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem6,
            valueListItem7});
            valueList4.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.Picture;
            valueList4.Key = "CheckBoxes";
            valueList4.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            valueList4.ScaleItemImage = Infragistics.Win.ScaleImage.Never;
            appearance40.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RibbonCheckboxUnchecked;
            appearance40.ImageHAlign = Infragistics.Win.HAlign.Center;
            valueListItem8.Appearance = appearance40;
            valueListItem8.DataValue = false;
            appearance41.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RibbonCheckboxChecked;
            appearance41.ImageHAlign = Infragistics.Win.HAlign.Center;
            valueListItem9.Appearance = appearance41;
            valueListItem9.DataValue = true;
            valueList4.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem8,
            valueListItem9});
            this.alertConfigurationGrid.DisplayLayout.ValueLists.AddRange(new Infragistics.Win.ValueList[] {
            valueList2,
            valueList3,
            valueList4});
            this.alertConfigurationGrid.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.alertConfigurationGrid.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.alertConfigurationGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.alertConfigurationGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.alertConfigurationGrid.Location = new System.Drawing.Point(0, 45);
            this.alertConfigurationGrid.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.alertConfigurationGrid.Name = "alertConfigurationGrid";
            this.alertConfigurationGrid.Size = new System.Drawing.Size(517, 106);
            this.alertConfigurationGrid.TabIndex = 5;
            this.alertConfigurationGrid.UpdateMode = Infragistics.Win.UltraWinGrid.UpdateMode.OnCellChangeOrLostFocus;
            this.alertConfigurationGrid.AfterCellUpdate += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.alertConfigurationGrid_AfterCellUpdate);
            this.alertConfigurationGrid.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(this.alertConfigurationGrid_InitializeRow);
            this.alertConfigurationGrid.BeforeCellActivate += new Infragistics.Win.UltraWinGrid.CancelableCellEventHandler(this.alertConfigurationGrid_BeforeCellActivate);
            this.alertConfigurationGrid.DoubleClickCell += new Infragistics.Win.UltraWinGrid.DoubleClickCellEventHandler(this.alertConfigurationGrid_DoubleClickCell);
            this.alertConfigurationGrid.MouseClick += new System.Windows.Forms.MouseEventHandler(this.alertsGrid_MouseClick);
            // 
            // baselineAlertConfigurationGrid
            // 
            this.baselineAlertConfigurationGrid.CalcManager = this.ultraCalcManager1;
            this.baselineAlertConfigurationGrid.DataMember = "FlattenedThresholdsBaseline";
            this.baselineAlertConfigurationGrid.DataSource = this.configBindingSource;
            appearance25.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            appearance25.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.baselineAlertConfigurationGrid.DisplayLayout.Appearance = appearance25;
            this.baselineAlertConfigurationGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            ultraGridColumn85.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn85.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            ultraGridColumn85.Header.Caption = "State";
            ultraGridColumn85.Header.VisiblePosition = 2;
            ultraGridColumn85.SortIndicator = Infragistics.Win.UltraWinGrid.SortIndicator.Disabled;
            ultraGridColumn85.Width = 82;
            ultraGridColumn86.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn86.Header.VisiblePosition = 3;
            ultraGridColumn86.Hidden = true;
            ultraGridColumn89.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn89.Header.VisiblePosition = 9;
            ultraGridColumn89.Hidden = true;
            ultraGridColumn90.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn90.Header.VisiblePosition = 10;
            ultraGridColumn90.Hidden = true;
            ultraGridColumn91.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn91.Header.VisiblePosition = 0;
            ultraGridColumn91.Hidden = true;
            ultraGridColumn92.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn92.Header.VisiblePosition = 5;
            ultraGridColumn92.Hidden = true;
            ultraGridColumn93.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn93.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            appearance24.TextHAlignAsString = "Center";
            ultraGridColumn93.CellAppearance = appearance24;
            appearance26.ImageHAlign = Infragistics.Win.HAlign.Center;
            ultraGridColumn93.Header.Appearance = appearance26;
            ultraGridColumn93.Header.Caption = "";
            ultraGridColumn93.Header.ToolTipText = "Enabled";
            ultraGridColumn93.Header.VisiblePosition = 1;
            ultraGridColumn93.MaxWidth = 24;
            ultraGridColumn93.MinWidth = 24;
            ultraGridColumn93.SortIndicator = Infragistics.Win.UltraWinGrid.SortIndicator.Disabled;
            ultraGridColumn93.Width = 24;
            ultraGridColumn94.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn94.Format = "F0";
            ultraGridColumn94.Header.Caption = "Start";
            ultraGridColumn94.Header.VisiblePosition = 7;
            ultraGridColumn94.InvalidValueBehavior = Infragistics.Win.UltraWinGrid.InvalidValueBehavior.RevertValueAndRetainFocus;
            ultraGridColumn94.PromptChar = ' ';
            ultraGridColumn94.SortIndicator = Infragistics.Win.UltraWinGrid.SortIndicator.Disabled;
            ultraGridColumn94.Width = 101;
            ultraGridColumn95.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn95.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            ultraGridColumn95.Format = "F0";
            ultraGridColumn95.Header.Caption = "End";
            ultraGridColumn95.Header.VisiblePosition = 8;
            ultraGridColumn95.SortIndicator = Infragistics.Win.UltraWinGrid.SortIndicator.Disabled;
            ultraGridColumn96.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn96.EditorComponent = this.dialogEditor;
            ultraGridColumn96.Header.VisiblePosition = 4;
            ultraGridColumn96.Hidden = true;
            ultraGridColumn96.SortIndicator = Infragistics.Win.UltraWinGrid.SortIndicator.Disabled;
            ultraGridColumn97.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn97.Header.VisiblePosition = 6;
            ultraGridColumn97.Hidden = true;
            ultraGridColumnIsBaselineTypeAlert.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumnIsBaselineTypeAlert.Header.VisiblePosition = 6;
            ultraGridColumnIsBaselineTypeAlert.Hidden = true;
            ultraGridBandForBaselineAlertGrid.Columns.AddRange(new object[] {
            ultraGridColumn85,
            ultraGridColumn86,
            ultraGridColumn89,
            ultraGridColumn90,
            ultraGridColumn91,
            ultraGridColumn92,
            ultraGridColumn93,
            ultraGridColumn94,
            ultraGridColumn95,
            ultraGridColumn96,
            ultraGridColumn97,
            ultraGridColumnIsBaselineTypeAlert});
            this.baselineAlertConfigurationGrid.DisplayLayout.BandsSerializer.Add(ultraGridBandForBaselineAlertGrid);
            this.baselineAlertConfigurationGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance28.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(239)))), ((int)(((byte)(255)))));
            appearance28.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance28.BackGradientStyle = Infragistics.Win.GradientStyle.None;
            appearance28.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(147)))), ((int)(((byte)(207)))));
            this.baselineAlertConfigurationGrid.DisplayLayout.GroupByBox.Appearance = appearance28;
            appearance29.ForeColor = System.Drawing.SystemColors.GrayText;
            this.baselineAlertConfigurationGrid.DisplayLayout.GroupByBox.BandLabelAppearance = appearance29;
            this.baselineAlertConfigurationGrid.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.baselineAlertConfigurationGrid.DisplayLayout.GroupByBox.Hidden = true;
            appearance30.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance30.BackColor2 = System.Drawing.SystemColors.Control;
            appearance30.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance30.ForeColor = System.Drawing.SystemColors.GrayText;
            this.baselineAlertConfigurationGrid.DisplayLayout.GroupByBox.PromptAppearance = appearance30;
            this.baselineAlertConfigurationGrid.DisplayLayout.MaxColScrollRegions = 1;
            this.baselineAlertConfigurationGrid.DisplayLayout.MaxRowScrollRegions = 1;
            this.baselineAlertConfigurationGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.baselineAlertConfigurationGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.baselineAlertConfigurationGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
            this.baselineAlertConfigurationGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.None;
            this.baselineAlertConfigurationGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.None;
            appearance31.BackColor = System.Drawing.SystemColors.Window;
            this.baselineAlertConfigurationGrid.DisplayLayout.Override.CardAreaAppearance = appearance31;
            appearance32.BorderColor = System.Drawing.Color.Silver;
            appearance32.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.baselineAlertConfigurationGrid.DisplayLayout.Override.CellAppearance = appearance32;
            appearance33.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance33.ImageVAlign = Infragistics.Win.VAlign.Middle;
            this.baselineAlertConfigurationGrid.DisplayLayout.Override.CellButtonAppearance = appearance33;
            this.baselineAlertConfigurationGrid.DisplayLayout.Override.CellPadding = 0;
            this.baselineAlertConfigurationGrid.DisplayLayout.Override.FixedHeaderIndicator = Infragistics.Win.UltraWinGrid.FixedHeaderIndicator.None;
            this.baselineAlertConfigurationGrid.DisplayLayout.Override.GroupByColumnsHidden = Infragistics.Win.DefaultableBoolean.False;
            appearance34.BackColor = System.Drawing.SystemColors.Control;
            appearance34.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance34.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance34.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance34.BorderColor = System.Drawing.SystemColors.Window;
            this.baselineAlertConfigurationGrid.DisplayLayout.Override.GroupByRowAppearance = appearance34;
            this.baselineAlertConfigurationGrid.DisplayLayout.Override.GroupByRowInitialExpansionState = Infragistics.Win.UltraWinGrid.GroupByRowInitialExpansionState.Expanded;
            appearance35.TextHAlignAsString = "Left";
            this.baselineAlertConfigurationGrid.DisplayLayout.Override.HeaderAppearance = appearance35;
            this.baselineAlertConfigurationGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortSingle;
            appearance36.BackColor = System.Drawing.SystemColors.Window;
            appearance36.BorderColor = System.Drawing.Color.Silver;
            this.baselineAlertConfigurationGrid.DisplayLayout.Override.RowAppearance = appearance36;
            this.baselineAlertConfigurationGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.baselineAlertConfigurationGrid.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.Single;
            this.baselineAlertConfigurationGrid.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.baselineAlertConfigurationGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            appearance37.BackColor = System.Drawing.SystemColors.ControlLight;
            this.baselineAlertConfigurationGrid.DisplayLayout.Override.TemplateAddRowAppearance = appearance37;
            this.baselineAlertConfigurationGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.baselineAlertConfigurationGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.baselineAlertConfigurationGrid.DisplayLayout.UseFixedHeaders = true;
            valueListBaselineAlertState.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.DisplayTextAndPicture;
            valueListBaselineAlertState.Key = "ThresholdTypeItems";
            valueListBaselineAlertState.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            valueListItemOK.DataValue = 0;
            valueListItemOK.DisplayText = "OK";
            valueListItemWarning.DataValue = 1;
            valueListItemWarning.DisplayText = "Warning";
            valueListItemCritical.DataValue = 2;
            valueListItemCritical.DisplayText = "Critical";
            valueListBaselineAlertState.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItemOK,
            valueListItemWarning,
            valueListItemCritical});
            valueListBaselineRadioButton.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.Picture;
            valueListBaselineRadioButton.Key = "RadioButtons";
            valueListBaselineRadioButton.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            valueListBaselineRadioButton.ScaleItemImage = Infragistics.Win.ScaleImage.Never;
            appearance38.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RibbonRadioButtonUnchecked;
            appearance38.ImageHAlign = Infragistics.Win.HAlign.Center;
            valueListItemRadio1.Appearance = appearance38;
            valueListItemRadio1.DataValue = false;
            appearance39.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RibbonRadioButtonChecked;
            appearance39.ImageHAlign = Infragistics.Win.HAlign.Center;
            valueListItemRadio2.Appearance = appearance39;
            valueListItemRadio2.DataValue = true;
            valueListBaselineRadioButton.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItemRadio1,
            valueListItemRadio2});
            valueListBaselineCheckBox.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.Picture;
            valueListBaselineCheckBox.Key = "CheckBoxes";
            valueListBaselineCheckBox.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            valueListBaselineCheckBox.ScaleItemImage = Infragistics.Win.ScaleImage.Never;
            appearance40.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RibbonCheckboxUnchecked;
            appearance40.ImageHAlign = Infragistics.Win.HAlign.Center;
            valueListItemCheck1.Appearance = appearance40;
            valueListItemCheck1.DataValue = false;
            appearance41.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RibbonCheckboxChecked;
            appearance41.ImageHAlign = Infragistics.Win.HAlign.Center;
            valueListItemCheck2.Appearance = appearance41;
            valueListItemCheck2.DataValue = true;
            valueListBaselineCheckBox.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItemCheck1,
            valueListItemCheck2});
            this.baselineAlertConfigurationGrid.DisplayLayout.ValueLists.AddRange(new Infragistics.Win.ValueList[] {
            valueListBaselineAlertState,
            valueListBaselineRadioButton,
            valueListBaselineCheckBox});
            this.baselineAlertConfigurationGrid.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.baselineAlertConfigurationGrid.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.baselineAlertConfigurationGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.baselineAlertConfigurationGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.baselineAlertConfigurationGrid.Location = new System.Drawing.Point(0, 45);
            this.baselineAlertConfigurationGrid.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.baselineAlertConfigurationGrid.Name = "baselineAlertConfigurationGrid";
            this.baselineAlertConfigurationGrid.Size = new System.Drawing.Size(517, 106);
            this.baselineAlertConfigurationGrid.TabIndex = 5;
            this.baselineAlertConfigurationGrid.UpdateMode = Infragistics.Win.UltraWinGrid.UpdateMode.OnCellChangeOrLostFocus;
            this.baselineAlertConfigurationGrid.AfterCellUpdate += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.baselineAlertConfigurationGrid_AfterCellUpdate);
            this.baselineAlertConfigurationGrid.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(this.alertConfigurationGrid_InitializeRow);
            this.baselineAlertConfigurationGrid.BeforeCellActivate += new Infragistics.Win.UltraWinGrid.CancelableCellEventHandler(this.alertConfigurationGrid_BeforeCellActivate);
            this.baselineAlertConfigurationGrid.DoubleClickCell += new Infragistics.Win.UltraWinGrid.DoubleClickCellEventHandler(this.alertConfigurationGrid_DoubleClickCell);
            this.baselineAlertConfigurationGrid.MouseClick += new System.Windows.Forms.MouseEventHandler(this.alertsGrid_MouseClick);
            this.baselineAlertConfigurationGrid.Visible = false;
            // 
            // configBindingSource
            // 
            this.configBindingSource.AllowNew = true;
            this.configBindingSource.DataMember = "ItemList";
            this.configBindingSource.DataSource = typeof(Idera.SQLdm.Common.Configuration.AlertConfiguration);
            this.configBindingSource.CurrentChanged += new System.EventHandler(this.configBindingSource_CurrentChanged);
            // 
            // informationLabelPanel
            // 
            this.informationLabelPanel.Controls.Add(this.informationLabelPictureBox);
            this.informationLabelPanel.Controls.Add(this.informationLabel);
            this.informationLabelPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.informationLabelPanel.Location = new System.Drawing.Point(0, 6);
            this.informationLabelPanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.informationLabelPanel.Name = "informationLabelPanel";
            this.informationLabelPanel.Size = new System.Drawing.Size(517, 39);
            this.informationLabelPanel.TabIndex = 19;
            // 
            // informationLabelPictureBox
            // 
            this.informationLabelPictureBox.BackColor = System.Drawing.Color.LightGray;
            this.informationLabelPictureBox.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusWarningSmall;
            this.informationLabelPictureBox.Location = new System.Drawing.Point(5, 10);
            this.informationLabelPictureBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.informationLabelPictureBox.Name = "informationLabelPictureBox";
            this.informationLabelPictureBox.Size = new System.Drawing.Size(16, 16);
            this.informationLabelPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.informationLabelPictureBox.TabIndex = 18;
            this.informationLabelPictureBox.TabStop = false;
            // 
            // informationLabel
            // 
            this.informationLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.informationLabel.AutoEllipsis = true;
            this.informationLabel.BackColor = System.Drawing.Color.LightGray;
            this.informationLabel.ForeColor = System.Drawing.Color.Black;
            this.informationLabel.Location = new System.Drawing.Point(0, 7);
            this.informationLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.informationLabel.Name = "informationLabel";
            this.informationLabel.Padding = new System.Windows.Forms.Padding(29, 0, 0, 0);
            this.informationLabel.Size = new System.Drawing.Size(517, 25);
            this.informationLabel.TabIndex = 17;
            this.informationLabel.Text = "One or more values are not assigned.";
            this.informationLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.informationLabel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.informationLabel_MouseClick);
            this.informationLabel.MouseEnter += new System.EventHandler(this.informationLabel_MouseEnter);
            this.informationLabel.MouseLeave += new System.EventHandler(this.informationLabel_MouseLeave);
            // 
            // configurationGaugePanel
            // 
            this.configurationGaugePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.configurationGaugePanel.Controls.Add(this.alertConfigurationGauge);
            this.configurationGaugePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.configurationGaugePanel.Location = new System.Drawing.Point(0, 0);
            this.configurationGaugePanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.configurationGaugePanel.Name = "configurationGaugePanel";
            this.configurationGaugePanel.Padding = new System.Windows.Forms.Padding(0, 6, 0, 6);
            this.configurationGaugePanel.Size = new System.Drawing.Size(517, 105);
            this.configurationGaugePanel.TabIndex = 0;
            // 
            // alertConfigurationGauge
            // 
            this.alertConfigurationGauge.Border.Visible = false;
            this.alertConfigurationGauge.Dock = System.Windows.Forms.DockStyle.Fill;
            this.alertConfigurationGauge.Location = new System.Drawing.Point(0, 6);
            this.alertConfigurationGauge.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.alertConfigurationGauge.Name = "alertConfigurationGauge";
            linearScale1.AutoScaleInterval = null;
            linearScale1.Bar.Color = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(225)))), ((int)(((byte)(243)))));
            linearScale1.Color = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(225)))), ((int)(((byte)(243)))));
            marker1.Color = System.Drawing.Color.Blue;
            marker1.Draggable = true;
            marker1.Format.Decimals = 0;
            marker1.Format.FormatCustom = "F0";
            marker1.Format.FormatType = ChartFX.WinForms.Gauge.ValueFormatType.Custom;
            marker1.Label.Position = ChartFX.WinForms.Gauge.Position.Bottom;
            marker1.Label.Visible = true;
            marker1.Position = ChartFX.WinForms.Gauge.Position.Bottom;
            marker1.Size = 1.4F;
            marker1.Value = 40;
            marker2.Color = System.Drawing.Color.Gold;
            marker2.Draggable = true;
            marker2.Format.FormatCustom = "F0";
            marker2.Format.FormatType = ChartFX.WinForms.Gauge.ValueFormatType.Custom;
            marker2.Label.Visible = true;
            marker2.Main = false;
            marker2.Position = ChartFX.WinForms.Gauge.Position.Top;
            marker2.PositionCustom = -0.1F;
            marker2.Size = 1.4F;
            marker2.Value = 60;
            marker3.Color = System.Drawing.Color.Red;
            marker3.Draggable = true;
            marker3.Format.FormatCustom = "F0";
            marker3.Format.FormatType = ChartFX.WinForms.Gauge.ValueFormatType.Custom;
            marker3.Label.Position = ChartFX.WinForms.Gauge.Position.Bottom;
            marker3.Label.Visible = true;
            marker3.Main = false;
            marker3.Position = ChartFX.WinForms.Gauge.Position.Bottom;
            marker3.Size = 1.4F;
            marker3.Value = 80;
            linearScale1.Indicators.AddRange(new ChartFX.WinForms.Gauge.Indicator[] {
            marker1,
            marker2,
            marker3});
            linearScale1.Max = 100D;
            linearScale1.MaxAlwaysDisplayed = true;
            linearScale1.MinAlwaysDisplayed = true;
            section1.Bar.Color = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            section1.Color = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            section1.Max = 40D;
            section2.Bar.Color = System.Drawing.Color.Blue;
            section2.Color = System.Drawing.Color.Blue;
            section2.Max = 60D;
            section2.Min = 40D;
            section3.Bar.Color = System.Drawing.Color.Yellow;
            section3.Color = System.Drawing.Color.Yellow;
            section3.Max = 80D;
            section3.Min = 60D;
            section4.Bar.Color = System.Drawing.Color.Red;
            section4.Color = System.Drawing.Color.Red;
            section4.Min = 80D;
            linearScale1.Sections.AddRange(new ChartFX.WinForms.Gauge.Section[] {
            section1,
            section2,
            section3,
            section4});
            linearStrip1.Border.Color = System.Drawing.Color.Black;
            linearStrip1.Color = System.Drawing.Color.Blue;
            linearStrip1.FillType = ChartFX.WinForms.Gauge.StripFillType.Pattern;
            linearStrip1.Max = 70D;
            linearStrip1.Min = 50D;
            linearStrip1.Offset = 0.17F;
            linearScale1.Stripes.AddRange(new ChartFX.WinForms.Gauge.LinearStrip[] {
            linearStrip1});
            linearScale1.Thickness = 0.17F;
            linearScale1.Tickmarks.Major.Label.Position = ChartFX.WinForms.Gauge.Position.Bottom;
            linearScale1.Tickmarks.Medium.Visible = false;
            linearScale1.Tickmarks.Visible = false;
            this.alertConfigurationGauge.Scales.AddRange(new ChartFX.WinForms.Gauge.LinearScale[] {
            linearScale1});
            this.alertConfigurationGauge.Size = new System.Drawing.Size(517, 93);
            this.alertConfigurationGauge.TabIndex = 1;
            this.alertConfigurationGauge.Text = "horizontalGauge1";
            this.alertConfigurationGauge.ValueChanged += new ChartFX.WinForms.Gauge.IndicatorEventHandler(this.alertConfigurationGauge_ValueChanged);
            this.alertConfigurationGauge.GetTip += new ChartFX.WinForms.Gauge.GetTipEventHandler(this.alertConfigurationGauge_GetTip);
            this.alertConfigurationGauge.MouseDown += new System.Windows.Forms.MouseEventHandler(this.alertConfigurationGauge_MouseDown);
            // 
            // advancedPanel
            // 
            this.advancedPanel.AutoSize = true;
            this.advancedPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;

            //10.0 srishti purohit -- for baseline alert modifications
            //this.advancedPanel.Controls.Add(this.isBaselineEnabledCheckBox);
            this.advancedPanel.Controls.Add(this.advancedButton);
            this.advancedPanel.Controls.Add(this.spacelabel);
            this.advancedPanel.Controls.Add(this.editButton);
            this.advancedPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.advancedPanel.Location = new System.Drawing.Point(0, 252);
            this.advancedPanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.advancedPanel.MinimumSize = new System.Drawing.Size(0, 48);
            this.advancedPanel.Name = "advancedPanel";
            this.advancedPanel.Padding = new System.Windows.Forms.Padding(11, 10, 11, 10);
            this.advancedPanel.Size = new System.Drawing.Size(517, 52);
            this.advancedPanel.TabIndex = 2;
            // 
            // isBaselineEnabledCheck
            // Pruthviraj Nikam: Done changes for 5.1.7 Baseline Alerts     Date: 22-Jan-2019
            //this.isBaselineEnabledCheckBox.AutoSize = false;
            //this.isBaselineEnabledCheckBox.Location = new System.Drawing.Point(0, 5);
            //this.isBaselineEnabledCheckBox.Name = "isBaselineEnabledCheck";
            //this.isBaselineEnabledCheckBox.Size = new System.Drawing.Size(245, 40);
            //this.isBaselineEnabledCheckBox.TabIndex = 0;
            //this.isBaselineEnabledCheckBox.Text = "Baseline Thresholds Enabled \n(as percentage of baseline)";
            //this.isBaselineEnabledCheckBox.UseVisualStyleBackColor = true;
            //this.isBaselineEnabledCheckBox.CheckedChanged += new System.EventHandler(this.isBaselineEnabledCheckBox_CheckedChanged);
            //this.isBaselineEnabledCheckBox.Visible = false;
            //this.isBaselineEnabledCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("CheckState", this.configBindingSource, "IsBaselineEnabled", true));
            // 
            // advancedButton
            // 
            this.advancedButton.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.advancedButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.advancedButton.Location = new System.Drawing.Point(336, 10);
            this.advancedButton.Margin = new System.Windows.Forms.Padding(40, 4, 40, 4);
            this.advancedButton.Name = "advancedButton";
            this.advancedButton.Size = new System.Drawing.Size(75, 28);
            this.advancedButton.TabIndex = 1;
            this.advancedButton.Text = "Advanced";
            this.advancedButton.UseVisualStyleBackColor = false;
            this.advancedButton.Click += new System.EventHandler(this.advancedButton_Click);
            // 
            // editButton
            // 
            this.editButton.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.editButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.editButton.Location = new System.Drawing.Point(432, 10);
            this.editButton.Margin = new System.Windows.Forms.Padding(40, 4, 40, 4);
            this.editButton.Name = "editButton";
            this.editButton.Size = new System.Drawing.Size(75, 28);
            this.editButton.TabIndex = 2;
            this.editButton.Text = "Edit";
            this.editButton.UseVisualStyleBackColor = false;
            this.editButton.Click += new System.EventHandler(this.editButton_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.categoryTextBox);
            this.panel2.Controls.Add(this.metricNameDescriptionLabel);
            this.panel2.Controls.Add(this.descriptionTextBox);
            this.panel2.Controls.Add(this.metricNameTextBox);
            this.panel2.Controls.Add(this.RankTextBox);

            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.RankLabel);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(7, 6);
            this.panel2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(517, 190);
            this.panel2.TabIndex = 18;
            // 
            // categoryTextBox
            // 
            this.categoryTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.categoryTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.configBindingSource, "Category", true));
            this.categoryTextBox.Location = new System.Drawing.Point(95, 39);
            this.categoryTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.categoryTextBox.Name = "categoryTextBox";
            this.categoryTextBox.ReadOnly = true;
            this.categoryTextBox.Size = new System.Drawing.Size(416, 22);
            this.categoryTextBox.TabIndex = 25;

            //RankLabel
            this.RankLabel.AutoSize = true;
            this.RankLabel.Location = new System.Drawing.Point(8, 150);
            this.RankLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.RankLabel.Name = "RankLabel";
            this.RankLabel.Size = new System.Drawing.Size(50, 17);
            this.RankLabel.TabIndex = 25;
            this.RankLabel.Text = "Rank:";

            ///RankTextBox
            ///
            this.RankTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RankTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.configBindingSource, "Rank", true));
            this.RankTextBox.Location = new System.Drawing.Point(95, 150);
            this.RankTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.RankTextBox.Name = "RankTextBox";
            this.RankTextBox.ReadOnly = false;
            this.RankTextBox.Size = new System.Drawing.Size(416, 22);
            this.RankTextBox.TabIndex = 8;
            // this.RankTextBox.KeyPress += new System.EventHandler(this.RankTextBox_KeyPress);
        //    this.KeyDown += new KeyEventHandler(this.RankTextBox_KeyPress);
            // this.KeyPress += new KeyEventHandler(this.RankTextBox_KeyPress);
            this.RankTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RankTextBox_KeyPress);
            this.RankTextBox.TextChanged += RankTextBox_TextChanged;
            this.RankTextBox.Leave += RankTextBox_Leave;

            // 
            // metricNameDescriptionLabel
            // 
            this.metricNameDescriptionLabel.AutoSize = true;
            this.metricNameDescriptionLabel.Location = new System.Drawing.Point(7, 11);
            this.metricNameDescriptionLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.metricNameDescriptionLabel.Name = "metricNameDescriptionLabel";
            this.metricNameDescriptionLabel.Size = new System.Drawing.Size(50, 17);
            this.metricNameDescriptionLabel.TabIndex = 0;
            this.metricNameDescriptionLabel.Text = "Metric:";
            // 
            // descriptionTextBox
            // 
            this.descriptionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.descriptionTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.configBindingSource, "Description", true));
            this.descriptionTextBox.Location = new System.Drawing.Point(95, 71);
            this.descriptionTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.descriptionTextBox.Multiline = true;
            this.descriptionTextBox.Name = "descriptionTextBox";
            this.descriptionTextBox.ReadOnly = true;
            this.descriptionTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.descriptionTextBox.Size = new System.Drawing.Size(416, 70);
            this.descriptionTextBox.TabIndex = 9;
            // 
            // metricNameTextBox
            // 
            this.metricNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.metricNameTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.configBindingSource, "Name", true));
            this.metricNameTextBox.Location = new System.Drawing.Point(95, 7);
            this.metricNameTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.metricNameTextBox.Name = "metricNameTextBox";
            this.metricNameTextBox.ReadOnly = true;
            this.metricNameTextBox.Size = new System.Drawing.Size(416, 22);
            this.metricNameTextBox.TabIndex = 1;

            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 75);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 17);
            this.label2.TabIndex = 8;
            this.label2.Text = "Description:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 43);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 17);
            this.label1.TabIndex = 6;
            this.label1.Text = "Category:";
            // 
            // ultraTabPageControl2
            // 
            this.ultraTabPageControl2.Controls.Add(this.commentsTextBox);
            this.ultraTabPageControl2.Controls.Add(this.panel1);
            this.ultraTabPageControl2.Location = new System.Drawing.Point(-13333, -12308);
            this.ultraTabPageControl2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ultraTabPageControl2.Name = "ultraTabPageControl2";
            this.ultraTabPageControl2.Padding = new System.Windows.Forms.Padding(9, 9, 9, 9);
            this.ultraTabPageControl2.Size = new System.Drawing.Size(531, 460);
            // 
            // commentsTextBox
            // 
            this.commentsTextBox.AcceptsReturn = true;
            this.commentsTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.commentsTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.configBindingSource, "Comments", true));
            this.commentsTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.commentsTextBox.Location = new System.Drawing.Point(9, 93);
            this.commentsTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.commentsTextBox.MaxLength = 4000;
            this.commentsTextBox.Multiline = true;
            this.commentsTextBox.Name = "commentsTextBox";
            this.commentsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.commentsTextBox.Size = new System.Drawing.Size(513, 358);
            this.commentsTextBox.TabIndex = 3;
            this.commentsTextBox.TextChanged += new System.EventHandler(this.commentsTextBox_TextChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.commentsInformationBox);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(9, 9);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(5, 84);
            this.panel1.TabIndex = 5;
            // 
            // commentsInformationBox
            // 
            this.commentsInformationBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.commentsInformationBox.Location = new System.Drawing.Point(0, 0);
            this.commentsInformationBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.commentsInformationBox.Name = "commentsInformationBox";
            this.commentsInformationBox.Size = new System.Drawing.Size(48, 84);
            this.commentsInformationBox.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoEllipsis = true;
            this.label3.Location = new System.Drawing.Point(49, 0);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(464, 84);
            this.label3.TabIndex = 5;
            this.label3.Text = resources.GetString("label3.Text");
            // 
            // space label
            // 
            this.spacelabel.Dock = System.Windows.Forms.DockStyle.Right;
            //this.label3.AutoEllipsis = true;
            this.spacelabel.Location = new System.Drawing.Point(400, 10);
            this.spacelabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.spacelabel.Name = "spacelabel";
            this.spacelabel.Size = new System.Drawing.Size(5, 20);
            this.spacelabel.Text = "  ";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(728, 534);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(100, 28);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // applyButton
            // 
            this.applyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.applyButton.Location = new System.Drawing.Point(621, 534);
            this.applyButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(100, 28);
            this.applyButton.TabIndex = 1;
            this.applyButton.Text = "Apply";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(515, 534);
            this.okButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(100, 28);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer.Location = new System.Drawing.Point(0, 33);
            this.splitContainer.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.alertsGrid);
            this.splitContainer.Panel1.Controls.Add(this.checkBox1); //CustomCheckbox 4.12 DarkTheme Babita Manral           
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.alertTabControl);
            this.splitContainer.Size = new System.Drawing.Size(827, 484);
            this.splitContainer.SplitterDistance = 289;
            this.splitContainer.SplitterWidth = 5;
            this.splitContainer.TabIndex = 3;
            this.splitContainer.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer_SplitterMoved);
            this.splitContainer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.splitContainer_MouseDown);
            this.splitContainer.MouseUp += new System.Windows.Forms.MouseEventHandler(this.splitContainer_MouseUp);
            // 
            // alertsGrid
            // 
            this.alertsGrid.CalcManager = this.ultraCalcManager1;
            this.alertsGrid.DataSource = this.configBindingSource;
            appearance3.BackColor = System.Drawing.SystemColors.Window;
            appearance3.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.alertsGrid.DisplayLayout.Appearance = appearance3;
            this.alertsGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            ultraGridColumn40.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn40.Header.VisiblePosition = 14;
            ultraGridColumn40.Hidden = true;
            ultraGridColumn41.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn41.Header.VisiblePosition = 10;
            ultraGridColumn41.Hidden = true;
            ultraGridColumn42.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn42.Header.VisiblePosition = 13;
            ultraGridColumn42.Hidden = true;
            ultraGridColumn42.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn42.RowLayoutColumnInfo.SpanY = 2;
            ultraGridColumn43.Header.VisiblePosition = 2;
            ultraGridColumn43.Hidden = true;
            ultraGridColumn44.Header.VisiblePosition = 3;
            ultraGridColumn44.Hidden = true;
            ultraGridColumn65.Header.VisiblePosition = 5;
            ultraGridColumn65.Hidden = true;
            ultraGridColumn66.Header.VisiblePosition = 6;
            ultraGridColumn66.Hidden = true;
            ultraGridColumn67.Header.VisiblePosition = 8;
            ultraGridColumn67.Hidden = true;
            ultraGridColumn68.Header.VisiblePosition = 11;
            ultraGridColumn68.Hidden = true;
            ultraGridColumn69.Header.VisiblePosition = 7;
            ultraGridColumn69.Hidden = true;

            ultraGridColumn101.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Default;
            ultraGridColumn101.Header.Caption = "Rank";
            ultraGridColumn101.Header.VisiblePosition = 4;
            ultraGridColumn101.RowLayoutColumnInfo.OriginX = 4;
            ultraGridColumn101.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn101.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn101.RowLayoutColumnInfo.SpanY = 2;
			ultraGridColumn101.Width = 35;//30242
            //appearance22.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.data_edit;
           // appearance22.ImageHAlign = Infragistics.Win.HAlign.Right;
           // appearance22.ImageVAlign = Infragistics.Win.VAlign.Middle;
          //  ultraGridColumn101.ValueBasedAppearance = conditionValueAppearance1;

            ultraGridColumn70.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn70.Header.Caption = "Metric";
            ultraGridColumn70.Header.VisiblePosition = 4;
            ultraGridColumn70.RowLayoutColumnInfo.OriginX = 4;
            ultraGridColumn70.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn70.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn70.RowLayoutColumnInfo.SpanY = 2;
            appearance22.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.data_edit;
            appearance22.ImageHAlign = Infragistics.Win.HAlign.Right;
            appearance22.ImageVAlign = Infragistics.Win.VAlign.Middle;
            ultraGridColumn70.ValueBasedAppearance = conditionValueAppearance1;
            ultraGridColumn71.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn71.Header.VisiblePosition = 1;
            ultraGridColumn71.Hidden = true;
            ultraGridColumn71.HiddenWhenGroupBy = Infragistics.Win.DefaultableBoolean.True;
            ultraGridColumn72.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn72.Header.VisiblePosition = 16;
            ultraGridColumn72.Hidden = true;
            ultraGridColumn73.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            appearance23.ImageHAlign = Infragistics.Win.HAlign.Center;
            ultraGridColumn73.CellAppearance = appearance23;
            ultraGridColumn73.Header.VisiblePosition = 0;
            ultraGridColumn73.MaxWidth = 50;
            ultraGridColumn73.MinWidth = 50;
            ultraGridColumn73.RowLayoutColumnInfo.OriginX = 2;
            ultraGridColumn73.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn73.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn73.RowLayoutColumnInfo.SpanY = 2;
            ultraGridColumn73.Width = 50;
            ultraGridColumn74.Header.VisiblePosition = 15;
            ultraGridColumn74.Hidden = true;
            ultraGridColumn75.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn75.Header.VisiblePosition = 19;
            ultraGridColumn75.Hidden = true;
            ultraGridColumn76.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn76.Header.VisiblePosition = 12;
            ultraGridColumn76.Hidden = true;
            ultraGridColumn77.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn77.Header.VisiblePosition = 23;
            ultraGridColumn77.Hidden = true;
            ultraGridColumn78.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn78.Header.VisiblePosition = 17;
            ultraGridColumn78.Hidden = true;
            ultraGridColumn79.Header.VisiblePosition = 20;
            ultraGridColumn79.Hidden = true;
            ultraGridColumn80.Header.VisiblePosition = 21;
            ultraGridColumn80.Hidden = true;
            ultraGridColumn81.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn81.Header.VisiblePosition = 9;
            ultraGridColumn81.Hidden = true;
            ultraGridColumn82.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn82.Header.VisiblePosition = 18;
            ultraGridColumn82.Hidden = true;
            ultraGridColumn83.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn83.Header.VisiblePosition = 24;
            ultraGridColumn83.Hidden = true;
            ultraGridColumn84.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn84.Header.VisiblePosition = 22;
            ultraGridColumn84.Hidden = true;
            ultraGridColumnIsBaselineEnabled.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            //ultraGridColumnIsBaselineEnabled.Header.VisiblePosition = 22;
            ultraGridColumnIsBaselineEnabled.Hidden = true;
            ultraGridColumnRangeStartBaseline1.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumnRangeStartBaseline1.Hidden = true;
            ultraGridColumnRangeStartBaseline2.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumnRangeStartBaseline2.Hidden = true;
            ultraGridColumnRangeStartBaseline3.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumnRangeStartBaseline3.Hidden = true; 
            ultraGridColumnFlattenedThresholdsBaseline.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumnFlattenedThresholdsBaseline.Hidden = true;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn40,
            ultraGridColumn41,
            ultraGridColumn42,
            ultraGridColumn43,
            ultraGridColumn44,
            ultraGridColumn65,
            ultraGridColumn66,
            ultraGridColumn67,
            ultraGridColumn68,
            ultraGridColumn69,
            ultraGridColumn70,
            ultraGridColumn71,
            ultraGridColumn72,
            ultraGridColumn73,
            ultraGridColumn74,
            ultraGridColumn75,
            ultraGridColumn76,
            ultraGridColumn77,
            ultraGridColumn78,
            ultraGridColumn79,
            ultraGridColumn80,
            ultraGridColumn81,
            ultraGridColumn82,
            ultraGridColumn83,
            ultraGridColumn84,
            ultraGridColumn101,
            ultraGridColumnIsBaselineEnabled,
            ultraGridColumnRangeStartBaseline1,
            ultraGridColumnRangeStartBaseline2,
            ultraGridColumnRangeStartBaseline3,
            ultraGridColumnFlattenedThresholdsBaseline
            });
            ultraGridBand1.GroupHeadersVisible = false;
            ultraGridBand1.Indentation = 0;
            ultraGridBand1.IndentationGroupByRow = 0;
            ultraGridBand1.IndentationGroupByRowExpansionIndicator = 0;
            ultraGridBand1.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
            ultraGridBand1.Override.RowFilterAction = Infragistics.Win.UltraWinGrid.RowFilterAction.HideFilteredOutRows;
            ultraGridBand1.Override.RowFilterMode = Infragistics.Win.UltraWinGrid.RowFilterMode.Default;
            rowLayoutColumnInfo3.OriginX = 0;
            rowLayoutColumnInfo3.OriginY = 0;
            rowLayoutColumnInfo3.ParentGroupIndex = 0;
            rowLayoutColumnInfo3.ParentGroupKey = "NewGroup0";
            rowLayoutColumnInfo3.SpanX = 2;
            rowLayoutColumnInfo3.SpanY = 2;
            rowLayoutColumnInfo11.OriginX = 4;
            rowLayoutColumnInfo11.OriginY = 0;
            rowLayoutColumnInfo11.SpanX = 2;
            rowLayoutColumnInfo11.SpanY = 2;
            rowLayoutColumnInfo14.OriginX = 2;
            rowLayoutColumnInfo14.OriginY = 0;
            rowLayoutColumnInfo14.SpanX = 2;
            rowLayoutColumnInfo14.SpanY = 2;
            rowLayout1.ColumnInfos.AddRange(new Infragistics.Win.UltraWinGrid.RowLayoutColumnInfo[] {
            rowLayoutColumnInfo1,
            rowLayoutColumnInfo2,
            rowLayoutColumnInfo3,
            rowLayoutColumnInfo4,
            rowLayoutColumnInfo5,
            rowLayoutColumnInfo6,
            rowLayoutColumnInfo7,
            rowLayoutColumnInfo8,
            rowLayoutColumnInfo9,
            rowLayoutColumnInfo10,
            rowLayoutColumnInfo11,
            rowLayoutColumnInfo12,
            rowLayoutColumnInfo13,
            rowLayoutColumnInfo14,
            rowLayoutColumnInfo15,
            rowLayoutColumnInfo16,
            rowLayoutColumnInfo17,
            rowLayoutColumnInfo18,
            rowLayoutColumnInfo19,
            rowLayoutColumnInfo20,
            rowLayoutColumnInfo21,
            rowLayoutColumnInfo22,
            rowLayoutColumnInfo23,
            rowLayoutColumnInfo24,
            rowLayoutColumnInfo25});
            rowLayout1.RowLayoutStyle = Infragistics.Win.UltraWinGrid.RowLayoutStyle.GroupLayout;
            ultraGridBand1.RowLayouts.AddRange(new Infragistics.Win.UltraWinGrid.RowLayout[] {
            rowLayout1});
            this.alertsGrid.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.alertsGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(239)))), ((int)(((byte)(255)))));
            appearance6.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance6.BackGradientStyle = Infragistics.Win.GradientStyle.None;
            appearance6.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(147)))), ((int)(((byte)(207)))));
            this.alertsGrid.DisplayLayout.GroupByBox.Appearance = appearance6;
            appearance7.ForeColor = System.Drawing.SystemColors.GrayText;
            this.alertsGrid.DisplayLayout.GroupByBox.BandLabelAppearance = appearance7;
            this.alertsGrid.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.alertsGrid.DisplayLayout.GroupByBox.Hidden = true;
            appearance8.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance8.BackColor2 = System.Drawing.SystemColors.Control;
            appearance8.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance8.ForeColor = System.Drawing.SystemColors.GrayText;
            this.alertsGrid.DisplayLayout.GroupByBox.PromptAppearance = appearance8;
            this.alertsGrid.DisplayLayout.MaxColScrollRegions = 1;
            this.alertsGrid.DisplayLayout.MaxRowScrollRegions = 1;
            this.alertsGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.alertsGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.alertsGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.alertsGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.None;
            this.alertsGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.None;
            appearance9.BackColor = System.Drawing.SystemColors.Window;
            this.alertsGrid.DisplayLayout.Override.CardAreaAppearance = appearance9;
            appearance10.BorderColor = System.Drawing.Color.Silver;
            appearance10.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.alertsGrid.DisplayLayout.Override.CellAppearance = appearance10;
            this.alertsGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.alertsGrid.DisplayLayout.Override.CellPadding = 0;
            this.alertsGrid.DisplayLayout.Override.FixedHeaderIndicator = Infragistics.Win.UltraWinGrid.FixedHeaderIndicator.None;
            this.alertsGrid.DisplayLayout.Override.GroupByColumnsHidden = Infragistics.Win.DefaultableBoolean.False;
            appearance11.BackColor = System.Drawing.SystemColors.Control;
            appearance11.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance11.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance11.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance11.BorderColor = System.Drawing.SystemColors.Window;
            this.alertsGrid.DisplayLayout.Override.GroupByRowAppearance = appearance11;
            this.alertsGrid.DisplayLayout.Override.GroupByRowInitialExpansionState = Infragistics.Win.UltraWinGrid.GroupByRowInitialExpansionState.Expanded;
            appearance12.TextHAlignAsString = "Left";
            this.alertsGrid.DisplayLayout.Override.HeaderAppearance = appearance12;
            this.alertsGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortSingle;
            appearance13.BackColor = System.Drawing.SystemColors.Window;
            appearance13.BorderColor = System.Drawing.Color.Silver;
            this.alertsGrid.DisplayLayout.Override.RowAppearance = appearance13;
            this.alertsGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.alertsGrid.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.alertsGrid.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.alertsGrid.DisplayLayout.Override.SelectTypeGroupByRow = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.alertsGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            appearance14.BackColor = System.Drawing.SystemColors.ControlLight;
            this.alertsGrid.DisplayLayout.Override.TemplateAddRowAppearance = appearance14;
            this.alertsGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.alertsGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.alertsGrid.DisplayLayout.UseFixedHeaders = true;
            valueList1.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.Picture;
            valueList1.Key = "CheckBoxes";
            valueList1.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            appearance15.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RibbonCheckboxUnchecked;
            appearance15.ImageHAlign = Infragistics.Win.HAlign.Center;
            valueListItem1.Appearance = appearance15;
            valueListItem1.DataValue = false;
            appearance16.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RibbonCheckboxChecked;
            appearance16.ImageHAlign = Infragistics.Win.HAlign.Center;
            valueListItem2.Appearance = appearance16;
            valueListItem2.DataValue = true;
            valueListItem2.DisplayText = "";
            valueList1.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem1,
            valueListItem2});
            this.alertsGrid.DisplayLayout.ValueLists.AddRange(new Infragistics.Win.ValueList[] {
            valueList1});
            this.alertsGrid.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.alertsGrid.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.alertsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.alertsGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.alertsGrid.Location = new System.Drawing.Point(0, 22);
            this.alertsGrid.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.alertsGrid.Name = "alertsGrid";
            this.alertsGrid.Size = new System.Drawing.Size(289, 462);
            this.alertsGrid.TabIndex = 4;
            this.alertsGrid.UpdateMode = Infragistics.Win.UltraWinGrid.UpdateMode.OnCellChangeOrLostFocus;
            this.alertsGrid.MouseClick += new System.Windows.Forms.MouseEventHandler(this.alertsGrid_MouseClick);
            //
            //CustomCheckbox 4.12 DarkTheme  Babita Manral           
            //
            this.checkBox1.Checked = global::Idera.SQLdm.DesktopClient.Properties.Settings.Default.AlertConfigurationDialogGroupByCategory;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Idera.SQLdm.DesktopClient.Properties.Settings.Default, "AlertConfigurationDialogGroupByCategory", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.checkBox1.Location = new System.Drawing.Point(0, 0);
            this.checkBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBox1.Name = "checkBox1";            
            this.checkBox1.Size= new System.Drawing.Size(289, 22);
            this.checkBox1.TabIndex = 5;
            this.checkBox1.Text = "Group by metric category";                       
            this.checkBox1.CheckedChanged += new System.EventHandler(this.ultraCheckEditor1_CheckedChanged);
            // 
            // alertTabControl
            // 
            appearance17.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(197)))), ((int)(((byte)(210)))));
            this.alertTabControl.Appearance = appearance17;
            appearance18.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            appearance18.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            if(Settings.Default.ColorScheme == "Dark")
            {
                appearance18.BackColor = ColorTranslator.FromHtml(DarkThemeColorConstants.BackColor);
                appearance18.BackColor2 = ColorTranslator.FromHtml(DarkThemeColorConstants.BackColor);
            }
            this.alertTabControl.ClientAreaAppearance = appearance18;
            this.alertTabControl.Controls.Add(this.ultraTabPageControl1);
            this.alertTabControl.Controls.Add(this.ultraTabSharedControlsPage1);
            this.alertTabControl.Controls.Add(this.ultraTabPageControl2);
            this.alertTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.alertTabControl.Location = new System.Drawing.Point(0, 0);
            this.alertTabControl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.alertTabControl.Name = "alertTabControl";
            this.alertTabControl.SharedControlsPage = this.ultraTabSharedControlsPage1;
            this.alertTabControl.Size = new System.Drawing.Size(533, 484);
            appearance19.BackColor = Settings.Default.ColorScheme == "Dark" ? ColorTranslator.FromHtml(DarkThemeColorConstants.BackColor) : System.Drawing.SystemColors.ControlDark;//SQLdm 9.1 (Vineet Kumar) -- Making inactive tab visible.
            this.alertTabControl.TabHeaderAreaAppearance = appearance19;
            this.alertTabControl.TabIndex = 0;
            ultraTab1.TabPage = this.ultraTabPageControl1;
            ultraTab1.Text = "Configuration";
            ultraTab2.TabPage = this.ultraTabPageControl2;
            ultraTab2.Text = "Comments";
            this.alertTabControl.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab1,
            ultraTab2});
            this.alertTabControl.UseAppStyling = false;
            this.alertTabControl.ViewStyle = Infragistics.Win.UltraWinTabControl.ViewStyle.Office2007;
            // 
            // ultraTabSharedControlsPage1
            // 
            this.ultraTabSharedControlsPage1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ultraTabSharedControlsPage1.Name = "ultraTabSharedControlsPage1";
            this.ultraTabSharedControlsPage1.Size = new System.Drawing.Size(531, 460);
            // 
            // applyTemplateButton
            // 
            this.applyTemplateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.applyTemplateButton.Enabled = false;
            this.applyTemplateButton.Location = new System.Drawing.Point(16, 534);
            this.applyTemplateButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.applyTemplateButton.Name = "applyTemplateButton";
            this.applyTemplateButton.Size = new System.Drawing.Size(127, 28);
            this.applyTemplateButton.TabIndex = 6;
            this.applyTemplateButton.Text = "Apply Template";
            this.applyTemplateButton.UseVisualStyleBackColor = true;
            this.applyTemplateButton.Visible = false;
            this.applyTemplateButton.Click += new System.EventHandler(this.applyTemplateButton_Click);
            // 
            // createTemplateButton
            // 
            this.createTemplateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.createTemplateButton.Location = new System.Drawing.Point(165, 534);
            this.createTemplateButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.createTemplateButton.Name = "createTemplateButton";
            this.createTemplateButton.Size = new System.Drawing.Size(127, 28);
            this.createTemplateButton.TabIndex = 7;
            this.createTemplateButton.Text = "Create Template";
            this.createTemplateButton.UseVisualStyleBackColor = true;
            this.createTemplateButton.Click += new System.EventHandler(this.createTemplateButton_Click);
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.Controls.Add(this.splitContainer);
            this.panel3.Controls.Add(this.alertRecommendationsPanel);
            this.panel3.Location = new System.Drawing.Point(8, 10);
            this.panel3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(827, 517);
            this.panel3.TabIndex = 9;
            // 
            // alertRecommendationsPanel
            // 
            this.alertRecommendationsPanel.Controls.Add(this.alertRecommendationsWarningImage);
            this.alertRecommendationsPanel.Controls.Add(this.alertRecommendationsLabel);
            this.alertRecommendationsPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.alertRecommendationsPanel.Location = new System.Drawing.Point(0, 0);
            this.alertRecommendationsPanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.alertRecommendationsPanel.Name = "alertRecommendationsPanel";
            this.alertRecommendationsPanel.Size = new System.Drawing.Size(827, 33);
            this.alertRecommendationsPanel.TabIndex = 0;
            this.alertRecommendationsPanel.Visible = false;
            // 
            // alertRecommendationsWarningImage
            // 
            this.alertRecommendationsWarningImage.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusWarningSmall;
            this.alertRecommendationsWarningImage.Location = new System.Drawing.Point(4, 6);
            this.alertRecommendationsWarningImage.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.alertRecommendationsWarningImage.Name = "alertRecommendationsWarningImage";
            this.alertRecommendationsWarningImage.Size = new System.Drawing.Size(16, 16);
            this.alertRecommendationsWarningImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.alertRecommendationsWarningImage.TabIndex = 5;
            this.alertRecommendationsWarningImage.TabStop = false;
            // 
            // alertRecommendationsLabel
            // 
            this.alertRecommendationsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.alertRecommendationsLabel.ForeColor = System.Drawing.Color.Black;
            this.alertRecommendationsLabel.Location = new System.Drawing.Point(0, 4);
            this.alertRecommendationsLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.alertRecommendationsLabel.Name = "alertRecommendationsLabel";
            this.alertRecommendationsLabel.Padding = new System.Windows.Forms.Padding(27, 0, 0, 0);
            this.alertRecommendationsLabel.Size = new System.Drawing.Size(827, 26);
            this.alertRecommendationsLabel.TabIndex = 4;
            this.alertRecommendationsLabel.Text = "Alert recommendations are available for this SQL Server. Click here to view the r" +
    "ecommendations now.";
            this.alertRecommendationsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.alertRecommendationsLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.alertRecommendationsLabel_MouseDown);
            this.alertRecommendationsLabel.MouseEnter += new System.EventHandler(this.alertRecommendationsLabel_MouseEnter);
            this.alertRecommendationsLabel.MouseLeave += new System.EventHandler(this.alertRecommendationsLabel_MouseLeave);
            this.alertRecommendationsLabel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.alertRecommendationsLabel_MouseUp);
            // 
            // alertRecommendationsBackgroundWorker
            // 
            this.alertRecommendationsBackgroundWorker.WorkerReportsProgress = true;
            this.alertRecommendationsBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.alertRecommendationsBackgroundWorker_DoWork);
            this.alertRecommendationsBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.alertRecommendationsBackgroundWorker_RunWorkerCompleted);
            // 
            // configureBaselineButton
            // 
            this.configureBaselineButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.configureBaselineButton.Location = new System.Drawing.Point(319, 534);
            this.configureBaselineButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.configureBaselineButton.Name = "configureBaselineButton";
            this.configureBaselineButton.Size = new System.Drawing.Size(164, 28);
            this.configureBaselineButton.TabIndex = 10;
            this.configureBaselineButton.Text = "Configure Baseline...";
            this.configureBaselineButton.UseVisualStyleBackColor = true;
            this.configureBaselineButton.Click += new System.EventHandler(this.configureBaselineButton_Click);
            // 
            // AlertConfigurationDialog_Fill_Panel
            // 
            this.AlertConfigurationDialog_Fill_Panel.Controls.Add(this.configureBaselineButton);
            this.AlertConfigurationDialog_Fill_Panel.Controls.Add(this.panel3);
            this.AlertConfigurationDialog_Fill_Panel.Controls.Add(this.createTemplateButton);
            this.AlertConfigurationDialog_Fill_Panel.Controls.Add(this.applyTemplateButton);
            this.AlertConfigurationDialog_Fill_Panel.Controls.Add(this.dialogEditor);
            this.AlertConfigurationDialog_Fill_Panel.Controls.Add(this.okButton);
            this.AlertConfigurationDialog_Fill_Panel.Controls.Add(this.applyButton);
            this.AlertConfigurationDialog_Fill_Panel.Controls.Add(this.cancelButton);
            this.AlertConfigurationDialog_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.AlertConfigurationDialog_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AlertConfigurationDialog_Fill_Panel.Location = new System.Drawing.Point(0, 0);
            this.AlertConfigurationDialog_Fill_Panel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.AlertConfigurationDialog_Fill_Panel.Name = "AlertConfigurationDialog_Fill_Panel";
            this.AlertConfigurationDialog_Fill_Panel.Size = new System.Drawing.Size(843, 574);
            this.AlertConfigurationDialog_Fill_Panel.TabIndex = 0;
            // 
            // existingDatabasesBackgroundWorker
            // 
            this.existingDatabasesBackgroundWorker.WorkerReportsProgress = true;
            this.existingDatabasesBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.existingDatabasesBackgroundWorker_DoWork);
            this.existingDatabasesBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.existingDatabasesBackgroundWorker_RunWorkerCompleted);
            // 
            // AlertConfigurationDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(843, 574);
            this.Controls.Add(this.AlertConfigurationDialog_Fill_Panel);
            this.HelpButton = true;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(843, 574);
            this.Name = "AlertConfigurationDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Alert Configuration - {Instance}";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.AlertConfigurationDialog_HelpButtonClicked);
            this.Load += new System.EventHandler(this.AlertConfigurationDialog_Load);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.AlertConfigurationDialog_HelpRequested);
            this.Resize += new System.EventHandler(this.AlertConfigurationDialog_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dialogEditor)).EndInit();
            this.ultraTabPageControl1.ResumeLayout(false);
            this.instanceConfigPanel.ResumeLayout(false);
            this.instanceConfigPanel.PerformLayout();
            this.instanceGridPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.instanceConfigGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraCalcManager1)).EndInit();
            this.instanceButtonPanel.ResumeLayout(false);
            this.configurationLayoutPanel.ResumeLayout(false);
            this.configurationLayoutPanel.PerformLayout();
            this.configurationGridPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.alertConfigurationGrid)).EndInit();
            //10.0 Srishti purohit // For baseline alert
            //Making copy of State grid to give baseline alert functioanlity
            ((System.ComponentModel.ISupportInitialize)(this.baselineAlertConfigurationGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.configBindingSource)).EndInit();
            this.informationLabelPanel.ResumeLayout(false);
            this.informationLabelPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.informationLabelPictureBox)).EndInit();
            this.configurationGaugePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.alertConfigurationGauge)).EndInit();
            this.advancedPanel.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ultraTabPageControl2.ResumeLayout(false);
            this.ultraTabPageControl2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.alertsGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.alertTabControl)).EndInit();
            this.alertTabControl.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.alertRecommendationsPanel.ResumeLayout(false);
            this.alertRecommendationsPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.alertRecommendationsWarningImage)).EndInit();
            this.AlertConfigurationDialog_Fill_Panel.ResumeLayout(false);
            this.AlertConfigurationDialog_Fill_Panel.PerformLayout();
            this.ResumeLayout(false);

        }

       

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton cancelButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton applyButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton okButton;
        private System.Windows.Forms.SplitContainer splitContainer;
        private Infragistics.Win.UltraWinTabControl.UltraTabControl alertTabControl;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage1;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl1;
        private Infragistics.Win.UltraWinGrid.UltraGrid alertsGrid;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel metricNameDescriptionLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox metricNameTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel RankLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox RankTextBox;
        private Infragistics.Win.UltraWinGrid.UltraGrid alertConfigurationGrid;
        //10.0 Srishti purohit // For baseline alert
        //Making copy of State grid to give baseline alert functioanlity
        private Infragistics.Win.UltraWinGrid.UltraGrid baselineAlertConfigurationGrid;
        private System.Windows.Forms.BindingSource configBindingSource;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox descriptionTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox categoryTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label1;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox commentsTextBox;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor dialogEditor;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  configurationLayoutPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  configurationGaugePanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  configurationGridPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  informationLabelPanel;
        private System.Windows.Forms.PictureBox informationLabelPictureBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel informationLabel;
        private ChartFX.WinForms.Gauge.HorizontalGauge alertConfigurationGauge;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox commentsInformationBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  advancedPanel;
        //10.0 srishti purohit -- for baseline alert modifications
        //private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox isBaselineEnabledCheckBox;      // Pruthviraj Nikam: Done changes for 5.1.7 Baseline Alerts     Date: 22-Jan-2019
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton advancedButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton editButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton applyTemplateButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton createTemplateButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox checkBox1; //CustomCheckbox 4.12 DarkTheme Babita Manral
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label3;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel spacelabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel2;
        private Infragistics.Win.UltraWinCalcManager.UltraCalcManager ultraCalcManager1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel3;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  alertRecommendationsPanel;
        private System.Windows.Forms.PictureBox alertRecommendationsWarningImage;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel alertRecommendationsLabel;
        private System.ComponentModel.BackgroundWorker alertRecommendationsBackgroundWorker;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton configureBaselineButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  AlertConfigurationDialog_Fill_Panel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  instanceConfigPanel;
        private Infragistics.Win.UltraWinGrid.UltraGrid instanceConfigGrid;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton btnAddInstance;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  instanceGridPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  instanceButtonPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton btnDeleteInstance;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton btnEditInstance;
        private System.ComponentModel.BackgroundWorker existingDatabasesBackgroundWorker;
        private Infragistics.Win.Appearance appearance18;
        private ChartFX.WinForms.Gauge.LinearScale linearScale1;
    }
}
