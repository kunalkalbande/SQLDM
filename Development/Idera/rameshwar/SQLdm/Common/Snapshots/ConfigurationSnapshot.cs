//------------------------------------------------------------------------------
// <copyright file="ConfigurationSnapshot.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Data;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents the configuration settings on a monitored server 
    /// </summary>
    [Serializable]
    public sealed class ConfigurationSnapshot : Snapshot
    {
        #region fields

        private DataTable configurationSettings = new DataTable("ConfigurationSettings");
        private DataTable serverProperties = new DataTable("ServerProperties");//SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- new datatable to collect server properties

        #endregion

        #region constructors

        internal ConfigurationSnapshot(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
            configurationSettings.RemotingFormat = SerializationFormat.Binary;
        }

        #endregion

        #region properties

        public DataTable ConfigurationSettings
        {
            get { return configurationSettings; }
            internal set { configurationSettings = value; }
        }

        //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- new datatable to collect server properties
        public DataTable ServerProperties
        {
            get { return serverProperties; }
            internal set { serverProperties = value; }
        }
        //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- new datatable to collect server properties

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
