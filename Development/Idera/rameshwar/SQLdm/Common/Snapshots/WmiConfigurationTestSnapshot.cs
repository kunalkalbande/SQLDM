using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.Common.Snapshots
{
    [Serializable]
    public class WmiConfigurationTestSnapshot : Snapshot
    {
        public WmiConfigurationTestSnapshot(string serverName) : base(serverName)
        {   
        }

        public string DirectWmiStatus { get; set; }
        public string OleAutomationStatus { get; set; }
        public object CollectedValue { get; set; }
    }
}
