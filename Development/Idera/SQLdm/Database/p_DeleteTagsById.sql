IF (object_id('p_DeleteTagsById') IS NOT NULL)
BEGIN
DROP PROCEDURE p_DeleteTagsById
END
GO

CREATE PROCEDURE [dbo].[p_DeleteTagsById]
(
	@XmlDocument nvarchar(max)
)
AS
BEGIN
DECLARE @TagsToDelete TABLE(TagId INT) 
DECLARE @xmlDoc INT

IF @XmlDocument IS NOT NULL
BEGIN
	EXEC sp_xml_preparedocument @xmlDoc OUTPUT, @XmlDocument

	INSERT INTO @TagsToDelete
	SELECT
		TagId
	FROM OPENXML(@xmlDoc, '//Tag', 1)
		WITH (TagId INT)

	EXEC sp_xml_removedocument @xmlDoc
END

DELETE FROM [Tags] WHERE [Id] IN (SELECT [TagId] FROM @TagsToDelete)

exec p_SyncCustomCounterThresholds

RETURN @@error
END