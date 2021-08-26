IF (object_id('p_GetTopInstancesByQueries') is not null)
BEGIN
drop procedure p_GetTopInstancesByQueries
END
GO

/*-- 
SQLdm 8.5 (Ankit Srivastava): for Top X API- Query Monitor Event
EXEC  [p_GetTopInstancesByQueries] '2014-05-07 10:24:29.587','2014-05-07 15:24:29.587',1
--*/

Create PROCEDURE [dbo].[p_GetTopInstancesByQueries](
	--@ServerId int=null,
	@UTCStartTime DateTime,
	@UTCEndTime DateTime,
	@TopX int=0
)
AS
BEGIN


	select Top (@TopX)  qms.SQLServerID , mss.InstanceName,  count(qms.QueryStatisticsID) as NumOfQueries
	from QueryMonitorStatistics qms WITH(NOLOCK)
	left join MonitoredSQLServers mss WITH(NOLOCK) on mss.SQLServerID = qms.SQLServerID
	where mss.Active=1 
	and qms.UTCCollectionDateTime  between @UTCStartTime and @UTCEndTime
	group by qms.SQLServerID,mss.InstanceName
	order by NumOfQueries desc 

END



