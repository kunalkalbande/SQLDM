using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Data.SqlClient;
using Idera.SQLdm.Service.Core;

namespace Idera.SQLdm.Service.DataContracts.v1.Errors
{
    [DataContract]
    public class JsonExceptionWrapper
    {
        [DataMember(Order = 1)]
        public int ApplicationID { get; set; }
        [DataMember(Order = 2)]
        public int ExtendedErrorCode { get; set; }
        [DataMember(Order = 3)]
        public int ExtendedErrorNumber { get; set; }
        [DataMember(Order = 4)]
        public string Message { get; set; }
        [DataMember(Order = 5)]
        public string Source { get; set; }
        [DataMember(Order = 6)]
        public string StackTrace { get; set; }
        [DataMember(Order = 7)]
        public JsonExceptionWrapper InnerException { get; set; }

        public JsonExceptionWrapper(Exception ex)
        {
            this.ApplicationID = CoreSettings.ApplicationId;
            if (null == ex) return;
            this.Message = ex.Message;
            this.Source = ex.Source;
            this.StackTrace = ex.StackTrace;

            // try to pull extended error code information (This should work for Win32Exception and SqlException objects)
            if (ex is ExternalException) this.ExtendedErrorCode = ((ExternalException)ex).ErrorCode;
            if (ex is SqlException) this.ExtendedErrorNumber = ((SqlException)ex).Number;

            if (null != ex.InnerException) this.InnerException = new JsonExceptionWrapper(ex.InnerException);
        }
    }
}
