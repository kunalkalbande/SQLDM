//------------------------------------------------------------------------------
// <copyright file="WaitingBatchesSnapshot.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Data;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;
using System.Collections.Generic;

    /// <summary>
    /// Represents the WaitingBatches information on a monitored server //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- new Snapshot class
    /// </summary>
    [Serializable]
    public sealed class WaitingBatchesSnapshot : Snapshot
    {
        #region fields

        private DataTable waitingBatches = new DataTable("WaitingBatches");
        private List<DataTable> listWaitingBatches = new List<DataTable>();//required for interval collection

     
        #endregion

        #region constructors

        internal WaitingBatchesSnapshot(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
            waitingBatches.RemotingFormat = SerializationFormat.Binary;
        }

        #endregion

        #region properties

        public DataTable WaitingBatches
        {
            get { return waitingBatches; }
            internal set { waitingBatches = value; }
        }

        public List<DataTable> ListWaitingBatches
        {
            get { return listWaitingBatches; }
            internal set { listWaitingBatches = value; }
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
