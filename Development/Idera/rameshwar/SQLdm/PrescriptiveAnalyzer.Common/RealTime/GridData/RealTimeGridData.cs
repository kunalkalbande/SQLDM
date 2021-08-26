using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Idera.SQLdoctor.Common.WMI;

namespace Idera.SQLdoctor.Common.RealTime.GridData
{
    [Serializable]
    internal class RealTimeGridData 
    {
        public WmiCounterUint64 Timestamp_Sys100NS = new WmiCounterUint64();
        private UInt64 Frequency_Sys100NS { get; set; }

        public RealTimeGridData() { }
        internal virtual bool MultipleSamplesNeeded() { return (false); }
        internal virtual string GetWmiClassName() { return (string.Empty); }
        internal virtual string[] GetPropNames() { return (null); }

        internal UInt64 GetFrequency_Sys100NS() { return (Frequency_Sys100NS); }

        internal virtual void SetProps(IProvideGridData props) 
        {
            Timestamp_Sys100NS.UpdateValue(props.GetString("Timestamp_Sys100NS"));
            Frequency_Sys100NS = props.GetUInt64("Frequency_Sys100NS");
        }

        internal virtual void Merge(RealTimeGridData data) 
        {
            Timestamp_Sys100NS.UpdateValue(data.Timestamp_Sys100NS.Current);
            Frequency_Sys100NS = data.Frequency_Sys100NS;
        }
    }
}
