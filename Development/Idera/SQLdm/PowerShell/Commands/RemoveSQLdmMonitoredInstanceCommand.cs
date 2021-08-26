//------------------------------------------------------------------------------
// <copyright file="RemoveSQLdmMonitoredInstanceCommand.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.PowerShell.Commands
{
    using System.Management.Automation;
    using Objects;

    [Cmdlet("Remove", "SQLdmMonitoredInstance")]
    public class RemoveSQLdmMonitoredInstanceCommand : RemoveItemProviderCmdletBase
    {
        private const string SamplePathConst = "\\" + InstancesInfo.ContainerName + "\\INSTANCENAME";

        public RemoveSQLdmMonitoredInstanceCommand()
            : base(InstancesInfo.ContainerName, SamplePathConst)
        {
        }

        [Alias("InstanceName")]
        [Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true, ParameterSetName = "FullPath")]
        public override string[] Path
        {
            get { return base.Path; }
            set { base.Path = value; }
        }
    }
}
