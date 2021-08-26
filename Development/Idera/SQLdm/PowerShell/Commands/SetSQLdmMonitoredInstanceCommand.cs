//------------------------------------------------------------------------------
// <copyright file="SetSQLdmMonitoredInstanceCommand.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.PowerShell.Commands
{
    using System.Management.Automation;
    using Objects;

    [Cmdlet("Set","SQLdmMonitoredInstance", DefaultParameterSetName="FullPath")]
    public class SetSQLdmMonitoredInstanceCommand : ItemProviderPathAndCredentialsCmdletBase
    {
        private const string SamplePathConst = "\\" + InstancesInfo.ContainerName + "\\INSTANCENAME";

        public SetSQLdmMonitoredInstanceCommand() : base(InstancesInfo.ContainerName, SamplePathConst)
        {
        }

        [Alias("InstanceName")]
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
                SetItem(path, null);
            }
        }
    }
}
