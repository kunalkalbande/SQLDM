--10.1 SQLdm Health Index
--Srishti Purohit

IF (object_id('[p_GetScaleFactors]') IS NOT NULL)
BEGIN
DROP PROCEDURE [p_GetScaleFactors]
END
GO

CREATE PROCEDURE [dbo].[p_GetScaleFactors]
AS
BEGIN
	DECLARE @error int

	SELECT ID, HealthIndexCoefficientName, HealthIndexCoefficientValue FROM HealthIndexCofficients

	--Instance specific details
	SELECT SQLServerID,
	COALESCE([FriendlyServerName],[InstanceName]) AS [InstanceName]--SQLdm 10.1 - (Pulkit Puri) - to add friendly server name
,Active,
--START: SQLdm 10.1 (srisht purohit) -New table to make health index global for all instance
--HealthIndexCoefficientForCriticalAlert,
--HealthIndexCoefficientForWarningAlert,
--HealthIndexCoefficientForInformationalAlert ,
--END: SQLdm 10.1 (srisht purohit) -New table to make health index global for all instance
InstanceScaleFactor FROM MonitoredSQLServers
WHERE Active = 1 AND Deleted = 0 -- [SQLdm10.1] Defect fix SQLDM-25532
	--Tag specific details
	SELECT Id,
Name,
TagScaleFactor FROM Tags

	SELECT @error = @@error

	IF @error != 0 GOTO HANDLE_ERROR

	RETURN(0)

	HANDLE_ERROR:
		RAISERROR('An error occurred while getting the monitored SQL Server Scale factor data.', 10, 1)
        RETURN(@error)			
END
GO