//------------------------------------------------------------------------------
// <copyright file="DatabaseSize.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents sizing information for a database
    /// </summary>
    [Serializable]
    public class DatabaseSize : Database
    {
        #region fields

        private AutogrowStatus dataAutogrow = AutogrowStatus.Unknown;
        private AutogrowStatus logAutogrow = AutogrowStatus.Unknown;
        private FileSize dataFileSize = new FileSize();
        private FileSize dataSize = new FileSize();
        private FileSize indexSize = new FileSize();
        private FileSize textSize = new FileSize();
        private FileSize logSizeUsed = new FileSize();
        private FileSize logFileSize = new FileSize();
        private FileSize databaseExpansion = new FileSize();
        private FileSize logExpansion = new FileSize();
        private float? percentLogSpace = null;
        private float? percentDataSize = null;
        private long? tableCount = null;
        private int? fileCount = null;
        private int? fileGroupCount = null;
        private FileSize fileSpaceUsed = new FileSize();

        #endregion

        public DatabaseSize(string serverName, string dbName) : base(serverName, dbName)
        {
        }

        public override object Clone()
        {               
            // base class does a memberwiseclone.  have to handle complex objects ourselves.
            var dbsize = (DatabaseSize)base.Clone();
            dbsize.dataFileSize = dataFileSize.Copy();
            dbsize.dataSize = dataFileSize.Copy();
            dbsize.indexSize = dataFileSize.Copy();
            dbsize.textSize = dataFileSize.Copy();
            dbsize.logSizeUsed = dataFileSize.Copy();
            dbsize.logFileSize = dataFileSize.Copy();
            dbsize.databaseExpansion = dataFileSize.Copy();
            dbsize.logExpansion = dataFileSize.Copy();
            dbsize.fileSpaceUsed = dataFileSize.Copy();
            return dbsize;
        }

        #region constructors

        #endregion

        #region properties

        /// <summary>
        /// Represents the rollup of autogrow settings for the data files
        /// </summary>
        public AutogrowStatus DataAutogrow
        {
            get { return dataAutogrow; }
        }
        
        /// <summary>
        /// Represents the rollup of autogrow settings for the log files
        /// </summary>
        public AutogrowStatus LogAutogrow
        {
            get { return logAutogrow; }
        }

        /// <summary>
        /// Gets the total allocated data file size.
        /// </summary>
        public FileSize DataFileSize
        {
            get { return dataFileSize; }
            internal set { dataFileSize = value; }
        }

        /// <summary>
        /// Gets the total used space in the data file (data, index, etc)
        /// </summary>
        public FileSize UsedSize
        {
            get {
                if (!FileSpaceUsed.Kilobytes.HasValue || (DataSize.Kilobytes + IndexSize.Kilobytes + TextSize.Kilobytes > FileSpaceUsed.Kilobytes && DataFileSize.Kilobytes > DataSize.Kilobytes + IndexSize.Kilobytes + TextSize.Kilobytes))
                    return new FileSize(DataSize.Kilobytes + IndexSize.Kilobytes + TextSize.Kilobytes);
                else
                    return FileSpaceUsed;
            }
        }

        /// <summary>
        /// Gets total unused space in the data file
        /// </summary>
        public FileSize UnusedSize
        {
            get { return new FileSize(DataFileSize.Kilobytes - UsedSize.Kilobytes); }
        }

        /// <summary>
        /// Gets the current data size used of the allocated data file size. This does NOT include text and index.
        /// </summary>
        public FileSize DataSize
        {
            get { return dataSize; }
            internal set { dataSize = value; }
        }

        /// <summary>
        /// Count of log and data files
        /// </summary>
        public int? FileCount
        {
            get { return fileCount; }
            internal set { fileCount = value; }
        }

        /// <summary>
        /// Count of file groups
        /// </summary>
        public int? FileGroupCount
        {
            get { return fileGroupCount; }
            internal set { fileGroupCount = value; }
        }

        /// <summary>
        /// Gets the current index size used.
        /// </summary>
        public FileSize IndexSize
        {
            get { return indexSize; }
            internal set { indexSize = value; }
        }

        /// <summary>
        /// Gets the current text size used.
        /// </summary>
        public FileSize TextSize
        {
            get { return textSize; }
            internal set { textSize = value; }
        }

        /// <summary>
        /// Gets the total allocated log file size.
        /// </summary>
        public FileSize LogFileSize
        {
            get { return logFileSize; }
            internal set { logFileSize = value; }
        }


        /// <summary>
        /// Gets the current log size used.
        /// </summary>
        public FileSize LogSizeUsed
        {
            get { return logSizeUsed; }
            internal set { logSizeUsed = value; }
        }

        /// <summary>
        /// Gets the current free log space.
        /// </summary>
        public FileSize LogSizeUnused
        {
            get { 
                return new FileSize(LogFileSize.Kilobytes - LogSizeUsed.Kilobytes); 
                }
        }

       
        /// <summary>
        /// Gets the expansion space available for the database.
        /// </summary>
        public FileSize DatabaseExpansion
        {
            get { return databaseExpansion; }
            internal set { databaseExpansion = value; }
        }

        /// <summary>
        /// Gets the expansion space available for the log.
        /// </summary>
        public FileSize LogExpansion
        {
            get { return logExpansion; }
            internal set { logExpansion = value; }
        }

        /// <summary>
        /// Gets the percentage of the log file (plus expansion size) currently occupied with transaction log information.
        /// </summary>
        public float? PercentLogSpace
        {
            get { return percentLogSpace; }
            internal set { percentLogSpace = value; }
        }

        public float? PercentLogFreeSpace
        {
            get { return 1 - PercentLogSpace; }
        }

        /// <summary>
        /// Gets the total number of tables within the database.
        /// </summary>
        public long? TableCount
        {
            get { return tableCount; }
            internal set { tableCount = value; }
        }

        /// <summary>
        /// Data size as a percent of the maximum expandable size of the file
        /// </summary>
        public float? PercentDataSize
        {
            get { return percentDataSize; }
            internal set { percentDataSize = value; }
        }

        /// <summary>
        /// Data size as a percent of the maximum expandable size of the file
        /// </summary>
        public float? PercentDataFreeSize
        {
            get { return 1 - PercentDataSize; }
        }

        /// <summary>
        /// Data size as a percent of the current size of the file
        /// </summary>
        public float? PercentDataSizeCurrent
        {
            get
            {
                if (DataSize.Bytes.HasValue && DataFileSize.Bytes.HasValue)
                {
                    return (float)DataSize.Bytes.Value / (float) DataFileSize.Bytes.Value;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Text size as a percent of the current size of the file
        /// </summary>
        public float? PercentTextSizeCurrent
        {
            get
            {
                if (TextSize.Bytes.HasValue && DataFileSize.Bytes.HasValue)
                {
                    return (float)TextSize.Bytes.Value / (float)DataFileSize.Bytes.Value;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Index size as a percent of the current size of the file
        /// </summary>
        public float? PercentIndexSizeCurrent
        {
            get
            {
                if (IndexSize.Bytes.HasValue && DataFileSize.Bytes.HasValue)
                {
                    return (float)IndexSize.Bytes.Value / (float)DataFileSize.Bytes.Value;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Used size as a percent of the current size of the file
        /// </summary>
        public float? PercentUsedSizeCurrent
        {
            get
            {
                if (UsedSize.Bytes.HasValue && DataFileSize.Bytes.HasValue)
                {
                    return (float)UsedSize.Bytes.Value / (float)DataFileSize.Bytes.Value;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Free size as a percent of the current size of the file
        /// </summary>
        public float? PercentDataFreeCurrent
        {
            get
            {
                if (UsedSize.Bytes.HasValue && DataFileSize.Bytes.HasValue)
                {
                    return 1 - PercentUsedSizeCurrent;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Free size as a percent of the current size of the file
        /// </summary>
        public float? PercentLogSizeCurrent
        {
            get
            {
                if (LogSizeUsed.Bytes.HasValue && LogFileSize.Bytes.HasValue)
                {
                    return (float)LogSizeUsed.Bytes.Value / (float)LogFileSize.Bytes.Value;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Free size as a percent of the current size of the file
        /// </summary>
        public float? PercentLogFreeCurrent
        {
            get
            {
                if (LogSizeUsed.Bytes.HasValue && LogFileSize.Bytes.HasValue)
                {
                    return 1 - PercentLogSizeCurrent;
                }
                else
                {
                    return null;
                }
            }
        }


        public FileSize FileSpaceUsed
        {
            get { return fileSpaceUsed; }
            internal set { fileSpaceUsed = value; }
        }

        #endregion

        #region events

        #endregion

        #region methods

        // Autogrow Flags
        // 1 = data autogrow on
        // 4 = data autogrow off
        // 8 = log autogrow on
        // 16 = log autogrow off
        internal void SetAutogrow(int autogrowFlag)
        {
            if (((autogrowFlag & 1) == 1) && ((autogrowFlag & 4) == 4))
            {
                dataAutogrow = AutogrowStatus.Mixed;
            }
            else
            {
                if (((autogrowFlag & 1) == 1))
                {
                    dataAutogrow = AutogrowStatus.AutogrowOn;
                }
                else
                {
                    if (((autogrowFlag & 4) == 4))
                        dataAutogrow = AutogrowStatus.AutogrowOff;
                }
            }

            if (((autogrowFlag & 8) == 8) && ((autogrowFlag & 16) == 16))
            {
                logAutogrow = AutogrowStatus.Mixed;
            }
            else
            {
                if (((autogrowFlag & 8) == 8))
                {
                    logAutogrow = AutogrowStatus.AutogrowOn;
                }
                else
                {
                    if (((autogrowFlag & 16) == 16))
                        logAutogrow = AutogrowStatus.AutogrowOff;
                }
            }

        }

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion
    }
}