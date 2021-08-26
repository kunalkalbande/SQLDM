if (object_id('p_GetAllMetricsAlertsName') is not null)
begin
drop procedure [p_GetAllMetricsAlertsName]
end
go

CREATE procedure [dbo].[p_GetAllMetricsAlertsName]
(
	@SQLServerListID nvarchar(MAX)
)
AS
BEGIN
	DECLARE @err INT
	DECLARE @Now datetime
	DECLARE @ServerList TABLE(ServerId INT)
	DECLARE @xmlDoc INT 

	IF (@SQLServerListID IS NOT NULL)
	BEGIN
		EXEC sp_xml_preparedocument @xmlDoc OUTPUT, @SQLServerListID

		INSERT INTO @ServerList	
		SELECT ServerId
			FROM openxml(@xmlDoc, '//Server', 1) WITH (ServerId INT) 

		EXEC sp_xml_removedocument @xmlDoc
	END
	
	SET @Now = GetUTCDate()
	
	SELECT DISTINCT MT.[Metric],MI.[Name] 
		FROM MetricThresholds MT (nolock) 
		INNER JOIN MetricInfo MI (nolock) ON MI.[Metric] = MT.[Metric]
	WHERE
	(MT.SQLServerID IN(SELECT ServerId FROM @ServerList)) AND
	(UTCSnoozeEnd > @Now)
	
	SELECT @err = @@error

	RETURN @err
END
 