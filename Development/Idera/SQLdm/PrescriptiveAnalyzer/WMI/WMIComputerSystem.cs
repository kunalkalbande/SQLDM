using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Idera.SQLdoctor.Common.Helpers;

namespace Idera.SQLdoctor.AnalysisEngine.Snapshot.WMI
{
    internal class WMIComputerSystem
    {
        public UInt32 DomainRole { get; private set; }
        public bool IsBackupDomainController { get { return (4 == DomainRole); } }
        public bool IsPrimaryDomainController { get { return (5 == DomainRole); } }

        private WMIComputerSystem() { }
        public WMIComputerSystem(DataTable dt)
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
                    case ("domainrole"): { DomainRole = DataHelper.ToUInt32(dr, 1); break; }
                    default: { System.Diagnostics.Debug.WriteLine("Unknown WMIComputerSystem Property: " + dr[0].ToString()); break; }

                }
            }
        }

        internal static string[] GetPropNames()
        {
            return (new string[] { 
                                    "DomainRole", 
                                    });
        }
    }
}
