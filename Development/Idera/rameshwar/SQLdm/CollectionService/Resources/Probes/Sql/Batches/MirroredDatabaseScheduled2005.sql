set nocount on

use msdb 
declare @dbname sysname, -- Name of database
@guid uniqueidentifier,
@partner_instance nvarchar(128),		--Partner instance
@mirroring_role int,     -- 1 = Principal, 2 = Mirror
@mirroring_state int,	 -- 0 = Suspended, 1 = Disconnected, 2 = Synchronizing, 3 = Pending Failover, 4 = Synchronized
@witness_state int,		 -- 1 = Connected, 2 = Disconnected
@safety_level int,       -- 0 = Unknown, 1 = Off\asynch, 2 = Full\synch
@witness_name nvarchar(128),
@partner_name nvarchar(128),
@connection_timeout int

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
	@end_of_log_lsn			numeric(25,0),
	@failover_lsn			numeric(25,0),
	@average_delay			int,
	@local_time				datetime
		
declare
	@role2					bit,
	@status2				tinyint,
	@witness_status2		tinyint,
	@log_flush_rate2		bigint ,
	@send_queue_size2		bigint ,
	@send_rate2				bigint ,
	@redo_queue_size2		bigint ,
	@redo_rate2				bigint ,
	@transaction_delay2		bigint ,
	@transactions_per_sec2	bigint ,
	@time2					datetime ,
	@end_of_log_lsn2		numeric(25,0),
	@failover_lsn2			numeric(25,0),
	@diff_time				bigint,
	@local_time2			datetime,
	@time_update_window		datetime
declare
	@time_behind			datetime,
	@temp_lsn				numeric(25,0),
	@time_cutoff			datetime,
	@rows_to_return			int				
declare
	@retention_period		int,
	@oldest_date			datetime
	
declare @results table(
	database_name			sysname,	-- Name of database
	role					tinyint,	-- 1 = Principal, 2 = Mirror
	mirroring_state			tinyint,	-- 0 = Suspended, 1 = Disconnected, 2 = Synchronizing, 3 = Pending Failover, 4 = Synchronized
	witness_status			tinyint,	-- 1 = Connected, 2 = Disconnected
	log_generation_rate		int null,	-- in kb / sec
	unsent_log				int,		-- in kb
	send_rate				int null,	-- in kb / sec
	unrestored_log			int,		-- in kb
	recovery_rate			int null,	-- in kb / sec
	transaction_delay		int null,	-- in ms
	transactions_per_sec	int null,	-- in trans / sec
	average_delay			int,		-- in ms
	time_recorded			datetime,
	time_behind				datetime,
	local_time				datetime	-- Added for UI		
)

declare mirrored_db_cursor cursor local for 			
	--This is executed once for every database that is being monitored, and against each instance
	--This is the sql for populating the realtime screen
	select d.name, 
	dm.mirroring_guid, 
	dm.mirroring_partner_instance, 
	dm.mirroring_role, 
	dm.mirroring_state, 
	dm.mirroring_witness_state,
	dm.mirroring_safety_level, 
	dm.mirroring_witness_name,
	dm.mirroring_partner_name,
	dm.mirroring_connection_timeout
	from sys.database_mirroring dm join sys.databases d on (dm.database_id=d.database_id) 
	where mirroring_guid is not null

	open mirrored_db_cursor

		fetch next from mirrored_db_cursor
		into @dbname, @guid, @partner_instance, 
		@mirroring_role, @mirroring_state, 
		@witness_state, @safety_level, 
		@witness_name, @partner_name, @connection_timeout

		while @@fetch_status = 0
		begin

		   select 'Database' = @dbname,
		   'LocalInstance' = SERVERPROPERTY('ServerName'), 
		   'PartnerInstance' = @partner_instance, 
		   'WitnessName' = @witness_name,
		   'PartnerName' = @partner_name, 
		   'Guid' = @guid, 
		   'Safety Level' = @safety_level,
		   'Role' = @mirroring_role,
		   'mirroring_state' = @mirroring_state,
		   'WitnessState' = @witness_state
		   
			--use msdb
			---This script fetches all available metric where sp_dbmonitorresults failed with an ugly exception
			--This needs to go in the scheduled mirroring probe
			--followed by sys.sp_dbmmonitorupdate [servername] in a cursor to try and get the history updated.
			declare @database_name nvarchar(512)
			select @database_name = @dbname
			set nocount on
			delete from @results
			
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
				@failover_lsn = mirroring_failover_lsn
				from sys.database_mirroring where database_id = @database_id
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
				
			IF IS_SRVROLEMEMBER('dbm_monitor') = 1 
			begin
			select @role2 = role, 
			@status2 = status,
			@witness_status2 = witness_status,
			@log_flush_rate2 = log_flush_rate,
			@send_queue_size2 = send_queue_size,
			@send_rate2 = send_rate,
			@redo_queue_size2 = redo_queue_size,
			@redo_rate2 = redo_rate,
			@transaction_delay2 = transaction_delay,
			@transactions_per_sec2 = transactions_per_sec,
			@time2=time,
			@end_of_log_lsn2 = end_of_log_lsn,
			@failover_lsn2 = failover_lsn,
			@local_time2 = local_time
			from msdb.dbo.dbm_monitor_data 
			where database_id = @database_id  and time = (select MAX(time) from  msdb.dbo.dbm_monitor_data where database_id = @database_id)
			end
			
			set @diff_time = datediff( second, @time2, @time )
			if (@diff_time = 0)
			begin
				set @log_flush_rate = 0
				set @send_rate =      0
				set @redo_rate =      0
				set @transactions_per_sec = 0
				set @transaction_delay = 0
			end
			else if (@role != @role2)	-- if the role has changed then the rate counters are meaningless
			begin
				set @log_flush_rate = null
				set @send_rate =      null
				set @redo_rate =      null
				set @transactions_per_sec = null
				set @transaction_delay = null
			end
			else
			begin
				if (@log_flush_rate < @log_flush_rate2 )
				begin
					set @log_flush_rate = null
				end
				else 
				begin
					set @log_flush_rate = (@log_flush_rate - @log_flush_rate2 ) / ( @diff_time * 1024)	-- This is in kilobytes
				end

				if (@send_rate < @send_rate2)
				begin
					set @send_rate = null
				end
				else
				begin
					set @send_rate =      (@send_rate - @send_rate2)/ ( @diff_time * 1024 )
				end

				if (@redo_rate < @redo_rate2)
				begin
					set @redo_rate = null
				end
				else
				begin
					set @redo_rate =      (@redo_rate - @redo_rate2)/ ( @diff_time * 1024 )
				end

				if (@transactions_per_sec < @transactions_per_sec2 )
				begin
					set @transactions_per_sec = null
				end
				else
				begin
					set @transactions_per_sec = (@transactions_per_sec - @transactions_per_sec2 ) / @diff_time
				end

				if (@transaction_delay < @transaction_delay2)
				begin
					set @transaction_delay = null
				end
				else
				begin
					set @transaction_delay = (@transaction_delay - @transaction_delay2 ) / @diff_time
				end
			end
				
			if (@transactions_per_sec = 0)
			begin
				set @average_delay = 0
			end
			else
			begin
				set @average_delay = @transaction_delay / @transactions_per_sec
			end
					
			--Get time_behind straight from dbm_monitor_data
			if ((@failover_lsn > @end_of_log_lsn) and (@end_of_log_lsn <> 0) )
			begin
			IF IS_SRVROLEMEMBER('dbm_monitor') = 1
			begin
			select top(1) @time_behind = time from msdb.dbo.dbm_monitor_data 
				where database_id = @database_id order by time desc
			end
			end
			else
			begin	
			  if ( (@failover_lsn = @end_of_log_lsn) or (@end_of_log_lsn = 0) )	-- I've seen cases after a failover or the system just start up 
			   begin										-- where the failover_lsn does not catch up to the end_of_log_lsn
			   	   set @time_behind = @time				-- until some log is generated.  we'll see how this works.
			   end
			   else
			   begin
			   IF IS_SRVROLEMEMBER('dbm_monitor') = 1
			   begin
				select top(1) @time_behind = time from msdb.dbo.dbm_monitor_data 
					where end_of_log_lsn <= @failover_lsn 
						and database_id = @database_id order by time desc		
			   end
			   end						
			end
					insert @results 
				values (
					@database_name,
					@role + 1,
					@status,
					@witness_status,
					@log_flush_rate,
					@send_queue_size,
					@send_rate,
					@redo_queue_size,
					@redo_rate,
					@transaction_delay,
					@transactions_per_sec,
					@average_delay,
					@time,
					@time_behind,
					@local_time
				)	
				select * from @results
						
			   --This proc is no use to us because it returns nothing if it does not update the table
			   --and sometimes the update of the table failes due to being unable to reconcile lsn's
			   --exec sys.sp_dbmmonitorresults @database_name=@dbname, @mode = 0, @update_table = 0
			   
			   fetch next from mirrored_db_cursor into @dbname, @guid, @partner_instance, 
			   @mirroring_role, @mirroring_state, 
			   @witness_state, @safety_level, 
			   @witness_name, @partner_name, @connection_timeout
			end
		close mirrored_db_cursor
	deallocate mirrored_db_cursor

select 'endMirrorMonitoring'