package com.idera.sqldm.ui.dashboard.instances.overview;


public class CustomizationConstants {

	//Model Names
	public static final String SQL_CPU_MODEL = "SQL_CPU";
	public static final String CPU_CALL_RATES_MODEL = "CPU_CALL_RATES";
	public static final String SQL_CPU_MEMORY_USAGE_MODEL = "MEMORY_USAGE";
	public static final String LOCK_WAITS_MODEL = "LOCK_WAITS";
	public static final String TRANS_DB_MODEL = "DB_TRANSACTION";
	public static final String LOG_FLUSHES_MODEL = "DB_LOG_FLUSHES";
	public static final String READS_DB_MODEL = "DB_READS";
	public static final String WRITES_DB_MODEL = "DB_WRITES";
	public static final String IO_STALL_DB_MODEL = "DB_IO_STALL";
	public static final String NETWORK_MODEL = "NETWORK_STATS";
	public static final String TEMPDB_PAGING_MODEL = "TEMPDB_PAGE";
	public static final String TEMPDB_USAGE_MODEL = "TEMPDB_USAGE";
	public static final String DISK_PHYSICAL_IO_MODEL = "PHYSICAL_IO";
	public static final String CUSTOM_COUNTERS_MODEL = "CUSTOM_COUNTER_STATS";
	public static final String CACHE_PAGE_LIFE_MODEL = "PAGE_LIFE";
	public static final String CACHE_MEMORY_AREAS_MODEL = "MEMORY_AREAS";
	public static final String CACHE_HIT_MODEL = "CACHE_HIT";
	public static final String DISK_TOTAL_RW_MODEL = "DISK_TOTAL_RW";
	public static final String MEMORY_OS_PAGING_MODEL = "OS_PAGING" ;
	
	//Virtualization Graph Model Names
	public static final String VM_AVAILABLE_MEMORY_MODEL = "VM_AVAILABLE_MEMORY";
	public static final String HOST_AVAILABLE_MEMORY_MODEL = "HOST_AVAILABLE_MEMORY";
	public static final String VM_ACTIVE_BALOONED_CONSUME_GRANTED_MODEL= "VM_ACTIVE_BALOONED_CONSUME_GRANTED";
	public static final String HOST_ACTIVE_BALOONED_CONSUME_GRANTED_MODEL= "HOST_ACTIVE_BALOONED_CONSUME_GRANTED";
	public static final String VM_READ_WRITE_MODEL= "VM_READ_WRITE";
	public static final String HOST_READ_WRITE_MODEL= "HOST_READ_WRITE";
	public static final String VIRTUALIZATION_GRAPH_TYPE= "HyperV";
	
	//Graph Category Titles
	public static final String LOCK_WAITS_TITLE = "Lock Waits";
	public static final String DATABASE_TITLE = "Database";
	public static final String DISK_TITLE = "Disk";
	public static final String TEMPDB_TITLE = "TempDB";
	public static final String CPU_TITLE = "CPU";
	public static final String CUSTOM_COUNTERS_TITLE = "Custom Counters";
	public static final String SERVER_WAITS_TITLE = "Server Waits";
	public static final String FILE_ACTIVITY_TITLE = "File Activity";
	public static final String NETWORK_TITLE = "Network(per second)";
	public static final String VIRTUALIZATION_TITLE = "Virtualization";
	public static final String CACHE_TITLE = "Cache";
	public static final String BLOCKED_SESSION_TITLE = "Blocked Sessions";
	public static final String MEMORY_TITLE = "Memory";
	
	//Graph Names
	public static final String TEMPDB_PAGING_GRAPH_NAME = "Paging (per second)";
	public static final String CPU_CALL_RATES_GRAPH_NAME = "Call Rates (per second)";
	public static final String DISK_TOTAL_TIME_GRAPH_NAME = "Total Time (ms)";
	public static final String DISK_AVG_TIME_GRAPH_NAME = "Avg Time (per second)";
	public static final String MEMORY_PAGING_GRAPH_NAME = "Paging (per second)";
	
	//configuration constants
	public static final String GRAPH_WIDTH = "1010px";
	public static final String GRAPH_HEIGHT = "360px";
	public static final String GRAPH_LEFT_MARGIN = "20px";
	public static final String GRAPH_RIGHT_MARGIN = "0px";
	public static final String GRAPH_TOP_MARGIN = "10px";
	public static final String GRAPH_BOTTOM_MARGIN = "45px";
	public static final String GRAPH_CONTAINER_DIV_HEIGHT = "400px";
	public static final String GRAPH_BORDER = "chart-border";
	public static final String GRAPH_LABEL_STYLE = "font-weight: bold;float: left;padding-left: 12px;";
	public static final String GRAPH_RIGHT_PADDING = "20px";
	public static final String GRAPH_BOTTOM_PADDING = "10px";
	
	//filterConstants
	public static final String DB_STAT_DATA_LIST = "dbOverViewStats";
	public static final Integer DB_TRANS_KEY = 0;
	public static final Integer DB_LOG_FLUSHES_KEY = 1;
	public static final Integer DB_READS_KEY = 2;
	public static final Integer DB_WRITES_KEY = 3;
	public static final Integer DB_IO_STALL_KEY = 4;
	public static final String DB_STAT_MODEL_NAME = "dbStatsLineChartModels";

}
