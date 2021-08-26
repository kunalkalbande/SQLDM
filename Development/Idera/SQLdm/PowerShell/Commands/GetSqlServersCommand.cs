//------------------------------------------------------------------------------
// <copyright file="GetSqlServersCommand.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.PowerShell.Commands
{
    using System;
    using System.Data;
    using System.Management.Automation;
    using Microsoft.SqlServer.Management.Smo;
    using Objects;

    [Cmdlet("Get","SqlServers", DefaultParameterSetName="Local")]
    public class GetSqlServersCommand : PSCmdlet
    {
//        [ParameterAttribute(Mandatory=false,ParameterSetName="Machine")]
//        public string Machine
//        {
//            get { return machineName;  }
//            set { machineName = value; }
//        }
//        private string machineName;

        [ParameterAttribute(Mandatory=false,ParameterSetName="Local")]
        public SwitchParameter LocalOnly
        {
            get { return localOnly;  }
            set { localOnly = value; }
        }
        private bool localOnly = false;

        protected override void BeginProcessing()
        {
            DataTable result = null;
            
            switch (this.ParameterSetName)
            {
//                case "Machine":
//                    if (String.IsNullOrEmpty(machineName))
//                        result = SmoApplication.EnumAvailableSqlServers();
//                    else
//                        result = SmoApplication.EnumAvailableSqlServers(machineName);
//                    break;
                case "Local":
                    result = SmoApplication.EnumAvailableSqlServers(localOnly);
                    break;
                default:
                    result = SmoApplication.EnumAvailableSqlServers();
                    break;
            }

            foreach (DataRow instance in result.Rows)
            {
                SqlServerInstanceInfo info = new SqlServerInstanceInfo(instance);
                WriteObject(info);
            }
        }
    }
}
