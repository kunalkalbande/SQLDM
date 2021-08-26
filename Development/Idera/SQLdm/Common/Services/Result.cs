//------------------------------------------------------------------------------
// <copyright file="RequestResult.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Services
{
    using System;

    /// <summary>
    /// Result enumeration for remoted calls
    /// </summary>
    public enum Result
    {
        Failure,
        Pending,
        Success,
        Unsupported,
        Partial,
        PermissionViolation // Added for Permission Violation to allow skipping and moving to next collector
    }
}
