-- SQLdm 10.0 (Srishti Purohit)

-- Prescriptive Analysis
-- Save Snapshot Values for server which is need to generate SDR-M16 Recomm
-- Details are stored and used every time analysis is triggered
if (object_id('p_UpsertPrescriptiveSnapshotValues') is not null)
begin
drop procedure [p_UpsertPrescriptiveSnapshotValues]
end
go

create procedure [p_UpsertPrescriptiveSnapshotValues] 
@MonitoredServerID INT,
		 @ActiveNetworkCards INT,
         @TotalNetworkBandwidth BIGINT,
         @AllowedProcessorCount INT,
         @TotalNumberOfLogicalProcessors BIGINT ,
        @TotalMaxClockSpeed BIGINT ,
         @TotalPhysicalMemory BIGINT,
         @MaxServerMemory BIGINT, 
		 @WindowsVersion nvarchar(100),
		 @ProductVersion nvarchar(100),
		 @SQLVersionString nvarchar(100)
as
begin


BEGIN TRY
   -- Start A Transaction
   BEGIN TRANSACTION
   
	
	--set @recordTimestamp= CURRENT_TIMESTAMP
	-- update if dashboarrd widget exists.
	if EXISTS(select [ActiveNetworkCards] from [PrescriptiveAnalysisSnapshotValuesPrevious] WHERE [MonitoredServerID] = @MonitoredServerID)
		begin
				update [dbo].[PrescriptiveAnalysisSnapshotValuesPrevious]
				set [ActiveNetworkCards] = @ActiveNetworkCards
		, [TotalNetworkBandwidth] = @TotalNetworkBandwidth
      ,[AllowedProcessorCount] = @AllowedProcessorCount
	  ,[TotalNumberOfLogicalProcessors] = @TotalNumberOfLogicalProcessors
      ,[TotalMaxClockSpeed] = @TotalMaxClockSpeed
      ,[TotalPhysicalMemory] = @TotalPhysicalMemory
	  ,[MaxServerMemory] = @MaxServerMemory
	  ,[WindowsVersion] = @WindowsVersion
	  ,[ProductVersion] = @ProductVersion
	  ,[SQLVersionString] = @SQLVersionString
				WHERE [MonitoredServerID] = @MonitoredServerID
			END
			ELSE
			BEGIN
			INSERT INTO [dbo].[PrescriptiveAnalysisSnapshotValuesPrevious]
			([MonitoredServerID]
           ,[ActiveNetworkCards]
           ,[TotalNetworkBandwidth]
           ,[AllowedProcessorCount]
           ,[TotalNumberOfLogicalProcessors]
           ,[TotalMaxClockSpeed]
           ,[TotalPhysicalMemory]
           ,[MaxServerMemory]
           ,[WindowsVersion]
           ,[ProductVersion]
           ,[SQLVersionString])
     VALUES
           (@MonitoredServerID,
		   @ActiveNetworkCards
		,@TotalNetworkBandwidth
      ,@AllowedProcessorCount
	  ,@TotalNumberOfLogicalProcessors
      ,@TotalMaxClockSpeed
      ,@TotalPhysicalMemory
	  ,@MaxServerMemory
	  ,@WindowsVersion
	  ,@ProductVersion
	  ,@SQLVersionString
		   )
		   		END
COMMIT

END TRY
BEGIN CATCH
 Print 'Transaction Failed - Will Rollback'
  -- Any Error Occurred during Transaction. Rollback
  ROLLBACK  -- Roll back
END CATCH

end
 
GO 
