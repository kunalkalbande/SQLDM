//------------------------------------------------------------------------------
// <copyright file="StaticContainerInfo.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace Idera.SQLdm.PowerShell.Objects
{
    public class StaticContainerInfo
    {
        public readonly string Name;
        public readonly SQLdmDriveInfo Drive;
        internal StaticContainerInfo(SQLdmDriveInfo drive, string name)
        {
            Drive = drive;
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
