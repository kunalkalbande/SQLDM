
using System;
namespace SQLdmCWFInstaller
{
    class SQLdmInstallInfo
    {

        private string hostName = Environment.MachineName;
        private string portName = "9292";
        private string instanceDisplayName;
        private string serviceAccount;
        private string serviceAccountPass;
        private string repositoryName;

        public string RepositoryName
        {
            get { return repositoryName; }
            set { repositoryName = value; }
        }

        public string ServiceAccount
        {
            get { return serviceAccount; }
            set { serviceAccount = value; }
        }

        public string ServiceAccountPass
        {
            get { return serviceAccountPass; }
            set { serviceAccountPass = value; }
        }

        public string InstanceDisplayName
        {
            get { return instanceDisplayName; }
            set { instanceDisplayName = value; }
        }

        public string HostName
        {
            get { return hostName; }
            set { hostName = value; }
        }
        
        public string PortName
        {
            get { return portName; }
            set { portName = value; }
        }

    }
}
