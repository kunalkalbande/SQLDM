//------------------------------------------------------------------------------
// <copyright file="ClientAuthorizationProvider.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Runtime.Remoting.Channels;
using BBS.TracerX;

namespace Idera.SQLdm.ManagementService.Configuration
{
    class ClientAuthorizationProvider : IAuthorizeRemotingConnection
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("ClientAuthorizationProvider");
 
        private static ClientAuthorizationProvider instance;

        public static ClientAuthorizationProvider Instance
        {
            get
            {
                if (instance == null)
                    instance = new ClientAuthorizationProvider();
                return instance;
            }
        }

        private ClientAuthorizationProvider()
        {
        }

        public bool IsConnectingEndPointAuthorized(System.Net.EndPoint endPoint)
        {
            LOG.InfoFormat("Connection from: {0} ", endPoint.ToString());
            return true;
        }

        public bool IsConnectingIdentityAuthorized(System.Security.Principal.IIdentity identity)
        {
            /*
            System.Security.Principal.WindowsIdentity wi = (System.Security.Principal.WindowsIdentity)identity;
            foreach (System.Security.Principal.IdentityReference reference in wi.Groups)
            { 
                Debug.Print("Group: " + reference.Value);
            }
            */
            LOG.InfoFormat("Connection from: {0}", identity.Name);
            return identity.IsAuthenticated;
        }
    }
}
