//------------------------------------------------------------------------------
// <copyright file="Lock.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using Idera.SQLdm.Common.Auditing;
using Wintellect.PowerCollections;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents a lock on an object in a database
    /// </summary>
    [Serializable]
    public sealed class Lock : ISerializable, IAuditable
    {

        #region fields

        private bool?           blocking;
        private int?            blocked_by;
        private string          command;
        private decimal?        instanceCount;         
        private string          database;
        private string          host;
        private LockMode?       mode;
        private string          objectName;
        private string          objectSchema;
        private string          application;
        private int?            spid;
        private LockStatus?     status;
        private LockType?       type;
        private string          user;
        private Guid            id;
        private TimeSpan        waitTime;
        
        #endregion

        #region constructors

        internal Lock()
        {
            blocking = null;
            blocked_by = null;
            command = null;
            instanceCount = null;
            database = null;
            host = null;
            mode = null;
            objectName = null;
            objectSchema = null;
            application = null;
            spid = null;
            status = null;
            type = null;
            user = null;
            id = Guid.NewGuid();
        }

        public Lock(SerializationInfo info, StreamingContext context)
        {
            blocking = (bool?)info.GetValue("blocking", typeof(bool?));
            blocked_by = (int?)info.GetValue("blocked_by", typeof(int?));
            command = info.GetString("command");
            instanceCount = (decimal?)info.GetValue("instanceCount", typeof(decimal?));
            database = info.GetString("database");
            host = info.GetString("host");
            mode = (LockMode?)info.GetValue("mode", typeof(LockMode?));
            objectName = info.GetString("objectName");
            objectSchema = info.GetString("objectSchema");
            application = info.GetString("application");
            spid = (int?)info.GetValue("spid", typeof(int?));
            status = (LockStatus?)info.GetValue("status", typeof(LockStatus?));
            type = (LockType?)info.GetValue("type", typeof(LockType?));
            user = info.GetString("user");
            id = (Guid)info.GetValue("id", typeof(Guid));
            waitTime = (TimeSpan)info.GetValue("waitTime", typeof(TimeSpan));
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets whether the lock is blocking
        /// </summary>
        public bool? Blocking
        {
            get { return blocking; }
            internal set { blocking = value; }
        }

        /// <summary>
        /// Spid of the session that is blocking this lock
        /// </summary>
        public int? Blocked_by
        {
            get { return blocked_by; }
            internal set {
                // If blocked by 0, it is not blocked at all
                if (value > 0)
                {
                    blocked_by = value;
                }
                else
                {
                    blocked_by = null;
                }
            }
        }

        /// <summary>
        /// Returns whether the lock is blocked
        /// </summary>
        public bool Blocked
        {
            get { return (Blocked_by.HasValue && Blocked_by.Value > 0); }
        }

        /// <summary>
        /// Command on the session holding the lock
        /// </summary>
        public string Command
        {
            get { return command; }
            internal set { command = value; }
        }

        /// <summary>
        /// How many instances of this lock exist on the server
        /// </summary>
        public decimal? InstanceCount
        {
            get { return instanceCount; }
            internal set { instanceCount = value; }
        }

        /// <summary>
        /// Database of the lock object
        /// </summary>
        public string Database
        {
            get { return database; }
            internal set { database = value; }
        }

        /// <summary>
        /// Hostname of the owning session
        /// </summary>
        public string Host
        {
            get { return host; }
            internal set { host = value; }
        }

        /// <summary>
        /// Unique identifier for a lock
        /// </summary>
        public Guid Id
        {
            get { return id; }
        }

        /// <summary>
        /// Returns whether the given session is a system process
        /// </summary>
        public bool IsSystemProcess
        {
            //Whether this is a system process is determined by the workstation name
            get { return !(IsUserProcess); }
        }

        /// <summary>
        /// Returns whether the given session is a user process
        /// </summary>
        public bool IsUserProcess
        {
            //Whether this is a system process is determined by the workstation name
            get { return (host != null && host.Length > 0); }
        }

        /// <summary>
        /// FOR SQLDM INTERNAL USE
        /// Non-unique identifier for a lock used for blocking chain creation
        /// </summary>
        public Pair<int, string> InternalLockIdentifier
        {
            get { return new Pair<int, string>(Spid.HasValue?Spid.Value:0, Database + '.' + ObjectSchema + '.' + ObjectName + ':' + TypeString); }
        }

        /// <summary>
        /// Lock Mode of the lock
        /// </summary>
        public LockMode? Mode
        {
            get { return mode; }
            internal set { mode = value; }
        }

        /// <summary> 
        /// Short descriptive string for lock mode
        /// </summary>
        public string ModeShortString
        {
            get { return LockModeToShortString(mode); }
            internal set { Mode = StringToLockMode(value); }
        }

        /// <summary> 
        /// Long descriptive string for lock mode
        /// </summary>
        public string ModeLongString
        {
            get { return LockModeToLongString(mode); }
        }
        
        /// <summary>
        /// Object name of the lock, if applicable and available
        /// </summary>
        public string ObjectName
        {
            get { return objectName; }
            internal set { objectName = value; }
        }

        /// <summary>
        /// Schema/User name of the lock object, if applicable and available
        /// </summary>
        public string ObjectSchema
        {
            get { return objectSchema; }
            internal set { objectSchema = value; }
        }

        /// <summary>
        /// Name of application owning the session that holds the lock
        /// </summary>
        public string Application
        {
            get { return application; }
            internal set { application = value; }
        }

        /// <summary>
        /// Spid of the session holding the lock
        /// </summary>
        public int? Spid
        {
            get { return spid; }
            internal set { spid = value; }
        }

        /// <summary>
        /// Status of the lock
        /// </summary>
        public LockStatus? Status
        {
            get { return status; }
            internal set { status = value; }
        }

        /// <summary>
        /// Status of the lock
        /// </summary>
        public string StatusString
        {
            get { return LockStatusToString(status); }
            internal set { status = StringToLockStatus(value); }
        }

        /// <summary>
        /// Type of the lock
        /// </summary>
        public LockType? Type
        {
            get { return type; }
            internal set { type = value; }
        }

        /// <summary>
        /// Type of the lock in string format
        /// </summary>
        public string TypeString
        {
            get { return LockTypeToString(Type); }
            internal set { Type = StringToLockType(value); }
        }

        /// <summary>
        /// Username associated with the session holding the lock
        /// </summary>
        public string User
        {
            get { return user; }
            internal set { user = value; }
        }

        /// <summary>
        /// Wait time of the session holding the lock
        /// </summary>
        public TimeSpan WaitTime
        {
            get { return waitTime; }
            internal set { waitTime = value; }
        }

        public string FormattedObjectName
        {
            get
            {
                if (!(this.Database != null && this.Database.Length > 0))
                {
                    return "Not applicable";
                }
                else
                {
                    if (this.ObjectSchema != null && this.ObjectSchema.Length > 0 && this.ObjectName != null &&
                        this.ObjectSchema.Length > 0)
                    {
                        return String.Format("{0}.{1}.{2}", this.Database, this.ObjectSchema, this.ObjectName);
                    }
                    else
                    {
                        return this.Database;
                    }
                }
            }
        }

        #endregion

        #region methods

        internal static LockMode StringToLockMode(string lockMode)
        {
            switch (lockMode.ToUpper())
            {
                case "NULL":
                    return LockMode.NULL;
                case "SCH-S":
                    return LockMode.Sch_S;
                case "Sch-M":
                    return LockMode.Sch_M;
                case "S":
                    return LockMode.S;
                case "U":
                    return LockMode.U;
                case "X":
                    return LockMode.X;
                case "IS":
                    return LockMode.IS;
                case "IU":
                    return LockMode.IU;
                case "IX":
                    return LockMode.IX;
                case "SIU":
                    return LockMode.SIU;
                case "SIX":
                    return LockMode.SIX;
                case "UIX":
                    return LockMode.UIX;
                case "BU":
                    return LockMode.BU;
                case "RANGES-S":
                    return LockMode.RangeS_S;
                case "RANGES-U":
                    return LockMode.RangeS_U;
                case "RANGEIN-NULL":
                case "RANGEI-N":
                    return LockMode.RangeIn_Null;
                case "RANGEIN-S":
                case "RANGEI-S":
                    return LockMode.RangeIn_S;
                case "RANGEIN-U":
                case "RANGEI-U":
                    return LockMode.RangeIn_U;
                case "RANGEIN-X":
                case "RANGEI-X":
                    return LockMode.RangeIn_X;
                case "RANGEX-S":
                    return LockMode.RangeX_S;
                case "RANGEX-U":
                    return LockMode.RangeX_U;
                case "RANGEX-X":
                    return LockMode.RangeX_X;
                default:
                    return LockMode.Unknown;
            }
        }

        public static string LockModeToShortString(LockMode? lockMode)
        {
            if (!lockMode.HasValue)
                return "NULL";

            switch (lockMode.Value)
            {
                case LockMode.NULL:
                    return "NULL";
                case LockMode.Sch_S:
                    return "Sch-S";
                case LockMode.Sch_M:
                    return "Sch-M";
                case LockMode.S:
                    return "S";
                case LockMode.U:
                    return "U";
                case LockMode.X:
                    return "X";
                case LockMode.IS:
                    return "IS";
                case LockMode.IU:
                    return "IU";
                case LockMode.IX:
                    return "IX";
                case LockMode.SIU:
                    return "SIU";
                case LockMode.SIX:
                    return "SIX";
                case LockMode.UIX:
                    return "UIX";
                case LockMode.BU:
                    return "BU";
                case LockMode.RangeS_S:
                    return "RangeS-S";
                case LockMode.RangeS_U:
                    return "RangeS-U";
                case LockMode.RangeIn_Null:
                    return "RangeIn-Null";
                case LockMode.RangeIn_S:
                    return "RangeIn-S";
                case LockMode.RangeIn_U:
                    return "RangeIn-U";
                case LockMode.RangeIn_X:
                    return "RangeIn-X";
                case LockMode.RangeX_S:
                    return "RangeX-S";
                case LockMode.RangeX_U:
                    return "RangeX-U";
                case LockMode.RangeX_X:
                    return "RangeX-X";
                default:
                    return "Unknown";
            }
        }

        public static string LockModeToLongString(LockMode? lockMode)
        {
            if (!lockMode.HasValue)
                return "NULL";

            switch (lockMode.Value)
            {
                case LockMode.NULL:
                    return "NULL";
                case LockMode.Sch_S:
                    return "Schema Stability";
                case LockMode.Sch_M:
                    return "Schema Modify";
                case LockMode.S:
                    return "Shared";
                case LockMode.U:
                    return "Update";
                case LockMode.X:
                    return "Exclusive";
                case LockMode.IS:
                    return "Intent Shared";
                case LockMode.IU:
                    return "Intent Update";
                case LockMode.IX:
                    return "Intent Exclusive";
                case LockMode.SIU:
                    return "Shared with Intent Update";
                case LockMode.SIX:
                    return "Shared with Intent Exclusive";
                case LockMode.UIX:
                    return "Update with Intent Exclusive";
                case LockMode.BU:
                    return "Bulk Update";
                case LockMode.RangeS_S:
                    return "Shared Range: Shared";
                case LockMode.RangeS_U:
                    return "Shared Range: Update";
                case LockMode.RangeIn_Null:
                    return "Insert Range: Null resource";
                case LockMode.RangeIn_S:
                    return "Conversion Lock: Shared and Insert Range: Null resource";
                case LockMode.RangeIn_U:
                    return "Conversion Lock: Update and Insert Range: Null resource";
                case LockMode.RangeIn_X:
                    return "Conversion Lock: Exclusive and Insert Range: Null resource";
                case LockMode.RangeX_S:
                    return "Conversion Lock: Insert Range: Null resource and Shared Range: Shared";
                case LockMode.RangeX_U:
                    return "Conversion Lock: Insert Range: Null resource and Shared Range: Update";
                case LockMode.RangeX_X:
                    return "Exclusive Range: Exclusive";
                default:
                    return "Unknown";
            }
        }

        public static string LockStatusToString(LockStatus? lockStatus)
        {
            if (!lockStatus.HasValue)
                return "Unknown";

            switch (lockStatus.Value)
            {
                case LockStatus.Converting: return "Converting";
                case LockStatus.Granted: return "Granted";
                case LockStatus.Waiting: return "Waiting";
                default: return "Unknown";
            }
        }

        internal static LockStatus StringToLockStatus(string lockStatus)
        {
            
            switch (lockStatus.ToLower())
            {
                case "converting":
                case "convert":
                    return LockStatus.Converting;
                case "granted":
                case "grant":
                    return LockStatus.Granted;
                case "waiting":
                case "wait":
                    return LockStatus.Waiting;
                default:
                    return LockStatus.Unknown;
           }
        }

        public static string LockTypeToString(LockType? lockType)
        {
            if (!lockType.HasValue)
                return "Unknown";

            switch (lockType.Value)
            {
                case LockType.AllocationUnit: return "Allocation Unit";
                case LockType.Application: return "Application";
                case LockType.Database: return "Database";
                case LockType.Extent: return "Extent";
                case LockType.File: return "File";
                case LockType.HoBT: return "Heap or BTree";
                case LockType.Index: return "Index";
                case LockType.Key: return "Key";
                case LockType.Metadata: return "Metadata";
                case LockType.Object: return "Object";
                case LockType.Page: return "Page";
                case LockType.RowId: return "Row ID";
                case LockType.Table: return "Table";
                case LockType.Unknown: return "Unknown";
                default: return "Unknown";
            }
        }

        internal static LockType StringToLockType(string lockType)
        {
            switch (lockType.ToLower())
            {
                case "allocation unit":
                case "allocationunit":
                case "allocunit":
                    return LockType.AllocationUnit;
                case "application":
                    return LockType.Application;
                case "database":
                    return LockType.Database;
                case "extent":
                    return LockType.Extent;
                case "file":
                    return LockType.File;
                case "hobt":
                    return LockType.HoBT;
                case "index":
                    return LockType.Index;
                case "key":
                    return LockType.Key;
                case "metadata":
                    return LockType.Metadata;
                case "object":
                    return LockType.Object;
                case "page":
                    return LockType.Page;
                case "rowid":
                case "rid":
                    return LockType.RowId;
                case "table":
                    return LockType.Table;
                default:
                    return LockType.Unknown;
            }
        }

        #endregion

        #region nested types

        
        #endregion


        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("blocking", blocking);
            info.AddValue("blocked_by", blocked_by);
            info.AddValue("command", command);
            info.AddValue("instanceCount", instanceCount);
            info.AddValue("database", database);
            info.AddValue("host", host);
            info.AddValue("mode", mode);
            info.AddValue("objectName", objectName);
            info.AddValue("objectSchema", objectSchema);
            info.AddValue("application", application);
            info.AddValue("spid", spid);
            info.AddValue("status", status);
            info.AddValue("type", type);
            info.AddValue("user", user);
            info.AddValue("id", id);
            info.AddValue("waitTime", waitTime);
        }

        #endregion

        /// <summary>
        /// Returns an Auditable Entity
        /// </summary>
        /// <returns></returns>
        public AuditableEntity GetAuditableEntity()
        {
            AuditableEntity auditableEntity = new AuditableEntity();
            auditableEntity.Name = this.host;

            // Create metadata.
            auditableEntity.AddMetadataProperty("SPID", this.spid.ToString());
            auditableEntity.AddMetadataProperty("Status", this.status.ToString());
            auditableEntity.AddMetadataProperty("User", this.user);
            auditableEntity.AddMetadataProperty("Session's Workstation", this.host);
            auditableEntity.AddMetadataProperty("Application", this.application);

            return auditableEntity;
        }

        /// <summary>
        /// Returns an Auditable Entity based on an oldValue
        /// </summary>
        /// <param name="oldValue"></param>
        /// <returns></returns>
        public AuditableEntity GetAuditableEntity(IAuditable oldValue)
        {
            return GetAuditableEntity();
        }
    }
}
