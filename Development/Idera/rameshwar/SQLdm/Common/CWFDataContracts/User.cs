using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.Common.CWFDataContracts
{
    /// <summary>
    /// SQLdm 9.0 (Gaurav Karwal): user class that represents the 
    /// </summary>
    public class User
    {
        /// <summary>
        /// SQLdm 9.0 (Gaurav Karwal): Valid types are U=user and G=group
        /// </summary>
        public string UserType { get; set; } //using a string because that is how cwf contracts are

        
        public string Account { get; set; }

        
        public string SID { get; set; }

        public bool IsAdmin { get; set; }
        //SQLdm10.1 (Srishti Purohit)
        //Property added to populate Common.Permission_Instances table using CWF AssignInstancePermissions function 
        public List<string> LinkedInstances { get; set; }
    }
}
