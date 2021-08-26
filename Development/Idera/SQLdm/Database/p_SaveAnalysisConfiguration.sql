if (object_id('p_SaveAnalysisConfiguration') is not null)
begin
drop procedure [p_SaveAnalysisConfiguration]
end
go


create procedure [p_SaveAnalysisConfiguration] 
@sqlServerID INT,
@productionServer bit,
@OLTP bit,
@analysisStartTime DateTime,
@duration INT,
@scheduledDays smallint,
@isActive bit,
@includeDatabase int,
@filterApplication nvarchar(max),
@categoryID xml,
@databaseID xml,
@recommendationID xml,
@schedulingStatus bit --SQLDM10.0 (praveen suhalka) : scheduling status
AS
begin

DECLARE  @analysisConfigID INT
DECLARE @checkXML INT 
--  BEGIN TRY
   -- Start A Transaction
   --srishti Purohit -- parent transaction will commit this change also
--BEGIN TRANSACTION

--BEGIN

--update records having IsActive true for this server id
UPDATE [dbo].[AnalysisConfiguration]
SET IsActive = 0
WHERE MonitoredServerID = @sqlServerID


			-- insert analysis record in table
			-- insert new record in [PrescriptiveAnalysis] table
			INSERT INTO [dbo].[AnalysisConfiguration] -- This hint locks the whole table  WITH(TABLOCK)
				   ([MonitoredServerID]
           ,[ProductionServer]
           ,[OLTP]
           ,[StartTime]
		   ,[Duration]
           ,[ScheduledDays]
           ,[IncludeDatabase]
           ,[IsActive]
           ,[FilterApplication]
		   ,[SchedulingStatus])   --SQLDM10.0 (praveen suhalka) : scheduling status
			 VALUES ( 
				 @sqlServerID,
				 @productionServer,
				@OLTP ,
				@analysisStartTime ,
				@duration,
				@scheduledDays ,
				@includeDatabase ,
				@isActive ,
				@filterApplication,
				@schedulingStatus   --SQLDM10.0 (praveen suhalka) : scheduling status
			 )
	
			
			--SET @analysisID = SCOPE_IDENTITY()
	SET @analysisConfigID = (CASE WHEN @@ROWCOUNT > 0 THEN SCOPE_IDENTITY() ELSE -1 END)
		if(@analysisConfigID > 0)
		BEGIN
		SET @checkXML = @categoryID.exist('(//CategoryID)')
			if( @checkXML != 0)
			BEGIN
			
				INSERT INTO [dbo].[AnalysisConfigCategories]
			   ([AnalysisConfigurationID]
			   ,[CategoryID])			
				--(SELECT @analysisConfigID, A.B.value('(CategoryID)[1]', 'int' ) ID
				--		FROM    @categoryID.nodes('/Category') A(B))
						(select @analysisConfigID, x.record.query('ID').value('.', 'int')
						from @categoryID.nodes('Category/CategoryID') as x(record))
						OPTION (LOOP JOIN)

			END
			SET @checkXML = @databaseID.exist('(//DatabaseID)')
			if( @checkXML != 0)
			BEGIN
			
					  INSERT INTO [dbo].[AnalysisConfigBlockedDatabases]
			  ([AnalysisConfigurationID]
			   ,[DatabaseID])
				--(SELECT @analysisConfigID,T.Item.value('DatabaseID[1]', 'int')
				--FROM   @databaseID.nodes('/Database') AS T(Item))
				(select @analysisConfigID, x.record.query('ID').value('.', 'int')
						from @databaseID.nodes('Database/DatabaseID') as x(record))
			END
			SET @checkXML = @recommendationID.exist('(//RecommendationID)')
			if( @checkXML != 0)
			BEGIN
			
				INSERT INTO [dbo].[AnalysisConfigBlockedRecommendation]
			   ([AnalysisConfigurationID]
			   ,[RecommendationID])
				--(SELECT @analysisConfigID,T.Item.value('RecommendationID[1]', 'nvarchar(10)')
				--FROM   @recommendationID.nodes('/Recommendation') AS T(Item))
				(select @analysisConfigID, x.record.query('ID').value('.', 'nvarchar(10)')
						from @recommendationID.nodes('Recommendation/RecommendationID') as x(record))
			END
	--END

	--COMMIT
	END
/*
END TRY
BEGIN CATCH
 Print 'Transaction Failed - Will Rollback'
  -- Any Error Occurred during Transaction. Rollback
  --ROLLBACK  -- Roll back
  		  
		  SET @analysisConfigID = -1
  RAISERROR ('Error while saving configuration changes.',
             16,
             1)
END CATCH
*/

end
 
GO 
