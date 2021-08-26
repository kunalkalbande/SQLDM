IF (object_id('p_GetTagConfiguration') IS NOT NULL)
BEGIN
DROP PROCEDURE p_GetTagConfiguration
END
GO

CREATE PROCEDURE [dbo].[p_GetTagConfiguration]
(
	@TagId INT
)
AS
BEGIN
	-- Get Servers
	SELECT [SQLServerId], [InstanceName]
	FROM [ServerTags] st
	LEFT JOIN [MonitoredSQLServers] ms
	ON st.SQLServerId = ms.SQLServerID
	WHERE [TagId] = @TagId

	-- Get Custom Counters
	SELECT [Metric]
	FROM [CustomCounterTags]
	WHERE [TagId] = @TagId

	-- Get Permissions
	SELECT [PermissionId]
	FROM [PermissionTags]
	WHERE [TagId] = @TagId
END