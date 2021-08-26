//------------------------------------------------------------------------------
// <copyright file="NewSQLdmUserCommand.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.PowerShell.Commands
{
    using System.Management.Automation;
    using Objects;

    [Cmdlet(VerbsCommon.New, "SQLdmUser", DefaultParameterSetName = "FullPath")]
    public class NewSQLdmUserCommand : NewItemProviderCmdletBase
    {
        private const string ItemTypeConst = "User";
        private const string SamplePathConst = "\\" + AppSecurityInfo.ContainerName + "\\UUUU";
        public NewSQLdmUserCommand() : base(AppSecurityInfo.ContainerName, ItemTypeConst, SamplePathConst)
        {
        }

        [Alias("Login", "User")]
        [Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true, ParameterSetName = "FullPath")]
        public override string[] Path
        {
            get { return base.Path; }
            set { base.Path = value; }
        }
    }
}
