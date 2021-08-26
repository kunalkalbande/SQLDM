using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using BBS.TracerX;
using System.Data;

namespace Idera.SQLdm.DesktopClient.Views.Reports {
    /// <summary>
    /// This class contains data about a chart obtained by parsing an rdlc file.
    /// It is primarily used to determine what metrics are plotted on each chart
    /// so a message can be displayed if a given chart has insufficient data
    /// points to generate a line.
    /// </summary>
    internal class Chart {
        private static readonly Logger Log = Logger.GetLogger("Reports.Chart");

        public string Title;            // For debugging/logging.
        public string Name;             // Chart's internal name.
        public string DataSetName;      // Specifies which DataTable to use.
        public string[] ValueCols;      // DataTable columns whose values are charted.
        public string SeriesGroupCol;   // Column for which every unique value is a series on the chart.  Can be null.

        public override string ToString() {
            StringBuilder builder = new StringBuilder("Chart Title: ");
         
            builder.AppendLine(Title);

            builder.AppendLine("DataSetName = " + DataSetName);
            
            if (SeriesGroupCol == null) {
                builder.AppendLine("SeriesGroupCol = null");
            } else {
                builder.AppendLine("SeriesGroupCol = " + SeriesGroupCol);
            }

            foreach (string col in ValueCols) {
                builder.AppendLine("ValueCol = " + col);
            }

            builder.Length = builder.Length - 1;
            return builder.ToString();
        }

        public bool HasEnoughData(DataTable dataTable) {
            // For each series that can be plotted (i.e. each unique value found in the
            // SeriesGroupCol), keep a list of value columns that data has been found for.
            // Once we have found two non-null values in any value column for a given series,
            // the chart has enough data to draw a line segment and we can return true.
            using (Log.VerboseCall()) {
                List<string> colsWithData = new List<string>();
                Dictionary<string, List<string>> seriesData = new Dictionary<string, List<string>>();

                Log.Verbose("Chart title: ", Title);
                if (dataTable != null) {
                    foreach (DataRow row in dataTable.Rows) {
                        if (SeriesGroupCol != null) {
                            string curSeriesId = (string)row[SeriesGroupCol];
                            if (!seriesData.TryGetValue(curSeriesId, out colsWithData)) {
                                Log.Verbose("First record for series: ", curSeriesId);
                                colsWithData = new List<string>();
                                seriesData.Add(curSeriesId, colsWithData);
                            } else {
                                Log.Verbose("Another record for series: ", curSeriesId);
                            }
                        }

                        foreach (string colName in ValueCols) {
                            if (row[colName] != DBNull.Value) {
                                if (colsWithData.Contains(colName)) {
                                    // We just found the second row that has data for the
                                    // current series+column.
                                    Log.Verbose("Found second value for column: ", colName, ", returning true.");
                                    return true;
                                } else {
                                    Log.Verbose("Found first value for column: ", colName);
                                    colsWithData.Add(colName);
                                }
                            }
                        }
                    }
                } else {
                    Log.Warn("dataTable is null!");
                }

                Log.Verbose("Returning false.");
                return false;
            } // using log
        }

        public bool HasEnoughRows(DataTable dataTable) {
            // Return true if there are at least two rows for any series  
            // (i.e. each unique value found in the SeriesGroupCol).
            using (Log.VerboseCall()) {
                Log.Verbose("Chart title: ", Title);
                if (dataTable != null) {
                    if (SeriesGroupCol == null) {
                        return dataTable.Rows.Count > 1;
                    } else {
                        List<string> seriesList = new List<string>();
                        foreach (DataRow row in dataTable.Rows) {
                            string series = (string)row[SeriesGroupCol];
                            if (seriesList.Contains(series)) {
                                Log.Verbose("Found another record for series: ", series);
                                return true;
                            } else {
                                Log.Verbose("Found first record for series: ", series);
                                seriesList.Add(series);
                            }
                        }
                    }
                } else {
                    Log.Warn("dataTable is null!");
                }

                Log.Verbose("Returning false.");
                return false;
            } // using log
        }


        public void LoadFromXml(XmlNode chartNode, XmlNamespaceManager ns) {
            Name = chartNode.Attributes["Name"].Value;

            XmlNode titleNode = chartNode.SelectSingleNode("ns:Title/ns:Caption", ns);
            Title = titleNode.InnerText;

            XmlNode dataSetNode = chartNode.SelectSingleNode("ns:DataSetName", ns);
            DataSetName = dataSetNode.InnerText;

            // There should only be zero or one SeriesGrouping.
            XmlNode seriesNode = chartNode.SelectSingleNode("ns:SeriesGroupings//ns:GroupExpression", ns);
            if (seriesNode != null) {
                // E.g. "=Fields!InstanceName.Value"
                SeriesGroupCol = GetFieldName(seriesNode.InnerText);
            }

            XmlNodeList valueNodes = chartNode.SelectNodes("ns:ChartData/ns:ChartSeries/ns:DataPoints/ns:DataPoint/ns:DataValues/ns:DataValue/ns:Value", ns);
            ValueCols = new string[valueNodes.Count];
            int ndx = 0;
            foreach (XmlNode valueNode in valueNodes) {
                ValueCols[ndx] = GetFieldName(valueNode.InnerText);
                ++ndx;
            }

            // Modify the chart setting that causes each data point to have its own label on the
            // X axis for PR 2011208.  Caller will save the doc to a new file.
            XmlNode scalarNode = chartNode.SelectSingleNode("ns:CategoryAxis/ns:Axis/ns:Scalar", ns);
            if (scalarNode != null) {
                scalarNode.InnerText = "false";
            }

        }

        static private string GetFieldName(string exp) {
            // exp is an expression that might look like this:
            // =Fields!PacketsReceived.Value + Fields!PacketsSent.Value + Fields!PacketErrors.Value</Value>
            // Return the first thing between "Fields!" and ".Value".
            int start = exp.IndexOf("Fields!") + 7;
            int end = exp.IndexOf(".Value", start);
            return exp.Substring(start, end - start);
        }
    }
}
