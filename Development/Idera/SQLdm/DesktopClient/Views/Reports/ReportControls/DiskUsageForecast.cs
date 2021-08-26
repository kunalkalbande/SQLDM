using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using Idera.SQLdm.Common.Objects;
using BBS.TracerX;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Infragistics.Win;
using Microsoft.Reporting.WinForms;
using Idera.SQLdm.DesktopClient.Helpers.MathLibrary;
using Idera.SQLdm.DesktopClient.Controls;

namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    public partial class DiskUsageForecast : ReportContol
    {
        private const string NoDrivesAvailableText = "< No Drives Available >";
        private static readonly Logger Log = Logger.GetLogger("DiskUsageForecast");
        private const string DrivesTag = "< Select a Drive >";
        private List<ReportParameter> passthroughParameters = new List<ReportParameter>(7);

        private WorkerData localReportData = null;
        private int orderPolynomial = 3;
        protected enum ForecastType { Linear = 1, Parabolic, Cubic, Quartic };

        readonly ValueListItem forecastLinear = new ValueListItem(ForecastType.Linear, "Linear");
        readonly ValueListItem forecastParabola = new ValueListItem(ForecastType.Parabolic, "Exponential (Aggressive)");
        readonly ValueListItem forecastCubic = new ValueListItem(ForecastType.Cubic, "Cubic");
        readonly ValueListItem forecastQuartic = new ValueListItem(ForecastType.Quartic, "Quartic");

        public DiskUsageForecast()
        {
            InitializeComponent();
            AdaptFontSize();
            showTablesCheckbox.Checked = true;
        }

        public override void InitReport()
        {
            base.InitReport();
            InitializeReportViewer();
            InitializeForecastType();
            State = UIState.ParmsNeeded;
            periodCombo.SelectedItem = period7;
            periodCombo.Items.Remove(periodToday);
            sampleSizeCombo.Items.Remove(sampleHours);
            sampleSizeCombo.SelectedItem = sampleDays;
            driveNamesCombo.Enabled = false;
            driveNamesCombo.Text = string.Empty;
            //driveNamesCombo.Items.Add(DrivesTag);
            //driveNamesCombo.SelectedIndex = 0;
            ReportType = ReportTypes.DiskSpaceForecast;
        }
        
        public override bool CanRunReport(out string message)
        {
            message = String.Empty;

            if (instanceCombo.SelectedIndex == 0)
            {
                message = "A SQL Server instance must be selected to generate this report.";
                return false;
            }
            else if (string.IsNullOrEmpty(driveNamesCombo.Text))
            {
                message = "No drive information is available for the selected SQL Server. Select a different server to run this report.";
                return false;
            }

            return true;
        }

        private void InitializeForecastType()
        {
            forecastTypeCombo.Items.Add(forecastLinear);
            forecastTypeCombo.Items.Add(forecastParabola);
            //forecastTypeCombo.Items.Add(forecastCubic);
            //forecastTypeCombo.Items.Add(forecastQuartic);
            forecastTypeCombo.SelectedItem = forecastLinear;
        }

        protected override void SetReportParameters()
        {
            base.SetReportParameters();

            if (forecastTypeCombo.SelectedItem != null)
            {
                reportParameters.Add("ForecastType", forecastTypeCombo.SelectedItem);
            }
            var selectedDrive = driveNamesCombo.Text;           
            reportParameters.Add("DriveName", string.IsNullOrEmpty(selectedDrive) ? null : selectedDrive);
            reportParameters.Add("GUIServer", instanceCombo.SelectedItem == null ? "" : instanceCombo.SelectedItem.ToString());
            reportParameters.Add("GUIDateRange", this.GetDateRange(reportData.dateRanges[0].UtcStart, reportData.dateRanges[0].UtcEnd));
            reportParameters.Add("GUIDrive", string.IsNullOrEmpty(selectedDrive) ? "" : selectedDrive);
            reportParameters.Add("GUIForecastUnits", forecastUnits.Text);
            reportParameters.Add("GUIForecastType", forecastTypeCombo.SelectedItem == null ? "" : ((ValueListItem)forecastTypeCombo.SelectedItem).DisplayText);
            reportParameters.Add("ShowAvailable", chkShowAvailable.Checked.ToString());
            reportParameters.Add("DisplayTables", showTablesCheckbox.Checked.ToString());
        }

        override protected void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (System.Threading.Thread.CurrentThread.Name == null) System.Threading.Thread.CurrentThread.Name = "ReportControlWorker";

            using (Log.DebugCall())
            {
                localReportData = (WorkerData)e.Argument;

                passthroughParameters.Clear();
                passthroughParameters.Add(new ReportParameter("executionStart", DateTime.Now.ToString()));
                passthroughParameters.Add(new ReportParameter("Interval", ((int)reportData.sampleSize).ToString()));

                passthroughParameters.Add(new ReportParameter("GUIServer", localReportData.reportParameters["GUIServer"].ToString()));
                passthroughParameters.Add(new ReportParameter("GUIDateRange", localReportData.reportParameters["GUIDateRange"].ToString()));
                passthroughParameters.Add(new ReportParameter("GUIDrive", localReportData.reportParameters["GUIDrive"].ToString()));
                passthroughParameters.Add(new ReportParameter("GUIForecastUnits", localReportData.reportParameters["GUIForecastUnits"].ToString()));
                passthroughParameters.Add(new ReportParameter("GUIForecastType", localReportData.reportParameters["GUIForecastType"].ToString()));
                passthroughParameters.Add(new ReportParameter("ShowAvailable", localReportData.reportParameters["ShowAvailable"].ToString()));
                passthroughParameters.Add(new ReportParameter("DisplayTables", (string)localReportData.reportParameters["DisplayTables"]));
                Log.Debug("localReportData.reportType = ", localReportData.reportType);
                localReportData.dataSources = new ReportDataSource[1];

                ReportDataSource dataSource = new ReportDataSource("DiskUsage");
                
                DataTable diskUsage = RepositoryHelper.GetReportData("p_DiskUsageForecast",
                                                                  localReportData.instanceID,
                                                                  localReportData.dateRanges[0].UtcStart,
                                                                  localReportData.dateRanges[0].UtcEnd,
                                                                  localReportData.dateRanges[0].UtcOffsetMinutes.ToString(),
                                                                  (int)localReportData.sampleSize,
                                                                  localReportData.reportParameters["DriveName"]);

                int intRowsOfSampledData = diskUsage.Rows.Count;

                passthroughParameters.Add(new ReportParameter("GUISampleCount", intRowsOfSampledData.ToString()));
                
                //Stopwatch time = new Stopwatch();
                //time.Start();
                
                //Using the new refactored routine is 35% faster
                ForecastDiskUsage1(ref diskUsage);
                //Console.WriteLine(time.ElapsedTicks);

                int intRowsOfForecastData = (int)(diskUsage.Rows.Count - intRowsOfSampledData);
                passthroughParameters.Add(new ReportParameter("GUIDriveCount", (intRowsOfForecastData/int.Parse(forecastUnits.Text)).ToString()));

                dataSource.Value = diskUsage;
                localReportData.dataSources[0] = dataSource;

                if (localReportData.cancelled)
                {
                    Log.Debug("reportData.cancelled = true.");
                    e.Cancel = true;
                }
                else
                {
                    e.Result = localReportData;
                }
            }
        }

        override protected void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            using (Log.DebugCall())
            {
                // Make sure this call is for the most recently requested report.
                Log.Debug("(reportData.bgWorker == sender) = ", reportData.bgWorker == sender);
                if (reportData.bgWorker == sender)
                {
                    // This event handler was called by the currently active report
                    if (reportData.cancelled)
                    {
                        Log.Debug("reportData.cancelled = true.");
                        return;
                    }
                    else if (e.Error != null)
                    {
                        if (e.Error.GetType() == typeof(System.Data.SqlClient.SqlException) &&
                            e.Error.Message.ToLower().Contains("msxmlsql.dll"))//
                        {
                            ApplicationMessageBox msgbox1 = new ApplicationMessageBox();
                            Exception msg = new Exception("An error occurred while retrieving data for the report.  It may be due to the problem described by the article available at http://support.microsoft.com/Default.aspx?kbid=918767", e.Error);
                            Log.Error("Showing message box: ", msg);
                            msgbox1.Message = msg;
                            msgbox1.SetCustomButtons("OK", "View Article");
                            msgbox1.Symbol = Microsoft.SqlServer.MessageBox.ExceptionMessageBoxSymbol.Error;
                            msgbox1.Show(this);
                            if (msgbox1.CustomDialogResult == Microsoft.SqlServer.MessageBox.ExceptionMessageBoxDialogResult.Button2)
                            {
                                Process.Start("http://support.microsoft.com/Default.aspx?kbid=918767");
                            }
                        }
                        else
                        {
                            ApplicationMessageBox.ShowError(this, "An error occurred while retrieving data for the report.  ",
                                            e.Error);
                        }

                        State = UIState.NoDataAcquired;
                    }
                    else
                    {
                        try
                        {
                            reportViewer.Reset();
                            reportViewer.LocalReport.EnableHyperlinks = true;

                            using (Stream stream = GetType().Assembly.GetManifestResourceStream(
                                "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.DiskUsageForecast.rdl"))
                            {
                                reportViewer.LocalReport.LoadReportDefinition(stream);
                            }
                            foreach (ReportDataSource dataSource in reportData.dataSources)
                            {
                                reportViewer.LocalReport.DataSources.Add(dataSource);
                            }

                            reportViewer.LocalReport.SetParameters(passthroughParameters);

                            reportViewer.RefreshReport();
                            reportViewer.LocalReport.DisplayName = "DiskUsageForecast";
                            State = UIState.Rendered;
                        }
                        catch (Exception exception)
                        {
                            ApplicationMessageBox.ShowError(ParentForm, "An error occurred while refreshing the report.", exception);
                            State = UIState.ParmsNeeded;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Calculate the forecast by iterating through the disk usage data set and then populating the disk usage data set
        /// to be passed to the report as a data source with the additional forcast data
        /// </summary>
        /// <param name="diskUsage"></param>
        private void ForecastDiskUsage(ref DataTable diskUsage)
        {
            //Arraylist containing the total disk used
            ArrayList diskSizeUsed = new ArrayList();
            ArrayList diskSizeTotal = new ArrayList();
            //Hashtable containing the drive letter and an array of sizes from different sample occurences
            Hashtable diskSizeValues = new Hashtable();
            Hashtable diskSizeValueCount = new Hashtable();
            Hashtable diskSizeTotalAvailable = new Hashtable();
            Hashtable diskAverageAvailable = new Hashtable();
            Hashtable diskSizeCoefficients = new Hashtable();

            string lastDrive;
            int index = 0;
            DateTime lastDate;

            // Bail out if there was no data returned by the query
            if (diskUsage.Rows.Count == 0) return;

            //save the name of the last drive
            lastDrive = (string)diskUsage.Rows[0]["DriveName"];

            //get the inital data in arrays for calculations. Sorted by drive name.
            foreach (DataRow row in diskUsage.Rows)
            {
                object value = row["DriveName"];
                //if this row has a drive name
                if (value != DBNull.Value)
                {
                    //if this is not the last drive (we are on the next drive) then save the diskusages for the drive
                    if ((string)value != lastDrive)
                    {
                        //write the last drives values to the hashtable
                        diskSizeValues.Add(lastDrive, diskSizeUsed);
                        diskSizeTotalAvailable.Add(lastDrive, diskSizeTotal);
                        diskSizeValueCount.Add(lastDrive, diskSizeUsed.Count);
                        diskSizeUsed = new ArrayList();
                        //save this drive as the last drive
                        lastDrive = (string)value;
                    }
                }
                else //if the drive name is null
                {
                    lastDrive = "";
                    diskSizeUsed = new ArrayList();
                    diskSizeTotal = new ArrayList();
                }

                value = row["TotalUsedGB"];
                if (value != DBNull.Value)
                {
                    diskSizeUsed.Add((double)((decimal)value));
                }
                else
                {
                    diskSizeUsed.Add(0.0);
                }
                
                value = row["TotalSizeGB"];
                if (value != DBNull.Value)
                {
                    diskSizeTotal.Add((double)((decimal)value));
                }
                else
                {
                    diskSizeTotal.Add(0.0);
                }
            }

            //we have finished iterating so whatever disk sizes we have belong to the last disk letter
            diskSizeValues.Add(lastDrive, diskSizeUsed);
            diskSizeTotalAvailable.Add(lastDrive, diskSizeTotalAvailable);
            diskSizeValueCount.Add(lastDrive, diskSizeUsed.Count);

            ForecastType forecastType = (ForecastType)((ValueListItem)localReportData.reportParameters["ForecastType"]).DataValue;
            orderPolynomial = (int)forecastType;

            //Calculate disk size forecast coefficients
            CalculateCoefficients(diskSizeValues, diskSizeValueCount, diskSizeCoefficients);

            // add a column to the DataTable for the function x and y values.
            diskUsage.Columns.Add("ForecastValue", Type.GetType("System.Double"));

            // add the x and y values
            // The formula for creating the forecast curve is C0 + C1x + C2x^2 + C3x^3
            double[] coefficients;
            index = 1;
            lastDrive = (string)diskUsage.Rows[0]["DriveName"];

            foreach (DataRow row in diskUsage.Rows)
            {
                //This only works because the data from the stored proc is sorted by table name.
                if ((string)row["DriveName"] != lastDrive)
                {
                    lastDrive = (string)row["DriveName"];
                    index = 1;
                }

                // Set the values for the existing rows.
                double forecastValue;
                coefficients = (double[])diskSizeCoefficients[row["DriveName"]];
                switch (forecastType)
                {
                    case ForecastType.Linear:
                        {
                            forecastValue = coefficients[1] * index + coefficients[0];
                            break;
                        }
                    case ForecastType.Parabolic:
                        {
                            forecastValue = coefficients[0] + (coefficients[1]*index) + (coefficients[2]*(Math.Pow(index, 2)));
                            break;
                        }
                    case ForecastType.Quartic:
                        {
                            forecastValue = coefficients[0] + (coefficients[1] * index) + (coefficients[2] * (Math.Pow(index, 2))) + (coefficients[3] * (Math.Pow(index, 3))) + (coefficients[4] * (Math.Pow(index, 4)));
                            break;
                        }
                    default: // Cubic
                        {
                            forecastValue = coefficients[0] + (coefficients[1] * index) + (coefficients[2] * (Math.Pow(index, 2))) + (coefficients[3] * (Math.Pow(index, 3)));
                            break;
                        }
                }

                row["ForecastValue"] = forecastValue >= 0 ? forecastValue : 0;

                // Set the values for the existing rows.
                index++;
                row.AcceptChanges();
            }

            ArrayList uniqueDrives = new ArrayList();
            string lastDriveName = "";

            // loop through one of the hash tables to get the list of unique drive names
            foreach (DictionaryEntry value in diskSizeValues)
            {
                if (lastDriveName != (string)value.Key)
                {
                    uniqueDrives.Add(value.Key);
                }
            }

            object date = diskUsage.Rows[diskUsage.Rows.Count - 1]["LastCollectioninInterval"];
            lastDate = (DateTime)date;

            //Add new Rows containing data forecasted for the future
            foreach (string drive in uniqueDrives)
            {
                DateTime lastModifiedDate = lastDate;

                //loop through each drive name and add x new rows
                int count = (int)diskSizeValueCount[drive];
                int units = Convert.ToInt32(forecastUnits.Text);

                if (units > 100)
                {
                    units = 100;
                    forecastUnits.Text = "100";
                }
                else
                {
                    if (units < 0)
                    {
                        units = 10;
                        forecastUnits.Text = "10";
                    }
                }

                for (int xValue = count + 1; xValue < count + units + 1; xValue++)
                {
                    DataRow row = diskUsage.NewRow();
                    switch (forecastType)
                    {
                        case ForecastType.Linear:
                            {
                                coefficients = (double[])diskSizeCoefficients[drive];
                                row["ForecastValue"] = coefficients[1] * xValue + coefficients[0];
                                break;
                            }
                        case ForecastType.Parabolic:
                            {
                                coefficients = (double[])diskSizeCoefficients[drive];
                                row["ForecastValue"] = coefficients[0] + (coefficients[1] * xValue) + (coefficients[2] * (Math.Pow(xValue, 2)));
                                break;
                            }
                        case ForecastType.Cubic:
                            {
                                coefficients = (double[])diskSizeCoefficients[drive];
                                row["ForecastValue"] = coefficients[0] + (coefficients[1] * xValue) + (coefficients[2] * (Math.Pow(xValue, 2))) + (coefficients[3] * (Math.Pow(xValue, 3)));
                                break;
                            }
                        case ForecastType.Quartic:
                            {
                                coefficients = (double[])diskSizeCoefficients[drive];
                                row["ForecastValue"] = coefficients[0] + (coefficients[1] * xValue) + (coefficients[2] * (Math.Pow(xValue, 2))) + (coefficients[3] * (Math.Pow(xValue, 3))) + (coefficients[4] * (Math.Pow(xValue, 4)));
                                break;
                            }
                        default:
                            {
                                coefficients = (double[])diskSizeCoefficients[drive];
                                row["ForecastValue"] = coefficients[0] + (coefficients[1] * xValue) + (coefficients[2] * (Math.Pow(xValue, 2))) + (coefficients[3] * (Math.Pow(xValue, 3)));
                                break;
                            }
                    }

                    // fill in the other columns with default data.
                    row["Records"] = 0;
                    lastModifiedDate = CalculateCurrentDate(lastModifiedDate);
                    row["LastCollectioninInterval"] = lastModifiedDate;
                    row["TotalUsedGB"] = DBNull.Value;
                    row["DriveName"] = drive;
                    row["TotalSizeGB"] = DBNull.Value;
                    diskUsage.Rows.Add(row);
                    row.AcceptChanges();
                }
            }
        }
        /// <summary>
        /// Returns a hastable of coeffiennts to be used in forcase for each drive
        /// </summary>
        /// <param name="values">hashtable of size samples per drive</param>
        /// <param name="valueCount">number of records per drive</param>
        /// <param name="coefficientTable">Hashtable to be returned containing coefficients for each drive</param>
        private void CalculateCoefficients(Hashtable values, Hashtable valueCount, Hashtable coefficientTable)
        {
            int count;
            double[] coefficients;
            double[] yValues;
            double[] xValues;

            ForecastType forecastType = (ForecastType)((ValueListItem)localReportData.reportParameters["ForecastType"]).DataValue;

            foreach (DictionaryEntry dataValues in values)
            {
                coefficients = new double[orderPolynomial + 1];
                count = (int)valueCount[dataValues.Key];
                yValues = new double[count];
                
                //for each sample data size for this drive
                for (int i = 0; i < count; i++)
                {
                    yValues[i] = (double)((ArrayList)dataValues.Value)[i];
                }

                //xValues countains 1 to count
                xValues = FillInXValues(count);

                if (forecastType != ForecastType.Linear)
                {
                    LeastSquares.BuildPolynomialLeastSquares(ref xValues, ref yValues, count, orderPolynomial,
                                                             ref coefficients);
                }
                else
                {
                    LeastSquares.BuildLinearLeastSquares(ref xValues, ref yValues, count, ref coefficients[0], ref coefficients[1]);
                }
                coefficientTable.Add(dataValues.Key, coefficients);
            }
        }

        /// <summary>
        /// Initialize a double array
        /// </summary>
        /// <param name="count"></param>
        /// <returns>A double array from 1 to count</returns>
        private double[] FillInXValues(int count)
        {
            double[] xValues = new double[count];

            for (int i = 0; i < count; i++)
            {
                //the x values are one based
                xValues[i] = i + 1;
            }
            return xValues;
        }

        private DateTime CalculateCurrentDate(DateTime lastDate)
        {
            DateTime nextDate = DateTime.Now;

            switch (localReportData.sampleSize)
            {
                case SampleSize.Minutes:
                    {
                        nextDate = lastDate.AddMinutes(1.0);
                        break;
                    }
                case SampleSize.Hours:
                    {
                        nextDate = lastDate.AddHours(1);
                        break;
                    }
                case SampleSize.Days:
                    {
                        nextDate = lastDate.AddDays(1);
                        break;
                    }
                case SampleSize.Months:
                    {
                        nextDate = lastDate.AddMonths(1);
                        break;
                    }
                case SampleSize.Years:
                    {
                        nextDate = lastDate.AddYears(1);
                        break;
                    }
            }
            return nextDate;
        }

        public override void ResetFilterCriteria()
        {
            base.ResetFilterCriteria();
            forecastUnits.Text = "10";
            forecastTypeCombo.SelectedItem = forecastLinear;
            sampleSizeCombo.SelectedIndex = sampleSizeCombo.Items.IndexOf(sampleDays);
            driveNamesCombo.Items.Clear();
            driveNamesCombo.Text = string.Empty;
            // driveNamesCombo.Items.Add(DrivesTag);
            driveNamesCombo.SelectedIndex = 0;
            chkShowAvailable.Checked = false;
            showTablesCheckbox.Checked = true;
            ResetTimeFilter();
        }

        /// <summary>
        /// Reset StartHours and EndHours
        /// </summary>
        private void ResetTimeFilter()
        {
            // If customDates is null then we dont need to set startHoursTimeEditor and endHoursTimeEditor
            if (customDates == null) return;

            startHoursTimeEditor.Time = System.TimeSpan.Parse("00:00:00");
            endHoursTimeEditor.Time = System.TimeSpan.Parse("23:00:00");
        }

        private void UpdateDriveNames()
        {
            driveNamesCombo.Items.Clear();
            if (instanceCombo.SelectedIndex == 0)
            {
                driveNamesCombo.Items.Clear();
                driveNamesCombo.Text = string.Empty;
                driveNamesCombo.Enabled = false;
            }
            else
            {
                driveNamesCombo.Items.Clear();
                driveNamesCombo.Text = string.Empty;
                driveNamesCombo.Enabled = true;
                try
                {
                    DataTable diskUsage = RepositoryHelper.GetReportData("p_GetDiskDrives", ((MonitoredSqlServer)((ValueListItem)instanceCombo.SelectedItem).DataValue).Id);

                    if (diskUsage.Rows.Count > 0)
                    {
                        // driveNamesCombo.Items.Add("< All Drives >");
                        int i = 0;
                        foreach (DataRow row in diskUsage.Rows)
                        {
                            string driveName = Convert.ToString(row["DriveName"]);
                            if (!string.IsNullOrEmpty(driveName))
                            {
                                CCBoxItem item = new CCBoxItem(driveName, i);
                                driveNamesCombo.Items.Add(item);
                            }
                            i++;
                        }
                        // If more then 5 items, add a scroll bar to the dropdown.
                        driveNamesCombo.MaxDropDownItems = 5;
                        // Make the "Name" property the one to display, rather than the ToString() representation.
                        driveNamesCombo.DisplayMember = "Name";
                        driveNamesCombo.ValueSeparator = ",";
                        // Check the first 2 items.
                        driveNamesCombo.SetItemChecked(0, true);
                    }
                }
                catch
                {
                    driveNamesCombo.Items.Clear();
                }
            }

        }

        private void instanceCombo_SelectionChangeCommitted(object sender, EventArgs e)
        {
            UpdateDriveNames();
        }

        /// <summary>
        /// Calculate the forecast by iterating through the disk usage data set and then populating the disk usage data set
        /// to be passed to the report as a data source with the additional forcast data
        /// </summary>
        /// <param name="diskUsage"></param>
        private void ForecastDiskUsage1(ref DataTable diskUsage)
        {
            int index = 0;

            // Bail out if there was no data returned by the query
            if (diskUsage.Rows.Count == 0) return;

            Dictionary<string, DiskDrive> drives = new Dictionary<string, DiskDrive>();

            //get the inital data in arrays for calculations. Sorted by drive name.
            foreach (DataRow row in diskUsage.Rows)
            {
                //save the name of the last drive
                string currentDrive = (string)row["DriveName"];
                
                if (row.IsNull("TotalUsedGB") || row.IsNull("TotalSizeGB")) continue;

                //if we do not already have an entry for this drive then add the drive
                if (!drives.ContainsKey(currentDrive)) drives.Add(currentDrive, new DiskDrive(currentDrive));
                
                //add sample to the drive
                drives[currentDrive].AddSamples((double)(decimal)row["TotalUsedGB"], (double)(decimal)row["TotalSizeGB"]);
            }

            ForecastType forecastType = (ForecastType)((ValueListItem)localReportData.reportParameters["ForecastType"]).DataValue;

            orderPolynomial = (int)forecastType;

            //Calculate disk size forecast coefficients
            CalculateCoefficients1(ref drives);

            // add a column to the DataTable for the function x and y values.
            diskUsage.Columns.Add("ForecastValue", Type.GetType("System.Double"));
            //diskUsage.Columns.Remove("TotalSizeGB");
            //diskUsage.Columns.Add("TotalSizeGB", Type.GetType("System.Double"));
            diskUsage.Columns["TotalSizeGB"].ReadOnly = false;

            // add the x and y values
            // The formula for creating the forecast curve is C0 + C1x + C2x^2 + C3x^3
            index = 1;
            string lastDrive = "";

            foreach (DataRow row in diskUsage.Rows)
            {
                string currentDrive = (string)row["DriveName"];

                if (!drives.ContainsKey(currentDrive)) continue;

                if (currentDrive != lastDrive) index = 1;

                row["ForecastValue"] = GetForecastValue(forecastType, index, drives[currentDrive].Coefficients);
                row["TotalSizeGB"] = drives[currentDrive].AverageAvailableSpace();

                index++;
                lastDrive = currentDrive;
                row.AcceptChanges();
            }

            DateTime lastDate = (DateTime)diskUsage.Rows[diskUsage.Rows.Count - 1]["LastCollectioninInterval"];

            //Add new Rows containing data forecasted for the future
            foreach (KeyValuePair<string, DiskDrive> d in drives)
            {
                DateTime lastModifiedDate = lastDate;

                //loop through each drive name and add x new rows
                int count = (int)d.Value.GetSampleCount();
                int units = Convert.ToInt32(forecastUnits.Text);

                if (units > 100)
                {
                    units = 100;
                    forecastUnits.Text = "100";
                }
                else
                {
                    if (units < 0)
                    {
                        units = 10;
                        forecastUnits.Text = "10";
                    }
                }
                for (int xValue = count + 1; xValue < count + units + 1; xValue++)
                {
                    DataRow row = null;

                    row = diskUsage.NewRow();
                    row["ForecastValue"] = GetForecastValue(forecastType, xValue, d.Value.Coefficients);

                    // fill in the other columns with default data.
                    row["Records"] = 0;
                    lastModifiedDate = CalculateCurrentDate(lastModifiedDate);
                    row["LastCollectioninInterval"] = lastModifiedDate;
                    row["TotalUsedGB"] = DBNull.Value;
                    row["DriveName"] = d.Key;
                    //row["TotalSizeGB"] = DBNull.Value;
                    row["TotalSizeGB"] = d.Value.AverageAvailableSpace();
                    diskUsage.Rows.Add(row);                        
                    
                    row.AcceptChanges();
                }
            }
        }

        private double GetForecastValue(ForecastType forecastType, int index, double[] coefficients)
        {
            // Set the values for the existing rows.
            double forecastValue;

            switch (forecastType)
            {
                case ForecastType.Linear:
                    {
                        forecastValue = coefficients[1] * index + coefficients[0];
                        break;
                    }
                case ForecastType.Parabolic:
                    {
                        forecastValue = coefficients[0] + (coefficients[1] * index) + (coefficients[2] * (Math.Pow(index, 2)));
                        break;
                    }
                case ForecastType.Quartic:
                    {
                        forecastValue = coefficients[0] + (coefficients[1] * index) + (coefficients[2] * (Math.Pow(index, 2))) + (coefficients[3] * (Math.Pow(index, 3))) + (coefficients[4] * (Math.Pow(index, 4)));
                        break;
                    }
                default: // Cubic
                    {
                        forecastValue = coefficients[0] + (coefficients[1] * index) + (coefficients[2] * (Math.Pow(index, 2))) + (coefficients[3] * (Math.Pow(index, 3)));
                        break;
                    }
            }

            return forecastValue >= 0 ? forecastValue : 0;            
        }

        /// <summary>
        /// Returns a hastable of coeffiennts to be used in forcase for each drive
        /// </summary>
        /// <param name="existingDrives"></param>
        private void CalculateCoefficients1(ref Dictionary<string, DiskDrive> existingDrives)
        {
            int count;
            double[] coefficients;
            double[] yValues;
            double[] xValues;

            ForecastType forecastType = (ForecastType)((ValueListItem)localReportData.reportParameters["ForecastType"]).DataValue;

            foreach (KeyValuePair<string, DiskDrive> d in existingDrives)
            {
                coefficients = new double[orderPolynomial + 1];
                count = d.Value.GetSampleCount();
                yValues = new double[d.Value.UsedDiskSamples.GetLength(0)];

                //copy the samples from this drive to yvalues
                d.Value.UsedDiskSamples.CopyTo(yValues,0);
                
                //xValues countains 1 to count
                xValues = FillInXValues(count);

                if (forecastType != ForecastType.Linear)
                {
                    LeastSquares.BuildPolynomialLeastSquares(ref xValues, ref yValues, count, orderPolynomial,
                                                             ref coefficients);
                }
                else
                {
                    LeastSquares.BuildLinearLeastSquares(ref xValues, ref yValues, count, ref coefficients[0], ref coefficients[1]);
                }
                d.Value.Coefficients = coefficients;
            }
        }

        private void forecastUnits_Leave(object sender, EventArgs e)
        {
            if (this.forecastUnits.Text.Equals(string.Empty))
            {
                this.forecastUnits.Text = this.forecastUnits.Minimum.ToString();
            }
        }

        private void SetStartHours()
        {
            DateTime first = customDates[0].UtcStart.ToLocalTime();
            DateTime final = new DateTime(first.Year, first.Month, first.Day, startHoursTimeEditor.Time.Hours, startHoursTimeEditor.Time.Minutes, startHoursTimeEditor.Time.Seconds);
            customDates[0].UtcStart = final.ToUniversalTime();
        }

        private void SetEndHours()
        {
            DateTime last = customDates[customDates.Count - 1].UtcEnd.ToLocalTime();
            DateTime final = new DateTime(last.Year, last.Month, last.Day, endHoursTimeEditor.Time.Hours, endHoursTimeEditor.Time.Minutes, endHoursTimeEditor.Time.Seconds);
            customDates[customDates.Count - 1].UtcEnd = final.ToUniversalTime();
        }

        private void periodCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (periodCombo.SelectedItem == periodSetCustom)
            {
                startHoursLbl.Visible = true;
                startHoursTimeEditor.Visible = true;
                endHoursLbl.Visible = true;
                endHoursTimeEditor.Visible = true;

                this.SetStartHours();
                this.SetEndHours();
            }
            else
            {
                startHoursLbl.Visible = false;
                startHoursTimeEditor.Visible = false;
                endHoursLbl.Visible = false;
                endHoursTimeEditor.Visible = false;
            }
        }

        private void startHoursTimeEditor_ValueChanged(object sender, EventArgs e)
        {
            SetStartHours();
        }

        private void endHoursTimeEditor_ValueChanged(object sender, EventArgs e)
        {
            SetEndHours();
        }

        private string GetDateRange(DateTime rsStart, DateTime rsEnd)
        {
            string dateRange = string.Empty;

            if (periodCombo.SelectedItem == periodSetCustom)
            {
                dateRange = rsStart.ToLocalTime() + " to " + rsEnd.ToLocalTime();
            }
            else
            {
                dateRange = rsStart.ToLocalTime().ToString("d") + " to " + rsEnd.ToLocalTime().ToString("d");
            }

            return dateRange;
        }
    }

    internal class DiskDrive
    {
        private string _DriveName = null;
        //private int _SampleCount = 0;
        private ArrayList _UsedDiskSamples = new ArrayList();
        private ArrayList _AvailableDiskSamples = new ArrayList();
        private ArrayList _Coefficients = new ArrayList();
        public DiskDrive(string DriveName)
        {
            _DriveName = DriveName;
        }
        
        public void AddSamples(double UsedDiskSpace, double AvailableDiskSpace)
        {
            AddUsedDiskSpaceSample(UsedDiskSpace);
            AddAvailableDiskSpaceSample(AvailableDiskSpace);
        }
        
        public void AddUsedDiskSpaceSample(double UsedDiskSpace)
        {
            _UsedDiskSamples.Add(UsedDiskSpace);
        }
        
        public void AddAvailableDiskSpaceSample(double AvailableDiskSpace)
        {
            _AvailableDiskSamples.Add(AvailableDiskSpace);
        }

        public int GetSampleCount()
        {
            return _UsedDiskSamples.Count;            
        }

        public double AverageAvailableSpace()
        {
            double average = 0;
            for(int i = 0; i < _AvailableDiskSamples.Count; i++)
            {
                average += (double)_AvailableDiskSamples[i];
            }
            return average/_AvailableDiskSamples.Count;
        }
        
        public double[] UsedDiskSamples
        {
            get { return Array.ConvertAll(_UsedDiskSamples.ToArray(), new Converter<object,double>(ObjectToDouble)); }
            set { _UsedDiskSamples.AddRange(Array.ConvertAll(value, new Converter<double,object>(DoubleToObject))); }
        }

        public double[] AvailableDiskSamples
        {
            get { return Array.ConvertAll(_AvailableDiskSamples.ToArray(), new Converter<object, double>(ObjectToDouble)); }
            set { _AvailableDiskSamples.AddRange(Array.ConvertAll(value, new Converter<double, object>(DoubleToObject))); }
        }

        public double[] Coefficients
        {
            get { return Array.ConvertAll(_Coefficients.ToArray(), new Converter<object, double>(ObjectToDouble)); }
            set { _Coefficients.AddRange(Array.ConvertAll(value, new Converter<double, object>(DoubleToObject))); }
        }

        private string DriveName
        {
            get { return _DriveName; }
        }

        private static double ObjectToDouble(object sample)
        {
            return (double) sample;
        }

        private static object DoubleToObject(double sample)
        {
            return (object) sample;
        }
    }
}