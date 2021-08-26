using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1
{
    [DataContract]
    public class ServerSystemProcesses
    {
        #region fields

        [DataMember]
        public int computersHoldingProcesses;
        [DataMember]
        public int currentSystemProcesses;
        [DataMember]
        public int currentUserProcesses;
        [DataMember]
        public int userProcessesConsumingCpu;
        [DataMember]
        public int systemProcessesConsumingCpu;
        [DataMember]
        public int blockedProcesses;
        [DataMember]
        public int openTransactions;
        [DataMember]
        public int leadBlockers;
        [DataMember]
        public int activeProcesses;

        #endregion
    }
}
