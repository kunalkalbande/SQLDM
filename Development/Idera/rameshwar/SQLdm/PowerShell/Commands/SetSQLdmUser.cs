//------------------------------------------------------------------------------
// <copyright file="SetSQLdmUser.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.PowerShell.Commands
{
    using System.Management.Automation;
    using Objects;

    [Cmdlet(VerbsCommon.Set, "SQLdmUser", SupportsShouldProcess=false, DefaultParameterSetName="FullPath")]
    public class SetSQLdmUserCommand : SetItemProviderCmdletBase
    {
        private const string SamplePathConst = "\\" + AppSecurityInfo.ContainerName + "\\USER";

        public SetSQLdmUserCommand()
            : base(AppSecurityInfo.ContainerName, SamplePathConst)
        {
        }

        [Alias("Login","User")]
        [Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true, ParameterSetName = "Path")]
        [Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true, ParameterSetName = "FullPath")]
        public override string[] Path
        {
            get { return base.Path;   }
            set { base.Path = value;  }
        }
    }
}
