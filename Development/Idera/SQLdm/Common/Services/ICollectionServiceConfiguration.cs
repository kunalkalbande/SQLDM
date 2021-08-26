namespace Idera.SQLdm.Common.Services
{
    using Idera.SQLdm.Common.Configuration;

    public interface ICollectionServiceConfiguration : ICommonAssemblyInfo
    {
        CollectionServiceConfigurationMessage GetCollectionServiceConfiguration();

        Result SetCollectionServiceConfiguration(CollectionServiceConfigurationMessage message);

        CollectionServiceStatus GetServiceStatus();
    }
}
