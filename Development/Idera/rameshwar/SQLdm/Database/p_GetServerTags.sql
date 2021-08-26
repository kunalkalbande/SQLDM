IF (object_id('p_GetServerTags') IS NOT NULL)
BEGIN
DROP PROCEDURE p_GetServerTags
END
GO

CREATE PROCEDURE [dbo].[p_GetServerTags]
(
	@SQLServerId INT
)
AS
BEGIN
	SELECT [TagId], [Name] as [TagName]
	FROM [ServerTags]
	LEFT JOIN [Tags]
	ON TagId = Id
	WHERE [SQLServerId] = @SQLServerId
END