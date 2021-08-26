
if (object_id('p_GetBlockedRecommendationDatabaseAnalysisConfiguration') is not null)
begin
drop procedure [p_GetBlockedRecommendationDatabaseAnalysisConfiguration]
end
go


create procedure [p_GetBlockedRecommendationDatabaseAnalysisConfiguration] 
@sqlServerID INT
AS
begin

DECLARE @analysisConfigID INT 


SELECT DatabaseID, DatabaseName FROM SQLServerDatabaseNames 

--get record having IsActive true for this server id
SELECT @analysisConfigID = ID FROM [dbo].[AnalysisConfiguration] (NOLOCK)
WHERE MonitoredServerID = @sqlServerID AND IsActive = 1

if(@analysisConfigID > 0)
BEGIN

-- will string comma saparated for list of database / recommendation
		SELECT (SELECT STUFF((SELECT ', ' + CAST(DatabaseID AS VARCHAR(10)) [text()]
         FROM AnalysisConfigBlockedDatabases (NOLOCK)
         WHERE AnalysisConfigurationID = acbd.AnalysisConfigurationID
         FOR XML PATH(''), TYPE)
        .value('.','NVARCHAR(MAX)'),1,2,' ') List_Output
		FROM AnalysisConfigBlockedDatabases acbd (NOLOCK)
		GROUP BY AnalysisConfigurationID
		HAVING analysisConfig.ID = AnalysisConfigurationID) AS BlockedDatabases
		,(SELECT STUFF((SELECT ',' + CAST(RecommendationID AS VARCHAR(10)) [text()]
         FROM AnalysisConfigBlockedRecommendation (NOLOCK)
         WHERE AnalysisConfigurationID = acbr.AnalysisConfigurationID
         FOR XML PATH(''), TYPE)
        .value('.','NVARCHAR(MAX)'),1,1,'') List_Output
		FROM AnalysisConfigBlockedRecommendation acbr (NOLOCK)
		GROUP BY AnalysisConfigurationID
		HAVING AnalysisConfigurationID = analysisConfig.ID ) AS BlockedRecommendations
		FROM AnalysisConfiguration analysisConfig (NOLOCK)
			WHERE analysisConfig.ID = @analysisConfigID


END
ELSE
	BEGIN
	Print 'Analysis Id not found - Will Rollback'
	  -- Any Error Occurred during Transaction. Rollback
	    
	  RAISERROR ('Analysis Id not found - Will Rollback',
             16,
             1)
	END 

 Print 'Error while getting blocked recommendations and databases.'
  -- Any Error Occurred during Transaction. Rollback
  		  
  RAISERROR ('Error while getting blocked recommendations and databases.',
             16,
             1)

end
 
GO 