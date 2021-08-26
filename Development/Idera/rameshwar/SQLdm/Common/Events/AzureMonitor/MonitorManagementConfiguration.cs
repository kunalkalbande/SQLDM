using System;
using System.Collections.Generic;
using System.Linq;
using Idera.SQLdm.Common.Events.AzureMonitor.Interfaces;
using Idera.SQLdm.Common.Security.Encryption;

namespace Idera.SQLdm.Common.Events.AzureMonitor
{
    [Serializable]
    public class AzureSubscription : IAzureSubscription
    {
        public long Id { get; set; }

        /// <summary>
        /// Azure Subscription Id
        /// </summary>
        public string SubscriptionId { get; set; }
        public string Description { get; set; }
        public IAzureSubscription Copy()
        {
            return new AzureSubscription
            {
                Id = Id,
                Description = Description,
                SubscriptionId = SubscriptionId
            };
        }

        public override string ToString()
        {
            return SubscriptionId;
        }
    }

    [Serializable]
    public class AzureApplication : IAzureApplication
    {
        public long Id { get; set; }
        // Azure Application Details - Start
        public string Name { get; set; }
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string Description { get; set; }
        public string Secret { get; set; }
        public string EncryptedSecret
        {
            // this will produce a different value every time
            get { return Cipher.EncryptPassword(ClientId, Secret); }

            set { Secret = Cipher.DecryptPassword(ClientId, value); }
        }

        public IAzureApplication Copy()
        {
            return new AzureApplication
            {
                Id = Id,
                Name = Name,
                ClientId = ClientId,
                Description = Description,
                TenantId = TenantId,
                EncryptedSecret = EncryptedSecret
            };
        }

        public override string ToString()
        {
            return Name;
        }
    }

    [Serializable]
    public class AzureResource : IAzureResource
    {
        public string Uri { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public IAzureApplicationProfile Profile { get; set; }
        public IAzureResource Copy()
        {
            return new AzureResource
            {
                Name = Name,
                Type = Type,
                Uri = Uri,
                Profile = Profile != null ? Profile.Copy() : null
            };
        }
    }

        [Serializable]
    public class AzureSqlModel : IAzureSqlModel
    {
        public string FullyQualifiedName { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Id { get; set; }
        public IAzureSqlModel Copy()
        {
            return new AzureSqlModel
            {
                Name = Name,
                Type = Type,
                FullyQualifiedName = FullyQualifiedName,
                Id = Id
                 };
        }
    }

    [Serializable]
    public class AzureApplicationProfile : IAzureApplicationProfile
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public IAzureSubscription Subscription { get; set; }
        public IAzureApplication Application { get; set; }
        public string Description { get; set; }
        public List<IAzureResource> Resources { get; set; }
        public AzureProfileTestStatus TestStatus { get; set; }

        public IAzureApplicationProfile Copy()
        {
            return new AzureApplicationProfile
            {
                Id = Id,
                Description = Description,
                Name = Name,
                TestStatus = TestStatus,
                Application = Application != null ? Application.Copy() : null,
                Subscription = Subscription != null ? Subscription.Copy() : null,
                Resources = (Resources ?? new List<IAzureResource>()).Where(r => r != null).Select(resource => resource.Copy()).ToList()
            };
        }

        public override string ToString()
        {
            return Name;
        }
    }

    [Serializable]
    public class AzureProfile : IAzureProfile
    {
        public long Id { get; set; }
        public int SqlServerId { get; set; }
        public IAzureApplicationProfile ApplicationProfile { get; set; }
        public string Description { get; set; }
        public IAzureProfile Copy()
        {
            return new AzureProfile
            {
                Id = Id,
                SqlServerId = SqlServerId,
                Description = Description,
                ApplicationProfile = ApplicationProfile != null ? ApplicationProfile.Copy() : null
            };
        }
    }

    [Serializable]
    public class AzureMonitorParameters : IAzureMonitorParameters
    {
        /// <remarks>
        /// Supported ones are
        /// 00:01:00,00:05:00,00:15:00,00:30:00,01:00:00,06:00:00,12:00:00,1.00:00:00
        /// </remarks>
        public TimeSpan Interval { get; set; }
        public string Timespan { get; set; }
        public string MetricName { get; set; }
        public IAzureResource Resource { get; set; }
        public string Dimension { get; set; }

        public IAzureMonitorParameters Copy()
        {
            return new AzureMonitorParameters
            {
                Interval = Interval,
                Timespan = Timespan,
                MetricName = MetricName,
                Resource = Resource != null ? Resource.Copy() : null
            };
        }
    }

    [Serializable]
    public class MonitorManagementConfiguration : IMonitorManagementConfiguration
    {
        public IAzureProfile Profile { get; set; }
        public IAzureMonitorParameters MonitorParameters { get; set; }
        public IMonitorManagementConfiguration Copy()
        {
            return new MonitorManagementConfiguration
            {
                Profile = Profile != null ? Profile.Copy() : null,
                MonitorParameters = MonitorParameters != null ? MonitorParameters.Copy() : null
            };
        }
    }
}
