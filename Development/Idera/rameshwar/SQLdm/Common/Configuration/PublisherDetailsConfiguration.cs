//------------------------------------------------------------------------------
// <copyright file="PublisherQueueConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Configuration
{
    using System;
using Idera.SQLdm.Common.Snapshots;

    /// <summary>
    /// Configuration object for publisher details
    /// </summary>
    [Serializable]
    public class PublisherDetailsConfiguration : OnDemandConfiguration
    {
        #region fields

        private string _publisherDatabase;
        private PublisherDetails _publisherDetails;
        #endregion

        #region constructors

        public PublisherDetailsConfiguration(int monitoredServerId, string publisherDatabase)
            : base(monitoredServerId)
        {
            _publisherDatabase = publisherDatabase;
        }

        #endregion

        #region properties
        public string PublisherDatabase
        {
            get { return _publisherDatabase; }
            set { _publisherDatabase = value; }
        }

        public PublisherDetails PublisherDetails
        {
            get { return _publisherDetails; }
            set { _publisherDetails = value; }
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
