    
if (object_id('p_GetAzureProfileWithMetricInfo') is not null)
begin
	drop procedure [p_GetAzureProfileWithMetricInfo]
end
go

create  PROCEDURE [dbo].[p_GetAzureProfileWithMetricInfo]    
 @sqlServerInstance NVARCHAR(MAX),    
 @resourceUri NVARCHAR(MAX),  
 @MetricId INT  
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
  azResource.[Uri] AS [ResourceUri],  
  counterDef.[Batch] AS [MetricName],  
  counterDef.[Counter] AS [MetricDisplayName],  
  mServer.[SQLServerID] AS [AzureSqlServerId]  
  
 FROM AzureProfile azProfile (NOLOCK)     
 INNER JOIN AzureApplicationProfile azAppProfile (NOLOCK) ON azProfile.[AzureApplicationProfileId] = azAppProfile.[ID]    
 INNER JOIN AzureApplication azApp (NOLOCK) ON azApp.[ID] = azAppProfile.[AzureApplicationId]    
 INNER JOIN AzureSubscription azSubs (NOLOCK) ON azSubs.[ID] = azAppProfile.[AzureSubscriptionId]    
 LEFT JOIN AzureResource azResource (NOLOCK) ON azAppProfile.ID =  azResource.[AzureApplicationProfileId]  
 INNER JOIN CustomCounterDefinition counterDef (NOLOCK) ON counterDef.AzureProfileId = azProfile.ID  
 INNER JOIN MonitoredSQLServers mServer (NOLOCK) ON mServer.InstanceName = counterDef.Instance  
 WHERE     
  azResource.[Uri] = @resourceUri AND counterDef.Metric = @MetricId  
  AND counterDef.Instance = @sqlServerInstance  
    
 SELECT @error = @@error    
    
 IF @error != 0 GOTO HANDLE_ERROR    
    
 RETURN(0)    
    
 HANDLE_ERROR:    
  RAISERROR('An error occurred while retrieving the Azure Profile.', 10, 1)    
        RETURN(@error)    
END 