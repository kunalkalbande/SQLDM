//------------------------------------------------------------------------------
// <copyright file="DependentObjectProbe.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// Author : Srishti Purohit
// Date : 21Aug2015
// Release : SQLdm 10.0
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.CollectionService.Probes.Sql
{
    using System;
    using System.Data.SqlClient;
    using BBS.TracerX;
    using Collectors;
    using Common;
    using Common.Configuration;
    using Common.Snapshots;
    using System.Data;
    using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations;

    internal class DependentObjectProbe : SqlBaseProbe
    {
        DataTable _dependObjects = new DataTable();
        DependentObjectSnapshot _dependObjectsSnapshot = new DependentObjectSnapshot();
        private Idera.SQLdm.Common.Services.Result result = Idera.SQLdm.Common.Services.Result.Pending;
        DatabaseObjectName _objectName = null;
     
        public DependentObjectProbe(SqlConnectionInfo conn, DatabaseObjectName ObjectName, int? cloudProviderId)
            : base(conn)
        {
            LOG = Logger.GetLogger("DependentObjectProbe");
            this.cloudProviderId = cloudProviderId;
            _objectName = ObjectName;
            _dependObjectsSnapshot = new DependentObjectSnapshot();
        }

        protected override void Start()
        {
            StartDependentObjectCollector();
        }

        /// <summary>
        /// Starts the Dependent Object collector.
        /// </summary>
        private void StartDependentObjectCollector()
        {
            StartGenericCollector(new Collector(DependentObjectCollector), _dependObjectsSnapshot, "DependentObjectCollector", "Dependent Object", DependentObjectCallback, new object[] { });
        }

        /// <summary>
        /// DependentObject Collector
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sdtCollector"></param>
        /// <param name="ver"></param>
        private void DependentObjectCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            var cmd = SqlCommandBuilder.BuildDependentObjectsCommand(conn, _objectName);

            if (string.IsNullOrEmpty(cmd.CommandText))
            {
                return;
            }

            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(DependentObjectCallback));
        }

        /// <summary>
        /// DependentObjectCallback
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DependentObjectCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(DependentObjectCallback), _dependObjectsSnapshot, "DependentObjectCallback", "Dependent Object", sender, e);
        }

        /// <summary>
        /// Define the DependentObject callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void DependentObjectCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretDependentObjects(rd);
            }
            FireCompletion(_dependObjectsSnapshot, Idera.SQLdm.Common.Services.Result.Success);
        }

        private void FireCompletion(string collectorName)
        {
            _dependObjectsSnapshot.DependentObject = _dependObjects;
            LOG.InfoFormat("Dependent Object Collector completed with {0} collection.", collectorName);
            FireCompletion(_dependObjectsSnapshot, result);
        }


        private void InterpretDependentObjects(SqlDataReader datareader)
        {
            using (LOG.DebugCall("InterpretDependentObjects"))
            {
                try
                {
                    _dependObjectsSnapshot.DependentObject.Columns.Add("object_id", typeof(int));
                    _dependObjectsSnapshot.DependentObject.Columns.Add("object_type", typeof(int));
                    _dependObjectsSnapshot.DependentObject.Columns.Add("relative_id", typeof(int));
                    _dependObjectsSnapshot.DependentObject.Columns.Add("relative_type", typeof(int));
                    _dependObjectsSnapshot.DependentObject.Columns.Add("object_name", typeof(string));
                    _dependObjectsSnapshot.DependentObject.Columns.Add("object_schema", typeof(string));
                    _dependObjectsSnapshot.DependentObject.Columns.Add("relative_name", typeof(string));
                    _dependObjectsSnapshot.DependentObject.Columns.Add("relative_schema", typeof(string));
                    _dependObjectsSnapshot.DependentObject.Columns.Add("rank", typeof(int));
                    do
                    {
                        while (datareader.Read())
                        {
                            if (datareader.IsDBNull(0) || datareader[0] == null) continue;
                            _dependObjectsSnapshot.DependentObject.Rows.Add(
                          ProbeHelpers.ToInt32(datareader, "object_id"),
                          ProbeHelpers.ToInt32(datareader, "object_type"),
                          ProbeHelpers.ToInt32(datareader, "relative_id"),
                          ProbeHelpers.ToInt32(datareader, "relative_type"),
                          ProbeHelpers.ToString(datareader, "object_name"),
                          ProbeHelpers.ToString(datareader, "object_schema"),
                          ProbeHelpers.ToString(datareader, "relative_name"),
                          ProbeHelpers.ToString(datareader, "relative_schema"),
                          ProbeHelpers.ToInt32(datareader, "rank")
                       );
                        }
                    }
                    while (datareader.NextResult());
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(_dependObjectsSnapshot,
                                                        LOG,
                                                        "Error interpreting DependentObjects Collector: {0}",
                                                        e,
                                                        false);
                    GenericFailureDelegate(_dependObjectsSnapshot);
                }
            }
        }



    }
}
