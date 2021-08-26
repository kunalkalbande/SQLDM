//------------------------------------------------------------------------------
// <copyright file="GrantSQLdmPermissionCommand.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.PowerShell.Commands
{
    using System.Management.Automation;
    using Objects;

    [Cmdlet("Grant","SQLdmPermission", DefaultParameterSetName="FullPath")]
    public class GrantSQLdmPermissionCommand : NewItemProviderCmdletBase
    {
        internal const string ItemTypeConst = "Permission";
        private const string SamplePathConst = "\\" + AppSecurityInfo.ContainerName + "\\USERNAME\\INSTANCENAME";

        private string[] tags;

        public GrantSQLdmPermissionCommand() : base(AppSecurityInfo.ContainerName, "Permission", SamplePathConst)
        {
        }

        [Alias("User","Login")]
        [Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true, ParameterSetName = "Tags")]
        [Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true, ParameterSetName = "Path")]
        [Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true, ParameterSetName = "FullPath")]
        public override string[] Path
        {
            get { return base.Path; }
            set { base.Path = value; }
        }

        [Alias("InstanceName","Instance")]
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

        protected override string NewItemType
        {
            get
            {
                if (tags != null)
                    return ItemTypeConst + "ViaTags";

                return base.NewItemType;
            }
        }

        protected override void ProcessRecord()
        {
            if (ParameterSetName.Equals("Tags"))
            {
                foreach (string path in Path)
                {
                    foreach (string tagName in Tag)
                    {
                        NewItem(path, tagName, NewItemType, null);
                    }
                }
            }                
            else
                base.ProcessRecord();
        }
    }
}
