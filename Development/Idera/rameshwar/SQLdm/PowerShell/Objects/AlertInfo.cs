//------------------------------------------------------------------------------
// <copyright file="AlertInfo.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.PowerShell.Objects
{
    using System;
    using System.Data.SqlClient;
    using Common;
    using Helpers;
    using Idera.SQLdm.Common.Events;

    public class AlertInfo
    {
        public readonly long AlertID;
        public readonly DateTime CollectionDateTime;
        public readonly string InstanceName;
        public readonly Metric Metric;
        public readonly MonitoredState Severity;
        public readonly Transition StateChange;
        public readonly string DatabaseName;
        public readonly string Message;
        public readonly string Details;

        public AlertInfo(MetricDefinitions metricDefs, SqlDataReader dataReader)
        {
            AlertID = dataReader.GetInt64(0);
            CollectionDateTime = dataReader.GetDateTime(1).ToLocalTime();
            InstanceName = Helper.GetReaderValue(dataReader, 2, String.Empty);
            DatabaseName = Helper.GetReaderValue(dataReader, 3, String.Empty);
            Message = Helper.GetReaderValue(dataReader, 10, String.Empty);
            Details = Helper.GetReaderValue(dataReader, 11, String.Empty);

            Metric = Helper.GetReaderValue<Idera.SQLdm.Common.Events.Metric>(dataReader, 6, Idera.SQLdm.Common.Events.Metric.Operational);
            Severity = Helper.GetReaderValue<MonitoredState>(dataReader, 7, MonitoredState.None);
            StateChange = Helper.GetReaderValue<Transition>(dataReader, 8, Transition.Critical_OK);
        }
    }
}
