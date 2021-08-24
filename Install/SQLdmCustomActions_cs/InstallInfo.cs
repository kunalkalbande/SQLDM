using System;
using System.Collections.Generic;
using System.Text;

namespace CustomActions
{
    /// <summary>
    /// SQL DM 9.0 (Vineet Kumar) (License Changes + CWF) -- Object of this class will be serialised and stored in file. Later on this information will be deserialised and read.
    /// </summary>
    [Serializable]
    public class InstallInfo
    {
        public string CWFHostName { get; set; }
        public string CWFPort { get; set; }
        public string CWFServiceUserName { get; set; }
        public string CWFServicePassword { get; set; }
        public string CWFInstanceName { get; set; }
        public string LicenseKey { get; set; }
    }
}