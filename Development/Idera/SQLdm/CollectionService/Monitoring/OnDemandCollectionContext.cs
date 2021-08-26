//------------------------------------------------------------------------------
// <copyright file="OnDemandCollectionContext.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.CollectionService.Monitoring
{
    using System;
    using System.Runtime.Remoting;
    using BBS.TracerX;

    using Idera.SQLdm.CollectionService.Probes;
    using Idera.SQLdm.Common.Data;
    using Idera.SQLdm.Common.Services;
    using Idera.SQLdm.Common.Snapshots;
    using System.Collections.Generic;
    using PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Snapshots;

    partial class OnDemandCollectionManager
    {
        /// <summary>
        /// This class manages a single on-demand collection request.
        /// </summary>
        private class OnDemandCollectionContext : IOnDemandContext
        {
            #region fields

            private static Logger LOG = Logger.GetLogger("OnDemandCollectionContext");

            private IProbe probe;
            private ISnapshotSink sink;
            private object state;
            private bool compressSnapshot;

            private IAsyncResult asyncResult;
            private List<IProbe> listProbe;//SQLdm 10.0 Vineet -- Added to support multiple probes
            private List<Snapshot> listSnapshot;//SQLdm 10.0 Vineet -- Added to support multiple probes
            private object lockingObj = new object(); //SQLdm 10.0 vineet kumar -- lockobject to prevent race condition
            private object lockingObj2 = new object();//SQLdm 10.0.2 -Praveen Suhalka - RunAnalysis issue fix

            #endregion

            #region constructors

            public OnDemandCollectionContext(IProbe probe, ISnapshotSink sink, object state) : this(probe, sink, state, false)
            {
            }

            public OnDemandCollectionContext(IProbe probe, ISnapshotSink sink, object state, bool compressSnapshot)
            {
                Probe = probe;
                Sink = sink;
                State = state;
                this.compressSnapshot = compressSnapshot;
                if (probe is IOnDemandProbe)
                {
                    ((IOnDemandProbe)probe).Context = this;
                }
            }

            /// <summary>
            /// SQLdm 10.0 Vineet -- Added to support multiple probes
            /// </summary>
            /// <param name="probe"></param>
            /// <param name="sink"></param>
            /// <param name="state"></param>
            public OnDemandCollectionContext(List<IProbe> probe, ISnapshotSink sink, object state)
                : this(probe, sink, state, false)
            {
            }

            public OnDemandCollectionContext(List<IProbe> probe, ISnapshotSink sink, object state, bool compressSnapshot)
            {
                ListProbe = probe;
                Sink = sink;
                State = state;
                this.compressSnapshot = compressSnapshot;
                if (probe is IOnDemandProbe)
                {
                    ((IOnDemandProbe)probe).Context = this;
                }
            }

            #endregion

            #region properties

            public IProbe Probe
            {
                get { return probe; }
                private set { probe = value; }
            }
            public ISnapshotSink Sink
            {
                get { return sink; }
                private set { sink = value; }
            }
            public object State
            {
                get { return state; }
                private set { state = value; }
            }

            /// <summary>
            /// SQLdm 10.0 Vineet -- Added to support multiple probes
            /// </summary>
            public List<IProbe> ListProbe
            {
                get { return listProbe; }
                private set { listProbe = value; }
            }

            /// <summary>
            /// SQLdm 10.0 Vineet -- Added to support multiple probes
            /// </summary>
            public List<Snapshot> ListSnapshot
            {
                get { return listSnapshot; }
                private set { listSnapshot = value; }
            }

            #endregion

            #region events

            #endregion

            #region methods

            public Result Start()
            {
                try
                {
                    asyncResult = probe.BeginProbe(SnapshotCallback);
                    if (asyncResult == null) return Result.Success;
                    return ((asyncResult.IsCompleted) ? Result.Success : Result.Pending);
                }
                catch
                {
                    return Result.Failure;
                }
            }

            /// <summary>
            /// SQLdm 10.0 Vineet -- Added to support multiple probes
            /// </summary>
            /// <returns></returns>
            public List<Result> StartMultiple()
            {

                List<Result> lstResult = new List<Result>();
                listSnapshot = new List<Snapshot>();
                try
                {
                    lock (lockingObj2)//SQLdm 10.0.2 -Praveen Suhalka - RunAnalysis issue fix
                    {
                        int nextCallbackCount = 20;
                        for (int index = 0; index < listProbe.Count; index++)
                        {
                            var probe = listProbe[index];
                            if (index > nextCallbackCount)
                            {
                                //This is required to prevent too many open connections. The list can have more than 100 probes, 
                                //starting all the probes may  impact the monitored server. So starting 20 probes at a time is the way to go.
                                while (listSnapshot.Count < nextCallbackCount)
                                {
                                }
                                nextCallbackCount += 20;
                            }
                            LOG.Info("BeginProbe Start #" + index.ToString() + listProbe[index].GetType().ToString() + " of " + ListProbe.Count.ToString());//SQLdm 10.0.2 -Praveen Suhalka - RunAnalysis issue fix
                            asyncResult = probe.BeginProbe(SnapshotCallbackMultiple);
                            if (asyncResult == null) lstResult.Add(Result.Success);
                            lstResult.Add(((asyncResult.IsCompleted) ? Result.Success : Result.Pending));
                        }
                    }
                }
                catch (Exception ex)
                {
                    LOG.Error("StartMultiple Probe Exception: " + ex);
                    lstResult.Clear();
                    lstResult.Add(Result.Failure);
                }
                return lstResult;
            }

            /// <summary>
            /// SQLdm 10.0 Vineet -- Added to support multiple probes
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="args"></param>
            private void SnapshotCallbackMultiple(object sender, SnapshotCompleteEventArgs args)
            {
                using (LOG.VerboseCall("SnapshotCallback" + listSnapshot.Count.ToString()))
                {
                    try
                    {

                        lock (lockingObj)//SQLdm 10.0.2 -Praveen Suhalka - RunAnalysis issue fix
                        {
                            Type snapshotType = null;

                            Type sinkType = Sink.GetType();
                            if (sinkType.IsGenericType)
                            {
                                snapshotType = sinkType.GetGenericArguments()[0];
                            }
                            else
                            {
                                snapshotType = args.Snapshot.GetType();
                            }
                            Type wrappedType = typeof(Serialized<>).MakeGenericType(snapshotType);

                            LOG.Info("Snapshot callback Got lock #" + listSnapshot.Count.ToString() + " of " + ListProbe.Count.ToString() + " of type:" + args.Snapshot.GetType().ToString());
                            listSnapshot.Add(args.Snapshot);
                            if (listSnapshot.Count == ListProbe.Count)
                            {
                                ISerialized snapshot = (ISerialized)Activator.CreateInstance(wrappedType, listSnapshot, compressSnapshot);
                                Sink.Process(snapshot, state);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        LOG.WarnFormat("Exception sending on-demand snapshot to management service ({0}): {1}", e.GetType().Name, e.Message);
                    }
                    finally
                    {
                        ((IDisposable)sender).Dispose();
                    }
                }
            }

            private void SnapshotCallback(object sender, SnapshotCompleteEventArgs args)
            {
                using (LOG.VerboseCall("SnapshotCallback"))
                {
                    try
                    {
                        Type snapshotType = null;

                        Type sinkType = Sink.GetType();
                        if (sinkType.IsGenericType)
                        {
                            snapshotType = sinkType.GetGenericArguments()[0];
                        }
                        else
                        {
                            snapshotType = args.Snapshot.GetType();
                        }
                        Type wrappedType = typeof(Serialized<>).MakeGenericType(snapshotType);

                        ISerialized snapshot = (ISerialized)Activator.CreateInstance(wrappedType, args.Snapshot, compressSnapshot);
                        Sink.Process(snapshot, state);
                    }
                    catch (Exception e)
                    {
                        LOG.WarnFormat("Exception sending on-demand snapshot to management service ({0}): {1}", e.GetType().Name, e.Message);
                    }
                    finally
                    {
                        ((IDisposable)sender).Dispose();
                    }
                }
            }

            #endregion

            #region interface implementations

            #region IOnDemandContext Members

            public bool IsCancelled
            {
                get
                {
                    return sink.Cancelled;
                }
            }

            #endregion

            #endregion

            #region nested types

            #endregion


        }
    }

    public interface IOnDemandContext
    {
        bool IsCancelled { get; }
    }
}