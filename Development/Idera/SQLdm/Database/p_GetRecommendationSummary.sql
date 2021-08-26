-- Get Recommendation Summary
-- example
-- EXEC [dbo].[p_GetRecommendationSummary]  @AnalysisID=3


if (object_id('p_GetRecommendationSummary') is not null)
begin
drop procedure p_GetRecommendationSummary
end
go
create procedure [dbo].[p_GetRecommendationSummary]
		(@AnalysisID nvarchar (max))
as
begin
	select TOP 5 * from (
		select Name CategoryName,Recommendations,MAXComputedRankFactor from (
			select count(*) Recommendations,CategoryID,max(ComputedRankFactor) MAXComputedRankFactor from PrescriptiveAnalysisRecommendation join PrescriptiveRecommendation on PrescriptiveAnalysisRecommendation.RecommendationID=PrescriptiveRecommendation.RecommendationID join PrescriptiveAnalysisDetails on PrescriptiveAnalysisRecommendation.PrescriptiveAnalysisDetailsID=PrescriptiveAnalysisDetails.PrescriptiveAnalysisDetailsID and AnalysisID=@AnalysisID group by CategoryID
		) as temp1 join PrescriptiveRecommendationCategory on temp1.CategoryID=PrescriptiveRecommendationCategory.CategoryID 
		Union select 'All' as CategoryName, sum(Recommendations) as Recommendations, Max(MAXComputedRankFactor)as MAXComputedRankFactor from (
			select count(*) Recommendations,CategoryID,max(ComputedRankFactor) MAXComputedRankFactor from PrescriptiveAnalysisRecommendation join PrescriptiveRecommendation on PrescriptiveAnalysisRecommendation.RecommendationID=PrescriptiveRecommendation.RecommendationID join PrescriptiveAnalysisDetails on PrescriptiveAnalysisRecommendation.PrescriptiveAnalysisDetailsID=PrescriptiveAnalysisDetails.PrescriptiveAnalysisDetailsID and AnalysisID=@AnalysisID group by CategoryID
		) as temp1
	) as temp2 order by MAXComputedRankFactor Desc
end

GO