//------------------------------------------------------------------------------
// <copyright file="SetSQLdmQuietTimeCommand.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.PowerShell.Commands
{
    using System.Management.Automation;
    using Microsoft.PowerShell.Commands;

    // [Cmdlet(VerbsCommon.Set, "SQLdmQuietTime", SupportsShouldProcess = false)]
    public class SetSQLdmQuietTimeCommand : SetItemCommand
    {

        public SetSQLdmQuietTimeCommand()
        {
        }

        public new PSCredential Credential
        {
            get { return null; }
        }

    }
}
