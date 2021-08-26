if (object_id('p_GetMasterRecommendations') is not null)
begin
drop procedure [p_GetMasterRecommendations]
end
go

-- SQLdm 10.0 Srishti Purohit Doctor implementation in DM
create procedure [p_GetMasterRecommendations] 

AS
BEGIN
	
		
		SELECT PR.[RecommendationID]
      ,PR.[AdditionalConsiderations]
      ,PR.[bitly]
	  ,PRC.Name AS Category
      ,PR.[ConfidenceFactor]
      ,PR.[Description]
      ,PR.[Finding]
      ,PR.[ImpactExplanation]
      ,PR.[ImpactFactor]
      ,PR.[InfoLinks]
      ,PR.[PluralFormFinding]
      ,PR.[PluralFormImpactExplanation]
      ,PR.[PluralFormRecommendation]
      ,PR.[ProblemExplanation]
      ,PR.[Recommendation]
      ,PR.[Relevance]
      ,PR.[Tags]
		FROM PrescriptiveRecommendation PR
		
		JOIN PrescriptiveRecommendationCategory PRC ON PRC.CategoryID = PR.[CategoryID]
		WHERE PR.IsActive = 1


		IF(@@ERROR <> 0)
		BEGIN
			DECLARE @ErMessage NVARCHAR(2048)
			declare @severity int; 
            declare @state int;

            select @ErMessage = ERROR_MESSAGE(), @severity=error_severity(),@state=error_state();

            RAISERROR(@ErMessage,@severity,@state)
		END
END
 
GO 
