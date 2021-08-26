//------------------------------------------------------------------------------
// <copyright file="ServerTagRule.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Notification
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using Wintellect.PowerCollections;

    [Serializable]
    public class ServerTagRule
    {   // we are storing the tag id not the tag name because tag names can be changed.
        private ArrayList serverTags;
        private bool enabled;

        public ServerTagRule()
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
                    serverTags = null;
            }
        }

        [XmlElement(ElementName = "ServerTag", Type = typeof(int))]
        public ArrayList ServerTagIds
        {
            get
            {
                if (serverTags == null)
                    serverTags = new ArrayList();
                return serverTags;
            }
            set
            {
                serverTags = value;
            }
        }
        
        /// <summary>
        /// Helper to get the tag id(s) in a typed list.
        /// </summary>
        /// <returns></returns>
        public IList<int> GetServerTags()
        {
            return Algorithms.TypedAs<int>(ServerTagIds);
        }

        public void Add(int tagId)
        {
            ArrayList ids = ServerTagIds;
            if (!ids.Contains(tagId))
            {
                ids.Add(tagId);
            }
        }
    }
}
