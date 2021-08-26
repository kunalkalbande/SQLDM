//------------------------------------------------------------------------------
// <copyright file="CustomCounterCollectionSnapshot.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Snapshots
{
    using System;
    using System.Collections.Generic;
    using Idera.SQLdm.Common.Configuration;

    /// <summary>
    /// Enter a description for this class
    /// </summary>
    [Serializable]
    public class CustomCounterCollectionSnapshot : Snapshot
    {
        #region fields

        private Dictionary<int, CustomCounterSnapshot> customCounterList = new Dictionary<int, CustomCounterSnapshot>();

        #endregion

        #region constructors

        public CustomCounterCollectionSnapshot(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
        }

        public CustomCounterCollectionSnapshot()
            : base(string.Empty)
        {
        }

        #endregion

        #region properties

        public Dictionary<int, CustomCounterSnapshot> CustomCounterList
        {
            get { return customCounterList; }
            internal set { customCounterList = value; }
        }

        #endregion

        #region events

        #endregion

        #region methods

        /// <summary>
        /// Sets the error that caused collection to fail
        /// Only one exception is permitted as it is the one which initially caused the failure
        /// </summary>
        internal new void SetError(string message, Exception e)
        {
            if (Error == null)
            {
                if (e == null)
                {
                    Error = new Exception(message);
                }
                else
                {
                    Error = new Exception(String.Format(message, e.Message), e);
                }
            }

            foreach (CustomCounterSnapshot counter in customCounterList.Values)
            {
                counter.SetError(String.Format("An error occurred when collecting custom counters: {0}", message), e);
            }
        }

        internal void LoadDictionaryFromList(List<CustomCounterSnapshot> list)
        {
            customCounterList = new Dictionary<int, CustomCounterSnapshot>();

            foreach (CustomCounterSnapshot snapshot in list)
            {
                if (snapshot != null && snapshot.Definition != null &&
                    !customCounterList.ContainsKey(snapshot.Definition.MetricID))
                {
                    if (ProductVersion == null)
                    {
                        ProductEdition = snapshot.ProductEdition;
                        ProductVersion = snapshot.ProductVersion;
                        ServerStartupTime = snapshot.ServerStartupTime;
                        TimeStamp = snapshot.ServerStartupTime;
                        TimeStampLocal = snapshot.TimeStampLocal;
                    }
                    customCounterList.Add(snapshot.Definition.MetricID, (CustomCounterSnapshot)snapshot.Clone());
                }
            }
        }

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion
    }
}
