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
END

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


if (select Internal_Value from RepositoryInfo where Name = 'SQLVersionAtUpgrade') > 8 and
	exists(select id from syscolumns where id =  object_id('QueryMonitor') and name = 'StatementText' collate database_default and length > -1)
begin
	execute('alter table QueryMonitor alter column StatementText nvarchar(max)')
end

GO

if (object_id('p_GetQueryMonitorStatements') is not null)
begin
drop procedure p_GetQueryMonitorStatements
end
go
-- @XmlDocument - SQLServerID AllowedDate parameters in XML format.
--  XmlDocument must contain at least one SQLServer and one date range.
-- @Interval - Granularity of calculation:
--	0 - Minutes
--	1 - Hours
--	2 - Days
--	3 - Months
--	4 - Years
create procedure [dbo].p_GetQueryMonitorStatements
	@SQLServerID int,
	@StartDate datetime = null,
	@EndDate datetime = null,
	@StartTime datetime = null,
	@EndTime datetime = null,
	@Interval int = 1,
	@IncludeSQLStatements bit = 1,
	@IncludeStoredProcedures bit = 1,
	@IncludeSQLBatches bit = 1,
	@ApplicationNameExcludeFilter nvarchar(255) = null,
	@ApplicationNameIncludeFilter nvarchar(255) = null,
	@ClientComputerNameExcludeFilter nvarchar(255) = null,
	@ClientComputerNameIncludeFilter nvarchar(255) = null,
	@DatabaseNameExcludeFilter nvarchar(255) = null,
	@DatabaseNameIncludeFilter nvarchar(255) = null,
	@NtUserExcludeFilter nvarchar(255) = null,
	@NtUserIncludeFilter nvarchar(255) = null,
	@SqlUserNameExcludeFilter nvarchar(255) = null,
	@SqlUserNameIncludeFilter nvarchar(255) = null,
	@SqlTextExcludeFilter nvarchar(4000) = null,
	@SqlTextIncludeFilter nvarchar(4000) = null,
	@DurationFilter bigint = 0,
	@UtcOffset int = 0,
	@SignatureMode bit = 0,
	@CaseInsensitive bit = 0
as
begin
set ansi_warnings off
set ansi_nulls off


declare 
	@StartDateNumber float,
	@StartTimeNumber float,
	@AbsoluteStartDate datetime,
	@EndDateNumber float,
	@EndTimeNumber float,
	@AbsoluteEndDate datetime

if (@StartTime is null)	set @StartTime = '00:00:00 AM'
if (@EndTime is null) set @EndTime = '11:59:59 PM'

set @StartTime = cast(cast(@StartTime as float) - floor(cast (@StartTime as float)) as datetime)
set @EndTime = cast(cast(@EndTime as float) - floor(cast (@EndTime as float)) as datetime)

declare @IntermediateTable table(DatabaseID int,DatabaseName nvarchar(255))

declare @OutputTable table(
	DatabaseName nvarchar(255),
	StartTime datetime,
	CompletionTime datetime,
	DurationMilliseconds bigint,
	CPUMilliseconds bigint,
	Reads bigint,
	Writes bigint,
	NtUserName nvarchar(255),
	SqlUserName nvarchar(255),
	ClientComputerName nvarchar(255),
	ApplicationName nvarchar(255),
	StatementType int,
	StatementText nvarchar(4000),
	Occurrances bigint
	)

insert into @IntermediateTable(DatabaseID,DatabaseName)
	select DatabaseID,DatabaseName 
	from 
		[SQLServerDatabaseNames] d
	where 
		d.SQLServerID = @SQLServerID
		and (@DatabaseNameExcludeFilter is null or DatabaseName not like @DatabaseNameExcludeFilter collate SQL_Latin1_General_CP1_CI_AS)
		and (DatabaseName like coalesce(@DatabaseNameIncludeFilter,'%') collate SQL_Latin1_General_CP1_CI_AS)

if @SignatureMode = 0
begin
insert into @OutputTable
select
	d.DatabaseName,
	StartTime = case when DurationMilliseconds < 2147483648 then dateadd(ms, -DurationMilliseconds,  [CompletionTime]) else dateadd(s, -DurationMilliseconds / 1000,  [CompletionTime]) end,
	CompletionTime,
	DurationMilliseconds,
	CPUMilliseconds,
	Reads,
	Writes,
	NtUserName,
	SqlUserName,
	ClientComputerName,
	ApplicationName,
	StatementType,
	ltrim(StatementText),
	1
from
	QueryMonitor q 
	right join @IntermediateTable d
	on d.DatabaseID = q.DatabaseID
where
	q.SQLServerID = @SQLServerID
	and cast(floor(cast (dateadd(hh, @UtcOffset, q.CompletionTime) as float)) as datetime) >= (coalesce(@StartDate,cast(floor(cast (dateadd(hh, @UtcOffset, q.CompletionTime) as float)) as datetime)))
	and cast(floor(cast (dateadd(hh, @UtcOffset, q.CompletionTime) as float)) as datetime) <= (coalesce(@EndDate,cast(floor(cast (dateadd(hh, @UtcOffset, q.CompletionTime) as float)) as datetime)))
	and (cast(cast(dateadd(hh, @UtcOffset,CompletionTime) as float) - floor(cast (dateadd(hh, @UtcOffset,CompletionTime) as float)) as datetime)) >= coalesce(@StartTime,(cast(cast(dateadd(hh, @UtcOffset,CompletionTime) as float) - floor(cast (dateadd(hh, @UtcOffset,CompletionTime) as float)) as datetime)))
	and (cast(cast(dateadd(hh, @UtcOffset,CompletionTime) as float) - floor(cast (dateadd(hh, @UtcOffset,CompletionTime) as float)) as datetime)) <= coalesce(@EndTime,(cast(cast(dateadd(hh, @UtcOffset,CompletionTime) as float) - floor(cast (dateadd(hh, @UtcOffset,CompletionTime) as float)) as datetime)))
	and q.DurationMilliseconds >= coalesce(@DurationFilter,q.DurationMilliseconds)
	and (@ApplicationNameExcludeFilter is null or isnull(q.ApplicationName,'') not like @ApplicationNameExcludeFilter collate SQL_Latin1_General_CP1_CI_AS)
	and isnull(q.ApplicationName,'') like coalesce(@ApplicationNameIncludeFilter,'%'  collate SQL_Latin1_General_CP1_CI_AS)
	and (@ClientComputerNameExcludeFilter is null or isnull(q.ClientComputerName,'') not like @ClientComputerNameExcludeFilter  collate SQL_Latin1_General_CP1_CI_AS)
	and isnull(q.ClientComputerName,'') like coalesce(@ClientComputerNameIncludeFilter,'%'  collate SQL_Latin1_General_CP1_CI_AS)
	and (@NtUserExcludeFilter is null or isnull(q.NtUserName,'') not like @NtUserExcludeFilter  collate SQL_Latin1_General_CP1_CI_AS)
	and isnull(q.NtUserName,'') like coalesce(@NtUserIncludeFilter,'%'  collate SQL_Latin1_General_CP1_CI_AS)
	and (@SqlUserNameExcludeFilter is null or isnull(q.SqlUserName,'') not like @SqlUserNameExcludeFilter collate SQL_Latin1_General_CP1_CI_AS)
	and isnull(q.SqlUserName,'') like coalesce(@SqlUserNameIncludeFilter,'%' collate SQL_Latin1_General_CP1_CI_AS)
	and (@SqlTextExcludeFilter is null or isnull(q.StatementText,'') not like @SqlTextExcludeFilter collate SQL_Latin1_General_CP1_CI_AS)
	and isnull(q.StatementText,'') like coalesce(@SqlTextIncludeFilter,'%' collate SQL_Latin1_General_CP1_CI_AS) 
	and q.StatementType in (
		case when @IncludeSQLStatements = 1 then 1 else null end,
		case when @IncludeStoredProcedures = 1 then 0 else null end,
		case when @IncludeSQLBatches = 1 then 2 else null end)

	select 
		DatabaseName,
		StartTime,
		CompletionTime,
		DurationMilliseconds,
		CPUMilliseconds,
		Reads,
		Writes,
		NtUserName,
		SqlUserName,
		ClientComputerName,
		ApplicationName,
		StatementType,
		StatementText,
		Occurrances
	from @OutputTable
end
else
begin
insert into @OutputTable
select
	d.DatabaseName,
	null,
	CompletionTime,
	sum(DurationMilliseconds),
	sum(CPUMilliseconds),
	sum(Reads),
	sum(Writes),
	null,
	null,
	null,
	null,
	StatementType,
	case when @CaseInsensitive = 1 then lower(dbo.SQL_Signature(ltrim(StatementText))) else dbo.SQL_Signature(ltrim(StatementText)) end,
	count(*)
from
	QueryMonitor q 
	right join @IntermediateTable d
	on d.DatabaseID = q.DatabaseID
where
	q.SQLServerID = @SQLServerID
	and cast(floor(cast (dateadd(hh, @UtcOffset, q.CompletionTime) as float)) as datetime) >= (coalesce(@StartDate,cast(floor(cast (dateadd(hh, @UtcOffset, q.CompletionTime) as float)) as datetime)))
	and cast(floor(cast (dateadd(hh, @UtcOffset, q.CompletionTime) as float)) as datetime) <= (coalesce(@EndDate,cast(floor(cast (dateadd(hh, @UtcOffset, q.CompletionTime) as float)) as datetime)))
	and (cast(cast(dateadd(hh, @UtcOffset,CompletionTime) as float) - floor(cast (dateadd(hh, @UtcOffset,CompletionTime) as float)) as datetime)) >= coalesce(@StartTime,(cast(cast(dateadd(hh, @UtcOffset,CompletionTime) as float) - floor(cast (dateadd(hh, @UtcOffset,CompletionTime) as float)) as datetime)))
	and (cast(cast(dateadd(hh, @UtcOffset,CompletionTime) as float) - floor(cast (dateadd(hh, @UtcOffset,CompletionTime) as float)) as datetime)) <= coalesce(@EndTime,(cast(cast(dateadd(hh, @UtcOffset,CompletionTime) as float) - floor(cast (dateadd(hh, @UtcOffset,CompletionTime) as float)) as datetime)))
	and q.DurationMilliseconds >= coalesce(@DurationFilter,q.DurationMilliseconds)
	and (@ApplicationNameExcludeFilter is null or isnull(q.ApplicationName,'') not like @ApplicationNameExcludeFilter collate SQL_Latin1_General_CP1_CI_AS)
	and isnull(q.ApplicationName,'') like coalesce(@ApplicationNameIncludeFilter,'%')  collate SQL_Latin1_General_CP1_CI_AS
	and (@ClientComputerNameExcludeFilter is null or isnull(q.ClientComputerName,'') not like @ClientComputerNameExcludeFilter collate SQL_Latin1_General_CP1_CI_AS)
	and isnull(q.ClientComputerName,'') like coalesce(@ClientComputerNameIncludeFilter,'%') collate SQL_Latin1_General_CP1_CI_AS
	and (@NtUserExcludeFilter is null or isnull(q.NtUserName,'') not like @NtUserExcludeFilter collate SQL_Latin1_General_CP1_CI_AS)
	and isnull(q.NtUserName,'') like coalesce(@NtUserIncludeFilter,'%') collate SQL_Latin1_General_CP1_CI_AS
	and (@SqlUserNameExcludeFilter is null or isnull(q.SqlUserName,'') not like @SqlUserNameExcludeFilter collate SQL_Latin1_General_CP1_CI_AS)
	and isnull(q.SqlUserName,'') like coalesce(@SqlUserNameIncludeFilter,'%') collate SQL_Latin1_General_CP1_CI_AS
	and (@SqlTextExcludeFilter is null or isnull(q.StatementText,'') not like @SqlTextExcludeFilter collate SQL_Latin1_General_CP1_CI_AS)
	and isnull(q.StatementText,'') like coalesce(@SqlTextIncludeFilter,'%') collate SQL_Latin1_General_CP1_CI_AS
	and q.StatementType in (
		case when @IncludeSQLStatements = 1 then 1 else null end,
		case when @IncludeStoredProcedures = 1 then 0 else null end,
		case when @IncludeSQLBatches = 1 then 2 else null end)
group by 
	CompletionTime,
	d.DatabaseName,
	StatementType,
	case when @CaseInsensitive = 1 then lower(dbo.SQL_Signature(ltrim(StatementText))) else dbo.SQL_Signature(ltrim(StatementText)) end

select 
	DatabaseName,
	StartTime,
	max(CompletionTime) as 'CompletionTime',
	sum(DurationMilliseconds) / case when sum(Occurrances) > 0 then sum(Occurrances) else 1 end as 'DurationMilliseconds',
	sum(CPUMilliseconds) / case when sum(Occurrances) > 0 then sum(Occurrances) else 1 end as 'CPUMilliseconds',
	sum(Reads) / case when sum(Occurrances) > 0 then sum(Occurrances) else 1 end as 'Reads',
	sum(Writes) / case when sum(Occurrances) > 0 then sum(Occurrances) else 1 end 'Writes',
	NtUserName,
	SqlUserName,
	ClientComputerName,
	ApplicationName,
	StatementType,
	StatementText,
	sum(Occurrances) as 'Occurrances'
from @OutputTable
group by
	DatabaseName,
	StartTime,
	NtUserName,
	SqlUserName,
	ClientComputerName,
	ApplicationName,
	StatementType,
	StatementText

end

select
	max(CompletionTime),
	sum(case when StatementType = 0 then Occurrances else 0 end) as [Stored Procedures],
	sum(case when StatementType = 1 then Occurrances else 0 end) as [Single Statements],
	sum(case when StatementType = 2 then Occurrances else 0 end) as [Batches]
from
	@OutputTable
group by
	datepart(yy, dateadd(hh, @UtcOffset, [CompletionTime]))
	,case when @Interval <= 3 then datepart(mm,dateadd(hh, @UtcOffset, [CompletionTime])) else datepart(yy,dateadd(hh, @UtcOffset, [CompletionTime])) end
	,case when @Interval <= 2 then datepart(dd,dateadd(hh, @UtcOffset, [CompletionTime])) else datepart(yy,dateadd(hh, @UtcOffset, [CompletionTime])) end
	,case when @Interval <= 1 then datepart(hh,dateadd(hh, @UtcOffset, [CompletionTime])) else datepart(yy,dateadd(hh, @UtcOffset, [CompletionTime])) end
	,case when @Interval =  0 then datepart(mi,dateadd(hh, @UtcOffset, [CompletionTime])) else datepart(yy,dateadd(hh, @UtcOffset, [CompletionTime])) end
order by
	max(CompletionTime)


end

GO

if (select Internal_Value from RepositoryInfo where Name = 'SQLVersionAtUpgrade') > 8
begin
	declare @cmd1 varchar(8000), @cmd2 varchar(8000), @cmd3 varchar(8000)
	select @cmd1 = '', @cmd2 = '', @cmd3 = ''
	
	select @cmd1 = replace(replace( text, 'nvarchar(4000)','nvarchar(max)'),'create procedure ','alter procedure ')
		from syscomments where id = object_id('p_GetQueryMonitorStatements') and colid = 1
		
	select @cmd2 = replace(replace( text, 'nvarchar(4000)','nvarchar(max)'),'create procedure ','alter procedure ')
		from syscomments where id = object_id('p_GetQueryMonitorStatements') and colid = 2

	select @cmd3 = replace(replace( text, 'nvarchar(4000)','nvarchar(max)'),'create procedure ','alter procedure ')
		from syscomments where id = object_id('p_GetQueryMonitorStatements') and colid = 3

	exec(@cmd1 + @cmd2 + @cmd3)

end

GO

if (object_id('p_GetQueryStatistics') is not null)
begin
drop procedure p_GetQueryStatistics
end
go
-- @Interval - Granularity of calculation:
--	0 - Minutes
--	1 - Hours
--	2 - Days
--	3 - Months
--	4 - Years
--  5 - All Time
create procedure [dbo].p_GetQueryStatistics
	@SQLServerID int,
	@StartDateTime datetime = null,
	@EndDateTime datetime = null,
	@UtcOffset int = 0,
	@Interval int,
	@QueryText nvarchar(4000),
	@ApplicationName nvarchar(4000),
	@StatementType int,
	@DatabaseName sysname,
	@SignatureMode bit = 0,
	@CaseInsensitive bit = 0
as 
begin

	if @UtcOffset is null
		set @UtcOffset = datediff(hh,getutcdate(),getdate())

	if @CaseInsensitive = 1
	begin
		set @ApplicationName = lower(@ApplicationName)
		set @DatabaseName = lower(@DatabaseName)
		set @QueryText = lower(@QueryText)
	end
	
	declare @LenCompare int
	set @LenCompare = len(@QueryText)
	if @LenCompare < 1000 
		set @LenCompare = 1000

	select
		FirstStartTime = min(dateadd(hh,@UtcOffset,case when DurationMilliseconds < 2147483648 then dateadd(ms, -DurationMilliseconds,  [CompletionTime]) else dateadd(s, -DurationMilliseconds / 1000,  [CompletionTime]) end)),
		LastEndTime = max(dateadd(hh,@UtcOffset,CompletionTime)),
		MinDuration = min(DurationMilliseconds),
		MaxDuration = max(DurationMilliseconds),
		SumDuration = sum(DurationMilliseconds),
		AvgDuration = avg(cast(DurationMilliseconds as float)),
		MinCPU = min(CPUMilliseconds),
		MaxCPU = max(CPUMilliseconds),
		SumCPU = sum(CPUMilliseconds),
		AvgCPU = avg(cast(CPUMilliseconds as float)),
		MinReads = min(Reads),
		MaxReads = max(Reads),
		SumReads = sum(Reads),
		AvgReads = avg(cast(Reads as float)),
		MinWrites = min(Writes),
		MaxWrites = max(Writes),
		SumWrites = sum(Writes),
		AvgWrites = avg(cast(Writes as float)),
		[Count] = count(*)
	from
		QueryMonitor qm
		join dbo.SQLServerDatabaseNames d on qm.DatabaseID = d.DatabaseID
	where
		qm.SQLServerID = @SQLServerID and
		rtrim(ltrim(case when @CaseInsensitive = 1 then lower(ApplicationName) else ApplicationName end)) like rtrim(ltrim(coalesce(@ApplicationName,case when @CaseInsensitive = 1 then lower(ApplicationName) else ApplicationName end))) and
		rtrim(ltrim(StatementType)) like coalesce(@StatementType,StatementType) and
		case when @CaseInsensitive = 1 then lower(d.DatabaseName) else d.DatabaseName end like coalesce(@DatabaseName,case when @CaseInsensitive = 1 then lower(d.DatabaseName) else d.DatabaseName end) and
		((@SignatureMode = 0 and 
			left(ltrim(case when @CaseInsensitive = 1 then lower(StatementText) else StatementText end),@LenCompare) = left(ltrim(@QueryText),@LenCompare))
		or
		(@SignatureMode = 1 and 
			left(ltrim(case when @CaseInsensitive = 1 then lower(dbo.SQL_Signature(ltrim(StatementText))) else dbo.SQL_Signature(ltrim(StatementText)) end ),@LenCompare)= left(ltrim(@QueryText),@LenCompare))) and
		isnull(@StartDateTime,'1 Jan 1900') <= case when DurationMilliseconds < 2147483648 then dateadd(ms, -DurationMilliseconds,  [CompletionTime]) else dateadd(s, -DurationMilliseconds / 1000,  [CompletionTime]) end and
		isnull(@EndDateTime,'1 Jan 2200') >= CompletionTime
	group by
		 case when isnull(@Interval,5) <= 4 then datepart(yy, dateadd(hh, @UtcOffset, [CompletionTime])) else 1 end
		,case when isnull(@Interval,5) <= 3 then datepart(mm,dateadd(hh, @UtcOffset, [CompletionTime])) else datepart(yy,dateadd(hh, @UtcOffset, [CompletionTime])) end
		,case when isnull(@Interval,5) <= 2 then datepart(dd,dateadd(hh, @UtcOffset, [CompletionTime])) else datepart(yy,dateadd(hh, @UtcOffset, [CompletionTime])) end
		,case when isnull(@Interval,5) <= 1 then datepart(hh,dateadd(hh, @UtcOffset, [CompletionTime])) else datepart(yy,dateadd(hh, @UtcOffset, [CompletionTime])) end
		,case when isnull(@Interval,5) =  0 then datepart(mi,dateadd(hh, @UtcOffset, [CompletionTime])) else datepart(yy,dateadd(hh, @UtcOffset, [CompletionTime])) end

end

Go

if (select Internal_Value from RepositoryInfo where Name = 'SQLVersionAtUpgrade') > 8
begin
	declare @cmd varchar(8000)
	select @cmd = replace(replace( text, '@QueryText nvarchar(4000)','@QueryText nvarchar(max)'),'create procedure ','alter procedure ')
	from syscomments where id = object_id('p_GetQueryStatistics')
	execute( @cmd )
end

GO

grant EXECUTE on p_GetQueryStatistics to [SQLdmConsoleUser]

GO


if (object_id('p_GetTopQueries') is not null)
begin
drop procedure p_GetTopQueries
end
go
-- @OrderBy
--	0 - Frequency
--	1 - Duration
--	2 - CPU
--	3 - Reads
--	4 - Writes
create procedure [dbo].p_GetTopQueries
	@SQLServerID int,
	@StartDateTime datetime = null,
	@EndDateTime datetime = null,
	@UtcOffset int = null,
	@IncludeStoredProcedures bit = 1,
	@IncludeSQLStatements bit = 1,
	@IncludeSQLBatches bit = 1,
	@MinExecutions bigint = null,
	@MinDuration bigint = null,
	@MinReads bigint = null,
	@MinWrites bigint = null,
	@MinCPU bigint = null,
	@TopN bigint = 10,
	@SignatureMode bit = 0,
	@OrderBy tinyint = 0,
	@CaseInsensitive bit = 0
as
begin
set ansi_warnings off
set ansi_nulls off

if @UtcOffset is null
	set @UtcOffset = datediff(hh,getutcdate(),getdate())

declare @AllTypes bit 
if @IncludeStoredProcedures = 1 AND @IncludeSQLStatements = 1 AND @IncludeSQLBatches = 1
	set @AllTypes = 1
else
	set @AllTypes = 0

set rowcount @TopN

select 
	d.DatabaseName,
	FirstStartTime = min(dateadd(hh,@UtcOffset,case when DurationMilliseconds < 2147483648 then dateadd(ms, -DurationMilliseconds,  [CompletionTime]) else dateadd(s, -DurationMilliseconds / 1000,  [CompletionTime]) end)),
	LastEndTime = max(dateadd(hh,@UtcOffset,CompletionTime)),
	AverageDuration = avg(DurationMilliseconds),
	AverageCPU = avg(CPUMilliseconds),
	AverageReads = avg(Reads),
	AverageWrites = avg(Writes),
	ApplicationName,
	StatementType,
	StatementText = left(case when @CaseInsensitive = 1 then lower(case when @SignatureMode = 0 then ltrim(StatementText) else dbo.SQL_Signature(ltrim(StatementText)) end)
	else case when @SignatureMode = 0 then ltrim(StatementText) else dbo.SQL_Signature(ltrim(StatementText)) end end,1000),
	[Count] = count(*)
from
	dbo.QueryMonitor qm
	join dbo.SQLServerDatabaseNames d on qm.DatabaseID = d.DatabaseID
where
	qm.SQLServerID = @SQLServerID and
	(
		@AllTypes = 1
		OR
		qm.StatementType = 0 and @IncludeStoredProcedures = 1
		OR
		qm.StatementType = 1 and @IncludeSQLStatements = 1
		OR
		qm.StatementType = 2 and @IncludeSQLBatches = 1
	) and
	isnull(@StartDateTime,'1 Jan 1900') <= case when DurationMilliseconds < 2147483648 then dateadd(ms, -DurationMilliseconds,  [CompletionTime]) else dateadd(s, -DurationMilliseconds / 1000,  [CompletionTime]) end and
	isnull(@EndDateTime,'1 Jan 2200') >= CompletionTime
group by
	StatementType,
	left(case when @CaseInsensitive = 1 then lower(case when @SignatureMode = 0 then ltrim(StatementText) else dbo.SQL_Signature(ltrim(StatementText)) end)
	else case when @SignatureMode = 0 then ltrim(StatementText) else dbo.SQL_Signature(ltrim(StatementText)) end end,1000),
	d.DatabaseName,
	ApplicationName
having
	count(*) >= isnull(@MinExecutions,0) and
	avg(DurationMilliseconds) >= isnull(@MinDuration,0) and
	avg(Reads) >= isnull(@MinReads,0) and
	avg(Writes) >= isnull(@MinWrites,0) and
	avg(CPUMilliseconds) >= isnull(@MinCPU,0)
order by
	case 
		when @OrderBy = 0 then count(*) 
		when @OrderBy = 1 then avg(DurationMilliseconds)
		when @OrderBy = 2 then avg(CPUMilliseconds)
		when @OrderBy = 3 then avg(Reads)
		when @OrderBy = 4 then avg(Writes)
		else  count(*)
	end 
	desc
end

Go

grant EXECUTE on p_GetTopQueries to [SQLdmConsoleUser]

go

if (object_id('p_GetTopQueriesByFrequency') is not null)
begin
drop procedure p_GetTopQueriesByFrequency
end
go
create procedure [dbo].p_GetTopQueriesByFrequency
	@SQLServerID int,
	@StartDateTime datetime = null,
	@EndDateTime datetime = null,
	@UtcOffset int = null,
	@IncludeStoredProcedures bit = 1,
	@IncludeSQLStatements bit = 1,
	@IncludeSQLBatches bit = 1,
	@MinExecutions bigint = null,
	@MinDuration bigint = null,
	@MinReads bigint = null,
	@MinWrites bigint = null,
	@MinCPU bigint = null,
	@TopN bigint = 10,
	@SignatureMode bit = 0,
	@CaseInsensitive bit = 0
as
begin
	exec p_GetTopQueries
		@SQLServerID,
		@StartDateTime,
		@EndDateTime,
		@UtcOffset,
		@IncludeStoredProcedures,
		@IncludeSQLStatements,
		@IncludeSQLBatches,
		@MinExecutions,
		@MinDuration,
		@MinReads,
		@MinWrites,
		@MinCPU,
		@TopN,
		@SignatureMode,
		0,
		@CaseInsensitive
end

Go

grant EXECUTE on p_GetTopQueriesByFrequency to [SQLdmConsoleUser]

go

if (object_id('p_GetTopQueriesByDuration') is not null)
begin
drop procedure p_GetTopQueriesByDuration
end
go
create procedure [dbo].p_GetTopQueriesByDuration
	@SQLServerID int,
	@StartDateTime datetime = null,
	@EndDateTime datetime = null,
	@UtcOffset int = null,
	@IncludeStoredProcedures bit = 1,
	@IncludeSQLStatements bit = 1,
	@IncludeSQLBatches bit = 1,
	@MinExecutions bigint = null,
	@MinDuration bigint = null,
	@MinReads bigint = null,
	@MinWrites bigint = null,
	@MinCPU bigint = null,
	@TopN bigint = 10,
	@SignatureMode bit = 0,
	@CaseInsensitive bit = 0
as
begin
	exec p_GetTopQueries
		@SQLServerID,
		@StartDateTime,
		@EndDateTime,
		@UtcOffset,
		@IncludeStoredProcedures,
		@IncludeSQLStatements,
		@IncludeSQLBatches,
		@MinExecutions,
		@MinDuration,
		@MinReads,
		@MinWrites,
		@MinCPU,
		@TopN,
		@SignatureMode,
		1,
		@CaseInsensitive
end

Go

grant EXECUTE on p_GetTopQueriesByDuration to [SQLdmConsoleUser]

go

if (object_id('p_GetTopQueriesByCPU') is not null)
begin
drop procedure p_GetTopQueriesByCPU
end
go
create procedure [dbo].p_GetTopQueriesByCPU
	@SQLServerID int,
	@StartDateTime datetime = null,
	@EndDateTime datetime = null,
	@UtcOffset int = null,
	@IncludeStoredProcedures bit = 1,
	@IncludeSQLStatements bit = 1,
	@IncludeSQLBatches bit = 1,
	@MinExecutions bigint = null,
	@MinDuration bigint = null,
	@MinReads bigint = null,
	@MinWrites bigint = null,
	@MinCPU bigint = null,
	@TopN bigint = 10,
	@SignatureMode bit = 0,
	@CaseInsensitive bit = 0
as
begin
	exec p_GetTopQueries
		@SQLServerID,
		@StartDateTime,
		@EndDateTime,
		@UtcOffset,
		@IncludeStoredProcedures,
		@IncludeSQLStatements,
		@IncludeSQLBatches,
		@MinExecutions,
		@MinDuration,
		@MinReads,
		@MinWrites,
		@MinCPU,
		@TopN,
		@SignatureMode,
		2,
		@CaseInsensitive
end

Go

grant EXECUTE on p_GetTopQueriesByCPU to [SQLdmConsoleUser]

go

if (object_id('p_GetTopQueriesByReads') is not null)
begin
drop procedure p_GetTopQueriesByReads
end
go
create procedure [dbo].p_GetTopQueriesByReads
	@SQLServerID int,
	@StartDateTime datetime = null,
	@EndDateTime datetime = null,
	@UtcOffset int = null,
	@IncludeStoredProcedures bit = 1,
	@IncludeSQLStatements bit = 1,
	@IncludeSQLBatches bit = 1,
	@MinExecutions bigint = null,
	@MinDuration bigint = null,
	@MinReads bigint = null,
	@MinWrites bigint = null,
	@MinCPU bigint = null,
	@TopN bigint = 10,
	@SignatureMode bit = 0,
	@CaseInsensitive bit = 0
as
begin
	exec p_GetTopQueries
		@SQLServerID,
		@StartDateTime,
		@EndDateTime,
		@UtcOffset,
		@IncludeStoredProcedures,
		@IncludeSQLStatements,
		@IncludeSQLBatches,
		@MinExecutions,
		@MinDuration,
		@MinReads,
		@MinWrites,
		@MinCPU,
		@TopN,
		@SignatureMode,
		3,
		@CaseInsensitive
end

Go

grant EXECUTE on p_GetTopQueriesByReads to [SQLdmConsoleUser]

go

if (object_id('p_GetTopQueriesByWrites') is not null)
begin
drop procedure p_GetTopQueriesByWrites
end
go
create procedure [dbo].p_GetTopQueriesByWrites
	@SQLServerID int,
	@StartDateTime datetime = null,
	@EndDateTime datetime = null,
	@UtcOffset int = null,
	@IncludeStoredProcedures bit = 1,
	@IncludeSQLStatements bit = 1,
	@IncludeSQLBatches bit = 1,
	@MinExecutions bigint = null,
	@MinDuration bigint = null,
	@MinReads bigint = null,
	@MinWrites bigint = null,
	@MinCPU bigint = null,
	@TopN bigint = 10,
	@SignatureMode bit = 0,
	@CaseInsensitive bit = 0
as
begin
	exec p_GetTopQueries
		@SQLServerID,
		@StartDateTime,
		@EndDateTime,
		@UtcOffset,
		@IncludeStoredProcedures,
		@IncludeSQLStatements,
		@IncludeSQLBatches,
		@MinExecutions,
		@MinDuration,
		@MinReads,
		@MinWrites,
		@MinCPU,
		@TopN,
		@SignatureMode,
		4,
		@CaseInsensitive
end

Go

grant EXECUTE on p_GetTopQueriesByWrites to [SQLdmConsoleUser]

GO

if (object_id('p_InsertQueryMonitorStatement') is not null)
begin
drop procedure p_InsertQueryMonitorStatement
end
go
create procedure p_InsertQueryMonitorStatement
	@SQLServerID int,
	@DatabaseName nvarchar(255),
	@UTCCollectionDateTime datetime,
	@CompletionTime datetime,
	@DurationMilliseconds bigint,
	@CPUMilliseconds bigint,
	@Reads bigint,
	@Writes bigint,
	@NtUserName nvarchar(255),
	@SqlUserName nvarchar(255),
	@ClientComputerName nvarchar(255),
	@ApplicationName nvarchar(255),
	@StatementType int,
	@StatementText nvarchar(3060),            
	@ReturnMessage nvarchar(128) output
as
begin

declare @DatabaseID int,
		@InnerReturnMessage nvarchar(128)

execute [p_InsertDatabaseName] 
   @SQLServerID
  ,@DatabaseName
  ,0	-- assume that this is not a system database (all system databases should already have been added to the names table
  ,null
  ,@DatabaseID output
  ,@InnerReturnMessage output

insert into [QueryMonitor]
   ([SQLServerID]
   ,[UTCCollectionDateTime]
   ,[DatabaseID]
   ,[CompletionTime]
   ,[DurationMilliseconds]
   ,[CPUMilliseconds]
   ,[Reads]
   ,[Writes]
   ,[NtUserName]
   ,[SqlUserName]
   ,[ClientComputerName]
   ,[ApplicationName]
   ,[StatementType]
   ,[StatementText])
values
	(@SQLServerID,
	@UTCCollectionDateTime,
	@DatabaseID,
	@CompletionTime,
	@DurationMilliseconds,
	@CPUMilliseconds,
	@Reads,
	@Writes,
	@NtUserName,
	@SqlUserName,
	@ClientComputerName,
	@ApplicationName,
	@StatementType,
	@StatementText)

end

go

if (select Internal_Value from RepositoryInfo where Name = 'SQLVersionAtUpgrade') > 8
begin
	declare @cmd varchar(8000)
	select @cmd = replace(
				  replace(
				  text, 
				  ' nvarchar(3060)',' nvarchar(max)'),
				  'create procedure ','alter procedure ')
	from syscomments where id = object_id('p_InsertQueryMonitorStatement')
	execute( @cmd )
end

if (object_id('SQL_Signature') is not null)
begin
drop function SQL_Signature
end
go
CREATE FUNCTION SQL_Signature 
	(@p1 ntext)
RETURNS nvarchar(3500)

--
-- This function is provided "AS IS" with no warranties, and confers no rights. 
-- Use of included script samples are subject to the terms specified at http://www.microsoft.com/info/cpyright.htm
-- 
-- Strips query strings in sysprocesses
AS
BEGIN 
	DECLARE @pos as INT
	DECLARE @mode as CHAR(10)
	DECLARE @maxlength as INT
	DECLARE @p2 as NCHAR(3500)
	DECLARE @currchar as CHAR(1), @nextchar as CHAR(1)
	DECLARE @p2len as INT


	SET @maxlength = len(rtrim(substring(@p1,1,3500)));
--	SET @maxlength = case when @maxlength > @parselength 
--			then @parselength else @maxlength end

	SET @pos = 1;
	SET @p2 = '';
	SET @p2len = 0;
	SET @currchar = ''
	set @nextchar = ''
	SET @mode = 'command';

	WHILE (@pos <= @maxlength) BEGIN
		SET @currchar = substring(@p1,@pos,1)
		SET @nextchar = substring(@p1,@pos+1,1)
		IF @mode = 'command' BEGIN
			SET @p2 = left(@p2,@p2len) + @currchar
			SET @p2len = @p2len + 1 
			IF @currchar in (',','(',' ','=','<','>','!') and
			   @nextchar between '0' and '9' BEGIN
				set @mode = 'number'
				SET @p2 = left(@p2,@p2len) + '#'
				SET @p2len = @p2len + 1
				END 
			IF @currchar = '''' BEGIN
				set @mode = 'literal'
				SET @p2 = left(@p2,@p2len) + '#'''
				SET @p2len = @p2len + 2 
				END
			END
		ELSE IF @mode = 'number' and @nextchar in (',',')',' ','=','<','>','!')
			SET @mode= 'command'
		ELSE IF @mode = 'literal' and @currchar = ''''
			SET @mode= 'command'

		SET @pos = @pos + 1
	END
	RETURN @p2 

END
GO

if (select Internal_Value from RepositoryInfo where Name = 'SQLVersionAtUpgrade') > 8
begin
	declare @cmd varchar(8000)
	select @cmd = replace(
				  replace(
				  replace( 
				  replace( 
				  replace( 
				  text, 
				  'nvarchar(3500)','nvarchar(max)'),
				  'NCHAR(3500)','nvarchar(max)'),
				  'SET @maxlength = len(rtrim(substring(@p1,1,3500)));','SET @maxlength = len(@p1)'),
				  '(@p1 ntext)','(@p1 nvarchar(max))'),
				  'CREATE FUNCTION SQL_Signature ','ALTER FUNCTION SQL_Signature ')
	from syscomments where id = object_id('SQL_Signature')
	execute( @cmd )
end

GO







