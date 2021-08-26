if (object_id('p_SaveAnalysisRecommendationAndProperties') is not null)
begin
drop procedure [p_SaveAnalysisRecommendationAndProperties]
end
go

create procedure [p_SaveAnalysisRecommendationAndProperties] 
@prescriptiveAnalysisDetailsID INT,
@listOfRecommendations xml
AS
begin
--For excepttion handling
DECLARE
  @ErrorMessage   varchar(2000)
 ,@ErrorSeverity  tinyint
 ,@ErrorState     tinyint
 -- handling while loop for each recomm
 
DECLARE @countRecomm int
DECLARE @incrementCounter int
DECLARE @prescriptiveAnalysisRecommendationID INT
SET @countRecomm = 0
SET @incrementCounter = 1
SET @prescriptiveAnalysisRecommendationID = 0
 --START save recommendations
		-- Insert recommendations details one by one 

SELECT @countRecomm = @listOfRecommendations.value('count(/ListOfRecommendation/Recomm)', 'int')
if(@countRecomm > 0)
BEGIN

WHILE(@incrementCounter <= @countRecomm)
BEGIN
IF EXISTS(select RecommendationID from PrescriptiveRecommendation (NOLOCK) where RecommendationID in( SELECT 
			N.Item.value('@RecommID[1]',  'nvarchar(10)') FROM 
			@listOfRecommendations.nodes('/ListOfRecommendation/Recomm[sql:variable("@incrementCounter")]') AS N(Item)
				) AND IsActive = 1)
				BEGIN
				   INSERT INTO [dbo].[PrescriptiveAnalysisRecommendation] 
           ([RecommendationID]
           ,[ComputedRankFactor]
           ,[PrescriptiveAnalysisDetailsID]
           ,[IsFlagged]
		   ,[OptimizationStatusID]
		   ,[OptimizationErrorMessage])
		-- (SELECT B.Item.value('@RecommID[1]',  'nvarchar(10)')
		-- ,B.Item.value('@ComputedRankFactor[1]',      'float')
		-- ,@prescriptiveAnalysisDetailsID
		-- ,B.Item.value('@isFlagged[1]',  'bit')
		--FROM   @listOfRecommendations.nodes('/ListOfAnlysis/Analysis') AS A(Item)
		--CROSS  APPLY A.Item.nodes('Recomm') AS B (Item))

		(
		SELECT 
			N.Item.value('@RecommID[1]',  'nvarchar(10)'),
			replace(N.Item.value('@ComputedRankFactor[1]', 'nvarchar(10)'),',','.'),
			@prescriptiveAnalysisDetailsID,
			N.Item.value('@isFlagged[1]', 'bit'),
			N.Item.value('@OptimizationStatus[1]', 'int'),
			N.Item.value('@OptimizationError[1]', 'nvarchar(max)')
		FROM 
			@listOfRecommendations.nodes('/ListOfRecommendation/Recomm[sql:variable("@incrementCounter")]') AS N(Item)
				)

		SET @prescriptiveAnalysisRecommendationID = (CASE WHEN @@ROWCOUNT > 0 THEN SCOPE_IDENTITY() ELSE -1 END)
		
		if(@prescriptiveAnalysisRecommendationID > 0)
		BEGIN

				   INSERT INTO [dbo].PrescriptiveRecommendationProperty
				   ([RecommendationID],
				   [PropertyName]
				   )
				   (
		SELECT DISTINCT(
			N.Item.value('@RecommID[1]',  'nvarchar(10)')),
			Tab1.Col1.value('@Name[1]', '[nvarchar](200)') AS PROPERTY
		FROM 
			@listOfRecommendations.nodes('/ListOfRecommendation/Recomm[sql:variable("@incrementCounter")]') AS N(Item)
	cross apply N.Item.nodes('Properties/Property') as Tab1(Col1)
				WHERE Tab1.Col1.value('@Name[1]', '[nvarchar](200)') NOT IN(SELECT [PropertyName] FROM PrescriptiveRecommendationProperty (NOLOCK)
		WHERE N.Item.value('@RecommID[1]',  'nvarchar(10)') =[RecommendationID])
		)

		if(@@ERROR <> 0 )
		BEGIN
			Print 'Error while saving anaysis in [PrescriptiveAnalysis].'
		  -- Any Error Occurred during Transaction. Rollback
		  ROLLBACK  -- Roll back
  		  
			SET @ErrorMessage  = ERROR_MESSAGE()
			SET @ErrorSeverity = ERROR_SEVERITY()
			SET @ErrorState    = ERROR_STATE()
			RAISERROR('Unable to save analysis in PrescriptiveAnalysis.', @ErrorSeverity, @ErrorState)
			RETURN 0
		END
		ELSE
		BEGIN 
		
					INSERT INTO [dbo].[PrescriptiveAnalysisRecommendationProperty]
				   ( [AnalysisRecommendationID] ,
					 [PropertyID]  ,
					 [Value] 
				   )
					  (SELECT @prescriptiveAnalysisRecommendationID
					 ,(SELECT ID FROM PrescriptiveRecommendationProperty (NOLOCK)
					 WHERE [PropertyName] = Tab1.Col1.value('@Name[1]', '[nvarchar](200)') AND [RecommendationID] = A.Item.value('@RecommID[1]',  'nvarchar(10)')) AS [PropertyID]
					,VALUE =CASE CAST(Tab1.Col1.query('./value/child::*') as nvarchar(max)) WHEN '' 
					THEN Tab1.Col1.value('.', '[nvarchar](max)')
					ELSE CAST(Tab1.Col1.query('./value/child::*') as nvarchar(max)) END
					--, Tab1.Col1.value('.', '[nvarchar](max)') AS VALUE
					FROM   @listOfRecommendations.nodes('/ListOfRecommendation/Recomm[sql:variable("@incrementCounter")]') AS A(Item)
					cross apply A.Item.nodes('Properties/Property') as Tab1(Col1)
					
					)

					if(@@ERROR <> 0 )
					BEGIN
						Print 'Error while saving anaysis in [PrescriptiveAnalysis].'
					  -- Any Error Occurred during Transaction. Rollback
					  ROLLBACK  -- Roll back
  		  
						SET @ErrorMessage  = ERROR_MESSAGE()
						SET @ErrorSeverity = ERROR_SEVERITY()
						SET @ErrorState    = ERROR_STATE()
						RAISERROR('Unable to save analysis in PrescriptiveAnalysis.', @ErrorSeverity, @ErrorState)
						RETURN 0
					END
			END
		END
END
SET @incrementCounter = @incrementCounter + 1
END

END

--END save recommendations
		if(@@ERROR <> 0)
		BEGIN
			Print 'Error while saving anaysis in [PrescriptiveAnalysis].'
		  -- Any Error Occurred during Transaction. Rollback
		  ROLLBACK  -- Roll back
  		  
			SET @ErrorMessage  = ERROR_MESSAGE()
			SET @ErrorSeverity = ERROR_SEVERITY()
			SET @ErrorState    = ERROR_STATE()
			RAISERROR('Unable to save analysis in PrescriptiveAnalysis.', @ErrorSeverity, @ErrorState)
			RETURN 0
		END
		ELSE 
		BEGIN
			RETURN 1
		END

end
 
GO 
