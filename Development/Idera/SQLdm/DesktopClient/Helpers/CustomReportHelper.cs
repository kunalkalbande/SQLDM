using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Xml.Serialization;


namespace Idera.SQLdm.DesktopClient.Helpers
{
    /// <summary>
    /// SQLdm 9.1 (Vineet Kumar) (Community Integration) - Custom report helper class for serialezing/deserializing custom counters and converting one object to serialisable object
    /// Also a few methods from add report wizard moved to this for reusing in import wizard
    /// </summary>
    public class CustomReportHelper
    {
        //Start: This section is moved from customReportWizard to this file to reuse it in CustomReportImportWizard
        public static readonly string INVALIDREPORTCHARS = @"/?:&\*<>|#%,;@""$+=";
        public static readonly string[] RESERVEDNAMES ={"CON","AUX","PRN","COM1","LPT2",".","..","nul","com2",
        "com3","com4","com5","com6","com7","com8","com9","lpt1","lpt3","lpt4","lpt5","lpt6",
        "lpt7","lpt8","lpt9","clock$"};

        /// <summary>
        /// Test this string for chars that are not allowabel in a report name
        /// </summary>
        /// <param name="reportName"></param>
        /// <returns></returns>
        public static bool ReportNameIsValid(string reportName)
        {
            if (string.IsNullOrEmpty(reportName)) return false;

            foreach (char c in reportName)
            {
                if (char.IsControl(c)) return false;
                if (char.IsSurrogate(c)) return false;
                if (CustomReportHelper.INVALIDREPORTCHARS.Contains(c.ToString())) return false;
            }
            foreach (string reservedName in CustomReportHelper.RESERVEDNAMES)
            {
                if (reportName.ToLower().Equals(reservedName.ToLower())) return false;
            }
            return true;
        }
        //End: This section is moved from customReportWizard to this file to reuse it in CustomReportImportWizard

        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("Message");
        public static string CreateXML(CustomReportSerializable customReport)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(CustomReportSerializable));
                using (MemoryStream xmlStream = new MemoryStream())
                {
                    xmlSerializer.Serialize(xmlStream, customReport);
                    xmlStream.Position = 0;
                    xmlDoc.Load(xmlStream);
                    return xmlDoc.InnerXml;
                }
            }
            catch (Exception ex)
            {
                LOG.ErrorFormat("Error occured while exporting custom counter {0}. Detailed error : {1}", customReport.ReportName, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                throw ex;
            }
        }

        public static string SerializeCustomReport(Idera.SQLdm.Common.Objects.CustomReport customReport)
        {
            var report = GetCustomReportSerializable(customReport);
            return CreateXML(report);
        }

        static public CustomReportSerializable DeserializeCustomReport(string xmlPath)
        {
            try
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(CustomReportSerializable));
                TextReader reader = new StreamReader(xmlPath);
                object obj = deserializer.Deserialize(reader);
                CustomReportSerializable XmlData = (CustomReportSerializable)obj;
                reader.Close();
                return XmlData;
            }
            catch (Exception ex)
            {
                LOG.ErrorFormat("Error occured in parsing custom report xml ({0}) during import. Detailed error : {1}", xmlPath, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                throw ex;
            }
        }

        public static CustomReportSerializable GetCustomReportSerializable(Idera.SQLdm.Common.Objects.CustomReport customReport)
        {
            if (customReport == null)
                return null;
            CustomReportSerializable customReportSerializable = new CustomReportSerializable();
            customReportSerializable.ReportName=customReport.Name;
            customReportSerializable.ShortDescription=customReport.ShortDescription;
            customReportSerializable.ShowTable = customReport.ShowTable;
            customReportSerializable.IsTopServerReport = customReport.ShowTopServers;
            customReportSerializable.Metrics = new List<CustomReportMetricSerializable>();
            foreach (var met in customReport.Metrics)
            {
                customReportSerializable.Metrics.Add(GetCustomReportMetricSerializable(met.Value));
            }
            return customReportSerializable;
        }

        public static Idera.SQLdm.Common.Objects.CustomReport GetCustomReport(CustomReportSerializable customReportSerializable)
        {
            if (customReportSerializable == null)
                return null;
            Idera.SQLdm.Common.Objects.CustomReport customReport = new Common.Objects.CustomReport(customReportSerializable.ReportName);
              customReport.Name=customReportSerializable.ReportName;
              customReport.ShortDescription = customReportSerializable.ShortDescription;
              customReport.ShowTable = customReportSerializable.ShowTable;
              customReport.Metrics = new SortedDictionary<int, Common.Objects.CustomReportMetric>();
              customReport.ShowTopServers = customReportSerializable.IsTopServerReport;
            foreach (var met in customReportSerializable.Metrics)
            {
                customReport.Metrics.Add(customReport.Metrics.Count + 1, GetCustomReportMetric(met));
            }
              return customReport;
        }

        public static Idera.SQLdm.Common.Objects.CustomReportMetric GetCustomReportMetric(CustomReportMetricSerializable metricSerializable)
        {
            if (metricSerializable == null)
                return null;
            Idera.SQLdm.Common.Objects.CustomReportMetric metric = new Common.Objects.CustomReportMetric(metricSerializable._metricName, metricSerializable._metricDescription, metricSerializable._metricSource, metricSerializable._aggregation);
            metric.GraphNumber = metricSerializable._graphNumber;
            return metric;
        }

        public static CustomReportMetricSerializable GetCustomReportMetricSerializable(Idera.SQLdm.Common.Objects.CustomReportMetric metric)
        {
            if (metric == null)
                return null;
            CustomReportMetricSerializable metricSer = new CustomReportMetricSerializable();
            metricSer._aggregation = metric.Aggregation;
            metricSer._graphNumber = metric.GraphNumber;
            metricSer._metricDescription = metric.MetricDescription;
            metricSer._metricName = metric.MetricName;
            metricSer._metricSource = metric.Source;
            return metricSer;
        }
    }

    /// <summary>
    /// Cutom report class for serialization
    /// </summary>
    public class CustomReportSerializable
    {
        public CustomReportSerializable()
        {
        }

        public string ReportName;
        public string ShortDescription;
        //public string ReportRDL;
        public bool ShowTable = true;
        public int ReportID;
        public List<CustomReportMetricSerializable> Metrics = new List<CustomReportMetricSerializable>();
        //Check Report is custom report or top server report
        public bool IsTopServerReport = false;
    }

    /// <summary>
    /// Custom report metric class for serialisation.
    /// </summary>
    public class CustomReportMetricSerializable
    {
        public CustomReportMetricSerializable(){}
        public string _metricName;
        public string _metricDescription;
        public Idera.SQLdm.Common.Objects.CustomReport.CounterType _metricSource;
        public Idera.SQLdm.Common.Objects.CustomReport.Aggregation _aggregation;
        public int _graphNumber;
    }
}
