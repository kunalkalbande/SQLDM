/****** Object:  StoredProcedure [dbo].[p_DeleteAzureSubscriptions]    Script Date: 20-03-2020 15:12:14 ******/
if (object_id('p_DeleteAzureSubscriptions') is not null)
begin
DROP PROCEDURE [dbo].[p_DeleteAzureSubscriptions]
end
GO

/****** Object:  StoredProcedure [dbo].[p_DeleteAzureSubscriptions]    Script Date: 20-03-2020 15:12:14 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



-- exec  [dbo].[p_DeleteAzureSubscriptions] 9

CREATE PROCEDURE [dbo].[p_DeleteAzureSubscriptions](
	@subscriptionId [nvarchar](max)
)
AS
begin
	
--For exception handling
DECLARE
  @ErrorMessage   varchar(2000)
 ,@ErrorSeverity  tinyint
 ,@ErrorState     tinyint
		
BEGIN

	Delete from AzureSubscription where [SubscriptionId] = @subscriptionId;
	end	
		IF(@@ERROR <>0)
BEGIN
	 Print 'Error while getting Azure Application Profiles.'
	  -- Any Error Occurred during Transaction. Rollback
	  SET @ErrorMessage  = ERROR_MESSAGE()
			SET @ErrorSeverity = ERROR_SEVERITY()
			SET @ErrorState    = ERROR_STATE()
			RAISERROR('Unable to delete subscription.', @ErrorSeverity, @ErrorState)
END
END	
 
GO


