using System;
using System.Collections.Generic;
using System.Text;
using BBS.TracerX;
using System.Data.SqlClient;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common;
using Idera.SQLdm.PrescriptiveAnalyzer.Metrics;
using Idera.SQLdm.PrescriptiveAnalyzer.Properties;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Analyzer
{
    internal class NetworkAnalyzer : AbstractAnalyzer
    {
        private const Int32 id = 10;
        private static Logger _logX = Logger.GetLogger("NetworkAnalyzer");
        protected override Logger GetLogger() { return (_logX); }

        public override string GetDescription() { return ("Network analysis"); }

        public NetworkAnalyzer()
        {
            _id = id;
        }

        public override void Analyze(SnapshotMetrics sm, SqlConnection conn)
        {
            using (_logX.DebugCall("Analyze"))
            {
                base.Analyze(sm, conn);
                if (null == sm.Options) { _logX.Debug("null Options"); return; }
                if (sm.Options.GenerateTestRecommendations) { _logX.Debug("GenerateTestRecommendations: Network recommendations are not valid due to Test recommendations being generated."); }
                LogCards(sm);
                AnalyzeRedirector(sm);
                AnalyzeTCP(sm, this);
                if (IsNetworkSlow(sm) || sm.Options.GenerateTestRecommendations)
                {
                    ComparePreviousSnapshot(sm);
                    AnalyzeCardErrors(sm);
                    AnalyzeNetworkUtilization(sm);
                    AnalyzeEncryptedConnections(sm);
                    AnalyzeNoCountOn(sm, conn);
                    AnalyzeCardCount(sm);
                }
            }
        }

        private void LogCard(WMINetworkInterfaceSnapshots c)
        {
            if (null == c) { _logX.Debug("null card"); return; }
            using (_logX.DebugCall(string.Format("LogCard({0})", c.Name)))
            {
                _logX.DebugFormat("AvgOutputQueueLength:{0}", c.AvgOutputQueueLength());
                _logX.DebugFormat("TotalOutputQueueLength:{0}", c.TotalOutputQueueLength());
                _logX.DebugFormat("AvgPacketsPerSec:{0}", c.AvgPacketsPerSec());
                _logX.DebugFormat("CurrentBandwidth:{0}", c.CurrentBandwidth());
                _logX.DebugFormat("IsActive:{0}", c.IsActive());
                _logX.DebugFormat("HasPacketErrors:{0}", c.HasPacketErrors());
                _logX.DebugFormat("TotalPacketsWithOutboundErrors:{0}", c.TotalPacketsWithOutboundErrors);
                _logX.DebugFormat("TotalPacketsWithReceivedErrors:{0}", c.TotalPacketsWithReceivedErrors);
                _logX.DebugFormat("TotalPacketsPerSec:{0}", c.TotalPacketsPerSec);
            }
        }
        private void LogCards(SnapshotMetrics sm)
        {
            using (_logX.DebugCall("LogCards"))
            {
                if (null == sm) { _logX.Debug("null SnapshotCollector"); return; }
                if (null == sm.WMINetworkInterfaceMetrics) { _logX.Debug("null WMI Network Interface Collector"); return; }
                //if (!IsValid(sc.WMINetworkInterfaceCollector)) return;
                if (null == sm.WMINetworkInterfaceMetrics.Cards) { _logX.Debug("null cards"); return; }
                foreach (var c in sm.WMINetworkInterfaceMetrics.Cards) { LogCard(c); }
            }
        }

        /// <summary>
        /// For an OLTP server, we want to recommend that there are multiple network cards.
        /// If we can only see one network card and it does not appear to be teamed based on
        /// the current bandwidth, recommend they add a redundant card.
        /// </summary>
        /// <param name="sc"></param>
        private void AnalyzeCardCount(SnapshotMetrics sm)
        {
            using (_logX.DebugCall("AnalyzeCardCount"))
            {
                CheckCancel();
                if (null == sm) { _logX.Debug("null SnapshotCollector"); return; }
                if (null == sm.WMINetworkInterfaceMetrics) { _logX.Debug("null WMI Network Interface Collector"); return; }
                //if (!IsValid(sc.WMINetworkInterfaceCollector)) return;
                if (null == sm.Options) { _logX.Debug("null Options"); return; }
                if (!sm.Options.ProductionServer) { _logX.Debug("Not a ProductionServer"); return; }
                _logX.DebugFormat("Cards={0}  Bandwidth={1}", sm.WMINetworkInterfaceMetrics.ActiveNetworkCards, sm.WMINetworkInterfaceMetrics.TotalNetworkBandwidth);
                if (sm.WMINetworkInterfaceMetrics.ActiveNetworkCards > 1) { _logX.Debug("More than one active network card found"); return; }
                if ((10000000 == sm.WMINetworkInterfaceMetrics.TotalNetworkBandwidth) ||
                    (100000000 == sm.WMINetworkInterfaceMetrics.TotalNetworkBandwidth) ||
                    (1000000000 == sm.WMINetworkInterfaceMetrics.TotalNetworkBandwidth) ||
                    (10000000000 == sm.WMINetworkInterfaceMetrics.TotalNetworkBandwidth))
                {
                    _logX.Debug("Recommend adding another network card");
                    AddRecommendation(new NetRedundantCardsRecommendation());
                }
            }
        }

        /// <summary>
        /// Try to determine if no count on is not the default option for the server and
        /// that a large number of queries are executed without setting 'nocount on'.
        /// Since this generates extra network traffic, recommend that they change the setting.
        /// </summary>
        /// <param name="sc"></param>
        /// <param name="conn"></param>
        private void AnalyzeNoCountOn(SnapshotMetrics sm, SqlConnection conn)
        {
            using (_logX.DebugCall("AnalyzeNoCountOn"))
            {
                if (null == sm) { _logX.Debug("null SnapshotCollector"); return; }
                if (null == sm.ServerPropertiesMetrics) { _logX.Debug("null Server Properties Collector"); return; }
                //if (!IsValid(sc.ServerPropertiesCollector)) return;
                if (sm.ServerPropertiesMetrics.NoCountOn) { _logX.Debug("Server property no count on"); return; }

                CheckCancel();
                try
                {
                    ServerVersion ver = new ServerVersion(conn.ServerVersion);
                    EstRowsMetrics est = new EstRowsMetrics(ver);
                    //est.Run(new CollectorState(conn));
                    //----------------------------------------------------------------------------
                    // This is not as accurate as I would like it to be, but the attempt is to determine
                    // how many sql statments that return/modify rows are executed without the 'set nocount on'
                    // option.  When counts are returned for the sql statements, this causes extra network
                    // traffic which could impact performance.
                    // 
                    _logX.DebugFormat("TotalStmtsWithNoCountOn={0}  TotalStmtsWithRows={1}", est.TotalStmtsWithNoCountOn, est.TotalStmtsWithRows);
                    if (est.TotalStmtsWithNoCountOn < (est.TotalStmtsWithRows * Settings.Default.Net_HighStatementsWithNoCountPercent))
                    {
                        _logX.Debug("Recommend no count on");
                        AddRecommendation(new NetNoCountRecommendation(est.TotalStmtsWithRows,
                                                                            est.TotalStmtsWithNoCountOn,
                                                                            null == sm.SampledServerResourcesMetrics ? 0 : sm.SampledServerResourcesMetrics.BatchReqSec));
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogger.Log(_logX, "NetworkAnalyzer.AnalyzeNoCountOn Exception ", ex);
                }
            }
        }

        private void AnalyzeEncryptedConnections(SnapshotMetrics sm)
        {
            using (_logX.DebugCall("AnalyzeEncryptedConnections"))
            {
                CheckCancel();
                if (null == sm) { _logX.Debug("null SnapshotCollector"); return; }
                if (null == sm.ServerPropertiesMetrics) { _logX.Debug("null ServerPropertiesCollector"); return; }
                //if (!IsValid(sc.ServerPropertiesCollector)) return;
                if (null == sm.SampledServerResourcesMetrics) { _logX.Debug("null SampledServerResourcesCollector"); return; }
                //if (!IsValid(sc.SampledServerResourcesCollector)) return;
                _logX.DebugFormat("EncryptedConnections={0}", sm.ServerPropertiesMetrics.EncryptedConnections);
                if (sm.ServerPropertiesMetrics.EncryptedConnections > 0)
                {
                    _logX.Debug("Recommend encryption off");
                    _common.EncryptedConnections = true;
                }
            }
        }

        /// <summary>
        /// If this SQL server instance is responsible for less than 70% of the network
        /// traffic and the server has been running for over 30 minutes, recommend
        /// that the non-sql server processes using up the network bandwidth should be
        /// stopped.
        /// </summary>
        /// <param name="sc"></param>
        private void AnalyzeNetworkUtilization(SnapshotMetrics sm)
        {
            using (_logX.DebugCall("AnalyzeNetworkUtilization"))
            {
                CheckCancel();
                if (null == sm) { _logX.Debug("null SnapshotCollector"); return; }
                if (null == sm.SampledServerResourcesMetrics) { _logX.Debug("null SampledServerResourcesCollector"); return; }
                //if (!IsValid(sc.SampledServerResourcesCollector)) return;
                if (null == sm.WMINetworkInterfaceMetrics) { _logX.Debug("null WMINetworkInterfaceCollector"); return; }
                //if (!IsValid(sc.WMINetworkInterfaceCollector)) return;
                if (null == sm.ServerPropertiesMetrics) { _logX.Debug("null ServerPropertiesCollector"); return; }
                //if (!IsValid(sc.ServerPropertiesCollector)) return;
                if (null == sm.Options) { _logX.Debug("null Options"); return; }
                if (!sm.Options.ProductionServer) { _logX.Debug("Not a production server"); return; }
                if (sm.ServerPropertiesMetrics.MinutesRunning <= (UInt64)Settings.Default.Net_MinUptimeInMinutesForNetworkUtilization) { _logX.DebugFormat("The server has only been running {0} minutes", sm.ServerPropertiesMetrics.MinutesRunning); return; }
                UInt64 sqlPackets = sm.SampledServerResourcesMetrics.SumPackSentAndReceived;
                UInt64 totalPackets = sm.WMINetworkInterfaceMetrics.TotalPacketsPerSec;
                _logX.DebugFormat("The server has been running {0} minutes.  SumPackSentAndReceived={1}  TotalPacketsPerSec={2}", sm.ServerPropertiesMetrics.MinutesRunning, sqlPackets, totalPackets);
                if (sqlPackets < (totalPackets * Settings.Default.Net_MinSqlServerNetworkUtilizationPercent))
                {
                    _logX.Debug("Network congestion recommendation");
                    AddRecommendation(new NetCongestionRecommendation(sqlPackets, totalPackets));
                }
            }
        }

        /// <summary>
        /// Determine if any cards had errors while the analysis was in progress.  If errors occurred, created
        /// a recommendation for the network card.
        /// </summary>
        /// <param name="sc"></param>
        private void AnalyzeCardErrors(SnapshotMetrics sm)
        {
            using (_logX.DebugCall("AnalyzeCardErrors"))
            {
                CheckCancel();
                if (null == sm) { _logX.Debug("null SnapshotCollector"); return; }
                if (null == sm.Options) { _logX.Debug("null SnapshotCollector.Options"); return; }
                if (null == sm.WMINetworkInterfaceMetrics) { _logX.Debug("null WMINetworkInterfaceCollector"); return; }
                //if (!IsValid(sc.WMINetworkInterfaceCollector)) return;
                bool recommendationAdded = false;
                foreach (WMINetworkInterfaceSnapshots s in sm.WMINetworkInterfaceMetrics.GetCardsWithPacketErrors())
                {
                    _logX.DebugFormat("CardErrors  Name={0} PacketsPerSec={1}  TotalPacketsWithOutboundErrors={2}  TotalPacketsWithReceivedErrors={3}", s.Name, s.TotalPacketsPerSec, s.TotalPacketsWithOutboundErrors, s.TotalPacketsWithReceivedErrors);
                    recommendationAdded = true;
                    AddRecommendation(new NetCardErrorsRecommendation(s.Name, s.TotalPacketsPerSec, s.TotalPacketsWithOutboundErrors, s.TotalPacketsWithReceivedErrors));
                }
                //--------------------------------------------------------------------------
                //  If no NetCardError recommendations are added, create dummy/test recommenations.
                //
                if (!recommendationAdded && sm.Options.GenerateTestRecommendations)
                {
                    _logX.Info("GenerateTestRecommendations: NetCardErrorsRecommendation (SDR-N9)");
                    AddRecommendation(new NetCardErrorsRecommendation("Test Network Card Error Recommendation Low", 5000, 1, 1));
                    AddRecommendation(new NetCardErrorsRecommendation("Test Network Card Error Recommendation Medium", 5000, 4000, 1000));
                }
            }
        }

        /// <summary>
        /// Try to determine if the network appears to be slow.
        /// </summary>
        /// <param name="sc"></param>
        /// <returns>true if network looks slow</returns>
        internal static bool IsNetworkSlow(SnapshotMetrics sm)
        {
            using (_logX.DebugCall("IsNetworkSlow"))
            {
                if (null == sm.WMINetworkInterfaceMetrics) { _logX.Debug("null WMINetworkInterfaceCollector"); return (false); }
                //----------------------------------------------------------------------------
                // If we have an output queue length we know that at least one network card reports
                // this value.  If that is the case determine if our average value indicates a network
                // slowdown.
                // 
                if (sm.WMINetworkInterfaceMetrics.TotalOutputQueueLength > 0)
                {
                    if (sm.WMINetworkInterfaceMetrics.MaxAvgOutputQueueLength > Settings.Default.Net_MaxAvgQueueLength)
                    {
                        _logX.Debug("Network is slow based on an average output queue length over " + Settings.Default.Net_MaxAvgQueueLength.ToString());
                        return (true);
                    }
                    return (false);
                }
                //----------------------------------------------------------------------------
                // Since we could not tell anything from the network card queue length, test the
                // wait stats to see if the network waits are larger than 5% of the non net wait stats.
                // 
                //if ((sc.SampledServerResourcesCollector.SumNetWaits > (sc.SampledServerResourcesCollector.SumNonNetWaits * .05)))
                //{
                //    _logX.Debug("Network is slow based on wait stats");
                //    return (true);
                //}
                if (AnalyzeTCP(sm, null) > Settings.Default.Net_HighRetranSegmentsPercent)
                {
                    _logX.Debug("Network is slow based on network retransmits");
                    return (true);
                }
                return (false);
            }
        }

        /// <summary>
        /// Compare current snapshot values to the previous snapshot values to determine
        /// if any recommendations are required based on hard ware changes.
        /// </summary>
        /// <param name="sc">snapshot</param>
        private void ComparePreviousSnapshot(SnapshotMetrics sm)
        {
            using (_logX.DebugCall("ComparePreviousSnapshot"))
            {
                CheckCancel();
                if (null == sm.Previous) { _logX.Debug("No previous information"); return; }
                if (null == sm.Current) { _logX.Debug("No current information"); return; }
                //----------------------------------------------------------------------------
                // If we have lost network bandwidth since our last analysis, inform the user
                // with a recommendation.
                // 
                _logX.DebugFormat("Previous bandwidth={0}  Current bandwidth={1}", sm.Previous.TotalNetworkBandwidth, sm.Current.TotalNetworkBandwidth);
                if (sm.Previous.TotalNetworkBandwidth > sm.Current.TotalNetworkBandwidth)
                {

                    _logX.DebugFormat("Previous cards={0}  Current cards={1}", sm.Previous.ActiveNetworkCards, sm.Current.ActiveNetworkCards);
                    if (sm.Previous.ActiveNetworkCards > sm.Current.ActiveNetworkCards)
                    {
                        _logX.Debug("Network card lost recommendation");
                        AddRecommendation(new NetCardLostRecommendation(
                                                sm.Previous.ActiveNetworkCards,
                                                sm.Previous.TotalNetworkBandwidth,
                                                sm.Current.ActiveNetworkCards,
                                                sm.Current.TotalNetworkBandwidth));
                    }
                    else
                    {
                        _logX.Debug("Network bandwidth lost recommendation");
                        AddRecommendation(new NetBandwidthLostRecommendation(
                                                sm.Previous.TotalNetworkBandwidth,
                                                sm.Current.TotalNetworkBandwidth));
                    }
                }
            }
        }

        /// <summary>
        /// Analyze the tcp information for recommendations
        /// </summary>
        /// <param name="sc"></param>
        internal static double AnalyzeTCP(SnapshotMetrics sm, NetworkAnalyzer na)
        {
            using (_logX.DebugCall(string.Format("AnalyzeTCP(skipRecommendation={0})", (null == na))))
            {
                UInt64 segsPerSec = 0;
                UInt64 segsRetranPerSec = 0;
                double result = 0;
                if (null != sm.WMITCPMetrics)
                {
                    segsPerSec += sm.WMITCPMetrics.SegmentsPersec;
                    segsRetranPerSec += sm.WMITCPMetrics.SegmentsRetransmittedPerSec;
                }
                if (null != sm.WMITCPv4Metrics)
                {
                    segsPerSec += sm.WMITCPv4Metrics.SegmentsPersec;
                    segsRetranPerSec += sm.WMITCPv4Metrics.SegmentsRetransmittedPerSec;
                }
                if (null != sm.WMITCPv6Metrics)
                {
                    segsPerSec += sm.WMITCPv6Metrics.SegmentsPersec;
                    segsRetranPerSec += sm.WMITCPv6Metrics.SegmentsRetransmittedPerSec;
                }
                _logX.DebugFormat("SegmentsPerSec={0}  SegmentsRetransmittedPerSec={1}", segsPerSec, segsRetranPerSec);
                if ((0 == segsPerSec) || (0 == segsRetranPerSec)) return (result);
                result = (double)segsRetranPerSec / (double)segsPerSec;
                if (null == na) return (result);
                _logX.DebugFormat("RetransPercentage={0}", result);
                if (result > Settings.Default.Net_HighRetranSegmentsPercent)
                {
                    _logX.Debug("Network retransmit recommendation");
                    na.AddRecommendation(new NetRetranSegsRecommendation(segsPerSec, segsRetranPerSec));
                }
                return (result);
            }
        }

        /// <summary>
        /// Analyze the redirector for recommendations
        /// </summary>
        /// <param name="sc"></param>
        private void AnalyzeRedirector(SnapshotMetrics sm)
        {
            using (_logX.DebugCall("AnalyzeRedirector"))
            {
                if (null == sm.WMINetworkRedirectorMetrics) { _logX.Debug("null WMINetworkRedirectorCollector"); return; }
                if (null == sm.Options) { _logX.Debug("null SnapshotCollector.Options"); return; }
                //if (!IsValid(sm.WMINetworkRedirectorMetrics)) return;
                if (sm.WMINetworkRedirectorMetrics.NetworkErrorsPerSec > 0)
                {
                    _logX.DebugFormat("Redirector Errors {0}", sm.WMINetworkRedirectorMetrics.NetworkErrorsPerSec);
                    AddRecommendation(new NetRedirectorErrorsRecommendation(sm.WMINetworkRedirectorMetrics.NetworkErrorsPerSec));
                    return;
                }
                if (sm.Options.GenerateTestRecommendations)
                {
                    _logX.Info("GenerateTestRecommendations: NetRedirectorErrorsRecommendation (SDR-N5)");
                    AddRecommendation(new NetRedirectorErrorsRecommendation(44));
                }
            }
        }

    }
}
