//------------------------------------------------------------------------------
// <copyright file="SmtpNotificationProviderInfo.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using Idera.SQLdm.Common.Attributes;

namespace Idera.SQLdm.Common.Notification.Providers
{
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Xml.Serialization;
    using Idera.SQLdm.Common.Security.Encryption;
    using System;
    using System.Text;

    [Serializable]
    [Guid("C7D96EEB-2150-4f61-BC4F-CE6F14BAE699")]
    [ProviderInfo(typeof(SmtpDestination), DestinationTypeLabel="Send an email to <a href=\"internal:SmtpDestination{0}\">recipient</a>")]
    [DisplayName("Smtp Action Provider"), Description("Action provider that sends alerts using the smtp protocol")]
    public class SmtpNotificationProviderInfo : NotificationProviderInfo
    {
        private const string CipherInstanceName = "Idera.SQLdm.Common";
        public const string PROPERTY_SMTP_SERVER = "SmtpServer";
        public const string PROPERTY_SMTP_PORT = "SmtpServerPort";
        public const string PROPERTY_SENDER_EMAIL = "SenderAddress";
        public const string PROPERTY_SENDER_DISPLAY_NAME = "SenderName";
        public const string PROPERTY_REQUIRES_AUTHENTICATION = "RequiresAuthentication";
        public const string PROPERTY_USER_NAME = "UserName";
        public const string PROPERTY_PASSWORD = "Password";
        public const string PROPERTY_LOGIN_TIMEOUT = "LoginTimeout";
        public const string PROPERTY_RETRY_ATTEMPTS = "RetryAttemtps";
        public const string PROPERTY_REQUIRES_SSL = "RequiresSSL";
        private const string SmtpServerRequiresSsl = "SMTP server requires SSL";
        private const string EncryptedPasswordText = "Encrypted Password";
        private const string AuthenticateToTheSmtpServerAsThisUser = "Authenticate to the SMTP server as this user";
        private const string SmtpServerRequiresAuthentication = "SMTP server requires authentication";
        private const string SmtpSenderNameToUseForSenderReplyTo = "SMTP sender name to use for sender/reply-to";
        private const string SmtpAddressToUseForSenderReplyTo = "SMTP address to use for sender/reply-to";
        private const string SmtpServerPortTextDescription = "SMTP server port";
        private const string SmtpServerToRelayMailThrough = "SMTP server to relay mail through";

        public SmtpNotificationProviderInfo() : this(true)
        {
        }
        
        public SmtpNotificationProviderInfo(bool enabled) : base("Idera.SQLdm.ManagementService.Notification.Providers.SmtpNotificationProvider", enabled)
        {
        }
        
        public SmtpNotificationProviderInfo(NotificationProviderInfo info) : base(info.ProviderTypeName, info.Enabled)
        {
            this.Id = info.Id;
            this.Name = info.Name;
//            this.Description = info.Description;
            this.Properties = info.Properties;
        }

        /// <summary>
        /// Returns a description for the notification provider.
        /// </summary>
        [XmlAttribute]
        [Auditable(false)]
        public override string Caption
        {
            get
            {
                return "Smtp Notification Provider";
            }
        }

        [XmlAttribute]
        [Description("Seconds to wait before giving up"),DefaultValue(15)]
        [Auditable(false)]
        public int LoginTimeout
        {
            get
            {
                NotificationProviderProperty property = GetProperty(PROPERTY_LOGIN_TIMEOUT);
                return (property == null) ? 15 : (int)property.Value;
            }
            set
            {
                this.SetProperty(PROPERTY_LOGIN_TIMEOUT, value);
            }
        }

        /// <summary>
        /// Returns the formatted string
        /// </summary>
        public string ReadableSeconds(int seconds)
        {
            TimeSpan timeout = TimeSpan.FromSeconds(seconds);
            int tminutes = timeout.Minutes;
            int tseconds = timeout.Seconds;

            StringBuilder builder = new StringBuilder();

            if (tminutes > 0)
            {
                builder.AppendFormat("{0} minute", tminutes);
                if (tminutes > 1)
                    builder.Append('s');
                builder.Append(' ');
            }
            if (tseconds > 0)
            {
                builder.AppendFormat("{0} second", tseconds);
                if (tseconds > 1)
                    builder.Append('s');
            }

            return builder.ToString();
        }

        /// <summary>
        /// Returns the LoginTimeout value as a formatted string
        /// </summary>
        [Auditable("Login Timeout")]
        public string LoginTimeoutStr
        {
            get
            {
                return ReadableSeconds(LoginTimeout);
            }
        }

        [XmlAttribute]
        [Description("Number of attempts to send the alert before giving up"),DefaultValue(3)]
        [Auditable(false)]
        public int RetryAttempts
        {
            get
            {
                NotificationProviderProperty property = GetProperty(PROPERTY_RETRY_ATTEMPTS);
                return (property == null) ? 3 : (int)property.Value;
            }
            set
            {
                this.SetProperty(PROPERTY_RETRY_ATTEMPTS, value);
            }
        }

        [XmlAttribute]
        [Description(SmtpServerToRelayMailThrough)]
        [Auditable(SmtpServerToRelayMailThrough)]
        public string SmtpServer
        {
            get
            {
                NotificationProviderProperty property = GetProperty(PROPERTY_SMTP_SERVER);
                return (property == null) ? "" : property.Value as string;
            }
            set
            {
                this.SetProperty(PROPERTY_SMTP_SERVER, value);     
            }
        }

        [XmlAttribute]
        [Description(SmtpServerPortTextDescription)]
        [Auditable(SmtpServerPortTextDescription)]
        public int SmtpServerPort
        {
            get
            {
                NotificationProviderProperty property = GetProperty(PROPERTY_SMTP_PORT);
                return (property == null) ? 25 : (int)property.Value;
            }
            set
            {
                this.SetProperty(PROPERTY_SMTP_PORT, value);
            }
        }



        [XmlAttribute]
        [Description(SmtpAddressToUseForSenderReplyTo)]
        [Auditable(SmtpAddressToUseForSenderReplyTo)]
        public string SenderAddress
        {
            get
            {
                NotificationProviderProperty property = GetProperty(PROPERTY_SENDER_EMAIL);
                return (property == null) ? "" : property.Value as string;
            }
            set
            {
                this.SetProperty(PROPERTY_SENDER_EMAIL, value);
            }
        }

        [XmlAttribute]
        [Description(SmtpAddressToUseForSenderReplyTo), DefaultValue("SQL Diagnostic Manager")]
        [Auditable(SmtpSenderNameToUseForSenderReplyTo)]
        public string SenderName
        {
            get
            {
                NotificationProviderProperty property = GetProperty(PROPERTY_SENDER_DISPLAY_NAME);
                return (property == null) ? "" : property.Value as string;
            }
            set
            {
                this.SetProperty(PROPERTY_SENDER_DISPLAY_NAME, value);
            }
        }

        [XmlAttribute]
        [Description(SmtpServerRequiresAuthentication), DefaultValue(false),Category("Authentication")]
        [Auditable(SmtpServerRequiresAuthentication)]
        public bool RequiresAuthentication
        {
            get
            {
                NotificationProviderProperty property = GetProperty(PROPERTY_REQUIRES_AUTHENTICATION);
                return (property == null) ? false : (bool)property.Value;
            }
            set
            {
                this.SetProperty(PROPERTY_REQUIRES_AUTHENTICATION, value);
            }
        }

        [XmlAttribute]
        [Description(AuthenticateToTheSmtpServerAsThisUser), Category("Authentication")]
        [Auditable(AuthenticateToTheSmtpServerAsThisUser)]
        public string UserName
        {
            get
            {
                NotificationProviderProperty property = GetProperty(PROPERTY_USER_NAME);
                return (property == null) ? "" : property.Value as string;
            }
            set
            {
                this.SetProperty(PROPERTY_USER_NAME, value);
            }
        }

        /// <summary>
        /// The password used to authenticate to the smtp server.  This property is marked XmlIgnore
        /// to prevent the serializer from serializing a readable password.
        /// </summary>
        [XmlIgnore]
        [Description("Authenticate to the SMTP server with this password"), PasswordPropertyText(true), Category("Authentication")]
        [AuditableAttribute(true, true)]
        public string Password
        {
            get
            {
                NotificationProviderProperty property = GetProperty(PROPERTY_PASSWORD);
                return (property == null) ? "" : property.Value as string;
            }
            set
            {
                this.SetProperty(PROPERTY_PASSWORD, value);
            }
        }

        /// <summary>
        /// The password used to authenticate to the smtp server.  This property is marked XmlIgnore
        /// to prevent the serializer from serializing a readable password.
        /// </summary>
        [XmlAttribute]
        [AuditableAttribute(false)]
        public string EncryptedPassword
        {
            get
            {
                NotificationProviderProperty property = GetProperty(PROPERTY_PASSWORD);
                if (property == null)
                    return null;

                return Cipher.EncryptPassword(CipherInstanceName, property.Value as string);
            }
            set
            {
                Password = Cipher.DecryptPassword(CipherInstanceName, value);
            }
        }

        [XmlAttribute]
        [Description(SmtpServerRequiresSsl), DefaultValue(false), Category("Authentication")]
        [Auditable(SmtpServerRequiresSsl)]
        public bool RequiresSSL
        {
            get
            {
                NotificationProviderProperty property = GetProperty(PROPERTY_REQUIRES_SSL);
                return (property == null) ? false : (bool)property.Value;
            }
            set
            {
                this.SetProperty(PROPERTY_REQUIRES_SSL, value);
            }
        }
    }
}
