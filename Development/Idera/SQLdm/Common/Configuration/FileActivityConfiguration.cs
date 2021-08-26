//------------------------------------------------------------------------------
// <copyright file="FileActivityConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------




namespace Idera.SQLdm.Common.Configuration
{
    using System;
    using Idera.SQLdm.Common.Snapshots;

    /// <summary>
    /// File Activity Configuration
    /// </summary>
    [Serializable]
    public class FileActivityConfiguration: OnDemandConfiguration

    {
        #region fields

        private FileActivitySnapshot previousValues;

        #endregion

        #region constructors

        public FileActivityConfiguration(int monitoredServerId, FileActivitySnapshot previousValues) : base(monitoredServerId)
        {
            this.previousValues = previousValues;
        }

        #endregion

        #region properties

        public FileActivitySnapshot PreviousValues
        {
            get { return previousValues; }
            set { previousValues = value; }
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
