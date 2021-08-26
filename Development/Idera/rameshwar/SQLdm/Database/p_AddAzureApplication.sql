if (object_id('p_AddAzureApplication') is not null)
begin
drop procedure [p_AddAzureApplication]
end
go


Create PROCEDURE [dbo].[p_AddAzureApplication]
(
	@Name NVARCHAR(MAX),
	@ClientId  NVARCHAR(MAX),
	@Description  NVARCHAR(MAX),
	@Secret  NVARCHAR(MAX),
	@TenantId  NVARCHAR(MAX),
	@Id BIGINT OUTPUT
)
AS
BEGIN
	DECLARE @LookupId INT
	IF @Id IS NOT NULL AND EXISTS(SELECT 1 FROM [AzureApplication] WHERE [ID] = @Id)
	BEGIN
		SELECT @LookupId = @Id 
	END
	ELSE
	BEGIN
		SELECT @LookupId = [ID] 
		FROM [AzureApplication]
		WHERE LOWER([Name]) = LOWER(@Name)
	END
	IF (@LookupId IS NULL)
	BEGIN
		INSERT INTO [AzureApplication]
		   ([Name],[ClientId],[Description],[Secret],[TenantId])
		VALUES
		   (@Name,@ClientId,@Description,@Secret,@TenantId)

		SELECT @LookupId = SCOPE_IDENTITY()
	END
	ELSE
	BEGIN
		UPDATE AzureApplication 
		Set [Name]=@Name,
			[ClientId]=@ClientId,
			[Description]=@Description,
			[Secret]=@Secret,
			[TenantId]=@TenantId
			Where ID=@LookupId
	END
	SELECT @Id = @LookupId
	if(@@ERROR <> 0)
	BEGIN
		 Print 'Error occured while adding/updating azure application.'
		  -- Any Error Occurred during Transaction. Rollback
		   RAISERROR ('Error occured while adding/updating azure application.',
				 16,
				 1)
	END
END
 
