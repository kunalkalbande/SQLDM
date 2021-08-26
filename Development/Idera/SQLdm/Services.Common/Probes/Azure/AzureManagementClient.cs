using System;
using Idera.SQLdm.Common.Events.AzureMonitor.Interfaces;
using Microsoft.Azure.Management.Monitor;
using Microsoft.Azure.Management.Monitor.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Idera.SQLdm.Common.Events.AzureMonitor;
using Idera.SQLdm.Services.Common.Probes.Azure.Interfaces;
using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Rest.Azure.OData;
using Microsoft.Azure.Management.Sql;

namespace Idera.SQLdm.Services.Common.Probes.Azure
{
    public class AzureManagementClient : IAzureMonitorManagementClient
    {
        public IMonitorManagementConfiguration Configuration { get; set; }

        public async Task<IEnumerable<MetricDefinition>> GetMetricDefinitions(IMonitorManagementConfiguration monitorConfiguration)
        {
            try
            {
                Configuration = monitorConfiguration;
                using (var readOnlyClient = await AzureManagementHelper.GetAuthenticatedMonitorClient(
                    Configuration.Profile.ApplicationProfile))
                {
                    return await readOnlyClient.MetricDefinitions.ListAsync(Configuration.MonitorParameters.Resource.Uri);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<IEnumerable<MetricNamespace>> GetMetricNamespaces()
        {
            try
            {
                using (var readOnlyClient = await AzureManagementHelper.GetAuthenticatedMonitorClient(
                       Configuration.Profile.ApplicationProfile))
                {

                    return await readOnlyClient.MetricNamespaces.ListAsync(Configuration.MonitorParameters.Resource.Uri);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Response> GetMetrics(IAzureMonitorParameters parameters)
        {
            try
            {
                using (var readOnlyClient = await AzureManagementHelper.GetAuthenticatedMonitorClient(
                        Configuration.Profile.ApplicationProfile))
                {
                    if (!string.IsNullOrWhiteSpace(parameters.Dimension))
                    {
                        return await readOnlyClient.Metrics.ListAsync(
                            resourceUri: Configuration.MonitorParameters.Resource.Uri,
                            odataQuery: new ODataQuery<MetadataValue>(parameters.Dimension),
                            timespan: parameters.Timespan, interval: parameters.Interval,
                            metricnames: parameters.MetricName);

                    }

                    return await readOnlyClient.Metrics.ListAsync(resourceUri: Configuration.MonitorParameters.Resource.Uri,
                        timespan: parameters.Timespan, interval: parameters.Interval, metricnames: parameters.MetricName);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<IAzureResource>> GetResources()
        {
            try
            {
                using (var readOnlyClient = await AzureManagementHelper.GetAuthenticatedResourceClient(
                       Configuration.Profile.ApplicationProfile))
                {
                    var pagedResources = await readOnlyClient.Resources.ListAsync();
                    if (pagedResources == null)
                    {
                        return new List<IAzureResource>();
                    }

                    return pagedResources.Select(res => new AzureResource
                    {
                        Name = res.Name,
                        Type = res.Type,
                        Uri = res.Id,
                        Profile = Configuration.Profile.ApplicationProfile
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<AzureSqlModel>> GetFilteredAzureApplicationResources()
        {
            try
            {
                List<AzureSqlModel> filteredResources = new List<AzureSqlModel>();
                using (var sqlReadOnlyClient = await AzureManagementHelper.GetAuthenticateWithSqlReadOnlyClient(
                Configuration.Profile.ApplicationProfile))
                {
                    var managedInstances = sqlReadOnlyClient.ManagedInstances.List();
                    var databaseServers = sqlReadOnlyClient.Servers.List();
                    if (managedInstances == null && databaseServers == null)
                    {
                        return new List<AzureSqlModel>();
                    }
                    if (managedInstances != null)
                    {
                        filteredResources = managedInstances.Select(res => new AzureSqlModel
                        {
                            Name = res.Name,
                            Type = res.Type,
                            Id = res.Id,
                            FullyQualifiedName = res.FullyQualifiedDomainName,
                        }).ToList();
                    }
                    if (databaseServers != null)
                    {
                        filteredResources.AddRange(databaseServers.Select(res => new AzureSqlModel
                        {
                            Name = res.Name,
                            Type = res.Type,
                            Id = res.Id,
                            FullyQualifiedName = res.FullyQualifiedDomainName
                        }));
                    }
                }
                return filteredResources;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
