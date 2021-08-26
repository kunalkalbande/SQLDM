; /*
;
;	Note that we do not specify the severity names or facility names for the categories
;   or messages.  Varying severity and facility causes the message ID numbers that are 
;   generated to be wildly different because the values for facility and severity are 
;   used in the upper bits of the message id.  Leaving them defaulted makes the message
;   identifiers stay sequential.
;
; Message formatting codes
; %n[!format_specifier!] 
;    Describes an insert. Each insert is an entry in the Arguments array in the FormatMessage function. The value of n can be a number between 1 and 99. The format specifier is optional. If no value is specified, the default is !s!. For more information, see wsprintf.
;    The format specifier can use * for either the precision or the width. When specified, they consume inserts numbered n+1 and n+2.
; %0 Terminates a message text line without a trailing newline character. This can be used to build a long line or terminate a prompt message without a trailing newline character. 
; %. Generates a single period. This can be used to display a period at the beginning of a line, which would otherwise terminate the message text. 
; %! Generates a single exclamation point. This can be used to specify an exclamation point immediately after an insert. 
; %% Generates a single percent sign. 
; %\ Generates a hard line break when it occurs at the end of a line. This can be used with FormatMessage to ensure that the message fits a certain width. 
; %b Generates a space character. This can be used to ensure an appropriate number of trailing spaces on a line. 
; %r Generates a hard carriage return without a trailing newline character. 
; 
; */
;
; /*
;
;  Added messages from legacy so we don't accidentally use an existing message id
;
; */ 


MessageIdTypedef=DWORD

SeverityNames=(
	Success=0x0:STATUS_SEVERITY_SUCCESS
    Informational=0x1:STATUS_SEVERITY_INFORMATIONAL
    Warning=0x2:STATUS_SEVERITY_WARNING
    Error=0x3:STATUS_SEVERITY_ERROR
    )

; // Facility names are pretty useless.  They serve as a way to group messages but
; // only within the generated constant in the generated .H file.  If in doubt use
; // the General facility.
FacilityNames=(
	System=0xFF:FACILITY_SYSTEM
	General=0x100:FACILITY_GENERAL
    Management_Service=0x200:FACILITY_MANAGEMENT
	Collection_Service=0x300:FACILITY_COLLECTION
	Console=0x400:FACILITY_CLIENT
	Database=0x500:FACILITY_DATABASE
	None=0x00:FACILITY_NONE
)

LanguageNames=(English=0x409:MSG00409)

; // The following are category definitions.  They should be used as 
; // an indicator of the area of code that generated the message.  

MessageId=0x1
SymbolicName=CATEGORY_GENERAL
Language=English
SQLdm
.

MessageId=0x2
SymbolicName=CATEGORY_AUDIT
Language=English
Audit
.

MessageId=0x3
SymbolicName=CATEGORY_ALERT
Language=English
Alert
.

MessageId=0x4
SymbolicName=CATEGORY_CUSTOM
Language=English
Custom Counter
.

; // The following are generic message definitions.

MessageId=0x100
SymbolicName=MSG_SUCCESS_GENERIC
Language=English
%1 
.

; // The following are general message definitions.

; /* MessageId=1
;    SymbolicName=MSG_Test
;    Language=English
;    %1
;    .
; */

MessageId=101
SymbolicName=MSG_CPU
Language=English
%1
.
MessageId=202
SymbolicName=MSG_Connections
Language=English
%1
.
MessageId=303
SymbolicName=MSG_TempDB
Language=English
%1
.
MessageId=404
SymbolicName=MSG_MaxThreads
Language=English
%1
.
MessageId=505
SymbolicName=MSG_CurrentThreads
Language=English
%1
.
MessageId=606
SymbolicName=MSG_RASlots
Language=English
%1
.
MessageId=708
SymbolicName=MSG_NonDistributed
Language=English
%1
.
MessageId=809
SymbolicName=MSG_NonSubscribed
Language=English
%1
.
MessageId=910
SymbolicName=MSG_OpenTran
Language=English
%1
.
MessageId=1011
SymbolicName=MSG_Reorg
Language=English
%1
.
MessageId=1112
SymbolicName=MSG_DBSize
Language=English
%1
.
MessageId=1115
SymbolicName=MSG_LogFileAutogrow
Language=English
%1
.
MessageId=1116
SymbolicName=MSG_DataFileAutogrow
Language=English
%1
.
MessageId=1213
SymbolicName=MSG_LogSize
Language=English
%1
.
MessageId=1314
SymbolicName=MSG_ExecRunning
Language=English
%1
.
MessageId=1315
SymbolicName=MSG_ExecStopped
Language=English
%1
.
MessageId=1316
SymbolicName=MSG_ExecOther
Language=English
%1
.
MessageId=1417
SymbolicName=MSG_IO
Language=English
%1
.
MessageId=1518
SymbolicName=MSG_ServerRunning
Language=English
%1
.
MessageId=1519
SymbolicName=MSG_ServerStopped
Language=English
%1
.
MessageId=1520
SymbolicName=MSG_ServerPaused
Language=English
%1
.
MessageId=1521
SymbolicName=MSG_NotAdmin
Language=English
%1
.
MessageId=1522
SymbolicName=MSG_ServerNotAccessible
Language=English
%1
.
MessageId=1523
SymbolicName=MSG_ServerOther
Language=English
%1
.
MessageId=1624
SymbolicName=MSG_Memory
Language=English
%1
.
MessageId=1725
SymbolicName=MSG_DBStatus
Language=English
%1
.
MessageId=1826
SymbolicName=MSG_ApplicationStarted
Language=English
%1
.
MessageId=1927
SymbolicName=MSG_ApplicationStopped
Language=English
%1
.
MessageId=2028
SymbolicName=MSG_NonSubscribedTime
Language=English
%1
.
MessageId=2129
SymbolicName=MSG_DTCRunning
Language=English
%1
.
MessageId=2130
SymbolicName=MSG_DTCStopped
Language=English
%1
.
MessageId=2131
SymbolicName=MSG_DTCOther
Language=English
%1
.
MessageId=2232
SymbolicName=MSG_FullTextRunning
Language=English
%1
.
MessageId=2233
SymbolicName=MSG_FullTextOther
Language=English
%1
.
MessageId=2534
SymbolicName=MSG_ServerResponseTime
Language=English
%1
.
MessageId=2635
SymbolicName=MSG_URLConnectionFailed
Language=English
%1
.
MessageId=2636
SymbolicName=MSG_URLTimeout
Language=English
%1
.
MessageId=2737
SymbolicName=MSG_ResourceLimit
Language=English
%1
.
MessageId=2838
SymbolicName=MSG_BlockingProcess
Language=English
%1
.
MessageId=2840
SymbolicName=MSG_Deadlock
Language=English
%1
.
MessageId=2940
SymbolicName=MSG_LongRunningJob
Language=English
%1
.
MessageId=3039
SymbolicName=MSG_BombedJob
Language=English
%1
.
MessageId=3140
SymbolicName=MSG_JobCompletion
Language=English
%1
.
MessageId=4001
SymbolicName=MSG_MaintenanceModeStart
Language=English
%1
.
MessageId=4002
SymbolicName=MSG_MaintenanceModeEnd
Language=English
%1
.
MessageId=4010
SymbolicName=MSG_OSMetricsStatusAvail
Language=English
%1
.
MessageId=4011
SymbolicName=MSG_OSMetricsStatusOther
Language=English
%1
.
MessageId=4012
SymbolicName=MSG_OSMetricsStatusDisabled
Language=English
%1
.
MessageId=4013
SymbolicName=MSG_OSMetricsStatusUnavail
Language=English
%1
.
MessageId=4020
SymbolicName=MSG_OSMemoryUsedPct
Language=English
%1
.
MessageId=4030
SymbolicName=MSG_OSMemoryPagesPerSecond
Language=English
%1
.
MessageId=4040
SymbolicName=MSG_OSCPUProcessorTime
Language=English
%1
.
MessageId=4050
SymbolicName=MSG_OSCPUPrivilegedTime
Language=English
%1
.
MessageId=4060
SymbolicName=MSG_OSCPUUserTime
Language=English
%1
.
MessageId=4070
SymbolicName=MSG_OSCPUProcessorQueueLength
Language=English
%1
.
MessageId=4080
SymbolicName=MSG_OSDiskPhysicalDiskTime
Language=English
%1
.
MessageId=4090
SymbolicName=MSG_OSDiskAverageDiskQueueLength
Language=English
%1
.
MessageId=4100
SymbolicName=MSG_OSDiskFull
Language=English
%1
.
MessageId=4110
SymbolicName=MSG_OSDiskTimePerDisk
Language=English
%1
.
MessageId=4120
SymbolicName=MSG_OSDiskAverageDiskQueueLengthPerDisk
Language=English
%1
.
MessageId=4130
SymbolicName=MSG_PageLifeExpectancy
Language=English
%1
.
MessageId=4140
SymbolicName=MSG_ProcCacheHitRatio
Language=English
%1
.
MessageId=4150
SymbolicName=MSG_AvgDiskMillisecondsWrite
Language=English
%1
.
MessageId=4160
SymbolicName=MSG_AvgDiskMillisecondsTransfer
Language=English
%1
.
MessageId=4170
SymbolicName=MSG_AvgDiskMillisecondsRead
Language=English
%1
.
MessageId=4180
SymbolicName=MSG_DiskReadsPerSecond
Language=English
%1
.
MessageId=4185
SymbolicName=MSG_DiskTransfersPerSecond
Language=English
%1
.
MessageId=4190
SymbolicName=MSG_DiskWritesPerSecond
Language=English
%1
.
MessageId=4200
SymbolicName=MSG_CLRStatus
Language=English
%1
.
MessageId=4210
SymbolicName=MSG_AutomationStatus
Language=English
%1
.
MessageId=4220
SymbolicName=MSG_QueryMonitorStatus
Language=English
%1
.
MessageId=4230
SymbolicName=MSG_CollectionServiceStatus
Language=English
%1
.
MessageId=4231
SymbolicName=MSG_ManagementServiceStatus
Language=English
%1
.
MessageId=4240
SymbolicName=MSG_AgentXPStatus
Language=English
%1
.
MessageId=4250
SymbolicName=MSG_WMIStatus
Language=English
%1
.
MessageId=4260
SymbolicName=MSG_XPCommandShellStatus
Language=English
%1
.
MessageId=4310
SymbolicName=MSG_ClientComputers
Language=English
%1
.
MessageId=4320
SymbolicName=MSG_BlockingSessions
Language=English
%1
.
MessageId=4330
SymbolicName=MSG_DataUsedPercent
Language=English
%1
.
MessageId=4340
SymbolicName=MSG_LogUsedPercent
Language=English
%1
.
MessageId=4350
SymbolicName=MSG_CustomCounter
Language=English
%1
.
MessageId=4351
SymbolicName=MSG_CustomCounterError
Language=English
%1
.
MessageId=4400
SymbolicName=MSG_ErrorLog
Language=English
%1
.
MessageId=4410
SymbolicName=MSG_AgentLog
Language=English
%1
.
MessageId=4500
SymbolicName=MSG_ClusterFailover
Language=English
%1
.
MessageId=4510
SymbolicName=MSG_PreferredClusterNode
Language=English
%1
.

MessageId=1000
SymbolicName=MSG_MANAGEMENT_SERVICE_START
Language=English
SQLdm Management Service (%1) was started successfully.
.
MessageId=1001
SymbolicName=MSG_MANAGEMENT_SERVICE_STOP
Language=English
SQLdm Management Service (%1) was stopped successfully.
.
MessageId=1002
SymbolicName=MSG_COLLECTION_SERVICE_START
Language=English
SQLdm Collection Service (%1) was started successfully.
.
MessageId=1003
SymbolicName=MSG_COLLECTION_SERVICE_STOP
Language=English
SQLdm Collection Service (%1) was stopped successfully.
.
MessageId=1004
SymbolicName=MSG_MANAGEMENT_SERVICE_STOP_FATAL
Language=English
SQLdm Management Service stopped due to a fatal error.
.
MessageId=1005
SymbolicName=MSG_COLLECTION_SERVICE_STOP_FATAL
Language=English
SQLdm Collection Service stopped due to a fatal error.
.
MessageId=1006
SymbolicName=MSG_MANAGEMENT_SERVICE_REMOTING_CONFIG_ERROR
Language=English
SQLdm Management Service is unable to configure communications.  This is usually due
to another application using a port required by the SQLdm Management Service.  

%1
.
MessageId=1007
SymbolicName=MSG_COLLECTION_SERVICE_REMOTING_CONFIG_ERROR
Language=English
SQLdm Collection Service is unable to configure communications.  This is usually due
to another application using a port required by the SQLdm Collection Service.  

%1
.
MessageId=1008
SymbolicName=MSG_PREDICTIVE_ANALYTICS_DISABLED
Language=English
SQLdm Predictive Analytics has been disabled.
.
MessageId=1009
SymbolicName=MSG_PREDICTIVE_ANALYTICS_ENABLED
Language=English
SQLdm Predictive Analytics has been enabled.
.

MessageId=0x00000101
SymbolicName=MSG_ERROR_ARGUMENT_REQUIRED
Severity=Error
Facility=General
Language=English
Required parameter not supplied (%1)
.

MessageId=0x00000102
SymbolicName=MSG_ERROR_ARGUMENT_INVALID
Severity=Error
Facility=General
Language=English
Required parameter out of range (%1)
.

MessageId=0x00000103
SymbolicName=MSG_ERROR_REQUEST_TIMEOUT
Severity=Error
Facility=General
Language=English
Request timed out
.

MessageId=0x00000104
SymbolicName=MSG_ERROR_UNKNOWN
Severity=Error
Facility=General
Language=English
Unknown error.  See log file for more detail.
.

MessageId=0x00000105
SymbolicName=MSG_ERROR_INVALID_MANAGEMENT_SERVICE_ID
Severity=Error
Facility=General
Language=English
Management Service with ID %1 not found.  The management service has determined that it is no longer registered with the repository and has sent a stop collection request to its registered collection services.  Use the configuration wizard to update the repository for this instance of the SQLdm Management Service.
.

MessageId=0x00000106
SymbolicName=MSG_ERROR_INVALID_COLLECTION_SERVICE_ID
Severity=Error
Facility=General
Language=English
Collection Service with ID %1 not found
.

MessageId=0x00000107
SymbolicName=MSG_ERROR_SMTP_PROVIDER_ERROR
Severity=Error
Facility=General
Language=English
%1
.

MessageId=0x00000108
SymbolicName=MSG_ERROR_INVALID_MONITORED_SERVER_ID
Severity=Error
Facility=General
Language=English
Monitored Server with ID %1 not found
.

MessageId=0x00000109
SymbolicName=MSG_ERROR_LICENSE_VIOLATION_ID
Severity=Error
Facility=General
Language=English
A license violation was detected.  The license summary is
%n%1
.

MessageId=0x0000010A
SymbolicName=MSG_LICENSE_EXPIRATION_WARNING_ID
Severity=Warning
Facility=General
Language=English
The license is near expiration.  The license summary is
%n%1
.

MessageId=0x0000010B
SymbolicName=MSG_ERROR_REPOSITORY_TEST_FAILED
Severity=Error
Facility=General
Language=English
%1
.
MessageId=0x0000010C
SymbolicName=MSG_ERROR_CONFIG_INVALID
Severity=Error
Facility=General
Language=English
Configuration file (%1) is invalid.  One or more expected items are missing.
.
MessageId=0x0000010D
SymbolicName=MSG_ERROR_CONFIG_FOOBAR
Severity=Error
Facility=General
Language=English
Configuration file (%1) is invalid.
%2
.
MessageId=0x0000010E
SymbolicName=MSG_REPOSITORY_TEST_PASSED
Severity=Success
Facility=General
Language=English
%1
.
MessageId=0x0000010F
SymbolicName=MSG_ERROR_UNAUTHORIZED_ACCESS_EXCEPTION
Severity=Error
Facility=General
Language=English
The service account (%1) requires write access to the %2 directory.
%3
.
MessageId=0x00000110
SymbolicName=MSG_ERROR_REPOSITORY_ERROR
Severity=Error
Facility=General
Language=English
%1
.
MessageId=5100
SymbolicName=MSG_MONITORED_SERVER_ADDED
Severity=Success
Facility=General
Language=English
Monitored SQL Server %1 added.
.

MessageId=5101
SymbolicName=MSG_MONITORED_SERVER_REMOVED
Severity=Success
Facility=General
Language=English
Monitored SQL Server %1 removed.
.

MessageId=5102
SymbolicName=MSG_MONITORED_SERVER_DELETED
Severity=Success
Facility=General
Language=English
Monitored SQL Server %1 deleted.
.

MessageId=5103
SymbolicName=MSG_MONITORED_SERVER_CHANGED
Severity=Success
Facility=General
Language=English
Monitored SQL Server %1 changed.
.

MessageId=5104
SymbolicName=MSG_TASK_CHANGED
Severity=Success
Facility=General
Language=English
Task %1 changed.
.
        
MessageId=5105
SymbolicName=MSG_UnsentLogThreshold
Facility=None
Language=English
%1
.
MessageId=5106
SymbolicName=MSG_UnrestoredLog
Language=English
%1
.
MessageId=5107
SymbolicName=MSG_OldestUnsentMirroringTran
Language=English
%1
.
MessageId=5108
SymbolicName=MSG_MirrorCommitOverhead
Language=English
%1
.
MessageId=5109
SymbolicName=MSG_MirroringSessionsStatus
Language=English
%1
.
MessageId=5110
SymbolicName=MSG_MirroringSessionNonPreferredConfig
Language=English
%1
.
MessageId=5111
SymbolicName=MSG_MirroringSessionRoleChange
Language=English
%1
.
MessageId=5112
SymbolicName=MSG_MirroringWitnessConnection
Language=English
%1
.
MessageId=5120
SymbolicName=MSG_NonDistributedTime
Language=English
%1
.
MessageId=5130
SymbolicName=MSG_VersionStoreGenerationRatio
Language=English
%1
.
MessageId=5131
SymbolicName=MSG_VersionStoreSize
Language=English
%1
.
MessageId=5132
SymbolicName=MSG_TempdbContention
Language=English
%1
.
MessageId=5135
SymbolicName=MSG_LongRunningVersionStoreTransaction
Language=English
%1
.
MessageId=5136
SymbolicName=MSG_SessionTempdbSpaceUsage
Language=English
%1
.
MessageId=5200
SymbolicName=MSG_VmConfigChange
Language=English
%1
.
MessageId=5201
SymbolicName=MSG_VmHostServerChange
Language=English
%1
.
MessageId=5202
SymbolicName=MSG_VmCPUUtilization
Language=English
%1
.
MessageId=5250
SymbolicName=MSG_VmESXCPUUtilization
Language=English
%1
.
MessageId=5203
SymbolicName=MSG_VmMemoryUtilization
Language=English
%1
.
MessageId=5251
SymbolicName=MSG_VmESXMemoryUsage
Language=English
%1
.
MessageId=5204
SymbolicName=MSG_VmCPUReadyWaitTime
Language=English
%1
.
MessageId=5205
SymbolicName=MSG_VmReclaimedMemory
Language=English
%1
.
MessageId=5206
SymbolicName=MSG_VmMemorySwapDelayDetected
Language=English
%1
.
MessageId=5252
SymbolicName=MSG_VmESXMemorySwapDetected
Language=English
%1
.
MessageID=5207
SymbolicName=MSG_VmResourceLimits
Language=English
%1
.
MessageID=5208
SymbolicName=MSG_VmPowerState
Language=English
%1
.
MessageID=5253
SymbolicName=MSG_VmESXPowerState
Language=English
%1
.
MessageID=1113
SymbolicName=MSG_DbSizeMb
Language=English
%1
.
MessageID=1214
SymbolicName=MSG_TranLogSizeMb
Language=English
%1
.
MessageID=4101
SymbolicName=MSG_OsDiskFreeMb
Language=English
%1
.
MessageID=5301
SymbolicName=MSG_AlwaysOnAvailabilityGroupRoleChange
Language=English
%1
.
MessageID=5302
SymbolicName=MSG_AlwaysOnEstimatedDataLossTime
Language=English
%1
.
MessageID=5303
SymbolicName=MSG_AlwaysOnSynchronizationHealthState
Language=English
%1
.
MessageID=5304
SymbolicName=MSG_AlwaysOnEstimatedRecoveryTime
Language=English
%1
.
MessageID=5305
SymbolicName=MSG_AlwaysOnSynchronizationPerformance
Language=English
%1
.
MessageID=5306
SymbolicName=MSG_AlwaysOnLogSendQueueSize
Language=English
%1
.
MessageID=5307
SymbolicName=MSG_AlwaysOnRedoQueueSize
Language=English
%1
.
MessageID=5308
SymbolicName=MSG_AlwaysOnRedoRate
Language=English
%1
.
