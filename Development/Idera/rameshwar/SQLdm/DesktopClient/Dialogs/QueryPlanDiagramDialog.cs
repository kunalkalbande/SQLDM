using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Xsl;

using BBS.TracerX;

using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Controls;
using Idera.SQLdm.DesktopClient.Helpers;

namespace Idera.SQLdm.DesktopClient.Dialogs
{


    public partial class QueryPlanDiagramDialog : Form
    {

        #region Html and SQL words and References
        private string htmlStart = "<!DOCTYPE html><html lang=\"en\" xmlns=\"http://www.w3.org/1999/xhtml\" \">" +
            "<head><link rel=StyleSheet href=\"" + System.Windows.Forms.Application.StartupPath + "\\Dialogs\\Style\\css\\qp\\qp.css\"><meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\"> " +
            "<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">";

        private string JSFindCaller = "<script type=\"text/javascript\" src=\"" + System.Windows.Forms.Application.StartupPath + "\\Dialogs\\Style\\JS\\jquery-1.11.1.min.js\"></script>" +
                                      "<script type=\"text/javascript\" src=\"" + System.Windows.Forms.Application.StartupPath + "\\Dialogs\\Style\\JS\\jquery-ui.js\"></script>" +
                                      "<script type=\"text/javascript\" src=\"" + System.Windows.Forms.Application.StartupPath + "\\Dialogs\\Style\\JS\\qp.js\"></script>" +
                                      "<script type=\"text/javascript\" src=\"" + System.Windows.Forms.Application.StartupPath + "\\Dialogs\\Style\\JS\\html2canvas.min.js\"></script>" +
                                      "<script type=\"text/javascript\" src=\"" + System.Windows.Forms.Application.StartupPath + "\\Dialogs\\Style\\JS\\download.js\"></script>" +
                                      "</head><body><div id='divQueryPlan' class='divQueryPlan'><div id='lassoDiv' class='draggable'>";

        private string JSDrawer = @"<script>
                                        $(document).ready(function (){
                                            callDrawLines();
                                            lassoZoom();
                                        })
                                    </script>";

        private string closing =    @"</div>
                                      <canvas id='myDrawcanvas' class='myDrawcanvas'></canvas>
                                        </div>
                                        </body>
                                        </html>";

        private string unSupportedServerString = "We are sorry to announce your SQL Server Version is not supported for this feature!";

        //Path to query plan xslt
        private string xsltDoc = System.Windows.Forms.Application.StartupPath + "\\Dialogs\\Style\\QueryPlanTransforms.xslt";
        //SQL reservced words used for color coding
        //Don't line wrap the following string or it wont work right
        //private static string sqlCommands = @"(ALTER TABLE|ADA|ADD|AFTER|ALL|ALTER|AND|ANY|AS|ASC|AT|AUTHORIZATION|AUTO|BACKUP|BEGIN|BETWEEN|BINARY|BIT|BIT_LENGTH|BREAK|BROWSE|BULK|BY|CASCADE|CASE|CAST|CHAR|CHARACTER|CHARACTER_LENGTH|CHAR_LENGTH|CHECK|CHECKPOINT|CLOSE|CLUSTERED|COLLATE|COLUMN|COMMIT|COMPUTE|CONNECT|CONNECTION|CONSTRAINT|CONTAINS|CONTAINSTABLE|CONTINUE|CREATE|CROSS|CUBE|CURRENT|CURRENT_DATE|CURRENT_TIME|CURSOR|DATABASE|DATE|DATETIME|DBCC|DEALLOCATE|DEC|DECIMAL|DECLARE|DEFAULT|DEFERRED|DELETE|DENY|DESC|DISK|DISTINCT|DISTRIBUTED|DOUBLE|DROP|DUMMY|DUMP|ELSE|ENCRYPTION|END|END - EXEC|ERRLVL|ESCAPE|EXCEPT|EXEC|EXECUTE|EXISTS|EXIT|EXTERNAL|EXTRACT|FALSE|FETCH| FILE|FILLFACTOR|FIRST|FLOAT|FOR|FOREIGN|FORTRAN|FREE|FREETEXT|FREETEXTTABLE|FROM|FULL|FUNCTION|GLOBAL|GOTO|GRANT|GROUP|HAVING|HOLDLOCK|HOUR|IDENTITY|IDENTITYCOL|IDENTITY_INSERT|IF|IGNORE|IMAGE|IMMEDIATE|IN|INCLUDE|INDEX|INNER|INSENSITIVE|INSERT|INSTEAD|INT|INTEGER|INTERSECT|INTO|IS|ISOLATION|JOIN|KEY|KILL|LANGUAGE|LAST|LEFT|LEVEL|LIKE|LINENO|LOAD|LOCAL|MINUTE|MODIFY|MONEY|NAME|NATIONAL|NCHAR|NEXT|NOCHECK|NOCOUNT|NONCLUSTERED|NONE|NOT|NUMERIC|NVARCHAR|OCTET_LENGTH|OF|OFF|OFFSETS|ON|ONLY|OPEN|OPENDATASOURCE|OPENQUERY|OPENROWSET|OPENXML|OPTION|OR|ORDER|OUTER|OUTPUT|OVER|OVERLAPS|PARTIAL|PASCAL|PERCENT|PLAN|POSITION|PRECISION|PREPARE|PRIMARY|PRINT|PRIOR|PRIVILEGES|PROC|PROCEDURE|PUBLIC|RAISERROR|READ|READTEXT|REAL|RECONFIGURE|REFERENCES|REPLICATION|RESTORE|RESTRICT|RETURN|RETURNS|REVERT|REVOKE|RIGHT|ROLLBACK|ROLLUP|ROWCOUNT|ROWGUIDCOL|ROWS|RULE|SAVE|SCHEMA|SCROLL|SECOND|SECTION|SELECT|SEQUENCE|SET|SETUSER|SHUTDOWN|SIZE|SMALLINT|SMALLMONEY|SOME|SQLCA|SQLERROR|STATISTICS|TABLE|TEMPORARY|TEXT|TEXTSIZE|THEN|TIME|TIMESTAMP|TO|TOP|TRAN|TRANSACTION|TRANSLATION|TRIGGER|TRUE|TRUNCATE|TSEQUAL|UNION|UNIQUE|UPDATE|UPDATETEXT|USE|VALUES|VARCHAR|VARYING|VIEW|WAITFOR|WHEN|WHERE|WHILE|WITH|WORK|WRITETEXT|BIGINT|TINYINT|SMALLDATETIME|NTEXT|XML|TYPE)";

        #endregion

        private static readonly Logger Log = Logger.GetLogger("QueryPlanDiagramDialog");

        XmlDocument xmlDocument;
        string estimated = "(Estimated)";
        string actual = " (Actual)";

        string planXML = string.Empty;
        bool isSupported = false;
        bool isActualPlan = false;
        string sqlStatement = string.Empty;
        bool canAnalyze = false;
        private int instanceId = 0;
        
        public QueryPlanDiagramDialog(string planXML, bool isSupported, bool isActualPlan, string sqlStatement, int instanceId, bool canAnalyze)
        {
            InitializeComponent();

            this.planXML = planXML;
            this.isSupported = isSupported;
            this.isActualPlan = isActualPlan;
            this.sqlStatement = sqlStatement;
            this.canAnalyze = canAnalyze;
            this.instanceId = instanceId;

            this.Load += QueryPlanDiagramDialog_Load;
            this.Shown += QueryPlanDiagramDialog_Shown;
        }

        private void QueryPlanDiagramDialog_Load(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            if (isActualPlan)
            {
                this.xmlPlanTab.Text = "Plan XML" + actual;
                this.QueryPlanTab.Text = "Plan Diagram" + actual;
            }
            else
            {
                this.xmlPlanTab.Text = "Plan XML " + estimated;
                this.QueryPlanTab.Text = "Plan Diagram " + estimated;
            }
            var sqlString = string.Empty;
            //Passing instance ID which is required to diagnose query
            if (null == System.Windows.Application.Current)
                new System.Windows.Application();

            if (string.IsNullOrEmpty(planXML))
                throw new NullReferenceException("Must provide a planXML string!");
                      

            xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(planXML);

            planXML = Beautify(xmlDocument);
          
            if (xmlDocument == null)
                throw new NullReferenceException("XMl not loaded ");

            if (isSupported)
            {
                string html = TransformXmlToHtml();
                htmlDisplay.DocumentText = html;
                hiddenBrowser.DocumentText = html;
            }
            else
            {
                htmlDisplay.DocumentText = UnsupportedBrowser();
                hiddenBrowser.DocumentText = UnsupportedBrowser();
            }

            if (String.IsNullOrEmpty(sqlStatement))
                sqlString = ParseSQLFromXML();
            else
                sqlString = sqlStatement.ToString();

            LoadXMLSyntax();
            LoadSQLSyntax();

            sqlEditor.Document.Text = sqlString;

            xmlEditor.Document.Text = planXML;

            SetRowsInGrid();
            
            System.Windows.Forms.Application.DoEvents();

            this.diagnoseQueryButton.Visible = canAnalyze;

            this.Cursor = Cursors.Default;
        }

        private string Beautify(XmlDocument doc)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                NewLineChars = Environment.NewLine,
                NewLineHandling = NewLineHandling.Replace
            };
            using (XmlWriter writer = XmlWriter.Create(sb, settings))
            {
                doc.Save(writer);
            }
            return sb.ToString();
        }

        private void LoadXMLSyntax()
        {
            System.Reflection.Assembly thisExe = System.Reflection.Assembly.GetExecutingAssembly();
            using (System.IO.Stream file = thisExe.GetManifestResourceStream("Idera.SQLdm.DesktopClient.ActiproSoftware.XML.xml"))
            {
                xmlEditor.Document.LoadLanguageFromXml(file, 0);
            }
        }
        private void LoadSQLSyntax()
        {
            System.Reflection.Assembly thisExe = System.Reflection.Assembly.GetExecutingAssembly();
            using (System.IO.Stream file = thisExe.GetManifestResourceStream("Idera.SQLdm.DesktopClient.ActiproSoftware.SQL.xml"))
            {
                sqlEditor.Document.LoadLanguageFromXml(file, 0);
            }
        }


        private void QueryPlanDiagramDialog_Shown(object sender, EventArgs e)
        {
            // to remove flickering from css :hover in this.htmlDisplay before it is has registered a click
            var desiredCursorPoint = htmlDisplay.PointToScreen(System.Drawing.Point.Empty);
            uint lmbDown = 0x02;
            uint lmbUp = 0x04;
            Cursor.Position = desiredCursorPoint;
            if(htmlDisplay.ClientRectangle.Contains(htmlDisplay.PointToClient(Cursor.Position)))
                mouse_event(lmbDown | lmbUp, (uint)desiredCursorPoint.X, (uint)desiredCursorPoint.Y, 0, 0);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        /// <summary>
        /// WebBrowser Displays SQLdm 10.4
        /// </summary>
        /// <returns></returns>
        private string TransformXmlToHtml()
        {
            return (htmlStart + JSFindCaller + FillQueryPlan() + JSDrawer + closing);
        }
        private string UnsupportedBrowser()
        {
            return (htmlStart + unSupportedServerString + closing);
        }

        private string FillQueryPlan()
        {
            try
            {
                using (var stringWriter = new StringWriter())
                {
                    using (var xw = XmlWriter.Create(stringWriter))
                    {
                        XslCompiledTransform myXslTransform = new XslCompiledTransform();
                        myXslTransform.Load(xsltDoc);
                        myXslTransform.Transform(xmlDocument.CreateNavigator(), null, stringWriter);
                        return stringWriter.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                string msg2 = string.Format("An error occurred while presenting the query plan XML design.");
                ApplicationMessageBox.ShowError(this, msg2, ex);
                return string.Empty;
            }
        }
        private string ParseSQLFromXML()
        {
            string sqlStatement = "";
            var nodeData = "";
            try
            {

                XmlNodeList mainNode = xmlDocument.GetElementsByTagName("Statements");
                XmlNodeList elemList = xmlDocument.GetElementsByTagName("StmtSimple");

                StringBuilder sqlString = new StringBuilder(sqlStatement);

                foreach (XmlNode nodes in mainNode)
                {
                    if (nodes == null)
                        continue;
                    foreach (XmlNode node in elemList)
                    {
                        if (node == null)
                            continue;

                        if (node.Attributes["StatementText"] != null)
                            nodeData = node.Attributes["StatementText"].InnerText;

                        sqlString.Append(nodeData);

                    }

                    return sqlString.ToString();
                }
                return ("There was no SQL data!!");
            }
            catch (Exception ex)
            {
                string msg2 = string.Format("An error occurred while presenting the query plan sql statements.");
                ApplicationMessageBox.ShowError(this, msg2, ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Button Controls for SQLdm 10.4 Desktop Client UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ZoomInBtn_Click(object sender, EventArgs e)
        {
            htmlDisplay.Document.InvokeScript("zoomin");
        }
        private void ZoomOutBtn_MouseClick(object sender, MouseEventArgs e)
        {
            htmlDisplay.Document.InvokeScript("zoomout");
        }
        private void ZoomToFitBtn_MouseClick(object sender, MouseEventArgs e)
        {
            htmlDisplay.Document.InvokeScript("zoomtofit");
        }
        private void QueryDiagramExportBtn_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                hiddenBrowser.Document.InvokeScript("exportData");
            }
            catch
            {
                string msg2 = string.Format("An error occurred while presenting the query plan.");
                ApplicationMessageBox.ShowError(this, msg2);
            }

        } 
        private void XmlExportBtn_MouseClick(object sender, MouseEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                NewLineChars = Environment.NewLine,
                NewLineHandling = NewLineHandling.Replace
            };
            using (XmlWriter writer = XmlWriter.Create(sb, settings))
            {
                xmlDocument.Save(writer);

            }
            try
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.FileName = "XMLPlan-" + DateTime.Now.ToString("yyyy-MM-dd--hh-mm-ss") + ".xml";
                saveFileDialog1.Filter = "Xml File (*.xml)|*.xml";
                XmlWriter xmlWriter = XmlWriter.Create(xmlDocument.ToString());
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {

                    xmlDocument.Save(saveFileDialog1.FileName);

                }
            }
            catch(Exception ex)
            {
                string msg2 = string.Format("An error occurred while retrieving the XML Plan.");

                Log.Error(msg2, ex);

                ApplicationMessageBox.ShowError(this, msg2);
            }

        }
        private void SqlExportBtn_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {

                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.FileName = "SqlStatment-" + DateTime.Now.ToString("yyyy-MM-dd--hh-mm-ss") + ".txt";
                saveFileDialog1.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";


                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    using (StreamWriter sqlExportFile = new StreamWriter(saveFileDialog1.FileName))
                        sqlExportFile.Write(sqlEditor.Document.Text);

                }
            }
            catch
            {
                string msg2 = string.Format("An error occurred while presenting the SQL statements.");
                ApplicationMessageBox.ShowError(this, msg2);
            }
        }
        private void QueryExportColumnBtn_MouseClick(object sender, MouseEventArgs e)
        {
            List<ResultGridData> columnData = GetRowDetails();
            try
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.FileName = "QueryColumns-" + DateTime.Now.ToString("yyyy-MM-dd--hh-mm-ss") + ".csv";
                saveFileDialog1.Filter = "CSV file (*.csv)|*.csv| All Files (*.*)|*.*";

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {

                    StringBuilder sb = new StringBuilder();
                    string columnHeads = "Database, Schema, Table, Column";
                    sb.Append(columnHeads);
                    sb.Append(Environment.NewLine);
                    foreach (var row in columnData)
                    {

                        sb.Append(row.Database);
                        sb.Append(",");
                        sb.Append(row.Schema);
                        sb.Append(",");
                        sb.Append(row.Table);
                        sb.Append(",");
                        sb.Append(row.Column);
                        sb.Append(Environment.NewLine);

                    }
                    using (StreamWriter queryColumnExport = new StreamWriter(saveFileDialog1.FileName))
                        queryColumnExport.Write(sb.ToString());
                }
            }
            catch
            {
                string msg2 = string.Format("An error occurred while presenting the query columns display.");
                ApplicationMessageBox.ShowError(this, msg2);
            }
        }/// <summary>
        private void CancelBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void diagnoseQueryButton_Click(object sender, EventArgs e)
        {
            ApplicationController.Default.ShowServerView(instanceId, Idera.SQLdm.DesktopClient.Views.Servers.Server.ServerViews.Analysis, 11);

            this.Close();
        }

        /// UltraGrid layout for Query Columns Requirement of SQLdm 10.4
        /// </summary>
        private void SetRowsInGrid()
        {
            DataTable ultraTable = new DataTable("UltraTable");
            DataColumn column1 = new DataColumn("Database ", typeof(string));
            DataColumn column2 = new DataColumn("Schema ", typeof(string));
            DataColumn column3 = new DataColumn("Table ", typeof(string));
            DataColumn column4 = new DataColumn("Column ", typeof(string));

            ultraTable.Columns.AddRange(new DataColumn[] { column1, column2, column3, column4 });

            List<ResultGridData> columnData = GetRowDetails();

            foreach (var row in columnData)
            {
                DataRow newRow = ultraTable.NewRow();

                newRow[column1] = row.Database;
                newRow[column2] = row.Schema;
                newRow[column3] = row.Table;
                newRow[column4] = row.Column;
                
                ultraTable.Rows.Add(newRow);
            }

            this.dataSet1.Tables.Add(ultraTable);
        }
        private List<ResultGridData> GetRowDetails()
        {

            try
            {
                XmlNamespaceManager namespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);
                namespaceManager.AddNamespace("f", "http://schemas.microsoft.com/sqlserver/2004/07/showplan");

                XmlNodeList elemList = xmlDocument.SelectNodes("f:ShowPlanXML/f:BatchSequence/f:Batch/f:Statements/f:StmtSimple/f:QueryPlan/f:RelOp/f:OutputList/f:ColumnReference", namespaceManager);
                List<ResultGridData> gridRowData = new List<ResultGridData>(elemList.Count);

                foreach (XmlNode n in elemList)
                {
                    ResultGridData newRow = new ResultGridData();

                    if (n.Attributes["Database"] != null)
                        newRow.Database = n.Attributes["Database"].InnerText;
                    else
                        newRow.Database = "[null]";
                    if (n.Attributes["Schema"] != null)
                        newRow.Schema = n.Attributes["Schema"].InnerText;
                    else
                        newRow.Schema = "[null]";
                    if (n.Attributes["Table"] != null)
                        newRow.Table = n.Attributes["Table"].InnerText;
                    else
                        newRow.Table = "[null]";
                    if (n.Attributes["Column"] != null)
                        newRow.Column = n.Attributes["Column"].InnerText;
                    else
                        newRow.Column = "[null]";
                    gridRowData.Add(newRow);
                }

                return gridRowData;
            }

            catch (System.Exception ex)
            {
                string msg2 = string.Format("An error occurred while gathering the data for results rows.");
                ApplicationMessageBox.ShowError(this, msg2, ex);
                return null;
            }

        }

    }
}