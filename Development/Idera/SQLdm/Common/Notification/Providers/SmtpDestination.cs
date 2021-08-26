//------------------------------------------------------------------------------
// <copyright file="SmtpNotificationDestination.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Notification.Providers
{
    using System;
    using System.ComponentModel;
    using System.Net.Mail;
    using System.Xml.Serialization;
    using System.Text;

    /// <summary>
    /// Represents an Smtp destination (email address, recipient name)
    /// </summary>
    [Serializable]
    public class SmtpDestination : NotificationDestinationInfo
    {
        public const string PROPERTY_SENDER = "Sender";
        public const string PROPERTY_RECIPIENTS = "Recipients";
        public const string PROPERTY_SUBJECT = "Subject";
        public const string PROPERTY_BODY = "Body";        
        
        #region constructors

        public SmtpDestination()
        {
        }

        public SmtpDestination(NotificationDestinationInfo destination)
        {
            this.Properties = destination.Properties;
        }
        
        #endregion

        #region properties

        [XmlAttribute]
        [Description("Name of the recipient")]
        public string Name
        {
            get
            {
                NotificationDestinationProperty property = GetProperty("Name");
                return property == null ? "" : property.Value.ToString();
            }
            set
            {
                SetProperty("Name", value);
            }
        }

        [XmlAttribute]
        [Description("Sender")]
        public string From
        {
            get
            {
                NotificationDestinationProperty property = GetProperty(PROPERTY_SENDER);
                return (property == null) ? "" : property.Value as string;
            }
            set
            {
                this.SetProperty(PROPERTY_SENDER, value);
            }
        }
        [XmlAttribute]
        [Description("Recipients")]
        public string To
        {
            get
            {
                NotificationDestinationProperty property = GetProperty(PROPERTY_RECIPIENTS);
                return (property == null) ? "" : property.Value as string;
            }
            set
            {
                this.SetProperty(PROPERTY_RECIPIENTS, value);
            }
        }

        [XmlAttribute]
        [Description("Subject")]
        public string Subject
        {
            get
            {
                NotificationDestinationProperty property = GetProperty(PROPERTY_SUBJECT);
                return (property == null) ? "" : property.Value as string;
            }
            set
            {
                this.SetProperty(PROPERTY_SUBJECT, value);
            }
        }

        [XmlText]
        [Description("Body")]
        public string Body
        {
            get
            {
                NotificationDestinationProperty property = GetProperty(PROPERTY_BODY);
                return (property == null) ? "" : property.Value as string;
            }
            set
            {
                this.SetProperty(PROPERTY_BODY, value);
            }
        }

        #endregion

        #region events

        #endregion

        #region methods

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            MailAddressCollection mac = new MailAddressCollection();
            if (SmtpAddressHelpers.ParseAddressesInto(To, mac))
            {
                foreach (MailAddress address in mac)
                {
                    if (builder.Length > 0)
                        builder.Append(", ");
                    string display = address.DisplayName;
                    if (String.IsNullOrEmpty(display))
                        display = address.Address;
                    builder.Append(display);
                }
            } else
            foreach (string address in To.Split(';', ','))
            {
                if (builder.Length > 0)
                    builder.Append(", ");
                builder.Append(address);
            }

            return builder.ToString();
        }

        public override void Validate()
        {
            string address = To.Trim();
            bool needsHelp = address.Length == 0;
            if (!needsHelp)
            {
                needsHelp = !SmtpAddressHelpers.IsMailAddressValid(address, false);
                if (!needsHelp)
                {
                    string value = From.Trim();
                    needsHelp = value.Length == 0;
                    if (!needsHelp)
                    {
                        needsHelp = !SmtpAddressHelpers.IsMailAddressValid(value, true);
                        if (!needsHelp)
                        {
                            value = Subject.Trim();
                            needsHelp = value.Length == 0;
                        }
                    }
                }
            }
            if (needsHelp)
            {
                string message2;
                if (address.Length == 0)
                    message2 = "Please select the red recipient link and configure your SMTP action.";
                else
                    message2 = String.Format("Please select the '{0}' link and configure your SMTP action.", ToString());

                throw new ActionRuleValidationException(
                    false,
                    true,
                    true,
                    "SMTP Action Needs Configuration",
                    message2);
            }
        }

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion
    }
}
