//------------------------------------------------------------------------------
// <copyright file="RemoveSQLdmUserCommand.cs" company="Idera, Inc.">
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
    using Objects;

    [Cmdlet(VerbsCommon.Remove, "SQLdmUser", DefaultParameterSetName = "FullPath")]
    public class RemoveSQLdmUserCommand : RemoveItemProviderCmdletBase
    {
        private const string SamplePathConst = "\\" + AppSecurityInfo.ContainerName + "\\USER";

        public RemoveSQLdmUserCommand()
            : base(AppSecurityInfo.ContainerName, SamplePathConst)
        {
        }

        [Alias("Login","User")]
        [Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true, ParameterSetName = "FullPath")]
        public override string[] Path
        {
            get { return base.Path; }
            set { base.Path = value; }
        }
    }
}
