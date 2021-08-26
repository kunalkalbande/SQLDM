/*

Note that we do not specify the severity names or facility names for the categories
or messages.  Varying severity and facility causes the message ID numbers that are
generated to be wildly different because the values for facility and severity are
used in the upper bits of the message id.  Leaving them defaulted makes the message
identifiers stay sequential.

Message formatting codes
%n[!format_specifier!]
Describes an insert. Each insert is an entry in the Arguments array in the FormatMessage function. The value of n can be a number between 1 and 99. The format specifier is optional. If no value is specified, the default is !s!. For more information, see wsprintf.
The format specifier can use * for either the precision or the width. When specified, they consume inserts numbered n+1 and n+2.
%0 Terminates a message text line without a trailing newline character. This can be used to build a long line or terminate a prompt message without a trailing newline character.
%. Generates a single period. This can be used to display a period at the beginning of a line, which would otherwise terminate the message text.
%! Generates a single exclamation point. This can be used to specify an exclamation point immediately after an insert.
%% Generates a single percent sign.
%\ Generates a hard line break when it occurs at the end of a line. This can be used with FormatMessage to ensure that the message fits a certain width.
%b Generates a space character. This can be used to ensure an appropriate number of trailing spaces on a line.
%r Generates a hard carriage return without a trailing newline character.

*/

/*

Added messages from legacy so we don't accidentally use an existing message id

*/
// Facility names are pretty useless.  They serve as a way to group messages but
// only within the generated constant in the generated .H file.  If in doubt use
// the General facility.
// The following are category definitions.  They should be used as 
// an indicator of the area of code that generated the message.  
//
//  Values are 32 bit values laid out as follows:
//
//   3 3 2 2 2 2 2 2 2 2 2 2 1 1 1 1 1 1 1 1 1 1
//   1 0 9 8 7 6 5 4 3 2 1 0 9 8 7 6 5 4 3 2 1 0 9 8 7 6 5 4 3 2 1 0
//  +---+-+-+-----------------------+-------------------------------+
//  |Sev|C|R|     Facility          |               Code            |
//  +---+-+-+-----------------------+-------------------------------+
//
//  where
//
//      Sev - is the severity code
//
//          00 - Success
//          01 - Informational
//          10 - Warning
//          11 - Error
//
//      C - is the Customer code flag
//
//      R - is a reserved bit
//
//      Facility - is the facility code
//
//      Code - is the facility's status code
//
//
// Define the facility codes
//
#define FACILITY_SYSTEM                  0xFF
#define FACILITY_GENERAL                 0x100
#define FACILITY_MANAGEMENT              0x200
#define FACILITY_COLLECTION              0x300
#define FACILITY_CLIENT                  0x400
#define FACILITY_DATABASE                0x500
#define FACILITY_NONE                    0x0


//
// Define the severity codes
//
#define STATUS_SEVERITY_SUCCESS          0x0
#define STATUS_SEVERITY_INFORMATIONAL    0x1
#define STATUS_SEVERITY_WARNING          0x2
#define STATUS_SEVERITY_ERROR            0x3


//
// MessageId: CATEGORY_GENERAL
//
// MessageText:
//
// SQLdm
//
#define CATEGORY_GENERAL                 ((DWORD)0x00000001L)

//
// MessageId: CATEGORY_AUDIT
//
// MessageText:
//
// Audit
//
#define CATEGORY_AUDIT                   ((DWORD)0x00000002L)

//
// MessageId: CATEGORY_ALERT
//
// MessageText:
//
// Alert
//
#define CATEGORY_ALERT                   ((DWORD)0x00000003L)

//
// MessageId: CATEGORY_CUSTOM
//
// MessageText:
//
// Custom Counter
//
#define CATEGORY_CUSTOM                  ((DWORD)0x00000004L)

// The following are generic message definitions.
//
// MessageId: MSG_SUCCESS_GENERIC
//
// MessageText:
//
// %1 
//
#define MSG_SUCCESS_GENERIC              ((DWORD)0x00000100L)

// The following are general message definitions.
/* MessageId=1
SymbolicName=MSG_Test
Language=English
%1
.
*/
//
// MessageId: MSG_CPU
//
// MessageText:
//
// %1
//
#define MSG_CPU                          ((DWORD)0x00000065L)

//
// MessageId: MSG_Connections
//
// MessageText:
//
// %1
//
#define MSG_Connections                  ((DWORD)0x000000CAL)

//
// MessageId: MSG_TempDB
//
// MessageText:
//
// %1
//
#define MSG_TempDB                       ((DWORD)0x0000012FL)

//
// MessageId: MSG_MaxThreads
//
// MessageText:
//
// %1
//
#define MSG_MaxThreads                   ((DWORD)0x00000194L)

//
// MessageId: MSG_CurrentThreads
//
// MessageText:
//
// %1
//
#define MSG_CurrentThreads               ((DWORD)0x000001F9L)

//
// MessageId: MSG_RASlots
//
// MessageText:
//
// %1
//
#define MSG_RASlots                      ((DWORD)0x0000025EL)

//
// MessageId: MSG_NonDistributed
//
// MessageText:
//
// %1
//
#define MSG_NonDistributed               ((DWORD)0x000002C4L)

//
// MessageId: MSG_NonSubscribed
//
// MessageText:
//
// %1
//
#define MSG_NonSubscribed                ((DWORD)0x00000329L)

//
// MessageId: MSG_OpenTran
//
// MessageText:
//
// %1
//
#define MSG_OpenTran                     ((DWORD)0x0000038EL)

//
// MessageId: MSG_Reorg
//
// MessageText:
//
// %1
//
#define MSG_Reorg                        ((DWORD)0x000003F3L)

//
// MessageId: MSG_DBSize
//
// MessageText:
//
// %1
//
#define MSG_DBSize                       ((DWORD)0x00000458L)

//
// MessageId: MSG_LogFileAutogrow
//
// MessageText:
//
// %1
//
#define MSG_LogFileAutogrow              ((DWORD)0x0000045BL)

//
// MessageId: MSG_DataFileAutogrow
//
// MessageText:
//
// %1
//
#define MSG_DataFileAutogrow             ((DWORD)0x0000045CL)

//
// MessageId: MSG_LogSize
//
// MessageText:
//
// %1
//
#define MSG_LogSize                      ((DWORD)0x000004BDL)

//
// MessageId: MSG_ExecRunning
//
// MessageText:
//
// %1
//
#define MSG_ExecRunning                  ((DWORD)0x00000522L)

//
// MessageId: MSG_ExecStopped
//
// MessageText:
//
// %1
//
#define MSG_ExecStopped                  ((DWORD)0x00000523L)

//
// MessageId: MSG_ExecOther
//
// MessageText:
//
// %1
//
#define MSG_ExecOther                    ((DWORD)0x00000524L)

//
// MessageId: MSG_IO
//
// MessageText:
//
// %1
//
#define MSG_IO                           ((DWORD)0x00000589L)

//
// MessageId: MSG_ServerRunning
//
// MessageText:
//
// %1
//
#define MSG_ServerRunning                ((DWORD)0x000005EEL)

//
// MessageId: MSG_ServerStopped
//
// MessageText:
//
// %1
//
#define MSG_ServerStopped                ((DWORD)0x000005EFL)

//
// MessageId: MSG_ServerPaused
//
// MessageText:
//
// %1
//
#define MSG_ServerPaused                 ((DWORD)0x000005F0L)

//
// MessageId: MSG_NotAdmin
//
// MessageText:
//
// %1
//
#define MSG_NotAdmin                     ((DWORD)0x000005F1L)

//
// MessageId: MSG_ServerNotAccessible
//
// MessageText:
//
// %1
//
#define MSG_ServerNotAccessible          ((DWORD)0x000005F2L)

//
// MessageId: MSG_ServerOther
//
// MessageText:
//
// %1
//
#define MSG_ServerOther                  ((DWORD)0x000005F3L)

//
// MessageId: MSG_Memory
//
// MessageText:
//
// %1
//
#define MSG_Memory                       ((DWORD)0x00000658L)

//
// MessageId: MSG_DBStatus
//
// MessageText:
//
// %1
//
#define MSG_DBStatus                     ((DWORD)0x000006BDL)

//
// MessageId: MSG_ApplicationStarted
//
// MessageText:
//
// %1
//
#define MSG_ApplicationStarted           ((DWORD)0x00000722L)

//
// MessageId: MSG_ApplicationStopped
//
// MessageText:
//
// %1
//
#define MSG_ApplicationStopped           ((DWORD)0x00000787L)

//
// MessageId: MSG_NonSubscribedTime
//
// MessageText:
//
// %1
//
#define MSG_NonSubscribedTime            ((DWORD)0x000007ECL)

//
// MessageId: MSG_DTCRunning
//
// MessageText:
//
// %1
//
#define MSG_DTCRunning                   ((DWORD)0x00000851L)

//
// MessageId: MSG_DTCStopped
//
// MessageText:
//
// %1
//
#define MSG_DTCStopped                   ((DWORD)0x00000852L)

//
// MessageId: MSG_DTCOther
//
// MessageText:
//
// %1
//
#define MSG_DTCOther                     ((DWORD)0x00000853L)

//
// MessageId: MSG_FullTextRunning
//
// MessageText:
//
// %1
//
#define MSG_FullTextRunning              ((DWORD)0x000008B8L)

//
// MessageId: MSG_FullTextOther
//
// MessageText:
//
// %1
//
#define MSG_FullTextOther                ((DWORD)0x000008B9L)

//
// MessageId: MSG_ServerResponseTime
//
// MessageText:
//
// %1
//
#define MSG_ServerResponseTime           ((DWORD)0x000009E6L)

//
// MessageId: MSG_URLConnectionFailed
//
// MessageText:
//
// %1
//
#define MSG_URLConnectionFailed          ((DWORD)0x00000A4BL)

//
// MessageId: MSG_URLTimeout
//
// MessageText:
//
// %1
//
#define MSG_URLTimeout                   ((DWORD)0x00000A4CL)

//
// MessageId: MSG_ResourceLimit
//
// MessageText:
//
// %1
//
#define MSG_ResourceLimit                ((DWORD)0x00000AB1L)

//
// MessageId: MSG_BlockingProcess
//
// MessageText:
//
// %1
//
#define MSG_BlockingProcess              ((DWORD)0x00000B16L)

//
// MessageId: MSG_Deadlock
//
// MessageText:
//
// %1
//
#define MSG_Deadlock                     ((DWORD)0x00000B18L)

//
// MessageId: MSG_LongRunningJob
//
// MessageText:
//
// %1
//
#define MSG_LongRunningJob               ((DWORD)0x00000B7CL)

//
// MessageId: MSG_BombedJob
//
// MessageText:
//
// %1
//
#define MSG_BombedJob                    ((DWORD)0x00000BDFL)

//
// MessageId: MSG_JobCompletion
//
// MessageText:
//
// %1
//
#define MSG_JobCompletion                ((DWORD)0x00000C44L)

//
// MessageId: MSG_MaintenanceModeStart
//
// MessageText:
//
// %1
//
#define MSG_MaintenanceModeStart         ((DWORD)0x00000FA1L)

//
// MessageId: MSG_MaintenanceModeEnd
//
// MessageText:
//
// %1
//
#define MSG_MaintenanceModeEnd           ((DWORD)0x00000FA2L)

//
// MessageId: MSG_OSMetricsStatusAvail
//
// MessageText:
//
// %1
//
#define MSG_OSMetricsStatusAvail         ((DWORD)0x00000FAAL)

//
// MessageId: MSG_OSMetricsStatusOther
//
// MessageText:
//
// %1
//
#define MSG_OSMetricsStatusOther         ((DWORD)0x00000FABL)

//
// MessageId: MSG_OSMetricsStatusDisabled
//
// MessageText:
//
// %1
//
#define MSG_OSMetricsStatusDisabled      ((DWORD)0x00000FACL)

//
// MessageId: MSG_OSMetricsStatusUnavail
//
// MessageText:
//
// %1
//
#define MSG_OSMetricsStatusUnavail       ((DWORD)0x00000FADL)

//
// MessageId: MSG_OSMemoryUsedPct
//
// MessageText:
//
// %1
//
#define MSG_OSMemoryUsedPct              ((DWORD)0x00000FB4L)

//
// MessageId: MSG_OSMemoryPagesPerSecond
//
// MessageText:
//
// %1
//
#define MSG_OSMemoryPagesPerSecond       ((DWORD)0x00000FBEL)

//
// MessageId: MSG_OSCPUProcessorTime
//
// MessageText:
//
// %1
//
#define MSG_OSCPUProcessorTime           ((DWORD)0x00000FC8L)

//
// MessageId: MSG_OSCPUPrivilegedTime
//
// MessageText:
//
// %1
//
#define MSG_OSCPUPrivilegedTime          ((DWORD)0x00000FD2L)

//
// MessageId: MSG_OSCPUUserTime
//
// MessageText:
//
// %1
//
#define MSG_OSCPUUserTime                ((DWORD)0x00000FDCL)

//
// MessageId: MSG_OSCPUProcessorQueueLength
//
// MessageText:
//
// %1
//
#define MSG_OSCPUProcessorQueueLength    ((DWORD)0x00000FE6L)

//
// MessageId: MSG_OSDiskPhysicalDiskTime
//
// MessageText:
//
// %1
//
#define MSG_OSDiskPhysicalDiskTime       ((DWORD)0x00000FF0L)

//
// MessageId: MSG_OSDiskAverageDiskQueueLength
//
// MessageText:
//
// %1
//
#define MSG_OSDiskAverageDiskQueueLength ((DWORD)0x00000FFAL)

//
// MessageId: MSG_OSDiskFull
//
// MessageText:
//
// %1
//
#define MSG_OSDiskFull                   ((DWORD)0x00001004L)

//
// MessageId: MSG_OSDiskTimePerDisk
//
// MessageText:
//
// %1
//
#define MSG_OSDiskTimePerDisk            ((DWORD)0x0000100EL)

//
// MessageId: MSG_OSDiskAverageDiskQueueLengthPerDisk
//
// MessageText:
//
// %1
//
#define MSG_OSDiskAverageDiskQueueLengthPerDisk ((DWORD)0x00001018L)

//
// MessageId: MSG_PageLifeExpectancy
//
// MessageText:
//
// %1
//
#define MSG_PageLifeExpectancy           ((DWORD)0x00001022L)

//
// MessageId: MSG_ProcCacheHitRatio
//
// MessageText:
//
// %1
//
#define MSG_ProcCacheHitRatio            ((DWORD)0x0000102CL)

//
// MessageId: MSG_AvgDiskMillisecondsWrite
//
// MessageText:
//
// %1
//
#define MSG_AvgDiskMillisecondsWrite     ((DWORD)0x00001036L)

//
// MessageId: MSG_AvgDiskMillisecondsTransfer
//
// MessageText:
//
// %1
//
#define MSG_AvgDiskMillisecondsTransfer  ((DWORD)0x00001040L)

//
// MessageId: MSG_AvgDiskMillisecondsRead
//
// MessageText:
//
// %1
//
#define MSG_AvgDiskMillisecondsRead      ((DWORD)0x0000104AL)

//
// MessageId: MSG_DiskReadsPerSecond
//
// MessageText:
//
// %1
//
#define MSG_DiskReadsPerSecond           ((DWORD)0x00001054L)

//
// MessageId: MSG_DiskTransfersPerSecond
//
// MessageText:
//
// %1
//
#define MSG_DiskTransfersPerSecond       ((DWORD)0x00001059L)

//
// MessageId: MSG_DiskWritesPerSecond
//
// MessageText:
//
// %1
//
#define MSG_DiskWritesPerSecond          ((DWORD)0x0000105EL)

//
// MessageId: MSG_CLRStatus
//
// MessageText:
//
// %1
//
#define MSG_CLRStatus                    ((DWORD)0x00001068L)

//
// MessageId: MSG_AutomationStatus
//
// MessageText:
//
// %1
//
#define MSG_AutomationStatus             ((DWORD)0x00001072L)

//
// MessageId: MSG_QueryMonitorStatus
//
// MessageText:
//
// %1
//
#define MSG_QueryMonitorStatus           ((DWORD)0x0000107CL)

//
// MessageId: MSG_CollectionServiceStatus
//
// MessageText:
//
// %1
//
#define MSG_CollectionServiceStatus      ((DWORD)0x00001086L)

//
// MessageId: MSG_ManagementServiceStatus
//
// MessageText:
//
// %1
//
#define MSG_ManagementServiceStatus      ((DWORD)0x00001087L)

//
// MessageId: MSG_AgentXPStatus
//
// MessageText:
//
// %1
//
#define MSG_AgentXPStatus                ((DWORD)0x00001090L)

//
// MessageId: MSG_WMIStatus
//
// MessageText:
//
// %1
//
#define MSG_WMIStatus                    ((DWORD)0x0000109AL)

//
// MessageId: MSG_XPCommandShellStatus
//
// MessageText:
//
// %1
//
#define MSG_XPCommandShellStatus         ((DWORD)0x000010A4L)

//
// MessageId: MSG_ClientComputers
//
// MessageText:
//
// %1
//
#define MSG_ClientComputers              ((DWORD)0x000010D6L)

//
// MessageId: MSG_BlockingSessions
//
// MessageText:
//
// %1
//
#define MSG_BlockingSessions             ((DWORD)0x000010E0L)

//
// MessageId: MSG_DataUsedPercent
//
// MessageText:
//
// %1
//
#define MSG_DataUsedPercent              ((DWORD)0x000010EAL)

//
// MessageId: MSG_LogUsedPercent
//
// MessageText:
//
// %1
//
#define MSG_LogUsedPercent               ((DWORD)0x000010F4L)

//
// MessageId: MSG_CustomCounter
//
// MessageText:
//
// %1
//
#define MSG_CustomCounter                ((DWORD)0x000010FEL)

//
// MessageId: MSG_CustomCounterError
//
// MessageText:
//
// %1
//
#define MSG_CustomCounterError           ((DWORD)0x000010FFL)

//
// MessageId: MSG_ErrorLog
//
// MessageText:
//
// %1
//
#define MSG_ErrorLog                     ((DWORD)0x00001130L)

//
// MessageId: MSG_AgentLog
//
// MessageText:
//
// %1
//
#define MSG_AgentLog                     ((DWORD)0x0000113AL)

//
// MessageId: MSG_ClusterFailover
//
// MessageText:
//
// %1
//
#define MSG_ClusterFailover              ((DWORD)0x00001194L)

//
// MessageId: MSG_PreferredClusterNode
//
// MessageText:
//
// %1
//
#define MSG_PreferredClusterNode         ((DWORD)0x0000119EL)

//
// MessageId: MSG_MANAGEMENT_SERVICE_START
//
// MessageText:
//
// SQLdm Management Service (%1) was started successfully.
//
#define MSG_MANAGEMENT_SERVICE_START     ((DWORD)0x000003E8L)

//
// MessageId: MSG_MANAGEMENT_SERVICE_STOP
//
// MessageText:
//
// SQLdm Management Service (%1) was stopped successfully.
//
#define MSG_MANAGEMENT_SERVICE_STOP      ((DWORD)0x000003E9L)

//
// MessageId: MSG_COLLECTION_SERVICE_START
//
// MessageText:
//
// SQLdm Collection Service (%1) was started successfully.
//
#define MSG_COLLECTION_SERVICE_START     ((DWORD)0x000003EAL)

//
// MessageId: MSG_COLLECTION_SERVICE_STOP
//
// MessageText:
//
// SQLdm Collection Service (%1) was stopped successfully.
//
#define MSG_COLLECTION_SERVICE_STOP      ((DWORD)0x000003EBL)

//
// MessageId: MSG_MANAGEMENT_SERVICE_STOP_FATAL
//
// MessageText:
//
// SQLdm Management Service stopped due to a fatal error.
//
#define MSG_MANAGEMENT_SERVICE_STOP_FATAL ((DWORD)0x000003ECL)

//
// MessageId: MSG_COLLECTION_SERVICE_STOP_FATAL
//
// MessageText:
//
// SQLdm Collection Service stopped due to a fatal error.
//
#define MSG_COLLECTION_SERVICE_STOP_FATAL ((DWORD)0x000003EDL)

//
// MessageId: MSG_MANAGEMENT_SERVICE_REMOTING_CONFIG_ERROR
//
// MessageText:
//
// SQLdm Management Service is unable to configure communications.  This is usually due
// to another application using a port required by the SQLdm Management Service.  
// 
// %1
//
#define MSG_MANAGEMENT_SERVICE_REMOTING_CONFIG_ERROR ((DWORD)0x000003EEL)

//
// MessageId: MSG_COLLECTION_SERVICE_REMOTING_CONFIG_ERROR
//
// MessageText:
//
// SQLdm Collection Service is unable to configure communications.  This is usually due
// to another application using a port required by the SQLdm Collection Service.  
// 
// %1
//
#define MSG_COLLECTION_SERVICE_REMOTING_CONFIG_ERROR ((DWORD)0x000003EFL)

//
// MessageId: MSG_PREDICTIVE_ANALYTICS_DISABLED
//
// MessageText:
//
// SQLdm Predictive Analytics has been disabled.
//
#define MSG_PREDICTIVE_ANALYTICS_DISABLED ((DWORD)0x000003F0L)

//
// MessageId: MSG_PREDICTIVE_ANALYTICS_ENABLED
//
// MessageText:
//
// SQLdm Predictive Analytics has been enabled.
//
#define MSG_PREDICTIVE_ANALYTICS_ENABLED ((DWORD)0x000003F1L)

//
// MessageId: MSG_ERROR_ARGUMENT_REQUIRED
//
// MessageText:
//
// Required parameter not supplied (%1)
//
#define MSG_ERROR_ARGUMENT_REQUIRED      ((DWORD)0xC1000101L)

//
// MessageId: MSG_ERROR_ARGUMENT_INVALID
//
// MessageText:
//
// Required parameter out of range (%1)
//
#define MSG_ERROR_ARGUMENT_INVALID       ((DWORD)0xC1000102L)

//
// MessageId: MSG_ERROR_REQUEST_TIMEOUT
//
// MessageText:
//
// Request timed out
//
#define MSG_ERROR_REQUEST_TIMEOUT        ((DWORD)0xC1000103L)

//
// MessageId: MSG_ERROR_UNKNOWN
//
// MessageText:
//
// Unknown error.  See log file for more detail.
//
#define MSG_ERROR_UNKNOWN                ((DWORD)0xC1000104L)

//
// MessageId: MSG_ERROR_INVALID_MANAGEMENT_SERVICE_ID
//
// MessageText:
//
// Management Service with ID %1 not found.  The management service has determined that it is no longer registered with the repository and has sent a stop collection request to its registered collection services.  Use the configuration wizard to update the repository for this instance of the SQLdm Management Service.
//
#define MSG_ERROR_INVALID_MANAGEMENT_SERVICE_ID ((DWORD)0xC1000105L)

//
// MessageId: MSG_ERROR_INVALID_COLLECTION_SERVICE_ID
//
// MessageText:
//
// Collection Service with ID %1 not found
//
#define MSG_ERROR_INVALID_COLLECTION_SERVICE_ID ((DWORD)0xC1000106L)

//
// MessageId: MSG_ERROR_SMTP_PROVIDER_ERROR
//
// MessageText:
//
// %1
//
#define MSG_ERROR_SMTP_PROVIDER_ERROR    ((DWORD)0xC1000107L)

//
// MessageId: MSG_ERROR_INVALID_MONITORED_SERVER_ID
//
// MessageText:
//
// Monitored Server with ID %1 not found
//
#define MSG_ERROR_INVALID_MONITORED_SERVER_ID ((DWORD)0xC1000108L)

//
// MessageId: MSG_ERROR_LICENSE_VIOLATION_ID
//
// MessageText:
//
// A license violation was detected.  The license summary is
// %n%1
//
#define MSG_ERROR_LICENSE_VIOLATION_ID   ((DWORD)0xC1000109L)

//
// MessageId: MSG_LICENSE_EXPIRATION_WARNING_ID
//
// MessageText:
//
// The license is near expiration.  The license summary is
// %n%1
//
#define MSG_LICENSE_EXPIRATION_WARNING_ID ((DWORD)0x8100010AL)

//
// MessageId: MSG_ERROR_REPOSITORY_TEST_FAILED
//
// MessageText:
//
// %1
//
#define MSG_ERROR_REPOSITORY_TEST_FAILED ((DWORD)0xC100010BL)

//
// MessageId: MSG_ERROR_CONFIG_INVALID
//
// MessageText:
//
// Configuration file (%1) is invalid.  One or more expected items are missing.
//
#define MSG_ERROR_CONFIG_INVALID         ((DWORD)0xC100010CL)

//
// MessageId: MSG_ERROR_CONFIG_FOOBAR
//
// MessageText:
//
// Configuration file (%1) is invalid.
// %2
//
#define MSG_ERROR_CONFIG_FOOBAR          ((DWORD)0xC100010DL)

//
// MessageId: MSG_REPOSITORY_TEST_PASSED
//
// MessageText:
//
// %1
//
#define MSG_REPOSITORY_TEST_PASSED       ((DWORD)0x0100010EL)

//
// MessageId: MSG_ERROR_UNAUTHORIZED_ACCESS_EXCEPTION
//
// MessageText:
//
// The service account (%1) requires write access to the %2 directory.
// %3
//
#define MSG_ERROR_UNAUTHORIZED_ACCESS_EXCEPTION ((DWORD)0xC100010FL)

//
// MessageId: MSG_ERROR_REPOSITORY_ERROR
//
// MessageText:
//
// %1
//
#define MSG_ERROR_REPOSITORY_ERROR       ((DWORD)0xC1000110L)

//
// MessageId: MSG_MONITORED_SERVER_ADDED
//
// MessageText:
//
// Monitored SQL Server %1 added.
//
#define MSG_MONITORED_SERVER_ADDED       ((DWORD)0x010013ECL)

//
// MessageId: MSG_MONITORED_SERVER_REMOVED
//
// MessageText:
//
// Monitored SQL Server %1 removed.
//
#define MSG_MONITORED_SERVER_REMOVED     ((DWORD)0x010013EDL)

//
// MessageId: MSG_MONITORED_SERVER_DELETED
//
// MessageText:
//
// Monitored SQL Server %1 deleted.
//
#define MSG_MONITORED_SERVER_DELETED     ((DWORD)0x010013EEL)

//
// MessageId: MSG_MONITORED_SERVER_CHANGED
//
// MessageText:
//
// Monitored SQL Server %1 changed.
//
#define MSG_MONITORED_SERVER_CHANGED     ((DWORD)0x010013EFL)

//
// MessageId: MSG_TASK_CHANGED
//
// MessageText:
//
// Task %1 changed.
//
#define MSG_TASK_CHANGED                 ((DWORD)0x010013F0L)

//
// MessageId: MSG_UnsentLogThreshold
//
// MessageText:
//
// %1
//
#define MSG_UnsentLogThreshold           ((DWORD)0x000013F1L)

//
// MessageId: MSG_UnrestoredLog
//
// MessageText:
//
// %1
//
#define MSG_UnrestoredLog                ((DWORD)0x000013F2L)

//
// MessageId: MSG_OldestUnsentMirroringTran
//
// MessageText:
//
// %1
//
#define MSG_OldestUnsentMirroringTran    ((DWORD)0x000013F3L)

//
// MessageId: MSG_MirrorCommitOverhead
//
// MessageText:
//
// %1
//
#define MSG_MirrorCommitOverhead         ((DWORD)0x000013F4L)

//
// MessageId: MSG_MirroringSessionsStatus
//
// MessageText:
//
// %1
//
#define MSG_MirroringSessionsStatus      ((DWORD)0x000013F5L)

//
// MessageId: MSG_MirroringSessionNonPreferredConfig
//
// MessageText:
//
// %1
//
#define MSG_MirroringSessionNonPreferredConfig ((DWORD)0x000013F6L)

//
// MessageId: MSG_MirroringSessionRoleChange
//
// MessageText:
//
// %1
//
#define MSG_MirroringSessionRoleChange   ((DWORD)0x000013F7L)

//
// MessageId: MSG_MirroringWitnessConnection
//
// MessageText:
//
// %1
//
#define MSG_MirroringWitnessConnection   ((DWORD)0x000013F8L)

//
// MessageId: MSG_NonDistributedTime
//
// MessageText:
//
// %1
//
#define MSG_NonDistributedTime           ((DWORD)0x00001400L)

//
// MessageId: MSG_VersionStoreGenerationRatio
//
// MessageText:
//
// %1
//
#define MSG_VersionStoreGenerationRatio  ((DWORD)0x0000140AL)

//
// MessageId: MSG_VersionStoreSize
//
// MessageText:
//
// %1
//
#define MSG_VersionStoreSize             ((DWORD)0x0000140BL)

//
// MessageId: MSG_TempdbContention
//
// MessageText:
//
// %1
//
#define MSG_TempdbContention             ((DWORD)0x0000140CL)

//
// MessageId: MSG_LongRunningVersionStoreTransaction
//
// MessageText:
//
// %1
//
#define MSG_LongRunningVersionStoreTransaction ((DWORD)0x0000140FL)

//
// MessageId: MSG_SessionTempdbSpaceUsage
//
// MessageText:
//
// %1
//
#define MSG_SessionTempdbSpaceUsage      ((DWORD)0x00001410L)

//
// MessageId: MSG_VmConfigChange
//
// MessageText:
//
// %1
//
#define MSG_VmConfigChange               ((DWORD)0x00001450L)

//
// MessageId: MSG_VmHostServerChange
//
// MessageText:
//
// %1
//
#define MSG_VmHostServerChange           ((DWORD)0x00001451L)

//
// MessageId: MSG_VmCPUUtilization
//
// MessageText:
//
// %1
//
#define MSG_VmCPUUtilization             ((DWORD)0x00001452L)

//
// MessageId: MSG_VmESXCPUUtilization
//
// MessageText:
//
// %1
//
#define MSG_VmESXCPUUtilization          ((DWORD)0x00001482L)

//
// MessageId: MSG_VmMemoryUtilization
//
// MessageText:
//
// %1
//
#define MSG_VmMemoryUtilization          ((DWORD)0x00001453L)

//
// MessageId: MSG_VmESXMemoryUsage
//
// MessageText:
//
// %1
//
#define MSG_VmESXMemoryUsage             ((DWORD)0x00001483L)

//
// MessageId: MSG_VmCPUReadyWaitTime
//
// MessageText:
//
// %1
//
#define MSG_VmCPUReadyWaitTime           ((DWORD)0x00001454L)

//
// MessageId: MSG_VmReclaimedMemory
//
// MessageText:
//
// %1
//
#define MSG_VmReclaimedMemory            ((DWORD)0x00001455L)

//
// MessageId: MSG_VmMemorySwapDelayDetected
//
// MessageText:
//
// %1
//
#define MSG_VmMemorySwapDelayDetected    ((DWORD)0x00001456L)

//
// MessageId: MSG_VmESXMemorySwapDetected
//
// MessageText:
//
// %1
//
#define MSG_VmESXMemorySwapDetected      ((DWORD)0x00001484L)

//
// MessageId: MSG_VmResourceLimits
//
// MessageText:
//
// %1
//
#define MSG_VmResourceLimits             ((DWORD)0x00001457L)

//
// MessageId: MSG_VmPowerState
//
// MessageText:
//
// %1
//
#define MSG_VmPowerState                 ((DWORD)0x00001458L)

//
// MessageId: MSG_VmESXPowerState
//
// MessageText:
//
// %1
//
#define MSG_VmESXPowerState              ((DWORD)0x00001485L)

//
// MessageId: MSG_DbSizeMb
//
// MessageText:
//
// %1
//
#define MSG_DbSizeMb                     ((DWORD)0x00000459L)

//
// MessageId: MSG_TranLogSizeMb
//
// MessageText:
//
// %1
//
#define MSG_TranLogSizeMb                ((DWORD)0x000004BEL)

//
// MessageId: MSG_OsDiskFreeMb
//
// MessageText:
//
// %1
//
#define MSG_OsDiskFreeMb                 ((DWORD)0x00001005L)

//
// MessageId: MSG_AlwaysOnAvailabilityGroupRoleChange
//
// MessageText:
//
// %1
//
#define MSG_AlwaysOnAvailabilityGroupRoleChange ((DWORD)0x000014B5L)

//
// MessageId: MSG_AlwaysOnEstimatedDataLossTime
//
// MessageText:
//
// %1
//
#define MSG_AlwaysOnEstimatedDataLossTime ((DWORD)0x000014B6L)

//
// MessageId: MSG_AlwaysOnSynchronizationHealthState
//
// MessageText:
//
// %1
//
#define MSG_AlwaysOnSynchronizationHealthState ((DWORD)0x000014B7L)

//
// MessageId: MSG_AlwaysOnEstimatedRecoveryTime
//
// MessageText:
//
// %1
//
#define MSG_AlwaysOnEstimatedRecoveryTime ((DWORD)0x000014B8L)

//
// MessageId: MSG_AlwaysOnSynchronizationPerformance
//
// MessageText:
//
// %1
//
#define MSG_AlwaysOnSynchronizationPerformance ((DWORD)0x000014B9L)

//
// MessageId: MSG_AlwaysOnLogSendQueueSize
//
// MessageText:
//
// %1
//
#define MSG_AlwaysOnLogSendQueueSize     ((DWORD)0x000014BAL)

//
// MessageId: MSG_AlwaysOnRedoQueueSize
//
// MessageText:
//
// %1
//
#define MSG_AlwaysOnRedoQueueSize        ((DWORD)0x000014BBL)

//
// MessageId: MSG_AlwaysOnRedoRate
//
// MessageText:
//
// %1
//
#define MSG_AlwaysOnRedoRate             ((DWORD)0x000014BCL)

