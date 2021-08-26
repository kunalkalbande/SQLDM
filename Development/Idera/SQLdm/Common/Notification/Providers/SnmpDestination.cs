//------------------------------------------------------------------------------
// <copyright file="SnmpDestination.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Notification.Providers
{
    using System;

    /// <summary>
    /// Represents an Snmp destination (manager address, listening port, community name)
    /// </summary>
    [Serializable]
    public class SnmpDestination : NotificationDestinationInfo
    {
        #region constructors

        public SnmpDestination()
        {
        }

        public SnmpDestination(NotificationDestinationInfo destination)
        {
            this.Properties = destination.Properties;
        }
        
        #endregion

        #region methods


        //public override void Validate()
        //{

        //}

        public override string ToString()
        {
            return "";
        }

        #endregion

    }
}
