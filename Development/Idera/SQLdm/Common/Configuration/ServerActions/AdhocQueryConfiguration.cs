//------------------------------------------------------------------------------
// <copyright file="AdhocQueryConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Configuration.ServerActions
{
    using System;

    [Serializable]
    public class AdhocQueryConfiguration : OnDemandConfiguration, IServerActionConfiguration
    {
        private bool returnData;
        private string sql;

        public AdhocQueryConfiguration(int monitoredServerId, string sql, bool returnData) : base(monitoredServerId)
        {
            this.sql = sql;
            this.returnData = returnData;
        }

        public string Sql
        {
            get { return sql; }
            set { sql = value; }
        }


        public bool ReturnData
        {
            get { return returnData; }
            set { returnData = value; }
        }
    }

}
