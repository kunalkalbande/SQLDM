using System.ComponentModel;
using Idera.SQLdm.Common.Configuration;
using System.Text;

namespace Idera.SQLdm.DesktopClient.Views.Reports
{
    [DefaultProperty("DurationFilter")]
    internal sealed class WorstPerformingFilter : IUserFilter
    {
        private string applicationLikeFilter = null;
        private string applicationNotLikeFilter = null;
        private string clientComputerLikeFilter = null;
        private string clientComputerNotLikeFilter = null;
        private string databaseLikeFilter = null;
        private string databaseNotLikeFilter = null;
        private string windowsUserLikeFilter = null;
        private string windowsUserNotLikeFilter = null;
        private string sqlUserLikeFilter = null;
        private string sqlUserNotLikeFilter = null;
        private string sqlTextLikeFilter = null;
        private string sqlTextNotLikeFilter = null;
        private bool signatureMode = false;

        internal void CopyTo(WorstPerformingFilter dest)
        {
            dest.ApplicationLikeFilter = ApplicationLikeFilter;
            dest.ApplicationNotLikeFilter = ApplicationNotLikeFilter;
            dest.ClientComputerLikeFilter = ClientComputerLikeFilter;
            dest.ClientComputerNotLikeFilter = ClientComputerNotLikeFilter;
            dest.DatabaseLikeFilter = DatabaseLikeFilter;
            dest.DatabaseNotLikeFilter = DatabaseNotLikeFilter;
            dest.WindowsUserLikeFilter = WindowsUserLikeFilter;
            dest.WindowsUserNotLikeFilter = WindowsUserNotLikeFilter;
            dest.SqlUserLikeFilter = SqlUserLikeFilter;
            dest.SqlUserNotLikeFilter = SqlUserNotLikeFilter;
            dest.SqlTextLikeFilter = SqlTextLikeFilter;
            dest.SqlTextNotLikeFilter = SqlTextNotLikeFilter;
            dest.SignatureMode = SignatureMode;
        }

        [DisplayName("Application like"), Category("General")]
        [Description(
            "Specify application names to include in the report in the form of a T-SQL LIKE clause, where '%' is used as a wildcard."
            )]
        public string ApplicationLikeFilter {
            get {
                if (applicationLikeFilter == null || applicationLikeFilter.Trim().Length == 0) {
                    return null;
                } else {
                    return applicationLikeFilter; //.ToLower();
                }
            }
            set { applicationLikeFilter = value; }
        }

        [DisplayName("Application not like"), Category("General")]
        [Description(
           "Specify application names to exclude from the report in the form of a T-SQL LIKE clause, where '%' is used as a wildcard."
           )]
        public string ApplicationNotLikeFilter {
            get {
                if (applicationNotLikeFilter == null || applicationNotLikeFilter.Trim().Length == 0) {
                    return null;
                } else {
                    return applicationNotLikeFilter; //.ToLower();
                }
            }
            set { applicationNotLikeFilter = value; }
        }

        [DisplayName("Client Computer Like"), Category("General")]
        [Description(
           "Specify client computers to include in the report in the form of a T-SQL LIKE clause, where '%' is used as a wildcard."
           )]
        public string ClientComputerLikeFilter {
            get {
                if (clientComputerLikeFilter == null || clientComputerLikeFilter.Trim().Length == 0) {
                    return null;
                } else {
                    return clientComputerLikeFilter; //.ToLower();
                }
            }
            set { clientComputerLikeFilter = value; }
        }

        [DisplayName("Client Computer Not Like"), Category("General")]
        [Description(
           "Specify client computers to exclude from the report in the form of a T-SQL LIKE clause, where '%' is used as a wildcard."
           )]
        public string ClientComputerNotLikeFilter {
            get {
                if (clientComputerNotLikeFilter == null || clientComputerNotLikeFilter.Trim().Length == 0) {
                    return null;
                } else {
                    return clientComputerNotLikeFilter; //.ToLower();
                }
            }
            set { clientComputerNotLikeFilter = value; }
        }

        [DisplayName("Database Like"), Category("General")]
        [Description(
          "Specify databases to include in the report in the form of a T-SQL LIKE clause, where '%' is used as a wildcard."
           )]
        public string DatabaseLikeFilter {
            get {
                if (databaseLikeFilter == null || databaseLikeFilter.Trim().Length == 0) {
                    return null;
                } else {
                    return databaseLikeFilter; //.ToLower();
                }
            }
            set { databaseLikeFilter = value; }
        }

        [DisplayName("Database Not Like"), Category("General")]
        [Description(
          "Specify databases to exclude from the report in the form of a T-SQL LIKE clause, where '%' is used as a wildcard."
           )]
        public string DatabaseNotLikeFilter {
            get {
                if (databaseNotLikeFilter == null || databaseNotLikeFilter.Trim().Length == 0) {
                    return null;
                } else {
                    return databaseNotLikeFilter; //.ToLower();
                }
            }
            set { databaseNotLikeFilter = value; }
        }

        [DisplayName("Windows User Like"), Category("General")]
        [Description(
         "Specify Windows users to include in the report in the form of a T-SQL LIKE clause, where '%' is used as a wildcard."
           )]
        public string WindowsUserLikeFilter {
            get {
                if (windowsUserLikeFilter == null || windowsUserLikeFilter.Trim().Length == 0) {
                    return null;
                } else {
                    return windowsUserLikeFilter; //.ToLower();
                }
            }
            set { windowsUserLikeFilter = value; }
        }

        [DisplayName("Windows User Not Like"), Category("General")]
        [Description(
        "Specify Windows users to exclude from the report in the form of a T-SQL LIKE clause, where '%' is used as a wildcard."
           )]
        public string WindowsUserNotLikeFilter {
            get {
                if (windowsUserNotLikeFilter == null || windowsUserNotLikeFilter.Trim().Length == 0) {
                    return null;
                } else {
                    return windowsUserNotLikeFilter; //.ToLower();
                }
            }
            set { windowsUserNotLikeFilter = value; }
        }

        [DisplayName("SQL User Like"), Category("General")]
        [Description(
        "Specify SQL users to include in the report in the form of a T-SQL LIKE clause, where '%' is used as a wildcard."
           )]
        public string SqlUserLikeFilter {
            get {
                if (sqlUserLikeFilter == null || sqlUserLikeFilter.Trim().Length == 0) {
                    return null;
                } else {
                    return sqlUserLikeFilter; //.ToLower();
                }
            }
            set { sqlUserLikeFilter = value; }
        }

        [DisplayName("SQL User Not Like"), Category("General")]
        [Description(
       "Specify SQL users to exclude from the report in the form of a T-SQL LIKE clause, where '%' is used as a wildcard."
           )]
        public string SqlUserNotLikeFilter {
            get {
                if (sqlUserNotLikeFilter == null || sqlUserNotLikeFilter.Trim().Length == 0) {
                    return null;
                } else {
                    return sqlUserNotLikeFilter; //.ToLower();
                }
            }
            set { sqlUserNotLikeFilter = value; }
        }

        [DisplayName("SQL Text Like"), Category("General")]
        [Description(
       "Specify SQL text to include in the report in the form of a T-SQL LIKE clause, where '%' is used as a wildcard."
           )]
        public string SqlTextLikeFilter {
            get {
                if (sqlTextLikeFilter == null || sqlTextLikeFilter.Trim().Length == 0) {
                    return null;
                } else {
                    return sqlTextLikeFilter; //.ToLower();
                }
            }
            set { sqlTextLikeFilter = value; }
        }

        [DisplayName("SQL Text Not Like"), Category("General")]
        [Description(
      "Specify SQL text to exclude from the report in the form of a T-SQL LIKE clause, where '%' is used as a wildcard."
           )]
        public string SqlTextNotLikeFilter {
            get {
                if (sqlTextNotLikeFilter == null || sqlTextNotLikeFilter.Trim().Length == 0) {
                    return null;
                } else {
                    return sqlTextNotLikeFilter; //.ToLower();
                }
            }
            set { sqlTextNotLikeFilter = value; }
        }

        [DisplayName("Signature Mode"), Category("General")]
        [Description("Signature Mode will combine stored procedure calls that are the same procedure with different parameters.")]
        public bool SignatureMode
        {
            get { return signatureMode; }
            set { signatureMode = value; }
        }

        public override string ToString() {

            if (HasDefaultValues()) return "No filter specified";
            else {
                StringBuilder builder = new StringBuilder(100);
                if (ApplicationLikeFilter != null) builder.AppendFormat("Application LIKE '{0}', ", ApplicationLikeFilter);
                if (ApplicationNotLikeFilter != null) builder.AppendFormat("Application NOT LIKE '{0}', ", ApplicationNotLikeFilter);
                if (ClientComputerLikeFilter != null) builder.AppendFormat("Client Computer LIKE '{0}', ", ClientComputerLikeFilter);
                if (ClientComputerNotLikeFilter != null) builder.AppendFormat("Client Computer NOT LIKE '{0}', ", ClientComputerNotLikeFilter);
                if (DatabaseLikeFilter != null) builder.AppendFormat("Database LIKE '{0}', ", DatabaseLikeFilter);
                if (DatabaseNotLikeFilter != null) builder.AppendFormat("Database NOT LIKE '{0}', ", DatabaseNotLikeFilter);
                if (WindowsUserLikeFilter != null) builder.AppendFormat("Windows User LIKE '{0}', ", WindowsUserLikeFilter);
                if (WindowsUserNotLikeFilter != null) builder.AppendFormat("Windows User NOT LIKE '{0}', ", WindowsUserNotLikeFilter);
                if (SqlUserLikeFilter != null) builder.AppendFormat("SQL User LIKE '{0}', ", SqlUserLikeFilter);
                if (SqlUserNotLikeFilter != null) builder.AppendFormat("SQL User NOT LIKE '{0}', ", SqlUserNotLikeFilter);
                if (SqlTextLikeFilter != null) builder.AppendFormat("SQL Text LIKE '{0}', ", SqlTextLikeFilter);
                if (SqlTextNotLikeFilter != null) builder.AppendFormat("SQL Text NOT LIKE '{0}', ", SqlTextNotLikeFilter);
                builder.AppendFormat("Signature Mode '{0}', ", SignatureMode);
                return builder.ToString().TrimEnd(' ', ',');
            }
        }
        #region IUserFilter Members

        public void ClearValues()
        {
            applicationLikeFilter = null;
            applicationNotLikeFilter = null;
            clientComputerLikeFilter = null;
            clientComputerNotLikeFilter = null;
            databaseLikeFilter = null;
            databaseNotLikeFilter = null;
            windowsUserLikeFilter = null;
            windowsUserNotLikeFilter = null;
            sqlUserLikeFilter = null;
            sqlUserNotLikeFilter = null;
            sqlTextLikeFilter = null;
            sqlTextNotLikeFilter = null;
            signatureMode = false;
        }

        public bool HasDefaultValues()
        {
            return applicationLikeFilter == null &&
                   applicationNotLikeFilter == null &&
                   clientComputerLikeFilter == null &&
                   clientComputerNotLikeFilter == null &&
                   databaseLikeFilter == null &&
                   databaseNotLikeFilter == null &&
                   windowsUserLikeFilter == null &&
                   windowsUserNotLikeFilter == null &&
                   sqlUserLikeFilter == null &&
                   sqlUserNotLikeFilter == null &&
                   sqlTextLikeFilter == null &&
                   sqlTextNotLikeFilter == null &&
                   signatureMode == false;
        }

        public bool IsFiltered()
        {
            return !(applicationLikeFilter == null &&
                   applicationNotLikeFilter == null &&
                   clientComputerLikeFilter == null &&
                   clientComputerNotLikeFilter == null &&
                   databaseLikeFilter == null &&
                   databaseNotLikeFilter == null &&
                   windowsUserLikeFilter == null &&
                   windowsUserNotLikeFilter == null &&
                   sqlUserLikeFilter == null &&
                   sqlUserNotLikeFilter == null &&
                   sqlTextLikeFilter == null &&
                   sqlTextNotLikeFilter == null &&
                   signatureMode == false);
        }

        public void ResetValues() {
            applicationLikeFilter = null;
            applicationNotLikeFilter = null;
            clientComputerLikeFilter = null;
            clientComputerNotLikeFilter = null;
            databaseLikeFilter = null;
            databaseNotLikeFilter = null;
            windowsUserLikeFilter = null;
            windowsUserNotLikeFilter = null;
            sqlUserLikeFilter = null;
            sqlUserNotLikeFilter = null;
            sqlTextLikeFilter = null;
            sqlTextNotLikeFilter = null;
            signatureMode = false;
        }

        public void UpdateValues(IUserFilter selectionFilter)
        {
            if (selectionFilter is WorstPerformingFilter)
            {
                WorstPerformingFilter filter = (WorstPerformingFilter)selectionFilter;
                applicationLikeFilter = filter.ApplicationLikeFilter;
                applicationNotLikeFilter = filter.ApplicationNotLikeFilter;
                clientComputerLikeFilter = filter.ClientComputerNotLikeFilter;
                clientComputerNotLikeFilter = filter.ClientComputerNotLikeFilter;
                databaseLikeFilter = filter.DatabaseLikeFilter;
                databaseNotLikeFilter = filter.DatabaseNotLikeFilter;
                windowsUserLikeFilter = filter.WindowsUserLikeFilter;
                windowsUserNotLikeFilter = filter.WindowsUserNotLikeFilter;
                sqlUserLikeFilter = filter.SqlUserLikeFilter;
                sqlUserNotLikeFilter = filter.SqlUserNotLikeFilter;
                sqlTextLikeFilter = filter.SqlTextLikeFilter;
                sqlTextNotLikeFilter = filter.SqlTextNotLikeFilter;
                signatureMode = filter.SignatureMode;
            }
        }

        public bool Validate(out string Message)
        {
            Message = null;
            return true;
        }

        #endregion
    }
}