//------------------------------------------------------------------------------
// <copyright file="ICounter.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Counters
{
    using System;

    /// <summary>
    /// Enter a description for this interface
    /// </summary>
    public interface ICounter
    {
        #region properties

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; }
        
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        long Value { get; }

        #endregion

        #region events

        #endregion

        #region methods

        /// <summary>
        /// Increments this instance.
        /// </summary>
        /// <returns></returns>
        long Increment();
        
        /// <summary>
        /// Decrements this instance.
        /// </summary>
        /// <returns></returns>
        long Decrement();
        
        /// <summary>
        /// Resets this instance.
        /// </summary>
        void Reset();

        #endregion
    }
}
