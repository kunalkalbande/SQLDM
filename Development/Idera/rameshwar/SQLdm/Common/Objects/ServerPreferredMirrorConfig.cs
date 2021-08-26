using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.Common.Snapshots;

namespace Idera.SQLdm.Common.Objects
{
    [Serializable]
    public class ServerPreferredMirrorConfig
    {
        private MirroringMetrics.MirroringRoleEnum? _role;
        private string _witness;
        private string _database;
        
        public ServerPreferredMirrorConfig(MirroringMetrics.MirroringRoleEnum? role, string Witness, string Database)
        {
            _role = role;
            _witness = Witness;
            _database = Database;
        }

        public string WitnessName
        {
            get { return _witness; }
            set { _witness = value; }
        }

        public string MirroredDatabase
        {
            get { return _database; }
            set { _database = value; }
        }

        public MirroringMetrics.MirroringRoleEnum? Role
        {
            get { return _role; }
            set { _role = value; }
        }
    }
}
