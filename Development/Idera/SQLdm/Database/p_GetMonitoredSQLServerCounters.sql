if (object_id('p_GetMonitoredSQLServerCounters') is not null)
begin
drop procedure p_GetMonitoredSQLServerCounters
end
go

CREATE PROCEDURE [dbo].p_GetMonitoredSQLServerCounters(
	@SQLServerID int,
	@IncludeTagged bit = 0
)
AS
begin
	declare @e int

	if (@IncludeTagged = 0) 
		SELECT SQLServerID, Metric from CustomCounterMap
			WHERE (@SQLServerID is null or [SQLServerID] = @SQLServerID)
			ORDER BY SQLServerID
	else
		-- include tagged counters also
		SELECT SQLServerID, Metric from CustomCounterMap
			WHERE (@SQLServerID is null or [SQLServerID] = @SQLServerID)
		UNION
		SELECT ST.SQLServerId, CCT.Metric from ServerTags ST
			JOIN CustomCounterTags CCT on ST.TagId = CCT.TagId
			LEFT OUTER JOIN CustomCounterMap CCM on ST.SQLServerId = CCM.SQLServerID and CCT.Metric = CCM.Metric
			WHERE (@SQLServerID is null or ST.[SQLServerId] = @SQLServerID)
				   and CCM.SQLServerID is null
		ORDER BY SQLServerID

	SET @e = @@ERROR

	RETURN @e
end