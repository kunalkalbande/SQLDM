if (object_id('p_GetPrescriptiveAnalysisSchedule') is not null)
begin
drop procedure [p_GetPrescriptiveAnalysisSchedule]
end
go

-- SQLdm 10.0 Praveen Suhalka Doctor implementation in DM
create procedure [p_GetPrescriptiveAnalysisSchedule] 
AS
BEGIN
	
--For excepttion handling
DECLARE
  @ErrorMessage   varchar(2000)
 ,@ErrorSeverity  tinyint
 ,@ErrorState     tinyint

	SELECT 
			MonitoredServerID, 
			StartTime, 
			ScheduledDays 
	FROM 
			AnalysisConfiguration (NOLOCK)
			
	WHERE 
			IsActive	=	1
			AND 
			ScheduledDays > 0;

	IF(@@ERROR <>0)
	BEGIN
			SET @ErrorMessage  = ERROR_MESSAGE()
			SET @ErrorSeverity = ERROR_SEVERITY()
			SET @ErrorState    = ERROR_STATE()
			RAISERROR('Unable to get PrescriptiveAnalysisSchedule.', @ErrorSeverity, @ErrorState)

	END
END
 
GO 
