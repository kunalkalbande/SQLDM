using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Values
{
    [Serializable]
    public class SnapshotValues
    {
        public SnapshotValues(SnapshotValues v)
        {
            if (null != v)
            {
                ActiveNetworkCards = v.ActiveNetworkCards;
                TotalNetworkBandwidth = v.TotalNetworkBandwidth;
                AllowedProcessorCount = v.AllowedProcessorCount;
                TotalNumberOfLogicalProcessors = v.TotalNumberOfLogicalProcessors;
                TotalMaxClockSpeed = v.TotalMaxClockSpeed;
                TotalPhysicalMemory = v.TotalPhysicalMemory;
                MaxServerMemory = v.MaxServerMemory;
                WindowsVersion = v.WindowsVersion;
                ProductVersion = v.ProductVersion;
                SQLVersionString = v.SQLVersionString;
            }
        }

        public SnapshotValues()
        { 
        }

        #region Network Interface Values

        public int ActiveNetworkCards { get; set; }
        public UInt64 TotalNetworkBandwidth { get; set; }

        #endregion

        #region CPU Consumption Values

        public int AllowedProcessorCount { get; set; }
        public UInt32 TotalNumberOfLogicalProcessors { get; set; }
        public UInt64 TotalMaxClockSpeed { get; set; }
        public UInt64 TotalPhysicalMemory { get; set; }
        public UInt64 MaxServerMemory { get; set; }

        #endregion

        #region Version Values

        public string WindowsVersion { get; set; }
        public string ProductVersion { get; set; }
        public string SQLVersionString { get; set; }

        #endregion
    }
}
