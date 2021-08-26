using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using ActiproSoftware.SyntaxEditor;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinListView;
using TracerX;
using Infragistics.Win.UltraWinStatusBar;
using Wintellect.PowerCollections;
using System.Text;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.UI.Dialogs;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    public partial class SqlViewerDialog : Form//, IRecommendationViewer
    {
        private static Logger LOG = Logger.GetLogger("SqlViewerDialog");
        private int spanIndicatorIndex = -1;
        //private string _helpTopic = HelpTopics.ShowMeTheProblem;

        private IList<IRecommendation> recommendations;
        private FormWindowState restoreWindowState;
        private UltraStatusPanel cursorPosPanel;
        private int instanceID;

        public SqlViewerDialog(int instanceID)
        {
            InitializeComponent();
            cursorPosPanel = statusBar.Panels["CursorPos"];
            Icon = Idera.SQLdm.DesktopClient.Properties.Resources.SQLRecommendationScript;
            restoreWindowState = WindowState;
            this.instanceID = instanceID;
            //Recommendations = new List<IRecommendation>();
        }

        private void SqlViewerDialog_Load(object sender, EventArgs args)
        {
            try
            {
                System.Reflection.Assembly thisExe = System.Reflection.Assembly.GetExecutingAssembly();
                using (System.IO.Stream file = thisExe.GetManifestResourceStream("Idera.SQLdm.DesktopClient.ActiproSoftware.SQL.xml"))
                {
                    syntaxEditor1.Document.LoadLanguageFromXml(file, 0);
                }
            }
            catch (Exception e)
            {
                LOG.Warn("Failed to load SQL syntax definitions for viewer.  ", e);
            }
        }

        private void GotoNextSpanIndicator(string name)
        {
            GotoNextSpanIndicator(name, false);
        }
        
        private void GotoNextSpanIndicator(string name, bool top)
        {
            if (syntaxEditor1.Document.SpanIndicatorLayers.Contains(name))
            {
                SpanIndicatorLayer layer = syntaxEditor1.Document.SpanIndicatorLayers[name];
                if (layer.Count > 0)
                {

                    spanIndicatorIndex++;
                    if (spanIndicatorIndex >= layer.Count)
                        spanIndicatorIndex = 0;

                    SpanIndicator indicator = layer[spanIndicatorIndex];
                    EditorView view = syntaxEditor1.SelectedView;
                    view.GotoNextSpanIndicator(layer, indicator.Name);
                    if (top)
                        view.ScrollLineToVisibleBottom();
                    else
                        view.EnsureVisible(indicator.TextRange.StartOffset, true); 
                    UpdateToolbar();
                }
            }
        }

        private void GotoPreviousSpanIndicator(string name)
        {
            if (syntaxEditor1.Document.SpanIndicatorLayers.Contains(name))
            {
                SpanIndicatorLayer layer = syntaxEditor1.Document.SpanIndicatorLayers[name];
                if (layer.Count > 0)
                {

                    spanIndicatorIndex--;
                    if (spanIndicatorIndex < 0)
                        spanIndicatorIndex = layer.Count - 1;

                    SpanIndicator indicator = layer[spanIndicatorIndex];
                    EditorView view = syntaxEditor1.SelectedView;
                    view.GotoPreviousSpanIndicator(layer, indicator.Name);
                    view.EnsureVisible(indicator.TextRange.StartOffset, true);
                    UpdateToolbar();
                }
            }
        }

        private void UpdateToolbar()
        {
            SpanIndicatorLayer layer = syntaxEditor1.Document.SpanIndicatorLayers["problems"];
            ButtonTool next = (ButtonTool)toolbarManager.Tools["NextButton"];
            next.SharedProps.Enabled = (layer != null && layer.Count > 1 && spanIndicatorIndex < layer.Count - 1);     
            ButtonTool prev = (ButtonTool)toolbarManager.Tools["PreviousButton"];
            prev.SharedProps.Enabled = (layer != null && layer.Count > 1 && spanIndicatorIndex > 0);
        }

        public string Script
        {
            get { return syntaxEditor1.Document.Text; }
            set { syntaxEditor1.Document.Text = value; }
        }

        internal void SetSelectionRectangle(BufferLocation location, int length)
        {
            if (location.Offset < 0)
                return;

            SpanIndicatorLayer layer = null;
            if (syntaxEditor1.Document.SpanIndicatorLayers.Contains("selection"))
                layer = syntaxEditor1.Document.SpanIndicatorLayers["selection"];
            else
            {
                layer = new SpanIndicatorLayer("selection", 1);
                syntaxEditor1.Document.SpanIndicatorLayers.Add(layer);
            }

            int line = location.Line - 1;
            if (line < 0)
                line = 0;
            int column = location.Column - 1;
            if (column < 0)
                column = 0;

            DocumentPosition position = new DocumentPosition(line, column);
            int offset = syntaxEditor1.Document.PositionToOffset(position);

            var indicator = new CurrentStatementSpanIndicator(String.Format("statement:{0}-{1}", offset, length), Color.Black, Color.Yellow);
            layer.Add(indicator, offset, length);
        }

        internal void SetErrorRectangle(BufferLocation location, int length, Color squiggleColor)
        {
            SpanIndicatorLayer layer = null;

            if (syntaxEditor1.Document.SpanIndicatorLayers.Contains("problems"))
                layer = syntaxEditor1.Document.SpanIndicatorLayers["problems"];
            else
            {
                layer = new SpanIndicatorLayer("problems", 2);
                syntaxEditor1.Document.SpanIndicatorLayers.Add(layer);
            }

            int line = location.Line - 1;
            if (line < 0)
                line = 0;
            int column = location.Column - 1;
            if (column < 0)
                column = 0;

            DocumentPosition position = new DocumentPosition(line, column);
            int offset = syntaxEditor1.Document.PositionToOffset(position);

            var indicator = new WaveLineSpanIndicator(String.Format("problem:{0}-{1}", offset, length), squiggleColor);
            layer.Add(indicator, offset, length);
        }

        private int GetCharacterCount(string s, char ch)
        {
            int result = 0;
            foreach (char c in s)
            {
                if (c == ch)
                    result++;
            }
            return result;
        }

        private void toolbarManager_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "NextButton":
                    GotoNextSpanIndicator("problems");
                    break;
                case "PreviousButton":
                    GotoPreviousSpanIndicator("problems");
                    break;

            }
        }

        public IList<IRecommendation> Recommendations
        {
            get
            {
                return recommendations;
            }
            set
            {
                recommendations = value;
                if (recommendations != null)
                {
                    LoadEditor();
                }
            }
        }

        private void LoadEditor()
        {
            IRecommendation r = null;
            lvRecommendations.Items.Clear();
            if (null != recommendations)
            {
                foreach (var i in recommendations)
                {
                    if (null == i) continue;

                    if ((i is TSqlRecommendation) || (i.IsScriptGeneratorProvider))
                    { 
                        if (null == r) r = i;
                        AddItem(i);
                    }
                }
            }
            if (lvRecommendations.Items.Count <= 1)
            {
                splitContainer1.Panel2Collapsed = true;
                splitContainer1.Panel2.Hide();
            }
            else
            {
                lvRecommendations.ViewSettingsDetails.ImageSize = Size.Empty;
                splitContainer1.Panel2Collapsed = false;
                splitContainer1.Panel2.Show();
            }
            LoadRecommendationIntoEditor(r);
        }

        private void AddItem(IRecommendation i)
        {
            var item = new UltraListViewItem();
            item.Value = i.FindingText;
            item.Tag = i;
            lvRecommendations.Items.Add(item);
            if (lvRecommendations.SelectedItems.Count <= 0)
                lvRecommendations.SelectedItems.Add(item);
        }
        private void LoadRecommendationIntoEditor(IRecommendation r)
        {
            //_helpTopic = HelpTopics.ShowMeTheProblem;
            if (null == r) { Script = string.Empty; return; }
            if (r is TSqlRecommendation) LoadEditor((TSqlRecommendation)r);
            else LoadEditor((Recommendation)r);
        }

        private void LoadEditor(PredicateRecommendation r)
        {
            using (LOG.DebugCall("LoadEditor(PredicateRecommendation r)"))
            {
                string batch = r.Batch.Replace("\r", "");
                string stmt = r.StatementText.Replace("\r", "");

                Script = batch;

                LOG.DebugFormat("Batch:\n{0}", batch);
                LOG.DebugFormat("Statement:\n{0}", stmt);
                LOG.DebugFormat("Batch Length:{0}  Statement Length:{1}", batch.Length, stmt.Length);
                SpanIndicatorLayer layer = null;
                SpanIndicatorLayer probs = null;
                if (syntaxEditor1.Document.SpanIndicatorLayers.Contains("problems")) probs = syntaxEditor1.Document.SpanIndicatorLayers["problems"];
                if (syntaxEditor1.Document.SpanIndicatorLayers.Contains("selection"))
                {
                    layer = syntaxEditor1.Document.SpanIndicatorLayers["selection"];
                }
                else
                {
                    layer = new SpanIndicatorLayer("selection", 1);
                    syntaxEditor1.Document.SpanIndicatorLayers.Add(layer);
                }
                int n = batch.IndexOf(stmt);
                LOG.DebugFormat("Starting index of statement within batch is {0}", n);
                List<Pair<int, int>> offsets = GetPredicateOffsets(r, stmt);

                while (n >= 0)
                {
                    try
                    {
                        LOG.DebugFormat("Creating statement indicator at {0} for a length of {1}", n, stmt.Length);
                        var indicator = new CurrentStatementSpanIndicator(String.Format("statement:{0}-{1}", n, stmt.Length), Color.Black, Color.Yellow);
                        LOG.Debug("Adding statement indicator");
                        layer.Add(indicator, n, stmt.Length);
                        LOG.Debug("Added statement indicator");
                        foreach (var o in offsets)
                        {
                            if (null == probs)
                            {
                                probs = new SpanIndicatorLayer("problems", 2);
                                syntaxEditor1.Document.SpanIndicatorLayers.Add(probs);
                            }
                            LOG.DebugFormat("Creating problem indicator at {0} for a length of {1}", n + o.First, o.Second);
                            var w = new WaveLineSpanIndicator(String.Format("problem:{0}-{1}", n + o.First, o.Second), Color.Red);
                            LOG.Debug("Adding problem indicator");
                            probs.Add(w, n + o.First, o.Second);
                            LOG.Debug("Added problem indicator");
                        }

                    }
                    catch (Exception ex)
                    {
                        LOG.Error(LOG, "LoadEditor(PredicateRecommendation r) Exception:", ex);
                    }
                    n = batch.IndexOf(stmt, n + stmt.Length);
                }
            }
        }

        private List<Pair<int, int>> GetPredicateOffsets(PredicateRecommendation r, string stmt)
        {
            using (LOG.DebugCall("GetPredicateOffsets(PredicateRecommendation r)"))
            {
                var offsets = new List<Pair<int, int>>();
                var fr = r as FunctionInPredicateRecommendation;
                if (null != fr)
                {
                    int starting = stmt.IndexOf("where", 0, StringComparison.InvariantCultureIgnoreCase);
                    if (starting < 0) starting = 0;
                    int n = 0;
                    int l = fr.FunctionName.Length;
                    while (0 < (n = stmt.IndexOf(fr.FunctionName, starting, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        offsets.Add(new Pair<int, int>(n, l));
                        starting = n + 1;
                        LOG.DebugFormat("Adding predicate offset {0} length {1}", n, l);
                    }
                }
                return (offsets);
            }

        }
        private void LoadEditor(TSqlRecommendation recommendation)
        {
            var pr = recommendation as PredicateRecommendation;
            if (null != pr)
            {
                LoadEditor(pr);
                return;
            }
            Script = recommendation.Sql.Script;

            SelectionRectangle rect = recommendation.Sql.StatementSelection;
            if (rect != null)
            {
                bool dumpScript = false;
                try
                {
                    SetSelectionRectangle(rect.Start, rect.Length);
                    #if DEBUG
                    LOG.InfoFormat("Error setting selection rectangle (Offset:{0},Line:{1},Length:{2})", rect.Start.Offset, rect.Start.Line, rect.Length);
                    dumpScript = true;
                    #endif
                }
                catch (Exception e)
                {
                    LOG.ErrorFormat("Selection rectangle (Offset:{0},Line:{1},Length:{2}) {3}", rect.Start.Offset, rect.Start.Line, rect.Length, e);
                    dumpScript = true;
                }
                if (recommendation.Sql.HasFocusSelections)
                {
                    foreach (SelectionRectangle erect in recommendation.Sql.FocusSelections)
                    {
                        try
                        {
                            SetErrorRectangle(erect.Start, erect.Length, Color.Red);
                            #if DEBUG
                            LOG.ErrorFormat("Error rectangle (Offset:{0},Line:{1},Length:{2})", erect.Start.Offset, erect.Start.Line, erect.Length);
                            dumpScript = true;
                            #endif
                        }
                        catch (Exception e)
                        {
                            LOG.ErrorFormat("Error setting error rectangle (Offset:{0},Line:{1},Length:{2}) {3}", erect.Start.Offset, erect.Start.Line, erect.Length, e);
                            dumpScript = true;
                        }
                    }
                }
                if (dumpScript)
                    LOG.Info("Script: ", Script);
            }
        }

        private void LoadEditor(Recommendation recommendation)
        {
            if (recommendation.IsScriptGeneratorProvider)
            {
                IScriptGenerator generator = ((IScriptGeneratorProvider)recommendation).GetScriptGenerator();
                //ServerConfiguration config = CommonSettings.Default.GetActiveServer();
                //if (config != null && generator != null)
                //{
                    try
                    {
                        if (recommendation is Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.IScriptGeneratorProvider)
                        {
                            Idera.SQLdm.Common.Services.IManagementService managementService =
                       Idera.SQLdm.DesktopClient.Helpers.ManagementServiceHelper.GetDefaultService(
                           Idera.SQLdm.DesktopClient.Properties.Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
                            string tsql = managementService.GetPrescriptiveOptimizeScript(instanceID, recommendation);
                            Script = tsql;
                            //Script.AppendLine("--  " + r.FindingText.Replace("\r", "").Replace("\n", ""));
                            //Script.AppendLine(tsql);
                            //if (workerLoad.CancellationPending) return;
                        }
                        else
                        {
                            ApplicationMessageBox.ShowInfo(this, "'" + recommendation.FindingText + "' does not support optimize query");
                        }
                    }
                    catch (Exception ex)
                    {
                        for (Exception excp = ex; excp != null; excp = excp.InnerException)
                        {
                            Script = excp.Message;
                            //Script.Append("--    ");
                            //Script.AppendLine(excp.Message);
                        }
                    }
                    syntaxEditor1.Text = Script.ToString();
                //} 
            }
        }

        public void ShowViewer()
        {
            Visible = true;
            WindowState = restoreWindowState;
            BringToFront();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void SqlViewerDialog_Shown(object sender, EventArgs e)
        {
            ShowIndicators();
        }

        private void ShowIndicators()
        {
            if (syntaxEditor1.Document.SpanIndicatorLayers.Contains("problems"))
                GotoNextSpanIndicator("problems");
            else
                if (syntaxEditor1.Document.SpanIndicatorLayers.Contains("selection"))
                    GotoNextSpanIndicator("selection", true);
                else
                    UpdateToolbar();
        }

        private void syntaxEditor1_SelectionChanged(object sender, SelectionEventArgs e)
        {
            if (cursorPosPanel != null)
            {
                cursorPosPanel.Text = "Ln " + e.DisplayCaretDocumentPosition.Line +
                    "  Col " + e.DisplayCaretCharacterColumn;
            }
        }

        private void lvRecommendations_ItemSelectionChanged(object sender, ItemSelectionChangedEventArgs e)
        {
            if (lvRecommendations.SelectedItems.Count > 0)
            {
                LoadRecommendationIntoEditor(lvRecommendations.SelectedItems[0].Tag as IRecommendation);
                ShowIndicators();
            }
        }

        private void copyButton_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(syntaxEditor1.Document.Text);
            ApplicationMessageBox.ShowInfo(FindForm(), "Editor text copied to the clipboard");
        }

        private void SqlViewerDialog_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            //HelpTopics.ShowHelpTopic(_helpTopic);
        }
    }
}
