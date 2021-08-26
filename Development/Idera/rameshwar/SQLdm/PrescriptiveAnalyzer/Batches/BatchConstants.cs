using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Batches
{
    /// <summary>
    /// Contains constant fields required in batch creation
    /// </summary>
    static class BatchConstants
    {
        private static int _defaultCommandTimeout = 0;
        internal const string CopyrightNotice =
            "-- SQL doctor \r\n" +
            "-- Copyright © 2010-2013, Idera, Inc., All Rights Reserved.\r\n\r\n";

        internal const string SwapBatchNotice =
            "-- SQL doctor Swap Batch \r\n" +
            "-- The contents of this batch may differ from built-in functionality  \r\n \r\n";

        internal const string BatchHeader =
            " set transaction isolation level read uncommitted; \r\n" +
            " set lock_timeout 20000; \r\n" +
            " set implicit_transactions off; \r\n" +
            " if @@trancount > 0 commit transaction; \r\n" +
            " set language us_english; \r\n" +
            " set cursor_close_on_commit off; \r\n" +
            " set query_governor_cost_limit 0; \r\n" +
            " set numeric_roundabort off; \r\n" +
            " set deadlock_priority low; \r\n" +
            " set nocount on; \r\n";

        internal const string BatchHeaderNoTransaction =
            " set lock_timeout 20000; \r\n" +
            " set implicit_transactions off; \r\n" +
            " set language us_english; \r\n" +
            " set cursor_close_on_commit off; \r\n" +
            " set query_governor_cost_limit 0; \r\n" +
            " set numeric_roundabort off; \r\n" +
            " set deadlock_priority low; \r\n" +
            " set nocount on; \r\n";

        internal static int DefaultCommandTimeout
        {
            get
            {
                if (_defaultCommandTimeout <= 0)
                {
                    if (Idera.SQLdm.PrescriptiveAnalyzer.Properties.Settings.Default.DefaultSqlCommandTimeoutInMinutes > 0)
                    {
                        _defaultCommandTimeout = 60 * Idera.SQLdm.PrescriptiveAnalyzer.Properties.Settings.Default.DefaultSqlCommandTimeoutInMinutes;
                    }
                    else
                    {
                        _defaultCommandTimeout = Idera.SQLdm.PrescriptiveAnalyzer.Common.Constants.DefaultCommandTimeout;
                    }
                }
                return (_defaultCommandTimeout);
            }
        }

    }
}
