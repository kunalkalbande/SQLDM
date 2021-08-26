//------------------------------------------------------------------------------
// <copyright file="ServerLoginConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Snapshots
{
    using System;
    //using System.Text;

    /// <summary>
    /// Represents the sampled SQL Server login configuration.
    /// </summary>
    [Serializable]
    public sealed class ServerLoginConfiguration
    {
        #region fields

        private AuditLevelType? _auditLevel = null;
        private LoginModeType?  _loginMode = null;

        #endregion

        #region constructors

        public ServerLoginConfiguration(string auditLevel, string loginMode)
        {
            _auditLevel = ConvertToAuditLevel(auditLevel);
            _loginMode = ConvertToLoginMode(loginMode);
        }
        
        public ServerLoginConfiguration()
        {
            
        }

        //internal ServerLoginConfiguration(AuditLevelType auditLevel, LoginModeType loginMode)
        //{
        //    _auditLevel = auditLevel;
        //    _loginMode = loginMode;
        //}

        #endregion

        #region properties

        /// <summary>
        /// Gets the SQL Server audit level. Audits are written to the error log and to the Windows NT Event Viewer.
        /// </summary>
        public AuditLevelType? AuditLevel
        {
            get { return _auditLevel; }
        }

        /// <summary>
        /// Gets the SQL Server login security mode.
        /// </summary>
        public LoginModeType? LoginMode
        {
            get { return _loginMode; }
        }

        #endregion

        #region methods

        /// <summary>
        /// Convert string to audit level
        /// </summary>
        /// <preliminary/>
        /// <param name="auditLevel">String value representing an audit level.  Valid values are as follows:
        /// <list type="table">
        /// <listheader>
        ///     <term>Input string</term>
        ///     <description>Returned value</description>
        /// </listheader>
        /// <item>
        ///     <term>"None"</term>
        ///     <description>AuditLevelType.None</description>
        /// </item>
        /// <item>
        ///     <term>"Success"</term>
        ///     <description>AuditLevelType.Success</description>
        /// </item>
        /// <item>
        ///     <term>"Failure"</term>
        ///     <description>AuditLevelType.Failure</description>
        /// </item>
        /// <item>
        ///     <term>"All"</term>
        ///     <description>AuditLevelType.All</description>
        /// </item>
        /// <item>
        ///     <term>All others</term>
        ///     <description>AuditLevelType.Unknown</description>
        /// </item>
        /// </list>
        /// </param>
        /// <returns>Audit level type as AuditLevelType</returns>
        private static AuditLevelType ConvertToAuditLevel(string auditLevel)
        {
            switch (auditLevel.ToLower())
            {
                case "none":
                    return AuditLevelType.None;
                case "success":
                    return AuditLevelType.Success;
                case "failure":
                    return AuditLevelType.Failure;
                case "all":
                    return AuditLevelType.All;
                default:
                    return AuditLevelType.Unknown;
            }
        }

        private static LoginModeType ConvertToLoginMode(string loginMode)
        {
            switch (loginMode.ToLower())
            {
                case "mixed":
                    return LoginModeType.Mixed;
                case "windows authentication":
                    return LoginModeType.WindowsAuthentication;
                default:
                    return LoginModeType.Unknown;
            }
        }

        #endregion

        #region nested types

       

        #endregion
    }
}
