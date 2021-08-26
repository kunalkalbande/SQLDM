//------------------------------------------------------------------------------
// <copyright file="PublisherQueueConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Configuration
{
    using System;

    /// <summary>
    /// Configuration object for publisher queue
    /// </summary>
    [Serializable]
    public class PublisherQueueConfiguration : OnDemandConfiguration
    {
        #region fields

        private int numberOfRecords = 200;

        #endregion

        #region constructors

        public PublisherQueueConfiguration(int monitoredServerId, int numberOfRecords) : base(monitoredServerId)
        {
            this.numberOfRecords = numberOfRecords;
        }

        #endregion

        #region properties

        public int NumberOfRecords
        {
            get { return numberOfRecords; }
            set { numberOfRecords = value; }
        }

        #endregion

        #region events

        #endregion

        #region methods

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

    }
}
