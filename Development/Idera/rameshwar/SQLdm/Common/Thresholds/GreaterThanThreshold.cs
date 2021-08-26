//------------------------------------------------------------------------------
// <copyright file="GreaterThanThreshold.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Thresholds
{
    using System;
    using System.Runtime.Serialization;
    
    /// <summary>
    /// Class that checks a value to see if it's greater than the specified threshold.
    /// </summary>
    [Serializable]
    public class GreaterThanThreshold : BaseThreshold
    {
        #region fields

        #endregion

        #region constructors

        /// <summary>
        /// Protected constructor for Hibernate
        /// </summary>
        internal GreaterThanThreshold()
        {
        }

        protected GreaterThanThreshold(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:GreaterThanThreshold"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        public GreaterThanThreshold(IComparable value, bool enabled)
            : base(value, enabled)
        {
        }

        #endregion

        #region properties

        #endregion

        #region events

        #endregion

        #region methods

        #region IThreshold Members

        /// <summary>
        /// Checks the specified value against the threshold
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>
        /// true if the value is in violation of this threshold
        /// </returns>
        public override bool IsInViolation(object target)
        {
            if (!(target is IComparable))
                throw new InvalidOperationException("value passed does not implement IComparable");

            return ((target as IComparable).CompareTo(Value) > 0);
        }

        #endregion

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion
    }
     
}
