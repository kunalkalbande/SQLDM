using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.Common.Configuration
{
    [Serializable]
    public class TestWmiConfiguration : OnDemandConfiguration
    {
        private WmiConfiguration _wmiConfiguration;
        public TestWmiConfiguration(int monitoredServerId, WmiConfiguration wmiCongifuration)
            : base(monitoredServerId)
        {
            _wmiConfiguration = wmiCongifuration;
        }

        public WmiConfiguration WmiConfig
        {
            get { return _wmiConfiguration; }
            set { _wmiConfiguration = value; }
        }

    }
}
