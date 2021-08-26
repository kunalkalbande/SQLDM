// <copyright file="SmtpNotificationProvider.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using Idera.SQLdm.Common.Attributes;

namespace Idera.SQLdm.Common.Import {
    using System.Collections.Generic;
    using Idera.SQLdm.Common.Objects;
    using Idera.SQLdm.Common.Thresholds;

    /// <summary>
    /// This class contains data about a server being imported.
    /// </summary>
    public class ServerImport {
        public ServerImport() {
        }


        public IList<MetricThresholdEntry> Thresholds {
            get {
                if (_thresholds == null) {
                    _thresholds = new List<MetricThresholdEntry>();
                }
                return _thresholds;
            }
            set { _thresholds = value; }
        }
        private IList<MetricThresholdEntry> _thresholds;

        public MonitoredSqlServer MonitoredServer {
            get { return _monitoredServer; }
            set { _monitoredServer = value; }
        }
        private MonitoredSqlServer _monitoredServer;

        /// <summary>
        /// The SQL user ID for connecting to SQL Server.
        /// Null means use NT Authentication.
        /// </summary>
        public string SqlUser {
            get { return _sqlUser; }
            set { _sqlUser = value; }
        }
        private string _sqlUser;

        /// <summary>
        /// The SQL password for connecting to SQL Server.
        /// </summary>
        [AuditableAttribute(true, true)]
        public string SqlPassword {
            get { return _sqlPassword; }
            set { _sqlPassword = value; }
        }
        private string _sqlPassword;

        /// <summary>
        /// The comments from the 4.x server settings dialog.
        /// </summary>
        public string SqlComments {
            get { return _sqlComments; }
            set { _sqlComments = value; }
        }
        private string _sqlComments;

        /// <summary>
        /// The department from the 4.x server info dialog.
        /// </summary>
        public string Department {
            get { return _department; }
            set { _department = value; }
        }
        private string _department;

        /// <summary>
        /// The department from the 4.x server info dialog.
        /// </summary>
        public string Location {
            get { return _location; }
            set { _location = value; }
        }
        private string _location;

        /// <summary>
        /// The department from the 4.x server info dialog.
        /// </summary>
        public bool MaintenanceMode {
            get { return _maintenanceMode; }
            set { _maintenanceMode = value; }
        }
        private bool _maintenanceMode;

    }
}
