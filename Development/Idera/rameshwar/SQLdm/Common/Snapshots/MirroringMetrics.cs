using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text;
using System.ComponentModel;

namespace Idera.SQLdm.Common.Snapshots
{
    [Serializable]
    public class MirroringMetrics
    {
        public enum MirroringRoleEnum : byte { Principal = 1, Mirror = 2 };
        public enum MirroringStateEnum : byte {
            Suspended = 0, 
            Disconnected, 
            Synchronizing, 
            [Description("Pending Failover")]
            PendingFailover, 
            Synchronized 
        };
        public enum WitnessStatusEnum : byte {
            [Description("No Witness Configured")]
            NoWitness = 0, 
            Connected, 
            Disconnected 
        };
        

        //start sp_dbmmonitorresults
        private MirroringRoleEnum _role;
        private MirroringStateEnum _MirroringState;
        private WitnessStatusEnum _WitnessStatus;
        private int _LogGenerationRate;
        private int _UnsentLog;
        private int _SendRate;
        private int _UnrestoredLog;
        private int _RecoveryRate;
        private int _TransactionDelay;
        private int _TransactionsPerSec;
        private int _AverageDelay;
        private DateTime _TimeRecorded;
        private DateTime _TimeBehind;
        private DateTime _LocalTime;
        //end sp_dbmmonitorresults

        /// <summary>
        /// Role played by current database in the mirroring relationship. Principal or Mirror
        /// </summary>
        public MirroringRoleEnum Role
        {
            get { return _role; }
            internal set { _role = value; }
        }
        /// <summary>
        /// This is the current state of the mirroring relationship
        /// Can be Suspended, Disconnected, Synchronizing, Pending Failover, Synchronized
        /// </summary>
        public MirroringStateEnum MirroringState
        {
            get { return _MirroringState; }
            internal set { _MirroringState = value; }
        }
        /// <summary>
        /// Connection status of the witness in the database mirroring session of the database, can be: - connected, disconnected or unknown
        /// </summary>
        public WitnessStatusEnum WitnessStatus
        {
            get { return _WitnessStatus; }
            internal set { _WitnessStatus = value; }
        }
        /// <summary>
        /// Amount of log generated since preceding update of the mirroring status of this database in kilobytes/sec.
        /// </summary>
        public int LogGenerationRate
        {
            get { return _LogGenerationRate; }
            internal set { _LogGenerationRate = value; }
        }

        /// <summary>
        /// Size of the unsent log in the send queue on the principal in kilobytes.
        /// </summary>
        public int UnsentLog
        {
            get { return _UnsentLog; }
            internal set { _UnsentLog = value; }
        }
        /// <summary>
        /// Send rate of log from the principal to the mirror in kilobytes/sec.
        /// </summary>
        public int SendRate
        {
            get { return _SendRate; }
            internal set { _SendRate = value; }
        }
        public TimeSpan? TimeToSend
        {
            get
            {
                if(_UnsentLog > 0 && _SendRate > 0)
                {
                    return new TimeSpan(0, 0, 0, _UnsentLog / _SendRate);
                }
                return null;
            }
        }
        /// <summary>
        /// Size of the redo queue on the mirror in kilobytes.
        /// </summary>
        public int UnrestoredLog
        {
            get { return _UnrestoredLog; }
            internal set { _UnrestoredLog = value; }
        }
        /// <summary>
        /// Redo rate on the mirror in kilobytes/sec.
        /// </summary>
        public int RecoveryRate
        {
            get { return _RecoveryRate; }
            internal set { _RecoveryRate = value; }
        }
        public TimeSpan? TimeToRestore
        {
            get
            {
                if (_UnrestoredLog > 0 && _RecoveryRate > 0)
                {
                    return new TimeSpan(0,0,0,_UnrestoredLog / _RecoveryRate);
                }
                return null;
            }
        }

        public TimeSpan? TimeToSendAndRestore
        {
            get
            {
                if(TimeToSend != null && TimeToRestore != null)
                {
                    return new TimeSpan(0,0,0,TimeToSend.Value.Add(TimeToRestore.Value).Seconds * 7 / 10);
                }
                return null;
            }
        }
        /// <summary>
        /// Total delay for all transactions in milliseconds.
        /// Transaction delay \ transpersec gives the average delay (mirror commit overhead)
        /// http://download.microsoft.com/download/4/7/a/47a548b9-249e-484c-abd7-29f31282b04d/DBM_Best_Pract.doc
        /// </summary>
        public int TransactionDelay
        {
            get { return _TransactionDelay; }
            internal set { _TransactionDelay = value; }
        }
        /// <summary>
        /// Number of transactions that are occurring per second on the principal server instance.
        /// Transaction delay \ transpersec gives the average delay (mirror commit overhead)
        /// http://download.microsoft.com/download/4/7/a/47a548b9-249e-484c-abd7-29f31282b04d/DBM_Best_Pract.doc
        /// </summary>
        public int TransactionsPerSec
        {
            get { return _TransactionsPerSec; }
            internal set { _TransactionsPerSec = value; }
        }

        /// <summary>
        /// Average delay on the principal server instance for each transaction because of database mirroring. In high-performance mode (that is, when the SAFETY property is set to OFF), this value is generally 0.
        /// Commonly known as mirror commit overhead (http://msdn.microsoft.com/en-us/library/ms365355.aspx)
        /// </summary>
        public int AverageDelay
        {
            get { return _AverageDelay; }
            internal set { _AverageDelay = value; }
        }

        /// <summary>
        /// Time at which the row was recorded by the database mirroring monitor. This is the system clock time of the principal.
        /// </summary>
        public DateTime TimeRecorded
        {
            get { return _TimeRecorded; }
            internal set { _TimeRecorded = value; }
        }
        /// <summary>
        /// Approximate system-clock time of the principal (UTC) to which the mirror database is currently caught up. This value is meaningful only on the principal server instance.
        /// </summary>
        public DateTime TimeBehind
        {
            //if this datetime is equal to a defalt datetime then return SQLDatetime.minvalue
            get { return _TimeBehind==new DateTime()?(DateTime)SqlDateTime.MinValue:_TimeBehind; }
            internal set { _TimeBehind = value; }
        }
        /// <summary>
        /// Formatted timespan version of LocalTime - TimeBehind giving the ages 
        /// of the oldest unsent transaction
        /// </summary>
        public TimeSpan OldestUnsentTransaction
        {
            get
            {
                //if time behind is not in it's raw initialized state
                if ((SqlDateTime.MinValue != TimeBehind) && (SqlDateTime.MinValue != TimeRecorded))
                {
                    return TimeRecorded.Subtract(TimeBehind);
                }
                 return TimeSpan.Zero;
            }
        }
        /// <summary>
        /// System clock time on the local server instance when this row was updated.
        /// </summary>
        public DateTime LocalTime
        {
            get { return _LocalTime; }
            internal set { _LocalTime = value; }
        }
    }
}
