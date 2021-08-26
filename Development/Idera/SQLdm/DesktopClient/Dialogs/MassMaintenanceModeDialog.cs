using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BBS.TracerX;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Presenters;
using Idera.SQLdm.DesktopClient.Properties;
using Microsoft.SqlServer.MessageBox;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    internal partial class MassMaintenanceModeDialog : Form
    {
        private class DummyClass
        {
            public string Name { get; set; }
            public string Description { get; set; }
        }

        private DateTime collectionServiceLocalDateTime = DateTime.MinValue;
        private ServerTreeViewPresenter _presenter;
        private bool viewClosing = false;
        private bool _validateConflics = false;
        private static readonly Logger Logger = Logger.GetLogger("MassMaintenanceModeDialog");

        public MassMaintenanceModeDialog()
        {
            InitializeComponent();
            _presenter = new ServerTreeViewPresenter(treeView);
            InitializeTime();
            InitializeComboBox();
        }

        public MassMaintenanceModeDialog(Tag tag)
        {
            InitializeComponent();
            _presenter = new ServerTreeViewPresenter(treeView, tag);
            InitializeTime();
            InitializeComboBox();
        }

        public MassMaintenanceModeDialog(UserView view)
        {
            InitializeComponent();
            _presenter = new ServerTreeViewPresenter(treeView, view);
            InitializeTime();
            InitializeComboBox();
        }

        /// <summary>
        /// Initializes the Current time from the first server
        /// </summary>
        private void InitializeTime()
        {
            mmOnceBeginDate.DateTime = DateTime.Now.Date;
            mmOnceBeginTime.DateTime = DateTime.Now;

            DateTime stopTime = DateTime.Now.Add(TimeSpan.FromHours(1));

            mmOnceStopDate.DateTime = stopTime.Date;
            mmOnceStopTime.DateTime = stopTime;

            serverDateTimeVersionBackgroundWorker.RunWorkerAsync();
            return;
        }

        /// <summary>
        /// Return the selected servers
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, MonitoredSqlServerConfiguration> SelectedServers()
        {
            Dictionary<int, MonitoredSqlServerConfiguration> resut = new Dictionary<int, MonitoredSqlServerConfiguration>();
            foreach (var server in _presenter.SelectedServers)
            {
                MonitoredSqlServerConfiguration configuration = server.Instance.GetConfiguration();

                LoadMaintenanceMode(ref configuration);
                resut.Add(server.Id, configuration);
            }

            return resut;
        }

        private bool ValidateMaintenanceModeSettings()
        {
            if (mmOnceRadio.Checked)
            {
                DateTime start = mmOnceBeginDate.DateTime.Date + mmOnceBeginTime.Time;
                DateTime stop = mmOnceStopDate.DateTime.Date + mmOnceStopTime.Time;

                if (start >= stop)
                {
                    ApplicationMessageBox.ShowInfo(this, "The maintanence mode start time and date must be less than the maintenance mode end time and date.");
                    return false;
                }

                if (collectionServiceLocalDateTime > DateTime.MinValue)
                {
                    if (stop < collectionServiceLocalDateTime)
                    {
                        ApplicationMessageBox.ShowInfo(this,
                                                       "The Maintenance Mode end time must be greater than the collection service current date and time.");
                        return false;
                    }
                }
                else
                {
                    if (stop < DateTime.Now)
                    {
                        ApplicationMessageBox.ShowInfo(this,
                                                       "The Maintenance Mode end time must be greater than the current date and time.");
                        return false;
                    }
                }
            }

            if (mmRecurringRadio.Checked)
            {
                bool dayChecked = false;

                if ((mmBeginSunCheckbox.Checked) ||
                    (mmBeginMonCheckbox.Checked) ||
                    (mmBeginTueCheckbox.Checked) ||
                    (mmBeginWedCheckbox.Checked) ||
                    (mmBeginThurCheckbox.Checked) ||
                    (mmBeginFriCheckbox.Checked) ||
                    (mmBeginSatCheckbox.Checked))
                {
                    dayChecked = true;
                }

                if (dayChecked == false)
                {
                    ApplicationMessageBox.ShowInfo(this, "You must select at least one day for recurring maintenance mode.");
                    return false;
                }

                if ((mmRecurringDuration.Time.Hours == 0) && (mmRecurringDuration.Time.Minutes == 0))
                {
                    ApplicationMessageBox.ShowInfo(this, "The duration for recurring maintenance mode must be greater than zero minutes.");
                    return false;
                }
            }
            if (mmMonthlyRecurringRadio.Checked)
            {
                if ((mmMonthRecurringDuration.Time.Hours == 0) && (mmMonthRecurringDuration.Time.Minutes == 0))
                {
                    ApplicationMessageBox.ShowInfo(this, "The duration for recurring maintenance mode must be greater than zero minutes.");
                    return false;
                }
            }
            return true;
        }

        private bool ValidateServers()
        {
            if (_presenter.SelectedServers.Count <= 0)
            {
                ApplicationMessageBox.ShowInfo(this,
                                               "You must select at least one server to apply the maintenance mode.");
                return false;
            }

            //_validateConflics always false to ignore the conflicts Dialog
            if (_validateConflics)
            {
                IValidate validateConflics = new QuietTimeValidate();

                List<DummyClass> conflicts = new List<DummyClass>();

                foreach (var server in _presenter.SelectedServers)
                {
                    string description;
                    if (!validateConflics.Validate(server.Instance.GetConfiguration(), out description))
                    {
                        conflicts.Add(new DummyClass() {Name = server.InstanceName, Description = description});
                    }
                }

                if (conflicts.Count > 0)
                {
                    MaintenanceModeConflicts dialog = new MaintenanceModeConflicts();

                    List<ConflicViewPresenter.DmGridColumn> columns = new List<ConflicViewPresenter.DmGridColumn>();
                    columns.Add(new ConflicViewPresenter.DmGridColumn() {PropertyName = "Name", Text = "Server Name"});
                    columns.Add(new ConflicViewPresenter.DmGridColumn()
                        {PropertyName = "Description", Text = "Conflict description"});

                    ConflicViewPresenter conflicViewPresenter = new ConflicViewPresenter(dialog.GridView, columns);
                    conflicViewPresenter.UpdateData(conflicts);

                    DialogResult result = dialog.ShowDialog(this);

                    if (result == DialogResult.Cancel)
                    {
                        return false;
                    }
                }
            }
            else
            {
                DialogResult result = ApplicationMessageBox.ShowWarning
                    (this,
                     "The current maintenance mode setting will overwrite any existing maintenance schedule preferences in SQLdm. Do you wish to continue?",
                     ExceptionMessageBoxButtons.YesNo);

                if (result == DialogResult.Cancel || result == DialogResult.No)
                {
                    return false;
                }
            }

            return true;
        }


        private void LoadMaintenanceMode(ref MonitoredSqlServerConfiguration newConfiguration)
        {
            //add maintenance mode info
            newConfiguration.MaintenanceMode.MaintenanceModeStart = mmOnceBeginDate.DateTime.Date + mmOnceBeginTime.Time;
            newConfiguration.MaintenanceMode.MaintenanceModeStop = mmOnceStopDate.DateTime.Date + mmOnceStopTime.Time;

            short mmDays = 0;
            if (mmBeginSunCheckbox.Checked)
                mmDays |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Sunday);
            if (mmBeginMonCheckbox.Checked)
                mmDays |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Monday);
            if (mmBeginTueCheckbox.Checked)
                mmDays |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Tuesday);
            if (mmBeginWedCheckbox.Checked)
                mmDays |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Wednesday);
            if (mmBeginThurCheckbox.Checked)
                mmDays |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Thursday);
            if (mmBeginFriCheckbox.Checked)
                mmDays |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Friday);
            if (mmBeginSatCheckbox.Checked)
                mmDays |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Saturday);

            int mmMonth = 0;
            if (mmMonthlyDayRadio.Checked)
                mmMonth = (int)inputOfEveryMonthLimiter.Value;
            else
                mmMonth = (int)inputOfEveryTheMonthLimiter.Value;

            int maintenanceModeSpecificDay = 0;
            if (mmMonthlyRecurringRadio.Checked)
                if (mmMonthlyDayRadio.Checked)
                    maintenanceModeSpecificDay = (int)inputDayLimiter.Value;
                else
                    maintenanceModeSpecificDay = 0;
            else
                maintenanceModeSpecificDay = 0;

            int maintenanceModeWeekOrdinal = 0;
            if (mmMonthlyRecurringRadio.Checked)
                if (mmMonthlyTheRadio.Checked)
                    maintenanceModeWeekOrdinal = (int)WeekcomboBox.SelectedValue;
                else
                    maintenanceModeWeekOrdinal = 0;
            else
                maintenanceModeWeekOrdinal = 0;

            int maintenanceModeWeekDay = 0;
            if (mmMonthlyRecurringRadio.Checked)
                if (mmMonthlyTheRadio.Checked)
                    maintenanceModeWeekDay = (int)DaycomboBox.SelectedValue;
                else
                    maintenanceModeWeekDay = 0;
            else
                maintenanceModeWeekDay = 0;

            newConfiguration.MaintenanceMode.MaintenanceModeDays = mmDays;
            newConfiguration.MaintenanceMode.MaintenanceModeRecurringStart = mmRecurringBegin.DateTime;
            newConfiguration.MaintenanceMode.MaintenanceModeDuration = mmRecurringDuration.Time;
            newConfiguration.MaintenanceMode.MaintenanceModeMonth = mmMonth;
            newConfiguration.MaintenanceMode.MaintenanceModeSpecificDay = maintenanceModeSpecificDay;
            newConfiguration.MaintenanceMode.MaintenanceModeWeekOrdinal = maintenanceModeWeekOrdinal;
            newConfiguration.MaintenanceMode.MaintenanceModeWeekDay = maintenanceModeWeekDay;

            DateTime mmModeStartTime = new DateTime(1900, 1, 1,
                                                                                 mmMonthRecurringBegin.DateTime.Hour,
                                                                                 mmMonthRecurringBegin.DateTime.Minute,
                                                                                mmMonthRecurringBegin.DateTime.Second);
            newConfiguration.MaintenanceMode.MaintenanceModeMonthRecurringStart = mmModeStartTime;

            newConfiguration.MaintenanceMode.MaintenanceModeMonthDuration = mmMonthRecurringDuration.Time;

            if (mmNeverRadio.Checked)
            {
                newConfiguration.MaintenanceMode.MaintenanceModeType = MaintenanceModeType.Never;
                newConfiguration.MaintenanceModeEnabled = false;
            }
            else if (mmOnceRadio.Checked)
            {
                newConfiguration.MaintenanceMode.MaintenanceModeType = MaintenanceModeType.Once;

                //do not set the maintenance mode flag if it is not time for maintenance mode yet.
                if (DateTime.Now < newConfiguration.MaintenanceMode.MaintenanceModeStart)
                {
                    newConfiguration.MaintenanceModeEnabled = false;
                }
                else
                {
                    newConfiguration.MaintenanceModeEnabled = true;
                }

            }
            else if (mmRecurringRadio.Checked)
            {
                newConfiguration.MaintenanceMode.MaintenanceModeType = MaintenanceModeType.Recurring;

                bool mmModeForToday = false;


                //See if today is of of the day for recurring maintenance mode.
                foreach (int val in Enum.GetValues(typeof(DayOfWeek)))
                {
                    if (MonitoredSqlServer.MatchDayOfWeek((DayOfWeek)val,
                                                          newConfiguration.MaintenanceMode.MaintenanceModeDays))
                    {
                        if (val == (int)DateTime.Now.DayOfWeek)
                        {
                            mmModeForToday = true;
                            break;
                        }
                    }
                }

                //do not set the maintenance mode flag if it is not time for maintenance mode yet.
                if (mmModeForToday &&
                    (DateTime.Now.TimeOfDay >=
                     newConfiguration.MaintenanceMode.MaintenanceModeRecurringStart.Value.TimeOfDay) &&
                    (DateTime.Now.TimeOfDay <
                     (newConfiguration.MaintenanceMode.MaintenanceModeRecurringStart.Value.TimeOfDay +
                      newConfiguration.MaintenanceMode.MaintenanceModeDuration)))
                {
                    newConfiguration.MaintenanceModeEnabled = true;
                }
                else
                {
                    newConfiguration.MaintenanceModeEnabled = false;
                }
            }
            else if (mmMonthlyRecurringRadio.Checked)
            {
                newConfiguration.MaintenanceMode.MaintenanceModeType = MaintenanceModeType.Monthly;
                bool mmModeForToday = false;

                DateTime dt = DateTime.Now;
                int currentMonth = dt.Month;
                int currentDay = dt.Day;
                int selectedWeek = currentDay / 7;
                int remForselectedWeek = currentDay % 7;
                if (remForselectedWeek > 0)
                    selectedWeek = selectedWeek + 1;

                // if the current month is devisible by mmMonth, then we can say that Maintenance is scheduled for current month.
                if (currentMonth % mmMonth == 0)
                {
                    if (mmMonthlyDayRadio.Checked)
                    {
                        if (currentDay.ToString().Equals(inputDayLimiter.Value.ToString()))
                        {
                            mmModeForToday = true;
                        }
                    }
                    else
                    {
                        if ((selectedWeek == (int)WeekcomboBox.SelectedValue) && ((int)dt.DayOfWeek == (int)WeekcomboBox.SelectedValue))
                        {
                            mmModeForToday = true;
                        }
                    }
                }

                //do not set the maintenance mode flag if it is not time for maintenance mode yet.
                if (mmModeForToday &&
                    (DateTime.Now.TimeOfDay >= newConfiguration.MaintenanceMode.MaintenanceModeMonthRecurringStart.Value.TimeOfDay) &&
                                                (DateTime.Now.TimeOfDay <
                                                 (newConfiguration.MaintenanceMode.MaintenanceModeMonthRecurringStart.Value.TimeOfDay +
                                                  newConfiguration.MaintenanceMode.MaintenanceModeMonthDuration)))
                {
                    newConfiguration.MaintenanceModeEnabled = true;
                }
                else
                {
                    newConfiguration.MaintenanceModeEnabled = false;
                }
            }
            else if (mmAlwaysRadio.Checked)
            {
                newConfiguration.MaintenanceMode.MaintenanceModeType = MaintenanceModeType.Always;
                newConfiguration.MaintenanceModeEnabled = true;
            }
            else
            {
                //We should never get here but it is included to be through
                newConfiguration.MaintenanceMode.MaintenanceModeType = MaintenanceModeType.Never;
                newConfiguration.MaintenanceModeEnabled = false;
            }
        }


        private void mmNeverRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (mmNeverRadio.Checked)
            {
                mmMonthlyRecurringPanel.Visible = false;
                mmRecurringPanel.Visible = false;
                mmOncePanel.Visible = false;
            }
        }

        private void mmAlwaysRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (mmAlwaysRadio.Checked)
            {
                mmMonthlyRecurringPanel.Visible = false;
                mmRecurringPanel.Visible = false;
                mmOncePanel.Visible = false;
            }
        }

        private void mmWeeklyRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (mmRecurringRadio.Checked)
            {
                mmRecurringPanel.Visible = true;
                mmOncePanel.Visible = false;
                mmMonthlyRecurringPanel.Visible = false;
            }
        }

        private void mmOnceRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (mmOnceRadio.Checked)
            {
                mmRecurringPanel.Visible = false;
                mmOncePanel.Visible = true;
                mmMonthlyRecurringPanel.Visible = false;
            }
        }

        private void mmMonthlyRecurringRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (mmMonthlyRecurringRadio.Checked)
            {
                mmMonthlyRecurringPanel.Visible = true;
                mmMonthlyDayRadio.Checked = true;
                mmRecurringPanel.Visible = false;
                mmOncePanel.Visible = false;
            }
        }

        private void mmMonthlyDayRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (mmMonthlyDayRadio.Checked)
            {
                WeekcomboBox.Enabled = false;
                DaycomboBox.Enabled = false;
                inputOfEveryTheMonthLimiter.Enabled = false;
                inputDayLimiter.Enabled = true;
                inputOfEveryMonthLimiter.Enabled = true;
            }
        }
        void mmMonthlyTheRadio_CheckedChanged(object sender, System.EventArgs e)
        {
            if (mmMonthlyTheRadio.Checked)
            {
                inputDayLimiter.Enabled = false;
                inputOfEveryMonthLimiter.Enabled = false;
                WeekcomboBox.Enabled = true;
                DaycomboBox.Enabled = true;
                inputOfEveryTheMonthLimiter.Enabled = true;

            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            bool result = ValidateMaintenanceModeSettings();
            if (result)
            {
                DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void serverDateTimeVersionBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var servers = _presenter.Servers;

            if (servers.Count <= 0)
            {
                return;
            }

            foreach (var server in servers)
            {
                try
                {
                    IManagementService defaultManagementService =
                        ManagementServiceHelper.GetDefaultService(
                            Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
                    e.Result = defaultManagementService.GetServerTimeAndVersion(server.Key);
                    break;
                }
                catch (Exception ex)
                {
                    Logger.Info(string.Format("The server:{0} not response to time request.", server.Value.InstanceName));
                    Logger.Verbose("Exeption occured:", ex);
                    e.Result = null;
                }
            }
            
            BackgroundWorker backgroundWorker = sender as BackgroundWorker;

            if (backgroundWorker != null && backgroundWorker.CancellationPending)
            {               
                e.Cancel = true;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            viewClosing = true;

            if (serverDateTimeVersionBackgroundWorker != null)
            {
                serverDateTimeVersionBackgroundWorker.CancelAsync();
            }

            base.OnClosing(e);
        }

        private void serverDateTimeVersionBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (viewClosing)
                return;

            if (!e.Cancelled)
            {
                if (e.Error == null && e.Result != null)
                {
                    Wintellect.PowerCollections.Triple<ServerVersion, DateTime, DateTime> result = (Wintellect.PowerCollections.Triple<ServerVersion, DateTime, DateTime>)e.Result;

                    if (result != null)
                    {
                        collectionServiceLocalDateTime = result.Third;
                    }
                }

                if (collectionServiceLocalDateTime > DateTime.MinValue)
                {
                    mmServerDateTime.Text = collectionServiceLocalDateTime.ToShortDateString() + ' ' + collectionServiceLocalDateTime.ToShortTimeString();
                }
                else
                {
                    mmServerDateTime.Text = "Unknown";
                }
            }
        }

        private void InitializeComboBox()
        {
            DataTable dt1 = new DataTable();
            DataColumn displayColumn1 = new DataColumn("Name", Type.GetType("System.String"));
            DataColumn valueColumn1 = new DataColumn("Value", Type.GetType("System.Int32"));
            dt1.Columns.Add(displayColumn1);
            dt1.Columns.Add(valueColumn1);

            for (int i = 1; i <= 4; i++)
            {
                DataRow row = dt1.NewRow();

                if (i == 1)
                    row["Name"] = "First";
                if (i == 2)
                    row["Name"] = "Second";
                if (i == 3)
                    row["Name"] = "Third";
                if (i == 4)
                    row["Name"] = "Fourth";

                row["Value"] = i;
                dt1.Rows.Add(row);
            }
            WeekcomboBox.DropDownStyle = ComboBoxStyle.DropDownList;//SQLdm 8.5 (Gaurav Karwal): added to make sure we cannot edit the combo box on the UI
            WeekcomboBox.BindingContext = new BindingContext();
            WeekcomboBox.DataSource = dt1;
            WeekcomboBox.DisplayMember = "Name";
            WeekcomboBox.ValueMember = "Value";
            WeekcomboBox.SelectedValue = 1;
            

            DataTable dt2 = new DataTable();
            DataColumn displayColumn2 = new DataColumn("Name", Type.GetType("System.String"));
            DataColumn valueColumn2 = new DataColumn("Value", Type.GetType("System.Int32"));
            dt2.Columns.Add(displayColumn2);
            dt2.Columns.Add(valueColumn2);

            for (int i = 1; i <= 7; i++)
            {
                DataRow row = dt2.NewRow();

                if (i == 1)
                    row["Name"] = "Sunday";
                if (i == 2)
                    row["Name"] = "Monday";
                if (i == 3)
                    row["Name"] = "Tuesday";
                if (i == 4)
                    row["Name"] = "Wednesday";
                if (i == 5)
                    row["Name"] = "Thursday";
                if (i == 6)
                    row["Name"] = "Friday";
                if (i == 7)
                    row["Name"] = "Saturday";

                row["Value"] = i;
                dt2.Rows.Add(row);
            }
            DaycomboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            DaycomboBox.BindingContext = new BindingContext();
            DaycomboBox.DataSource = dt2;
            DaycomboBox.DisplayMember = "Name";
            DaycomboBox.ValueMember = "Value";
            DaycomboBox.SelectedValue = 1;

        }
     
    }
}
