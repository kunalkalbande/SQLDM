using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1.CustomDashboard
{
    /// <summary>
    /// SQLdm 10.0 (srishti purohit) : Class to give data contract for Custom Dashboard functionality
    /// </summary>
    /// 
    [DataContract]
    public class CustomDashboard
    {

        #region fields

        [DataMember]
        public Int64 CustomDashboardId
        {
            get;
            set;
        }

        [DataMember]
        public string CustomDashboardName { get; set; }

        [DataMember]
        public bool IsDefaultOnUI { get; set; }

        [DataMember]
        public string UserSID { get; set; }

        //SQLdm 10.0 srishti purohit -While editing custom dashboard Tags field will get inserted

        [DataMember]
        public List<string> TagsDashboard { get; set; }


        //SQLdm 10.0 srishti purohit -Created Record Time and updated time will not be response 
        public Nullable<DateTime> RecordCreatedTimestamp { get; set; }

        public Nullable<DateTime> RecordUpdateDateTimestamp { get; set; }

        #endregion
    }
}
