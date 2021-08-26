using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.Common.HyperV
{
    [Serializable]
    public class HyperVCommonObjects
    {
        public double? timeStamp_Sys100NS_Raw = null;
        public double? timestamp_PerfTime_Raw = null;
        public double? timestamp_PerfTime_Network_Raw = null;

        public double? timestamp_PerfTime_Memory_Raw = null;
        public double? frequency_PerfTime_Memory_Raw = null;

        public double? Timestamp_PerfTime_Memory_Raw
        {
            get { return timestamp_PerfTime_Memory_Raw; }
            set { timestamp_PerfTime_Memory_Raw = value; }
        }

        public double? Frequency_PerfTime_Memory_Raw
        {
            get { return frequency_PerfTime_Memory_Raw; }
            set { frequency_PerfTime_Memory_Raw = value; }
        }

        public double? Timestamp_PerfTime_Network_Raw
        {
            get { return timestamp_PerfTime_Network_Raw; }
            set { timestamp_PerfTime_Network_Raw = value; }
        }

        public double? TimeStamp_Sys100NS_Raw
        {
            get { return timeStamp_Sys100NS_Raw; }
            set { timeStamp_Sys100NS_Raw = value; }
        }

        public double? Timestamp_PerfTime_Raw
        {
            get { return timestamp_PerfTime_Raw; }
            set { timestamp_PerfTime_Raw = value; }
        }

        public HyperVCommonObjects()
        {

        }
    }


    public enum ConnectState
    {
        Connected,
        Disconnected
    }

}
