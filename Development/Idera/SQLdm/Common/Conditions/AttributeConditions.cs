//------------------------------------------------------------------------------
// <copyright file="AttributeConditions.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Conditions
{
    using System;
    using System.Xml;
using System.Runtime.Serialization;

    /// <summary>
    /// Checks for the presence of an attribute
    /// </summary>
    [Serializable]
    public class AttributePresentCondition : ICondition
    {
        private string target;
        
        public AttributePresentCondition(string target)
        {
            this.target = target;
        }

        private AttributePresentCondition(SerializationInfo info, StreamingContext context)
        {
            target = info.GetString("target");
        }
        #region ICondition Members

        /// <summary>
        /// Matcheses the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public bool Matches(XmlElement element)
        {
            if (element.Attributes[target] != null)
            {
                element.SetAttribute("passed", "true");
                return true;
            }
            
            return false;
        }

        #endregion

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("target", target);
        }
    }
}
