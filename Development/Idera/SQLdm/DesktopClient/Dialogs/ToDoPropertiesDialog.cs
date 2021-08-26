using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Diagnostics;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Events;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    public partial class ToDoPropertiesDialog : Form
    {
        #region constants

        private const string ColTaskID = "TaskID";
        private const string ColStatus = "Status";
        private const string ColStatusIcon = "StatusIcon";
        private const string ColCompletedCheck = "CompletedCheck";
        private const string ColSubject = "Subject";
        private const string ColMessage = "Message";
        private const string ColServerName = "ServerName";
        private const string ColComments = "Comments";
        private const string ColOwner = "Owner";
        private const string ColCreatedOn = "CreatedOn";
        private const string ColCompletedOn = "CompletedOn";
        private const string ColMetric = "Metric";
        private const string ColSeverity = "Severity";
        private const string ColSeverityIcon = "SevIcon";
        private const string ColValue = "Value";
        private const string ColEventID = "EventID";

        #endregion

        #region members

        private UltraGridRow m_ToDoItemRow;
        ValueList m_MetricValues; 

        #endregion

        #region ctors

        public ToDoPropertiesDialog(UltraGridRow row)
        {
            InitializeComponent();

            m_ToDoItemRow = row;
            m_MetricValues = new ValueList();

            MinimumSize = Size; // set minimum size
            AdaptFontSize();
        }

        #endregion

        #region properties

        public int TaskId
        {
            get { return (int)m_ToDoItemRow.Cells[ColTaskID].Value; }
        }

        public TaskStatus TaskStatus
        {
            get 
            {
                switch (_statusCombo.SelectedIndex)
                {
                    case 1:
                        return TaskStatus.InProgress;
                    case 2:
                        return TaskStatus.OnHold;
                    case 3:
                        return TaskStatus.Completed;
                    case 0:
                    default:
                        return TaskStatus.NotStarted;
                }
            }
        }

        public string TaskOwner
        {
            get { return _txtbx_Owner.Text; }
        }

        public string TaskComments
        {
            get { return _txtbx_Comments.Text; }
        }

        #endregion

        #region methods

        private void ToDoPropertiesDialog_Load(object sender, EventArgs e)
        {
            // Load the metric names.
            m_MetricValues.ValueListItems.AddRange(ValueListHelpers.GetMetricValueListItems());

            // Set header information.
            setHeaderSeverity(m_ToDoItemRow.Cells[ColSeverity].Value as byte?);
            _propertyPage.Text = m_ToDoItemRow.Cells[ColSubject].Value as string;

            // Set alert information.
            _lbl_CreatedOn.Text = ((DateTime)m_ToDoItemRow.Cells[ColCreatedOn].Value).ToString();
            _lbl_Server.Text = m_ToDoItemRow.Cells[ColServerName].Value as string;

            // Set alert details.
            setMetric((m_ToDoItemRow.Cells[ColMetric].Value as int?), (m_ToDoItemRow.Cells[ColValue].Value as double?));
            _txtbx_Message.Text = m_ToDoItemRow.Cells[ColMessage].Value as string;

            // Set status
            setTaskStatus((TaskStatus)m_ToDoItemRow.Cells[ColStatus].Value);
            setCompletedOn((m_ToDoItemRow.Cells[ColCompletedOn].Value as DateTime?));
            _txtbx_Owner.Text = m_ToDoItemRow.Cells[ColOwner].Value as string;
            _txtbx_Comments.Text = m_ToDoItemRow.Cells[ColComments].Value as string;

            //int taskId = (int)m_ToDoItemRow.Cells[ColTaskID].Value;
            //_lbl_Subject.Text = m_ToDoItemRow.Cells[ColSubject].Value as string;
            //int? eventId = m_ToDoItemRow.Cells[ColEventID].Value as int?;
            _btn_Cancel.Focus();
        }

        private void setHeaderSeverity(byte? msVal)
        {
            MonitoredState ms = (msVal == null) ? MonitoredState.OK : ((MonitoredState)msVal);
            switch (ms)
            {
                case MonitoredState.Warning:
                    _propertyPage.HeaderImage = Properties.Resources.Warning32x32;
                    break;
                case MonitoredState.Critical:
                    _propertyPage.HeaderImage = Properties.Resources.Critical32x32;
                    break;
                case MonitoredState.Informational:
                    _propertyPage.HeaderImage = Properties.Resources.Information32x32;
                    break;
                case MonitoredState.OK:
                default:
                    _propertyPage.HeaderImage = Properties.Resources.OK32x32;
                    break;
            }
        }

        private void setMetric(int? metric, double? metricVal)
        {
            _lbl_Metric.Text = (metric == null) ? string.Empty : (m_MetricValues.FindByDataValue(metric).DisplayText);
            //_lbl_MetricValue.Text = (metricVal == null) ? string.Empty : ((float)metricVal).ToString();
        }

        private void setTaskStatus(TaskStatus ts)
        {
            switch (ts)
            {
                case TaskStatus.NotStarted:
                    _statusCombo.SelectedItem = _statusCombo.Items[0];
                    break;
                case TaskStatus.InProgress:
                    _statusCombo.SelectedItem = _statusCombo.Items[1];
                    break;
                case TaskStatus.OnHold:
                    _statusCombo.SelectedItem = _statusCombo.Items[2];
                    break;
                case TaskStatus.Completed:
                    _statusCombo.SelectedItem = _statusCombo.Items[3];
                    break;
                default:
                    _statusCombo.SelectedItem = _statusCombo.Items[0];
                    break;
            }
        }

        private void setCompletedOn(DateTime? dtVal)
        {
            _lbl_CompletedOn.Text = (dtVal == null) ? "Not completed" : dtVal.ToString();
        }

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }
        #endregion
    }
}