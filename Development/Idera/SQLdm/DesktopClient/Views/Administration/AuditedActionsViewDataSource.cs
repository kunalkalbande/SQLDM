//------------------------------------------------------------------------------
// <copyright file="AlertsViewDataSource.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Drawing;
using Idera.SQLdm.Common.Events;
using Idera.SQLdm.DesktopClient.Helpers;
using Infragistics.Win.UltraWinDataSource;

namespace Idera.SQLdm.DesktopClient.Views.Administration
{
    /*
     * Column 0 is a long containing the alert id.  For refreshes of the data using the same 
     * filter should only have to add items that have an alert id greater than highest alert id.
     * 
     */

    public class AuditedActionsViewDataSource : DataSourceWithID<long>
    {
        public AuditedActionsViewDataSource()
        {
            base.KeyIndex = 0;
        }

        public override void EndInit()
        {
            ConfigureColumns();
            base.EndInit();
        }

        public void ConfigureColumns()
        {
            // Convert UTCOccuranceDateTime to local time
            this.Band.Columns["DateTime"].Tag = new DateConverter();
        }
    }
}
