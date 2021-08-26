//------------------------------------------------------------------------------
// <copyright file="Enumerations.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace Idera.SQLdm.PowerShell.Objects
{
    [Flags]
    public enum Days : short
    {
        None = 0,
        Sunday = 1,
        Monday = 4,
        Tuesday = 8,
        Wednesday = 16,
        Thursday = 32,
        Friday = 64,
        Saturday = 128
    }
}
