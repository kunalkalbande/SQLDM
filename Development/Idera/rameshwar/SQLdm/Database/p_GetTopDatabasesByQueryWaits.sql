-- Created by Aditya Shukla for SQLdm 8.6
-- This procedure is used to fetch data for the Query Waits section in Top databases report

--exec [p_GetTopDatabasesByQueryWaits] 9, null, '2013-7-24 00:00:00', '2014-7-24 00:00:00', 330.0, 0, 0, 0, 0, 0, 5, 1, 2000 

if (object_id('p_GetTopDatabasesByQueryWaits') is not null)
begin
drop procedure [p_GetTopDatabasesByQueryWaits]
end
go

create procedure [dbo].[p_GetTopDatabasesByQueryWaits]
	@ServerId int,
	@DatabaseNameFilter sysname = null,
	@UTCStart DateTime,
	@UTCEnd DateTime,
	@UTCOffset int = 0,
	@MinSizeMB decimal = null,
	@MinGrowthPercent bigint = null,
	@MinTransactionsPerSecond bigint = null,
	@MinReadsPerSecond bigint = null,
	@MinWritesPerSecond bigint = null,
	@TopN bigint = 10,
	@IncludeSystem bit = 1,
	@threshold bigint = 0 -- a value in milliseconds
as
begin

	set rowcount 0

	create table #DatabaseInfo
	(
		DatabaseID int primary key clustered, 
		DatabaseName nvarchar(255),
		StartTime datetime,
		EndTime datetime,
		StartTotalDataSizeInKilobytes decimal,
		EndDataFileSizeInKilobytes decimal,
		EndLogFileSizeInKilobytes decimal,
		EndTotalDataSizeInKilobytes decimal,
		UTCStart datetime,
		UTCEnd datetime
	)

	--Applying the @DatabaseName filter if any. Provided by user on the UI
	if @DatabaseNameFilter is null or len(rtrim(ltrim(@DatabaseNameFilter))) = 0
	begin
		insert into #DatabaseInfo(DatabaseID,DatabaseName, StartTime, EndTime,UTCStart,UTCEnd)
			select 
				names.DatabaseID,
				DatabaseName,
				min(UTCCollectionDateTime),
				max(UTCCollectionDateTime) ,
				@UTCStart,
				@UTCEnd
			from 
				[dbo].[SQLServerDatabaseNames] names (nolock)
				left join DatabaseSize (nolock) ds
				on ds.DatabaseID = names.DatabaseID
			where
				names.[SQLServerID] = @ServerId
				and (@IncludeSystem = 1 or names.SystemDatabase = 0)
				and dateadd(minute, datediff(minute, 0, [UTCCollectionDateTime]), 0) between @UTCStart and @UTCEnd
			group by
				names.DatabaseID,
				DatabaseName
	end		
	else
	begin
		insert into #DatabaseInfo(DatabaseID,DatabaseName, StartTime, EndTime,UTCStart,UTCEnd)
			select 
				names.DatabaseID,
				DatabaseName,
				min(UTCCollectionDateTime),
				max(UTCCollectionDateTime) ,
				@UTCStart,
				@UTCEnd
			from 
				[dbo].[SQLServerDatabaseNames] names (nolock)
				left join DatabaseSize (nolock) ds
				on ds.DatabaseID = names.DatabaseID
			where
				names.[SQLServerID] = @ServerId
				and names.[DatabaseName] like @DatabaseNameFilter	
				and (@IncludeSystem = 1 or names.SystemDatabase = 0)
				and dateadd(minute, datediff(minute, 0, [UTCCollectionDateTime]), 0) between @UTCStart and @UTCEnd
			group by
				names.DatabaseID,
				DatabaseName
	end		

	update #DatabaseInfo
	set 
		StartTotalDataSizeInKilobytes = isnull(DataSizeInKilobytes,0)
	from
		#DatabaseInfo di inner join 
		[DatabaseSize] ds (nolock)
		on di.DatabaseID = ds.DatabaseID
	where
		[UTCCollectionDateTime] = StartTime

	update #DatabaseInfo
	set 
		EndDataFileSizeInKilobytes = DataFileSizeInKilobytes,
		EndLogFileSizeInKilobytes = LogFileSizeInKilobytes,
		EndTotalDataSizeInKilobytes = isnull(DataSizeInKilobytes,0)
	from
		#DatabaseInfo di inner join 
		[DatabaseSize] ds (nolock)
		on di.DatabaseID = ds.DatabaseID
	where
		[UTCCollectionDateTime] =  EndTime
	
	--Applying the Size filter 
	if (@MinSizeMB is not null)
	begin	
		delete from #DatabaseInfo
		where
		isnull(EndTotalDataSizeInKilobytes,0) / 1024 < @MinSizeMB	
	end

	--Applying Growth filter
	if (@MinGrowthPercent is not null)
	begin
		delete from #DatabaseInfo
		where DatabaseID in(
		select DatabaseID 
		from #DatabaseInfo
		group by DatabaseID
		having
		isnull(max(EndTotalDataSizeInKilobytes / nullif(StartTotalDataSizeInKilobytes,0)),0) < isnull((cast(@MinGrowthPercent as float) / 100) + case @MinGrowthPercent when 0 then 0 else 1 end,1))
	end

	--Assigning row numbers to rows of sets partitioned by database name and ordered by wait desc 
	select aws.SQLServerID,
		di.DatabaseName, 
		aws.WaitTypeID, 
		aws.LoginNameID, 
		aws.WaitDuration, 
		aws.WaitDuration - @threshold as [difference], 
		aws.StatementUTCStartTime,
		row_number() over (partition by aws.[DatabaseID] order by aws.WaitDuration desc) as rankNumber 
	into #RankedData
	from #DatabaseInfo di inner join ActiveWaitStatistics aws (nolock)
		on di.DatabaseID = aws.DatabaseID
	where aws.[UTCCollectionDateTime] between @UTCStart and @UTCEnd
	and aws.WaitDuration > @threshold

	--This ensures that only @TopN rows are selected
	set rowcount @TopN

	--Selecting the top row from each of the partitioned sets while the row count is less than @TopN
	select 
		rd.DatabaseName [DatabaseName], 
		wt.WaitType [WaitType],
		rd.StatementUTCStartTime [StatementUTCStartTime],
		rd.WaitDuration [WaitDuration],
		rd.[difference] [ExceededThresholdBy],
		ln.LoginName [LoginName]
	from #RankedData rd left join LoginNames ln (nolock) 
	on rd.LoginNameID = ln.LoginNameID
	left join WaitTypes wt (nolock)
	on wt.WaitTypeID = rd.WaitTypeID
	where rankNumber = 1
	order by rd.WaitDuration desc

	set rowcount 0
end
GO


