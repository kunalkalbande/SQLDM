//------------------------------------------------------------------------------
// <copyright file="SqlDestination.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Notification.Providers
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Xml.Serialization;
    

    [Serializable]
    public class SqlDestination : NotificationDestinationInfo
    {
        public const string PROPERTY_DESCRIPTION = "Description";
        public const string PROPERTY_SERVER      = "Server";
        public const string PROPERTY_SQL         = "Sql";

        public SqlDestination()
        {
            // get the id of the notification provider from attributes
            GuidAttribute attribute =
                Attribute.GetCustomAttribute(typeof(SqlNotificationProviderInfo), typeof(GuidAttribute)) as GuidAttribute;
            if (attribute != null)
                this.ProviderID = new Guid(attribute.Value);
        }

        public SqlDestination(NotificationDestinationInfo destination)
        {
            this.Properties = destination.Properties;
        }

        public override string ToString()
        {
            string desc = Description;
            if (String.IsNullOrEmpty(desc))
                return String.Empty;

            return desc;
        }

        [XmlText]
        [Description("Description")]
        public string Description
        {
            get
            {
                NotificationDestinationProperty property = GetProperty(PROPERTY_DESCRIPTION);
                return (property == null) ? "" : property.Value as string;
            }
            set
            {
                this.SetProperty(PROPERTY_DESCRIPTION, value);
            }
        }

        [XmlAttribute]
        [Description("Server")]
        public string Server
        {
            get
            {
                NotificationDestinationProperty property = GetProperty(PROPERTY_SERVER);
                return (property == null) ? "" : property.Value as string;
            }
            set
            {
                this.SetProperty(PROPERTY_SERVER, value);
            }
        }

        [XmlElement]
        [Description("Sql")]
        public string Sql
        {
            get
            {
                NotificationDestinationProperty property = GetProperty(PROPERTY_SQL);
                return (property == null) ? "" : property.Value as string;
            }
            set
            {
                this.SetProperty(PROPERTY_SQL, value);
            }
        }

        public override void Validate()
        {
            string desc = Description.Trim();
            bool needsHelp = desc.Length == 0;
            if (!needsHelp)
            {
                string value = Server.Trim();
                needsHelp = value.Length == 0;
                if (!needsHelp)
                {
                    value = Sql.Trim();
                    needsHelp = value.Length == 0;
                }
            }
            if (needsHelp)
            {
                string message2;
                if (desc.Length == 0)
                    message2 = "Please select the red Sql Script link and configure your sql script action.";
                else
                    message2 = String.Format("Please select the '{0}' link and configure your sql script action.", desc);
                
                throw new ActionRuleValidationException(
                    false, 
                    true, 
                    true, 
                    "Sql Script Action Needs Configuration",
                    message2);
            }            
        }
    }
}
