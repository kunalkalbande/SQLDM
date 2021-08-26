using System;
using System.Collections.Generic;

namespace Idera.SQLdm.Common.Events.AzureMonitor.Interfaces
{
    public enum AzureProfileTestStatus
    {
        Unknown,
        Testing,
        Success,
        Failed
    }
    public interface IEnableCopy<out T>
    {
        T Copy();
    }
    public interface IAzureSubscription : IEnableCopy<IAzureSubscription>
    {
        long Id { get; set; }
        string SubscriptionId { get; set; }
        string Description { get; set; }
    }

    public interface IAzureApplication : IEnableCopy<IAzureApplication>
    {
        long Id { get; set; }
        string Name { get; set; }
        string TenantId { get; set; }
        string ClientId { get; set; }
        string Description { get; set; }
        string Secret { get; set; }

        string EncryptedSecret
        {
            // this will produce a different value every time
            get;
            set;
        }
    }

    public interface IAzureApplicationProfile : IEnableCopy<IAzureApplicationProfile>
    {
        long Id { get; set; }
        string Name { get; set; }
        IAzureSubscription Subscription { get; set; }
        IAzureApplication Application { get; set; }
        string Description { get; set; }
        List<IAzureResource> Resources { get; set; }
        /// <summary>
        /// Used by Views to test the connection status of the application profile
        /// </summary>
        AzureProfileTestStatus TestStatus { get; set; }
    }

    /// <remarks>
    /// Id should not be used for the Azure Resource because it's dependent on the Azure Application
    /// </remarks>>
    public interface IAzureResource : IEnableCopy<IAzureResource>
    {
        string Uri { get; set; }
        string Name { get; set; }
        string Type { get; set; }
        IAzureApplicationProfile Profile { get; set; }
    }

    public interface IAzureProfile : IEnableCopy<IAzureProfile>
    {
        long Id { get; set; }
        int SqlServerId { get; set; }
        IAzureApplicationProfile ApplicationProfile { get; set; }
        string Description { get; set; }
    }

    public interface IAzureSqlModel : IEnableCopy<IAzureSqlModel>
    {
        string FullyQualifiedName { get; set; }
        string Name { get; set; }
        string Type { get; set; }
        string Id { get; set; }
    }


    public interface IAzureMonitorParameters : IEnableCopy<IAzureMonitorParameters>
    {
        TimeSpan Interval { get; set; }
        string Timespan { get; set; }
        string MetricName { get; set; }
        string Dimension { get; set; }
        IAzureResource Resource { get; set; }
    }

    public interface IMonitorManagementConfiguration : IEnableCopy<IMonitorManagementConfiguration>
    {
        IAzureProfile Profile { get; set; }
        IAzureMonitorParameters MonitorParameters { get; set; }
    }
}
