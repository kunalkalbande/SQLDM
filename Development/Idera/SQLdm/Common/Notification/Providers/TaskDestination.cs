//------------------------------------------------------------------------------
// <copyright file="ODBCDestination.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Notification.Providers
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using Wintellect.PowerCollections;
    using System.ComponentModel;
    using System.Xml.Serialization;

    [Serializable]
    public class TaskDestination : NotificationDestinationInfo
    {
        public const string PROPERTY_SUBJECT = "Subject";
        public const string PROPERTY_BODY = "Body";        

        private OrderedSet<string> users;

        public TaskDestination()
        {
            // get the id of the notification provider from attributes
            GuidAttribute attribute =
                Attribute.GetCustomAttribute(typeof(TaskNotificationProviderInfo), typeof (GuidAttribute)) as GuidAttribute;
            if (attribute != null)
                this.ProviderID = new Guid(attribute.Value);

            Subject = "$(AlertSummary)";
            Body = "$(AlertText)\r\n\r\n$(Metric): $(Description)";
        }

        public TaskDestination(NotificationDestinationInfo destination)
        {
            this.Properties = destination.Properties;
        }

        public string[] Users
        {
            get
            {
                if (users == null)
                    return new string[0];
                return users.ToArray();
            }
            set 
            {
                if (value == null)
                {
                    users = null;
                } else
                {
                    foreach (string s in value)
                    {
                        if (s.TrimEnd().Length > 256)
                        {
                            throw new ArgumentException(
                                String.Format("User '{0}' is longer than the allowable length of 256.  Please shorten the name or remove the offending entry.", s));
                        }
                    }

                    if (users == null)
                        users = new OrderedSet<string>();
                    else
                        users.Clear();

                    foreach (string s in value)
                    {
                        users.Add(s.Trim());
                    }
                }
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


        public override string ToString()
        {
            if (users == null || users.Count == 0)
            {
                return "someone";
            }
            
            StringBuilder builder = new StringBuilder();

            foreach (string user in users)
            {
                if (builder.Length > 0)
                    builder.Append(", ");
                builder.Append(user);
            }

            return builder.ToString();
        }
        
    }
}
