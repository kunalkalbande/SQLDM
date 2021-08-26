IF (OBJECT_ID('p_DeleteAzureResource') IS NOT NULL)
BEGIN
DROP PROCEDURE [p_DeleteAzureResource]
END
GO

CREATE PROCEDURE [p_DeleteAzureResource] 
	@AzureApplicationProfileId INT
AS
BEGIN

	IF EXISTS (SELECT 1 FROM [AzureResource] WHERE [AzureApplicationProfileId] = @AzureApplicationProfileId)
	BEGIN
		DELETE [AzureResource]
			WHERE [AzureApplicationProfileId] = @AzureApplicationProfileId
	END
	ELSE
	BEGIN
		PRINT 'The resources associated with Azure Application Profile was not found for deletion.'
	END

	if(@@ERROR <> 0)
	BEGIN
		Print 'Error occured while deleting the resources associated with the azure application profile.'
		RAISERROR ('Error occured while deleting the resources associated with the azure application profile.', 16, 1)
	END
END
