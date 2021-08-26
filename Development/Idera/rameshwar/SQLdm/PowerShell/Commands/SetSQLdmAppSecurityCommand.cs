//------------------------------------------------------------------------------
// <copyright file="SetSQLdmAppSecurity.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.PowerShell.Commands
{
    using System.Management.Automation;
    using Objects;

    [Cmdlet("Set", "SQLdmAppSecurity", DefaultParameterSetName="FullPath")]
    public class SetSQLdmAppSecurityCommand : SetItemProviderCmdletBase
    {
        private const string SamplePathConst = "\\" + AppSecurityInfo.ContainerName;

        public SetSQLdmAppSecurityCommand()
            : base(AppSecurityInfo.ContainerName, SamplePathConst)
        {
        }

        [Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true, ParameterSetName = "FullPath")]
        public override string[] Path
        {
            get { return base.Path;   }
            set { base.Path = value;  }
        }

    }
}
