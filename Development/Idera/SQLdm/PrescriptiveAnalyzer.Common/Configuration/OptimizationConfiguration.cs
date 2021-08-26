using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Idera.SQLdoctor.Common.Values;

namespace Idera.SQLdoctor.Common.Configuration
{
    [Serializable]
    public enum OptimizationStatus
    {
        [Description("Waiting to start optimization")]
        OptimizationWaiting,
        [Description("Optimization completed")]
        OptimizationCompleted,
        [Description("Optimization processing cancelled")]
        OptimizationCancelled,
        [Description("Optimization aborted early due to error")]
        OptimizationEarlyAbort,
        
        [Description("Step processing started")]
        StepStarted,
        [Description("Step processing progress")]
        StepProgress,
        [Description("Step processing finished")]
        StepFinished
    }

    public delegate void OptimizationStatusHandler(Guid requestId, OptimizationStatus status, object data);

    public class OptimizationConfiguration : MarshalByRefObject
    {
        /// <summary>
        /// When status is started data is a string description otherwise it is an int containing percent complete.
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="status"></param>
        /// <param name="data"></param>
        public event OptimizationStatusHandler OptimizationStatusChanged;
        
        public readonly Guid _requestId;
        public bool Undoflag;
        private readonly SqlConnectionInfo connectionInfo;

        public Guid GetRequestId() { return (_requestId); }

        public OptimizationConfiguration(SqlConnectionInfo connectionInfo, bool undoflag)
        {
            _requestId = Guid.NewGuid();
            this.connectionInfo = connectionInfo;
            Undoflag = undoflag;
        }

        // Prevent this fuqer from expiring 
        public override object InitializeLifetimeService()
        {
            return null;
        }

        public string InstanceName
        {
            get { return connectionInfo.InstanceName; }
        }

        public SqlConnectionInfo ConnectionInfo
        {
            get { return connectionInfo; }
        }

        internal void FireStatusChanged(OptimizationStatus status, object data)
        {
            if (OptimizationStatusChanged != null)
            {
                OptimizationStatusHandler handler = null;
                foreach (Delegate del in OptimizationStatusChanged.GetInvocationList())
                {
                    try
                    {
                        handler = (OptimizationStatusHandler)del;
                        handler(GetRequestId(), status, data);
                    }
                    catch (Exception e)
                    {
                        ExceptionLogger.Log("OptimizationConfiguration.FireStatusChanged() Exception: ", e);
                        // remove trouble makers
                        OptimizationStatusChanged -= handler;
                    }
                }
            }
        }
    }
}
