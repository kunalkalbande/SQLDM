using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;
using Infragistics.Win.UltraWinGrid;
using Wintellect.PowerCollections;

namespace Idera.SQLdm.DesktopClient.Objects
{
    [SerializableAttribute]
    public class GridSettings // : IXmlSerializable
    {
        #region fields

        private bool groupbyBoxVisible;
        private Dictionary<Pair<int, string>, ColumnSettings> columns;

        #endregion

        #region constructors

        public GridSettings()
        {
            groupbyBoxVisible = false;
            columns = new Dictionary<Pair<int, string>, ColumnSettings>();
        }

        #endregion

        #region properties

        public bool GroupByBoxVisible
        {
            get { return groupbyBoxVisible; }
            set { groupbyBoxVisible = value; }
        }

        public Dictionary<Pair<int, string>, ColumnSettings> Columns
        {
            get { return columns; }
            set { columns = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Apply the passed GridSettings to the passed UltraGrid
        /// </summary>
        /// <param name="settings">The GridSettings to apply to the grid</param>
        /// <param name="grid">The UltraGrid that will have the settings applied to it</param>
        public static void ApplySettingsToGrid(GridSettings settings, UltraGrid grid)
        {
            UltraGridColumn col;
            SortedList<Pair<int, int>, ColumnSettings> colSorts = new SortedList<Pair<int, int>, ColumnSettings>();

            grid.DisplayLayout.GroupByBox.Hidden = !settings.GroupByBoxVisible;
            foreach (ColumnSettings colSettings in settings.Columns.Values)
            {
                if (grid.DisplayLayout.Bands.Count > colSettings.Band
                    && grid.DisplayLayout.Bands[colSettings.Band].Columns.IndexOf(colSettings.Key) > -1)
                {
                    col = grid.DisplayLayout.Bands[colSettings.Band].Columns[colSettings.Key];
                    col.Header.VisiblePosition = colSettings.Position;
                    col.Header.Fixed = colSettings.Fixed;
                    col.Width = colSettings.Width;
                    // don't redisplay a column that is now permanently hidden
                    col.Hidden = colSettings.Hidden || col.ExcludeFromColumnChooser == ExcludeFromColumnChooser.True;
                    col.SortIndicator = SortIndicator.None;
                    if (colSettings.SortDirection != SortIndicator.None)
                    {
                        colSorts.Add(new Pair<int, int>(colSettings.Band, colSettings.SortPosition), colSettings);
                    }
                }
            }

            // Apply the sorts in order
            foreach (KeyValuePair<Pair<int, int>, ColumnSettings> colSort in colSorts)
            {
                grid.DisplayLayout.Bands[colSort.Key.First].SortedColumns.Add(
                        grid.DisplayLayout.Bands[colSort.Key.First].Columns[colSort.Value.Key],
                        colSort.Value.SortDirection == SortIndicator.Descending ? true : false,
                        colSort.Value.IsGroupByColumn);
            }
        }

        /// <summary>
        /// Compare to the passed GridSettings object and make sure all internal values are equal
        /// </summary>
        /// <param name="obj">a GridSettings object to compare against</param>
        /// <returns>true if a GridSettings object is passed and all settings match otherwise false</returns>
        public override bool Equals(object obj)
        {
            if (obj is GridSettings)
            {
                GridSettings settings = (GridSettings)obj;
                if (groupbyBoxVisible != settings.GroupByBoxVisible
                    || columns.Count != settings.columns.Count)
                {
                    return false;
                }
                else
                {
                    ColumnSettings columnSettings;
                    foreach (KeyValuePair<Pair<int, string>, ColumnSettings> colSettings in columns)
                    {
                        if (settings.Columns.TryGetValue(colSettings.Key, out columnSettings))
                        {
                            if (!colSettings.Value.Equals(columnSettings))
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }
            else
            {
                return base.Equals(obj);
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Create a new GridSettings object with settings from the passed grid
        /// </summary>
        /// <param name="grid">The UltraGrid from which the settings will be retrieved</param>
        /// <returns>A GridSettings object with the current settings of the grid</returns>
        public static GridSettings GetSettings(UltraGrid grid)
        {
            GridSettings gridSettings = new GridSettings();

            SaveSettings(gridSettings, grid);

            return gridSettings;
        }

        /// <summary>
        /// Save the current settings from the passed grid
        /// </summary>
        public void SaveSettings(UltraGrid grid)
        {
            SaveSettings(this, grid);
        }

        /// <summary>
        /// Save current settings from the passed grid to the passed GridSettings
        /// </summary>
        /// <param name="settings">The GridSettings that will have the settings saved to it</param>
        /// <param name="grid">The UltraGrid from which the settings will be saved</param>
        public static void SaveSettings(GridSettings settings, UltraGrid grid)
        {
            SortedList<Pair<int, int>, ColumnSettings> colSorts = new SortedList<Pair<int, int>, ColumnSettings>();

            settings.GroupByBoxVisible = !grid.DisplayLayout.GroupByBox.Hidden;
            settings.Columns = new Dictionary<Pair<int, string>, ColumnSettings>();
            ColumnSettings colSettings;
            foreach (UltraGridBand band in grid.DisplayLayout.Bands)
            {
                foreach (UltraGridColumn col in band.Columns)
                {
                    colSettings = new ColumnSettings(band.Index, col.Key, col.Header.VisiblePosition, col.Header.Fixed, col.Width, col.Hidden);
                    colSettings.SetColumnSort(band.SortedColumns.IndexOf(col), col.SortIndicator, col.IsGroupByColumn);
                    settings.Columns.Add(new Pair<int, string>(band.Index, col.Key), colSettings);
                }
            }
        }

        #endregion

        //#region IXmlSerializable Members

        //public System.Xml.Schema.XmlSchema GetSchema()
        //{
        //    return null;
        //}

        //public void ReadXml(System.Xml.XmlReader reader)
        //{
        //    ColumnSettings columnSettings;

        //    reader.ReadStartElement();
        //    reader.MoveToElement();

        //    bool? x = reader.ReadElementContentAsBoolean();


        //    int columnCount = reader.ReadContentAsInt();
        //    columns.Clear();
        //    for (int i = 0; i < columnCount; i++)
        //    {
        //        //columnSettings = reader.ReadContentAs(typeof(ColumnSettings), Idera.SQLdm.DesktopClient.Objects);
        //        //if (columnSettings is ColumnSettings)
        //        //{
        //        //    columns.Add(new Pair<int, string>(columnSettings.Band, columnSettings.Key), columnSettings);
        //        //}
        //    }

        //    reader.ReadEndElement();
        //}

        //public void WriteXml(System.Xml.XmlWriter writer)
        //{
        //    writer.WriteElementString("GroupByBoxVisible", GroupByBoxVisible.ToString());
        //    writer.WriteElementString("ColumnCount", Columns.Count.ToString());
        //    foreach (ColumnSettings colSettings in Columns.Values)
        //    {
        //        colSettings.WriteXml(writer);
        //    }
        //}

        //#endregion
    }

    [SerializableAttribute]
    public class ColumnSettings // : IXmlSerializable
    {
        private int band;
        private bool fixedCol;
        private bool hidden;
        private string key;
        private int position;
        private int width;
        private int sortPosition;
        private SortIndicator sortDirection = SortIndicator.None;
        private bool isGroupByColumn = false;

        public ColumnSettings(int Band, string Key, int Position, bool Fixed, int Width, bool Hidden)
        {
            band = Band;
            key = Key;
            position = Position;
            fixedCol = Fixed;
            width = Width;
            hidden = Hidden;
        }

        public int Band { get { return band; } }
        public bool Fixed { get { return fixedCol; } }
        public bool Hidden { get { return hidden; } }
        public string Key { get { return key; } }
        public int Position { get { return position; } }
        public int Width { get { return width; } }
        public int SortPosition { get { return sortPosition; } }
        public SortIndicator SortDirection { get { return sortDirection; } }
        public bool IsGroupByColumn { get { return isGroupByColumn; } }

        public override bool Equals(object obj)
        {
            if (obj is ColumnSettings)
            {
                ColumnSettings settings = (ColumnSettings)obj;
                if (band == settings.Band
                    && key == settings.Key
                    && position == settings.Position
                    && fixedCol == settings.Fixed
                    && width == settings.Width
                    && hidden == settings.Hidden
                    && sortPosition == settings.SortPosition
                    && sortDirection == settings.SortDirection
                    && isGroupByColumn == settings.IsGroupByColumn)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return base.Equals(obj);
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public void SetColumnSort(int Position, SortIndicator SortDirection, bool IsGroupByColumn)
        {
            sortPosition = Position;
            sortDirection = SortDirection;
            isGroupByColumn = IsGroupByColumn;
        }

        //#region IXmlSerializable Members

        //public System.Xml.Schema.XmlSchema GetSchema()
        //{
        //    return null;
        //}

        //public void ReadXml(System.Xml.XmlReader reader)
        //{
        //    band = reader.ReadContentAsInt();
        //    key = reader.ReadContentAsString();
        //    reader.MoveToNextAttribute();
        //    fixedCol = reader.ReadContentAsBoolean();
        //    hidden = reader.ReadElementContentAsBoolean("Hidden", "");
        //    position = reader.ReadContentAsInt();
        //    width = reader.ReadContentAsInt();
        //}

        //public void WriteXml(System.Xml.XmlWriter writer)
        //{
        //    writer.WriteStartElement("Column");
        //    writer.WriteElementString("Band", band.ToString());
        //    writer.WriteElementString("Key", key);
        //    writer.WriteElementString("Fixed", fixedCol.ToString());
        //    writer.WriteElementString("Hidden", hidden.ToString());
        //    writer.WriteElementString("Position", position.ToString());
        //    writer.WriteElementString("Width", width.ToString());
        //    if (sortPosition > -1)
        //    {
        //        writer.WriteElementString("SortPosition", sortPosition.ToString());
        //        writer.WriteElementString("SortDirection", sortDirection.ToString());
        //        writer.WriteElementString("IsGroupByColumn", isGroupByColumn.ToString());
        //    }
        //    writer.WriteEndElement();
        //}

        //#endregion
    }
}
