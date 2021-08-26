/*----------------------------- Idera SQL diagnostic manager -------------------------------
**
**	Copyright Idera, Inc. 2005-2012
**		All rights reserved
**
**------------------------------------------------------------------------------------------
**
**	Description:  SQL script to install/migrate the SQLdm repository database.
**
**------------------------------------------------------------------------------------------
**
**	Instructions: 
**		1. Use the Find/Replace function to locate and replace all references to 
**			'SQLdmDatabase' with the name of your SQLdm Repository Database.
**		2. Use the Find/Replace function to locate and replace all references to 
**			'SQLdmCheckDatabaseExists' with the name of your SQLdm Repository Database.
**		3. Connect to the SQL Server that is hosting your SQLdm Repository database and 
**			execute this script.
**		4. Run the upgrade installer to complete the upgrade process.
**	
**------------------------------------------------------------------------------------------
*/
 
use [master]
set nocount on
GO

SET LANGUAGE us_english
GO

-- Get the SQL Server data path
declare @path nvarchar(256), @filename nvarchar(256), @upgrade bit
select @path = substring(filename, 1, charindex(N'master.mdf', lower(filename)) - 1) 
from 
	master..sysfiles
where 
	fileid = 1


declare @modelSize int
Select @modelSize = CAST( ((SUM(Size)* 8) / 1024.0) AS DECIMAL(18,2) ) FROM sys.master_files 
WHERE database_id = DB_ID('model')

set @modelSize = @modelSize + 1;

declare @finalSize varchar(10)
select @finalSize = CAST(@modelSize as varchar(10))

select @filename = 'SQLdmDatabase'

if (db_id( 'SQLdmCheckDatabaseExists') is null)
begin		
	declare @createcommand nvarchar(2000)
	set @createcommand = 'create database ' + quotename(@filename) + '
	on (
		Name = ' + quotename(@filename) + ', 
		FileName = ' + quotename(@path + @filename + '.mdf','''') + ', '
		+ 'Size = '+@finalSize+', Filegrowth = 32MB
		)
	log on 
		(Name = ' + quotename(@filename+ '_log') + ', 
		FileName = ' + quotename(@path + @filename + '_log.ldf','''') + ', '
		+ 'Size = 5MB, Filegrowth = 32MB) 
	collate SQL_Latin1_General_CP1_CS_AS'
	execute(@createcommand)
end

GO

if (object_id('SQLdmDatabase..fn_GetDatabaseVersion') is null)
begin
	print 'Setting recovery mode'
	ALTER DATABASE [SQLdmDatabase] SET RECOVERY SIMPLE
end

GO

use [SQLdmDatabase]

GO	

set quoted_identifier on
set ansi_nulls on
set ansi_warnings off

GO

-- Add SQLdmConsoleUser database role
if ((select count(uid) from dbo.sysusers where name = 'SQLdmConsoleUser' and issqlrole = 1) = 0)
begin
exec sp_addrole 'SQLdmConsoleUser'
end


if not exists(select name from sys.schemas where name = 'Grooming')
begin
	declare @cmd nvarchar(1000),@filename nvarchar(256), @schema nvarchar(500)
	set @filename = N'SQLdmDatabase'
	set @schema = 'create schema Grooming authorization dbo'
	set @cmd = quotename(@filename) + N'.sys.sp_executesql'
	exec @cmd @schema
end

----------------------------------------------------------------------
Print 'Adding Install Utility Proc'
----------------------------------------------------------------------
---------------------------------------------------------------------------------------
--Create a proc that will help us to drop default constraints
---------------------------------------------------------------------------------------
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_DropDefaultConstraint]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_DropDefaultConstraint]
GO
create proc p_DropDefaultConstraint(@TableName nvarchar(256), @ColumnName nvarchar(256))
as
begin
	DECLARE @defname VARCHAR(100), @cmd VARCHAR(1000)

	set @defname = 
	(SELECT sc.name
	FROM sys.objects so JOIN sys.default_constraints sc
	ON so.object_id = sc.object_id 
	WHERE object_name(so.parent_object_id) = @TableName
	AND so.type = 'D'
	AND sc.parent_column_id = 
	 (SELECT column_id FROM sys.columns 
	 WHERE object_id = object_id(@TableName) AND 
	 name = @ColumnName))

	SET @cmd ='ALTER TABLE ' + quotename(@TableName) + ' DROP CONSTRAINT '
	+ quotename(@defname)

	EXEC(@cmd)
end

GO

----------------------------------------------------------------------
Print 'Processing Tables'
----------------------------------------------------------------------

IF (OBJECT_ID('RepositoryInfo') IS NULL)
BEGIN
CREATE TABLE [RepositoryInfo](
	[Name] [nvarchar](30) NOT NULL,
	[Internal_Value] [int] NULL,
	[Character_Value] [nvarchar](1024) NULL,
	CONSTRAINT [Name] PRIMARY KEY CLUSTERED 
	(
		[Name] ASC
	)
)
	insert into [RepositoryInfo](Name,Internal_Value) values ('PredictiveAnalyticsEnabled',1)
END
else
begin
	if not exists(select Name from [RepositoryInfo] where Name = 'PredictiveAnalyticsEnabled')
	begin
		insert into [RepositoryInfo](Name,Internal_Value) values ('PredictiveAnalyticsEnabled',0)
	end

	---- Upgrade Scenario - Updating Grooming Forecasting Details
	if not exists (select [Internal_Value] from RepositoryInfo where [Name] = 'GroomForecasting')
	begin
		-- Default GroomForecasting Value with Groom Metrics Default value
		declare @groomForecastingValue int;
		set @groomForecastingValue = 1095;

		-- If Groom Metrics is set and available, update @groomForecastingValue
		if exists (select [Internal_Value] from RepositoryInfo where [Name] = 'GroomMetrics')
		begin
			select @groomForecastingValue = [Internal_Value] from RepositoryInfo where [Name] = 'GroomMetrics';
		end
		else
		begin
			-- If Groom Metrics is not available add entry to reuse the existing default 365 rather thatn new default
			insert into [RepositoryInfo] ([Name],[Internal_Value]) values ('GroomMetrics',@groomForecastingValue);
		end
		-- Update GroomForecasting
		insert into [RepositoryInfo] ([Name],[Internal_Value]) values ('GroomForecasting',@groomForecastingValue);
	end

	if not exists (select [Internal_Value] from RepositoryInfo where [Name] = 'AggregateForecasting')
	begin
		-- Default Aggregate Forecasting Value with GroomQueryAggregation Default value
		declare @AggregateForecastingValue int;
		set @AggregateForecastingValue = 1095;

		-- If GroomQueryAggregation is set and available, update @AggregateForecastingValue
		if exists (select [Internal_Value] from RepositoryInfo where [Name] = 'GroomQueryAggregation')
		begin
			select @AggregateForecastingValue = [Internal_Value] from RepositoryInfo where [Name] = 'GroomQueryAggregation';
		end
		
		-- Update GroomForecasting
		insert into [RepositoryInfo] ([Name],[Internal_Value]) values ('AggregateForecasting',@AggregateForecastingValue);
	end
end


create table #tempversion(Ind int, Name nvarchar(30),Internal_Value bigint, Character_Value nvarchar(50))
insert #tempversion
	exec master..xp_msver 'ProductVersion' 

if exists(select Name from [RepositoryInfo] where Name = 'SQLVersionAtUpgrade')
begin
	update [RepositoryInfo] 
		set [RepositoryInfo].Internal_Value = cast(left(#tempversion.Character_Value,4) as float),
			[RepositoryInfo].Character_Value = #tempversion.Character_Value
		from #tempversion
	where
		[RepositoryInfo].Name =  'SQLVersionAtUpgrade'
		
end
else
begin
	insert into [RepositoryInfo]  (Name, Internal_Value, Character_Value)
		select  
			'SQLVersionAtUpgrade',
			cast(left(Character_Value,4) as float),
			Character_Value
		from #tempversion

end

drop table #tempversion


GO

declare @cmptleveltarget int
select @cmptleveltarget = case when Internal_Value = 9 then 90 when Internal_Value = 10 then 100 else 110 end from RepositoryInfo where Name =  'SQLVersionAtUpgrade'
exec sp_dbcmptlevel 'SQLdmDatabase', @cmptleveltarget

GO

----------------------------------------------------------------------

IF (OBJECT_ID('ManagementServices') IS NULL)
BEGIN

CREATE TABLE [ManagementServices](
	[ManagementServiceID] [uniqueidentifier] NOT NULL,
	[InstanceName] [nvarchar](15) NOT NULL,
	[MachineName] [nvarchar](15) NOT NULL,
	[Address] [nvarchar](256) NOT NULL,
	[Port] [int] NOT NULL,
	[DefaultCollectionServiceID] [uniqueidentifier] NULL,
	CONSTRAINT [PK_ManagementServices] PRIMARY KEY CLUSTERED 
	(
		[ManagementServiceID] ASC
	)
	) 

	CREATE UNIQUE INDEX [IXManagementServices] ON [ManagementServices]([MachineName],[InstanceName]) 

END

----------------------------------------------------------------------

IF (OBJECT_ID('CollectionServices') IS NULL)
BEGIN

CREATE TABLE [CollectionServices](
	[CollectionServiceID] [uniqueidentifier] NOT NULL,
	[InstanceName] [nchar](15) NOT NULL,
	[MachineName] [nchar](15) NOT NULL,
	[Address] [nvarchar](256) NOT NULL,
	[Port] [int] NOT NULL,
	[Enabled] [bit] NOT NULL,
	[LastHeartbeatUTC] [datetime] NULL,
	[ManagementServiceID] [uniqueidentifier] NOT NULL,
	CONSTRAINT [PK_CollectionService] PRIMARY KEY CLUSTERED 
	(
		[CollectionServiceID] ASC
	),
	CONSTRAINT [FKCollectionServicesManagementServices] FOREIGN KEY 
	(
		[ManagementServiceID]
	) 
	REFERENCES [ManagementServices] 
	(
		[ManagementServiceID]
	)
	ON DELETE CASCADE
	)

	CREATE UNIQUE INDEX [IXCollectionServices] ON [CollectionServices]([MachineName],[InstanceName]) 

END


--START : SQLdm 10.0 (Tarun Sapra) -Minimal Cloud Support --Created a new table for CloudSupportProviders mapping
IF (OBJECT_ID('CloudProviders') IS NULL)
	BEGIN	
		CREATE TABLE [dbo].[CloudProviders](
		[CloudProviderId] int NOT NULL,
		[CloudProviderName] varchar(500) NOT NULL,
		CONSTRAINT [PK_CloudProviders] PRIMARY KEY CLUSTERED ([CloudProviderId] ASC)
		)
	END
--END : SQLdm 10.0 (Tarun Sapra) -Minimal Cloud Support --Created a new table for CloudSupportProviders mapping


----------------------------------------------------------------------

IF (OBJECT_ID('MonitoredSQLServers') IS NULL)
BEGIN
CREATE TABLE  [MonitoredSQLServers]
	(
	[SQLServerID] [int] IDENTITY NOT NULL,
	[InstanceName] [nvarchar](256) NOT NULL,
	[Active] [bit] NOT NULL CONSTRAINT [DF_MonitoredSQLServers_Active]  DEFAULT ((1)),
	[Deleted] [bit] NOT NULL CONSTRAINT [DF_MonitoredSQLServers_Deleted]  DEFAULT ((0)),
	[RegisteredDate] [datetime] NOT NULL CONSTRAINT [DF_MonitoredSQLServers_RegisteredDate]  DEFAULT (getutcdate()),
	[CollectionServiceID] [uniqueidentifier] NULL,
	[UseIntegratedSecurity] [bit] NOT NULL,
	[Username] [nvarchar](128) NULL,
	[Password] [nvarchar](256) NULL,
	[EncryptData] [bit] NOT NULL CONSTRAINT [DF_MonitoredSQLServers_EncryptData] DEFAULT((0)),
	[TrustServerCert] [bit] NOT NULL CONSTRAINT [DF_MonitoredSQLServers_TrustServerCert] DEFAULT((0)),
	[ScheduledCollectionIntervalInSeconds] [int] NOT NULL,
	[LastScheduledCollectionTime] [datetime] NULL,
	[ServerVersion] [nvarchar](30) NULL,
	[ServerEdition] [nvarchar](30) NULL,
	[MaintenanceModeEnabled] [bit] NOT NULL CONSTRAINT [DF_MonitoredSQLServers_MaintenanceModeEnabled]  DEFAULT ((0)),
	[QueryMonitorEnabled] [bit] NOT NULL CONSTRAINT [DF_MonitoredSQLServers_QueryMonitorEnabled]  DEFAULT ((0)),
	[QueryMonitorSqlBatchEventsEnabled] [bit] NOT NULL CONSTRAINT [DF_MonitoredSQLServers_CaptureSqlBatches]  DEFAULT ((0)),
	[QueryMonitorSqlStatementEventsEnabled] [bit] NOT NULL CONSTRAINT [DF_MonitoredSQLServers_CaptureSqlStatements]  DEFAULT ((0)),
	[QueryMonitorStoredProcedureEventsEnabled] [bit] NOT NULL CONSTRAINT [DF_MonitoredSQLServers_QueryMonitorStoredProcedureEventsEnabled]  DEFAULT ((0)),
	[QueryMonitorDurationFilterInMilliseconds] [int] NOT NULL CONSTRAINT [DF_MonitoredSQLServers_QueryMonitorDurationFilter]  DEFAULT ((500)),
	[QueryMonitorCpuUsageFilterInMilliseconds] [int] NOT NULL CONSTRAINT [DF_MonitoredSQLServers_QueryMonitorCpuUsageFilterInMilliseconds]  DEFAULT ((0)),
	[QueryMonitorLogicalDiskReadsFilter] [int] NOT NULL CONSTRAINT [DF_MonitoredSQLServers_QueryMonitorLogicalReadsFilter]  DEFAULT ((0)),
	[QueryMonitorPhysicalDiskWritesFilter] [int] NOT NULL CONSTRAINT [DF_MonitoredSQLServers_QueryMonitorPhysicalWritesFilter]  DEFAULT ((0)),
	[QueryMonitorTraceFileSizeKB] [int] NOT NULL CONSTRAINT [DF_MonitoredSQLServers_QueryMonitorTraceFileSizeKB]  DEFAULT ((1024)),
	[QueryMonitorTraceFileRollovers] [int] NOT NULL CONSTRAINT [DF_MonitoredSQLServers_QueryMonitorTraceFileRollovers]  DEFAULT ((2)),
	[QueryMonitorTraceRecordsPerRefresh] [int] NOT NULL CONSTRAINT [DF_MonitoredSQLServers_QueryMonitorTraceRecordsPerRefresh]  DEFAULT ((1000)),
	[ActivityMonitorEnabled] [bit] NOT NULL CONSTRAINT [DF_MonitoredSQLServers_ActivityMonitorEnabled]  DEFAULT ((1)),
	[ActivityMonitorDeadlockEventsEnabled] [bit] NOT NULL CONSTRAINT [DF_MonitoredSQLServers_ActivityMonitorDeadlockEventsEnabled]  DEFAULT ((0)),	
	[ActivityMonitorAutoGrowEventsEnabled] [bit] NOT NULL CONSTRAINT [DF_MonitoredSQLServers_ActivityMonitorAutogrowEventsEnabled]  DEFAULT ((0)),	
	[ActivityMonitorBlockingEventsEnabled] [bit] NOT NULL CONSTRAINT [DF_MonitoredSQLServers_ActivityMonitorBlockingEventsEnabled]  DEFAULT ((1)),		
	[ActivityMonitorBlockedProcessThreshold] [int] NOT NULL CONSTRAINT [DF_MonitoredSQLServers_ActivityMonitorBlockedProcessThreshold] DEFAULT ((30)),		
	[GrowthStatisticsStartTime] [datetime] CONSTRAINT [DF_MonitoredSQLServers_GrowthStatisticsStartTime] DEFAULT '1900-01-01 3:00:00 AM',
	[ReorgStatisticsStartTime] [datetime] CONSTRAINT [DF_MonitoredSQLServers_ReorgStatisticsStartTime] DEFAULT '1900-01-01 3:00:00 AM',
	[LastGrowthStatisticsRunTime] [datetime] CONSTRAINT [DF_MonitoredSQLServers_LastGrowthStatisticsRunTime] DEFAULT NULL,
	[LastGrowthStatisticsRunTimeUTC] [datetime] CONSTRAINT [DF_MonitoredSQLServers_LastGrowthStatisticsRunTimeUTC] DEFAULT NULL,
	[LastReorgStatisticsRunTime] [datetime] CONSTRAINT [DF_MonitoredSQLServers_LastReorgStatisticsRunTime] DEFAULT NULL,
	[LastReorgStatisticsRunTimeUTC] [datetime] CONSTRAINT [DF_MonitoredSQLServers_LastReorgStatisticsRunTimeUTC] DEFAULT NULL,
	[EarliestDateImportedFromLegacySQLdm] [datetime] DEFAULT NULL,
	[GrowthStatisticsDays] [tinyint] DEFAULT 253,
	[ReorgStatisticsDays] [tinyint] DEFAULT 253,
	[TableStatisticsExcludedDatabases] [nvarchar] (max) DEFAULT NULL,
	[GroomAlerts] [int] NOT NULL CONSTRAINT [DF_MonitoredSQLServers_GroomAlerts]  DEFAULT -1,
	[GroomMetrics] [int] NOT NULL CONSTRAINT [DF_MonitoredSQLServers_GroomMetrics]  DEFAULT -1,
	[GroomTasks] [int] NOT NULL CONSTRAINT [DF_MonitoredSQLServers_GroomTasks]  DEFAULT -1,
	[ReorgMinTableSizeKB] int NOT NULL CONSTRAINT [DF_MonitoredSQLServers_ReorgMinTableSize] DEFAULT 64,
	[GroomActivity] [int] NOT NULL CONSTRAINT [DF_MonitoredSQLServers_GroomActivity]  DEFAULT -1,
	[CustomCounterTimeoutInSeconds] [int] DEFAULT 180,
	[OutOfProcOleAutomation] [bit] DEFAULT 0,
	[DisableReplicationMonitoring] [bit] DEFAULT 0,
	[MaintenanceModeType] [int] DEFAULT 0,
	[MaintenanceModeStart] [datetime] DEFAULT NULL,
	[MaintenanceModeStop]  [datetime] DEFAULT NULL,
	[MaintenanceModeDays] [smallint] DEFAULT NULL,
	[MaintenanceModeDurationSeconds] [int] DEFAULT NULL,	
    [MaintenanceModeRecurringStart] [datetime] DEFAULT NULL,
	[QueryMonitorAdvancedConfiguration] [nvarchar] (max) DEFAULT '<?xml version="1.0" encoding="utf-16"?><AdvancedQueryMonitorConfiguration xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"><ApplicationExcludeLike><Like>SQLProfiler%</Like><Like>SQLDMO%</Like><Like>SQLAgent%</Like></ApplicationExcludeLike></AdvancedQueryMonitorConfiguration>',
	[DisableExtendedHistoryCollection] [bit] DEFAULT 0,
	[RefRangeUseDefaults] [bit] DEFAULT 1,
	[RefRangeStartTimeUTC] [datetime] DEFAULT NULL,
	[RefRangeEndTimeUTC] [datetime] DEFAULT NULL,
	[RefRangeDays] [tinyint] DEFAULT 124, 
	[DisableOleAutomation] bit DEFAULT 0,
	[DiskCollectionSettings] [nvarchar] (max) DEFAULT NULL,
	[QueryMonitorStopTimeUTC] [datetime] DEFAULT NULL,
	[InputBufferLimiter] [int] default 500 not null,
	[InputBufferLimited] [bit] default 0 not null,
	[ActiveClusterNode] [nvarchar](256) null,
	[PreferredClusterNode] [nvarchar](256) null,
	[RealServerName] [nvarchar](100) null,
	[FriendlyServerName] [nvarchar](256) null,
	[ActiveWaitCollectorStartTimeRelative] [datetime] DEFAULT NULL,
	[ActiveWaitCollectorRunTimeSeconds] [int] DEFAULT 0,
	[ActiveWaitCollectorCollectionTimeSeconds] [int] DEFAULT 30,
	[ActiveWaitCollectorEnabled] [bit] DEFAULT 0,
	[ActiveWaitLoopTimeMilliseconds] [int] DEFAULT 500,
	[ActiveWaitAdvancedConfiguration] [nvarchar] (max) DEFAULT null,
	[ActiveWaitXEEnable] bit DEFAULT 1 not null,
	[ActiveWaitXEFileSizeMB] int default 1 not null,
	[ActiveWaitXEFilesRollover] int default 3 not null,
	[ActiveWaitXERecordsPerRefresh] int default 1000 not null,
	[ActiveWaitXEMaxMemoryMB] int default 1 not null,
	[ActiveWaitXEEventRetentionMode] tinyint default 1 not null,
	[ActiveWaitXEMaxDispatchLatencySecs] int default 300 not null,
	[ActiveWaitXEMaxEventSizeMB] int default 1 not null,
	[ActiveWaitXEMemoryPartitionMode] int default 0 not null, 
	[ActiveWaitXETrackCausality] bit default 0 not null,
	[ActiveWaitXEStartupState] bit default 0 not null,
	[ActiveWaitsXEFileName] nvarchar(1024)default 'dm7XESessionOut.xel' not null,
	[ClusterCollectionSetting] [smallint] DEFAULT 0,
	[ServerPingInterval] [smallint] NOT NULL DEFAULT 30,
	[VHostID] [int] NULL,
	[VmUID] [nvarchar](256) NULL,
	[VmName] [nvarchar](256) NULL,
	[VmDomainName] [nvarchar](256) NULL,
	[AlertRefreshInMinutes] [bit] NOT NULL DEFAULT 1,
	[DatabaseStatisticsRefreshIntervalInSeconds] [int] NOT NULL DEFAULT 3600,
	[WmiCollectionEnabled] [bit] NOT NULL DEFAULT 0,
	[WmiConnectAsService] [bit] NOT NULL DEFAULT 1,
	[WmiUserName] [nvarchar](256) NULL,
	[WmiPassword] [nvarchar](256) NULL,
	[LastDatabaseCollectionTime] [datetime] NULL,
	[LastAlertRefreshTime] [datetime] NULL,
	[GroomAudit] [int] NOT NULL CONSTRAINT [DF_MonitoredSQLServers_GroomAudit]  DEFAULT -1,
	[MaintenanceModeMonth] [int] NULL,
	[MaintenanceModeSpecificDay] [int] NULL,
	[MaintenanceModeWeekOrdinal] [int] NULL,
	[MaintenanceModeWeekDay] [int] NULL,
	[MaintenanceModeMonthDuration] [int] NULL,
	[MaintenanceModeMonthRecurringStart] [DATETIME] NULL,
    --SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session  --   new columns for the table
	QueryMonitorTraceMonitoringEnabled BIT NOT NULL CONSTRAINT [DF_MonitoredSQLServers_QueryMonitorTraceMonitoringEnabled] DEFAULT(0),
	QueryMonitorCollectQueryPlan BIT NOT NULL CONSTRAINT [DF_MonitoredSQLServers_QueryMonitorCollectQueryPlan] DEFAULT(0) ,

	QueryMonitorCollectEstimatedQueryPlan BIT NOT NULL CONSTRAINT [DF_MonitoredSQLServers_QueryMonitorCollectEstimatedQueryPlan] DEFAULT(0) , --SQLdm 10.0 (Tarun Sapra) - Flag for collecting estimated query plan only

	--START SQLdm 9.1 (Ankit Srivastava): Activity Monitor with Extended Events  --   new columns for the table
	ActivityMonitorTraceMonitoringEnabled BIT NOT NULL CONSTRAINT [DF_MonitoredSQLServers_ActivityMonitorTraceMonitoringEnabled] DEFAULT(1), 
	[ActivityMonitorXEFileSizeMB] int default 1 not null,
	[ActivityMonitorXEFilesRollover] int default 3 not null,
	[ActivityMonitorXERecordsPerRefresh] int default 1000 not null,
	[ActivityMonitorXEMaxMemoryMB] int default 1 not null,
	[ActivityMonitorXEEventRetentionMode] tinyint default 1 not null,
	[ActivityMonitorXEMaxDispatchLatencySecs] int default 300 not null,
	[ActivityMonitorXEMaxEventSizeMB] int default 1 not null,
	[ActivityMonitorXEMemoryPartitionMode] int default 0 not null, 
	[ActivityMonitorXETrackCausality] bit default 0 not null,
	[ActivityMonitorXEStartupState] bit default 0 not null,
	[ActivityMonitorXEFileName] nvarchar(1024) not null CONSTRAINT [DF_MonitoredSQLServers_ActivityMonitorXEFileName] DEFAULT('AMExtendedEventLog.xel'),
	--END SQLdm 9.1 (Ankit Srivastava): Activity Monitor with Extended Events  --   new columns for the table

	-- Commenting below code to make health index global for all instance and moving health index value to new table (HealthIndexCofficients)
	----START SQLdm 9.1 (Sanjali Makkar): Configurable Coefficients for Health Index  --   new columns for the table
	--[HealthIndexCoefficientForCriticalAlert] int default 6 not null,
	--[HealthIndexCoefficientForWarningAlert] int default 2 not null,
	--[HealthIndexCoefficientForInformationalAlert] int default 1 not null,
	----END SQLdm 9.1 (Sanjali Makkar): Configurable Coefficients for Health Index  --   new columns for the table

	--START SQLdm 9.1 (Ankit Srivastava): Query Monitor 9.0 Improvement feedback - adding new columns for uery monitoring extended event session configuration
	[QueryMonitorXEFileSizeMB] int default 20 Check ([QueryMonitorXEFileSizeMB] > -1) not null,
	[QueryMonitorXEFilesRollover] int default 5 Check ([QueryMonitorXEFilesRollover] > -1) not null,
	--END SQLdm 9.1 (Ankit Srivastava): Query Monitor 9.0 Improvement feedback - adding new columns for uery monitoring extended event session configuration
	[CloudProviderId] INT DEFAULT NULL,--SQLdm 10.0 (Gaurav Karwal): adding the reference to the cloud provider id in the monitored sql servers
	[GroomPrescriptiveAnalysis] int NOT NULL CONSTRAINT [DF_MonitoredSQLServers_GroomPrescriptiveAnalysis]  DEFAULT -1,
	--Adding new column to calculate health index
	--Srishti Purohit 10.1
	[InstanceScaleFactor] [float] NULL,
	--SQLdm 10.1 Barkha Khatri adding sys admin flag
	[IsUserSysAdmin] bit default 0 NOT NULL,
	-- SQLdm 10.4 Varun Chopra adding query monitor top count filter
	[QueryMonitorTopPlanCountFilter] [int] DEFAULT 75 NOT NULL,
	[QueryMonitorTopPlanCategoryFilter] [int] DEFAULT 0 NOT NULL,
	[QueryMonitorQueryStoreMonitoringEnabled] BIT DEFAULT(0) NOT NULL,
	[ActiveWaitQsEnable] BIT DEFAULT (1) NOT NULL,
	[MaintenanceModeOnDemand] [bit] DEFAULT NULL	
	CONSTRAINT [PKMonitoredSQLServers] PRIMARY KEY CLUSTERED
	(
		[SQLServerID]
	)
)

CREATE UNIQUE INDEX [IX_U_MonitoredSQLServers_InstanceName] ON [MonitoredSQLServers]([InstanceName]) 

END
ELSE
BEGIN
	-- upgrade to 5.0.7
	IF (not exists(select id from syscolumns where id = object_id('MonitoredSQLServers') and name = 'ReorgMinTableSizeKB' collate database_default)) 
	BEGIN
		ALTER TABLE [MonitoredSQLServers] 
			ADD [ReorgMinTableSizeKB] int NOT NULL CONSTRAINT [DF_MonitoredSQLServers_ReorgMinTableSizeKB] DEFAULT 0
	END
	-- upgrade to 5.5.0
	IF (not exists(select id from syscolumns where id = object_id('MonitoredSQLServers') and name = 'GroomActivity' collate database_default)) 
	BEGIN
		ALTER TABLE [MonitoredSQLServers] 
			ADD [GroomActivity] [int] NOT NULL CONSTRAINT [DF_MonitoredSQLServers_GroomActivity]  DEFAULT -1,
				[CustomCounterTimeoutInSeconds] [int] DEFAULT 180, 
				[OutOfProcOleAutomation] [bit] DEFAULT 0,
				[DisableReplicationMonitoring] [bit] DEFAULT 0
	END
	-- upgrade to 5.6
	IF (not exists(select id from syscolumns where id = object_id('MonitoredSQLServers') and name = 'MaintenanceModeType' collate database_default))
	BEGIN
		ALTER TABLE [MonitoredSQLServers]
			ADD [MaintenanceModeType] [int] DEFAULT 0,
				[MaintenanceModeStart] [datetime] DEFAULT NULL,
				[MaintenanceModeStop]  [datetime] DEFAULT NULL,
				[MaintenanceModeDays] [smallint] DEFAULT NULL,
				[MaintenanceModeDurationSeconds] [int] DEFAULT NULL,
				[MaintenanceModeRecurringStart] [datetime] DEFAULT NULL
	END
	-- upgrade to 5.6.0
	IF (not exists(select id from syscolumns where id = object_id('MonitoredSQLServers') and name = 'RefRangeStartTimeUTC' collate database_default)) 
	BEGIN
		ALTER TABLE [MonitoredSQLServers] 
			ADD [RefRangeUseDefaults] [bit] DEFAULT 1,
				[RefRangeStartTimeUTC] [datetime] DEFAULT NULL,
				[RefRangeEndTimeUTC] [datetime] DEFAULT NULL,
				[RefRangeDays] [tinyint] DEFAULT 124, 
				[QueryMonitorAdvancedConfiguration] [nvarchar] (max) DEFAULT '<?xml version="1.0" encoding="utf-16"?><AdvancedQueryMonitorConfiguration xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"><ApplicationExcludeLike><Like>SQLProfiler%</Like><Like>SQLDMO%</Like><Like>SQLAgent%</Like></ApplicationExcludeLike></AdvancedQueryMonitorConfiguration>',
				[DisableExtendedHistoryCollection] [bit] DEFAULT 0
	END

	-- upgrade to 6.0
	IF (not exists(select id from syscolumns where id=object_id('MonitoredSQLServers') and name = 'DisableOleAutomation' collate database_default))
	BEGIN
		ALTER TABLE [MonitoredSQLServers]
			ADD [DisableOleAutomation] [bit] DEFAULT 0,
				[DiskCollectionSettings] [nvarchar] (max) DEFAULT NULL,
				[QueryMonitorStopTimeUTC] [datetime] DEFAULT NULL
	END

	-- upgrade to 6.0.1
	IF (not exists(select id from syscolumns where id=object_id('MonitoredSQLServers') and name = 'InputBufferLimiter' collate database_default))
	BEGIN
		ALTER TABLE [MonitoredSQLServers]
			ADD [InputBufferLimiter] [int] default 500 not null,
				[InputBufferLimited] [bit] default 0 not null 
	END

	-- upgrade to 6.1
	IF (not exists(select id from syscolumns where id=object_id('MonitoredSQLServers') and name = 'ActiveClusterNode' collate database_default))
	BEGIN
		ALTER TABLE [MonitoredSQLServers]
			ADD [ActiveClusterNode] [nvarchar](256) null,
			[PreferredClusterNode] [nvarchar](256) null,
			[QueryMonitorDeadlockEventsEnabled] [bit] NOT NULL CONSTRAINT [DF_MonitoredSQLServers_QueryMonitorDeadlockEventsEnabled]  DEFAULT ((0)),
			[RealServerName] [nvarchar](100) null
	END

	-- upgrade to 6.2
	IF (not exists(select id from syscolumns where id=object_id('MonitoredSQLServers') and name = 'ActiveWaitCollectorStartTimeRelative' collate database_default))
	BEGIN
		ALTER TABLE [MonitoredSQLServers]
			ADD [ActiveWaitCollectorStartTimeRelative] [datetime] DEFAULT NULL,
			[ActiveWaitCollectorRunTimeSeconds] [int] DEFAULT 0,
			[ActiveWaitCollectorCollectionTimeSeconds] [int] DEFAULT 30,
			[ActiveWaitCollectorEnabled] [bit] DEFAULT 0,
			[ActiveWaitLoopTimeMilliseconds] [int] DEFAULT 500,
			[ActiveWaitAdvancedConfiguration] [nvarchar] (max) DEFAULT null,
			[ClusterCollectionSetting] [smallint] DEFAULT 0
	END

	-- upgrade to 7.0
	IF (not exists(select id from syscolumns where id=object_id('MonitoredSQLServers') and name = 'ServerPingInterval' collate database_default))
	BEGIN
		ALTER TABLE [MonitoredSQLServers]
			ADD [ServerPingInterval] smallint NOT NULL DEFAULT 30
	END
	
	-- upgrade to 7.2
	IF (not exists(select id from syscolumns where id=OBJECT_ID('MonitoredSQLServers') and name='VHostID' collate database_default))
	BEGIN
		ALTER TABLE [MonitoredSQLServers]
			ADD [VHostID] [int] DEFAULT NULL,
			[VmUID] [nvarchar](256) DEFAULT NULL,
			[VmName] [nvarchar](256) DEFAULT NULL,
			[VmDomainName] [nvarchar](256) DEFAULT NULL
	END
	
	-- upgrade to 7.2
	IF (not exists(select id from syscolumns where id=OBJECT_ID('MonitoredSQLServers') and name='AlertRefreshInMinutes' collate database_default))
	BEGIN
		ALTER TABLE [MonitoredSQLServers]
			ADD [AlertRefreshInMinutes] [bit] NOT NULL DEFAULT 1
	END
	
	 --if the TableStatisticsExcludedDatabases column does not exist as a varchar then all field must be updated.
	 --Upgrade to 7.2
	IF (not exists(select sc.object_id from sys.columns sc inner join sys.types st on sc.system_type_id = st.system_type_id
		where object_id=OBJECT_ID('MonitoredSQLServers') 
		and sc.name='TableStatisticsExcludedDatabases' and lower(st.name) = 'nvarchar'  collate database_default))
	begin
		--drop default constraints
		exec p_DropDefaultConstraint 'MonitoredSQLServers','TableStatisticsExcludedDatabases'
		exec p_DropDefaultConstraint 'MonitoredSQLServers','QueryMonitorAdvancedConfiguration'
		exec p_DropDefaultConstraint 'MonitoredSQLServers','DiskCollectionSettings'
		exec p_DropDefaultConstraint 'MonitoredSQLServers','ActiveWaitAdvancedConfiguration'		
		
		alter table MonitoredSQLServers alter column QueryMonitorAdvancedConfiguration nvarchar(max)
		-- add the constraint back
		alter table MonitoredSQLServers with check
            add constraint df_QueryMonitorAdvancedConfiguration
            default '<?xml version="1.0" encoding="utf-16"?><AdvancedQueryMonitorConfiguration xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"><ApplicationExcludeLike><Like>SQLProfiler%</Like><Like>SQLDMO%</Like><Like>SQLAgent%</Like></ApplicationExcludeLike></AdvancedQueryMonitorConfiguration>' for QueryMonitorAdvancedConfiguration
			
		alter table MonitoredSQLServers alter column  TableStatisticsExcludedDatabases nvarchar(max) null
		--This field might have been added as part of the 6.0 upgrade
		alter table MonitoredSQLServers alter column DiskCollectionSettings nvarchar(max) null
		alter table MonitoredSQLServers alter column ActiveWaitAdvancedConfiguration nvarchar(max) null
	end
	
	-- upgrade to 7.5
	IF (not exists(select id from syscolumns where id=OBJECT_ID('MonitoredSQLServers') and name='DatabaseStatisticsRefreshIntervalInSeconds' collate database_default))
	BEGIN
		ALTER TABLE [MonitoredSQLServers]
			ADD [DatabaseStatisticsRefreshIntervalInSeconds] [int] NOT NULL DEFAULT 3600,
				[WmiCollectionEnabled] [bit] NOT NULL DEFAULT 0,
				[WmiConnectAsService] [bit] NOT NULL DEFAULT 1,
				[WmiUserName] [nvarchar](256) NULL,
				[WmiPassword] [nvarchar](256) NULL
				
		-- For upgrades set the DatabaseStatisticsRefreshIntervalInSeconds to the same value as ScheduledCollectionIntervalInSeconds
		-- This will maintain the same behavior as the previous version
		exec('update [MonitoredSQLServers]
		set [DatabaseStatisticsRefreshIntervalInSeconds] = [ScheduledCollectionIntervalInSeconds]')
		
	END
	
	--upgrade to 7.5
	IF (not exists(select id from syscolumns where id=OBJECT_ID('MonitoredSQLServers') and name='ActiveWaitXEEnable' collate database_default))	
	begin
		ALTER TABLE [MonitoredSQLServers]
			ADD ActiveWaitXEEnable bit default 1 not null,
			ActiveWaitXEFileSizeMB int default 5 not null,
			ActiveWaitXEFilesRollover int default 1 not null,
			ActiveWaitXERecordsPerRefresh int default 1000 not null,
			ActiveWaitXEMaxMemoryMB int default 4 not null,
			ActiveWaitXEEventRetentionMode tinyint default 1 not null,
			ActiveWaitXEMaxDispatchLatencySecs int default 300 not null,
			ActiveWaitXEMaxEventSizeMB int default 8 not null,
			ActiveWaitXEMemoryPartitionMode int default 0 not null, 
			ActiveWaitXETrackCausality bit default 0 not null,
			ActiveWaitXEStartupState bit default 0 not null,
			ActiveWaitsXEFileName nvarchar(1024) default 'dm7XESessionOut.xel' not null,
			LastDatabaseCollectionTime datetime null,
			LastAlertRefreshTime datetime null
	end
	
	--upgrade to 8.0
	IF (not exists(select id from syscolumns where id=OBJECT_ID('MonitoredSQLServers') and name='ActivityMonitorBlockingEventsEnabled' collate database_default))	
	begin
		 EXEC sp_rename 'MonitoredSQLServers.QueryMonitorDeadlockEventsEnabled', 
		 'ActivityMonitorDeadlockEventsEnabled','COLUMN'

		ALTER TABLE [MonitoredSQLServers]
			ADD ActivityMonitorEnabled bit default 1 not null,
			ActivityMonitorBlockingEventsEnabled bit default 1 not null,
			ActivityMonitorAutoGrowEventsEnabled bit default 0 not null,
			ActivityMonitorBlockedProcessThreshold int default 30 not null,
			GroomAudit int NOT NULL CONSTRAINT [DF_MonitoredSQLServers_GroomAudit]  DEFAULT -1
	end	
	
	--upgrade to 8.5: Gaurav Karwal - For Maintenance Mode Monthly mode - END
	IF (not exists(select id from syscolumns where id=OBJECT_ID('MonitoredSQLServers') and name='MaintenanceModeMonth' collate database_default))	
	BEGIN
		ALTER TABLE [MonitoredSQLServers] 
		ADD MaintenanceModeMonth INT NULL,
		MaintenanceModeSpecificDay INT NULL,
		MaintenanceModeWeekOrdinal INT NULL,
		MaintenanceModeWeekDay INT NULL,
		MaintenanceModeMonthDuration INT NULL,
		MaintenanceModeMonthRecurringStart DATETIME NULL
	END	
	--upgrade to 8.5: Gaurav Karwal - For Maintenance Mode Monthly mode - END	

	--upgrade to 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session  --   new field for query Plan - START
	IF (not exists(select id from syscolumns where id=OBJECT_ID('MonitoredSQLServers') and name='QueryMonitorTraceMonitoringEnabled' collate database_default))	
	BEGIN
		ALTER TABLE [MonitoredSQLServers] 
		ADD QueryMonitorTraceMonitoringEnabled BIT NOT NULL CONSTRAINT [DF_MonitoredSQLServers_QueryMonitorTraceMonitoringEnabled] DEFAULT(0) 
	END	

	IF (not exists(select id from syscolumns where id = object_id('MonitoredSQLServers') and name = 'QueryMonitorCollectQueryPlan' collate database_default)) 
	BEGIN
		ALTER TABLE [MonitoredSQLServers] 
		ADD QueryMonitorCollectQueryPlan BIT NOT NULL CONSTRAINT [DF_MonitoredSQLServers_QueryMonitorCollectQueryPlan] DEFAULT(0) 
	END
	--upgrade to 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session  --   new field for query Plan - END

	--START upgrade to 10.0 (Tarun Sapra): Added a flag for collecting estimated query plan only
	IF (not exists(select id from syscolumns where id=OBJECT_ID('MonitoredSQLServers') and name='QueryMonitorCollectEstimatedQueryPlan' collate database_default))	
	BEGIN
		ALTER TABLE [MonitoredSQLServers] 
		ADD QueryMonitorCollectEstimatedQueryPlan BIT NOT NULL CONSTRAINT [DF_MonitoredSQLServers_QueryMonitorCollectEstimatedQueryPlan] DEFAULT (0)
	END	
	--END upgrade to 10.0 (Tarun Sapra): Added a flag for collecting estimated query plan only

	--START upgrade to 10.0 (Tarun Sapra): Added another column for cloud provider id
	IF (not exists(select id from syscolumns where id=OBJECT_ID('MonitoredSQLServers') and name='CloudProviderId' collate database_default))	
	BEGIN
		ALTER TABLE MonitoredSQLServers
		ADD CloudProviderId INT NULL DEFAULT NULL

		ALTER TABLE MonitoredSQLServers
		ADD CONSTRAINT fk_cloudProviderId FOREIGN KEY (CloudProviderId) REFERENCES CloudProviders(CloudProviderId)
	END	
	--END upgrade to 10.0 (Tarun Sapra): Added another column for cloud provider id
	-- START : SQLdm 10.0 (srishti) To support grooming
	IF (not exists(select id from syscolumns where id=OBJECT_ID('MonitoredSQLServers') and name='GroomPrescriptiveAnalysis' collate database_default))	
	BEGIN
		ALTER TABLE [MonitoredSQLServers] 
		ADD [GroomPrescriptiveAnalysis] int NOT NULL CONSTRAINT [DF_MonitoredSQLServers_GroomPrescriptiveAnalysis]  DEFAULT -1
	END
	-- END : SQLdm 10.0 (srishti) To support grooming
	--START upgrade to 9.1 (Ankit Srivastava): Activity Monitor with Extended Events  --   new columns for the table
	IF (not exists(select id from syscolumns where id=OBJECT_ID('MonitoredSQLServers') and name='ActivityMonitorTraceMonitoringEnabled' collate database_default))	
	BEGIN
		ALTER TABLE [MonitoredSQLServers] 
		ADD [ActivityMonitorTraceMonitoringEnabled] BIT NOT NULL CONSTRAINT [DF_MonitoredSQLServers_ActivityMonitorTraceMonitoringEnabled] DEFAULT(1),
			[ActivityMonitorXEFileSizeMB] int default 1 not null,
			[ActivityMonitorXEFilesRollover] int default 3 not null,
			[ActivityMonitorXERecordsPerRefresh] int default 1000 not null,
			[ActivityMonitorXEMaxMemoryMB] int default 1 not null,
			[ActivityMonitorXEEventRetentionMode] tinyint default 1 not null,
			[ActivityMonitorXEMaxDispatchLatencySecs] int default 300 not null,
			[ActivityMonitorXEMaxEventSizeMB] int default 1 not null,
			[ActivityMonitorXEMemoryPartitionMode] int default 0 not null, 
			[ActivityMonitorXETrackCausality] bit default 0 not null,
			[ActivityMonitorXEStartupState] bit default 0 not null,
			[ActivityMonitorXEFileName] nvarchar(1024) not null CONSTRAINT [DF_MonitoredSQLServers_ActivityMonitorXEFileName] DEFAULT('AMExtendedEventLog.xel')
	END
	--END upgrade to 9.1 (Ankit Srivastava): Activity Monitor with Extended Events  --   new columns for the table

	-- Commenting below code to make health index global for all instance and moving health index value to new table (HealthIndexCofficients)
--	--START upgrade to 9.1 (Sanjali Makkar): Configurable Coefficients for Health Index  --   new columns for the table
--	IF (not exists(select id from syscolumns where id=OBJECT_ID('MonitoredSQLServers') and name='HealthIndexCoefficientForCriticalAlert' collate database_default))	
--	BEGIN
--		ALTER TABLE [MonitoredSQLServers] 
--		ADD [HealthIndexCoefficientForCriticalAlert] int default 6 not null
--END
--	IF (not exists(select id from syscolumns where id=OBJECT_ID('MonitoredSQLServers') and name='HealthIndexCoefficientForWarningAlert' collate database_default))	
--	BEGIN
--		ALTER TABLE [MonitoredSQLServers] 
--		ADD	[HealthIndexCoefficientForWarningAlert] int default 2 not null
--	END
--	IF (not exists(select id from syscolumns where id=OBJECT_ID('MonitoredSQLServers') and name='HealthIndexCoefficientForInformationalAlert' collate database_default))	
--	BEGIN
--		ALTER TABLE [MonitoredSQLServers] 
--		ADD	[HealthIndexCoefficientForInformationalAlert] int default 1 not null
--	END
--	--END upgrade to 9.1 (Sanjali Makkar): Configurable Coefficients for Health Index  --   new columns for the table
	--START upgrade to  9.1 (Ankit Srivastava): Query Monitor 9.0 Improvement feedback - adding new columns for uery monitoring extended event session configuration
	IF (not exists(select id from syscolumns where id=OBJECT_ID('MonitoredSQLServers') and name='QueryMonitorXEFileSizeMB' collate database_default))	
	BEGIN
		ALTER TABLE [MonitoredSQLServers] 
		ADD
	[QueryMonitorXEFileSizeMB] int default 20 Check ([QueryMonitorXEFileSizeMB] > -1) not null,
	[QueryMonitorXEFilesRollover] int default 5 Check ([QueryMonitorXEFilesRollover] > -1) not null
	END
	--END upgrade to  9.1 (Ankit Srivastava): Query Monitor 9.0 Improvement feedback - adding new columns for uery monitoring extended event session configuration
	
	--upgrade to 10.0 (Vandana Gogna): Add friendly names

	--Start : Upgrade to 10.1 (Srishti Purohit) - Adding new column to calculate health index
	if not exists(select name from sys.columns where object_id = object_id('MonitoredSQLServers') and name=N'InstanceScaleFactor' collate database_default )
	BEGIN
		ALTER TABLE [MonitoredSQLServers]
		ADD [InstanceScaleFactor] [float] NULL	
	END
	--End : Upgrade to 10.1 (Srishti Purohit) - Adding new column to calculate health index
	
	--  SQLdm kit1(Barkha Khatri) alter password length if table exists(DE44487 fix)
	IF EXISTS(SELECT COUNT(*) FROM sys.columns WHERE name = N'Password' AND object_id = object_id(N'dbo.MonitoredSQLServers'))
	BEGIN
		ALTER TABLE [MonitoredSQLServers]
        ALTER COLUMN [Password] NVARCHAR(256) NULL
	END
	ELSE
	BEGIN
        ALTER TABLE [MonitoredSQLServers]
        ADD [Password] NVARCHAR(256) NULL
	END
	--Start : Upgrade to 10.1 (Barkha Khatri) - Adding new column for sys admin flag
	IF (not exists(select id from syscolumns where id = object_id('MonitoredSQLServers') and name = 'IsUserSysAdmin' collate database_default)) 
	BEGIN
		ALTER TABLE [MonitoredSQLServers] 
			ADD [IsUserSysAdmin] bit NOT NULL DEFAULT 0
	END
	--Start : Upgrade to 10.1 (Barkha Khatri) - Adding new column for sys admin flag

	--Start : Upgrade to 10.4 (Varun Chopra) - Adding new column for Query Store and Top X Plans Filter
	IF (NOT EXISTS(SELECT id FROM syscolumns WHERE id=OBJECT_ID('MonitoredSQLServers') AND name='QueryMonitorQueryStoreMonitoringEnabled' COLLATE database_default))	
	BEGIN
		ALTER TABLE [MonitoredSQLServers] 
		ADD QueryMonitorQueryStoreMonitoringEnabled BIT DEFAULT(0) NOT NULL 
	END	
	-- Add For Query Waits
	IF (NOT EXISTS(SELECT id FROM syscolumns WHERE id=OBJECT_ID('MonitoredSQLServers') AND name='ActiveWaitQsEnable' COLLATE database_default))	
	BEGIN
		ALTER TABLE [MonitoredSQLServers] 
		ADD ActiveWaitQsEnable BIT DEFAULT(1) NOT NULL 
	END
	-- Add For TopX Columns
	IF (NOT EXISTS(SELECT id FROM syscolumns WHERE id=OBJECT_ID('MonitoredSQLServers') AND name='QueryMonitorTopPlanCountFilter' COLLATE database_default))	
	BEGIN
		ALTER TABLE [MonitoredSQLServers] 
		ADD QueryMonitorTopPlanCountFilter [int] DEFAULT 75 NOT NULL 
	END
	IF (NOT EXISTS(SELECT id FROM syscolumns WHERE id=OBJECT_ID('MonitoredSQLServers') AND name='QueryMonitorTopPlanCategoryFilter' COLLATE database_default))	
	BEGIN
		ALTER TABLE [MonitoredSQLServers] 
		ADD QueryMonitorTopPlanCategoryFilter [int] DEFAULT 0 NOT NULL 
	END	
	
END
go

if (exists(select id from syscolumns where id =  object_id('MonitoredSQLServers') and name = 'QueryMonitorExcludeProfiler' collate database_default))
	begin
		declare @SQLServerID int, @xmlstring nvarchar(2000), @lenstring int, @cmd nvarchar(2000)
		create table #UpgradeColumns(SQLServerID int, [QueryMonitorExcludeProfiler] bit, [QueryMonitorExcludeDMO] bit, [QueryMonitorExcludeAgent] bit)
		select @cmd = 'select SQLServerID, isnull([QueryMonitorExcludeProfiler],0), isnull([QueryMonitorExcludeDMO],0), isnull([QueryMonitorExcludeAgent],0) from MonitoredSQLServers'
		insert into #UpgradeColumns
			exec (@cmd)
		select @SQLServerID = min(SQLServerID) from #UpgradeColumns
		while isnull(@SQLServerID,0) > 0
		begin
			set @xmlstring= null
			if (select cast(isnull([QueryMonitorAdvancedConfiguration],'') as nvarchar(2000)) from MonitoredSQLServers where SQLServerID = @SQLServerID) = ''
			begin
				set @xmlstring = '<?xml version="1.0" encoding="utf-16"?><AdvancedQueryMonitorConfiguration xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"><ApplicationExcludeLike>'
				set @lenstring = len(@xmlstring)
				if (select isnull([QueryMonitorExcludeProfiler],0) from #UpgradeColumns where SQLServerID = @SQLServerID) > 0
					set @xmlstring = @xmlstring + '<Like>SQLProfiler%</Like>'
				if (select isnull([QueryMonitorExcludeDMO],0) from #UpgradeColumns where SQLServerID = @SQLServerID) > 0
					set @xmlstring = @xmlstring + '<Like>SQLDMO%</Like>'
				if (select isnull([QueryMonitorExcludeAgent],0) from #UpgradeColumns where SQLServerID = @SQLServerID) > 0
					set @xmlstring = @xmlstring + '<Like>SQLAgent%</Like>'
				if len(@xmlstring) > @lenstring
					set @xmlstring = @xmlstring + '</ApplicationExcludeLike></AdvancedQueryMonitorConfiguration>'
				else
					set @xmlstring = null

				if @xmlstring is not null
					update MonitoredSQLServers
					set [QueryMonitorAdvancedConfiguration] = @xmlstring
					where SQLServerID = @SQLServerID
			end
			select @SQLServerID = min(SQLServerID) from #UpgradeColumns where SQLServerID > @SQLServerID
		end

		alter table MonitoredSQLServers drop constraint DF_MonitoredSQLServers_QueryMonitorExcludeProfiler
		alter table MonitoredSQLServers drop constraint DF_MonitoredSQLServers_QueryMonitorExcludeDMO
		alter table MonitoredSQLServers drop constraint DF_MonitoredSQLServers_QueryMonitorExcludeAgent

		alter table MonitoredSQLServers drop column [QueryMonitorExcludeProfiler] 
		alter table MonitoredSQLServers drop column [QueryMonitorExcludeDMO] 
		alter table MonitoredSQLServers drop column [QueryMonitorExcludeAgent]

		drop table #UpgradeColumns

	end

go
	--upgrade to 10.0 (Vandana Gogna): Add friendly names
	
	IF (not exists(select id from syscolumns where id=object_id('MonitoredSQLServers') and name = 'FriendlyServerName' collate database_default))
	BEGIN
		ALTER TABLE [MonitoredSQLServers]
			ADD [FriendlyServerName] [nvarchar](256) null
	END

----------------------------------------------------------------------

--START : SQLdm 10.0 (Rajesh) Predictive Analytics Changes
IF (OBJECT_ID('PrescriptiveRecommendationCategory') IS NULL)
BEGIN

CREATE TABLE [dbo].[PrescriptiveRecommendationCategory](
	[CategoryID] [int] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[ParentCategory] [int] NULL,
	[Description] [nvarchar](250) NULL,
 CONSTRAINT [PK_PrescriptiveRecommendationCategory] PRIMARY KEY CLUSTERED 
(
	[CategoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

END

--END : SQLdm 10.0 (Rajesh) Predictive Analytics Changes

----------------------------------------------------------------------

--START : SQLdm 10.0 (Rajesh) Predictive Analytics Changes

IF (OBJECT_ID('PrescriptiveRecommendation') IS NULL)
BEGIN
--START : SQLdm 10.0 (srishti) changing size of 4 columns(as follows) to accomadate data 
--Impact Explanation
--Info links
--Description
--Recommendation
--Problem Explanation
CREATE TABLE [dbo].[PrescriptiveRecommendation](
	[RecommendationID] [nvarchar](10) NOT NULL,
	[AdditionalConsiderations] [nvarchar](1500) NULL,
	[bitly] [nvarchar](500) NULL,
	[CategoryID] [int] NOT NULL,
	[ConfidenceFactor] [int] NULL,
	[Description] [nvarchar](4000) NULL,
	[Finding] [nvarchar](500) NULL,
	[ImpactExplanation] [nvarchar](4000) NULL,
	[ImpactFactor] [int] NULL,
	[InfoLinks] [nvarchar](4000) NULL,
	[PluralFormFinding] [nvarchar](500) NULL,
	[PluralFormImpactExplanation] [nvarchar](4000) NULL,
	[PluralFormRecommendation] [nvarchar](4000) NULL,
	[ProblemExplanation] [nvarchar](4000) NULL,
	[Recommendation] [nvarchar](max) NULL,
	[Relevance] [int] NULL,
	[Tags] [nvarchar](500) NULL,
	[IsActive] bit NOT NULL
 CONSTRAINT [PK_PrescripivetRecommendation] PRIMARY KEY CLUSTERED 
(
	[RecommendationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]



ALTER TABLE [dbo].[PrescriptiveRecommendation]  WITH CHECK ADD  CONSTRAINT [FK_PrescriptiveRecommendation_PrescriptiveRecommendationCategory] FOREIGN KEY([CategoryID])
REFERENCES [dbo].[PrescriptiveRecommendationCategory] ([CategoryID])


ALTER TABLE [dbo].[PrescriptiveRecommendation] CHECK CONSTRAINT [FK_PrescriptiveRecommendation_PrescriptiveRecommendationCategory]


END

--END : SQLdm 10.0 (Rajesh) Predictive Analytics Changes

----------------------------------------------------------------------

--START : SQLdm 10.0 (Rajesh) Predictive Analytics Changes

IF (OBJECT_ID('PrescriptiveAnalyzerCategory') IS NULL)

BEGIN

CREATE TABLE [dbo].[PrescriptiveAnalyzerCategory](
	[ID] [int] NOT NULL,
	[Category]  [varchar](100) NOT NULL,
 CONSTRAINT [PK_PrescriptiveAnalyzerCategory] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


END

--END : SQLdm 10.0 (Rajesh) Predictive Analytics Changes

----------------------------------------------------------------------

--START : SQLdm 10.0 (Rajesh) Predictive Analytics Changes

IF (OBJECT_ID('PrescriptiveAnalyzer') IS NULL)

BEGIN

CREATE TABLE [dbo].[PrescriptiveAnalyzer](
	[ID] [int] NOT NULL,
	[Name] [nvarchar](100) NULL,
	[AnalyzerCategoryID] [int] NOT NULL,
 CONSTRAINT [PK_PrescriptiveAnalyzer] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


ALTER TABLE [dbo].[PrescriptiveAnalyzer]  WITH CHECK ADD  CONSTRAINT [FK_PrescriptiveAnalyzer_PrescriptiveAnalyzerCategory] FOREIGN KEY([AnalyzerCategoryID])
REFERENCES [dbo].[PrescriptiveAnalyzerCategory] ([ID])


ALTER TABLE [dbo].[PrescriptiveAnalyzer] CHECK CONSTRAINT [FK_PrescriptiveAnalyzer_PrescriptiveAnalyzerCategory]



END

--END : SQLdm 10.0 (Rajesh) Predictive Analytics Changes


--START : SQLdm 10.0 (srishti purohit) Predictive Analytics types
IF (OBJECT_ID('PrescriptiveAnalysisType') IS NULL)
BEGIN

CREATE TABLE [dbo].[PrescriptiveAnalysisType](
	[AnalysisTypeID] [int] NOT NULL,
	[AnalysisType] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_PrescriptiveAnalysisType] PRIMARY KEY CLUSTERED 
(
	[AnalysisTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

END

--END : SQLdm 10.0 (srishti purohit) Predictive Analytics types

----------------------------------------------------------------------

--START : SQLdm 10.0 (Rajesh) Predictive Analytics Changes

IF (OBJECT_ID('PrescriptiveAnalysis') IS NULL)

BEGIN

CREATE TABLE [dbo].[PrescriptiveAnalysis](
	[AnalysisID] [int] IDENTITY(1,1) NOT NULL,
	[SQLServerID] [int] NOT NULL,
	[UTCAnalysisStartTime] [datetime] NULL,
	[UTCAnalysisCompleteTime] [datetime] NULL,
	[RecommendationCount] [int] NULL,
	--To check which type trigger the analysis
	[AnalysisTypeID] [int] NOT NULL,
	[RecordCreatedTimestamp] [datetime] NULL,
	[RecordUpdateDateTimestamp] [datetime] NULL,
 CONSTRAINT [PK_PrescriptiveAnalysis] PRIMARY KEY CLUSTERED 
(
	[AnalysisID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[PrescriptiveAnalysis]  WITH CHECK ADD  CONSTRAINT [FK_PrescriptiveAnalysis_PrescriptiveAnalysisType] FOREIGN KEY([AnalysisTypeID])
REFERENCES [dbo].[PrescriptiveAnalysisType] ([AnalysisTypeID])

END

--END : SQLdm 10.0 (Rajesh) Predictive Analytics Changes

----------------------------------------------------------------------


IF (OBJECT_ID('PrescriptiveAnalysisDetails') IS NULL)
BEGIN

CREATE TABLE [dbo].[PrescriptiveAnalysisDetails](
	[PrescriptiveAnalysisDetailsID] [int] IDENTITY(1,1) NOT NULL,
	[AnalysisID] [int] NOT NULL,
	[AnalyzerID] [int] NOT NULL,
	[Status] [int] NULL,
	[RecommendationCount] [int] NULL,
	[RecordCreatedTimestamp] [datetime] NULL,
	[RecordUpdateDateTimestamp] [datetime] NULL,
 CONSTRAINT [PK_PrescriptiveAnalysisDetails] PRIMARY KEY CLUSTERED 
(
	[PrescriptiveAnalysisDetailsID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


ALTER TABLE [dbo].[PrescriptiveAnalysisDetails]  WITH CHECK ADD  CONSTRAINT [FK_PrescriptiveAnalysisDetails_PrescriptiveAnalysis] FOREIGN KEY([AnalysisID])
REFERENCES [dbo].[PrescriptiveAnalysis] ([AnalysisID]) ON DELETE CASCADE;

ALTER TABLE [dbo].[PrescriptiveAnalysisDetails] CHECK CONSTRAINT [FK_PrescriptiveAnalysisDetails_PrescriptiveAnalysis]

ALTER TABLE [dbo].[PrescriptiveAnalysisDetails]  WITH CHECK ADD  CONSTRAINT [FK_PrescriptiveAnalysisDetails_PrescriptiveAnalyzer] FOREIGN KEY([AnalyzerID])
REFERENCES [dbo].[PrescriptiveAnalyzer] ([ID])

ALTER TABLE [dbo].[PrescriptiveAnalysisDetails] CHECK CONSTRAINT [FK_PrescriptiveAnalysisDetails_PrescriptiveAnalyzer]


--END : SQLdm 10.0 (srishti) To support grooming



END

--END : SQLdm 10.0 (Rajesh) Predictive Analytics Changes

----------------------------------------------------------------------

--START : SQLdm 10.0 (Rajesh) Predictive Analytics Changes
--OptimiztionExecutionStatusMasterTable
IF (OBJECT_ID('PrescriptiveOptimiztionExecutionStatus') IS NULL)
BEGIN
	CREATE TABLE [PrescriptiveOptimiztionExecutionStatus](
		 [ID] [int] NOT NULL,
		 [Status] [nvarchar](100) NOT NULL,
		 [Description] [nvarchar](250) NULL,
		 
		CONSTRAINT [pk_PrescriptiveOptimiztionExecutionStatusID] PRIMARY KEY CLUSTERED 
		(
			[ID] ASC
		))
END

IF (OBJECT_ID('PrescriptiveAnalysisRecommendation') IS NULL)
BEGIN

CREATE TABLE [dbo].[PrescriptiveAnalysisRecommendation](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[RecommendationID] [nvarchar](10) NOT NULL,
	----Removing Columns which are directly referenced from PrescriptiveRecommendation
	--[RankID] [int] NULL, removed due to not useful
	[ComputedRankFactor] [float] NULL,
	--[Relevance] [decimal](18, 0) NULL,
	[PrescriptiveAnalysisDetailsID] [int] NULL,
	--[Description] [nvarchar](4000) NULL,
	--[Finding] [nvarchar](500) NULL,
	--[ImpactExplanation] [nvarchar](4000) NULL,
	--[ProblemExplanation] [nvarchar](4000) NULL,
    --[Recommendation] [nvarchar](max) NULL,
	[IsFlagged] [bit] NULL,
	[OptimizationStatusID] [INT] NOT NULL,	
	[OptimizationErrorMessage] [nvarchar](max) NULL,
 CONSTRAINT [PK_PrescriptiveAnalysisRecommendation] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


ALTER TABLE [dbo].[PrescriptiveAnalysisRecommendation]  WITH CHECK ADD  CONSTRAINT [FK_PrescriptiveAnalysisRecommendation_PrescriptiveRecommendation] FOREIGN KEY([PrescriptiveAnalysisDetailsID])
REFERENCES [dbo].[PrescriptiveAnalysisDetails] ([PrescriptiveAnalysisDetailsID]) ON DELETE CASCADE;

ALTER TABLE [dbo].[PrescriptiveAnalysisRecommendation] CHECK CONSTRAINT [FK_PrescriptiveAnalysisRecommendation_PrescriptiveRecommendation]

ALTER TABLE [dbo].[PrescriptiveAnalysisRecommendation]  WITH CHECK ADD  CONSTRAINT [FK_PrescriptiveAnalysisRecommendation_PrescriptiveRecommendation1] FOREIGN KEY([RecommendationID])
REFERENCES [dbo].[PrescriptiveRecommendation] ([RecommendationID])


ALTER TABLE [dbo].[PrescriptiveAnalysisRecommendation] CHECK CONSTRAINT [FK_PrescriptiveAnalysisRecommendation_PrescriptiveRecommendation1]

ALTER TABLE [dbo].[PrescriptiveAnalysisRecommendation]  WITH CHECK ADD  CONSTRAINT [FK_OptimizationID] FOREIGN KEY([OptimizationStatusID])
REFERENCES [dbo].PrescriptiveOptimiztionExecutionStatus ([ID])
ALTER TABLE [dbo].[PrescriptiveAnalysisRecommendation] CHECK CONSTRAINT [FK_OptimizationID]

END
--END : SQLdm 10.0 (Rajesh) Predictive Analytics Changes

----------------------------------------------------------------------

IF (OBJECT_ID('ServerStatistics') IS NULL)
BEGIN

CREATE TABLE  [ServerStatistics] 
	(
	[SQLServerID] [int] NOT NULL ,
	[UTCCollectionDateTime] [datetime] NOT NULL ,
	[TimeDeltaInSeconds] [float] NOT NULL ,
	[AgentServiceStatus] [int] NULL,
	[DataIOUsage] [float] NULL,
	[LogIOUsage] [float] NULL,
	[DataIORate] [float] NULL,
	[LogIORate] [float] NULL,
    [SqlServerServiceStatus] [int] NULL,
    [DTCServiceStatus] [int] NULL,
    [FullTextSearchStatus] [int] NULL,
	[BufferCacheHitRatioPercentage] [float] NULL,
	[CheckpointWrites] [bigint] NULL,
	[ClientComputers] [int] NULL,
	[CPUActivityPercentage] [float] NULL,
	[CPUTimeDelta] [bigint] NULL,
	[CPUTimeRaw] [bigint] NULL,
	[FullScans] [bigint] NULL,
	[IdleTimeDelta] [bigint] NULL,
	[IdleTimePercentage] [float] NULL,
	[IdleTimeRaw] [bigint] NULL,
	[IOActivityPercentage] [float] NULL,
	[IOTimeDelta] [bigint] NULL,
	[IOTimeRaw] [bigint] NULL,
	[LazyWriterWrites] [bigint] NULL,
	[LockWaits] [bigint] NULL,
	[Logins] [bigint] NULL,
	[LogFlushes] [bigint] NULL,
	[SqlMemoryAllocatedInKilobytes] [bigint] NULL,
	[SqlMemoryUsedInKilobytes] [bigint] NULL,
	[OldestOpenTransactionsInMinutes] [bigint] NULL,
	[PacketErrors] [bigint] NULL,
	[PacketsReceived] [bigint] NULL,
	[PacketsSent] [bigint] NULL,
	[PageErrors] [bigint] NULL,
	[PageLifeExpectancy] [bigint] NULL,
	[PageLookups] [bigint] NULL,
	[PageReads] [bigint] NULL,
	[PageSplits] [bigint] NULL,
	[PageWrites] [bigint] NULL,
	[ProcedureCacheHitRatioPercentage] [float] NULL,
	[ProcedureCacheSizeInKilobytes] [bigint] NULL,
	[ProcedureCacheSizePercent] [float] NULL,
	[ReadAheadPages] [bigint] NULL,
	[ReplicationLatencyInSeconds] [float] NULL,
	[DistributionLatencyInSeconds] [float] NULL,
	[ReplicationSubscribed] [bigint] NULL,
	[ReplicationUndistributed] [bigint] NULL,
	[ReplicationUnsubscribed] [bigint] NULL,
	[ResponseTimeInMilliseconds] [int] NULL,
	[ServerVersion] [nvarchar](30) NULL,
	[SqlCompilations] [bigint] NULL,
	[SqlRecompilations] [bigint] NULL,
	[TableLockEscalations] [bigint] NULL,
	[TempDBSizeInKilobytes] [bigint] NULL,
	[TempDBSizePercent] [float] NULL,
	[Batches] [bigint] NULL,
	[UserProcesses] [int] NULL,
	[WorkFilesCreated] [bigint] NULL,
	[WorkTablesCreated] [bigint] NULL,
	[SystemProcesses] [int] NULL,
	[UserProcessesConsumingCPU] [int] NULL, 
	[SystemProcessesConsumingCPU] [int] NULL, 
	[BlockedProcesses] [int] NULL, 
	[OpenTransactions] [int] NULL,
	[DatabaseCount] [int] NULL,
	[DataFileCount] [int] NULL,
	[LogFileCount] [int] NULL,
	[DataFileSpaceAllocatedInKilobytes] [decimal] NULL,
	[DataFileSpaceUsedInKilobytes] [decimal] NULL,
	[LogFileSpaceAllocatedInKilobytes] [decimal] NULL,
	[LogFileSpaceUsedInKilobytes] [decimal] NULL,
	[TotalLocks] [decimal] NULL,
	[BufferCacheSizeInKilobytes] [bigint] NULL,
	[ActiveProcesses] [int] NULL,
	[LeadBlockers] [int] NULL,
	[CommittedInKilobytes] [bigint] NULL,
	[ConnectionMemoryInKilobytes] [bigint] NULL,
	[FreePagesInKilobytes] [bigint] NULL,
	[GrantedWorkspaceMemoryInKilobytes] [bigint] NULL,
	[LockMemoryInKilobytes] [bigint] NULL,
	[OptimizerMemoryInKilobytes] [bigint] NULL,
	[TotalServerMemoryInKilobytes] [bigint] NULL,
	[FreeCachePagesInKilobytes] [bigint] NULL,
	[CachePagesInKilobytes] [bigint] NULL,
	[MaxConnections] [bigint] NULL,
	[PhysicalMemoryInKilobytes] [bigint] NULL,
	[ProcessorCount] [int] NULL,
	[ProcessorsUsed] [int] NULL,
	[ProcessorType]  [nvarchar](20) NULL,
	[ServerHostName] [nvarchar](50) NULL,
	[RealServerName] [nvarchar](100) NULL,
	[WindowsVersion] [nvarchar](128) NULL,
	[SqlServerEdition] [nvarchar](30) NULL,
	[RunningSince] [datetime] NULL,
	[IsClustered] [bit] NULL,
	[ClusterNodeName] [nvarchar](256) NULL,
	[OsStatisticAvailability] [nvarchar](50) NULL,
	[Transactions] [bigint] NULL,
	[VersionStoreGenerationKilobytes] [bigint] NULL,
	[VersionStoreCleanupKilobytes] [bigint] NULL,
	[TempdbPFSWaitTimeMilliseconds] [bigint] NULL,
	[TempdbGAMWaitTimeMilliseconds] [bigint] NULL,
	[TempdbSGAMWaitTimeMilliseconds] [bigint] NULL,
	[SQLBrowserServiceStatus] [int] NULL,    --SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --add SQL browser service status column
	[SQLActiveDirectoryHelperServiceStatus] [int] NULL,  --SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --add SQL Active Directory Helper Service status column
	[ManagedInstanceStorageLimit] [decimal] NULL,
	[ReadThroughput] [float] NULL,
	[WriteThroughput] [float] NULL,
	[SwapUsage] [float] NULL,
	[ReadLatency] [float] NULL,
	[WriteLatency] [float] NULL,
	[CPUCreditBalance] [float] NULL,
	[CPUCreditUsage] [float] NULL,
	[DiskQueueDepth] [float] NULL,
	CONSTRAINT [PKServerStatistics] PRIMARY KEY CLUSTERED 
	(
		[SQLServerID],
		[UTCCollectionDateTime]
	),
	CONSTRAINT [FKServerStatisticsMonitoredSQLServers] FOREIGN KEY 
		(
		[SQLServerID]
		) 
	REFERENCES  [MonitoredSQLServers] 
		(
		[SQLServerID]
		) on delete cascade
	)
	-- 6.0 added for new installs (grooming job handles upgrade installs)
	CREATE INDEX [IXServerStatistics] ON [ServerStatistics]([SQLServerID],[UTCCollectionDateTime]) 
END
ELSE
BEGIN
	-- upgrade to 5.5.0
	IF (not exists(select id from syscolumns where id = object_id('ServerStatistics') and name = 'ActiveProcesses' collate database_default)) 
	BEGIN
		ALTER TABLE [ServerStatistics] 
			ADD 
			[ActiveProcesses] [int] NULL,
			[LeadBlockers] [int] NULL,
			[CommittedInKilobytes] [bigint] NULL,
			[ConnectionMemoryInKilobytes] [bigint] NULL,
			[FreePagesInKilobytes] [bigint] NULL,
			[GrantedWorkspaceMemoryInKilobytes] [bigint] NULL,
			[LockMemoryInKilobytes] [bigint] NULL,
			[OptimizerMemoryInKilobytes] [bigint] NULL,
			[TotalServerMemoryInKilobytes] [bigint] NULL,
			[FreeCachePagesInKilobytes] [bigint] NULL,
			[CachePagesInKilobytes] [bigint] NULL,
			[MaxConnections] [bigint] NULL,
			[PhysicalMemoryInKilobytes] [bigint] NULL,
			[ProcessorCount] [int] NULL,
			[ProcessorsUsed] [int] NULL,
			[ProcessorType]  [nvarchar](20) NULL,
			[ServerHostName] [nvarchar](50) NULL,
			[RealServerName] [nvarchar](100) NULL,
			[WindowsVersion] [nvarchar](128) NULL,
			[SqlServerEdition] [nvarchar](30) NULL,
			[RunningSince] [datetime] NULL,
			[IsClustered] [bit] NULL,
			[OsStatisticAvailability] [nvarchar](50) NULL
	END
	-- upgrade to 6.1
	IF (not exists(select id from syscolumns where id = object_id('ServerStatistics') and name = 'ClusterNodeName' collate database_default)) 
	BEGIN
		ALTER TABLE [ServerStatistics] 
			ADD [ClusterNodeName] [nvarchar](256) NULL
	END
	IF (not exists(select id from syscolumns where id = object_id('ServerStatistics') and name = 'DistributionLatencyInSeconds' collate database_default)) 
	BEGIN
		ALTER TABLE [ServerStatistics] 
			ADD [DistributionLatencyInSeconds] [float] NULL
	END
	if (not exists(select object_id from sys.columns where object_id = object_id('ServerStatistics') and name = 'VersionStoreGenerationKilobytes' collate database_default))
	begin
		exec sp_rename 'ServerStatistics.Transactions', 'Batches', 'COLUMN';

		ALTER TABLE [ServerStatistics] 
			ADD 
			[Transactions] [bigint] NULL,
			[VersionStoreGenerationKilobytes] [bigint] NULL,
				[VersionStoreCleanupKilobytes] [bigint] NULL,
				[TempdbPFSWaitTimeMilliseconds] [bigint] NULL,
				[TempdbGAMWaitTimeMilliseconds] [bigint] NULL,
				[TempdbSGAMWaitTimeMilliseconds] [bigint] NULL
	end
	--START: SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --add new SQL service status columns
	IF (not exists(select id from syscolumns where id = object_id('ServerStatistics') and name = 'SQLBrowserServiceStatus' collate database_default)) 
	BEGIN
		ALTER TABLE [ServerStatistics] 
			ADD [SQLBrowserServiceStatus] [int] NULL
	END

	IF (not exists(select id from syscolumns where id = object_id('ServerStatistics') and name = 'SQLActiveDirectoryHelperServiceStatus' collate database_default)) 
	BEGIN
		ALTER TABLE [ServerStatistics] 
			ADD [SQLActiveDirectoryHelperServiceStatus] [int] NULL
	END
	--END: SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --add new SQL service status columns
	IF (not exists(select id from syscolumns where id = object_id('ServerStatistics') and name = 'DataIOUsage' collate database_default)) 
	BEGIN
		ALTER TABLE [ServerStatistics] 
			ADD 
			[DataIOUsage] [float] NULL,
			[LogIOUsage] [float] NULL,
			[DataIORate] [float] NULL,
			[LogIORate] [float] NULL
	END

	IF (not exists(select id from syscolumns where id = object_id('ServerStatistics') and name = 'ManagedInstanceStorageLimit' collate database_default)) 
	BEGIN
		ALTER TABLE [ServerStatistics] 
			ADD 
			[ManagedInstanceStorageLimit] [decimal] NULL
	END
	IF (not exists(select id from syscolumns where id = object_id('ServerStatistics') and name = 'ReadThroughput' collate database_default)) 
	BEGIN
		ALTER TABLE [ServerStatistics] 
			ADD 
			[ReadThroughput] [float] NULL,
			[WriteThroughput] [float] NULL,
			[SwapUsage] [float] NULL,
			[ReadLatency] [float] NULL,
			[WriteLatency] [float] NULL,
			[CPUCreditBalance] [float] NULL,
			[CPUCreditUsage] [float] NULL,
			[DiskQueueDepth] [float] NULL
	END
END

----------------------------------------------------------------------

IF (OBJECT_ID('OSStatistics') IS NULL)
BEGIN

CREATE TABLE  [OSStatistics] 
	(
	[SQLServerID] [int] NOT NULL ,
	[UTCCollectionDateTime] [datetime] NOT NULL ,
	[OSTotalPhysicalMemoryInKilobytes] [bigint] NULL,
	[OSAvailableMemoryInKilobytes] [bigint] NULL,
	[PagesPerSecond] [float] NULL,
	[ProcessorTimePercent] [float] NULL,
	[PrivilegedTimePercent] [float] NULL,
	[UserTimePercent] [float] NULL,	
	[ProcessorQueueLength] [float] NULL,
	[DiskTimePercent] [float] NULL,
	[DiskQueueLength] [float] NULL,
	[AvailableByteVm] [bigint] NULL,
	[UsedByteVm] [bigint] NULL,
	[TotalByteVm] [bigint] NULL,
	CONSTRAINT [PKOSStatistics] PRIMARY KEY CLUSTERED 
	(
		[SQLServerID],
		[UTCCollectionDateTime]
	),
	CONSTRAINT [FKSOSStatisticsMonitoredSQLServers] FOREIGN KEY 
		(
		[SQLServerID]
		) 
	REFERENCES  [MonitoredSQLServers] 
		(
		[SQLServerID]
		) on delete cascade
	)
/*ELSE

	ALTER TABLE [dbo].[OSStatistics] ADD[AvailableByteVm] [bigint] NULL
	ALTER TABLE [dbo].[OSStatistics] ADD[UsedByteVm] [bigint] NULL
	ALTER TABLE [dbo].[OSStatistics] ADD[TotalByteVm] [bigint] NULL
*/

	BEGIN
		IF (not exists(select id from syscolumns where id=object_id('OSStatistics') and name = 'AvailableByteVm' collate database_default))
		BEGIN
			ALTER TABLE [OSStatistics]
				ADD [AvailableByteVm] [bigint] NULL
		END
	END
	BEGIN
		IF (not exists(select id from syscolumns where id=object_id('OSStatistics') and name = 'UsedByteVm' collate database_default))
		BEGIN
			ALTER TABLE [OSStatistics]
				ADD [UsedByteVm] [bigint] NULL
		END
	END
	BEGIN
		IF (not exists(select id from syscolumns where id=object_id('OSStatistics') and name = 'TotalByteVm' collate database_default))
		BEGIN
			ALTER TABLE [OSStatistics]
				ADD [TotalByteVm] [bigint] NULL
		END
	END

END

----------------------------------------------------------------------

IF (OBJECT_ID('QueryMonitor') IS NOT NULL)
BEGIN

IF (not exists(select id from syscolumns where id = object_id('QueryMonitor') and name = 'Spid' collate database_default)) 
begin
	ALTER TABLE [QueryMonitor]
			ADD Spid [int] NULL
end

IF (not exists(select id from syscolumns where id = object_id('QueryMonitor') and name = 'DeleteFlag' collate database_default)) 
begin
	ALTER TABLE [QueryMonitor]
			ADD DeleteFlag [bit] NULL
end


END

----------------------------------------------------------------------

IF (OBJECT_ID('SQLServerDatabaseNames') IS NULL)
BEGIN
CREATE TABLE  [SQLServerDatabaseNames] 
	(
	[DatabaseID] [int] IDENTITY NOT NULL ,
	[SQLServerID] [int] NOT NULL ,
	[DatabaseName] [nvarchar] (255) NOT NULL,
	[SystemDatabase] [bit] NOT NULL,
	[CreationDateTime] [datetime] DEFAULT NULL,
	--  SQLdm kit1(Barkha Khatri) adding IsDeleted column to SQLServerDatabaseNames
	[IsDeleted] [bit] NOT NULL DEFAULT 0
	CONSTRAINT [PKSQLServerDatabaseNames] PRIMARY KEY CLUSTERED 
	(
		[DatabaseID]
	), 
	CONSTRAINT [FKSQLServerDatabaseNamesMonitoredSQLServers] FOREIGN KEY 
		(		
		[SQLServerID]	
		) 
	REFERENCES  [MonitoredSQLServers] 
		(
		[SQLServerID]
		) on delete cascade
	)
--Srishti--4.1.5 Index Suitability Fixes -- DROPPING INDEX
--CREATE INDEX [IXSQLServerDatabaseNames] ON [SQLServerDatabaseNames]([SQLServerID],[DatabaseName]) 
END
ELSE
BEGIN
	IF (not exists(select id from syscolumns where id=object_id('SQLServerDatabaseNames') and name = 'CreationDateTime' collate database_default))
	BEGIN
		ALTER TABLE [SQLServerDatabaseNames]
			ADD [CreationDateTime] [datetime] DEFAULT NULL	
	END
END

if not exists(select * from sys.indexes where name = 'IXSQLServerDatabaseNameServerIDDBNameInclDBID')
Begin
	CREATE NONCLUSTERED INDEX [IXSQLServerDatabaseNameServerIDDBNameInclDBID] ON [dbo].[SQLServerDatabaseNames] 
	(
		[SQLServerID] ASC,
		[DatabaseName] ASC
	)
	INCLUDE ( [DatabaseID]) ON [PRIMARY]
end

--Srishti--4.1.5 Index Suitability Fixes -- DROPPING and RECREATING INDEX
--if not exists(select * from sys.indexes where name = 'IXSQLServerDatabaseNamesServerIDDBID')
--Begin
--	CREATE NONCLUSTERED INDEX [IXSQLServerDatabaseNamesServerIDDBID] ON [dbo].[SQLServerDatabaseNames] 
--	(
--		[SQLServerID] ASC,
--		[DatabaseID] ASC
--	) ON [PRIMARY]
--end


--  SQLdm kit1(Barkha Khatri) adding IsDeleted column to SQLServerDatabaseNames
	IF NOT EXISTS(SELECT id FROM syscolumns WHERE id = object_id(N'dbo.SQLServerDatabaseNames') and name='IsDeleted' collate database_default)
	BEGIN
		ALTER TABLE [SQLServerDatabaseNames]
        ADD [IsDeleted] [bit] NOT NULL DEFAULT 0
	END

----------------------------------------------------------------------


IF (OBJECT_ID('DatabaseFiles') IS NULL)
BEGIN
CREATE TABLE  [DatabaseFiles] 
	(
	[FileID]  [int] IDENTITY NOT NULL ,
	[DatabaseID] [int] NOT NULL ,
	[FileName] [nvarchar](255),
	[FileType] [int],
	[FilePath] [nvarchar](MAX), --Update by Gaurav Karwal to accommodate new data
	[DriveName] [nvarchar](256),
	-- SQLdm 9.1 (Abhishek Joshi) -Filegroup improvements -add column for filegroup name
	[FileGroupName] [nvarchar](4000),
	CONSTRAINT [PKDatabaseFiles] PRIMARY KEY  CLUSTERED 
		(
		[FileID]
		), 
	CONSTRAINT [FKDatabaseFilesSQLServerDatabaseNames] FOREIGN KEY 
		(
		[DatabaseID]
		) 
	REFERENCES  [SQLServerDatabaseNames] 
		(
		[DatabaseID]
		)  on delete cascade
	)
END
--START: SQLdm 9.1 (Abhishek Joshi) -Filegroup improvements -add FileGroupName column if table already exists
ELSE
BEGIN
	if not exists(select name from sys.columns where object_id = object_id('DatabaseFiles') and name=N'FileGroupName' collate database_default )
	BEGIN
		alter table [DatabaseFiles]
			add [FileGroupName] [nvarchar](4000)
	END
	--Gaurav Karwal SQLdm 9.1: For accommodating screwed up file paths
	if exists(select name from sys.columns where object_id = object_id('DatabaseFiles') and name=N'FilePath' collate database_default )
	BEGIN
		alter table [DatabaseFiles]
			alter column [FilePath] [nvarchar](MAX)
	END
END
--END: SQLdm 9.1 (Abhishek Joshi) -Filegroup improvements -add FileGroupName column if table already exists


----------------------------------------------------------------------



IF (OBJECT_ID('DatabaseFileActivity') IS NULL)
BEGIN
CREATE TABLE  [DatabaseFileActivity] 
	(
	[FileID] [int] NOT NULL ,
	[UTCCollectionDateTime] [datetime] NOT NULL,
	[Reads] [bigint],
	[Writes] [bigint],
	[TimeDeltaInSeconds] [float],
	CONSTRAINT [PKDatabaseFileActivity] PRIMARY KEY CLUSTERED 
		(
		[UTCCollectionDateTime],
		[FileID]
		), 
	CONSTRAINT [FKDatabaseFileActivityDatabaseFiles] FOREIGN KEY 
		(
		[FileID]
		) 
	REFERENCES  [DatabaseFiles] 
		(
		[FileID]
		)  on delete cascade
	)
END



----------------------------------------------------------------------

IF (OBJECT_ID('TempdbFileData') IS NULL)
BEGIN
CREATE TABLE  [TempdbFileData] 
	(
	[FileID] [int] NOT NULL ,
	[UTCCollectionDateTime] [datetime] NOT NULL,
	[FileSizeInKilobytes] [bigint],
	[UserObjectsInKilobytes] [bigint],
	[InternalObjectsInKilobytes] [bigint],
	[VersionStoreInKilobytes] [bigint],
	[MixedExtentsInKilobytes] [bigint],
	[UnallocatedSpaceInKilobytes] [bigint],
	[TimeDeltaInSeconds] [float],
	CONSTRAINT [PKTempdbFileData] PRIMARY KEY CLUSTERED 
		(
		[UTCCollectionDateTime],
		[FileID]
		), 
	CONSTRAINT [FKTempdbFileActivityDatabaseFiles] FOREIGN KEY 
		(
		[FileID]
		) 
	REFERENCES  [DatabaseFiles] 
		(
		[FileID]
		)  on delete cascade
	)
END

----------------------------------------------------------------------

-- upgrade to 7.5
if (exists(select object_id from sys.columns where object_id = object_id('DatabaseStatistics') and name = 'DataFileSizeInKilobytes' collate database_default)) 
BEGIN
	-- upgrade to 6.0
	IF (not exists(select id from syscolumns where id = object_id('DatabaseStatistics') and name = 'NumberReads' collate database_default)) 
	BEGIN
		ALTER TABLE [DatabaseStatistics] 
			ADD
			[NumberReads] [decimal] NULL,
			[NumberWrites] [decimal] NULL,
			[BytesRead] [decimal] NULL,
			[BytesWritten] [decimal] NULL,
			[IoStallMS] [decimal] NULL
	end
	
	declare @cmd nvarchar(1000)
	Set @cmd = (
    select N'drop index DatabaseStatistics.' + name + '; '
    from sys.indexes where object_id = object_id('DatabaseStatistics') and type > 1
    for xml path(''))
    
    exec sp_executesql @cmd

	exec sp_rename 'FKDatabaseStatisticsSQLServerDatabaseNames','FKDatabaseStatisticsSQLServerDatabaseNames_upgrade'
	exec sp_rename 'DatabaseStatistics','DatabaseStatistics_upgrade'
	exec sp_rename 'PKDatabaseStatistics','PKDatabaseStatistics_upgrade'
	
END


IF (OBJECT_ID('DatabaseStatistics') IS NULL)
BEGIN
	CREATE TABLE  [DatabaseStatistics] 
		(
		[DatabaseStatisticsID] [bigint] IDENTITY(-9223372036854775808,1) UNIQUE CLUSTERED,
		[DatabaseID] [int] NOT NULL ,
		[UTCCollectionDateTime] [datetime] NOT NULL,
		[DatabaseStatus] [int] NULL,
		[Transactions] [bigint] NULL ,
		[LogFlushWaits] [bigint] NULL ,
		[LogFlushes] [bigint] NULL ,
		[LogKilobytesFlushed] [bigint] NULL ,
		[LogCacheReads] [bigint] NULL ,
		[LogCacheHitRatio] [float] NULL ,
		[TimeDeltaInSeconds] [float],
		[NumberReads] [decimal] NULL,
		[NumberWrites] [decimal] NULL,
		[BytesRead] [decimal] NULL,
		[BytesWritten] [decimal] NULL,
		[IoStallMS] [decimal] NULL,
		[DatabaseSizeTime] [datetime] NULL,
		[LastBackupDateTime] [datetime] NULL, -- SQLdm 10.0 (Vandana Gogna) - Database backup alerts
		[AverageDataIO] [decimal](18, 4) NULL, --SQLdm 11.0 (Azure Support)
		[AverageLogIO] [decimal](18, 4) NULL,
		[MaxWorker] [decimal](18, 4) NULL,
		[MaxSession] [decimal](18, 4) NULL,
		[DatabaseAverageMemoryUsage] [decimal](18, 4) NULL,
		[InMemoryStorageUsage] [decimal](18, 4) NULL,
		[AvgCpuPercent] [decimal] NULL, -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
		[AvgDataIoPercent] [decimal] NULL, -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
		[AvgLogWritePercent] [decimal] NULL, -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
		[DtuLimit] [int] NULL, -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
		[AgverageMemoryUsage] [decimal](18, 4) NULL,
		[AzureCloudAllocatedMemory] [decimal] NULL,
		[AzureCloudUsedMemory] [decimal] NULL,
		[AzureCloudStorageLimit] [decimal] NULL,
		[ElasticPool][nvarchar](255) NULL,
		CONSTRAINT [PKDatabaseStatistics] PRIMARY KEY NONCLUSTERED 
			(
			[DatabaseID],
			[UTCCollectionDateTime]
			), 
		CONSTRAINT [FKDatabaseStatisticsSQLServerDatabaseNames] FOREIGN KEY 
			(
			[DatabaseID]
			) 
		REFERENCES  [SQLServerDatabaseNames] 
			(
			[DatabaseID]
			)  on delete cascade
		)
	

END
--START SQLdm 10.0 (Vandana Gogna) - Database backup alerts
ELSE
BEGIN
	IF (not exists(select id from syscolumns where id = object_id('DatabaseStatistics') and name = 'LastBackupDateTime' collate database_default)) 
	BEGIN
		ALTER TABLE [DatabaseStatistics] 
			ADD
			[LastBackupDateTime] [datetime] NULL
	END
	--START SQLdm 11.0 - Azure Support
	IF (not exists(select id from syscolumns where id = object_id('DatabaseStatistics') and name = 'AverageDataIO' collate database_default)) 
	BEGIN
		ALTER TABLE [DatabaseStatistics] 
			ADD
			[AverageDataIO] [decimal](18, 4) NULL, --SQLdm 11.0 (Azure Support)
		    [AverageLogIO] [decimal](18, 4) NULL,
		    [MaxWorker] [decimal](18, 4) NULL,
		    [MaxSession] [decimal](18, 4) NULL,
		    [DatabaseAverageMemoryUsage] [decimal](18, 4) NULL,
		    [InMemoryStorageUsage] [decimal](18, 4) NULL
	END	
	IF (not exists(select id from syscolumns where id = object_id('DatabaseStatistics') and name = 'AzureCloudAllocatedMemory' collate database_default)) 
	BEGIN
		ALTER TABLE [DatabaseStatistics] 
			ADD
			[AzureCloudAllocatedMemory] [decimal] NULL
	END
	IF (not exists(select id from syscolumns where id = object_id('DatabaseStatistics') and name = 'AzureCloudUsedMemory' collate database_default)) 
	BEGIN
		ALTER TABLE [DatabaseStatistics] 
			ADD
			[AzureCloudUsedMemory] [decimal] NULL
	END
	IF (not exists(select id from syscolumns where id = object_id('DatabaseStatistics') and name = 'AzureCloudStorageLimit' collate database_default)) 
	BEGIN
		ALTER TABLE [DatabaseStatistics] 
			ADD
			[AzureCloudStorageLimit] [decimal] NULL
	END
	IF (not exists(select id from syscolumns where id = object_id('DatabaseStatistics') and name = 'ElasticPool' collate database_default)) 
	BEGIN
		ALTER TABLE [DatabaseStatistics] 
			ADD
			[ElasticPool][nvarchar](255) NULL
	END
END
--END SQLdm 10.0 (Vandana Gogna) - Database backup alerts

 -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
IF (not exists(select id from syscolumns where id = object_id('DatabaseStatistics') and name = 'AvgCpuPercent' collate database_default)) 
	BEGIN
		ALTER TABLE [DatabaseStatistics] 
			ADD
			[AvgCpuPercent] [decimal] NULL, -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
			[AvgDataIoPercent] [decimal] NULL, -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
			[AvgLogWritePercent] [decimal] NULL, -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
			[DtuLimit] [int] NULL -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
	END

IF (OBJECT_ID('stageDatabaseStatistics') IS NULL)
BEGIN
	CREATE TABLE [dbo].[stageDatabaseStatistics]
	(
		[SourceID] [uniqueidentifier] NOT NULL,
		[ServerID] [int] NOT NULL,
		[DatabaseName] [nvarchar](255) NOT NULL,
		[SystemDatabase] [bit] NOT NULL,
		[UTCCollectionDateTime] [datetime] NOT NULL,
		[DatabaseStatus] [int] NULL,
		[Transactions] [bigint] NULL,
		[LogFlushWaits] [bigint] NULL,
		[LogFlushes] [bigint] NULL,
		[LogKilobytesFlushed] [bigint] NULL,
		[LogCacheReads] [bigint] NULL,
		[LogCacheHitRatio] [float] NULL,
		[TimeDeltaInSeconds] [float] NULL,
		[NumberReads] [decimal](18, 0) NULL,
		[NumberWrites] [decimal](18, 0) NULL,
		[BytesRead] [decimal](18, 0) NULL,
		[BytesWritten] [decimal](18, 0) NULL,
		[IoStallMS] [decimal](18, 0) NULL,
		[DatabaseCreateDate] [datetime] NULL,
		[LastBackupDateTime] [datetime] NULL, -- SQLdm 10.0 (Vandana Gogna) - Database backup alerts
		[AverageDataIO] [decimal](18, 4) NULL, --SQLdm 11.0 (Azure Support)
		[AverageLogIO] [decimal](18, 4) NULL,
		[MaxWorker] [decimal](18, 4) NULL,
		[MaxSession] [decimal](18, 4) NULL,
		[DatabaseAverageMemoryUsage] [decimal](18, 4) NULL,
		[InMemoryStorageUsage] [decimal](18, 4) NULL,
		[AvgCpuPercent] [decimal] NULL, -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
		[AvgDataIoPercent] [decimal] NULL, -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
		[AvgLogWritePercent] [decimal] NULL, -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
		[DtuLimit] [int] NULL, -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
		[AzureCloudAllocatedMemory] [decimal] NULL,
		[AzureCloudUsedMemory] [decimal] NULL,
		[AzureCloudStorageLimit] [decimal] NULL,
		[ElasticPool][nvarchar](255) NULL
	)
END
ELSE
BEGIN
	--START SQLdm 10.0 (Vandana Gogna) - Database backup alerts
	IF (not exists(select id from syscolumns where id = object_id('stageDatabaseStatistics') and name = 'LastBackupDateTime' collate database_default)) 
	BEGIN
		ALTER TABLE [stageDatabaseStatistics] 
			ADD
			[LastBackupDateTime] [datetime] NULL
	END
	IF (not exists(select id from syscolumns where id = object_id('stageDatabaseStatistics') and name = 'AverageDataIO' collate database_default)) 
	BEGIN
		ALTER TABLE [stageDatabaseStatistics] 
			ADD
			[AverageDataIO] [decimal](18, 4) NULL, --SQLdm 11.0 (Azure Support)
		    [AverageLogIO] [decimal](18, 4) NULL,
		    [MaxWorker] [decimal](18, 4) NULL,
		    [MaxSession] [decimal](18, 4) NULL,
		    [DatabaseAverageMemoryUsage] [decimal](18, 4) NULL,
		    [InMemoryStorageUsage] [decimal](18, 4) NULL
	END	IF (not exists(select id from syscolumns where id = object_id('stageDatabaseStatistics') and name = 'AzureCloudAllocatedMemory' collate database_default)) 
	BEGIN
		ALTER TABLE [stageDatabaseStatistics] 
			ADD
			[AzureCloudAllocatedMemory] [decimal] NULL
	END
	IF (not exists(select id from syscolumns where id = object_id('stageDatabaseStatistics') and name = 'AzureCloudUsedMemory' collate database_default)) 
	BEGIN
		ALTER TABLE [stageDatabaseStatistics] 
			ADD
			[AzureCloudUsedMemory] [decimal] NULL
	END
	IF (not exists(select id from syscolumns where id = object_id('stageDatabaseStatistics') and name = 'AzureCloudStorageLimit' collate database_default)) 
	BEGIN
		ALTER TABLE [stageDatabaseStatistics] 
			ADD
			[AzureCloudStorageLimit] [decimal] NULL
	END
	IF (not exists(select id from syscolumns where id = object_id('stageDatabaseStatistics') and name = 'ElasticPool' collate database_default)) 
	BEGIN
		ALTER TABLE [stageDatabaseStatistics] 
			ADD
			[ElasticPool][nvarchar](255) NULL
	END
END
--END SQLdm 10.0 (Vandana Gogna) - Database backup alerts

-- SQLdm 11.0 (Rajat) - Azure Cpu Chart
IF (not exists(select id from syscolumns where id = object_id('stageDatabaseStatistics') and name = 'AvgCpuPercent' collate database_default)) 
	BEGIN
		ALTER TABLE [stageDatabaseStatistics] 
			ADD
			[AvgCpuPercent] [decimal] NULL, -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
			[AvgDataIoPercent] [decimal] NULL, -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
			[AvgLogWritePercent] [decimal] NULL, -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
			[DtuLimit] [int] NULL -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
	END

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IXSourceServerIDUTCCollectionTime' )
Begin
	CREATE NONCLUSTERED INDEX [IXSourceServerIDUTCCollectionTime] ON [dbo].[stageDatabaseStatistics] 
	(
		[SourceID] ASC,
		[ServerID] ASC,
		[UTCCollectionDateTime] ASC
	) ON [PRIMARY]
End

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IXSourceIDIncServerDBNameSystemCreate' )
Begin
	CREATE NONCLUSTERED INDEX [IXSourceIDIncServerDBNameSystemCreate] ON [dbo].[stageDatabaseStatistics] 
	(
		[SourceID] ASC
	)
	INCLUDE ([ServerID],
		[DatabaseName],
		[SystemDatabase],
		[DatabaseCreateDate]) ON [PRIMARY]
END

-- In case of upgrade this table may be re-created in p_PostInstallUpgrade
IF (OBJECT_ID('DatabaseSize') IS NULL)
BEGIN
	CREATE TABLE  [DatabaseSize] 
	(
		[DatabaseSizeID] [bigint] IDENTITY(-9223372036854775808,1) UNIQUE CLUSTERED,
		[DatabaseID] [int] NOT NULL ,
		[UTCCollectionDateTime] [datetime] NOT NULL,
		[DatabaseStatus] [int] NULL,
		[DataFileSizeInKilobytes] [decimal] NULL,
		[LogFileSizeInKilobytes] [decimal] NULL,
		[DataSizeInKilobytes] [decimal] NULL,
		[LogSizeInKilobytes] [decimal] NULL,
		[TextSizeInKilobytes] [decimal] NULL,
		[IndexSizeInKilobytes] [decimal] NULL,
		[LogExpansionInKilobytes] [decimal] NULL,
		[DataExpansionInKilobytes] [decimal] NULL,
		[PercentLogSpace] [float] NULL,
		[PercentDataSize] [float] NULL,
		[TimeDeltaInSeconds] [float],
		[DatabaseStatisticsTime] [datetime] NULL,
		CONSTRAINT [PKDatabaseSize] PRIMARY KEY NONCLUSTERED 
			(
			[DatabaseID],
			[UTCCollectionDateTime]
			), 
		CONSTRAINT [FKDatabaseSizeSQLServerDatabaseNames] FOREIGN KEY 
			(
			[DatabaseID]
			) 
		REFERENCES  [SQLServerDatabaseNames] 
			(
			[DatabaseID]
			)  on delete cascade
		)

	create index [IXDatabaseSizeUTCCollectionDateTime] ON [DatabaseSize] ([UTCCollectionDateTime]) include ([DatabaseID], [DataSizeInKilobytes], [TextSizeInKilobytes], [IndexSizeInKilobytes])

END

----------------------------------------------------------------------

IF (OBJECT_ID('SQLServerTableNames') IS NULL)
BEGIN
CREATE TABLE  [SQLServerTableNames] 
	(
	[TableID] [int] IDENTITY NOT NULL ,
	[DatabaseID] [int] NOT NULL ,
	[TableName] [nvarchar] (255)   NOT NULL,
	[SchemaName] [nvarchar] (255) NOT NULL,
	[SystemTable] [bit] NOT NULL,
	CONSTRAINT [PKSQLServerTableNames] PRIMARY KEY  CLUSTERED 
	(
		[TableID]
	),  
	CONSTRAINT [FKSQLServerTableNamesSQLServerDatabaseNames] FOREIGN KEY 
		(
		[DatabaseID]
		) 
	REFERENCES  [SQLServerDatabaseNames] 
		(
		[DatabaseID]
		)  on delete cascade
	)

CREATE INDEX [IXSQLServerTableNames1] ON [SQLServerTableNames]([DatabaseID],[TableName]) 
	
END

----------------------------------------------------------------------

IF (OBJECT_ID('TableGrowth') IS NULL)
BEGIN
CREATE TABLE  [TableGrowth] 
	(
	[TableID] [int] NOT NULL ,
	[UTCCollectionDateTime] [datetime] NOT NULL ,
	[NumberOfRows] [bigint] NULL,
	[DataSize] [float] NULL,
	[TextSize] [float] NULL,
	[IndexSize] [float] NULL,
	[TimeDeltaInSeconds] [float] DEFAULT (86400),
	CONSTRAINT [PKTableGrowth] PRIMARY KEY  CLUSTERED 
	(
		[TableID],
		[UTCCollectionDateTime]
	),
	CONSTRAINT [FKTableGrowth] FOREIGN KEY 
		(
		[TableID]
		) 
	REFERENCES  [SQLServerTableNames] 
		(
		[TableID]
		)  on delete cascade
	)
END



IF (OBJECT_ID('TableReorganization') IS NULL)
BEGIN
CREATE TABLE  [TableReorganization] 
	(
	[TableID] [int] NOT NULL ,
	[UTCCollectionDateTime] [datetime] NOT NULL ,
	[ScanDensity] [float] NULL, 
	[LogicalFragmentation] [float] NULL, 
	[TimeDeltaInSeconds] [float] DEFAULT (86400),
	CONSTRAINT [PKTableReorganization] PRIMARY KEY  CLUSTERED 
	(
		[TableID],
		[UTCCollectionDateTime]
	),
	CONSTRAINT [FKTableReorganization] FOREIGN KEY 
		(
		[TableID]
		) 
	REFERENCES  [SQLServerTableNames] 
		(
		[TableID]
		)  on delete cascade
	)
END


----------------------------------------------------------------------

IF (OBJECT_ID('LicenseKeys') IS NULL)
BEGIN
CREATE TABLE [LicenseKeys](
	[LicenseID] [uniqueidentifier] NOT NULL,
	[LicenseKey] [nvarchar](255) NOT NULL,
	[DateAddedUtc] [datetime] NOT NULL,
	CONSTRAINT [PKLicenseKeys] PRIMARY KEY CLUSTERED 
	(
		[LicenseID]
	)
)

	CREATE UNIQUE INDEX [IXLicenseKeys] ON [LicenseKeys]([LicenseKey]) 

END

IF (OBJECT_ID('NotificationRules') IS NULL)
BEGIN
CREATE TABLE [NotificationRules](
	[RuleID] [uniqueidentifier] NOT NULL,
	[SerializedObject] [nvarchar] (max) NOT NULL,
	CONSTRAINT [PKNotificationRules] PRIMARY KEY CLUSTERED 
	(
		[RuleID] ASC
	)
)
END
ELSE
BEGIN
	IF (not exists(select sc.object_id from sys.columns sc inner join sys.types st on sc.system_type_id = st.system_type_id
		where object_id=OBJECT_ID('NotificationRules') 
		and sc.name='SerializedObject' and lower(st.name) = 'nvarchar'  collate database_default))
	begin
		alter table NotificationRules alter column SerializedObject nvarchar(max) NOT NULL
	END
END

IF (OBJECT_ID('NotificationProviders') IS NULL)
BEGIN
CREATE TABLE [NotificationProviders](
	[ProviderId] [uniqueidentifier] NOT NULL,
	[SerializedType] [nvarchar] (64) NOT NULL,
	[SerializedObject] [nvarchar] (max) NOT NULL,
	CONSTRAINT [PKNotificationProviders] PRIMARY KEY CLUSTERED 
	(
		[ProviderId] ASC
	)
)
END
ELSE
BEGIN
IF (not exists(select sc.object_id from sys.columns sc inner join sys.types st on sc.system_type_id = st.system_type_id
		where object_id=OBJECT_ID('NotificationProviders') 
		and sc.name='SerializedObject' and lower(st.name) = 'nvarchar'  collate database_default))
	begin
		alter table NotificationProviders alter column SerializedObject nvarchar(max) NOT NULL
	END
END
----------------------------------------------------------------------
IF (OBJECT_ID('AlertTemplateLookup') IS NULL)
BEGIN
CREATE TABLE [dbo].[AlertTemplateLookup](
	[TemplateID] [int] NOT NULL IDENTITY(0,1),
	[Name] [nvarchar] (256) NOT NULL DEFAULT ('Default'),
	[Description] [nvarchar] (1024) NULL,
	[Default] [bit] NOT NULL DEFAULT (1)
	CONSTRAINT [PKAlertTemplateLookup] PRIMARY KEY CLUSTERED
	(
		[TemplateID]
	)
)
END


----------------------------------------------------------------------

IF (OBJECT_ID('DBMetrics') IS NULL)
BEGIN
	CREATE TABLE DBMetrics([MetricID] int)
END


----------------------------------------------------------------------

IF (OBJECT_ID('DefaultMetricThresholds') IS NULL)
BEGIN
CREATE TABLE [DefaultMetricThresholds](
	[UserViewID] [int] NOT NULL,
	[Metric] [int] NOT NULL,
	[Enabled] [bit] NOT NULL,
	[WarningThreshold] [nvarchar](2048) NULL,
	[CriticalThreshold] [nvarchar](2048) NULL,
	[Data] [nvarchar] (max) NULL,
	[InfoThreshold] [nvarchar](2048) NULL,
	[ThresholdInstanceID] [int] CONSTRAINT [DFDefaultMetricThresholdsThresholdInstanceID] DEFAULT -1 NOT NULL,
	[ThresholdEnabled] [bit] CONSTRAINT [DFDefaultMetricThresholdsThresholdEnabled] DEFAULT ((1)) NOT NULL,
	[IsBaselineEnabled] [bit] DEFAULT ((0)) NOT NULL,
	[BaselineWarningThreshold] [nvarchar](2048) NULL,
	[BaselineCriticalThreshold] [nvarchar](2048) NULL,
	[BaselineInfoThreshold] [nvarchar](2048) NULL,
	CONSTRAINT [PKDefaultMetricThresholds] PRIMARY KEY CLUSTERED 
	(
		[UserViewID] ASC,
		[Metric] ASC,
		[ThresholdInstanceID] ASC
	),
	CONSTRAINT [FKDefaultMetricThresholdsAlertTemplateLookup] FOREIGN KEY([UserViewID])
	REFERENCES [AlertTemplateLookup] ([TemplateID])
	ON DELETE CASCADE
)
END
ELSE
begin
	-- upgrade to 7.0
	IF (not exists(select id from syscolumns where id = object_id('DefaultMetricThresholds') and name = 'InfoThreshold' collate database_default)) 
	BEGIN
		ALTER TABLE [DefaultMetricThresholds] 
			ADD
			[InfoThreshold] [nvarchar](2048) NULL
			
	end
	IF (exists(select [UserViewID] from [DefaultMetricThresholds] Where [UserViewID] = 0))
	BEGIN
		IF (not exists(select [TemplateID] from [AlertTemplateLookup] where [TemplateID] = 0))
		BEGIN
			INSERT INTO AlertTemplateLookup (
				[Name],
				[Description],
				[Default]) 
			VALUES (
				'Default Template', 
				'SQLdm Default Template created by Management Services', 
				1)
		END
	END
	IF (not exists(select [OBJECT_ID] from sys.foreign_keys where name = 'FKDefaultMetricThresholdsAlertTemplateLookup' and parent_object_id = object_id(N'[dbo].DefaultMetricThresholds')))	
	BEGIN
		ALTER TABLE [DefaultMetricThresholds] WITH CHECK 
		ADD CONSTRAINT [FKDefaultMetricThresholdsAlertTemplateLookup] FOREIGN KEY([UserViewID])
		REFERENCES [AlertTemplateLookup] ([TemplateID])
		ON DELETE CASCADE	
	END
	--Upgrade to 7.2 if Data is not nvarchar(max)
	IF (not exists(select sc.object_id from sys.columns sc inner join sys.types st on sc.system_type_id = st.system_type_id
		where object_id=OBJECT_ID('DefaultMetricThresholds') 
		and sc.name='Data' and lower(st.name) = 'nvarchar'  collate database_default))
	begin
		alter table [DefaultMetricThresholds] alter column [Data] nvarchar(max) NULL
	END
	
	--Upgrade to 7.5 Add ThresholdInstanceID
	IF (not exists(select id from syscolumns where id = OBJECT_ID('DefaultMetricThresholds') and name = 'ThresholdInstanceID' collate database_default))
	BEGIN
		ALTER TABLE [DefaultMetricThresholds] DROP CONSTRAINT [PKDefaultMetricThresholds]
		ALTER TABLE [DefaultMetricThresholds]
			ADD
			[ThresholdInstanceID] [int] 
			CONSTRAINT [DFDefaultMetricThresholdsThresholdInstanceID] DEFAULT -1 NOT NULL,
			[ThresholdEnabled] [bit]
			CONSTRAINT [DFDefaultMetricThresholdsThresholdEnabled] DEFAULT ((1)) NOT NULL
		ALTER TABLE [DefaultMetricThresholds] ADD CONSTRAINT [PKDefaultMetricThresholds] PRIMARY KEY CLUSTERED 
		(
			[UserViewID] ASC,
			[Metric] ASC,
			[ThresholdInstanceID] ASC
		)
	END
	-- upgrade to 10.0
	-- START --Srishti Purohit -- To accomadate baseline alert feature
	IF (not exists(select id from syscolumns where id = object_id('DefaultMetricThresholds') and name = 'IsBaselineEnabled' collate database_default))
	BEGIN
		ALTER TABLE [DefaultMetricThresholds]
			ADD
			[IsBaselineEnabled] [bit] DEFAULT ((0)) NOT NULL,
			[BaselineWarningThreshold] [nvarchar](2048) NULL,
			[BaselineCriticalThreshold] [nvarchar](2048) NULL,
			[BaselineInfoThreshold] [nvarchar](2048) NULL
	END
	-- END --Srishti Purohit -- To accomadate baseline alert feature
	
	-- Fix SQLDM-28957 
	-- START --Luis Barrientos -- To support serialization of DataBase Status Threshold serialization
	ALTER TABLE [DefaultMetricThresholds] ALTER COLUMN [WarningThreshold] nvarchar(2048)
	ALTER TABLE [DefaultMetricThresholds] ALTER COLUMN [CriticalThreshold] nvarchar(2048)
	ALTER TABLE [DefaultMetricThresholds] ALTER COLUMN [InfoThreshold] nvarchar(2048)
	IF (exists(select id from syscolumns where id = object_id('DefaultMetricThresholds')))
	BEGIN
		ALTER TABLE [DefaultMetricThresholds] ALTER COLUMN [BaselineWarningThreshold] nvarchar(2048)
		ALTER TABLE [DefaultMetricThresholds] ALTER COLUMN [BaselineCriticalThreshold] nvarchar(2048)
		ALTER TABLE [DefaultMetricThresholds] ALTER COLUMN [BaselineInfoThreshold] nvarchar(2048)	
	END	
	-- END --Luis Barrientos -- To support serialization of DataBase Status Threshold serialization
end


----------------------------------------------------------------------

IF (OBJECT_ID('MetricThresholds') IS NULL)
BEGIN
CREATE TABLE [MetricThresholds](
	[SQLServerID] [int] NOT NULL,
	[Metric] [int] NOT NULL,
	[Enabled] [bit] NOT NULL,
	[WarningThreshold] [nvarchar](2048) NULL,
	[CriticalThreshold] [nvarchar](2048) NULL,
	[Data] [nvarchar] (max) NULL,
	[UTCSnoozeStart] [datetime] NULL,
	[UTCSnoozeEnd] [datetime] NULL,
	[SnoozeStartUser] [nvarchar](255) NULL,
	[SnoozeEndUser] [nvarchar](255) NULL,
	[InfoThreshold] [nvarchar](2048) NULL,
	[ThresholdInstanceID] [int] CONSTRAINT [DFMetricThresholdsThresholdInstanceID] DEFAULT -1 NOT NULL,
	[ThresholdEnabled] [bit] CONSTRAINT [DFMetricThresholdsThresholdEnabled] DEFAULT ((1)) NOT NULL,
	[IsBaselineEnabled] [bit] DEFAULT ((0)) NOT NULL,
	[BaselineWarningThreshold] [nvarchar](2048) NULL,
	[BaselineCriticalThreshold] [nvarchar](2048) NULL,
	[BaselineInfoThreshold] [nvarchar](2048) NULL,
	CONSTRAINT [PKMetricThresholds] PRIMARY KEY CLUSTERED 
	(
		[SQLServerID] ASC,
		[Metric] ASC,
		[ThresholdInstanceID] ASC
	),
	CONSTRAINT [FKMetricThresholdsMonitoredSQLServers] FOREIGN KEY
	(
		[SQLServerID]
	)
	REFERENCES [MonitoredSQLServers] 
	(
		[SQLServerID]
	)  on delete cascade
)
-- upgrade to DM10.3 CWF Integration
	IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_MetricThresholds_SQLServerID_Metric_UTCSnoozeEnd' )
	BEGIN
	CREATE NONCLUSTERED INDEX [IX_MetricThresholds_SQLServerID_Metric_UTCSnoozeEnd] ON [MetricThresholds]
	(
	 [SQLServerID] ASC,
	 [Metric] ASC,
	 [UTCSnoozeEnd] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	END
END
ELSE
BEGIN
	-- upgrade to 5.6.0
	IF (not exists(select id from syscolumns where id = object_id('MetricThresholds') and name = 'UTCSnoozeStart' collate database_default)) 
	BEGIN
		ALTER TABLE [MetricThresholds] 
			ADD 
			[UTCSnoozeStart] [datetime] NULL,
			[UTCSnoozeEnd] [datetime] NULL,
			[SnoozeStartUser] [nvarchar](255) NULL,
			[SnoozeEndUser] [nvarchar](255) NULL
	END
	-- upgrade to 7.0
	IF (not exists(select id from syscolumns where id = object_id('MetricThresholds') and name = 'InfoThreshold' collate database_default))
	BEGIN
		ALTER TABLE [MetricThresholds]
			ADD
			[InfoThreshold] [nvarchar](2048) NULL
	END
	
	--Upgrade to 7.2 if Data is not nvarchar(max)
	IF (not exists(select sc.object_id from sys.columns sc inner join sys.types st on sc.system_type_id = st.system_type_id
		where object_id=OBJECT_ID('MetricThresholds') 
		and sc.name='Data' and lower(st.name) = 'nvarchar'  collate database_default))
	begin
		alter table MetricThresholds alter column [Data] nvarchar(max) NULL
	END
	
	--Upgrade to 7.5 if ThresholdInstanceID does not exist
	IF (not exists(select id from syscolumns where id = OBJECT_ID('MetricThresholds') and name = 'ThresholdInstanceID' collate database_default))
	BEGIN
		ALTER TABLE [MetricThresholds] DROP CONSTRAINT [PKMetricThresholds]
		ALTER TABLE [MetricThresholds]
			ADD
			[ThresholdInstanceID] [int] 
			CONSTRAINT [DFMetricThresholdsThresholdInstanceID] DEFAULT -1 NOT NULL,
			[ThresholdEnabled] [bit] 
			CONSTRAINT [DFMetricThresholdsThresholdEnabled] DEFAULT ((1)) NOT NULL
		ALTER TABLE [MetricThresholds] ADD CONSTRAINT [PKMetricThresholds] PRIMARY KEY CLUSTERED 
		(
			[SQLServerID] ASC,
			[Metric] ASC,
			[ThresholdInstanceID] ASC
		)
	END
	-- upgrade to 10.0
	-- START --Srishti Purohit -- To accomadate baseline alert feature
	IF (not exists(select id from syscolumns where id = object_id('MetricThresholds') and name = 'IsBaselineEnabled' collate database_default))
	BEGIN
		ALTER TABLE [MetricThresholds]
			ADD
			[IsBaselineEnabled] [bit] DEFAULT ((0)) NOT NULL,
			[BaselineWarningThreshold] [nvarchar](2048) NULL,
			[BaselineCriticalThreshold] [nvarchar](2048) NULL,
			[BaselineInfoThreshold] [nvarchar](2048) NULL
	END
	-- END --Srishti Purohit -- To accomadate baseline alert feature
	
	-- Fix SQLDM-28957 
	-- START --Luis Barrientos -- To support serialization of DataBase Status Threshold serialization
	ALTER TABLE [MetricThresholds] ALTER COLUMN [WarningThreshold] nvarchar(2048)
	ALTER TABLE [MetricThresholds] ALTER COLUMN [CriticalThreshold] nvarchar(2048)
	ALTER TABLE [MetricThresholds] ALTER COLUMN [InfoThreshold] nvarchar(2048)
	IF (exists(select id from syscolumns where id = object_id('MetricThresholds')))
	BEGIN
		ALTER TABLE [MetricThresholds] ALTER COLUMN [BaselineWarningThreshold] nvarchar(2048)
		ALTER TABLE [MetricThresholds] ALTER COLUMN [BaselineCriticalThreshold] nvarchar(2048)
		ALTER TABLE [MetricThresholds] ALTER COLUMN [BaselineInfoThreshold] nvarchar(2048)	
	END	
	-- END --Luis Barrientos -- To support serialization of DataBase Status Threshold serialization	

	-- upgrade to DM10.3 CWF Integration
	BEGIN
		IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_MetricThresholds_SQLServerID_Metric_UTCSnoozeEnd' )
		BEGIN
		CREATE NONCLUSTERED INDEX [IX_MetricThresholds_SQLServerID_Metric_UTCSnoozeEnd] ON [MetricThresholds]
		(
		 [SQLServerID] ASC,
		 [Metric] ASC,
		 [UTCSnoozeEnd] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		END
	END
END


---------------------------------------------------------------------

IF (OBJECT_ID('Alerts') IS NULL)
BEGIN
CREATE TABLE [Alerts](
		[AlertID] bigint IDENTITY NOT NULL,
		[UTCOccurrenceDateTime] datetime NOT NULL,
		[ServerName] nvarchar(256) NULL,
		[DatabaseName] nvarchar(255) NULL,
		[TableName] nvarchar(255) NULL,
		[Active] bit NULL,
		[Metric] int NULL,
		[Severity] tinyint NULL,
		[StateEvent] tinyint NULL,
		[Value] float NULL,
		[Heading] nvarchar(256) NULL,
		[Message] nvarchar(2048) NULL, -- DM Kit 1 Defect Id - DE44578 - (Biresh Kumar Mishra) - Increased Message column size from 1024 to 2048
		[QualifierHash] nvarchar(28) NULL,
		[LinkedData] uniqueidentifier NULL,
		CONSTRAINT [PK_Alerts] PRIMARY KEY CLUSTERED
			(AlertID));

	ALTER TABLE dbo.Alerts WITH NOCHECK
		ADD CONSTRAINT [FK_AlertsMonitoredSQLServers] FOREIGN KEY(ServerName)
		REFERENCES dbo.MonitoredSQLServers (InstanceName)
		ON DELETE CASCADE;

	ALTER TABLE [Alerts] CHECK CONSTRAINT [FK_AlertsMonitoredSQLServers];

	-- Add nonclustered indexes
	CREATE NONCLUSTERED INDEX [IX_Alerts_Metric_UTCOccurrenceDateTime] ON [Alerts] 
	(
		[Metric] ASC,
		[UTCOccurrenceDateTime] ASC,
		[ServerName] ASC
	)
	INCLUDE ([DatabaseName],
		[Severity],
		[Message],
		[Heading],
		[QualifierHash],
		[LinkedData]);
		
	CREATE NONCLUSTERED INDEX [IX_Alerts_PredictiveTrainingIndex] ON [Alerts] 
	(
		[ServerName] ASC,
		[Metric] ASC,
		[Severity] ASC,
		[UTCOccurrenceDateTime] ASC
	)
	INCLUDE (StateEvent);

	CREATE NONCLUSTERED INDEX [IX_Alerts_ViewSupport] ON [Alerts] 
	(
		[ServerName] ASC,
		[Metric] ASC,
		[UTCOccurrenceDateTime] ASC,
		[AlertID] ASC
	)
	INCLUDE ([DatabaseName],
		[TableName],
		[Severity],
		[Active],
		[StateEvent],
		[Value],
		[Heading],
		[Message]
	);

	-- IXAlerts2 added to keep grooming from doing a table scan
	CREATE NONCLUSTERED INDEX [IXAlerts2] ON [Alerts] 
	(
		[ServerName] ASC,
		[UTCOccurrenceDateTime] DESC,
		[Active] ASC
	)

	CREATE NONCLUSTERED INDEX [IXAlerts4] ON [Alerts]
	(
		[UTCOccurrenceDateTime] DESC,
		[ServerName] ASC,
		[Severity] ASC
	)

	CREATE NONCLUSTERED INDEX [IXAlerts5] on [Alerts]
	(
		[ServerName] ASC,
		[UTCOccurrenceDateTime] ASC
	) INCLUDE ([AlertID]) ON [PRIMARY]

END
ELSE
BEGIN
	-- upgrade to 5.5.0
	IF (not exists(select id from syscolumns where id = object_id('Alerts') and name = 'LinkedData' collate database_default)) 
	BEGIN
		ALTER TABLE [Alerts] 
			ADD [LinkedData] [uniqueidentifier] NULL
	END

	-- DM Kit 1 Defect Id - DE44578 - (Biresh Kumar Mishra) - Increased Message column size from 1024 to 2048
	IF (not exists(select id from syscolumns where id = object_id('Alerts') and name = 'Message' collate database_default and length >= 4096))
	BEGIN
		ALTER TABLE [Alerts] ALTER COLUMN [Message] nvarchar(2048) NULL
	END	
	
END

----------------------------------------------------------------------

IF (OBJECT_ID('Tasks') IS NULL)
BEGIN
CREATE TABLE [Tasks](
	[TaskID] [int] IDENTITY NOT NULL,
	[ServerName] [nvarchar] (256) NULL,
	[Subject] [nvarchar](256) NOT NULL,
	[Message] [nvarchar](1024) NULL,
	[Comments] [nvarchar](1024) NULL,
	[Owner] [nvarchar](256) NULL,
	[CreatedOn] [datetime] NOT NULL,
	[CompletedOn] [datetime] NULL,
	[Status] [tinyint] NOT NULL,
	[Metric] [int] NULL,
	[Severity] [tinyint] NULL,
	[Value] [float] NULL,
	[EventID] [int] NULL,
	[DatabaseName] [nvarchar] (255) NULL,
	CONSTRAINT [PKTasks] PRIMARY KEY CLUSTERED 
	(
		[TaskID]
	)
)

CREATE NONCLUSTERED INDEX [IXTasks] ON [Tasks] 
(
	[ServerName] ASC,
	[CreatedOn] ASC,
	[Status] ASC,
	[Severity] ASC
)
END
ELSE
BEGIN
	-- upgrade to 5.5.0
	IF (not exists(select id from syscolumns where id = object_id('Tasks') and name = 'DatabaseName' collate database_default)) 
	BEGIN
		ALTER TABLE [Tasks] 
			ADD [DatabaseName] [nvarchar] (255) NULL
	END
END

----------------------------------------------------------------------

IF (OBJECT_ID('MetricInfo') IS NULL)
BEGIN
CREATE TABLE [MetricInfo](
	[Metric] [int] NOT NULL,
	[UTCLastChangeDateTime] [datetime] NOT NULL CONSTRAINT [DF_MetricInfo_UTCLastChangeDateTime]  DEFAULT ((GETUTCDATE())),
	[Rank] [int] NOT NULL,
	[Category] [nvarchar](64) NOT NULL,
	[Name] [nvarchar] (128) NOT NULL,
	[Description] [nvarchar](512) NOT NULL,
	[Comments] [nvarchar] (max) NULL,
	[PaaS] BIT NOT NULL DEFAULT 0,
	CONSTRAINT [PKMetricInfo] PRIMARY KEY CLUSTERED 
	(
		[Metric]
	)
	)
END
ELSE
BEGIN
	-- upgrade to 5.5.0
	IF (not exists(select id from syscolumns where id = object_id('MetricInfo') and name = 'UTCLastChangeDateTime' collate database_default)) 
	BEGIN
		ALTER TABLE [MetricInfo] ADD 
			[UTCLastChangeDateTime] [datetime] NOT NULL CONSTRAINT [DF_MetricInfo_UTCLastChangeDateTime]  DEFAULT ((GETUTCDATE()))
	END
	
		--Upgrade to 7.2 if Comments is not nvarchar(max)
	IF (not exists(select sc.object_id from sys.columns sc inner join sys.types st on sc.system_type_id = st.system_type_id
		where object_id=OBJECT_ID('MetricInfo') 
		and sc.name='Comments' and lower(st.name) = 'nvarchar'  collate database_default))
	begin
		alter table [MetricInfo] alter column [Comments] nvarchar(max) NULL
	END
END


IF OBJECT_ID (N'MetricInfo', N'U') IS NOT NULL 
BEGIN
	-- Check 'PasS' column is exists or not
	IF NOT EXISTS ( SELECT COLUMN_NAME 
					FROM INFORMATION_SCHEMA.COLUMNS
					WHERE  TABLE_NAME='MetricInfo' AND COLUMN_NAME='PaaS' 
				   )
			   BEGIN
					ALTER TABLE [MetricInfo]
					ADD [PaaS] BIT NOT NULL DEFAULT 0
			   END	
END

----------------------------------------------------------------------

IF (OBJECT_ID('ServerActivity') IS NULL)
BEGIN
CREATE TABLE [ServerActivity](
	[SQLServerID] [int] NOT NULL,
	[UTCCollectionDateTime] [datetime] NOT NULL,
	[StateOverview] [nvarchar] (max) NULL,
	[SystemProcesses] [image] NULL,
	[SessionList] [image] NULL,
	[LockStatistics] [image] NULL,
	[LockList] [image] NULL,
	[RefreshType] int null,
	CONSTRAINT [PKServerActivity] PRIMARY KEY CLUSTERED 
	(
		[SQLServerID],
		[UTCCollectionDateTime]
	),
	CONSTRAINT [FKServerActivityMonitoredSQLServers] FOREIGN KEY 
		(
		[SQLServerID]
		) 
	REFERENCES [MonitoredSQLServers] 
		(
		[SQLServerID]
		) on delete cascade
	)
END
ELSE
BEGIN
	--Upgrade to 7.2 if StateOverview is not nvarchar(max)
	IF (not exists(select sc.object_id from sys.columns sc inner join sys.types st on sc.system_type_id = st.system_type_id
		where object_id=OBJECT_ID('ServerActivity') 
		and sc.name='StateOverview' and lower(st.name) = 'nvarchar'  collate database_default))
	begin
		alter table [ServerActivity] alter column [StateOverview] nvarchar(max) NULL
	END
	
	if (not exists(select object_id from sys.columns where object_id = object_id('ServerActivity') and name = 'RefreshType' collate database_default))
	begin
		alter table [ServerActivity] add [RefreshType] int null
	end
END
----------------------------------------------------------------------
IF (OBJECT_ID('GroomingLog') IS NULL)
BEGIN
CREATE TABLE [GroomingLog](
	[RunID] [uniqueidentifier] NOT NULL,
	[Sequence] int NOT NULL,
	[UTCActionEndDateTime] [datetime] NOT NULL,
	[Action] [nvarchar](256) NOT NULL,
	[AffectedRecords] [int] NULL,
	[InstanceName] [nvarchar](256) NULL
	)
END


if exists(select name from sys.objects where name = 'PKGroomingLog') 
begin
	alter table [GroomingLog] drop constraint [PKGroomingLog]
end	

if not exists(select * from sys.indexes where name = 'GroomingLogCL')
begin
	create clustered index [GroomingLogCL] on [GroomingLog]([UTCActionEndDateTime])
end

----------------------------------------------------------------------
IF (OBJECT_ID('MetricMetaData') IS NULL)
BEGIN
CREATE TABLE [dbo].[MetricMetaData](
	[Metric] [int] NOT NULL,
	[UTCLastChangeDateTime] [datetime] NOT NULL CONSTRAINT [DF_MetricMetaData_UTCLastChangeDateTime]  DEFAULT ((GETUTCDATE())),
	[Deleted] [bit] NOT NULL CONSTRAINT [DF_MetricMetaData_Deleted]  DEFAULT ((0)),
	[Class] [int] NOT NULL,
	[Flags] [int] NOT NULL,
	[MinValue] [int] NOT NULL,
	[MaxValue] [bigint] NOT NULL,
	[DefaultWarningValue] [bigint] NOT NULL,
	[DefaultCriticalValue] [bigint] NOT NULL,
	[DoNotifications] [bit] NOT NULL,
	[EventCategory] [int] NULL,
	[DefaultMessageID] [int] NULL,
	[AlertEnabledDefault] [bit] NOT NULL,
	[ValueComparison] [int] NOT NULL,
	[ValueType] [nvarchar](128) NOT NULL,
	[Rank] int NULL, 
	[DefaultInfoValue] [bigint] NOT NULL,
	TableName NVARCHAR(256) NULL,
	ColumnName NVARCHAR(256) NULL,
	[IsValidForSqlExpress] [BIT] NOT NULL CONSTRAINT [DF_MetricMetaData_IsValidForSqlExpress] DEFAULT(0),
	[BaselineMaxValue] [bigint] NOT NULL DEFAULT (300),
	[BaselineDefaultWarningValue] [bigint] NOT NULL DEFAULT (120),
	[BaselineDefaultCriticalValue] [bigint] NOT NULL DEFAULT (100),
	[BaselineDefaultInfoValue] [bigint] NOT NULL DEFAULT (50),
	CONSTRAINT [PKMetricMetaData] PRIMARY KEY CLUSTERED
	(
		[Metric]
	)
)
END
ELSE
BEGIN
	-- upgrade to 7.0
	if (not exists(select id from syscolumns where id = object_id('MetricMetaData') and name = 'DefaultInfoValue' collate database_default))
	BEGIN
		ALTER TABLE [MetricMetaData] ADD
			[DefaultInfoValue] [bigint] NULL
	END

	--upgrade to 8.5: Ashu - For metric history display in Web console
	IF (not exists(select id from syscolumns where id=OBJECT_ID('MetricMetaData') and name='TableName' collate database_default))	
	BEGIN
		ALTER TABLE [MetricMetaData] ADD TableName NVARCHAR(256) NULL;
	END		

	IF (not exists(select id from syscolumns where id=OBJECT_ID('MetricMetaData') and name='ColumnName' collate database_default))	
	BEGIN
		ALTER TABLE [MetricMetaData] ADD ColumnName NVARCHAR(256) NULL;
	END		
	--upgrade to 8.5: Ashu - For metric history display in Web console - END	

	-- upgrade to 8.6 :Ankit Srivastava - For supressing alerts in SQL Express 
	IF (not exists(select id from syscolumns where id = object_id('MetricMetaData') and name = 'IsValidForSqlExpress' collate database_default)) 
	BEGIN
		ALTER TABLE [MetricMetaData] ADD 
			[IsValidForSqlExpress] [BIT] NOT NULL CONSTRAINT [DF_MetricMetaData_IsValidForSqlExpress] DEFAULT(0) 
	END
	-- upgrade to 10.0 :Srishti - For baseline configuration alert
	IF (not exists(select id from syscolumns where id = object_id('MetricMetaData') and name = 'BaselineMaxValue' collate database_default)) 
	BEGIN
		ALTER TABLE [MetricMetaData] ADD 
			[BaselineMaxValue] [bigint] DEFAULT(99999999) NOT NULL,
			[BaselineDefaultWarningValue] [bigint] DEFAULT(100) NOT NULL,
			[BaselineDefaultCriticalValue] [bigint] DEFAULT(120) NOT NULL,
			[BaselineDefaultInfoValue] [bigint] DEFAULT(50) NOT NULL
	END
END

----------------------------------------------------------------------
IF (OBJECT_ID('MetricMetaDataMessages') IS NULL)
BEGIN
CREATE TABLE [dbo].[MetricMetaDataMessages](
	[Metric] [int] NOT NULL,
	[MessageID] [int] NOT NULL,
	[EventID] [bigint] NULL,
	[HeaderTemplate] [nvarchar](256) NOT NULL,
	[BodyTemplate] [nvarchar](512) NOT NULL,
	[TodoTemplate] [nvarchar](512) NOT NULL,
	[PulseTemplate] [nvarchar](256) NOT NULL,
	--START: SQLdm 10.0 (Tarun Sapra): DE45820-ALert 'Details'  should be changed for all the baseline alerts
	[HeaderTemplate_Baseline] [nvarchar](256) NULL,
	[BodyTemplate_Baseline] [nvarchar](512) NULL,
	[TodoTemplate_Baseline] [nvarchar](512) NULL,
	[PulseTemplate_Baseline] [nvarchar](256) NULL
	--END: SQLdm 10.0 (Tarun Sapra): DE45820-ALert 'Details'  should be changed for all the baseline alerts
	CONSTRAINT [PKMetricMetaDataMessages] PRIMARY KEY CLUSTERED
	(
		[Metric],
		[MessageID]
	)
) 
END
ELSE
BEGIN
	-- upgrade to 7.0
	if (not exists(select id from syscolumns where id = object_id('MetricMetaDataMessages') and name = 'PulseTemplate' collate database_default))
	BEGIN
		ALTER TABLE [MetricMetaDataMessages] ADD
			[PulseTemplate] [nvarchar](256) NULL
	END

	--upgrade to 10.0
	if (not exists(select id from syscolumns where id = object_id('MetricMetaDataMessages') and name = 'HeaderTemplate_Baseline' collate database_default))
	BEGIN
		ALTER TABLE [MetricMetaDataMessages] ADD
			[HeaderTemplate_Baseline] [nvarchar](256) NULL
	END
	if (not exists(select id from syscolumns where id = object_id('MetricMetaDataMessages') and name = 'BodyTemplate_Baseline' collate database_default))
	BEGIN
		ALTER TABLE [MetricMetaDataMessages] ADD
			[BodyTemplate_Baseline] [nvarchar](256) NULL
	END
	if (not exists(select id from syscolumns where id = object_id('MetricMetaDataMessages') and name = 'TodoTemplate_Baseline' collate database_default))
	BEGIN
		ALTER TABLE [MetricMetaDataMessages] ADD
			[TodoTemplate_Baseline] [nvarchar](256) NULL
	END
	if (not exists(select id from syscolumns where id = object_id('MetricMetaDataMessages') and name = 'PulseTemplate_Baseline' collate database_default))
	BEGIN
		ALTER TABLE [MetricMetaDataMessages] ADD
			[PulseTemplate_Baseline] [nvarchar](256) NULL
	END

END


----------------------------------------------------------------------
IF (OBJECT_ID('MetricMetaDataMessageMap') IS NULL)
BEGIN
CREATE TABLE [dbo].[MetricMetaDataMessageMap](
	[Metric] [int] NOT NULL,
	[Value] [int] NOT NULL,
	[MessageID] [int] NOT NULL,
	CONSTRAINT [PKMetricMetaDataMessageMap] PRIMARY KEY CLUSTERED
	(
		[Metric],
		[Value]
	)
) 
END

----------------------------------------------------------------------
IF (OBJECT_ID('CustomCounterDefinition') IS NULL)
BEGIN
CREATE TABLE [dbo].[CustomCounterDefinition](
	[Metric] [int] IDENTITY(1000,1) NOT NULL,
	[UTCLastChangeDateTime] [datetime] NOT NULL CONSTRAINT [DF_CustomCounterDefinition_UTCLastChangeDateTime]  DEFAULT ((GETUTCDATE())),
	[MetricType] [int] NOT NULL,
	[CalculationType] [int] NOT NULL,
	[Enabled] [bit] NOT NULL,
	[Scale] [float] NOT NULL,
	[Object] [nvarchar](256) NULL,
	[Counter] [nvarchar](256) NULL,
	[Instance] [nvarchar](256) NULL,
	[Batch] [nvarchar] (max) NULL,
	[ServerType] [nvarchar](256) NULL DEFAULT ('Unknown'),
	[AzureProfileId] BIGINT NULL,
	CONSTRAINT [PKCustomCounterDefinition] PRIMARY KEY CLUSTERED 
	(
		[Metric] 
	)
) 
END
ELSE
BEGIN
	--Upgrade to 7.2 if Batch is not nvarchar(max)
	IF (not exists(select sc.object_id from sys.columns sc inner join sys.types st on sc.system_type_id = st.system_type_id
		where object_id=OBJECT_ID('CustomCounterDefinition') 
		and sc.name='Batch' and lower(st.name) = 'nvarchar'  collate database_default))
	begin
		alter table [CustomCounterDefinition] alter column [Batch] nvarchar(max) NULL
	END
	
	 --Upgrade to 8.5 (HyperV) if column ServerType does not exist
	IF (not exists(select sc.object_id from sys.columns sc inner join sys.types st on sc.system_type_id = st.system_type_id
		where object_id=OBJECT_ID('CustomCounterDefinition') 
		and sc.name='ServerType' and lower(st.name) = 'nvarchar'  collate database_default))
	BEGIN
		ALTER TABLE [dbo].CustomCounterDefinition ADD ServerType [nvarchar](256) NULL DEFAULT('Unknown')
	END
	ELSE
	BEGIN
		-- Upgrade to 8.6 from 8.5 if column ServerType exist
		DECLARE @Default sysname
		SET @Default = (SELECT OBJECT_NAME(cdefault) FROM syscolumns WHERE id = OBJECT_ID('CustomCounterDefinition') AND name = 'ServerType')
		
		IF(LEN(@Default) > 0)
		BEGIN
			EXEC ('ALTER TABLE [dbo].CustomCounterDefinition DROP CONSTRAINT ' + @Default)
		END
		
		ALTER TABLE [dbo].[CustomCounterDefinition] ALTER COLUMN [ServerType] [nvarchar](256) NULL
		ALTER TABLE [dbo].[CustomCounterDefinition] ADD  DEFAULT ('Unknown') FOR [ServerType]
	END

	-- SQLdm 11 - 5.5 Azure Custom Counter changes - AzureProfileId
	IF (NOT EXISTS(SELECT sc.[object_id] FROM sys.columns sc INNER JOIN sys.types st ON sc.[system_type_id] = st.[system_type_id]
		WHERE sc.[object_id]=OBJECT_ID('CustomCounterDefinition') 
		AND sc.[name]='AzureProfileId' AND LOWER(st.[name]) = 'bigint'  COLLATE database_default))
	BEGIN
		ALTER TABLE CustomCounterDefinition ADD [AzureProfileId] BIGINT NULL
	END
END
----------------------------------------------------------------------
IF (OBJECT_ID('CustomCounterMap') IS NULL)
BEGIN
CREATE TABLE [dbo].[CustomCounterMap](
	[SQLServerID] [int] NOT NULL,
	[Metric] [int] NOT NULL,
	CONSTRAINT [PKCustomCounterMap] PRIMARY KEY CLUSTERED 
	(
		[SQLServerID],
		[Metric]
	),
	CONSTRAINT [FKCustomCounterMapMetricMetaData] 
		FOREIGN KEY([Metric])
		REFERENCES [MetricMetaData] ([Metric]) 
			on delete cascade,
	CONSTRAINT [FKCustomCounterMapMonitoredSQLServers] 
		FOREIGN KEY([SQLServerID])
		REFERENCES [MonitoredSQLServers] ([SQLServerID])
			on delete cascade
) 
END

----------------------------------------------------------------------
IF (OBJECT_ID('CustomCounterStatistics') IS NULL)
BEGIN
CREATE TABLE [dbo].[CustomCounterStatistics](
	[SQLServerID] [int] NOT NULL ,
	[UTCCollectionDateTime] [datetime] NOT NULL ,
	[MetricID] [int] NOT NULL,	
	[TimeDeltaInSeconds] [float] NULL ,
	[RawValue] [decimal](38,9) NULL,
	[DeltaValue] [decimal](38,9) NULL,
	[ErrorMessage] [nvarchar](255) NULL,
	[RunTimeInMilliseconds] [float] NULL,
	CONSTRAINT [PKCustomCounterStatistics] PRIMARY KEY CLUSTERED
	(
		[SQLServerID],
		[UTCCollectionDateTime],
		[MetricID]
	),
	CONSTRAINT [FKCustomCounterStatisticsMonitoredSQLServers] FOREIGN KEY 
		(
		[SQLServerID]
		) 
	REFERENCES  [MonitoredSQLServers] 
		(
		[SQLServerID]
		) on delete cascade
) 
END

----------------------------------------------------------------------
IF (OBJECT_ID('Permission') IS NULL)
BEGIN
CREATE TABLE [dbo].[Permission] (
	[PermissionID] [int] IDENTITY NOT NULL, 
	[LoginSID] [varbinary](85) NOT NULL,
	[Permission] [int] NOT NULL,
	[Enabled] [bit] NOT NULL,
	[Comment] [nvarchar](1024) NULL,
	WebAppPermission BIT  NOT NULL,
	CONSTRAINT [PKPermission] PRIMARY KEY CLUSTERED 
		(
		[PermissionID]
		)
	
)
IF((SELECT COUNT(0)FROM sys.default_constraints WHERE [object_id] = OBJECT_ID('Permission') AND LOWER(name) collate database_default ='df_webapppermission' collate database_default) = 0)
	BEGIN
		ALTER TABLE Permission ADD CONSTRAINT DF_WebAppPermission DEFAULT 1 FOR WebAppPermission
	END
END
ELSE
BEGIN
	IF((SELECT COUNT(0) FROM sys.columns WHERE [object_id] = OBJECT_ID('Permission') AND LOWER(name) collate database_default = 'webapppermission' collate database_default) =0)
	BEGIN
		ALTER TABLE [dbo].[Permission] ADD WebAppPermission BIT  NULL
	END
	IF((SELECT COUNT(0)FROM sys.default_constraints WHERE  LOWER(name) collate database_default ='df_webapppermission' collate database_default) = 0)
	BEGIN
		ALTER TABLE Permission ADD CONSTRAINT DF_WebAppPermission DEFAULT 1 FOR WebAppPermission
	END
END

IF (OBJECT_ID('PermissionServers') IS NULL)
BEGIN
CREATE TABLE  [dbo].[PermissionServers] (
	[PermissionID] [int] NOT NULL,
	[SQLServerID] [int] NOT NULL,
	CONSTRAINT [PKPermissionServers] PRIMARY KEY CLUSTERED 
		(
		[PermissionID],
		[SQLServerID]
		),
	CONSTRAINT [FKSPermissionServersMonitoredSQLServers] FOREIGN KEY 
		(
		[SQLServerID]
		) 
	REFERENCES  [MonitoredSQLServers] 
		(
		[SQLServerID]
		) on delete cascade,
	CONSTRAINT [FKSPermissionServersPermission] FOREIGN KEY 
		(
		[PermissionID]
		) 
	REFERENCES  [Permission] 
		(
		[PermissionID]
		) on delete cascade
)
END

----------------------------------------------------------------------
IF (OBJECT_ID('DiskDrives') IS NULL)
BEGIN
CREATE TABLE [dbo].[DiskDrives] (
	[SQLServerID] int NOT NULL,
	[UTCCollectionDateTime] datetime NOT NULL ,
	[DriveName] nvarchar(256) NOT NULL,
	[UnusedSizeKB] dec(18,0),
	[TotalSizeKB] dec(18,0),
	[DiskIdlePercent] bigint,
	[AverageDiskQueueLength] bigint,
	[AverageDiskMillisecondsPerRead] bigint,
	[AverageDiskMillisecondsPerTransfer] bigint,
	[AverageDiskMillisecondsPerWrite] bigint,
	[DiskReadsPerSecond] bigint,
	[DiskTransfersPerSecond] bigint,
	[DiskWritesPerSecond] bigint,
	[DatabaseSizeTime] datetime null,
	CONSTRAINT [PKDiskDrives] PRIMARY KEY CLUSTERED
	(
		[SQLServerID],
		[UTCCollectionDateTime],
		[DriveName]
	)
)

		create index IXDiskDrivesDatabaseSizeTime on [DiskDrives]([DatabaseSizeTime])
END
ELSE
BEGIN
	-- upgrade to 6.0
	if (not exists(select id from syscolumns where id = object_id('DiskDrives') and name = 'DriveName' collate database_default and length >= 512))
	BEGIN
		ALTER TABLE [DiskDrives]
			ALTER COLUMN [DriveName] [nvarchar](256) NOT NULL
	END

	-- upgrade to 6.1
	IF (not exists(select id from syscolumns where id = object_id('DiskDrives') and name = 'AverageDiskMillisecondsPerRead' collate database_default)) 
	BEGIN
		ALTER TABLE [DiskDrives] ADD 
			[AverageDiskMillisecondsPerRead] bigint,
			[AverageDiskMillisecondsPerTransfer] bigint,
			[AverageDiskMillisecondsPerWrite] bigint
	END

	-- upgrade to 6.2 SP2
	IF (not exists(select id from syscolumns where id = object_id('DiskDrives') and name = 'DiskReadsPerSecond' collate database_default)) 
	BEGIN
		ALTER TABLE [DiskDrives] ADD 
			[DiskReadsPerSecond] bigint,
			[DiskTransfersPerSecond] bigint,
			[DiskWritesPerSecond] bigint
	END
	
	if (not exists(select object_id from sys.columns where object_id = object_id('DiskDrives') and name = 'DatabaseSizeTime' collate database_default)) 
	begin
		ALTER TABLE [DiskDrives] ADD [DatabaseSizeTime] datetime NULL
		create index IXDiskDrivesDatabaseSizeTime on [DiskDrives]([DatabaseSizeTime])
	end
	
	
END

----------------------------------------------------------------------
IF (OBJECT_ID('BaselineMetaData') IS NULL)
BEGIN
CREATE TABLE [dbo].[BaselineMetaData] (
	[ItemID] int NOT NULL,
	[Name] nvarchar(128) NULL,
	[Description] nvarchar(255) NULL,
	[Category] nvarchar(128) NULL,
	[Unit] nvarchar(32) NULL,
	[Format] nvarchar(64) NULL,
	[NullFormat] nvarchar(64) NULL,
	[MetricID] int DEFAULT NULL,
	[StatisticTable] nvarchar(256) NULL,
	[MetricValue] nvarchar(256) NULL,
	[Decimals] int NULL,
	[LLimit] int NULL,
	[ULimit] bigint NULL,
	[Scale] decimal(15,9) NULL,
	CONSTRAINT [PKBaselineMetaData] PRIMARY KEY CLUSTERED
	(
		[ItemID]
	)
)
END
----------------------------------------------------------------------
IF (OBJECT_ID('MirroringParticipants') IS NULL)
BEGIN
CREATE TABLE [MirroringParticipants](
	[DatabaseID] [int] NOT NULL,
	[mirroring_guid] [uniqueidentifier] NOT NULL,
	[role] [tinyint] NULL,
	[principal_address] [nvarchar](128) NULL,
	[Mirror_address] [nvarchar](128) NULL,
	[witness_address] [nvarchar](128) NULL,
	[safety_level] [int] NULL,
	[is_suspended] [bit] NULL,
	[mirroring_state] [tinyint] NULL,
	[witness_status] [tinyint] NULL,
	[mirror_instanceID] [int] NULL,
	[principal_instanceID] [int] NULL,
	[partner_instance] [nvarchar](128) NULL,
	[last_updated] [datetime] NULL,
        CONSTRAINT [PKMirroringParticipants] PRIMARY KEY CLUSTERED 
        (
			[DatabaseID] ASC
		),
		CONSTRAINT [FKMirroringParticipantsSQLServerDatabaseNames] 
			FOREIGN KEY([DatabaseID]) REFERENCES [SQLServerDatabaseNames] ([DatabaseID])ON DELETE CASCADE,

	)
end
-----------------------------------------------------------------------

IF (OBJECT_ID('MirroringPreferredConfig') IS NULL)
BEGIN
CREATE TABLE [dbo].[MirroringPreferredConfig](
	[MirroringGuid] [uniqueidentifier] NOT NULL,
	[MirrorInstanceID] [int] NULL,
	[PrincipalInstanceID] [int] NULL,
	[NormalConfiguration] [bit] NOT NULL,
	[DatabaseName] [nvarchar](128) NULL,
	[WitnessAddress] [nvarchar](128) NULL,
	CONSTRAINT [PKMirroringPreferredConfig] PRIMARY KEY CLUSTERED 
	(
		[MirroringGuid] ASC
	))

END
------------------------------------------------------------------------
IF (OBJECT_ID('MirroringStatistics') IS NULL)
BEGIN
CREATE TABLE [dbo].[MirroringStatistics](
	[DatabaseID] [int] NOT NULL,
	[mirroring_guid] [uniqueidentifier] NOT NULL,
	[UTCCollectionDateTime] [datetime] NOT NULL,
	[role] [tinyint] NULL,
	[mirroring_state] [tinyint] NULL,
	[witness_status] [tinyint] NULL,
	[log_generation_rate] [int] NULL,
	[unsent_log] [int] NULL,
	[send_rate] [int] NULL,
	[unrestored_log] [int] NULL,
	[recovery_rate] [int] NULL,
	[transaction_delay] [int] NULL,
	[transactions_per_sec] [int] NULL,
	[average_delay] [int] NULL,
	[time_recorded] [datetime] NULL,
	[time_behind] [datetime] NULL,
	[local_time] [datetime] NULL,
	 CONSTRAINT [PKMirroringStatistics] PRIMARY KEY CLUSTERED 
	(
		[DatabaseID] ASC,
		[UTCCollectionDateTime] ASC
	),
	CONSTRAINT [FKMirroringStatisticsSQLServerDatabaseNames] 
		FOREIGN KEY([DatabaseID])REFERENCES [dbo].[SQLServerDatabaseNames] ([DatabaseID])
		ON DELETE CASCADE
	)
END
---------------------------------------------------------------------------
IF (OBJECT_ID('Tags') IS NULL)
BEGIN
CREATE TABLE [dbo].[Tags]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	--Adding new column to calculate health index
	--Srishti Purohit 10.1
	[TagScaleFactor] [float] NULL
	CONSTRAINT [PKTags] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)
) 
END

--Start : Upgrade to 10.1 (Srishti Purohit) - Adding new column to calculate health index
	if not exists(select name from sys.columns where object_id = object_id('Tags') and name=N'TagScaleFactor' collate database_default )
	BEGIN
		ALTER TABLE [Tags]
		ADD [TagScaleFactor] [float] NULL	
	END
--End : Upgrade to 10.1 (Srishti Purohit) - Adding new column to calculate health index

---------------------------------------------------------------------------
IF (OBJECT_ID('ServerTags') IS NULL)
BEGIN
CREATE TABLE [dbo].[ServerTags]
(
	[SQLServerId] [int] NOT NULL,
	[TagId] [int] NOT NULL,
	CONSTRAINT [PKServerTags] PRIMARY KEY CLUSTERED 
	(
		[SQLServerId] ASC,
		[TagId] ASC
	),
	CONSTRAINT [FKServerTagsMonitoredSQLServers] FOREIGN KEY
	(
		[SQLServerId]
	) 
	REFERENCES [dbo].[MonitoredSQLServers] 
	(
		[SQLServerID]
	)
	ON DELETE CASCADE,
	CONSTRAINT [FKServerTagsTags] FOREIGN KEY
	(
		[TagId]
	)
	REFERENCES [dbo].[Tags] 
	(
		[Id]
	)
	ON DELETE CASCADE
)
END
---------------------------------------------------------------------------
IF (OBJECT_ID('CustomCounterTags') IS NULL)
BEGIN
CREATE TABLE [dbo].[CustomCounterTags]
(
	[Metric] [int] NOT NULL,
	[TagId] [int] NOT NULL,
	CONSTRAINT [PK_CustomCounterTags] PRIMARY KEY CLUSTERED 
	(
		[Metric] ASC,
		[TagId] ASC
	),
	CONSTRAINT [FK_CustomCounterTags_CustomCounterDefinition] FOREIGN KEY
	(
		[Metric]
	)
	REFERENCES [dbo].[CustomCounterDefinition] 
	(
		[Metric]
	)
	ON DELETE CASCADE,
	CONSTRAINT [FK_CustomCounterTags_Tags] FOREIGN KEY
	(
		[TagId]
	)
	REFERENCES [dbo].[Tags] 
	(
		[Id]
	)
	ON DELETE CASCADE
)
END
---------------------------------------------------------------------------
IF (OBJECT_ID('PermissionTags') IS NULL)
BEGIN
CREATE TABLE [dbo].[PermissionTags]
(
	[PermissionId] [int] NOT NULL,
	[TagId] [int] NOT NULL,
	CONSTRAINT [PK_PermissionTags] PRIMARY KEY CLUSTERED 
	(
		[PermissionId] ASC,
		[TagId] ASC
	),
	CONSTRAINT [FK_PermissionTags_Permission] FOREIGN KEY
	(
		[PermissionId]
	)
	REFERENCES [dbo].[Permission] 
	(
		[PermissionID]
	)
	ON DELETE CASCADE,
	CONSTRAINT [FK_PermissionTags_Tags] FOREIGN KEY
	(
		[TagId]
	)
	REFERENCES [dbo].[Tags] 
	(
		[Id]
	)
	ON DELETE CASCADE
)
END
---------------------------------------------------------------------------
IF (OBJECT_ID('ReportIntervals') IS NULL)
BEGIN
CREATE TABLE [dbo].[ReportIntervals](
	[Value] [int] NULL,
	[Label] [varchar](50) NULL
) ON [PRIMARY]
END
----------------------------------------------------------------------------
IF (OBJECT_ID('ReportPeriods') IS NULL)
BEGIN
CREATE TABLE [dbo].[ReportPeriods](
	[Value] [int] NULL,
	[Label] [varchar](50) NULL
) ON [PRIMARY]
END
----------------------------------------------------------------------------
IF (OBJECT_ID('ReportPeriodIntervals') IS NULL)
BEGIN
CREATE TABLE [dbo].[ReportPeriodIntervals](
	[ReportNumber] [int] NOT NULL,
	[PeriodValue] [int] NOT NULL,
	[IntervalValue] [int] NOT NULL,
 CONSTRAINT [PK_ReportPeriodIntervals] PRIMARY KEY CLUSTERED 
(
	[ReportNumber] ASC,
	[PeriodValue] ASC,
	[IntervalValue] ASC
) ON [PRIMARY]
) ON [PRIMARY]
END
----------------------------------------------------------------------------
IF (OBJECT_ID('SQLServerVersions') IS NULL)
BEGIN
CREATE TABLE [dbo].[SQLServerVersions](
	[MajorVersion] [int] NOT NULL,
	[MinorVersion] [int] NOT NULL,
	[BuildNumber] [int] NOT NULL,
	[VersionName] [nvarchar](30),
	[ServicePackName] [nvarchar](5)
) ON [PRIMARY]
END

------------------------------------------------------------------------------------------

IF (OBJECT_ID('ReplicationTopology') IS NULL)
BEGIN
CREATE TABLE [dbo].[ReplicationTopology]
(
	[PublisherInstance] [nvarchar](128) NOT NULL,
	[PublisherDB] [nvarchar](128) NOT NULL,
	[Publication] [nvarchar](128),
	[LastPublisherSnapshotDateTime] [datetime],
	[LastDistributorSnapshotDateTime] [datetime],
	[LastSubscriberSnapshotDateTime] [datetime],
	[DistributorInstance] [nvarchar](128),
	[DistributorDB] [nvarchar](128),
	[SubscriberInstance] [nvarchar](128),
	[SubscriberDB] [nvarchar](128),
	[PublisherDBID] [int],
	[DistributorDBID] [int],
	[SubscriberDBID] [int],
	[SubscribedTransactions] [int],
	[NonSubscribedTransactions] [int],
	[NonDistributedTransactions] [int],
	[ReplicationLatency] [float],
	[MaxSubscriptionLatency] [int],
	[ReplicationType] [tinyint],
	[SubscriptionType] [tinyint],
	[LastSubscriberUpdate] [datetime],
	[LastSyncStatus] [tinyint],
	[LastSyncSummary] [nvarchar](128),
	[LastSyncTime] [datetime],
	[SubscriptionStatus] [tinyint],
	[PublicationDescription] [nvarchar](255),
	[ArticleCount] [int]
)
END
-----------------------------------------------------------------------------------------------
IF (OBJECT_ID('CounterMasterList') IS NULL)
BEGIN
CREATE TABLE [dbo].[CounterMasterList](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CounterName] [nvarchar](255) NOT NULL,
	[CounterFriendlyName] [nvarchar](255) NULL,
	[CounterType] [int] NULL,
	[AvailableInCustomReport] [bit] NULL
) ON [PRIMARY]
END
-----------------------------------------------------------------------------------------------
IF (OBJECT_ID('CustomReports') IS NULL)
BEGIN
CREATE TABLE [dbo].[CustomReports](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[reportName] [nvarchar](255) NOT NULL,
	[reportText] [ntext] NULL,
	[reportShortDescription] [nvarchar](255) NULL,
	[ShowTopServers] [bit] default 0
 CONSTRAINT [PK_CustomReports] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
) ON [PRIMARY]
) ON [PRIMARY]


CREATE UNIQUE NONCLUSTERED INDEX [IX_CustomReports] ON [dbo].[CustomReports] 
(
	[ID] ASC,
	[reportName] ASC
) ON [PRIMARY]
END
ELSE
BEGIN
	IF (not exists(select id from syscolumns where id=object_id('CustomReports') and name = 'ShowTopServers' collate database_default))
	BEGIN
		ALTER TABLE [CustomReports]
			ADD [ShowTopServers] [bit] default 0
	END
END
-----------------------------------------------------------------------------------------------
IF (OBJECT_ID('CustomReportsCounters') IS NULL)
BEGIN
CREATE TABLE [dbo].[CustomReportsCounters](
	[ID] [int] NOT NULL,
	[GraphNumber] [int] NOT NULL,
	[CounterShortDescription] [nvarchar](255) NULL,
	[CounterName] [nvarchar](255) NULL,
	[Aggregation] [int] NULL,
	[Source] [int] NULL
	CONSTRAINT [PK_CustomReportsCounters] PRIMARY KEY CLUSTERED 
	(
		[ID] ASC,
		[GraphNumber] ASC
	) ON [PRIMARY],
	CONSTRAINT [FK_CustomReportsCounters_CustomReports] FOREIGN KEY([ID]) 
	REFERENCES [dbo].[CustomReports] ([ID]) ON DELETE CASCADE
) ON [PRIMARY]
END

-----------------------------------------------------------------------------------------------
IF (OBJECT_ID('WaitCategories') IS NULL)
BEGIN
CREATE TABLE [dbo].[WaitCategories](
	[CategoryID] [int] IDENTITY NOT NULL,
	[Category] [varchar](120) UNIQUE NOT NULL,
	[ExcludeFromCollection] [bit] DEFAULT ((0)),
	CONSTRAINT [PKWaitCategories] PRIMARY KEY CLUSTERED 
	(
		[CategoryID] ASC
	),
	) ON [PRIMARY]
END

-----------------------------------------------------------------------------------------------
IF (OBJECT_ID('WaitTypes') IS NULL)
BEGIN
CREATE TABLE [dbo].[WaitTypes](
	[WaitTypeID] [int] IDENTITY NOT NULL,
	[WaitType] [varchar](120) NOT NULL,
	[CategoryID] [int] NULL,
	[Description] [nvarchar](1000),
	[HelpLink] [nvarchar](500)
	CONSTRAINT [PKWaitTypes] PRIMARY KEY CLUSTERED 
	(
		[WaitTypeID] ASC
	)
	CONSTRAINT [FKWaitTypesWaitCategories] FOREIGN KEY 
		(
		[CategoryID]
		) 
	REFERENCES  [WaitCategories] 
		(
		[CategoryID]
		) on delete cascade) ON [PRIMARY]
END  

-----------------------------------------------------------------------------------------------
IF (OBJECT_ID('WaitStatistics') IS NULL)
BEGIN
CREATE TABLE [dbo].[WaitStatistics](
	[WaitStatisticsID] [bigint] IDENTITY NOT NULL,
	[SQLServerID] [int] NOT NULL ,
	[UTCCollectionDateTime] [datetime] NOT NULL ,
	[TimeDeltaInSeconds] [float] NULL,
	CONSTRAINT [FKWaitStatisticsMonitoredSQLServers] FOREIGN KEY 
		(
		[SQLServerID]
		) 
	REFERENCES  [MonitoredSQLServers] 
		(
		[SQLServerID]
		) on delete cascade,
	CONSTRAINT [UKWaitStatistics] UNIQUE NONCLUSTERED
	(
		[WaitStatisticsID] ASC
	),
	CONSTRAINT [PKWaitStatistics] PRIMARY KEY CLUSTERED 
	(
		[UTCCollectionDateTime],[SQLServerID] ASC
	)
	) ON [PRIMARY]
END

-----------------------------------------------------------------------------------------------
IF (OBJECT_ID('WaitStatisticsDetails') IS NULL)
BEGIN
CREATE TABLE [dbo].[WaitStatisticsDetails](
	[WaitStatisticsID] [bigint] NOT NULL,
	[WaitTypeID] [int] NOT NULL,
	[WaitingTasks] [bigint] NULL,
	[WaitTimeInMilliseconds] [bigint] NULL,
	[MaxWaitTimeInMilliseconds] [bigint] NULL,
	[ResourceWaitTimeInMilliseconds] [bigint] NULL,
	CONSTRAINT [FKWaitStatisticsDetailsWaitTypes] FOREIGN KEY 
		(
		[WaitTypeID]
		) 
	REFERENCES  [WaitTypes] 
		(
		[WaitTypeID]
		) on delete cascade,
	CONSTRAINT [FKWaitStatisticsDetailsWaitStatistics] FOREIGN KEY 
		(
		[WaitStatisticsID]
		) 
	REFERENCES  [WaitStatistics] 
		(
		[WaitStatisticsID]
		) on delete cascade,
	CONSTRAINT [PKWaitStatisticsDetails] PRIMARY KEY CLUSTERED 
	(
		[WaitStatisticsID], [WaitTypeID] ASC
	)
	) ON [PRIMARY]
END

-----------------------------------------------------------------------------------------------
IF (OBJECT_ID('XEMapKeys') IS NULL)
BEGIN

CREATE TABLE [dbo].[XEMapKeys](
	[WaitType] varchar(120) NOT NULL,
	[SQLVersion] [int] NOT NULL,
	[MapKey] [int] NOT NULL,
 CONSTRAINT [PK_XEMapKeys] PRIMARY KEY CLUSTERED 
(
	[WaitType] ASC,
	[SQLVersion] ASC
))
END
-----------------------------------------------------------------------------------------------


IF (OBJECT_ID('SQLStatements') IS NULL)
BEGIN
CREATE TABLE [dbo].[SQLStatements](
	[SQLStatementID] int identity,
	[SQLStatementHash] varchar(30),
	[SQLStatement] varchar(4000),
	[Overflow] bit,
	CONSTRAINT [UKSQLStatementHash] UNIQUE NONCLUSTERED
		(
			[SQLStatementHash] ASC
		),
	CONSTRAINT [PKSQLStatements] PRIMARY KEY CLUSTERED 
		(
			[SQLStatementID] ASC
		))

END

-----------------------------------------------------------------------------------------------
IF (OBJECT_ID('SQLStatementsOverflow') IS NULL)
BEGIN
CREATE TABLE [dbo].[SQLStatementsOverflow](
	[SQLStatementID] int,
	[SQLStatementOverflow] nvarchar(max),
	CONSTRAINT [PKSQLStatementsUnicode] PRIMARY KEY CLUSTERED 
	(
		[SQLStatementID] ASC
	),
	CONSTRAINT [FKSQLStatementsUnicodeSQLStatements] FOREIGN KEY 
		(
		[SQLStatementID]
		) 
	REFERENCES  [SQLStatements] 
		(
		[SQLStatementID]
		) on delete cascade)

	
END
else
begin
	-- Upgrade table if originally installed on 2000
	if exists(select id from syscolumns where id =  object_id('SQLStatementsOverflow') and name = 'SQLStatementOverflow' collate database_default and length > -1)
	begin
		alter table SQLStatementsOverflow alter column SQLStatementOverflow nvarchar(max)
	end
end

-----------------------------------------------------------------------------------------------
IF (OBJECT_ID('SQLSignatures') IS NULL)
BEGIN
CREATE TABLE [dbo].[SQLSignatures](
	[SQLSignatureID] int identity,
	[SQLSignatureHash] varchar(30) NOT NULL,
	[SQLSignature] varchar(4000),
	[Overflow] bit,
	[SQLStatementExampleID] int,
	[DoNotAggregate] bit,
	CONSTRAINT [UKSQLSignatureHash] UNIQUE NONCLUSTERED
	(
		[SQLSignatureHash] ASC
	),
	CONSTRAINT [PKSQLSignatures] PRIMARY KEY CLUSTERED 
	(
		[SQLSignatureID] ASC
	))

	CREATE INDEX [IXSignatureExample] on [SQLSignatures]([SQLStatementExampleID])
END

-- Upgrade table if originally installed on 2000
if (not exists(select 1 from INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE where CONSTRAINT_NAME = 'FKSQLSignaturesSQLStatements' collate database_default))
begin
	
	ALTER TABLE [SQLSignatures]
	ADD CONSTRAINT [FKSQLSignaturesSQLStatements] FOREIGN KEY 
		(
		[SQLStatementExampleID]
		) 
	REFERENCES  [SQLStatements] 
		(
		[SQLStatementID]
		) on delete set null
end


-----------------------------------------------------------------------------------------------
IF (OBJECT_ID('SQLSignaturesOverflow') IS NULL)
BEGIN
CREATE TABLE [dbo].[SQLSignaturesOverflow](
	[SQLSignatureID] int,
	[SQLSignatureOverflow] nvarchar(max),
	CONSTRAINT [PKSQLSignaturesUnicode] PRIMARY KEY CLUSTERED 
	(
		[SQLSignatureID] ASC
	),
	CONSTRAINT [FKSQLSignaturesUnicodeSQLSignatures] FOREIGN KEY 
		(
		[SQLSignatureID]
		) 
	REFERENCES  [SQLSignatures] 
		(
		[SQLSignatureID]
		) on delete cascade)

END

-- Upgrade table if originally installed on 2000
if exists(select id from syscolumns where id =  object_id('SQLSignaturesOverflow') 
	and name = 'SQLSignatureOverflow' collate database_default and length > -1)
begin
	alter table SQLSignaturesOverflow alter column SQLSignatureOverflow nvarchar(max)
end
-----------------------------------------------------------------------------------------------
IF (OBJECT_ID('HostNames') IS NULL)
BEGIN
CREATE TABLE [dbo].[HostNames](
	[HostNameID] int identity,
	[HostName] nvarchar(256),
	CONSTRAINT [UKHostName] UNIQUE NONCLUSTERED
	(
		[HostName] ASC
	),
	CONSTRAINT [PKHostNames] PRIMARY KEY CLUSTERED 
	(
		[HostNameID] ASC
	))
END

-----------------------------------------------------------------------------------------------
IF (OBJECT_ID('ApplicationNames') IS NULL)
BEGIN
CREATE TABLE [dbo].[ApplicationNames](
	[ApplicationNameID] int identity,
	[ApplicationName] nvarchar(256),
	CONSTRAINT [UKApplicationName] UNIQUE NONCLUSTERED
	(
		[ApplicationName] ASC
	),
	CONSTRAINT [PKApplicationNames] PRIMARY KEY CLUSTERED 
	(
		[ApplicationNameID] ASC
	))
END

-----------------------------------------------------------------------------------------------
IF (OBJECT_ID('LoginNames') IS NULL)
BEGIN
CREATE TABLE [dbo].[LoginNames](
	[LoginNameID] int identity,
	[LoginName] nvarchar(256),
	CONSTRAINT [UKLoginName] UNIQUE NONCLUSTERED
	(
		[LoginName] ASC
	),
	CONSTRAINT [PKLoginNames] PRIMARY KEY CLUSTERED 
	(
		[LoginNameID] ASC
	))
END

-----------------------------------------------------------------------------------------------
IF (OBJECT_ID('ActiveWaitStatistics') IS NULL)
BEGIN
CREATE TABLE [dbo].[ActiveWaitStatistics](
	[ActiveWaitID] [int] IDENTITY,
	[SQLServerID] [int] NOT NULL ,
	[UTCCollectionDateTime] [datetime] NOT NULL ,	
	[StatementUTCStartTime] [datetime] NOT NULL,
	[StatementLocalStartTime] [datetime],
	[WaitDuration] [bigint],
	[SessionID] [smallint],
	[WaitTypeID] [int],
	[HostNameID] [int],
	[ApplicationNameID] [int],
	[LoginNameID] [int],
	[DatabaseID] [int],
	[SQLStatementID] [int],
	[MSTicks] [bigint],
	[SQLSignatureID] [int],
	CONSTRAINT [FKActiveWaitStatisticsMonitoredSQLServers] FOREIGN KEY 
		(
		[SQLServerID]
		)
	REFERENCES  [MonitoredSQLServers] 
		(
		[SQLServerID]
		) on delete cascade,
	CONSTRAINT [FKActiveWaitStatisticsWaitTypes] FOREIGN KEY 
		(
		[WaitTypeID]
		) 
	REFERENCES  [WaitTypes] 
		(
		[WaitTypeID]
		) on delete cascade,
	CONSTRAINT [FKActiveWaitStatisticsHostNames] FOREIGN KEY 
		(
		[HostNameID]
		) 
	REFERENCES  [HostNames] 
		(
		[HostNameID]
		) on delete no action,
	CONSTRAINT [FKActiveWaitStatisticsApplicationNames] FOREIGN KEY 
		(
		[ApplicationNameID]
		) 
	REFERENCES  [ApplicationNames] 
		(
		[ApplicationNameID]
		) on delete no action,
	CONSTRAINT [FKActiveWaitStatisticsLoginNames] FOREIGN KEY 
		(
		[LoginNameID]
		) 
	REFERENCES  [LoginNames] 
		(
		[LoginNameID]
		) on delete no action,

	CONSTRAINT [FKActiveWaitStatisticsSQLServerDatabaseNames] FOREIGN KEY 
		(
		[DatabaseID]
		) 
	REFERENCES  [SQLServerDatabaseNames] 
		(
		[DatabaseID]
		) on delete no action,
	CONSTRAINT [FKActiveWaitStatisticsSQLStatements] FOREIGN KEY 
		(
		[SQLStatementID]
		) 
	REFERENCES  [SQLStatements] 
		(
		[SQLStatementID]
		) on delete cascade)
	CREATE CLUSTERED INDEX [IXActiveWaitStatistics] on [ActiveWaitStatistics]([StatementUTCStartTime])
	CREATE INDEX [IXActiveWaitStatisticsMSTicks] on [ActiveWaitStatistics]([SQLServerID],[MSTicks])
END


-- upgrade to 6.5
IF (not exists(select id from syscolumns where id=object_id('ActiveWaitStatistics') and name = 'SQLSignatureID' collate database_default))
BEGIN
		ALTER TABLE [ActiveWaitStatistics]
			ADD [SQLSignatureID] [int],
			[StatementLocalStartTime] datetime
END


--SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session  -- Created new tables for Query Plans .
-------------------------------------------------------------------------------------
IF (OBJECT_ID('SQLQueryPlans') IS NULL)
BEGIN
CREATE TABLE [dbo].[SQLQueryPlans](
	[PlanID] int identity(1,1),
	[SQLStatementID] int ,
	[PlanXML] varchar(4000), --(Base64 encoded, compressed XML)
	[Overflow] bit,
	[IsActualPlan] bit NOT NULL DEFAULT 1, --SQLdm 10.0 (Tarun Sapra)- Flag for determining if the plan is actual or estimated
	CONSTRAINT [PKSQLQueryPlans] PRIMARY KEY CLUSTERED 
		(
			[PlanID] ASC
		),
	CONSTRAINT [FKSQLQueryPlansSQLStatementID] FOREIGN KEY 
			(
			[SQLStatementID]
			) 
		REFERENCES  [SQLStatements] 
			(
			[SQLStatementID]
			)  on delete cascade
		)

END

--Start -Upgrade to 10.0 (Tarun Sapra) - Estimated Query Plan View -- Flag for determining if the plan is actual or estimated
	if not exists(select name from sys.columns where object_id = object_id('SQLQueryPlans') and name=N'IsActualPlan' collate database_default )
	BEGIN
		ALTER TABLE SQLQueryPlans
		ADD IsActualPlan BIT NOT NULL DEFAULT 1	
	END
--End -Upgrade to 10.0 (Tarun Sapra) - Estimated Query Plan View -- Flag for determining if the plan is actual or estimated
------------------------------------------------------------------------------------------
--SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session  --   new table for query plan overflowe
IF (OBJECT_ID('SQLQueryPlansOverflow') IS NULL)
BEGIN
CREATE TABLE [dbo].[SQLQueryPlansOverflow](
	[PlanID] int,
	[PlanXMLOverflow] nvarchar(max),
	CONSTRAINT [PKSQLQueryPlansOverflow] PRIMARY KEY CLUSTERED 
	(
		[PlanID] ASC
	),
	CONSTRAINT [PKSQLQueryPlansOverflowPlanID] FOREIGN KEY 
		(
		[PlanID]
		) 
	REFERENCES  [SQLQueryPlans] 
		(
		[PlanID]
		) on delete cascade
	)	
	
END
------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------
IF (OBJECT_ID('QueryMonitorStatistics') IS NULL)
BEGIN

CREATE TABLE  [QueryMonitorStatistics] 
	(
	[QueryStatisticsID] [int] IDENTITY,
	[SQLServerID] [int] NOT NULL ,
	[UTCCollectionDateTime] [datetime] NOT NULL ,
	[StatementUTCStartTime] [datetime] NOT NULL,
	[StatementLocalStartTime] [datetime],
	[CompletionTime] as dateadd(ss,[DurationMilliseconds]/1000,[StatementUTCStartTime]),
	[DurationMilliseconds] [bigint] NULL,
	[CPUMilliseconds] [bigint] NULL,
	[Reads] [bigint] NULL,
	[Writes] [bigint] NULL,
	[HostNameID] [int],
	[ApplicationNameID] [int],
	[LoginNameID] [int],
	[DatabaseID] [int],
	[StatementType] [int] NULL,
	[SQLStatementID] [int],
	[SQLSignatureID] [int],
	[SessionID] [smallint],
	--Start -Upgrade to 9.0 (Ankit Srivastava ) - Query Plan View -- Added new column for PlanID
	[PlanID] [int] NULL,
	CONSTRAINT [FKQueryMonitorStatisticsSQLQueryPlans] FOREIGN KEY 
		(
		[PlanID]
		)
	REFERENCES  [SQLQueryPlans] 
		(
		[PlanID]
		),
	CONSTRAINT [FKQueryMonitorStatisticsMonitoredSQLServers] FOREIGN KEY 
		(
		[SQLServerID]
		)
	--End -Upgrade to 9.0 (Ankit Srivastava ) - Query Plan View -- Added new column for PlanID
	REFERENCES  [MonitoredSQLServers] 
		(
		[SQLServerID]
		) on delete cascade,
	CONSTRAINT [FKQueryMonitorStatisticsHostNames] FOREIGN KEY 
		(
		[HostNameID]
		) 
	REFERENCES  [HostNames] 
		(
		[HostNameID]
		) on delete no action,
	CONSTRAINT [FKQueryMonitorStatisticsApplicationNames] FOREIGN KEY 
		(
		[ApplicationNameID]
		) 
	REFERENCES  [ApplicationNames] 
		(
		[ApplicationNameID]
		) on delete no action,
	CONSTRAINT [FKQueryMonitorStatisticsLoginNames] FOREIGN KEY 
		(
		[LoginNameID]
		) 
	REFERENCES  [LoginNames] 
		(
		[LoginNameID]
		) on delete no action,
	CONSTRAINT [FKQueryMonitorStatisticsSQLServerDatabaseNames] FOREIGN KEY 
		(
		[DatabaseID]
		) 
	REFERENCES  [SQLServerDatabaseNames] 
		(
		[DatabaseID]
		) on delete no action,
	CONSTRAINT [FKQueryMonitorStatisticsSQLStatements] FOREIGN KEY 
		(
		[SQLStatementID]
		) 
	REFERENCES  [SQLStatements] 
		(
		[SQLStatementID]
		) on delete no action,
	CONSTRAINT [FKQueryMonitorStatisticsSQLSignatures] FOREIGN KEY 
		(
		[SQLSignatureID]
		)  
	REFERENCES  [SQLSignatures] 
		(
		[SQLSignatureID]
		) on delete no action
	)

	CREATE CLUSTERED INDEX [IXQueryMonitorStatistics] on [QueryMonitorStatistics]([StatementUTCStartTime])

	create index [IXQueryMonitorStatisticsSQLServerIDGrooming] on [QueryMonitorStatistics]([SQLServerID])
		
	create index [IXQueryMonitorStatisticsQueryStatisticsID] on [QueryMonitorStatistics]([QueryStatisticsID])
	
	CREATE INDEX [IXQueryMonitorStatisticsSQLServerID] ON [QueryMonitorStatistics] ([SQLServerID],[DurationMilliseconds], [StatementType]) 
		INCLUDE ([StatementUTCStartTime], [CPUMilliseconds], [Reads], [Writes], [HostNameID], [ApplicationNameID], [LoginNameID], [DatabaseID], [SQLStatementID], [SQLSignatureID], [SessionID])
END
else
begin
	-- Correct computed column if necessary
	if exists(select name from sys.computed_columns where object_id = object_id('QueryMonitorStatistics') and name=N'CompletionTime' and definition like '%UTCCollectionDateTime%')
	begin
		alter table QueryMonitorStatistics
			drop column [CompletionTime] 
		alter table QueryMonitorStatistics
			add [CompletionTime] as dateadd(ss,[DurationMilliseconds]/1000,[StatementUTCStartTime])
	end	
	--Start -Upgrade to 9.0 (Ankit Srivastava ) - Query Plan View -- Added new column for PlanID
	if not exists(select name from sys.columns where object_id = object_id('QueryMonitorStatistics') and name=N'PlanID' collate database_default )
	BEGIN
		alter table QueryMonitorStatistics
			add [PlanID] int NULL
		alter table QueryMonitorStatistics
			Add constraint [FKQueryMonitorStatisticsSQLQueryPlans] FOREIGN KEY 
			(
				[PlanID]
			)
			references [SQLQueryPlans] 
			(
				[PlanID]
			)
	END
	--End -Upgrade to 9.0 (Ankit Srivastava) - Query Plan View -- Added new column for PlanID	
		
end

-----------------------------------------------------------------------------------------------
IF (OBJECT_ID('AllSQLStatements') is not null)
BEGIN
	DROP VIEW  [dbo].[AllSQLStatements]
END
GO
create view [dbo].[AllSQLStatements]
as
	select
		ss.[SQLStatementID],
		ss.[SQLStatementHash],
		[SQLStatement] = cast(case when [Overflow] = 1 then so.[SQLStatementOverflow] else ss.[SQLStatement] end as nvarchar(max))
	from
		SQLStatements ss
		left join SQLStatementsOverflow so
		on ss.SQLStatementID = so.SQLStatementID

go

-----------------------------------------------------------------------------------------------
IF (OBJECT_ID('AllSQLSignatures') is not null)
BEGIN
	DROP VIEW  [dbo].[AllSQLSignatures]
END
GO
create view [dbo].[AllSQLSignatures]
as
	select
		ss.[SQLSignatureID],
		ss.[SQLSignatureHash],
		[SQLSignature] = cast(case when [Overflow] = 1 then so.[SQLSignatureOverflow] else ss.[SQLSignature] end as nvarchar(max)),
		[SQLStatementExample] = s.[SQLStatement],
		[DoNotAggregate] = ss.[DoNotAggregate]
	from
		SQLSignatures ss
		left join SQLSignaturesOverflow so
		on ss.SQLSignatureID = so.SQLSignatureID
		left join AllSQLStatements s
		on ss.[SQLStatementExampleID] = s.[SQLStatementID]

go
-------------------------------------------------------------------------------------------------
IF (OBJECT_ID('Blocks') IS NULL)
BEGIN
Create TABLE [dbo].[Blocks](
	[BlockID] [uniqueidentifier] NOT NULL,
	[XActID] [bigint] NOT NULL,
	[SQLServerID] [int] NOT NULL,
	[UTCCollectionDateTime] [datetime] NOT NULL,
	[XDLData] [nvarchar](max) NULL,
	CONSTRAINT [PKBlocks] PRIMARY KEY NONCLUSTERED
	(
		[BlockID]
	),
	CONSTRAINT [FKBlocksMonitoredSQLServers] FOREIGN KEY 
		(
		[SQLServerID]
		)
	REFERENCES  [MonitoredSQLServers] 
		(
		[SQLServerID]
		) on delete cascade 
	)

CREATE UNIQUE NONCLUSTERED INDEX [IX_Blocks] ON [dbo].[Blocks]
(
	[XActID] ASC,
	[UTCCollectionDateTime] ASC
)
END

-----------------------------------------------------------------------------------------------
IF (OBJECT_ID('BlockingSessionStatistics') IS NULL)
BEGIN

CREATE TABLE  [BlockingSessionStatistics] 
	(
	[BlockingSessionID] [int] IDENTITY,
	[SQLServerID] [int] NOT NULL ,
	[UTCCollectionDateTime] [datetime] NOT NULL,
	[BlockID] [uniqueidentifier] NULL,
	[BlockingUTCStartTime] [datetime] NOT NULL,
	[BlockingLocalStartTime] [datetime],
	[BlockingDurationMilliseconds] [bigint] NULL,
	[HostNameID] [int],
	[ApplicationNameID] [int],
	[LoginNameID] [int],
	[DatabaseID] [int],
	[SQLStatementID] [int],
	[SQLSignatureID] [int],
	[SessionID] [smallint],
	CONSTRAINT [FKBlockingSessionStatisticsMonitoredSQLServers] FOREIGN KEY 
		(
		[SQLServerID]
		)
	REFERENCES  [MonitoredSQLServers] 
		(
		[SQLServerID]
		) on delete cascade,
	CONSTRAINT [FKBlockingSessionStatisticsHostNames] FOREIGN KEY 
		(
		[HostNameID]
		) 
	REFERENCES  [HostNames] 
		(
		[HostNameID]
		) on delete no action,
	CONSTRAINT [FKBlockingSessionStatisticsApplicationNames] FOREIGN KEY 
		(
		[ApplicationNameID]
		) 
	REFERENCES  [ApplicationNames] 
		(
		[ApplicationNameID]
		) on delete no action,
	CONSTRAINT [FKBlockingSessionStatisticsLoginNames] FOREIGN KEY 
		(
		[LoginNameID]
		) 
	REFERENCES  [LoginNames] 
		(
		[LoginNameID]
		) on delete no action,
	CONSTRAINT [FKBlockingSessionStatisticsSQLServerDatabaseNames] FOREIGN KEY 
		(
		[DatabaseID]
		) 
	REFERENCES  [SQLServerDatabaseNames] 
		(
		[DatabaseID]
		) on delete no action,
	CONSTRAINT [FKBlockingSessionStatisticsSQLStatements] FOREIGN KEY 
		(
		[SQLStatementID]
		) 
	REFERENCES  [SQLStatements] 
		(
		[SQLStatementID]
		) on delete no action,
	CONSTRAINT [FKBlockingSessionStatisticsSQLSignatures] FOREIGN KEY 
		(
		[SQLSignatureID]
		) 
	REFERENCES  [SQLSignatures] 
		(
		[SQLSignatureID]
		) on delete no action	
	)
	CREATE CLUSTERED INDEX [IXBlockingSessionStatistics] on [BlockingSessionStatistics]([BlockingUTCStartTime])
	--SQLdm 9.1 (Ankit Srivastava ) --Rally Defect DE43572 -- Adding index to improve the query performance
	CREATE NONCLUSTERED INDEX [IXBlockingSessionStatistics_DurationMilliSeconds] on [BlockingSessionStatistics]([BlockingDurationMilliseconds])
END
ELSE
BEGIN
	if (not exists(select object_id from sys.columns where object_id = object_id('BlockingSessionStatistics') and name = 'BlockID' collate database_default)) 
	begin
		ALTER TABLE [BlockingSessionStatistics] add [BlockID] [uniqueidentifier] NULL
	end
	
END
----------------------------------------------------------------------------

IF (OBJECT_ID('Deadlocks') IS NULL)
BEGIN
CREATE TABLE  [Deadlocks] 
	(
	[DeadlockID] [uniqueidentifier] NOT NULL,
	[SQLServerID] [int] NOT NULL ,
	[UTCCollectionDateTime] [datetime] NOT NULL ,
	[XDLData] nvarchar(max) NULL,
	CONSTRAINT [PKDeadlocks] PRIMARY KEY 
	(
		[DeadlockID]
	),
	CONSTRAINT [FKSDeadlocksMonitoredSQLServers] FOREIGN KEY 
		(
		[SQLServerID]
		)
	REFERENCES  [MonitoredSQLServers] 
		(
		[SQLServerID]
		) on delete cascade 
	)
	
	CREATE INDEX [IXDeadlocks] ON [Deadlocks]([SQLServerID],[UTCCollectionDateTime]) 

END
ELSE
BEGIN
	if (exists(select id from syscolumns where id =  object_id('Deadlocks') and name = 'XDLData' collate database_default and length > -1))
	begin
		execute('alter table Deadlocks alter column XDLData nvarchar(max)')
	end
END

-----------------------------------------------------------------------------------------------
IF (OBJECT_ID('DeadlockProcesses') IS NULL)
BEGIN

CREATE TABLE  [DeadlockProcesses] 
	(
	[DeadlockProcessID] int IDENTITY,
	[DeadlockID] [uniqueidentifier],
	[SQLServerID] [int] NOT NULL ,
	[UTCCollectionDateTime] [datetime] NOT NULL ,
	[UTCOccurrenceDateTime] datetime,
	[LocalOccurrenceDateTime] datetime,
	[HostNameID] [int],
	[ApplicationNameID] [int],
	[LoginNameID] [int],
	[DatabaseID] [int],
	[SQLStatementID] [int],
	[SQLSignatureID] [int],
	[SessionID] [smallint],
	CONSTRAINT [PKDeadlockProcesses] PRIMARY KEY 
	(
		[DeadlockProcessID]
	),
	CONSTRAINT [FKDeadlockProcessesDeadlocks] FOREIGN KEY 
		(
		[DeadlockID]
		)
	REFERENCES  [Deadlocks] 
		(
		[DeadlockID]
		) on delete cascade,
	CONSTRAINT [FKDeadlockProcessesHostNames] FOREIGN KEY 
		(
		[HostNameID]
		) 
	REFERENCES  [HostNames] 
		(
		[HostNameID]
		) on delete no action,
	CONSTRAINT [FKDeadlockProcessesApplicationNames] FOREIGN KEY 
		(
		[ApplicationNameID]
		) 
	REFERENCES  [ApplicationNames] 
		(
		[ApplicationNameID]
		) on delete no action,
	CONSTRAINT [FKDeadlockProcessesLoginNames] FOREIGN KEY 
		(
		[LoginNameID]
		) 
	REFERENCES  [LoginNames] 
		(
		[LoginNameID]
		) on delete no action,
	CONSTRAINT [FKDeadlockProcessesSQLServerDatabaseNames] FOREIGN KEY 
		(
		[DatabaseID]
		) 
	REFERENCES  [SQLServerDatabaseNames] 
		(
		[DatabaseID]
		) on delete no action,
	CONSTRAINT [FKDeadlockProcessesSQLStatements] FOREIGN KEY 
		(
		[SQLStatementID]
		) 
	REFERENCES  [SQLStatements] 
		(
		[SQLStatementID]
		) on delete no action,
	CONSTRAINT [FKDeadlockProcessesSQLSignatures] FOREIGN KEY 
		(
		[SQLSignatureID]
		) 
	REFERENCES  [SQLSignatures] 
		(
		[SQLSignatureID]
		) on delete no action
		)
		--SQLdm 9.1 (Ankit Srivastava ) --Rally Defect DE43572 -- Adding index to improve the query performance
		--CREATE index [DeadlockProcess_NonClustered] on DeadlockProcesses([SQLServerID],[UTCOccurrenceDateTime],[SQLSignatureID])
END

if not exists(select name from sys.indexes where name = 'DeadlockProcessDeadlockID')
begin

	create index DeadlockProcessDeadlockID on DeadlockProcesses([DeadlockID])

end


-----------------------------------------------------------------------------------------------
IF (OBJECT_ID('QuerySignatureAggregation') IS NULL)
BEGIN
create table QuerySignatureAggregation
(
	[QuerySignatureAggregationID] int identity,
	[SQLServerID] int,
	[EventUTCStartTime] datetime,	
	[QueryMonitorOccurrences] dec(38,0),
	[WaitOcurrences] dec(38,0),
	[BlockingOcurrences] dec(38,0),
	[DeadlockOcurrences] dec(38,0),
	[TotalDurationMilliseconds] dec(38,0),
	[MaxDurationMilliseconds] dec(38,0),
	[TotalCPUMilliseconds] dec(38,0),
	[MaxCPUMilliseconds] dec(38,0),
	[TotalReads] dec(38,0),
	[MaxReads] dec(38,0),	
	[TotalWrites] dec(38,0),
	[MaxWrites] dec(38,0),
	[StatementType] int,
	[TotalBlockingDurationMilliseconds] dec(38,0),
	[MaxBlockingDurationMilliseconds] dec(38,0),
	[TotalWaitDuration] dec(38,0),
	[MaxWaitDuration] dec(38,0),
	[ApplicationNameID] int,
	[DatabaseID] int,
	[SQLSignatureID] int,
	CONSTRAINT [PKQuerySignatureAggregation] PRIMARY KEY 
	(
		[QuerySignatureAggregationID]
	),
	CONSTRAINT [FKQuerySignatureAggregationMonitoredSQLServers] FOREIGN KEY 
		(
		[SQLServerID]
		)
	REFERENCES  [MonitoredSQLServers] 
		(
		[SQLServerID]
		)  on delete cascade,
	CONSTRAINT [FKQuerySignatureAggregationApplicationNames] FOREIGN KEY 
		(
		[ApplicationNameID]
		) 
	REFERENCES  [ApplicationNames] 
		(
		[ApplicationNameID]
		) on delete no action,
	CONSTRAINT [FKQuerySignatureAggregationSQLServerDatabaseNames] FOREIGN KEY 
		(
		[DatabaseID]
		) 
	REFERENCES  [SQLServerDatabaseNames] 
		(
		[DatabaseID]
		) on delete no action,
	CONSTRAINT [FKQuerySignatureAggregationSQLSignatures] FOREIGN KEY 
		(
		[SQLSignatureID]
		) 
	REFERENCES  [SQLSignatures] 
		(
		[SQLSignatureID]
		) on delete no action
	)
	--SQLdm 9.1 (Ankit Srivastava ) --Rally Defect DE43572 -- Adding index to improve the query performance
	create index [QuerySignatureAggregation_SQLServerID] on QuerySignatureAggregation([SQLServerID])
end

--------------------------------------------------------------
IF (OBJECT_ID('PredictiveModels') IS NULL)
BEGIN

	CREATE TABLE [PredictiveModels]
	(
		[SQLServerID] [int]   NOT NULL,
		[Metric]      [int]   NOT NULL,
		[Severity]    [int]   NOT NULL,
		[Timeframe]   [int]   NOT NULL,
		[Model]       [image] NOT NULL,
		CONSTRAINT [PK_PredictiveModels] PRIMARY KEY CLUSTERED 
		(
			[SQLServerID] ASC,
			[Metric]      ASC,
			[Severity]    ASC,
			[Timeframe]   ASC
		),
		CONSTRAINT [FKPredictiveModelsServerID] FOREIGN KEY 
		(
			[SQLServerID]
		)	
		REFERENCES [MonitoredSQLServers] 
		(
			[SQLServerID]
		)  on delete cascade
	)	

END

--------------------------------------------------------------
IF (OBJECT_ID('PredictiveForecasts') IS NULL)
BEGIN

	CREATE TABLE [PredictiveForecasts]
	(
		[SQLServerID] [int]   NOT NULL,
		[Metric]      [int]   NOT NULL,
		[Severity]    [int]   NOT NULL,
		[Timeframe]   [int]   NOT NULL,
		[Forecast]    [int]   NOT NULL,
		[Accuracy]    [decimal](5,2) NOT NULL,
		[Expiration]  [datetime] NOT NULL,
		CONSTRAINT [PK_PredictiveForecasts] PRIMARY KEY CLUSTERED 
		(
			[SQLServerID] ASC,
			[Metric]      ASC,
			[Severity]    ASC,
			[Timeframe]   ASC
		),
		CONSTRAINT [FKPredictiveForecastsServerID] FOREIGN KEY 
		(
			[SQLServerID]
		)	
		REFERENCES [MonitoredSQLServers] 
		(
			[SQLServerID]
		)  on delete cascade
	)	

END

--------------------------------------------------------------
IF (OBJECT_ID('RunQueryScripts') IS NULL)
BEGIN
	CREATE TABLE [RunQueryScripts]
	(
		[ScriptID] [int] IDENTITY(1,1) NOT NULL,
		[Type] [smallint] NOT NULL DEFAULT ((1)),
		[Name] [nvarchar](100) NOT NULL,
		[ScriptText] [nvarchar] (max) NOT NULL,
		CONSTRAINT [PK_RunQueryScripts] PRIMARY KEY CLUSTERED 
		(
			[ScriptID] ASC
		)
	)
END
ELSE
BEGIN
	--Upgrade to 7.2 if StateOverview is not nvarchar(max)
	IF (not exists(select sc.object_id from sys.columns sc inner join sys.types st on sc.system_type_id = st.system_type_id
		where object_id=OBJECT_ID('RunQueryScripts') 
		and sc.name='ScriptText' and lower(st.name) = 'nvarchar'  collate database_default))
	begin
		alter table [RunQueryScripts] alter column [ScriptText] nvarchar(max) NOT NULL
	END
END

--------------------------------------------------------------

IF (OBJECT_ID('VirtualHostServers') IS NULL)
BEGIN
	CREATE TABLE [VirtualHostServers]
	(
		[VHostID] [int] IDENTITY(1,1) NOT NULL,
		[VHostName] [nvarchar](256) NOT NULL,
		[VHostAddress] [nvarchar](256) NOT NULL,
		[Active] [bit] NOT NULL,
		[Deleted] [bit] NOT NULL,
		[Username] [nvarchar](128) NULL,
		[Password] [nvarchar](128) NULL,
		[RegisteredDate] [datetime] NOT NULL,
		[ServerType] [nvarchar](128) NOT NULL,
		CONSTRAINT [PKVirtualHostServers] PRIMARY KEY CLUSTERED 
		(
			[VHostID] ASC
		)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[VirtualHostServers] ADD CONSTRAINT [DF_VirtualServers_Active] DEFAULT ((1)) FOR [Active]

	ALTER TABLE [dbo].[VirtualHostServers] ADD CONSTRAINT [DF_VirtualHostServers_Deleted] DEFAULT ((0)) FOR [Deleted]

	ALTER TABLE [dbo].[VirtualHostServers] ADD CONSTRAINT [DF_VirtualHostServers_RegisteredDate] DEFAULT (getutcdate()) FOR [RegisteredDate]

	ALTER TABLE [dbo].[VirtualHostServers] ADD CONSTRAINT [UQ_VirtualHostServers_VHostAddress] UNIQUE NONCLUSTERED ([VHostAddress])
END
ELSE
BEGIN	
    --Upgrade to 8.5 (HyperV) if column ServerType does not exist
	IF (not exists(select sc.object_id from sys.columns sc inner join sys.types st on sc.system_type_id = st.system_type_id
		where object_id=OBJECT_ID('VirtualHostServers') 
		and sc.name='ServerType' and lower(st.name) = 'nvarchar'  collate database_default))
	BEGIN
		ALTER TABLE [dbo].[VirtualHostServers] ADD ServerType nvarchar(128) NOT NULL DEFAULT('Unknown')
	END
END
--------------------------------------------------------------

IF (OBJECT_ID('VMConfigData') IS NULL)
BEGIN

	create table [dbo].VMConfigData (
		[SQLServerID] int not null,
		[UTCCollectionDateTime] datetime not null,
		[UUID] nvarchar(128) null,				-- summary.config.instanceUUID
		[VMName] nvarchar(256) null,			-- summary.config.name
		[VMHeartBeat] int null,					-- guestHeartbeatStatus
		[DomainName] nvarchar(256) null,		-- guest.hostname
		[BootTime] datetime null,				-- runtime.boottime
		[NumCPUs] int null,						-- summary.config.numCpu
		[CPULimit] bigint null,					-- resourceConfig.cpuAllocation.limit
		[CPUReserve] bigint null,				-- resourceConfig.cpuAllocation.reservation
		[MemSize] bigint null,					-- summary.config.memorySizeMB
		[MemLimit] bigint null,					-- resourceConfig.memoryAllocation.limit
		[MemReserve] bigint null,				-- resourceConfig.memoryAllocation.reservation
		Constraint [PKVMConfigData] PRIMARY KEY CLUSTERED
		(
			[SQLServerID] ASC,
			[UTCCollectionDateTime] ASC
		)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[VMConfigData]  WITH CHECK ADD  CONSTRAINT [FKVMConfigDataMonitoredSQLServers] FOREIGN KEY([SQLServerID])
	REFERENCES [dbo].[MonitoredSQLServers] ([SQLServerID])
	ON DELETE CASCADE

	ALTER TABLE [dbo].[VMConfigData] CHECK CONSTRAINT [FKVMConfigDataMonitoredSQLServers]
	
	CREATE INDEX [IX_VMConfigData_UTCCollectionDateTime] on [VMConfigData]([UTCCollectionDateTime])
END

--------------------------------------------------------------

IF (OBJECT_ID('ESXConfigData') IS NULL)
BEGIN

	create table [dbo].ESXConfigData (
		[SQLServerID] int not null,
		[UTCCollectionDateTime] datetime not null,
		[UUID] nvarchar(128) null,				-- summary.hardware.uuid
		[HostName] nvarchar(256) null,			-- summary.config.name
		[DomainName] nvarchar(256) null,		-- config.network.dnsconfig.hostname + config.network.dnsconfig.domainname
		[Status] int null,						-- summary.overallStatus
		[BootTime] datetime null,				-- runtime.boottime
		[CPUMHz] int null,						-- summary.hardware.CPUMhz
		[NumCPUCores] smallint null,			-- summary.hardware.numCpuCores
		[NumCPUPkgs] smallint null,				-- summary.hardware.numCpuPkgs
		[NumCPUThreads] smallint null,			-- summary.hardware.numCpuThreads
		[NumNICs] int null,						-- summary.hardware.numNics
		[MemorySize] bigint null				-- summary.hardware.memorySize
		Constraint [PKESXConfigData] PRIMARY KEY CLUSTERED
		(
			[SQLServerID] ASC,
			[UTCCollectionDateTime] ASC
		)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[ESXConfigData]  WITH CHECK ADD  CONSTRAINT [FKESXConfigDataMonitoredSQLServers] FOREIGN KEY([SQLServerID])
	REFERENCES [dbo].[MonitoredSQLServers] ([SQLServerID])
	ON DELETE CASCADE

	ALTER TABLE [dbo].[ESXConfigData] CHECK CONSTRAINT [FKESXConfigDataMonitoredSQLServers]
	
	CREATE INDEX [IX_ESXConfigData_UTCCollectionDateTime] on [ESXConfigData]([UTCCollectionDateTime])

END

--------------------------------------------------------------

IF (OBJECT_ID('VMStatistics') IS NULL)
BEGIN

	create table [dbo].VMStatistics (
		[SQLServerID] int not null,
		[UTCCollectionDateTime] datetime not null,
		[CPUUsage] float null,
		[CPUUsageMHz] int null,
		[CPUReady] bigint null,
		[CPUSwapWait] bigint null,
		[MemSwapInRate] bigint null,
		[MemSwapOutRate] bigint null,
		[MemSwapped] bigint null,
		[MemActive] bigint null,
		[MemConsumed] bigint null,
		[MemGranted] bigint null,
		[MemBalooned] bigint null,
		[MemUsage] float null,
		[DiskRead] bigint null,
		[DiskWrite] bigint null, 
		[DiskUsage] bigint null,
		[NetUsage] bigint null,
		[NetReceived] bigint null,
		[NetTransmitted] bigint null,
		[PagePerSecVM] bigint null,
		[AvailableByteVm] bigint null
		Constraint [PKVMStatistics] PRIMARY KEY CLUSTERED
		(
			[SQLServerID] ASC,
			[UTCCollectionDateTime] ASC
		)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[VMStatistics]  WITH CHECK ADD  CONSTRAINT [FKVMStatisticsMonitoredSQLServers] FOREIGN KEY([SQLServerID])
	REFERENCES [dbo].[MonitoredSQLServers] ([SQLServerID])
	ON DELETE CASCADE

	ALTER TABLE [dbo].[VMStatistics] CHECK CONSTRAINT [FKVMStatisticsMonitoredSQLServers]
	
	CREATE INDEX [IX_VMStatistics_UTCCollectionDateTime] on [VMStatistics]([UTCCollectionDateTime])

END
ELSE
BEGIN
---upgrade to sqldm 8.5 (Gaurav Karwal): Fixed a bug in the script (START)
	IF (NOT EXISTS(select id from syscolumns where id=OBJECT_ID('VMStatistics') and name='PagePerSecVM' collate database_default))
	BEGIN
		ALTER TABLE VMStatistics ADD [PagePerSecVM] BIGINT NULL
	END
	IF (NOT EXISTS(select id from syscolumns where id=OBJECT_ID('VMStatistics') and name='AvailableByteVm' collate database_default))
	BEGIN
		ALTER TABLE VMStatistics ADD [AvailableByteVm] BIGINT NULL
	END
---upgrade to sqldm 8.5 (Gaurav Karwal): Fixed a bug in the script (END)
END

--------------------------------------------------------------

IF (OBJECT_ID('ESXStatistics') IS NULL)
BEGIN

	create table [dbo].[ESXStatistics] (
		[SQLServerID] int not null,
		[UTCCollectionDateTime] datetime not null,
		[CPUUsage] float null,
		[CPUUsageMHz] int null,
		[MemSwapInRate] bigint null,
		[MemSwapOutRate] bigint null,
		[MemActive] bigint null,
		[MemConsumed] bigint null,
		[MemGranted] bigint null,
		[MemBalooned] bigint null,
		[MemUsage] float null,
		[DiskRead] bigint null,
		[DiskWrite] bigint null, 
		[DiskDeviceLatency] bigint null,
		[DiskKernelLatency] bigint null,
		[DiskQueueLatency] bigint null,
		[DiskTotalLatency] bigint null,
		[DiskUsage] bigint null,
		[NetUsage] bigint null,
		[NetReceived] bigint null,
		[NetTransmitted] bigint null,
		[MemPagePerSec] bigint null,
		[AvailableMemBytes] bigint null
		Constraint [PKESXStatistics] PRIMARY KEY CLUSTERED
		(
			[SQLServerID] ASC,
			[UTCCollectionDateTime] ASC
		)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[ESXStatistics]  WITH CHECK ADD  CONSTRAINT [FKESXStatisticsMonitoredSQLServers] FOREIGN KEY([SQLServerID])
	REFERENCES [dbo].[MonitoredSQLServers] ([SQLServerID])
	ON DELETE CASCADE

	ALTER TABLE [dbo].[ESXStatistics] CHECK CONSTRAINT [FKESXStatisticsMonitoredSQLServers]

	CREATE INDEX [IX_ESXStatistics_UTCCollectionDateTime] on [ESXStatistics]([UTCCollectionDateTime])
END
ELSE
BEGIN
---upgrade to sqldm 8.5 (Gaurav Karwal): Fixed a bug in the script (START)
	IF (NOT EXISTS(select id from syscolumns where id=OBJECT_ID('ESXStatistics') and name='MemPagePerSec' collate database_default))
	BEGIN
		ALTER TABLE ESXStatistics ADD [MemPagePerSec] BIGINT NULL
	END
	IF (NOT EXISTS(select id from syscolumns where id=OBJECT_ID('ESXStatistics') and name='AvailableMemBytes' collate database_default))
	BEGIN
		ALTER TABLE ESXStatistics ADD [AvailableMemBytes] BIGINT NULL
	END
---upgrade to sqldm 8.5 (Gaurav Karwal): Fixed a bug in the script (END)
END

-------------------------------------------------------------------------------------

IF (OBJECT_ID('BaselineTemplates') IS NULL)
BEGIN

CREATE TABLE [BaselineTemplates]
	(
		[TemplateID]  [int] IDENTITY(1,1) NOT NULL,		
		[SQLServerID] [int] NOT NULL,
		[Template]    [nvarchar](1024) NOT NULL,
		[BaselineName] nvarchar(500) NOT NULL CONSTRAINT [DF_BaselineTemplates_BaselineName] DEFAULT 'Default', --[START] SQLdm 10.0 (Gaurav Karwal): for upgrade
		[Active] bit NOT NULL CONSTRAINT [DF_BaselineTemplates_Active] DEFAULT 0	--[START] SQLdm 10.0 (Gaurav Karwal): for upgrade		
		CONSTRAINT [PK_BaselineTemplates] PRIMARY KEY CLUSTERED 
			(
			[TemplateID] ASC
			),	
		CONSTRAINT [FKBaselineServersSQLServers] FOREIGN KEY 
			(
			[SQLServerID]
			)
		REFERENCES [MonitoredSQLServers] 
			(
			[SQLServerID]
			) on delete cascade
	)
	
	CREATE INDEX [IXBaselineTemplates] ON [BaselineTemplates]([SQLServerID]) 
END
ELSE
BEGIN
	--[START] SQLdm 10.0 (Gaurav Karwal): for upgrade
	IF (NOT EXISTS(select id from syscolumns where id = OBJECT_ID('BaselineTemplates') and name='BaselineName' collate database_default))
	BEGIN
		ALTER TABLE [BaselineTemplates] ADD BaselineName nvarchar(500) NOT NULL CONSTRAINT DF_BaselineTemplates_BaselineName DEFAULT 'Default'
	END
	
	IF (NOT EXISTS(select id from syscolumns where id = OBJECT_ID('BaselineTemplates') and name='Active' collate database_default))
	BEGIN
		ALTER TABLE [BaselineTemplates] ADD [Active] bit NOT NULL CONSTRAINT DF_BaselineTemplates_Active DEFAULT 0	
	END
	--[END] SQLdm 10.0 (Gaurav Karwal): for upgrade
END

-------------------------------------------------------------------------------------

IF (OBJECT_ID('BaselineStatistics') IS NULL)
BEGIN

CREATE TABLE [BaselineStatistics]
	(
		[UTCCalculation] [datetime] NOT NULL,
		[SQLServerID]  [int] NOT NULL,
		[TemplateID]   [int] NOT NULL,
		[MetricID]     [int] NOT NULL,
		[Mean]         [decimal] (38, 5) NOT NULL,
		[StdDeviation] [decimal] (38, 5) NOT NULL,
		[Min]          [decimal] (38, 5) NOT NULL,
		[Max]          [decimal] (38, 5) NOT NULL,
		[Count]        [bigint]  NOT NULL,		
		CONSTRAINT [PKBaselineStatistics] PRIMARY KEY CLUSTERED 
			(
				[SQLServerID],
				[UTCCalculation],
				[TemplateID],
				[MetricID]
			),	
		CONSTRAINT [FKBaselineStatisticsSQLServers] FOREIGN KEY 
			(
			[SQLServerID]
			)
		REFERENCES [MonitoredSQLServers] 
			(
			[SQLServerID]
			) on delete cascade	
	)
	
	CREATE INDEX [IXBaselineStatistics] ON [BaselineStatistics]([SQLServerID],[UTCCalculation]) 
END

-------------------------------------------------------------------------------------
-------------------------------------------------------------------------------------

IF (OBJECT_ID('DashboardLayouts') IS NULL)
BEGIN

CREATE TABLE [DashboardLayouts]
(
	[DashboardLayoutID] [int] IDENTITY(1,1) NOT NULL,
	[LoginName] [nvarchar](256) NULL,
	[Name] [nvarchar](128) NOT NULL,
	[LastUpdated] [datetime] NULL,
	[LastViewed] [datetime] NULL,
	[Configuration] [nvarchar](max) NOT NULL,
	[LayoutImage] [varbinary](max) NULL,
	CONSTRAINT [PKDashboardLayouts] PRIMARY KEY CLUSTERED 
	(
		[DashboardLayoutID] ASC
	)
)

	CREATE INDEX [IXDashboardLayouts] ON [DashboardLayouts]([LoginName],[Name]) 
END

-------------------------------------------------------------------------------------

IF (OBJECT_ID('DashboardDefaults') IS NULL)
BEGIN

CREATE TABLE [DashboardDefaults]
(
	[DashboardDefaultID] [int] IDENTITY(1,1) NOT NULL,
	[LoginName] [nvarchar](256) NOT NULL,
	[SQLServerID] [int] NULL,
	[DashboardLayoutID] [int] NOT NULL,
	CONSTRAINT [PKDashboardDefaults] PRIMARY KEY CLUSTERED 
	(
		[DashboardDefaultID] ASC
	)
)

	CREATE INDEX [IXDashboardDefaults] ON [DashboardDefaults]([LoginName],[SQLServerID],[DashboardLayoutID]) 
END

-------------------------------------------------------------------------------------

IF (OBJECT_ID('MetricThresholdInstances') IS NULL)
BEGIN
CREATE TABLE [dbo].[MetricThresholdInstances](
	[InstanceID] [int] IDENTITY(1,1) NOT NULL,
	[InstanceType] [int] NOT NULL,
	[ThresholdInstanceName] [nvarchar](255) NULL,
	[CreationDateTime] [datetime] NULL DEFAULT GETDATE(),
	 CONSTRAINT [PKMetricThresholdInstances] PRIMARY KEY CLUSTERED 
		(
		[InstanceID]
		)
	)

	CREATE INDEX [IXMetricThresholdInstances] ON [MetricThresholdInstances] ([InstanceType],[ThresholdInstanceName])
END

-------------------------------------------------------------------------------------
IF (OBJECT_ID('SQLsafeConnections') IS NULL)
BEGIN
create table SQLsafeConnections (
	RepositoryId		int Identity(1,1) not null,
	FriendlyName		nvarchar(256) null,
	InstanceName		nvarchar(256) not null,
	DatabaseName		nvarchar(128) not null,
	Active				bit not null default(1),
	Deleted				bit not null default(0),
	SecurityMode		bit not null,
	UserName			nvarchar(128) null,
	EncryptedPassword	nvarchar(128) null,
	RegisteredDate		datetime default(getutcdate()),
 CONSTRAINT [PKSQLsafeConnections] PRIMARY KEY ([RepositoryId])
)
END

-------------------------------------------------------------------------------------
IF (OBJECT_ID('MonitoredServerSQLsafeInstance') IS NULL)
BEGIN
	create table MonitoredServerSQLsafeInstance (
		SQLServerID int not null,
		RepositoryId int not null,
		RelatedInstanceId int not null,
		LastBackupActionId int not null,
		LastDefragActionId int not null
	 constraint [PKMonitoredServerSQLsafeInstance] PRIMARY KEY (SQLServerID),
	 constraint [FKMonitoredServerSQLsafeInstanceSQLServerId] foreign key (SQLServerID)
		references [MonitoredSQLServers] ([SQLServerID]) on delete cascade,
	 constraint [FKMonitoredServerSQLsafeInstanceRepositoryId] foreign key (RepositoryId)
		references [SQLsafeConnections] ([RepositoryId]) on delete cascade
	)	
END

-------------------------------------------------------------------------------------

IF  (OBJECT_ID(N'[dbo].[AuditableEvents]') IS NULL)
BEGIN
	CREATE TABLE [dbo].[AuditableEvents](
		[AuditableEventID] [bigint] identity(1,1) NOT NULL,
		[ActionID] [int] NOT NULL,
		[DateTime] [datetime] NOT NULL,
		[Workstation] [nvarchar](256) NOT NULL,
		[WorkstationUser] [nvarchar](256) NOT NULL,
		[SQLUser] [nvarchar](256) NOT NULL,
		[Name] [nvarchar](256) NOT NULL,
		[MetaData] [nvarchar](MAX) NULL,
		[Header] [nvarchar](500) NULL,
		CONSTRAINT [PK_AuditableEvents] PRIMARY KEY CLUSTERED ([AuditableEventID])
	 )
END

-------------------------------------------------------------------------------------

IF  (OBJECT_ID(N'[dbo].[AudtitableActions]') IS NOT NULL)
BEGIN
	drop table AudtitableActions
END

IF  (OBJECT_ID(N'[dbo].[AuditableActions]') IS NULL)
BEGIN
	CREATE TABLE [dbo].[AuditableActions](
		[ActionID] [int] NOT NULL,
		[Name] [nvarchar](128) NOT NULL,
		[SecurityRequired] [smallint] DEFAULT NULL,
		[HeaderTemplate] [nvarchar](256) NULL,
		CONSTRAINT [PK_AudtitableActions] PRIMARY KEY CLUSTERED ([ActionID])	 
	)
END

-------------------------------------------------------------------------------------

IF (OBJECT_ID(N'[dbo].[AlwaysOnAvailabilityGroups]') is null)
BEGIN
	CREATE TABLE [dbo].[AlwaysOnAvailabilityGroups](
        [GroupTopologyId] [bigint] IDENTITY(1,1) NOT NULL,
		[GroupId] [uniqueidentifier] NOT NULL,
		[GroupName] [sysname] NOT NULL,
        [ServerSourceName] [sysname] NOT NULL,
		[ListenerDnsName] [nvarchar](63) NOT NULL,
		[ListenerPort] [int] NOT NULL,
		[ListenerIpAddress] [nvarchar](48) NULL,
        [Active] [bit] NOT NULL DEFAULT 0,
        [Delete] [bit] NOT NULL DEFAULT 1
		CONSTRAINT [PK_AlwaysOnAvailabilityGroups] PRIMARY KEY CLUSTERED ([GroupTopologyId])
	)

END
-------------------------------------------------------------------------------------

IF (OBJECT_ID(N'[dbo].[AlwaysOnReplicas]') IS NULL)
BEGIN
	CREATE TABLE [dbo].[AlwaysOnReplicas](
        [ReplicaTopologyId] [bigint] IDENTITY(1,1) NOT NULL,
		[ReplicaId] [uniqueidentifier] NOT NULL,
		[GroupId] [uniqueidentifier] NOT NULL,
		[SQLServerID] [int] NOT NULL,
        [ServerSourceName] [sysname] NOT NULL,
		[ReplicaName] [nvarchar](256) NOT NULL,
        [ReplicaRole] [int] NOT NULL, -- TODO: Review if it is necesarry
		[FailoverMode] [int] NOT NULL,
		[AvailabilityMode] [int] NOT NULL,
		[PrimaryConnectionMode] [tinyint] NOT NULL,
		[SecondaryConnectionMode] [tinyint] NOT NULL,
        [Active] [bit] NOT NULL DEFAULT 0,
        [Delete] [bit] NOT NULL DEFAULT 1
		CONSTRAINT [PK_AlwaysOnReplicas] PRIMARY KEY CLUSTERED ([ReplicaTopologyId]),
	)
END

-------------------------------------------------------------------------------------

if (OBJECT_ID(N'[dbo].[AlwaysOnDatabases]') IS NULL)
BEGIN
	CREATE TABLE [dbo].[AlwaysOnDatabases](
		[AlwaysOnDatabasesID] [bigint] IDENTITY(1,1) NOT NULL,
		[ReplicaId] [uniqueidentifier] NOT NULL,
		[GroupId] [uniqueidentifier] NOT NULL,
		[GroupDatabaseId] [uniqueidentifier] NOT NULL,
        [ServerSourceName] [sysname] NOT NULL,
		[DatabaseID] [int] NULL,
		[DatabaseName] [sysname] NOT NULL,
        [Delete] [bit] NOT NULL DEFAULT 1
		CONSTRAINT [PK_AlwaysOnDatabases] PRIMARY KEY CLUSTERED ([AlwaysOnDatabasesID]),
	)
END

-------------------------------------------------------------------------------------
IF (OBJECT_ID(N'[dbo].[AlwaysOnStatistics]') IS NULL)
BEGIN
	CREATE TABLE [dbo].[AlwaysOnStatistics](
		[AlwaysOnStatisticsID] [bigint] IDENTITY(1,1) NOT NULL,
		[UTCCollectionDateTime] [datetime] NOT NULL,
		[ReplicaId] [uniqueidentifier] NOT NULL,
		[GroupId] [uniqueidentifier] NOT NULL,
        [GroupDatabaseId] [uniqueidentifier] NOT NULL,
		[DatabaseId] [int] NOT NULL,
		[SQLServerID] [int] NOT NULL,
		[IsFailoverReady] [bit] NOT NULL,
		[SynchronizationState] [tinyint] NOT NULL,
		[SynchronizationHealth] [tinyint] NOT NULL,
		[DatabaseState] [tinyint] NOT NULL,
		[IsSuspended] [bit] NOT NULL,
		[LastHardenedTime] [datetime] NULL,
		[LogSedQueueSize] [bigint] NOT NULL,
		[LogSendRate] [bigint] NOT NULL,
		[RedoQueueSize] [bigint] NOT NULL,
		[RedoRate] [bigint] NOT NULL,
		[ReplicaRole] [int] NULL,
		[OperationalState] [tinyint] NULL,
		[ConnectedState] [tinyint] NULL,
		[SynchronizationHealthAvailabilityReplica] [tinyint] NULL,
		[LastConnectErrorNumber] [int] NULL,
		[LastConnectErrorDescription] [nvarchar](1024) NULL,
		[LastConnectErrorTimestamp] [datetime] NULL,
		[EstimatedDataLossTime] [bigint] NULL,
		[SynchronizationPerformance] [int] NULL,
		[FilestreamSendRate] [bigint] NULL,
		[TimeDeltaInSeconds] [float] NULL,
        [EstimatedRecoveryTime] [int] NULL
		constraint [PK_AlwaysOnStatistics] primary key clustered ([AlwaysOnStatisticsID])
	)
END
-------------------------------------------------------------------------------------
IF (OBJECT_ID(N'[dbo].[DatabaseSizeDateTime]') IS NULL)
BEGIN
	CREATE TABLE  [DatabaseSizeDateTime] 
	(
		[DatabaseID] [int] NOT NULL ,
		[UTCCollectionDateTime] [datetime] NOT NULL,
		CONSTRAINT [PKDatabaseSizeTime] PRIMARY KEY NONCLUSTERED 
			(
			[DatabaseID],
			[UTCCollectionDateTime]
			), 
		CONSTRAINT [FKDatabaseSizeTimeSQLServerDatabaseNames] FOREIGN KEY 
			(
			[DatabaseID]
			) 
		REFERENCES  [SQLServerDatabaseNames] 
			(
			[DatabaseID]
			)  on delete cascade
		)

	insert into DatabaseSizeDateTime
	select DatabaseID, max(UTCCollectionDateTime)
	from DatabaseSize with (nolock)
	group by DatabaseID
END	

-- start -SQLdm 9.0 (Ankit Srivastava): Grooming Time out - creating new table for logging latest grooming status 
IF (OBJECT_ID('LatestGroomingStatus') IS NULL)
BEGIN
CREATE TABLE [dbo].[LatestGroomingStatus]
	(
		[SQLServerID] int NULL, 
		[GroomingRunID] uniqueidentifier NOT NULL, 
		[GroomingDateTimeUTC] DateTime, 
		[Status] smallint,-- failed =0 , succeeded =1, Hung=2
		[LastStatusMessage] nvarchar(250),
		[IsPrimary] bit
		CONSTRAINT [FKLatestGroomingStatusSqlServerID] FOREIGN KEY 
		(
			[SQLServerID]
		) 
		REFERENCES  [MonitoredSQLServers] 
		(
			[SQLServerID]
		) on delete cascade
	)
END
ELSE
BEGIN
	declare @objectID int
	select @objectID=object_id FROM sys.tables where name = 'LatestGroomingStatus'
	-- Drop the constraint IDENTITY for coulmn ID
	if exists(select name from sys.objects where name = 'PKLatestGroomingStatusID') 
	begin
		alter table [LatestGroomingStatus] drop constraint [PKLatestGroomingStatusID]		
	end
	-- Drop the column ID
	if exists(select * from sys.columns where object_id = @objectID and name = 'ID') 
	begin
		alter table [LatestGroomingStatus] drop column [ID]
	end
	
	alter table [LatestGroomingStatus] alter column [SQLServerID] int NULL
END

-------------------------------------------------------------------------------------------------
-- end -SQLdm 9.0 (Ankit Srivastava): Grooming Time out - creating new table for logging latest grooming status 

--START : SQLdm 9.0 (Abhishek Joshi): -CWF Integration  -- Created new table for Web Framework .
-------------------------------------------------------------------------------------
IF (OBJECT_ID('WebFramework') IS NULL)
BEGIN
CREATE TABLE [dbo].[WebFramework](
	[WebFrameworkID] int identity(1,1),
	[HostName] nvarchar(255) ,
	[Port] nvarchar(4),
	[UserName] nvarchar(100),
	[Password] nvarchar(100),
	[InstanceName] nvarchar(200),
	[ProductID] int,
	CONSTRAINT [PKWebFramework] PRIMARY KEY CLUSTERED 
		(
			[WebFrameworkID] ASC
		)
	)

END
ELSE
BEGIN
	IF NOT EXISTS(SELECT name FROM sys.columns WHERE LOWER(name) = LOWER('InstanceName') AND OBJECT_NAME(object_id) = 'WebFramework')
	BEGIN
		ALTER TABLE WebFramework ADD [InstanceName] nvarchar(200);
	END
END

------------------------------------------------------------------------------------------
--END : SQLdm 9.0 (Abhishek Joshi): -CWF Integration  -- Created new table for Web Framework .


--START : SQLdm 9.1 (Abhishek Joshi) -FileGroup Improvements --Created new tables for Disk Drive Statistics and filegroups 

IF (OBJECT_ID('DiskDriveStatistics') IS NULL)
	BEGIN
	CREATE TABLE [dbo].[DiskDriveStatistics] (
		[DiskDriveStatisticsID] int identity NOT NULL,
		[SQLServerID] int NOT NULL,
		[UTCCollectionDateTime] datetime NOT NULL ,
		[DriveName] nvarchar(256) NOT NULL,
		[UnusedSizeKB] dec(18,0),
		[TotalSizeKB] dec(18,0),
		[DiskIdlePercent] bigint,
		[AverageDiskQueueLength] bigint,
		[AverageDiskMillisecondsPerRead] bigint,
		[AverageDiskMillisecondsPerTransfer] bigint,
		[AverageDiskMillisecondsPerWrite] bigint,
		[DiskReadsPerSecond] bigint,
		[DiskTransfersPerSecond] bigint,
		[DiskWritesPerSecond] bigint,
		[DatabaseSizeTime] datetime null,
		CONSTRAINT [PK_DiskDriveStatistics] PRIMARY KEY CLUSTERED
		(
			[DiskDriveStatisticsID]
		)
	)
END


IF (OBJECT_ID('DatabaseFileStatistics') IS NULL)
BEGIN
	CREATE TABLE [dbo].[DatabaseFileStatistics] (
		[FileStatisticsID] int IDENTITY NOT NULL,
		[UTCCollectionDateTime] DateTime NOT NULL,
		[FileID] [int] NOT NULL ,
		[MaxSize] decimal,
		[InitialSize] decimal,
		[UsedSpace] decimal,
		[AvailableSpace] decimal,
		[FreeDiskSpace] decimal,
		[DriveName] nvarchar(256),
		CONSTRAINT [PK_DatabaseFileStatistics_ID] PRIMARY KEY  CLUSTERED 
		(
			[FileStatisticsID]
		), 
		CONSTRAINT [FK_DatabaseFileStatistics_FileID] FOREIGN KEY 
		(
			[FileID]
		) 
		REFERENCES  [DatabaseFiles] 
		(
			[FileID]
		) on delete cascade
	)
END
ELSE
BEGIN
--SQLdm 9.1 Added new Column
	IF (NOT EXISTS(select id from syscolumns where id=OBJECT_ID('DatabaseFileStatistics') and name='UTCCollectionDateTime' collate database_default))
	BEGIN
		ALTER TABLE DatabaseFileStatistics ADD [UTCCollectionDateTime] DateTime NOT NULL
	END
	--SQLdm 9.1.1 Increase the size of the varchar column(DriveName)
	IF (EXISTS(select id from syscolumns where id=OBJECT_ID('DatabaseFileStatistics') and name='DriveName' collate database_default))
	BEGIN
		ALTER TABLE DatabaseFileStatistics ALTER COLUMN [DriveName] nvarchar(256)
	END
END

--END : SQLdm 9.1 (Abhishek Joshi) -FileGroup Improvements --Created new tables for Disk Drive Statistics and filegroups

--START : SQLdm 10.0 (Srishti Purohit) -Custom DashBoard Implementation --Created new table for CustomDashboard 

IF (OBJECT_ID('CustomDashboard') IS NULL)
	BEGIN
		CREATE TABLE [dbo].[CustomDashboard](
		[CustomDashboardId] [bigint] NOT NULL
				 IDENTITY(1, 1),
		[CustomDashboardName] [nvarchar](500) NOT NULL,
		[IsDefaultOnUI] [bit] NULL,
		[UserSID] NVARCHAR(200) NULL,
		[Tags] [varchar](max) NULL,
		
		[RecordCreatedTimestamp] [datetime] NULL,
		[RecordUpdateDateTimestamp] [datetime] NULL
		
		 CONSTRAINT [PK_CustomDashboard] PRIMARY KEY CLUSTERED 
		(
			[CustomDashboardId] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]

	END

--END SQLdm 10.0 (Srishti Purohit) -Custom DashBoard Implementation --Created new table for CustomDashboard 
--START : SQLdm 10.0 (Srishti Purohit) -Custom DashBoard Implementation --Created new table for CustomDashboardWidgetTypes 

IF (OBJECT_ID('CustomDashboardWidgetTypes') IS NULL)
	BEGIN
	
	CREATE TABLE [dbo].[CustomDashboardWidgetTypes](
		[WidgetTypeID] [int] NOT NULL,
		[WidgetType] [nvarchar](250) NOT NULL,
	 CONSTRAINT [PK_dbo.CustomDashboardWidgetTypes] PRIMARY KEY CLUSTERED 
	(
		[WidgetTypeID] ASC
	)
	)

	
END

--END : SQLdm 10.0 (Srishti Purohit) -Custom DashBoard Implementation --Created new table for CustomDashboardWidgetTypes 

--START : SQLdm 10.0 (Srishti Purohit) -Custom DashBoard Implementation --Created new table for CustomDashboardMatchTypes 

IF (OBJECT_ID('CustomDashboardMatchTypes') IS NULL)
	BEGIN
		CREATE TABLE [dbo].[CustomDashboardMatchTypes](
			[MatchID] [int] NOT NULL,
			[MatchType] [nvarchar](250) NOT NULL,
		 CONSTRAINT [PK_dbo.CustomDashboardMatchTypes] PRIMARY KEY CLUSTERED 
		(
			[MatchID] ASC
		))

		
	END
--END : SQLdm 10.0 (Srishti Purohit) -Custom DashBoard Implementation --Created new table for CustomDashboardMatchTypes 

--START : SQLdm 10.0 (Srishti Purohit) -Custom DashBoard Implementation --Created new table for CustomDashboardWidget 

IF (OBJECT_ID('CustomDashboardWidgets') IS NULL)
	BEGIN
CREATE TABLE [dbo].[CustomDashboardWidgets](
	[WidgetID] [bigint] IDENTITY(1,1) NOT NULL,
	[DashboardID] [bigint] NOT NULL,
	[WidgetName] [nvarchar](500) NOT NULL,
	[WidgetTypeID] [int] NOT NULL,
	[MetricID] [int] NOT NULL,
	[MatchId] [int] NOT NULL,
	[RecordCreatedTimestamp] [datetime] NULL,
	[RecordUpdateDateTimestamp] [datetime] NULL,
 CONSTRAINT [PK_dbo.CustomDashboardWidgets] PRIMARY KEY CLUSTERED 
(
	[WidgetID] ASC
),
CONSTRAINT [fk_CustomDashboard] FOREIGN KEY([DashboardID])
REFERENCES [dbo].[CustomDashboard] ([CustomDashboardId]) on delete cascade
,CONSTRAINT [fk_Metric] FOREIGN KEY([MetricID])
REFERENCES [dbo].[MetricInfo] ([Metric]) on delete cascade
,CONSTRAINT [fk_Match] FOREIGN KEY([MatchId])
REFERENCES [dbo].[CustomDashboardMatchTypes] ([MatchID]) on delete cascade
,CONSTRAINT [fk_WidgetType] FOREIGN KEY([WidgetTypeID])
REFERENCES [dbo].[CustomDashboardWidgetTypes] ([WidgetTypeID]) on delete cascade)

END
--END SQLdm 10.0 (Srishti Purohit) -Custom DashBoard Implementation --Created new table for CustomDashboardWidget 

--START : SQLdm 10.0 (Srishti Purohit) -Custom DashBoard Implementation --Created new table for WidgetSourceMapping 

IF (OBJECT_ID('WidgetSourceMapping') IS NULL)
	BEGIN
	
		CREATE TABLE [dbo].[WidgetSourceMapping](
		[WidgetID] [bigint] NOT NULL,
		[SourceServerID] [int] NOT NULL,
		 CONSTRAINT [PK_WidgetSourceMapping] PRIMARY KEY CLUSTERED 
		(
			[WidgetID] ASC,
			[SourceServerID] ASC
		),
		CONSTRAINT [fk_Widget] FOREIGN KEY([WidgetID])
		REFERENCES [dbo].[CustomDashboardWidgets] ([WidgetID]) on delete cascade
		,CONSTRAINT [fk_Source] FOREIGN KEY([SourceServerID])
		REFERENCES [dbo].[MonitoredSQLServers] ([SQLServerID]) on delete cascade
		)
	END
--END : SQLdm 10.0 (Srishti Purohit) -Custom DashBoard Implementation --Created new table for WidgetSourceMapping


--START : SQLdm 10.0 (Srishti Purohit) -Custom DashBoard Implementation --Created new table for [WidgetTagMapping] 

IF (OBJECT_ID('WidgetTagMapping') IS NULL)
	BEGIN
	
		CREATE TABLE [dbo].[WidgetTagMapping](
		[WidgetID] [bigint] NOT NULL,
		[TagId] [int] NOT NULL,
		 CONSTRAINT [PK_WidgetTagMapping] PRIMARY KEY CLUSTERED 
		(
			[WidgetID] ASC,
			[TagId] ASC
		),
		CONSTRAINT [fk_WidgetTag] FOREIGN KEY([WidgetID])
		REFERENCES [dbo].[CustomDashboardWidgets] ([WidgetID]) on delete cascade
		,CONSTRAINT [fk_TagWidget] FOREIGN KEY([TagId])
		REFERENCES [dbo].[Tags] ([Id]) on delete cascade
		)
	END
--END : SQLdm 10.0 (Srishti Purohit) -Custom DashBoard Implementation --Created new table for [WidgetTagMapping]

--START 10.0 SQLdm srishti purohit -- Doctors Table

-- AnalysisConfiguration for Doctor

IF (OBJECT_ID('AnalysisConfiguration') IS NULL)
BEGIN
CREATE TABLE [AnalysisConfiguration](
	[ID] [int] identity(1,1) NOT NULL,
	[MonitoredServerID] [int] NOT NULL,
	[ProductionServer] [bit] NOT NULL,
	[OLTP] [bit] NOT NULL,
	[StartTime] [DATETIME] NULL,
	[Duration] [int] NULL,
	[ScheduledDays] smallint NOT NULL,
	[IncludeDatabase] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[FilterApplication] nvarchar(max) NULL,
	[SchedulingStatus] bit NOT NULL DEFAULT(0) -- SQLDM 10.0 - Praveen Suhalka - Scheduling Status

	CONSTRAINT [ID] PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	)
)
END

-- AnalysisConfigCategories

IF (OBJECT_ID('AnalysisConfigCategories') IS NULL)
BEGIN
CREATE TABLE [AnalysisConfigCategories](
	[AnalysisConfigurationID] [int] NOT NULL,
	[CategoryID] [int] NOT NULL,

		CONSTRAINT [fk_AnalysisConfigurationIDForCategory] FOREIGN KEY([AnalysisConfigurationID])
		REFERENCES [dbo].[AnalysisConfiguration] ([ID]) on delete cascade,
		CONSTRAINT [fk_PrescriptiveRecommendationCategoryID] FOREIGN KEY([CategoryID])
		REFERENCES [dbo].PrescriptiveRecommendationCategory ([CategoryID]) on delete cascade
)
END

--AnalysisConfigBlockedDatabases
IF (OBJECT_ID('AnalysisConfigBlockedDatabases') IS NULL)
BEGIN
CREATE TABLE [AnalysisConfigBlockedDatabases](
	[AnalysisConfigurationID] [int] NOT NULL,
	[DatabaseID] [int] NOT NULL,

		CONSTRAINT [fk_AnalysisConfigurationIDForDB] FOREIGN KEY([AnalysisConfigurationID])
		REFERENCES [dbo].[AnalysisConfiguration] ([ID]) on delete cascade,
		CONSTRAINT [fk_DatabaseID] FOREIGN KEY([DatabaseID])
		
		REFERENCES  [SQLServerDatabaseNames] 
			(
			[DatabaseID]
			)  on delete cascade
)
END
-- AnalysisConfigBlockedRecommendation

IF (OBJECT_ID('AnalysisConfigBlockedRecommendation') IS NULL)
BEGIN
CREATE TABLE [AnalysisConfigBlockedRecommendation](
	[AnalysisConfigurationID] [int] NOT NULL,
	[RecommendationID] nvarchar(10) NOT NULL,

		CONSTRAINT [fk_AnalysisConfigurationIDForRecommendation] FOREIGN KEY([AnalysisConfigurationID])
		REFERENCES [dbo].[AnalysisConfiguration] ([ID]) on delete cascade,
		CONSTRAINT [fk_RecommendationIDForAnalysisConfigBlockedRecommendation] FOREIGN KEY([RecommendationID])
		
		REFERENCES  [PrescriptiveRecommendation] 
			(
			[RecommendationID]
			)  on delete cascade
)
END

--PrescriptiveRecommendationProperty
 
IF (OBJECT_ID('PrescriptiveRecommendationProperty') IS NULL)
BEGIN
	CREATE TABLE [PrescriptiveRecommendationProperty](
		 [ID] [int] identity(1,1) NOT NULL,
		 [RecommendationID] [nvarchar](10) NOT NULL,
		 [PropertyName] [nvarchar](200) NOT NULL,
		 
		CONSTRAINT [pk_PrescriptiveRecommendationPropertyID] PRIMARY KEY CLUSTERED 
		(
			[ID] ASC
		),

		CONSTRAINT [fk_PrescriptiveRecommendationPropertyRecommendation] FOREIGN KEY([RecommendationID])
		
		REFERENCES  [PrescriptiveRecommendation] 
			(
			[RecommendationID]
			)  on delete cascade)
END


--PrescriptiveAnalysisRecommendationProperty 
IF (OBJECT_ID('PrescriptiveAnalysisRecommendationProperty') IS NULL)
BEGIN
	CREATE TABLE [PrescriptiveAnalysisRecommendationProperty](
		 [ID] [int] identity(1,1) NOT NULL,
		 [AnalysisRecommendationID] [int] NOT NULL,
		 [PropertyID] [int] NOT NULL,
		 [Value] [nvarchar](max) NULL,
		 
		CONSTRAINT [pk_PrescriptiveAnalysisRecommendationPropertyID] PRIMARY KEY CLUSTERED 
		(
			[ID] ASC
		),

		CONSTRAINT [fk_PrescriptiveRecommendationPropertyID] FOREIGN KEY([PropertyID])
		
		REFERENCES  [PrescriptiveRecommendationProperty] 
			(
			[ID]
			)  on delete cascade,

		CONSTRAINT [fk_PrescriptiveAnalysisRecommendationID] FOREIGN KEY([AnalysisRecommendationID])
		
		REFERENCES  [PrescriptiveAnalysisRecommendation] 
			(
			[ID]
			)  on delete cascade)
END

--PrescriptiveAnalysisSnapshotValuesPrevious 
-- To support SDR-M16
IF (OBJECT_ID('PrescriptiveAnalysisSnapshotValuesPrevious') IS NULL)
BEGIN
	CREATE TABLE [PrescriptiveAnalysisSnapshotValuesPrevious](
		 [MonitoredServerID] [int] NOT NULL,
		 [ActiveNetworkCards] INT NOT NULL,
         [TotalNetworkBandwidth] BIGINT NOT NULL,
         [AllowedProcessorCount] INT NOT NULL,
         [TotalNumberOfLogicalProcessors] BIGINT NOT NULL,
         [TotalMaxClockSpeed] BIGINT NOT NULL,
         [TotalPhysicalMemory] BIGINT NOT NULL,
         [MaxServerMemory] BIGINT NOT NULL, 
		 [WindowsVersion] nvarchar(100) NULL,
		 [ProductVersion] nvarchar(100) NULL,
		 [SQLVersionString] nvarchar(100) NULL
		)
END


--END 10.0 SQLdm srishti purohit -- Doctors Table

-- START 10.1 SQLdm Srishti Purohit -- Health index cofficient
IF (OBJECT_ID('HealthIndexCofficients') IS NULL)
BEGIN
	/*** New table to make health index global for all instance ***/

	CREATE TABLE [dbo].[HealthIndexCofficients](
		[ID] [int] NOT NULL,
		[HealthIndexCoefficientName] [nvarchar](max) NOT NULL,
		[HealthIndexCoefficientValue] float NOT NULL,
		[UTCLastUpdatedDateTime] [datetime] NOT NULL
	CONSTRAINT [pk_HealthIndexCofficientsName] PRIMARY KEY CLUSTERED 
		(
			[ID] ASC
		)
		)
END

--Upgrade to 10.1 

--SQLdm 10.1 (Srishti Purohit)
--SCOM Alert Response Action
--Adding RuleId column (Barkha Khatri)
IF (OBJECT_ID('SCOMAlertEvent') IS NULL)
BEGIN
	CREATE TABLE [SCOMAlertEvent]
	 (
		 
		 [MetricID] int not null ,
		 [IsInSCOMAsAlert] bit not null DEFAULT 0,
		 [RuleID] [uniqueidentifier] NOT NULL,
		 primary key([RuleID],[MetricID], [IsInSCOMAsAlert]),
		 CONSTRAINT [fk_SCOMMetric] FOREIGN KEY([MetricID])
		 REFERENCES [dbo].[MetricMetaData] ([Metric]) on delete cascade,
		 CONSTRAINT [fk_RuleID] FOREIGN KEY([RuleID])
		 REFERENCES [dbo].[NotificationRules] ([RuleID]) on delete cascade
		 
	 )
 END

 -- removing exsisting health index column from monitoredSQlServer
IF EXISTS(SELECT name FROM sys.columns WHERE LOWER(name) = LOWER('HealthIndexCoefficientForCriticalAlert') AND OBJECT_NAME(object_id) = 'MonitoredSQLServers')
	BEGIN
	
	--First drop default constraint then drop column
	DECLARE @ObjectName NVARCHAR(500)
	SELECT @ObjectName = OBJECT_NAME([default_object_id]) FROM sys.columns
	WHERE [object_id] = OBJECT_ID('dbo.MonitoredSQLServers') AND [name] = 'HealthIndexCoefficientForCriticalAlert';
	SELECT @ObjectName = 'ALTER TABLE dbo.MonitoredSQLServers DROP CONSTRAINT ' + @ObjectName
	EXEC(@ObjectName)
		ALTER TABLE MonitoredSQLServers DROP COLUMN [HealthIndexCoefficientForCriticalAlert]

	SELECT @ObjectName = OBJECT_NAME([default_object_id]) FROM sys.columns
	WHERE [object_id] = OBJECT_ID('dbo.MonitoredSQLServers') AND [name] = 'HealthIndexCoefficientForWarningAlert';
	SELECT @ObjectName = 'ALTER TABLE dbo.MonitoredSQLServers DROP CONSTRAINT ' + @ObjectName
	EXEC(@ObjectName)
		ALTER TABLE MonitoredSQLServers DROP COLUMN [HealthIndexCoefficientForWarningAlert]

	SELECT @ObjectName = OBJECT_NAME([default_object_id]) FROM sys.columns
	WHERE [object_id] = OBJECT_ID('dbo.MonitoredSQLServers') AND [name] = 'HealthIndexCoefficientForInformationalAlert';
	SELECT @ObjectName = 'ALTER TABLE dbo.MonitoredSQLServers DROP CONSTRAINT ' + @ObjectName
	EXEC(@ObjectName)
		ALTER TABLE MonitoredSQLServers DROP COLUMN [HealthIndexCoefficientForInformationalAlert]
END
-- START 10.1 SQLdm Srishti Purohit -- Health index cofficient

--START SQLdm 10.2 Anshika Sharma
--Create table for Storing User session settings
IF (OBJECT_ID('UserSessionSettings') is null)
begin
CREATE TABLE [dbo].[UserSessionSettings](
	[UserID] [nvarchar](100) NOT NULL,
	[Key] [nvarchar](200) NOT NULL,
	[Value] [nvarchar](max) NOT NULL,
 CONSTRAINT [PKUserSessionSettings] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
end
--END SQLdm 10.2 Anshika Sharma -UserSessionSettings create

-- START 10.3 Forecasting Aggregation

IF (OBJECT_ID('DatabaseSizeDateTimeAggregation') is null)
begin
CREATE TABLE [dbo].[DatabaseSizeDateTimeAggregation](
	         [AggregatedDatabaseID] int identity NOT NULL,
            [MinUTCCollectionDateTime] datetime NOT NULL ,
            [MaxUTCCollectionDateTime] datetime NOT NULL 
) 
end
-- SQLDM-29448 (SQLDM 10.3) improving performance for Forecasting Aggregation
IF (not exists(select id from syscolumns where id = object_id('DatabaseSizeDateTimeAggregation') and name = 'DatabaseID' collate database_default)) 
BEGIN
    ALTER TABLE [dbo].[DatabaseSizeDateTimeAggregation] 
	    ADD [DatabaseID] int NOT NULL DEFAULT 0
	ALTER TABLE [dbo].[DatabaseSizeDateTimeAggregation]
	    ALTER COLUMN [AggregatedDatabaseID] bigint
END

--------------------------------------------------------

IF (OBJECT_ID('TableGrowthAggregation') is null)
begin
CREATE TABLE [dbo].[TableGrowthAggregation](
	        [AggregatedTableID] int identity NOT NULL,
            [MinUTCCollectionDateTime] datetime NOT NULL ,
			[MaxUTCCollectionDateTime] datetime NOT NULL ,
			[MinNumberOfRows] bigint,
		    [MaxNumberOfRows] bigint,
		    [TotalNumberOfRows] bigint,
			[MinDataSize] bigint,
		    [MaxDataSize] bigint,
		    [TotalDataSize] bigint,
	        [MinTextSize] bigint,
		    [MaxTextSize] bigint,
		    [TotalTextSize] bigint,
		    [MinIndexSize] bigint,
		    [MaxIndexSize] bigint,
		    [TotalIndexSize] bigint,
		    [MinTimeDeltaInSeconds] bigint,
		   [MaxTimeDeltaInSeconds] bigint,
		   [TotalTimeDeltaInSeconds] bigint
) 
end

-- SQLDM-29448 (SQLDM 10.3) improving performance for Forecasting Aggregation
IF (not exists(select id from syscolumns where id = object_id('TableGrowthAggregation') and name = 'TableID' collate database_default)) 
BEGIN
    ALTER TABLE [TableGrowthAggregation]
	    ADD [TableID] int NOT NULL DEFAULT 0
	ALTER TABLE [dbo].[TableGrowthAggregation]
	    ALTER COLUMN [AggregatedTableID] bigint
END
-------------------------------------------------------

IF (OBJECT_ID('DatabaseSizeAggregation') is null)
begin
CREATE TABLE [dbo].[DatabaseSizeAggregation](
	          [AggregatedDatabaseSizeID] int identity NOT NULL
            ,[DatabaseID] int NOT NULL
			,[MinUTCCollectionDateTime] datetime NOT NULL
			,[MaxUTCCollectionDateTime] datetime NOT NULL
			,[MinDataFileSizeInKilobytes] dec(18,0)
			,[MaxDataFileSizeInKilobytes] dec(18,0)
			,[TotalDataFileSizeInKilobytes] dec(18,0)
			,[DatabaseStatus] int
			,[MinLogFileSizeInKilobytes] dec(18,0)
			,[MaxLogFileSizeInKilobytes] dec(18,0)
			,[TotalLogFileSizeInKilobytes] dec(18,0)
			,[MinDataSizeInKilobytes] dec(18,0)
			,[MaxDataSizeInKilobytes] dec(18,0)
			,[TotalDataSizeInKilobytes] dec(18,0)
			,[MinLogSizeInKilobytes] dec(18,0)
			,[MaxLogSizeInKilobytes] dec(18,0)
			,[TotalLogSizeInKilobytes] dec(18,0)
			,[MinTextSizeInKilobytes] dec(18,0)
			,[MaxTextSizeInKilobytes] dec(18,0)
			,[TotalTextSizeInKilobytes] dec(18,0)
			,[MinIndexSizeInKilobytes] dec(18,0)
			,[MaxIndexSizeInKilobytes] dec(18,0)
			,[TotalIndexSizeInKilobytes] dec(18,0)
			,[MinLogExpansionInKilobytes] dec(18,0)
			,[MaxLogExpansionInKilobytes] dec(18,0)
			,[TotalLogExpansionInKilobytes] dec(18,0)
			,[MinDataExpansionInKilobytes] dec(18,0)
			,[MaxDataExpansionInKilobytes] dec(18,0)
			,[TotalDataExpansionInKilobytes] dec(18,0)
			,[MinPercentLogSpace] dec(18,0)
			,[MaxPercentLogSpace] dec(18,0)
			,[TotalPercentLogSpace] dec(18,0)
			,[MinPercentDataSize] dec(18,0)
			,[MaxPercentDataSize] dec(18,0)
			,[TotalPercentDataSize] dec(18,0)
			,[MinTimeDeltaInSeconds] bigint
		    ,[MaxTimeDeltaInSeconds] bigint
			,[TotalTimeDeltaInSeconds] bigint
			,[MinDatabaseStatisticsTime] datetime
			,[MaxDatabaseStatisticsTime] datetime 
) 
end

-----------------------------------------------------------------

IF (OBJECT_ID('DatabaseFileStatisticsAggregation') is null)
begin
CREATE TABLE [dbo].[DatabaseFileStatisticsAggregation](
	         [AggregatedFileStatisticsID] int identity NOT NULL
			,[MinUTCCollectionDateTime] datetime
			,[MaxUTCCollectionDateTime] datetime
			,[FileID] int
			,[MinMaxSize] dec(18,0)
			,[MaxMaxSize] dec(18,0)
			,[TotalMaxSize] dec(18,0)
		    ,[MinInitialSize] dec(18,0)
			,[MaxInitialSize] dec(18,0)
			,[TotalInitialSize] dec(18,0)
		    ,[MinUsedSpace] dec(18,0)
			,[MaxUsedSpace] dec(18,0)
			,[TotalUsedSpace] dec(18,0)
		    ,[MinAvailableSpace] dec(18,0)
			,[MaxAvailableSpace] dec(18,0)
			,[TotalAvailableSpace] dec(18,0)
		    ,[MinFreeDiskSpace] dec(18,0)
			,[MaxFreeDiskSpace] dec(18,0)
			,[TotalFreeDiskSpace] dec(18,0)
		    ,[DriveName] nvarchar(256) NOT NULL
) 
end

---------------------------------------------------------------

IF  (OBJECT_ID(N'[dbo].[DiskDriveStatisticsAggregation]') IS NULL)
BEGIN
CREATE TABLE [dbo].[DiskDriveStatisticsAggregation](
	        [AggregatedDiskDriveStatisticsID] int identity NOT NULL,
			[SQLServerID] int,
            [MinUTCCollectionDateTime] datetime NOT NULL ,
			[MaxUTCCollectionDateTime] datetime NOT NULL ,
			[DriveName] nvarchar(256) NOT NULL,
			[MinUnusedSizeKB] dec(18,0),
			[MaxUnusedSizeKB] dec(18,0),
			[TotalUnusedSizeKB] dec(18,0),
			[MinTotalzSieKB] dec(18,0),
			[MaxTotalSizeKB] dec(18,0),
			[TotalSizeKBPerDay] dec(18,0),
			[MinDiskIdlePercent] bigint,
			[MaxDiskIdlePercent] bigint,
			[TotalDiskIdlePercent] bigint,
			[MinDiskReadsPerSecond] bigint,
			[MaxDiskReadsPerSecond] bigint,
			[TotalDiskReadsPerSecond] bigint,
			[MinDiskTransferPerSecond] bigint,
			[MaxDiskTransferPerSecond] bigint,
			[TotalTransferPerSecond] bigint,
			[MinDiskWritesPerSecond] bigint,
			[MaxDiskWritesPerSecond] bigint,
			[TotalDiskWritesPerSecond] bigint,
			[MinDatabaseSizeTime] datetime,
			[MaxDatabaseSizeTime] datetime,
			[TotalDatabaseSizeTime] datetime,
			[MinAverageDiskQueueLength] bigint,
		    [MaxAverageDiskQueueLength] bigint,
	        [MinAverageDiskMillisecondsPerRead] bigint,
		    [MaxAverageDiskMillisecondsPerRead] bigint,
	        [MinAverageDiskMillisecondsPerTransfer] bigint,
		    [MaxAverageDiskMillisecondsPerTransfer] bigint,
		    [MinAverageDiskMillisecondsPerWrite] bigint,
		    [MaxAverageDiskMillisecondsPerWrite] bigint
) 
end

-- -- SQLDM-29448 (SQLDM 10.3) improving performance for Forecasting Aggregation
IF (exists(select id from syscolumns where id = object_id('DiskDriveStatisticsAggregation') and name = 'TotalDatabaseSizeTime' collate database_default)) 
BEGIN
    ALTER TABLE [DatabaseFileStatisticsAggregation]
	    ALTER COLUMN [DriveName] nvarchar(256) null
    ALTER TABLE [DiskDriveStatisticsAggregation] 
	    DROP COLUMN [TotalDatabaseSizeTime]
	ALTER TABLE [dbo].[DiskDriveStatisticsAggregation]
	    ALTER COLUMN [AggregatedDiskDriveStatisticsID] bigint
	-- Alter column type to bigint type for aggregation tables - DatabaseFileStatisticsAggregation and DatabaseSizeAggregation
	ALTER TABLE [dbo].[DatabaseFileStatisticsAggregation]
	    ALTER COLUMN [AggregatedFileStatisticsID] bigint
	ALTER TABLE [dbo].[DatabaseSizeAggregation]
	    ALTER COLUMN [AggregatedDatabaseSizeID] bigint
END

-----------------------------------------------------------

IF (OBJECT_ID(N'[dbo].[DatabaseStatisticsAggregation]') IS NULL)
BEGIN

CREATE TABLE [dbo].[DatabaseStatisticsAggregation](
	[AggregatedDatabaseStatisticsID] [bigint] IDENTITY NOT NULL,
	[DatabaseID] [int] NOT NULL,
	[MaxUTCCollectionDateTime] [datetime] NOT NULL,
	[MinUTCCollectionDateTime] [datetime] NOT NULL,
	[DatabaseStatus] [int] NULL,
	[MaxTransactions] [bigint] NULL,
	[MinTransactions] [bigint] NULL,
	[TotalTransactions] [bigint] NULL,
	[MaxLogFlushWaits] [bigint] NULL,
	[MinLogFlushWaits] [bigint] NULL,
	[TotalLogFlushWaits] [bigint] NULL,
	[MaxLogFlushes] [bigint] NULL,
	[MinLogFlushes] [bigint] NULL,
	[TotalLogFlushes] [bigint] NULL,
	[MaxLogKilobytesFlushed] [bigint] NULL,
	[MinLogKilobytesFlushed] [bigint] NULL,
	[TotalLogKilobytesFlushed] [bigint] NULL,
	[MaxLogCacheReads] [bigint] NULL,
	[MinLogCacheReads] [bigint] NULL,
	[TotalLogCacheReads] [bigint] NULL,
	[MaxLogCacheHitRatio] [float] NULL,
	[MinLogCacheHitRatio] [float] NULL,
	[TotalLogCacheHitRatio] [float] NULL,
	[MaxTimeDeltaInSeconds] [float] NULL,
	[MinTimeDeltaInSeconds] [float] NULL,
	[TotalTimeDeltaInSeconds] [float] NULL,
	[MaxNumberReads] [decimal](18, 0) NULL,
	[MinNumberReads] [decimal](18, 0) NULL,
	[TotalNumberReads] [decimal](18, 0) NULL,
	[MaxNumberWrites] [decimal](18, 0) NULL,
	[MinNumberWrites] [decimal](18, 0) NULL,
	[TotalNumberWrites] [decimal](18, 0) NULL,
	[MaxBytesRead] [decimal](18, 0) NULL,
	[MinBytesRead] [decimal](18, 0) NULL,
	[TotalBytesRead] [decimal](18, 0) NULL,
	[MaxBytesWritten] [decimal](18, 0) NULL,
	[MinBytesWritten] [decimal](18, 0) NULL,
	[TotalBytesWritten] [decimal](18, 0) NULL,
	[MaxIoStallMS] [decimal](18, 0) NULL,
	[MinIoStallMS] [decimal](18, 0) NULL,
	[TotalIoStallMS] [decimal](18, 0) NULL,
	[MaxDatabaseSizeTime] [datetime] NULL,
	[MinDatabaseSizeTime] [datetime] NULL,
	[MaxLastBackupDateTime] [datetime] NULL,
	[MinLastBackupDateTime] [datetime] NULL,
	[MaxAverageDataIO] [decimal](18, 4) NULL, --SQLdm 11.0 (Azure Support)
	[MinAverageDataIO] [decimal](18, 4) NULL,
	[MaxAverageLogIO] [decimal](18, 4) NULL,
    [MinAverageLogIMinO] [decimal](18, 4) NULL,
	[MaxMaxWorker] [decimal](18, 4) NULL,
	[MinMaxWorker] [decimal](18, 4) NULL,
	[MaxMaxSession] [decimal](18, 4) NULL,
	[MinMaxSession] [decimal](18, 4) NULL,
	[MaxDatabaseAverageMemoryUsage] [decimal](18, 4) NULL,
	[MinDatabaseAverageMemoryUsage] [decimal](18, 4) NULL,
	[MaxInMemoryStorageUsage] [decimal](18, 4) NULL,
	[MinInMemoryStorageUsage] [decimal](18, 4) NULL,
	[MaxAvgCpuPercent] [decimal](18, 4) NULL, -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
	[MinAvgCpuPercent] [decimal](18, 4) NULL, -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
	[MaxAvgDataIoPercent] [decimal](18, 4) NULL, -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
	[MinAvgDataIoPercent] [decimal](18, 4) NULL, -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
	[MaxAvgLogWritePercent] [decimal](18, 4) NULL, -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
	[MinAvgLogWritePercent] [decimal](18, 4) NULL, -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
	[MaxDtuLimit] [int] NULL, -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
	[MinDtuLimit] [int] NULL, -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
	
	[DatabaseStatisticsID] [bigint] NULL,
	[MaxAzureCloudAllocatedMemory] [decimal](18, 4) NULL,
	[MinAzureCloudAllocatedMemory] [decimal](18, 4) NULL,
	[MaxAzureCloudUsedMemory] [decimal] NULL,
	[MinAzureCloudUsedMemory] [decimal] NULL,
	[MaxAzureCloudStorageLimit] [decimal] NULL,
	[MinAzureCloudStorageLimit] [decimal] NULL,
	[ElasticPool] [varchar] NULL  --SQLdm 11.0 ElasticPool Support
) ON [PRIMARY]

END
Else
--START SQLdm 11.0 - Azure Support
	IF (not exists(select id from syscolumns where id = object_id('DatabaseStatisticsAggregation') and name = 'MaxAverageDataIO' collate database_default)) 
	BEGIN
		ALTER TABLE [DatabaseStatisticsAggregation] 
			ADD
				[MaxAverageDataIO] [decimal](18, 4) NULL, --SQLdm 11.0 (Azure Support)
	            [MinAverageDataIO] [decimal](18, 4) NULL,
	            [MaxAverageLogIO] [decimal](18, 4) NULL,
                [MinAverageLogIMinO] [decimal](18, 4) NULL,
	            [MaxMaxWorker] [decimal](18, 4) NULL,
	            [MinMaxWorker] [decimal](18, 4) NULL,
	            [MaxMaxSession] [decimal](18, 4) NULL,
	            [MinMaxSession] [decimal](18, 4) NULL,
	            [MaxDatabaseAverageMemoryUsage] [decimal](18, 4) NULL,
	            [MinDatabaseAverageMemoryUsage] [decimal](18, 4) NULL,
	            [MaxInMemoryStorageUsage] [decimal](18, 4) NULL,
	            [MinInMemoryStorageUsage] [decimal](18, 4) NULL
	END
-- -- SQLDM-29448 (SQLDM 10.3) improving performance for Forecasting Aggregation
IF (exists(select id from syscolumns where id = object_id('DatabaseStatisticsAggregation') and name = 'DatabaseStatisticsID' collate database_default)) 
BEGIN
    ALTER TABLE [DatabaseStatisticsAggregation] 
	    DROP COLUMN [DatabaseStatisticsID]
	ALTER TABLE [dbo].[DatabaseStatisticsAggregation]
	    ALTER COLUMN [AggregatedDatabaseStatisticsID] bigint
END

IF (not exists(select id from syscolumns where id = object_id('DatabaseStatisticsAggregation') and name = 'MaxAvgCpuPercent' collate database_default)) 
BEGIN
    ALTER TABLE [dbo].[DatabaseStatisticsAggregation]
	    ADD [MaxAvgCpuPercent] [decimal](18, 4) NULL, -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
	[MinAvgCpuPercent] [decimal](18, 4) NULL, -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
	[MaxAvgDataIoPercent] [decimal](18, 4) NULL, -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
	[MinAvgDataIoPercent] [decimal](18, 4) NULL, -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
	[MaxAvgLogWritePercent] [decimal](18, 4) NULL, -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
	[MinAvgLogWritePercent] [decimal](18, 4) NULL, -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
	[MaxDtuLimit] [int] NULL, -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
	[MinDtuLimit] [int] NULL -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
END

IF (not exists(select id from syscolumns where id = object_id('DatabaseStatisticsAggregation') and name = 'MaxAzureCloudAllocatedMemory' collate database_default)) 
BEGIN
    ALTER TABLE [dbo].[DatabaseStatisticsAggregation]
	    ADD 
		[MaxAzureCloudAllocatedMemory] [decimal](18, 4) NULL,
		[MinAzureCloudAllocatedMemory] [decimal](18, 4) NULL,
		[MaxAzureCloudUsedMemory] [decimal](18, 4) NULL,
		[MinAzureCloudUsedMemory] [decimal](18, 4) NULL,
		[MaxAzureCloudStorageLimit] [decimal](18, 4) NULL,
		[MinAzureCloudStorageLimit] [decimal](18, 4) NULL
END

IF (not exists(select id from syscolumns where id = object_id('DatabaseStatisticsAggregation') and name = 'ElasticPool' collate database_default)) 
BEGIN
    ALTER TABLE [dbo].[DatabaseStatisticsAggregation]
	    ADD 
		[ElasticPool] [varchar] NULL  --SQLdm 11.0 ElasticPool Support	
END

----------------------------------------------------

IF (OBJECT_ID(N'[dbo].[AlwaysOnReplicas]') IS NOT NULL)
BEGIN
	IF NOT EXISTS(SELECT * FROM sys.foreign_keys WHERE object_id = object_id(N'[dbo].[fk_AlwaysOnReplicasForServerID]') and parent_object_id = object_id(N'[dbo].[AlwaysOnReplicas]'))
	BEGIN
		ALTER TABLE [dbo].[AlwaysOnReplicas]
			WITH NOCHECK ADD CONSTRAINT [fk_AlwaysOnReplicasForServerID]
			FOREIGN KEY([SQLServerID])
			REFERENCES [dbo].[MonitoredSQLServers] ([SQLServerID])
		ON DELETE CASCADE
		ON UPDATE CASCADE
	END
END

--------------------------------------------------------------------
-- END
-- Start 10.3 DM CWF Integration
IF (OBJECT_ID('AlertsAdvanceFilter') is null)
begin
CREATE TABLE [dbo].[AlertsAdvanceFilter](
	[FilterName] [nvarchar](100) NOT NULL,
	[Config] [nvarchar](max) NOT NULL,
	primary key([FilterName])
)
end
--END 10.3 DM CWF Integration

--START SQLdm 10.3 Jose Luis Barrientos
-- SQLDM-27123
	IF (not exists(select id from syscolumns where id = object_id('MonitoredSQLServers') and name = 'MaintenanceModeOnDemand' collate database_default)) 
	BEGIN
		ALTER TABLE [MonitoredSQLServers] 
			ADD [MaintenanceModeOnDemand] [bit] DEFAULT NULL

	END
--END SQLdm 10.3. Jose Luis Barrientos -MaintenanceModeOnDemand create	


--START NITOR 20-MAR-2019
IF (OBJECT_ID('AlertInstanceTemplate') is null)
BEGIN
CREATE TABLE [dbo].[AlertInstanceTemplate]
(
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SQLServerID] [int] NULL,
	[TemplateID] [int] NULL
	)
END
--END

--START SQLdm 10.5 NITOR - Add columns to support storing service principle details for reading AWS metrics
	IF (not exists(select id from syscolumns where id = object_id('MonitoredSQLServers') and name = 'aws_access_key' collate database_default)) 
	BEGIN
		ALTER TABLE [MonitoredSQLServers]
			ADD [aws_access_key] NVARCHAR (MAX) NULL,
				[aws_secret_key] NVARCHAR (MAX)  NULL,
				[aws_region_endpoint] NVARCHAR (MAX)  NULL
	END
--END SQLdm 10.5 NITOR - Add columns to support storing service principle details for reading AWS metrics


--START SQLdm 10.5 NITOR  Add New Table of AdvanceFilteringAlert
IF (OBJECT_ID('AdvanceFilteringAlert') is null)
CREATE TABLE [dbo].[AdvanceFilteringAlert](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[ServerName] [nvarchar](256) NULL,
	[Metric] [int] NULL,
	[ThresholdType] [varchar](max) NULL,
	[CurrentVoilentionCount] [int] NULL,
	[TotalSnapShot] [int] NULL,
	[DatabaseName][varchar](250) NULL
	)
 --END 


IF (OBJECT_ID('RecommedationClassification') IS NULL)
BEGIN

CREATE TABLE RecommedationClassification(
  RecommendationID VARCHAR(40),
  AWS BIT,
  Azure BIT,
  OnPremisesDb BIT
)

INSERT INTO RecommedationClassification VALUES ('SDR-BP1',1,1,1),('SDR-D1',1,0,1),('SDR-D10',1,0,1),('SDR-D11',1,0,1),('SDR-D12',1,0,1),('SDR-D13',1,0,1),('SDR-D14',1,1,1),('SDR-D15',0,0,1),('SDR-D16',1,1,1),('SDR-D17',1,1,1),('SDR-D18',1,1,1),('SDR-D19',1,1,1),('SDR-D2',1,0,1),('SDR-D20',1,1,1),('SDR-D21',1,1,1),('SDR-D22',1,0,1),('SDR-D23',0,0,1),('SDR-D3',1,1,1),('SDR-D4',0,0,1),('SDR-D5',0,0,1),('SDR-D6',0,0,1),('SDR-D7',1,0,1),('SDR-D8',0,0,1),('SDR-D9',1,0,1),('SDR-DC1',1,0,1),('SDR-DC2',1,0,1),('SDR-DC3',1,0,1),('SDR-DC4',1,1,1),('SDR-DC5',0,0,1),('SDR-DL1',1,1,1),('SDR-DL2',1,1,1),('SDR-I1',1,1,1),('SDR-I10',1,1,1),('SDR-I11',1,1,1),('SDR-I12',1,1,1),('SDR-I13',1,1,1),('SDR-I14',1,1,1),('SDR-I15',1,1,1),('SDR-I16',1,1,1),('SDR-I17',1,1,1),('SDR-I18',1,1,1),('SDR-I19',1,1,1),('SDR-I2',1,1,1),('SDR-I20',1,1,1),('SDR-I21',1,1,1),('SDR-I22',1,1,1),('SDR-I23',1,1,1),('SDR-I24',1,1,1),('SDR-I25',0,1,1),('SDR-I26',0,1,1),('SDR-I27',1,1,1),('SDR-I28',0,1,1),('SDR-I29',0,1,1),('SDR-I3',1,1,1),('SDR-I30',1,1,1),('SDR-I31',1,1,1),('SDR-I4',1,1,1),('SDR-I5',1,1,1),('SDR-I6',1,1,1),('SDR-I7',1,1,1),('SDR-I8',1,1,1),('SDR-I9',1,1,1),('SDR-LR1',1,0,1),('SDR-M1',0,0,1),('SDR-M10',0,0,1),('SDR-M11',0,0,1),('SDR-M12',0,0,1),('SDR-M13',1,1,1),('SDR-M14',1,1,1),('SDR-M15',1,1,1),('SDR-M16',0,0,1),('SDR-M17',1,0,1),('SDR-M18',1,0,1),('SDR-M19',1,0,1),('SDR-M2',0,0,1),('SDR-M20',0,0,1),('SDR-M21',0,0,1),('SDR-M22',0,0,1),('SDR-M23',0,0,1),('SDR-M24',0,0,1),('SDR-M25',0,0,1),('SDR-M26',0,0,1),('SDR-M27',0,0,1),('SDR-M28',1,1,1),('SDR-M29',1,0,1),('SDR-M3',1,0,1),('SDR-M30',0,0,1),('SDR-M31',0,0,1),('SDR-M32',0,0,1),('SDR-M33',0,0,1),('SDR-M4',1,0,1),('SDR-M5',1,0,1),('SDR-M6',1,0,1),('SDR-M7',1,1,1),('SDR-M8',0,0,1),('SDR-M9',0,0,1),('SDR-N1',0,0,1),('SDR-N2',0,0,1),('SDR-N3',0,0,1),('SDR-N4',0,0,1),('SDR-N5',0,0,1),('SDR-N6',1,0,1),('SDR-N7',1,1,1),('SDR-N8',0,0,1),('SDR-N9',0,0,1),('SDR-OT1',1,1,1),('SDR-P1',0,0,1),('SDR-P10',0,0,1),('SDR-P11',1,0,1),('SDR-P12',1,0,1),('SDR-P13',0,0,1),('SDR-P14',0,0,1),('SDR-P15',0,0,1),('SDR-P16',0,0,1),('SDR-P17',1,0,1),('SDR-P18',0,0,1),('SDR-P19',0,0,1),('SDR-P2',0,0,1),('SDR-P20',0,0,1),('SDR-P21',0,0,1),('SDR-P22',0,0,1),('SDR-P23',0,0,1),('SDR-P3',0,0,1),('SDR-P4',0,0,1),('SDR-P5',0,0,1),('SDR-P6',1,0,1),('SDR-P7',1,1,1),('SDR-P8',1,1,1),('SDR-P9',1,1,1),('SDR-Q1',1,1,1),('SDR-Q10',1,1,1),('SDR-Q11',1,1,1),('SDR-Q12',1,1,1),('SDR-Q13',1,1,1),('SDR-Q14',1,1,1),('SDR-Q15',1,1,1),('SDR-Q16',1,1,1),('SDR-Q17',1,1,1),('SDR-Q18',1,1,1),('SDR-Q19',1,1,1),('SDR-Q2',1,1,1),('SDR-Q20',1,1,1),('SDR-Q21',1,1,1),('SDR-Q22',1,1,1),('SDR-Q23',1,1,1),('SDR-Q24',1,1,1),('SDR-Q25',1,1,1),('SDR-Q26',1,1,1),('SDR-Q27',1,1,1),('SDR-Q28',1,1,1),('SDR-Q29',1,1,1),('SDR-Q3',1,1,1),('SDR-Q30',1,1,1),('SDR-Q31',1,1,1),('SDR-Q32',1,1,1),('SDR-Q33',1,1,1),('SDR-Q34',1,1,1),('SDR-Q35',1,1,1),('SDR-Q36',1,1,1),('SDR-Q37',0,0,1),('SDR-Q38',0,0,1),('SDR-Q39',0,1,1),('SDR-Q4',1,1,1),('SDR-Q40',0,1,1),('SDR-Q41',0,1,1),('SDR-Q42',0,1,1),('SDR-Q43',1,1,1),('SDR-Q44',1,0,1),('SDR-Q45',0,0,1),('SDR-Q46',1,1,1),('SDR-Q47',1,1,1),('SDR-Q48',1,1,1),('SDR-Q49',1,1,1),('SDR-Q5',1,1,1),('SDR-Q50',1,1,1),('SDR-Q6',1,1,1),('SDR-Q7',0,0,1),('SDR-Q8',1,1,1),('SDR-Q9',1,1,1),('SDR-R1',1,0,1),('SDR-R2',0,0,1),('SDR-R3',1,1,1),('SDR-R4',1,0,1),('SDR-R5',0,0,1),('SDR-R6',0,0,1),('SDR-R7',0,0,1),('SDR-R8',0,0,1),('SDR-S1',0,0,1),('SDR-S10',1,0,1),('SDR-S2',0,0,1),('SDR-S3',1,0,1),('SDR-S4',1,0,1),('SDR-S5',0,0,1),('SDR-S6',1,0,1),('SDR-S7',0,0,1),('SDR-S8',0,0,1),('SDR-S9',1,1,1),('SDR-SC1',1,0,1),('SDR-SC2',0,0,1),('SDR-SC3',1,0,1),('SDR-SC4',1,0,1),('SDR-SC5',1,0,1),('SDR-SC6',1,0,1),('SDR-SC7',1,0,1),('SDR-SC8',0,0,1),('SDR-SC9',1,0,1),('SDR-SF1',0,0,1),('SDR-UG1',0,0,1),('SDR-VL1',0,0,1),('SDR-W1',1,0,1),('SDR-W10',1,0,1),('SDR-W2',1,0,1),('SDR-W3',1,0,1),('SDR-W4',1,0,1),('SDR-W5',1,0,1),('SDR-W6',1,0,1),('SDR-W7',1,0,1),('SDR-W8',1,0,1),('SDR-W9',1,0,1),('SDR-D24',1,0,0),('SDR-DC6',1,0,0),('SDR-DC7',1,1,1),('SDR-DC8',0,1,0),('SDR-DC9',1,1,1),('SDR-DC10',1,1,1),('SDR-DC11',1,1,0),('SDR-DC12',0,0,1),('SDR-DC13',0,0,1),('SDR-M34',1,0,0);

INSERT INTO RecommedationClassification VALUES ('SDR-M35',1,0,0),('SDR-R9',0,0,1),('SDR-R10',0,0,1),('SDR-R11',0,0,1),('SDR-S11',0,1,0),('SDR-S12',0,1,0),('SDR-S13',0,1,0),('SDR-S14',0,1,0),('SDR-S15',0,1,0),('SDR-P24',1,0,0),('SDR-SC10',1,0,0);

END
--START SQLdm 11 Add New Table of AzureApplication
IF (OBJECT_ID('AzureApplication') is null)
CREATE TABLE [dbo].[AzureApplication](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](MAX) NOT NULL,
	[TenantId] [nvarchar](MAX) NOT NULL,
	[ClientId] [nvarchar](MAX) NOT NULL,
	[Secret] [nvarchar](MAX) NOT NULL,
	[Description] [nvarchar](MAX) NULL,
	CONSTRAINT [PK_AzureApplication] PRIMARY KEY CLUSTERED ( [ID] ASC )
	)
 --END

--START SQLdm 11 Add New Table of AzureSubscription
IF (OBJECT_ID('AzureSubscription') is null)
CREATE TABLE [dbo].[AzureSubscription](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[SubscriptionId] [nvarchar](MAX) NOT NULL,
	[Description] [nvarchar](MAX) NULL
	CONSTRAINT [PK_AzureSubscription] PRIMARY KEY CLUSTERED ( [ID] ASC )
	)

--START SQLdm 11 Add New Table of AzureResourceGroup
IF (OBJECT_ID('AzureApplicationProfile') is null)
CREATE TABLE [dbo].[AzureApplicationProfile](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](MAX) NOT NULL,
	[AzureSubscriptionId] [bigint] NOT NULL,
	[AzureApplicationId] [bigint] NOT NULL,
	[Description] [nvarchar](MAX) NULL
	CONSTRAINT [PK_AzureApplicationProfile] PRIMARY KEY CLUSTERED ( [ID] ASC ),
	CONSTRAINT [fk_AzureApplicationProfile_AzureSubscriptionId] FOREIGN KEY([AzureSubscriptionId])
		REFERENCES [dbo].[AzureSubscription]([ID]) ON DELETE CASCADE,
	CONSTRAINT [fk_AzureApplicationProfile_AzureApplicationId] FOREIGN KEY([AzureApplicationId])
		REFERENCES [dbo].[AzureApplication]([ID]) ON DELETE CASCADE,
	)

--START SQLdm 11 Add New Table of AzureResourceGroup
IF (OBJECT_ID('AzureResource') is null)
CREATE TABLE [dbo].[AzureResource](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](MAX) NOT NULL,
	[Uri] [nvarchar](MAX) NOT NULL,
	[Type] [nvarchar](MAX) NOT NULL,
	[AzureApplicationProfileId] [bigint] NOT NULL,
	CONSTRAINT [PK_AzureResource] PRIMARY KEY CLUSTERED ( [ID] ASC ),
	CONSTRAINT [fk_AzureResource_AzureApplicationProfileId] FOREIGN KEY([AzureApplicationProfileId])
		REFERENCES [dbo].[AzureApplicationProfile]([ID]) ON DELETE CASCADE
	)

--START SQLdm 11 Add New Table of AzureProfile
IF (OBJECT_ID('AzureProfile') is null)
CREATE TABLE [dbo].[AzureProfile](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[AzureApplicationProfileId] [bigint] NOT NULL,
	[SQLServerID] [int] NOT NULL,
	[Description] [nvarchar](MAX) NULL,
	CONSTRAINT [PK_AzureProfile] PRIMARY KEY CLUSTERED ( [ID] ASC ),
	CONSTRAINT [fk_AzureProfile_AzureApplicationProfileId] FOREIGN KEY([AzureApplicationProfileId])
		REFERENCES [dbo].[AzureApplicationProfile]([ID]) ON DELETE CASCADE,
	CONSTRAINT [fk_AzureProfile_SQLServerID] FOREIGN KEY ([SQLServerID])
		REFERENCES [dbo].[MonitoredSQLServers]([SQLServerID]) ON DELETE CASCADE
	)
 --END