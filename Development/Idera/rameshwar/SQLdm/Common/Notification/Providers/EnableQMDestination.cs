//------------------------------------------------------------------------------
// <copyright file="EnableQMDestination.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace Idera.SQLdm.Common.Notification.Providers
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Xml.Serialization;

    [Serializable]
    public class EnableQMDestination : NotificationDestinationInfo
    {
        public const string PROPERTY_DURATION = "Duration";

        public EnableQMDestination()
        {
            // get the id of the notification provider from attributes
            GuidAttribute attribute =
                Attribute.GetCustomAttribute(typeof(EnableQMNotificationProviderInfo), typeof(GuidAttribute)) as GuidAttribute;
            if (attribute != null)
                this.ProviderID = new Guid(attribute.Value);
        }

        public EnableQMDestination(NotificationDestinationInfo destination)
        {
            this.Properties = destination.Properties;
        }

        [XmlAttribute]
        [Description("DurationInMinutes")]
        public int DurationInMinutes
        {
            get
            {
                NotificationDestinationProperty property = GetProperty(PROPERTY_DURATION);
                return (property == null) ? 15 : Convert.ToInt32(property.Value);
            }
            set
            {
                this.SetProperty(PROPERTY_DURATION, value);
            }
        }

        public override string ToString()
        {
            if (IsPropertySet(PROPERTY_DURATION))
            {
                int duration = DurationInMinutes;
                if (duration == 0)
                    return "until turned off";
                return String.Format("for {0} minutes", DurationInMinutes);
            }
            return String.Empty;
        }

        public override void Validate()
        {
            bool needsHelp = false;
            string message2 = String.Empty;

            if (!IsPropertySet(PROPERTY_DURATION))
            {
                needsHelp = true;
                message2 = "Please select the red 'for this time period' link and configure the enable query monitor action.";
            }
            else
            {
                int duration = DurationInMinutes;
                if (duration < 0 || duration > 1440)
                {
                    needsHelp = true;
                    message2 = String.Format("Please select the 'for this time period' link and configure the enable query monitor action.");
                }
            }
            if (needsHelp)
            {
                throw new ActionRuleValidationException(
                    false,
                    true,
                    true,
                    "Program Action Needs Configuration",
                    message2);
            }
        }
    }
}
