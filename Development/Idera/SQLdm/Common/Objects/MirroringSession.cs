using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.Common.Snapshots;

namespace Idera.SQLdm.Common.Objects
{
    /// <summary>
    /// Represents a preferred mirroring configuration for a mirroring session
    /// </summary>
    [Serializable]
    public class MirroringSession
    {
        /// <summary>
        /// Valid operational statuses that can be used to describe a mirroring session
        /// </summary>
        public enum MirroringPreferredConfig
        {
            Delete = -1,
            FailedOver = 0,
            Normal = 1
        }

        private Guid _Guid;
        private int _principalID;
        private int _mirrorID;
        private string _dbname;
        private string _witnessName;
        private MirroringPreferredConfig _preferredConfig;
        /// <summary>
        /// Creates an object that contains the mirroring configuration and a perferredConfig enum that contains the preference.
        /// For example, if principal is 1 and mirror is 2 but preferred config is "Failed Over", the preference is for 2 to be the principal
        /// and for 1 to be the mirror
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="principalID"></param>
        /// <param name="mirrorID"></param>
        /// <param name="preferredConfig"></param>
        /// <param name="MirroredDatabase"></param>
        /// <param name="Witness"></param>
        public MirroringSession(Guid guid, int principalID, int mirrorID, string MirroredDatabase, string Witness, MirroringPreferredConfig preferredConfig)
        {
            _Guid = guid;
            _principalID = principalID;
            _mirrorID = mirrorID;
            _preferredConfig = preferredConfig;
            _dbname = MirroredDatabase;
            _witnessName = Witness;
        }
        public MirroringSession(Guid guid, int principalID, int mirrorID, MirroringPreferredConfig preferredConfig)
            :this(guid,principalID,mirrorID,null,null,preferredConfig)
        {
            
        }

        public Guid MirroringGuid
        {
            set { _Guid = value; }
            get { return _Guid; }
        }
        public string Database
        {
            set { _dbname = value;}
            get { return _dbname; }
        }
        public string WitnessName
        {
            set { _witnessName = value;}
            get { return _witnessName; }
        }
        public int PrincipalID
        {
            set { _principalID = value; }
            get { return _principalID; }
        }
        public int MirrorID
        {
            set { _mirrorID = value; }
            get { return _mirrorID; }
        }
        
        /// <summary>
        /// This is actually the state that the user set for the saved principal and mirror ids
        /// If the saved state is failed over the the ids are obviously not really preferred at all
        /// and must be swapped to get the true preference.
        /// </summary>
        public MirroringPreferredConfig PreferredConfig
        {
            set { _preferredConfig = value; }
            get { return _preferredConfig; }
        }
        
        /// <summary>
        /// Pass in an operational configuration and this fubnction returns an enum telling you if this is the preferred state or not
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="mirror"></param>
        /// <returns>Is this failed over, normal or not set</returns>
        public MirroringPreferredConfig getCurrentOperationalState(int principal, int mirror)
        {
            if (_preferredConfig == MirroringPreferredConfig.FailedOver)
            {
                if (principal == _mirrorID && mirror == _principalID)
                {
                    return MirroringPreferredConfig.Normal;
                }
                if (principal == _principalID && mirror == _mirrorID)
                {
                    return MirroringPreferredConfig.FailedOver;
                }
                return MirroringPreferredConfig.Delete;
            }
            if (_preferredConfig == MirroringPreferredConfig.Normal)
            {
                if (principal == _principalID && mirror == _mirrorID)
                {
                    return MirroringPreferredConfig.Normal;
                }
                if (principal == _mirrorID && mirror == _principalID)
                {
                    return MirroringPreferredConfig.FailedOver;
                }
                return MirroringPreferredConfig.Delete;
            }
            return MirroringPreferredConfig.Delete;
        }
        public int PreferredMirrorID
        {
            get 
            {
                if (_preferredConfig == MirroringPreferredConfig.FailedOver)
                {
                    return _principalID;
                }
                return _mirrorID;
            }
        }

        public int PreferredPrincipalID
        {
            get
            {
                if (_preferredConfig == MirroringPreferredConfig.FailedOver)
                {
                    return _mirrorID;
                }
                return _principalID;
            }
        }
    }
}
