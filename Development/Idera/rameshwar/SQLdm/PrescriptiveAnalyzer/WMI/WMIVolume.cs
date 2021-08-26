using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.WMI
{
    public class WMIVolume
    {
        public string Name { get; private set; }
        public ulong BlockSize { get; private set; }
        public string FileSystem { get; private set; }

        private WMIVolume() { }
        public WMIVolume(WMIObjectProperties props)
        {
            Name = props.GetString("Name");
            FileSystem = props.GetString("FileSystem");
            BlockSize = props.GetUInt64("BlockSize");
        }

        internal static string[] GetPropNames()
        {
            return (new string[] { 
                                    "Name", 
                                    "BlockSize",
                                    "FileSystem"
                                    });
        }
    }
}
