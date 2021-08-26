namespace Idera.SQLdm.Common.Services
{
    using Idera.SQLdm.Common.Configuration;
    using System;

    public interface IManagementServiceConfiguration : ICommonAssemblyInfo
    {
        /// <summary>
        /// Get the basic configuration info stored in the management service config file.
        /// </summary>
        /// <returns></returns>
        ManagementServiceConfigurationMessage GetManagementServiceConfiguration();

        /// <summary>
        /// Set the basic configuration info stored in the management service config file.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        bool SetManagementServiceConfiguration(ManagementServiceConfigurationMessage config);

        string GetCollectionServiceCommonAssemblyVersion(Guid? collectionServiceID);

        string GetCollectionServiceCommonAssemblyInformationVersion(Guid? collectionServiceID);

        ManagementServiceStatus GetServiceStatus();

        void TestRepositoryConnection(SqlConnectionInfo connectionInfo);

    }
}
