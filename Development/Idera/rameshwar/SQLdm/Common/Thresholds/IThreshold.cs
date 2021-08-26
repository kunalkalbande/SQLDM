//------------------------------------------------------------------------------
// <copyright file="IThreshold.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Thresholds
{
    using System;

    /// <summary>
    /// Interface that all thresholds must implement.
    /// </summary>
    public interface IThreshold
    {
        #region properties


        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        object Value { get; set; }

        /// <summary>
        /// Get or set if this threshold is enabled.
        /// </summary>
        bool Enabled { get; set; }

        #endregion

        #region events

        #endregion

        #region methods

        /// <summary>
        /// Checks the specified value against the threshold
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>
        /// true if the value is in violation of this threshold
        /// </returns>
        bool IsInViolation(object target);

        #endregion
    }
}
