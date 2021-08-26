//------------------------------------------------------------------------------
// <copyright file="SnapshotList.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Snapshots
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a generic collection of objects which, taken together, constitute 
    /// a single snapshot.  (For example: a list of AgentLogFile objects.)
    /// </summary>
    public sealed class SnapshotList <T> : Snapshot
    {
        #region fields

        private List<T> _listItems;

        #endregion

        #region constructors

        public SnapshotList()
        {
        }

        public SnapshotList(List<T> listItems)
        {
            _listItems = listItems;
        }

        #endregion

        #region properties

        public List<T> ListItems
        {
            get
            {
                return _listItems;
            }
            set
            {
                _listItems = value;
            }
        }
        #endregion

    }
}
