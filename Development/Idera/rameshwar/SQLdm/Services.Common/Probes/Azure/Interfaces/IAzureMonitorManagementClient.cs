using System.Collections.Generic;
using System.Threading.Tasks;
using Idera.SQLdm.Common.Events.AzureMonitor.Interfaces;
using Microsoft.Azure.Management.Monitor.Models;

namespace Idera.SQLdm.Services.Common.Probes.Azure.Interfaces
{
    public interface IAzureMonitorManagementClient
    {
        IMonitorManagementConfiguration Configuration { get; set; }
        Task<IEnumerable<MetricNamespace>> GetMetricNamespaces();
        Task<IEnumerable<MetricDefinition>> GetMetricDefinitions(IMonitorManagementConfiguration Configuration);
        Task<Response> GetMetrics(IAzureMonitorParameters parameters);
    }
}
