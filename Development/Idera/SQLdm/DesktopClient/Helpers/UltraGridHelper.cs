namespace Idera.SQLdm.DesktopClient.Helpers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;
    using Infragistics.Win;
    using Infragistics.Win.UltraWinGrid;
    using Wintellect.PowerCollections;
    using BBS.TracerX;

    public static class UltraGridHelper
    {
        private static readonly Logger Log = Logger.GetLogger("UltraGridHelper");

        public enum CopyOptions
        {
            AllRows,
            AllSelectedRows,
            FirstSelectedRow
        }
        public enum CopyFormat
        {
            FormattedText,
            CommaSeparatedValues,
            AllFormats
        }

        private static int UltraGridColumnOrderComparison(UltraGridColumn left, UltraGridColumn right)
        {
            return left.Header.VisiblePosition.CompareTo(right.Header.VisiblePosition);
        }

        public static void CopyToClipboard(UltraGrid grid, CopyOptions options, CopyFormat format)
        {
            IEnumerator iterator;

            if (grid == null)
                return;

            StringBuilder sb = new StringBuilder();
            MemoryStream memoryStream = null;
            DataObject dataObject = new DataObject();

            List<UltraGridColumn> columns = new List<UltraGridColumn>();
            foreach (UltraGridColumn column in grid.DisplayLayout.Bands[0].Columns)
            {
                if (!column.Hidden)
                    columns.Add(column);
            }

            Algorithms.SortInPlace(columns, UltraGridColumnOrderComparison);
            try
            {
                if (format == CopyFormat.CommaSeparatedValues || format == CopyFormat.AllFormats)
                {
                    iterator = options == CopyOptions.AllRows
                                   ?
                                       iterator = grid.Rows.GetEnumerator()
                                   :
                                       iterator = grid.Selected.Rows.GetEnumerator();
                    while (iterator.MoveNext())
                    {
                        UltraGridRow row = iterator.Current as UltraGridRow;
                        if (row == null || !row.IsDataRow)
                            continue;
                        FormatRowAsCSV(columns, row, sb);
                        if (options == CopyOptions.FirstSelectedRow)
                            break;
                    }
                    // null terminate the string
                    sb.Append("\0");

                    byte[] blob = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
                    memoryStream = new MemoryStream(blob);
                    dataObject.SetData(DataFormats.CommaSeparatedValue, memoryStream);
                }
                if (format == CopyFormat.FormattedText || format == CopyFormat.AllFormats)
                {
                    sb.Length = 0;
                    iterator = options == CopyOptions.AllRows
                                   ?
                                       iterator = grid.Rows.GetEnumerator()
                                   :
                                       iterator = grid.Selected.Rows.GetEnumerator();
                    while (iterator.MoveNext())
                    {
                        UltraGridRow row = iterator.Current as UltraGridRow;
                        if (row == null || !row.IsDataRow)
                            continue;
                        FormatRowAsText(columns, row, sb);
                        if (options == CopyOptions.FirstSelectedRow)
                            break;
                    }
                    dataObject.SetText(sb.ToString());
                }

                Clipboard.SetDataObject(dataObject, true);
            } 
            catch (Exception e)
            {
                Log.Error("An error occurred while copying text to clipboard. ", e);
            }
            finally
            {
                if (memoryStream != null)
                    memoryStream.Dispose();
            }
        }

        private static string GetDisplayText(UltraGridCell cell)
        {
            string displayText = cell.Text;
            if (displayText.Equals(cell.Value.GetType().ToString()))
                displayText = String.Empty;

            if (String.IsNullOrEmpty(displayText))
            {
                ValueList valueList = cell.Column.ValueList as ValueList;
                if (valueList != null)
                {
                    ValueListItem item = valueList.FindByDataValue(cell.Value);
                    if (item != null)
                    {
                        displayText = item.DisplayText;
                    }
                }
            }
            if (String.IsNullOrEmpty(displayText))
            {
                if (cell.Value.GetType().IsEnum)
                {
                    displayText = cell.Value.ToString();
                }
            }
            return displayText;
        }
        private static string GetColumnHeading(UltraGridCell cell)
        {
            string heading = cell.Column.Header.Caption;
            if (heading.Length <= 1)
                heading = null;

            if (String.IsNullOrEmpty(heading))
                heading = cell.Column.Header.ToolTipText;

            if (String.IsNullOrEmpty(heading))
                heading = cell.Column.Key;

            return heading;
        }

        private static void FormatRowAsText(IEnumerable<UltraGridColumn> columns, UltraGridRow row, StringBuilder sb)
        {
            if (sb.Length > 0)
                sb.Append(Environment.NewLine);

            foreach (UltraGridColumn column in columns)
            {
                UltraGridCell cell = row.Cells[column];
                if (cell != null)
                {
                    string displayText = GetDisplayText(cell);
                    if (!String.IsNullOrEmpty(displayText))
                    {
                        string caption = GetColumnHeading(cell);
                        sb.AppendFormat("{0}: {1}{2}", caption, displayText, Environment.NewLine);
                    }
                }
            }
        }

        private static void FormatRowAsCSV(IEnumerable<UltraGridColumn> columns, UltraGridRow row, StringBuilder sb)
        {
            int mark = sb.Length;

            foreach (UltraGridColumn column in columns)
            {
                UltraGridCell cell = row.Cells[column];
                if (cell != null)
                {
                    if (sb.Length > mark)
                        sb.Append(",");

                    string displayText = GetDisplayText(cell);
                    if (displayText == null)
                        displayText = String.Empty;

                    displayText = displayText.Replace('\"', '\'');

                    sb.AppendFormat("\"{0}\"", displayText);
                }
            }
            if (sb.Length > mark)
                sb.Append(Environment.NewLine);
        }

        /// <summary>
        /// Saves a list of collapsed groups and selected rows in the grid.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static GridState GetGridState(UltraGrid grid, string keyColumn) {
            using (Log.VerboseCall()) {
                GridState state = new GridState();
                state.grid = grid;
                state.keyColumn = keyColumn;

                // save the state of the first row of the scroll region.
                state.scrollRegionFirstRow = null;
                state.scrollRegionFirstRowGroupPath = null;
                UltraGridRow scrollRow = grid.ActiveRowScrollRegion.FirstRow;
                if (scrollRow != null) {
                    if (scrollRow.IsDataRow) {
                        state.scrollRegionFirstRow = scrollRow;
                    } else if (grid.ActiveRowScrollRegion.FirstRow.IsGroupByRow) {
                        state.scrollRegionFirstRowGroupPath = GetPath(scrollRow);
                    }
                }

                // if any columns are specified as group by columns then save the collapsed groups
                foreach (UltraGridColumn column in grid.DisplayLayout.Bands[0].Columns) {

                    if (column.IsGroupByColumn) {
                        state.groupByRowState = GetGroupByRowState(grid);
                        break;
                    }
                }

                if (keyColumn != null) {
                    if (grid.Selected.Rows.Count > 0) {
                        state.selectedRows = new Set<object>();
                        foreach (UltraGridRow row in grid.Selected.Rows) {
                            if (row.IsDataRow) {
                                try {
                                    UltraGridCell cell = row.Cells[keyColumn];
                                    if (cell != null) {
                                        Log.Verbose("Saving selected row ID ", cell.Value);
                                        state.selectedRows.Add(cell.Value);
                                    }
                                } catch (Exception e) {
                                    Debug.Print(e.Message);
                                }
                            }
                        }
                    }
                }

                return state;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        public static void RestoreGridState(GridState state) {
            using (Log.VerboseCall()) {
                UltraGrid grid = state.grid;
                Dictionary<string, GroupByRowState> groupByState = state.groupByRowState;

                // resort and group the rows
                grid.DisplayLayout.Bands[0].SortedColumns.RefreshSort(true);

                // clear current row selections
                grid.Selected.Rows.Clear();

                // restore the group by row state, and also figure out the first row of scroll region.
                UltraGridRow sRow = state.scrollRegionFirstRow;
                if (groupByState != null) {
                    GroupByRowState gbrs;
                    foreach (UltraGridGroupByRow row in
                        grid.DisplayLayout.Rows.GetRowEnumerator(GridRowType.GroupByRow, null, null)) {
                        string rowPath = GetPath(row);
                        if (groupByState.TryGetValue(rowPath, out gbrs)) {
                            row.Selected = gbrs.IsSelected;
                            row.Expanded = gbrs.IsExpanded;
                        }
                        if (sRow == null && state.scrollRegionFirstRowGroupPath != null
                            && string.Compare(rowPath, state.scrollRegionFirstRowGroupPath) == 0) {
                            sRow = row;
                        }
                    }
                }

                // Restore scroll region.
                grid.ActiveRowScrollRegion.FirstRow = sRow;

                // restore selected data rows
                if (state.selectedRows != null) {
                    string keyColumn = state.keyColumn;
                    foreach (UltraGridRow row in grid.Rows.GetAllNonGroupByRows()) {
                        if (row.IsDataRow) {
                            try {
                                UltraGridCell cell = row.Cells[keyColumn];
                                if (cell != null) {
                                    if (state.selectedRows.Contains(cell.Value)) {
                                        Log.Verbose("Selecting row with ID ", cell.Value);

                                        // make this row active and select it
                                        row.Activate(); // Fixes PR 2011130 
                                        row.Selected = true;

                                        // get the currency manager and refresh it
                                        CurrencyManager currencyManager = (String.IsNullOrEmpty(grid.DataMember))
                                                ? ((ICurrencyManagerProvider)grid.DataSource).CurrencyManager
                                                : ((ICurrencyManagerProvider) grid.DataSource).GetRelatedCurrencyManager(grid.DataMember);
                                        currencyManager.Refresh();
                                        // make sure the currency manager is set to the correct index
                                        if (currencyManager.Position != row.ListIndex)
                                            currencyManager.Position = row.ListIndex;
                                    }
                                }
                            } catch (Exception e) {
                                Debug.Print(e.Message);
                            }
                        }
                    }
                }
            }
        }

        private static Dictionary<string, GroupByRowState> GetGroupByRowState(UltraGrid grid)
        {
            Dictionary<string, GroupByRowState> rows = new Dictionary<string, GroupByRowState>();

            foreach (UltraGridGroupByRow row in
                grid.DisplayLayout.Rows.GetRowEnumerator(GridRowType.GroupByRow, null, null))
            {
                if (row.IsExpanded || row.Selected)
                {
                    try
                    {
                        GroupByRowState gbrs = new GroupByRowState();
                        gbrs.IsExpanded = row.IsExpanded;
                        gbrs.IsSelected = row.Selected;
                        rows.Add(GetPath(row), gbrs);
                    }
                    catch (Exception)
                    {
                        Debug.Print("Duplicate row: {0}", GetPath(row));
                    }
                }
            }

            return rows.Count == 0 ? null : rows;
        }

        private static string GetPath(UltraGridRow row)
        {
            StringBuilder builder = new StringBuilder();

            for (UltraGridRow r = row; r != null; r = r.ParentRow)
            {
                if (r is UltraGridGroupByRow)
                {
                    if (builder.Length > 0)
                        builder.Append('\\');
                    builder.AppendFormat("{0}:{1}", ((UltraGridGroupByRow)r).Column.Key, ((UltraGridGroupByRow)r).Value);
                }
            }

            return builder.ToString();
        }
        public static int GetNotHiddenColumns(UltraGrid grid)
        {
            int notHiddenColumns = 0;

            if (grid != null && grid.DisplayLayout.Bands.Count > 0 && grid.DisplayLayout.Bands[0].Columns != null)
            {
                foreach (UltraGridColumn col in grid.DisplayLayout.Bands[0].Columns)
                {
                    if (col != null && !col.Hidden)
                    {
                        notHiddenColumns++;
                    }
                }
            }
            return notHiddenColumns;
        }
        public struct GroupByRowState
        {
            public bool IsSelected;
            public bool IsExpanded;
        }

        public struct GridState
        {
            public UltraGrid grid;
            public string scrollRegionFirstRowGroupPath;
            public UltraGridRow scrollRegionFirstRow;
            public string keyColumn;
            public Set<object> selectedRows;
            public Dictionary<string, GroupByRowState> groupByRowState;
        }

    }
}