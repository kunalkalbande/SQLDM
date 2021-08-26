//------------------------------------------------------------------------------
// <copyright file="SetSQLdmMaintenanceMode.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.PowerShell.Commands
{
    using System.Management.Automation;
    using Microsoft.PowerShell.Commands;

    // [Cmdlet(VerbsCommon.Set, "SQLdmMaintenanceMode", SupportsShouldProcess = false)]
    public class SetSQLdmMaintenanceModeCommand : SetItemCommand
    {

        public SetSQLdmMaintenanceModeCommand()
        {
        }

        public new PSCredential Credential
        {
            get { return null; }
        }

    }
}
