//------------------------------------------------------------------------------
// <copyright file="SetSQLdmQueryMonitorCommand.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.PowerShell.Commands
{
    using System.Management.Automation;
    using Microsoft.PowerShell.Commands;

    // [Cmdlet(VerbsCommon.Set, "SQLdmQueryMonitor", SupportsShouldProcess = false)]
    public class SetSQLdmQueryMonitorCommand : SetItemCommand
    {

        public SetSQLdmQueryMonitorCommand()
        {
        }
        public new PSCredential Credential
        {
            get { return null; }
        }
    }
}
