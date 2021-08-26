if (object_id('p_GetQueryMonitorUpgradeData') is not null)
begin
drop procedure p_GetQueryMonitorUpgradeData
end
go

declare @command nvarchar(3000), @RowsRemaining bigint

select 
	@RowsRemaining = sum(rows)
from
	sys.partitions
where
	object_id = object_id('QueryMonitor')
	and index_id < 2 
	
if (isnull(@RowsRemaining,0) <= 0)
begin
set @command = '
create procedure p_GetQueryMonitorUpgradeData
as
begin
	return
end'

end
else
begin

set @command = '
create procedure p_GetQueryMonitorUpgradeData
as
begin

	declare 
		@UpgradeRowCount int,
		@UpgradeTimeLimit int,
		@UpgradeTimeOfDay datetime,
		@UpgradeRowCountDefault int,
		@UpgradeTimeLimitDefault int,
		@UpgradeTimeOfDayDefault datetime,
		@CurrentTime datetime,
		@StartTime datetime,
		@rc int

	set @UpgradeRowCountDefault = 1000
	set @UpgradeTimeLimitDefault = 180
	set @UpgradeTimeOfDayDefault = ''2010-01-01 00:00:00''
	set @CurrentTime = getdate()

	select @UpgradeRowCount = isnull(Internal_Value,@UpgradeRowCountDefault) from RepositoryInfo where [Name] = ''QMUpgradeRowCount''	
	select @UpgradeTimeLimit = isnull(Internal_Value,@UpgradeTimeLimitDefault) from RepositoryInfo where [Name] = ''QMUpgradeTimeLimit''	
	select @UpgradeTimeOfDay = isnull(cast(Character_Value as datetime),@UpgradeTimeOfDayDefault) from RepositoryInfo where [Name] = ''QMUpgradeTimeOfDay''	
	select @StartTime = cast(Character_Value as datetime) from RepositoryInfo where [Name] = ''QMUpgradeStartTime''
	
	set @UpgradeRowCount = isnull(@UpgradeRowCount,@UpgradeRowCountDefault)
	set @UpgradeTimeLimit = isnull(@UpgradeTimeLimit,@UpgradeTimeLimitDefault)
	set @UpgradeTimeOfDay = isnull(@UpgradeTimeOfDay,@UpgradeTimeOfDayDefault)
	set @UpgradeTimeOfDay = cast(@UpgradeTimeOfDay as float) - floor(cast(@UpgradeTimeOfDay as float))


	if (@StartTime is null)
	begin
		set @StartTime = @CurrentTime	
		insert into RepositoryInfo(Name,Character_Value)
			select 
				''QMUpgradeStartTime'',
				convert(nvarchar(100),@StartTime,121)
	end

	if (@StartTime > @CurrentTime)
	begin
		select ''Waiting''
		return
	end

	if (dateadd(mi,@UpgradeTimeLimit,@StartTime) < @CurrentTime)
	begin
		declare @NextStartTime datetime
		set @NextStartTime = dateadd(dd,1,cast((floor(cast(@CurrentTime as float)) + cast(@UpgradeTimeOfDay as float)) as datetime))
				
		update RepositoryInfo set Character_Value = convert(nvarchar(100),@NextStartTime ,121)
			where Name = ''QMUpgradeStartTime''
		
		select ''Waiting''
		return
	end

	set rowcount @UpgradeRowCount

	select 
		SQLServerID,
		UTCCollectionDateTime,
		DatabaseID,
		isnull(CompletionTime,UTCCollectionDateTime) as CompletionTime,
		DurationMilliseconds,
		CPUMilliseconds,
		Reads,
		Writes,
		SqlUserName,
		ClientComputerName,
		ApplicationName,
		StatementType,
		StatementText,
		Spid
	from 
		QueryMonitor
	where
		len(StatementText) > 0
		and isnull(DeleteFlag,0) = 0


	set @rc = @@rowcount

	delete from RepositoryInfo where Name = ''QMUpgrading''

	if @rc > 0
	begin
		insert into RepositoryInfo(Name,Character_Value)
			select 
				''QMUpgrading'',
				 convert(nvarchar(100),getdate() ,121)
	end
	

	
end
--'
end
execute(@command)

