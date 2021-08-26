using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class DatabaseObjectName : IProvideDatabase
    {
        public string Database { get { return DatabaseName; } }
        public String DatabaseName { get; set; }
        public String SchemaName { get; set; }
        public String ObjectName { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            
            if (!String.IsNullOrEmpty(DatabaseName))
                sb.Append(DatabaseName).Append(".");

            if (!String.IsNullOrEmpty(SchemaName))
                sb.Append(SchemaName);
            
            sb.Append(".");
            sb.Append(ObjectName);
            
            while (sb.Length > 0 && sb[0] == '.')
                sb.Remove(0, 1);

            return sb.ToString();
        }
    }
}
