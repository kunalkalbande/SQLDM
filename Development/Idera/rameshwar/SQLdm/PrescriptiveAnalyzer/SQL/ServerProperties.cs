using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Idera.SQLdoctor.Common.Helpers;
using TracerX;
using Idera.SQLdoctor.Common.Configuration;

namespace Idera.SQLdoctor.AnalysisEngine.Snapshot.SQL
{
    internal class ServerProperties
    {
        private static Logger _logX = Logger.GetLogger("ServerProperties");

        public UInt64 MinutesRunning { get; private set; }
        public string WindowsVersion { get; private set; }
        public string ProductVersion { get; private set; }
        public bool C2AuditMode { get; private set; }
        public UInt32 DefaultFillFactor { get; private set; }
        public UInt64 IndexCreationMemory { get; private set; }
        public UInt64 MaxUserConnections { get; private set; }
        public UInt64 MaxWorkerThreads { get; private set; }
        public UInt64 UserOptions { get; private set; }
        public UInt64 AffinityMask { get; private set; }
        public UInt64 PhysicalMemory { get; private set; }
        public UInt64 MinServerMemory { get; private set; }
        public UInt64 MaxServerMemory { get; private set; }
        public UInt32 MaxDOP { get; private set; }
        public UInt32 ParallelismThreshold { get; private set; }
        public bool LightweightPooling { get; private set; }
        public bool AWEEnabled { get; private set; }
        public bool OptimizeForAdHocWorkloads { get; private set; }
        public UInt32 EncryptedConnections { get; private set; }
        public UInt32 ServerProcessID { get; private set; }
        public String TempDbRecoveryModel { get; private set; }
        public String Edition { get; private set; }
        public bool NoCountOn { get { return ((UserOptions & 512) == 512); } }
        public string SeManageVolumePrivilege { get; private set; }
        public string SQLServerServiceAccount { get; private set; }

        private ServerProperties() { }

        public ServerProperties(DataTable dt) 
        {
            if (null == dt) return;
            if (null == dt.Rows) return;
            if (null == dt.Columns) return;
            if (dt.Columns.Count < 2) return;
            using (_logX.InfoCall("ServerProperties(DataTable dt)"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    if (null == dr[0]) continue;
                    switch (dr[0].ToString().ToLower())
                    {
                        case ("windowsversion"): { WindowsVersion = DataHelper.ToString(dr, 1); break; }
                        case ("productversion"): { ProductVersion = DataHelper.ToString(dr, 1); break; }
                        case ("minutesrunning"): { MinutesRunning = DataHelper.ToUInt64(dr, 1); break; }
                        case ("c2auditmode"): { C2AuditMode = DataHelper.ToBoolean(dr, 1); break; }
                        case ("indexcreationmemory"): { IndexCreationMemory = DataHelper.ToUInt64(dr, 1); break; }
                        case ("useroptions"): { UserOptions = DataHelper.ToUInt64(dr, 1); break; }
                        case ("maxuserconnections"): { MaxUserConnections = DataHelper.ToUInt64(dr, 1); break; }
                        case ("maxworkerthreads"): { MaxWorkerThreads = DataHelper.ToUInt64(dr, 1); break; }
                        case ("affinitymask"): { AffinityMask = DataHelper.ToUInt64(dr, 1); break; }
                        case ("physicalmemory"): { PhysicalMemory = DataHelper.ToUInt64(dr, 1); break; }
                        case ("minservermemory"): { MinServerMemory = DataHelper.ToUInt64(dr, 1); break; }
                        case ("maxservermemory"): { MaxServerMemory = DataHelper.ToUInt64(dr, 1); break; }
                        case ("maxdop"): { MaxDOP = DataHelper.ToUInt32(dr, 1); break; }
                        case ("parallelismthreshold"): { ParallelismThreshold = DataHelper.ToUInt32(dr, 1); break; }
                        case ("lightweightpooling"): { LightweightPooling = DataHelper.ToBoolean(dr, 1); break; }
                        case ("aweenabled"): { AWEEnabled = DataHelper.ToBoolean(dr, 1); break; }
                        case ("optimizeforadhocworkloads"): { OptimizeForAdHocWorkloads = DataHelper.ToBoolean(dr, 1); break; }
                        case ("encryptedconnections"): { EncryptedConnections = DataHelper.ToUInt32(dr, 1); break; }
                        case ("serverprocessid"): { ServerProcessID = DataHelper.ToUInt32(dr, 1); break; }
                        case ("defaultfillfactor"): { DefaultFillFactor = DataHelper.ToUInt32(dr, 1); break; }
                        case ("tempdbrecoverymodel"): { TempDbRecoveryModel = DataHelper.ToString(dr, 1); break; }
                        case ("edition"): { Edition = DataHelper.ToString(dr, 1); break; }
                        case ("semanagevolumeprivilege"): { SeManageVolumePrivilege = DataHelper.ToString(dr, 1).TrimEnd(); break; }
                        case ("sqlserverserviceaccount"): { SQLServerServiceAccount = DataHelper.ToString(dr, 1).TrimEnd(); break; }
                        default: { _logX.Debug("Unknown Server Property: " + dr[0].ToString()); break; }
                    }
                    _logX.InfoFormat("{0}: {1}", DataHelper.ToString(dr, 0), DataHelper.ToString(dr, 1));
                }
            }
        }    
    }
}
