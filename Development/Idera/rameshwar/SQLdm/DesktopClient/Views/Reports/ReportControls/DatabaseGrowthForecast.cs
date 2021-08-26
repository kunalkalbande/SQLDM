using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using BBS.TracerX;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Helpers.MathLibrary;
using Infragistics.Win;
using Microsoft.Reporting.WinForms;

namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    public partial class DatabaseGrowthForecast : Idera.SQLdm.DesktopClient.Views.Reports.ReportControls.DatabaseReport
    {
        private static readonly Logger Log = Logger.GetLogger("DatabaseGrowthForecast");
        private int orderPolynomial = 3;
        private WorkerData localReportData = null;
        private List<ReportParameter> passthroughParameters = new List<ReportParameter>(1);

        protected enum ForecastType { Linear = 1, Parabolic, Cubic, Quartic };

        readonly ValueListItem forecastLinear = new ValueListItem(ForecastType.Linear, "Linear");
        readonly ValueListItem forecastParabola = new ValueListItem(ForecastType.Parabolic, "Exponential (Aggressive)");
        readonly ValueListItem forecastCubic = new ValueListItem(ForecastType.Cubic, "Cubic");
        readonly ValueListItem forecastQuartic = new ValueListItem(ForecastType.Quartic, "Quartic");

        public DatabaseGrowthForecast()
        {
            InitializeComponent();
            AdaptFontSize();
            showTablesCheckbox.Checked = true;
        }

        public override bool CanRunReport(out string message)
        {
            message = String.Empty;

            if (instanceCombo.SelectedIndex == 0)
            {
                message = "A SQL Server instance must be selected to generate this report.";
                return false;
            }

            if (Databases == null || Databases.Count == 0)
            {
                message = "At least one database must be selected to generate this report.";
                return false;
            }

            return true;
        }

        public override void InitReport()
        {
            base.InitReport();
            periodCombo.SelectedItem = period7;
            periodCombo.Items.Remove(periodToday);
            sampleSizeCombo.Items.Remove(sampleHours);
            sampleSizeCombo.SelectedItem = sampleDays;
            InitializeReportViewer();
            InitializeForecastType();
            State = UIState.ParmsNeeded;
            multipleDatabasesAllowed = true;
            ReportType = ReportTypes.DatabaseGrowthForecast;
           
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

            if ((Databases != null) && (Databases.Count > 0))
            {
                reportParameters.Add("DatabaseXML", ConvertToXml(XmlType.Database, Databases));
            }

            if (forecastTypeCombo.SelectedItem != null)
            {
                reportParameters.Add("ForecastType", forecastTypeCombo.SelectedItem);
            }
            
            string strDatabases = databaseTextbox.Text.Length > 50?databaseTextbox.Text.Substring(0,50) + "...":databaseTextbox.Text;

            reportParameters.Add("GUIServer", instanceCombo.SelectedItem == null ? "" : instanceCombo.SelectedItem.ToString());
            reportParameters.Add("GUIDateRange", this.GetDateRange(reportData.dateRanges[0].UtcStart, reportData.dateRanges[0].UtcEnd));
            reportParameters.Add("GUIDatabase", strDatabases);
            reportParameters.Add("GUIForecastUnits", forecastUnits.Text);
            reportParameters.Add("GUIForecastType", forecastTypeCombo.SelectedItem == null ? "" : ((ValueListItem)forecastTypeCombo.SelectedItem).DisplayText);
            reportParameters.Add("DisplayTables", showTablesCheckbox.Checked.ToString());
            reportParameters.Add("GUIDBCount", Databases==null?0:Databases.Count);
            reportParameters.Add("ShowAvailable", chkShowAvailableSpace.Checked.ToString());
        }

        override protected void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (System.Threading.Thread.CurrentThread.Name == null) System.Threading.Thread.CurrentThread.Name = "ReportControlWorker";

            using (Log.DebugCall())
            {
                localReportData = (WorkerData)e.Argument;

                Log.Debug("localReportData.reportType = ", localReportData.reportType);
                passthroughParameters.Clear();
                passthroughParameters.Add(new ReportParameter("executionStart", DateTime.Now.ToString()));
                passthroughParameters.Add(new ReportParameter("Interval", ((int)localReportData.sampleSize).ToString()));

                passthroughParameters.Add(new ReportParameter("GUIServer", localReportData.reportParameters["GUIServer"].ToString()));
                passthroughParameters.Add(new ReportParameter("GUIDateRange", localReportData.reportParameters["GUIDateRange"].ToString()));
                passthroughParameters.Add(new ReportParameter("GUIDatabase", localReportData.reportParameters["GUIDatabase"].ToString()));
                passthroughParameters.Add(new ReportParameter("GUIForecastUnits", localReportData.reportParameters["GUIForecastUnits"].ToString()));
                passthroughParameters.Add(new ReportParameter("GUIForecastType", localReportData.reportParameters["GUIForecastType"].ToString()));
                passthroughParameters.Add(new ReportParameter("GUIDBCount", localReportData.reportParameters["GUIDBCount"].ToString()));
                passthroughParameters.Add(new ReportParameter("ShowAvailable", localReportData.reportParameters["ShowAvailable"].ToString()));
                passthroughParameters.Add(new ReportParameter("DisplayTables", (string)localReportData.reportParameters["DisplayTables"]));
                localReportData.dataSources = new ReportDataSource[1];
                ReportDataSource dataSource = new ReportDataSource("DatabaseGrowth");

                DataTable databaseGrowth = RepositoryHelper.GetReportData("p_DatabaseGrowthForecast",
                                                                          (string)localReportData.reportParameters["DatabaseXML"],
                                                                          localReportData.instanceID,
                                                                          localReportData.dateRanges[0].UtcStart,
                                                                          localReportData.dateRanges[0].UtcEnd,
                                                                          localReportData.dateRanges[0].UtcOffsetMinutes.ToString(),
                                                                          (int)localReportData.sampleSize);
                
                databaseGrowth.Columns["AvailableDataSizeMb"].ReadOnly = false;
                databaseGrowth.Columns["ForecastFileSize"].ReadOnly = false;
                databaseGrowth.Columns["ForecastDBSize"].ReadOnly = false;

                passthroughParameters.Add(new ReportParameter("GUISampleCount", databaseGrowth.Rows.Count.ToString()));

                ForecastDatabaseGrowth(ref databaseGrowth);
                
                //TextWriter tw = new StreamWriter("c:\\datatable.txt");

                //foreach (DataColumn column in databaseGrowth.Columns)
                //{
                //    tw.Write("{0}\t ", column.ColumnName);
                //}
                //tw.Write("\r\n");

                //foreach (DataRow row in databaseGrowth.Rows)
                //{
                //    foreach (DataColumn column in databaseGrowth.Columns)
                //    {
                //        // write a line of text to the file
                //        tw.Write("{0}\t", row[column]);
                //    }
                //    tw.Write("\r\n");
                //}
                //// close the stream
                //tw.Close();


                dataSource.Value = databaseGrowth;
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
                                "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.DatabaseGrowthForecast.rdl"))
                            {
                                reportViewer.LocalReport.LoadReportDefinition(stream);
                            }
                            foreach (ReportDataSource dataSource in reportData.dataSources)
                            {
                                reportViewer.LocalReport.DataSources.Add(dataSource);
                            }
                            
                            reportViewer.LocalReport.SetParameters(passthroughParameters);

                            reportViewer.RefreshReport();
                            reportViewer.LocalReport.DisplayName = "DatabaseGrowthForecast";
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


       private void ForecastDatabaseGrowth(ref DataTable databaseGrowth)
        {
            ArrayList databaseSize = new ArrayList();
            Hashtable databaseSizeValues = new Hashtable();
            Hashtable databaseSizeValueCount = new Hashtable();

            ArrayList dataFileSize = new ArrayList();
            Hashtable dataFileValues = new Hashtable();
            Hashtable dataFileValueCount = new Hashtable();

            ArrayList availableDiskSpace = new ArrayList();
            Hashtable availableDiskSpaceValues = new Hashtable();
            Hashtable availableDiskSpaceCount = new Hashtable();
            Hashtable availableDiskSpaceAve = new Hashtable();

            Hashtable databaseSizeCoefficients = new Hashtable();
            Hashtable dataFileCoefficients = new Hashtable();

            string lastDatabase;
            int index;
            //int intNonZeroSamples = 0;
            DateTime lastDate;

            // Bail out if there was no data returned by the query
            if (databaseGrowth.Rows.Count == 0) return;

            //save the last database name
            lastDatabase = (string)databaseGrowth.Rows[0]["DatabaseName"];

            //get the inital data in arrays for calculations. Sorted by Database.
            //iterate through each row in the dataset. Each database has an arraylist of values.
            foreach (DataRow row in databaseGrowth.Rows)
            {
                object value = row["DatabaseName"];
                if (value != DBNull.Value)
                {
                    if ((string)value != lastDatabase)
                    {
                        databaseSizeValues.Add(lastDatabase, databaseSize);
                        databaseSizeValueCount.Add(lastDatabase, databaseSize.Count);

                        dataFileValues.Add(lastDatabase, dataFileSize);
                        dataFileValueCount.Add(lastDatabase, dataFileSize.Count);

                        availableDiskSpaceValues.Add(lastDatabase, availableDiskSpace);
                        availableDiskSpaceCount.Add(lastDatabase, availableDiskSpace.Count);
                        availableDiskSpaceAve.Add(lastDatabase, AverageArrayList(availableDiskSpace));

                        dataFileSize = new ArrayList();
                        databaseSize = new ArrayList();
                        availableDiskSpace = new ArrayList();

                        lastDatabase = (string)value;
                    }
                }
                else
                {
                    //Initialize variable
                    lastDatabase = "";
                    dataFileSize = new ArrayList();
                    databaseSize = new ArrayList();

                    availableDiskSpace = new ArrayList();
                }

                value = row["DataFileSizeMb"];
                if (value != DBNull.Value)
                {
                    dataFileSize.Add((double)value);
                }
                else
                {
                    dataFileSize.Add(0.0);
                }

                value = row["TotalSizeMb"];
                if (value != DBNull.Value)
                {
                    databaseSize.Add((double)value);
                }
                else
                {
                    databaseSize.Add(0.0);
                }

                value = row["AvailableDataSizeMb"];
                if (value != DBNull.Value)
                {
                    availableDiskSpace.Add((double)value);
                }
                else
                {
                    availableDiskSpace.Add(0.0);
                }
            }

            //add the last values.
            databaseSizeValues.Add(lastDatabase, databaseSize);
            databaseSizeValueCount.Add(lastDatabase, databaseSize.Count);

            dataFileValues.Add(lastDatabase, dataFileSize);
            dataFileValueCount.Add(lastDatabase, dataFileSize.Count);

            availableDiskSpaceValues.Add(lastDatabase, availableDiskSpace);
            availableDiskSpaceCount.Add(lastDatabase, availableDiskSpace.Count);
            availableDiskSpaceAve.Add(lastDatabase, AverageArrayList(availableDiskSpace));

            ForecastType forecastType = (ForecastType)((ValueListItem)localReportData.reportParameters["ForecastType"]).DataValue;

            orderPolynomial = (int)forecastType;

            //Calculate Database Size forecast coefficients
            CalculateCoefficients(databaseSizeValues, databaseSizeValueCount, ref databaseSizeCoefficients);

            //calculate Data File Size forecast coefficients
            CalculateCoefficients(dataFileValues, dataFileValueCount, ref dataFileCoefficients);

            //Add the forecasted data to the original data rows
            //The formula for creating the forecast curve is C0 + C1x + C2x^2 + C3x^3
            double[] coefficients;
            index = 1;
            lastDatabase = (string)databaseGrowth.Rows[0]["DatabaseName"];

            foreach (DataRow row in databaseGrowth.Rows)
            {
                // Set the values for the existing rows.
                //This only works because the data from the stored proc is sorted by database name.
                if ((string)row["DatabaseName"] != lastDatabase)
                {
                    lastDatabase = (string)row["DatabaseName"];
                    index = 1;
                }

                switch (forecastType)
                {
                    case ForecastType.Linear:
                        {
                            coefficients = (double[])dataFileCoefficients[row["DatabaseName"]];
                            double forecastFileSize = coefficients[1] * index + coefficients[0];
                            row["ForecastFileSize"] = forecastFileSize >= 0 ? forecastFileSize : 0;

                            coefficients = (double[])databaseSizeCoefficients[row["DatabaseName"]];
                            double forecastDBSize = coefficients[1] * index + coefficients[0];
                            row["ForecastDBSize"] = forecastDBSize >= 0 ? forecastDBSize : 0;

                            break;
                        }
                    case ForecastType.Parabolic:
                        {
                            coefficients = (double[])dataFileCoefficients[row["DatabaseName"]];
                            double forecastFileSize = coefficients[0] + (coefficients[1] * index) +
                                                      (coefficients[2] * (Math.Pow(index, 2)));
                            row["ForecastFileSize"] = forecastFileSize >= 0 ? forecastFileSize : 0;
                            coefficients = (double[])databaseSizeCoefficients[row["DatabaseName"]];
                            double forecastDBSize = coefficients[0] + (coefficients[1] * index) +
                                                    (coefficients[2] * (Math.Pow(index, 2)));
                            row["ForecastDBSize"] = forecastDBSize >= 0 ? forecastDBSize : 0;
                            break;
                        }
                    case ForecastType.Cubic:
                        {
                            coefficients = (double[])dataFileCoefficients[row["DatabaseName"]];
                            row["ForecastFileSize"] = coefficients[0] + (coefficients[1] * index) + (coefficients[2] * (Math.Pow(index, 2))) + (coefficients[3] * (Math.Pow(index, 3)));
                            coefficients = (double[])databaseSizeCoefficients[row["DatabaseName"]];
                            row["ForecastDBSize"] = coefficients[0] + (coefficients[1] * index) + (coefficients[2] * (Math.Pow(index, 2))) + (coefficients[3] * (Math.Pow(index, 3)));
                            break;
                        }
                    case ForecastType.Quartic:
                        {
                            coefficients = (double[])dataFileCoefficients[row["DatabaseName"]];
                            row["ForecastFileSize"] = coefficients[0] + (coefficients[1] * index) + (coefficients[2] * (Math.Pow(index, 2))) + (coefficients[3] * (Math.Pow(index, 3))) + (coefficients[4] * (Math.Pow(index, 4)));
                            coefficients = (double[])databaseSizeCoefficients[row["DatabaseName"]];
                            row["ForecastDBSize"] = coefficients[0] + (coefficients[1] * index) + (coefficients[2] * (Math.Pow(index, 2))) + (coefficients[3] * (Math.Pow(index, 3))) + (coefficients[4] * (Math.Pow(index, 4)));
                            break;
                        }
                    default:
                        {
                            coefficients = (double[])dataFileCoefficients[row["DatabaseName"]];
                            row["ForecastFileSize"] = coefficients[0] + (coefficients[1] * index) + (coefficients[2] * (Math.Pow(index, 2))) + (coefficients[3] * (Math.Pow(index, 3)));
                            coefficients = (double[])databaseSizeCoefficients[row["DatabaseName"]];
                            row["ForecastDBSize"] = coefficients[0] + (coefficients[1] * index) + (coefficients[2] * (Math.Pow(index, 2))) + (coefficients[3] * (Math.Pow(index, 3)));
                            break;
                        }
                }

                row["AvailableDataSizeMb"] = availableDiskSpaceAve[row["DatabaseName"]];

                index++;
                row.AcceptChanges();
            }

            ArrayList uniqueDatabases = new ArrayList();
            string lastDBName = "";

            // loop through one of the hash tables to get a list of unique database names.
            foreach (DictionaryEntry value in dataFileValues)
            {
                if (lastDBName != (string)value.Key)
                {
                    uniqueDatabases.Add(value.Key);
                }
            }

            //Get the last date returned from the query
            object date = databaseGrowth.Rows[databaseGrowth.Rows.Count - 1]["LastCollectioninInterval"];
            lastDate = (DateTime)date;

            // Add new Rows containing data forecasted for the future.
            foreach (string database in uniqueDatabases)
            {
                DateTime lastModifiedDate = lastDate;

                //loop through each database name and add x new rows
                int count = (int)dataFileValueCount[database];
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
                    DataRow row = databaseGrowth.NewRow();
                    switch (forecastType)
                    {
                        case ForecastType.Linear:
                            {
                                coefficients = (double[])dataFileCoefficients[database];
                                row["ForecastFileSize"] = coefficients[1] * xValue + coefficients[0];
                                coefficients = (double[])databaseSizeCoefficients[database];
                                row["ForecastDBSize"] = coefficients[1] * xValue + coefficients[0];
                                break;
                            }
                        case ForecastType.Parabolic:
                            {
                                coefficients = (double[])dataFileCoefficients[database];
                                row["ForecastFileSize"] = coefficients[0] + (coefficients[1] * xValue) + (coefficients[2] * (Math.Pow(xValue, 2)));
                                coefficients = (double[])databaseSizeCoefficients[database];
                                row["ForecastDBSize"] = coefficients[0] + (coefficients[1] * xValue) + (coefficients[2] * (Math.Pow(xValue, 2)));
                                break;
                            }
                        case ForecastType.Cubic:
                            {
                                coefficients = (double[])dataFileCoefficients[database];
                                row["ForecastFileSize"] = coefficients[0] + (coefficients[1] * xValue) + (coefficients[2] * (Math.Pow(xValue, 2))) + (coefficients[3] * (Math.Pow(xValue, 3)));
                                coefficients = (double[])databaseSizeCoefficients[database];
                                row["ForecastDBSize"] = coefficients[0] + (coefficients[1] * xValue) + (coefficients[2] * (Math.Pow(xValue, 2))) + (coefficients[3] * (Math.Pow(xValue, 3)));
                                break;
                            }
                        case ForecastType.Quartic:
                            {
                                coefficients = (double[])dataFileCoefficients[database];
                                row["ForecastFileSize"] = coefficients[0] + (coefficients[1] * xValue) + (coefficients[2] * (Math.Pow(xValue, 2))) + (coefficients[3] * (Math.Pow(xValue, 3))) + (coefficients[4] * (Math.Pow(xValue, 4)));
                                coefficients = (double[])databaseSizeCoefficients[database];
                                row["ForecastDBSize"] = coefficients[0] + (coefficients[1] * xValue) + (coefficients[2] * (Math.Pow(xValue, 2))) + (coefficients[3] * (Math.Pow(xValue, 3))) + (coefficients[4] * (Math.Pow(xValue, 4)));
                                break;
                            }
                        default:
                            {
                                coefficients = (double[])dataFileCoefficients[database];
                                row["ForecastFileSize"] = coefficients[0] + (coefficients[1] * xValue) + (coefficients[2] * (Math.Pow(xValue, 2))) + (coefficients[3] * (Math.Pow(xValue, 3)));
                                coefficients = (double[])databaseSizeCoefficients[database];
                                row["ForecastDBSize"] = coefficients[0] + (coefficients[1] * xValue) + (coefficients[2] * (Math.Pow(xValue, 2))) + (coefficients[3] * (Math.Pow(xValue, 3)));
                                break;
                            }
                    }
                    row["AvailableDataSizeMb"] = availableDiskSpaceAve[database];

                    // fill in the other columns with default data.
                    lastModifiedDate = CalculateCurrentDate(lastModifiedDate);

                    row["LastCollectioninInterval"] = lastModifiedDate;
                    row["DatabaseName"] = database;
                    row["TotalSizeMB"] = DBNull.Value;
                    row["DataFileSizeMb"] = DBNull.Value;

                    databaseGrowth.Rows.Add(row);
                    row.AcceptChanges();
                }
            }
        }

        internal static double AverageArrayList(ArrayList AverageThis)
        {
            double average = 0;
            for (int i = 0; i < AverageThis.Count; i++)
            {
                average += (double) (AverageThis[i] ?? 0);
            }

            return average / AverageThis.Count;
        }

        private void CalculateCoefficients(Hashtable values, Hashtable valueCount, ref Hashtable coefficientTable)
        {
            int count;
            double[] coefficients;
            double[] yValues;
            double[] xValues;

            ForecastType forecastType = (ForecastType)((ValueListItem)localReportData.reportParameters["ForecastType"]).DataValue;
            coefficientTable.Clear();

            foreach (DictionaryEntry dataValues in values)
            {
                coefficients = new double[orderPolynomial + 1];
                count = (int)valueCount[dataValues.Key];
                yValues = new double[count];

                for (int i = 0; i < count; i++)
                {
                    yValues[i] = (double)((ArrayList)dataValues.Value)[i];
                }
                xValues = FillInXValues(count);

                if (forecastType != ForecastType.Linear)
                {
                    LeastSquares.BuildPolynomialLeastSquares(ref xValues, ref yValues, count, orderPolynomial, ref coefficients);
                }
                else
                {
                    LeastSquares.BuildLinearLeastSquares(ref xValues, ref yValues, count, ref coefficients[0], ref coefficients[1]);
                }
                coefficientTable.Add(dataValues.Key, coefficients);
            }
        }

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
            Databases = null;
            forecastUnits.Text = "10";
            forecastTypeCombo.SelectedItem = forecastLinear;
            sampleSizeCombo.SelectedIndex = sampleSizeCombo.Items.IndexOf(sampleDays);
            chkShowAvailableSpace.Checked = false;
            ResetTimeFilter();
            showTablesCheckbox.Checked = true;
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
}

