//------------------------------------------------------------------------------
// <copyright file="JobDestination.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace Idera.SQLdm.Common.Notification.Providers
{
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Xml.Serialization;

    [Serializable]
    public class JobDestination : NotificationDestinationInfo
    {
        public const string PROPERTY_SERVER = "Server";
        public const string PROPERTY_JOB    = "Job";
        public const string PROPERTY_STEP   = "Step";        

        public JobDestination()
        {
            // get the id of the notification provider from attributes
            GuidAttribute attribute =
                Attribute.GetCustomAttribute(typeof(JobNotificationProviderInfo), typeof(GuidAttribute)) as GuidAttribute;
            if (attribute != null)
                this.ProviderID = new Guid(attribute.Value);
        }

        public JobDestination(NotificationDestinationInfo destination)
        {
            this.Properties = destination.Properties;
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

        [XmlAttribute]
        [Description("Step")]
        public string Step
        {
            get
            {
                NotificationDestinationProperty property = GetProperty(PROPERTY_STEP);
                return (property == null) ? "" : property.Value as string;
            }
            set
            {
                this.SetProperty(PROPERTY_STEP, value);
            }
        }

        [XmlText]
        [Description("Job")]
        public string Job
        {
            get
            {
                NotificationDestinationProperty property = GetProperty(PROPERTY_JOB);
                return (property == null) ? "" : property.Value as string;
            }
            set
            {
                this.SetProperty(PROPERTY_JOB, value);
            }
        }

        public override string ToString()
        {
            string job = Job;
            if (String.IsNullOrEmpty(job))
                return String.Empty;

            string server = Server;
            if (!String.IsNullOrEmpty(server))
                return String.Format("{0} on {1}", job, server);

            return job;
        }

        public override void Validate()
        {
            string server = Server.Trim();
            string job = Job.Trim();
            bool needsHelp = job.Length == 0 || server.Length == 0;
            if (needsHelp)
            {
                string message2;
                if (job.Length == 0)
                    message2 = "Please select the red SQL Agent Job link and configure your SQL Agent Job action.";
                else
                    message2 = String.Format("Please select the '{0}' link and configure your SQL Agent Job action.", ToString());

                throw new ActionRuleValidationException(
                    false,
                    true,
                    true,
                    "Sql Agent Job Action Needs Configuration",
                    message2);
            }
        }
    }
}
