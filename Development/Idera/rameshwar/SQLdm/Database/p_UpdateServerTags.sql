IF (object_id('p_UpdateServerTags') IS NOT NULL)
BEGIN
DROP PROCEDURE p_UpdateServerTags
END
GO

CREATE PROCEDURE [dbo].[p_UpdateServerTags]
(
	@SQLServerID INT,
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
			DELETE FROM ServerTags 
			WHERE 
				SQLServerId = @SQLServerID AND
				TagId NOT IN (SELECT TagId FROM @TagIds)
		END

		DELETE FROM @TagIds
		WHERE TagId IN (SELECT TagId FROM ServerTags WHERE SQLServerId = @SQLServerID)

		INSERT INTO ServerTags
			SELECT @SQLServerID, TagId FROM @TagIds
	END
	ELSE
	BEGIN
		DELETE FROM ServerTags WHERE SQLServerId = @SQLServerID	
	END

	SELECT TagId FROM ServerTags WHERE SQLServerId = @SQLServerID

	SELECT @error = @@error
	RETURN @error
END