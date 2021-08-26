if (object_id('[p_GetOSStatisticsAvailable]') is not null)
begin
drop procedure [dbo].[p_GetOSStatisticsAvailable]
end
go
Create Proc p_GetOSStatisticsAvailable
@ServerID int
as
begin
SELECT     OsStatisticAvailability
FROM         ServerStatistics AS ss (nolock)
WHERE     (UTCCollectionDateTime =
                          (SELECT     MAX(UTCCollectionDateTime) AS EXPR1
                            FROM          ServerStatistics (nolock)
                            WHERE      (SQLServerID = @ServerID))) AND (SQLServerID = @ServerID)
END