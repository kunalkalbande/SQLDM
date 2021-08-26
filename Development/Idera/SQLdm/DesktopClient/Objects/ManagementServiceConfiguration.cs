namespace Idera.SQLdm.DesktopClient.Objects
{
    class ManagementServiceConfiguration
    {
        private string identifier;
        private string machineName;
        private string instanceName;
        private string address;
        private int port;

        public ManagementServiceConfiguration(string identifier, string machineName, string instanceName, string address, int port)
        {
            this.identifier = identifier;
            this.machineName = machineName;
            this.instanceName = instanceName;
            this.address = address;
            this.port = port;
        }

        public string Identifier
        {
            get { return identifier; }
        }

        public string MachineName
        {
            get { return machineName; }
        }

        public string InstanceName
        {
            get { return instanceName; }
        }

        public string Address
        {
            get { return address; }
        }

        public int Port
        {
            get { return port; }
        }

        public override string ToString() {
            return string.Format("MachineName = {0}, InstanceName = {1}, Address = {2}, Port = {3}.", MachineName, InstanceName, Address, Port);
        }
    }
}
