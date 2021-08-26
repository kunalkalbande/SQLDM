using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using UltraColumnHeader = Infragistics.Win.UltraWinGrid.ColumnHeader;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Resources;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.DesktopClient.Properties;
using Infragistics.Windows.Themes;

namespace Idera.SQLdm.DesktopClient.Controls.Analysis
{
    public partial class BlockedRecommendationsTab : UserControl
    {
        private bool _modified = false;
        private readonly DataTable _blockedRows;
        private readonly HideFocusRectangleDrawFilter _hideFocusRectangleDrawFilter = new HideFocusRectangleDrawFilter();

        private List<string> blockedRecommendationID;
        public bool Modified { get { return (_modified); } }
        public event EventHandler SettingsChanged;

        public List<string> BlockedRecommendationID
        {
            get { return blockedRecommendationID; }
            set { blockedRecommendationID = value; }
        }
        public BlockedRecommendationsTab(List<string> blockedRecommendations)
        {
            InitializeComponent();
            SetGridTheme();
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);

            if (DesignMode) return;
            blockedRecommendationID = new List<string>();
            _blockedRows = new DataTable("Blocked Recommendations");
            _blockedRows.Columns.Add("Blocked", typeof(bool));
            _blockedRows.Columns.Add("ID", typeof(string));
            _blockedRows.Columns.Add("Category", typeof(string));
            _blockedRows.Columns.Add("Description", typeof(string));
            GetMasterRecommendations();
            Initialize();

            _blockedGrid.DrawFilter = _hideFocusRectangleDrawFilter;
            _blockedGrid.DataSource = _blockedRows;
            _blockedGrid.DisplayLayout.Bands[0].Columns["ID"].SortComparer = new RecommendationIdSortComparer();
            if (blockedRecommendations != null && blockedRecommendations.Count != 0)
            {
                blockedRecommendationID = blockedRecommendations;
                InitializeForm();
            }

        }

        void OnCurrentThemeChanged(object sender, EventArgs e)
        {
            SetGridTheme();
        }

        private void SetGridTheme()
        {
            // Update UltraGrid Theme
            var themeManager = new GridThemeManager();
            themeManager.updateGridTheme(this._blockedGrid);
        }

        //To get master recommendation data from database through Management Service
        private void GetMasterRecommendations()
        {
            try
            {
                IManagementService managementService =
                                        ManagementServiceHelper.GetDefaultService(
                                            Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
                if (managementService != null)
                {
                    var result = managementService.GetMasterRecommendations();
                    MasterRecommendations.MasterRecommendationsInformation = result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Not able to get master recommendation list from database. "+ ex.Message);
            }
        }
        private void Initialize()
        {
            //var values = Enum.GetValues(typeof(RecommendationType));
            foreach (MasterRecommendation master in MasterRecommendations.MasterRecommendationsInformation)
            {
                var recommendationId = master.RecommendationID;

                if (string.IsNullOrEmpty(recommendationId)) continue;

                var description = MasterRecommendations.GetDescription(recommendationId);

                if (!string.IsNullOrEmpty(description))
                {
                    var category = MasterRecommendations.GetCategory(recommendationId);
                    DataRow row = _blockedRows.NewRow();
                    row["Blocked"] = false;
                    row["ID"] = recommendationId;
                    row["Category"] = String.IsNullOrEmpty(category) ? String.Empty : category;
                    row["Description"] = description;
                    _blockedRows.Rows.Add(row);
                }
            }

            UpdateLists();
        }

        private void InitializeForm()
        {
            foreach (DataRow row in _blockedRows.Rows)
            {
                string id = row["ID"].ToString();
                row["Blocked"] = blockedRecommendationID.Contains(id);
            }
        }

        private void UpdateLists()
        {
            _blockedGrid.SuspendLayout();

            try
            {
                HashSet<string> _blockedRecommendations = new HashSet<string>();


                //if (_sc != null)
                //{
                //    foreach (string id in _sc.BlockedRecommendations)
                //    {
                //        _blockedRecommendations.Add(id);
                //    }

                //    foreach (DataRow row in _blockedRows.Rows)
                //    {
                //        string id = row["ID"].ToString();
                //        row["Blocked"] = _blockedRecommendations.Contains(id);
                //    }
                //}
            }
            finally
            {
                _blockedGrid.ResumeLayout();
            }

            if (_blockedGrid.DisplayLayout.ActiveRow != null) _blockedGrid.DisplayLayout.ActiveRow.Refresh(RefreshRow.RefreshDisplay);

            _modified = false;
        }

        private void OnSettingsChanged(EventArgs e)
        {
            _modified = true;
            if (null != SettingsChanged) SettingsChanged(this, e);
        }

        private void _blockedGrid_MouseClick(object sender, MouseEventArgs e)
        {
            UIElement selectedElement = _blockedGrid.DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));
            if (selectedElement == null)
                return;

            if (selectedElement is EditorWithTextDisplayTextUIElement)
                selectedElement = selectedElement.Parent;
            else
                if (!(selectedElement is ImageUIElement))
                {
                    selectedElement = selectedElement.Parent;
                    if (!(selectedElement is ImageUIElement || selectedElement is HeaderUIElement))
                        return;
                }
            // logic to handle toggling a checkbox in a non-editable (no cell selection) column
            object contextObject = selectedElement.GetContext();

            UltraColumnHeader columnHeader = contextObject as UltraColumnHeader;
            if (columnHeader != null)
            {
                //_lastBlockedColumnHeaderElement = selectedElement;
                //if (columnHeader.Column.Key.Equals("Blocked"))
                //{
                //    if ((selectedElement is ImageUIElement) && IsBlockAllowed())
                //    {
                //        ToggleSelection();
                //        UltraGrid grid = (UltraGrid)sender;
                //        if (grid.Selected.Rows.Count > 0)
                //            grid.Selected.Rows[0].Cells[columnHeader.Column].Refresh();

                //        OnSettingsChanged(EventArgs.Empty);
                //    }
                //}
            }
            else if (contextObject is Infragistics.Win.UltraWinGrid.UltraGridColumn)
            {
                bool updateControlsNeeded = false;

                string column = ((UltraGridColumn)contextObject).Key;
                if (!column.Equals("Blocked"))
                    return;

                UltraGridRow selectedRow = selectedElement.SelectableItem is UltraGridCell ? ((UltraGridCell)selectedElement.SelectableItem).Row : selectedElement.SelectableItem as UltraGridRow;
                if ((selectedRow != null) && IsBlockAllowed())
                {
                    DataRowView drv = selectedRow.ListObject as DataRowView;
                    if (drv != null)
                    {
                        object cellValue = drv[column];
                        if (cellValue is bool)
                        {
                            updateControlsNeeded = (bool)cellValue;
                            drv[column] = !updateControlsNeeded;
                            selectedRow.Refresh(RefreshRow.RefreshDisplay);
                            if (!(bool)cellValue)
                            {
                                //blockedRecommendationID.Add(selectedRow.Cells[1].Value.ToString());

                                if (!blockedRecommendationID.Contains(selectedRow.Cells[1].Value.ToString()))
                                {
                                    blockedRecommendationID.Add(selectedRow.Cells[1].Value.ToString());
                                }
                            }
                            else
                                blockedRecommendationID.Remove(selectedRow.Cells[1].Value.ToString());

                        }
                    }

                    OnSettingsChanged(EventArgs.Empty);
                }
            }
        }

        private bool IsBlockAllowed()
        {
            //if (ApplicationController.Default.IsLimitedTrialMode)
            //{
            //    ApplicationMessageBox.ShowError(this.FindForm(), "Recommendation blocking is not allowed for trial.");
            //    return (false);
            //}
            return (true);
        }

        public List<string> GetBlockedRecommendations()
        {
            List<string> blocked = new List<string>();
            foreach (DataRow row in _blockedRows.Rows)
            {
                if ((bool)row["Blocked"])
                    blocked.Add(row["ID"].ToString());
            }
            return blocked;
        }

        public void SaveChanges()
        {
            if (!_modified) return;

            List<string> blocked = GetBlockedRecommendations();

            if (blocked.Count == _blockedRows.Rows.Count) throw new ApplicationException("At least one recommendation must be allowed for analysis.");

            //if (_sc != null)
            //{
            //    _sc.BlockedRecommendations = blocked;
            //}

            _modified = false;
        }

    }
    internal class RecommendationIdSortComparer : IComparer
    {
        public int Compare(object xcell, object ycell)
        {
            string x = ((UltraGridCell)xcell).Value.ToString();
            string y = ((UltraGridCell)ycell).Value.ToString();

            string xCategoryId, yCategoryId;
            int xRecommendationIndex, yRecommendationIndex;

            SplitRecommendationId(x, out xCategoryId, out xRecommendationIndex);
            SplitRecommendationId(y, out yCategoryId, out yRecommendationIndex);

            int result = xCategoryId.CompareTo(yCategoryId);
            if (result == 0)
            {
                result = xRecommendationIndex.CompareTo(yRecommendationIndex);
                if (result == 0)
                    return ((string)x).CompareTo(y);
            }
            return result;
        }

        private static void SplitRecommendationId(string recommendationId, out string categoryId, out int recommendationIndex)
        {
            var firstDigitIndex = recommendationId.IndexOf(recommendationId.ToCharArray().First(Char.IsDigit));
            categoryId = recommendationId.Substring(4, firstDigitIndex - 4);

            var right = recommendationId.Substring(firstDigitIndex);
            var versionPrefixIndex = right.IndexOf('v');

            if (versionPrefixIndex > 0)
            {
                right = right.Substring(0, versionPrefixIndex);
            }

            recommendationIndex = int.Parse(right);
        }
    }
}
