-- SQLdm 10.0 (Srishti Purohit)

-- Prescriptive Analysis
-- Get Snapshot Values for server which is need to generate SDR-M16 Recomm
-- Details are stored and used every time analysis is triggered
if (object_id('p_GetPrescriptiveSnapshotValuesForServer') is not null)
begin
drop procedure [p_GetPrescriptiveSnapshotValuesForServer]
end
go

create procedure [p_GetPrescriptiveSnapshotValuesForServer] 

@MonitoredServerID INT
AS
BEGIN
	
--For excepttion handling
DECLARE
  @ErrorMessage   varchar(2000)
 ,@ErrorSeverity  tinyint
 ,@ErrorState     tinyint
	
	BEGIN
		-- check if records for UserSID
		SELECT [ActiveNetworkCards]
		, [TotalNetworkBandwidth]
      ,[AllowedProcessorCount]
	  ,[TotalNumberOfLogicalProcessors]
      ,[TotalMaxClockSpeed]
      ,[TotalPhysicalMemory]
	  ,[MaxServerMemory]
	  ,[WindowsVersion]
	  ,[ProductVersion]
	  ,[SQLVersionString]
		FROM [PrescriptiveAnalysisSnapshotValuesPrevious] 
		WHERE [MonitoredServerID] = @MonitoredServerID
	
	END
		
	
END
 
GO 
