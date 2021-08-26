//------------------------------------------------------------------------------
// <copyright file="NewSQLdmMonitoredInstance.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.PowerShell.Commands
{
    using System.Management.Automation;
    using Objects;

    [Cmdlet("New", "SQLdmMonitoredInstance", DefaultParameterSetName = "FullPath")]
    public class NewSQLdmMonitoredInstanceCommand : ItemProviderPathAndCredentialsCmdletBase
    {
        internal const string ItemTypeConst = "Instance";
        private const string SamplePathConst = "\\" + InstancesInfo.ContainerName + "\\INSTANCENAME";

        public NewSQLdmMonitoredInstanceCommand() : base(InstancesInfo.ContainerName, SamplePathConst)
        {
        }

        [Alias("InstanceName","Name")]
        [Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true, ParameterSetName = "FullPath")]
        public override string[] Path
        {
            get { return base.Path;   }
            set { base.Path = value;  }
        }

        protected override void ProcessRecord()
        {
            foreach (string path in Path)
            {
                NewItem(path, null, ItemTypeConst, null);
            }
        }

    }
}
