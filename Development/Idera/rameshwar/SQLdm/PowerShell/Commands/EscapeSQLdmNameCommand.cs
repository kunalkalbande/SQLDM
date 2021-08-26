//------------------------------------------------------------------------------
// <copyright file="EscapeSQLdmNameCommand.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.PowerShell.Commands
{
    using System.Collections.Generic;
    using System.Management.Automation;

    [Cmdlet("Escape", "SQLdmName")]
    public class EscapeSQLdmNameCommand : Cmdlet
    {
        private bool undo;
        private string[] path;

        [Parameter(Mandatory=true, ValueFromPipeline=true, ValueFromPipelineByPropertyName=true)]
        public string[] Name
        {
            get { return path; }
            set { path = value; }
        }

        [Parameter()]
        public SwitchParameter Undo
        {
            get { return undo; }
            set { undo = value; }
        }

        protected override void ProcessRecord()
        {
            List<string> result = new List<string>();
            foreach (string n in Name)
            {
                if (undo)
                    result.Add(SQLdmProvider.UnescapeString(n));
                else
                    result.Add(SQLdmProvider.EscapeString(n));
            }
            WriteObject(result.ToArray(), false);
        }
    }
}