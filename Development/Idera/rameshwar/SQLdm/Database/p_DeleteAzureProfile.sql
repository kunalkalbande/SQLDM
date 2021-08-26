/****** Object:  StoredProcedure [dbo].[p_DeleteAzureProfile]    Script Date: 20-03-2020 14:57:03 ******/
if (object_id('p_DeleteAzureProfile') is not null)
begin
DROP PROCEDURE [dbo].[p_DeleteAzureProfile]
end
GO

/****** Object:  StoredProcedure [dbo].[p_DeleteAzureProfile]    Script Date: 20-03-2020 14:57:03 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- exec  [dbo].[p_DeleteAzureProfile] 9

CREATE PROCEDURE [dbo].[p_DeleteAzureProfile](
	@profileId [nvarchar](max)
)
AS
begin
	
--For exception handling
DECLARE
  @ErrorMessage   varchar(2000)
 ,@ErrorSeverity  tinyint
 ,@ErrorState     tinyint
		
BEGIN

	Delete from [dbo].[AzureProfile] where [ID] = @profileId;
	end	
		IF(@@ERROR <>0)
BEGIN
	 Print 'Error while deleting Azure Profile.'
	  -- Any Error Occurred during Transaction. Rollback
	  SET @ErrorMessage  = ERROR_MESSAGE()
			SET @ErrorSeverity = ERROR_SEVERITY()
			SET @ErrorState    = ERROR_STATE()
			RAISERROR('Unable to delete Azure Profile.', @ErrorSeverity, @ErrorState)
END
END

GO


