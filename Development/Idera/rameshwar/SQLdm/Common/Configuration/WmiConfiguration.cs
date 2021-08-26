using System;
using System.Runtime.Serialization;
using Idera.SQLdm.Common.Attributes;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Security.Encryption;

namespace Idera.SQLdm.Common.Configuration
{
    [Serializable]
    public class WmiConfiguration : ISerializable
    {
        private const string CipherInstanceName = "Idera.SQLdm.Common";
        
        public MonitoredSqlServer.OleAutomationExecutionContext OleAutomationContext { get; set; }
        private bool optionWmiNoneDisabled;
        public bool OptionWmiNoneDisabled
        {
            get { return optionWmiNoneDisabled; }
            set
            {
                if (!value) directWmEnabled = false;
                optionWmiNoneDisabled = value;
            }
        }
        private bool oleAutomationDisabled;
        public bool OleAutomationDisabled 
        { 
            get { return oleAutomationDisabled; } 
            set
            {
                if (!value) directWmEnabled = false;
                oleAutomationDisabled = value;
            } 
        }

        private bool directWmEnabled;
        public bool DirectWmiEnabled 
        { 
            get { return directWmEnabled; } 
            set 
            { 
                if (value) OleAutomationDisabled = true;
                if (value) optionWmiNoneDisabled = true;
                directWmEnabled = value;
            } 
        }

        [AuditableAttribute("Use SQLdm Collection Service Account")]
        public bool DirectWmiConnectAsCollectionService { get; set; }

        [AuditableAttribute("WMI User Name")]
        public string DirectWmiUserName { get; set; }

        [AuditableAttribute("Changed WMI Password", true)]
        public string DirectWmiPassword { get; set; }

        public WmiConfiguration()
        {
            OptionWmiNoneDisabled = true;
            DirectWmiEnabled = true;
            DirectWmiConnectAsCollectionService = true;
            OleAutomationContext = MonitoredSqlServer.OleAutomationExecutionContext.Both;
        }

        private WmiConfiguration(SerializationInfo info, StreamingContext context)
        {
            OptionWmiNoneDisabled = true;
            OleAutomationDisabled = info.GetBoolean("OleAutomationDisabled");
            OleAutomationContext = (MonitoredSqlServer.OleAutomationExecutionContext)info.GetValue("OleAutomationContext", typeof(MonitoredSqlServer.OleAutomationExecutionContext));
            DirectWmiEnabled = info.GetBoolean("DirectWmiEnabled");
            DirectWmiConnectAsCollectionService = info.GetBoolean("DirectWmiConnectAsCollectionService");
            DirectWmiUserName = info.GetString("DirectWmiUserName");
            EncryptedPassword = info.GetString("EncryptedPassword");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("OptionWmiNoneDisabled", OptionWmiNoneDisabled);
            info.AddValue("OleAutomationDisabled", OleAutomationDisabled);
            info.AddValue("OleAutomationContext", OleAutomationContext);
            info.AddValue("DirectWmiEnabled", DirectWmiEnabled);
            info.AddValue("DirectWmiConnectAsCollectionService", DirectWmiConnectAsCollectionService);
            info.AddValue("DirectWmiUserName", DirectWmiUserName);
            info.AddValue("EncryptedPassword", EncryptedPassword);
        }

        [AuditableAttribute(false)]
        public string EncryptedPassword
        {
            get
            {
                // this will produce a different value every time
                return Cipher.EncryptPassword(CipherInstanceName, DirectWmiPassword);
            }

            set { DirectWmiPassword = Cipher.DecryptPassword(CipherInstanceName, value); }
        }

        public WmiConfiguration Copy()
        {
            var config = (WmiConfiguration)this.MemberwiseClone();
            config.DirectWmiEnabled = false;
            config.OleAutomationDisabled = false;
            config.OptionWmiNoneDisabled = false;
            return config;
        }
    }
}
