if (object_id('p_SaveAnalysisRecords') is not null)
begin
drop procedure [p_SaveAnalysisRecords]
end
go

create procedure [p_SaveAnalysisRecords] 
@sqlServerID INT,
@analysisStartTime DATETIME,
@analysisCompleteTime DATETIME ,
@recommendationCount INT,
@listOfRecommendations xml,
@analysisID INT OUT,
@prescriptiveAnalysisDetailsID INT OUT,
@prescriptiveAnalysisRecommendationID INT OUT
AS
begin

BEGIN TRY
   -- Start A Transaction
BEGIN TRANSACTION


BEGIN

			-- insert analysis record in table
			-- insert new record in [PrescriptiveAnalysis] table
			INSERT INTO [dbo].[PrescriptiveAnalysis] WITH(TABLOCK)
				   ([SQLServerID]
           ,[UTCAnalysisStartTime]
           ,[UTCAnalysisCompleteTime]
           ,[RecommendationCount]
           ,[RecordCreatedTimestamp]
           ,[RecordUpdateDateTimestamp])
			 VALUES ( @sqlServerID
			 ,@analysisStartTime
			 ,@analysisCompleteTime
			 ,@recommendationCount
			  
			  ,CURRENT_TIMESTAMP
			  ,CURRENT_TIMESTAMP)
	
			
			SET @analysisID = (CASE WHEN @@ROWCOUNT > 0 THEN SCOPE_IDENTITY() ELSE -1 END)
		if(@analysisID > 0)
		BEGIN
			INSERT INTO [dbo].[PrescriptiveAnalysisDetails]
           ([AnalysisID]
           ,[AnalyzerID]
           ,[Status]
           ,[RecommendationCount]
           ,[RecordCreatedTimestamp]
           ,[RecordUpdateDateTimestamp])
			
			(SELECT @analysisID,T.Item.value('@AnalyzerID[1]', 'int'),
			T.Item.value('@status[1]',  'int'),
			T.Item.value('@recommCount[1]', 'int')
			,CURRENT_TIMESTAMP
			,CURRENT_TIMESTAMP
FROM   @listOfRecommendations.nodes('/ListOfAnlysis/Analysis') AS T(Item))

		SET @prescriptiveAnalysisDetailsID = (CASE WHEN @@ROWCOUNT > 0 THEN SCOPE_IDENTITY() ELSE -1 END)
		if(@prescriptiveAnalysisDetailsID > 0)
		BEGIN

				   INSERT INTO [dbo].[PrescriptiveAnalysisRecommendation]
           ([RecommendationID]
           ,[ComputedRankFactor]
           --,[Relevance]
           ,[PrescriptiveAnalysisDetailsID]
     --      ,[Description]
     --      ,[Finding]
     --      ,[ImpactExplanation]
		   --,[ProblemExplanation] 
		   --,[Recommendation]
           ,[IsFlagged])
     (SELECT B.Item.value('@RecommID[1]',  'nvarchar(10)')
	 ,B.Item.value('@ComputedRankFactor[1]',      'float')
	 --,B.Item.value('@Relevance[1]',        'decimal(18,0)')
	 ,PAD.PrescriptiveAnalysisDetailsID
	 --,B.Item.value('@Description[1]',  'nvarchar(4000)')
	 --,B.Item.value('@Finding[1]',  'nvarchar(500)')
	 --,B.Item.value('@Impact[1]',  'nvarchar(4000)')
	 --,B.Item.value('@ProblemExplanation[1]',  'nvarchar(4000)')
	 --,B.Item.value('@Recommendation[1]',  'nvarchar(max)')
	 ,B.Item.value('@isFlagged[1]',  'bit')
	FROM   @listOfRecommendations.nodes('/ListOfAnlysis/Analysis') AS A(Item)
	CROSS  APPLY A.Item.nodes('Recomm') AS B (Item), [PrescriptiveAnalysisDetails] AS PAD
	WHERE [AnalysisID] = @analysisID AND [AnalyzerID] = A.Item.value('@AnalyzerID[1]', 'int'))

		  
		   --START saving proeprties of recommedations for each analysis

		   SET @prescriptiveAnalysisRecommendationID = (CASE WHEN @@ROWCOUNT > 0 THEN SCOPE_IDENTITY() ELSE -1 END)
		if(@prescriptiveAnalysisRecommendationID > 0)
		BEGIN

				   INSERT INTO [dbo].PrescriptiveRecommendationProperty
				   ([RecommendationID],
				   [PropertyName]
				   )
     (SELECT DISTINCT B.Item.value('@RecommID[1]',  'nvarchar(10)') AS RECOMM
	 , Tab1.Col1.value('@Name[1]', '[nvarchar](200)') AS PROPERTY
	FROM   @listOfRecommendations.nodes('/ListOfAnlysis/Analysis') AS A(Item)
	CROSS  APPLY A.Item.nodes('Recomm') AS B (Item)
	cross apply B.Item.nodes('Properties/Property') as Tab1(Col1)
	WHERE Tab1.Col1.value('@Name[1]', '[nvarchar](200)') NOT IN(SELECT [PropertyName] FROM PrescriptiveRecommendationProperty
		WHERE B.Item.value('@RecommID[1]',  'nvarchar(10)') =[RecommendationID])
 ) 

		if(@@ERROR <> 0 )
		BEGIN
			Print 'no record insrted . Transaction Failed - Will Rollback'
		  -- Any Error Occurred during Transaction. Rollback
		  ROLLBACK  -- Roll back
		  
		  SET @analysisID = -1
		  SET @prescriptiveAnalysisDetailsID = -1
		  SET @prescriptiveAnalysisRecommendationID = -1
		  RAISERROR ('Error while saving anaysis.',
             16,
             1)
		END
		ELSE
		BEGIN 
		
					INSERT INTO [dbo].[PrescriptiveAnalysisRecommendationProperty]
				   ( [AnalysisRecommendationID] ,
					 [PropertyID]  ,
					 [Value] 
				   )
					  (SELECT DISTINCT( PAR.ID) AS [AnalysisRecommendationID]
					 ,(SELECT MIN(ID) FROM PrescriptiveRecommendationProperty 
					 WHERE [PropertyName] = Tab1.Col1.value('@Name[1]', '[nvarchar](200)') AND [RecommendationID] = B.Item.value('@RecommID[1]',  'nvarchar(10)')) AS [PropertyID]
					,VALUE =CASE CAST(Tab1.Col1.query('./value/child::*') as nvarchar(max)) WHEN '' 
					THEN Tab1.Col1.value('.', '[nvarchar](max)')
					ELSE CAST(Tab1.Col1.query('./value/child::*') as nvarchar(max)) END
					--, Tab1.Col1.value('.', '[nvarchar](max)') AS VALUE
					FROM   @listOfRecommendations.nodes('/ListOfAnlysis/Analysis') AS A(Item)
					CROSS  APPLY A.Item.nodes('Recomm') AS B (Item)
					cross apply B.Item.nodes('Properties/Property') as Tab1(Col1)
					JOIN PrescriptiveAnalysisRecommendation AS PAR ON PAR.RecommendationID = B.Item.value('@RecommID[1]',  'nvarchar(10)')
					JOIN PrescriptiveAnalysisDetails PAD ON PAD.PrescriptiveAnalysisDetailsID = PAR.PrescriptiveAnalysisDetailsID 
					JOIN PrescriptiveAnalysis PA ON PA.AnalysisID = PAD.AnalysisID
					WHERE PA.[AnalysisID] = @analysisID  AND PAD.[AnalyzerID] = A.Item.value('@AnalyzerID[1]', 'int')
					)

					if(@@ERROR <> 0 )
					BEGIN
						Print 'no record insrted . Transaction Failed - Will Rollback'
					  -- Any Error Occurred during Transaction. Rollback
					  ROLLBACK  -- Roll back
		  
					  SET @analysisID = -1
					  SET @prescriptiveAnalysisDetailsID = -1
					  SET @prescriptiveAnalysisRecommendationID = -1
					  RAISERROR ('Error while saving anaysis.',
						 16,
						 1)
					END
		END


		END
		ELSE
		BEGIN
			Print 'no record insrted . Transaction Failed - Will Rollback'
		  -- Any Error Occurred during Transaction. Rollback
		  ROLLBACK  -- Roll back
		  
		  SET @analysisID = -1
		  SET @prescriptiveAnalysisDetailsID = -1
		  SET @prescriptiveAnalysisRecommendationID = -1
		  RAISERROR ('Error while saving anaysis.',
             16,
             1)
		END
		
		   --END saving proeprties of recommedations for each analysis

		END
		ELSE
		BEGIN
			SET @analysisID = NULL
			SET @prescriptiveAnalysisDetailsID = NULL
			Print 'no record insrted . Transaction Failed - Will Rollback'
		  -- Any Error Occurred during Transaction. Rollback
		  ROLLBACK  -- Roll back
		  
		  SET @analysisID = -1
		  SET @prescriptiveAnalysisDetailsID = -1
		  SET @prescriptiveAnalysisRecommendationID = -1
		  RAISERROR ('Error while saving anaysis.',
             16,
             1)
		END
	END
	ELSE
	BEGIN
			Print 'no record insrted . Transaction Failed - Will Rollback'
		  -- Any Error Occurred during Transaction. Rollback
		  ROLLBACK  -- Roll back
		  		  
		  SET @analysisID = -1
		  SET @prescriptiveAnalysisDetailsID = -1
		  SET @prescriptiveAnalysisRecommendationID = -1
		  RAISERROR ('Error while saving anaysis.',
             16,
             1)
	END
		
	END

	COMMIT

END TRY
BEGIN CATCH
 Print 'Error while saving anaysis.'
  -- Any Error Occurred during Transaction. Rollback
  ROLLBACK  -- Roll back
  		  
		  SET @analysisID = -1
		  SET @prescriptiveAnalysisDetailsID = -1
		  SET @prescriptiveAnalysisRecommendationID = -1
  RAISERROR ('Error while saving anaysis.',
             16,
             1)
END CATCH

end
 
GO 
