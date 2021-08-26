using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.Common.Launch
{
    /// <summary>
    /// SQLdm 9.1 (Gaurav Karwal): Base class that represents
    /// </summary>
    public abstract class LaunchParameters
    {

    }

    public class AlertLaunchParameters : LaunchParameters 
    {
        public string InstanceName { get; set; }
        public int InstanceId { get; set; }
        public long AlertId { get; set; }

        public AlertLaunchParameters(string instanceName,int instanceId, long alertId) 
        {
            InstanceName = instanceName;
            InstanceId = instanceId;
            AlertId = alertId;
        }
    }

    public class InstanceLaunchParameters : LaunchParameters 
    {
        public int InstanceId { get; set; }

        public InstanceLaunchParameters(int instanceId) 
        {
            InstanceId = instanceId;
        }
    }

    /// <summary>
    /// SQLdm10.2 (srishti purohit )defect fix
    /// SQLDM-27637('Prescriptive Analysis Summary' component click not working)
    /// </summary>
    public class AnalysisLaunchParameters : LaunchParameters
    {
        public int InstanceId { get; set; }

        public AnalysisLaunchParameters(int instanceId)
        {
            InstanceId = instanceId;
        }
    }

}
