IF (object_id('p_UpdateCustomCounterTags') IS NOT NULL)
BEGIN
DROP PROCEDURE p_UpdateCustomCounterTags
END
GO

CREATE PROCEDURE [dbo].[p_UpdateCustomCounterTags]
(
	@Metric INT,
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
			DELETE FROM CustomCounterTags 
			WHERE 
				Metric = @Metric AND
				TagId NOT IN (SELECT TagId FROM @TagIds)
		END

		DELETE FROM @TagIds
		WHERE TagId IN (SELECT TagId FROM CustomCounterTags WHERE Metric = @Metric)

		INSERT INTO CustomCounterTags
			SELECT @Metric, TagId FROM @TagIds
	END
	ELSE
	BEGIN
		DELETE FROM CustomCounterTags WHERE Metric = @Metric	
	END

	SELECT TagId FROM CustomCounterTags WHERE Metric = @Metric

	SELECT @error = @@error
	RETURN @error
END