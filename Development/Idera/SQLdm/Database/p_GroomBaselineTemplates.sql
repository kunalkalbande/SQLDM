if (object_id('Grooming.p_GroomBaselineTemplates') is not null)
begin
drop procedure Grooming.p_GroomBaselineTemplates
end
go
create procedure [Grooming].[p_GroomBaselineTemplates]
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
	@BlockName = 'BaselineTemplates'		


if (GetUTCDate() > @TimeoutTime)           
	raiserror (N'Timeout in %s', 11, 1,@BlockName);

     
begin try
	create table #AllList(TemplateID int primary key clustered)
	insert into #AllList
		select TemplateID 
		from BaselineTemplates
	
	create table #KeepList(TemplateID int primary key clustered)
	insert into	#KeepList(TemplateID)
		select TemplateID
		from BaselineTemplates a
		where TemplateID = (select TemplateID from BaselineTemplates b where b.SQLServerID = a.SQLServerID AND b.Active = 1) -- SQLdm10.0.2 (Srishti) -- Fixing active Baselines getting deleted
			  or TemplateID in (select distinct(TemplateID) from BaselineStatistics)
		
	create table #DeleteList(TemplateID int primary key clustered, deleteGroup int default(0))
	insert into	
		#DeleteList (TemplateID)
		select allList.TemplateID
		from #AllList allList
		left outer merge join #KeepList keepList
		on allList.TemplateID = keepList.TemplateID 
		where keepList.TemplateID is null 

	
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
		
		delete top (@RecordsToDelete) templates
		from	
			dbo.BaselineTemplates templates 
			inner merge join #DeleteList d 
			on templates.TemplateID = d.TemplateID
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
			
			if @msg like '%BaselineTemplates%'
					delete #DeleteList
					from #DeleteList d
					inner merge join
						dbo.BaselineTemplates t on
							d.TemplateID = t.TemplateID
						
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
