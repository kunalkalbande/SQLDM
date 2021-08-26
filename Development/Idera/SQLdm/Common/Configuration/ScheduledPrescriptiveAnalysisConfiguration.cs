using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.Common.Configuration
{
    [Serializable]
    public class ScheduledPrescriptiveAnalysisConfiguration
    {
        public AnalysisConfiguration analysisConfig;

        public bool maintenanceMode;

        //Scheduled analysis through the alert response should't happen for alerts raised for SQL servers <2005.
        //Defect DE45819 Fix 10.0 Srishti Purohit
        public ServerVersion serverVersion;
    }
}
