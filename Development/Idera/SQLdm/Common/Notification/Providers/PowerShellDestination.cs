//------------------------------------------------------------------------------
// <copyright file="PowerShellDestination.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
//File added to support Alert Power Shell Provider
//SQLdm 10.1 Srishti Purohit
namespace Idera.SQLdm.Common.Notification.Providers
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Xml.Serialization;
    

    [Serializable]
    public class PowerShellDestination : NotificationDestinationInfo
    {
        public const string PROPERTY_DESCRIPTION = "Description";
        public const string PROPERTY_SERVER      = "Server";
        public const string PROPERTY_Command = "Command";

        public PowerShellDestination()
        {
            // get the id of the notification provider from attributes
            GuidAttribute attribute =
                Attribute.GetCustomAttribute(typeof(PowerShellNotificationProviderInfo), typeof(GuidAttribute)) as GuidAttribute;
            if (attribute != null)
                this.ProviderID = new Guid(attribute.Value);
        }

        public PowerShellDestination(NotificationDestinationInfo destination)
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

        //[XmlAttribute]
        //[Description("Server")]
        //public string Server
        //{
        //    get
        //    {
        //        NotificationDestinationProperty property = GetProperty(PROPERTY_SERVER);
        //        return (property == null) ? "" : property.Value as string;
        //    }
        //    set
        //    {
        //        this.SetProperty(PROPERTY_SERVER, value);
        //    }
        //}

        [XmlElement]
        [Description("Command")]
        public string Command
        {
            get
            {
                NotificationDestinationProperty property = GetProperty(PROPERTY_Command);
                return (property == null) ? "" : property.Value as string;
            }
            set
            {
                this.SetProperty(PROPERTY_Command, value);
            }
        }

        public override void Validate()
        {
            string desc = Description.Trim();
            bool needsHelp = desc.Length == 0;
            //if (!needsHelp)
            //{
            //    string value = Server.Trim();
            //    needsHelp = value.Length == 0;
            //    if (!needsHelp)
            //    {
            //        value = Command.Trim();
            //        needsHelp = value.Length == 0;
            //    }
            //}
            if (needsHelp)
            {
                string message2;
                if (desc.Length == 0)
                    message2 = "Please select the red power shell command link and configure your power shell command action.";
                else
                    message2 = String.Format("Please select the '{0}' link and configure your power shell action.", desc);
                
                throw new ActionRuleValidationException(
                    false, 
                    true, 
                    true,
                    "Power Shell Action Needs Configuration",
                    message2);
            }            
        }
    }
}
