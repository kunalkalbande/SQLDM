//------------------------------------------------------------------------------
// <copyright file="SetSQLdmGeneralConfigurationCommand.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.PowerShell.Commands
{
    using System.Management.Automation;
    using Microsoft.PowerShell.Commands;

    // [Cmdlet(VerbsCommon.Set, "SQLdmGeneralConfiguration", SupportsShouldProcess = false)]
    public class SetSQLdmGeneralConfigurationCommand : SetItemCommand
    {

        public SetSQLdmGeneralConfigurationCommand()
        {
        }
        public new PSCredential Credential
        {
            get { return null; }
        }
    }
}
