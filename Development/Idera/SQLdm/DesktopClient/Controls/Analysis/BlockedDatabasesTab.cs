using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Wintellect.PowerCollections;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using UltraColumnHeader = Infragistics.Win.UltraWinGrid.ColumnHeader;
using TracerX;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.DesktopClient.Properties;
using Idera.SQLdm.DesktopClient.Presenters.GridEntries;
using Idera.SQLdm.DesktopClient.Dialogs.Analysis;
using Infragistics.Windows.Themes;

namespace Idera.SQLdm.DesktopClient.Controls.Analysis
{
    public partial class BlockedDatabasesTab : UserControl
    {
        private static readonly Logger Log = Logger.GetLogger("BlockedDatabasesTab");

        private readonly DataTable _blockedRows;
        private readonly HideFocusRectangleDrawFilter _hideFocusRectangleDrawFilter = new HideFocusRectangleDrawFilter();

        private bool _modified = false;

        private bool _refreshNeeded = false;
        private bool _deferedRefresh = false;
        private int sqlServerID = 0;
        //private readonly HashSet<string> _blockedDatabases = new HashSet<string>();

        public bool Modified { get { return (_modified); } }
        public event EventHandler SettingsChanged;

        private List<int> blockedDatabaseIDList;

        public List<int> BlockedDatabaseIDList
        {
            get { return blockedDatabaseIDList; }
            set { blockedDatabaseIDList = value; }
        }

        private IDictionary<int, string> blockedDatabaseIDDictionary;
        public IDictionary<int, string> BlockedDatabaseIDDictionary
        {
            get { return blockedDatabaseIDDictionary; }
            set { blockedDatabaseIDDictionary = value; }
        }
        public BlockedDatabasesTab(int instanceID, List<int> blockedDBlist)
        {
            InitializeComponent();
            SetGridTheme();
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
            if (DesignMode) return;

            this.sqlServerID = instanceID;            
            blockedDatabaseIDDictionary = new Dictionary<int, string>();
            _blockedRows = new DataTable("Blocked Databases");
            _blockedRows.Columns.Add("Blocked", typeof(bool));
            _blockedRows.Columns.Add("DatabaseID", typeof(int));
            _blockedRows.Columns.Add("Database", typeof(string));
            _loadingProgressControl.InnerCircleRadius = 6;
            _loadingProgressControl.OuterCircleRadius = 10;

            _blockedGrid.DrawFilter = _hideFocusRectangleDrawFilter;
            _blockedGrid.DataSource = _blockedRows;
            _blockedGrid.DisplayLayout.Bands[0].Columns["DatabaseID"].Hidden = true;

            blockedDatabaseIDList = blockedDBlist == null ? new List<int>() : blockedDBlist;
            UpdateBlocked();

        }

        private void UpdateLists()
        {
            _modified = false;
            blockedDatabaseIDList.Clear();
            _blockedRows.Clear();
            RefreshDatabases();
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

        private void RefreshDatabases()
        {
            ServerSettingsDialog.Databases = new List<DatabaseInformation>();
            if (!this.Visible) { _deferedRefresh = true; return; }
            _deferedRefresh = false;
            if (_refreshDatabasesWorker.IsBusy) {_refreshNeeded = true; return;}

        }

        private void OnSettingsChanged(EventArgs e)
        {
            _modified = true;
            if (null != SettingsChanged) SettingsChanged(this, e);
        }

        private List<int> GetBlockedDatabaseList()
        {
            List<int> blocked = new List<int>();
            foreach (DataRow row in _blockedRows.Rows)
            {
                if ((bool)row["Blocked"]) blocked.Add( Convert.ToInt32( row["DatabaseID"]));
            }
            return (blocked);
        }

        public void SaveChanges()
        {
            if (!_modified) return;

            List<int> blocked = GetBlockedDatabaseList();

            if (blocked.Count == _blockedRows.Rows.Count)
            {
                if (ApplicationMessageBox.ShowQuestion(ParentForm, "Are you sure you want to block all databases from analysis?") == DialogResult.No)
                {
                    Log.Info("Saving advance analysis configuration canceled by user due to all databases being blocked from the analysis.");
                    throw new ApplicationException("Blocked databases were not saved due to all databases being blocked from the analysis.");
                }
                else
                {
                    Log.Info("All databases are blocked from the analysis by user.");
                }
            }

            //if (null != _sc)
            //{
            //    _sc.BlockedDatabases = blocked;
            //}

            blockedDatabaseIDList.Clear();
            foreach (int db in blocked)
            {
                if (!blockedDatabaseIDList.Contains(db))
                {
                    blockedDatabaseIDList.Add(db);
                }
            }

            _modified = false;
        }

        private void _refreshDatabasesWorker_DoWork(object sender, DoWorkEventArgs e)
        {
        }

        private void _refreshDatabasesWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (IsDisposed) return;
            if (_refreshNeeded) 
            { 
                _refreshNeeded = false;
                return;
            }
            _loadingPanel.Visible = _loadingProgressControl.Active = false;
            Exception ex = e.Result as Exception;
            if (null == ex) ex = e.Error;
            if (null != ex)
            {
                ApplicationMessageBox.ShowError(this.FindForm(), ex);
                return;
            }
            Log.Info("Database refresh complete.");
            UpdateBlocked();
        }

        private void UpdateBlocked()
        {

            _blockedRows.Clear();
            if (null != ServerSettingsDialog.Databases)
            {
                foreach (DatabaseInformation db in ServerSettingsDialog.Databases)
                {
                    DataRow row = _blockedRows.NewRow();
                    row["DatabaseID"] = db.DatabaseID;
                    row["Database"] = db.DatabaseName;
                    row["Blocked"] = (blockedDatabaseIDList != null && blockedDatabaseIDList.Count >0) ? blockedDatabaseIDList.Contains(db.DatabaseID) : false;
                    _blockedRows.Rows.Add(row);
                }
            }
        }

        private void BlockedDatabasesTab_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible && _deferedRefresh) RefreshDatabases();
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
                                blockedDatabaseIDList.Add(Convert.ToInt32(selectedRow.Cells[1].Value));
                            else
                                blockedDatabaseIDList.Remove(Convert.ToInt32( selectedRow.Cells[1].Value));

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
            //    ApplicationMessageBox.ShowError(this.FindForm(), "Database blocking is not allowed for trial.");
            //    return (false);
            //}
            return (true);
        }
    }
}
