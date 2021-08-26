using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Idera.SQLdm.Common.Objects
{
    [Serializable]
    public class CustomReport
    {
        #region enums
        public enum Operation: int
        {
            Delete = 1, 
            Update, 
            Append, 
            Rename, 
            New
        }
        public enum CounterType:int
        {
            Server = 0,
            OS,
            Custom,
            //SQLdm 8.5 (Ankit Srivastava): for Virtualization Counters - starts here
            Virtualization
            //SQLdm 8.5 (Ankit Srivastava): for Virtualization Counters - ends here
        }
        public enum Aggregation
        {
            MaxSinceLastCollection = 0,
            AverageSinceLastCollection,
            PerMinuteSinceLastCollection
        }
        #endregion
        private string _reportName;
        private string _shortDescription;
        private string _reportRDL;        
        //private string _customReportRDL;
        private bool _showTable = true;
        private int _reportID;
        private CultureInfo ReportCultureInfo = null;
        private bool _showTopServers = false;
        private SortedDictionary<int, CustomReportMetric> _metrics = new SortedDictionary<int, CustomReportMetric>();

        public CustomReport(string reportName):this(reportName, null, null,false)
        {

        }

        public CustomReport(string reportName, string ShortDesc, string ReportRDL, bool showTopServers)
        {
            _reportName = reportName;
            _shortDescription = ShortDesc;
            _reportRDL = ReportRDL;
            _showTopServers = showTopServers;
            if (showTopServers)
            {
                _showTable = false;
            }
            else
            {
                _showTable = GetShowTables(_reportRDL);
            }

            ReportCultureInfo = new CultureInfo("en-US");
        }
        
        /// <summary>
        /// determine if the display tables parameter is set in the passed rdl
        /// </summary>
        /// <param name="reportText"></param>
        /// <returns></returns>
        private static bool GetShowTables(string reportText)
        {
            if(reportText == null) return true;

            int start = -1;
            int end = -1;
            
            string resultValue = "true";

            start = reportText.IndexOf("<ReportParameter Name=\"DisplayTables\">");
            if (start <= 0) return true;

            start = reportText.IndexOf("<Value>=", start);
            if (start <= 0) return true;

            end = reportText.IndexOf("</Value>", start);
            if (end <= 0) return true;
            resultValue = reportText.Substring(start + 8, end - (start + 8));
            return bool.Parse(resultValue);
        }

        public int ID
        {
            get { return _reportID; }
            set { _reportID = value; }
        }
        /// <summary>
        /// The user wishes to see the table in the body of the report
        /// </summary>
        public bool ShowTable
        {
            get { return _showTable; }
            set { _showTable = value; }
        }

        public bool ShowTopServers
        {
            get { return _showTopServers; }
            set { _showTopServers = value; }
        }

        public string Name
        {
            get { return _reportName; }
            set { _reportName = value; }
        }

        public string ShortDescription
        {
            get { return _shortDescription; }
            set { _shortDescription = value; }
        }

        //public string CustomReportRDL
        //{
        //    get { return _customReportRDL; }
        //    set { _customReportRDL = value; }
        //}

        public string ReportRDL
        {
            get { return _reportRDL; }
            set { _reportRDL = value; }
        }

        /// <summary>
        /// key is the graph number
        /// </summary>
        public SortedDictionary<int, CustomReportMetric> Metrics
        {
            get { return _metrics; }
            set { _metrics = value; }
        }

        #region Report XML Sections
        public string GetStoredProcedureName()
        {
            return "p_GetCustomReportsDataSet";
        }
        /// <summary>
        /// get the sql select for the custom report.
        /// Customized for the selected metrics
        /// </summary>
        /// <returns>a sql batch commandtext</returns>
        public string GetSQLSelectDelete()
        {
            if (_metrics == null) return "";

            string specificMetricSelect = "";
            string returnString = "declare @UTCOffset1 int "
                                  + "select @UTCOffset1 = @UTCOffset "
                                  + "select m.InstanceName "
                                  + ",dbo.fn_RoundDateTime(@Interval, max(dateadd(hh, @UTCOffset1, s1.[UTCCollectionDateTime]))) as [LastCollectioninInterval], ";

                foreach(KeyValuePair<int, CustomReportMetric> kv in _metrics)
                {
                    string averageOverTimeTemplate = "sum({prefix}.{metric} * TimeDeltaInSeconds) / nullif(sum(case when {prefix}.{metric} is not null then TimeDeltaInSeconds else 0 end),0) as {metric}, ";
                    string averagePerMinuteTemplate = "sum(convert(float,{prefix}.{metric})) / nullif((sum(convert(float,case when {prefix}.{metric} is not null then TimeDeltaInSeconds else 0 end)) / 60) ,0) as {metric}, ";
                    string maxTemplate = "max(convert(float,{prefix}.{metric})) as {metric}, ";
                    //string rawCounterTemplate = "avg(convert(float,{prefix}.{metric})) as {metric}, ";

                    string prefix = "";
                    string metricName = kv.Value.MetricName;
                    
                    switch(kv.Value.Source)
                    {
                        case CounterType.OS:
                            prefix = "o";
                            break;
                        case CounterType.Server:
                            prefix = "s1";
                            break;
                        case CounterType.Custom:
                            break;             
                //SQLdm 8.5 (Ankit Srivastava): for Virtualization Counters - starts here
                        case CounterType.Virtualization:
                            prefix="vm";
                            break;      
                //SQLdm 8.5 (Ankit Srivastava): for Virtualization Counters - ends here
                    }

                    switch (kv.Value.Aggregation)
                    {
                        case Aggregation.AverageSinceLastCollection:
                            specificMetricSelect += averageOverTimeTemplate.Replace("{prefix}", prefix).Replace("{metric}", metricName);
                            break;
                        //case Aggregation.RawCounter:
                        //    specificMetricSelect += rawCounterTemplate.Replace("{prefix}", prefix).Replace("{metric}", metricName);
                        //    break;
                        case Aggregation.MaxSinceLastCollection:
                            specificMetricSelect += maxTemplate.Replace("{prefix}", prefix).Replace("{metric}", metricName);
                            break;
                        case Aggregation.PerMinuteSinceLastCollection:
                            specificMetricSelect += averagePerMinuteTemplate.Replace("{prefix}", prefix).Replace("{metric}",metricName);
                            break;
                    }
                }
                specificMetricSelect = specificMetricSelect.TrimEnd(new char[] { ',', ' ' });
                //specificMetricSelect.TrimEnd(' ');
                //specificMetricSelect.TrimEnd(',');

                //+ "sum(o.[ProcessorTimePercent] * TimeDeltaInSeconds) / nullif(sum(case when [ProcessorTimePercent] is not null then TimeDeltaInSeconds else 0 end),0) as OSProcessorTimePercent, "
                //+ "sum(s1.[CPUActivityPercentage] * TimeDeltaInSeconds) / nullif(sum(case when [CPUActivityPercentage] is not null then TimeDeltaInSeconds else 0 end),0) as SQLCPUActivityPercentage, "
                //+ "sum(o.[ProcessorQueueLength] * TimeDeltaInSeconds) / nullif(sum(case when [ProcessorQueueLength] is not null then TimeDeltaInSeconds else 0 end),0) as OSProcessorQueueLength, "
                //+ "sum(convert(float,s1.[SqlCompilations])) / nullif((sum(convert(float,case when s1.[SqlCompilations] is not null then TimeDeltaInSeconds else 0 end)) / 60) ,0) as SqlCompilations, "
                //+ "sum(convert(float,s1.[SqlRecompilations])) / nullif((sum(convert(float,case when s1.[SqlRecompilations] is not null then TimeDeltaInSeconds else 0 end)) / 60) ,0) as SqlRecompilations, "
                //+ "sum(convert(float,s1.[LockWaits])) / nullif((sum(convert(float,case when s1.[LockWaits] is not null then TimeDeltaInSeconds else 0 end)) / 60) ,0) as LockWaits,"
                //+ "sum(convert(float,s1.[TableLockEscalations])) / nullif((sum(convert(float,case when s1.[TableLockEscalations] is not null then TimeDeltaInSeconds else 0 end)) / 60) ,0) as TableLockEscalations "
                returnString += specificMetricSelect + " ";

                returnString += "from [MonitoredSQLServers] m (nolock) "
                + "left join [ServerStatistics] s1 (nolock) "
                + "on m.[SQLServerID] = s1.[SQLServerID] "
                + "left join [OSStatistics] o (nolock) "
                + "on o.[SQLServerID] = s1.[SQLServerID] and o.[UTCCollectionDateTime] = s1.[UTCCollectionDateTime] "
                //SQLdm 8.5 (Ankit Srivastava): for Virtualization Counters - starts here
                + "left join [VMStatistics] vm (nolock) "
                + "on vm.[SQLServerID] = s1.[SQLServerID] and vm.[UTCCollectionDateTime] = s1.[UTCCollectionDateTime] "
                //SQLdm 8.5 (Ankit Srivastava): for Virtualization Counters - ends here
                + "where s1.[SQLServerID] = @ServerID and s1.[UTCCollectionDateTime] between @UTCStart and @UTCEnd "
                + "group by [InstanceName] ,datepart(yy, dateadd(hh, @UTCOffset1, s1.[UTCCollectionDateTime])) "
                + ",case when @Interval = 3 then datepart(mm,dateadd(hh, @UTCOffset1, s1.[UTCCollectionDateTime])) else datepart(yy,dateadd(hh, @UTCOffset1, s1.[UTCCollectionDateTime])) end "
                + ",case when @Interval = 2 then datepart(dd,dateadd(hh, @UTCOffset1, s1.[UTCCollectionDateTime])) else datepart(yy,dateadd(hh, @UTCOffset1, s1.[UTCCollectionDateTime])) end "
                + ",case when @Interval = 1 then datepart(hh,dateadd(hh, @UTCOffset1, s1.[UTCCollectionDateTime])) else datepart(yy,dateadd(hh, @UTCOffset1, s1.[UTCCollectionDateTime])) end "
                + ",case when @Interval = 0 then datepart(mi,dateadd(hh, @UTCOffset1, s1.[UTCCollectionDateTime])) else datepart(yy,dateadd(hh, @UTCOffset1, s1.[UTCCollectionDateTime])) end";
            return returnString;
        }
        /// <summary>
        /// Get the rdl describing charts for the selected counters to be viewed in this report
        /// </summary>
        /// <returns></returns>
        public string GetCustomReportCharts()
        {
            string returnString;
            int intChartNumber = 1;
            //double dblWidth = 10.25;
            const string WIDTH = "10.25in";
            const string YAXISFORMAT = "N2";
            const string MetricValue = "=Sum(IIf(IsNothing(Fields!{0}), 0, Fields!{0}.Value))";
            double dblTopOfChart = 1.25;
            //const string TOP = "1.25in";
            //double dblTop = 1.25;
            
            double dblChartHeight = 4.0;
            

            using (MemoryStream stream = new MemoryStream())
            {
                using (System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(stream, Encoding.UTF8))
                {

                    // Causes child elements to be indented
                    writer.Formatting = System.Xml.Formatting.Indented;

                    foreach (CustomReportMetric metric in Metrics.Values)
                    {
                        string AggregationString = null;
                        switch (metric.Aggregation)
                        {
                            case Aggregation.AverageSinceLastCollection:
                                AggregationString = " (Avg)";
                                break;
                            case Aggregation.MaxSinceLastCollection:
                                AggregationString = " (Max)";
                                break;
                            case Aggregation.PerMinuteSinceLastCollection:
                                AggregationString = " (per Minute)";
                                break;
                            //case Aggregation.RawCounter:
                            //    AggregationString = " (Raw)";
                            //    break;

                        }

                      writer.WriteStartElement("Chart");
                      writer.WriteAttributeString("Name", null,
                                                  string.Format("chart{0}", intChartNumber));
                      writer.WriteStartElement("Legend");
                      writer.WriteElementString("Visible", null, "true");
                      writer.WriteStartElement("Style");
                      writer.WriteStartElement("BorderStyle");
                      writer.WriteElementString("Default", null, "Solid");
                      writer.WriteEndElement(); //end border style
                      writer.WriteEndElement(); //end style
                      writer.WriteElementString("Position", null, "BottomCenter");
                      writer.WriteElementString("Layout", null, "Table");
                      writer.WriteEndElement(); //end Legend
                      writer.WriteStartElement("CategoryAxis");
                      writer.WriteStartElement("Axis");
                      writer.WriteElementString("Title", null, "");
                      writer.WriteElementString("Style", null, "");
                      writer.WriteStartElement("MajorGridLines");
                      writer.WriteStartElement("Style");
                      writer.WriteStartElement("BorderStyle");
                      writer.WriteElementString("Default", null, "Solid");
                      writer.WriteEndElement(); //end border style
                      writer.WriteEndElement(); //end style
                      writer.WriteEndElement(); //end MajorgridLines

                      writer.WriteStartElement("MinorGridLines");
                      writer.WriteStartElement("Style");
                      writer.WriteStartElement("BorderStyle");
                      writer.WriteElementString("Default", null, "Solid");
                      writer.WriteEndElement(); //end border style
                      writer.WriteEndElement(); //end style
                      writer.WriteEndElement(); //end MinorGridLines
                      writer.WriteElementString("MajorTickMarks", null, "Outside");
                      writer.WriteElementString("Min", null, "0");
                      writer.WriteElementString("MajorInterval", null,
                                                "=Code.GetTickMarks(Count(Fields!InstanceName.Value))");
                      writer.WriteElementString("Visible", null, "true");
                      writer.WriteEndElement(); //end axis
                      writer.WriteEndElement(); //end category axis
                      //  <ZIndex>8</ZIndex>
                      writer.WriteElementString("DataSetName", null, "CounterSummary");
                      writer.WriteStartElement("Visibility");
                      writer.WriteElementString("Hidden", null, "=IIF(CountRows(\"CounterSummary\") > 0, false, true)");
                      writer.WriteEndElement();
                      writer.WriteStartElement("PlotArea");
                      writer.WriteStartElement("Style");
                      writer.WriteElementString("BackgroundColor", null, "#e5e5e5");
                      writer.WriteStartElement("BorderStyle");
                      writer.WriteElementString("Default", null, "Solid");
                      writer.WriteEndElement(); //end border style
                      writer.WriteEndElement(); //end style
                      writer.WriteEndElement(); //end plot area

                      writer.WriteStartElement("ThreeDProperties");
                      writer.WriteElementString("Rotation", null, "30");
                      writer.WriteElementString("Inclination", null, "30");
                      writer.WriteElementString("Shading", null, "Simple");
                      writer.WriteElementString("WallThickness", null, "50");
                      writer.WriteEndElement(); //end 3d properties

                      writer.WriteElementString("PointWidth", null, "0");

                      writer.WriteStartElement("SeriesGroupings");
                      writer.WriteStartElement("SeriesGrouping");
                      writer.WriteStartElement("StaticSeries");
                      writer.WriteStartElement("StaticMember");
                      writer.WriteElementString("Label", null, metric.MetricDescription + AggregationString);
                      writer.WriteEndElement(); //End Static Member
                      writer.WriteEndElement(); //End Static Series
                      writer.WriteEndElement(); //End Series Grouping
                      writer.WriteEndElement(); //End Series Groupings

                      if (intChartNumber == 1)
                      {
                          dblTopOfChart = 1.25;
                      }
                      else
                      {
                          dblTopOfChart += dblChartHeight;
                      }

                      writer.WriteElementString("Top", null, string.Format(ReportCultureInfo, "{0}in", dblTopOfChart)); //top

                      writer.WriteElementString("Subtype", null, "Plain");//top

                      writer.WriteStartElement("ValueAxis");
                      writer.WriteStartElement("Axis");
                      writer.WriteElementString("Title", null, "");
                      writer.WriteStartElement("Style");
                      writer.WriteElementString("Format", null, YAXISFORMAT );
                      writer.WriteEndElement(); //End Style
                      writer.WriteStartElement("MajorGridLines");
                      writer.WriteElementString("ShowGridLines", null, "true");
                      writer.WriteStartElement("Style");
                      writer.WriteStartElement("BorderStyle");
                      writer.WriteElementString("Default", null, "Solid");
                      writer.WriteEndElement(); //end border style
                      writer.WriteEndElement(); //end style
                      writer.WriteEndElement(); //end Major grid lines

                      writer.WriteStartElement("MinorGridLines");
                      writer.WriteStartElement("Style");
                      writer.WriteStartElement("BorderStyle");
                      writer.WriteElementString("Default", null, "Solid");
                      writer.WriteEndElement(); //end border style
                      writer.WriteEndElement(); //end style
                      writer.WriteEndElement(); //end Minor grid lines

                      writer.WriteElementString("MajorTickMarks", null, "Outside");
                      writer.WriteElementString("Min", null, string.Format("=Code.GetMinY(Min(Fields!{0}.Value))", metric.MetricName));          //MIN
                      writer.WriteElementString("Max", null, string.Format("=Code.GetMaxY(Max(Fields!{0}.Value))", metric.MetricName));        //max
                      writer.WriteElementString("Margin", null, "true");
                      writer.WriteElementString("Visible", null, "true");
                      writer.WriteElementString("Scalar", null, "true");

                      writer.WriteEndElement(); //end axis
                      writer.WriteEndElement(); //end value axis

                      writer.WriteElementString("Type", null, "Line");
                      writer.WriteElementString("Width", null, WIDTH);

                      writer.WriteStartElement("CategoryGroupings");
                      writer.WriteStartElement("CategoryGrouping");
                      writer.WriteStartElement("DynamicCategories");
                      writer.WriteStartElement("Grouping");
                      writer.WriteAttributeString("Name", string.Format("chart{0}_CategoryGroup1", intChartNumber));
                      writer.WriteStartElement("GroupExpressions");
                      writer.WriteElementString("GroupExpression", "=Fields!LastCollectioninInterval.Value");
                      writer.WriteEndElement(); //end group expressions
                      writer.WriteEndElement(); //end grouping
                      writer.WriteStartElement("Sorting");
                      writer.WriteStartElement("SortBy");
                      writer.WriteElementString("SortExpression", "=Fields!LastCollectioninInterval.Value");
                      writer.WriteElementString("Direction", "Ascending");
                      writer.WriteEndElement(); //end sort by
                      writer.WriteEndElement(); //end sorting
                      writer.WriteElementString("Label", "=Code.getTimeLabelLocal(Parameters!Interval.Value, Fields!LastCollectioninInterval.Value)");

                      writer.WriteEndElement();//end dynamic categories
                      writer.WriteEndElement();//end category grouping
                      writer.WriteEndElement();//end category groupings

                      writer.WriteElementString("Palette", "Default");

                      writer.WriteStartElement("ChartData");
                      writer.WriteStartElement("ChartSeries");
                      writer.WriteStartElement("DataPoints");
                      writer.WriteStartElement("DataPoint");
                      writer.WriteStartElement("DataValues");
                      writer.WriteStartElement("DataValue");
                      writer.WriteElementString("Value", string.Format(MetricValue, metric.MetricName));
                      writer.WriteEndElement();//end Data value
                      writer.WriteEndElement();//end Data values
                      writer.WriteElementString("DataLabel", "");
                      
                      writer.WriteStartElement("Marker");
                      writer.WriteElementString("Type", "Circle");
                      writer.WriteElementString("Size", "6pt");
                      writer.WriteEndElement();//end market

                      writer.WriteEndElement();//end data point
                      writer.WriteEndElement();//end data points
                      writer.WriteEndElement();//end ChartSeries
                      writer.WriteEndElement();//end ChartData

                      writer.WriteStartElement("Style");
                      writer.WriteElementString("BackgroundColor", null, "White");
                      writer.WriteElementString("FontWeight", null, "600");
                      writer.WriteEndElement(); //End Style

                      writer.WriteStartElement("Title");
                      
                      writer.WriteElementString("Caption", null, metric.MetricDescription + AggregationString);
                      writer.WriteStartElement("Style");
                      writer.WriteElementString("FontSize", null, "12pt");
                      writer.WriteElementString("FontWeight", null, "700");
                      writer.WriteEndElement(); //End Style
                      writer.WriteEndElement(); //End Title
                      writer.WriteElementString("Height", null, "4.0in");
                      
                      writer.WriteEndElement(); //End Chart
                      intChartNumber++;
                  }
                  writer.Flush();
                  stream.Flush();
                  stream.Position = 0;

                  using (StreamReader reader = new StreamReader(stream))
                  {
                      returnString = reader.ReadToEnd();
                  }
                }
            }
            return returnString;
        }
        /// <summary>
        /// Get the table showing all selected counters for this custom report
        /// </summary>
        /// <returns></returns>

        //START: SQLdm 10.0 (Tarun Sapra)-Filling the table template in the rdl
        /// <summary>
        /// Get the tables showing all selected counters for this top servers custom report
        /// </summary>
        /// <returns></returns>
        public string GetTopSeversCustomReportTables()
        {

            double dblTableTop = 29.75;
            const double dblReportWidth = 10.25;
            const double dblDateColumnWidth = 1.8;
            const double dblRowHeight = 0.25;

            string returnString = null;
            string TOP = string.Format(ReportCultureInfo, "{0}in", dblTableTop);
            string WIDTH = string.Format(ReportCultureInfo, "{0}in", dblReportWidth);
            string DATECOLUMNWIDTH = string.Format(ReportCultureInfo, "{0}in", dblDateColumnWidth);
            string ROWHEIGHT = string.Format(ReportCultureInfo, "{0}in", dblRowHeight);

            int tableNum = 0;
            int textBoxNum = 8;
            double distFromTop = 0.1;
            //double dblAvailableSpace = dblReportWidth - dblDateColumnWidth;

            const string BACKCOLOR_FirstHalf = @"=IIf(RowNumber(""table";
            const string BACKCOLOR_SecondHalf = @""") Mod 2 = 0, ""#f0f0f0"", ""White"")";
            //const string MetricValue = "=IIf(IsNothing(Fields!{0}.Value), \"N/A\", Fields!{0}.Value)";
            //const string sortExpression = "=IIf(IsNothing(Fields!{0}.Value), 0, Fields!{0}.Value)";            

            dblTableTop = 1.25 + (Metrics.Count * 4);
            using (MemoryStream stream = new MemoryStream())
            {
                //FileStream stream;
                //stream = System.IO.File.OpenWrite("C:\\RDLGen3.rdl");

                using (System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(stream, Encoding.UTF8))
                {
                    // Causes child elements to be indented
                    writer.Formatting = System.Xml.Formatting.Indented;
                    //writer.WriteStartElement("Rectangle");
                    //writer.WriteAttributeString("Name", null, "rectangleForDisplayingReports");
                    //writer.WriteElementString("Top", null, 0.5 + "in");
                    foreach (CustomReportMetric metric in _metrics.Values)
                    {
                        tableNum++;
                        string metricName = metric.MetricName;
                        string metricDesc = metric.MetricDescription;
                        #region tableHeading
                        writer.WriteStartElement("Textbox");
                        writer.WriteAttributeString("Name", null, "textbox" + textBoxNum.ToString());
                        textBoxNum++;
                        writer.WriteElementString("Top", null, distFromTop.ToString() + "in");
                        distFromTop += 0.25;
                        writer.WriteStartElement("Style");
                        writer.WriteElementString("FontSize", null, "12pt");
                        writer.WriteElementString("FontWeight", null, "700");
                        writer.WriteElementString("PaddingLeft", null, "2pt");
                        writer.WriteElementString("PaddingRight", null, "2pt");
                        writer.WriteElementString("PaddingTop", null, "2pt");
                        writer.WriteElementString("PaddingBottom", null, "2pt");
                        writer.WriteEndElement();//Style
                        writer.WriteElementString("CanGrow", null, "true");
                        writer.WriteElementString("Value", null, "Top Servers By "+metricDesc.ToString());
                        writer.WriteEndElement();
                        #endregion  
                        #region tableDef
                        writer.WriteStartElement("Table");
                        writer.WriteAttributeString("Name", null, "table"+tableNum.ToString());
                        //<ZIndex>2</ZIndex>
                        writer.WriteElementString("DataSetName", null, "TopServersForCustomCounters");
                        writer.WriteElementString("Top", null, distFromTop.ToString()+"in");
                        distFromTop += 1;
                        //writer.WriteElementString("Width", null, WIDTH);
                        #region Details
                        writer.WriteStartElement("Details");
                        #region serverName
                        writer.WriteStartElement("TableRows");
                        writer.WriteStartElement("TableRow");
                        writer.WriteStartElement("Visibility");
                        writer.WriteElementString("Hidden", null, "=iif(Fields!CounterName.Value = \"" + metricName + "\" AND Fields!Value.Value <> \"-1\",false,true)");
                        writer.WriteEndElement();//end of visibility
                        writer.WriteStartElement("TableCells");
                        writer.WriteStartElement("TableCell");
                        writer.WriteStartElement("ReportItems");
                        writer.WriteStartElement("Textbox");
                        writer.WriteAttributeString("Name", null, "textbox"+textBoxNum.ToString());
                        writer.WriteElementString("rd:DefaultName", null, "textbox"+textBoxNum.ToString());
                        textBoxNum++;
                        writer.WriteStartElement("Style");
                        writer.WriteElementString("BackgroundColor", null, "White");
                        writer.WriteStartElement("BorderColor");
                        writer.WriteElementString("Default", null, "#dddddd");
                        writer.WriteEndElement();//end of style
                        writer.WriteStartElement("BorderStyle");
                        writer.WriteElementString("Default", null, "Solid");
                        writer.WriteEndElement();//end border style
                        writer.WriteElementString("TextAlign", null, "Center");
                        writer.WriteElementString("PaddingLeft", null, "2pt");
                        writer.WriteElementString("PaddingRight", null, "2pt");
                        writer.WriteElementString("PaddingTop", null, "2pt");
                        writer.WriteElementString("PaddingBottom", null, "2pt");
                        writer.WriteEndElement();//Style
                        writer.WriteElementString("CanGrow", null, "true");
                        writer.WriteElementString("Value", null, "=Fields!InstanceName.Value");
                        //<ZIndex>7</ZIndex>
                        writer.WriteEndElement();//end of Textbox
                        writer.WriteEndElement();//end of ReportItems
                        writer.WriteEndElement();//End of table cell
                        #endregion
                        #region MetricValue
                        writer.WriteStartElement("TableCell");
                        writer.WriteStartElement("ReportItems");
                        writer.WriteStartElement("Textbox");
                        writer.WriteAttributeString("Name", null, "textbox"+textBoxNum.ToString());
                        writer.WriteElementString("rd:DefaultName", null, "textbox"+textBoxNum.ToString());
                        textBoxNum++;
                        writer.WriteStartElement("Style");
                        writer.WriteElementString("BackgroundColor", null, "White");
                        writer.WriteStartElement("BorderColor");
                        writer.WriteElementString("Default", null, "#dddddd");
                        writer.WriteEndElement();//end of style
                        writer.WriteStartElement("BorderStyle");
                        writer.WriteElementString("Default", null, "Solid");
                        writer.WriteEndElement();//end border style
                        writer.WriteElementString("Format", null, "N2");
                        writer.WriteElementString("TextAlign", null, "Center");
                        writer.WriteElementString("PaddingLeft", null, "2pt");
                        writer.WriteElementString("PaddingRight", null, "2pt");
                        writer.WriteElementString("PaddingTop", null, "2pt");
                        writer.WriteElementString("PaddingBottom", null, "2pt");
                        writer.WriteEndElement();//Style
                        writer.WriteElementString("CanGrow", null, "true");
                        writer.WriteElementString("Value", null, "=Fields!Value.Value");
                        //              <ZIndex>7</ZIndex>
                        writer.WriteEndElement();//end of Textbox
                        writer.WriteEndElement();//end of ReportItems
                        writer.WriteEndElement();//End of table cell
                        #endregion
                        writer.WriteEndElement(); //end of table cells
                        writer.WriteElementString("Height", null, ROWHEIGHT);
                        writer.WriteEndElement();//end table row
                        writer.WriteEndElement();//end table rows
                        writer.WriteEndElement();//end details
                        #endregion
                        #region Headers
                        writer.WriteStartElement("Header");
                        writer.WriteStartElement("TableRows");
                        writer.WriteStartElement("TableRow");
                        writer.WriteStartElement("TableCells");
                        #region serverHeader
                        writer.WriteStartElement("TableCell");
                        writer.WriteStartElement("ReportItems");
                        writer.WriteStartElement("Textbox");
                        writer.WriteAttributeString("Name", null, "textbox"+textBoxNum.ToString());
                        writer.WriteElementString("rd:DefaultName", null, "textbox"+textBoxNum.ToString());
                        textBoxNum++;
                        writer.WriteStartElement("Style");
                        writer.WriteElementString("BackgroundColor", null, "#e5e5e5");
                        writer.WriteStartElement("BorderColor");
                        writer.WriteElementString("Default", null, "#bbbbbb");
                        writer.WriteEndElement(); //end border color
                        writer.WriteStartElement("BorderStyle");
                        writer.WriteElementString("Default", null, "Solid");
                        writer.WriteEndElement();//end border style                    
                        writer.WriteElementString("FontSize", null, "9pt");
                        writer.WriteElementString("TextAlign", null, "Center");
                        writer.WriteElementString("PaddingLeft", null, "2pt");
                        writer.WriteElementString("PaddingRight", null, "2pt");
                        writer.WriteElementString("PaddingTop", null, "2pt");
                        writer.WriteElementString("PaddingBottom", null, "2pt");
                        writer.WriteEndElement();//Style
                        writer.WriteElementString("CanGrow", null, "true");
                        writer.WriteElementString("Value", null, "Server Name");
                        //              <ZIndex>15</ZIndex>
                        writer.WriteEndElement();//end of textbox
                        writer.WriteEndElement();//end of ReportItems
                        writer.WriteEndElement();//End of table cell
                        #endregion
                        #region valueHeader
                        writer.WriteStartElement("TableCell");
                        writer.WriteStartElement("ReportItems");
                        writer.WriteStartElement("Textbox");
                        writer.WriteAttributeString("Name", null, "textbox"+textBoxNum.ToString());
                        writer.WriteElementString("rd:DefaultName", null, "textbox"+textBoxNum.ToString());
                        textBoxNum++;
                        writer.WriteStartElement("Style");
                        writer.WriteElementString("BackgroundColor", null, "#e5e5e5");
                        writer.WriteStartElement("BorderColor");
                        writer.WriteElementString("Default", null, "#bbbbbb");
                        writer.WriteEndElement(); //end border color
                        writer.WriteStartElement("BorderStyle");
                        writer.WriteElementString("Default", null, "Solid");
                        writer.WriteEndElement();//end border style 
                        writer.WriteElementString("FontSize", null, "9pt");
                        writer.WriteElementString("TextAlign", null, "Center");
                        writer.WriteElementString("PaddingLeft", null, "2pt");
                        writer.WriteElementString("PaddingRight", null, "2pt");
                        writer.WriteElementString("PaddingTop", null, "2pt");
                        writer.WriteElementString("PaddingBottom", null, "2pt");
                        writer.WriteEndElement();//Style
                        writer.WriteElementString("CanGrow", null, "true");
                        writer.WriteElementString("Value", null, metricDesc);
                        //              <ZIndex>15</ZIndex>
                        writer.WriteEndElement();//end of textbox
                        writer.WriteEndElement();//end of ReportItems
                        writer.WriteEndElement();//End of table cell
                        #endregion
                        writer.WriteEndElement(); //end of table cells
                        writer.WriteElementString("Height", null, ROWHEIGHT);
                        writer.WriteEndElement();//end table row
                        writer.WriteEndElement();//end table rows
                        writer.WriteEndElement();//end Header
                        #endregion
                        writer.WriteStartElement("TableColumns");
                        writer.WriteStartElement("TableColumn");
                        writer.WriteElementString("Width", null, DATECOLUMNWIDTH);
                        writer.WriteEndElement();
                        writer.WriteStartElement("TableColumn");
                        writer.WriteElementString("Width", null, DATECOLUMNWIDTH);
                        writer.WriteEndElement();
                        writer.WriteEndElement();//end table columns
                        writer.WriteEndElement();//end table
                        #endregion
                    }
                    //writer.WriteEndElement();//end of rectangle
                    writer.Flush();
                    stream.Flush();
                    stream.Position = 0;

                    using (StreamReader reader = new StreamReader(stream))
                    {
                        returnString = reader.ReadToEnd();
                    }
                }
            }
            return returnString;
        }
        //END: SQLdm 10.0 (Tarun Sapra)-Filling the table template in the rdl

        /// <summary>
        /// Get the table showing all selected counters for this custom report
        /// </summary>
        /// <returns></returns>
        public string GetCustomReportsTable()
        {

            double dblTableTop = 29.75;
            const double dblReportWidth = 10.25;
            const double dblDateColumnWidth = 1.8;
            const double dblRowHeight = 0.25;

            string returnString = null;
            string TOP = string.Format(ReportCultureInfo, "{0}in", dblTableTop);
            string WIDTH = string.Format(ReportCultureInfo, "{0}in", dblReportWidth);
            string DATECOLUMNWIDTH = string.Format(ReportCultureInfo, "{0}in", dblDateColumnWidth);
            string ROWHEIGHT = string.Format(ReportCultureInfo, "{0}in", dblRowHeight);

            double dblAvailableSpace = dblReportWidth - dblDateColumnWidth;

            const string BACKCOLOR = @"=IIf(RowNumber(""table1"") Mod 2 = 0, ""#f0f0f0"", ""White"")";
            const string MetricValue = "=IIf(IsNothing(Fields!{0}.Value), \"N/A\", Fields!{0}.Value)";
            const string sortExpression = "=IIf(IsNothing(Fields!{0}.Value), 0, Fields!{0}.Value)";

            dblTableTop = 1.25 + (Metrics.Count*4);
            using (MemoryStream stream = new MemoryStream())
            {
                //FileStream stream;
                //stream = System.IO.File.OpenWrite("C:\\RDLGen3.rdl");

                using (System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(stream, Encoding.UTF8))
                {

                    // Causes child elements to be indented
                    writer.Formatting = System.Xml.Formatting.Indented;

                    writer.WriteStartElement("Table");
                    writer.WriteAttributeString("Name", null, "table1");
        //<ZIndex>2</ZIndex>
                    writer.WriteElementString("DataSetName",null, "CounterSummary");
                    writer.WriteStartElement("Visibility");
                    writer.WriteElementString("Hidden", null, "=IIF(CountRows(\"CounterSummary\") > 0 AND Parameters!DisplayTables.Value = true, false, true)");
                    writer.WriteEndElement();
                    writer.WriteElementString("Top", null, string.Format(ReportCultureInfo, "{0}in", dblTableTop));
                    writer.WriteElementString("Width", null, WIDTH);
                    
                    writer.WriteStartElement("Details");
                    writer.WriteStartElement("TableRows");
                    writer.WriteStartElement("TableRow");
                    
                    writer.WriteStartElement("TableCells");
                    
                    writer.WriteStartElement("TableCell");
                    writer.WriteStartElement("ReportItems");
                    writer.WriteStartElement("Textbox");
                    writer.WriteAttributeString("Name", null, "txtLastCollection");
                    writer.WriteElementString("rd:DefaultName", null, "txtLastCollection");
                    writer.WriteStartElement("Style");
                    writer.WriteElementString("BackgroundColor", null, BACKCOLOR);
                    writer.WriteStartElement("BorderColor");
                    writer.WriteElementString("Default", null, "#dddddd");
                    writer.WriteEndElement();
                    writer.WriteStartElement("BorderStyle");
                    writer.WriteElementString("Default", null, "Solid");
                    writer.WriteEndElement();//end border style
                    writer.WriteElementString("TextAlign", null, "Left");
                    writer.WriteElementString("PaddingLeft", null, "2pt");
                    writer.WriteElementString("PaddingRight", null, "2pt");
                    writer.WriteElementString("PaddingTop", null, "2pt");
                    writer.WriteElementString("PaddingBottom", null, "2pt");
                    writer.WriteEndElement();//Style
                    writer.WriteElementString("CanGrow", null, "true");
                    writer.WriteElementString("Value", null, "=Fields!LastCollectioninInterval.Value");
        //              <ZIndex>7</ZIndex>
                    writer.WriteEndElement();//end of Textbox
                    writer.WriteEndElement();//end of ReportItems
                    writer.WriteEndElement();//End of table cell

                    int MetricNumber = 0;
                    foreach (CustomReportMetric metric in Metrics.Values)
                    {
                        writer.WriteStartElement("TableCell");
                        writer.WriteStartElement("ReportItems");
                        writer.WriteStartElement("Textbox");
                        writer.WriteAttributeString("Name", null, "txtMetric" + MetricNumber);
                        writer.WriteElementString("rd:DefaultName", null, "txtMetric" + MetricNumber);
                        writer.WriteStartElement("Style");
                        writer.WriteElementString("BackgroundColor", null, BACKCOLOR);
                        writer.WriteStartElement("BorderColor");
                        writer.WriteElementString("Default", null, "#dddddd");
                        writer.WriteEndElement();
                        writer.WriteStartElement("BorderStyle");
                        writer.WriteElementString("Default", null, "Solid");
                        writer.WriteEndElement(); //end border style
                        writer.WriteElementString("Format", null, "N2");
                        writer.WriteElementString("PaddingLeft", null, "2pt");
                        writer.WriteElementString("PaddingRight", null, "2pt");
                        writer.WriteElementString("PaddingTop", null, "2pt");
                        writer.WriteElementString("PaddingBottom", null, "2pt");
                        writer.WriteEndElement();//Style
                        //              <ZIndex>6</ZIndex>
                        writer.WriteElementString("CanGrow", null, "true");
                        writer.WriteElementString("Value", null, string.Format(MetricValue, metric.MetricName));
                        writer.WriteEndElement();//end of textbox
                        writer.WriteEndElement();//end of ReportItems
                        writer.WriteEndElement();//End of table cell

                        //              <Value>=IIf(IsNothing(Fields!OSProcessorTimePercent.Value), "N/A", Fields!OSProcessorTimePercent.Value)</Value>
                        MetricNumber++;
                    }
                    writer.WriteEndElement(); //end of table cells
                    writer.WriteElementString("Height", null, ROWHEIGHT);
                    writer.WriteEndElement();//end table row
                    writer.WriteEndElement();//end table rows
                    writer.WriteEndElement();//end details

                    #region Metric Table Raw XML for reference
                    //        <TableCell>
        //          <ReportItems>
        //            <Textbox Name="textbox23">
        //              <rd:DefaultName>textbox23</rd:DefaultName>
        //              <Style>
        //                <BackgroundColor>=IIf(RowNumber("table1") Mod 2 = 0, "#f0f0f0", "White")</BackgroundColor>
        //                <BorderColor>
        //                  <Default>#dddddd</Default>
        //                </BorderColor>
        //                <BorderStyle>
        //                  <Default>Solid</Default>
        //                </BorderStyle>
        //                <Format>N2</Format>
        //                <PaddingLeft>2pt</PaddingLeft>
        //                <PaddingRight>2pt</PaddingRight>
        //                <PaddingTop>2pt</PaddingTop>
        //                <PaddingBottom>2pt</PaddingBottom>
        //              </Style>
        //              <ZIndex>5</ZIndex>
        //              <CanGrow>true</CanGrow>
        //              <Value>=IIf(IsNothing(Fields!SQLCPUActivityPercentage.Value), "N/A", Fields!SQLCPUActivityPercentage.Value)</Value>
        //            </Textbox>
        //          </ReportItems>
        //        </TableCell>
        //        <TableCell>
        //          <ReportItems>
        //            <Textbox Name="textbox11">
        //              <rd:DefaultName>textbox11</rd:DefaultName>
        //              <Style>
        //                <BackgroundColor>=IIf(RowNumber("table1") Mod 2 = 0, "#f0f0f0", "White")</BackgroundColor>
        //                <BorderColor>
        //                  <Default>#dddddd</Default>
        //                </BorderColor>
        //                <BorderStyle>
        //                  <Default>Solid</Default>
        //                </BorderStyle>
        //                <Format>N2</Format>
        //                <PaddingLeft>2pt</PaddingLeft>
        //                <PaddingRight>2pt</PaddingRight>
        //                <PaddingTop>2pt</PaddingTop>
        //                <PaddingBottom>2pt</PaddingBottom>
        //              </Style>
        //              <ZIndex>4</ZIndex>
        //              <CanGrow>true</CanGrow>
        //              <Value>=IIf(IsNothing(Fields!OSProcessorQueueLength.Value), "N/A", Fields!OSProcessorQueueLength.Value)</Value>
        //            </Textbox>
        //          </ReportItems>
        //        </TableCell>
        //        <TableCell>
        //          <ReportItems>
        //            <Textbox Name="textbox13">
        //              <rd:DefaultName>textbox13</rd:DefaultName>
        //              <Style>
        //                <BackgroundColor>=IIf(RowNumber("table1") Mod 2 = 0, "#f0f0f0", "White")</BackgroundColor>
        //                <BorderColor>
        //                  <Default>#dddddd</Default>
        //                </BorderColor>
        //                <BorderStyle>
        //                  <Default>Solid</Default>
        //                </BorderStyle>
        //                <Format>N2</Format>
        //                <PaddingLeft>2pt</PaddingLeft>
        //                <PaddingRight>2pt</PaddingRight>
        //                <PaddingTop>2pt</PaddingTop>
        //                <PaddingBottom>2pt</PaddingBottom>
        //              </Style>
        //              <ZIndex>3</ZIndex>
        //              <CanGrow>true</CanGrow>
        //              <Value>=IIf(IsNothing(Fields!SqlCompilations.Value), "N/A", Fields!SqlCompilations.Value)</Value>
        //            </Textbox>
        //          </ReportItems>
        //        </TableCell>
        //        <TableCell>
        //          <ReportItems>
        //            <Textbox Name="textbox15">
        //              <rd:DefaultName>textbox15</rd:DefaultName>
        //              <Style>
        //                <BackgroundColor>=IIf(RowNumber("table1") Mod 2 = 0, "#f0f0f0", "White")</BackgroundColor>
        //                <BorderColor>
        //                  <Default>#dddddd</Default>
        //                </BorderColor>
        //                <BorderStyle>
        //                  <Default>Solid</Default>
        //                </BorderStyle>
        //                <Format>N2</Format>
        //                <PaddingLeft>2pt</PaddingLeft>
        //                <PaddingRight>2pt</PaddingRight>
        //                <PaddingTop>2pt</PaddingTop>
        //                <PaddingBottom>2pt</PaddingBottom>
        //              </Style>
        //              <ZIndex>2</ZIndex>
        //              <CanGrow>true</CanGrow>
        //              <Value>=IIf(IsNothing(Fields!SqlRecompilations.Value), "N/A", Fields!SqlRecompilations.Value)</Value>
        //            </Textbox>
        //          </ReportItems>
        //        </TableCell>
        //        <TableCell>
        //          <ReportItems>
        //            <Textbox Name="textbox17">
        //              <rd:DefaultName>textbox17</rd:DefaultName>
        //              <Style>
        //                <BackgroundColor>=IIf(RowNumber("table1") Mod 2 = 0, "#f0f0f0", "White")</BackgroundColor>
        //                <BorderColor>
        //                  <Default>#dddddd</Default>
        //                </BorderColor>
        //                <BorderStyle>
        //                  <Default>Solid</Default>
        //                </BorderStyle>
        //                <Format>N2</Format>
        //                <PaddingLeft>2pt</PaddingLeft>
        //                <PaddingRight>2pt</PaddingRight>
        //                <PaddingTop>2pt</PaddingTop>
        //                <PaddingBottom>2pt</PaddingBottom>
        //              </Style>
        //              <ZIndex>1</ZIndex>
        //              <CanGrow>true</CanGrow>
        //              <Value>=IIf(IsNothing(Fields!LockWaits.Value), "N/A", Fields!LockWaits.Value)</Value>
        //            </Textbox>
        //          </ReportItems>
        //        </TableCell>
        //        <TableCell>
        //          <ReportItems>
        //            <Textbox Name="textbox19">
        //              <rd:DefaultName>textbox19</rd:DefaultName>
        //              <Style>
        //                <BackgroundColor>=IIf(RowNumber("table1") Mod 2 = 0, "#f0f0f0", "White")</BackgroundColor>
        //                <BorderColor>
        //                  <Default>#dddddd</Default>
        //                </BorderColor>
        //                <BorderStyle>
        //                  <Default>Solid</Default>
        //                </BorderStyle>
        //                <Format>N2</Format>
        //                <PaddingLeft>2pt</PaddingLeft>
        //                <PaddingRight>2pt</PaddingRight>
        //                <PaddingTop>2pt</PaddingTop>
        //                <PaddingBottom>2pt</PaddingBottom>
        //              </Style>
        //              <CanGrow>true</CanGrow>
        //              <Value>=IIf(IsNothing(Fields!TableLockEscalations.Value), "N/A", Fields!TableLockEscalations.Value)</Value>
        //            </Textbox>
        //          </ReportItems>
        //        </TableCell>
        //      </TableCells>
        //      <Height>0.25in</Height>
        //    </TableRow>
        //  </TableRows>
        //</Details>
#endregion
                    writer.WriteStartElement("Header");

                    writer.WriteStartElement("TableRows");
                    writer.WriteStartElement("TableRow");

                    writer.WriteStartElement("TableCells");

                    writer.WriteStartElement("TableCell");
                    writer.WriteStartElement("ReportItems");

                    writer.WriteStartElement("Textbox");
                    writer.WriteAttributeString("Name", null, "txtLastCollectionHeader");
                    writer.WriteElementString("rd:DefaultName", null, "txtLastCollectionHeader");
                    writer.WriteStartElement("Style");
                    writer.WriteElementString("BackgroundColor", null, "#e5e5e5");
                    writer.WriteStartElement("BorderColor");
                    writer.WriteElementString("Default", null, "#bbbbbb");
                    writer.WriteEndElement(); //end border color
                    writer.WriteStartElement("BorderStyle");
                    writer.WriteElementString("Default", null, "Solid");
                    writer.WriteEndElement();//end border style
                    //writer.WriteElementString("TextAlign", null, "Left");
                    writer.WriteElementString("FontSize", null, "9pt");
                    writer.WriteElementString("PaddingLeft", null, "2pt");
                    writer.WriteElementString("PaddingRight", null, "2pt");
                    writer.WriteElementString("PaddingTop", null, "2pt");
                    writer.WriteElementString("PaddingBottom", null, "2pt");
                    writer.WriteEndElement();//Style
                    writer.WriteElementString("CanGrow", null, "true");
                    writer.WriteStartElement("UserSort");
                    writer.WriteElementString("SortExpression", null, "=Fields!LastCollectioninInterval.Value");
                    writer.WriteEndElement();//end user sort
                    writer.WriteElementString("Value", null, "Date");
                    //              <ZIndex>15</ZIndex>
                    writer.WriteEndElement();//end of textbox
                    writer.WriteEndElement();//end of ReportItems
                    writer.WriteEndElement();//End of table cell

                    MetricNumber = 0;
                    foreach (CustomReportMetric metric in Metrics.Values)
                    {
                        writer.WriteStartElement("TableCell");
                        writer.WriteStartElement("ReportItems");

                        writer.WriteStartElement("Textbox");
                        writer.WriteAttributeString("Name", null, "txtMetricHeader" + MetricNumber);
                        writer.WriteElementString("rd:DefaultName", null, "txtMetricHeader" + MetricNumber);
                        writer.WriteStartElement("Style");
                        writer.WriteElementString("BackgroundColor", null, "#e5e5e5");
                        writer.WriteStartElement("BorderColor");
                        writer.WriteElementString("Default", null, "#bbbbbb");
                        writer.WriteEndElement();// end border color
                        writer.WriteStartElement("BorderStyle");
                        writer.WriteElementString("Default", null, "Solid");
                        writer.WriteEndElement();//end border style
                        
                        writer.WriteElementString("FontSize", null, "9pt");
                        writer.WriteElementString("TextAlign", null, "Center");

                        writer.WriteElementString("PaddingLeft", null, "2pt");
                        writer.WriteElementString("PaddingRight", null, "2pt");
                        writer.WriteElementString("PaddingTop", null, "2pt");
                        writer.WriteElementString("PaddingBottom", null, "2pt");
                        writer.WriteEndElement();//Style
                        //              <ZIndex>14</ZIndex>
                        writer.WriteElementString("CanGrow", null, "true");
                        writer.WriteStartElement("UserSort");
                        writer.WriteElementString("SortExpression", null, string.Format(sortExpression, metric.MetricName));
                        writer.WriteEndElement();//end user sort
                        
                        string AggregationString = null;
                        switch (metric.Aggregation)
                        {
                            case Aggregation.AverageSinceLastCollection:
                                AggregationString = " (Avg)";
                                break;
                            case Aggregation.MaxSinceLastCollection:
                                AggregationString = " (Max)";
                                break;
                            case Aggregation.PerMinuteSinceLastCollection:
                                AggregationString = " (per Minute)";
                                break;
                            //case Aggregation.RawCounter:
                            //    AggregationString = " (Raw)";
                            //    break;

                        }
                        writer.WriteElementString("Value", null, metric.MetricDescription + AggregationString);

                        writer.WriteEndElement();//end of textbox
                        writer.WriteEndElement();//end of ReportItems
                        writer.WriteEndElement();//End of table cell

                        MetricNumber++;
                    }
                    writer.WriteEndElement(); //end of table cells
                    writer.WriteElementString("Height", null, ROWHEIGHT);
                    writer.WriteEndElement();//end table row
                    writer.WriteEndElement();//end table rows
                    writer.WriteEndElement();//end Header
                    
                    writer.WriteStartElement("TableColumns");
                    writer.WriteStartElement("TableColumn");
                    writer.WriteElementString("Width", null, DATECOLUMNWIDTH);

                    writer.WriteEndElement();

                    double currentColWidth = dblAvailableSpace/Metrics.Count;
                    
                    for (MetricNumber = 0; MetricNumber < Metrics.Count; MetricNumber++ )
                    {
                        writer.WriteStartElement("TableColumn");
                        writer.WriteElementString("Width", null, string.Format(ReportCultureInfo, "{0}in", currentColWidth));

                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();//end table columns
                    writer.WriteEndElement();//end table
      
                    writer.Flush();
                    stream.Flush();
                    stream.Position = 0;

                    using (StreamReader reader = new StreamReader(stream))
                    {
                        returnString = reader.ReadToEnd();
                    }
                }
            }
            return returnString;
        }
        
        /// <summary>
        /// set the default value for the 
        /// </summary>
        /// <returns></returns>
        public string GetShowTablesParameter(bool showTable)
        {
            string returnString;
            using (MemoryStream stream = new MemoryStream())
            {
                using (System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(stream, Encoding.UTF8))
                {

                    // Causes child elements to be indented
                    writer.Formatting = System.Xml.Formatting.Indented;

                    writer.WriteStartElement("ReportParameter");
                    writer.WriteAttributeString("Name", null, "DisplayTables");
                    writer.WriteElementString("DataType", null, "Boolean");
                    writer.WriteStartElement("DefaultValue");
                    writer.WriteStartElement("Values");
                    writer.WriteElementString("Value", null, "=" + showTable);
                    writer.WriteEndElement();//end values
                    writer.WriteEndElement();//end default value
                    writer.WriteElementString("AllowBlank", null, "true");
                    writer.WriteElementString("Prompt", null, "Display Tabular Data");
                    writer.WriteEndElement();//end report parameter

                    writer.Flush();
                    stream.Flush();
                    stream.Position = 0;

                    using (StreamReader reader = new StreamReader(stream))
                    {
                        returnString = reader.ReadToEnd();
                    }
                }
            }
            return returnString;
        }
        /// <summary>
        /// Builds the No Data available Textbox
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public string GetNoDataForCustomTopServers()
        {
            string returnString = "=iif(";
            int i = 0;
            foreach (var metric in Metrics.Values)
            {
                if (i != 0)
                {
                    returnString += " AND ";
                }
                returnString += "CountRows(\""+metric.MetricName+"DataSet\") = 0 ";
                i++;
            }
            returnString += ", false, true)";
            return returnString;
        }
        /// <summary>
        /// Builds the counter summary dataset for custom reports
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public string GetCounterSummaryTopServersDataSet()
        {
            string returnString = null;

            using (MemoryStream stream = new MemoryStream())
            {
                using (System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(stream, Encoding.UTF8))
                {

                    // Causes child elements to be indented
                    writer.Formatting = System.Xml.Formatting.Indented;

                    foreach (var metric in Metrics.Values)
                    {
                        //DataSet element
                        writer.WriteStartElement("DataSet");
                        writer.WriteAttributeString("Name", null, metric.MetricName + "DataSet");

                        // Fields elements
                        writer.WriteStartElement("Fields");

                        writer.WriteStartElement("Field");
                        writer.WriteAttributeString("Name", null, "InstanceName");
                        writer.WriteElementString("DataField", null, "InstanceName");
                        writer.WriteElementString("rd:TypeName", "System.String");
                        writer.WriteEndElement(); // Field

                        writer.WriteStartElement("Field");
                        writer.WriteAttributeString("Name", null, "MetricValue");
                        writer.WriteElementString("DataField", null, "MetricValue");
                        writer.WriteElementString("rd:TypeName", "System.Double");
                        writer.WriteEndElement(); // Field

                        writer.WriteStartElement("Field");
                        writer.WriteAttributeString("Name", null, "SQLServerID");
                        writer.WriteElementString("DataField", null, "SQLServerID");
                        writer.WriteElementString("rd:TypeName", "System.Double");
                        writer.WriteEndElement(); // Field

                        // End previous elements
                        writer.WriteEndElement(); // Fields

                        // Query element
                        writer.WriteStartElement("Query");
                        writer.WriteElementString("DataSourceName", "SQL diagnostic manager Data Source");
                        writer.WriteElementString("CommandType", "StoredProcedure");
                        writer.WriteElementString("CommandText", "p_GetCustomReportsTopServersDataSet");

                        writer.WriteStartElement("QueryParameters");

                        writer.WriteStartElement("QueryParameter");
                        writer.WriteAttributeString("Name", null, "@NumServers");
                        writer.WriteElementString("Value", "=Parameters!NumServers.Value");
                        writer.WriteEndElement(); // QueryParameter

                        writer.WriteStartElement("QueryParameter");
                        writer.WriteAttributeString("Name", null, "@SQLServerIDs");
                        writer.WriteElementString("Value", "=iif(isNothing(Parameters!ServerXML.Value), Nothing, iif(Parameters!ServerXML.Value = \"&lt;Srvrs/&gt;\", Nothing, Parameters!ServerXML.Value))");
                        writer.WriteEndElement(); // QueryParameter

                        writer.WriteStartElement("QueryParameter");
                        writer.WriteAttributeString("Name", null, "@UTCStart");
                        writer.WriteElementString("Value", "=Code.GetStart(Parameters!Period.Value, Code.CreateCustomDatetime(Parameters!rsStart.Value, Parameters!rsStartHours.Value), true)");
                        writer.WriteEndElement(); // QueryParameter

                        writer.WriteStartElement("QueryParameter");
                        writer.WriteAttributeString("Name", null, "@UTCEnd");
                        writer.WriteElementString("Value", "=Code.GetEnd(Parameters!Period.Value, Code.CreateCustomDatetime(Parameters!rsEnd.Value, Parameters!rsEndHours.Value), true)");
                        writer.WriteEndElement(); // QueryParameter

                        writer.WriteStartElement("QueryParameter");
                        writer.WriteAttributeString("Name", null, "@metricName");
                        writer.WriteElementString("Value", metric.MetricName );
                        writer.WriteEndElement(); // QueryParameter

                        writer.WriteEndElement(); // QueryParameters

                        //writer.WriteElementString("Timeout", "30");
                        writer.WriteEndElement(); // Query

                        writer.WriteEndElement(); // DataSet
                    }
                    writer.Flush();
                    //stream.Close();
                    stream.Flush();
                    stream.Position = 0;
                    
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        returnString = reader.ReadToEnd();
                    }
                }
            }
            return returnString;
        }
        /// <summary>
        /// Builds the counter summary dataset for custom reports
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public string GetCounterSummaryDataSet()
        {
            string returnString = null;

            using (MemoryStream stream = new MemoryStream())
            {
                //FileStream stream;
                //stream = System.IO.File.OpenWrite("C:\\RDLGen3.rdl");

                using (System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(stream, Encoding.UTF8))
                {

                    //writer.Flush();

                    // Causes child elements to be indented
                    writer.Formatting = System.Xml.Formatting.Indented;

                    //DataSet element
                    //writer.WriteStartElement("DataSets");
                    writer.WriteStartElement("DataSet");
                    writer.WriteAttributeString("Name", null, "CounterSummary");

                    // Fields elements
                    writer.WriteStartElement("Fields");

                    writer.WriteStartElement("Field");
                    writer.WriteAttributeString("Name", null, "InstanceName");
                    writer.WriteElementString("DataField", null, "InstanceName");
                    writer.WriteElementString("rd:TypeName", "System.String");
                    writer.WriteEndElement(); // Field

                    writer.WriteStartElement("Field");
                    writer.WriteAttributeString("Name", null, "LastCollectioninInterval");
                    writer.WriteElementString("DataField", null, "LastCollectioninInterval");
                    writer.WriteElementString("rd:TypeName", "System.DateTime");
                    writer.WriteEndElement(); // Field

                    foreach (CustomReportMetric metric in Metrics.Values)
                    {
                        writer.WriteStartElement("Field");
                        writer.WriteAttributeString("Name", null, metric.MetricName);
                        writer.WriteElementString("DataField", null, metric.MetricName);
                        writer.WriteElementString("rd:TypeName", "System.Double");
                        writer.WriteEndElement(); // Field
                    }

                    // End previous elements
                    writer.WriteEndElement(); // Fields

                    // Query element
                    writer.WriteStartElement("Query");
                    writer.WriteElementString("DataSourceName", "SQL diagnostic manager Data Source");
                    writer.WriteElementString("CommandType", "StoredProcedure");
                    writer.WriteElementString("CommandText", "p_GetCustomReportsDataSet");

                    writer.WriteStartElement("QueryParameters");

                    writer.WriteStartElement("QueryParameter");
                    writer.WriteAttributeString("Name", null, "@UTCOffset");
                    writer.WriteElementString("Value", "=Parameters!UTCOffset.Value");
                    writer.WriteEndElement(); // QueryParameter

                    writer.WriteStartElement("QueryParameter");
                    writer.WriteAttributeString("Name", null, "@Interval");
                    writer.WriteElementString("Value", "=Parameters!Interval.Value");
                    writer.WriteEndElement(); // QueryParameter

                    writer.WriteStartElement("QueryParameter");
                    writer.WriteAttributeString("Name", null, "@ServerID");
                    writer.WriteElementString("Value", "=Iif(Parameters!GUIServerID.Value=-1,Parameters!servername.Value, Parameters!GUIServerID.Value)");
                    writer.WriteEndElement(); // QueryParameter

                    writer.WriteStartElement("QueryParameter");
                    writer.WriteAttributeString("Name", null, "@UTCStart");
                    writer.WriteElementString("Value", "=Code.GetStart(Parameters!Period.Value, Parameters!rsStart.Value, true)");
                    writer.WriteEndElement(); // QueryParameter

                    writer.WriteStartElement("QueryParameter");
                    writer.WriteAttributeString("Name", null, "@UTCEnd");
                    writer.WriteElementString("Value", "=Code.GetEnd(Parameters!Period.Value, Parameters!rsEnd.Value, true)");
                    writer.WriteEndElement(); // QueryParameter

                    writer.WriteStartElement("QueryParameter");
                    writer.WriteAttributeString("Name", null, "@reportName");
                    writer.WriteElementString("Value", Name);
                    writer.WriteEndElement(); // QueryParameter

                    writer.WriteEndElement(); // QueryParameters

                    //writer.WriteElementString("Timeout", "30");
                    writer.WriteEndElement(); // Query

                    writer.WriteEndElement(); // DataSet
                    //writer.WriteEndElement(); // DataSets
                    writer.Flush();
                    //stream.Close();
                    stream.Flush();
                    stream.Position = 0;
                    
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        returnString = reader.ReadToEnd();
                    }
                }
            }
            return returnString;
        }
        #endregion


    }
    
    public class CustomReportMetric
    {
        private string _metricName;
        private string _metricDescription;
        private CustomReport.CounterType _metricSource;
        private CustomReport.Aggregation _aggregation;
        private int _graphNumber;

        public CustomReportMetric(string MetricName, string MetricDescription, CustomReport.CounterType Source, CustomReport.Aggregation Aggregation)
        {
            _metricName = MetricName;
            _metricDescription = MetricDescription;
            _metricSource = Source;
            _aggregation = Aggregation;
        }
        public string MetricName
        {
            get { return _metricName;}
            set { _metricName = value;}
        }
        public string MetricDescription
        {
            get { return _metricDescription;}
            set { _metricDescription = value;}
        }
        public CustomReport.CounterType Source
        {
            get { return _metricSource;}
            set { _metricSource = value;}
        }
        public CustomReport.Aggregation Aggregation
        {
            get { return _aggregation;}
            set { _aggregation = value;}
        }
        public int GraphNumber
        {
            get { return _graphNumber; }
            set { _graphNumber = value; }
        }
        public override string ToString()
        {
            return _metricDescription;
        }
    }
}
