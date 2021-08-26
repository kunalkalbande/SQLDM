//------------------------------------------------------------------------------
// <copyright file="WaitStatsSnapshot.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Data;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents Server-Wide Wait Statistics
    /// </summary>
    [Serializable]
    public class WaitStatisticsSnapshot: Snapshot
    {
        #region fields

        private Dictionary<string, Wait> waits = new Dictionary<string, Wait>();
        private DataTable waitsTable;
        private bool hasBeenCalculated = false;

        #endregion

        #region constructors

        internal WaitStatisticsSnapshot()
        {}
        internal WaitStatisticsSnapshot(SqlConnectionInfo info)
        : base(info.InstanceName)
        {
            //InitializeTable();
        }
        #endregion

        #region properties

        public DataTable WaitsTable
        {
            get
            {
                if (waitsTable == null)
                {
                    CreateWaitsTable();
                }
                return waitsTable;
            }
            internal set { waitsTable = value; }
        }

        public Dictionary<string, Wait> Waits
        {
            get {
                return waits;
            }
            internal set { waits = value; }
        }

        public bool HasBeenCalculated
        {
            get { return hasBeenCalculated; }
        }



        #endregion

        #region events

        #endregion

        #region methods

        internal void CalculateWaitsDeltas(Dictionary<string, Wait> previousWaits)
        {
            if (previousWaits != null && waits != null)
            {
                hasBeenCalculated = true;

                foreach (Wait w in waits.Values)
                {
                    if (previousWaits.ContainsKey(w.WaitType))
                    {
                        if (w.WaitingTasksCountTotal.HasValue && previousWaits[w.WaitType].WaitingTasksCountTotal.HasValue
                            && w.WaitingTasksCountTotal >= previousWaits[w.WaitType].WaitingTasksCountTotal)
                        {
                            w.WaitingTasksCountDelta = w.WaitingTasksCountTotal -
                                                       previousWaits[w.WaitType].WaitingTasksCountTotal;
                        }
                        else
                        {
                            w.WaitingTasksCountDelta = null;
                        }

                        if (w.WaitTimeTotal.HasValue &&  previousWaits[w.WaitType].WaitTimeTotal.HasValue
                            && w.WaitTimeTotal.Value.TotalMilliseconds >= previousWaits[w.WaitType].WaitTimeTotal.Value.TotalMilliseconds)
                        {
                            w.WaitTimeDelta = TimeSpan.FromMilliseconds(
                                w.WaitTimeTotal.Value.TotalMilliseconds -
                                previousWaits[w.WaitType].WaitTimeTotal.Value.TotalMilliseconds);
                        }
                        else
                        {
                            w.WaitTimeDelta = null;
                        }

                         if (w.ResourceWaitTimeTotal.HasValue &&  previousWaits[w.WaitType].ResourceWaitTimeTotal.HasValue
                            && w.ResourceWaitTimeTotal.Value.TotalMilliseconds >= previousWaits[w.WaitType].ResourceWaitTimeTotal.Value.TotalMilliseconds)
                        {
                            w.ResourceWaitTimeDelta = TimeSpan.FromMilliseconds(
                                w.ResourceWaitTimeTotal.Value.TotalMilliseconds -
                                previousWaits[w.WaitType].ResourceWaitTimeTotal.Value.TotalMilliseconds);
                        }
                        else
                        {
                            w.ResourceWaitTimeDelta = null;
                        }
                    }
                    else // previous value was 0
                    {
                        if (w.WaitingTasksCountTotal.HasValue)
                        {
                            w.WaitingTasksCountDelta = w.WaitingTasksCountTotal;
                        }

                        if (w.WaitTimeTotal.HasValue )
                        {
                            w.WaitTimeDelta = w.WaitTimeTotal;
                        }
                       

                        if (w.ResourceWaitTimeTotal.HasValue)
                        {
                            w.ResourceWaitTimeDelta = w.ResourceWaitTimeTotal;
                        }
                    }
                }
            }
        }

        private void CreateWaitsTable()
        {
            waitsTable = new DataTable();
            DataColumn column;
            column = new DataColumn();

            column.DataType = typeof (string);
            column.ColumnName = "WaitType";
            column.Unique = true;
            waitsTable.Columns.Add(column);

            column = new DataColumn();
            column.DataType = typeof (double);
            column.ColumnName = "TotalWaitMillisecondsPerSecond";
            column.Unique = false;
            waitsTable.Columns.Add(column);

            column = new DataColumn();
            column.DataType = typeof (double);
            column.ColumnName = "SignalWaitMillisecondsPerSecond";
            column.Unique = false;
            waitsTable.Columns.Add(column);

            column = new DataColumn();
            column.DataType = typeof (double);
            column.ColumnName = "ResourceWaitMillisecondsPerSecond";
            column.Unique = false;
            waitsTable.Columns.Add(column);

            column = new DataColumn();
            column.DataType = typeof (double);
            column.ColumnName = "WaitingTasksPerSecond";
            column.Unique = false;
            waitsTable.Columns.Add(column);

            column = new DataColumn();
            column.DataType = typeof (TimeSpan);
            column.ColumnName = "MaxWaitTime";
            column.Unique = false;
            waitsTable.Columns.Add(column);

            waitsTable.RemotingFormat = SerializationFormat.Binary;
            if (Waits != null && Waits.Values.Count > 0)
            {
                waitsTable.BeginLoadData();

                foreach (Wait w in Waits.Values)
                {
                    waitsTable.Rows.Add(w.WaitType,
                                        w.TotalWaitMillisecondsPerSecond,
                                        w.SignalWaitMillisecondsPerSecond,
                                        w.ResourceWaitMillisecondsPerSecond,
                                        w.WaitingTasksPerSecond,
                                        w.MaxWaitTime.Value.Milliseconds);
                }

                waitsTable.EndLoadData();
            }
        }

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

    }
}
