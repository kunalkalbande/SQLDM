using Microsoft.Azure.Management.Monitor;
using Microsoft.Rest.Azure.Authentication;
using System.Threading.Tasks;
using Idera.SQLdm.Common.Events.AzureMonitor.Interfaces;
using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Azure.Management.Sql;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Idera.SQLdm.Services.Common.Probes.Azure
{
    public static class AzureManagementHelper
    {
        #region Authentication
        public static async Task<MonitorManagementClient> GetAuthenticatedMonitorClient(string tenantId, string clientId, string secret, string subscriptionId)
        {
            // Build the service credentials and Monitor client
            var serviceCreds = await ApplicationTokenProvider.LoginSilentAsync(tenantId, clientId, secret, ActiveDirectoryServiceSettings.Azure, new TokenCache());
            var monitorClient = new MonitorManagementClient(serviceCreds) { SubscriptionId = subscriptionId };

            return monitorClient;
        }


        public static async Task<MonitorManagementClient> GetAuthenticatedMonitorClient(IAzureApplicationProfile applicationProfile)
        {
            return await GetAuthenticatedMonitorClient(applicationProfile.Application.TenantId,
                applicationProfile.Application.ClientId, applicationProfile.Application.Secret, applicationProfile.Subscription.SubscriptionId);
        }

        private static async Task<ResourceManagementClient> GetAuthenticatedResourceClient(string tenantId, string clientId, string secret, string subscriptionId)
        {
            // Build the service credentials and Monitor client
            var serviceCreds = await ApplicationTokenProvider.LoginSilentAsync(tenantId, clientId, secret, ActiveDirectoryServiceSettings.Azure, new TokenCache());
            var monitorClient = new ResourceManagementClient(serviceCreds) { SubscriptionId = subscriptionId };

            return monitorClient;
        }

        public static async Task<ResourceManagementClient> GetAuthenticatedResourceClient(IAzureApplicationProfile applicationProfile)
        {
            return await GetAuthenticatedResourceClient(applicationProfile.Application.TenantId,
                applicationProfile.Application.ClientId, applicationProfile.Application.Secret, applicationProfile.Subscription.SubscriptionId);
        }

        public static async Task<SqlManagementClient> GetAuthenticateWithSqlReadOnlyClient(IAzureApplicationProfile applicationProfile)
        {
            var sqlManagementClient = await GetAuthenticateWithSqlReadOnlyClient(applicationProfile.Application.TenantId,
               applicationProfile.Application.ClientId, applicationProfile.Application.Secret, applicationProfile.Subscription.SubscriptionId);
            
            return sqlManagementClient;
        }

        private static async Task<SqlManagementClient> GetAuthenticateWithSqlReadOnlyClient(string tenantId, string clientId, string secret, string subscriptionId)
        {
            // Build the service credentials and Monitor client
            var serviceCreds = await ApplicationTokenProvider.LoginSilentAsync(tenantId, clientId, secret, ActiveDirectoryServiceSettings.Azure, new TokenCache());
            var sqlManagementClient = new SqlManagementClient(serviceCreds) { SubscriptionId = subscriptionId };
           
            return sqlManagementClient;
        }
        #endregion
    }
}
