using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.Common.Configuration;
using System.ComponentModel;

namespace Idera.SQLdm.DesktopClient.Objects
{
    internal sealed class TopQueriesAdvancedFilter : IUserFilter
    {
        public TopQueriesAdvancedFilter()
        {
            initialize();
        }

        private void initialize()
        {
            applicationLike = null;
            applicationNotLike = null;
            clientLike = null;
            clientNotLike = null;
            databaseLike = null;
            databaseNotLike = null;
            sqlTextLike = null;
            sqlTextNotLike = null;
            userLike = null;
            userNotLike = null;
           
        }

        #region fields

        private string applicationLike; 
        private string applicationNotLike;
        private string clientLike;
        private string clientNotLike;
        private string databaseLike; 
        private string databaseNotLike; 
        private string sqlTextLike; 
        private string sqlTextNotLike; 
        private string userLike;
        private string userNotLike;

        #endregion

        #region Properties

        [DisplayName("Application Like"), Category("General")]
        [Description("Specify the application name to include in the filter. This filter option can be specifed specifically or in the form of a LIKE clause in T-SQL, where '%' is used as a wildcard.")]
        public string ApplicationLike
        {
            get
            {
                if (applicationLike == null || applicationLike.Trim().Length == 0)
                {
                    return null;
                }
                else
                {
                    return applicationLike;
                }
            }

            set { applicationLike = value; }
        }

        [DisplayName("Application Not Like"), Category("General")]
        [Description("Specify the application name to exclude in the filter. This filter option can be specifed specifically or in the form of a LIKE clause in T-SQL, where '%' is used as a wildcard.")]
        public string ApplicationNotLike
        {
            get
            {
                if (applicationNotLike == null || applicationNotLike.Trim().Length == 0)
                {
                    return null;
                }
                else
                {
                    return applicationNotLike;
                }
            }

            set { applicationNotLike = value; }
        }

        [DisplayName("Client Computer Like"), Category("General")]
        [Description("Specify the client computer name to include in the filter. This filter option can be specifed specifically or in the form of a LIKE clause in T-SQL, where '%' is used as a wildcard.")]
        public string ClientLike
        {
            get
            {
                if (clientLike == null || clientLike.Trim().Length == 0)
                {
                    return null;
                }
                else
                {
                    return clientLike;
                }
            }

            set { clientLike = value; }
        }

        [DisplayName("Client Computer Not Like"), Category("General")]
        [Description("Specify the client computer name to exclude in the filter. This filter option can be specifed specifically or in the form of a LIKE clause in T-SQL, where '%' is used as a wildcard.")]
        public string ClientNotLike
        {
            get
            {
                if (clientNotLike == null || clientNotLike.Trim().Length == 0)
                {
                    return null;
                }
                else
                {
                    return clientNotLike;
                }
            }

            set { clientNotLike = value; }
        }

        [DisplayName("Database Like"), Category("General")]
        [Description("Specify the database name to include in the filter. This filter option can be specifed specifically or in the form of a LIKE clause in T-SQL, where '%' is used as a wildcard.")]
        public string DatabaseLike
        {
            get
            {
                if (databaseLike == null || databaseLike.Trim().Length == 0)
                {
                    return null;
                }
                else
                {
                    return databaseLike;
                }
            }

            set { databaseLike = value; }
        }

        [DisplayName("Database Not Like"), Category("General")]
        [Description("Specify the database name to exclude in the filter. This filter option can be specifed specifically or in the form of a LIKE clause in T-SQL, where '%' is used as a wildcard.")]
        public string DatabaseNotLike
        {
            get
            {
                if (databaseNotLike == null || databaseNotLike.Trim().Length == 0)
                {
                    return null;
                }
                else
                {
                    return databaseNotLike;
                }
            }

            set { databaseNotLike = value; }
        }

        [DisplayName("SQL Text Like"), Category("General")]
        [Description("Specify the SQL Text to include in the filter. This filter option can be specifed specifically or in the form of a LIKE clause in T-SQL, where '%' is used as a wildcard.")]
        public string SQLTextLike
        {
            get
            {
                if (sqlTextLike == null || sqlTextLike.Trim().Length == 0)
                {
                    return null;
                }
                else
                {
                    return sqlTextLike;
                }
            }

            set { sqlTextLike = value; }
        }

        [DisplayName("SQL Text Not Like"), Category("General")]
        [Description("Specify the SQL Text to exclude in the filter. This filter option can be specifed specifically or in the form of a LIKE clause in T-SQL, where '%' is used as a wildcard.")]
        public string SQLTextNotLike
        {
            get
            {
                if (sqlTextNotLike == null || sqlTextNotLike.Trim().Length == 0)
                {
                    return null;
                }
                else
                {
                    return sqlTextNotLike;
                }
            }

            set { sqlTextNotLike = value; }
        }

        [DisplayName("User Like"), Category("General")]
        [Description("Specify the  User to include in the filter. This filter option can be specifed specifically or in the form of a LIKE clause in T-SQL, where '%' is used as a wildcard.")]
        public string UserLike
        {
            get
            {
                if (userLike == null || userLike.Trim().Length == 0)
                {
                    return null;
                }
                else
                {
                    return userLike;
                }
            }

            set { userLike = value; }
        }

        [DisplayName("User Not Like"), Category("General")]
        [Description("Specify the User to exclude in the filter. This filter option can be specifed specifically or in the form of a LIKE clause in T-SQL, where '%' is used as a wildcard.")]
        public string UserNotLike
        {
            get
            {
                if (userNotLike == null || userNotLike.Trim().Length == 0)
                {
                    return null;
                }
                else
                {
                    return userNotLike;
                }
            }

            set { userNotLike = value; }
        }


        #endregion

        #region IUserFilter Members

        public bool HasDefaultValues()
        {
            return applicationLike == null &&
                   applicationNotLike == null &&
                   clientLike == null &&
                   clientNotLike == null &&
                   databaseLike == null &&
                   databaseNotLike == null &&
                   sqlTextLike == null &&
                   sqlTextNotLike == null &&
                   userLike == null &&
                   userNotLike == null;
        }

        public bool IsFiltered()
        {
            return !(applicationLike == null &&
                applicationNotLike == null &&
                clientLike == null &&
                clientNotLike == null &&
                databaseLike == null &&
                databaseNotLike == null &&
                sqlTextLike == null &&
                sqlTextNotLike == null &&
                userLike == null &&
                userNotLike == null
            );
        }

        public void ClearValues()
        {
            initialize();
        }

        public void ResetValues()
        {
            initialize();
        }

        public void UpdateValues(IUserFilter genericFilter)
        {
            if (!(genericFilter is TopQueriesAdvancedFilter))
                return;

            TopQueriesAdvancedFilter filter = (TopQueriesAdvancedFilter)genericFilter;

            applicationLike = filter.ApplicationLike;
            applicationNotLike = filter.ApplicationNotLike;
            clientLike = filter.ClientLike;
            clientNotLike = filter.ClientNotLike;
            databaseLike = filter.DatabaseLike;
            databaseNotLike = filter.DatabaseNotLike;
            sqlTextLike = filter.SQLTextLike;
            sqlTextNotLike = filter.SQLTextNotLike;
            userLike = filter.userLike;
            userNotLike = filter.userNotLike;
        }

        public bool Validate(out string Message)
        {
            Message = null;
            return true;
        }

        #endregion


    }


}
