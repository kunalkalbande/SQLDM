using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using ChartFX.WinForms;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.Printing;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.DesktopClient.Dialogs;
using Wintellect.PowerCollections;
using Infragistics.Excel;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Configuration;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Objects;
using System.Text;

namespace Idera.SQLdm.DesktopClient.Helpers
{
    internal static class ExportHelper
    {
        public static string GetValidFileName(string filename, bool removeSpaces)
        {
            string fn = filename;
            if (removeSpaces)
            {
                fn = fn.Replace(" ", string.Empty);
            }

            return GetValidFileName(fn);
        }
        public static string GetValidFileName(string filename)
        {
            string fn = filename;
            foreach (char chr in Path.GetInvalidFileNameChars())
            {
                fn = fn.Replace(chr.ToString(), string.Empty);
            }
            return fn;
        }

        /// <summary>
        /// Exports the blocking report
        /// <author>Gaurav Karwal</author>
        /// </summary>
        /// <param name="owner">the window calling this function</param>
        /// <param name="blockXML">the xml data to be exported</param>
        /// <param name="defaultFileName">name of the file to be exported</param>
        public static void ExportBlockGraph(IWin32Window owner, string blockXML, string defaultFileName)
        {
            string fileName = XmlWriter.GetFileName(owner, defaultFileName);
            if (fileName.Length > 0)
            {
                try
                {
                    FileStream fs = new FileStream(fileName, FileMode.Create);
                    StreamWriter sw = new StreamWriter(fs);
                    sw.Write(blockXML);
                    sw.Close();
                }
                catch (Exception ex)
                {
                    ApplicationMessageBox.ShowError(owner, "Unable to export block graph", ex);
                }
            }
        }

        public static void ExportDeadlockGraph(IWin32Window owner, string xdl, string defaultFileName)
        {
            string fileName = XdlWriter.GetFileName(owner, defaultFileName);
            if (fileName.Length > 0)
            {
                try
                {
                    FileStream fs = new FileStream(fileName, FileMode.Create);
                    StreamWriter sw = new StreamWriter(fs);
                    sw.Write(xdl);
                    sw.Close();
                }
                catch (Exception ex)
                {
                    ApplicationMessageBox.ShowError(owner, "Unable to export deadlock graph", ex);
                }
            }
        }

        public static void ExportToCsv(IWin32Window owner, DataTable table, string defaultFilename)
        {
            string filename = CsvWriter.GetFileName(defaultFilename);

            ExportToCsvFile(owner, table, filename);
        }

        public static void ExportToCsvFile(IWin32Window owner, DataTable table, string fileName)
        {
            if (fileName.Length > 0)
            {
                try
                {
                    string str = CsvWriter.WriteToString(table, new string[0], true, false);
                    FileStream fs = new FileStream(fileName, FileMode.Create);
                    StreamWriter sw = new StreamWriter(fs);
                    sw.Write(str);
                    sw.Close();
                }
                catch (Exception ex)
                {
                    ApplicationMessageBox.ShowError(owner, "Unable to export chart data", ex);
                }
            }
        }

        internal static class ChartHelper
        {
            public static void PrintChartWithTitle(IWin32Window owner, Chart chart, string title, UltraPrintPreviewDialog previewDialog)
            {
                try
                {
                    chart.SuspendLayout();
                    AddChartTitle(chart, title);

                    PrintChart(owner, chart, previewDialog);

                    RemoveChartTitle(chart, title);
                    chart.ResumeLayout();
                }
                catch (Exception ex)
                {
                    ApplicationMessageBox.ShowError(owner, "Unable to set chart title", ex);
                }
            }

            public static void PrintChart(IWin32Window owner, Chart chart, UltraPrintPreviewDialog previewDialog)
            {
                try
                {
                    // use the Infragistics Print Preview window so the chart and grid are consistent
                    previewDialog.Document = chart.Printer.Document;
                    previewDialog.ShowDialog();
                }
                catch (Exception ex)
                {
                    ApplicationMessageBox.ShowError(owner, "Unable to print chart", ex);
                }
            }

            public static void ExportImageWithTitle(IWin32Window owner, Chart chart, string title, string defaultFilename)
            {
                string filename = ImageWriter.GetFileName(defaultFilename);
                if (filename.Length > 0)
                {
                    try
                    {
                        //add the title to the chart for exporting and then remove it after done
                        chart.SuspendLayout();
                        AddChartTitle(chart, title);

                        ExportImageFile(owner, chart, filename);

                        RemoveChartTitle(chart, title);
                        chart.ResumeLayout();
                    }
                    catch (Exception ex)
                    {
                        ApplicationMessageBox.ShowError(owner, "Unable to set chart title", ex);
                    }
                }
            }

            public static void ExportImageFile(IWin32Window owner, Chart chart, string fileName)
            {
                if (fileName.Length > 0)
                {
                    try
                    {
                        chart.Export(FileFormat.Bitmap, fileName);
                    }
                    catch (Exception ex)
                    {
                        ApplicationMessageBox.ShowError(owner, "Unable to export chart image", ex);
                    }
                }
            }

            public static void ExportToCsv(IWin32Window owner, Chart chart, string defaultFilename)
            {
                string filename = CsvWriter.GetFileName(defaultFilename);

                ExportToCsvFile(owner, chart, filename);
            }

            public static void ExportToCsvFile(IWin32Window owner, Chart chart, string fileName)
            {
                if (fileName.Length > 0)
                {
                    try
                    {
                        string[] columns = GetFieldList(chart);
                        string str = CsvWriter.WriteToString(chart.DataSource, columns, true, false);
                        FileStream fs = new FileStream(fileName, FileMode.Create);
                        StreamWriter sw = new StreamWriter(fs);
                        sw.Write(str);
                        sw.Close();
                    }
                    catch (Exception ex)
                    {
                        ApplicationMessageBox.ShowError(owner, "Unable to export chart data", ex);
                    }
                }
            }

            public static string[] GetFieldList(Chart chart)
            {
                string[] columns = new string[chart.DataSourceSettings.Fields.Count];
                int i = 0;
                // Process the x values first
                foreach (FieldMap field in chart.DataSourceSettings.Fields)
                {
                    if (field.Usage == FieldUsage.XValue)
                    {
                        columns.SetValue(field.Name, i);
                        i++;
                    }
                }

                foreach (FieldMap field in chart.DataSourceSettings.Fields)
                {
                    if (field.Usage != FieldUsage.XValue)
                    {
                        columns.SetValue(field.Name, i);
                        i++;
                    }
                }

                return columns;
            }

            private static TitleDockable AddChartTitle(Chart chart, string title)
            {
                TitleDockable newTitle = new TitleDockable();
                newTitle.Text = title;
                newTitle.Font = new Font(newTitle.Font, FontStyle.Bold);
                chart.Titles.Add(newTitle);
                newTitle.Dock = DockArea.Top;

                return newTitle;
            }

            private static void RemoveChartTitle(Chart chart, string title)
            {
                for (int i = 0; i < chart.Titles.Count; i++)
                {
                    if (chart.Titles[i].Text == title)
                    {
                        chart.Titles.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        internal static class ImageWriter
        {
            public static string GetFileName(string defaultFilename)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();

                saveFileDialog.DefaultExt = "bmp";
                saveFileDialog.FileName = defaultFilename;
                saveFileDialog.Filter = "Bitmap (*.bmp)|*.bmp";
                saveFileDialog.Title = "Save as Image";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    return saveFileDialog.FileName;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        internal static class XdlWriter
        {
            public static string GetFileName(IWin32Window owner, string defaultFilename)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();

                saveFileDialog.DefaultExt = "xdl";
                saveFileDialog.FileName = defaultFilename;
                saveFileDialog.Filter = "Deadlock Graph (*.xdl)|*.xdl";
                saveFileDialog.Title = "Save Deadlock Graph";
                if (saveFileDialog.ShowDialog(owner) == DialogResult.OK)
                {
                    return saveFileDialog.FileName;
                }
                else
                {
                    return string.Empty;
                }
            }

        }


        /// <summary>
        /// Class representing the helper that writes XML data to disk
        /// </summary>
        internal static class XmlWriter
        {
            /// <summary>
            /// Gets the filename input from the user for blocking report
            /// </summary>
            /// <param name="owner">caller window</param>
            /// <param name="defaultFilename">file name by default - can be changed by the user</param>
            /// <returns>file name that the user entered - default if user did not change</returns>
            public static string GetFileName(IWin32Window owner, string defaultFilename)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();

                saveFileDialog.DefaultExt = "xml";
                saveFileDialog.FileName = defaultFilename;
                saveFileDialog.Filter = "Blocking Graph (*.xml)|*.xml";
                saveFileDialog.Title = "Save Blocking Graph";
                if (saveFileDialog.ShowDialog(owner) == DialogResult.OK)
                {
                    return saveFileDialog.FileName;
                }
                else
                {
                    return string.Empty;
                }
            }

        }

        internal static class CsvWriter
        {
            public static string GetFileName(string defaultFilename)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();

                saveFileDialog.DefaultExt = "csv";
                saveFileDialog.FileName = defaultFilename;
                saveFileDialog.Filter = "Text File for Excel (*.csv)|*.csv";
                saveFileDialog.Title = "Save Chart Data";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    return saveFileDialog.FileName;
                }
                else
                {
                    return string.Empty;
                }
            }

            public static string WriteToString(object dataSource, string[] columns, bool header, bool quoteall)
            {
                StringWriter writer = new StringWriter();
                WriteToStream(writer, dataSource, columns, header, quoteall);
                return writer.ToString();
            }

            public static void WriteToStream(TextWriter stream, object dataSource, string[] columns, bool header, bool quoteall)
            {
                if (dataSource is BindingSource)
                    dataSource = ((BindingSource)dataSource).DataSource;

                if (dataSource is UltraDataSource)
                {
                    UltraDataSource tbl = (UltraDataSource)dataSource;
                    UltraDataColumnsCollection tblColumns = tbl.Band.Columns;
                    if (header)
                    {
                        if (columns.Length > 0)
                        {
                            for (int i = 0; i < columns.Length; i++)
                            {
                                // make sure the column is actually in the table
                                if (tblColumns.IndexOf(columns[i]) > -1)
                                {
                                    WriteItem(stream, columns[i], quoteall);
                                    if (i < columns.Length - 1)
                                        stream.Write(',');
                                    else
                                        stream.Write(stream.NewLine);
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < tblColumns.Count; i++)
                            {
                                WriteItem(stream, tblColumns[i].Key, quoteall);
                                if (i < tblColumns.Count - 1)
                                    stream.Write(',');
                                else
                                    stream.Write(stream.NewLine);
                            }
                        }
                    }

                    foreach (UltraDataRow row in tbl.Rows)
                    {
                        if (columns.Length > 0)
                        {
                            for (int i = 0; i < columns.Length; i++)
                            {
                                if (tblColumns.IndexOf(columns[i]) > -1)
                                {
                                    WriteItem(stream, row[columns[i]], quoteall);
                                    if (i < columns.Length - 1)
                                        stream.Write(',');
                                    else
                                        stream.Write(stream.NewLine);
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < tblColumns.Count; i++)
                            {
                                WriteItem(stream, row[i], quoteall);
                                if (i < tblColumns.Count - 1)
                                    stream.Write(',');
                                else
                                    stream.Write(stream.NewLine);
                            }
                        }
                    }
                }
                else if (dataSource is DataSet || dataSource is DataTable)
                {
                    DataTable tbl;
                    if (dataSource is DataSet)
                    {
                        if (dataSource is BindingSource && ((BindingSource)dataSource).DataMember.Length > 0)
                        {
                            tbl = ((DataSet)dataSource).Tables[((BindingSource)dataSource).DataMember];
                        }
                        else
                        {
                            tbl = ((DataSet)dataSource).Tables[0];
                        }
                    }
                    else
                    {
                        tbl = (DataTable)dataSource;
                    }
                    DataColumnCollection tblColumns = tbl.Columns;
                    if (header)
                    {
                        if (columns.Length > 0)
                        {
                            for (int i = 0; i < columns.Length; i++)
                            {
                                // make sure the column is actually in the table
                                if (tblColumns.IndexOf(columns[i]) > -1)
                                {
                                    WriteItem(stream, columns[i], quoteall);
                                    if (i < columns.Length - 1)
                                        stream.Write(',');
                                    else
                                        stream.Write(stream.NewLine);
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < tblColumns.Count; i++)
                            {
                                WriteItem(stream, tblColumns[i].ColumnName, quoteall);
                                if (i < tblColumns.Count - 1)
                                    stream.Write(',');
                                else
                                    stream.Write(stream.NewLine);
                            }
                        }
                    }

                    foreach (DataRow row in tbl.Rows)
                    {
                        if (columns.Length > 0)
                        {
                            for (int i = 0; i < columns.Length; i++)
                            {
                                if (tblColumns.IndexOf(columns[i]) > -1)
                                {
                                    WriteItem(stream, row[columns[i]], quoteall);
                                    if (i < columns.Length - 1)
                                        stream.Write(',');
                                    else
                                        stream.Write(stream.NewLine);
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < tblColumns.Count; i++)
                            {
                                WriteItem(stream, row[i], quoteall);
                                if (i < tblColumns.Count - 1)
                                    stream.Write(',');
                                else
                                    stream.Write(stream.NewLine);
                            }
                        }
                    }
                }
                else if (dataSource is DataView)
                {
                    DataView tbl = (DataView)dataSource;
                    DataColumnCollection tblColumns = tbl.Table.Columns;
                    if (header)
                    {
                        if (columns.Length > 0)
                        {
                            for (int i = 0; i < columns.Length; i++)
                            {
                                // make sure the column is actually in the table
                                if (tblColumns.IndexOf(columns[i]) > -1)
                                {
                                    WriteItem(stream, columns[i], quoteall);
                                    if (i < columns.Length - 1)
                                        stream.Write(',');
                                    else
                                        stream.Write(stream.NewLine);
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < tblColumns.Count; i++)
                            {
                                WriteItem(stream, tblColumns[i].ColumnName, quoteall);
                                if (i < tblColumns.Count - 1)
                                    stream.Write(',');
                                else
                                    stream.Write(stream.NewLine);
                            }
                        }
                    }

                    foreach (DataRowView row in tbl)
                    {
                        if (columns.Length > 0)
                        {
                            for (int i = 0; i < columns.Length; i++)
                            {
                                if (tblColumns.IndexOf(columns[i]) > -1)
                                {
                                    WriteItem(stream, row[columns[i]], quoteall);
                                    if (i < columns.Length - 1)
                                        stream.Write(',');
                                    else
                                        stream.Write(stream.NewLine);
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < tblColumns.Count; i++)
                            {
                                WriteItem(stream, row[i], quoteall);
                                if (i < tblColumns.Count - 1)
                                    stream.Write(',');
                                else
                                    stream.Write(stream.NewLine);
                            }
                        }
                    }
                }
            }

            private static void WriteItem(TextWriter stream, object item, bool quoteall)
            {
                if (item == null)
                    return;
                string s = item.ToString();
                if (quoteall || s.IndexOfAny("\",\x0A\x0D".ToCharArray()) > -1)
                    stream.Write("\"" + s.Replace("\"", "\"\"") + "\"");
                else
                    stream.Write(s);
            }

            static void backgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
            {
                if (e.Cancelled)
                    return;
                if (e.Error != null)
                {
                    BackgroundWorkerWithProgressDialog worker = sender as BackgroundWorkerWithProgressDialog;
                    ApplicationMessageBox.ShowError(worker.ownerForm, "Unable to export chart data", e.Error);
                    return;
                }
            }

            private static void backgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
            {
                BackgroundWorkerWithProgressDialog worker = (BackgroundWorkerWithProgressDialog)sender;
                worker.SetStatusText("Exporting to Excel...");
                Pair<Stream, IRecommendation[]> args = (Pair<Stream, IRecommendation[]>)e.Argument;
                try
                {
                    Write(args.First, args.Second, worker);
                }
                finally
                {
                    if (args.First != null)
                        args.First.Close();
                }
            }
            public static void Write(Stream stream, IRecommendation[] recommendations, BackgroundWorkerWithProgressDialog worker)
            {
                Workbook book = new Workbook(WorkbookFormat.Excel97To2003);
                Worksheet sheet = book.Worksheets.Add("SQL doctor Recommendations");

                //if (null == ar) throw new ApplicationException("Export failed due to no analysis results");
                //ServerConfiguration config = Common.CommonSettings.Default.FindServer(ar.InstanceName);

                //if (null == ar) throw new ApplicationException(string.Format("Export failed.  Could not find configuration for {0}", ar.InstanceName));
                //SqlConnectionInfo ci = config.ConnectionInfo;

                int r = 0;

                sheet.DisplayOptions.FrozenPaneSettings.FrozenRows = 1;
                WorksheetRow row = sheet.Rows[r];

                row.Cells[0].Value = "ID";
                row.Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                WorksheetColumn column = sheet.Columns[0];
                column.Width = 10 * 256;

                row.Cells[1].Value = "Category";
                row.Cells[1].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                column = sheet.Columns[1];
                column.Width = 20 * 256;

                row.Cells[2].Value = "Finding";
                row.Cells[2].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                column = sheet.Columns[2];
                column.Width = 75 * 256;
                column.CellFormat.WrapText = ExcelDefaultableBoolean.True;

                row.Cells[3].Value = "Recommendation";
                row.Cells[3].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                column = sheet.Columns[3];
                column.Width = 75 * 256;
                column.CellFormat.WrapText = ExcelDefaultableBoolean.True;

                row.Cells[4].Value = "Impact Explanation";
                row.Cells[4].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                column = sheet.Columns[4];
                column.Width = 75 * 256;
                column.CellFormat.WrapText = ExcelDefaultableBoolean.True;

                row.Cells[5].Value = "Additional Considerations";
                row.Cells[5].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                column = sheet.Columns[5];
                column.Width = 75 * 256;
                column.CellFormat.WrapText = ExcelDefaultableBoolean.True;

                row.Cells[6].Value = "Database";
                row.Cells[6].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                column = sheet.Columns[6];
                column.Width = 20 * 256;

                row.Cells[7].Value = "Table";
                row.Cells[7].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                column = sheet.Columns[7];
                column.Width = 20 * 256;

                row.Cells[8].Value = "Application";
                row.Cells[8].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                column = sheet.Columns[8];
                column.Width = 20 * 256;

                row.Cells[9].Value = "Login Name";
                row.Cells[9].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                column = sheet.Columns[9];
                column.Width = 10 * 256;

                row.Cells[10].Value = "Workstation";
                row.Cells[10].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                column = sheet.Columns[10];
                column.Width = 10 * 256;

                row.Cells[11].Value = "TSQL Batch";
                row.Cells[11].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                column = sheet.Columns[11];
                column.Width = 75 * 256;
                column.CellFormat.WrapText = ExcelDefaultableBoolean.True;

                row.Cells[12].Value = "Fix Script";
                row.Cells[12].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                column = sheet.Columns[12];
                column.Width = 75 * 256;
                column.CellFormat.WrapText = ExcelDefaultableBoolean.True;

                row.Cells[13].Value = "Undo Script";
                row.Cells[13].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                column = sheet.Columns[13];
                column.Width = 75 * 256;
                column.CellFormat.WrapText = ExcelDefaultableBoolean.True;

                row.Cells[14].Value = "Learn more about";
                row.Cells[14].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                column = sheet.Columns[14];
                column.Width = 75 * 256;
                column.CellFormat.WrapText = ExcelDefaultableBoolean.True;


                r++;
                foreach (var recommendation in recommendations)
                {
                    if (worker.CancellationPending)
                        return;

                    AddRow(sheet, ref r, recommendation);
                }

                book.Save(stream);
            }
            private static void AddRow(Worksheet sheet, ref int rowIndex, IRecommendation r)
            {
                if (null == r) return;
                //if (r is IList<IRecommendation>)
                //{
                //    var gr = (IList<IRecommendation>)r;
                //    foreach (IRecommendation recommendation in gr)
                //    {
                //        AddRow(sheet, ref rowIndex, recommendation, ci, ar);
                //    }
                //    return;
                //}

                WorksheetRow row = sheet.Rows[rowIndex];
                row.CellFormat.VerticalAlignment = VerticalCellAlignment.Top;

                row.Cells[0].Value = r.ID;
                row.Cells[1].Value = r.Category;
                row.Cells[2].Value = r.FindingText;
                row.Cells[3].Value = r.RecommendationText;
                row.Cells[4].Value = r.ImpactExplanationText;
                row.Cells[5].Value = r.AdditionalConsiderations;

                try
                {

                    if (r is IProvideDatabase)
                    {
                        string s = ((IProvideDatabase)r).Database;
                        if (!String.IsNullOrEmpty(s))
                            row.Cells[6].Value = s;
                    }

                    if (r is IProvideTableName)
                    {
                        string s = ((IProvideTableName)r).Schema;
                        string t = ((IProvideTableName)r).Table;
                        if (!String.IsNullOrEmpty(t))
                            row.Cells[7].Value = String.Format("{0}.{1}", String.IsNullOrEmpty(s) ? "dbo" : s, t);
                    }

                    if (r is IProvideApplicationName)
                    {
                        string s = ((IProvideApplicationName)r).ApplicationName;
                        if (!String.IsNullOrEmpty(s))
                            row.Cells[8].Value = s;
                    }

                    if (r is IProvideUserName)
                    {
                        string s = ((IProvideUserName)r).UserName;
                        if (!String.IsNullOrEmpty(s))
                            row.Cells[9].Value = s;
                    }

                    if (r is IProvideHostName)
                    {
                        string s = ((IProvideHostName)r).HostName;
                        if (!String.IsNullOrEmpty(s))
                            row.Cells[10].Value = s;
                    }

                    if (r is TSqlRecommendation)
                    {
                        TSqlRecommendation tsqlr = r as TSqlRecommendation;
                        if (null != tsqlr)
                            if (null != tsqlr.Sql)
                                if (null != tsqlr.Sql.Script)
                                {
                                    string tsql = tsqlr.Sql.Script;
                                    if (!String.IsNullOrEmpty(tsql))
                                    {
                                        if (tsql.Length > 32767)
                                            tsql = tsql.Substring(0, 32700).TrimEnd() + "..." + Environment.NewLine + "  TSQL exceeds Excel cell limit.";
                                        row.Cells[11].Value = tsql;
                                    }
                                }
                    }

                    #region column 11
                    string fix = null;
                    try
                    {
                        if (r.IsScriptGeneratorProvider)
                        {
                            IScriptGenerator isg = ((IScriptGeneratorProvider)r).GetScriptGenerator();
                            if (isg != null)
                            {
                                //fix = isg.GetTSqlFix(ci);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // eat any exceptions thrown from script generator
                    }

                    if (!String.IsNullOrEmpty(fix))
                    {
                        if (fix.Length > 32767)
                        {
                            fix = fix.Substring(0, 32700).TrimEnd() + "..." + Environment.NewLine + "  Fix script exceeds Excel cell limit.";
                        }
                        row.Cells[12].Value = fix;
                    }
                    #endregion

                    #region column 12
                    string undo = null;
                    try
                    {
                        if (r.IsUndoScriptGeneratorProvider)
                        {
                            IUndoScriptGenerator iusg = ((IUndoScriptGeneratorProvider)r).GetUndoScriptGenerator();
                            if (iusg != null)
                            {
                                //undo = iusg.GetTSqlUndo(ci);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // eat any exceptions thrown from script generator
                    }
                    if (!String.IsNullOrEmpty(undo))
                    {
                        if (undo.Length > 32767)
                        {
                            undo = undo.Substring(0, 32700).TrimEnd() + "..." + Environment.NewLine + "  Undo script exceeds Excel cell limit.";
                        }
                        row.Cells[13].Value = undo;
                    }
                    #endregion

                    #region column 13
                    RecommendationLinks links = r.Links;
                    //if (links != null) links = links.Filtered(r, ar);
                    if (links != null && links.Count > 0)
                    {
                        StringBuilder builder = new StringBuilder();
                        foreach (var l in links)
                        {
                            if (!String.IsNullOrEmpty(l.Link))
                            {
                                if (builder.Length > 0) builder.AppendLine();
                                builder.Append(l.Link);
                            }
                        }
                        if (builder.Length > 0) row.Cells[14].Value = builder.ToString();
                    }
                    #endregion
                }
                catch (Exception exRow)
                {

                    //ApplicationMessageBox.ShowError(owner,string.Format("ExportHelper.AddRow({0}, {1}, {2})", rowIndex, r.ID, r.FindingText), exRow);
                }

                rowIndex++;
            }
        }
    }
}
