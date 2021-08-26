IF (object_id('p_GetServersWithTagId') IS NOT NULL)
BEGIN
DROP PROCEDURE p_GetServersWithTagId
END
GO

CREATE PROCEDURE [dbo].[p_GetServersWithTagId]
(
	@TagId INT,
	@addSelectRequest bit = 0
)
AS
BEGIN
	create table #SecureMonitoredSQLServers(InstanceName nvarchar(256),SQLServerID int)

	insert into #SecureMonitoredSQLServers
	exec [p_GetReportServers]
	
	if @addSelectRequest=1
	begin
		if @TagId = 0
		begin
			SELECT 0 as 'SQLServerId','< All Servers >' as 'InstanceName'
			union
			SELECT ms.[SQLServerID], ms.[InstanceName]
			FROM #SecureMonitoredSQLServers ms (nolock)
		end
	else 
        begin
			SELECT 0 as 'SQLServerId','< All Servers >' as 'InstanceName'
			union
			SELECT ms.[SQLServerID], [InstanceName]
			FROM [ServerTags] st (nolock)
			LEFT JOIN #SecureMonitoredSQLServers ms (nolock)
			ON st.SQLServerId = ms.SQLServerID
			WHERE st.[TagId] = @TagId
	     end
	end 
	else
		SELECT ms.[SQLServerID], [InstanceName]
		FROM [ServerTags] st (nolock)
		LEFT JOIN #SecureMonitoredSQLServers ms (nolock)
		ON st.SQLServerId = ms.SQLServerID
		WHERE st.[TagId] = @TagId
END
