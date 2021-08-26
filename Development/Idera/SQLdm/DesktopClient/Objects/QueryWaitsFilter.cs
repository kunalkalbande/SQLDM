using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.Common.Configuration;
using System.ComponentModel;

namespace Idera.SQLdm.DesktopClient.Objects
{
    [DefaultProperty("TopN")]
    internal sealed class QueryWaitsFilter : IUserFilter
    {
        public QueryWaitsFilter()
        {
            this.topN = DefaultTopN;
            this.sqldmapplicationExcludeFilter = true;
        }

        #region fields

        private const int DefaultTopN = 10;

        private int    topN;
        private string userNameIncludeFilter;
        private string userNameExcludeFilter;
        private string applicationNameIncludeFilter; //
        private string applicationNameExcludeFilter; //
        private string databaseNameIncludeFilter; //
        private string databaseNameExcludeFilter; //
        private string clientNameIncludeFilter; //
        private string clientNameExcludeFilter; //
        private string sqlTextIncludeFilter;
        private string sqlTextExcludeFilter;
        private bool sqldmapplicationExcludeFilter;

        #endregion

        #region Properties

        public event EventHandler<EventArgs> OnChanged;

        [DisplayName("Maximum Number of Rows"), Category("Data"), DefaultValue(DefaultTopN)]
        [Description("Specify the maximum number of rows to display in the graph.")]
        public int MaxRows
        {
            get { return topN;  }
            set { topN = value; NotifyChanged(); }
        }

        [DisplayName("Exclude SQL User"), Category("Exclude Filters")]
        [Description("Specify the SQL user name to exclude in the filter. This filter can be specifed specifically or in the form of a LIKE clause in T-SQL, where '%' is used as a wildcard.")]
        public string SqlUserNameExcludeFilter
        {
            get
            {
                if (userNameExcludeFilter == null || userNameExcludeFilter.Trim().Length == 0)
                {
                    return null;
                }
                else
                {
                    return userNameExcludeFilter;
                }
            }
            set { userNameExcludeFilter = value; NotifyChanged(); }
        }

        [DisplayName("Include SQL User"), Category("Include Filters")]
        [Description("Specify the SQL user name to include in the filter. This filter can be specifed specifically or in the form of a LIKE clause in T-SQL, where '%' is used as a wildcard.")]
        public string SqlUserNameIncludeFilter
        {
            get
            {
                if (userNameIncludeFilter == null || userNameIncludeFilter.Trim().Length == 0)
                {
                    return null;
                }
                else
                {
                    return userNameIncludeFilter;
                }
            }
            set { userNameIncludeFilter = value; NotifyChanged(); }
        }

        [DisplayName("Exclude Application"), Category("Exclude Filters")]
        [Description("Specify the application name to exclude in the filter. This filter option can be specifed specifically or in the form of a LIKE clause in T-SQL, where '%' is used as a wildcard.")]
        public string ApplicationNameExcludeFilter
        {
            get
            {
                if (applicationNameExcludeFilter == null || applicationNameExcludeFilter.Trim().Length == 0)
                {
                    return null;
                }
                else
                {
                    return applicationNameExcludeFilter;
                }
            }

            set { applicationNameExcludeFilter = value; NotifyChanged(); }
        }

        [DisplayName("Include Application"), Category("Include Filters")]
        [Description("Specify the application name to include in the filter. This filter option can be specifed specifically or in the form of a LIKE clause in T-SQL, where '%' is used as a wildcard.")]
        public string ApplicationNameIncludeFilter
        {
            get
            {
                if (applicationNameIncludeFilter == null || applicationNameIncludeFilter.Trim().Length == 0)
                {
                    return null;
                }
                else
                {
                    return applicationNameIncludeFilter;
                }
            }

            set { applicationNameIncludeFilter = value; NotifyChanged(); }
        }

        [DisplayName("Exclude Client Computer"), Category("Exclude Filters")]
        [Description("Specify the client computer name to exclude in the filter. This filter option can be specifed specifically or in the form of a LIKE clause in T-SQL, where '%' is used as a wildcard.")]
        public string ClientNameExcludeFilter
        {
            get
            {
                if (clientNameExcludeFilter == null || clientNameExcludeFilter.Trim().Length == 0)
                {
                    return null;
                }
                else
                {
                    return clientNameExcludeFilter;
                }
            }
            set { clientNameExcludeFilter = value; NotifyChanged(); }
        }

        [DisplayName("Include Client Computer"), Category("Include Filters")]
        [Description("Specify the client computer name to include in the filter. This filter option can be specifed specifically or in the form of a LIKE clause in T-SQL, where '%' is used as a wildcard.")]
        public string ClientNameIncludeFilter
        {
            get
            {
                if (clientNameIncludeFilter == null || clientNameIncludeFilter.Trim().Length == 0)
                {
                    return null;
                }
                else
                {
                    return clientNameIncludeFilter;
                }
            }
            set { clientNameIncludeFilter = value; NotifyChanged(); }
        }

        [DisplayName("Exclude Database"), Category("Exclude Filters")]
        [Description("Specify the database name to exclude in the filter. This filter option can be specifed specifically or in the form of a LIKE clause in T-SQL, where '%' is used as a wildcard.")]
        public string DatabaseNameExcludeFilter
        {
            get
            {
                if (databaseNameExcludeFilter == null || databaseNameExcludeFilter.Trim().Length == 0)
                {
                    return null;
                }
                else
                {
                    return databaseNameExcludeFilter;
                }
            }
            set { databaseNameExcludeFilter = value; NotifyChanged(); }
        }

        [DisplayName("Include Database"), Category("Include Filters")]
        [Description("Specify the database name to include in the filter. This filter option can be specifed specifically or in the form of a LIKE clause in T-SQL, where '%' is used as a wildcard.")]
        public string DatabaseNameIncludeFilter
        {
            get
            {
                if (databaseNameIncludeFilter == null || databaseNameIncludeFilter.Trim().Length == 0)
                {
                    return null;
                }
                else
                {
                    return databaseNameIncludeFilter;
                }
            }
            set { databaseNameIncludeFilter = value; NotifyChanged(); }
        }

        [DisplayName("Exclude SQL Text"), Category("Exclude Filters")]
        [Description("Specify the SQL text to exclude in the filter. This filter can be specifed specifically or in the form of a LIKE clause in T-SQL, where '%' is used as a wildcard.")]
        public string SqlTextExcludeFilter
        {
            get
            {
                if (sqlTextExcludeFilter == null || sqlTextExcludeFilter.Trim().Length == 0)
                {
                    return null;
                }
                else
                {
                    return sqlTextExcludeFilter;
                }
            }
            set { sqlTextExcludeFilter = value; NotifyChanged(); }
        }

        [DisplayName("Exclude SQLdm"), Category("System")]
        [Description("Exlude SQL Diagnostic Manager queries")]
        public bool SqldmapplicationExcludeFilter
        {
            get{ return sqldmapplicationExcludeFilter; }
            set { sqldmapplicationExcludeFilter = value; NotifyChanged(); }
        }

        [DisplayName("Include SQL Text"), Category("Include Filters")]
        [Description("Specify the SQL text to include in the filter. This filter can be specifed specifically or in the form of a LIKE clause in T-SQL, where '%' is used as a wildcard.")]
        public string SqlTextIncludeFilter
        {
            get
            {
                if (sqlTextIncludeFilter == null || sqlTextIncludeFilter.Trim().Length == 0)
                {
                    return null;
                }
                else
                {
                    return sqlTextIncludeFilter;
                }
            }
            set { sqlTextIncludeFilter = value; NotifyChanged(); }
        }

        #endregion

        #region IUserFilter Members

        public bool HasDefaultValues()
        {
            return topN                         == DefaultTopN &&
                   applicationNameExcludeFilter == null &&
                   applicationNameIncludeFilter == null &&
                   clientNameExcludeFilter      == null &&
                   clientNameIncludeFilter      == null &&
                   databaseNameExcludeFilter    == null &&
                   databaseNameIncludeFilter    == null &&
                   userNameExcludeFilter        == null &&
                   userNameIncludeFilter        == null &&
                   sqlTextExcludeFilter         == null &&
                   sqlTextIncludeFilter         == null &&
                   sqldmapplicationExcludeFilter;
        }

        public bool IsFiltered()
        {
            return !(topN == 0 && 
                   applicationNameExcludeFilter == null &&
                   applicationNameIncludeFilter == null &&
                   clientNameExcludeFilter      == null &&
                   clientNameIncludeFilter      == null &&
                   databaseNameExcludeFilter    == null &&
                   databaseNameIncludeFilter    == null &&
                   userNameExcludeFilter        == null &&
                   userNameIncludeFilter        == null &&
                   sqlTextExcludeFilter         == null &&
                   sqlTextIncludeFilter         == null &&
                   sqldmapplicationExcludeFilter
                );
        }

        public void ClearValues()
        {          
            applicationNameExcludeFilter = null;
            applicationNameIncludeFilter = null;
            clientNameExcludeFilter      = null;
            clientNameIncludeFilter      = null;
            databaseNameExcludeFilter    = null;
            databaseNameIncludeFilter    = null;
            userNameExcludeFilter        = null;
            userNameIncludeFilter        = null;
            sqlTextExcludeFilter         = null;
            sqlTextIncludeFilter         = null;
            sqldmapplicationExcludeFilter = true;
        }

        public void ResetValues()
        {
            topN                         = DefaultTopN;
            applicationNameExcludeFilter = null;
            applicationNameIncludeFilter = null;
            clientNameExcludeFilter      = null;
            clientNameIncludeFilter      = null;
            databaseNameExcludeFilter    = null;
            databaseNameIncludeFilter    = null;
            userNameExcludeFilter        = null;
            userNameIncludeFilter        = null;
            sqlTextExcludeFilter         = null;
            sqlTextIncludeFilter         = null;
            sqldmapplicationExcludeFilter = true;
        }

        public void UpdateValues( IUserFilter genericFilter )
        {
            if( !( genericFilter is QueryWaitsFilter ) )
                return;

            QueryWaitsFilter filter = ( QueryWaitsFilter )genericFilter;

            topN                         = filter.MaxRows;
            applicationNameExcludeFilter = filter.ApplicationNameExcludeFilter;
            applicationNameIncludeFilter = filter.ApplicationNameIncludeFilter;
            clientNameExcludeFilter      = filter.ClientNameExcludeFilter;
            clientNameIncludeFilter      = filter.ClientNameIncludeFilter;
            databaseNameExcludeFilter    = filter.DatabaseNameExcludeFilter;
            databaseNameIncludeFilter    = filter.DatabaseNameIncludeFilter;
            userNameExcludeFilter        = filter.SqlUserNameExcludeFilter;
            userNameIncludeFilter        = filter.SqlUserNameIncludeFilter;
            sqlTextExcludeFilter         = filter.SqlTextExcludeFilter;
            sqlTextIncludeFilter         = filter.SqlTextIncludeFilter;
            sqldmapplicationExcludeFilter = filter.SqldmapplicationExcludeFilter;

            NotifyChanged();
        }

        public bool Validate( out string Message )
        {
            Message = null;
            return true;
        }

        #endregion

        public void NotifyChanged()
        {
            if( OnChanged != null )
                OnChanged( this, EventArgs.Empty );
        }
    }
}
