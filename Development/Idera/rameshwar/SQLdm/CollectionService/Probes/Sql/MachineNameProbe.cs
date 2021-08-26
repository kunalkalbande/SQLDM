//------------------------------------------------------------------------------
// <copyright file="MachineNameProbe.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
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
    using Common.Services;
    using Common.Snapshots;
    using System.Net.NetworkInformation;


    //SQLdm 9.1 (Ankit Srivastava) - Rally Defect DE15255 -- New Class MachineNameProbe which return MachineName whichever connecting in the given order 1) ServerName 2) NetBIOS name 3)The name that is registered with SQLdm
    internal class MachineNameProbe : SqlBaseProbe
    {

        string _machineName = String.Empty;
        MachineNameSnapshot _machineNameSnapshot = new MachineNameSnapshot();
        private string _instanceName;
        private Result result = Result.Pending;


        public MachineNameProbe(SqlConnectionInfo conn,int? cloudProviderId)
            : base(conn)
        {
            this.cloudProviderId = cloudProviderId;
            LOG = Logger.GetLogger("MachineNameProbe");
            _instanceName = conn.InstanceName;
        }

        protected override void Start()
        {
            StartMachineNameCollector();
        }

        /// <summary>
        /// Starts the Machine Name collector.
        /// The WMI probe for disk statistics needs the machine name and needs to run prior to 
        /// the database summary probe.
        /// </summary>
        private void StartMachineNameCollector()
        {
            StartGenericCollector(new Collector(MachineNameCollector), _machineNameSnapshot, "StartMachineNameCollector", "Machine Name", null, new object[] { });
        }

        /// <summary>
        /// MachineName Collector
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sdtCollector"></param>
        /// <param name="ver"></param>
        private void MachineNameCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            var cmd = SqlCommandBuilder.BuildMachineName(conn);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(MachineNameCallback));
        }

        /// <summary>
        /// MachineNameCallback
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MachineNameCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(MachineNameCallback), _machineNameSnapshot, "MachineNameCallback", "Machine Name", sender, e);
        }

        /// <summary>
        /// Define the Machine Name callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void MachineNameCallback(CollectorCompleteEventArgs e)
        {
            if (e.Result == Result.Success)
            {
                using (SqlDataReader rd = e.Value as SqlDataReader)
                {
                    if (rd.Read())
                        _machineName = rd.GetString(0);
                }
                if (IsAbleToPing(_machineName))//if not able to reach the machine name fetched by SQL Server
                {
                    result = Result.Success;
                    FireCompletion("MachineName");
                }
                else
                    StartComputerBIOSNameCollector();
            }
            else
            {
                result = e.Result;
                FireCompletion("MachineNameFailed");
            }
            

        }


        /// <summary>
        /// Computer BIOS collector
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sdtCollector"></param>
        /// <param name="ver"></param>
        private void ComputerBIOSNameCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            var cmd = SqlCommandBuilder.BuildComputerBIOSName(conn);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(ComputerBIOSNameCallBack));
        }

        /// <summary>
        /// Starts the Computer BIOS Name collector.
        /// before the WMI probe for disk statistics/OS metric/Service Status which need the machine name and needs to run prior to 
        /// the database summary probe.
        /// </summary>
        private void StartComputerBIOSNameCollector()
        {
            StartGenericCollector(new Collector(ComputerBIOSNameCollector), _machineNameSnapshot, "StartComputerBIOSNameCollector", "Machine Name", null, new object[] { });
        }

        /// <summary>
        /// Define the Computer BIOS Name callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void ComputerBIOSNameCallBack(CollectorCompleteEventArgs e)
        {
            if (e.Result == Result.Success)
            {
                using (SqlDataReader rd = e.Value as SqlDataReader)
                {
                    if (rd.Read())
                        _machineName = rd.GetString(0);

                }
                result = Result.Success;
                if (!IsAbleToPing(_machineName))//if not able to connect to ComputerBIOS name 
                {
                    if (!String.IsNullOrEmpty(GetInstanceHostName()))//if instanceHostName Empty let the previously assigned _machineName be returned
                    {
                        _machineName = GetInstanceHostName();
                        if (!IsAbleToPing(_machineName))//if not able to connect to the hostname provided by user
                            FireCompletion("InstanceHostName-Unpingable Though");// it means nothing is pingable
                        else
                            FireCompletion("InstanceHostName-Pingable");// it means MachineName and ComputerBIOSName not pingable and InstanceHostName(might be FQDN) pingable

                        return;
                    }
                    LOG.Warn("MachineName and ComputerBIOSName not pingable and InstanceHostName somehow is empty.");
                    FireCompletion("ComputerBIOSNameLastOption");  //so firing completion with the ComputerName as the last option
                }
                FireCompletion("ComputerBIOSName");  
            }
            else
            {
                result = e.Result;
                FireCompletion("ComputerBIOSNameFailed");
            }
            

        }

        private void FireCompletion(string collectorName)
        {
            _machineNameSnapshot.MachineName = _machineName;
            LOG.InfoFormat("Machine Name Collector completed with {0} collection.", collectorName);
            FireCompletion(_machineNameSnapshot, result);
        }

        private string GetInstanceHostName()
        {
            if (!String.IsNullOrEmpty(_instanceName))
            {
                string[] separator = new string[] { @",", @"\" };
                string[] elements=_instanceName.Split(separator,StringSplitOptions.None);
                if (elements.Length > 0)
                    return elements[0];//hostname 
            }
            return String.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComputerBIOSNameCallBack(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(ComputerBIOSNameCallBack), _machineNameSnapshot, "MachineNameCallback", "Machine Name", sender, e);
        }

        private bool IsAbleToPing(string hostName)
        {
            bool result = false;
            try
            {
                Ping ping = new Ping();
                PingReply reply = ping.Send(hostName, 1000);
                if (reply.Status == IPStatus.Success)
                    return true;
            }
            catch (Exception ex)
            {				LOG.ErrorFormat("MachineNameProbe--Error while pinging {0} ; {1} ",hostName, ex);
            }
            return result;

        }
    }
}
