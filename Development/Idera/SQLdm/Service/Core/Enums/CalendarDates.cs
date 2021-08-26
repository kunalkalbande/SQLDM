using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Idera.SQLdm.Service.Core.Enums
{
    /// <summary>
    /// SQLdm10.2(srishti purohit)-Enum to help decide duration according to history range passed to GetQueryWaitStatisticsForInstanceOverview API
    /// </summary>
    public enum SummaryLevel
    {
        [Description("null")]
        S = 30,
        [Description("null")]
        C = 60,
        [Description("null")]
        U = 120,
        [Description("null")]
        X = 600,
        [Description("hour")]
        H = 3600,
        [Description("null")]
        G = 14400,
        [Description("day")]
        D = 86400,
        [Description("week")]
        W = 604800
    }
}
