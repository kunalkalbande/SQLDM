//------------------------------------------------------------------------------
// <copyright file="LockDetails.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Data;
using Idera.SQLdm.Common.Configuration;
using Wintellect.PowerCollections;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;
    using System.Data.SqlClient;
    using System.Runtime.Serialization;
    using Data;

    /// <summary>
    /// Represents the snapshot for the Disk Size details on-demand probe //--SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements -Added new class 
    /// </summary>
    [Serializable]
    public sealed class DiskSizeDetails : Snapshot, ISerializable
    {
        #region fields

        private Dictionary<string,DiskDriveStatistics> diskDrives;

        private bool hasBeenRowLimited;

        #endregion

        #region constructors

        public DiskSizeDetails(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
            diskDrives = new Dictionary<string,DiskDriveStatistics>();
            hasBeenRowLimited = false;
        }

        public DiskSizeDetails(SerializationInfo info, StreamingContext context)
        {
            SetObjectData(info, context);
            diskDrives = (Dictionary<string, DiskDriveStatistics>)info.GetValue("diskDrives", typeof(Dictionary<string, DiskDriveStatistics>));
            try
            {
                hasBeenRowLimited = (bool)info.GetValue("hasBeenRowLimited", typeof (bool));
            }
            catch
            {
                hasBeenRowLimited = false;
            }
        }



        public DiskSizeDetails(DateTime utcCollectionDateTime, Dictionary<string, DiskDriveStatistics> diskDrives)
        {
            TimeStamp = utcCollectionDateTime;
            this.diskDrives = diskDrives;
        }

        #endregion

        #region properties


        /// <summary>
        /// List of all disks on the system
        /// </summary>
        public Dictionary<string,DiskDriveStatistics> DiskDrives
        {
            get { return diskDrives; }
            set { diskDrives = value; }
        }

        /// <summary>
        /// True when the batch has been cancelled due to row limiting
        /// </summary>
        public bool HasBeenRowLimited
        {
            get { return hasBeenRowLimited; }
            internal set { hasBeenRowLimited = value; }
        }

        #endregion

        #region events

        #endregion

        #region methods

        #endregion

        #region interface implementations
        
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            ISerializable_GetObjectData(info, context);
            info.AddValue("diskDrives", diskDrives);
            info.AddValue("hasBeenRowLimited", hasBeenRowLimited);
            // do not serialize the datasets - they can be rebuilt
        }

        #endregion

        #region nested types

        #endregion

    }
}
