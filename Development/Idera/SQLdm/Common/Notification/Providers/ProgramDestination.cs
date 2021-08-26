//------------------------------------------------------------------------------
// <copyright file="ProgramDestination.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Notification.Providers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Xml.Serialization;

    [Serializable]
    public class ProgramDestination : NotificationDestinationInfo
    {
        public const string PROPERTY_DESCRIPTION = "Description";
        public const string PROPERTY_CMDLINE     = "CmdLine";
        public const string PROPERTY_STARTIN     = "StartIn";        

        public ProgramDestination()
        {
            // get the id of the notification provider from attributes
            GuidAttribute attribute =
                Attribute.GetCustomAttribute(typeof(ProgramNotificationProviderInfo), typeof(GuidAttribute)) as GuidAttribute;
            if (attribute != null)
                this.ProviderID = new Guid(attribute.Value);
        }

        public ProgramDestination(NotificationDestinationInfo destination)
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
                return (property == null) ? String.Empty : property.Value as string;
            }
            set
            {
                this.SetProperty(PROPERTY_DESCRIPTION, value);
            }
        }

        [XmlAttribute]
        [Description("Command")]
        public string Command
        {
            get
            {
                NotificationDestinationProperty property = GetProperty(PROPERTY_CMDLINE);
                return (property == null) ? String.Empty : property.Value as string;
            }
            set
            {
                this.SetProperty(PROPERTY_CMDLINE, value);
            }
        }
        
        [XmlAttribute]
        [Description("StartIn")]
        public string StartIn
        {
            get
            {
                NotificationDestinationProperty property = GetProperty(PROPERTY_STARTIN);
                return (property == null) ? String.Empty : property.Value as string;
            }
            set
            {
                this.SetProperty(PROPERTY_STARTIN, value);
            }
        }

        public override void Validate()
        {
            bool needsHelp = false;
            // make sure the description is configured
            string desc = Description.Trim();
            needsHelp = desc.Length == 0;
            if (!needsHelp)
            {
                string value = Command.Trim();
                needsHelp = value.Length == 0;
            }
            if (needsHelp)
            {
                string message2;
                if (desc.Length == 0)
                    message2 = "Please select the red Program link and configure the program action.";
                else
                    message2 = String.Format("Please select the '{0}' link and configure the program action.", desc);

                throw new ActionRuleValidationException(
                    false, 
                    true, 
                    true, 
                    "Program Action Needs Configuration", 
                    message2);
            }
        }

        public static void ParseCommandLine(string commandLine, out string path, out string arguments)
        {
            string arg = "";
            char quoteChar = '"';
            bool inQuotes = false;

            for (int i = 0; i != commandLine.Length; ++i)
            {
                char c = commandLine[i];

                if (!inQuotes && c == '\'' || c == '"')
                    quoteChar = c;

                if (c == quoteChar)
                {
                    if (inQuotes)
                    {
                        // Read ahead to check for doublequote pairs
                        ++i;
                        if (i == commandLine.Length) break;
                        if (commandLine[i] == quoteChar)
                        {
                            arg += c;
                            continue;
                        }
                        --i;
                        inQuotes = false;
                    }
                    else
                    {
                        inQuotes = true;
                    }

                    continue;
                }
                if (!inQuotes && Char.IsWhiteSpace(c))
                {
                    path = commandLine.Substring(0, i);
                    arguments = commandLine.Substring(i).Trim();
                    return;
                }
            }
            path = commandLine.Trim();
            arguments = String.Empty;
        }

    }
}
