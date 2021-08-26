using Idera.SQLdm.Common.Attributes;

namespace Idera.SQLdm.Common.Notification.Providers
{
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Xml.Serialization;
    using Idera.SQLdm.Common.Security.Encryption;
    using System;

    [Serializable]
    [Guid("A8B9AA43-CC8C-45a6-88B3-11727B3A5405")]
    [ProviderInfo(typeof(PulseDestination), DestinationTypeLabel = "Send alert to <a href=\"internal:PulseDestination{0}\">Newsfeed</a>")]
    [DisplayName("Newsfeed Action Provider"), Description("Action provider that sends alerts to the IDERA News Server")]
    public class PulseNotificationProviderInfo : NotificationProviderInfo
    {
        public const string PROPERTY_PULSE_DESCRIPTION  = "Description";
        public const string PROPERTY_PULSE_SERVER       = "PulseServer";
        public const string PROPERTY_PULSE_PORT         = "PulseServerPort";
        private const string NewsServer = "News server";
        private const string NewsServerPort = "News server port";

        public PulseNotificationProviderInfo()
            : this(true)
        {
        }

        public PulseNotificationProviderInfo(bool enabled)
            : base("Idera.SQLdm.ManagementService.Notification.Providers.PulseNotificationProvider", enabled)
        {
        }

        public PulseNotificationProviderInfo(NotificationProviderInfo info)
            : base(info.ProviderTypeName, info.Enabled)
        {
            this.Id = info.Id;
            this.Name = info.Name;
            this.Properties = info.Properties;
        }

        /// <summary>
        /// Returns a description for the notification provider.
        /// </summary>
        public override string Caption
        {
            get
            {
                return "Pulse Notification Provider";
            }
        }

        [XmlAttribute]
        [Description(PROPERTY_PULSE_DESCRIPTION)]
        [Auditable(PROPERTY_PULSE_DESCRIPTION)]
        public string Description
        {
            get
            {
                NotificationProviderProperty property = GetProperty(PROPERTY_PULSE_DESCRIPTION);
                return (property == null) ? "" : property.Value as string;
            }
            set
            {
                this.SetProperty(PROPERTY_PULSE_DESCRIPTION, value);
            }
        }

        [XmlAttribute]
        [Description(NewsServer)]
        [Auditable(NewsServer)]
        public string PulseServer
        {
            get
            {
                NotificationProviderProperty property = GetProperty(PROPERTY_PULSE_SERVER);
                return (property == null) ? "" : property.Value as string;
            }
            set
            {
                this.SetProperty(PROPERTY_PULSE_SERVER, value);
            }
        }

        [XmlAttribute]
        [Description(NewsServerPort)]
        [Auditable(NewsServerPort)]
        public int PulseServerPort
        {
            get
            {
                NotificationProviderProperty property = GetProperty(PROPERTY_PULSE_PORT);
                return (property == null) ? 5168 : (int)property.Value;
            }
            set
            {
                this.SetProperty(PROPERTY_PULSE_PORT, value);
            }
        }
    }
}
