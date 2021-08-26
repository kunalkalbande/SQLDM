//------------------------------------------------------------------------------
// <copyright file="OrCondition.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Conditions
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
using System.Runtime.Serialization;

    /// <summary>
    /// Composite OR Condition.  Matches if any child condition matches.
    /// </summary>
    [Serializable]
    public class OrCondition : BaseCompositeCondition
    {
        #region fields

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:OrCondition"/> class.
        /// </summary>
        public OrCondition()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:OrCondition"/> class.
        /// </summary>
        /// <param name="conditions">The conditions.</param>
        public OrCondition(params ICondition[] conditions)
        {
            Add(conditions);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:OrCondition"/> class.
        /// </summary>
        /// <param name="conditions">The conditions.</param>
        public OrCondition(IEnumerable<ICondition> conditions)
        {
            Add(conditions);
        }

        protected OrCondition(SerializationInfo info, StreamingContext context)
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
            bool ret = false;
            
            foreach (ICondition condition in conditions)
            {
                if (condition.Matches(element))
                    ret = true;
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
