//------------------------------------------------------------------------------
// <copyright file="NotCondition.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Conditions
{
    using System;
    using System.Xml;
using System.Runtime.Serialization;

    /// <summary>
    /// Composite NOT Condition.  Matches if no child conditions match.
    /// </summary>
    [Serializable]
    public class NotCondition : ICondition
    {
        #region fields

        private ICondition condition;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:NotCondition"/> class.
        /// </summary>
        /// <param name="condition">The condition.</param>
        public NotCondition(ICondition condition)
        {
            this.condition = condition;
        }

        private NotCondition(SerializationInfo info, StreamingContext context)
        {
            condition = (ICondition)info.GetValue("condition", typeof(ICondition));
        }

        #endregion

        #region properties

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
            if(!condition.Matches(element))
            {
                element.SetAttribute("passed", "true");
                return true;
            }
            
            return false;
        }

        #endregion

        #endregion

        #region nested types

        #endregion

        public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            info.AddValue("condition", condition);
        }
    }
}
