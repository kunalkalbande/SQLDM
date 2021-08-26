if (object_id('p_GetAzureProfiles') is not null)
begin
drop procedure [p_GetAzureProfiles]
end
go

CREATE PROCEDURE [dbo].[p_GetAzureProfiles]
AS
BEGIN
	DECLARE @error int

	-- Read Azure Profiles
	SELECT
		azProfile.[ID],
		azProfile.[Description],
		azProfile.[SQLServerID],
		azProfile.[AzureApplicationProfileId],
		azAppProfile.[Name] AS [AzureApplicationProfileName],
		azAppProfile.[Description] AS [AzureApplicationProfileDescription],
		azAppProfile.[AzureApplicationId],
		azApp.[Name] AS [ApplicationName],
		azApp.[Description] AS [ApplicationDescription],
		azApp.[ClientId],
		azApp.[TenantId],
		azApp.[Secret],
		azAppProfile.[AzureSubscriptionId],
		azSubs.[SubscriptionId],
		azSubs.[Description] AS [SubscriptionDescription],
		azResource.[ID] AS [ResourceId],
		azResource.[Name] AS [ResourceName],
		azResource.[Type] AS [ResourceType],
		azResource.[Uri] AS [ResourceUri]
	FROM AzureProfile azProfile (NOLOCK) 
	INNER JOIN AzureApplicationProfile azAppProfile (NOLOCK) ON azProfile.[AzureApplicationProfileId] = azAppProfile.[ID]
	INNER JOIN AzureApplication azApp (NOLOCK) ON azApp.[ID] = azAppProfile.[AzureApplicationId]
	INNER JOIN AzureSubscription azSubs (NOLOCK) ON azSubs.[ID] = azAppProfile.[AzureSubscriptionId]
	LEFT JOIN AzureResource azResource (NOLOCK) ON azAppProfile.ID =  azResource.[AzureApplicationProfileId]

	SELECT @error = @@error

	IF @error != 0 GOTO HANDLE_ERROR

	RETURN(0)

	HANDLE_ERROR:
		RAISERROR('An error occurred while retrieving the Azure Profile.', 10, 1)
        RETURN(@error)
END
