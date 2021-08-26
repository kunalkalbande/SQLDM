using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Events;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using TableAction = Idera.SQLdm.DesktopClient.Objects.TableActionObject.TableAction;
using Wintellect.PowerCollections;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    /// <summary>
    /// This dialog is used for selecting single or multiple tables for actions.
    /// </summary>
    internal partial class SelectTablesDialog : Idera.SQLdm.DesktopClient.Dialogs.BaseDialog
    {
        #region constants

        #endregion

        #region fields

        private Dictionary<Pair<string, int>, TableActionObject> tableList;
        private DataTable tablesDataTable;
        private Dictionary<Pair<string, int>, TableActionObject> selectedTables = new Dictionary<Pair<string, int>, TableActionObject>();
        private string helpLink;

        #endregion

        #region constructors

        public SelectTablesDialog(TableAction action, Dictionary<Pair<string, int>, TableActionObject> tables, AlertConfigurationItem alertConfigItem)
        {
            InitializeComponent();
            AdaptFontSize();

            this.Text = string.Concat(ApplicationHelper.GetEnumDescription(action), " - Select Tables");
            infoLabel.Text = string.Empty;

            tableList = tables;
            loadTables();

            if (action == TableAction.RebuildIndexes)
            {
                helpLink = HelpTopics.RebuildIndexes;
                infoLabel.Text = "Note: Rebuild indexes is only allowed on User tables";
                tablesGrid.DisplayLayout.Bands[0].Columns["% Fragmentation"].Hidden = false;
                if (alertConfigItem != null && alertConfigItem.Enabled)
                {
                    UpdateCellColor(alertConfigItem, tablesGrid, "% Fragmentation", 100);
                }
            } else if (action == TableAction.UpdateStatistics) {
                helpLink = HelpTopics.UpdateIndexStatistics;
            }

            selectAll.Enabled = (tableList.Count > 0);
        }

        #endregion

        #region properties

        public Dictionary<Pair<string, int>, TableActionObject> SelectedTables
        {
            get { return selectedTables; }
        }

        #endregion

        #region methods

        #endregion

        #region helpers

        private void loadTables()
        {
            tablesDataTable = new DataTable();
            tablesDataTable.Columns.Add("Selected", typeof(Boolean));
            tablesDataTable.Columns.Add("Key", typeof(Pair<string, int>));
            tablesDataTable.Columns.Add("TableId", typeof(int));
            tablesDataTable.Columns.Add("TableName", typeof(string));
            tablesDataTable.Columns.Add("% Fragmentation", typeof(decimal));
            tablesDataTable.Columns.Add("TableActionObject", typeof(TableActionObject));

            tablesDataTable.BeginLoadData();
            DataRow row;
            int? selectedTableId = null;
            foreach (TableActionObject table in tableList.Values)
            {
                row = tablesDataTable.LoadDataRow(
                    new object[]
                    {
                        table.Selected,
                        table.UniqueIdentifier,
                        table.TableId,
                        table.TableName,
                        table.PercentFragmentation,
                        table
                    }, true);
                if (table.Selected)
                {
                    selectedTables.Add(table.UniqueIdentifier, table);
                    selectedTableId = table.TableId;
                    okButton.Enabled = true;
                }
            }
            tablesDataTable.EndLoadData();
            tablesGrid.SetDataBinding(tablesDataTable, string.Empty);

            if (selectedTableId.HasValue)
            {
                // scroll the last selection into view
                foreach (UltraGridRow gridRow in tablesGrid.Rows.GetAllNonGroupByRows())
                {
                    if ((int)((DataRowView)gridRow.ListObject)["TableId"] == selectedTableId)
                    {
                        tablesGrid.ActiveRowScrollRegion.ScrollRowIntoView(gridRow);
                        break;
                    }
                }
            }
        }

        private void UpdateCellColor(AlertConfigurationItem alertConfigItem, UltraGrid grid, string columnName, int adjustmentMultiplier)
        {
            foreach (UltraGridRow gridRow in grid.Rows.GetAllNonGroupByRows())
            {
                UltraGridCell cell = gridRow.Cells[columnName];
                if (cell != null)
                {
                    DataRowView dataRowView = (DataRowView)gridRow.ListObject;
                    DataRow dataRow = dataRowView.Row;
                    if (dataRow.IsNull(columnName))
                    {
                        cell.Appearance.ResetBackColor();
                    }
                    else
                    {
                        IComparable value = (IComparable)dataRow[columnName];
                        if (value != null && adjustmentMultiplier != 1)
                        {
                            double dbl = (double)Convert.ChangeType(value, typeof(double));
                            value = dbl * adjustmentMultiplier;
                        }

                        switch (alertConfigItem.GetSeverity(value))
                        {
                            case MonitoredState.Informational:
                                cell.Appearance.BackColor = Color.Blue;
                                cell.Appearance.ForeColor = Color.White;
                                break;
                            case MonitoredState.Warning:
                                cell.Appearance.BackColor = Color.Gold;
                                cell.Appearance.ResetForeColor();
                                break;
                            case MonitoredState.Critical:
                                cell.Appearance.BackColor = Color.Red;
                                cell.Appearance.ForeColor = Color.White;
                                break;
                            default:
                                cell.Appearance.ResetBackColor();
                                cell.Appearance.ResetForeColor();
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adapts the size of the font for this control in case of OS font changes.
        /// </summary>
        private void AdaptFontSize()
        {            
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }
        #endregion

        #region events

        protected override void OnHelpButtonClicked(CancelEventArgs e) {
            if (e != null) e.Cancel = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(helpLink);
        }

        protected override void OnHelpRequested(HelpEventArgs hevent) {
            if (hevent != null) hevent.Handled = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(helpLink);
        }

        private void selectAll_CheckedChanged(object sender, EventArgs e)
        {
            if (tablesDataTable.Rows.Count > 0)
            {
                if (selectAll.Checked)
                {
                    foreach (DataRow row in tablesDataTable.Rows)
                    {
                        row["Selected"] = true;
                    }
                    okButton.Enabled = true;
                }
                else if (selectAll.Focused)
                {
                    foreach (DataRow row in tablesDataTable.Rows)
                    {
                        row["Selected"] = false;
                    }
                    selectedTables.Clear();
                    okButton.Enabled = false;
                }
            }
            selectAll.Refresh();
            tablesGrid.Refresh();
        }

        private void tablesGrid_MouseDown(object sender, MouseEventArgs e)
        {
            UIElement selectedElement =
                    ((UltraGrid)sender).DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));

            if (selectedElement != null)
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (selectedElement is CheckIndicatorUIElement)
                    {
                        RowUIElement selectedRowUIElement = selectedElement.GetAncestor(typeof(RowUIElement)) as RowUIElement;

                        if (selectedRowUIElement != null && selectedRowUIElement.Row.ListObject is DataRowView)
                        {
                            DataRowView dataRow = selectedRowUIElement.Row.ListObject as DataRowView;

                            Pair<string, int> key = (Pair<string, int>)dataRow["Key"];
                            dataRow["Selected"] = !(bool)dataRow["Selected"];
                            if ((bool)dataRow["Selected"])
                            {
                                selectedTables.Add(key, (TableActionObject)dataRow["TableActionObject"]);
                                okButton.Enabled = true;
                            }
                            else
                            {
                                selectAll.Checked = false;
                                selectedTables.Remove(key);
                                okButton.Enabled = selectedTables.Count > 0;
                            }
                            selectedElement.Invalidate();
                        }
                    }
                }
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            TableActionObject tableId;
            selectedTables.Clear();
            foreach (DataRow row in tablesDataTable.Rows)
            {
                if ((bool)row["Selected"])
                {
                    Pair<string, int> key = (Pair<string, int>)row["Key"];
                    if (tableList.TryGetValue(key, out tableId))
                    {
                        selectedTables.Add(key, tableId);
                    }
                }
            }
        }

        #endregion
    }
}

