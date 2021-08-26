//------------------------------------------------------------------------------
// <copyright file="WMICollector.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using BBS.TracerX;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.CollectionService.Probes.Wmi;
using System.ComponentModel;
using Idera.SQLdm.CollectionService.Configuration;
using Idera.SQLdm.CollectionService.Monitoring;

namespace Idera.SQLdm.CollectionService.Probes.Collectors
{

    internal class WmiCollector : IDisposable
    {
        public delegate object ManagementObjectReady(WmiCollector collector, ManagementBaseObject newObject);
        private static string ME;

        #region fields

        protected static Logger Log = Logger.GetLogger("WmiCollector");

        private readonly bool _ownsScope;
        private string _machineName;

        private ManagementScope _scope;
        private ConnectionOptions _connectionOptions;

        private ManagementObjectSearcher _searcher;
        private ManagementOperationObserver _operationObserver;
        private ManagementStatus _operationStatus = ManagementStatus.NoError;

        private Stopwatch _stopwatch = new Stopwatch();

        private ArrayList _results;

        private ImpersonationContext _impersonationContext;

        private ManagementObjectReady _interpretObjectCallback;
        private EventHandler<CollectorCompleteEventArgs> _collectionCompleteCallback;
        private ManualResetEvent _asyncWiatHandle;

        public event UnhandledExceptionEventHandler UnhandledException;

        private bool canContinue = true;
        private Timer cancelTimer;
        private bool wmiTimedout = false;
        private bool isOnDemand = false;//SQLdm 10.0.2 -Praveen Suhalka - RunAnalysis issue fix

        #endregion

        #region constructors

        static WmiCollector()
        {
            ME = String.Format("{0}\\{1}", Environment.UserDomainName, Environment.UserName);
        }

        public WmiCollector(string machineName, ConnectionOptions connectionOptions, ImpersonationContext impersonationContext)
        {
            // create a scope
            var scope = new ManagementScope(String.Format(@"\\{0}\root\cimv2", machineName));
            if (connectionOptions != null)
                scope.Options = connectionOptions;

            _impersonationContext = impersonationContext;

            // we created it - so own it
            _ownsScope = true;
            Initialize(scope);
        }

        //SQLdm 10.0.2 -Praveen Suhalka - RunAnalysis issue fix
        public WmiCollector(string machineName, ConnectionOptions connectionOptions, ImpersonationContext impersonationContext, bool onDemand) 
            : this (machineName, connectionOptions, impersonationContext)
        {
            isOnDemand = onDemand;
        }

        /// <summary>
        /// 10.0 SQLdm
        /// Srishti Purohit
        /// To Support WmiEncryptableVolumeProbe for generation of SDR-P16
        /// </summary>
        public WmiCollector(string machineName, ConnectionOptions connectionOptions, ImpersonationContext impersonationContext, string pathForMicrosoftVolumeEncryption)
        {
            // create a scope
            var scope = new ManagementScope(pathForMicrosoftVolumeEncryption);
            if (connectionOptions != null)
                scope.Options = connectionOptions;

            _impersonationContext = impersonationContext;

            // we created it - so own it
            _ownsScope = true;
            Initialize(scope);
        }


        //SQLdm 10.0.2 -Praveen Suhalka - RunAnalysis issue fix
        public WmiCollector(string machineName, ConnectionOptions connectionOptions, ImpersonationContext impersonationContext, string pathForMicrosoftVolumeEncryption, bool onDemand)
            : this(machineName, connectionOptions, impersonationContext, pathForMicrosoftVolumeEncryption)
        {
            isOnDemand = onDemand;
        }

        public WmiCollector(ManagementScope scope, bool ownsScope, ImpersonationContext impersonationContext)
        {
            _ownsScope = ownsScope;
            _impersonationContext = impersonationContext;
            Initialize(scope);
        }

        private void Initialize(ManagementScope scope)
        {
            _scope = scope;
            _machineName = scope.Path.Server;
            _connectionOptions = scope.Options;
        }

        internal static ConnectionOptions CreateConnectionOptions(string machineName, WmiConfiguration wmiConfig, out ImpersonationContext impersonation)
        {
            Log.Verbose("WmiCollector: createConnectionOptions");
            ImpersonationContext ctx = null;
            var opts = new ConnectionOptions();
            //opts.Timeout = TimeSpan.FromSeconds(30);
            opts.Timeout = CollectionServiceConfiguration.WMIQueryTimeout;//SQLdm 8.5(Gaurav Karwal):changed to pick the config from config manager

            if (!wmiConfig.DirectWmiConnectAsCollectionService)
            {
                if (!string.Equals(machineName, Environment.MachineName, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (!String.IsNullOrEmpty(wmiConfig.DirectWmiUserName) && !String.IsNullOrEmpty(wmiConfig.DirectWmiPassword))
                    {
                        var nameparts = wmiConfig.DirectWmiUserName.Split('\\');
                        var domain = (nameparts.Length == 1) ? machineName : nameparts[0];
                        opts.Username = (nameparts.Length > 1) ? nameparts[1] : nameparts[0];
                        opts.Password = wmiConfig.DirectWmiPassword;
                        if (!string.IsNullOrEmpty(domain))
                        {
                            if (domain.Contains(":"))
                                opts.Authority = domain;
                            else
                                opts.Authority = "ntlmdomain:" + domain;
                        }
                    }
                }
                else
                {
                    if (!ME.Equals(wmiConfig.DirectWmiUserName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        Log.Warn("Local WMI connections with credentials require impersonation.  Will run as ", wmiConfig.DirectWmiUserName);
                        var nameparts = wmiConfig.DirectWmiUserName.Split('\\');
                        var domain = (nameparts.Length == 1) ? machineName : nameparts[0];
                        var user = (nameparts.Length > 1) ? nameparts[1] : nameparts[0];
                        ctx = new ImpersonationContext(domain, user, wmiConfig.DirectWmiPassword);
                    }
                    else
                    {
                        Log.Debug("Collection service userid and password specified for local WMI connection.  Impersonation skipped.");
                    }
                }
            }

            impersonation = ctx;
            return opts;
        }


        #endregion

        public ObjectQuery Query { get; set; }

        public bool CanContinue { get { return canContinue; } }

        public IList Results 
		{
            get
            {
                if (_results == null)
                    _results = new ArrayList();

                return _results;
            }
        }

        public bool WMITimedout
        {
            get { return wmiTimedout; }
        }

        public ManagementStatus Status { get { return _operationStatus; } }

        public void BeginCollection(EventHandler<CollectorCompleteEventArgs> collectionCompleteCallback, ManagementObjectReady interpretObjectCallback, ManualResetEvent asyncWaitHandle)
        {
            using (Log.VerboseCall("WMICollector: BeginCollection"))//SQLdm 10.0.2 -Praveen Suhalka - RunAnalysis issue fix
            {
                if (collectionCompleteCallback == null)
                    throw new ArgumentNullException("collectionCompleteCallback");

                canContinue = true;

                _interpretObjectCallback = interpretObjectCallback;
                _collectionCompleteCallback = collectionCompleteCallback;
                _asyncWiatHandle = asyncWaitHandle;

                if (asyncWaitHandle != null)
                    asyncWaitHandle.Reset();

                _stopwatch.Start();

                //cancelTimer = new Timer(
                //    new TimerCallback(ExitCollection),
                //    null,
                //    30000,
                //    30000);

                //SQLdm 8.5 (Gaurav Karwal): using from configuration

                // Tolga K - to fix memory leak begins
                if (cancelTimer != null)
                {
                    cancelTimer.Dispose();
                }
                // Tolga K - to fix memory leak ends

                cancelTimer = new Timer(
                    new TimerCallback(ExitCollection),
                    null,
                    Convert.ToInt64(CollectionServiceConfiguration.WMIQueryTimeout.TotalMilliseconds),
                    Convert.ToInt64(CollectionServiceConfiguration.WMIQueryTimeout.TotalMilliseconds));


                //Start: SQLdm 10.0.2 -Praveen Suhalka - RunAnalysis issue fix
                if (isOnDemand)
                {
                    // start execution in our thread pool
                    try
                    {
                        Collection.OnDemandQueueDelegate(SyncExecute);
                    }
                    catch
                    {
                        SyncExecute();
                    }
                }
                else
                {
                    // start execution in our thread pool
                    try
                    {
                        Collection.QueueDelegate(SyncExecute);
                    }
                    catch
                    {
                        SyncExecute();
                    }
                }
                //End: SQLdm 10.0.2 -Praveen Suhalka - RunAnalysis issue fix
            }
        }

        /// <summary>
        /// Exits the collection after elpased interval from cancelTimer.
        /// </summary>
        /// <param name="state">The state.</param>
        private void ExitCollection(object state)
        {
            using (Log.InfoCall("WMICollector: ExitCollection"))//SQLdm 10.0.2 -Praveen Suhalka - RunAnalysis issue fix
            {
                Exception opex = null;
                if (cancelTimer != null)
                {
                    cancelTimer.Dispose();
                    cancelTimer = null;
                }

                Log.Verbose(
                    String.Format("WMI collector was timed out before this counter could be attempted"));
                _operationStatus = ManagementStatus.Timedout;
                opex = new System.TimeoutException();
                wmiTimedout = true;
                operationObserver_Completed(_operationStatus, opex);
                canContinue = CanContinueAfterException(opex);
            }
        }

        public void SyncExecute()
        {
            using (Log.VerboseCall("WMICollector:SyncExecute"))//SQLdm 10.0.2 -Praveen Suhalka - RunAnalysis issue fix
            {
                Log.Verbose("WMICollector: Executing WQL Query: ", Query.QueryString);
                Exception opex = null;
                try
                {
                    _operationStatus = ManagementStatus.NoError;

                    if (!_scope.IsConnected)
                    {
                        Log.Verbose("Establishing WMI connection to " + _scope.Path);
                    }

                    if (_impersonationContext != null)
                        _impersonationContext.RunAs(SyncExecuteInternal);
                    else
                        SyncExecuteInternal();

                }
                catch (ManagementException e)
                {
                    canContinue = CanContinueAfterException(e);
                    _operationStatus = e.ErrorCode;
                    opex = e;
                }
                catch (Exception e)
                {
                    canContinue = CanContinueAfterException(e);
                    _operationStatus = ManagementStatus.Failed;
                    opex = e;
                }
                finally
                {
                    operationObserver_Completed(_operationStatus, opex);
                    ReleaseSearcher();
                }
            }
        }

        private void SyncExecuteInternal()
        {
            // create a wmi serch object
            CreateSearcher();

            // start the async query
            foreach (var obj in _searcher.Get())
            {
                operationObserver_ObjectReady(obj);
            }
        }

        private void CreateSearcher()
        {
            ReleaseSearcher();

            var enumOptions = new EnumerationOptions();
            enumOptions.Timeout = CollectionServiceConfiguration.WMIQueryTimeout;
            enumOptions.ReturnImmediately = true;
            enumOptions.Rewindable = false;
            _searcher = new ManagementObjectSearcher(_scope, Query, enumOptions);
        }

        private void ReleaseSearcher()
        {
            if (_searcher == null) return;
            try
            {
                var s = _searcher;
                _searcher = null;
                s.Dispose();
            } catch (Exception e)
            {
                Log.Error("Error disposing of wmi search object: ", e);                
            }
        }

        public static bool CanContinueAfterException(Exception e)
        {
            // Operation timedout
            if (e is TimeoutException) return false;
            // creds don't match
            if (e is UnauthorizedAccessException) return false;
            // service not available or some other connection issue
            if (e is COMException) return false;

            if (e is ManagementException)
            {   
                var status = ((ManagementException) e).ErrorCode;
                switch (status)
                {
                    case ManagementStatus.AccessDenied:
                    case ManagementStatus.PrivilegeNotHeld:
                    case ManagementStatus.LocalCredentials:
                    case ManagementStatus.ShuttingDown:
                    case ManagementStatus.Timedout:
                        return false; // for sure no need to keep trying
                }
            }

            return true;
        }

        void operationObserver_ObjectReady(ManagementBaseObject newObject)
        {
            try
            {
                if (_results == null)
                    _results = new ArrayList();
                
                if (_interpretObjectCallback != null)
                {
                    var values = _interpretObjectCallback(this, newObject);
                    if (values == null)
                        return;
                    if (values is ICollection)
                        _results.AddRange((ICollection)values);
                    else
                        if (values is IEnumerable)
                        {
                            foreach (var item in ((IEnumerable)values))
                                _results.Add(item);
                        }
                        else
                            _results.Add(values);
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        void operationObserver_Completed(ManagementStatus status, Exception exception)
        {
            try
            {
                _stopwatch.Stop();

                if (status == ManagementStatus.NoError)
                {
                    _collectionCompleteCallback(this, new CollectorCompleteEventArgs(Results, _stopwatch.ElapsedMilliseconds, Result.Success));

                    //SQLdm 10.0 : Small Features : Updating counter 'Collection Run Time'
                    //Statistics.collectionRunTimeInSeconds += Convert.ToInt64(Convert.ToDouble(_stopwatch.ElapsedMilliseconds) / 0.001 );

                    return;
                }

//                if (exception is ManagementException)
//                {
                    _collectionCompleteCallback(this, new CollectorCompleteEventArgs(null, exception));
//                }
            }
            catch (Exception e)
            {
                // should not happen but since it did we should at least log it and eat it.  This call is usually going 
                // to be from a background thread so 
                Log.ErrorFormat("operationObserver_Completed callback threw an unhandled exception status={0}: {1}", status, e);
                if (UnhandledException != null)
                {
                    try
                    {
                        UnhandledException(this, new UnhandledExceptionEventArgs(e, false));
                    } catch (Exception)
                    {
                        /* */
                    }
                }
            }
            finally
            {
                _stopwatch.Reset();

                if (cancelTimer != null)
                {
                    cancelTimer.Dispose();
                    cancelTimer = null;
                }
            }
        }

        public void Cancel()
        {
            throw new NotImplementedException();
            //if (_operationObserver != null)
            //{
            //    try
            //    {
            //        _operationObserver.Cancel();
            //    }
            //    catch (Exception e)
            //    {
            //        Log.Info("Cancel failed: ", e);
            //    }
            //}
        }

        private static ConstructorInfo MECTOR = null;
        private static MethodInfo GetMessageMethod = null;
        public static ManagementException CreateManagementException(ManagementStatus status, string message, ManagementBaseObject errorInfo)
        {
            if (MECTOR == null)
            {
                var type = typeof(ManagementException);
                MECTOR = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static, 
                                            null, 
                                            new[] { typeof(ManagementStatus), typeof(string), typeof(ManagementBaseObject) }, 
                                            null);

                GetMessageMethod = type.GetMethod("GetMessage", 
                                                   BindingFlags.NonPublic | BindingFlags.Static, 
                                                   null,
                                                   new Type[] {typeof (ManagementStatus)}, 
                                                   null);
            }
            
            if (String.IsNullOrEmpty(message))
            {
                message = GetMessageMethod.Invoke(null, new object[] { status }) as string;
                if (String.IsNullOrEmpty(message) && errorInfo != null)
                {
                    try
                    {
                        message = (string)errorInfo["Description"];
                    }
                    catch
                    {
                    }
                }
            }

            if (String.IsNullOrEmpty(message))
            {
                message = "Management query failed with unknown error: " + status.ToString();
            }

            return MECTOR.Invoke(new object[] { status, message, errorInfo }) as ManagementException; 
        }
    
        internal static T GetReferencePropertyValue<T>(ManagementBaseObject mbo, string propertyName) where T : class
        {
            try
            {
                return mbo.GetPropertyValue(propertyName) as T;
            }
            catch (Exception)
            {
                return null;
            }
        }

        internal static T? GetPropertyValue<T>(ManagementBaseObject mbo, string propertyName) where T : struct
        {
            try
            {
                var o = mbo.GetPropertyValue(propertyName);
                if (o == null) return null;
                if (o is T)
                    return (T)o;
                o = Convert.ChangeType(o, typeof(T));
                if (o == null) return null;
                return (T)o;
            }
            catch (Exception)
            {
                return null;
            }
        }

        internal static T GetPropertyValueOrDefault<T>(ManagementBaseObject mbo, string propertyName) where T : struct
        {
            try
            {
                var o = mbo.GetPropertyValue(propertyName);
                if (o == null) return default(T);
                if (o is T) return (T) o;
                o = Convert.ChangeType(o, typeof (T));
                if (o == null) return default(T);
                return (T) o;
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        public virtual void Dispose()
        {
            _interpretObjectCallback = null;
            _collectionCompleteCallback = null;
        }
    }
}
