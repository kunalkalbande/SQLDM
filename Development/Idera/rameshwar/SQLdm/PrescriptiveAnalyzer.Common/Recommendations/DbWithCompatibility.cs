using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class DbWithCompatibility
    {
        private string dbName;

        private int compatibility;

        public string DbName
        {
            get { return dbName; }
            set { dbName = value; }
        }

        public int Compatibility
        {
            get { return compatibility; }
            set { compatibility = value; }
        }

    }
}
