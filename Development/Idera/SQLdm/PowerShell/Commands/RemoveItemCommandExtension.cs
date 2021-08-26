//------------------------------------------------------------------------------
// <copyright file="RemoveItemCommandExtension.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace Idera.SQLdm.PowerShell.Commands
{
    using System.Management.Automation;
    using Microsoft.PowerShell.Commands;

    public class RemoveItemCommandExtension : RemoveItemCommand
    {
        public RemoveItemCommandExtension()
        {
            Recurse = true;
        }

        /// <summary>
        /// Redefine Recurse property to hide attributes in base class.
        /// </summary>
        public new bool Recurse
        {
            get { return base.Recurse; }
            set { base.Recurse = value; }
        }

        public new PSCredential Credential
        {
            get { return null; }
        }
    }
}
