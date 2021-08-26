//------------------------------------------------------------------------------
// <copyright file="BackupOperation.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents a backup operation.
    /// </summary>
    public class BackupOperation : RecoveryOperation
    {
        #region fields

        #endregion

        #region constructors

        //internal BackupOperation(
        //    string deviceName,
        //    string filegroupName,
        //    string performedBy,
        //    decimal size,
        //    string tableIndexName,
        //    DateTime timestamp,
        //    RecoveryOperationType type)
        //{
        //    SetDeviceName(deviceName);
        //    SetFilegroupName(filegroupName);
        //    SetPerformedBy(performedBy);
        //    SetSize(size);
        //    SetTableIndexName(tableIndexName);
        //    SetTimestamp(timestamp);
        //    SetType(type);
        //}

        #endregion

        #region properties

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
        //    dump.Append("\tSize: " + Size); dump.Append("\n");
        //    dump.Append("\tTableIndexName: " + TableIndexName); dump.Append("\n");
        //    dump.Append("\tTimestamp: " + Timestamp); dump.Append("\n");
        //    dump.Append("\tType: " + Type); dump.Append("\n");

        //    return dump.ToString();
        //}

        #endregion
    }
}