using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BBS.TracerX;

namespace Idera.SQLdm.Service.Helpers
{
    public static class SQLdmLogHelper
    {
        public static Logger GetSQLdmLogger(string logClient) 
        {
            return Logger.GetLogger(logClient);
        }
    }
}
