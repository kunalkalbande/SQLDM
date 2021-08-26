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
    using System.Collections.Generic;
    using Idera.SQLdm.Common.Analysis;
    using System.Threading;

    [Serializable]
    public class EnablePADestination : NotificationDestinationInfo
    {
        public const string PROPERTY_CATEGORY = "BlockedCategories";
        private List<RecommendationCategory> blockedCategories;

        public EnablePADestination()
        {
            // get the id of the notification provider from attributes
            GuidAttribute attribute =
                Attribute.GetCustomAttribute(typeof(EnablePANotificationProviderInfo), typeof(GuidAttribute)) as GuidAttribute;
            if (attribute != null)
                this.ProviderID = new Guid(attribute.Value);
        }

        public EnablePADestination(NotificationDestinationInfo destination)
        {
            this.Properties = destination.Properties;
        }

        [XmlArray]
        public List<RecommendationCategory> BlockedCategoriesListObject
        {
            get
            {
                NotificationDestinationProperty property = GetProperty(PROPERTY_CATEGORY);
                return (property == null) ? null : property.Value as List<RecommendationCategory>;
            }
            set
            {
                this.SetProperty(PROPERTY_CATEGORY, value);
            }

        }

        public override string ToString()
        {
            if (IsPropertySet(PROPERTY_CATEGORY))
            {
                return "For selected Categories";
            }
            return String.Empty;
        }

        public override void Validate()
        {
            bool needsHelp = false;
            string message2 = String.Empty;

            if (!IsPropertySet(PROPERTY_CATEGORY))
            {
                needsHelp = true;
                message2 = "Please select the red 'for this time period' link and configure the enable prescriptive analyzer action.";
            }
            else
            {
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
