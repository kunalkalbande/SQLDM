//------------------------------------------------------------------------------
// <copyright file="DataSources.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.DesktopClient.Helpers
{
    using System;
    using System.ComponentModel;
    using System.Data;
    using System.Data.SqlClient;
    using System.Drawing;
    using System.Threading;
    using BBS.TracerX;
    using TimeZone=Idera.SQLdm.Common.TimeZone.TimeZoneInfo;
    using Infragistics.Win.UltraWinDataSource;
    using Wintellect.PowerCollections;
using System.Collections.Generic;

    #region DataSource

    public class DataSource : UltraDataSource, ISupportInitialize
    {
        private TimeZone timezone = TimeZone.CurrentTimeZone;

        public bool CacheData;

        protected IList<object[]> table;
        protected object[] xarow;
        protected bool inxa;

        public DataSource()
        {
            this.table = new BigList<object[]>();
            Band.Key = "Band 0";
            CellDataRequested += new CellDataRequestedEventHandler(DataTableDataSource_CellDataRequested);
            CellDataUpdated += new CellDataUpdatedEventHandler(DataTableDataSource_CellDataUpdated);
        }

        public DataSource(IContainer container) : base(container)
        {
            this.table = new BigList<object[]>();
            Band.Key = "Band 0";
            CellDataRequested += new CellDataRequestedEventHandler(DataTableDataSource_CellDataRequested);
            CellDataUpdated += new CellDataUpdatedEventHandler(DataTableDataSource_CellDataUpdated);
        }

        public DataSource(SqlDataReader dataReader) : this()
        {
            Load(dataReader);
        }

        internal void SetSchema(SqlDataReader dataReader)
        {
            UltraDataColumnsCollection udcc = this.Band.Columns;

            DataTable schemaTable = dataReader.GetSchemaTable();
            for (int i = 0; i < schemaTable.Rows.Count; i++)
            {
                DataRow schemaRow = schemaTable.Rows[i];
                UltraDataColumn udc = udcc.Add(schemaRow["ColumnName"].ToString(), schemaRow["DataType"] as Type);
                udc.AllowDBNull =
                    (Convert.ToBoolean(schemaRow["AllowDBNull"]))
                        ? Infragistics.Win.DefaultableBoolean.True
                        : Infragistics.Win.DefaultableBoolean.False;
                udc.ReadOnly =
                    (Convert.ToBoolean(schemaRow["IsReadOnly"]))
                        ? Infragistics.Win.DefaultableBoolean.True
                        : Infragistics.Win.DefaultableBoolean.False;

                // attach a date converter to the tag of date columns
                if (udc.DataType == typeof (DateTime))
                {
                    udc.Tag = new DateTimeConverter();
                }
            }
        }

        public void Load(SqlDataReader dataReader) 
        {
            // clear existing rows
            table.Clear();
            // load from the reader
            Merge(dataReader);
        }

        public virtual void Merge(SqlDataReader dataReader)
        {
            // create the schema if one does not already exist
            if (this.Band.Columns.Count == 0)
                SetSchema(dataReader);

            try
            {
                while (dataReader.Read())
                {
                    object[] rowValues = new object[dataReader.FieldCount];
                    dataReader.GetValues(rowValues);
                    AddRow(rowValues);
                }
            }
            finally
            {   // set the number of available rows
                Rows.SetCount(table.Count);
            }
        }

        /// <summary>
        /// Add a new row to the table.  Made virtual to allow subclassers to 
        /// control merging of new data with existing data.
        /// </summary>
        /// <param name="values">a row of data</param>
        internal virtual void AddRow(object[] values)
        {
            table.Add(values);
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TimeZone TimeZoneInfo
        {
            get { return timezone; }
            set
            {
                timezone = value;
                foreach (UltraDataColumn udc in Band.Columns)
                {
                    if (udc.DataType == typeof(DateTime))
                        udc.ResetCachedValues();
                }
                this.Rows.NotifyListReset();
            }
        }

        protected override void OnRowBeginEdit(RowBeginEditEventArgs e)
        {
            int i = e.Row.Index;

            // take a copy of the row
            xarow = (object[])table[i].Clone();
            inxa = true;

            base.OnRowBeginEdit(e);
        }

        protected override void OnRowCancelEdit(RowCancelEditEventArgs e)
        {
            if (inxa)
            {
                int i = e.Row.Index;
                table[i] = xarow;
                xarow = null;
                inxa = false;
            }
            base.OnRowCancelEdit(e);
        }

        protected override void OnRowEndEdit(RowEndEditEventArgs e)
        {
            inxa = false;
            xarow = null;
            base.OnRowEndEdit(e);
        }

        protected void DataTableDataSource_CellDataUpdated(object sender, CellDataUpdatedEventArgs e)
        {
            // stuff the new value back into the list object
            int i = e.Row.Index;
            if (i < table.Count)
            {
                int c = e.Column.Index;
                object[] row = table[i];
                if (c < row.Length)
                    row[c] = e.NewValue;
            }
        }

        protected void DataTableDataSource_CellDataRequested(object sender, CellDataRequestedEventArgs e)
        {
            int i = e.Row.Index;
            if (i < table.Count)
            {
                IValueConverter converter = e.Column.Tag as IValueConverter;
                if (converter != null)
                {
                    e.Data = converter.ConvertData(e.Column, e.Row);
                } else
                {
                    e.Data = GetRawCellValue(i, e.Column.Index);
                }
            }
            e.CacheData = CacheData;
        }

        internal object GetRawCellValue(int rowIndex, int columnIndex)
        {
            object[] values = table[rowIndex];
            if (columnIndex < values.Length)
            {
                return values[columnIndex];
            }
            return null;
        }

        public virtual void Clear()
        {
            this.table.Clear();
            this.Rows.SetCount(0);
        }

        public virtual void BeginInit()
        {
            this.SuspendBindingNotifications();
        }

        public virtual void EndInit()
        {
            this.ResumeBindingNotifications();
        }
    }
    
    #endregion

    #region DataSourceWithID

    public class DataSourceWithID<T> : UltraDataSource, ISupportInitialize
    {
        private static Logger LOG = Logger.GetLogger(String.Format("DataSourceWithID<{0}>", typeof(T).Name));
        private object sync = new object();
        private object updateLock = new object();
        private long   updateId;

        private TimeZoneInfo timezone;
        private int keyIndex;
        private Dictionary<T, UltraDataRow> rowMap;

        public DataSourceWithID() 
        {
            rowMap = new Dictionary<T, UltraDataRow>();
        }

        public DataSourceWithID(IContainer container) : base(container)
        {
            rowMap = new Dictionary<T, UltraDataRow>();
        }

        public int KeyIndex
        {
            get { return keyIndex; }
            set { keyIndex = value; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TimeZoneInfo TimeZoneInfo
        {
            get { return timezone; }
            set
            {
                timezone = value;
                foreach (UltraDataColumn udc in Band.Columns)
                {
                    if (udc.DataType == typeof(DateTime))
                        udc.ResetCachedValues();
                }
                this.Rows.NotifyListReset();
            }
        }

        internal void SetSchema(SqlDataReader dataReader)
        {
            UltraDataColumnsCollection udcc = this.Band.Columns;

            DataTable schemaTable = dataReader.GetSchemaTable();
            for (int i = 0; i < schemaTable.Rows.Count; i++)
            {
                DataRow schemaRow = schemaTable.Rows[i];
                UltraDataColumn udc = udcc.Add(schemaRow["ColumnName"].ToString(), schemaRow["DataType"] as Type);
                udc.AllowDBNull =
                    (Convert.ToBoolean(schemaRow["AllowDBNull"]))
                        ? Infragistics.Win.DefaultableBoolean.True
                        : Infragistics.Win.DefaultableBoolean.False;
                udc.ReadOnly =
                    (Convert.ToBoolean(schemaRow["IsReadOnly"]))
                        ? Infragistics.Win.DefaultableBoolean.True
                        : Infragistics.Win.DefaultableBoolean.False;

                // attach a date converter to the tag of date columns
                if (udc.DataType == typeof(DateTime))
                {
                    udc.Tag = new DateTimeConverter();
                }
            }
        }

        public void Clear()
        {
            while (true)
            {
                long updateSeq = Interlocked.Increment(ref updateId);
                lock (updateLock)
                {
                    if (updateSeq != Interlocked.Read(ref updateId))
                        continue;

                    rowMap.Clear();
                    base.Rows.Clear();
                    break;
                }
            }
        }

        public void Load(SqlDataReader dataReader)
        {
            // clear existing rows
            Clear();
            // load from the reader
            Merge(dataReader);
        }

        /// <summary>
        /// Adds new records and updates existing.
        /// </summary>
        /// <param name="dataReader"></param>
        public virtual void Merge(SqlDataReader dataReader)
        {
            lock (updateLock)
            {
                long updateSequence = Interlocked.Increment(ref updateId);

                // create the schema if one does not already exist
                if (this.Band.Columns.Count == 0)
                    SetSchema(dataReader);

                while (dataReader.Read())
                {
                    if (updateSequence != Interlocked.Read(ref updateId))
                    {
                        LOG.Debug("Update sequence changed - exiting Merge");
                        break;
                    }
                    object[] rowValues = new object[dataReader.FieldCount];
                    dataReader.GetValues(rowValues);
                    InternalAddRow(rowValues);
                }
            }
        }

        /// <summary>
        /// Adds new records, updates existing, deletes the rest.
        /// </summary>
        /// <param name="dataReader"></param>
        public virtual void Update(SqlDataReader dataReader) {
            using (LOG.VerboseCall()) {
                lock (updateLock) {
                    long updateSequence = Interlocked.Increment(ref updateId);

                    // create the schema if one does not already exist
                    if (this.Band.Columns.Count == 0)
                        SetSchema(dataReader);

                    Set<T> keySet = new Set<T>();

                    while (dataReader.Read()) {
                        if (updateSequence != Interlocked.Read(ref updateId)) {
                            LOG.Debug("Update sequence changed - exiting Update (add/update)");
                            break;
                        }
                        object[] rowValues = new object[dataReader.FieldCount];
                        dataReader.GetValues(rowValues);
                        //SQLdm (Tushar)--Fix for issue SQLDM-28167
                        rowValues = ConvertDateTime(rowValues);
                        keySet.Add(InternalAddRow(rowValues));
                    }

                    UltraDataRow row = null;
                    // remove all the rows not in the keyset
                    foreach (T key in Algorithms.SetDifference(rowMap.Keys, keySet)) {
                        if (updateSequence != Interlocked.Read(ref updateId)) {
                            LOG.Debug("Update sequence changed - exiting Update (delete)");
                            break;
                        }
                        if (rowMap.TryGetValue(key, out row)) {
                            this.Rows.Remove(row);
                            rowMap.Remove(key);
                        }
                    }
                }
            }
        }

        //Start-SQLdm (Tushar)--Fix for issue SQLDM-28167
        /// <summary>
        /// This method will take a single row of the result of p_GetAlerts procedure and update timestamps in the message
        /// text and Header text of the BlockingSessions alert (which are in UTC) to the local time of the Desktop Client.
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private object[] ConvertDateTime(object[] row)
        {
            try
            {
                int metricID;
                string messageHeader, messageBody;
                DateTime utcDateTime;
                DateTime localDateTime;

                //Extracting the metric ID value
                int.TryParse(Convert.ToString((row[6])), out metricID);
                if (metricID == 33)
                {
                    //If metric is a blocking session metric, then process. otherwise return the original row.
                    //Extract messageHeader and messageBody of the blocking sessions alert.
                    try
                    {
                        messageHeader = Convert.ToString(row[10]);
                        messageBody = Convert.ToString(row[11]);
                        if (row[4] != null)
                        {
                            //Extract the UTC time field.
                            utcDateTime = DateTime.Parse(Convert.ToString(row[4]), System.Globalization.CultureInfo.InvariantCulture);

                            //Convert the UTC daate time to desktop client local time.
                            localDateTime = utcDateTime.ToLocalTime();

                            //Find all the occurrences of datetime text in messageBody of the alert and replace it with converted time in desktop client time zone.
                            messageBody = System.Text.RegularExpressions.Regex.Replace(messageBody, @"(?<=, since | issued at ).+?(?=\(UTC\))\(UTC\)", localDateTime.ToString("M/d/yyyy h:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture));

                            //Find all the occurrences of datetime text in messageHeader of the alert and replace it with converted time in desktop client time zone.
                            messageHeader = System.Text.RegularExpressions.Regex.Replace(messageHeader, @"(?<=, since | issued at ).+?(?=\(UTC\))\(UTC\)", localDateTime.ToString("M/d/yyyy h:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture));

                            //Update the row with modified message header and body messages.
                            row[10] = messageHeader;
                            row[11] = messageBody;
                        }
                    }
                    catch (Exception ex)
                    {
                        LOG.ErrorFormat("Error occurred during conversion of message header and body for BlockingSessions alert : " + ex.Message + ex.StackTrace);
                    }
                    return row;
                }
                else
                    return row;
            }
            catch (Exception ex)
            {
                LOG.ErrorFormat("Error occurred during conversion of message header and body for BlockingSessions alert : " + ex.Message + ex.StackTrace);
            }
            return row;
        }
        //End-SQLdm (Tushar)--Fix for issue SQLDM-28167

        protected T InternalAddRow(object[] values)
        {
            UltraDataRow row = null;
            
            T key = (T)values[keyIndex];

            if (rowMap.TryGetValue(key, out row)) 
            {
                InternalUpdateRow(row, values);
                return key;
            }

            row = this.Rows.Add(values);
            rowMap.Add(key, row);

            // fixup the columns
            UltraDataColumnsCollection columns = this.Band.Columns;
            for (int i = 0; i < values.Length; i++)
            {
                UltraDataColumn column = columns[i];
                if (column.Tag != null && column.Tag is IValueConverter)
                {
                    row[column] = ((IValueConverter) column.Tag).ConvertData(column, row);
                }
            }

            return key;
        }

        protected void InternalUpdateRow(UltraDataRow row, object[] values)
        {
            UltraDataColumnsCollection columns = this.Band.Columns;

            for (int i = 0; i < values.Length; i++)
            {
                object value = values[i];
                UltraDataColumn column = columns[i];
                if (column.Tag != null && column.Tag is IValueConverter)
                {
                    value = ((IValueConverter)column.Tag).ConvertData(column, row);
                } 
                row[column] = value;
            }
        }

        protected override void OnCellDataRequested(CellDataRequestedEventArgs e)
        {
            IValueConverter converter = e.Column.Tag as IValueConverter;
            if (converter != null)
            {
                e.Data = converter.ConvertData(e.Column, e.Row);
            }
            else
            {
                e.Data = e.Column.DefaultValue;
            }
            e.CacheData = false;
        }

        public UltraDataRow GetRowUsingKey(T key)
        {
            UltraDataRow row = null;

            rowMap.TryGetValue(key, out row);

            return row;
        }

        public virtual void BeginInit()
        {
            this.SuspendBindingNotifications();
        }

        public virtual void EndInit()
        {
            this.ResumeBindingNotifications();
        }



    }

    #endregion


    #region DataTableDataSource

    public class DataTableDataSource : UltraDataSource
    {
        private TimeZone timezone = TimeZone.CurrentTimeZone;

        private DataTable table;
        public bool CacheData;

        public DataTableDataSource(DataTable table)
        {
            this.table = table;
            Band.Key = "Band 0";

            foreach (DataColumn column in table.Columns)
            {
                Band.Columns.Add(column.ColumnName, column.DataType);
            }
            CellDataRequested += new CellDataRequestedEventHandler(DataTableDataSource_CellDataRequested);
            Rows.SetCount(table.Rows.Count);
        }

        [Browsable(false)]
        public TimeZone TimeZoneInfo
        {
            get { return timezone;  }
            set { 
                    timezone = value;
                    foreach (UltraDataColumn udc in Band.Columns)
                    {
                        if (udc.DataType == typeof(DateTime))
                            udc.ResetCachedValues();
                    }
                    this.Rows.NotifyListReset();
                }
        }

        void DataTableDataSource_CellDataRequested(object sender, CellDataRequestedEventArgs e)
        {
            int i = e.Row.Index;
            if (i < table.Rows.Count)
            {
                DataRow row = table.Rows[i];
                object value = row[e.Column.Key];
                if (value is DateTime && ((DateTime)value).Kind != DateTimeKind.Local)
                    value = timezone.ToLocalTime((DateTime)value);
                e.Data = value;
            }
            e.CacheData = CacheData;
        }
    }
    #endregion

    #region Value Converters

    public interface IValueConverter
    {
        object ConvertData(UltraDataColumn column, UltraDataRow row);
    }

    public class DateConverter : IValueConverter
    {
        public object ConvertData(UltraDataColumn column, UltraDataRow row)
        {
            object value = null;
            if (row.DataSource is DataSource)
                value = ((DataSource)row.DataSource).GetRawCellValue(row.Index, column.Index);
            else if (row.DataSource is UltraDataSource)
            {
                value = row.GetCellValue(column);
            }

            if (value is DateTime)
            {
                return ((DateTime)value).ToLocalTime();
            }
            return value;
        }
    }

    public class MonitoredStateImageConverter : IValueConverter
    {
        private const string LABEL_NONE = "None";
        private const string LABEL_OK = "OK";
        private const string LABEL_WARNING = "Warning";
        private const string LABEL_CRITICAL = "Critical";

        private static Bitmap[] severityBitmaps = 
            {
                null,
                global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusOKSmall,
                global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusWarningSmall,
                null,
                global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusCriticalSmall
            };

        private static string[] severityLabels = 
            {
                LABEL_NONE,
                LABEL_OK,
                LABEL_WARNING,
                null,
                LABEL_CRITICAL
            };

        private string basedOnColumn;

        public MonitoredStateImageConverter(string basedOnColumn)
        {
            this.basedOnColumn = basedOnColumn;
        }

        public object ConvertData(UltraDataColumn column, UltraDataRow row)
        {
            UltraDataColumn sourceColumn = column.ParentCollection[basedOnColumn];
            if (sourceColumn != null)
            {
                object value = ((DataSource) row.DataSource).GetRawCellValue(row.Index, sourceColumn.Index);
                if (column.DataType == typeof (string))
                {
                    return severityLabels[(byte) value];
                }
                if (column.DataType == typeof (Bitmap))
                {
                    return severityBitmaps[(byte) value];
                }
                return value;
            }
            return null;
        }
    }

    #endregion

    internal class UniqueOrderedList<T> : OrderedSet<T>, IList<T>
    {
        public UniqueOrderedList(IComparer<T> comparer) : base(comparer)
        {
            
        }

        public void Insert(int index, T item)
        {
            Add(item);
        }

        public void RemoveAt(int index)
        {
            Remove(base[index]);   
        }

        public new T this[int index]
        {
            get
            {
                return base[index];
            }
            set
            {
                this.Add(value);
            }
        }
    }

}
