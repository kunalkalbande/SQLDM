//------------------------------------------------------------------------------
// <copyright file="SQLdmPermissionInfo.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace Idera.SQLdm.PowerShell.Objects
{
    using Idera.SQLdm.Common.Objects.ApplicationSecurity;

    public class SQLdmPermissionInfo : ICloneable
    {
        private readonly PermissionDefinition permission;
        private readonly Server server;

        public SQLdmPermissionInfo(PermissionDefinition permission, Server server)
        {
            this.permission = permission;
            this.server = server;
        }

        public String InstanceName
        {
            get { return server.InstanceName; }
        }
        public PermissionType Permission
        {
            get { return permission.PermissionType; }
        }

        public override string ToString()
        {
            return server.InstanceName;
        }

        public object  Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
