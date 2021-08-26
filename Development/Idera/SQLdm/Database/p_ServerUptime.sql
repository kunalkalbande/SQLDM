if (object_id('p_ServerUptime') is not null)
begin
drop procedure [p_ServerUptime]
end
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create procedure  [dbo].[p_ServerUptime]
@UTCStart DateTime,
@UTCEnd DateTime,
@Servername Varchar(256) = null
AS
Begin
-- Retrieve all alerts that specify the Server is down between the specified time period

create table #ServerInfo(
    ServerName nvarchar(256),
    UTCOccurrenceDateTime DateTime,	
	Value float NULL
)

if @Servername is null
begin
	insert into #ServerInfo(ServerName,UTCOccurrenceDateTime,Value)
		select ServerName COLLATE DATABASE_DEFAULT ,UTCOccurrenceDateTime,Value from Alerts		
end		
else
begin
	insert into #ServerInfo(ServerName,UTCOccurrenceDateTime,Value)
		select ServerName COLLATE DATABASE_DEFAULT,UTCOccurrenceDateTime,Value from Alerts where ServerName Collate DATABASE_DEFAULT = @Servername COLLATE DATABASE_DEFAULT
end;

WITH cteAlert AS
(
            SELECT ServerName = ServerName COLLATE DATABASE_DEFAULT,UTCOccurrenceDateTime,'0' as Status
      FROM #ServerInfo
     WHERE Value IN (4,9) 
               AND UTCOccurrenceDateTime BETWEEN @UTCStart AND @UTCEnd
 )
 
-- For all servers with alerts grab all the collection datetime values when the server was up
,cteUptime AS
(
            SELECT ms.InstanceName as ServerName
                           ,ss.UTCCollectionDateTime
                           ,'1' as Status
              FROM ServerStatistics ss
              INNER JOIN MonitoredSQLServers ms
                 ON ss.SQLServerID = ms.SQLServerID 
                        AND ss.UTCCollectionDateTime BETWEEN @UTCStart AND @UTCEnd 
              INNER JOIN cteAlert ca
                 ON ca.ServerName collate DATABASE_DEFAULT = ms.InstanceName  collate DATABASE_DEFAULT
 
)
 
-- Join the downtime datetime values with the uptime datetime values
,cteMain AS 
(
            SELECT * FROM cteAlert
            UNION
            SELECT * FROM cteUptime
)
 
-- Sort and dedupe, there appears to be a collection datetime at the exact time there was an alert for server being down.
,cteSort as
(
            select ServerName
                           ,UTCOccurrenceDateTime
                           ,Status
                           ,ROW_NUMBER() OVER (PARTITION BY ServerName,UTCOccurrenceDateTime ORDER BY  Status) as rownum
              from cteMain
 
)
,cteDedupe as
(
            SELECT *
              FROM cteSort
            WHERE rownum = 1
 
)
 
-- Calculate the time difference between the datetime values in minutes 
,rows AS
        (
        SELECT  ServerName,UTCOccurrenceDateTime,Status,  ROW_NUMBER() OVER (PARTITION BY ServerName ORDER BY UTCOccurrenceDateTime) AS rn
        FROM    cteDedupe
        )

, diff AS (
		SELECT  mc.ServerName, mc.UTCOccurrenceDateTime, mc.Status,DATEDIFF(MI, mc.UTCOccurrenceDateTime, mp.UTCOccurrenceDateTime) as TimeDiff
FROM    rows mc
JOIN    rows mp
ON      mc.rn = mp.rn - 1
)
--Sum the uptime differences and the downtime differences.

	SELECT ServerName
               ,CASE Status WHEN 0 THEN 'Down' 
                                                    WHEN 1 THEN 'Up'
                             ELSE 'UNKNOWN' END AS Status
               ,SUM(TimeDiff) as 'TotalTime (min)'
  FROM diff
GROUP BY ServerName, Status 
ORDER BY ServerName, Status
    END