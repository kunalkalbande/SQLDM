using Idera.SQLdm.Common;

namespace Idera.SQLdm.DesktopClient.Objects
{
    internal sealed class RepositoryInfo
    {
        private string versionString;
        private int monitoredServerCount;
        private string instanceName;

        public RepositoryInfo(string versionString, int monitoredServerCount, string instanceName)
        {
            this.versionString = versionString;
            this.monitoredServerCount = monitoredServerCount;
            this.instanceName = instanceName;
        }

        public string VersionString 
        {
            get { return versionString; }
        }


        public string InstanceName 
        {
            get { return instanceName; }
        }

        public int MonitoredServerCount
        {
            get { return monitoredServerCount; }
        }

        public bool IsValidVersion
        {
            get { return versionString == Constants.ValidRepositorySchemaVersion; }
        }
    }
}
