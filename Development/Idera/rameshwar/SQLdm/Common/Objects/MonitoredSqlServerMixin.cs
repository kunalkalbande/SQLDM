using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLdm.Common.Objects
{
    public class MonitoredSqlServerMixin
    {
        #region fields
        private ServerVersion serverVersion = null;
        private bool isUserSysAdmin = false;
        #endregion

        #region constructors

        /// <summary>
        /// Private default constructor
        /// </summary>
        public MonitoredSqlServerMixin()
        {
        }
        
        #endregion

        #region properties
        public ServerVersion ServerVersion
        {
            get
            {
                return serverVersion;
            }
            set
            {
                serverVersion = value;
            }
        }

        public bool IsUserSysAdmin
        {
            get
            {
                return isUserSysAdmin;
            }

            set
            {
                isUserSysAdmin = value;
            }
        }
        #endregion

        #region events

        #endregion

        #region methods

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion
    }
}
