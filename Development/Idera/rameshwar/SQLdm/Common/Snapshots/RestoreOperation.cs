//------------------------------------------------------------------------------
// <copyright file="RestoreOperation.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents a restore operation.
    /// </summary>
    public class RestoreOperation : RecoveryOperation
    {
        #region fields

        private DateTime? _pointInTime = null;
        private bool? _replacedDatabase = null;

        #endregion

        #region constructors

        //internal RestoreOperation(
        //    string deviceName,
        //    string filegroupName,
        //    string performedBy,
        //    DateTime pointInTime,
        //    bool replacedDatabase,
        //    decimal size,
        //    string tableIndexName,
        //    DateTime timestamp,
        //    RecoveryOperationType type)
        //{
        //    SetDeviceName(deviceName);
        //    SetFilegroupName(filegroupName);
        //    SetPerformedBy(performedBy);
        //    _pointInTime = pointInTime;
        //    _replacedDatabase = replacedDatabase;
        //    SetSize(size);
        //    SetTableIndexName(tableIndexName);
        //    SetTimestamp(timestamp);
        //    SetType(type);
        //}

        #endregion

        #region properties

        /// <summary>
        /// The end point for a point in time restore.
        /// </summary>
        public DateTime? PointInTime
        {
            get { return _pointInTime; }
        }

        /// <summary>
        /// Boolean representing whether the database was replaced.
        /// </summary>
        public bool? ReplacedDatabase
        {
            get { return _replacedDatabase; }
        }

        #endregion

        #region methods

        ///// <summary>
        ///// Dumps sample data to a string.
        ///// </summary>
        ///// <returns>Sample data.</returns>
        //public override string Dump()
        //{
        //    StringBuilder dump = new StringBuilder();

        //    dump.Append("\tDeviceName: " + DeviceName); dump.Append("\n");
        //    dump.Append("\tFilegroupName: " + FilegroupName); dump.Append("\n");
        //    dump.Append("\tPerformedBy: " + PerformedBy); dump.Append("\n");
        //    dump.Append("\tPointInTime: " + PointInTime); dump.Append("\n");
        //    dump.Append("\tReplacedDatabase: " + ReplacedDatabase); dump.Append("\n");
        //    dump.Append("\tSize: " + Size); dump.Append("\n");
        //    dump.Append("\tTableIndexName: " + TableIndexName); dump.Append("\n");
        //    dump.Append("\tTimestamp: " + Timestamp); dump.Append("\n");
        //    dump.Append("\tType: " + Type); dump.Append("\n");

        //    return dump.ToString();
        //}

        #endregion
    }
}
