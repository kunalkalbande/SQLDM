IF (object_id('p_GetMonitoredSqlServersUsingCounter') IS NOT NULL)
BEGIN
DROP PROCEDURE p_GetMonitoredSqlServersUsingCounter
END
GO

CREATE PROCEDURE [dbo].[p_GetMonitoredSqlServersUsingCounter]
(
	@Metric int
)
AS
BEGIN
	DECLARE @error INT
	
	-- custom counters from tags
	SELECT ST.SQLServerId
		FROM [CustomCounterTags] CCT
			JOIN [ServerTags] ST on CCT.TagId = ST.TagId
		WHERE CCT.[Metric] = @Metric
	UNION 
	-- directly associated counters
	SELECT SQLServerID 
		FROM CustomCounterMap
		WHERE Metric = @Metric

	SELECT @error = @@error
	RETURN @error
END