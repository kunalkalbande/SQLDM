IF (object_id('p_UpdatePermissionTags') IS NOT NULL)
BEGIN
DROP PROCEDURE p_UpdatePermissionTags
END
GO

CREATE PROCEDURE [dbo].[p_UpdatePermissionTags]
(
	@PermissionId INT,
	@Tags nvarchar(max),
	@Synchronize BIT = 1
)
AS
BEGIN
	DECLARE @error INT
	DECLARE @xmlDoc INT

	IF (@Tags IS NOT NULL)
	BEGIN
		DECLARE @TagIds TABLE(TagId INT)

		EXEC sp_xml_preparedocument @xmlDoc OUTPUT, @Tags
		INSERT INTO @TagIds	
			SELECT TagId
				FROM OPENXML(@xmlDoc, '//Tag', 1) WITH (TagId INT)
		EXEC sp_xml_removedocument @xmlDoc

		IF (@Synchronize = 1)
		BEGIN
			DELETE FROM PermissionTags 
			WHERE 
				PermissionId = @PermissionId AND
				TagId NOT IN (SELECT TagId FROM @TagIds)
		END

		DELETE FROM @TagIds
		WHERE TagId IN (SELECT TagId FROM PermissionTags WHERE PermissionId = @PermissionId)

		INSERT INTO PermissionTags
			SELECT @PermissionId, TagId FROM @TagIds
	END
	ELSE
	BEGIN
		DELETE FROM PermissionTags WHERE PermissionId = @PermissionId	
	END

	SELECT TagId FROM PermissionTags WHERE PermissionId = @PermissionId

	SELECT @error = @@error
	RETURN @error
END