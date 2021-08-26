if (object_id('p_GetTopDatabaseApplications') is not null)
begin
drop procedure p_GetTopDatabaseApplications
end
go
-- @OrderBy
--	0 - CPU
--	1 - Reads
--	2 - Writes
create procedure [dbo].p_GetTopDatabaseApplications
	@SQLServerID int,
	@DatabaseName sysname = null,
	@StartDateTime datetime = null,
	@EndDateTime datetime = null,
	@UtcOffset int = null,
	@MinReads bigint = null,
	@MinWrites bigint = null,
	@MinCPU bigint = null,
	@TopN bigint = 10,
	@OrderBy tinyint = 0
as
begin
set ansi_warnings off
set ansi_nulls off

if @DatabaseName is null or len(@DatabaseName) = 0
	set @DatabaseName = '%'

if @UtcOffset is null
	set @UtcOffset = datediff(mi,getutcdate(),getdate())

declare @DatabaseIds table (Id int, DatabaseName sysname)

insert into @DatabaseIds
select isnull(DatabaseID,-1), DatabaseName
from
	SQLServerDatabaseNames (nolock)
where
	SQLServerID = @SQLServerID
	and DatabaseName like @DatabaseName

set rowcount @TopN

select -- top @TopN
	DatabaseName,
	ApplicationName,
	SumCPU = sum(SumCPU),
	SumReads = sum(SumReads),
	SumWrites = sum(SumWrites),
	SumEvents = sum(SumEvents),
	AvgDuration = sum(TotalDuration) / nullif(sum(SumEvents),0)
from
(
select 
	DatabaseName,
	ApplicationNameID,
	SumCPU = sum(CPUMilliseconds),
	SumReads = sum(Reads),
	SumWrites = sum(Writes),
	SumEvents = count(*),
	TotalDuration = sum(DurationMilliseconds)
from
	@DatabaseIds d 
	join dbo.QueryMonitorStatistics qm (nolock) on qm.DatabaseID = d.Id
where
	SQLServerID = @SQLServerID and
	isnull(@StartDateTime,'1 Jan 1900') <= StatementUTCStartTime and
	isnull(@EndDateTime,'1 Jan 2200') >= case when DurationMilliseconds < 2147483648 then dateadd(ms, DurationMilliseconds,  StatementUTCStartTime) else dateadd(s, DurationMilliseconds / 1000,  StatementUTCStartTime) end
group by
	DatabaseName,
	ApplicationNameID
union all
select 
	DatabaseName,
	ApplicationNameID,
	SumCPU = sum(TotalCPUMilliseconds),
	SumReads = sum(TotalReads),
	SumWrites = sum(TotalWrites),
	SumEvents = sum(QueryMonitorOccurrences),
	TotalDuration = sum(TotalDurationMilliseconds)
from
	@DatabaseIds d 
	join dbo.QuerySignatureAggregation qm (nolock) on qm.DatabaseID = d.Id
where
	SQLServerID = @SQLServerID and
	isnull(@StartDateTime,'1 Jan 1900') <= EventUTCStartTime and
	isnull(@EndDateTime,'1 Jan 2200') >= EventUTCStartTime
group by
	DatabaseName,
	ApplicationNameID
) as data
inner join ApplicationNames an
on data.ApplicationNameID = an.ApplicationNameID
group by
	DatabaseName,
	ApplicationName
having
	sum(SumReads) >= isnull(@MinReads,0) and
	sum(SumWrites) >= isnull(@MinWrites,0) and
	sum(SumCPU) >= isnull(@MinCPU,0)
order by
	case 
		when @OrderBy = 0 then avg(SumCPU)
		when @OrderBy = 1 then avg(SumReads)
		when @OrderBy = 2 then avg(SumWrites)
		else  count(*)
	end 
	desc
end

Go

grant EXECUTE on p_GetTopDatabaseApplications to [SQLdmConsoleUser]

