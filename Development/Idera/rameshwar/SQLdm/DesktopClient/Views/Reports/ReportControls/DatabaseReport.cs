using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Properties;
using Infragistics.Win;

namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    public partial class DatabaseReport : Idera.SQLdm.DesktopClient.Views.Reports.ReportControls.ReportContol
    {
        private const int dbtableLimit = 10;
        protected enum XmlType
        {
            Database,
            Tables
        }

        protected bool multipleDatabasesAllowed = false;

        public DatabaseReport()
        {
            InitializeComponent();
        }

        public override void InitReport()
        {
             base.InitReport();
             tagsComboBox.Visible = false;
             tagsLabel.Visible = false;
         }

        // Currently selected list of databases.
        protected List<string> curDbs = new List<string>();
        protected List<string> Databases
        {
            get { return curDbs; }
            set
            {
                if (!ListsAreEqual(curDbs, value))
                {
                    curDbs = value;
                    MakeCSVList(databaseTextbox, curDbs);
                }
            }
        }

        protected void databaseBrowseButton_Click(object sender, EventArgs e)
        {
            if ((instanceCombo.SelectedItem != null) && (instanceCombo.SelectedItem != instanceSelectOne))
            {
                DatabaseBrowserDialog dlg =
                    new DatabaseBrowserDialog(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                              ((MonitoredSqlServer)((ValueListItem)instanceCombo.SelectedItem).DataValue).Id,
                                              ((MonitoredSqlServer)((ValueListItem)instanceCombo.SelectedItem).DataValue).InstanceName, multipleDatabasesAllowed, true,
                                              "Check one or more databases for the report.", "Select Databases");
                if (multipleDatabasesAllowed)
                {
                    dlg =
                        new DatabaseBrowserDialog(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                                  ((MonitoredSqlServer)
                                                   ((ValueListItem) instanceCombo.SelectedItem).DataValue).Id,
                                                  ((MonitoredSqlServer)
                                                   ((ValueListItem) instanceCombo.SelectedItem).DataValue).InstanceName,
                                                  multipleDatabasesAllowed, true,
                                                  "Check one or more databases for the report.", "Select Databases");
                    dlg.CheckedDatabases = Databases;
                }
                else
                {
                    dlg =
                        new DatabaseBrowserDialog(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                                  ((MonitoredSqlServer)
                                                   ((ValueListItem)instanceCombo.SelectedItem).DataValue).Id,
                                                  ((MonitoredSqlServer)
                                                   ((ValueListItem)instanceCombo.SelectedItem).DataValue).InstanceName,
                                                  multipleDatabasesAllowed, true,
                                                  "Select a database for the report.", "Select Database");

                    if (Databases != null)
                    {
                        dlg.SelectedDatabase = Databases[0];
                    }
                }

                if (DialogResult.OK == dlg.ShowDialog(FindForm()))
                {
                    if (multipleDatabasesAllowed)
                    {
                        Databases = dlg.CheckedDatabases;
                    }
                    else
                    {
                        List<string> database = new List<string>();
                        database.Add(dlg.SelectedDatabase);
                        Databases = database;
                    }
                }
            }
            else
            {
                ApplicationMessageBox.ShowInfo(this, "Select a server before selecting a database.");
            }
        }

        protected string ConvertToXml(XmlType type, List<string> data)
        {
            string xml;
            string escapedXml;
            int count = 0;

            switch(type)
            {
                case XmlType.Database:
                    {
                        xml = "<Databases>";

                        foreach (string database in data)
                        {
                            escapedXml = database.Replace("&", "&amp;");
                            escapedXml = escapedXml.Replace("\"", "&quot;");
                            escapedXml = escapedXml.Replace("<", "&lt;");
                            escapedXml = escapedXml.Replace(">", "&gt;");
                            xml += String.Format("<Database DatabaseName=\"{0}\" />", escapedXml);
                            
                            if (++count >= dbtableLimit)
                            {
                                break;
                            }
                        }
                        xml += "</Databases>";
                    }
                    break;
                case XmlType.Tables:
                    {
                        xml = "<Tables>";

                        foreach (string table in data)
                        {
                            escapedXml = table.Replace("&", "&amp;");
                            escapedXml = escapedXml.Replace("\"", "&quot;");
                            escapedXml = escapedXml.Replace("<", "&lt;");
                            escapedXml = escapedXml.Replace(">", "&gt;");
                            xml += String.Format("<Table TableName=\"{0}\" />", escapedXml);
                            
                            if (++count >= dbtableLimit)
                            {
                                break;
                            }
                        }
                        xml += "</Tables>";
                        break;
                    }
                    default:
                    {
                        xml = "";
                        break;
                    }
            }
            return xml;
        }

        private void instanceCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            Databases = null;
        }
    }
}

