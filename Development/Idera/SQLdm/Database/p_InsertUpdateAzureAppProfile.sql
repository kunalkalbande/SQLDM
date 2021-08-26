/****** Object:  StoredProcedure [dbo].[p_InsertUpdateAzureAppProfile]    Script Date: 20-03-2020 14:59:36 ******/
IF (OBJECT_ID('p_InsertUpdateAzureAppProfile') IS NOT NULL)
BEGIN
DROP PROCEDURE [dbo].[p_InsertUpdateAzureAppProfile]
end
GO


/****** Object:  StoredProcedure [dbo].[p_InsertUpdateAzureAppProfile]    Script Date: 20-03-2020 14:59:36 ******/
-- exec  [dbo].[p_InsertUpdateAzureAppProfile] 'test','test',9,1
CREATE PROCEDURE [dbo].[p_InsertUpdateAzureAppProfile](
	@name NVARCHAR(MAX),
	@desc NVARCHAR(MAX),
    @subscriptionId BIGINT,
    @appId BIGINT,
	@azureAppProfileId BIGINT = NULL,
	@newAzureAppProfileId BIGINT OUTPUT
)
AS
BEGIN
	--For exception handling
	DECLARE
	  @ErrorMessage   VARCHAR(2000)
	 ,@ErrorSeverity  TINYINT
	 ,@ErrorState     TINYINT
		
	BEGIN
		SELECT @newAzureAppProfileId = @azureAppProfileId
		BEGIN TRAN
		-- Update the existing entry
		IF @azureAppProfileId IS NOT NULL AND EXISTS (SELECT 1 FROM [dbo].[AzureApplicationProfile] NOLOCK WHERE [ID] = @azureAppProfileId)
		BEGIN
			UPDATE 
				[AzureApplicationProfile]
			SET
				[Name] = @name,
				[Description] =  @desc,
				[AzureSubscriptionId] = @subscriptionId,
				[AzureApplicationId] = 	@appId
			WHERE
				[ID] = @azureAppProfileId;		
		END
		-- Try to update comments for existing subscription
		IF EXISTS (SELECT 1 FROM [dbo].[AzureApplicationProfile] WITH (NOLOCK) WHERE [Name] = @name)
		BEGIN
			UPDATE 
				[AzureApplicationProfile]
			SET
				[Description] = @desc,
				[AzureSubscriptionId] = @subscriptionId,
				[AzureApplicationId] = @appId
			WHERE 
				[Name] = @name;
		END
		ELSE
		BEGIN
			INSERT INTO [dbo].[AzureApplicationProfile]  (
				[Name],
				[AzureSubscriptionId],
				[AzureApplicationId],
    			[Description] 
			) 
			VALUES (
				@name,
				@subscriptionId,
				@appId,
				@desc
			)
			SELECT @newAzureAppProfileId = SCOPE_IDENTITY()
		END
		COMMIT TRAN
	END
	IF(@@ERROR <>0)
	BEGIN
		 Print 'Error while adding or updating the Azure Application Profiles.'
		  -- Any Error Occurred during Transaction. Rollback
		  SET @ErrorMessage  = ERROR_MESSAGE()
				SET @ErrorSeverity = ERROR_SEVERITY()
				SET @ErrorState    = ERROR_STATE()
				RAISERROR('Unable to add or update the Azure Application Profiles.', @ErrorSeverity, @ErrorState)
	END
END	
GO


