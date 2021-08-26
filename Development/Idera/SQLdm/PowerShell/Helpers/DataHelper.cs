//------------------------------------------------------------------------------
// <copyright file="DataHelper.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
//SQLdm 10.1 (Srishti Purohit) File added for Remoting to management service
using Idera.SQLdm.Common.Services;
using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PowerShell.Helpers
{
    internal static class DataHelper
    {

        private static Uri managementServiceURL;

        public static Uri ManagementServiceURL
        {
            get { return managementServiceURL; }
            set { managementServiceURL = value; }
        }
        public static IManagementService ManagementService
        {
            get
            {
                if (ManagementServiceURL == null)
                {
                    throw new ApplicationException(
                        String.Format(
                            "Drive {0} is not connected to a management service.  Please recreate the drive to establish a connection to the SQLDM Repository."));
                }
                ServiceCallProxy proxy = new ServiceCallProxy(typeof(IManagementService), ManagementServiceURL.ToString());
                return proxy.GetTransparentProxy() as IManagementService;
            }
        }
    }
}
