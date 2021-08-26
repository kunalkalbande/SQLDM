IF (object_id('p_AddTag') IS NOT NULL)
BEGIN
DROP PROCEDURE p_AddTag
END
GO

CREATE PROCEDURE [dbo].[p_AddTag]
(
	@Name NVARCHAR(50),
	@Id INT OUTPUT
)
AS
BEGIN
	DECLARE @LookupId INT

	SELECT @LookupId = [Id] 
	FROM [Tags]
	WHERE LOWER([Name]) = LOWER(@Name)

	IF (@LookupId IS NULL)
	BEGIN
		INSERT INTO [Tags]
		   ([Name])
		VALUES
		   (@Name)

		SELECT @LookupId = SCOPE_IDENTITY()
	END

	SELECT @Id = @LookupId
END