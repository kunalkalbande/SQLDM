using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Idera.SQLdm.Common;

namespace Idera.SQLdm.Service.DataContracts.v1.License
{
    [DataContract]
    public class LicenseDetails
    {
        private DateTime _expiration;

        [DataMember]
        public int CheckedKeyCount { get; set; }
        
        [DataMember]
        public DateTime Expiration
        {
            get 
            {
                return (_expiration==DateTime.MinValue || _expiration == DateTime.MaxValue)?new DateTime(2099,12,31):_expiration;
            }
            set
            {
                _expiration = value;
            }
        }

        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public string Repository { get; set; }
        [DataMember]
        public int MonitoredServers { get; set; }
        [DataMember]
        public int LicensedServers { get; set; }
        [DataMember]
        public bool IsUnlimited { get; set; }
        [DataMember]
        public bool IsTrial { get; set; }
        [DataMember]
        public bool IsPermanent { get; set; }
    }    
}
