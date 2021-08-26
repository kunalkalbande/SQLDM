using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLdoctor.Common.RealTime.GridData
{
    [Serializable]
    internal class RealTimeBios : RealTimeGridData
    {
        public string SerialNumber { get; private set; }
        public string Version { get; private set; }

        public RealTimeBios() { }

        internal override bool MultipleSamplesNeeded() { return (false); }
        internal override string GetWmiClassName() { return "Win32_BIOS"; }
        internal override string[] GetPropNames()
        {
            return (new string[] { 
                                    "SerialNumber", 
                                    "Version", 
                                });
        }

        internal override void SetProps(IProvideGridData props) 
        {
            base.SetProps(props);
            SerialNumber = props.GetString("SerialNumber");
            Version = props.GetString("Version");
        }
        public override string ToString() { return (SerialNumber); }
        public override int GetHashCode() { return this.SerialNumber.GetHashCode(); }
        public override bool Equals(object obj)
        {
            RealTimeBios rtp = obj as RealTimeBios;
            if (null == rtp) return (false);
            return (rtp.SerialNumber == this.SerialNumber);
        }
        internal override void Merge(RealTimeGridData data)
        {
            RealTimeBios rtp = data as RealTimeBios;
            if (null == rtp) return;
            if (!this.Equals(rtp)) return;
            base.Merge(data);
            SerialNumber = rtp.SerialNumber;
            Version = rtp.Version;
        }
    }
}
