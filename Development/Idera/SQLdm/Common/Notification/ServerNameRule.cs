//------------------------------------------------------------------------------
// <copyright file="ServerNameRule.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Notification
{
    using System;
    using System.Collections;
    using System.Xml.Serialization;

    [Serializable]
    public class ServerNameRule
    {
        private ArrayList serverNames;
        private bool enabled;

        public ServerNameRule()
        {
            Enabled = false;
        }

        public bool Enabled
        {
            get { return enabled; }
            set
            {
                enabled = value;
                if (!enabled)
                    serverNames = null;
            }
        }

        [XmlElement(ElementName = "ServerName", Type = typeof(string))]
        public ArrayList ServerNames
        {
            get
            {
                if (serverNames == null)
                    serverNames = new ArrayList();
                return serverNames;
            }
            set
            {
                serverNames = value;
            }
        }

        public void Add(string serverName)
        {
            ArrayList names = ServerNames;
            if (!names.Contains(serverName))
            {
                names.Add(serverName);
            }
        }
    }
}
