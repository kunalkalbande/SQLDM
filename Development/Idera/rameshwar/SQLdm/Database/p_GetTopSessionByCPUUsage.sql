-- SQLDM 8.5:Mahesh : Added for Rest service consumption
if (object_id('p_GetSessionByCPUUsage') is not null)
begin
drop procedure p_GetSessionByCPUUsage
end
go

CREATE PROCEDURE [dbo].[p_GetSessionByCPUUsage](

	@ServerId int = null,
	@StartDateTime datetime = null,
	@EndDateTime datetime = null,
	@TopN bigint = 10
)

AS

begin

	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	declare @e int

	set rowcount @TopN

				select M.SQLServerID
				,M.InstanceName
				,D.DatabaseName
				,D.DatabaseID
				,QS.SessionID
				,max(QS.CPUMilliseconds) as CPUMilliseconds
				,H.HostName
				from 
				QueryMonitorStatistics QS (NOLOCK)
				Join MonitoredSQLServers M (NOLOCK)
				on M.SQLServerID = QS.SQLServerID
				and M.Active = 1
				Join SQLServerDatabaseNames D (nolock)
				on D.DatabaseID = QS.DatabaseID
				join HostNames H (NOLOCK)
				on H.HostNameID = QS.HostNameID
				Where QS.SessionID IS NOT NULL
				AND QS.UTCCollectionDateTime between ISNULL(@StartDateTime,QS.UTCCollectionDateTime) and ISNULL(@EndDateTime,QS.UTCCollectionDateTime) 
				Group by M.SQLServerID
				,M.InstanceName
				,D.DatabaseName
				,D.DatabaseID
				,QS.SessionID
				,H.HostName
				Having M.SQLServerID = ISNULL(@ServerId,M.SQLServerID)
				Order by CPUMilliseconds DESC

	SELECT @e = @@error

	RETURN @e

END	 

GO


