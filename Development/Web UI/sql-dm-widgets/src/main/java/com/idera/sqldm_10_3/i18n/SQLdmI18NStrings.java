package com.idera.sqldm_10_3.i18n;

import mazz.i18n.annotation.I18NMessage;
import mazz.i18n.annotation.I18NResourceBundle;

@I18NResourceBundle(baseName = "messages")
public interface SQLdmI18NStrings {

	@I18NMessage("SQL Version")
	public static final String SQL_VERSION = "Labels.sqlversion";

	@I18NMessage("SQL Edition")
	public static final String SQL_EDITION = "Labels.sqledition";

	@I18NMessage("Running since")
	public static final String RUNNING_SINCE = "Labels.runningsince";

	@I18NMessage("Clustered")
	public static final String CLUSTERED = "Labels.clustered";

	@I18NMessage("Processors")
	public static final String PROCESSORS = "Labels.processors";

	@I18NMessage("Host")
	public static final String HOST = "Labels.host";

	@I18NMessage("Host OS")
	public static final String HOST_OS = "Labels.hostOS";

	@I18NMessage("Host Memory(GB)")
	public static final String HOST_MEMORY = "Labels.hostmemory";

	@I18NMessage("SQL Server Service")
	public static final String SERVER_SERVICE = "Labels.sqlserver-service";

	@I18NMessage("SQL Agent Service")
	public static final String AGENT_SERVICE = "Labels.agent-service";

	@I18NMessage("DTC Service")
	public static final String DTC_SERVICE = "Labels.dtc-service";

	@I18NMessage("HOME")
	public static final String NAV_BAR_DASHBOARD_LABEL = "SQLdm.Labels.nav-bar-dashboard-label";

	@I18NMessage("TOP X LISTS")
	public static final String NAV_BAR_TOPTEN = "SQLdm.Labels.nav-bar-topten-label";

	@I18NMessage("alerts")
	public static final String NAV_BAR_ALERTS_LABEL = "SQLdm.Labels.nav-bar-alerts-label";

	@I18NMessage("CUSTOM DASHBOARDS")
	public static final String NAV_BAR_CUSTOM_DASHBOARD_LABEL = "SQLdm.Labels.nav-bar-custom-dashboard-label";
	
	@I18NMessage("Show Alert")
	public static final String SHOW_ALERT = "SQLdm.Messages.show-alert";

	@I18NMessage("Alerts:")
	public static final String ALERTS_COUNT = "SQLdm.Labels.alerts-count";

	@I18NMessage("Go To Category")
	public static final String GO_TO_CATEGORY = "SQLdm.Messages.go-to-category";

	@I18NMessage("Total Wait")
	public static final String SERVER_TOTAL_WAIT = "SQLdm.Labels.serverwaits.totalwait";

	@I18NMessage("Wait")
	public static final String SERVER_WAIT = "SQLdm.Labels.serverwaits.wait";

	@I18NMessage("Drive: ")
	public static final String FILE_DRIVE = "SQLdm.Labels.file.drive";

	@I18NMessage("Drive Name")
	public static final String FILE_DRIVE_NAME = "SQLdm.Labels.file.drivename";

	@I18NMessage("File Path Like:")
	public static final String FILE_PATH = "SQLdm.Labels.file.path";

	@I18NMessage("Database: ")
	public static final String FILE_DATABASE = "SQLdm.Labels.file.database";

	@I18NMessage("File Name Like:")
	public static final String FILE_NAME = "SQLdm.Labels.file.name";

	@I18NMessage("File Type: ")
	public static final String FILE_TYPE = "SQLdm.Labels.file.type";

	@I18NMessage("Sort By: ")
	public static final String SORT_BY = "SQLdm.Labels.sortby";

	@I18NMessage("Sort Direction: ")
	public static final String SORT_DIRECTION = "SQLdm.Labels.sortdirection";

	@I18NMessage("Help")
	public static final String LABEL_HELP = "SQLdm.Labels.help";

	@I18NMessage("BLOCKED SESSIONS")
	public static final String BLOCKED_SESSION = "SQLdm.Labels.blocked-session";

	@I18NMessage("Blocked Sessions")
	public static final String BLOCKED_SESSION_CC = "SQLdm.Labels.blocked-session-camelcase";

	@I18NMessage("Blocked Processes")
	public static final String NUBER_OF_BLOCKED_PROCESSES = "SQLdm.Labels.no-of-blocked-process";

	@I18NMessage("TOP SERVERS BY ACTIVE CONNECTIONS")
	public static final String MOST_ACTIVE_USER_CONNECTION__BY_DATABASE = "SQLdm.Labels.most-active-user-connections";

	@I18NMessage("Connections")
	public static final String CONNECTIONS = "SQLdm.Labels.connections";

	@I18NMessage("Waits")
	public static final String WAIT_TIME = "SQLdm.Labels.wait-time";

	@I18NMessage("Session")
	public static final String SESSION = "SQLdm.Labels.session";

	@I18NMessage("CPU Usage")
	public static final String RESOURCES_CPU_USAGE = "SQLdm.Labels.resources-cpu-usage";

	@I18NMessage("CPU USAGE (%)")
	public static final String RESOURCES_CPU_USAGE_PERCENTAGE = "SQLdm.Labels.resources-cpu-usage-percentage";

	@I18NMessage("Processor Time")
	public static final String RESOURCES_PROCESSOR_TIME = "SQLdm.Labels.resources-processor-time";

	@I18NMessage("PROCESSOR TIME (%)")
	public static final String RESOURCES_PROCESSOR_TIME_IN_PERCENTAGE = "SQLdm.Labels.resources-processor-time-in-percentage";
	
	@I18NMessage("TOTAL IN DB TRENDING")
	public static final String TOTAL_IN_DB_TRENDING = "SQLdm.Labels.total-in-db-trending";

	@I18NMessage("PROCESSOR QUEUE LENGTH")
	public static final String RESOURCES_PROCESSOR_QUEUE_LENGTH = "SQLdm.Labels.resources-processor-queue-length";

	@I18NMessage("DISK READS/SECOND PER DISK")
	public static final String RESOURCES_DISK_READS_PER_SECOND = "SQLdm.Labels.resources-disk-reads-per-second-per-disk";

	@I18NMessage("DISK WRITES/SECOND PER DISK")
	public static final String RESOURCES_DISK_WRITES_PER_SECOND = "SQLdm.Labels.resources-disk-writes-per-second-per-disk";

	@I18NMessage("SQL SERVER PHYSICAL I/O")
	public static final String RESOURCES_SQL_SERVER_PHYSICAL_IO = "SQLdm.Labels.resources-sql-server-physical-io";

	@I18NMessage("SQL Memory Usage")
	public static final String RESOURCES_SQL_MEMORY_USAGE = "SQLdm.Labels.resources-sql-memory-usage";

	@I18NMessage("SQL MEMORY USAGE (MB)")
	public static final String RESOURCES_SQL_MEMORY_USAGE_IN_MB = "SQLdm.Labels.resources-sql-memory-usage-in-mb";

	@I18NMessage("Memory Areas")
	public static final String RESOURCES_MEMEORY_AREAS = "SQLdm.Labels.resources-memory-areas";

	@I18NMessage("MEMORY AREAS (MB)")
	public static final String RESOURCES_MEMEORY_AREAS_IN_MB = "SQLdm.Labels.resources-memory-areas-in-mb";

	@I18NMessage("PAGE LIFE EXPECTANCY (sec)")
	public static final String RESOURCES_PAGE_LIFE_EXPECTANCY = "SQLdm.Labels.resources-page-life-expectancy";

	@I18NMessage("Cache Hit Ratios")
	public static final String RESOURCES_CACHE_HIT_RATIOS = "SQLdm.Labels.resources-cache-hit-ratios";

	@I18NMessage("CACHE HIT RATIOS (%)")
	public static final String RESOURCES_CACHE_HIT_RATIOS_PERCENT = "SQLdm.Labels.resources-cache-hit-ratios-percentage";

	@I18NMessage("File Activity")
	public static final String RESOURCES_FILE_ACTIVITY = "SQLdm.Labels.resources-file-activity";

	@I18NMessage("LOG SEND Q SIZE")
	public static final String LOG_SEND_Q = "SQLdm.Labels.transfer-rate-percentage";

	/*@I18NMessage("Transfer Rates (Redo and Log)")
	public static final String TRANSFER_RATE_PERCENTAGE = "SQLdm.Labels.transfer-rate-percentage";

	@I18NMessage("Redo Q Size")
	public static final String REDO_Q = "SQLdm.Labels.transfer-rate-percentage";*/

	@I18NMessage("QUEUE SIZE (REDO AND LOG)")
	public static final String QUEUE_SIZE = "SQLdm.Labels.queue-size";

	@I18NMessage("SESSION ACTIVITY")
	public static final String SESSION_ACTIVITY = "SQLdm.Messages.session-activity";

	@I18NMessage(" RESPONSE TIME (ms)")
	public static final String RESPONSE_TIME_MS = "SQLdm.Labels.response-time-ms";

	@I18NMessage("Response Time (ms)")
	public static final String RESPONSE_TIME_MS_CC = "SQLdm.Labels.response-time-ms-camelcase";

	@I18NMessage("An error occurred loading this chart.")
	public static final String ERROR_OCCURRED_LOADING_CHART = "SQLdm.Messages.error_occurred_loading_chart";

	@I18NMessage("SQL SERVER CPU ACTIVITY (%)")
	public static final String SQL_SERVER_CPU_ACTIVITY = "SQLdm.Messages.sql-server-cpu-activity";

	@I18NMessage("DISK BUSY (%)")
	public static final String DISK_BUSY = "SQLdm.Messages.disk-busy";

	@I18NMessage("OS Memory Usage (GB)")
	public static final String OS_MEMORY_USAGE = "SQLdm.Messages.os-mem-usage";

	@I18NMessage("OS MEMORY USAGE (MB)")
	public static final String OS_MEMORY_USAGE_IN_MB = "SQLdm.Messages.os-mem-usage-in-mb";

	@I18NMessage("Data not available. Please try after some time.")
	public static final String DATA_NOT_AVAILABLE = "SQLdm.Labels.data-not-available";

	@I18NMessage("No data available. Please try after some time.")
	public static final String QUERIES_DATA_NOT_AVAILABLE = "SQLdm.Labels.queries-data-not-available";

	@I18NMessage("No data available for query plan.")
	public static final String QUERY_PLAN_DATA_NOT_AVAILABLE = "SQLdm.Labels.query-plan-data-not-available";

	@I18NMessage("No Data Available.")
	public static final String NO_DATA_AVAILABLE = "SQLdm.Labels.no-data-available";

	@I18NMessage("No data available.")
	public static final String NO_DATA_AVAILABLE_SENTENCE_CASE = "SQLdm.Labels.no-data-available-sentence-case";

	@I18NMessage("Failed to retrieve alerts.")
	public static final String ERROR_OCCURRED_FETCHING_ALERTS = "SQLdm.Messages.error_occurred_fetching_alerts";

	@I18NMessage("Failed to retrieve single alert.")
	public static final String ERROR_OCCURRED_SINGLE_ALERT = "SQLdm.Messages.error_occurred_single_alert";

	@I18NMessage("Failed to retrieve all metrics data")
	public static final String ERROR_FETCHING_ALL_METRICS = "SQLdm.Messages.error_occurred_fetching_metrics";

	@I18NMessage("Failed to retrieve alert history")
	public static final String ERROR_FETCHING_ALERT_HISTORY = "SQLdm.Messages.error_occurred_alert_history";

	@I18NMessage("ID")
	public static final String SESSION_Id = "SQLdm.Labels.sessionId";

	@I18NMessage("File ID")
	public static final String FILE_ID = "SQLdm.Labels.fileID";

	@I18NMessage("Session ID")
	public static final String SESSION_ID = "SQLdm.Labels.sessionID";

	@I18NMessage("CPU Usage")
	public static final String SESSION_CPU_USAGE = "SQLdm.Labels.sessionCPUUsage";

	@I18NMessage("TOP SERVERS BY CPU USAGE")
	public static final String SQL_CPU_LOAD = "SQLdm.Labels.sql-cpu-load";

	@I18NMessage("Utilization (%)")
	public static final String UTILIZATION = "SQLdm.Labels.utilization";

	@I18NMessage("CPU Usage")
	public static final String CPU_UTILIZATION = "SQLdm.Labels.cpu-utilization";

	@I18NMessage("Usage (KB)")
	public static final String MEMORY_USAGE = "SQLdm.Labels.memory-usage";

	@I18NMessage("Allocated (KB)")
	public static final String MEMORY_ALLOCATED = "SQLdm.Labels.memory-allocated";

	@I18NMessage("Response Time")
	public static final String RESPONSE_TIME = "SQLdm.Labels.response-time";

	@I18NMessage("(per sec)")
	public static final String PER_SEC = "SQLdm.Labels.per-sec";

	@I18NMessage("Physical I/O")
	public static final String PHYSICAL_IO = "SQLdm.Labels.physical-io";

	@I18NMessage("Transactions")
	public static final String TRANSACTION_VALUE = "SQLdm.Labels.transactional-value";

	@I18NMessage("Projected Growth Rate")
	public static final String PROJECTED_GROWTH_RATE = "SQLdm.Labels.projected-growth-rate";

	@I18NMessage("TOP SERVERS BY TEMPDB UTILIZATION")
	public static final String TEMPDB_UTILIZATION_MB = "SQLdm.Labels.tempdb-utilization-mb";

	@I18NMessage("TOP DATABASES BY SIZE")
	public static final String LARGEST_DATABASE_BY_SIZE = "SQLdm.Labels.largest-database-by-size";

	@I18NMessage("TOP DATABASES BY ACTIVITY")
	public static final String TOP_DATABASE_BY_ACTIVITY = "SQLdm.Labels.top-database-by-activity";

	@I18NMessage("TOP DATABASES BY GROWTH (LAST 7 DAYS)")
	public static final String FASTEST_PROJECTED_GROWING_DATABASES = "SQLdm.Labels.fastest-projected-growing-databases";

	@I18NMessage("Longest Running Alert")
	public static final String LONGEST_RUNNING_ALERTS = "SQLdm.Labels.longest-running-alerts";

	@I18NMessage("TOP QUERIES BY EXECUTION TIME")
	public static final String LONGEST_RUNNING_QUERIES = "SQLdm.Labels.longest-running-queries";

	@I18NMessage("Queries")
	public static final String NUMBER_OF_QUERIES = "SQLdm.Labels.number-of-queries";

	@I18NMessage("TOP SERVERS BY ALERTS")
	public static final String INSTANCE_ALERT = "SQLdm.Labels.top-ten.instance-alert";

	@I18NMessage("TOP DATABASES BY ALERTS")
	public static final String DATABSE_MOST_ALERT = "SQLdm.Labels.top-ten.database-most-alert";

	@I18NMessage("TOP SERVERS BY QUERIES")
	public static final String QUERY_EVENTS = "SQLdm.Labels.top-ten.query-events";

	@I18NMessage("TOP SERVERS BY SESSIONS")
	public static final String SESSION_NUMBERS = "SQLdm.Labels.top-ten.session-number";

	@I18NMessage("TOP SERVERS BY WAITS")
	public static final String TOPX_WAITS = "SQLdm.Labels.top-ten.waits";

	@I18NMessage("TOP SERVERS BY BLOCKED SESSIONS")
	public static final String TOPX_BLOCKED_SESSION = "SQLdm.Labels.top-ten.blocked-sessions";

	@I18NMessage("TOP SERVERS BY I/O")
	public static final String TOPX_IO = "SQLdm.Labels.top-ten.io";

	@I18NMessage("Widget Configuration")
	public static final String WIDGET_CONFIGURATION = "SQLdm.Labels.widget-conf";

	@I18NMessage("Disk Space Utilization")
	public static final String DISK_SPACE_UTILIZATION = "SQLdm.Labels.disk-space-utilization";

	@I18NMessage("TOP SERVERS BY DISK SPACE UTILIZATION")
	public static final String DISK_SPACE_UTILIZATION_PERCENTAGE = "SQLdm.Labels.disk-space-utilization-percentage";

	@I18NMessage("TOP SERVERS BY RESPONSE TIME")
	public static final String TOP_TEN_RESPONSE_TIME = "SQLdm.Labels.topten.responseTime";

	@I18NMessage("Instance")
	public static final String TOP_TEN_INSTANCE = "SQLdm.Labels.topten.instance";

	@I18NMessage("TOP SERVERS BY MEMORY USAGE")
	public static final String TOP_TEN_MEMORY = "SQLdm.Labels.topten.memory";

	@I18NMessage("CPU Time (ms)")
	public static final String CPU_TIME = "SQLdm.Labels.cpu-time";

	@I18NMessage("Physical Reads")
	public static final String PHYSICAL_READS = "SQLdm.Labels.physical-reads";

	@I18NMessage("Logical Reads")
	public static final String LOGICAL_READS = "SQLdm.Labels.logical-reads";

	@I18NMessage("Logical Writes")
	public static final String LOGICAL_WRITES = "SQLdm.Labels.logical-writes";

	@I18NMessage("Repository Name")
	public static final String REPOSITORY_NAME = "SQLdm.Labels.repository-name";

	@I18NMessage("Space Utilization (MB)")
	public static final String TEMPDB_SPACE_UTILIZATION = "SQLdm.Labels.tempdb-space-utilization";

	@I18NMessage("Size (MB)")
	public static final String FILE_SIZE = "SQLdm.Labels.file-size";

	@I18NMessage("Query")
	public static final String QUERY = "SQLdm.Labels.query";

	@I18NMessage("Query Exec Time")
	public static final String QUERY_TIME = "SQLdm.Labels.query-time";

	@I18NMessage("Query Name")
	public static final String QUERY_NAME = "SQLdm.Labels.query-name";

	@I18NMessage("SQL Text")
	public static final String SQL_TEXT = "SQLdm.Labels.sql-text";

	@I18NMessage("Descending")
	public static final String DESCENDING = "SQLdm.Labels.descending";

	@I18NMessage("Ascending")
	public static final String ASCENDING = "SQLdm.Labels.ascending";

	@I18NMessage("Growth (MB)")
	public static final String TOP_DATABASES_BY_GROWTH = "SQLdm.Labels.growth-over-7-days";

	@I18NMessage("Category")
	public static final String SERVER_CATEGORY = "SQLdm.Labels.serverwaits.category";

	@I18NMessage("SHOW CATEGORY DETAILS")
	public static final String CATEGORY_BUTTON = "SQLdm.Labels.alerts.category-button";

	@I18NMessage("SHOW INSTANCE DETAILS")
	public static final String INSTANCE_BUTTON = "SQLdm.Labels.alerts.instance-button";

	@I18NMessage("Database Name")
	public static final String DATABASE_NAME = "SQLdm.Labels.database-name";

	@I18NMessage("File Name")
	public static final String FA_FILE_NAME = "SQLdm.Labels.file-name";

	@I18NMessage("Reads / Sec")
	public static final String READS_PERSEC = "SQLdm.Labels.reads-per-sec";

	@I18NMessage("Writes /Sec")
	public static final String WRITES_PERSEC = "SQLdm.Labels.writes-per-sec";

	@I18NMessage("CPU Name")
	public static final String CPU_NAME = "SQLdm.Labels.cpu-name";

	@I18NMessage("Data Size (MB)")
	public static final String DATABASE_DATA_SIZE = "SQLdm.Labels.database.data.size";

	@I18NMessage("Data File Size (MB)")
	public static final String DATABASE_FILE_SIZE = "SQLdm.Labels.database.file.size";

	@I18NMessage("Log File Size (MB)")
	public static final String DATABASE_LOG_FILE_SIZE = "SQLdm.Labels.database.log.file.size";

	@I18NMessage("Log Size (MB)")
	public static final String DATABASE_LOG_SIZE = "SQLdm.Labels.database.log.size";

	@I18NMessage("Type")
	public static final String DATABASE_TYPE = "SQLdm.Labels.database.type";

	@I18NMessage("Database Size")
	public static final String DB_SIZE = "SQLdm.Labels.db-size";

	@I18NMessage("Database Size (MB)")
	public static final String DB_SIZE_IN_MB = "SQLdm.Labels.db-size-inmb";

	@I18NMessage("Description")
	public static final String DESCRIPTION = "SQLdm.Labels.description";

	@I18NMessage("Data File( Allocated GB)")
	public static final String ALLOCATED_DATA_FILE = "SQLdm.Labels.allocated-data-file";

	@I18NMessage("Data File( Used GB)")
	public static final String USED_DATA_FILE = "SQLdm.Labels.used-data-file";

	@I18NMessage("Log File (Allocated GB)")
	public static final String ALLOCATED_LOG_FILE = "SQLdm.Labels.allocated-log-file";

	@I18NMessage("Log File( Used GB)")
	public static final String USED_LOG_FILE = "SQLdm.Labels.used-log-file";

	@I18NMessage("Database count")
	public static final String DB_COUNT = "SQLdm.Labels.db-count";

	@I18NMessage("Open Transactions")
	public static final String SESSION_OPEN_TRANSACTION = "SQLdm.Labels.sessions.openTransactions";

	@I18NMessage("Command")
	public static final String SESSION_COMMAND = "SQLdm.Labels.sessions.command";

	@I18NMessage("Application")
	public static final String SESSION_APPLICATION = "SQLdm.Labels.sessions.application";

	@I18NMessage("Wait Time (ms)")
	public static final String SESSION_WAITTIME = "SQLdm.Labels.sessions.waitTime";

	@I18NMessage("Wait Type")
	public static final String SESSION_WAITTYPE = "SQLdm.Labels.sessions.waitType";

	@I18NMessage("Wait Resource")
	public static final String SESSION_RESOURCE = "SQLdm.Labels.sessions.waitResource";

	@I18NMessage("CPU (ms)")
	public static final String SESSION_CPU = "SQLdm.Labels.sessions.cpu";

	@I18NMessage("CPU Delta (ms)")
	public static final String SESSION_CPUDELTA = "SQLdm.Labels.sessions.cpuDelta";

	@I18NMessage("Physical I/O")
	public static final String SESSION_IO = "SQLdm.Labels.sessions.io";

	@I18NMessage("Memory")
	public static final String SESSION_MEMORY_USAGE = "SQLdm.Labels.sessions.memoryUsage";

	@I18NMessage("Login Time")
	public static final String SESSION_LOGIN_TIME = "SQLdm.Labels.sessions.loginTime";

	@I18NMessage("Last Batch")
	public static final String SESSION_LAST_BATCH = "SQLdm.Labels.sessions.lastBatch";

	@I18NMessage("Blocked By")
	public static final String SESSION_BLOCKEDBY = "SQLdm.Labels.sessions.blockedBy";

	@I18NMessage("Blocking")
	public static final String SESSION_BLOCKING = "SQLdm.Labels.sessions.blocking";

	@I18NMessage("Blocking Count")
	public static final String SESSION_COUNT = "SQLdm.Labels.sessions.blocking.count";

	@I18NMessage("Net Address")
	public static final String SESSION_ADDRESS = "SQLdm.Labels.sessions.address";

	@I18NMessage("Net Library")
	public static final String SESSION_LIBRARY = "SQLdm.Labels.sessions.netLibrary";

	@I18NMessage("Version Store Elapsed (sec)")
	public static final String SESSION_VERSION_STRORE_SEC = "SQLdm.Labels.sessions.versionStoreSec";

	@I18NMessage("Failed to retrieve Resources.")
	public static final String ERROR_OCCURRED_FETCHING_RESOURCES = "SQLdm.Messages.error_occurred_fetching_resources";
	
	@I18NMessage("Failed to save user settings.")
	public static final String ERROR_OCCURRED_SAVE_USER_SETTINGS = "SQLdm.Messages.error_occurred_save_user_settings";

	@I18NMessage("Failed to retrieve File drives.")
	public static final String ERROR_OCCURRED_FILE_DRIVES = "SQLdm.Messages.error_occurred_file_drives";

	@I18NMessage("Failed to retrieve File Activity")
	public static final String ERROR_FETCHING_FILE_ACTIVITY = "SQLdm.Messages.error_occurred_file_activity";

	@I18NMessage("Failed to retrieve Server Waits")
	public static final String ERROR_FETCHING_SERVER_WAIT = "SQLdm.Messages.error_occurred_server_wait";

	@I18NMessage("Failed to retrieve CPU Call Rates.")
	public static final String ERROR_OCCURRED_FETCHING_CPU_STATS = "SQLdm.Messages.error_occurred_fetching_cpu_stats";
	
	@I18NMessage("Failed to retrieve OS Paging.")
	public static final String ERROR_OCCURRED_FETCHING_OSPAGING_STATS = "SQLdm.Messages.error_occurred_fetching_os_paging";

	@I18NMessage("Failed to retrieve Lock waits.")
	public static final String ERROR_OCCURRED_FETCHING_LOCK_WAITS_STATS = "SQLdm.Messages.error_occurred_fetching_lock_waits";

	@I18NMessage("Failed to retrieve Database details.")
	public static final String ERROR_OCCURRED_FETCHING_DB_STATS = "SQLdm.Messages.error_occurred_fetching_db_details";
	
	@I18NMessage("Failed to retrieve Custom Counters.")
	public static final String ERROR_OCCURRED_FETCHING_CUSTOM_COUNTERS = "SQLdm.Messages.error_occurred_fetching_custom_counters";

	@I18NMessage("Failed to retrieve Network Details.")
	public static final String ERROR_OCCURRED_FETCHING_NETWORK_STATS = "SQLdm.Messages.error_occurred_fetching_network_stats";
	
	@I18NMessage("Failed to retrieve File Details.")
	public static final String ERROR_OCCURRED_FETCHING_FILE_STATS = "SQLdm.Messages.error_occurred_fetching_file_stats";
	
	@I18NMessage("Failed to retrieve Virtualization details.")
	public static final String ERROR_OCCURRED_FETCHING_VIRTUALIZATION_STATS = "SQLdm.Messages.error_occurred_fetching_Virtualization_details";

	
	@I18NMessage("SERVER WAITS")
	public static final String SERVER_WAITS = "SQLdm.Labels.serverwaits";

	@I18NMessage("Views")
	public static final String VIEWS = "SQLdm.Labels.views";

	@I18NMessage("VIEWS")
	public static final String VIEWS_CAPS = "SQLdm.Labels.views-capital";

	@I18NMessage("View Selection")
	public static final String VIEWS_SELECTION = "SQLdm.Labels.views-selection";

	@I18NMessage("Filtering")
	public static final String FILTERING = "SQLdm.Labels.query-filtering";

	@I18NMessage("View")
	public static final String QUERY_VIEW = "SQLdm.Labels.query-view";

	@I18NMessage("Group by")
	public static final String QUERY_GROUP = "SQLdm.Labels.query-group";

	@I18NMessage("Time")
	public static final String QUERY_TIME_PERIOD = "SQLdm.Labels.query-time-period";

	@I18NMessage("Start Date:")
	public static final String QUERY_START_DATE = "SQLdm.Labels.query-start-date";

	@I18NMessage("End Date:")
	public static final String QUERY_END_DATE = "SQLdm.Labels.query-end-date";

	@I18NMessage("Start Time:")
	public static final String QUERY_START_TIME = "SQLdm.Labels.query-start-time";

	@I18NMessage("End Time:")
	public static final String QUERY_END_TIME = "SQLdm.Labels.query-end-time";

	@I18NMessage("Application")
	public static final String FILTER_APPLICATION = "SQLdm.Labels.filter-application";

	@I18NMessage("Databases")
	public static final String FILTER_DATABASES = "SQLdm.Labels.filter-databases";

	@I18NMessage("Users")
	public static final String FILTER_USERS = "SQLdm.Labels.filter-users";

	@I18NMessage("Clients")
	public static final String FILTER_CLIENTS = "SQLdm.Labels.filter-clients";

	@I18NMessage("SQL")
	public static final String FILTER_SQL = "SQLdm.Labels.filter-sql";

	@I18NMessage("Advanced Filters")
	public static final String FILTER_ADVANCED_FILTERS = "SQLdm.Labels.filter-advanced";

	@I18NMessage("No applications available for this instance.")
	public static final String NO_APPLICATIONS_AVAILABLE = "SQLdm.Messages.no-applications-available";

	@I18NMessage("No databases available for this instance.")
	public static final String NO_DATABASES_AVAILABLE = "SQLdm.Messages.no-databases-available";

	@I18NMessage("No clients available for this instance.")
	public static final String NO_CLIENTS_AVAILABLE = "SQLdm.Messages.no-clients-available";

	@I18NMessage("No users available for this instance.")
	public static final String NO_USERS_AVAILABLE = "SQLdm.Messages.no-users-available";

	@I18NMessage("BY STATUS")
	public static final String VIEW_BY_STATUS = "SQLdm.Labels.view-by-status";

	@I18NMessage("No Tags are registered.")
	public static final String NO_TAGS_REGISTERED = "SQLdm.Messages.no-tags-registered";

	@I18NMessage("MOST CRITICAL INSTANCES")
	public static final String VIEW_CRTICAL_INSTANCES = "SQLdm.Labels.view-critical-instances";

	@I18NMessage("BY TAGS")
	public static final String VIEW_BY_GROUP = "SQLdm.Labels.view-by-group";

	@I18NMessage("CRITICAL,")
	public static final String DASHBOARD_CRITICAL = "SQLdm.Labels.dashboard-critical";

	@I18NMessage("WARNING,")
	public static final String DASHBOARD_WARNING = "SQLdm.Labels.dashboard-warning";

	@I18NMessage("OK,")
	public static final String DASHBOARD_OK = "SQLdm.Labels.dashboard-ok";

	@I18NMessage("MAINTENANCE MODE")
	public static final String DASHBOARD_MAINTENANCE_MODE = "SQLdm.Labels.dashboard-maintenance-mode";

	@I18NMessage("Items per page")
	public static final String PAGINATION_ITEMS_PER_PAGE = "SQLdm.Labels.pagination-items-per-page";

	@I18NMessage("Instance")
	public static final String INSTANCE = "SQLdm.Labels.instance";

	@I18NMessage("Version")
	public static final String INSTANCE_DASHBOARD_VERSION = "SQLdm.Labels.instance-dashboard-version";

	@I18NMessage("Status")
	public static final String INSTANCE_DASHBOARD_STATUS = "SQLdm.Labels.instance-dashboard-status";

	@I18NMessage("Agent Status")
	public static final String AGENT_STATUS = "SQLdm.Labels.instance-agent-status";

	@I18NMessage("DTC Status")
	public static final String DTC_STATUS = "SQLdm.Labels.instance-dtc-status";

	@I18NMessage("Available Memory (MB)")
	public static final String AVAILABLE_MEMORY = "SQLdm.Labels.instance-available-menory";

	@I18NMessage("Blocked Processes")
	public static final String BLOCKED_PROCESSES = "SQLdm.Labels.instance-blocked-processes";

	@I18NMessage(" Blocked Sessions")
	public static final String BLOCKED_SESSIONS = "SQLdm.Labels.instance-blocked-sessions";

	@I18NMessage("CPU USAGE (ALERTS)")
	public static final String GRAPH_CPU_USAGE = "SQLdm.Labels.graph-cpu-usage";

	@I18NMessage("DISK READS PER SECOND")
	public static final String GRAPH_DISK_READS = "SQLdm.Labels.graph-disk-reads";

	@I18NMessage("DISK WRITES PER SECOND")
	public static final String GRAPH_DISK_WRITES = "SQLdm.Labels.graph-disk-writes";

	@I18NMessage("DISK TRANSFERS PER SECOND")
	public static final String GRAPH_DISK_TRANSFERS = "SQLdm.Labels.graph-disk-transfers";

	@I18NMessage("TOP SESSIONS BY CPU USAGE")
	public static final String TOPX_SESSION_CPU_USAGE = "SQLdm.Labels.session-cpu-usage";

	@I18NMessage("TOP SESSIONS BY I/O ACTIVITY")
	public static final String TOPX_SESSION_IO_ACTIVITY = "SQLdm.Labels.session-io-activity";

	@I18NMessage("TOP FILES BY I/O ACTIVITY")
	public static final String TOPX_FILES_IO_ACTIVITY = "SQLdm.Labels.files-io-activity";

	@I18NMessage("TOP SESSIONS BY ACTIVE  CPU")
	public static final String TOPX_SESSIONS_ACTIVE_CPU = "SQLdm.Labels.sessions-active-cpu";

	@I18NMessage("CPU Activity")
	public static final String CPU_ACTIVITY = "SQLdm.Labels.instance-cpu-activity";

	@I18NMessage("Disk Queue Length")
	public static final String DISK_QUEUE_LIST = "SQLdm.Labels.instance-disk-queue-length";

	@I18NMessage("Active Alerts")
	public static final String ACTIVE_ALERTS = "SQLdm.Labels.active-alert";

	@I18NMessage("No Active Alerts.")
	public static final String NO_ACTIVE_ALERTS = "SQLdm.Messages.dashboard-no-alerts";

	@I18NMessage("No Alerts found for the selected filters")
	public static final String NO_ALERTS = "SQLdm.Messages.alerts-no-alerts";

	@I18NMessage("Time")
	public static final String ALERT_DATE = "SQLdm.Labels.alert-date";

	@I18NMessage("SUMMARY")
	public static final String SUMMARY = "SQLdm.Labels.summary";

	@I18NMessage("Summary")
	public static final String SUMMARY_CC = "SQLdm.Labels.summary-cc";

	@I18NMessage("TEMPDB")
	public static final String TEMPDB = "SQLdm.Labels.tempdb";

	@I18NMessage("OVERVIEW")
	public static final String OVERVIEW = "SQLdm.Labels.sid.overview";

	@I18NMessage("SESSIONS")
	public static final String SESSIONS = "SQLdm.Labels.sid.sessions";

	@I18NMessage("QUERIES")
	public static final String QUERIES = "SQLdm.Labels.sid.queries";

	@I18NMessage("RESOURCES")
	public static final String RESOURCES = "SQLdm.Labels.sid.resources";

	@I18NMessage("AVAILABILITY GROUPS")
	public static final String AVAILABILITY_GRPS = "SQLdm.Labels.availability-grps";

	@I18NMessage("QUERY WAITS")
	public static final String QUERY_WAITS = "SQLdm.Labels.sid.query-waits";

	@I18NMessage("CPU")
	public static final String CPU = "SQLdm.Labels.cpu";

	@I18NMessage("MEMORY")
	public static final String MEMORY = "SQLdm.Labels.memory";

	@I18NMessage("DISK")
	public static final String DISK = "SQLdm.Labels.disk";

	@I18NMessage("Severity")
	public static final String SEVERITY = "SQLdm.Labels.alert-severity";

	@I18NMessage("Description")
	public static final String ALERT_DESCRIPTION = "SQLdm.Labels.alert-description";

	@I18NMessage("Database")
	public static final String DATABASE = "SQLdm.Labels.database";

	@I18NMessage("No SQL Server instances have been registered.")
	public static final String INSTANCE_DASHBOARD_NO_INSTANCES_REGISTERED = "SQLdm.Messages.instance-dashboard-no-instances-registered";

	@I18NMessage("No Category Alerts found.")
	public static final String INSTANCE_DASHBOARD_NO_ALERTS_FOR_ALERTSBYCATEGORY = "SQLdm.Messages.instance-dashboard-no-alerts-for-alertsforcategory";

	@I18NMessage("No Database Alerts found.")
	public static final String INSTANCE_DASHBOARD_NO_ALERTS_FOR_ALERTSBYDATABASE = "SQLdm.Messages.instance-dashboard-no-alerts-for-alertsfordatabase";

	@I18NMessage("No Instance Alerts found.")
	public static final String INSTANCE_DASHBOARD_NO_ALERTS_FOR_ALERTSBYINSTANCE = "SQLdm.Messages.instance-dashboard-no-alerts-for-alertsforinstance";

	@I18NMessage("No Response Time Alerts found.")
	public static final String INSTANCE_DASHBOARD_NO_ALERTS_FOR_WORSTRESPONSETIME = "SQLdm.Messages.instance-dashboard-no-alerts-for-worstresponsetime";

	@I18NMessage("No instances match the selected view.")
	public static final String NO_INSTANCES_REGISTERED_FOR_SELECTED_FILTER = "SQLdm.Messages.no-instances-registered-for-selected-filter";

	@I18NMessage("No Queries been found for the instance.")
	public static final String INSTANCE_DASHBOARD_NO_QUERIES_FOUND = "SQLdm.Messages.no-query-found";

	@I18NMessage("Active Sessions")
	public static final String ACTIVE_SESSIONS = "SQLdm.Labels.active-sessions";

	@I18NMessage("TOP ALERT CATEGORIES")
	public static final String ALERTS_CATEGORY = "SQLdm.Labels.alerts-category";

	@I18NMessage("TOP DATABASES BY ALERTS")
	public static final String ALERTS_DATABASE = "SQLdm.Labels.alerts-database";

	@I18NMessage("TOP SERVERS BY ALERT COUNT")
	public static final String ALERTS_INSTANCES = "SQLdm.Labels.alerts-instances";

	@I18NMessage("TOP SERVERS BY RESPONSE TIME")
	public static final String WORST_RESPOSNE_TIME = "SQLdm.Labels.worst-response-time";

	@I18NMessage("Instance Name")
	public static final String INSTANCE_NAME = "SQLdm.Labels.instance-name";

	@I18NMessage("Value")
	public static final String VALUE = "SQLdm.Labels.value";

	@I18NMessage("Current alert time")
	public static final String CURRENT_ALERT_TIME = "SQLdm.Labels.current-alert-time";

	@I18NMessage("Warning Threshold")
	public static final String WARNING_THRESHOLD = "SQLdm.Labels.warning-threshold";

	@I18NMessage("Critical Threshold")
	public static final String CRITICAL_THRESHOLD = "SQLdm.Labels.critical-threshold";

	@I18NMessage("Critical Alerts")
	public static final String CRITICAL_ALERTS = "SQLdm.Labels.critical-alerts";

	@I18NMessage("Warning Alerts")
	public static final String WARNING_ALERTS = "SQLdm.Labels.warning-alerts";

	@I18NMessage("Informational Alerts")
	public static final String INFORMATIONAL_ALERTS = "SQLdm.Labels.informational-alerts";

	@I18NMessage("4 Hours")
	public static final String FOUR_HOURS = "SQLdm.Labels.four-hours";

	@I18NMessage("24 Hours")
	public static final String TWENTY_FOUR_HOURS = "SQLdm.Labels.twenty-four-hours";

	@I18NMessage("7 Days")
	public static final String SEVEN_DAYS = "SQLdm.Labels.seven-days";

	@I18NMessage("Metric History (Last 4 hours)")
	public static final String METRIC_HISTORY_FOUR_HOUR = "SQLdm.Labels.history.four-hours";

	@I18NMessage("Metric History (Last 7 days)")
	public static final String METRIC_HISTORY_SEVEN_DAYS = "SQLdm.Labels.history.seven-days";

	@I18NMessage("Metric History (Last 24 hours)")
	public static final String METRIC_HISTORY_ONE_DAY = "SQLdm.Labels.history.twenty-four-hours";

	@I18NMessage("Hide Thresholds")
	public static final String HIDE_THRESHOLDS = "SQLdm.Labels.hide-thresholds";

	@I18NMessage("Minimum Value")
	public static final String MINIMUM_VALUE = "SQLdm.Labels.minimum-value";

	@I18NMessage("Maximum Value")
	public static final String MAXIMUM_VALUE = "SQLdm.Labels.maximum-value";

	@I18NMessage("Alert Details")
	public static final String ALERT_DETAIL = "SQLdm.Labels.alert.detail";

	@I18NMessage("Expired License")
	public static final String LICENSE_STATUS_EXPIRED = "SQLdm.Labels.license.status.expired";

	@I18NMessage("Trial Copy. Valid until {0}.")
	public static final String LICENSE_TRIAL_LICENSE_STATUS = "SQLdm.Labels.license.trial.license.status";

	@I18NMessage("License will expire in {0} day(s)")
	public static final String LICENSE_PERMANENT_LICENSE_STATUS = "SQLdm.Labels.license.permananet.license.status";

	@I18NMessage("An exception occurred getting license details.")
	public static final String EXCEPTION_OCCURRED_GETTING_LICENSE_DETAILS = "SQLdm.Messages.exception-occurred-getting-license-details";

	@I18NMessage("No data available.")
	public static final String CHART_HAS_NO_DATA = "SQLdm.Messages.chart-has-no-data";

	@I18NMessage("Not specified")
	public static final String NOT_SPECIFIED = "SQLdm.Labels.not-specified";

	@I18NMessage("No Tags Found")
	public static final String NO_TAGS_FOUND = "SQLdm.Labels.no-tags-found";

	@I18NMessage("An error occurred loading the tags.")
	public static final String ERROR_OCCURRED_LOADING_TAGS = "SQLdm.Messages.error-occurred-loading-tags";

	@I18NMessage("Unlimited")
	public static final String UNLIMITED = "SQLdm.Labels.unlimited";

	@I18NMessage("Yes")
	public static final String YES = "SQLdm.Labels.UI.yes";

	@I18NMessage("No")
	public static final String NO = "SQLdm.Labels.UI.no";

	@I18NMessage("Enabled")
	public static final String ENABLED = "SQLdm.Labels.enabled";

	@I18NMessage("Disabled")
	public static final String DISABLED = "SQLdm.Labels.disabled";

	@I18NMessage("Disable")
	public static final String DISABLE = "SQLdm.Labels.disable";

	@I18NMessage("Enable")
	public static final String ENABLE = "SQLdm.Labels.enable";

	@I18NMessage("N/A")
	public static final String N_A = "SQLdm.Labels.n_a";

	@I18NMessage("DATABASES")
	public static final String DATABASES = "SQLdm.Labels.databases";

	@I18NMessage("Loading Data...")
	public static final String LOADING_DATA = "SQLdm.Messages.loading-data";

	@I18NMessage("No Data available")
	public static final String NO_DATA = "SQLdm.Messages.no-data";

	@I18NMessage("No Critical Instances found.")
	public static final String NO_CRITICAL_INSTANCES_FOUND = "SQLdm.Messages.no-critical-instances-found";

	@I18NMessage("No Data Found.")
	public static final String NO_DATA_FOUND = "SQLdm.Messages.no-data-found";

	@I18NMessage("Statements By Duration (per second)")
	public static final String STATEMENTS_BY_DURATION = "SQLdm.Messages.statementsbyduration";

	@I18NMessage("Statements By Wait Time (per second)")
	public static final String STATEMENTS_BY_WAITTIME = "SQLdm.Messages.statementsbywaittime";

	@I18NMessage("Application By Duration (per second)")
	public static final String APPLICATION_BY_DURATION = "SQLdm.Messages.applicationbyduration";

	@I18NMessage("Application By Wait Time (per second)")
	public static final String APPLICATION_BY_WAITTIME = "SQLdm.Messages.applicationbywaittime";

	@I18NMessage("Database By Duration (per second)")
	public static final String DATABASE_BY_DURATION = "SQLdm.Messages.databasebyduration";

	@I18NMessage("Database By Wait Time (per second)")
	public static final String DATABASE_BY_WAITTIME = "SQLdm.Messages.databasebywaittime";

	@I18NMessage("Client By Duration (per second)")
	public static final String CLIENT_BY_DURATION = "SQLdm.Messages.clientbyduration";

	@I18NMessage("Client By Wait Time (per second)")
	public static final String CLIENT_BY_WAITTIME = "SQLdm.Messages.clientbywaittime";

	@I18NMessage("Sessions By Duration (per second)")
	public static final String SESSIONS_BY_DURATION = "SQLdm.Messages.sessionsbyduration";

	@I18NMessage("Sessions By Wait Time (per second)")
	public static final String SESSIONS_BY_WAITTIME = "SQLdm.Messages.sessionsbywaittime";

	@I18NMessage("Users By Duration (per second)")
	public static final String USERS_BY_DURATION = "SQLdm.Messages.usersbyduration";

	@I18NMessage("Users By Wait Time (per second)")
	public static final String USERS_BY_WAITTIME = "SQLdm.Messages.usersbywaittime";

	@I18NMessage("All Wait Types")
	public static final String ALL_WAIT_TYPES = "SQLdm.Messages.all.wait.types";

	@I18NMessage("ALL WAIT TYPES (ms)")
	public static final String ALL_WAIT_TYPES_IN_MS = "SQLdm.Messages.all.wait.types.in.ms";

	@I18NMessage("Indexes (MB)")
	public static final String DATABASE_DATA_INDEXES_SIZE_MB = "SQLdm.Labels.database.data.indexes.size.mb";
	
	@I18NMessage("Text (MB)")
	public static final String DATABASE_DATA_TEXT_SIZE_MB = "SQLdm.Labels.database.data.text.size.mb";
	
	@I18NMessage("Tables (MB)")
	public static final String DATABASE_DATA_TABLES_SIZE_MB = "SQLdm.Labels.database.data.tables.size.mb";
	
	@I18NMessage("Unused (MB)")
	public static final String DATABASE_DATA_UNUSED_SIZE_MB = "SQLdm.Labels.database.data.unused.size.mb";
	
	@I18NMessage("Indexes (%)")
	public static final String DATABASE_DATA_INDEXES_SIZE_PERCENT = "SQLdm.Labels.database.data.indexes.size.percent";
	
	@I18NMessage("Text (%)")
	public static final String DATABASE_DATA_TEXT_SIZE_PERCENT = "SQLdm.Labels.database.data.text.size.percent";
	
	@I18NMessage("Tables (%)")
	public static final String DATABASE_DATA_TABLES_SIZE_PERCENT = "SQLdm.Labels.database.data.tables.size.percent";
	
	@I18NMessage("Unused (%)")
	public static final String DATABASE_DATA_UNUSED_SIZE_PERCENT = "SQLdm.Labels.database.data.unused.size.percent";
	
	@I18NMessage("Data Unused (MB)")
	public static final String DATABASE_DATA_UNUSED_SIZE = "SQLdm.Labels.database.data.unused.size";

	@I18NMessage("Log Unused (MB)")
	public static final String DATABASE_UNUSED_LOG_SIZE = "SQLdm.Labels.database.log.unused.size";

	@I18NMessage("Log Used (MB)")
	public static final String DATABASE_USED_LOG_SIZE = "SQLdm.Labels.database.log.used.size";

	@I18NMessage("Log Unused (%)")
	public static final String DATABASE_UNUSED_LOG_SIZE_PERCENT = "SQLdm.Labels.database.log.unused.size.percent";

	@I18NMessage("Log Used (%)")
	public static final String DATABASE_USED_LOG_SIZE_PERCENT = "SQLdm.Labels.database.log.used.size.percent";
	
	@I18NMessage("CAPACITY USAGE FOR ")
	public static final String CAPACITY_USAGE = "SQLdm.Labels.capacity.usage";

	@I18NMessage("Out of Range (1-999)")
	public static final String PAGE_SIZE_ERROR = "SQLdm.Messages.page-size-error";

	@I18NMessage("Alerts")
	public static final String NUMBER_OF_AlERTS = "SQLdm.Labels.number-of-alerts";

	@I18NMessage("All Records")
	public static final String ALL_RECORDS = "SQLdm.Labels.all-records";

	@I18NMessage("# of Records")
	public static final String NO_OF_RECORDS = "SQLdm.Labels.topten.noOfRecords";

	@I18NMessage("Status")
	public static final String STATUS = "SQLdm.Labels.status";

	@I18NMessage("Recovery Model")
	public static final String DATABASE_RECOVERY_MODEL = "SQLdm.Labels.database.recoverymodel";

	@I18NMessage("Date Created")
	public static final String DATABASE_DATE_CREATED = "SQLdm.Labels.database.datecreated";

	@I18NMessage("Creation Date")
	public static final String DATABASE_CREATION_DATE = "SQLdm.Labels.database.creationdate";

	@I18NMessage("Last Backup")
	public static final String DATABASE_LASTBACKUP_DATE = "SQLdm.Labels.database.lastbackup";

	@I18NMessage("Files")
	public static final String DATABASE_FILES = "SQLdm.Labels.database.files";

	@I18NMessage("No database information available")
	public static final String NO_DATABASE_INFORMATION_AVAILABLE = "SQLdm.Labels.no-database-information-available";

	@I18NMessage("No databases have any transaction activity.")
	public static final String NO_DATABASES_HAVE_ANY_TRANSACTION_ACTIVITY = "SQLdm.Messages.no-databases-have-any-transaction-activity";

	@I18NMessage("Currently showing only 1500 alerts for the selected filters. Please adjust your filters to see more relevant results.")
	public static final String MORE_ALERTS_AVAILABLE = "SQLdm.Messages.more-alerts-available";

	@I18NMessage("User Sessions")
	public static final String USER_SESSION = "SQLdm.Labels.user-session";

	@I18NMessage("SQL CPU Usage")
	public static final String SQL_CPU_USAGE = "SQLdm.Labels.sql-cpu-usage";

	@I18NMessage("SQL Memory Usage")
	public static final String SQL_MEMORY_USAGE = "SQLdm.Labels.sql-memory-usage";

	@I18NMessage("SQL Disk I/O")
	public static final String SQL_DISK_IO = "SQLdm.Labels.sql-disk-io";

	@I18NMessage("Instance: ")
	public static final String ALERTS_INSTANCE_FILTER = "SQLdm.Labels.alerts.filter.instance";

	@I18NMessage("Metric: ")
	public static final String ALERTS_METRIC_FILTER = "SQLdm.Labels.alerts.filter.metrics";

	@I18NMessage("Severity: ")
	public static final String ALERTS_SEVERITY_FILTER = "SQLdm.Labels.alerts.filter.severity";
	
	@I18NMessage("SQLDM : ")
	public static final String ALERTS_REPO_FILTER = "SQLdm.Labels.alerts.filter.repo";

	@I18NMessage("Show active alerts")
	public static final String ALERTS_ACTIVE_FILTER = "SQLdm.Labels.alerts.filter.active";

	@I18NMessage("Show all alerts for the time span")
	public static final String ALERTS_TIME_FILTER = "SQLdm.Labels.alerts.filter.time";

	@I18NMessage("From :")
	public static final String ALERTS_DATE_FROM_FILTER = "SQLdm.Labels.alerts.filter.datefrom";

	@I18NMessage("To :")
	public static final String ALERTS_DATE_TO_FILTER = "SQLdm.Labels.alerts.filter.dateto";

	@I18NMessage("APPLY")
	public static final String ALERTS_FILTER_APPLY = "SQLdm.Labels.alerts.filter.apply";

	@I18NMessage("CLEAR FILTER")
	public static final String ALERTS_FILTER_CLEAR = "SQLdm.Labels.alerts.filter.clear";

	@I18NMessage("Response Time")
	public static final String RESPONSE_TIME_THUMBNAIL = "SQLdm.Labels.response-time-thumbnail";

	@I18NMessage("Active")
	public static final String ALERTS_VIEW_ACTIVE = "SQLdm.Labels.alerts.view.active";

	@I18NMessage("By Severity")
	public static final String ALERTS_VIEW_SEVERITY = "SQLdm.Labels.alerts.view.severity";

	@I18NMessage("By Instance")
	public static final String ALERTS_VIEW_INSTANCE = "SQLdm.Labels.alerts.view.instance";

	@I18NMessage("By Metric")
	public static final String ALERTS_VIEW_METRIC = "SQLdm.Labels.alerts.view.metric";

	@I18NMessage("By Category")
	public static final String ALERTS_VIEW_CATEGORY = "SQLdm.Labels.alerts.view.category";

	@I18NMessage("Custom")
	public static final String ALERTS_VIEW_CUSTOM = "SQLdm.Labels.alerts.view.custom";

	@I18NMessage("By SQLdm Repository")
	public static final String SQLDM_REPO_CUSTOM = "SQLdm.Labels.alerts.view.sqldmRepo";
	
	@I18NMessage("Current View")
	public static final String ALERTS_VIEW_CURRENT = "SQLdm.Labels.alerts.view.current";

	@I18NMessage("Alert Count")
	public static final String ALERT_COUNT = "SQLdm.Labels.alertcount";

	@I18NMessage("Alerts")
	public static final String ALERTS = "SQLdm.Labels.alerts";

	@I18NMessage("ALERTS")
	public static final String ALERTS_CAPITAL = "SQLdm.Labels.alerts_capital";

	@I18NMessage("Time")
	public static final String TIME = "SQLdm.Labels.time";

	@I18NMessage("Time (ms)")
	public static final String TIME_IN_MS = "SQLdm.Labels.timems";

	@I18NMessage("Refresh")
	public static final String REFRESH = "SQLdm.Tooltip.refresh";

	@I18NMessage("Heat Map View")
	public static final String HEAT_MAP_VIEW = "SQLdm.Tooltip.heatmap";

	@I18NMessage("Thumbnail View")
	public static final String THUMBNAIL_VIEW = "SQLdm.Tooltip.thumbnail";

	@I18NMessage("List View")
	public static final String LIST_VIEW = "SQLdm.Tooltip.listview";
	
	@I18NMessage("Heatmap Calculation Configuration")
	public static final String HEATMAP_CALCULATION_CONFIGURATION = "SQLdm.Tooltip.heatmapCalculationConfiguration";

	@I18NMessage("Search Instances..")
	public static final String SEARCH_INSTANCE = "SQLdm.Labels.serachinstance";

	@I18NMessage("Apply")
	public static final String APPLY = "SQLdm.Labels.apply";

	@I18NMessage("Group Name")
	public static final String AG_GROUP = "SQLdm.Labels.ag.group";

	@I18NMessage("Replica Name")
	public static final String AG_REPLICANAME = "SQLdm.Labels.ag.replica-name";

	@I18NMessage("Replica Role")
	public static final String AG_REPLICAROLE = "SQLdm.Labels.ag.replica-role";

	@I18NMessage("Synchronization Health")
	public static final String AG_HEALTH = "SQLdm.Labels.ag.sync-health";

	@I18NMessage("Redo Queue")
	public static final String AG_REDOSIZE = "SQLdm.Labels.ag.redo-size";

	@I18NMessage("Redo Rate")
	public static final String AG_REDORATE = "SQLdm.Labels.ag.redo-rate";

	@I18NMessage("Log Send Queue")
	public static final String AG_LOGSIZE = "SQLdm.Labels.ag.log-size";

	@I18NMessage("Log Rate")
	public static final String AG_LOGRATE = "SQLdm.Labels.ag.log-rate";

	@I18NMessage("Database Status")
	public static final String AG_DBSTATUS = "SQLdm.Labels.ag.db-status";

	@I18NMessage("Queue Size")
	public static final String AG_QUEUESIZE = "SQLdm.Labels.ag.queue-size";

	@I18NMessage("Transfer Rate (%)")
	public static final String AG_TXRATE = "SQLdm.Labels.ag.tx-rate";

	@I18NMessage("INSTANCE NAME")
	public static final String INSTANCE_NAME_CAPITAL = "SQLdm.Labels.instance-name-capital";

	@I18NMessage("INSTANCE DETAILS")
	public static final String INSTANCE_DETAILS_CAPITAL = "SQLdm.Labels.instance-details-capital";

	@I18NMessage("SERVER STATUS")
	public static final String SERVER_STATUS = "SQLdm.Labels.server-status-capital";

	@I18NMessage("SERVER PROPERTIES")
	public static final String SERVER_PROPERTIES = "SQLdm.Labels.server-properties-capital";

	@I18NMessage("SERVICE STATUS")
	public static final String SERVICE_STATUS = "SQLdm.Labels.service-status-capital";

	@I18NMessage("FILE USE")
	public static final String FILE_USE = "SQLdm.Labels.file-use-capital";

	@I18NMessage("Instance Status")
	public static final String INSTANCE_STATUS = "SQLdm.Labels.instance-status";

	@I18NMessage("Loading Name..")
	public static final String INSTANCE_NAME_LOADING = "SQLdm.Labels.instance-name-loading";

	@I18NMessage("Launch instance in SWA")
	public static final String SWA_LAUNCH_ICON_TOOLTIP = "SQLdm.Labels.swa-instance-launch-tooltip";
	
	@I18NMessage("DATABASES")
	public static final String INSTANCE_DATABASE_LABEL = "SQLdm.Labels.databases-label-capital";

	@I18NMessage("SESSIONS")
	public static final String SESSIONS_LABEL = "SQLdm.Labels.sessions-capital";

	@I18NMessage("Sessions")
	public static final String SESSIONS_LABEL_CAMELCASE = "SQLdm.Labels.sessions-camelcase";

	
	@I18NMessage("Tempdb SPACE USED BY FILE (MB).")
	public static final String TEMP_DB_BY_FILE_MB = "SQLdm.Labels.tempdb-used-file-mb";

	@I18NMessage("Tempdb SPACE USED OVER TIME (MB).")
	public static final String TEMP_DB_BY_TIME_MB = "SQLdm.Labels.tempdb-used-time-mb";

	@I18NMessage("VERSION STORE CLEANUP RATE (KB/Sec) .")
	public static final String VERSION_STORE_KILOBYTE_PER_SEC = "SQLdm.Labels.version-store-chart-kilobyte-per-sec";

	@I18NMessage("No Query data available.  To view Query & Waits data, enable collection of Query Monitoring for monitored instances.")
	public static final String TOPTEN_QUERY_DATA_NOT_AVAILABLE = "SQLdm.Labels.topten.query.nodata";

	@I18NMessage("No data available. To view Query & Waits data, enable collection of Query Monitoring for this instance.")
	public static final String DASHBOARD_QUERY_DATA_NOT_AVAILABLE = "SQLdm.Labels.dashboard.query.nodata";

	@I18NMessage("Query ID")
	public static final String QUERY_NUM = "SQLdm.Labels.query-num";

	@I18NMessage("Application")
	public static final String APPLICATION = "SQLdm.Labels.application";

	@I18NMessage("Occurrences")
	public static final String OCCURRENCES = "SQLdm.Labels.occurrences";

	@I18NMessage("Total \\n Duration (ms)")
	public static final String TOTAL_DURATION = "SQLdm.Labels.total.duration";

	@I18NMessage("Avg \\n Duration (ms)")
	public static final String AVG_DURATION = "SQLdm.Labels.avg.duration";

	@I18NMessage("Total CPU \\nTime (ms)")
	public static final String TOTAL_CPU_TIME = "SQLdm.Labels.total.cpu-time";

	@I18NMessage("Total Reads")
	public static final String TOTAL_READS = "SQLdm.Labels.total.reads";

	@I18NMessage("Total Writes")
	public static final String TOTAL_WRITES = "SQLdm.Labels.total.writes";

	@I18NMessage("Total IO")
	public static final String TOTAL_IO = "SQLdm.Labels.total.io";

	@I18NMessage("Total Wait \\nTime (ms)")
	public static final String TOTAL_WAIT_TIME = "SQLdm.Labels.total.wait-time";

	@I18NMessage("Most Recent\\nCompletion")
	public static final String MOST_RECENT_COMPLETION = "SQLdm.Labels.most-recent-completion";

	@I18NMessage("Total Blocking \\nTime (ms)")
	public static final String TOTAL_BLOCKING_TIME = "SQLdm.Labels.total.blocking-time";

	@I18NMessage("Total\\nDeadlocks")
	public static final String TOTAL_DEADLOCKS = "SQLdm.Labels.total.deadlocks";

	@I18NMessage("Avg CPU \\nTime (ms)")
	public static final String AVG_CPU_TIME = "SQLdm.Labels.avg.cpu-time";

	@I18NMessage("Avg CPU\\nPer Sec")
	public static final String AVG_CPU_PER_SEC = "SQLdm.Labels.avg.cpu-per-sec";

	@I18NMessage("Avg Reads")
	public static final String AVG_READS = "SQLdm.Labels.avg.reads";

	@I18NMessage("Avg Writes")
	public static final String AVG_WRITES = "SQLdm.Labels.avg.writes";

	@I18NMessage("Avg I/O")
	public static final String AVG_IO = "SQLdm.Labels.avg.io";

	@I18NMessage("Avg Wait\\nTime (ms)")
	public static final String AVG_WAIT_TIME = "SQLdm.Labels.avg.wait-time";

	@I18NMessage("Avg Blocking\\nTime (ms)")
	public static final String AVG_BLOCKING_TIME = "SQLdm.Labels.avg.blocking-time";

	@I18NMessage("Avg\\nDeadlocks")
	public static final String AVG_DEADLOCKS = "SQLdm.Labels.avg.deadlocks";

	@I18NMessage("CPU as\\n  % of list")
	public static final String CPU_AS_PERCENTAGE_LIST = "SQLdm.Labels.cpu-percentage-list";

	@I18NMessage("Reads as \\n%  of list")
	public static final String READS_AS_PERCENTAGE_LIST = "SQLdm.Labels.reads-percentage-list";

	@I18NMessage("Signature \\nSQL Text")
	public static final String SIGNATURE_SQL_TEXT = "SQLdm.Labels.signature-sql-text";

	@I18NMessage("Keep Detailed\\nHistory Flag")
	public static final String KEEP_DETAILED_HISTORY_FLAG = "SQLdm.Labels.detailed-history-flag";

	@I18NMessage("Aggregated")
	public static final String AGGREGATED = "SQLdm.Labels.aggregated";

	@I18NMessage("Query Statement")
	public static final String QUERY_STATEMENT = "SQLdm.Labels.query.statement";

	@I18NMessage("Query Signature")
	public static final String QUERY_SIGNATURE = "SQLdm.Labels.query.signature";

	@I18NMessage("Configure Top Servers by Alerts")
	public static final String CONFIGURE_INSTANCE_ALERT = "SQLdm.Labels.top-ten.configure-instance-alert";

	@I18NMessage("Configure row count")
	public static final String CONFIGURE_ROW = "SQLdm.Labels.top-ten.configure-row";

	@I18NMessage("Configure Top Databases by Alerts")
	public static final String CONFIGURE_DATABSE_MOST_ALERT = "SQLdm.Labels.top-ten.configure-database-most-alert";

	@I18NMessage("Configure Top Servers by Queries")
	public static final String CONFIGURE_QUERY_EVENTS = "SQLdm.Labels.top-ten.configure-query-events";

	@I18NMessage("Configure Top Servers by Sessions")
	public static final String CONFIGURE_SESSION_NUMBERS = "SQLdm.Labels.top-ten.configure-session-number";

	@I18NMessage("Configure Top Servers by Waits")
	public static final String CONFIGURE_TOPX_WAITS = "SQLdm.Labels.top-ten.configure-waits";

	@I18NMessage("Configure Top Servers by Blocked Sessions")
	public static final String CONFIGURE_TOPX_BLOCKED_SESSION = "SQLdm.Labels.top-ten.configure-blocked-sessions";

	@I18NMessage("Configure Top Servers by I/O")
	public static final String CONFIGURE_TOPX_IO = "SQLdm.Labels.top-ten.configure-io";

	@I18NMessage("Number of queries to display:  ")
	public static final String CONFIGURE_QUERIES_COUNT = "SQLdm.Labels.top-ten.configure-query-count";

	@I18NMessage("Number of servers to display:  ")
	public static final String CONFIGURE_SERVER_COUNT = "SQLdm.Labels.top-ten.configure-server-count";

	@I18NMessage("Number of sessions to display:  ")
	public static final String CONFIGURE_SESSIONS_COUNT = "SQLdm.Labels.top-ten.configure-sessions-count";

	@I18NMessage("Number of files to display:  ")
	public static final String CONFIGURE_FILES_COUNT = "SQLdm.Labels.top-ten.configure-files-count";

	@I18NMessage("Number of databases to display:  ")
	public static final String CONFIGURE_DB_COUNT = "SQLdm.Labels.top-ten.configure-db-count";

	@I18NMessage("Number of alert category to display:  ")
	public static final String CONFIGURE_ALERTCAT_COUNT = "SQLdm.Labels.top-ten.configure-alert-cat-count";

	@I18NMessage("Configure Top Sessions by CPU Usage")
	public static final String CONFIGURE_TOPX_SESSION_CPU_USAGE = "SQLdm.Labels.configure-session-cpu-usage";

	@I18NMessage("Configure Top Sessions by Active CPU")
	public static final String CONFIGURE_TOPX_SESSION_ACTIVE_CPU = "SQLdm.Labels.configure-session-active-cpu";

	@I18NMessage("Configure Top Sessions by I/O Activity")
	public static final String CONFIGURE_TOPX_SESSION_IO_ACTIVITY = "SQLdm.Labels.configure-session-io-activity";

	@I18NMessage("Configure Top Files by I/O Activity")
	public static final String CONFIGURE_TOPX_FILES_IO_ACTIVITY = "SQLdm.Labels.configure-files-io-activity";

	@I18NMessage("Configure Top Servers by Disk Space Utilization")
	public static final String CONFIGURE_DISK_SPACE_UTILIZATION_PERCENTAGE = "SQLdm.Labels.configure-disk-space-utilization-percentage";

	@I18NMessage("Configure Top Servers by Response Time")
	public static final String CONFIGURE_TOP_TEN_RESPONSE_TIME = "SQLdm.Labels.topten.configure-responseTime";

	@I18NMessage("Configure Top Servers by Memory Usage")
	public static final String CONFIGURE_TOP_TEN_MEMORY = "SQLdm.Labels.topten.configure-memory";

	@I18NMessage("Configure Top Servers by Active Connections")
	public static final String CONFIGURE_MOST_ACTIVE_USER_CONNECTION__BY_DATABASE = "SQLdm.Labels.configure-most-active-user-connections";

	@I18NMessage("Configure Top Servers by CPU Usage")
	public static final String CONFIGURE_SQL_CPU_LOAD = "SQLdm.Labels.configure-sql-cpu-load";

	@I18NMessage("Configure Top Servers by TempDB Utilization")
	public static final String CONFIGURE_TEMPDB_UTILIZATION_MB = "SQLdm.Labels.configure-tempdb-utilization-mb";

	@I18NMessage("Configure Top Queries by Execution Time")
	public static final String CONFIGURE_LONGEST_RUNNING_QUERIES = "SQLdm.Labels.configure-longest-running-queries";

	@I18NMessage("Configure Top Databases by Size")
	public static final String CONFIGURE_LARGEST_DATABASE_BY_SIZE = "SQLdm.Labels.configure-largest-database-by-size";

	@I18NMessage("Configure Top Databases by Activity")
	public static final String CONFIGURE_TOP_DATABASE_BY_ACTIVITY = "SQLdm.Labels.configure-top-database-by-activity";

	@I18NMessage("Configure Top Databases by Growth (Last 7 Days)")
	public static final String CONFIGURE_FASTEST_PROJECTED_GROWING_DATABASES = "SQLdm.Labels.configure-fastest-projected-growing-databases";

	@I18NMessage("Avg I/O\\nper sec")
	public static final String AVG_IO_PER_SEC = "SQLdm.Labels.avg.io-per-sec";

	@I18NMessage("Spid")
	public static final String SPID = "SQLdm.Labels.spid";

	@I18NMessage("Start Time")
	public static final String START_TIME = "SQLdm.Labels.start-time";

	@I18NMessage("Event Type")
	public static final String EVENT_TYPE = "SQLdm.Labels.event-type";

	@I18NMessage("Total OS MEMORY USAGE (MB)")
	public static final String TOTAL_OS_MEMORY_USAGE_IN_MB = "SQLdm.Messages.total-os-mem-in-mb";

	@I18NMessage("SESSION GRAPHS")
	public static final String SESSION_GRAPHS = "SQLdm.Labels.session-graphs";

	@I18NMessage("Zoom In")
	public static final String ZOOM_IN = "SQLdm.Labels.zoom-in";

	@I18NMessage("Zoom Out")
	public static final String ZOOM_OUT = "SQLdm.Labels.zoom-out";

	@I18NMessage("Zoom to Fit")
	public static final String ZOOM_TO_FIT = "SQLdm.Labels.zoom-to-fit";

	@I18NMessage("Export")
	public static final String EXPORT = "SQLdm.Labels.export";

	@I18NMessage("Reads")
	public static final String READS = "SQLdm.Labels.reads";

	@I18NMessage("Writes")
	public static final String WRITES = "SQLdm.Labels.writes";

	@I18NMessage("Wait Time (ms)")
	public static final String WAIT_TIME_MS = "SQLdm.Labels.wait-time-ms";

	@I18NMessage("Blocking Time (ms)")
	public static final String BLOCK_TIME_MS = "SQLdm.Labels.block-time-ms";

	@I18NMessage("Query Wait Duration Graph")
	public static final String QUERY_WAIT_GRAPH = "SQLdm.Labels.query.wait.duration";

	@I18NMessage("Actions")
	public static final String ACTIONS = "SQLdm.Labels.actions";

	@I18NMessage("Failed to get instance server statistics")
	public static final String FAILED_TO_GET_DASHBOARD_INSTANCE_SERVER_STATUS = "Labels.failed-to-get-dashboard-server-stats";

	@I18NMessage("Failed to get dashboard instances")
	public static final String FAILED_TO_GET_DASHBOARD_INSTANCES = "Labels.failed-to-get-dashboard-instances";

	@I18NMessage("Failed to get dashboard tags")
	public static final String FAILED_TO_GET_DASHBOARD_TAGS = "Labels.failed-to-get-dashboard-tags";

	@I18NMessage("Failed to get the instance.")
	public static final String FAILED_TO_GET_DASHBOARD_INSTANCE = "Errors.failed-to-get-instance";

	@I18NMessage("Failed to get alerts by category")
	public static final String FAILED_TO_GET_ALERTS_BY_CATEGORY = "Labels.failed-to-get-alerts-by-category";

	@I18NMessage("Failed to get alerts by database")
	public static final String FAILED_TO_GET_ALERTS_BY_DATABASE = "Labels.failed-to-get-alerts-by-database";

	@I18NMessage("An exception occurred enabling or disabling the instance.")
	public static final String EXCEPTION_OCCURRED_ENABLING_DISABLING_SQL_DM_INSTANCE = "Messages.exception-occurred-enabling-disabling-sqldm-instance";

	@I18NMessage("An exception occurred getting a SQL Elements instance.")
	public static final String EXCEPTION_OCCURRED_GETTING_A_SQLDM_INSTANCE = "Messages.exception-occurred-getting-a-sqldm-instance";

	@I18NMessage("An exception occurred getting the SQL diagnostic manager version.")
	public static final String EXCEPTION_OCCURRED_GETTING_SQL_DM_VERSION = "Messages.exception-occurred-getting-sqldm-version";

	@I18NMessage("An exception occurred getting SQL diagnostic manager instance versions.")
	public static final String EXCEPTION_OCCURRED_GETTING_SQL_DM_INSTANCE_VERSIONS = "Messages.exception-occurred-getting-sqlnovice-instance-versions";

	@I18NMessage("An exception occurred getting the SQL diagnostic manager service base URL.")
	public static final String EXCEPTION_OCCURRED_GETTING_SQL_DM_SERVICEBASEURL = "Messages.exception-occurred-getting-sqldm-servicebaseurl";

	@I18NMessage("You do not have permissions to view details of this alert.")
	public static final String EXCEPTION_OCCURRED_INVALID_INSTANCE = "Messages.exception-occurred-invalid-instance";

	@I18NMessage("An exception occurred getting SQLDM tags.")
	public static final String EXCEPTION_OCCURRED_GETTING_SQLDM_TAGS = "Messages.exception-occurred-getting-sqldm-tags";

	@I18NMessage("An exception occurred getting top databases by activity.")
	public static final String EXCEPTION_OCCURRED_GETTING_TOP_DATABASES_BY_ACTIVITY = "Messages.exception-occurred-getting-top-databases-by-activity";

	@I18NMessage("An exception occurred getting top databases by growth.")
	public static final String EXCEPTION_OCCURRED_GETTING_TOP_DATABASES_BY_GROWTH = "Messages.exception-occurred-getting-top-databases-by-growth";

	@I18NMessage("An exception occurred getting top databases by size.")
	public static final String EXCEPTION_OCCURRED_GETTING_TOP_DATABASES_BY_SIZE = "Messages.exception-occurred-getting-top-databases-by-size";

	@I18NMessage("An exception occurred getting all SQL diagnostic manager instances.")
	public static final String EXCEPTION_OCCURRED_GETTING_ALL_SQLDM_INSTANCES = "Messages.exception-occurred-getting-all-sqldm-instances";

	@I18NMessage("An exception occurred getting all SQL diagnostic manager tags.")
	public static final String EXCEPTION_OCCURRED_GETTING_ALL_SQLDM_TAGS = "Messages.exception-occurred-getting-all-sqldm-tags";

	@I18NMessage("An exception occurred getting database details.")
	public static final String EXCEPTION_OCCURRED_GETTING_DATABASE_DETAILS = "Messages.exception-occurred-getting-database-details";

	@I18NMessage("Exception in getting Alerts by Database for Widgets")
	public static final String EXCEPTION_OCCURRED_GETTING_ALERT_BY_DATABASE = "Messages.exception-occurred-getting-number-of-alerts-by-database";

	@I18NMessage("Exception in getting latest response time")
	public static final String EXCEPTION_OCCURRED_GETTING_LATEST_RESPONSE_TIME_BY_INSTANCE = "Messages.exception-occurred-getting-number-of-latest-response-time";

	@I18NMessage("Exception in getting Alerts by Category for Widgets")
	public static final String EXCEPTION_OCCURRED_GETTING_ALERT_BY_CATEGORY = "Messages.exception-occurred-getting-number-of-alerts-by-category";

	@I18NMessage("An exception occurred getting the instance configuration information.")
	public static final String EXCEPTION_OCCURRED_GETTING_INSTANCE_CONFIGURATION = "Messages.exception-occurred-getting-instance-configuration";

	@I18NMessage("An exception occurred getting the instance databases.")
	public static final String EXCEPTION_OCCURRED_GETTING_INSTANCE_DATABASES = "Messages.exception-occurred-getting-instance-databases";

	@I18NMessage("An exception occurred getting the SQL Server instance.")
	public static final String EXCEPTION_OCCURRED_GETTING_INSTANCE = "Messages.exception-occurred-getting-instance";

	@I18NMessage("An exception occurred getting the SQL Server server stats.")
	public static final String EXCEPTION_OCCURRED_GETTING_INSTANCE_SERVER_STATS = "Messages.exception-occurred-getting-server-stats";

	@I18NMessage("An exception occurred getting the instance overview information.")
	public static final String EXCEPTION_OCCURRED_GETTING_INSTANCE_OVERVIEW = "Messages.exception-occurred-getting-instance-overview";

	@I18NMessage("An exception occurred getting the instance session.")
	public static final String EXCEPTION_OCCURRED_GETTING_INSTANCE_SESSION = "Messages.exception-occurred-getting-instance-session";

	@I18NMessage("An unknown error occurred in the service. "
			+ "Contact your Product administrator to get the issue resolved.")
	public static final String ERROR_MESSAGE = "Messages.error-message";

	@I18NMessage("An exception occurred getting the product configuration.")
	public static final String EXCEPTION_OCCURRED_GETTING_PRODUCT_CONFIGURATION = "Messages.exception-occurred-getting-product-configuration";

	@I18NMessage("An exception occurred setting the product configuration.")
	public static final String EXCEPTION_OCCURRED_SETTING_PRODUCT_CONFIGURATION = "Messages.exception-occurred-setting-product-configuration";

	@I18NMessage("Unable to communicate with SQL diagnostic manager service. SQL diagnostic manager was not found as a registered application.")
	public static final String UNABLE_TO_COMMUNICATE_WITH_SQL_DM_SERVICE_NOT_FOUND_AS_REGISTERED_APPLICATION = "Messages.unable-to-communicate-with-sqldm-service-not-found-as-registered-application";

	@I18NMessage("Failed to query data.")
	public static final String FAILED_TO_QUERY_DATA = "Errors.failed-to-query-data";

	@I18NMessage("Failed to close file")
	public static final String FAILED_TO_CLOSE_FILE = "Messages.failed-to-close-file";

	@I18NMessage("Failed to read file")
	public static final String FAILED_TO_READ_FILE = "Messages.failed-to-read-file";

	@I18NMessage("An exception occurred refreshing the instance information.")
	public static final String EXCEPTION_OCCURRED_REFRESHING_INSTANCE_INFORMATION = "Messages.exception-occurred-refreshing-instance-information";

	@I18NMessage("An exception occurred getting SQL instance counts.")
	public static final String EXCEPTION_OCCURRED_GETTING_SQL_INSTANCE_COUNTS = "Messages.exception-occurred-getting-sql-instance-counts";

	@I18NMessage("An exception occurred getting SQL diagnostic manager service status.")
	public static final String EXCEPTION_OCCURRED_GETTING_SQL_DM_SERVICE_STATUS = "Messages.exception-occurred-getting-sql-dm-service-status";

	@I18NMessage("Failed to get dashboard severity groups")
	public static final String FAILED_TO_GET_DASHBOARD_SEVERITY_GROUP = "Labels.failed-to-get-dashboard-severity_group";

	@I18NMessage("SQL diagnostic manager")
	public static final String SQLDM = "Messages.Main.sqldm";

	@I18NMessage("SQL diagnostic manager cannot run because some application requirements are not met.")
	public static final String SQL_DM_APPLICATION_REQUIREMENT_NOT_MET = "Labels.sqldm-application-requirement-not-met";

	@I18NMessage("The service response about its status was empty.")
	public static final String SERVICE_RESPONSE_ABOUT_STATUS_EMPTY = "Labels.service-response-about-status-empty";

	@I18NMessage("Total Waits")
	public static final String TOTAL_WAITS = "Labels.serverwaits.totalwaits";
	
	@I18NMessage("Signal Waits")
	public static final String SIGNAL_WAITS = "Labels.serverwaits.signalwaits";
	
	@I18NMessage("Resource Waits")
	public static final String RESOURCE_WAITS = "Labels.serverwaits.resourcewaits";
	
	@I18NMessage("EXPORT")
	public static final String EXPORT_UC = "Labels.export_uc";
	
	@I18NMessage("Unable to get current product.")
	public static final String CURRENT_PRODUCT_IS_NULL = "Labels.sqlbi.current-product-is-null";

    @I18NMessage("SQL DM")
    public static final String PRODUCT_NAME = "Labels.UI.product-name";
    
    @I18NMessage("Launch DM Console")
	public static final String DM_CONSOLE_BUTTON = "SQLdm.Labels.launch-dm";
    
    @I18NMessage("NO DATABASE SELECTED")
    public static final String NO_DATABASE_SELECTED = "Labels.no-database-selected";
    
    @I18NMessage("Some error occured. Please refresh the page.")
	public static final String ERROR_MESSAGE_QUERIES = "Messages.Error.Queries";
    
    @I18NMessage("Save")
	public static final String SAVE = "SQLdm.Labels.save";
    
    @I18NMessage("Cancel")
	public static final String CANCEL = "SQLdm.Labels.cancel";
    
    @I18NMessage("Default")
	public static final String DEFAULT = "SQLdm.Labels.default";
    
    @I18NMessage("Data Source")
	public static final String MATCH = "SQLdm.Labels.match";
    
    @I18NMessage("Metric")
	public static final String METRIC = "SQLdm.Labels.metric";
    
    @I18NMessage("Widget")
	public static final String WIDGET = "SQLdm.Labels.widget";
    
    @I18NMessage("Label")
	public static final String LABEL = "SQLdm.Labels.label";
    
    @I18NMessage("Edit")
	public static final String EDIT = "SQLdm.Labels.edit";
    
    @I18NMessage("Remove")
	public static final String REMOVE = "SQLdm.Labels.remove";
    
    @I18NMessage("Close")
	public static final String CLOSE = "SQLdm.Labels.close";
    
    @I18NMessage("Live")
	public static final String LIVE = "SQLdm.Labels.live";
    
    @I18NMessage("Historic")
	public static final String HISTORIC = "SQLdm.Labels.historic";
    
    @I18NMessage("From")
	public static final String HISTORIC_FORM = "SQLdm.Labels.from";
    
    @I18NMessage("To")
	public static final String HISTORIC_TO = "SQLdm.Labels.to";
    
    @I18NMessage("Dashboard Tags")
	public static final String TAG = "SQLdm.Labels.tag";
    
    @I18NMessage("Details")
	public static final String DETAILS = "SQLdm.Labels.details";
    
    @I18NMessage("Identifier")
	public static final String IDENTIFIER = "SQLdm.Labels.identifier";
    
    @I18NMessage("Dashboard Name")
	public static final String DASHBOARD_NAME = "SQLdm.Labels.dashboard-name";
    
    @I18NMessage("Add a Dashboard")
	public static final String ADD_A_DASHBOARD = "SQLdm.Labels.add-a-dashboard";
    
    @I18NMessage("Edit Dashboard")
	public static final String EDIT_DASHBOARD = "SQLdm.Labels.edit-dashboard";
    
    @I18NMessage("Remove Dashboard")
	public static final String REMOVE_DASHBOARD = "SQLdm.Labels.remove-dashboard";
    
    @I18NMessage("Add a Widget")
	public static final String ADD_A_WIDGET = "SQLdm.Labels.add-a-widget";
    
    @I18NMessage("(optional - leave empty for default)")
	public static final String LEAVE_EMPTY_FOR_DEFAULT = "SQLdm.Labels.optional-leave-empty-for-default";
    
    @I18NMessage("please select a metric")
	public static final String SELECT_METRIC = "SQLdm.Labels.select-metric";

    @I18NMessage("Default")
	public static final String DEFAULT_INSTANCE_NAME = "SQLdm.Labels.default-instance-name";
	
	@I18NMessage("Health")
	public static final String HEALTH_INDEX = "SQLdm.Labels.instance-health-index";
	
	@I18NMessage("I/O")
	public static final String INSTANCE_IO = "SQLdm.Labels.instance-io"; 
	
/*	@I18NMessage("DB")
	public static final String INSTANCE_DB = "SQLdm.Labels.instance-db";*/
	
	@I18NMessage("Logs")
	public static final String INSTANCE_LOGS = "SQLdm.Labels.instance-logs";
	
	@I18NMessage("Alert")
	public static final String INSTANCE_AlERT = "SQLdm.Labels.instance-alert";
	
	@I18NMessage("Services")
	public static final String INSTANCE_SERVICES = "SQLdm.Labels.instance-services";
	
	@I18NMessage("Virtualization")
	public static final String INSTANCE_VIRTUALIZATION = "SQLdm.Labels.instance-virtualization";
	
	@I18NMessage("Operational")
	public static final String INSTANCE_OPERATIONAL = "SQLdm.Labels.instance-Operational";
	
	@I18NMessage("Sessions")
	public static final String INSTANCE_SESSIONS = "SQLdm.Labels.instance-Sessions";
	
	@I18NMessage("Queries")
	public static final String INSTANCE_QUERIES = "SQLdm.Labels.instance-Queries";
	
	@I18NMessage("Memory")
	public static final String INSTANCE_MEMORY = "SQLdm.Labels.instance-Memory";
	
	@I18NMessage("CPU")
	public static final String INSTANCE_CPU = "SQLdm.Labels.instance-Cpu";
	
	@I18NMessage("SWA")
	public static final String INSTANCE_SWA = "SQLdm.Labels.instance-swa";
	
}

