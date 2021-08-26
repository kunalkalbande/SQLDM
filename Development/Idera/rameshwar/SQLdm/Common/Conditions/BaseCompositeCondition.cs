//------------------------------------------------------------------------------
// <copyright file="BaseCompositeCondition.cs" company="Idera, Inc.">
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
    /// Abstract base composite condition.
    /// </summary>
    [Serializable]
    public abstract class BaseCompositeCondition : ICondition
    {
        #region fields

        protected List<ICondition> conditions;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:BaseCompositeCondition"/> class.
        /// </summary>
        public BaseCompositeCondition()
        {
            conditions = new List<ICondition>();
        }

        protected BaseCompositeCondition(SerializationInfo info, StreamingContext context)
        {
            conditions = (List<ICondition>)info.GetValue("conditions", typeof(List<ICondition>));
        }

        #endregion

        #region properties

        #endregion

        #region events

        #endregion

        #region methods

        /// <summary>
        /// Adds the specified condition.
        /// </summary>
        /// <param name="condition">The condition.</param>
        public void Add(ICondition condition)
        {
            if (condition == null)
                throw new ArgumentNullException("condition");
            conditions.Add(condition);
        }

        /// <summary>
        /// Adds the specified conditions.
        /// </summary>
        /// <param name="conditions">The conditions.</param>
        public void Add(IEnumerable<ICondition> conditions)
        {
            foreach (ICondition condition in conditions)
            {
                if (condition == null)
                    throw new ArgumentNullException("condition");
            }
            this.conditions.AddRange(conditions);
        }

        /// <summary>
        /// Adds the specified conditions.
        /// </summary>
        /// <param name="conditions">The conditions.</param>
        public void Add(params ICondition[] conditions)
        {
            foreach (ICondition condition in conditions)
            {
                if (condition == null)
                    throw new ArgumentNullException("condition");
            }
            this.conditions.AddRange(conditions);
        }

        /// <summary>
        /// Removes the specified condition.
        /// </summary>
        /// <param name="condition">The condition.</param>
        public void Remove(ICondition condition)
        {
            conditions.Remove(condition);
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            conditions.Clear();
        }

        #endregion

        #region interface implementations

        #region ICondition Members

        /// <summary>
        /// Matcheses the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public abstract bool Matches(XmlElement element);

        #endregion

        #endregion

        #region nested types

        #endregion

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("conditions", conditions);
        }
    }
}
