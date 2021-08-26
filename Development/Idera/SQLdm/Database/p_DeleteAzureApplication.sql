/****** Object:  StoredProcedure [dbo].[p_DeleteAzureApplication]    Script Date: 20-03-2020 14:52:55 ******/

if (object_id('p_DeleteAzureApplication') is not null)
begin
DROP PROCEDURE [dbo].[p_DeleteAzureApplication]
end
GO

/****** Object:  StoredProcedure [dbo].[p_DeleteAzureApplication]    Script Date: 20-03-2020 14:52:55 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- exec  [dbo].[p_DeleteAzureApplication] 9

CREATE PROCEDURE [dbo].[p_DeleteAzureApplication](
	@appId [nvarchar](max)
)
AS
begin
	
--For exception handling
DECLARE
  @ErrorMessage   varchar(2000)
 ,@ErrorSeverity  tinyint
 ,@ErrorState     tinyint
		
BEGIN

	Delete from [dbo].[AzureApplication] where [ID] = @appId;
	end	
		IF(@@ERROR <>0)
BEGIN
	 Print 'Error while deleting Azure Application.'
	  -- Any Error Occurred during Transaction. Rollback
	  SET @ErrorMessage  = ERROR_MESSAGE()
			SET @ErrorSeverity = ERROR_SEVERITY()
			SET @ErrorState    = ERROR_STATE()
			RAISERROR('Unable to delete Azure Application.', @ErrorSeverity, @ErrorState)
END
END	
 
GO


