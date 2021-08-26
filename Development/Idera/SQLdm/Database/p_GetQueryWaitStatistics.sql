
--Description : Added by Vineet Kumar in SQLDM 8.6 release For a new report Query Wait Statistics Report.

--This procedure fetches the Query Wait Statistics based on given filter criteria.

----Sample Execution


--DECLARE	@return_value int

--EXEC	@return_value = [dbo].[p_GetQueryWaitStatistics]
--		@SQLServerID = 9,
--		@DatabaseName = N'master',
--		@UTCStart = N'15-jul-2014',
--		@UTCEnd = N'16-jul-2014',
--		@UTCOffset = 0,
--		@WaitCategoryID = 9

--SELECT	'Return Value' = @return_value

--GO


if (object_id('p_GetQueryWaitStatistics') is not null)
begin
drop procedure p_GetQueryWaitStatistics
end
go
CREATE PROCEDURE [dbo].[p_GetQueryWaitStatistics]
	@SQLServerID int,
	@DatabaseName varchar(max),
	@UTCStart DateTime,
	@UTCEnd DateTime,
	@UTCOffset int = 0,
	@WaitCategoryID int = null
AS
begin

--Start : Getting SQL Servers as per user access
declare @SQLServers table(
		SQLServerID int,
		InstanceName nvarchar(256))

create table #SecureMonitoredSQLServers(InstanceName nvarchar(256),SQLServerID int)

insert into #SecureMonitoredSQLServers
exec [p_GetReportServers]
	
	insert into @SQLServers
		select smss.SQLServerID, smss.InstanceName
		from #SecureMonitoredSQLServers smss 
			inner join MonitoredSQLServers mss (nolock) on mss.SQLServerID = smss.SQLServerID
		where Active = 1 AND smss.SQLServerID = @SQLServerID
--END : Getting SQL Servers as per user access


IF(@WaitCategoryID =0)
BEGIN
	select ss.InstanceName,  an.ApplicationName, dbn.DatabaseName,
	wt.WaitType,aws.StatementUTCStartTime, aws.WaitDuration,ln.LoginName  from ActiveWaitStatistics aws
	INNER JOIN ApplicationNames an ON an.ApplicationNameID = aws.ApplicationNameID
	INNER JOIN WaitTypes wt ON wt.WaitTypeID = aws.WaitTypeID
	INNER JOIN WaitCategories wc ON wc.CategoryID = wt.CategoryID
	INNER JOIN SQLServerDatabaseNames dbn ON dbn.DatabaseID = aws.DatabaseID
	INNER JOIN LoginNames ln ON ln.LoginNameID = aws.LoginNameID
	INNER JOIN @SQLServers ss ON ss.SQLServerID = aws.SQLServerID
	WHERE aws.[UTCCollectionDateTime] between @UTCStart and @UTCEnd 
	AND aws.SQLServerID = @SQLServerID AND dbn.DatabaseName = @DatabaseName
END
ELSE
BEGIN
	select ss.InstanceName,  an.ApplicationName, dbn.DatabaseName,
	wt.WaitType,aws.StatementUTCStartTime, aws.WaitDuration,ln.LoginName  from ActiveWaitStatistics aws
	INNER JOIN ApplicationNames an ON an.ApplicationNameID = aws.ApplicationNameID
	INNER JOIN WaitTypes wt ON wt.WaitTypeID = aws.WaitTypeID
	INNER JOIN WaitCategories wc ON wc.CategoryID = wt.CategoryID
	INNER JOIN SQLServerDatabaseNames dbn ON dbn.DatabaseID = aws.DatabaseID
	INNER JOIN LoginNames ln ON ln.LoginNameID = aws.LoginNameID
	INNER JOIN @SQLServers ss ON ss.SQLServerID = aws.SQLServerID
	WHERE aws.[UTCCollectionDateTime] between @UTCStart and @UTCEnd 
	AND aws.SQLServerID = @SQLServerID AND dbn.DatabaseName = @DatabaseName
	AND wc.CategoryID = @WaitCategoryID
END
end

