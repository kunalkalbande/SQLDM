//------------------------------------------------------------------------------
// <copyright file="AndCondition.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Conditions
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using System.Xml;
using System.Runtime.Serialization;

    /// <summary>
    /// Composite AND Condition.  Matches if all child conditions match.
    /// </summary>
    [Serializable]
    public class AndCondition : BaseCompositeCondition
    {
        #region fields

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AndCondition"/> class.
        /// </summary>
        public AndCondition()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AndCondition"/> class.
        /// </summary>
        /// <param name="conditions">The conditions.</param>
        public AndCondition(params ICondition[] conditions)
        {
            Add(conditions);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AndCondition"/> class.
        /// </summary>
        /// <param name="conditions">The conditions.</param>
        public AndCondition(IEnumerable<ICondition> conditions)
        {
            Add(conditions);
        }

        protected AndCondition(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion

        #region properties

        #endregion

        #region events

        #endregion

        #region methods

        /// <summary>
        /// Matcheses the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public override bool Matches(XmlElement element)
        {
            bool ret = true;
            
            foreach (ICondition condition in conditions)
            {
                if (!condition.Matches(element))
                    ret = false;
            }

            return ret;
        }

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion
    }
}
