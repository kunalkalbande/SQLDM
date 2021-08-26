using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Idera.SQLdm.Common.Security.Encryption;

namespace Idera.SQLdm.Service.Configuration
{
    public class RestServiceElement : ConfigurationElement
    {
        #region fields

        private const string InstanceNameKey = "instanceName";
        private const string RepositoryServerKey = "repositoryServer";
        private const string RepositoryDatabaseKey = "repositoryDatabase";
        private const string UseWindowsAuthenticationKey = "windowsAuthentication";
        private const string RepositoryUserNameKey = "repositoryUserName";
        private const string RepositoryPasswordKey = "repositoryPassword";
        private const string ServicePortKey = "servicePort";
        private const string RepositoryTimeoutKey = "repositoryConnectionTimeoutInSeconds";
        private const string CommandTimeoutKey = "repositoryCommandTimeoutInSeconds";

        private const string TracerXSectionKey = "TracerX";

        //private const string OutOfProcOleAutomationKey = "OutOfProcOleAutomation";
        //private const string ScheduledCollectionMaxQueueLengthKey = "scheduledCollectionMaxQueueLength";
        //private const string IgnoreScheduledCollectionKey = "ignoreScheduledCollection";
        //private const string DataDirectoryKey = "dataDirectory";

        #endregion

        #region constructors

        #endregion

        #region properties

        [ConfigurationProperty(InstanceNameKey, DefaultValue = "Default", IsRequired = true, IsKey = true)]
        public string InstanceName
        {
            get { return (string)this[InstanceNameKey]; }
            set { this[InstanceNameKey] = value; }
        }

        [ConfigurationProperty(RepositoryServerKey)]
        public string RepositoryServer
        {
            get { return (string)this[RepositoryServerKey]; }
            set { this[RepositoryServerKey] = value; }
        }

        [ConfigurationProperty(ServicePortKey, DefaultValue = 9278)]
        public int ServicePort
        {
            get { return (int)this[ServicePortKey]; }
            set { this[ServicePortKey] = value; }
        }

        [ConfigurationProperty(RepositoryDatabaseKey)]
        public string RepositoryDatabase
        {
            get { return (string)this[RepositoryDatabaseKey]; }
            set { this[RepositoryDatabaseKey] = value; }
        }

        [ConfigurationProperty(UseWindowsAuthenticationKey, DefaultValue = "true")]
        public bool RepositoryWindowsAuthentication
        {
            get { return (bool)this[UseWindowsAuthenticationKey]; }
            set { this[UseWindowsAuthenticationKey] = value; }
        }

        [ConfigurationProperty(RepositoryUserNameKey)]
        public string RepositoryUsername
        {
            get { return (string)this[RepositoryUserNameKey]; }
            set { this[RepositoryUserNameKey] = value; }
        }

        [ConfigurationProperty(RepositoryPasswordKey)]
        public string RepositoryPassword
        {
            get { return GetRepositoryPassword(); }
            set { SetRepositoryPassword(value); }
        }

        [ConfigurationProperty(RepositoryTimeoutKey, DefaultValue = 0)]
        public int RepositoryTimeout
        {
            get { return (int)this[RepositoryTimeoutKey]; }
            set { this[RepositoryTimeoutKey] = value; }
        }

        [ConfigurationProperty(CommandTimeoutKey, DefaultValue = 0)]
        public int CommandTimeout
        {
            get { return (int)this[CommandTimeoutKey]; }
            set { this[CommandTimeoutKey] = value; }
        }

        //[ConfigurationProperty(ScheduledCollectionMaxQueueLengthKey, DefaultValue = 5)]
        //public int ScheduledCollectionMaxQueueLength
        //{
        //    get
        //    {
        //        int result = (int)this[ScheduledCollectionMaxQueueLengthKey];
        //        if (result < 1)
        //            result = 5;
        //        return result;
        //    }
        //    set { this[ScheduledCollectionMaxQueueLengthKey] = value; }
        //}

        //[ConfigurationProperty(IgnoreScheduledCollectionKey, DefaultValue = false)]
        //public bool IgnoreScheduledCollection
        //{
        //    get { return (bool)this[IgnoreScheduledCollectionKey]; }
        //    set { this[IgnoreScheduledCollectionKey] = value; }
        //}

        [ConfigurationProperty(TracerXSectionKey, DefaultValue = "TracerX")]
        public string TracerXSectionName
        {
            get { return (string)this[TracerXSectionKey]; }
            set { this[TracerXSectionKey] = value; }
        }

        //[ConfigurationProperty(DataDirectoryKey, DefaultValue = "")]
        //public string DataDirectory
        //{
        //    get { return (string)this[DataDirectoryKey]; }
        //    set { this[DataDirectoryKey] = value; }
        //}

        internal string GetRepositoryPassword()
        {
            string encryptedPassword = (string)this[RepositoryPasswordKey];
            return Cipher.DecryptPassword(InstanceName, encryptedPassword);
        }

        internal void SetRepositoryPassword(string plainText)
        {
            string encryptedPassword = Cipher.EncryptPassword(InstanceName, plainText);
            this[RepositoryPasswordKey] = encryptedPassword;
        }

        public bool IsValid
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region events

        #endregion

        #region methods

        public void Save()
        {
            string instanceName = InstanceName;

            RestServiceElement element = null;
            RestServicesSection section = RestServicesSection.GetSection();

            // update the local configuration file
            System.Configuration.Configuration configuration =
                ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            // Get/Create the Idera.SQLdm configuration section
            section = configuration.GetSection(RestServicesSection.SectionName) as RestServicesSection;
            if (section == null)
            {
                section = new RestServicesSection();
                configuration.Sections.Add(RestServicesSection.SectionName, section);
            }
            element = section.RestServices[instanceName];
            if (element == null)
            {
                element = new RestServiceElement();
                element.InstanceName = instanceName;
                section.RestServices[instanceName] = element;
            }

            element.RepositoryServer = RepositoryServer;
            element.RepositoryDatabase = RepositoryDatabase;
            element.RepositoryWindowsAuthentication = RepositoryWindowsAuthentication;
            element.RepositoryUsername = RepositoryUsername;
            element.RepositoryPassword = RepositoryPassword;
            element.TracerXSectionName = TracerXSectionName;
            element.ServicePort = ServicePort;

            if (element.CommandTimeout == 0)
                element.CommandTimeout = 180;

            if (element.RepositoryTimeout == 0)
                element.RepositoryTimeout = 30;

            //if (element.ScheduledCollectionMaxQueueLength == 0)
            //    element.ScheduledCollectionMaxQueueLength = 5;

            //element.DataDirectory = DataDirectory;

            configuration.Save();

            RestServicesSection.Refresh();
        }


        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

    }
}
