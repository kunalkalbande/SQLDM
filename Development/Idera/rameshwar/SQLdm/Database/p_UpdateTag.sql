IF (object_id('p_UpdateTag') IS NOT NULL)
BEGIN
DROP PROCEDURE p_UpdateTag
END
GO

CREATE PROCEDURE [dbo].[p_UpdateTag]
(
	@Id	INT,
	@NewName NVARCHAR(50)
)
AS
BEGIN
	IF NOT EXISTS 
	(
		SELECT [Id] 
		FROM [Tags]
		WHERE 
			[Id] <> @Id AND
			LOWER([Name]) = LOWER(@NewName)
	)
	BEGIN
		UPDATE [Tags] SET	
			[Name] = @NewName
		WHERE [Id] = @Id
	END
END