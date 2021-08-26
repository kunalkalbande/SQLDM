using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1.Widgets
{
    [DataContract]
    public class ProductStatus
    {
        [DataMember(EmitDefaultValue = true, IsRequired = true, Order = 1)]
        public AlertProductStatus AlertStatus { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = true, Order = 2)]
        public string TimeStamp { 
            get 
            {
                return DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss"); 
            }
            set 
            {
                
            }
        }
    }
}
