IF (object_id('p_GetCustomCountersWithTagIds') IS NOT NULL)
BEGIN
DROP PROCEDURE p_GetCustomCountersWithTagIds
END
GO

CREATE PROCEDURE [dbo].[p_GetCustomCountersWithTagIds]
(
	@Tags nvarchar(max) = NULL
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

		SELECT [Metric], [TagId] 
		FROM [CustomCounterTags] 
		WHERE [TagId] IN (SELECT TagId FROM @TagIds)
	END
	ELSE
	BEGIN
		SELECT [Metric], [TagId] 
		FROM [CustomCounterTags] 
	END

	SELECT @error = @@error
	RETURN @error
END