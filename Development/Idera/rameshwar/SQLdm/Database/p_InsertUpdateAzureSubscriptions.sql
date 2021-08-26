/****** Object:  StoredProcedure [dbo].[p_InsertUpdateAzureSubscriptions]    Script Date: 20-03-2020 15:00:09 ******/
IF (object_id('p_InsertUpdateAzureSubscriptions') is not null)
BEGIN
DROP PROCEDURE [dbo].[p_InsertUpdateAzureSubscriptions]
end
GO

/****** Object:  StoredProcedure [dbo].[p_InsertUpdateAzureSubscriptions]    Script Date: 20-03-2020 15:00:09 ******/
CREATE PROCEDURE [dbo].[p_InsertUpdateAzureSubscriptions](
	@subscriptionId nvarchar(max),
	@desc varchar(max),
	@azureSubscriptionId bigint = NULL
)
AS
BEGIN
	--For exception handling
	DECLARE
	  @ErrorMessage   varchar(2000)
	 ,@ErrorSeverity  tinyint
	 ,@ErrorState     tinyint
		
	BEGIN
	BEGIN TRAN
	-- Update the existing entry
	IF @azureSubscriptionId IS NOT NULL AND EXISTS (SELECT 1 FROM [dbo].[AzureSubscription] NOLOCK WHERE [ID] = @azureSubscriptionId)
	BEGIN
	  UPDATE AzureSubscription SET [SubscriptionId] = @subscriptionId, [Description] =  @desc 
		WHERE [ID] = @azureSubscriptionId;		
	END
	-- Try to update comments for existing subscription
	IF EXISTS (SELECT 1 FROM [dbo].[AzureSubscription] WITH (NOLOCK) where [SubscriptionId] = @subscriptionId)
	BEGIN
	  UPDATE AzureSubscription SET [Description] =  @desc
		WHERE [SubscriptionId] = @subscriptionId;
	END
	ELSE
	BEGIN
	-- Fresh insert of the azure subscription
	   INSERT INTO [dbo].[AzureSubscription]  (
			[SubscriptionId],
    		[Description] 
		) 
		VALUES (
			@subscriptionId,
			@desc
		)
	END
	COMMIT TRAN
	END	
	IF(@@ERROR <>0)
	BEGIN
		Print 'Unable to insert or update azure subscription.'
		-- Any Error Occurred during Transaction. Rollback
		SET @ErrorMessage  = ERROR_MESSAGE()
			SET @ErrorSeverity = ERROR_SEVERITY()
			SET @ErrorState    = ERROR_STATE()
			RAISERROR('Unable to insert or update azure subscription.', @ErrorSeverity, @ErrorState)
	END
END	
GO


