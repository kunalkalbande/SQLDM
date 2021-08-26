IF (object_id('p_GetCustomCounterTags') IS NOT NULL)
BEGIN
DROP PROCEDURE p_GetCustomCounterTags
END
GO

CREATE PROCEDURE [dbo].[p_GetCustomCounterTags]
(
	@Metric INT
)
AS
BEGIN
	SELECT [TagId], [Name] as [TagName]
	FROM [CustomCounterTags]
	LEFT JOIN [Tags]
	ON TagId = Id
	WHERE [Metric] = @Metric
END