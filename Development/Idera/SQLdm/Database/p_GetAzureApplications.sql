if (object_id('p_GetAzureApplications') is not null)
begin
drop procedure [p_GetAzureApplications]
end
go

CREATE PROCEDURE [dbo].[p_GetAzureApplications]
AS
BEGIN
	DECLARE @error int

	SELECT
		azApp.[ID] AS [AzureApplicationId],
		azApp.[Name] AS [ApplicationName],
		azApp.[Description] AS [ApplicationDescription],
		azApp.[ClientId],
		azApp.[TenantId],
		azApp.[Secret]
	FROM AzureApplication azApp (NOLOCK)

	SELECT @error = @@error

	IF @error != 0 GOTO HANDLE_ERROR

	RETURN(0)

	HANDLE_ERROR:
		RAISERROR('An error occurred while retrieving the Azure Applications.', 10, 1)
        RETURN(@error)
END
