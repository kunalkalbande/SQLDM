using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using BBS.TracerX;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Helpers.MathLibrary;
using Infragistics.Win;
using Microsoft.Reporting.WinForms;
using Wintellect.PowerCollections;

namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    public partial class TableGrowthForecast : DatabaseReport
    {
        private static readonly Logger Log = Logger.GetLogger("Table Growth Forecast");
        private List<ReportParameter> passthroughParameters = new List<ReportParameter>(8);
        private WorkerData localReportData = null;
        private int orderPolynomial = 3;

        protected enum ForecastType { Linear, Parabolic, Cubic, Quartic };

        readonly ValueListItem forecastLinear = new ValueListItem(ForecastType.Linear, "Linear");
        readonly ValueListItem forecastParabola = new ValueListItem(ForecastType.Parabolic, "Exponential (Aggressive)");
        readonly ValueListItem forecastCubic = new ValueListItem(ForecastType.Cubic, "Cubic");
        readonly ValueListItem forecastQuartic = new ValueListItem(ForecastType.Quartic, "Quartic");

        public TableGrowthForecast()
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
                message = "A database must be selected to generate this report.";
                return false;
            }

            if (tablesTextbox.Text.Length == 0)
            {
                message = "At least one table must be selected to generate this report.";
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
            InitializeForecastType();
            State = UIState.ParmsNeeded;
            multipleDatabasesAllowed = false;
            ReportType = ReportTypes.TableGrowthForecast;
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
            List<string> Tables = MakeTableNameArray(MultiTables, true);
            reportParameters.Add("TableXML", ConvertToXml(XmlType.Tables, Tables));

            reportParameters.Add("ForecastType", forecastTypeCombo.SelectedItem);

            if ((Databases != null) && (Databases.Count > 0))
            {
                reportParameters.Add("Database", Databases[0]);
            }

            string startTime = reportData.dateRanges[0].UtcStart.ToLocalTime().ToString("d");
            string endTime = reportData.dateRanges[0].UtcEnd.ToLocalTime().ToString("d");
            string strTables = tablesTextbox.Text.Length > 50 ? tablesTextbox.Text.Substring(0, 50) + "..." : tablesTextbox.Text;

            reportParameters.Add("GUIServer", instanceCombo.SelectedItem==null?"":instanceCombo.SelectedItem.ToString());
            reportParameters.Add("GUIDateRange", startTime + " to " + endTime);
            reportParameters.Add("GUIDatabase", databaseTextbox.Text);
            reportParameters.Add("GUITable", strTables);
            reportParameters.Add("GUIForecastUnits", forecastUnits.Text);
            reportParameters.Add("GUIForecastType", forecastTypeCombo.SelectedItem==null?"":((ValueListItem)forecastTypeCombo.SelectedItem).DisplayText);
            reportParameters.Add("GUITableCount", Tables.Count);
            reportParameters.Add("DisplayTables", showTablesCheckbox.Checked.ToString());
        }

        override protected void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (System.Threading.Thread.CurrentThread.Name == null) System.Threading.Thread.CurrentThread.Name = "ReportControlWorker";

            using (Log.DebugCall())
            {
                localReportData = (WorkerData)e.Argument;
                ReportDataSource dataSource;

                Log.Debug("localReportData.reportType = ", localReportData.reportType);

                passthroughParameters.Clear();
                passthroughParameters.Add(new ReportParameter("executionStart", DateTime.Now.ToString()));
                passthroughParameters.Add(new ReportParameter("Interval", ((int)localReportData.sampleSize).ToString()));

                passthroughParameters.Add(new ReportParameter("GUIServer", localReportData.reportParameters["GUIServer"].ToString()));
                passthroughParameters.Add(new ReportParameter("GUIDateRange", localReportData.reportParameters["GUIDateRange"].ToString()));
                passthroughParameters.Add(new ReportParameter("GUIDatabase", localReportData.reportParameters["GUIDatabase"].ToString()));
                passthroughParameters.Add(new ReportParameter("GUITable", localReportData.reportParameters["GUITable"].ToString()));
                passthroughParameters.Add(new ReportParameter("GUIForecastUnits", localReportData.reportParameters["GUIForecastUnits"].ToString()));
                passthroughParameters.Add(new ReportParameter("GUIForecastType", localReportData.reportParameters["GUIForecastType"].ToString()));
                passthroughParameters.Add(new ReportParameter("GUITableCount", localReportData.reportParameters["GUITableCount"].ToString()));
                passthroughParameters.Add(new ReportParameter("DisplayTables", (string)localReportData.reportParameters["DisplayTables"]));
                localReportData.dataSources = new ReportDataSource[1];
                dataSource = new ReportDataSource("TableGrowth");
                DataTable tableGrowth = RepositoryHelper.GetReportData("p_TableGrowthForecast",
                                                                  (string)localReportData.reportParameters["TableXML"],
                                                                  (string)localReportData.reportParameters["Database"],
                                                                  localReportData.instanceID,
                                                                  localReportData.dateRanges[0].UtcStart,
                                                                  localReportData.dateRanges[0].UtcEnd,
                                                                  localReportData.dateRanges[0].UtcOffsetMinutes.ToString(),
                                                                  (int)localReportData.sampleSize);

                passthroughParameters.Add(new ReportParameter("GUISampleCount", tableGrowth.Rows.Count.ToString()));

                ForecastTableGrowth(ref tableGrowth);

                dataSource.Value = tableGrowth;

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
                                "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.TableGrowthForecast.rdl"))
                            {
                                reportViewer.LocalReport.LoadReportDefinition(stream);
                            }
                            foreach (ReportDataSource dataSource in reportData.dataSources)
                            {
                                reportViewer.LocalReport.DataSources.Add(dataSource);
                            }

                            reportViewer.LocalReport.SetParameters(passthroughParameters);
                            reportViewer.RefreshReport();
                            reportViewer.LocalReport.DisplayName = "TableGrowthForecast";
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

        private void ForecastTableGrowth(ref DataTable tableGrowth)
        {
            ArrayList tableSize = new ArrayList();
            Hashtable tableSizeValues = new Hashtable();
            Hashtable tableSizeValueCount = new Hashtable();
            Hashtable tableSizeCoefficients = new Hashtable();
            string lastTable;
            int index;
            DateTime lastDate;

            // Bail out if there was no data returned by the query
            if (tableGrowth.Rows.Count == 0)
            {
                return;
            }
            lastTable = (string)tableGrowth.Rows[0]["TableName"];

            //get the inital data in arrays for calculations. Sorted by Database.
            foreach (DataRow row in tableGrowth.Rows)
            {
                object value = row["TableName"];
                if (value != DBNull.Value)
                {
                    if ((string)value != lastTable)
                    {
                        tableSizeValues.Add(lastTable, tableSize);
                        tableSizeValueCount.Add(lastTable, tableSize.Count);
                        tableSize = new ArrayList();
                        lastTable = (string)value;
                    }
                }
                else
                {
                    lastTable = "";
                    tableSize = new ArrayList();
                }
                value = row["TotalSize"];

                if (value != DBNull.Value)
                {
                    tableSize.Add((double)value);
                }
                else
                {
                    tableSize.Add(0.0);
                }
            }
            //add the last values.
            tableSizeValues.Add(lastTable, tableSize);
            tableSizeValueCount.Add(lastTable, tableSize.Count);

            ForecastType forecastType = (ForecastType)((ValueListItem)localReportData.reportParameters["ForecastType"]).DataValue;

            switch (forecastType)
            {
                case ForecastType.Linear:
                    {
                        orderPolynomial = 1;
                        break;
                    }
                case ForecastType.Parabolic:
                    {
                        orderPolynomial = 2;
                        break;
                    }
                case ForecastType.Cubic:
                    {
                        orderPolynomial = 3;
                        break;
                    }
                case ForecastType.Quartic:
                    {
                        orderPolynomial = 4;
                        break;
                    }
                default:
                    {
                        orderPolynomial = 3;
                        break;
                    }
            }

            //Calculate Database Size forecast coefficients
            CalculateCoefficients(tableSizeValues, tableSizeValueCount, tableSizeCoefficients);

            // add a column to the DataTable for the function x and y values.
            tableGrowth.Columns.Add("TableGrowthForecast", Type.GetType("System.Double"));

            //Add the forecasted data to the original data rows
            //The formula for creating the forecast curve is C0 + C1x + C2x^2 + C3x^3
            double[] coefficients;
            index = 1;
            lastTable = (string)tableGrowth.Rows[0]["TableName"];

            foreach (DataRow row in tableGrowth.Rows)
            {
                // Set the values for the existing rows.
                //This only works because the data from the stored proc is sorted by table name.
                if ((string)row["TableName"] != lastTable)
                {
                    lastTable = (string)row["TableName"];
                    index = 1;
                }

                double tableGrowthForecast;
                coefficients = (double[])tableSizeCoefficients[row["TableName"]];
                switch (forecastType)
                {
                    case ForecastType.Linear:
                        {
                            tableGrowthForecast = coefficients[1] * index + coefficients[0];
                            break;
                        }
                    case ForecastType.Parabolic:
                        {
                            tableGrowthForecast = coefficients[0] + (coefficients[1]*index) +
                                                         (coefficients[2]*(Math.Pow(index, 2)));
                            break;
                        }
                    case ForecastType.Quartic:
                        {
                            tableGrowthForecast = coefficients[0] + (coefficients[1] * index) + (coefficients[2] * (Math.Pow(index, 2))) + (coefficients[3] * (Math.Pow(index, 3))) + (coefficients[4] * (Math.Pow(index, 4)));
                            break;
                        }
                    default: // Cubic
                        {
                            tableGrowthForecast = coefficients[0] + (coefficients[1] * index) + (coefficients[2] * (Math.Pow(index, 2))) + (coefficients[3] * (Math.Pow(index, 3)));
                            break;
                        }
                }

                row["TableGrowthForecast"] = tableGrowthForecast >= 0 ? tableGrowthForecast : 0;
                index++;
                row.AcceptChanges();
            }
            ArrayList uniqueTables = new ArrayList();
            string lastTableName = "";

            // loop through one of the hash tables to get a list of unique database names.
            foreach (DictionaryEntry value in tableSizeValues)
            {
                if (lastTableName != (string)value.Key)
                {
                    uniqueTables.Add(value.Key);
                }
            }
            //Get the last date returned from the query
            object date = tableGrowth.Rows[tableGrowth.Rows.Count - 1]["LastCollectioninInterval"];
            lastDate = (DateTime)date;

            // Add new Rows containing data forecasted for the future.
            foreach (string database in uniqueTables)
            {
                DateTime lastModifiedDate = lastDate;

                //loop through each database name and add x new rows
                int count = (int)tableSizeValueCount[database];
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
                    DataRow row = tableGrowth.NewRow();
                    switch (forecastType)
                    {
                        case ForecastType.Linear:
                            {
                                coefficients = (double[])tableSizeCoefficients[database];
                                row["TableGrowthForecast"] = coefficients[1] * xValue + coefficients[0];
                                break;
                            }
                        case ForecastType.Parabolic:
                            {
                                coefficients = (double[])tableSizeCoefficients[database];
                                row["TableGrowthForecast"] = coefficients[0] + (coefficients[1] * xValue) + (coefficients[2] * (Math.Pow(xValue, 2)));
                                break;
                            }
                        case ForecastType.Cubic:
                            {
                                coefficients = (double[])tableSizeCoefficients[database];
                                row["TableGrowthForecast"] = coefficients[0] + (coefficients[1] * xValue) + (coefficients[2] * (Math.Pow(xValue, 2))) + (coefficients[3] * (Math.Pow(xValue, 3)));
                                break;
                            }
                        case ForecastType.Quartic:
                            {
                                coefficients = (double[])tableSizeCoefficients[database];
                                row["TableGrowthForecast"] = coefficients[0] + (coefficients[1] * xValue) + (coefficients[2] * (Math.Pow(xValue, 2))) + (coefficients[3] * (Math.Pow(xValue, 3))) + (coefficients[4] * (Math.Pow(xValue, 4)));
                                break;
                            }
                        default:
                            {
                                coefficients = (double[])tableSizeCoefficients[database];
                                row["TableGrowthForecast"] = coefficients[0] + (coefficients[1] * xValue) + (coefficients[2] * (Math.Pow(xValue, 2))) + (coefficients[3] * (Math.Pow(xValue, 3)));
                                break;
                            }
                    }
                    // fill in the other columns with default data.
                    lastModifiedDate = CalculateCurrentDate(lastModifiedDate);
                    row["LastCollectioninInterval"] = lastModifiedDate;
                    row["TableName"] = database;
                    row["TotalSize"] = DBNull.Value;
                    tableGrowth.Rows.Add(row);
                    row.AcceptChanges();
                }
            }            
        }

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

                for (int i = 0; i < count; i++)
                {
                    yValues[i] = (double)((ArrayList)dataValues.Value)[i];
                }
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
        /// <summary>
        /// Show the appropriate sample size options based on _customDates.
        /// The goal is to prevent the user from trying to plot too many or two few
        /// data points.  The limits are arbitrary.  
        /// </summary>
        protected override void UpdateCustomSampleSizes()
        {
            DateTime first = customDates[0].UtcStart.ToLocalTime();
            DateTime last = customDates[customDates.Count - 1].UtcEnd.ToLocalTime();
            TimeSpan span = new TimeSpan(0);

            foreach (DateRangeOffset range in customDates)
            {
                span += range.UtcEnd.ToLocalTime().Date - range.UtcStart.ToLocalTime().Date;
            }
            if (span.TotalDays < 500.0) sampleSizeCombo.Items.Add(sampleDays);
            if (first.Month != last.Month || first.Year != last.Year) sampleSizeCombo.Items.Add(sampleMonths);
            if (first.Year != last.Year) sampleSizeCombo.Items.Add(sampleYears);
        }

        /// <summary>
        /// Show the appropriate sample size options based on the period.
        /// </summary>
        /// <param name="selectDefault"></param>
        protected override void UpdateSampleSizes(bool selectDefault)
        {
            ValueListItem selected = (ValueListItem)sampleSizeCombo.SelectedItem;
            
            sampleSizeCombo.Items.Clear();

            if (periodCombo.SelectedItem == periodToday 
                || periodCombo.SelectedItem == period7 
                || periodCombo.SelectedItem == period30)
            {
                sampleSizeCombo.Items.Add(sampleDays);
            }
            else if (periodCombo.SelectedItem == period365)
            {
                sampleSizeCombo.Items.Add(sampleDays);
                sampleSizeCombo.Items.Add(sampleMonths);
            }
            else if (periodCombo.SelectedItem == periodSetCustom)
            {
                UpdateCustomSampleSizes();
            }
            else
            {
                // Unexpected
                throw new ApplicationException("An unexpected report period value was selected.");
            }

            if ((selected != null) && (!selectDefault && sampleSizeCombo.Items.Contains(selected)))
            {
                sampleSizeCombo.SelectedItem = selected;
            }
            else
            {
                sampleSizeCombo.SelectedIndex = 0;
            }
        }

        public override void ResetFilterCriteria()
        {
            base.ResetFilterCriteria();
            sampleSizeCombo.SelectedIndex = 0;
            MultiTables = null;
            tablesTextbox.Text = "";
            forecastUnits.Text = "10";
            forecastTypeCombo.SelectedItem = forecastLinear;
            sampleSizeCombo.SelectedIndex = sampleSizeCombo.Items.IndexOf(sampleDays);
        }

        // Currently selected list of tables.
        // Each triple has a schema, table name, and bool indicating if system table.
        private List<Triple<string, string, bool>> curTables; // Current multi-selection of tables.
        private List<Triple<string, string, bool>> MultiTables
        {
            get { return curTables; }
            set
            {
                if (!ListsAreEqual(curTables, value))
                {
                    curTables = value;
                    MakeCSVList(tablesTextbox, MakeTableNameArray(curTables, true));
                }
            }
        }

        private static List<string> MakeTableNameArray(List<Triple<string, string, bool>> list, bool withSchema)
        {
            List<string> tables = new List<string>();

            if (list != null && list.Count != 0)
            {
                foreach (Triple<string, string, bool> triple in list)
                {
                    if (withSchema)
                    {
                        tables.Add(triple.First + "." + triple.Second);
                    }
                    else
                    {
                        tables.Add(triple.Second);
                    }
                }
            }
            return tables;
        }


        private void tableBrowseButton_Click(object sender, EventArgs e)
        {
            if (Databases != null)
            {
                TableBrowserDialog dlg = new TableBrowserDialog(((MonitoredSqlServer)((ValueListItem)instanceCombo.SelectedItem).DataValue).Id, Databases[0]);
                dlg.CheckedTables = MultiTables;

                if (DialogResult.OK == dlg.ShowDialog(this))
                {
                    MultiTables = dlg.CheckedTables;
                }
            }
            else
            {
                ApplicationMessageBox.ShowInfo(this, "Select a database before selecting a table.");
            }
        }

        private void databaseTextbox_TextChanged(object sender, EventArgs e)
        {
            MultiTables = null;
        }

        private void forecastUnits_Leave(object sender, EventArgs e)
        {
            if (this.forecastUnits.Text.Equals(string.Empty))
            {
                this.forecastUnits.Text = this.forecastUnits.Minimum.ToString();
            }
        }
    }
}

