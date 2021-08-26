//------------------------------------------------------------------------------
// <copyright file="TestSQLdmPermissionCommand.cs" company="Idera, Inc.">
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
    using System.Reflection;
    using Microsoft.PowerShell.Commands;
    using Objects;

    // [Cmdlet("Test", "SQLdmPermission", DefaultParameterSetName="FullPath")]
    public class TestSQLdmPermissionCommand : ItemProviderCmdletBase // CoreCommandBase, IDynamicParameters
    {
        private string contextPath = AppSecurityInfo.ContainerName;
        private string newType = "Permission";

        public TestSQLdmPermissionCommand() : base("/AppSecurity/UUUU/XXX")
        {
            
        }

        [Alias("PSPath","User")]
        [Parameter(Mandatory=true,Position=1,ValueFromPipelineByPropertyName=true,ParameterSetName="Path")]
        [Parameter(Mandatory = true, Position = 1, ValueFromPipelineByPropertyName = true, ParameterSetName = "FullPath")]
        public string[] Path
        {
            get { return paths; }
            set
            {
                paths = value;
                if (!String.IsNullOrEmpty(contextPath))
                {
                    // force relative paths to start at the root
                    for (int i = 0; i < paths.Length; i++)
                    {
                        string path = value[i];
                        // is this a relative path
                        if (!path.StartsWith(SQLdmProvider.pathSeparator))
                        {        
                            // does path already have the correct context
                            if (!path.StartsWith(contextPath, StringComparison.CurrentCultureIgnoreCase))
                            {
                                paths[i] = SQLdmProvider.pathSeparator + contextPath + SQLdmProvider.pathSeparator + SQLdmProvider.EscapeString(value[i]);
                            }
                        }
                    }
                }
                paths = value;
            }
        }
        private string[] paths;

        [Alias("InstanceName","Name")]
        [Parameter(Mandatory = true, Position = 2, ValueFromPipelineByPropertyName = true, ParameterSetName="Path")]
        public string[] Instance
        {
            get { return instances; }
            set { instances = value; }
        }
        private string[] instances;

        protected override void ProcessRecord() 
        {
            switch (ParameterSetName)
            {
                case "FullPath":
                    foreach (string path in paths)
                    {
                        NewItem(path, null, newType, null);
                    }
                    break;
                case "Path":
                    foreach (string path in paths)
                    {
                        foreach (string instance in instances)
                        {
                            NewItem(path, instance, newType, null);
                        }
                    }
                    break;
            }
        }

    }
}
