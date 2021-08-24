SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

Create procedure [p_GetMirroringMetrics]
(
	@database_name		sysname = null	-- if null update all mirrored databases
)
as
begin
	set nocount on
	if (is_srvrolemember(N'sysadmin') <> 1 )
    begin
		raiserror(21089, 16, 1)
		return 1
	end
	if ( db_name() != N'msdb' )
	begin
		raiserror(32045, 16, 1, N'p_GetMirroringMetrics')
		return 1
	end

	declare		@retcode	int

		--
		-- Check if the database specified exists 
		--
		if not exists (select * from master.sys.databases where name = @database_name)
		begin
			raiserror(15010, 16, 1, @database_name)
			return 1
		end
		-- 
		-- Check to see if it is mirrored
		--
		if (select mirroring_guid from master.sys.database_mirroring where database_id = db_id(@database_name)) is null 
		begin
			raiserror(32039, 16, 1, @database_name)
			return 1
		end

	declare
		@database_id			smallint,
		@role					bit,
		@status					tinyint,
		@witness_status			tinyint,
		@log_flush_rate			bigint ,
		@send_queue_size		bigint ,
		@send_rate				bigint ,
		@redo_queue_size		bigint ,
		@redo_rate				bigint ,
		@transaction_delay		bigint ,
		@transactions_per_sec	bigint ,
		@time					datetime ,
		@local_time				datetime

	declare
		@retention_period		int,
		@oldest_date			datetime

		set @database_id = DB_ID( @database_name )

-- To select the correct perf counter, we need the instance name.
		declare 
			@perf_instance1		nvarchar(256),
			@perf_instance2		nvarchar(256),
			@instance			nvarchar(128)

		select @instance = convert( nvarchar,  serverproperty(N'instancename'))
		if @instance is null
		begin
			set @instance = N'SQLServer'
		end
		else
		begin
			set @instance = N'MSSQL$' + @instance
		end

		set @perf_instance1 = left(@instance, len(@instance)) + N':Database Mirroring'
		set @perf_instance2 = left(@instance, len(@instance)) + N':Databases'

-- 1. Pull out the perf counters
-- 2. Pull out the information from sys.database_mirroring

		declare @perfcounters table(
			counter_name		nchar(128),
			cntr_value			bigint
		)

		insert into @perfcounters select counter_name, cntr_value from sys.dm_os_performance_counters where
			(object_name = @perf_instance1 or object_name = @perf_instance2 ) and 
			instance_name = @database_name and
			counter_name IN (N'Log Send Queue KB', N'Log Bytes Sent/sec', N'Redo Queue KB', N'Redo Bytes/sec', N'Transaction Delay', N'Log Bytes Flushed/sec', N'Transactions/sec')
		-- This is the nasty part where I put everything into a single variable
		-- TO DO select all perfcounters for all databases so that you only need to access them once.
		select @role = (mirroring_role - 1),
			@status = mirroring_state, 
			@witness_status = mirroring_witness_state,
			@failover_lsn = mirroring_failover_lsn from sys.database_mirroring where database_id = @database_id
		-- TO DO: when doing the join, store the database id.
		select @log_flush_rate = cntr_value from @perfcounters where counter_name = N'Log Bytes Flushed/sec'
		select @send_queue_size = cntr_value from @perfcounters where counter_name = N'Log Send Queue KB'
		select @send_rate = cntr_value from @perfcounters where counter_name = N'Log Bytes Sent/sec'
		select @redo_queue_size = cntr_value from @perfcounters where counter_name = N'Redo Queue KB'
		select @redo_rate = cntr_value from @perfcounters where counter_name = N'Redo Bytes/sec'
		select @transaction_delay = cntr_value from @perfcounters where counter_name = N'Transaction Delay'
		select @transactions_per_sec = cntr_value from @perfcounters where counter_name = N'Transactions/sec'
		set @time = getutcdate()
		set @local_time = getdate()

		
return 0
end
