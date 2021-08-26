using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLdoctor.AnalysisEngine.Snapshot.WMI
{
    internal class WMIPageFile
    {
        public string Name { get; private set; }
        private WMIPageFile() { }
        public WMIPageFile(WMIObjectProperties props)
        {
            Name = props.GetString("Name");
        }

        internal static string[] GetPropNames()
        {
            return (new string[] { "Name" });
        }
    }
}
