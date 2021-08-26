using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace Idera.SQLdm.Common.Auditing
{
    [Serializable]
   public class AuditAuxiliar<T> : ILogicalThreadAffinative
    {
        public AuditAuxiliar(T data)
        {
            this.Data = data;
        }

        public T Data { get; set; }
    }



     [Serializable]
   public class AuditContextData : ILogicalThreadAffinative
     {
        public const string ContextName = "AuditableEntity";
        public AuditContextData()
        {
            Workstation = String.Empty;
            WorkstationUser = String.Empty;
            SqlUser = String.Empty;
        }

        public String Workstation { get; set; }

        public String WorkstationUser { get; set; }

        public String SqlUser { get; set; }
    }
}
