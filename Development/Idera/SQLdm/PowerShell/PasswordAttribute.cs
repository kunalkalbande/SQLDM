//------------------------------------------------------------------------------
// <copyright file="PasswordAttribute.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace Idera.SQLdm.PowerShell
{
    using System.Management.Automation;
    using System.Management.Automation.Host;

    public class PasswordAttribute : ArgumentTransformationAttribute
    {

        public PasswordAttribute()
        {
        }




        public override object Transform(EngineIntrinsics engineIntrinsics, object inputData)
        {
            PSHost host = engineIntrinsics.Host;
            if (host == null)
                return inputData;
            PSHostUserInterface ui = host.UI;
            if (ui == null)
                return inputData;

            return inputData;
        }
    }
}
