using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.WMI
{
    internal class WMIEncryptableVolume
    {
        public string DriveLetter { get; private set; }
        public UInt32 ProtectionStatus { get; private set; }

        private WMIEncryptableVolume() { }
        public WMIEncryptableVolume(WMIObjectProperties props)
        {
            DriveLetter = props.GetString("DriveLetter");
            ProtectionStatus = props.GetUInt32("ProtectionStatus");
        }

        internal static string[] GetPropNames()
        {
            return (new string[] { 
                                    "DriveLetter", 
                                    "ProtectionStatus" 
                                    });
        }
    }
}
