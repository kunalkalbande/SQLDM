//------------------------------------------------------------------------------
// <copyright file="Deadlock.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Collections.Generic;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents a SQL Server deadlock
    /// </summary>
    [Serializable]
    public sealed class Deadlock
    {
        #region fields

        private string xdlString = "";
        private Dictionary<Int64,string> databaseNames = new Dictionary<long, string>();
        private Dictionary<Int64, string> objectNames = new Dictionary<long, string>();

        #endregion

        #region constructors

        #endregion

        #region properties



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
