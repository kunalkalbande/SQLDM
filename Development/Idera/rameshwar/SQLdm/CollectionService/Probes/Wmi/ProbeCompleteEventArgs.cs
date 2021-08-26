using System;
using Idera.SQLdm.Common.Services;

namespace Idera.SQLdm.CollectionService.Probes.Wmi
{
    public class ProbeCompleteEventArgs : EventArgs
    {
        private readonly Result result;
        private readonly object data;
        private readonly Exception exception;

        public ProbeCompleteEventArgs(object data, Result result) : this(null, result, data)
        {
        }

        public ProbeCompleteEventArgs(Exception exception, Result result) : this(exception, result, null)
        {
        }

        public ProbeCompleteEventArgs(Exception exception, Result result, object data)
        {
            this.exception = exception;
            this.result = result;
            this.data = data;
        }

        public object Data
        {
            get { return data; }
        }

        public Result Result
        {
            get { return result; }
        }

        public Exception Error
        {
            get { return exception; }
        }
    }
}
