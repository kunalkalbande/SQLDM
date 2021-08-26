using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Idera.SQLdm.Common.Snapshots
{
    /// <summary>
    /// Stores Collection related permissions used for checking access before probes
    /// </summary>
    [Flags]
    public enum CollectionPermissions : ulong
    {
        None = 0,
        ALTERANYDATABASE = 1,
        ALTERANYEVENTSESSION = 2,
        ALTERANYLINKEDSERVER = 4,
        ALTERANYLOGIN = 8,
        ALTERANYUSER = 16,
        ALTERSERVERSTATE = 32,
        ALTERSETTINGS = 64,
        ALTERTRACE = 128,
        CONTROLSERVER = 256,
        CREATEDATABASE = 512,
        DBDDLADMINAccess = 1024,
        DBOWNERAccess = 2048,
        DBMMONITORAccess = 4096,
        EXECUTESERVER = 8192,
        MSDBAccess = 16384,
        MSDBAccessSYSJOBHISTORY = 32768,
        MSDBAccessSYSJOBS = 65536,
        MSDBAccessSPGETCOMPOSITEJOBINFO = 131072,
        MSDBAccessBACKUPFILE = 262144,
        MSDBAccessBACKUPMEDIAFAMILY = 524288,
        MSDBAccessBACKUPSET = 1048576,
        MSDBAccessSYSCATEGORIES = 2097152,
        MSDBAccessSYSJOBSTEPS = 4194304,
        SETUPADMINMember = 8388608,
        SYSADMINMember = 16777216,
        TEMPDBAccess = 33554432,
        VIEWDEFINITION = 67108864,
        WINDOWSMember = 134217728,
        ReplicationCheck = 268435456,   // ReplicationCheckPermissions batch
        // Add code changes for Azure Admin account
        AzureAdmin = 536870912,
        SECURITYADMINMEMBER = 1073741824,   // Security Admin batch
        PUBLICMEMBER = 2147483648,   // Public Role Member batch
        EXECUTEMASTERXPSQLAGENTENUMJOBS = 4294967296, //MASTER XP SQL AGENT ENUM JOBS
        MSDBRESTOREHISTORY = 8589934592,
        EXECUTEMASTERDBOXPREGREAD = 17179869184,
        EXECUTEMASTERDBOXPINSTANCEREGREAD = 34359738368,
        SELECTSYSDMSERVERSERVICES = 68719476736,
        EXECUTEMASTERXPSERVICECONTROL = 137438953472,   // Added for DTC Status Check
        SERVERADMIN = 274877906944,
        EXECUTEMASTERXPLOGINCONFIG = 549755813888,
        MsdbSQLAgentOperatorRole = 1099511627776,    // Added for Agent Job Summary
        MsdbSQLAgentReaderRole = 2199023255552,      // Added for Agent Job Summary
        MsdbDbOwner = 4398046511104,                 // Added for Agent Job Summary
        EXECUTEMASTERXPREADERRORLOG = 8796093022208,
        ControlDb = 17592186044416, // New Permissions Added for Azure - Start
        ViewDatabaseStateMaster = 70368744177664    // Executed against master db - New permissions Added for Azure - End
    }

    /// <summary>
    /// Stores Metadata Visibility Configuration permissions on listed tables
    /// </summary>
    [Flags]
    public enum MetadataPermissions : ulong
    {
        None = 0,
        MetadataVisibilitymastersysavailabilitygrouplisteneripaddresses = 1,
        MetadataVisibilitymastersysavailabilitygrouplisteners = 2,
        MetadataVisibilitymastersystraces = 4,
        MetadataVisibilitymastersysallobjects = 8,
        MetadataVisibilitymastersysallsqlmodules = 16,
        MetadataVisibilitymastersysassemblies = 32,
        MetadataVisibilitymastersysassemblymodules = 64,
        MetadataVisibilitymastersysassemblyreferences = 128,
        MetadataVisibilitymastersysassemblytypes = 256,
        MetadataVisibilitymastersyscolumns = 512,
        MetadataVisibilitymastersysforeignkeys = 1024,
        MetadataVisibilitymastersysfulltextcatalogs = 2048,
        MetadataVisibilitymastersyshashindexes = 4096,
        MetadataVisibilitymastersysindexcolumns = 8192,
        MetadataVisibilitymastersysindexes = 16384,
        MetadataVisibilitymastersysinternaltables = 32768,
        MetadataVisibilitymastersysobjects = 65536,
        MetadataVisibilitymastersysparameters = 131072,
        MetadataVisibilitymastersysplanguides = 262144,
        MetadataVisibilitymastersysstats = 524288,
        MetadataVisibilitymastersysstatscolumns = 1048576,
        MetadataVisibilitymastersyssymmetrickeys = 2097152,
        MetadataVisibilitymastersyssynonyms = 4194304,
        MetadataVisibilitymastersyssysindexes = 8388608,
        MetadataVisibilitymastersystables = 16777216,
        MetadataVisibilitymastersystypes = 33554432,
        MetadataVisibilitymastersysxmlindexes = 67108864,
        MetadataVisibilitymastersysxmlschemacollections = 134217728,
        MetadataVisibilitysysdepends = 268435456,
        MetadataVisibilitysysfilegroups = 536870912,
        MetadataVisibilitysysfiles = 1073741824,
        MetadataVisibilitysysfulltextcatalogs = 2147483648,
        MetadataVisibilitysysindexes = 4294967296,
        MetadataVisibilitysysobjects = 8589934592,
        MetadataVisibilitysysreferences = 17179869184,
        MetadataVisibilitysystypes = 34359738368,
        MetadataVisibilitysysusers = 68719476736,
        MetadataVisibilitytempdbsysobjects = 137438953472,
        MetadataVisibilitytempdbsyscolumns = 274877906944,
        MetadataVisibilitysyscolumns = 549755813888,
        MetadataVisibilitymastersysdmdbstatsproperties = 1099511627776,
        MetadataVisibilitysyssqllogins = 2199023255552,
        MetadataVisibilitySysAllocationUnits = 4398046511104,
        MetadataVisibilitySysPartitions = 8796093022208,
        MetadataVisibilityTempdbSysDatabaseFiles = 17592186044416,
        MetadataVisibilitySysSchemas = 35184372088832,
        MetadataVisibilitySysConfigurations = 70368744177664,
        MetadataVisibilityXpMsver = 140737488355328,
        MetadataSysPartitionFunction = 281474976710656,
        MetadataSysPartitionSchemes = 562949953421312,
        MetadataSysSqlDependencies = 1125899906842624,
        MetadataSysDataSpaces = 2251799813685248,
        TempDbDbOwnerAccess = 4503599627370496,
        TempDbDataWriter = 9007199254740992
    }

    /// <summary>
    /// Stores Minimum permissions required for SQLdm to monitor
    /// </summary>
    [Flags]
    public enum MinimumPermissions : ulong
    {
        None = 0,
        VIEWANYDATABASE = 1,
        VIEWANYDEFINITION = 2,
        VIEWDATABASESTATE = 4,
        VIEWSERVERSTATE = 8,
        DbDataReaderAccess = 16,    // New permissions Added for Azure - Start
        VIEWDEFINITION = 32,
        SelectAccess = 64,
        ExecuteAccess = 128 // New permissions Added for Azure - End
    }
}
