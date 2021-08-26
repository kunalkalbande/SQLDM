IF (OBJECT_ID('p_AddAzureResource') IS NOT NULL)
BEGIN
DROP PROCEDURE [p_AddAzureResource]
END
GO

CREATE PROCEDURE [p_AddAzureResource]
	@AzureApplicationProfileId [BIGINT],
	@Name NVARCHAR(MAX),
	@Type NVARCHAR(MAX),
	@Uri NVARCHAR(MAX),
	@NewID INT OUTPUT
AS
BEGIN

	DECLARE @ID INT

	INSERT INTO [AzureResource]
			([AzureApplicationProfileId],
			[Name],
			[Type],
			[Uri]
			)
		VALUES (@AzureApplicationProfileId,
				@Name,
				@Type,
				@Uri
			)

	SELECT @ID = SCOPE_IDENTITY()

	select @NewID = @ID
end	

