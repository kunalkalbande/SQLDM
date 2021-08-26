if (object_id('Grooming.p_GroomDatabaseNames') is not null)
begin
drop procedure Grooming.p_GroomDatabaseNames
end
go
create procedure [Grooming].[p_GroomDatabaseNames]
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
	@BlockName = 'DatabaseNames'		


if (GetUTCDate() > @TimeoutTime)           
	raiserror (N'Timeout in %s', 11, 1,@BlockName);

     
begin try
	create table #AllList(DatabaseID int primary key clustered)
	insert into #AllList
		select DatabaseID 
		from SQLServerDatabaseNames
	
	create table #KeepList(DatabaseID int primary key clustered)
	insert into	#KeepList(DatabaseID)
		select DatabaseID
		from dbo.DatabaseFiles
		where DatabaseID is not null
		union
		select DatabaseID
		from dbo.DatabaseStatistics
		where DatabaseID is not null
		union
		select DatabaseID
		from dbo.DatabaseSize
		where DatabaseID is not null
		union
		select DatabaseID
		from dbo.SQLServerTableNames
		where DatabaseID is not null
		union
		select DatabaseID
		from dbo.MirroringParticipants
		where DatabaseID is not null
		union
		select DatabaseID
		from dbo.MirroringStatistics
		where DatabaseID is not null
		union
		select DatabaseID
		from dbo.ActiveWaitStatistics
		where DatabaseID is not null
		union
		select DatabaseID
		from dbo.QueryMonitorStatistics
		where DatabaseID is not null
		union
		select DatabaseID
		from dbo.BlockingSessionStatistics
		where DatabaseID is not null
		union
		select DatabaseID
		from dbo.DeadlockProcesses
		where DatabaseID is not null
		union
		select DatabaseID
		from dbo.QuerySignatureAggregation

--	SQLDM-30896 Fix to remove those database names whose IsDelete flag is set to true.
	delete from #KeepList where DatabaseID in 
	(
		select DatabaseID from SQLServerDatabaseNames where IsDeleted = 1
	)
		
	create table #DeleteList(DatabaseID int primary key clustered, deleteGroup int default(0))
	insert into	
		#DeleteList (DatabaseID)
		select allList.DatabaseID
		from #AllList allList
		left outer merge join #KeepList keepList
		on allList.DatabaseID = keepList.DatabaseID 
		where keepList.DatabaseID is null 

	
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
		
		delete top (@RecordsToDelete) dbNames
		from	
			dbo.SQLServerDatabaseNames dbNames 
			inner merge join #DeleteList d 
			on dbNames.DatabaseID = d.DatabaseID
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
			
			
			
			if @msg like '%DatabaseFiles%'
					delete #DeleteList
					from #DeleteList d
					inner merge join
						dbo.DatabaseFiles df on
							d.DatabaseID = df.DatabaseID;
							
			if @msg like '%DatabaseStatistics%'
					delete #DeleteList
					from #DeleteList d
					inner merge join
						dbo.DatabaseStatistics ds on
							d.DatabaseID = ds.DatabaseID;			


			if @msg like '%DatabaseSize%'
					delete #DeleteList
					from #DeleteList d
					inner merge join
						dbo.DatabaseSize ds on
							d.DatabaseID = ds.DatabaseID;			
							
			if @msg like '%SQLServerTableNames%'
					delete #DeleteList
					from #DeleteList d
					inner merge join
						dbo.SQLServerTableNames tn on
							d.DatabaseID = tn.DatabaseID;						
							
			if @msg like '%MirroringParticipants%'
					delete #DeleteList
					from #DeleteList d
					inner merge join
						dbo.MirroringParticipants mp on
							d.DatabaseID = mp.DatabaseID;				
							
			if @msg like '%MirroringStatistics%'
					delete #DeleteList
					from #DeleteList d
					inner merge join
						dbo.MirroringStatistics ms on
							d.DatabaseID = ms.DatabaseID;														


			if @msg like '%ActiveWait%'
					delete #DeleteList
					from #DeleteList d
					inner merge join
						dbo.ActiveWaitStatistics aws on
							d.DatabaseID = aws.DatabaseID;
			
			
			if @msg like '%QueryMonitor%'
					delete #DeleteList
					from #DeleteList d
					inner merge join
						dbo.QueryMonitorStatistics qms on
							d.DatabaseID = qms.DatabaseID;
			
			if @msg like '%DeadlockProcesses%'
					delete #DeleteList
					from #DeleteList d
					inner merge join
						dbo.DeadlockProcesses dp on
							d.DatabaseID = dp.DatabaseID;
			
			if @msg like '%BlockingSession%'
					delete #DeleteList
					from #DeleteList d
					inner merge join
						dbo.BlockingSessionStatistics bss on
							d.DatabaseID = bss.DatabaseID;
							
		
			if @msg like '%QuerySignature%'
					delete #DeleteList
					from #DeleteList d
					inner merge join
						dbo.QuerySignatureAggregation qsa on
							d.DatabaseID = qsa.DatabaseID;								
		
						
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
