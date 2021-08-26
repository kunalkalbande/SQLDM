
--SQLdm 9.0 --(Ankit Srivastava) --New procedure to get if grooming have been timed out
--EXEC [p_GetGroomingStatusInfo] 37
IF (object_id('p_GetGroomingStatusInfo') IS NOT NULL)
BEGIN
DROP PROCEDURE p_GetGroomingStatusInfo
END
GO

CREATE PROCEDURE [dbo].[p_GetGroomingStatusInfo]
(
	@SQLServerID INT
)
AS 
BEGIN

	DECLARE @Run_Id uniqueidentifier
	 
	IF (EXISTS(Select * FROM [LatestGroomingStatus] where ([SQLServerID]= @SQLServerID OR [SQLServerID] IS NULL) AND [Status]=0 AND [IsPrimary]=1 AND [LastStatusMessage] like '%Timeout%' )) 
	BEGIN
		SELECT @Run_Id=[GroomingRunID] FROM [LatestGroomingStatus] where [SQLServerID]= @SQLServerID AND [Status]=0 AND [LastStatusMessage] like '%Timeout%'
		
		SELECT Convert(bit,1) -- it timed out
		
		SELECT MSS.[InstanceName]
		FROM [LatestGroomingStatus] LGS
		RIGHT JOIN [MonitoredSQLServers] MSS ON LGS.SQLServerID=MSS.SQLServerID
		where [GroomingRunID]=@Run_Id AND [IsPrimary]=0
	END	
END
