//------------------------------------------------------------------------------
// <copyright file="CollectionServiceHibernateDataManager.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.ManagementService.Monitoring.Data
{
    /// <summary>
    /// Enter a description for this class
    /// </summary>
    class CollectionServiceHibernateDataManager // : ICollectionServiceDataManager
    {
        /*
        #region fields

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:CollectionServiceHibernateDataManager"/> class.
        /// </summary>
        public CollectionServiceHibernateDataManager()
            : base()
        {
            Initialize();
        }

        #endregion

        #region properties

        #endregion

        #region events

        #endregion

        #region methods

        /// <summary>
        /// Gets the collection service.
        /// </summary>
        /// <param name="collectionServiceId">The collection service id.</param>
        /// <returns></returns>
        public CollectionServiceInfo GetCollectionService(Guid collectionServiceId)
        {
            IQuery query = CreateQuery("FROM CollectionServiceInfo AS csi WHERE csi.Id=:csid");
            query.SetParameter("csid", collectionServiceId);
            return query.UniqueResult() as CollectionServiceInfo;
        }

        /// <summary>
        /// Gets the collection service for monitored server.
        /// </summary>
        /// <param name="monitoredServer">The monitored server.</param>
        /// <returns></returns>
        public CollectionServiceInfo GetCollectionServiceForMonitoredServer(MonitoredServer monitoredServer)
        {
            IQuery query = CreateQuery("FROM CollectionServiceInfo AS csi JOIN FETCH csi.MonitoredServers AS ms WHERE ms.Id =:msId");
            query.SetParameter("msId", monitoredServer.Id);
            return query.UniqueResult() as CollectionServiceInfo;
        }

        /// <summary>
        /// Gets the collection service workload.
        /// </summary>
        /// <param name="collectionServiceId">The collection service id.</param>
        /// <returns></returns>
        public CollectionServiceWorkload GetCollectionServiceWorkload(Guid collectionServiceId)
        {
            IQuery query = CreateQuery("SELECT csw FROM CollectionServiceWorkload AS csw LEFT JOIN FETCH csw.MonitoredServerWorkloads WHERE csw.Id=:csid");
            query.SetParameter("csid", collectionServiceId);

            return query.UniqueResult() as CollectionServiceWorkload;
        }

        /// <summary>
        /// Saves the collection service.
        /// </summary>
        /// <param name="csi">The csi.</param>
        /// <returns></returns>
        public bool SaveCollectionService(CollectionServiceInfo csi)
        {
            using (IDataTransaction t = Session.BeginTransaction())
            {
                Save(csi);
                t.Commit();
                Flush();
                return true;
            }
        }

        /// <summary>
        /// Saves the collection service workload.
        /// </summary>
        /// <param name="csw">The CSW.</param>
        /// <returns></returns>
        public bool SaveCollectionServiceWorkload(CollectionServiceWorkload csw)
        {
            using (IDataTransaction t = Session.BeginTransaction())
            {
                Save(csw);
                t.Commit();
                Flush();
                return true;
            }
        }

        /// <summary>
        /// Gets the collection services.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CollectionServiceInfo> GetCollectionServices()
        {
            IQuery query = CreateQuery("FROM CollectionServiceInfo AS csi");
            return new List<CollectionServiceInfo>(query.Enumerable<CollectionServiceInfo>());
        }

        /// <summary>
        /// Gets the monitored server workload.
        /// </summary>
        /// <param name="monitoredServer">The monitored server.</param>
        /// <returns></returns>
        public MonitoredServerWorkload GetMonitoredServerWorkload(MonitoredServer monitoredServer)
        {
            IQuery query = CreateQuery("FROM MonitoredServerWorkload AS msw WHERE msw.ID = :msId");
            query.SetParameter("msId", monitoredServer.Id);
            return query.UniqueResult() as MonitoredServerWorkload;
        }

        /// <summary>
        /// Saves the monitored server.
        /// </summary>
        /// <param name="monitoredServer">The monitored server.</param>
        /// <param name="collectionServiceId">The collection service id.</param>
        /// <returns></returns>
        public bool SaveMonitoredServer(MonitoredServerWorkload monitoredServer, Guid collectionServiceId)
        {
            using (IDataTransaction t = Session.BeginTransaction())
            {
                Save(monitoredServer);

                CollectionServiceInfo csi = GetCollectionService(collectionServiceId);
                if (csi == null)
                    return false;
                csi.MonitoredServers.Add(monitoredServer);
                Update(csi);

                t.Commit();
                Flush();
                return true;
            }
        }

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion
        */
    }
}
