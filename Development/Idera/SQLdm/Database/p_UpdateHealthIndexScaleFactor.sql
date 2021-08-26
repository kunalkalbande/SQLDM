--10.1 SQLdm Health Index
--Srishti Purohit
-- Expected Data in format
--<Root><Instance ID="0" CriticalScaleFactor="6" InformationalScaleFactor="1" WarningScaleFactor="2" InstanceScaleFactor="" /></Root>
--<Root><Tag ID="0" TagScaleFactor="" /></Root>

if (object_id('p_UpdateHealthIndexScaleFactor') is not null)
begin
drop procedure [p_UpdateHealthIndexScaleFactor]
end
go


create procedure [p_UpdateHealthIndexScaleFactor] 
@healthIndexCoefficientForCriticalAlert DECIMAL(16,2),--SQLdm 10.1 (Pulkit Puri) value must be DECIMAL
@healthIndexCoefficientForWarningAlert DECIMAL(16,2),--SQLdm 10.1 (Pulkit Puri) value must be DECIMAL
@healthIndexCoefficientForInformationalAlert DECIMAL(16,2),--SQLdm 10.1 (Pulkit Puri) value must be DECIMAL
@instanceHealthIndex xml,
@tagHealthIndex xml
AS
begin

DECLARE @checkXML INT 
BEGIN TRY
   -- Start A Transaction
BEGIN TRANSACTION

BEGIN
            --SQLdm 10.1 (pulkit puri)--defect fix for SQLDM- 25912(start)
			--UPDATION OF [HealthIndexCofficients] has nothing to do with value of xml
            --Update Helath index global table record
			--Critical Index
				UPDATE [dbo].[HealthIndexCofficients]
			  SET HealthIndexCoefficientValue = @healthIndexCoefficientForCriticalAlert, UTCLastUpdatedDateTime = GETUTCDATE() WHERE ID=1

			  --Update Helath index global table record
			--Warning Index
				UPDATE [dbo].[HealthIndexCofficients]
			  SET HealthIndexCoefficientValue = @healthIndexCoefficientForWarningAlert, UTCLastUpdatedDateTime = GETUTCDATE() WHERE ID=2

			  --Update Helath index global table record
			--Informational Index
				UPDATE [dbo].[HealthIndexCofficients]
			  SET HealthIndexCoefficientValue = @healthIndexCoefficientForInformationalAlert, UTCLastUpdatedDateTime = GETUTCDATE() WHERE ID=3
			   --SQLdm 10.1 (pulkit puri)--defect fix for SQLDM- 25912(end)

			SET @checkXML = @instanceHealthIndex.exist('(//Instance)')
			if( @checkXML != 0)
			BEGIN
        	-- Creating Temporary Table
			CREATE TABLE #TEMP_TABLE
			(
				[InstanceId] INT,
				--[HealthIndexCoefficientForCriticalAlert] INT,
				--[HealthIndexCoefficientForWarningAlert] INT,
				--[HealthIndexCoefficientForInformationalAlert] INT,
				[InstanceScaleFactor] DECIMAL(16,2)--SQLdm 10.1 (Pulkit Puri) value must be DECIMAL
			);
			--Insert record in temp table
			INSERT INTO [dbo].[#TEMP_TABLE]
			   ([InstanceId],
			   --[HealthIndexCoefficientForCriticalAlert],
			   --[HealthIndexCoefficientForWarningAlert],
			   --[HealthIndexCoefficientForInformationalAlert],
			   [InstanceScaleFactor])
				(SELECT x.record.value('@ID', 'INT'),
						--x.record.value('@CriticalScaleFactor', 'INT'),
						--x.record.value('@WarningScaleFactor', 'INT'),
						--x.record.value('@InformationalScaleFactor', 'INT'),
						CASE x.record.value('@InstanceScaleFactor', 'nvarchar(max)') WHEN '' 
						THEN NULL
						ELSE 
						x.record.value('@InstanceScaleFactor', 'FLOAT') END
						from @instanceHealthIndex.nodes('Root/Instance') as x(record))

			--Update record
				UPDATE [dbo].[MonitoredSQLServers]
			  SET 
			  --HealthIndexCoefficientForCriticalAlert= TT.HealthIndexCoefficientForCriticalAlert
			  -- ,HealthIndexCoefficientForWarningAlert = TT.HealthIndexCoefficientForWarningAlert,
			  -- HealthIndexCoefficientForInformationalAlert = TT.HealthIndexCoefficientForInformationalAlert,
			   InstanceScaleFactor = TT.InstanceScaleFactor
				from [#TEMP_TABLE] TT WHERE SQLServerID = TT.[InstanceId]

			--Drop Temp table
			DROP TABLE #TEMP_TABLE
			END
			
			SET @checkXML = @tagHealthIndex.exist('(//Tag)')
			if( @checkXML != 0)
			BEGIN
			
				-- Creating Temporary Table
			CREATE TABLE #TEMP_TABLE2
			(
				[TagId] INT,
				[TagScaleFactor] DECIMAL(16,2)--SQLdm 10.1 (Pulkit Puri) value must be DECIMAL
			);
			--Insert record in temp table
			INSERT INTO [dbo].[#TEMP_TABLE2]
			   ([TagId],
			   [TagScaleFactor])
				(SELECT x.record.value('@ID', 'INT'),
				CASE x.record.value('@TagScaleFactor', 'nvarchar(max)') WHEN '' 
					THEN NULL
					ELSE 
						x.record.value('@TagScaleFactor', 'FLOAT') END
						from @tagHealthIndex.nodes('Root/Tag') as x(record))

			--Update record
				UPDATE [dbo].[Tags]
			  SET TagScaleFactor= TT.[TagScaleFactor]
				from [#TEMP_TABLE2] TT WHERE Id = TT.[TagId]

				--Drop Temp table
			DROP TABLE #TEMP_TABLE2
			END

COMMIT
END
END TRY
BEGIN CATCH
 Print 'Error while updating health factors.'
  -- Any Error Occurred during Transaction. Rollback
  ROLLBACK  -- Roll back
  		  
  RAISERROR ('Error while updating health factors.',
             16,
             1)
END CATCH

end
 
GO 