using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.Common.Configuration;


namespace Idera.SQLdm.Common.Snapshots
{
    [Serializable]
    public sealed class DatabaseNamesSnapshot : Snapshot
    {
        
        #region fields

        //private List<string> databaseNames;
        private Dictionary<int, string> databases;

        #endregion

        #region constructors

        public DatabaseNamesSnapshot(SqlConnectionInfo info)
            : base(info.InstanceName)
        {

        }

        #endregion

        #region properties


        public Dictionary<int, string> Databases
        {
            get { return databases; }
            set { databases = value; }
        }
        
        #endregion

        #region events

        #endregion

        #region methods

       
        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion
    }
}
