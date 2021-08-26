using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.UI.Controls;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Idera.SQLdm.Common.Recommendations;
using Idera.Newsfeed.Plugins.UI.Dialogs;
using Infragistics.Windows.Themes;
using Idera.SQLdm.DesktopClient.Controls;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Analysis
{
    internal partial class DefaultScreenAnalysisTab : ServerBaseView
    {
        #region "PRIVATE VARIABLE"
        private string _headingFormat = string.Empty;
        private int InstanceId;
        private readonly DataTable _AnalysisListTable;
        private Analysis.Recommendations recommendationAnalysis;
        private IServerView activeView;
        ViewContainer viewHost;
        ServerViewViewModel serverViewViewModel = null;
        #endregion

        public bool IsAnalyzeEnabled { get; set; }
        #region "CONSTRUCTORS"
        public DefaultScreenAnalysisTab()
        {
            InitializeComponent();
            SetGridTheme();
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
        }

        public DefaultScreenAnalysisTab(int instanceId, ViewContainer vHost, bool isDashboard, ServerViewViewModel svm = null)
        {
            serverViewViewModel = svm;
            this.InstanceId = instanceId;
            viewHost = vHost;
            InitializeComponent();
            SetGridTheme();
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
            IsAnalyzeEnabled = false;

            _headingFormat = lblHeading.Text;
            lblHeading.Text = string.Format(_headingFormat, ApplicationModel.Default.AllInstances[instanceId].InstanceName);
            _historyGrid.DrawFilter = new HideFocusRectangleDrawFilter();

            AnalysisListCollection allResults = new AnalysisListCollection();
            IManagementService managementService = ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            if (managementService != null)
            {
                allResults = managementService.GetAnalysisListing(this.InstanceId);
            }

            try
            {
                if (allResults != null)
                {
                    _AnalysisListTable = new DataTable();
                    _AnalysisListTable.Clear();
                    _AnalysisListTable.Columns.Add("SQLServerID", typeof(int));
                    _AnalysisListTable.Columns.Add("AnalysisID", typeof(int));
                    _AnalysisListTable.Columns.Add("AnalysisStartTime", typeof(DateTime));
                    _AnalysisListTable.Columns.Add("AnalysisCompleteTime", typeof(DateTime));
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
                        row["AnalysisCompleteTime"] = listAnalysis.AnalysisCompleteTime;
                        row["AnalysisDuration"] = listAnalysis.AnalysisDuration;
                        row["TotalRecommendationCount"] = listAnalysis.TotalRecommendationCount;
                        //row["Type"] = listAnalysis.Type;
                        row["Type"] = getDefaultTaskTypeValue(listAnalysis.Type);
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
                    _historyGrid.DisplayLayout.Bands[0].Columns["AnalysisCompleteTime"].Hidden = true;

                    _historyGrid.DisplayLayout.Bands[0].Columns["AnalysisStartTime"].Header.Caption = "Started";
                    _historyGrid.DisplayLayout.Bands[0].Columns["AnalysisStartTime"].Format = "f";
                    _historyGrid.DisplayLayout.Bands[0].Columns["AnalysisStartTime"].Width = 150;

                    _historyGrid.DisplayLayout.Bands[0].Columns["AnalysisDuration"].Header.Caption = "Duration";
                    _historyGrid.DisplayLayout.Bands[0].Columns["Type"].Header.Caption = "Type";
                    _historyGrid.DisplayLayout.Bands[0].Columns["TotalRecommendationCount"].Header.Caption = "Recommendations";
                    _historyGrid.DisplayLayout.Bands[0].Columns["ComputedRankFactor"].Header.Caption = "Priority Value";
                    _historyGrid.DisplayLayout.Bands[0].Columns["Priority"].Header.Caption = "Priority";

                    _historyGrid.DisplayLayout.Bands[0].Columns["AnalysisStartTime"].SortIndicator = SortIndicator.Descending;
                    _historyGrid.DisplayLayout.Rows.EnsureSortedAndFiltered();
                    _historyGrid.BringToFront();
                    this.horzSplitContainer.Panel2.Controls.Add(_historyGrid);

                    if (isDashboard)
                    {
                        this.Size = new System.Drawing.Size(284, 261);
                        this._historyGrid.Size = new System.Drawing.Size(284, 261);
                    }
                    this.welcomeMessageLabel.Visible = false;
                }
                else
                {
                    if (isDashboard)
                    {
                        this.welcomeMessageLabel.AutoSize = true;
                    }
                    this.horzSplitContainer.Visible = false;
                    this.welcomeMessageLabel.Visible = true;
                }

                if (allResults != null && allResults.AnalysisListColl.Count > 0)
                    IsAnalyzeEnabled = true;
            }
            catch (Exception ex)
            {
                //ApplicationMessageBox.ShowError(this, "Unable to display recommendtionns on screen.", ex);
                Log.Error(ex.Message + ": Not able to show records. Error in GetRealTimeAnalysisRecords for run analysis.");
                throw new Exception("Unable to display recommendtionns on screen.", ex);
            }
            if (AutoScaleSizeHelper.isScalingRequired)
                ScaleControlsAsPerResolution();
        }
        private void ScaleControlsAsPerResolution()
        {
            this.lblHeading.Location = new System.Drawing.Point(75, 11);
            this.horzSplitContainer.Size = new System.Drawing.Size(3000, 841);
            this.horzSplitContainer.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom;
            this.horzSplitContainer.SplitterDistance = 80;
            this.Height = this.Height + 2000;
        }
        #endregion

        #region "PUBLIC METHODS"
        public override void UpdateData(object data)
        {
            ApplicationController.Default.OnRefreshActiveViewCompleted(new RefreshActiveViewCompletedEventArgs(DateTime.Now));
        }
        public override void ShowHelp()
        {
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.AnalysisView);
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
                            Rectangle newRect = rect;
                            if (AutoScaleSizeHelper.isScalingRequired)
                                newRect = new Rectangle(rect.X, rect.Y, rect.Width + 40, rect.Height + 4);
                            PriorityBar.Draw(drawParams.Graphics, newRect, (float)value);
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
        public void display()
        {
            ShowView(this);
        }
        #endregion

        #region "PRIVATE METHODS" 
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        }

        private void ShowView(IServerView view)
        {
            ShowView(view, false);
        }

        private void ShowView(IServerView view, bool handleActiveViewFilter)
        {
            if (view != null)
            {
                viewHost.Add((IView)view);

                view.UpdateUserTokenAttributes();
                if (view is Control)
                {
                    ((Control)view).Dock = DockStyle.Fill;
                    ((Control)view).Visible = true;
                    ((Control)view).BringToFront();
                }
                activeView = view;
                viewHost.ResumeLayout();
            }
        }
        #endregion

        #region "GRID EVENTS"
        private void _historyGrid_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            UIElement selectedElement = _historyGrid.DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));
            if (selectedElement == null) return;
            UltraGridRow selectedRow = selectedElement.SelectableItem is UltraGridCell ? ((UltraGridCell)selectedElement.SelectableItem).Row : selectedElement.SelectableItem as UltraGridRow;

            if (selectedRow == null)
                return;
            // If no recommendation is found then display message
            if(Convert.ToInt32(selectedRow.Cells["TotalRecommendationCount"].Value) == 0)
            {
                ApplicationMessageBox.ShowInfo(this, "No recommendations are generated for this Analysis snapshot.", false);
                return;
            }
            if (selectedRow != null)
            {
                ApplicationController.Default.ShowServerView(InstanceId, ServerViews.Analysis, null, (DateTime)((System.Data.DataRowView)selectedRow.ListObject).Row.ItemArray[3]);
            }
            else
            {
                ApplicationController.Default.ShowServerView(InstanceId, ServerViews.Analysis);
            }
            DrillOutMethod();
        }

        private void _historyGrid_KeyPress(object sender, KeyPressEventArgs e)
        {
            ShowSelectedResults(this.InstanceId, GetFirstSelectedEntry());
        }

        private void ShowSelectedResults(int id, DateTime? currentHistoricalSnapshotDateTime)
        {
            recommendationAnalysis = new Analysis.Recommendations(id, currentHistoricalSnapshotDateTime);
            ShowView(recommendationAnalysis);
        }

        private DateTime GetFirstSelectedEntry()
        {
            if (null == _historyGrid.Selected) return (DateTime.Now);
            if (null == _historyGrid.Selected.Rows) return (DateTime.Now);
            if (_historyGrid.Selected.Rows.Count <= 0) return (DateTime.Now);
            return ((DateTime)((System.Data.DataRowView)_historyGrid.Selected.Rows[0].ListObject).Row.ItemArray[3]);
        }

        private string getDefaultTaskTypeValue(string currentValue)
        {
            string newValue = "Analysis";
            switch (currentValue)
            {
                case "Default":
                    newValue = "Analysis";
                    break;
                default:
                    newValue = "Analysis";
                    break;
            }
            return newValue;
        }
        #endregion

        public void DrillOutMethod()
        {
            if (serverViewViewModel != null)
            {
                serverViewViewModel.DrillOutButtonVisible = true;
            }
        }

        void OnCurrentThemeChanged(object sender, EventArgs e)
        {
            SetGridTheme();
        }

        private void SetGridTheme()
        {
            // Update UltraGrid Theme
            if(Settings.Default.ColorScheme == "Dark")
            {
                this.BackColor = ColorTranslator.FromHtml(DarkThemeColorConstants.UltraGridBackColor);
            }
            else
            {
                this.BackColor = Color.White;
            }
            var themeManager = new GridThemeManager();
            themeManager.updateGridTheme(this._historyGrid);
        }
    }
}