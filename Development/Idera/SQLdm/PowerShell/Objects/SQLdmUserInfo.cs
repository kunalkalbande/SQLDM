//------------------------------------------------------------------------------
// <copyright file="SQLdmUserInfo.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace Idera.SQLdm.PowerShell.Objects
{
    using System.Collections.Generic;
    using Idera.SQLdm.Common.Objects.ApplicationSecurity;
    using Wintellect.PowerCollections;

    public class SQLdmUserInfo : ICloneable, IEqualityComparer<SQLdmPermissionInfo>
    {
        private string m_Login;
        private LoginType m_LoginType;
        private string password;

        private bool m_System;
        private bool m_IsAdmin;
        private string m_Comment;
        private bool m_WebAppPermission;
        
        private Set<SQLdmPermissionInfo> m_Servers;

        private List<int> m_PendingServers;

        internal SQLdmUserInfo()
        {
            this.m_Servers = new Set<SQLdmPermissionInfo>(this);
        }

        internal SQLdmUserInfo(PermissionDefinition permission) : this()
        {
            this.m_Login = permission.Login;
            this.m_LoginType = permission.LoginType;
            this.m_System = permission.System;
            this.m_Comment = permission.Comment;
            this.m_IsAdmin = permission.PermissionType == PermissionType.Administrator;
            this.m_WebAppPermission = permission.WebAppPermission;

            AddPermissions(permission);
        }

        internal void AddPermissions(PermissionDefinition permission)
        {
            if (!permission.Enabled)
                return;

            PermissionType rights = permission.PermissionType;

            if (rights == PermissionType.Administrator)
            {
                m_Comment = permission.Comment;
                m_IsAdmin = true;
            }

            if (m_IsAdmin)
                m_Servers.Clear();
            else
            {
                // add servers permissions
                foreach (Server server in permission.GetServerList())
                {
                    SQLdmPermissionInfo grant = new SQLdmPermissionInfo(permission, server);
                    m_Servers.Add(grant);
                }
            }
        }

        public string Login
        {
            get { return m_Login; }
            internal set { m_Login = value; }
        }
        public LoginType LoginType
        {
            get { return m_LoginType; }
            internal set { m_LoginType = value; }
        }
         public bool IsSystem
        {
            get { return m_System; }
            internal set { m_System = value; }
        }
        public bool IsAdministrator
        {
            get { return m_IsAdmin; }
            internal set { m_IsAdmin = value; }
        }
        public string Comments
        {
            get { return m_Comment ?? String.Empty; }
            internal set { m_Comment = value; }
        }

        internal string Password
        {
            get { return password;  }
            set { password = value; }
        }

        public bool WebAppPermission
        {
            get { return m_WebAppPermission; }
            internal set { m_WebAppPermission = value; }
        }

        public SQLdmPermissionInfo[] Permissions
        {
            get
            {
                return Algorithms.Sort<SQLdmPermissionInfo>(m_Servers, PermissionSortComparison);
            }
        }

        private static int PermissionSortComparison(SQLdmPermissionInfo left, SQLdmPermissionInfo right)
        {   // compare only by using name
            return left.InstanceName.CompareTo(right.InstanceName);
        }

        public override string ToString()
        {
            return Login;
        }

/*    
 * 
        internal int[] PendingServers
        {
            get 
            {  
                if (m_PendingServers != null)
                    return m_PendingServers.ToArray();
                List<int> result = new List<int>();
                foreach (Server server in m_Servers)
                {
                    result.Add(server.SQLServerID);
                }
                return result.ToArray();
            }
            set
            {
                if (m_PendingServers == null)
                    m_PendingServers = new List<int>();
                else
                    m_PendingServers.Clear();
                m_PendingServers.AddRange(value);
            }
        }
  */

        public object Clone()
        {
            SQLdmUserInfo user = (SQLdmUserInfo) this.MemberwiseClone();
            user.m_Servers.Clear();

            foreach (SQLdmPermissionInfo pi in this.m_Servers)
            {
                user.m_Servers.Add((SQLdmPermissionInfo)pi.Clone());
            }

            return user;
        }

        #region IEqualityComparer<SQLdmPermissionInfo> Members

        public bool Equals(SQLdmPermissionInfo x, SQLdmPermissionInfo y)
        {
            return (x.InstanceName.Equals(y.InstanceName));
        }

        public int GetHashCode(SQLdmPermissionInfo obj)
        {
            return obj.InstanceName.GetHashCode();
        }

        #endregion
    }
}
