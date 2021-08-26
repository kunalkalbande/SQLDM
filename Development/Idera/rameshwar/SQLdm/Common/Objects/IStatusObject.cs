//------------------------------------------------------------------------------
// <copyright file="IStatusObject.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Objects
{
    using System;

    /// <summary>
    /// This interface is implemented by any object that has an ObjectState
    /// </summary>
    public interface IStatusObject
    {
        #region properties

        /// <summary>
        /// Gets the status.
        /// </summary>
        /// <returns></returns>
        MonitoredState GetStatus();

        #endregion

    }
}
