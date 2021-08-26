//------------------------------------------------------------------------------
// <copyright file="LogFile.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using Wintellect.PowerCollections;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents a SQL Server log file
    /// </summary>
    [Serializable]
    public sealed class LogFile
    {
        #region fields

        private LogFileType? logType = null;
        private string name;
        private int? number = null;
        private FileSize logFileSize = new FileSize();
        private DateTime? lastModified;
        
        #endregion

        #region constructors

        internal LogFile()
        {
        }

        internal LogFile(LogFileType logType, string name, int number, FileSize logFileSize, DateTime? lastModified)
        {
            this.logType = logType;
            this.name = name;
            this.number = number;
            this.logFileSize = logFileSize;
            this.lastModified = lastModified;
        }

        internal LogFile(LogFileType logType, string name, FileSize logFileSize, DateTime? lastModified)
        {
            this.logType = logType;
            this.name = name;
            this.logFileSize = logFileSize;
            this.lastModified = lastModified;
        }
        #endregion

        #region properties

        /// <summary>
        /// FOR SQLDM INTERNAL USE
        /// Unique identifier for a log file between refreshes
        /// </summary>
        public Pair<LogFileType?, String> InternalLogFileIdentifier
        {
            get { return new Pair<LogFileType?, String>(LogType, Name); }
        }

        /// <summary>
        /// Name of log file
        /// </summary>
        public string Name
        {
            get { return name; }
            internal set { name = value; }
        }

        /// <summary>
        /// Archive number of log file
        /// </summary>
        public int? Number
        {
            get { return number; }
            internal set { number = value; }
        }

        /// <summary>
        /// File size of log
        /// </summary>
        public FileSize LogFileSize
        {
            get { return logFileSize; }
            internal set { logFileSize = value; }
        }

        /// <summary>
        /// Log file type
        /// </summary>
        public LogFileType? LogType
        {
            get { return logType; }
            internal set { logType = value; }
        }

        /// <summary>
        /// Last modified date of log
        /// <remarks>This has internationalization problems so I am hiding from the GUI</remarks>
        /// </summary>
        public DateTime? LastModified
        {
            get { return lastModified; }
            set { lastModified = value; }
        }

        #endregion

        #region events

        #endregion

        #region methods

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

    }
}
