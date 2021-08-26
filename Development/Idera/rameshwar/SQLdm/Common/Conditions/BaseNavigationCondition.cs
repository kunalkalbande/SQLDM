//------------------------------------------------------------------------------
// <copyright file="BaseNavigationCondition.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Conditions
{
    using System;
    using System.Xml;
using System.Runtime.Serialization;

    /// <summary>
    /// Enter a description for this class
    /// </summary>
    [Serializable]
    public abstract class BaseNavigationCondition : ICondition
    {
        #region fields

        private string path;
        private ICondition subCondition;

        #endregion

        #region constructors

        public BaseNavigationCondition(string path)
            : this(path, null)
        {
        }

        public BaseNavigationCondition(ICondition subCondition)
            : this(null, subCondition)
        {
        }

        public BaseNavigationCondition(string path, ICondition subCondition)
        {
            Path = path;
            SubCondition = subCondition;
        }

        protected BaseNavigationCondition(SerializationInfo info, StreamingContext context)
        {
            path = info.GetString("path");
            subCondition = (ICondition)info.GetValue("subcondition", typeof(ICondition));
        }

        #endregion

        #region properties

        protected string Path
        {
            get { return path; }
            set { path = value; }
        }

        public ICondition SubCondition
        {
            get { return subCondition; }
            private set { subCondition = value; }
        }

        #endregion

        #region events

        #endregion

        #region methods

        #endregion

        #region interface implementations

        #region ICondition Members

        /// <summary>
        /// Matcheses the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public bool Matches(XmlElement element)
        {
            XmlNodeList childNodes = element.SelectNodes(Path);

            // No matching nodes
            if (childNodes == null)
                return false;

            // Matching nodes and no sub-condition
            if (SubCondition == null)
            {
                element.SetAttribute("passed", "true");
                return true;
            }
                

            bool ret = false;
            
            // Matching nodes and a sub-condition
            foreach (XmlNode childNode in childNodes)
            {
                XmlElement childElement = childNode as XmlElement;
                if (childElement == null)
                    continue;

                if (SubCondition.Matches(childElement))
                {
                    ret = true;
                }
            }
            
            return ret;
        }

        #endregion

        #endregion

        #region nested types

        #endregion

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("path", path);
            info.AddValue("subcondition", subCondition);
        }
    }
}
