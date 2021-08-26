//------------------------------------------------------------------------------
// <copyright file="PathType.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace Idera.SQLdm.PowerShell.Objects
{
    public enum PathType
    {
        Root,
        Instances,
        Instance,
        Alerts,
        Alert,
        CustomCounters,
        CustomCounter,
        AppSecurity,
        AppUser,
        AppUserPermission,
        Invalid
    };
}
