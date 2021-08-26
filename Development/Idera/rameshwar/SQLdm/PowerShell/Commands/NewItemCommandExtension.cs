//------------------------------------------------------------------------------
// <copyright file="NewItemCommandExtension.cs" company="Idera, Inc.">
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

    public class NewItemCommandExtension : NewItemCommand
    {
        public NewItemCommandExtension(string path, string itemType)
        {
            ItemType = itemType;
            Path = new string[] { path };
        }

        public new string ItemType
        {
            get { return base.ItemType;  }
            set { base.ItemType = value; }
        }

        public new string[] Path
        {
            get { return base.Path;  }
            set { base.Path = value; }
        }

        [Alias("InstanceName")]
        [Parameter(Mandatory=true,Position=0,ValueFromPipelineByPropertyName=true,ParameterSetName="NameOnly")]
        public new string Name
        {
            get { return base.Name;  }
            set { base.Name = SQLdmProvider.EscapeString(value); }
        }

        public new object Value
        {
            get { return base.Value;  }
            set { base.Value = value; }
        }

        public new bool Force
        {
            get { return base.Force;  }
            set { base.Force = value; }
        }

        public new PSCredential Credential
        {
            get { return base.Credential;  }
            set { base.Credential = value; }
        }

    }
}
