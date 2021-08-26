//------------------------------------------------------------------------------
// <copyright file="PinnedTable.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;

namespace Idera.SQLdm.Common.Snapshots
{
    using System.Text;

    /// <summary>
    /// Represents a pinned table.
    /// </summary>
    [Serializable]
    public class PinnedTable
    {
        #region fields

        private string name;
        private int rowCount;
        private int usedSpace;

        #endregion

        #region constructors

        #endregion

        #region properties

        public string Name
        {
            get { return name; }
            internal set { name = value; }
        }

        public int RowCount
        {
            get { return rowCount; }
            internal set { rowCount = value; }
        }

        public int UsedSpace
        {
            get { return usedSpace; }
            internal set { usedSpace = value; }
        }

        #endregion

        #region methods

        #endregion
    }
}
