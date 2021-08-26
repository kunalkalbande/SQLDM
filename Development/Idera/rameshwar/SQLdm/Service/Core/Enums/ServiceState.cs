using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Idera.SQLdm.Service.Core.Enums
{
    public enum ServiceState
    {
        [Description("Service is creating the repository")]
        RepositoryCreating = 3,
        [Description("Service is upgrading the repository")]
        RepositoryUpgrading = 4,
        [Description("Service failed to access the SQL Server instance hosting the repository")]
        RepositoryAccessError = 5,
        [Description("Service failed to create the repository")]
        RepositoryCreateError = 6,
        [Description("Service failed to upgrade the repository")]
        RepositoryUpgradeError = 7,
        [Description("Service found an unsupported BETA repository")]
        RepositoryBetaUpgradeError = 8,
        [Description("Service encountered an error (catch all state for unexpected errors and exceptions)")]
        GeneralError = 9,

        //App specific 
        [Description("Service failed to connect to IDERA Core Services")]
        CoreServicesError = 201,
        [Description("Scheduler not running")]
        SchedulerError = 202,
    }
}
