 if (object_id('p_AddAzureLinkedProfile') is not null)
begin
drop procedure [p_AddAzureLinkedProfile]
end
go

Create PROCEDURE [dbo].[p_AddAzureLinkedProfile]
( 
	@Description  NVARCHAR(MAX),
	@SqlServerId  int,
	@ApplicationProfileId  bigint,
	@Id BIGINT OUTPUT
)
AS
BEGIN
	-- Update the existing entry
	IF @Id IS NOT NULL AND @Id <> 0 AND EXISTS (SELECT 1 FROM [dbo].[AzureProfile] NOLOCK WHERE [ID] = @Id)
	BEGIN
		UPDATE 
			[AzureProfile]
		SET
			[Description] =  @Description,
			[SQLServerID] = @SqlServerId,
			[AzureApplicationProfileId] = 	@ApplicationProfileId
		WHERE
			[ID] = @Id;
	END

	-- Insert if existing mapping doesn't exists
	IF NOT EXISTS (SELECT 1 FROM [dbo].[AzureProfile] NOLOCK WHERE [SQLServerID] = @SqlServerId AND [AzureApplicationProfileId] = 	@ApplicationProfileId)
	BEGIN
		INSERT INTO [AzureProfile]
		   ([Description],[SQLServerID],[AzureApplicationProfileId])
		VALUES
		   (@Description,@SqlServerId,@ApplicationProfileId)

		SELECT @Id = SCOPE_IDENTITY()	
	END
			
	if(@@ERROR <> 0)
	BEGIN
		 Print 'Error occured while adding/updating azure linked profile.'
		  -- Any Error Occurred during Transaction. Rollback
		   RAISERROR ('Error occured while adding/updating azure linked profile.',
				 16,
				 1)
	END
END
 
