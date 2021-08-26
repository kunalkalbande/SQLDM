using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using Idera.SQLdm.Common;

namespace Idera.SQLdm.Service.Repository
{
    public class SQLServerHelper
    {
        public static string EscapeQuotes(string value)
        {
            if (value == null)
                return string.Empty;

            StringBuilder builder = new StringBuilder(value);

            builder.Replace("'", "''");

            return builder.ToString();
        }
    }

    public class DatabaseState
    {
        public string Name { get; private set; }
        public int State { get; private set; }
        public string StateDescription { get; private set; }
        public bool Online { get { return (0 == State); } }

        public DatabaseState(SqlDataReader dataReader)
        {
            if (dataReader == null)
            {
                throw new ArgumentNullException("dataReader");
            }

            Name = dataReader["name"] as string;
            State = dataReader["state"] != null ? (int)Convert.ChangeType(dataReader["state"], typeof(int)) : 0;
            StateDescription = dataReader["state_desc"] as string;
        }        
    }     
}
