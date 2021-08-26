if (object_id('p_GetWaitCategory') is not null)
begin
drop procedure p_GetWaitCategory
end
go
create procedure p_GetWaitCategory
	@WaitType varchar(120),
	@ReturnID int output
as
begin

	declare @WaitCategory nvarchar(120), @WaitCategoryID int

	select @WaitCategory = 
	case
		when @WaitType = 'BAD_PAGE_PROCESS' then 'Excluded' 
		when @WaitType = 'BROKER_EVENTHANDLER' then 'Excluded' 
		when @WaitType = 'BROKER_RECEIVE_WAITFOR' then 'Excluded' 
		when @WaitType = 'BROKER_TASK_STOP' then 'Excluded' 
		when @WaitType = 'BROKER_TO_FLUSH' then 'Excluded' 
		when @WaitType = 'BROKER_TRANSMITTER' then 'Excluded' 
		when @WaitType = 'CHECKPOINT_QUEUE' then 'Excluded' 
		when @WaitType = 'CLR_SEMAPHORE' then 'Excluded' 
		when @WaitType = 'DBMIRROR_EVENTS_QUEUE' then 'Excluded' 
		when @WaitType = 'KSOURCE_WAKEUP' then 'Excluded' 
		when @WaitType = 'LAZYWRITER_SLEEP' then 'Excluded' 
		when @WaitType = 'LOGMGR_QUEUE' then 'Excluded' 
		when @WaitType = 'MISCELLANEOUS' then 'Excluded' 
		when @WaitType = 'ONDEMAND_TASK_QUEUE' then 'Excluded' 
		when @WaitType = 'REQUEST_FOR_DEADLOCK_SEARCH' then 'Excluded' 
		when @WaitType = 'RESOURCE_QUEUE' then 'Excluded' 
		when @WaitType = 'SLEEP_SYSTEMTASK' then 'Excluded' 
		when @WaitType = 'SLEEP_TASK' then 'Excluded' 
		when @WaitType = 'SQLTRACE_BUFFER_FLUSH' then 'Excluded' 
		when @WaitType = 'WAITFOR'  then 'Excluded' 
		when @WaitType = 'CHKPT' then 'Excluded'
		when @WaitType = 'FT_IFTS_SCHEDULER_IDLE_WAIT' then 'Excluded'
		when @WaitType = 'ASYNC_IO_COMPLETION' then 'I/O'
		when @WaitType = 'CXPACKET' then 'Other'
		when @WaitType = 'EXCHANGE' then 'Other'
		when @WaitType = 'PAGESUPP' then 'Other'
		when @WaitType = 'ASYNC_DISKPOOL_LOCK' then 'Other'
		when @WaitType = 'CURSOR' then 'Other'
		when @WaitType = 'PSS_CHILD' then 'Cursor'
		when @WaitType = 'DISKIO_SUSPEND' then 'Backup'
		when @WaitType = 'IO_COMPLETION' then 'I/O'
		when @WaitType = 'LOGBUFFER' then 'Transaction Log'
		when @WaitType = 'LOGMGR' then 'Transaction Log'
		when @WaitType = 'RESOURCE_QUERY_SEMAPHORE_COMPILE' then 'Query Compilation'
		when @WaitType = 'RESOURCE_SEMAPHORE' then 'Memory'
		when @WaitType = 'TEMPOBJ' then 'Other'
		when @WaitType = 'SOS_RESERVEDMEMBLOCKLIST' then 'Memory'
		when @WaitType = 'SOS_SCHEDULER_YIELD' then 'Other'
		when @WaitType = 'THREADPOOL' then 'Other'
		when @WaitType = 'WRITELOG' then 'Transaction Log'
		when @WaitType = 'XE_DISPATCHER_WAIT' then 'Excluded'
		when @WaitType = 'XE_TIMER_EVENT' then 'Excluded'
		when @WaitType = 'CMEMTHREAD' then 'Memory'
		when @WaitType = 'XACTLOCKINFO' then 'Lock'
		when @WaitType = 'ASYNC_DISKPOOL_LOCK' then 'I/O'
		when @WaitType = 'ASYNC_NETWORK_IO ' then 'Other'
		when @WaitType = 'BACKUP' then 'Backup'
		when @WaitType = 'FILESTREAM_WORKITEM_QUEUE' then 'Excluded'
		when @WaitType = 'SP_SERVER_DIAGNOSTICS_SLEEP' then 'Excluded'
		when @WaitType = 'DISPATCHER_QUEUE_SEMAPHORE' then 'Excluded'
		when @WaitType = 'FT_IFTSHC_MUTEX' then 'Excluded'
		when @WaitType like 'BACKUP%' then 'Backup'
		when @WaitType like 'XE_%' then 'Other'
		when @WaitType like 'SLEEP%' then 'Other'
		when @WaitType like 'PREEMPTIVE_%' then 'Other'
		when @WaitType like 'RESOURCE_SEMAPHORE%' then 'Memory'
		when @WaitType like 'LCK_%' then 'Lock'	
		when @WaitType like 'LATCH%' then 'Non-Page Latch'
		when @WaitType like 'PAGEIOLATCH%' then 'I/O'
		when @WaitType like 'TRAN_MARKLATCH%' then 'Non-Page Latch'
		when @WaitType like 'PAGELATCH%' then 'Non-I/O Page Latch'
		when @WaitType like 'BROKER%' then 'Other'
		else 'Other'
	end

	select @WaitCategoryID = CategoryID
	from [WaitCategories]
	where [Category] = @WaitCategory

	if @WaitCategoryID is null
	begin
		insert into [WaitCategories]
		(
			[Category]
		)
		values
		(
			@WaitCategory
		)

		select @WaitCategoryID = CategoryID
		from [WaitCategories]
		where [Category] = @WaitCategory
	end

	select @ReturnID = @WaitCategoryID

end

