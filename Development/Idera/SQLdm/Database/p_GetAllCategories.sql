if (object_id('p_GetAllCategories') is not null)
begin
drop procedure [p_GetAllCategories]
end
go

-- SQLdm 10.0 Srishti Purohit Doctor implementation in DM
CREATE PROCEDURE [dbo].[p_GetAllCategories] 
	@sqlServerId INT = NULL,
	@cloudProviderId INT = NULL
AS
BEGIN

-- Get cloud provider id for the SQL Server Id
IF @cloudProviderId IS NULL
BEGIN
	SELECT 
		@cloudProviderId = CloudProviderId 
	FROM
		MonitoredSQLServers mss (NOLOCK)
	WHERE 
		mss.SQLServerID = @sqlServerId
END

-- Ensure that the cloud provider id is respected
IF @cloudProviderId IS NOT NULL AND @sqlServerId IS NULL
BEGIN
	SET @sqlServerId = 0
END

-- Handle cases where Cloud Provider Id is null and SQL Server Id is not null
IF @sqlServerId = 0 AND @cloudProviderId IS NULL
BEGIN
	SET @sqlServerId = NULL
END

-- Get real categories count
select (count(distinct(category1.CategoryID)) - COUNT(DISTINCT(category2.ParentCategory))) AS categoryCount
from PrescriptiveRecommendationCategory category1
join PrescriptiveRecommendation PR on PR.CategoryID = category1.CategoryID
JOIN RecommedationClassification RC (NOLOCK)
	ON PR.RecommendationID = RC.RecommendationID
left join PrescriptiveRecommendationCategory category2 on category2.ParentCategory = category1.CategoryID
WHERE
	@sqlServerId IS NULL OR
	((RC.AWS = 1 AND @cloudProviderId = 1) OR 
	(RC.Azure = 1 AND @cloudProviderId = 2) OR
	(RC.OnPremisesDb = 1 AND (@cloudProviderId = 3 OR @cloudProviderId = 4)) OR
	@cloudProviderId > 4)
	
		-- check if records for recommendation
		
SELECT distinct category1.CategoryID, category1.Name, 
(select Name from PrescriptiveRecommendationCategory where CategoryID = category1.ParentCategory) AS Parent,
category1.Description 
from PrescriptiveRecommendationCategory category1
join PrescriptiveRecommendation PR on PR.CategoryID = category1.CategoryID
JOIN RecommedationClassification RC (NOLOCK)
	ON PR.RecommendationID = RC.RecommendationID
left join PrescriptiveRecommendationCategory category2 on category1.ParentCategory = category2.CategoryID
WHERE
	@sqlServerId IS NULL OR
	((RC.AWS = 1 AND @cloudProviderId = 1) OR 
	(RC.Azure = 1 AND @cloudProviderId = 2) OR
	(RC.OnPremisesDb = 1 AND (@cloudProviderId = 3 OR @cloudProviderId = 4)) OR
	@cloudProviderId > 4)

UNION
SELECT CategoryID, Name , NULL AS Parent, category1.Description 
FROM PrescriptiveRecommendationCategory category1 WHERE category1.ParentCategory IS NULL AND
	(@sqlServerId IS NULL OR @cloudProviderId IS NOT NULL)

UNION 
select category1.CategoryID, category1.Name, (select Name from PrescriptiveRecommendationCategory where 
CategoryID = category1.ParentCategory)
AS Parent, category1.Description from PrescriptiveRecommendationCategory category1
where category1.CategoryID in (select ParentCategory from PrescriptiveRecommendationCategory)
and category1.ParentCategory IS not NULL AND
	(@sqlServerId IS NULL OR @cloudProviderId IS NOT NULL)
ORDER BY Parent ASC

		
if(@@ERROR <> 0)
BEGIN
	 Print 'Error occured while getting categories.'
	  -- Any Error Occurred during Transaction. Rollback
	   RAISERROR ('Error occured while getting categories.',
             16,
             1)
END

END
 
