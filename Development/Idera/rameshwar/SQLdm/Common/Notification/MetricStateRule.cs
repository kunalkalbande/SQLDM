//------------------------------------------------------------------------------
// <copyright file="MetricStateRule.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Notification
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    public class MetricStateRule
    {
        [XmlAttribute]
        public bool IsOK;
        [XmlAttribute]
        public bool IsWarning;
        [XmlAttribute]
        public bool IsCritical;
        [XmlAttribute]
        public bool IsInformational;

        private bool enabled;

        public MetricStateRule()
        {
            enabled = false;
            Initialize();
        }

        public bool Enabled
        {
            get { return enabled; }
            set
            {
                enabled = value;
                if (!enabled)
                {
                    Initialize();
                }
            }
        }

        private void Initialize()
        {
            IsOK = false;
            IsInformational = IsWarning = IsCritical = true;
        }
    }
}
