//------------------------------------------------------------------------------
// <copyright file="RecoveryOperation.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// The base class for a backup or restore recovery operation.
    /// </summary>
    public abstract class RecoveryOperation
    {
        #region fields

        private string _deviceName = null;
        private string _filegroupName = null;
        private string _performedBy = null;
        private FileSize _size = null;
        private string _tableIndexName = null;
        private DateTime? _timestamp = null;
        private RecoveryOperationType? _type = null;

        #endregion

        #region properties

        /// <summary>
        /// Gets the name of the recovery device.
        /// </summary>
        public string DeviceName
        {
            get { return _deviceName; }
        }

        /// <summary>
        /// Gets the filegroup name.
        /// </summary>
        public string FilegroupName
        {
            get { return _filegroupName; }
        }

        /// <summary>
        /// Gets the name of the user that performed the recovery operation.
        /// </summary>
        public string PerformedBy
        {
            get { return _performedBy; }
        }

        /// <summary>
        /// Gets the size of the backup data.
        /// </summary>
        public FileSize Size
        {
            get { return _size; }
        }

        /// <summary>
        /// Gets the table and index information if the recovery operation used a file or filegroup option. 
        /// </summary>
        public string TableIndexName
        {
            get { return _tableIndexName; }
        }

        /// <summary>
        /// Gets the date\time of the recovery operation.
        /// </summary>
        public DateTime? Timestamp
        {
            get { return _timestamp; }
        }

        /// <summary>
        /// Gets the 
        /// </summary>
        public RecoveryOperationType? Type
        {
            get { return _type; }
        }

        #endregion

        #region methods

        //public abstract string Dump();

        //internal void SetDeviceName(string deviceName)
        //{
        //    _deviceName = deviceName;
        //}

        //internal void SetFilegroupName(string filegroupName)
        //{
        //    _filegroupName = filegroupName;
        //}

        //internal void SetPerformedBy(string performedBy)
        //{
        //    _performedBy = performedBy;
        //}

        //internal void SetSize(decimal size)
        //{
        //    _size = size;
        //}

        //internal void SetTableIndexName(string tableIndexName)
        //{
        //    _tableIndexName = tableIndexName;
        //}

        //internal void SetTimestamp(DateTime timestamp)
        //{
        //    _timestamp = timestamp;
        //}

        //internal void SetType(RecoveryOperationType type)
        //{
        //    _type = type;
        //}

        #endregion
    }

    
}
