using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLdm.Common.Configuration
{
    [Serializable]
    public class AzureSQLMetricConfiguration: OnDemandConfiguration
    {
        #region "CONSTRUCTORS"
        public AzureSQLMetricConfiguration(int monitoredServerId) : base(monitoredServerId)
        {
        }

        public AzureSQLMetricConfiguration(int monitoredServerId, bool showFailedOnly) : base(monitoredServerId)
        {            
        }

        #endregion
    }
}
