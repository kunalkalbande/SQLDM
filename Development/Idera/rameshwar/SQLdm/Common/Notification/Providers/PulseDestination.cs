using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.Common.Notification.Providers
{
    using System;
    using System.ComponentModel;
    using System.Xml.Serialization;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Represents an Pulse destination 
    /// </summary>
    [Serializable]
    public class PulseDestination : NotificationDestinationInfo
    {
        public const string PROPERTY_SUBJECT = "Subject";
        public const string PROPERTY_BODY = "Body";        

        public PulseDestination()
        {
            // get the id of the notification provider from attributes
            GuidAttribute attribute =
                Attribute.GetCustomAttribute(typeof(PulseNotificationProviderInfo), typeof(GuidAttribute)) as GuidAttribute;
            if (attribute != null)
                this.ProviderID = new Guid(attribute.Value);

            Subject = "$(AlertSummary)";
            Body = "$(AlertSummary)";
        }

        public PulseDestination(NotificationDestinationInfo destination)
        {
            this.Properties = destination.Properties;
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
            return Provider.Name;
        }

        public override void Validate()
        {
            base.Validate();
        }
    }
}
