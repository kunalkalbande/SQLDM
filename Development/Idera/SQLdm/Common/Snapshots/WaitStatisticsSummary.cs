
namespace Idera.SQLdm.Common.Snapshots
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents Server-Wide Wait Statistics Summary
    /// </summary>
    [Serializable]
    public class WaitStatisticsSummary
    {
        #region fields

        private decimal? iowaits;
        private decimal? lockwaits;
        private decimal? memorywaits;
        private decimal? transactionlogwaits;
        private decimal? otherwaits;
        private decimal? signalwaits;

        #endregion

        #region Properties

        public decimal? IOWaits
        {
            get { return iowaits; }
            internal set { iowaits = value; }
        }

        public decimal? LockWaits
        {
            get { return lockwaits; }
            internal set { lockwaits = value; }
        }

        public decimal? MemoryWaits
        {
            get { return memorywaits; }
            internal set { memorywaits = value; }
        }

        public decimal? TransactionLogWaits
        {
            get { return transactionlogwaits; }
            internal set { transactionlogwaits = value; }
        }

        public decimal? OtherWaits
        {
            get { return otherwaits; }
            internal set { otherwaits = value; }
        }

        public decimal? SignalWaits
        {
            get { return signalwaits; }
            internal set { signalwaits = value; }
        }

        #endregion
    }
}
