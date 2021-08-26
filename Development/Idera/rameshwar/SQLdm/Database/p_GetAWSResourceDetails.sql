IF (object_id('[p_GetAWSResourceDetails]') IS NOT NULL)
BEGIN
DROP PROCEDURE [p_GetAWSResourceDetails]
END
GO

CREATE PROCEDURE [dbo].[p_GetAWSResourceDetails]
(
	@InstanceName NVARCHAR(256) = NULL
)
AS
BEGIN
	
	SELECT	aws_access_key,
			aws_secret_key,
			aws_region_endpoint
	FROM [dbo].[MonitoredSQLServers]
	WHERE InstanceName = @InstanceName

END

GO