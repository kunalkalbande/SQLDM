using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLdm.Common.Configuration
{
    [Serializable]
    public class AmazonRDSMetricConfiguration : OnDemandConfiguration
    {
        #region "CONSTRUCTORS"
        public AmazonRDSMetricConfiguration(int monitoredServerId) : base(monitoredServerId)
        {
        }

        public AmazonRDSMetricConfiguration(int monitoredServerId, bool showFailedOnly) : base(monitoredServerId)
        {
        }
        #endregion
    }
}
