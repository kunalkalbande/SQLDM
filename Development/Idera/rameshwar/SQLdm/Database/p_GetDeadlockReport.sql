-- SQLdm 10.2.1 (Varun Chopra)

-- [p_GetDeadlockReport] Gets the deadloc report for the selected date range

--DECLARE @return_value int

--EXEC @return_value = [dbo].[p_GetDeadlockReport]
--  @SQLServerID = 1,
--  @DatabaseName = N'SQLdmRepository',
--  @UTCStart = N'03/07/2015',
--  @UTCEnd = N'03/07/2017',
--  @UTCOffset = 330

--SELECT 'Return Value' = @return_value


if (object_id('p_GetDeadlockReport') is not null)
BEGIN
	DROP PROCEDURE [p_GetDeadlockReport]
END
GO

CREATE PROCEDURE [dbo].[p_GetDeadlockReport]
	@SQLServerID INT,
	@DatabaseName varchar(max),
	@UTCStart DateTime = null,
	@UTCEnd DateTime = null,
	@UTCOffset int = 0
AS 
BEGIN

if @UTCOffset is null
		set @UTCOffset = datediff(mi,getutcdate(),getdate())

if @UTCStart is null
		set @UTCStart = GETDATE()

if @UTCEnd is null
		set @UTCEnd = GETDATE()

select * from (
select DATEADD(mi, @UTCOffset, dlp.UTCCollectionDateTime) as CollectionDateTime,
       lgnn.LoginName,
	   hn.HostName,
	   ss.SQLStatement,
	   dl.XDLData,
	   apln.ApplicationName,
	   mss.InstanceName,
	   ssdn.DatabaseName, 
	   ROW_NUMBER() OVER (PARTITION BY dlp.DeadlockID ORDER BY dlp.UTCCollectionDateTime) AS Rnk
from DeadlockProcesses dlp
inner join Deadlocks dl
on dlp.DeadlockID = dl.DeadlockID
inner join SQLStatements ss
on dlp.SQLStatementID = ss.SQLStatementID
inner join MonitoredSQLServers mss
on dl.SQLServerID = mss.SQLServerID
inner join SQLServerDatabaseNames ssdn
on dlp.DatabaseID = ssdn.DatabaseID
inner join ApplicationNames apln
on dlp.ApplicationNameID = apln.ApplicationNameID
inner join LoginNames lgnn
on dlp.LoginNameID = lgnn.LoginNameID
inner join HostNames hn
on dlp.HostNameID = hn.HostNameID
where mss.SQLServerID = @SQLServerID
and ssdn.DatabaseName = @DatabaseName
and dlp.UTCOccurrenceDateTime BETWEEN @UTCStart and @UTCEnd
) x
where x.Rnk = 1
order by CollectionDateTime desc

END