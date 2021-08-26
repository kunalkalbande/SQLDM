//------------------------------------------------------------------------------
// <copyright file="WaitStatsConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Runtime.Remoting.Messaging;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Attributes;

namespace Idera.SQLdm.Common.Configuration
{
    using System;
    using Idera.SQLdm.Common.VMware;

    /// <summary>
    /// Configuration object for vm configuration for a server
    /// </summary>
    [Serializable]
    public class VirtualizationConfiguration : IAuditable, ILogicalThreadAffinative
    {

        #region Fields

        private vCenterHosts vcInfo;
        private string instanceUUID;
        private string vmName;
        private string vmDomainName;
        private string vcServerType;

        #endregion

        #region Constructors

        public VirtualizationConfiguration(string uuid, string name, string domainname, vCenterHosts vc)
        {
            instanceUUID = uuid;
            vmName = name;
            vmDomainName = domainname;
            vcInfo = vc;
        }

        public VirtualizationConfiguration(string uuid, string name, string domainname, int hostid, string vcname, string vcurl, string user, string password, string vmType)
        {
            instanceUUID = uuid;
            vmName = name;
            vmDomainName = domainname;
            vcServerType = vmType;
            vcInfo = new vCenterHosts(hostid, vcname, vcurl, user, password, vmType);
        }
        #endregion

        #region Properties

        [Auditable(false)]
        public string InstanceUUID
        {
            get { return instanceUUID; }
        }

        [Auditable("VM Name")]
        public string VMName
        {
            get { return vmName; }
        }

        [Auditable("VM Domain Name")]
        public string VMDomainName
        {
            get { return vmDomainName; }
        }

        [Auditable("Virtualization Host Address")]
        public string VCAddress
        {
            get { return vcInfo.vcAddress; }
        }

        [Auditable("Virtualization Host User")]
        public string VCUser
        {
            get { return vcInfo.vcUser; }
        }

        [AuditableAttribute(true, true)]
        public string VCPassword
        {
            get { return vcInfo.vcPassword; }
        }

        [Auditable(false)]
        public int VCHostID
        {
            get { return vcInfo.vcHostID; }
        }

         [Auditable("Virtualization Host Name")]
        public string VCName
        {
            get { return vcInfo.vcName; }
        }

         public string VCServerType
         {
             get { return vcInfo.ServerType; }
         }
        #endregion

        //public bool IsEqual(VirtualizationConfiguration config)
        //{
        //    if (config == null) return false;

        //    return    this.VCHostID.Equals(config.VCHostID)
        //           && this.VCName.Equals(config.VCName)
        //           && this.VCAddress.Equals(config.VCAddress)
        //           && this.VCUser.Equals(config.VCUser)
        //           && this.VCPassword.Equals(config.VCPassword)
        //           && this.InstanceUUID.Equals(config.InstanceUUID)
        //           && this.VMName.Equals(config.VMName)
        //           && this.VMDomainName.Equals(config.VMDomainName);
        //}

        /// <summary>
        /// Get an Auditable Entity from Virtualization Configuration
        /// </summary>
        /// <returns> Returns an Auditable Entity</returns>
        public AuditableEntity GetAuditableEntity()
        {
            AuditableEntity auditable = new AuditableEntity();
            auditable.Name = this.VMName;
            auditable.AddMetadataProperty("Virtualization Host Address", this.VCAddress);
            auditable.AddMetadataProperty("Virtualization Host Name", this.VCName);
            auditable.AddMetadataProperty("Virtualization Host User", this.VCUser);
            auditable.AddMetadataProperty("VM Name", this.VMName);
            if (!string.IsNullOrEmpty(vmDomainName))
            {
                auditable.AddMetadataProperty("VM Domain Name", this.VMDomainName);
            }

            return auditable;
        }


        /// <summary>
        /// Get an Auditable Entity from Virtualization Configuration
        /// </summary>
        /// <param name="oldValue">IAuditable</param>
        /// <returns> Returns an Auditable Entity</returns>
        public AuditableEntity GetAuditableEntity(IAuditable oldValue)
        {
            AuditableEntity auditable = new AuditableEntity();
            auditable.Name = this.VMName;

            PropertiesComparer comparer = new PropertiesComparer();
            var propertiesChanged = comparer.GetNewProperties(oldValue, this);

            foreach (var property in propertiesChanged)
            {
                auditable.AddMetadataProperty(property.Name, property.Value);
            }

            return auditable;
        }
    }
    public static class VirtualizationConfigurationExtensions
    {
        public static bool IsEqual(this VirtualizationConfiguration left, VirtualizationConfiguration right)
        {
            if (right == null) return (left == null);
            if (left == null) return false;

            return    left.VCHostID.Equals(right.VCHostID)
                   && left.VCName.Equals(right.VCName)
                   && left.VCAddress.Equals(right.VCAddress)
                   && left.VCUser.Equals(right.VCUser)
                   && left.VCPassword.Equals(right.VCPassword)
                   && left.InstanceUUID.Equals(right.InstanceUUID)
                   && left.VMName.Equals(right.VMName)
                   && left.VMDomainName.Equals(right.VMDomainName);
        }
    }
}
