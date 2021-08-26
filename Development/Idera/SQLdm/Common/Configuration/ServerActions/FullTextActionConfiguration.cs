//------------------------------------------------------------------------------
// <copyright file="FullTextActionConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Configuration.ServerActions
{
    using System;

    /// <summary>
    /// Configuration object for full text catalog actions
    /// </summary>
    [Serializable]
    public class FullTextActionConfiguration: OnDemandConfiguration, IServerActionConfiguration
    {
        #region fields

        private string databaseName = null;
        private string catalogname = null;
        private FullTextAction action;

        #endregion

        #region constructors

        public FullTextActionConfiguration(int monitoredServerId, string databaseName, string catalogname, FullTextAction action)
            : base(monitoredServerId)
        {
            this.databaseName = databaseName;
            this.catalogname = catalogname;
            this.action = action;
        }
        #endregion

        #region properties

        public string DatabaseName
        {
            get { return databaseName; }
            set { databaseName = value; }
        }

        public string Catalogname
        {
            get { return catalogname; }
            set { catalogname = value; }
        }

        public FullTextAction Action
        {
            get { return action; }
            set { action = value; }
        }

        #endregion

        #region events

        #endregion

        #region methods

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        /// <summary>
        /// Note: Optimize will only operate for SQL 2005
        /// </summary>
        public enum FullTextAction
        {
            Repopulate,
            Rebuild,
            Optimize
        }

        #endregion

    }
}
