if (object_id('Grooming.p_GroomSQLStatements') is not null)
begin
drop procedure Grooming.p_GroomSQLStatements
end
go
create procedure [Grooming].[p_GroomSQLStatements]
(
@run_id uniqueidentifier,
@Sequence int out,
@TimeoutTime datetime,
@RecordsToDelete int = 1000
)
as
begin

declare @RowsAffected int
declare @RC int
declare @BlockName nvarchar(256)
declare @ErrorMessage nvarchar(2048)

select 
	@RowsAffected = 0, 
	@RC = 0, 
	@BlockName = 'SQLStatements'		

	
if (GetUTCDate() > @TimeoutTime)           
	raiserror (N'Timeout in %s', 11, 1,@BlockName);
	
      
begin try
	create table #AllList(SQLStatementID int primary key clustered)
	insert into #AllList
		select SQLStatementID 
		from SQLStatements
	
	create table #KeepList(SQLStatementID int primary key clustered)
	insert into	#KeepList(SQLStatementID)
		select SQLStatementID
		from ActiveWaitStatistics
		where SQLStatementID is not null
		union 
		select SQLStatementID
		from QueryMonitorStatistics
		where SQLStatementID is not null
		union 
		select SQLStatementID
		from DeadlockProcesses
		where SQLStatementID is not null
		union 
		select SQLStatementID
		from BlockingSessionStatistics
		where SQLStatementID is not null
		
	create table #DeleteList(SQLStatementID int primary key clustered, deleteGroup int default(0))
	insert into	
		#DeleteList (SQLStatementID)
		select allList.SQLStatementID
		from #AllList allList
		left outer merge join #KeepList keepList
		on allList.SQLStatementID = keepList.SQLStatementID 
		where keepList.SQLStatementID is null 

	
	drop table #AllList	
	drop table #KeepList
	
end try
begin catch

	set @ErrorMessage = @BlockName + ERROR_MESSAGE()
	exec Grooming.p_LogGroomingAction  @run_id, @Sequence out,@ErrorMessage, @RC, null

end catch
	
 
declare @deleteGroup int
declare @msg nvarchar(2048)
set @deleteGroup = 0
set @RowsAffected = @RecordsToDelete
	
if (select count(*) from #DeleteList) > 0
		begin		
	while @RowsAffected > 0
	begin

	begin try

		set @deleteGroup = @deleteGroup + 1

		update top (@RecordsToDelete) #DeleteList
		set deleteGroup = @deleteGroup
		where deleteGroup = 0
		
		if @@rowcount = 0
		begin
			select @deleteGroup = min(deleteGroup) from #DeleteList
			if @deleteGroup is not null
				begin
					set @msg = 'Retrying Group ' + cast(@deleteGroup as nvarchar(10)) 
					exec Grooming.p_LogGroomingAction  @run_id, @Sequence out,@msg, @RC, null
				end
		end
		
		delete top (@RecordsToDelete) stmts
		from	
			dbo.SQLStatements stmts 
			inner merge join #DeleteList d 
			on stmts.SQLStatementID = d.SQLStatementID
		where
			d.deleteGroup = @deleteGroup

		--SQLdm 9.0 (Ankit Srivastava) -- Query Plan Collection -- Grooming of Query Plans - start
		--delete top (@RecordsToDelete) plans
		--from
		--	dbo.SQLQueryPlans plans
		--	inner merge join #DeleteList d 
		--	on plans.SQLStatementID = d.SQLStatementID
			
		--where
		--	d.deleteGroup = @deleteGroup
		--SQLdm 9.0 (Ankit Srivastava) -- Query Plan Collection -- Grooming of Query Plans - end

		set @RowsAffected = @@ROWCOUNT
		set @RC = @RC + @RowsAffected
			
		delete from #DeleteList 
			where deleteGroup = @deleteGroup
			
	end try
	begin catch

		if	error_number() = 547
		begin
			
			set @msg = error_message();
			
			if @msg like '%ActiveWait%'
					delete #DeleteList
					from #DeleteList d
					inner merge join
						dbo.ActiveWaitStatistics aws on
							d.SQLStatementID = aws.SQLStatementID;
			
			if @msg like '%QueryMonitor%'
					delete #DeleteList
					from #DeleteList d
					inner merge join
						dbo.QueryMonitorStatistics qms on
							d.SQLStatementID = qms.SQLStatementID;
			
			if @msg like '%DeadlockProcesses%'
					delete #DeleteList
					from #DeleteList d
					inner merge join
						dbo.DeadlockProcesses dp on
							d.SQLStatementID = dp.SQLStatementID;
			
			if @msg like '%BlockingSession%'
					delete #DeleteList
					from #DeleteList d
					inner merge join
						dbo.BlockingSessionStatistics bss on
							d.SQLStatementID = bss.SQLStatementID;
		
							
			set @msg = 'Conflict detected: ' + cast(@@rowcount as nvarchar(10)) + ' rows affected. Will Retry Group ' + cast(@deleteGroup as nvarchar(10)) + ' (' + @msg + ')'
			exec Grooming.p_LogGroomingAction  @run_id, @Sequence out,@msg, @RC, null
		end
		else
		begin
			set @ErrorMessage = @BlockName + ERROR_MESSAGE()
			exec Grooming.p_LogGroomingAction  @run_id, @Sequence out,@ErrorMessage, @RC, null
			return @RC;
		end
	end catch


	end
end

drop table #DeleteList

exec Grooming.p_LogGroomingAction @run_id, @Sequence out, @BlockName, @RC, null

return @RC

end




