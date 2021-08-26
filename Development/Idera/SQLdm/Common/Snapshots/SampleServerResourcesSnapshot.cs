//------------------------------------------------------------------------------
// <copyright file="SampleServerResourcesSnapshot.cs" company="Idera, Inc.">
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
    /// Represents the DisabledIndexes information on a monitored server //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- new Snapshot class
    /// </summary>
    [Serializable]
    public sealed class SampleServerResourcesSnapshot : Snapshot
    {
        #region fields

        private DataTable sampleServerResources = new DataTable("SampleServerResources");

        #endregion

        #region constructors

        internal SampleServerResourcesSnapshot(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
            sampleServerResources.RemotingFormat = SerializationFormat.Binary;
        }

        #endregion

        #region properties

        public DataTable SampleServerResources
        {
            get { return sampleServerResources; }
            internal set { sampleServerResources = value; }
        }

        public List<DataTable> LstSampleServerResources
        {
            get;
            set;
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
