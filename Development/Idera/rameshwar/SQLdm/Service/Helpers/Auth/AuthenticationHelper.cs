using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using Idera.SQLdm.Service.Core.Security;
using Idera.SQLdm.Service.Repository;
using System.Security;
using Idera.SQLdm.Service.Core.Enums;
using Idera.SQLdm.Service.Configuration;
using System.IdentityModel.Tokens;
using BBS.TracerX;

namespace Idera.SQLdm.Service.Helpers.Auth
{
    public class AuthenticationHelper
    {
        private readonly Logger LogX = Logger.GetLogger("AuthenticationHelper"); //initializing the logger for this class

        public IPrincipal Validate(string userName, string password) 
        {
            using (LogX.InfoCall("Entering Validation"))
            {
                return (!RepositoryHelper.IsApplicationSecurityEnabled(RestServiceConfiguration.SQLConnectInfo)) ? ValidateWithDisabledSecurity(userName, password) : ValidateWithEnabledSecurity(userName, password);
            }
            
        }

        private IPrincipal ValidateWithEnabledSecurity(string userName, string password)
        {
            using (LogX.InfoCall("Entering ValidateWithEnabledSecurity"))
            {
                IPrincipal principal = null;

                string domain = null;
                var splittedUserName = userName.Split('\\');
                bool userValidated = false;
                if (splittedUserName.Length != 2) throw new ArgumentException("Incorrect user name format");
                if (string.IsNullOrEmpty(password)) throw new ArgumentException("Password is not specified.");

                userName = splittedUserName[1];
                domain = splittedUserName[0];


                using (var ctx = new ImpersonationContext(domain, userName, password))
                {
                    // Validate password.
                    ctx.LogonUser();

                    // Get native windows groups by doing query through the repository SQL Server.
                    // It is quite likely that the user will not have permission to access the repository,
                    // in which case we need to get the native groups some other way.    For now we are
                    // saying there are no windows groups retrieved so ignoring them.
                    try
                    {
                        ctx.RunAs(() =>
                        {
                            userValidated = RepositoryHelper.AuthenticateUserFromSQLForEnabledSecurity(splittedUserName[0], password, AuthType.Integrated);
                        });
                    }
                    catch
                    {
                        // Ignore this exception, we don't have windows groups at this point.
                    }

                    // Build the return prinicipal.
                    if (userValidated)
                    {
                        var wi = new GenericIdentity(userName, "Windows");
                        principal = new SimplePrincipal(wi, new string[] { userName });
                        LogX.Info("user with user name " + userName + " has been validated");
                    }
                    else
                    {
                        LogX.Info("user with user name " + userName + " has not been validated");
                        throw new System.Security.Authentication.AuthenticationException("User is invalid!");
                    }
                }



                return principal;
            }
        }

        private IPrincipal ValidateWithDisabledSecurity(string userName, string password)
        {
            using (LogX.InfoCall("Entering ValidateWithDisabledSecurity"))
            {
                var isadmin = false;
                //string[] nativeGroups = new string[]{"admin"};
                IPrincipal principal = null;
                bool userValidated = false;
                string domain = null;
                var splittedUserName = userName.Split('\\');
                if (splittedUserName.Length != 2) throw new ArgumentException("Incorrect user name format");
                if (string.IsNullOrEmpty(password)) throw new ArgumentException("Password is not specified.");

                userName = splittedUserName[1];
                domain = splittedUserName[0];

                using (var ctx = new ImpersonationContext(domain, userName, password))
                {
                    // Validate password.
                    ctx.LogonUser();

                    // Get native windows groups by doing query through the repository SQL Server.
                    // It is quite likely that the user will not have permission to access the repository,
                    // in which case we need to get the native groups some other way.    For now we are
                    // saying there are no windows groups retrieved so ignoring them.
                    try
                    {
                        ctx.RunAs(() =>
                        {
                            //nativeGroups = RepositoryHelper.AuthenticateUserFromSQL(parts[0], password, AuthType.Integrated, out isadmin);
                            userValidated = RepositoryHelper.AuthenticateUserFromSQLForDisabledSecurity(userName, password, AuthType.Integrated);

                        });
                    }
                    catch
                    {
                        // Ignore this exception, we don't have windows groups at this point.
                    }

                    if (userValidated)
                    {
                        var wi = new GenericIdentity(userName, "Windows");

                        principal = new SimplePrincipal(wi, new string[] { userName });
                        LogX.Info("user with user name " + userName + " has been validated");
                    }
                    else
                    {
                        LogX.Info("user with user name " + userName + " has not been validated");
                        throw new System.Security.Authentication.AuthenticationException(("User is invalid!"));
                    }

                }

                return principal;
            }
        }

       
    }

    internal class SimplePrincipal : IPrincipal
    {
        private IIdentity identity;
        internal string[] Roles { get; set; }

        internal SimplePrincipal(IIdentity identity, string[] groups)
        {
            this.identity = identity;
            Roles = groups;
        }

        public IIdentity Identity { get { return identity; } }

        public bool IsInRole(string role)
        {
            if (Roles == null || Roles.Length == 0) return false;
            return Roles.Contains(role, StringComparer.InvariantCultureIgnoreCase);
        }
    }
}
