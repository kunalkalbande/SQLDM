
if (object_id('p_UpdateBlockedRecommendationDatabaseAnalysisConfiguration') is not null)
begin
drop procedure [p_UpdateBlockedRecommendationDatabaseAnalysisConfiguration]
end
go


create procedure [p_UpdateBlockedRecommendationDatabaseAnalysisConfiguration] 
@sqlServerID INT,
@databaseID xml,
@recommendationID xml
AS
begin

DECLARE @checkXML INT 
DECLARE @analysisConfigID INT 
BEGIN TRY
   -- Start A Transaction
BEGIN TRANSACTION

BEGIN
--get record having IsActive true for this server id
SELECT @analysisConfigID = ID FROM [dbo].[AnalysisConfiguration] (NOLOCK)
WHERE MonitoredServerID = @sqlServerID AND IsActive = 1

if(@analysisConfigID > 0)
BEGIN

	-- delete all existting records for that config
	delete from AnalysisConfigBlockedDatabases WHERE AnalysisConfigurationID =@analysisConfigID
	delete from AnalysisConfigBlockedRecommendation WHERE AnalysisConfigurationID =@analysisConfigID

			SET @checkXML = @databaseID.exist('(//DatabaseID)')
			if( @checkXML != 0)
			BEGIN
				INSERT INTO [dbo].[AnalysisConfigBlockedDatabases]
			  ([AnalysisConfigurationID]
			   ,[DatabaseID])
				(select @analysisConfigID, x.record.query('ID').value('.', 'int')
						from @databaseID.nodes('Database/DatabaseID') as x(record))
			END
			
			SET @checkXML = @recommendationID.exist('(//RecommendationID)')
			if( @checkXML != 0)
			BEGIN
			
				INSERT INTO [dbo].[AnalysisConfigBlockedRecommendation]
			   ([AnalysisConfigurationID]
			   ,[RecommendationID])
				(select @analysisConfigID, x.record.query('ID').value('.', 'nvarchar(10)')
						from @recommendationID.nodes('Recommendation/RecommendationID') as x(record))

			END

END
ELSE
	BEGIN
	Print 'Analysis Id not found - Will Rollback'
	  -- Any Error Occurred during Transaction. Rollback
	  ROLLBACK  -- Roll back
  		  
	  RAISERROR ('Analysis Id not found - Will Rollback',
             16,
             1)
	END 

COMMIT
END
END TRY
BEGIN CATCH
 Print 'Error while updating configurations.'
  -- Any Error Occurred during Transaction. Rollback
  ROLLBACK  -- Roll back
  		  
  RAISERROR ('Error while updating configurations.',
             16,
             1)
END CATCH

end
 
GO 