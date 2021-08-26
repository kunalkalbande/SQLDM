using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.PredictiveAnalyticsService.Configuration;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.Common.Configuration;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Configuration;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Snapshots;
using Idera.SQLdm.Common.Objects;

namespace Idera.SQLdm.PredictiveAnalyticsService.Helpers
{
    /// <summary>
    /// //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics)
    /// </summary>
    internal static partial class DataHelper
    {
        private static ICollectionService GetCollectionService()
        {
            string address = PredictiveAnalyticsConfiguration.CollectionServiceAddress;
            int port = PredictiveAnalyticsConfiguration.CollectionServicePort;

            try
            {
                Uri uri = new Uri(String.Format("tcp://{0}:{1}/Collection", address, port));
                ServiceCallProxy proxy = new ServiceCallProxy(typeof(ICollectionService), uri.ToString());
                ICollectionService ics = proxy.GetTransparentProxy() as ICollectionService;

                return ics;
            }
            catch (Exception ex)
            {
                LOG.Error("Caught exception contacting collection service.", ex);
                throw ex;
            }
        }

        public static List<Snapshot> SnapShotCollector(int monitoredSqlServerId, ICollectionService service, AnalysisConfiguration config, AnalysisCollectorType collectorType)
        {
            List<Snapshot> snapshots = new List<Snapshot>();
            LOG.InfoFormat("Collecting " + collectorType.ToString() + " snapshots");
            using (OnDemandCollectionContext<List<Snapshot>> context = new OnDemandCollectionContext<List<Snapshot>>(TimeSpan.FromSeconds(PredictiveAnalyticsConfiguration.GetPrescriptiveAnalysisSnapshotsInSeconds)))
            {
                service.GetPrescriptiveAnalysisSnapshots(monitoredSqlServerId, context, null, config, collectorType);
                snapshots = context.Wait();
            }
            int errorCount = GetErrorCount(snapshots);
            int successCount = snapshots.Count - errorCount;
            LOG.InfoFormat(
                "{0}  " + collectorType.ToString() + "  snapshots collected. {1} Succeeded, {2} failed",
                snapshots.Count.ToString(),
                successCount.ToString(),
                errorCount.ToString()
            );

            return snapshots;
        }

        public static PrescriptiveAnalyticsSnapshot GetPrescriptiveAnalysisSnapshots(int monitoredSqlServerId, AnalysisConfiguration config)
        {
            ICollectionService service = GetCollectionService();
            if (service == null)
                return new PrescriptiveAnalyticsSnapshot();

            try
            {
                List<Snapshot> databaseSnapshots = SnapShotCollector(monitoredSqlServerId, service, config, AnalysisCollectorType.DatabaseSpecific);
                List<Snapshot> startupSnapshots = SnapShotCollector(monitoredSqlServerId, service, config, AnalysisCollectorType.Startup);
                List<Snapshot> intervalSnapshots = new List<Snapshot>();
                intervalSnapshots.AddRange(SnapShotCollector(monitoredSqlServerId, service, config, AnalysisCollectorType.Interval));
                List<Snapshot> shutdownSnapshots = SnapShotCollector(monitoredSqlServerId, service, config, AnalysisCollectorType.Shutdown);

                List<PASnapshot> paStartupSnapshots = AutoMapper.Mapper.Map<List<PASnapshot>>(startupSnapshots);
                List<PASnapshot> paIntervalSnapshots = AutoMapper.Mapper.Map<List<PASnapshot>>(intervalSnapshots);
                List<PASnapshot> paShutdownSnapshots = AutoMapper.Mapper.Map<List<PASnapshot>>(shutdownSnapshots);
                List<PASnapshot> paDatabaseSnapshots = AutoMapper.Mapper.Map<List<PASnapshot>>(databaseSnapshots);

                return new PrescriptiveAnalyticsSnapshot(startupSnapshots, intervalSnapshots, shutdownSnapshots, databaseSnapshots);
            }
            catch (Exception ex)
            {
                LOG.Error("Caught exception in GetPrescriptiveAnalysisSnapshots", ex);
                throw;
            }
        }

        public static PAPrescriptiveAnalyticsSnapshot GetPAPrescriptiveAnalysisSnapshots(int monitoredSqlServerId, AnalysisConfiguration config)
        {
            ICollectionService service = GetCollectionService();
            if (service == null)
                return new PAPrescriptiveAnalyticsSnapshot();

            try
            {
                List<Snapshot> databaseSnapshots = SnapShotCollector(monitoredSqlServerId, service, config, AnalysisCollectorType.DatabaseSpecific);
                List<Snapshot> startupSnapshots = SnapShotCollector(monitoredSqlServerId, service, config, AnalysisCollectorType.Startup);
                List<Snapshot> intervalSnapshots = new List<Snapshot>();
                intervalSnapshots.AddRange(SnapShotCollector(monitoredSqlServerId, service, config, AnalysisCollectorType.Interval));
                List<Snapshot> shutdownSnapshots = SnapShotCollector(monitoredSqlServerId, service, config, AnalysisCollectorType.Shutdown);


                PrescriptiveAnalyticsSnapshot analyticsSnapshot = new PrescriptiveAnalyticsSnapshot(startupSnapshots, intervalSnapshots, shutdownSnapshots, databaseSnapshots);
                PAPrescriptiveAnalyticsSnapshot paAnalyticsSnapshot = AutoMapper.Mapper.Map<PAPrescriptiveAnalyticsSnapshot>(analyticsSnapshot);

                if (paAnalyticsSnapshot != null)
                {
                    paAnalyticsSnapshot.IsDatabaseSnapshot = databaseSnapshots != null && databaseSnapshots.Count > 0;
                    paAnalyticsSnapshot.IsSnapshotsSnapshot = (startupSnapshots != null && startupSnapshots.Count > 0) ||
                                          (intervalSnapshots != null && intervalSnapshots.Count > 0) ||
                                          (shutdownSnapshots != null && shutdownSnapshots.Count > 0);
                }

                return paAnalyticsSnapshot;
            }
            catch (Exception ex)
            {
                LOG.Error("Caught exception in GetPrescriptiveAnalysisSnapshots", ex);
                throw;
            }
        }

        private static int GetErrorCount(List<Snapshot> lstSnapshot)
        {
            int errorCount = 0;
            foreach (Snapshot snp in lstSnapshot)
            {
                if (snp.Error != null)
                {
                    LOG.Info("Error in snapshot : " + snp.ToString());
                    errorCount++;
                }
            }
            return errorCount;
        }

        private static int GetErrorCount(List<PASnapshot> lstSnapshot)
        {
            int errorCount = 0;
            foreach (PASnapshot snp in lstSnapshot)
            {
                if (snp.Error != null)
                {
                    LOG.Info("Error in snapshot : " + snp.ToString());
                    errorCount++;
                }
            }
            return errorCount;
        }

        public static Dictionary<int, string> GetDatabasesForServer(int monitoredSqlServerId)
        {
            Dictionary<int, string> databseNameAndID = null;

            ICollectionService service = GetCollectionService();

            if (service == null)
                return new Dictionary<int, string>();

            try
            {
                using (OnDemandCollectionContext<Snapshot> context = new OnDemandCollectionContext<Snapshot>())
                {
                    service.GetDatabasesForServer(monitoredSqlServerId, context, null);
                    DatabaseNamesSnapshot snap = (DatabaseNamesSnapshot)context.Wait();
                    databseNameAndID = snap.Databases;
                    return databseNameAndID;
                }
            }
            catch (Exception ex)
            {
                LOG.Error("Caught exception in GetDatabasesForServer", ex);
                throw;
            }
        }

        public static string GetConnectionStringForServer(int monitoredSqlServerId)
        {
            string connStr = string.Empty;

            ICollectionService service = GetCollectionService();

            if (service == null)
                return connStr;

            try
            {
                connStr = service.GetConnectionStringForServer(monitoredSqlServerId);
                return connStr;
            }
            catch (Exception ex)
            {
                LOG.Error("Caught exception in GetDatabasesForServer", ex);
                throw;
            }
        }

        public static string GetMachineName(int monitoredSqlServerId)
        {
            string machineName = string.Empty;

            ICollectionService service = GetCollectionService();

            if (service == null)
                return machineName;

            try
            {
                using (OnDemandCollectionContext<Snapshot> context = new OnDemandCollectionContext<Snapshot>())
                {
                    service.GetMachineName(monitoredSqlServerId, context, null);
                    MachineNameSnapshot snap = (MachineNameSnapshot)context.Wait();
                    machineName = snap.MachineName;
                }
                return machineName;
            }
            catch (Exception ex)
            {
                LOG.Error("Caught exception in GetDatabasesForServer", ex);
                throw;
            }
        }

        public static MachineInfo GetMachineInfo(int monitoredSqlServerId)
        {
            ICollectionService service = GetCollectionService();

            if (service == null)
                return null;

            try
            {
                string connectionString = service.GetConnectionStringForServer(monitoredSqlServerId);
                int? cloudProvider = service.GetCloudProvider(monitoredSqlServerId);
                using (OnDemandCollectionContext<Snapshot> context = new OnDemandCollectionContext<Snapshot>())
                {
                    service.GetMachineName(monitoredSqlServerId, context, null);
                    MachineNameSnapshot snap = (MachineNameSnapshot)context.Wait();
                    string machineName = snap.MachineName;

                    return new MachineInfo {
                        ConnectionString = connectionString,
                        MachineName = machineName,
                        IsAzure = cloudProvider.HasValue && cloudProvider.Value == 2,
                        IsAmazon = cloudProvider.HasValue && cloudProvider.Value == 1,
                        IsLinux = false
                    };
                }
            }
            catch (Exception ex)
            {
                LOG.Error("Caught exception in GetMachineInfo", ex);
                throw;
            }
        }

        /// <summary>
        /// Get Cloud Provider for the SQL Server Id
        /// </summary>
        /// <param name="monitoredSqlServerId">Monitored Server Id</param>
        /// <returns></returns>
        public static int? GetCloudByInstanceId(int monitoredSqlServerId)
        {
            var service = GetCollectionService();

            if (service == null)
                return null;

            try
            {
                return service.GetCloudProvider(monitoredSqlServerId);
            }
            catch (Exception ex)
            {
                LOG.Error("Caught exception in GetMachineInfo", ex);
                throw;
            }
        }
    }



    public class MachineInfo
    {
        public string ConnectionString { get; set; }
        public string MachineName { get; set; }
        public bool IsAzure { get; set; }
        public bool IsAmazon { get; set; }
        public bool IsLinux { get; set; }
    }
}
