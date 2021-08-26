using System;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Idera.SQLdm.Common.UI.Controls;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;
using Idera.SQLdm.Common.Services;
using BBS.TracerX;
using Idera.SQLdm.Common.Recommendations;
using Infragistics.Windows.Themes;
using Idera.SQLdm.DesktopClient.Controls;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Analysis
{
    public partial class AnalysisResultsList : UserControl, Infragistics.Win.IUIElementDrawFilter
    {
        private Logger Log;
        private int InstanceId;
        private readonly DataTable _AnalysisListTable;
        private Analysis.Recommendations recommendationAnalysis;
        public event EventHandler<AnalysisResultSelectedEventArgs> AnalysisResultSelected;

        public AnalysisResultsList()
        {
            InitializeComponent();
        }

        public AnalysisResultsList(int instanceId)
        {
            InstanceId = instanceId;
            InitializeComponent();
            _historyGrid.DrawFilter = new HideFocusRectangleDrawFilter();

            SetGridTheme();
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);

            try
            {
                AnalysisListCollection allResults = new AnalysisListCollection();
                IManagementService managementService = ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                if (managementService != null)
                {
                    allResults = managementService.GetAnalysisListing(instanceId);

                    //_historyGrid.DataSource = allResults;

                    _AnalysisListTable = new DataTable();
                    _AnalysisListTable.Clear();
                    _AnalysisListTable.Columns.Add("SQLServerID", typeof(int));
                    //_AnalysisListTable.Columns.Add("AnalysisID", typeof(int));
                    _AnalysisListTable.Columns.Add("AnalysisStartTime", typeof(DateTime));
                    //_AnalysisListTable.Columns.Add("AnalysisCompleteTime", typeof(DateTime));
                    _AnalysisListTable.Columns.Add("AnalysisDuration", typeof(String));
                    _AnalysisListTable.Columns.Add("TotalRecommendationCount", typeof(int));
                    _AnalysisListTable.Columns.Add("Type", typeof(string));
                    _AnalysisListTable.Columns.Add("ComputedRankFactor", typeof(float));
                    _AnalysisListTable.Columns.Add("Priority", typeof(Bitmap));

                    foreach (AnalysisList listAnalysis in allResults.AnalysisListColl)
                    {
                        DataRow row = _AnalysisListTable.NewRow();
                        row["SQLServerID"] = listAnalysis.SQLServerID;
                        //row["AnalysisID"] = listAnalysis.AnalysisID;
                        row["AnalysisStartTime"] = listAnalysis.AnalysisStartTime;
                        //row["AnalysisCompleteTime"] = listAnalysis.AnalysisCompleteTime;
                        row["AnalysisDuration"] = listAnalysis.AnalysisDuration;
                        row["TotalRecommendationCount"] = listAnalysis.TotalRecommendationCount;
                        row["Type"] = listAnalysis.Type;
                        row["ComputedRankFactor"] = listAnalysis.ComputedRankFactor;
                        _AnalysisListTable.Rows.Add(row);
                    }

                    _historyGrid.DataSource = _AnalysisListTable;
                    _historyGrid.DataBind();
                    _historyGrid.Visible = true;
                    _historyGrid.DisplayLayout.Bands[0].Columns["SQLServerID"].Hidden = true;
                    _historyGrid.DisplayLayout.Bands[0].Columns["AnalysisStartTime"].Header.Caption = "Started";
                    _historyGrid.DisplayLayout.Bands[0].Columns["AnalysisDuration"].Header.Caption = "Duration";
                    _historyGrid.DisplayLayout.Bands[0].Columns["Type"].Header.Caption = "TaskType";
                    _historyGrid.DisplayLayout.Bands[0].Columns["TotalRecommendationCount"].Header.Caption = "Recommendations";
                    _historyGrid.DisplayLayout.Bands[0].Columns["ComputedRankFactor"].Header.Caption = "Priority Value";
                    _historyGrid.DisplayLayout.Bands[0].Columns["Priority"].Header.Caption = "Priority";
                    _historyGrid.DisplayLayout.Bands[0].Columns["AnalysisStartTime"].SortIndicator = SortIndicator.Descending;
                    _historyGrid.DisplayLayout.Rows.EnsureSortedAndFiltered();
                    _historyGrid.BringToFront();
                }
            }
            catch (Exception ex)
            {
                //ApplicationMessageBox.ShowError(this, "Unable to display recommendtionns on screen.", ex);
                Log.Error(ex.Message + ": Not able to show records. Error in GetRealTimeAnalysisRecords for run analysis.");
                throw new Exception("Unable to display recommendtionns on screen.", ex);
            }
        }

        public AnalysisResultsList(int instanceId, AnalysisListCollection allResults)
        {
            this.InstanceId = instanceId;
            InitializeComponent();
            _historyGrid.DrawFilter = new HideFocusRectangleDrawFilter();

            try
            {
                if (allResults != null)
                {
                    _AnalysisListTable = new DataTable();
                    _AnalysisListTable.Clear();
                    _AnalysisListTable.Columns.Add("SQLServerID", typeof(int));
                    _AnalysisListTable.Columns.Add("AnalysisID", typeof(int));
                    _AnalysisListTable.Columns.Add("AnalysisStartTime", typeof(DateTime));
                    //_AnalysisListTable.Columns.Add("AnalysisCompleteTime", typeof(DateTime));
                    _AnalysisListTable.Columns.Add("AnalysisDuration", typeof(String));
                    _AnalysisListTable.Columns.Add("TotalRecommendationCount", typeof(int));
                    _AnalysisListTable.Columns.Add("Type", typeof(string));
                    _AnalysisListTable.Columns.Add("ComputedRankFactor", typeof(float));
                    _AnalysisListTable.Columns.Add("Priority", typeof(Bitmap));

                    foreach (AnalysisList listAnalysis in allResults.AnalysisListColl)
                    {
                        DataRow row = _AnalysisListTable.NewRow();
                        row["SQLServerID"] = listAnalysis.SQLServerID;
                        row["AnalysisID"] = listAnalysis.AnalysisID;
                        row["AnalysisStartTime"] = listAnalysis.AnalysisStartTime;
                        //row["AnalysisCompleteTime"] = listAnalysis.AnalysisCompleteTime;
                        row["AnalysisDuration"] = listAnalysis.AnalysisDuration;
                        row["TotalRecommendationCount"] = listAnalysis.TotalRecommendationCount;
                        row["Type"] = listAnalysis.Type;
                        row["ComputedRankFactor"] = listAnalysis.ComputedRankFactor;

                        // 
                        // priority
                        // 
                        PriorityBar priority = new PriorityBar();
                        priority.Anchor = System.Windows.Forms.AnchorStyles.Right;
                        priority.Location = new System.Drawing.Point(6, 7);
                        priority.Name = "priority";
                        priority.Size = AutoScaleSizeHelper.isScalingRequired ? new System.Drawing.Size(120, 12) : new Size(80, 8);
                        priority.TabIndex = 3;
                        priority.Value = 20F;
                        priority.Value = listAnalysis.ComputedRankFactor;
                        Bitmap IMG = AutoScaleSizeHelper.isScalingRequired ? new Bitmap(120, 12) : new Bitmap(80, 8);
                        Rectangle rectDraw = AutoScaleSizeHelper.isScalingRequired ? new Rectangle(0, 0, 120, 12) : new Rectangle(0, 0, 80, 8);
                        priority.DrawToBitmap(IMG, rectDraw);
                        row["Priority"] = IMG;
                        row["ComputedRankFactor"] = listAnalysis.ComputedRankFactor;
                        _AnalysisListTable.Rows.Add(row);
                    }

                    _historyGrid.DataSource = _AnalysisListTable;
                    _historyGrid.DataBind();
                    _historyGrid.Visible = true;
                    _historyGrid.DisplayLayout.Bands[0].Columns["SQLServerID"].Hidden = true;
                    _historyGrid.DisplayLayout.Bands[0].Columns["AnalysisID"].Hidden = true;
                    _historyGrid.DisplayLayout.Bands[0].Columns["ComputedRankFactor"].Hidden = true;

                    _historyGrid.DisplayLayout.Bands[0].Columns["AnalysisStartTime"].Header.Caption = "Started";
                    _historyGrid.DisplayLayout.Bands[0].Columns["AnalysisDuration"].Header.Caption = "Duration";
                    _historyGrid.DisplayLayout.Bands[0].Columns["Type"].Header.Caption = "TaskType";
                    _historyGrid.DisplayLayout.Bands[0].Columns["TotalRecommendationCount"].Header.Caption = "Recommendations";
                    _historyGrid.DisplayLayout.Bands[0].Columns["ComputedRankFactor"].Header.Caption = "Priority Value";
                    _historyGrid.DisplayLayout.Bands[0].Columns["Priority"].Header.Caption = "Priority";

                    _historyGrid.DisplayLayout.Bands[0].Columns["AnalysisStartTime"].SortIndicator = SortIndicator.Descending;
                    _historyGrid.DisplayLayout.Rows.EnsureSortedAndFiltered();
                    _historyGrid.BringToFront();
                }
            }
            catch (Exception ex)
            {
                //ApplicationMessageBox.ShowError(this, "Unable to display recommendtionns on screen.", ex);
                Log.Error(ex.Message + ": Not able to show records. Error in GetRealTimeAnalysisRecords for run analysis.");
                throw new Exception("Unable to display recommendtionns on screen.", ex);
            }
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        }

        public bool DrawElement(Infragistics.Win.DrawPhase drawPhase, ref Infragistics.Win.UIElementDrawParams drawParams)
        {
            if (drawPhase == Infragistics.Win.DrawPhase.BeforeDrawElement)
            {
                var imageElement = drawParams.Element as ImageUIElement;
                if (imageElement != null && imageElement.Parent is EmbeddableImageRendererUIElement)
                {
                    Infragistics.Win.UltraWinGrid.CellUIElement item =
                        (Infragistics.Win.UltraWinGrid.CellUIElement)imageElement.GetAncestor(typeof(Infragistics.Win.UltraWinGrid.CellUIElement));

                    if (item != null && item.Column.Key == "Priority")
                    {
                        var value = item.Cell.Value;
                        if (null != value)
                        {
                            Rectangle area = imageElement.RectInsideBorders;
                            int width = area.Width - 4;
                            width -= (width % 5);
                            int hinset = (area.Width - width) / 2;
                            int vinset = (area.Height - 8) / 2 + 1;
                            Rectangle rect = new Rectangle(area.X + hinset, area.Y + vinset, width, 8);
                            Rectangle rectNew = rect;
                            if (AutoScaleSizeHelper.isScalingRequired)
                                rectNew = new Rectangle(rect.X, rect.Y, rect.Width + 40, rect.Height + 4);
                            PriorityBar.Draw(drawParams.Graphics, rectNew, (float)value);
                        }
                    }
                }
            }

            return false;
        }

        public Infragistics.Win.DrawPhase GetPhasesToFilter(ref Infragistics.Win.UIElementDrawParams drawParams)
        {
            if (drawParams.Element is Infragistics.Win.ImageUIElement) return Infragistics.Win.DrawPhase.BeforeDrawElement;

            return Infragistics.Win.DrawPhase.None;
        }

        #region Grid Events
        private void _historyGrid_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            UIElement selectedElement = _historyGrid.DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));
            if (selectedElement == null) return;
            UltraGridRow selectedRow = selectedElement.SelectableItem is UltraGridCell ? ((UltraGridCell)selectedElement.SelectableItem).Row : selectedElement.SelectableItem as UltraGridRow;
            if (selectedRow != null)
            {
                AnalysisResultSelectedEventArgs arse = new AnalysisResultSelectedEventArgs(InstanceId, (DateTime)((System.Data.DataRowView)selectedRow.ListObject).Row.ItemArray[2]);
                //AnalysisResultSelected(this, arse);
                recommendationAnalysis = new Analysis.Recommendations(this.InstanceId, (DateTime)((System.Data.DataRowView)selectedRow.ListObject).Row.ItemArray[2]);
                this.Visible = false;
                recommendationAnalysis.Visible = true;
                recommendationAnalysis.BringToFront();
                recommendationAnalysis.Show();
            }
        }

        private void _historyGrid_KeyPress(object sender, KeyPressEventArgs e)
        {
            AnalysisResultSelectedEventArgs arse = new AnalysisResultSelectedEventArgs(InstanceId, GetFirstSelectedEntry());
            AnalysisResultSelected(this, arse);
        }

        private void ShowSelectedResults(int id, DateTime? currentHistoricalSnapshotDateTime)
        {

        }

        private DateTime GetFirstSelectedEntry()
        {
            if (null == _historyGrid.Selected) return (DateTime.Now);
            if (null == _historyGrid.Selected.Rows) return (DateTime.Now);
            if (_historyGrid.Selected.Rows.Count <= 0) return (DateTime.Now);
            return ((DateTime)((System.Data.DataRowView)_historyGrid.Selected.Rows[0].ListObject).Row.ItemArray[2]);
        }
        #endregion
        void OnCurrentThemeChanged(object sender, EventArgs e)
        {
            SetGridTheme();
        }

        private void SetGridTheme()
        {
            // Update UltraGrid Theme
            var themeManager = new GridThemeManager();
            themeManager.updateGridTheme(this._historyGrid);
        }
    }

    public sealed class AnalysisResultSelectedEventArgs : EventArgs
    {
        public readonly int InstanceId;
        public readonly DateTime SnapshotDateTime;

        public AnalysisResultSelectedEventArgs(int instanceId, DateTime snapshotDateTime)
        {
            InstanceId = instanceId;
            SnapshotDateTime = snapshotDateTime;
        }
    }
}