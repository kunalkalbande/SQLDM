using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Idera.SQLdoctor.Common.Configuration
{
    [Serializable]
    public enum DiagnosticOrderStatusType
    {
        [Description("Waiting to start processing")]
        Waiting,
        [Description("Diagnostic orders completed")]
        Completed,
        [Description("Order processing started")]
        Order_Started,
        [Description("Order processing progress")]
        Order_Progress,
        [Description("Order processing finished")]
        Order_Finished,
        [Description("Order processing cancelled")]
        Order_Cancelled
    }

    [Serializable]
    public class DiagnosticOrderStatus
    {
        public readonly DiagnosticOrderStatusType statusType;
        public readonly DiagnosticOrder order;
        public readonly int? percentComplete;
        public readonly List<Exception> exceptions;
    }

    public class DiagnosticOrders : MarshalByRefObject
    {
        public delegate void DiagnosticOrderStatusHandler(Guid requestId, DiagnosticOrderStatus status);
        public event DiagnosticOrderStatusHandler DiagnosticOrderStatusChanged;
        private readonly Guid RequestId;

        internal void FireOrderStatusChanged(DiagnosticOrderStatus status)
        {
            if (DiagnosticOrderStatusChanged != null)
            {
                DiagnosticOrderStatusHandler handler = null;
                foreach (Delegate del in DiagnosticOrderStatusChanged.GetInvocationList())
                {
                    try
                    {
                        handler = (DiagnosticOrderStatusHandler)del;
                        handler(RequestId, status);
                    }
                    catch (Exception e)
                    {
                        DiagnosticOrderStatusChanged -= handler;
                    }
                }
            }
        }
    }

    [Serializable]
    public class DiagnosticOrder
    {

    }
}
