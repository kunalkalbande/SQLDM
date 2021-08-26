//------------------------------------------------------------------------------
// <copyright file="RevokeSQLdmPermissionCommand.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.PowerShell.Commands
{
    using System.Management.Automation;
    using Objects;

    [Cmdlet("Revoke","SQLdmPermission", DefaultParameterSetName="FullPath")]
    public class RevokeSQLdmPermissionCommand : RemoveItemProviderCmdletBase
    {
        private const string SamplePathConst = "\\" + AppSecurityInfo.ContainerName + "\\USERNAME\\INSTANCENAME";

        private string[] tags;

        public RevokeSQLdmPermissionCommand() : base(AppSecurityInfo.ContainerName, SamplePathConst)
        {
        }

        [Alias("Login","User")]
        [Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true, ParameterSetName = "Tags")]
        [Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true, ParameterSetName = "Path")]
        [Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true, ParameterSetName = "FullPath")]
        public override string[] Path
        {
            get { return base.Path; }
            set { base.Path = value; }
        }

        [Alias("InstanceName", "Instance")]
        [Parameter(Mandatory = true, Position = 1, ValueFromPipelineByPropertyName = true, ParameterSetName = "Path")]
        public override string[] Name
        {
            get { return base.Name; }
            set { base.Name = value; }
        }

        [Alias("Tags")]
        [Parameter(Mandatory = true, Position = 1, ParameterSetName = "Tags")]
        public string[] Tag
        {
            get { return tags; }
            set { tags = value; }
        }

        protected override void ProcessRecord()
        {
            if (ParameterSetName.Equals("Tags"))
            {
                foreach (string path in Path)
                {
                    foreach (string tagName in Tag)
                    {
                        RemoveItem(path + SQLdmProvider.pathSeparator + "#--" + tagName, true);
                    }
                }
            }                
            else
                base.ProcessRecord();
        }
    }
}
