using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    using System.Reflection;
    using Idera.SQLdm.Common;
    using Idera.SQLdm.Common.Objects.ApplicationSecurity;
    using Idera.SQLdm.Common.Thresholds;
    using Infragistics.Win;
    using Infragistics.Win.UltraWinGrid;
    using Idera.SQLdm.DesktopClient.Helpers;
    using Idera.SQLdm.Common.Configuration;

    public partial class AlertEnumeratedValuesConfigDialog : Form
    {
        private AlertConfigurationItem configItem;
        private bool readOnly;

        public AlertEnumeratedValuesConfigDialog()
        {
            InitializeComponent();

            ConfigureGrid();
            AdaptFontSize();
        }

        private void ConfigureGrid()
        {
            grid.DrawFilter = new HideFocusRectangleDrawFilter();

            EditorWithText editor = new EditorWithText();
            grid.DisplayLayout.Bands[0].Columns["OK"].Editor = editor;
            grid.DisplayLayout.Bands[0].Columns["Info"].Editor = editor;
            grid.DisplayLayout.Bands[0].Columns["Warning"].Editor = editor;
            grid.DisplayLayout.Bands[0].Columns["Critical"].Editor = editor;
        }

        public void SetThresholdEntry(AlertConfigurationItem item)
        {
            this.configItem = item;

            int id = item.ThresholdEntry.MonitoredServerID;
            readOnly = id == -1 || !(ApplicationModel.Default.UserToken.GetServerPermission(id) >= PermissionType.Modify);
            okButton.DialogResult = readOnly ? DialogResult.Cancel : DialogResult.OK;

            MetricThresholdEntry entry = item.ThresholdEntry;
            List<object> infoItems = new List<object>(entry.InfoThreshold.Value as Threshold.ComparableList);
            List<object> warningItems = new List<object>(entry.WarningThreshold.Value as Threshold.ComparableList);
            List<object> criticalItems = new List<object>(entry.CriticalThreshold.Value as Threshold.ComparableList);
            SetValues(item.GetPossibleValues<object>(), warningItems, criticalItems, infoItems);
        }

        private void SetValues(List<object> all, List<object> warning, List<object> critical, List<object> info)
        {
            bindingSource.Clear();
            foreach (object o in all)
            {
                MonitoredState severity = MonitoredState.OK;
                if (critical.Contains(o))
                    severity = MonitoredState.Critical;
                else
                    if (warning.Contains(o))
                        severity = MonitoredState.Warning;
                    else
                        if (info.Contains(o))
                            severity = MonitoredState.Informational;

                EnumeratedValue value = new EnumeratedValue(o, ApplicationHelper.GetEnumDescription(o), severity);
                bindingSource.Add(value);
            }            
        }

        private void grid_MouseClick(object sender, MouseEventArgs e)
        {
            if (readOnly)
                return;

            UIElement selectedElement = grid.DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));

            if (selectedElement is EditorWithTextDisplayTextUIElement)
                selectedElement = selectedElement.Parent;
            else
            if (!(selectedElement is ImageUIElement))
                return;

            // logic to handle toggling a checkbox in a non-editable (no cell selection) column
            object contextObject = selectedElement.GetContext();
            if (contextObject is Infragistics.Win.UltraWinGrid.UltraGridColumn)
            {
                string column = ((UltraGridColumn) contextObject).Key;
                switch(column)
                {
                    case "OK":
                    case "Info":
                    case "Warning":
                    case "Critical":
                        break;
                    default:
                        return;
                }

                UltraGridRow selectedRow = selectedElement.SelectableItem is UltraGridCell ? ((UltraGridCell)selectedElement.SelectableItem).Row  : selectedElement.SelectableItem as UltraGridRow;
                if (selectedRow != null)
                {
                    CurrencyManager cm = ((ICurrencyManagerProvider) grid.DataSource).CurrencyManager;
                    PropertyDescriptor descriptor = cm.GetItemProperties()[column];
                    object selectedObject = selectedRow.ListObject;
                    if (selectedObject is EnumeratedValue)
                    {
                        object value = descriptor.GetValue(selectedRow.ListObject);
                        if (value is bool && !true.Equals(value))
                        {
                            descriptor.SetValue(selectedObject, true);
                        }
                    }
                }
            }
            configItem.RaisePropertyChanged("Value");
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (readOnly)
                return;

            MetricThresholdEntry entry = configItem.ThresholdEntry;
            List<object> infoItems = entry.InfoThreshold.Value as List<object>;
            List<object> warningItems = entry.WarningThreshold.Value as List<object>;
            List<object> criticalItems = entry.CriticalThreshold.Value as List<object>;
            List<object> okItems = configItem.GetThreshold(ThresholdItemType.OK, false).Value as List<object>;

            infoItems.Clear();
            warningItems.Clear();
            criticalItems.Clear();
            okItems.Clear();
            foreach (EnumeratedValue value in bindingSource)
            {
                if (value.Info)
                    infoItems.Add(value.Value);
                else
                    if (value.Warning)
                        warningItems.Add(value.Value);
                    else
                        if (value.Critical)
                            criticalItems.Add(value.Value);
                        else
                            okItems.Add(value.Value);
            }
        }

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }
    }

    internal class EnumeratedValue
    {
        private object value;
        private string name;
        private MonitoredState state;

        public EnumeratedValue(object value, string name, MonitoredState state)
        {
            this.value = value;
            this.name = name;
            this.state = state;
        }

        public object Value
        {
            get 
            {
                return value; 
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public MonitoredState Severity
        {
            get { return state;  }
            set { state = value; }
        }

        public bool OK
        {
            get { return state == MonitoredState.OK; }
            set
            {
                if (!value)
                    throw new ArgumentOutOfRangeException("OK", "True is the only acceptable value");
                state = MonitoredState.OK;
            }
        }

        public bool Info
        {
            get { return state == MonitoredState.Informational; }
            set
            {
                if (!value)
                    throw new ArgumentOutOfRangeException("Info", "True is the only acceptable value");
                state = MonitoredState.Informational;
            }
        }

        public bool Warning
        {
            get { return state == MonitoredState.Warning; }
            set
            {
                if (!value)
                    throw new ArgumentOutOfRangeException("Warning", "True is the only acceptable value");
                state = MonitoredState.Warning;
            }
        }
        public bool Critical
        {
            get { return state == MonitoredState.Critical; }
            set
            {
                if (!value)
                    throw new ArgumentOutOfRangeException("Critical", "True is the only acceptable value");
                state = MonitoredState.Critical;
            }
        }
    }
}