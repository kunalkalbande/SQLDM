--------------------------------------------------------------------------------
--  Batch: Database Configuration 2005
--  Tables: master.sys.databases
--	Returns:
--	Database Name
--	Collation
--	IsAnsiNullDefault
--	IsAnsiNullsEnabled
--	IsAnsiPaddingEnabled
--	IsAnsiWarningsEnabled
--	IsArithmeticAbortEnabled 
--	IsAutoClose
--	IsAutoCreateStatistics
--	IsAutoShrink
--	IsAutoUpdateStatistics
--	IsCloseCursorsOnCommitEnabled
--	IsFulltextEnabled
--	IsInStandBy
--	IsLocalCursorsDefault
--	IsMergePublished
--	IsNullConcat
--	IsNumericRoundAbortEnabled
--	IsParameterizationForced
--	IsQuotedIdentifiersEnabled
--	IsPublished
--	IsRecursiveTriggersEnabled
--	IsSubscribed
--	IsSyncWithBackup
--	IsTornPageDetectionEnabled
--	Recovery
--	Status
--	Updateability
--	UserAccess
--	Version
--  Cross-Database Ownership Chaining
--  Date Correlation Optimization Enabled
--  VarDecimal Storage Format Enabled
--  Page Verify
--  IsAutoUpdateStatsAsyncOn
--  IsBrokerEnabled
--  IsReadOnly
--  IsTrustworthy
--  Snapshot isolation state
--------------------------------------------------------------------------------
select 
	name, 
	rtrim(collation_name),
	is_ansi_null_default_on,
	is_ansi_nulls_on,
	is_ansi_padding_on,
	is_ansi_warnings_on,
	is_arithabort_on,
	is_auto_close_on,
	is_auto_create_stats_on,
	is_auto_shrink_on,
	is_auto_update_stats_on,
	is_cursor_close_on_commit_on,
	is_fulltext_enabled,
	is_in_standby,
	is_local_cursor_default,
	is_merge_published,
	is_concat_null_yields_null_on,
	is_numeric_roundabort_on,
	is_parameterization_forced,
	is_quoted_identifier_on,
	is_published,
	is_recursive_triggers_on,
	is_subscribed,
	is_sync_with_backup,
	torn_page_detection = case when page_verify_option = 1 then cast(1 as bit) else cast(0 as bit) end,
	recovery_model_desc,
	state_desc,
	Updateability = (select databasepropertyex(name,'Updateability')),
	user_access_desc,
	Version = (select databasepropertyex(name,'Version')),
	compatibility_level,
	is_db_chaining_on,
	is_date_correlation_on,
	is_vardecimal_enabled = case when databasepropertyex(name, 'version') = 612 then cast(1 as bit) else cast(0 as bit) end,
	page_verify_option_desc,
	is_auto_update_stats_async_on,
	is_broker_enabled,
	is_trustworthy_on,
	snapshot_isolation_state_desc,
	is_read_only                        -- SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- new column
from 
	master.sys.databases
where 
	1=1
	{0}

order by
	case when name = 'model' then -1 else 0 end
