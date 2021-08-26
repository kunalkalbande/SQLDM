//------------------------------------------------------------------------------
// <copyright file="ServerOverviewInterpreter.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Data.SqlClient;
using System.Globalization;
using System.Text.RegularExpressions;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Snapshots;
using BBS.TracerX;

namespace Idera.SQLdm.CollectionService.Probes.Sql
{
    using System;

    /// <summary>
    /// Enter a description for this class
    /// </summary>
    class ServerOverviewInterpreter
    {
        #region fields
        const int  cloud_id_azure= 2;
        #endregion

        #region constructors

        #endregion

        #region properties

        #endregion

        #region events

        #endregion

        #region methods

        internal static void InterpretForScheduledRefreshStepOne(SqlDataReader dataReader, ServerStatistics previousRefresh, ServerOverview refresh, Snapshot logTarget, BBS.TracerX.Logger LOG, DateTime? prevTimeStamp)
        {
            using (LOG.DebugCall("InterpretForScheduledRefreshStepOne"))
            {
                refresh.MaxConnections = ReadMaxServerConnections(dataReader, logTarget, LOG);
                refresh.ProcessorsUsed = CalculateProcessorsUsed(dataReader, refresh.ProcessorCount, logTarget, LOG);
                ReadThroughput(dataReader, previousRefresh, refresh.Statistics, logTarget, LOG, prevTimeStamp);
                ReadProcessDetails(dataReader, refresh, logTarget, LOG);
            }
        }

        internal static void InterpretForScheduledRefreshStepTwo(SqlDataReader dataReader, ServerStatistics previousRefresh, ServerOverview refresh, Memory memoryStatistics, Snapshot logTarget, BBS.TracerX.Logger LOG, ClusterCollectionSetting clusterCollectionSetting)
        {
            using (LOG.DebugCall("InterpretForScheduledRefreshStepTwo"))
            {
                ReadServerStatistics(dataReader, previousRefresh, refresh, logTarget, LOG);
               
                    ReadProcedureCache(dataReader, refresh, memoryStatistics, logTarget, LOG);
                    ReadIsClustered(dataReader, refresh, logTarget, LOG, clusterCollectionSetting);
                
                refresh.TotalLocks = ReadTotalLocks(dataReader, logTarget, LOG);
            }
        }

        internal static void InterpretForOnDemand(SqlDataReader dataReader, ServerStatistics previousRefresh, ServerOverview refresh, Snapshot logTarget, BBS.TracerX.Logger LOG, ClusterCollectionSetting clusterCollectionSetting, DateTime? prevTimeStamp,int? cloud_provider_id=null)
        {
            using (LOG.DebugCall("InterpretForOnDemand"))
            {
                ReadVersionInformation(dataReader, refresh, logTarget, LOG);
                ReadServerHostName(dataReader, refresh, logTarget, LOG);
                ReadEdition(dataReader, refresh, logTarget, LOG);
                ReadWindowsServicePack(dataReader, refresh, logTarget, LOG);
                refresh.ProcessorsUsed = CalculateProcessorsUsed(dataReader, refresh.ProcessorCount, logTarget, LOG);
                refresh.MaxConnections = ReadMaxServerConnections(dataReader, logTarget, LOG);
                // if (cloud_provider_id != cloud_id_azure)
                ReadThroughput(dataReader, previousRefresh, refresh.Statistics, logTarget, LOG, prevTimeStamp);
            
                    ReadProcedureCache(dataReader, refresh, null, logTarget, LOG);
                refresh.RunningSince = refresh.ServerStartupTime;
               
                ReadIsClustered(dataReader, refresh, logTarget, LOG, clusterCollectionSetting);

                //if (dataReader.FieldCount == 2)
                //{
                //    refresh.SqlServiceStatus = ServiceState.Running;
                //    ReadServiceStatusFromWMI(dataReader, refresh, logTarget, LOG);
                //}
                //else
                //{
                //    refresh.AgentServiceStatus = ProbeHelpers.ReadServiceState(dataReader, LOG);
                //    refresh.SqlServiceStatus = ServiceState.Running;
                //}

                refresh.TotalLocks = ReadTotalLocks(dataReader, logTarget, LOG);
           
                refresh.LoginConfiguration = ReadLoginConfiguration(dataReader, logTarget, LOG);
                ReadProcessDetails(dataReader, refresh, logTarget, LOG);
                ProbeHelpers.ReadLockStatistics(dataReader, logTarget, LOG, refresh.LockCounters, null);
                ReadServerStatistics(dataReader, previousRefresh, refresh, logTarget, LOG);
                //refresh.OpenTransactions = ReadOpenTransactions(dataReader, logTarget, LOG);
                if (dataReader.Read())
                {
                    ReadDatabaseSummary(dataReader, refresh.DatabaseSummary, refresh, LOG);
                    //try
                    //{
                        //refresh.DatabaseSummary.LogFileSpaceUsed = ReadUsedLogSize(dataReader, refresh, LOG);
                        refresh.DatabaseSummary.LogFileSpaceUsed.Bytes = ReadUsedLogSize(dataReader, refresh, LOG).Bytes;
                        //dataReader.NextResult();
                        //if (dataReader.HasRows)
                        //{
                        //    refresh.FullTextServiceStatus = ProbeHelpers.ReadServiceState(dataReader, LOG);
                        //}
                    //}
                    //catch
                    //{
                    //    refresh.FullTextServiceStatus = ServiceState.NotInstalled;
                    //}
                }
            }
        }

        /// <summary>
        ///  if user has not sysAdmin permission we have have specific result from SQLDataReader 
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="previousRefresh"></param>
        /// <param name="refresh"></param>
        /// <param name="logTarget"></param>
        /// <param name="LOG"></param>
        /// <param name="clusterCollectionSetting"></param>
        /// <param name="prevTimeStamp"></param>
        internal static void InterpretForOnDemandNoSysAdmin(SqlDataReader dataReader,
            ServerStatistics previousRefresh, ServerOverview refresh, Snapshot logTarget, 
            BBS.TracerX.Logger LOG, ClusterCollectionSetting clusterCollectionSetting, DateTime? prevTimeStamp)
        {
            using (LOG.DebugCall("InterpretForOnDemand"))
            {
                refresh.RunningSince = refresh.ServerStartupTime;

                GetSQLEdition(dataReader, refresh, logTarget, LOG);
                GetIsClustered(dataReader, refresh, logTarget, LOG, clusterCollectionSetting);
                GetProcessorCount(dataReader, refresh, logTarget, LOG, clusterCollectionSetting);
                GetProcessorUsed(dataReader, refresh, logTarget, LOG, clusterCollectionSetting);
                GetServerHostName(dataReader, refresh, logTarget, LOG);
                GetServerHostOS(dataReader, refresh, logTarget, LOG);
                GetHostMemory(dataReader, refresh, logTarget, LOG);
                GetDatabaseSummary(dataReader, refresh.DatabaseSummary, refresh, LOG);
                GetDataSize(dataReader, refresh, logTarget, LOG);
                GetLogSize(dataReader, refresh, logTarget, LOG);
            }
        }

        //private static void ReadServiceStatusFromWMI(SqlDataReader dataReader, ServerOverview refresh, Snapshot logTarget, BBS.TracerX.Logger LOG)
        //{
        //    using (LOG.DebugCall("ReadServiceStatusFromWMI"))
        //    {
        //        try
        //        {
        //            int serviceNameControl = -1;
        //            string serviceStatusString = null;
        //            do
        //            {
        //                if (dataReader.Read())
        //                {
        //                    if (!dataReader.IsDBNull(0)) serviceNameControl = dataReader.GetInt32(0);
        //                    if (!dataReader.IsDBNull(1)) serviceStatusString = dataReader.GetString(1);

        //                    switch ((ServiceName)serviceNameControl)
        //                    {
        //                        case ServiceName.Agent:
        //                            refresh.AgentServiceStatus =
        //                                ProbeHelpers.GetServiceState(serviceStatusString);
        //                            break;
        //                        case ServiceName.DTC:
        //                            refresh.DtcServiceStatus = ProbeHelpers.GetServiceState(serviceStatusString);
        //                            break;
        //                        case ServiceName.FullTextSearch:
        //                            refresh.FullTextServiceStatus =
        //                                ProbeHelpers.GetServiceState(serviceStatusString);
        //                            break;
        //                        case ServiceName.SqlServer:
        //                            //refresh.SqlServiceStatus = ProbeHelpers.GetServiceState(serviceStatusString);
        //                            break;
        //                    }
        //                }
        //                dataReader.NextResult();
        //            } while (dataReader.FieldCount == 2);
        //        }
        //        catch (Exception exception)
        //        {
        //            ProbeHelpers.LogAndAttachToSnapshot(logTarget, LOG, "Read service status from WMI failed: {0}", exception, false);
        //        }
        //    }
        //}

        internal static void ReadWindowsServicePack(SqlDataReader dataReader, ServerOverview refresh, Snapshot logTarget, BBS.TracerX.Logger LOG)
        {
            using (LOG.DebugCall("ReadWindowsServicePack"))
            {
                try
                {
                    if (dataReader.Read())
                    {
                        if (!dataReader.IsDBNull(0))
                        {
                            Regex servicePackRegex = new Regex(@"(?<=:.Service.Pack\s+)[^\)]*(?=\))", RegexOptions.IgnoreCase);
                            string versionString = dataReader.GetString(0);
                            if (servicePackRegex.IsMatch(versionString))
                            {
                                refresh.WindowsVersion += " SP" + servicePackRegex.Matches(versionString)[0];
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(logTarget, LOG, "Read Windows service pack failed: {0}", exception, false);
                }
                finally
                {
                    dataReader.NextResult();
                }
            }
        }

        internal static void ReadServerHostName(SqlDataReader dataReader, ServerOverview refresh, Snapshot logTarget, BBS.TracerX.Logger LOG)
        {
            using (LOG.DebugCall("ReadRealServerName"))
            {
                try
                {
                    if (dataReader.Read())
                    {
                        if (!dataReader.IsDBNull(0)) refresh.ServerHostName = dataReader.GetString(0);
                    }
                }
                catch (Exception exception)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(logTarget, LOG, "Read server host name failed: {0}", exception, false);
                }
                finally
                {
                    dataReader.NextResult();
                }
            }
        }

        /// <summary>
        /// Get the server host name
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="refresh"></param>
        /// <param name="logTarget"></param>
        /// <param name="LOG"></param>
        internal static void GetServerHostName(SqlDataReader dataReader, ServerOverview refresh, Snapshot logTarget,
            BBS.TracerX.Logger LOG)
        {
            using (LOG.DebugCall("GetRealServerName"))
            {
                try
                {
                    if (dataReader.Read())
                    {
                        Int32 ordinal = dataReader.GetOrdinal("Host");

                        if (!dataReader.IsDBNull(ordinal))
                        {
                            refresh.ServerHostName = dataReader.GetString(ordinal);
                        }
                    }
                }
                catch (Exception exception)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(logTarget, LOG, "Read server host name failed: {0}", 
                        exception, false);
                }
                finally
                {
                    dataReader.NextResult();
                }
            }
        }

        /// <summary>
        /// Get Host operating system version
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="refresh"></param>
        /// <param name="logTarget"></param>
        /// <param name="LOG"></param>
        internal static void GetServerHostOS(SqlDataReader dataReader, ServerOverview refresh, Snapshot logTarget,
            BBS.TracerX.Logger LOG)
        {
            using (LOG.DebugCall("GetSQLOSVersion"))
            {
                try
                {
                    if (dataReader.Read())
                    {
                        Int32 ordinal = dataReader.GetOrdinal("OSVersion");

                        if (!dataReader.IsDBNull(ordinal))
                        {
                            refresh.WindowsVersion = dataReader.GetString(ordinal) as string;
                        }
                    }
                }
                catch (Exception exception)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(logTarget, LOG, "Read server host operating system failed: {0}", 
                        exception, false);
                }
                finally
                {
                    dataReader.NextResult();
                }
            }
        }

        /// <summary>
        /// Get Host memory
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="refresh"></param>
        /// <param name="logTarget"></param>
        /// <param name="LOG"></param>
        internal static void GetHostMemory(SqlDataReader dataReader, ServerOverview refresh, Snapshot logTarget, 
            BBS.TracerX.Logger LOG)
        {
            using (LOG.DebugCall("GetHostMemory"))
            {
                try
                {
                    if (dataReader.Read())
                    {
                        Int32 ordinal = dataReader.GetOrdinal("Internal_Value");

                        if (!dataReader.IsDBNull(ordinal))
                        {
                            //refresh.OSMetricsStatistics.TotalPhysicalMemory = new FileSize();
                            refresh.OSMetricsStatistics.TotalPhysicalMemory.Kilobytes =(int) dataReader.GetInt32(ordinal);
                        }
                    }
                }
                catch (Exception exception)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(logTarget, LOG, "Read server host memory failed: {0}",
                        exception, false);
                }
                finally
                {
                    dataReader.NextResult();
                }
            }
        }

        /// <summary>
        /// Read Permissions to Enums - Provided the permissions name is same as enum name
        /// </summary>
        /// <typeparam name="T">Type of Enums</typeparam>
        /// <param name="dataReader">Datareader to read enum values</param>
        /// <param name="log">Logger</param>
        /// <param name="logTarget">Optional Snapshot to attach error for failures</param>
        /// <returns>Populated Enum Variable of Type T</returns>
        internal static T ReadPermissionsToEnum<T>(SqlDataReader dataReader, Logger log, Snapshot logTarget = null) where T : struct, IComparable, IFormattable, IConvertible
        {
            if (logTarget == null)
            {
                return ProbePermissionHelpers.ReadPermissionsToEnum<T>(dataReader, log);
            }

            // If logTarget is provided attach to the snapshot in case of Exception
            Exception failureException = null;
            var retVal = ProbePermissionHelpers.ReadPermissionsToEnum<T>(dataReader, log, out failureException);
            if(failureException != null)
            {
                ProbeHelpers.LogAndAttachToSnapshot(logTarget, log,
                    "Read" + typeof(T).Name + " Permissions failed: {0}", failureException, true);
                    
            }
            return retVal;
        }

        internal static void ReadEdition(SqlDataReader dataReader, ServerOverview refresh, Snapshot logTarget, BBS.TracerX.Logger LOG)
        {
            using (LOG.DebugCall("ReadEdition"))
            {
                try
                {
                    if (dataReader.Read())
                    {
                        if (!dataReader.IsDBNull(0)) refresh.SqlServerEdition = dataReader.GetString(0);
                    }
                }
                catch (Exception exception)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(logTarget, LOG, "Read server edition failed: {0}", exception, false);
                }
                finally
                {
                    dataReader.NextResult();
                }
            }
        }

        internal static void ReadDefaultInstancesPresent(SqlDataReader dataReader, ServerOverview refresh, Snapshot logTarget, BBS.TracerX.Logger LOG)
        {
            using (LOG.DebugCall("ReadDefaultInstancesPresent"))
            {
                try
                {
                    int count = 0;
                    while (dataReader.Read())
                    {
                        if (!dataReader.IsDBNull(0))
                        {
                            count++;
                        }                    
                    }
                    refresh.MultipleDefaultInstancesPresent = (count > 1);
                }
                catch (Exception exception)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(logTarget, LOG, "Reading default instances failed: {0}", exception, false);
                }
                finally
                {
                    dataReader.NextResult();
                }
            }
        }



        /// <summary>
        /// Get and set SQL edition for basic view
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="refresh"></param>
        /// <param name="logTarget"></param>
        /// <param name="LOG"></param>
        private static void GetSQLEdition(SqlDataReader dataReader, ServerOverview refresh,
            Snapshot logTarget, BBS.TracerX.Logger LOG)
        {
            using (LOG.DebugCall("GetSQLEdition"))
            {
                try
                {
                    if (dataReader.Read())
                    {
                        Int32 ordinal = dataReader.GetOrdinal("Edition");

                        if (!dataReader.IsDBNull(ordinal))
                        {
                            refresh.SqlServerEdition = dataReader.GetString(ordinal) as string;
                        }
                    }
                }
                catch (Exception exception)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(logTarget, LOG, "Read SQL server edition failed: {0}",
                        exception, false);
                }
                finally
                {
                    dataReader.NextResult();
                }
            }
        }

        /// <summary>
        /// Get DataBase size from user has public permission
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="refresh"></param>
        /// <param name="logTarget"></param>
        /// <param name="LOG"></param>
        private static void GetDataSize(SqlDataReader dataReader, ServerOverview refresh,
            Snapshot logTarget, BBS.TracerX.Logger LOG)
        {
            using (LOG.DebugCall("GetDataSize"))
            {
                try
                {
                    if (dataReader.Read())
                    {
                        Int32 ordinal = dataReader.GetOrdinal("dataSize");

                        if (!dataReader.IsDBNull(ordinal))
                        {
                            //refresh.DatabaseSummary.DataFileSpaceUsed = new FileSize();
                            refresh.DatabaseSummary.DataFileSpaceUsed.Megabytes =(decimal) dataReader.GetInt32(ordinal);
                        }
                    }
                }
                catch (Exception exception)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(logTarget, LOG, "Read SQL server Data Base size failed: {0}",
                        exception, false);
                }
                finally
                {
                    dataReader.NextResult();
                }
            }
        }

        private static void GetLogSize(SqlDataReader dataReader, ServerOverview refresh, Snapshot logTarget, 
            BBS.TracerX.Logger LOG)
        {
            using (LOG.DebugCall("GetLogSize"))
            {
                try
                {
                    if (dataReader.Read())
                    {
                        Int32 ordinal = dataReader.GetOrdinal("logSize");

                        if (!dataReader.IsDBNull(ordinal))
                        {
                            //refresh.DatabaseSummary.LogFileSpaceUsed = new FileSize();
                            refresh.DatabaseSummary.LogFileSpaceUsed.Megabytes = (decimal) dataReader.GetInt32(ordinal);
                        }
                    }
                }
                catch (Exception exception)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(logTarget, LOG, "Read SQL server Data Base Log size failed: {0}", 
                        exception, false);
                }
                finally
                {
                    dataReader.NextResult();
                }
            }
        }

        internal static Int64? ReadMaxServerConnections(SqlDataReader dataReader, Snapshot logTarget, BBS.TracerX.Logger LOG)
        {
            using (LOG.DebugCall("ReadMaxServerConnections"))
            {
                try
                {
                    //Check for valid data
                    if (!dataReader.Read())
                        return null;
                    if (dataReader.FieldCount != 2)
                        return null;
                    if (dataReader.IsDBNull(0) || dataReader.IsDBNull(1))
                        return null;

                    //Read both columns
                    Int64? syscurconfigsValue = Int64.Parse(dataReader.GetValue(0).ToString());
                    Int64? maxConnectionsValue = Int64.Parse(dataReader.GetValue(1).ToString());

                    //Return the first column if it has a value
                    if (syscurconfigsValue > 0)
                    {
                        return syscurconfigsValue;
                    }
                    else
                    {
                        //Return the second column if it has a value
                        if (maxConnectionsValue > 0)
                            return maxConnectionsValue;
                        else
                        {
                            //Return null if there is no max set
                            return null;
                        }
                    }
                }
                catch (Exception exception)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(logTarget, LOG, "Read server datetime failed: {0}", exception, false);
                    return null;
                }
                finally
                {
                    dataReader.NextResult();
                }
            }
        }

        internal static void ReadVersionInformation(SqlDataReader dataReader, ServerOverview refresh, Snapshot logTarget, BBS.TracerX.Logger LOG)
        {
            using (LOG.DebugCall("ReadVersionInformation"))
            {
                const string productVersionString = "ProductVersion";
                const string languageString = "Language";
                const string windowsVersionString = "WindowsVersion";
                const string processorCountString = "ProcessorCount";
                const string processorTypeString = "ProcessorType";
                const string physicalMemoryString = "PhysicalMemory";

                try
                {
                    while (dataReader.Read())
                    {
                        if (dataReader.FieldCount == 4)
                        {
                            if (!(dataReader.IsDBNull(1) & dataReader.IsDBNull(1) & dataReader.IsDBNull(2) & dataReader.IsDBNull(3)))
                            {
                                switch (dataReader.GetString(1))
                                {
                                    case productVersionString:
                                        refresh.ProductVersion = new ServerVersion(dataReader.GetString(3));
                                        break;
                                    case languageString:
                                        refresh.Language = dataReader.GetString(3);
                                        break;
                                    case windowsVersionString:
                                        refresh.WindowsVersion = ProbeHelpers.WindowsVersion(dataReader.GetString(3));
                                        break;
                                    case processorCountString:
                                        refresh.ProcessorCount = int.Parse(dataReader.GetString(3));
                                        break;
                                    case processorTypeString:
                                        refresh.ProcessorType = dataReader.GetValue(2).ToString();
                                        break;
                                    case physicalMemoryString:
                                        refresh.PhysicalMemory.Kilobytes = dataReader.GetInt32(2) * 1024;
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }

                    dataReader.NextResult();
                    dataReader.Read();
                    if (dataReader.FieldCount == 2 && !dataReader.IsDBNull(1))
                    {
                        refresh.WindowsVersion = dataReader.GetString(1);
                    }
                }
                catch (Exception exception)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(logTarget, LOG, "Read server version data failed: {0}", exception,
                                                        false);
                }
                finally
                {
                    dataReader.NextResult();
                }
            }
        }

        /// <summary>
        /// Calculates processors used
        /// </summary>
        /// <param name="dataReader">Dataset containing affinity mask data</param>
        /// <param name="processorCount">Total processor count</param>
        /// <param name="logTarget">Target snapshot for traceinfo logging</param>
        /// <param name="LOG">Error log destination</param>
        /// <returns></returns>
        internal static int? CalculateProcessorsUsed(SqlDataReader dataReader, int? processorCount, Snapshot logTarget, BBS.TracerX.Logger LOG)
        {
            using (LOG.DebugCall("CalculateProcessorsUsed"))
            {
                try
                {
                    //Check for valid data
                    if (!dataReader.Read())
                        return null;

                    //Check for valid data
                    if (dataReader.FieldCount >= 1)
                    {
                        if (!dataReader.IsDBNull(0))
                        {
                            long affinityMask = Math.Abs(dataReader.GetInt64(0));
                            long count = 0;

                            while (affinityMask != 0)
                            {
                                count += affinityMask & 1;
                                affinityMask = affinityMask >> 1;
                            }

                            if (processorCount.HasValue && (count > 0) && (processorCount > count))
                            {
                                return (int)count;
                            }
                            else
                            {
                                return processorCount;
                            }
                        }
                    }
                    return null;
                }
                catch (Exception exception)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(logTarget, LOG, "Read cpu affinity failed: {0}", exception, false);
                    return null;
                }
                finally
                {
                    dataReader.NextResult();
                }
            }
        }

        internal static void ReadThroughput(SqlDataReader dataReader, ServerStatistics previousRefresh, ServerStatistics refresh, Snapshot logTarget, BBS.TracerX.Logger LOG, DateTime? prevTimeStamp)
        {
            using (LOG.DebugCall("ReadThroughput"))
            {
                try
                {
                    //SQLDM-21167: removing SQL cpu from sys.dm_os_ring_buffers
                    /*if (dataReader.FieldCount == 2)
                    {
                        int totalCPU = 0;
                        int cpuCount = 0;

                        // Read 2005+ CPU data from ring buffers    
                        // Always read first even if it's a duplicate to prevent flatlining
                        if (dataReader.Read())
                        {
                            if (!dataReader.IsDBNull(1))
                            {
                                totalCPU += dataReader.GetInt32(1);
                                cpuCount++;
                            }
                        }
                        // Only read remaining ones if we have not already
                        while (dataReader.Read())
                        {
                            if (!dataReader.IsDBNull(0) && (!prevTimeStamp.HasValue || dataReader.GetDateTime(0) > prevTimeStamp.Value))
                            {
                                if (!dataReader.IsDBNull(1))
                                {
                                    totalCPU += dataReader.GetInt32(1);
                                }
                                cpuCount++;
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (cpuCount > 0)
                        {
                            refresh.CpuPercentage = totalCPU / cpuCount;
                        }

                        dataReader.NextResult();
                    }*/

                    //Check for valid data
                    if (dataReader.FieldCount == 1)
                    {
                        if (!dataReader.Read())
                            return;
                    
                        refresh.SQLProcessID = dataReader.GetInt32(0);
                    
                        dataReader.NextResult();
                    
                        //Check for valid data
                        if (!dataReader.Read())
                            return;
                    
                        refresh.LogicalProcessors = dataReader.GetInt32(0);

                        dataReader.NextResult();
                    }

                    //Check for valid data
                    if (!dataReader.Read())
                        return;

                    if (!dataReader.IsDBNull(0))
                    {
                        refresh.CpuBusyRaw = dataReader.GetInt32(0);
                    }

                    if (!dataReader.IsDBNull(1))
                    {
                        refresh.IdleTimeRaw = dataReader.GetInt32(1);
                    }

                    if (!dataReader.IsDBNull(2))
                    {
                        refresh.IoTimeRaw = dataReader.GetInt32(2);
                    }

                    if (!dataReader.IsDBNull(3))
                    {
                        refresh.PacketsReceivedRaw = dataReader.GetInt32(3);
                    }

                    if (!dataReader.IsDBNull(4))
                    {
                        refresh.PacketsSentRaw = dataReader.GetInt32(4);
                    }

                    if (!dataReader.IsDBNull(5))
                    {
                        refresh.PacketErrorsRaw = dataReader.GetInt32(5);
                    }

                    if (!dataReader.IsDBNull(6))
                    {
                        refresh.DiskReadRaw = dataReader.GetInt32(6);
                    }

                    if (!dataReader.IsDBNull(7))
                    {
                        refresh.DiskWriteRaw = dataReader.GetInt32(7);
                    }

                    if (!dataReader.IsDBNull(8))
                    {
                        refresh.DiskErrorsRaw = dataReader.GetInt32(8);
                    }

                    if (!dataReader.IsDBNull(9))
                    {
                        refresh.TotalConnectionsRaw = dataReader.GetInt32(9);
                    }

                    if (!dataReader.IsDBNull(10))
                    {
                        refresh.TimeTicks = dataReader.GetInt32(10);
                    }

                }
                catch (Exception exception)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(logTarget, LOG, "Read throughput failed: {0}", exception, false);
                    return;
                }
                finally
                {
                    dataReader.NextResult();
                    CalculateThroughput(previousRefresh, refresh);
                }
            }
        }

        private static void CalculateThroughput(ServerStatistics previousRefresh, ServerStatistics refresh)
        {

            if (previousRefresh != null)
            {
                refresh.CpuBusyDelta = new CpuTime(ServerStatistics.CalculateCpuTime(previousRefresh, refresh), refresh.TimeTicks);
                refresh.IdleTimeDelta = new CpuTime(ServerStatistics.CalculateIdleTime(previousRefresh, refresh), refresh.TimeTicks);
                refresh.IoTimeDelta = new CpuTime(ServerStatistics.CalculateIoTime(previousRefresh, refresh), refresh.TimeTicks);

                Int64? TotalCPU = refresh.CpuBusyDelta.Ticks + refresh.IdleTimeDelta.Ticks +
                                  refresh.IoTimeDelta.Ticks;

                // Only use the @@cpu_busy on SQL 2000 or where the ring buffer didn't return data
                if (!refresh.CpuPercentage.HasValue && !refresh.LogicalProcessors.HasValue)
                    refresh.CpuPercentage = (refresh.CpuBusyDelta.Ticks * 100f) / TotalCPU;
                refresh.IdlePercentage = (refresh.IdleTimeDelta.Ticks * 100f) / TotalCPU;
                refresh.IoPercentage = (refresh.IoTimeDelta.Ticks * 100f) / TotalCPU;

                refresh.PacketsReceived = ServerStatistics.CalculatePacketsReceived(previousRefresh, refresh);
                refresh.PacketsSent = ServerStatistics.CalculatePacketsSent(previousRefresh, refresh);
                refresh.PacketErrors = ServerStatistics.CalculatePacketErrors(previousRefresh, refresh);
                refresh.DiskRead = ServerStatistics.CalculateDiskRead(previousRefresh, refresh);
                refresh.DiskWrite = ServerStatistics.CalculateDiskWrite(previousRefresh, refresh);
                refresh.DiskErrors = ServerStatistics.CalculateDiskErrors(previousRefresh, refresh);
                refresh.TotalConnections = ServerStatistics.CalculateTotalConnections(previousRefresh, refresh);
            }

        }

        internal static void ReadProcessDetails(SqlDataReader dataReader, ServerOverview refresh, Snapshot logTarget, BBS.TracerX.Logger LOG)
        {
            using (LOG.DebugCall("ReadProcessDetails"))
            {
                ReadProcessDetails(dataReader, refresh.SystemProcesses, logTarget, LOG);
            }
        }


        internal static void ReadProcessDetails(SqlDataReader dataReader, ServerSystemProcesses refresh, Snapshot logTarget, BBS.TracerX.Logger LOG)
        {
            using (LOG.DebugCall("ReadProcessDetails"))
            {
                try
                {
                    //Check for valid data
                    if (!dataReader.Read())
                        return;

                    //Check for valid data
                    if (dataReader.FieldCount != 9)
                        return;

                    if (!dataReader.IsDBNull(0))
                    {
                        if (refresh.SystemProcessesConsumingCpu == null)
                            refresh.SystemProcessesConsumingCpu = dataReader.GetInt32(0);
                        else
                            refresh.SystemProcessesConsumingCpu += dataReader.GetInt32(0);
                    }

                    if (!dataReader.IsDBNull(1))
                    {
                        if (refresh.UserProcessesConsumingCpu == null)
                            refresh.UserProcessesConsumingCpu = dataReader.GetInt32(1);
                        else
                            refresh.UserProcessesConsumingCpu += dataReader.GetInt32(1);
                    }

                    if (!dataReader.IsDBNull(2))
                    {
                        if (refresh.BlockedProcesses == null)
                            refresh.BlockedProcesses = dataReader.GetInt32(2);
                        else
                            refresh.BlockedProcesses += dataReader.GetInt32(2);
                    }
                    if (!dataReader.IsDBNull(3))
                    {
                        if (refresh.OpenTransactions == null)
                            refresh.OpenTransactions = dataReader.GetInt32(3);
                        else
                            refresh.OpenTransactions += dataReader.GetInt32(3);
                    }
                    if (!dataReader.IsDBNull(4))
                    {
                        if (refresh.CurrentUserProcesses == null)
                            refresh.CurrentUserProcesses = dataReader.GetInt32(4);
                        else
                            refresh.CurrentUserProcesses += dataReader.GetInt32(4);
                    }
                    if (!dataReader.IsDBNull(5))
                    {
                        if (refresh.CurrentSystemProcesses == null)
                            refresh.CurrentSystemProcesses = dataReader.GetInt32(5);
                        else
                            refresh.CurrentSystemProcesses += dataReader.GetInt32(5);
                    }
                    if (!dataReader.IsDBNull(6))
                    {
                        if (refresh.ComputersHoldingProcesses == null)
                            refresh.ComputersHoldingProcesses = dataReader.GetInt32(6);
                        else
                            refresh.ComputersHoldingProcesses += dataReader.GetInt32(6);
                    }
                    if (!dataReader.IsDBNull(7))
                    {
                        if (refresh.LeadBlockers == null)
                            refresh.LeadBlockers = dataReader.GetInt32(7);
                        else
                            refresh.LeadBlockers += dataReader.GetInt32(7);
                    }
                    if (!dataReader.IsDBNull(8))
                    {
                        if (refresh.ActiveProcesses == null)
                            refresh.ActiveProcesses = dataReader.GetInt32(8);
                        else
                            refresh.ActiveProcesses += dataReader.GetInt32(8);
                    }


                }
                catch (Exception exception)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(logTarget, LOG, "Read process details failed: {0}", exception, false);
                    return;
                }
                finally
                {
                    dataReader.NextResult();
                }
            }
        }




        internal static void ReadServerStatistics(SqlDataReader dataReader, ServerStatistics previousRefresh, ServerOverview refresh, Snapshot logTarget, BBS.TracerX.Logger LOG)
        {
            using (LOG.DebugCall("ReadServerStatistics"))
            {
                try
                {

                    while (dataReader.Read())
                    {
                        if (dataReader.FieldCount == 2)
                        {
                            if (!dataReader.IsDBNull(0) && !dataReader.IsDBNull(1))
                            {
                                switch (dataReader.GetString(0).Trim().ToLower())
                                {
                                    case "batch requests/sec":
                                        if(refresh.Statistics.BatchRequestsRaw==null)
                                           refresh.Statistics.BatchRequestsRaw = dataReader.GetInt64(1);
                                        else
                                           refresh.Statistics.BatchRequestsRaw += dataReader.GetInt64(1);
                                        break;
                                    case "buffer cache hit ratio":
                                        if(refresh.Statistics.BufferCacheHitRatio==null)
                                           refresh.Statistics.BufferCacheHitRatioRaw = dataReader.GetInt64(1);
                                        else
                                           refresh.Statistics.BufferCacheHitRatioRaw += dataReader.GetInt64(1);
                                        break;
                                    case "buffer cache hit ratio base":
                                        if(refresh.Statistics.BufferCacheHitRatioBaseRaw==null)
                                            refresh.Statistics.BufferCacheHitRatioBaseRaw = dataReader.GetInt64(1);
                                        else
                                            refresh.Statistics.BufferCacheHitRatioBaseRaw += dataReader.GetInt64(1);
                                        break;
                                    case "cache hit ratio":
                                        if(refresh.Statistics.CacheHitRatio==null)
                                           refresh.Statistics.CacheHitRatioRaw = dataReader.GetInt64(1);
                                        else
                                            refresh.Statistics.CacheHitRatioRaw += dataReader.GetInt64(1);
                                        break;
                                    case "cache hit ratio base":
                                        if (refresh.Statistics.CacheHitRatioBaseRaw == null)
                                          refresh.Statistics.CacheHitRatioBaseRaw = dataReader.GetInt64(1);
                                        else
                                            refresh.Statistics.CacheHitRatioBaseRaw += dataReader.GetInt64(1);
                                        break;
                                    case "checkpoint pages/sec":
                                        if(refresh.Statistics.CheckpointPagesRaw==null)
                                        refresh.Statistics.CheckpointPagesRaw = dataReader.GetInt64(1);
                                        else
                                            refresh.Statistics.CheckpointPagesRaw += dataReader.GetInt64(1);
                                        break;
                                    case "full scans/sec":
                                        if(refresh.Statistics.FullScans==null)
                                        refresh.Statistics.FullScansRaw = dataReader.GetInt64(1);
                                        else
                                            refresh.Statistics.FullScansRaw += dataReader.GetInt64(1);
                                        break;
                                    case "lazy writes/sec":
                                        if(refresh.Statistics.LazyWritesRaw==null)
                                        refresh.Statistics.LazyWritesRaw = dataReader.GetInt64(1);
                                        else
                                            refresh.Statistics.LazyWritesRaw += dataReader.GetInt64(1);
                                        break;
                                    case "lock waits/sec":
                                        if(refresh.Statistics.LockWaitsRaw==null)
                                        refresh.Statistics.LockWaitsRaw = dataReader.GetInt64(1);
                                        else
                                            refresh.Statistics.LockWaitsRaw += dataReader.GetInt64(1);
                                        break;
                                    case "log flushes/sec":
                                        if(refresh.Statistics.LogFlushesRaw==null)
                                        refresh.Statistics.LogFlushesRaw = dataReader.GetInt64(1);
                                        else
                                            refresh.Statistics.LogFlushesRaw += dataReader.GetInt64(1);
                                        break;
                                    case "page life expectancy":
                                        if(refresh.Statistics.PageLifeExpectancy==null)
                                        refresh.Statistics.PageLifeExpectancy = TimeSpan.FromSeconds(dataReader.GetInt64(1));
                                        else
                                            refresh.Statistics.PageLifeExpectancy += TimeSpan.FromSeconds(dataReader.GetInt64(1));
                                        break;
                                    case "page lookups/sec":
                                        if(refresh.Statistics.PageLookupsRaw==null)
                                        refresh.Statistics.PageLookupsRaw = dataReader.GetInt64(1);
                                        else
                                            refresh.Statistics.PageLookupsRaw += dataReader.GetInt64(1);
                                        break;
                                    case "page reads/sec":
                                        if(refresh.Statistics.PageReadsRaw==null)
                                        refresh.Statistics.PageReadsRaw = dataReader.GetInt64(1);
                                        else
                                            refresh.Statistics.PageReadsRaw += dataReader.GetInt64(1);
                                        break;
                                    case "page splits/sec":
                                        if(refresh.Statistics.PageSplitsRaw==null)
                                        refresh.Statistics.PageSplitsRaw = dataReader.GetInt64(1);
                                        else
                                            refresh.Statistics.PageSplitsRaw += dataReader.GetInt64(1);
                                        break;
                                    case "page writes/sec":
                                        if(refresh.Statistics.PageWritesRaw==null)
                                        refresh.Statistics.PageWritesRaw = dataReader.GetInt64(1);
                                        else
                                            refresh.Statistics.PageWritesRaw += dataReader.GetInt64(1);
                                        break;
                                    case "readahead pages/sec":
                                        if(refresh.Statistics.ReadaheadPagesRaw==null)
                                        refresh.Statistics.ReadaheadPagesRaw = dataReader.GetInt64(1);
                                        else
                                            refresh.Statistics.ReadaheadPagesRaw += dataReader.GetInt64(1);
                                        break;
                                    case "sql compilations/sec":
                                        if (refresh.Statistics.SqlCompilationsRaw == null)
                                            refresh.Statistics.SqlCompilationsRaw = dataReader.GetInt64(1);
                                        else
                                            refresh.Statistics.SqlCompilationsRaw += dataReader.GetInt64(1);
                                        break;
                                    case "sql re-compilations/sec":
                                        if(refresh.Statistics.SqlRecompilationsRaw==null)
                                        refresh.Statistics.SqlRecompilationsRaw = dataReader.GetInt64(1);
                                        else
                                            refresh.Statistics.SqlRecompilationsRaw += dataReader.GetInt64(1);
                                        break;
                                    case "total pages":
                                        if(refresh.Statistics.BufferCacheSize.Kilobytes==null)
                                        refresh.Statistics.BufferCacheSize.Kilobytes = dataReader.GetInt64(1) * 8;
                                        else
                                            refresh.Statistics.BufferCacheSize.Kilobytes += dataReader.GetInt64(1) * 8;
                                        break;
                                    case "table lock escalations/sec":
                                        if(refresh.Statistics.TableLockEscalationsRaw==null)
                                        refresh.Statistics.TableLockEscalationsRaw = dataReader.GetInt64(1);
                                        else
                                            refresh.Statistics.TableLockEscalationsRaw += dataReader.GetInt64(1);
                                        break;
                                    case "target server memory(kb)":
                                    case "target server memory (kb)":
                                        if (refresh.TargetServerMemory.Kilobytes == null)
                                            refresh.TargetServerMemory.Kilobytes = dataReader.GetInt64(1);
                                        else
                                            refresh.TargetServerMemory.Kilobytes += dataReader.GetInt64(1);
                                        break;
                                    case "total server memory (kb)":
                                        if(refresh.TotalServerMemory.Kilobytes==null)
                                        refresh.TotalServerMemory.Kilobytes = dataReader.GetInt64(1);
                                        else
                                            refresh.TotalServerMemory.Kilobytes += dataReader.GetInt64(1);
                                        break;
                                    case "transactions/sec":
                                        if(refresh.Statistics.TransactionsRaw==null)
                                        refresh.Statistics.TransactionsRaw = dataReader.GetInt64(1);
                                        else
                                            refresh.Statistics.TransactionsRaw += dataReader.GetInt64(1);
                                        break;
                                    case "workfiles created/sec":
                                        if(refresh.Statistics.WorkfilesCreatedRaw==null)
                                            refresh.Statistics.WorkfilesCreatedRaw = dataReader.GetInt64(1);
                                        else
                                            refresh.Statistics.WorkfilesCreatedRaw += dataReader.GetInt64(1);
                                        break;
                                    case "worktables created/sec":
                                        if(refresh.Statistics.WorkfilesCreatedRaw==null)
                                        refresh.Statistics.WorktablesCreatedRaw = dataReader.GetInt64(1);
                                        else
                                        refresh.Statistics.WorktablesCreatedRaw += dataReader.GetInt64(1);
                                        break;
                                }
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(logTarget, LOG, "Read server statistics failed: {0}", exception, false);
                    return;
                }
                finally
                {
                    dataReader.NextResult();
                    CalculateServerStatistics(previousRefresh, refresh.Statistics);
                }
            }
        }

        internal static void CalculateServerStatistics(ServerStatistics previousRefresh, ServerStatistics refresh)
        {

            if (previousRefresh != null)
            {
                refresh.BatchRequests = ServerStatistics.CalculateBatchRequests(previousRefresh, refresh);
                refresh.CheckpointPages = ServerStatistics.CalculateCheckpointPages(previousRefresh, refresh);
                refresh.FullScans = ServerStatistics.CalculateFullScans(previousRefresh, refresh);
                refresh.LazyWrites = ServerStatistics.CalculateLazyWrites(previousRefresh, refresh);
                refresh.LockWaits = ServerStatistics.CalculateLockWaits(previousRefresh, refresh);
                refresh.LogFlushes = ServerStatistics.CalculateLogFlushes(previousRefresh, refresh);
                refresh.PageLookups = ServerStatistics.CalculatePageLookups(previousRefresh, refresh);
                refresh.PageReads = ServerStatistics.CalculatePageReads(previousRefresh, refresh);
                refresh.PageSplits = ServerStatistics.CalculatePageSplits(previousRefresh, refresh);
                refresh.PageWrites = ServerStatistics.CalculatePageWrites(previousRefresh, refresh);
                refresh.ReadaheadPages = ServerStatistics.CalculateReadaheadPages(previousRefresh, refresh);
                refresh.SqlCompilations = ServerStatistics.CalculateSqlCompilations(previousRefresh, refresh);
                refresh.SqlRecompilations = ServerStatistics.CalculateSqlRecompilations(previousRefresh, refresh);
                refresh.TableLockEscalations = ServerStatistics.CalculateTableLockEscalations(previousRefresh, refresh);
                refresh.WorkfilesCreated = ServerStatistics.CalculateWorkfilesCreated(previousRefresh, refresh);
                refresh.WorktablesCreated = ServerStatistics.CalculateWorktablesCreated(previousRefresh, refresh);
                refresh.Transactions = ServerStatistics.CalculateTransactions(previousRefresh, refresh);

            }

        }

        internal static void ReadProcedureCache(SqlDataReader dataReader, ServerOverview refresh, Memory MemoryStatistics, Snapshot logTarget, BBS.TracerX.Logger LOG)
        {
            using (LOG.DebugCall("ReadProcedureCache"))
            {
                try
                {
                    //Check for valid data
                    if (!dataReader.Read())
                        return;

                    //Check for valid data
                    if (dataReader.FieldCount != 6)
                    {
                        ProbeHelpers.LogAndAttachToSnapshot(logTarget, LOG, "Read procedure cache failed - fieldcount was incorrect", true);
                        return;
                    }

                    if (!dataReader.IsDBNull(4) && (!dataReader.IsDBNull(3)) && (!dataReader.IsDBNull(0)))
                    {
                        Int64 procCacheInUse = refresh.ProductVersion.Major >= 9 ? // SQL Server 2005/2008
                                                dataReader.GetInt64(1) : dataReader.GetInt32(4);

                        Int64 procCacheTotal = (refresh.ProductVersion.Major >= 9 // SQL Server 2005/2008
                                                            ? dataReader.GetInt64(0)
                                                            : dataReader.GetInt32(3));

                        refresh.ProcedureCacheSize.Kilobytes = procCacheTotal * 8;

                        if ((!dataReader.IsDBNull(3)) && (procCacheInUse != 0))
                        {
                            refresh.ProcedureCachePercentageUsed = 100d * (double)procCacheInUse /
                                                                                        (double)procCacheTotal;

                            if (MemoryStatistics != null)
                            {
                                
                                MemoryStatistics.FreeCachePages.Pages = procCacheTotal - procCacheInUse;

                                MemoryStatistics.ProcedureCachePercentageUsed = refresh.ProcedureCachePercentageUsed;
                            }
                        }
                    }

                }
                catch (Exception exception)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(logTarget, LOG, "Read procedure cache failed: {0}", exception, true);
                    return;
                }
                finally
                {
                    dataReader.NextResult();
                }
            }
        }

        internal static void ReadIsClustered(SqlDataReader dataReader, ServerOverview refresh, Snapshot logTarget, BBS.TracerX.Logger LOG, ClusterCollectionSetting clusterCollectionSetting)
        {
            using (LOG.DebugCall("ReadIsClustered"))
            {
                try
                {
                    //Check for valid data
                    if (!dataReader.Read())
                        return;

                    //Check for valid data
                    if (dataReader.FieldCount > 2)
                    {
                        ProbeHelpers.LogAndAttachToSnapshot(logTarget, LOG, "Read cluster settings failed - fieldcount was incorrect", true);
                        return;
                    }

                    if ((int)clusterCollectionSetting > (int)ClusterCollectionSetting.Default)
                    {
                        refresh.IsClustered = true;
                    }
                    else
                    {
                        if (!dataReader.IsDBNull(0))
                        {
                            refresh.IsClustered = dataReader.GetInt32(0) == 1 ? true : false;
                            if (refresh.ProductVersion.Major > 8 && refresh.IsClustered.HasValue &&
                                refresh.IsClustered.Value && !dataReader.IsDBNull(1))
                                refresh.ClusterNodeName = dataReader.GetString(1);
                        }
                    }

                    dataReader.NextResult();

                    if (dataReader.Read() &&
                        (clusterCollectionSetting == ClusterCollectionSetting.ForceClusteredWithRegread ||
                        (refresh.IsClustered.HasValue && refresh.IsClustered.Value && !dataReader.IsDBNull(1) && refresh.ClusterNodeName == null))
                        )
                    {
                        refresh.ClusterNodeName = dataReader.GetString(1);
                    }
                }
                catch (Exception exception)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(logTarget, LOG, "Read cluster settings failed: {0}", exception, true);
                    return;
                }
                finally
                {
                    dataReader.NextResult();
                }
            }
        }

        /// <summary>
        /// Get if data base is clustering
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="refresh"></param>
        /// <param name="logTarget"></param>
        /// <param name="LOG"></param>
        /// <param name="clusterCollectionSetting"></param>
        private static void GetIsClustered(SqlDataReader dataReader, ServerOverview refresh, Snapshot logTarget,
            BBS.TracerX.Logger LOG, ClusterCollectionSetting clusterCollectionSetting)
        {
            using (LOG.DebugCall("ReadIsClustered"))
            {
                try
                {
                    //Check for valid data
                    if (dataReader.Read())
                    {
                        Int32 ordinal = dataReader.GetOrdinal("IsClustered");

                        if (!dataReader.IsDBNull(ordinal))
                        {
                            refresh.IsClustered = dataReader.GetInt32(ordinal) == 1 ? true : false;
                        }
                    }
                }
                catch (Exception exception)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(logTarget, LOG, "Read cluster settings failed: {0}",
                        exception, false);
                    return;
                }
                finally
                {
                    dataReader.NextResult();
                }
            }
        }

        /// <summary>
        /// Get number of processor from server
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="refresh"></param>
        /// <param name="logTarget"></param>
        /// <param name="LOG"></param>
        /// <param name="clusterCollectionSetting"></param>
        private static void GetProcessorCount(SqlDataReader dataReader, ServerOverview refresh, Snapshot logTarget,
            BBS.TracerX.Logger LOG, ClusterCollectionSetting clusterCollectionSetting)
        {
            using (LOG.DebugCall("GetProcessorCount"))
            {
                try
                {
                    //Check for valid data
                    if (dataReader.Read())
                    {
                        Int32 ordinal = dataReader.GetOrdinal("Internal_Value");

                        if (!dataReader.IsDBNull(ordinal))
                        {
                            refresh.ProcessorCount = dataReader.GetInt32(ordinal);
                        }
                    }
                }
                catch (Exception exception)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(logTarget, LOG, "Read processor count settings failed: {0}", 
                        exception, false);
                    return;
                }
                finally
                {
                    dataReader.NextResult();
                }
            }
        }

        /// <summary>
        /// Get How many procesor are using 
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="refresh"></param>
        /// <param name="logTarget"></param>
        /// <param name="LOG"></param>
        /// <param name="clusterCollectionSetting"></param>
        private static void GetProcessorUsed(SqlDataReader dataReader, ServerOverview refresh, Snapshot logTarget,
            BBS.TracerX.Logger LOG, ClusterCollectionSetting clusterCollectionSetting)
        {
            using (LOG.DebugCall("GetProcessorUsed"))
            {
                try
                {
                    //Check for valid data
                    if (dataReader.Read())
                    {
                        Int32 ordinal = dataReader.GetOrdinal("Internal_Value");

                        if (!dataReader.IsDBNull(ordinal))
                        {
                            int var = dataReader.GetInt32(ordinal);
                            long affinityMask = Math.Abs((int)dataReader.GetInt32(ordinal));
                            long count = 0;

                            while (affinityMask != 0)
                            {
                                count += affinityMask & 1;
                                affinityMask = affinityMask >> 1;
                            }

                            if (refresh.ProcessorCount.HasValue && (count > 0) && (refresh.ProcessorCount.Value > count))
                            {
                                refresh.ProcessorsUsed = (int) count;
                            }
                            else
                            {
                                refresh.ProcessorsUsed = refresh.ProcessorCount;
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(logTarget, LOG, "Read processor count settings failed: {0}", exception,
                                                        false);
                    return;
                }
                finally
                {
                    dataReader.NextResult();
                }
            }
        }

        internal static ServerLoginConfiguration ReadLoginConfiguration(SqlDataReader dataReader, Snapshot logTarget, BBS.TracerX.Logger LOG)
        {
            using (LOG.DebugCall("ReadLoginConfiguration"))
            {
                try
                {
                    string loginMode = ReadLoginMode(dataReader, logTarget, LOG);
                    string auditLevel = ReadAuditLevel(dataReader, logTarget, LOG);

                    if (loginMode == null)
                        loginMode = "";
                    if (auditLevel == null)
                        auditLevel = "";
                    return new ServerLoginConfiguration(auditLevel, loginMode);
                }
                catch (Exception exception)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(logTarget, LOG, "Read login configuration failed: {0}", exception, false);
                    return null;
                }
            }
        }

        private static string ReadLoginMode(SqlDataReader dataReader, Snapshot logTarget, BBS.TracerX.Logger LOG)
        {
            using (LOG.DebugCall("ReadLoginMode"))
            {
                try
                {

                    //Check for valid data
                    if (!dataReader.Read() || dataReader.IsDBNull(1))
                        return null;

                    return dataReader.GetString(1);
                }
                catch (Exception exception)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(logTarget, LOG, "Read login mode failed: {0}", exception, false);
                    return null;
                }
                finally
                {
                    dataReader.NextResult();
                }
            }
        }

        private static string ReadAuditLevel(SqlDataReader dataReader, Snapshot logTarget, BBS.TracerX.Logger LOG)
        {
            using (LOG.DebugCall("ReadAuditLevel"))
            {
                try
                {
                    //Check for valid data
                    if (!dataReader.Read() || dataReader.IsDBNull(1))
                        return null;

                    return dataReader.GetString(1);
                }
                catch (Exception exception)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(logTarget, LOG, "Read audit level failed: {0}", exception, false);
                    return null;
                }
                finally
                {
                    dataReader.NextResult();
                }
            }
        }

        internal static void ReadDatabaseSummary(SqlDataReader dataReader, ServerDatabaseSummary refresh, Snapshot logTarget, BBS.TracerX.Logger LOG)
        {
            using (LOG.DebugCall("ReadDatabaseSummary"))
            {
                try
                {
                    //This data reader should be read before entering this method 
                    //dataReader.Read();

                    //Check for valid data
                    if (dataReader.FieldCount != 7)
                        return;
                    if (!dataReader.IsDBNull(0))
                    {
                        string aux = dataReader.GetString(0);
                    }

                    if (!dataReader.IsDBNull(1))
                    {
                        refresh.DatabaseCount = dataReader.GetInt32(1);
                    }

                    if (!dataReader.IsDBNull(2))
                    {
                        refresh.DataFileCount = dataReader.GetInt32(2);
                    }

                    if (!dataReader.IsDBNull(3))
                    {
                        refresh.LogFileCount = dataReader.GetInt32(3);
                    }

                    if (!dataReader.IsDBNull(4))
                    {
                        refresh.DataFileSpaceAllocated.Kilobytes = dataReader.GetDecimal(4);
                    }

                    if (!dataReader.IsDBNull(5))
                    {
                        refresh.LogFileSpaceAllocated.Kilobytes = dataReader.GetDecimal(5);
                    }

                    if (!dataReader.IsDBNull(6))
                    {
                        refresh.DataFileSpaceUsed.Kilobytes = dataReader.GetDecimal(6);
                    }

                }
                catch (Exception exception)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(logTarget, LOG, "Read database summary data failed: {0}", exception, false);
                    return;
                }
                finally
                {
                    dataReader.NextResult();
                }
            }

        }

        /// <summary>
        /// Get Total number of DB in the current instance
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="refresh"></param>
        /// <param name="logTarget"></param>
        /// <param name="LOG"></param>
        internal static void GetDatabaseSummary(SqlDataReader dataReader, ServerDatabaseSummary refresh, Snapshot logTarget,
            BBS.TracerX.Logger LOG)
        {
            using (LOG.DebugCall("GetDatabaseSummary"))
            {
                try
                {
                    //Check for valid data
                    if (dataReader.Read())
                    {
                        Int32 ordinal = dataReader.GetOrdinal("TotalDB");

                        if (!dataReader.IsDBNull(ordinal))
                        {
                            refresh.DatabaseCount = dataReader.GetInt32(ordinal);
                        }
                    }
                }
                catch (Exception exception)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(logTarget, LOG, "Read how many data bas has the instace settings failed: {0}",
                        exception,false);
                    return;
                }
                finally
                {
                    dataReader.NextResult();
                }
            }
        }


        internal static decimal? ReadTotalLocks(SqlDataReader dataReader, Snapshot logTarget, BBS.TracerX.Logger LOG)
        {
            using (LOG.DebugCall("ReadTotalLocks"))
            {
                try
                {
                    if (dataReader.Read() &&  !dataReader.IsDBNull(0))   //SQLDM 10.3 (Manali H): Fix for SQLDM-28699
                        return dataReader.GetDecimal(0);
                    else
                        return null;
                }
                catch (Exception exception)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(logTarget, LOG, "Read total locks failed: {0}", exception, false);
                    return null;
                }
                finally
                {
                    dataReader.NextResult();
                }
            }
        }

        internal static FileSize ReadUsedLogSize(SqlDataReader dataReader, Snapshot logTarget, BBS.TracerX.Logger LOG)
        {
            using (LOG.DebugCall("ReadUsedLogSize"))
            {
                FileSize usedLogSize = new FileSize();
                try
                {
                    //Do not offset the fields by default
                    int sql2012FieldOffset = 0;

                    while (dataReader.Read())
                    {
                        //Build 11.0.1750 RC0 return 5 fields. This is fullyexpected to change back to 4 fields.
                        //On this build the second column is "Principal Filegroup Name"
                        if (dataReader.FieldCount == 5 & dataReader.GetName(1).Equals("Principal Filegroup Name",StringComparison.InvariantCultureIgnoreCase))
                        {
                            sql2012FieldOffset = 1;
                            LOG.Debug("build 11.0.1750 specific code for interpreting log sizes");
                        }
                        else
                        //This is not 2012. The filedcount must be 4
                        //Check for valid data
                        if (dataReader.FieldCount != 4)
                        {
                            ProbeHelpers.LogAndAttachToSnapshot(logTarget, LOG, "Read log sizes failed - fieldcount was incorrect", true);
                            return usedLogSize;
                        }

                        if (!dataReader.IsDBNull(0) && !dataReader.IsDBNull(1) && !dataReader.IsDBNull(2))
                        {
                            string dbName = dataReader.GetString(0);

                            // Only attempt calculations for accessible databases
                            if (dbName != "mssqlsystemresource")
                            {

                                if (usedLogSize.Bytes.HasValue)
                                {
                                    usedLogSize.Megabytes += (decimal)(dataReader.GetFloat(1 + sql2012FieldOffset) * (dataReader.GetFloat(2 + sql2012FieldOffset) / 100));
                                }
                                else
                                {
                                    usedLogSize.Megabytes = (decimal)(dataReader.GetFloat(1 + sql2012FieldOffset) * (dataReader.GetFloat(2 + sql2012FieldOffset) / 100));
                                }
                            }
                        }
                    }
                    return usedLogSize;


                }
                catch (Exception exception)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(logTarget, LOG, "Read log sizes failed: {0}", exception, false);
                    return usedLogSize;
                }
            }
        }

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

    }
}
