-- SQLDM 8.5:Mahesh : Added for Rest service consumption
if (object_id('p_GetAlert') is not null)
begin
drop procedure p_GetAlert
end
go

CREATE PROCEDURE [dbo].[p_GetAlert](
	@AlertID bigint	
)
AS
begin
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	declare @e int
	
	
  	SELECT	A.[AlertID],A.[UTCOccurrenceDateTime],COALESCE(mss.[FriendlyServerName],mss.[InstanceName]) AS ServerName,--SQLdm 10.1 - (Pulkit Puri) - to add friendly server name
  	A.[DatabaseName],A.[TableName],
					A.[Active],A.[Metric],A.[Severity],A.[StateEvent],A.[Value],A.[Heading],A.[Message], mss.SQLServerID
			FROM [Alerts] A (NOLOCK)
			LEFT OUTER JOIN MonitoredSQLServers mss (NOLOCK) ON A.ServerName = mss.InstanceName
			WHERE A.AlertID = @AlertID
	SELECT @e = @@error

	RETURN @e

END	 

GO


