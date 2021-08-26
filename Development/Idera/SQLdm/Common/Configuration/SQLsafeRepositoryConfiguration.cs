using System;
using System.Runtime.Serialization;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Security.Encryption;

namespace Idera.SQLdm.Common.Configuration
{
    [Serializable]
    public class SQLsafeRepositoryConfiguration : ISerializable
    {
        
        private const string CipherInstanceName = "Idera.SQLdm.Common";

        #region Properties

        public SqlConnectionInfo SQLsafeConnectionInfo { get; set; }
        public int SQLsafeInstanceId { get; set; }
        public int LastBackupActionId { get; set; }
        public int LastDefragActionId { get; set; }

        #endregion


        #region Constructors

        public SQLsafeRepositoryConfiguration()
        {
            
        }

        public SQLsafeRepositoryConfiguration(string instance, string database, bool securityMode, string user, string encryptedPassword, int instanceId, int lastBackupAction, int lastDefragAction)
        {
            this.SQLsafeConnectionInfo = new SqlConnectionInfo();
            SQLsafeConnectionInfo.InstanceName = instance;
            SQLsafeConnectionInfo.DatabaseName = database;
            SQLsafeConnectionInfo.UseIntegratedSecurity = securityMode;
            if (!SQLsafeConnectionInfo.UseIntegratedSecurity)
            {
                SQLsafeConnectionInfo.UserName = user;
                SQLsafeConnectionInfo.EncryptedPassword = Cipher.EncryptPassword(CipherInstanceName,encryptedPassword); // TODO: Change to remove Cipher
            }
            SQLsafeInstanceId = instanceId;
            LastBackupActionId = lastBackupAction;
            LastDefragActionId = lastDefragAction;

        }

        public SQLsafeRepositoryConfiguration(SerializationInfo info, StreamingContext context)
        {
            this.SQLsafeConnectionInfo = (SqlConnectionInfo) info.GetValue("SQLsafeConnectionInfo",typeof(SqlConnectionInfo));
            SQLsafeInstanceId = info.GetInt32("SQLsafeInstanceId");
            LastBackupActionId = info.GetInt32("LastBackupActionId");
            LastDefragActionId = info.GetInt32("LastDefragActionId");
        }


        #endregion

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("SQLsafeConnectionInfo", SQLsafeConnectionInfo);
            info.AddValue("SQLsafeInstanceId", SQLsafeInstanceId);
            info.AddValue("LastBackupActionId", LastBackupActionId);
            info.AddValue("LastDefragActionId", LastDefragActionId);
        }


    }
}
