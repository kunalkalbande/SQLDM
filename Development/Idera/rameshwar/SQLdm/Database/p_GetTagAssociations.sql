IF (object_id('p_GetTagAssociations') IS NOT NULL)
BEGIN
DROP PROCEDURE p_GetTagAssociations
END
GO

CREATE PROCEDURE [dbo].[p_GetTagAssociations]
(
	@TagId int,
	@IncludeServerTags bit,
	@IncludeCustomCounterTags bit,
	@IncludePermissionTags bit
)
AS
BEGIN

	if (@IncludeServerTags = 1)
	begin
		SELECT [SQLServerId], [TagId]
		FROM [ServerTags]
		WHERE (@TagId is NULL or [TagId] = @TagId)
	end

	if (@IncludeCustomCounterTags = 1)
	begin
		SELECT [Metric], [TagId]
		FROM [CustomCounterTags]
		WHERE (@TagId is NULL or [TagId] = @TagId)
	end

	if (@IncludePermissionTags = 1)
	begin
		SELECT [PermissionId], [TagId]
		FROM [PermissionTags]
		WHERE (@TagId is NULL or [TagId] = @TagId)
	end

END