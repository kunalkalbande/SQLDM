using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Idera.SQLdoctor.Common.Helpers;
using Idera.SQLdoctor.AnalysisEngine.Batches;

namespace Idera.SQLdoctor.AnalysisEngine.Snapshot.WMI
{
    internal class WMIBios
    {
        public string SerialNumber { get; private set; }
        public string Version { get; private set; }

        public bool IsVMWare { get { return (SerialNumber.StartsWith("VMware-")); } }
        public bool IsHyperV { get { return (Version.StartsWith("VRTUAL - ")); } }
        public bool IsXen { get { return (Version.StartsWith("Xen - ")); } }

        private WMIBios() { }
        public WMIBios(WMIObjectProperties props)
        {
            SerialNumber = props.GetString("SerialNumber");
            Version = props.GetString("Version");
        }
        public WMIBios(DataTable dt)
        {
            if (null == dt) return;
            if (null == dt.Rows) return;
            if (null == dt.Columns) return;
            if (dt.Columns.Count < 2) return;
            foreach (DataRow dr in dt.Rows)
            {
                if (null == dr[0]) continue;
                switch (dr[0].ToString().ToLower())
                {
                    case ("serialnumber"): { SerialNumber = DataHelper.ToString(dr, 1); break; }
                    case ("version"): { Version = DataHelper.ToString(dr, 1); break; }
                    default: { System.Diagnostics.Debug.WriteLine("Unknown WMIBios Property: " + dr[0].ToString()); break; }

                }
            }
        }

        internal static string[] GetPropNames()
        {
            return (new string[] { 
                                    "SerialNumber", 
                                    "Version", 
                                    });
            //return (new BatchFinder.WmiObjectNameType[] { 
            //                        new BatchFinder.WmiObjectNameType(){Name="SerialNumber", Type=BatchFinder.WmiObjectType.String}, 
            //                        new BatchFinder.WmiObjectNameType(){Name="Version", Type=BatchFinder.WmiObjectType.String}
            //                        });
        }

        public override string ToString()
        {
            return (string.Format("WMIBios - SerialNumber: {0}  Version: {1}", SerialNumber, Version));
        }
    }
}
