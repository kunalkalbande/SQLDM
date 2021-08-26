using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.Common.CWFDataContracts
{
    public class DashboardWidget
    {   
        public string Name { get; set; }

        
        public string Type { get; set; }

        
        public string NavigationLink { get; set; }

        
        public string PackageURI { get; set; }

        
        public string DataURI { get; set; }

        
        public string Description { get; set; }

        
        public string Version { get; set; }

        
        public Dictionary<string, string> Settings { get; set; }

        
        public string DefaultViews { get; set; }

        
        public Boolean Collapsed { get; set; }

    }

}
