//------------------------------------------------------------------------------
// <copyright file="IScheduledCollectionDataManager.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.ManagementService.Monitoring.Data
{
    using Idera.SQLdm.Common.Objects;
    using Idera.SQLdm.Common.Snapshots;
    using Idera.SQLdm.Common.Status;

    /// <summary>
    /// Enter a description for this interface
    /// </summary>
    interface IScheduledCollectionDataManager {
        #region properties

        #endregion

        #region events

        #endregion

        #region methods

        MonitoredObjectStateGraph GetStateGraph(MonitoredObject monitoredObject);
        bool SaveOutstandingEvent(OutstandingEventEntry deviation);
        bool UpdateOutstandingEvent(OutstandingEventEntry deviation);
        bool DeleteOutstandingEvent(OutstandingEventEntry deviation);

        void SaveScheduledRefresh(ScheduledRefresh refresh);
        
        #endregion
    }
}
