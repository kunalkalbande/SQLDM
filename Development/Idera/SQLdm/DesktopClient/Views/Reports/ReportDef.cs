using System;
using System.Collections.Generic;
using System.Xml;
using BBS.TracerX;
using Microsoft.Reporting.WinForms;
using System.Data;
using System.Windows.Forms;
using System.IO;

namespace Idera.SQLdm.DesktopClient.Views.Reports {
    internal class ReportDef {
        private static readonly Logger Log = Logger.GetLogger("Reports.ReportDef");

        // Info about each chart in the report.
        private Chart[] Charts;

        public readonly string RegularPath;
        public readonly string PathForNonContiguousDates; 

        public ReportDef(string rdlcPath) {
            using (Log.DebugCall()) {
                try {
                    Log.Verbose("Parsing rdlc file ", rdlcPath);
                    RegularPath = rdlcPath;
                    PathForNonContiguousDates = rdlcPath; // for now
                    XmlDocument doc = new XmlDocument();
                    doc.Load(rdlcPath);

                    XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
                    ns.AddNamespace("ns", "http://schemas.microsoft.com/sqlserver/reporting/2005/01/reportdefinition");

                    LoadCharts(doc, ns);

                    string dir = Path.Combine(Application.LocalUserAppDataPath, "RDLCsForNonContiguousDates\\");
                    if (!Directory.Exists(dir)) {
                        Directory.CreateDirectory(dir);
                    }
                    string newpath = Path.Combine(dir, Path.GetFileName(rdlcPath));
                    doc.Save(newpath);
                    PathForNonContiguousDates = newpath;
                } catch (Exception ex) {
                    Log.Error("Exception in ReportDef ctor for '", rdlcPath, "'.\n", ex);
                }
            }
        }

        //private void MakeVersionForNonContiguousDates(XmlDocument doc, XmlNamespaceManager ns) {
        //    throw new Exception("The method or operation is not implemented.");
        //}

        private void LoadCharts(XmlDocument doc, XmlNamespaceManager ns) {
            XmlNodeList chartNodes = doc.SelectNodes("ns:Report/ns:Body/ns:ReportItems/ns:Chart", ns);
            Charts = new Chart[chartNodes.Count];
            int chartNum = 0;
            foreach (XmlNode chartNode in chartNodes) {
                Chart chart = new Chart();
                chart.LoadFromXml(chartNode, ns);
                Log.Verbose(chart);
                Charts[chartNum] = chart;
                ++chartNum;
            }
        }   

        // Returns an List of all charts in the specified report definition file that don't 
        // have enough data to draw at least one line segment (i.e. two non-null values for the 
        // same metric in the same series). 
        public List<string> AllChartsHaveEnoughData(ReportDataSourceCollection dataSources) {
            List<string> emptyCharts = new List<string>();
            
            foreach (Chart chart in Charts) {
                DataTable dataTable = (DataTable)dataSources[chart.DataSetName].Value;
                
                if (!chart.HasEnoughRows(dataTable)) {
                    emptyCharts.Add(chart.Title);
                    continue;
                }

                if (!chart.HasEnoughData(dataTable)) {
                    emptyCharts.Add(chart.Title);
                }
            }

            return emptyCharts;
        }
    }
}
