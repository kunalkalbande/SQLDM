if (object_id('Grooming.p_GroomSQLSignatures') is not null)
begin
drop procedure Grooming.p_GroomSQLSignatures
end
go
create procedure [Grooming].[p_GroomSQLSignatures]
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
	@BlockName = 'SQLSignatures'		
			

if (GetUTCDate() > @TimeoutTime)           
	raiserror (N'Timeout in %s', 11, 1,@BlockName);
       
begin try
	create table #AllList(SQLSignatureID int primary key clustered)
	insert into #AllList
		select SQLSignatureID 
		from SQLSignatures
	
	create table #KeepList(SQLSignatureID int primary key clustered)
	insert into	#KeepList(SQLSignatureID)
		select SQLSignatureID
		from dbo.ActiveWaitStatistics
		where SQLSignatureID is not null
		union 
		select SQLSignatureID
		from dbo.QueryMonitorStatistics
		where SQLSignatureID is not null
		union 
		select SQLSignatureID
		from dbo.DeadlockProcesses
		where SQLSignatureID is not null
		union 
		select SQLSignatureID
		from BlockingSessionStatistics
		where SQLSignatureID is not null
		union 
		select SQLSignatureID
		from dbo.QuerySignatureAggregation
		where SQLSignatureID is not null
		
	create table #DeleteList(SQLSignatureID int primary key clustered, deleteGroup int default(0))
	insert into	
		#DeleteList (SQLSignatureID)
		select allList.SQLSignatureID
		from #AllList allList
		left outer merge join #KeepList keepList
		on allList.SQLSignatureID = keepList.SQLSignatureID 
		where keepList.SQLSignatureID is null 

	
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
		
		delete top (@RecordsToDelete) sigs
		from	
			dbo.SQLSignatures sigs 
			inner merge join #DeleteList d 
			on sigs.SQLSignatureID = d.SQLSignatureID
		where
			d.deleteGroup = @deleteGroup
		
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
							d.SQLSignatureID = aws.SQLSignatureID;
			
			if @msg like '%QueryMonitor%'
					delete #DeleteList
					from #DeleteList d
					inner merge join
						dbo.QueryMonitorStatistics qms on
							d.SQLSignatureID = qms.SQLSignatureID;
			
			if @msg like '%DeadlockProcesses%'
					delete #DeleteList
					from #DeleteList d
					inner merge join
						dbo.DeadlockProcesses dp on
							d.SQLSignatureID = dp.SQLSignatureID;
			
			if @msg like '%BlockingSession%'
					delete #DeleteList
					from #DeleteList d
					inner merge join
						dbo.BlockingSessionStatistics bss on
							d.SQLSignatureID = bss.SQLSignatureID;
		
			if @msg like '%QuerySignature%'
					delete #DeleteList
					from #DeleteList d
					inner merge join
						dbo.QuerySignatureAggregation qsa on
							d.SQLSignatureID = qsa.SQLSignatureID;		
							
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


