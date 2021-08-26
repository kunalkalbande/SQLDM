	if (object_id('[p_GetSessionStatistics]') is not null)
	begin
	drop procedure [p_GetSessionStatistics]
	end
	go
	create procedure [dbo].[p_GetSessionStatistics]
		@SQLServerID int,
		@HistoryInMinutes int = null
	as
	begin
	set transaction isolation level read uncommitted
	declare @err int

	declare @BeginDateTime datetime
	declare @EndDateTime datetime  = (select max(UTCCollectionDateTime) from [ServerActivity] where [SQLServerID] = @SQLServerID)

	if (@HistoryInMinutes is null)
		select @BeginDateTime = @EndDateTime
	else
		select @BeginDateTime = dateadd(n, -@HistoryInMinutes, @EndDateTime)

	-- Get session and lock list for requested snapshot
	select
		ss.SQLServerID
		,mss.InstanceName
		,ss.UTCCollectionDateTime
		,ss.ResponseTimeInMilliseconds
		,ss.UserProcesses
		,sa.SessionList
	from 
		[ServerActivity] sa
		LEFT JOIN ServerStatistics ss ON sa.SQLServerID = ss.SQLServerID 
			AND sa.UTCCollectionDateTime = ss.UTCCollectionDateTime
		INNER join [MonitoredSQLServers] mss
		on sa.SQLServerID = mss.SQLServerID
	where 
		sa.[SQLServerID] = @SQLServerID
		and sa.[UTCCollectionDateTime] between @BeginDateTime and @EndDateTime

	select @err = @@error
	return @err
	end