using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

using Microsoft.Win32;
using Idera.SQLdm.ImportWizard.Properties;
using Idera.SQLdm.ImportWizard.Helpers;
using Wintellect.PowerCollections;

namespace Idera.SQLdm.ImportWizard.Objects
{
    public static class SQLdm4x
    {
        #region types

        private enum SQLdmVersion { DM40, DM41, DM45, DM46, Unsupported };

        public abstract class StatisticsReader
        {
            public abstract string[,] ReadServerStatistics(
                    string server,
                    DateTime beginDate,
                    DateTime endDate
                );

            public abstract string[,] ReadOSMetrics(
                    string server,
                    DateTime beginDate,
                    DateTime endDate
                );

            public abstract string[] GetDatabases(string server);

            public abstract string[,] ReadDatabaseStatistics(
                    string server,
                    string database,
                    DateTime beginDate,
                    DateTime endDate
                );

            public abstract string[,] ReadDatabaseSpaceStatistics(
                    string server,
                    string database,
                    DateTime beginDate,
                    DateTime endDate
                );

            public abstract string[] GetTables(
                    string server,
                    string database
                );

            public abstract string[,] ReadTableStatistics(
                    string server,
                    string database,
                    string table,
                    DateTime beginDate,
                    DateTime endDate
                );
        }

        private sealed class StatisticsReaderDM40 : StatisticsReader
        {
            #region fields

            private static object legacyStatisticsReaderLock = new object();
            private string path;

            #endregion

            #region ctors

            public StatisticsReaderDM40(string path)
            {
                if (path == null || path.Trim().Length == 0)
                {
                    throw new ArgumentNullException("path");
                }

                if (!Directory.Exists(path))
                {
                    throw new DirectoryNotFoundException("The statistics path provided does not exist.");
                }

                this.path = path;
            }

            #endregion

            #region methods

            private Interop.DMStatsReaderDLL_405.DMStatsReaderClass getLegacyStatisticsReader()
            {
                Interop.DMStatsReaderDLL_405.DMStatsReaderClass legacyStatisticsReader = new Interop.DMStatsReaderDLL_405.DMStatsReaderClass();
                legacyStatisticsReader.set_DMgrApplicationPath(ref path);
                return legacyStatisticsReader;
            }

            public override string[,] ReadServerStatistics(
                    string server,
                    DateTime beginDate,
                    DateTime endDate
                )
            {
                if (server == null || server.Trim().Length == 0)
                {
                    Log.Error("Server name not specified for reading server statistics");
                    throw new ArgumentNullException("server");
                }

                lock (legacyStatisticsReaderLock)
                {
                    Interop.DMStatsReaderDLL_405.DMStatsReaderClass legacyStatisticsReader = getLegacyStatisticsReader();

                    try
                    {
                        string[,] statistics = new string[,] { { String.Empty }, { String.Empty } };
                        Array safeArray = statistics;
                        string databaseName = String.Empty;
                        Interop.DMStatsReaderDLL_405.enumSvrViewOptions viewOptions = Interop.DMStatsReaderDLL_405.enumSvrViewOptions.All_Server_Options;
                        Interop.DMStatsReaderDLL_405.enumSvrResolution resolution = Interop.DMStatsReaderDLL_405.enumSvrResolution.constSvrTenMinute;
                        legacyStatisticsReader.ReadServerStats(ref server, ref databaseName, ref safeArray, ref viewOptions, ref resolution, ref beginDate, ref endDate);
                        statistics = safeArray as string[,];
                        safeArray = null;
                        return statistics;
                    }
                    catch (COMException ex)
                    {
                        // This exception would likely result from no valid data being available for the specified period
                        if (!isNoDataError(ex.ErrorCode))
                        {
                            Log.Warn("COM exception encountered when reading server statistics, ", ex);
                        }
                        return null;
                    }
                    finally
                    {
                        Marshal.ReleaseComObject(legacyStatisticsReader);
                    }
                }
            }

            public override string[,] ReadOSMetrics(
                    string server,
                    DateTime beginDate,
                    DateTime endDate
                )
            {
                // DM 4.0 does not support OS metrics.
                Log.Info("DM 4.0 does not support OS metrics");
                return null;
            }

            public override string[] GetDatabases(string server)
            {
                if (server == null || server.Trim().Length == 0)
                {
                    Log.Error("Server name not specified for getting database list");
                    throw new ArgumentException("An invalid server name was provided.");
                }

                lock (legacyStatisticsReaderLock)
                {
                    Interop.DMStatsReaderDLL_405.DMStatsReaderClass legacyStatisticsReader = getLegacyStatisticsReader();

                    try
                    {
                        string[] databases = new string[] { String.Empty };
                        Array databasesSafeArray = databases;
                        legacyStatisticsReader.GetTheDatabaseList(ref server, ref databasesSafeArray);
                        databases = databasesSafeArray as string[];
                        databasesSafeArray = null;
                        return databases;
                    }
                    catch (COMException ex)
                    {
                        // This exception would likely result because no databases are available
                        if (!isNoDataError(ex.ErrorCode))
                        {
                            Log.Warn("COM exception encountered when getting database list, ", ex);
                        }
                        return null;
                    }
                    finally
                    {
                        Marshal.ReleaseComObject(legacyStatisticsReader);
                    }
                }
            }

            public override string[,] ReadDatabaseStatistics(
                    string server,
                    string database,
                    DateTime beginDate,
                    DateTime endDate
                )
            {
                if (server == null || server.Trim().Length == 0)
                {
                    Log.Error("Server name not specified for reading database statistics");
                    throw new ArgumentNullException("server");
                }

                if (database == null || database.Length == 0)
                {
                    Log.Error("Database name not specified for reading database statistics");
                    throw new ArgumentNullException("database");
                }

                lock (legacyStatisticsReaderLock)
                {
                    Interop.DMStatsReaderDLL_405.DMStatsReaderClass legacyStatisticsReader = getLegacyStatisticsReader();

                    try
                    {
                        string[,] statistics = new string[,] { { String.Empty }, { String.Empty } };
                        Array safeArray = statistics;
                        Interop.DMStatsReaderDLL_405.enumDBCtrOptions viewOptions = Interop.DMStatsReaderDLL_405.enumDBCtrOptions.AllDBCtrOptions;
                        Interop.DMStatsReaderDLL_405.enumDBCtrResolution resolution = Interop.DMStatsReaderDLL_405.enumDBCtrResolution.constDBCtrTenMinute;
                        legacyStatisticsReader.ReadDBStats(ref server, ref database, ref safeArray, ref viewOptions, ref resolution, ref beginDate, ref endDate);
                        statistics = safeArray as string[,];
                        safeArray = null;
                        return statistics;
                    }
                    catch (COMException ex)
                    {
                        // This exception would likely result from no valid data being available for the specified period
                        if (!isNoDataError(ex.ErrorCode))
                        {
                            Log.Warn("COM exception encountered when reading database statistics, ", ex);
                        }
                        return null;
                    }
                    finally
                    {
                        Marshal.ReleaseComObject(legacyStatisticsReader);
                    }
                }
            }

            public override string[,] ReadDatabaseSpaceStatistics(
                    string server,
                    string database,
                    DateTime beginDate,
                    DateTime endDate
                )
            {
                if (server == null || server.Trim().Length == 0)
                {
                    Log.Error("Server name not specified for reading database space statistics");
                    throw new ArgumentNullException("server");
                }

                if (database == null || database.Length == 0)
                {
                    Log.Error("Database name not specified for reading database space statistics");
                    throw new ArgumentNullException("database");
                }

                lock (legacyStatisticsReaderLock)
                {
                    Interop.DMStatsReaderDLL_405.DMStatsReaderClass legacyStatisticsReader = getLegacyStatisticsReader();

                    try
                    {
                        string[,] statistics = new string[,] { { String.Empty }, { String.Empty } };
                        Array safeArray = statistics;
                        Interop.DMStatsReaderDLL_405.enumDBViewOptions viewOptions = Interop.DMStatsReaderDLL_405.enumDBViewOptions.All_DB_Options;
                        Interop.DMStatsReaderDLL_405.enumDBResolution resolution = Interop.DMStatsReaderDLL_405.enumDBResolution.DBDay;
                        legacyStatisticsReader.ReadGrowthStats(ref server, ref database, ref safeArray, ref viewOptions, ref resolution, ref beginDate, ref endDate);
                        statistics = safeArray as string[,];
                        safeArray = null;
                        return statistics;
                    }
                    catch (COMException ex)
                    {
                        // This exception would likely result from no valid data being available for the specified period
                        if (!isNoDataError(ex.ErrorCode))
                        {
                            Log.Warn("COM exception encountered when reading database space statistics, ", ex);
                        }
                        return null;
                    }
                    finally
                    {
                        Marshal.ReleaseComObject(legacyStatisticsReader);
                    }
                }
            }

            public override string[] GetTables(
                    string server,
                    string database
                )
            {
                if (server == null || server.Trim().Length == 0)
                {
                    Log.Error("Server name not specified for getting a list of tables");
                    throw new ArgumentException("An invalid server name was provided.");
                }

                if (database == null || database.Trim().Length == 0)
                {
                    Log.Error("Database name not specified for getting a list of tables");
                    throw new ArgumentException("An invalid database name was provided.");
                }

                lock (legacyStatisticsReaderLock)
                {
                    Interop.DMStatsReaderDLL_405.DMStatsReaderClass legacyStatisticsReader = getLegacyStatisticsReader();

                    try
                    {
                        string[] tables = new string[] { String.Empty };
                        Array tablesSafeArray = tables;
                        legacyStatisticsReader.GetTheTableList(ref server, ref database, ref tablesSafeArray);
                        tables = tablesSafeArray as string[];
                        tablesSafeArray = null;
                        return tables;
                    }
                    catch (COMException ex)
                    {
                        // This exception would likely result because no databases are available
                        if (!isNoDataError(ex.ErrorCode))
                        {
                            Log.Warn("COM exception encountered when getting list of tables, ", ex);
                        }
                        return null;
                    }
                    finally
                    {
                        Marshal.ReleaseComObject(legacyStatisticsReader);
                    }
                }
            }

            public override string[,] ReadTableStatistics(
                    string server,
                    string database,
                    string table,
                    DateTime beginDate,
                    DateTime endDate
                )
            {
                if (server == null || server.Trim().Length == 0)
                {
                    Log.Error("Server name not specified for reading table statistics");
                    throw new ArgumentNullException("server");
                }

                if (database == null || database.Length == 0)
                {
                    Log.Error("Database name not specified for reading table statistics");
                    throw new ArgumentNullException("database");
                }

                if (table == null || table.Length == 0)
                {
                    Log.Error("Table name not specified for reading table statistics");
                    throw new ArgumentNullException("table");
                }

                lock (legacyStatisticsReaderLock)
                {
                    Interop.DMStatsReaderDLL_405.DMStatsReaderClass legacyStatisticsReader = getLegacyStatisticsReader();

                    try
                    {
                        string[,] statistics = new string[,] { { String.Empty }, { String.Empty } };
                        Array safeArray = statistics;
                        Interop.DMStatsReaderDLL_405.enumTableViewOptions viewOptions = Interop.DMStatsReaderDLL_405.enumTableViewOptions.Table_All_Options;
                        Interop.DMStatsReaderDLL_405.enumTableResolution resolution = Interop.DMStatsReaderDLL_405.enumTableResolution.TableDay;
                        legacyStatisticsReader.ReadTableStats(ref server, ref database, ref table, ref safeArray, ref viewOptions, ref resolution, ref beginDate, ref endDate);
                        statistics = safeArray as string[,];
                        safeArray = null;
                        return statistics;
                    }
                    catch (COMException ex)
                    {
                        // This exception would likely result from no valid data being available for the specified period
                        if (!isNoDataError(ex.ErrorCode))
                        {
                            Log.Warn("COM exception encountered when reading table statistics, ", ex);
                        }
                        return null;
                    }
                    finally
                    {
                        Marshal.ReleaseComObject(legacyStatisticsReader);
                    }
                }
            }

            #endregion
        }

        private sealed class StatisticsReaderDM41 : StatisticsReader
        {
            #region fields

            private static object legacyStatisticsReaderLock = new object();
            private string path;

            #endregion

            #region ctors

            public StatisticsReaderDM41(string path)
            {
                if (path == null || path.Trim().Length == 0)
                {
                    throw new ArgumentNullException("path");
                }

                if (!Directory.Exists(path))
                {
                    throw new DirectoryNotFoundException("The statistics path provided does not exist.");
                }

                this.path = path;
            }

            #endregion

            #region methods

            private Interop.DMStatsReaderDLL_410.DMStatsReaderClass getLegacyStatisticsReader()
            {
                Interop.DMStatsReaderDLL_410.DMStatsReaderClass legacyStatisticsReader = new Interop.DMStatsReaderDLL_410.DMStatsReaderClass();
                legacyStatisticsReader.set_DMgrApplicationPath(ref path);
                return legacyStatisticsReader;
            }

            public override string[,] ReadServerStatistics(
                    string server,
                    DateTime beginDate,
                    DateTime endDate
                )
            {
                if (server == null || server.Trim().Length == 0)
                {
                    Log.Error("Server name not specified for reading server statistics");
                    throw new ArgumentNullException("server");
                }

                lock (legacyStatisticsReaderLock)
                {
                    Interop.DMStatsReaderDLL_410.DMStatsReaderClass legacyStatisticsReader = getLegacyStatisticsReader();

                    try
                    {
                        string[,] statistics = new string[,] { { String.Empty }, { String.Empty } };
                        Array safeArray = statistics;
                        string databaseName = String.Empty;
                        Interop.DMStatsReaderDLL_410.enumSvrViewOptions viewOptions = Interop.DMStatsReaderDLL_410.enumSvrViewOptions.All_Server_Options;
                        Interop.DMStatsReaderDLL_410.enumSvrResolution resolution = Interop.DMStatsReaderDLL_410.enumSvrResolution.constSvrTenMinute;
                        legacyStatisticsReader.ReadServerStats(ref server, ref databaseName, ref safeArray, ref viewOptions, ref resolution, ref beginDate, ref endDate);
                        statistics = safeArray as string[,];
                        safeArray = null;
                        return statistics;
                    }
                    catch (COMException ex)
                    {
                        // This exception would likely result from no valid data being available for the specified period
                        if (!isNoDataError(ex.ErrorCode))
                        {
                            Log.Warn("COM exception encountered when reading server statistics, ", ex);
                        }
                        return null;
                    }
                    finally
                    {
                        Marshal.ReleaseComObject(legacyStatisticsReader);
                    }
                }
            }

            public override string[,] ReadOSMetrics(
                    string server,
                    DateTime beginDate,
                    DateTime endDate
                )
            {
                // DM 4.1 does not support OS metrics.
                Log.Info("DM 4.1 does not support OS metrics");
                return null;
            }

            public override string[] GetDatabases(string server)
            {
                if (server == null || server.Trim().Length == 0)
                {
                    Log.Error("Server name not specified for getting database list");
                    throw new ArgumentException("An invalid server name was provided.");
                }

                lock (legacyStatisticsReaderLock)
                {
                    Interop.DMStatsReaderDLL_410.DMStatsReaderClass legacyStatisticsReader = getLegacyStatisticsReader();

                    try
                    {
                        string[] databases = new string[] { String.Empty };
                        Array databasesSafeArray = databases;
                        legacyStatisticsReader.GetTheDatabaseList(ref server, ref databasesSafeArray);
                        databases = databasesSafeArray as string[];
                        databasesSafeArray = null;
                        return databases;
                    }
                    catch (COMException ex)
                    {
                        // This exception would likely result because no databases are available
                        if (!isNoDataError(ex.ErrorCode))
                        {
                            Log.Warn("COM exception encountered when getting database list, ", ex);
                        }
                        return null;
                    }
                    finally
                    {
                        Marshal.ReleaseComObject(legacyStatisticsReader);
                    }
                }
            }

            public override string[,] ReadDatabaseStatistics(
                    string server,
                    string database,
                    DateTime beginDate,
                    DateTime endDate
                )
            {
                if (server == null || server.Trim().Length == 0)
                {
                    Log.Error("Server name not specified for reading database statistics");
                    throw new ArgumentNullException("server");
                }

                if (database == null || database.Length == 0)
                {
                    Log.Error("Database name not specified for reading database statistics");
                    throw new ArgumentNullException("database");
                }

                lock (legacyStatisticsReaderLock)
                {
                    Interop.DMStatsReaderDLL_410.DMStatsReaderClass legacyStatisticsReader = getLegacyStatisticsReader();

                    try
                    {
                        string[,] statistics = new string[,] { { String.Empty }, { String.Empty } };
                        Array safeArray = statistics;
                        Interop.DMStatsReaderDLL_410.enumDBCtrOptions viewOptions = Interop.DMStatsReaderDLL_410.enumDBCtrOptions.AllDBCtrOptions;
                        Interop.DMStatsReaderDLL_410.enumDBCtrResolution resolution = Interop.DMStatsReaderDLL_410.enumDBCtrResolution.constDBCtrTenMinute;
                        legacyStatisticsReader.ReadDBStats(ref server, ref database, ref safeArray, ref viewOptions, ref resolution, ref beginDate, ref endDate);
                        statistics = safeArray as string[,];
                        safeArray = null;
                        return statistics;
                    }
                    catch (COMException ex)
                    {
                        // This exception would likely result from no valid data being available for the specified period
                        if (!isNoDataError(ex.ErrorCode))
                        {
                            Log.Warn("COM exception encountered when reading database statistics, ", ex);
                        }
                        return null;
                    }
                    finally
                    {
                        Marshal.ReleaseComObject(legacyStatisticsReader);
                    }
                }
            }

            public override string[,] ReadDatabaseSpaceStatistics(
                    string server,
                    string database,
                    DateTime beginDate,
                    DateTime endDate
                )
            {
                if (server == null || server.Trim().Length == 0)
                {
                    Log.Error("Server name not spcecified for reading database space statistics");
                    throw new ArgumentNullException("server");
                }

                if (database == null || database.Length == 0)
                {
                    Log.Error("Database name not specified for reading database space statistics");
                    throw new ArgumentNullException("database");
                }

                lock (legacyStatisticsReaderLock)
                {
                    Interop.DMStatsReaderDLL_410.DMStatsReaderClass legacyStatisticsReader = getLegacyStatisticsReader();

                    try
                    {
                        string[,] statistics = new string[,] { { String.Empty }, { String.Empty } };
                        Array safeArray = statistics;
                        Interop.DMStatsReaderDLL_410.enumDBViewOptions viewOptions = Interop.DMStatsReaderDLL_410.enumDBViewOptions.All_DB_Options;
                        Interop.DMStatsReaderDLL_410.enumDBResolution resolution = Interop.DMStatsReaderDLL_410.enumDBResolution.DBDay;
                        legacyStatisticsReader.ReadGrowthStats(ref server, ref database, ref safeArray, ref viewOptions, ref resolution, ref beginDate, ref endDate);
                        statistics = safeArray as string[,];
                        safeArray = null;
                        return statistics;
                    }
                    catch (COMException ex)
                    {
                        // This exception would likely result from no valid data being available for the specified period
                        if (!isNoDataError(ex.ErrorCode))
                        {
                            Log.Warn("COM exception encountered when reading database space statistics, ", ex);
                        }
                        return null;
                    }
                    finally
                    {
                        Marshal.ReleaseComObject(legacyStatisticsReader);
                    }
                }
            }

            public override string[] GetTables(
                    string server,
                    string database
                )
            {
                if (server == null || server.Trim().Length == 0)
                {
                    Log.Error("Server name not specified for getting a list of tables");
                    throw new ArgumentException("An invalid server name was provided.");
                }

                if (database == null || database.Trim().Length == 0)
                {
                    Log.Error("Database name not specified for getting a list of tables");
                    throw new ArgumentException("An invalid database name was provided.");
                }

                lock (legacyStatisticsReaderLock)
                {
                    Interop.DMStatsReaderDLL_410.DMStatsReaderClass legacyStatisticsReader = getLegacyStatisticsReader();

                    try
                    {
                        string[] tables = new string[] { String.Empty };
                        Array tablesSafeArray = tables;
                        legacyStatisticsReader.GetTheTableList(ref server, ref database, ref tablesSafeArray);
                        tables = tablesSafeArray as string[];
                        tablesSafeArray = null;
                        return tables;
                    }
                    catch (COMException ex)
                    {
                        // This exception would likely result because no databases are available
                        if (!isNoDataError(ex.ErrorCode))
                        {
                            Log.Warn("COM exception encountered when getting list of tables, ", ex);
                        }
                        return null;
                    }
                    finally
                    {
                        Marshal.ReleaseComObject(legacyStatisticsReader);
                    }
                }
            }

            public override string[,] ReadTableStatistics(
                    string server,
                    string database,
                    string table,
                    DateTime beginDate,
                    DateTime endDate
                )
            {
                if (server == null || server.Trim().Length == 0)
                {
                    Log.Error("Server name not specified for reading table statistics");
                    throw new ArgumentNullException("server");
                }

                if (database == null || database.Length == 0)
                {
                    Log.Error("Database name not specified for reading table statistics");
                    throw new ArgumentNullException("database");
                }

                if (table == null || table.Length == 0)
                {
                    Log.Error("Table name not specified for reading table statistics");
                    throw new ArgumentNullException("table");
                }

                lock (legacyStatisticsReaderLock)
                {
                    Interop.DMStatsReaderDLL_410.DMStatsReaderClass legacyStatisticsReader = getLegacyStatisticsReader();

                    try
                    {
                        string[,] statistics = new string[,] { { String.Empty }, { String.Empty } };
                        Array safeArray = statistics;
                        Interop.DMStatsReaderDLL_410.enumTableViewOptions viewOptions = Interop.DMStatsReaderDLL_410.enumTableViewOptions.Table_All_Options;
                        Interop.DMStatsReaderDLL_410.enumTableResolution resolution = Interop.DMStatsReaderDLL_410.enumTableResolution.TableDay;
                        legacyStatisticsReader.ReadTableStats(ref server, ref database, ref table, ref safeArray, ref viewOptions, ref resolution, ref beginDate, ref endDate);
                        statistics = safeArray as string[,];
                        safeArray = null;
                        return statistics;
                    }
                    catch (COMException ex)
                    {
                        // This exception would likely result from no valid data being available for the specified period
                        if (!isNoDataError(ex.ErrorCode))
                        {
                            Log.Warn("COM exception encountered when reading table statistics, ", ex);
                        }
                        return null;
                    }
                    finally
                    {
                        Marshal.ReleaseComObject(legacyStatisticsReader);
                    }
                }
            }

            #endregion
        }

        private sealed class StatisticsReaderDM45 : StatisticsReader
        {
            #region fields

            private static object legacyStatisticsReaderLock = new object();
            private string path;

            #endregion

            #region ctors

            public StatisticsReaderDM45(string path)
            {
                if (path == null || path.Trim().Length == 0)
                {
                    throw new ArgumentNullException("path");
                }

                if (!Directory.Exists(path))
                {
                    throw new DirectoryNotFoundException("The statistics path provided does not exist.");
                }

                this.path = path;
            }

            #endregion

            #region methods

            private Interop.DMStatsReaderDLL_450.DMStatsReaderClass getLegacyStatisticsReader()
            {
                Interop.DMStatsReaderDLL_450.DMStatsReaderClass legacyStatisticsReader = new Interop.DMStatsReaderDLL_450.DMStatsReaderClass();
                legacyStatisticsReader.set_DMgrApplicationPath(ref path);
                return legacyStatisticsReader;
            }

            public override string[,] ReadServerStatistics(
                    string server,
                    DateTime beginDate,
                    DateTime endDate
                )
            {
                if (server == null || server.Trim().Length == 0)
                {
                    Log.Error("Server name not specified for reading server statistics");
                    throw new ArgumentNullException("server");
                }

                lock (legacyStatisticsReaderLock)
                {
                    Interop.DMStatsReaderDLL_450.DMStatsReaderClass legacyStatisticsReader = getLegacyStatisticsReader();

                    try
                    {
                        string[,] statistics = new string[,] { { String.Empty }, { String.Empty } };
                        Array safeArray = statistics;
                        string databaseName = String.Empty;
                        Interop.DMStatsReaderDLL_450.enumSvrViewOptions viewOptions = Interop.DMStatsReaderDLL_450.enumSvrViewOptions.All_Server_Options;
                        Interop.DMStatsReaderDLL_450.enumSvrResolution resolution = Interop.DMStatsReaderDLL_450.enumSvrResolution.constSvrTenMinute;
                        legacyStatisticsReader.ReadServerStats(ref server, ref databaseName, ref safeArray, ref viewOptions, ref resolution, ref beginDate, ref endDate);
                        statistics = safeArray as string[,];
                        safeArray = null;
                        return statistics;
                    }
                    catch (COMException ex)
                    {
                        // This exception would likely result from no valid data being available for the specified period
                        if (!isNoDataError(ex.ErrorCode))
                        {
                            Log.Warn("COM exception encountered when reading server statistics, ", ex);
                        }
                        return null;
                    }
                    finally
                    {
                        Marshal.ReleaseComObject(legacyStatisticsReader);
                    }
                }
            }

            public override string[,] ReadOSMetrics(
                    string server,
                    DateTime beginDate,
                    DateTime endDate
                )
            {
                if (server == null || server.Trim().Length == 0)
                {
                    Log.Error("Server name not specified for reading OS metrics");
                    throw new ArgumentNullException("server");
                }

                lock (legacyStatisticsReaderLock)
                {
                    Interop.DMStatsReaderDLL_450.DMStatsReaderClass legacyStatisticsReader = getLegacyStatisticsReader();

                    try
                    {
                        string[,] statistics = new string[,] { { String.Empty }, { String.Empty } };
                        Array safeArray = statistics;
                        string databaseName = String.Empty;
                        Interop.DMStatsReaderDLL_450.enumOSMetricsViewOptions viewOptions = Interop.DMStatsReaderDLL_450.enumOSMetricsViewOptions.OSMetrics_All_Options;
                        Interop.DMStatsReaderDLL_450.enumOSMetricsResolution resolution = Interop.DMStatsReaderDLL_450.enumOSMetricsResolution.constOSMetricsSynchronizationService;
                        legacyStatisticsReader.ReadOSMetricsStats(ref server, ref safeArray, ref viewOptions, ref resolution, ref beginDate, ref endDate);
                        statistics = safeArray as string[,];
                        safeArray = null;
                        return statistics;
                    }
                    catch (COMException ex)
                    {
                        // This exception would likely result from no valid data being available for the specified period
                        if (!isNoDataError(ex.ErrorCode))
                        {
                            Log.Warn("COM exception encountered when reading OS metrics, ", ex);
                        }
                        return null;
                    }
                    finally
                    {
                        Marshal.ReleaseComObject(legacyStatisticsReader);
                    }
                }
            }

            public override string[] GetDatabases(string server)
            {
                if (server == null || server.Trim().Length == 0)
                {
                    Log.Error("Server name not specified for getting a list of database");
                    throw new ArgumentException("An invalid server name was provided.");
                }

                lock (legacyStatisticsReaderLock)
                {
                    Interop.DMStatsReaderDLL_450.DMStatsReaderClass legacyStatisticsReader = getLegacyStatisticsReader();

                    try
                    {
                        string[] databases = new string[] { String.Empty };
                        Array databasesSafeArray = databases;
                        legacyStatisticsReader.GetTheDatabaseList(ref server, ref databasesSafeArray);
                        databases = databasesSafeArray as string[];
                        databasesSafeArray = null;
                        return databases;
                    }
                    catch (COMException ex)
                    {
                        // This exception would likely result because no databases are available
                        if (!isNoDataError(ex.ErrorCode))
                        {
                            Log.Warn("COM exception encountered when getting list of databases, ", ex);
                        }
                        return null;
                    }
                    finally
                    {
                        Marshal.ReleaseComObject(legacyStatisticsReader);
                    }
                }
            }

            public override string[,] ReadDatabaseStatistics(
                    string server,
                    string database,
                    DateTime beginDate,
                    DateTime endDate
                )
            {
                if (server == null || server.Trim().Length == 0)
                {
                    Log.Error("Server name not specified for reading database statistics");
                    throw new ArgumentNullException("server");
                }

                if (database == null || database.Length == 0)
                {
                    Log.Error("Database name not specified for reading database statistics");
                    throw new ArgumentNullException("database");
                }

                lock (legacyStatisticsReaderLock)
                {
                    Interop.DMStatsReaderDLL_450.DMStatsReaderClass legacyStatisticsReader = getLegacyStatisticsReader();

                    try
                    {
                        string[,] statistics = new string[,] { { String.Empty }, { String.Empty } };
                        Array safeArray = statistics;
                        Interop.DMStatsReaderDLL_450.enumDBCtrOptions viewOptions = Interop.DMStatsReaderDLL_450.enumDBCtrOptions.AllDBCtrOptions;
                        Interop.DMStatsReaderDLL_450.enumDBCtrResolution resolution = Interop.DMStatsReaderDLL_450.enumDBCtrResolution.constDBCtrTenMinute;
                        legacyStatisticsReader.ReadDBStats(ref server, ref database, ref safeArray, ref viewOptions, ref resolution, ref beginDate, ref endDate);
                        statistics = safeArray as string[,];
                        safeArray = null;
                        return statistics;
                    }
                    catch (COMException ex)
                    {
                        // This exception would likely result from no valid data being available for the specified period
                        if (!isNoDataError(ex.ErrorCode))
                        {
                            Log.Warn("COM exception encountered when reading database statistics, ", ex);
                        }
                        return null;
                    }
                    finally
                    {
                        Marshal.ReleaseComObject(legacyStatisticsReader);
                    }
                }
            }

            public override string[,] ReadDatabaseSpaceStatistics(
                    string server,
                    string database,
                    DateTime beginDate,
                    DateTime endDate
                )
            {
                if (server == null || server.Trim().Length == 0)
                {
                    Log.Error("Server name not specified for reading database space statistics");
                    throw new ArgumentNullException("server");
                }

                if (database == null || database.Length == 0)
                {
                    Log.Error("Database name not specified for reading database space statistics");
                    throw new ArgumentNullException("database");
                }

                lock (legacyStatisticsReaderLock)
                {
                    Interop.DMStatsReaderDLL_450.DMStatsReaderClass legacyStatisticsReader = getLegacyStatisticsReader();

                    try
                    {
                        string[,] statistics = new string[,] { { String.Empty }, { String.Empty } };
                        Array safeArray = statistics;
                        Interop.DMStatsReaderDLL_450.enumDBViewOptions viewOptions = Interop.DMStatsReaderDLL_450.enumDBViewOptions.All_DB_Options;
                        Interop.DMStatsReaderDLL_450.enumDBResolution resolution = Interop.DMStatsReaderDLL_450.enumDBResolution.DBDay;
                        legacyStatisticsReader.ReadGrowthStats(ref server, ref database, ref safeArray, ref viewOptions, ref resolution, ref beginDate, ref endDate);
                        statistics = safeArray as string[,];
                        safeArray = null;
                        return statistics;
                    }
                    catch (COMException ex)
                    {
                        // This exception would likely result from no valid data being available for the specified period
                        if (!isNoDataError(ex.ErrorCode))
                        {
                            Log.Warn("COM exception encountered when reading database space statistics, ", ex);
                        }
                        return null;
                    }
                    finally
                    {
                        Marshal.ReleaseComObject(legacyStatisticsReader);
                    }
                }
            }

            public override string[] GetTables(
                    string server,
                    string database
                )
            {
                if (server == null || server.Trim().Length == 0)
                {
                    Log.Error("Server name not specified for getting a list of tables");
                    throw new ArgumentException("An invalid server name was provided.");
                }

                if (database == null || database.Trim().Length == 0)
                {
                    Log.Error("Database name not specified for getting a list of tables");
                    throw new ArgumentException("An invalid database name was provided.");
                }

                lock (legacyStatisticsReaderLock)
                {
                    Interop.DMStatsReaderDLL_450.DMStatsReaderClass legacyStatisticsReader = getLegacyStatisticsReader();

                    try
                    {
                        string[] tables = new string[] { String.Empty };
                        Array tablesSafeArray = tables;
                        legacyStatisticsReader.GetTheTableList(ref server, ref database, ref tablesSafeArray);
                        tables = tablesSafeArray as string[];
                        tablesSafeArray = null;
                        return tables;
                    }
                    catch (COMException ex)
                    {
                        // This exception would likely result because no databases are available
                        if (!isNoDataError(ex.ErrorCode))
                        {
                            Log.Warn("COM exception encountered when getting list of tables, ", ex);
                        }
                        return null;
                    }
                    finally
                    {
                        Marshal.ReleaseComObject(legacyStatisticsReader);
                    }
                }
            }

            public override string[,] ReadTableStatistics(
                    string server,
                    string database,
                    string table,
                    DateTime beginDate,
                    DateTime endDate
                )
            {
                if (server == null || server.Trim().Length == 0)
                {
                    Log.Error("Server name not specified for reading table statistics");
                    throw new ArgumentNullException("server");
                }

                if (database == null || database.Length == 0)
                {
                    Log.Error("Database name not specified for reading table statistics");
                    throw new ArgumentNullException("database");
                }

                if (table == null || table.Length == 0)
                {
                    Log.Error("Table name not specified for reading table statistics");
                    throw new ArgumentNullException("table");
                }

                lock (legacyStatisticsReaderLock)
                {
                    Interop.DMStatsReaderDLL_450.DMStatsReaderClass legacyStatisticsReader = getLegacyStatisticsReader();

                    try
                    {
                        string[,] statistics = new string[,] { { String.Empty }, { String.Empty } };
                        Array safeArray = statistics;
                        Interop.DMStatsReaderDLL_450.enumTableViewOptions viewOptions = Interop.DMStatsReaderDLL_450.enumTableViewOptions.Table_All_Options;
                        Interop.DMStatsReaderDLL_450.enumTableResolution resolution = Interop.DMStatsReaderDLL_450.enumTableResolution.TableDay;
                        legacyStatisticsReader.ReadTableStats(ref server, ref database, ref table, ref safeArray, ref viewOptions, ref resolution, ref beginDate, ref endDate);
                        statistics = safeArray as string[,];
                        safeArray = null;
                        return statistics;
                    }
                    catch (COMException ex)
                    {
                        // This exception would likely result from no valid data being available for the specified period
                        if (!isNoDataError(ex.ErrorCode))
                        {
                            Log.Warn("COM exception encountered when reading table statistics, ", ex);
                        }
                        return null;
                    }
                    finally
                    {
                        Marshal.ReleaseComObject(legacyStatisticsReader);
                    }
                }
            }

            #endregion
        }

        private sealed class StatisticsReaderDM46 : StatisticsReader
        {
            #region fields

            private static object legacyStatisticsReaderLock = new object();
            private string path;

            #endregion

            #region ctors

            public StatisticsReaderDM46(string path)
            {
                if (path == null || path.Trim().Length == 0)
                {
                    throw new ArgumentNullException("path");
                }

                if (!Directory.Exists(path))
                {
                    throw new DirectoryNotFoundException("The statistics path provided does not exist.");
                }

                this.path = path;
            }

            #endregion

            #region methods

            private Interop.DMStatsReaderDLL_460.DMStatsReaderClass getLegacyStatisticsReader()
            {
                Interop.DMStatsReaderDLL_460.DMStatsReaderClass legacyStatisticsReader = new Interop.DMStatsReaderDLL_460.DMStatsReaderClass();
                legacyStatisticsReader.set_DMgrApplicationPath(ref path);
                return legacyStatisticsReader;
            }

            public override string[,] ReadServerStatistics(
                    string server,
                    DateTime beginDate,
                    DateTime endDate
                )
            {
                if (server == null || server.Trim().Length == 0)
                {
                    Log.Error("No server name specified for reading server statistics");
                    throw new ArgumentNullException("server");
                }

                lock (legacyStatisticsReaderLock)
                {
                    Interop.DMStatsReaderDLL_460.DMStatsReaderClass legacyStatisticsReader = getLegacyStatisticsReader();

                    try
                    {
                        string[,] statistics = new string[,] { { String.Empty }, { String.Empty } };
                        Array safeArray = statistics;
                        string databaseName = String.Empty;
                        Interop.DMStatsReaderDLL_460.enumSvrViewOptions viewOptions = Interop.DMStatsReaderDLL_460.enumSvrViewOptions.All_Server_Options;
                        Interop.DMStatsReaderDLL_460.enumSvrResolution resolution = Interop.DMStatsReaderDLL_460.enumSvrResolution.constSvrTenMinute;
                        legacyStatisticsReader.ReadServerStats(ref server, ref databaseName, ref safeArray, ref viewOptions, ref resolution, ref beginDate, ref endDate);
                        statistics = safeArray as string[,];
                        safeArray = null;
                        return statistics;
                    }
                    catch (COMException ex)
                    {
                        // This exception would likely result from no valid data being available for the specified period
                        if (!isNoDataError(ex.ErrorCode))
                        {
                            Log.Warn("COM exception encountered when reading server statistics, ", ex);
                        }
                        return null;
                    }
                    finally
                    {
                        Marshal.ReleaseComObject(legacyStatisticsReader);
                    }
                }
            }

            public override string[,] ReadOSMetrics(
                    string server,
                    DateTime beginDate,
                    DateTime endDate
                )
            {
                if (server == null || server.Trim().Length == 0)
                {
                    Log.Error("Server not specified for reading OS metrics");
                    throw new ArgumentNullException("server");
                }

                lock (legacyStatisticsReaderLock)
                {
                    Interop.DMStatsReaderDLL_460.DMStatsReaderClass legacyStatisticsReader = getLegacyStatisticsReader();

                    try
                    {
                        string[,] statistics = new string[,] { { String.Empty }, { String.Empty } };
                        Array safeArray = statistics;
                        string databaseName = String.Empty;
                        Interop.DMStatsReaderDLL_460.enumOSMetricsViewOptions viewOptions = Interop.DMStatsReaderDLL_460.enumOSMetricsViewOptions.OSMetrics_All_Options;
                        Interop.DMStatsReaderDLL_460.enumOSMetricsResolution resolution = Interop.DMStatsReaderDLL_460.enumOSMetricsResolution.constOSMetricsSynchronizationService;
                        legacyStatisticsReader.ReadOSMetricsStats(ref server, ref safeArray, ref viewOptions, ref resolution, ref beginDate, ref endDate);
                        statistics = safeArray as string[,];
                        safeArray = null;
                        return statistics;
                    }
                    catch (COMException ex)
                    {
                        // This exception would likely result from no valid data being available for the specified period
                        if (!isNoDataError(ex.ErrorCode))
                        {
                            Log.Warn("COM exception encountered when reading OS metrics, ", ex);
                        }
                        return null;
                    }
                    finally
                    {
                        Marshal.ReleaseComObject(legacyStatisticsReader);
                    }
                }
            }

            public override string[] GetDatabases(string server)
            {
                if (server == null || server.Trim().Length == 0)
                {
                    Log.Error("Server name was not specified for getting a list of databases");
                    throw new ArgumentException("An invalid server name was provided.");
                }

                lock (legacyStatisticsReaderLock)
                {
                    Interop.DMStatsReaderDLL_460.DMStatsReaderClass legacyStatisticsReader = getLegacyStatisticsReader();

                    try
                    {
                        string[] databases = new string[] { String.Empty };
                        Array databasesSafeArray = databases;
                        legacyStatisticsReader.GetTheDatabaseList(ref server, ref databasesSafeArray);
                        databases = databasesSafeArray as string[];
                        databasesSafeArray = null;
                        return databases;
                    }
                    catch (COMException ex)
                    {
                        // This exception would likely result because no databases are available
                        if (!isNoDataError(ex.ErrorCode))
                        {
                            Log.Warn("COM exception encountered when getting a list of databases, ", ex);
                        }
                        return null;
                    }
                    finally
                    {
                        Marshal.ReleaseComObject(legacyStatisticsReader);
                    }
                }
            }

            public override string[,] ReadDatabaseStatistics(
                    string server,
                    string database,
                    DateTime beginDate,
                    DateTime endDate
                )
            {
                if (server == null || server.Trim().Length == 0)
                {
                    Log.Error("Server not specified for reading database statistics");
                    throw new ArgumentNullException("server");
                }

                if (database == null || database.Length == 0)
                {
                    Log.Error("Database not specified for reading database statistics");
                    throw new ArgumentNullException("database");
                }

                lock (legacyStatisticsReaderLock)
                {
                    Interop.DMStatsReaderDLL_460.DMStatsReaderClass legacyStatisticsReader = getLegacyStatisticsReader();

                    try
                    {
                        string[,] statistics = new string[,] { { String.Empty }, { String.Empty } };
                        Array safeArray = statistics;
                        Interop.DMStatsReaderDLL_460.enumDBCtrOptions viewOptions = Interop.DMStatsReaderDLL_460.enumDBCtrOptions.AllDBCtrOptions;
                        Interop.DMStatsReaderDLL_460.enumDBCtrResolution resolution = Interop.DMStatsReaderDLL_460.enumDBCtrResolution.constDBCtrTenMinute;
                        legacyStatisticsReader.ReadDBStats(ref server, ref database, ref safeArray, ref viewOptions, ref resolution, ref beginDate, ref endDate);
                        statistics = safeArray as string[,];
                        safeArray = null;
                        return statistics;
                    }
                    catch (COMException ex)
                    {
                        // This exception would likely result from no valid data being available for the specified period
                        if (!isNoDataError(ex.ErrorCode))
                        {
                            Log.Warn("COM exception encountered when reading database statistics, ", ex);
                        }
                        return null;
                    }
                    finally
                    {
                        Marshal.ReleaseComObject(legacyStatisticsReader);
                    }
                }
            }

            public override string[,] ReadDatabaseSpaceStatistics(
                    string server,
                    string database,
                    DateTime beginDate,
                    DateTime endDate
                )
            {
                if (server == null || server.Trim().Length == 0)
                {
                    Log.Error("Server name not specified for reading database space statistics");
                    throw new ArgumentNullException("server");
                }

                if (database == null || database.Length == 0)
                {
                    Log.Error("Database name not specified for reading database space statistics");
                    throw new ArgumentNullException("database");
                }

                lock (legacyStatisticsReaderLock)
                {
                    Interop.DMStatsReaderDLL_460.DMStatsReaderClass legacyStatisticsReader = getLegacyStatisticsReader();

                    try
                    {
                        string[,] statistics = new string[,] { { String.Empty }, { String.Empty } };
                        Array safeArray = statistics;
                        Interop.DMStatsReaderDLL_460.enumDBViewOptions viewOptions = Interop.DMStatsReaderDLL_460.enumDBViewOptions.All_DB_Options;
                        Interop.DMStatsReaderDLL_460.enumDBResolution resolution = Interop.DMStatsReaderDLL_460.enumDBResolution.DBDay;
                        legacyStatisticsReader.ReadGrowthStats(ref server, ref database, ref safeArray, ref viewOptions, ref resolution, ref beginDate, ref endDate);
                        statistics = safeArray as string[,];
                        safeArray = null;
                        return statistics;
                    }
                    catch (COMException ex)
                    {
                        // This exception would likely result from no valid data being available for the specified period
                        if (!isNoDataError(ex.ErrorCode))
                        {
                            Log.Warn("COM exception encountered when reading database space statistics, ", ex);
                        }
                        return null;
                    }
                    finally
                    {
                        Marshal.ReleaseComObject(legacyStatisticsReader);
                    }
                }
            }

            public override string[] GetTables(
                    string server,
                    string database
                )
            {
                if (server == null || server.Trim().Length == 0)
                {
                    Log.Error("Server name not specified for getting list of tables");
                    throw new ArgumentException("An invalid server name was provided.");
                }

                if (database == null || database.Trim().Length == 0)
                {
                    Log.Error("Database name not specified for getting a list of tables");
                    throw new ArgumentException("An invalid database name was provided.");
                }

                lock (legacyStatisticsReaderLock)
                {
                    Interop.DMStatsReaderDLL_460.DMStatsReaderClass legacyStatisticsReader = getLegacyStatisticsReader();

                    try
                    {
                        string[] tables = new string[] { String.Empty };
                        Array tablesSafeArray = tables;
                        legacyStatisticsReader.GetTheTableList(ref server, ref database, ref tablesSafeArray);
                        tables = tablesSafeArray as string[];
                        tablesSafeArray = null;
                        return tables;
                    }
                    catch (COMException ex)
                    {
                        // This exception would likely result because no databases are available
                        if (!isNoDataError(ex.ErrorCode))
                        {
                            Log.Warn("COM exception encountered when getting a list of tables, ", ex);
                        }
                        return null;
                    }
                    finally
                    {
                        Marshal.ReleaseComObject(legacyStatisticsReader);
                    }
                }
            }

            public override string[,] ReadTableStatistics(
                    string server,
                    string database,
                    string table,
                    DateTime beginDate,
                    DateTime endDate
                )
            {
                if (server == null || server.Trim().Length == 0)
                {
                    Log.Error("Server name not specified for getting table statistics");
                    throw new ArgumentNullException("server");
                }

                if (database == null || database.Length == 0)
                {
                    Log.Error("Database name not specified for getting table statistics");
                    throw new ArgumentNullException("database");
                }

                if (table == null || table.Length == 0)
                {
                    Log.Error("Table name not specified for getting table statistics");
                    throw new ArgumentNullException("table");
                }

                lock (legacyStatisticsReaderLock)
                {
                    Interop.DMStatsReaderDLL_460.DMStatsReaderClass legacyStatisticsReader = getLegacyStatisticsReader();

                    try
                    {
                        string[,] statistics = new string[,] { { String.Empty }, { String.Empty } };
                        Array safeArray = statistics;
                        Interop.DMStatsReaderDLL_460.enumTableViewOptions viewOptions = Interop.DMStatsReaderDLL_460.enumTableViewOptions.Table_All_Options;
                        Interop.DMStatsReaderDLL_460.enumTableResolution resolution = Interop.DMStatsReaderDLL_460.enumTableResolution.TableDay;
                        legacyStatisticsReader.ReadTableStats(ref server, ref database, ref table, ref safeArray, ref viewOptions, ref resolution, ref beginDate, ref endDate);
                        statistics = safeArray as string[,];
                        safeArray = null;
                        return statistics;
                    }
                    catch (COMException ex)
                    {
                        // This exception would likely result from no valid data being available for the specified period
                        if (!isNoDataError(ex.ErrorCode))
                        {
                            Log.Warn("COM exception encountered when reading table statistics, ", ex);
                        }
                        return null;
                    }
                    finally
                    {
                        Marshal.ReleaseComObject(legacyStatisticsReader);
                    }
                }
            }

            #endregion
        }

        public class WorstPerformingReader
        {
            #region types

            public enum Fields {
                LocalCollectionDateTime,
                DatabaseUsed,
                CompletionTime,
                AverageDurationMS,
                CPUTimeMS,
                Reads,
                Writes,
                NTUserName,
                SQLUserName,
                ApplicationName,
                ClientComputerName,
                StatementType,
                StatementText,
                SIZE // Always at the end for array size purposes.
            };

            private enum FieldIndex
            {
                DatabaseUsed = 2,
                CompletionDate = 1,
                CompletionTime = 3,
                AverageDurationMS = 0,
                CPUTimeMS = 10,
                Reads = 8,
                Writes = 9,
                NTUserName = 4,
                SQLUserName = 7,
                ApplicationName = 6,
                ClientComputerName = 5,
                StatementText = 11
            }

            private enum StatementType
            {
                StoredProc,
                SingleStatement,
                Batch
            }

            private class WorstPerformingFileReader : IDisposable
            {
                #region fields

                private Stream _stream;
                private StreamReader _reader;
                private string[] _currentLine = null;

                #endregion

                #region ctors

                public WorstPerformingFileReader(FileInfo f)
                {
                    _stream = f.OpenRead();
                    _reader = new StreamReader(_stream);

                }

                #endregion

                #region properties

                internal string[] CurrentLine
                {
                    get
                    {
                        return _currentLine;
                    }
                }

                #endregion

                #region methods

                internal bool ReadLine(out bool isError)
                {
                    isError = false;

                    string line = null;
                    string templine = null;

                    if (0 < _reader.Peek())
                    {
                        line = _reader.ReadLine();

                        //Keep adding to a single line until the required number of single quotes has been found - this covers line wraps
                        while (CountSingleQuotes(line) < 16)
                        {
                            templine = _reader.ReadLine();

                            //If the readline is null, we've reached the end of the file
                            if (templine != null)
                                line += Environment.NewLine + templine;
                            else
                                return false; // End of file
                        }

                        //Split the line on commas not separated by quotes
                        Regex regex = new Regex(@",(?=(?:[^\""]*\""[^\""]*\"")*(?![^\""]*\""))", RegexOptions.IgnoreCase);
                        string[] unescaped = UnEscape(regex.Split(line));

                        if (unescaped != null)
                        {
                            _currentLine = unescaped;
                            return true;
                        }
                        else
                        {
                            isError = true;
                            return false;
                        }
                    }
                    else
                    {
                        return false; // No more data to read
                    }
                }

                private int CountSingleQuotes(string line)
                {
                    int count = 0;
                    int i = 0;

                    while (++i < line.Length)
                    {
                        if (line[i] == '"')
                        {
                            if (i < line.Length - 1 && line[i + 1] == '"')
                            {
                                if (count == 14 && i + 1 == line.Length - 1)
                                {
                                    return 16;
                                }
                                else
                                {
                                    i++;
                                    continue;
                                }
                            }
                            else
                            {
                                count++;
                                continue;
                            }
                        }
                    }

                    return count;
                }

                public void Dispose()
                {
                    if (_reader != null) _reader.Close();
                    else if (_stream != null) _stream.Close();
                    GC.SuppressFinalize(this);
                }

                private string[] UnEscape(string[] line)
                {
                    if (line.Length != 12)
                    {
                        return null;
                    }
                    else
                    {
                        line[1] = UnEscape(line[1]);
                        line[2] = UnEscape(line[2]);
                        line[3] = UnEscape(line[3]);
                        line[4] = UnEscape(line[4]);
                        line[5] = UnEscape(line[5]);
                        line[6] = UnEscape(line[6]);
                        line[7] = UnEscape(line[7]);
                        line[11] = UnEscape(line[11]);
                        return line;
                    }

                }

                /// <summary>
                /// Trim single quotes and convert double quotes to single quotes
                /// </summary>
                private string UnEscape(string field)
                {
                    field = field.Trim(new char[] { '"' });
                    field = field.Replace(@"""""", @"""");
                    return field;
                }

                #endregion
            }

            #endregion

            #region members

            private string m_DataDirectory;

            #endregion

            #region ctors

            public WorstPerformingReader(string dataDirectory)
            {
                Debug.Assert(!string.IsNullOrEmpty(dataDirectory));
                m_DataDirectory = dataDirectory;
            }

            #endregion

            #region methods

            public ArrayList ReadWorstPerforming ( 
                    string serverName, 
                    DateTime beginDate, 
                    DateTime endDate
                )
            {
                DateTime readDate = beginDate.Date;
                string directory = directoryPath(m_DataDirectory, serverName);
                FileInfo f;
                int[] types = new int[] { 12, 41, 43 };
                ArrayList lines = new ArrayList();

                while (readDate <= endDate)
                {
                    foreach (int type in types)
                    {
                        // Set statement type based on the type, which is the event code.
                        int typeVal = 0;
                        switch (type)
	                    {
                            case 12:
                                typeVal = (int)StatementType.Batch; // Batch
                                break;

                            case 41:
                                typeVal = (int)StatementType.SingleStatement; // Single Statement
                                break;

                            case 43:
                                typeVal = (int)StatementType.Batch; // Stored Proc
                                break;

		                    default:
                                Debug.Assert(false);
                                break;
	                    }

                        // If the file exists, read worst performing data.
                        f = new FileInfo(directory + fileName(readDate, type));
                        if (f.Exists)
                        {
                            using (WorstPerformingFileReader reader = new WorstPerformingFileReader(f))
                            {
                                bool isError = false;
                                while (reader.ReadLine(out isError))
                                {
                                    try
                                    {
                                        DateTime lineDate = dateFromFile(readDate, reader.CurrentLine[3]);

                                        if (lineDate > endDate)
                                        {
                                            break;
                                        }
                                        else if (lineDate >= beginDate)
                                        {
                                            lines.Add(convertLineForReading(typeVal, lineDate, reader.CurrentLine));
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Log.Error("Exception encountered when reading worst performing data entry from "
                                            + f.FullName + ", ", ex);
                                        throw ex;
                                    }
                                }

                                // Log if error.
                                if (isError)
                                {
                                    Log.Warn("Error encountered when reading worst performing data entry from "
                                        + f.FullName + " file, this data will be skipped.");
                                }
                            }
                        }
                    }

                    readDate = readDate.AddDays(1);
                }

                return lines;
            }

            private static string[] convertLineForReading(
                    int statementType,
                    DateTime lineDate, 
                    string[] currentLine
                )
            {
                string[] convertedLine = new string[(int)Fields.SIZE];
                convertedLine[(int)Fields.LocalCollectionDateTime] = lineDate.ToString();
                convertedLine[(int)Fields.DatabaseUsed] = currentLine[(int)FieldIndex.DatabaseUsed];
                convertedLine[(int)Fields.CompletionTime] = parseCompletionTime(currentLine[(int)FieldIndex.CompletionDate], currentLine[(int)FieldIndex.CompletionTime]);
                convertedLine[(int)Fields.AverageDurationMS] = currentLine[(int)FieldIndex.AverageDurationMS];
                convertedLine[(int)Fields.CPUTimeMS] = currentLine[(int)FieldIndex.CPUTimeMS];
                convertedLine[(int)Fields.Reads] = currentLine[(int)FieldIndex.Reads];
                convertedLine[(int)Fields.Writes] = currentLine[(int)FieldIndex.Writes];
                convertedLine[(int)Fields.NTUserName] = currentLine[(int)FieldIndex.NTUserName];
                convertedLine[(int)Fields.SQLUserName] = currentLine[(int)FieldIndex.SQLUserName];
                convertedLine[(int)Fields.ApplicationName] = currentLine[(int)FieldIndex.ApplicationName];
                convertedLine[(int)Fields.ClientComputerName] = currentLine[(int)FieldIndex.ClientComputerName];
                convertedLine[(int)Fields.StatementType] = statementType.ToString();
                convertedLine[(int)Fields.StatementText] = currentLine[(int)FieldIndex.StatementText];

                return convertedLine;
            }

            private static DateTime dateFromFile(
                    DateTime date, 
                    string timeStamp
                )
            {
                DateTime tempDate = Convert.ToDateTime("1/1/0001 " + timeStamp);
                TimeSpan ts = tempDate.TimeOfDay;
                return date.Add(ts);
            }

            private static string directoryPath(
                    string dataDirectory, 
                    string serverName
                )
            {
                return dataDirectory + @"\[" + serverName.Replace(@"\", @"~1`") + @"]\";
            }

            private static string fileName(
                    DateTime fileDate, 
                    int type
                )
            {
                return "dmtrace" + type + "_" + String.Format("{0:0000}", fileDate.Year) + String.Format("{0:000}", fileDate.DayOfYear) + ".sps";
            }

            private static string parseCompletionTime(
                    string date,
                    string timeStamp
                )
            {
                if (string.IsNullOrEmpty(date) || string.IsNullOrEmpty(timeStamp)
                    || date.Length != 4)
                {
                    throw new Exception("Input date and timestamp format invalid for parsing worst performing completion time");
                }

                // Get the month and day component of the date piece.
                int month = int.Parse(date.Substring(0, 2));
                int day = int.Parse(date.Substring(2, 2));

                // Get the month and day component of the current time.
                DateTime nowT = DateTime.Now;
                int nowMonth = nowT.Month;
                int nowDay = nowT.Day;

                // If the now month/day is less then input month/day, then
                // set year to last year, else set it to the current year.
                int year = nowT.Year;
                if ((nowMonth < month) || (nowMonth == month && nowDay < day))
                {
                    year -= 1;
                }

                // Combine the two time stamps.
                DateTime ymd = new DateTime(year, month, day);

                string ret = ymd.ToShortDateString() + " " + timeStamp;
                return ret;
            }

            #endregion
        }

        #endregion

        #region constants

        private const string DM_ROOT_REGISTRY_KEY = @"Software\Idera\DiagnosticManager";
        private const string DM_SERVERS_REGISTRY_KEY = DM_ROOT_REGISTRY_KEY + @"\Servers";
        private const string APPLICATION_PATH_REGISTRY_VALUE = "Application Path";
        private const string ALERT_LOG_FILE_REGISTRY_VALUE = "Alert Log File";
        private const string ALERT_LOG_FILE_PATH_REGISTRY_VALUE = "Alert Log Path";
        private const string DATA_FOLDER_PATH = @"Data";
        private const string DM_EXE_NAME_PATH = @"DiagnosticManager.exe";
        private const string DEFAULT_ALERT_LOG_FILE_NAME = "SPAlertMessages.log";
        private const string DM_VER_PREFIX = "4.";
        private const char VER_SEP_CHAR = '.';
        private const int MINOR_VER_INDEX = 1;

        private const int NO_DATA_ERROR = -2146778287;

        #endregion

        #region members

        private static bool m_IsInstalled = true;
        private static string m_RootPathInRegistry;
        private static string m_AlertPathInRegistry;
        private static string m_AlertFileInRegistry;
        private static string m_DataPath;
        private static string m_AlertFilePath;
        private static string m_VersionString;
        private static SQLdmVersion m_Version = SQLdmVersion.Unsupported;

        private static readonly BBS.TracerX.Logger Log = BBS.TracerX.Logger.GetLogger(typeof(Program));

        #endregion

        #region ctors

        static SQLdm4x()
        {
            // The members are initialized as part of their declaration.  The
            // important member initialization are m_IsInstalled is true, and 
            // m_Version is set to Unsupported.

            // Open the registry root of SQLdm 4.x, and get the SQLdm root path.
            using (RegistryKey rkSQLdm = Registry.LocalMachine.OpenSubKey(DM_ROOT_REGISTRY_KEY))
            {
                // Check if SQLdm key was opened.
                if (rkSQLdm == null)
                {
                    Log.Error("Failed to open SQLdm 4.x registry root, DM is not installed.");
                    m_IsInstalled = false;
                }

                // Get the root path for SQLdm from the registry.
                if (m_IsInstalled)
                {
                    try
                    {
                        // Make sure that the application path registry value type is a string.
                        if (rkSQLdm.GetValueKind(APPLICATION_PATH_REGISTRY_VALUE) != RegistryValueKind.String)
                        {
                            Log.Error("Application Path data type is not a string, DM is not installed.");
                            m_IsInstalled = false;
                        }

                        // Get the application path value and make sure its not null/empty and 
                        // root folder exists.
                        if (m_IsInstalled)
                        {
                            m_RootPathInRegistry = rkSQLdm.GetValue(APPLICATION_PATH_REGISTRY_VALUE).ToString();
                            if (!string.IsNullOrEmpty(m_RootPathInRegistry))
                            {
                                if (!Directory.Exists(m_RootPathInRegistry))
                                {
                                    Log.Error("SQLdm root folder does not exist (" + m_RootPathInRegistry + "), SQLdm is not installed.");
                                    m_IsInstalled = false;
                                }
                            }
                            else
                            {
                                Log.Error("Application Path registry value is null/empty, DM is not installed.");
                                m_IsInstalled = false;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Exception was encountered when reading the Application Path value.", ex);
                        m_IsInstalled = false;
                    }
                }

                // Get the alert log paths from registry.
                if (m_IsInstalled)
                {
                    // We try to get these values, if they are not available then we set them to empty.
                    try
                    {
                        string m_AlertPathInRegistry = rkSQLdm.GetValue(ALERT_LOG_FILE_PATH_REGISTRY_VALUE).ToString();
                        string m_AlertFileInRegistry = rkSQLdm.GetValue(ALERT_LOG_FILE_REGISTRY_VALUE).ToString();
                    }
                    catch
                    {
                        m_AlertFileInRegistry = m_AlertPathInRegistry = string.Empty;
                    }
                }
            }

            // Initialize the data path & alert log path.
            if (m_IsInstalled)
            {
                m_DataPath = Path.Combine(m_RootPathInRegistry, DATA_FOLDER_PATH);
                if (string.IsNullOrEmpty(m_AlertPathInRegistry) || string.IsNullOrEmpty(m_AlertFileInRegistry))
                {
                    m_AlertFilePath = Path.Combine(m_DataPath, DEFAULT_ALERT_LOG_FILE_NAME);
                }
                else
                {
                    m_AlertFilePath = Path.Combine(m_AlertPathInRegistry, m_AlertFileInRegistry);
                }
            }

            // Get the SQLdm version from the file version info of the exe.
            if (m_IsInstalled)
            {
                // Check if file exists.
                string dmExePath = Path.Combine(m_RootPathInRegistry,DM_EXE_NAME_PATH);
                if (!File.Exists(dmExePath))
                {
                    Log.Error("SQLdm binary DiagnosticManager.exe does not exist (" + dmExePath + "), SQLdm is not installed.");
                    m_IsInstalled = false;
                }

                // Get the product version from file version info.
                if (m_IsInstalled)
                {
                    m_VersionString = getProductVersionFromFileVersionInfo(dmExePath);
                    if (string.IsNullOrEmpty(m_VersionString))
                    {
                        Log.Error("Version information was not retrieved (" + dmExePath + "), SQLdm is not installed.");
                        m_IsInstalled = false;
                    }
                }
            }

            // Parse the version string to determine the version.
            if (m_IsInstalled)
            {
                if (m_VersionString.StartsWith(DM_VER_PREFIX))
                {
                    try
                    {
                        // Split the version string.
                        string[] verComponents = m_VersionString.Split(new char[] { VER_SEP_CHAR }, 3);

                        // The second version sub-component is the minor version,
                        // use this information to determine the version.
                        int minorVer = Convert.ToInt32(verComponents[MINOR_VER_INDEX]);
                        switch (minorVer)
                        {
                            case 0:
                                m_Version = SQLdmVersion.DM40;
                                break;

                            case 1:
                                m_Version = SQLdmVersion.DM41;
                                break;

                            case 5:
                                m_Version = SQLdmVersion.DM45;
                                break;

                            case 6:
                                m_Version = SQLdmVersion.DM46;
                                break;

                            default:
                                m_Version = SQLdmVersion.Unsupported;
                                Log.Error("SQLdm minor version " + minorVer.ToString() + " is not supported SQLdm version");
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        m_Version = SQLdmVersion.Unsupported;
                        Log.Error("Error in parsing the SQLdm minor version number from the version string <" + m_VersionString + ">.", ex);
                    }
                }
                else
                {
                    m_Version = SQLdmVersion.Unsupported;
                    Log.Error("SQLdm version is not supported (" + m_VersionString + ").");
                }
            }
        }

        #endregion

        #region properties

        public static string VersionString
        {
            get { return m_VersionString; }
        }

        #endregion

        #region methods

        public static bool Is4xInstalled(out string errMsg)
        {
            // Init return.
            errMsg = string.Empty;

            // If SQLdm is not installed, return false.
            if (!m_IsInstalled)
            {
                errMsg = Resources.SQLdmNotInstalledError;
                return false;
            }

            // If not supported version, return false.
            if (m_Version == SQLdmVersion.Unsupported)
            {
                errMsg = Resources.SQLdmVersionNotSupportedError;
                return false;
            }

            return true;
        }

        public static OrderedSet<string> RegisteredSQLServers()
        {
            Debug.Assert(m_Version != SQLdmVersion.Unsupported);

            // Create the return set.
            OrderedSet<string> set = new OrderedSet<string>();

            // Open the servers registry key.
            using (RegistryKey rkServers = Registry.LocalMachine.OpenSubKey(DM_SERVERS_REGISTRY_KEY))
            {
                Debug.Assert(rkServers != null);

                if (rkServers == null)
                {
                    throw (new Exception("Failed to open SQLdm 4.x servers registry key"));
                }

                // Enumerate each sub-key and get the server name.
                foreach (string s in rkServers.GetSubKeyNames())
                {
                    // If ~_ pattern in the string, replace with whack before 
                    // adding to the set.
                    if (s.Contains("~_"))
                    {
                        set.Add(s.Replace("~_", "\\").ToUpper());
                    }
                    else
                    {
                        set.Add(s.ToUpper());
                    }
                }
            }

            return set;
        }

        public static bool ReadData(
                string server,
                DateTime startUTC,
                DateTime endUTC,
                out SQLdm4xData data
            )
        {
            // Init return.
            bool isAnyError = false;
            data = null;

            try
            {
                // Get the statistics and worst performing reader objects.
                StatisticsReader srdr = getStatisticsReaderObject();
                WorstPerformingReader wprdr = getWorstPerformingReaderObject();

                // Read the data.
                if (srdr != null && wprdr != null)
                {
                    data = new SQLdm4xData(server, startUTC, endUTC);
                    isAnyError = data.UpdateData(srdr, wprdr);
                }
                else
                {
                    Log.Error("No reader objects exiting data read.");
                }
            }
            catch (Exception ex)
            {
                Log.Error("Exception was raised when reading data - ", ex);
                data = null;
                isAnyError = true;
            }

            return isAnyError;
        }

        private static unsafe string getProductVersionFromFileVersionInfo(string filePath)
        {
            string version = string.Empty;
            try
            {
                // Figure out how much version info there is:
                int handle = 0;
                int size = NativeMethods.GetFileVersionInfoSize(filePath, out handle);

                // Allocate buffer for reading version information.
                if (size != 0)
                {
                    byte[] buffer = new byte[size];
                    if (buffer != null)
                    {
                        // Read file version information.
                        if (NativeMethods.GetFileVersionInfo(filePath, handle, size, buffer))
                        {
                            // Get the locale info from the version info, the subBlock memory is deallocated
                            // when the buffer block memory goes out of scope.
                            short* subBlock = null;
                            uint len = 0;
                            if (NativeMethods.VerQueryValue(buffer, @"\VarFileInfo\Translation", out subBlock, out len))
                            {
                                // Get the ProductVersion value
                                string spv = @"\StringFileInfo\" + subBlock[0].ToString("X4") + subBlock[1].ToString("X4") + @"\ProductVersion";
                                if (!NativeMethods.VerQueryValue(buffer, spv, out version, out len))
                                {
                                    Log.Error("Failed to read version information string");
                                    version = string.Empty;
                                }
                            }
                            else
                            {
                                Log.Error("Failed to read locale info");
                            }
                        }
                        else
                        {
                            Log.Error("Failed to read the file version info into the allocated buffer");
                        }
                    }
                    else
                    {
                        Log.Error("Read buffer allocation failed for size " + size);
                    }
                }
                else
                {
                    Log.Error("Version information block size is 0");
                }
            }
            catch (Exception ex)
            {
                Log.Error("Exception raised when retrieving version information ", ex);
                version = string.Empty;
            }

            return version;
        }

        private static StatisticsReader getStatisticsReaderObject()
        {
            Debug.Assert(m_Version != SQLdmVersion.Unsupported);
            Debug.Assert(Directory.Exists(m_DataPath));

            // Create reader object if DM 4.x is installed, and the data folder exists.
            StatisticsReader rdrObj = null;
            if (m_Version != SQLdmVersion.Unsupported && Directory.Exists(m_DataPath))
            {
                // Create the reader object based on version.
                switch (m_Version)
                {
                    case SQLdmVersion.DM40:
                        rdrObj = new StatisticsReaderDM40(m_DataPath);
                        break;
                    case SQLdmVersion.DM41:
                        rdrObj = new StatisticsReaderDM41(m_DataPath);
                        break;
                    case SQLdmVersion.DM45:
                        rdrObj = new StatisticsReaderDM45(m_DataPath);
                        break;
                    case SQLdmVersion.DM46:
                        rdrObj = new StatisticsReaderDM46(m_DataPath);
                        break;
                    default:
                        Log.Error("The SQLdm version is not supported, no statistics reader object was created.");
                        Debug.Assert(false);
                        break;
                }
            }
            else
            {
                Log.Error("SQLdm not supported or data directory does not exist");
            }

            return rdrObj;
        }

        private static WorstPerformingReader getWorstPerformingReaderObject()
        {
            return new WorstPerformingReader(m_DataPath);
        }

        private static bool isNoDataError(int err)
        {
            return err == NO_DATA_ERROR;
        }

        #endregion
    }
}
