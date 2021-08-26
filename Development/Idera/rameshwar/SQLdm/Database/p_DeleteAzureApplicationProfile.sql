/****** Object:  StoredProcedure [dbo].[p_DeleteAzureApplicationProfile]    Script Date: 20-03-2020 14:55:49 ******/
if (object_id('p_DeleteAzureApplicationProfile') is not null)
begin
DROP PROCEDURE [dbo].p_DeleteAzureApplicationProfile
end
GO
/****** Object:  StoredProcedure [dbo].[p_DeleteAzureApplicationProfile]    Script Date: 20-03-2020 14:55:49 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- exec  [dbo].[p_DeleteAzureApplicationProfile] 9

CREATE PROCEDURE [dbo].[p_DeleteAzureApplicationProfile](
	@appProfileId [nvarchar](max)
)
AS
begin
	
--For exception handling
DECLARE
  @ErrorMessage   varchar(2000)
 ,@ErrorSeverity  tinyint
 ,@ErrorState     tinyint
		
BEGIN

	Delete from [dbo].[AzureApplicationProfile] where [ID] = @appProfileId;
	end	
		IF(@@ERROR <>0)
BEGIN
	 Print 'Error while deleting Azure Application Profile.'
	  -- Any Error Occurred during Transaction. Rollback
	  SET @ErrorMessage  = ERROR_MESSAGE()
			SET @ErrorSeverity = ERROR_SEVERITY()
			SET @ErrorState    = ERROR_STATE()
			RAISERROR('Unable to delete Azure Application Profile.', @ErrorSeverity, @ErrorState)
END
END	
 
GO


