package com.idera.sqldm.data.alerts;

public enum AlertMetrics {
	NULL,  // This is a necessary place holder.  There is no corresponding value used by the C# services.
	DATABASE_NEVER_BACKUP,
	DATABASE_STALE_BACKUP,
	DATABASE_CHECK_DB_NEVER,
	DATABASE_CHECK_DB_LONG_TIME,
	DATABASE_STATE,
	DATABASE_AUTO_SHRINK_ENABLED,
	TEMPDB_FILE_SIZE_INCONSISTENT,
	INSTANCE_DOWN,
	INSTANCE_SLOW,
	INSTANCE_XP_CMD_SHELL_ENABLED,
	INSTANCE_OPTIMIZE_ADHOC_ENABLED,
	VOLUME_GETTING_FULL,
	DATABASE_DATA_GETTING_FULL,
	DATABASE_LOG_GETTING_FULL,
	INSTANCE_UNABLE_TO_MONITOR,
	INSTANCE_UNABLE_TO_MONITOR_HOST,
	UNUSED,	
	INSTANCE_PARTIALLY_MONITORED
}