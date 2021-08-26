using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.CollectionService.Probes.Sql;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.CollectionService.Probes
{
    /// <summary>
    /// SQLdm 10.0 - vineet kumar. Doctor integration. This is a base calss for probes which should support interval collection
    /// </summary>
    abstract class SqlIntervalProbe : SqlBaseProbe, IIntervalProbe
    {
        private DateTime _nextTimeToExecute = DateTime.Now;
        private int _interval = 0;
        private int _executionCount = 0;
        private int _max = 0;
        private bool _isInterval = false;
        private int _totalMinutes = 5;
        private DateTime _endtime;
        public DateTime NextTimeToExecute { get { return (_nextTimeToExecute); } }
        public virtual bool IsTimeToExecute { get { return (_nextTimeToExecute <= DateTime.Now); } }
        public int ExecutionCount { get; set; }
        public int MaxCount { get { return _max; } }
        public int Interval { get { return _interval; } }
        public bool IsInterval { get { return _isInterval; } }
        public DateTime EndTime { get { return _endtime; } }
        public bool IsRunnable
        {
            get
            {
                if (EndTime <= DateTime.Now)
                    return false;
                if (MaxCount <= 0)
                    return true;
                if (ExecutionCount < MaxCount)
                    return true;
                return false;
            }
        }

        public SqlIntervalProbe(SqlConnectionInfo connectionInfo, int interval, int max, int? cloudProviderId)
            : base(connectionInfo)
        {
            this.cloudProviderId = cloudProviderId;
            _interval = interval;
            _max = max;
            _isInterval = true;
            _endtime = DateTime.Now.AddMinutes(_totalMinutes);
        }

        public SqlIntervalProbe(SqlConnectionInfo connectionInfo, int? cloudProviderId)
            : base(connectionInfo)
        {
            this.cloudProviderId = cloudProviderId;
            _isInterval = false;
        }

        private void UpdateNextTimeToExecute()
        {
            _nextTimeToExecute = DateTime.MaxValue;
            if (_interval <= 0) return;
            if (_max > 0) if (_max <= _executionCount) return;
            _nextTimeToExecute = DateTime.Now.AddSeconds(_interval);
        }
    }
}
