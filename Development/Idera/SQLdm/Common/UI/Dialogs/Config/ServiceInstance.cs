//------------------------------------------------------------------------------
// <copyright file="ServiceInstance.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace Idera.SQLdm.Common.UI.Dialogs.Config
{
    public class ServiceInstance
    {
        private string machineName;
        private string instanceName;
        private int servicePort;
        public ServiceInstance(string machineName, string instanceName, int servicePort)
        {
            this.machineName = machineName;
            this.instanceName = instanceName != null ? instanceName : "<null>";
            this.servicePort = servicePort;
        }
        public string MachineName
        {
            get { return machineName; }
        }
        public string InstanceName
        {
            get { return instanceName; }
        }
        public int ServicePort
        {
            get { return servicePort; }
        }

        public string DisplayName
        {
            get
            {
                if (String.IsNullOrEmpty(InstanceName))
                    return ServicePort.ToString();

                return String.Format("{0} ({1})", this.InstanceName, this.ServicePort);
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is ServiceInstance)
            {
                ServiceInstance other = obj as ServiceInstance;
                return (this.ServicePort == other.ServicePort &&
                        this.machineName.Equals(other.MachineName) &&
                        this.instanceName.Equals(other.InstanceName));
            }
            return false;
        }

        public override string ToString()
        {
            return DisplayName;
//            return String.Format("{0}\\{1} ({2})", this.MachineName, this.InstanceName, this.ServicePort);
        }
    }
}
