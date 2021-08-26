//------------------------------------------------------------------------------
// <copyright file="SetItemCommandExtension.cs" company="Idera, Inc.">
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
    using Microsoft.PowerShell.Commands;

    public class SetItemCommandExtension : SetItemCommand
    {
        private string contextPath;

        public SetItemCommandExtension()
        {
        }

        public SetItemCommandExtension(string contextPath)
        {
            this.contextPath = contextPath;
        }

        [Alias("Name")]
        [Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true, ParameterSetName = "Path")]
        public new string[] Path
        {
            get { return base.Path; }
            set
            {
                if (!String.IsNullOrEmpty(contextPath))
                {
                    // force relative paths to start at the root
                    for (int i = 0; i < value.Length; i++)
                    {
                        string path = value[i];
                        // is this a relative path
                        if (!path.StartsWith(SQLdmProvider.pathSeparator))
                        {        
                            // does path already have the correct context
                            if (!path.StartsWith(contextPath, StringComparison.CurrentCultureIgnoreCase))
                            {
                                value[i] = SQLdmProvider.pathSeparator + contextPath + SQLdmProvider.pathSeparator + SQLdmProvider.EscapeString(value[i]);
                            }
                        }
                    }
                }
                base.Path = value;
            }
        }
    }
}
