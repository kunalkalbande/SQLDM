using System;
using System.Management;
using System.Security.Principal;
using System.Threading;
using BBS.TracerX;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Services;

namespace Idera.SQLdm.CollectionService.Probes.Wmi
{
    internal abstract class WmiMiniProbe : IDisposable
    {
        private BBS.TracerX.Logger LOG;

        private AsyncResult asyncResult;
        private object asyncResultLock = new object();
        private DateTime startTime;
        private EventHandler<ProbeCompleteEventArgs> snapshotReadyCallback;
        private bool hasCompleted;

        protected Collectors.WmiCollector _collector;
        protected WmiConfiguration _wmiConfig;

        private ImpersonationContext impersonation;
        private bool wmiServiceTimedout;

        public bool WMIServiceTimedout
        {
            get { return wmiServiceTimedout; }
            internal set { wmiServiceTimedout = value; }
        }

        protected WmiMiniProbe(string machineName, WmiConfiguration wmiConfig, Logger logger)
        {
            LOG = logger;

            _wmiConfig = wmiConfig;

            var opts = Collectors.WmiCollector.CreateConnectionOptions(machineName, wmiConfig, out impersonation);

            _collector = new Collectors.WmiCollector(machineName, opts, impersonation);
            _collector.UnhandledException += Collector_UnhandledException;

            startTime = DateTime.Now;
        }

        void Collector_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            FireCompletion(e.ExceptionObject, Result.Failure);
        }

        public void Dispose()
        {
            if (_collector != null)
            {
                _collector.UnhandledException -= Collector_UnhandledException;
            }
            if (asyncResult != null)
            {
                asyncResult.Dispose();
                asyncResult = null;
            }

            if (impersonation != null)
            {
                impersonation.Dispose();
                impersonation = null;
            }
        }

        public WaitHandle AsyncWaitHandle
        {
            get
            {
                if (asyncResult != null)
                    return asyncResult.AsyncWaitHandle;

                return null;
            }
        }

        public DateTime StartTime
        {
            get { return startTime; }
            private set { startTime = value; }
        }

        protected bool IsFinished
        {
            get
            {
                if (asyncResult != null)
                    return asyncResult.IsCompleted;

                return false;
            }
        }

        protected abstract void Start();

        public IAsyncResult BeginProbe(EventHandler<ProbeCompleteEventArgs> snapshotReadyCallback)
        {
            this.snapshotReadyCallback = snapshotReadyCallback;
            asyncResult = new AsyncResult(null, false, false);

            try
            {
                if (impersonation != null)
                {
                    if (!impersonation.IsLoggedOn)
                        impersonation.LogonUser();
                }

                Start();
            } 
            catch (Exception e)
            {
                FireCompletion(e, Result.Failure);    
            }

            return asyncResult;
        }

        protected void FireCompletion(object data, Result result)
        {
            FireCompletion(data, result, null);
        }

        protected void FireCompletion(Exception exception, Result result)
        {
            FireCompletion(null, result, exception);
        }

        protected void FireCompletion(object data, Result result, Exception exception)
        {
            // Add check to ensure FireCompletion has not already fired
            if (!hasCompleted)
            {
                lock (asyncResultLock)
                {
                    if (IsFinished)
                        return;

                    hasCompleted = true;

                    if (snapshotReadyCallback != null)
                        snapshotReadyCallback(this, new ProbeCompleteEventArgs(exception, result, data));

                    if (asyncResult != null)
                        asyncResult.FireCompletion(result);
                }
				
                if (_collector != null)
                {
                    _collector.UnhandledException -= Collector_UnhandledException;
                }
                if (_collector != null)
                {
                    _collector.UnhandledException -= Collector_UnhandledException;
                }
                _collector.Dispose();
            }
            else
            {
                Logger.GetLogger("WmiMiniProbe").Error("FireCompletion fired twice.  Suppressing second return.");
            }
        }

        protected void LogException(Logger logger, Exception e)
        {
            logger.Error("Exception collecting WMI data: ", e);
        }

    }
}
