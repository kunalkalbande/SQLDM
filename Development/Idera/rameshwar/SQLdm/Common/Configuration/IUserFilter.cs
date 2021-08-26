//------------------------------------------------------------------------------
// <copyright file="IUserFilter.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Configuration
{
    using System;

    /// <summary>
    /// This interface is implemented by any object that allows user filtering via the Properties Dialog
    /// </summary>
    public interface IUserFilter
    {
        #region properties

        /// <summary>
        /// Determine if the default values are currently being used
        /// </summary>
        /// <returns>true if all properties are set to the default values, otherwise false</returns>
        bool HasDefaultValues();

        /// <summary>
        /// Determine if there is any filtering currently applied
        /// </summary>
        /// <returns>true if any property restricts data, otherwise false</returns>
        bool IsFiltered();

        #endregion

        #region methods

        /// <summary>
        /// Set all properties to their unfiltered state
        /// </summary>
        void ClearValues();

        /// <summary>
        /// Reset all properties to their default values
        /// </summary>
        void ResetValues();

        /// <summary>
        /// Update the filter properties to match the values in the passed selection object
        /// </summary>
        void UpdateValues(IUserFilter filter);

        /// <summary>
        /// Validate that the current combination of property selections make sense
        /// </summary>
        /// <returns>true if the selections can return data, otherwise false and an information Message is returned with the problem</returns>
        bool Validate(out string Message);

        #endregion
    }
}
