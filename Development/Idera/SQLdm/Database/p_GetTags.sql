IF (object_id('p_GetTags') IS NOT NULL)
BEGIN
DROP PROCEDURE p_GetTags
END
GO

CREATE PROCEDURE [dbo].[p_GetTags]
	@addSelectRequest bit = 0
AS
BEGIN
	if @addSelectRequest=1
	begin
       SELECT 0 as 'Id','< All >' as 'Name',0 as 'Servers',0 as 'CustomCounters', 0 as 'Permissions'
	   union
	   SELECT 
		[Id],
		[Name],
		[Servers] = (SELECT COUNT(*) FROM ServerTags WHERE TagId = Tags.Id),
		[CustomCounters] = (SELECT COUNT(*) FROM CustomCounterTags WHERE TagId = Tags.Id),
		[Permissions] = (SELECT COUNT(*) FROM PermissionTags WHERE TagId = Tags.Id)
	   FROM 
		[Tags] (nolock)
	end
	else
	SELECT 
		[Id],
		[Name],
		[Servers] = (SELECT COUNT(*) FROM ServerTags WHERE TagId = Tags.Id),
		[CustomCounters] = (SELECT COUNT(*) FROM CustomCounterTags WHERE TagId = Tags.Id),
		[Permissions] = (SELECT COUNT(*) FROM PermissionTags WHERE TagId = Tags.Id)
	FROM 
		[Tags] (nolock)

	SELECT [SQLServerId], [TagId]
	FROM [ServerTags] (nolock)

	SELECT [Metric], [TagId]
	FROM [CustomCounterTags] (nolock)

	SELECT [PermissionId], [TagId]
	FROM [PermissionTags] (nolock)
END