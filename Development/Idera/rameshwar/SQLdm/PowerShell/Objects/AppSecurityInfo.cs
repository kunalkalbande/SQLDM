//------------------------------------------------------------------------------
// <copyright file="AppSecurityInfo.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Idera.SQLdm.PowerShell.Objects
{
    using Helpers;
    using Idera.SQLdm.Common.Objects.ApplicationSecurity;
    using Wintellect.PowerCollections;

    public class AppSecurityInfo : StaticContainerInfo
    {
        internal const string ContainerName = "AppSecurity";
        internal const string ContainerNameLower = "appsecurity";
       
        public AppSecurityInfo(SQLdmDriveInfo drive) : base(drive, ContainerName)
        {
        }

        public bool IsEnabled
        {
            get { return Drive.UserToken.IsSecurityEnabled; }
        }

        public IList<SQLdmUserInfo> Users
        {
            get
            {
                List<SQLdmUserInfo> users = new List<SQLdmUserInfo>();
                UserToken userToken = Drive.UserToken;
                if (userToken.IsSecurityEnabled && (userToken.IsSysadmin || userToken.IsSQLdmAdministrator))
                {
                    foreach (PermissionDefinition permission in Drive.GetAppSecurityConfiguration().Permissions.Values)
                    {
                        users.Add(new SQLdmUserInfo(permission));
                    }
                }

                return Algorithms.ReadOnly(users);
            }
        }

    }
}
