using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.Common.Snapshots
{
    [Serializable]
    public sealed class MirrorMonitoringDatabaseDetail: Database
    {
        #region fields
        
        public enum SafetyLevelEnum : int { Unknown = 0, Off, Full };

        private Guid _MirroringGuid;
        private string _ServerInstance;
        private string _partner;

        //start sp_dbmmonitorresults
        private string _DatabaseName;
        private MirroringMetrics _realtimeMetrics;
        //end sp_dbmmonitorresults
        
        private string _PartnerAddress;
        private string _WitnessAddress;
        private SafetyLevelEnum _SafetyLevel;
        #endregion
        #region constructors
        
        public MirrorMonitoringDatabaseDetail(string serverName, string dbName) : base(serverName, dbName)
        {
            _ServerInstance = serverName;
            _DatabaseName = dbName;
            _realtimeMetrics = new MirroringMetrics();
        }

        #endregion
        #region properties
        /// <summary>
        /// The name of the server that is hosting the local participant in the mirroring relationship
        /// </summary>
        public string ServerInstance
        {
            get { return _ServerInstance; }
            internal set { _ServerInstance = value; }
        }
        /// <summary>
        /// The name of the mirrored database
        /// </summary>
        public string DatabaseName
        {
            get { return _DatabaseName; }
            internal set { _DatabaseName = value; }
        }
        public MirroringMetrics CurrentMirroringMetrics
        {
            get { return _realtimeMetrics; }
            internal set { _realtimeMetrics = value; }
        }
        /// <summary>
        /// returns the name of the server hosting the current principal database for this mirroring relationship
        /// </summary>
        public string PrincipalName
        {
         get { return _realtimeMetrics.Role == MirroringMetrics.MirroringRoleEnum.Principal ? _ServerInstance : _partner; }
        }
        /// <summary>
        /// Returns the name of the server hosting the current mirror
        /// </summary>
        public string MirrorName
        {
            get { return _realtimeMetrics.Role == MirroringMetrics.MirroringRoleEnum.Principal ? _partner : _ServerInstance; }
        }
        public string MirrorAddress
        {
            get { return _realtimeMetrics.Role == MirroringMetrics.MirroringRoleEnum.Principal ? _PartnerAddress : "Unknown"; }
        }
        public string PrincipalAddress
        {
            get { return _realtimeMetrics.Role == MirroringMetrics.MirroringRoleEnum.Mirror ? _PartnerAddress : "Unknown"; }
        }

        public string OperatingMode
        {
            get
            {
                string mode = null;
                switch(_SafetyLevel)
                {
                    case SafetyLevelEnum.Full:
                        mode = _WitnessAddress.Trim().Length > 0 ? "High Availability with automatic failover (synchronous)" : "High Availability without automatic failover (synchronous)";
                        break;
                    case SafetyLevelEnum.Off:
                        mode = "High Performance (asynchronous)";
                        break;
                    case SafetyLevelEnum.Unknown:
                        break;
                }
                return mode;
            }
        }
        /// <summary>
        /// Server that is hosting the partner (remote)instance
        /// </summary>
        public string Partner
        {
            get { return _partner; }
            internal set { _partner = value; }
        }

        public Guid MirroringGuid
        {
            get { return _MirroringGuid; }
            internal set { _MirroringGuid = value; }
        }

        public string PartnerAddress
        {
            get { return _PartnerAddress; }
            internal set { _PartnerAddress = value; }
        }

        public string WitnessAddress
        {
            get { return _WitnessAddress==""?"No Witness":_WitnessAddress; }
            internal set { _WitnessAddress = value; }
        }
        /// <summary>
        /// Unknown, Off or Full
        /// </summary>
        public SafetyLevelEnum SafetyLevel
        {
            get { return _SafetyLevel; }
            set { _SafetyLevel = value; }
        }

        public string SafetyLevelDesc
        {
            get
            {
                switch (_SafetyLevel)
                {
                    case SafetyLevelEnum.Unknown:
                        return "Unknown";
                    case SafetyLevelEnum.Off:
                        return "Off";
                    case SafetyLevelEnum.Full:
                        return "Full";
                    default:
                        return "Unknown";
                }
            }
        }

        #endregion

    }
}
