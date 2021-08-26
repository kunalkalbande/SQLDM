using System;
using System.Collections.Generic;
using Idera.SQLdm.Service.ServiceContracts.v1;
using Idera.SQLdm.Service.DataContracts.v1.Auth;
using System.ServiceModel;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using Idera.SQLdm.Service.Core.Enums;
using Idera.SQLdm.Service.Helpers;
using Idera.SQLdm.Service.Types.Extensions;
using Idera.SQLdm.Service.Repository;
using Idera.SQLdm.Service.Configuration;
using Idera.SQLdm.Service.Helpers.Auth;

namespace Idera.SQLdm.Service.Web
{

    public partial class WebService : IAuthManager
    {
        public UserAuthenticationResponse GetUserAuthenticationResponse(UserAuthenticationRequest authReq)
        {
            

            bool _isAuthSuccess = true;
            //bool _validWindowsUser = false;
            UserType uType = UserType.Admin;

            ////check for valid windows credentials if app security is turned off
            //if (!RepositoryHelper.IsApplicationSecurityEnabled(RestServiceConfiguration.SQLConnectInfo))
            //{
            //    using (PrincipalContext pCtx = new PrincipalContext(ContextType.Machine))
            //    {
            //        _isAuthSuccess = _isAuthSuccess == false ? pCtx.ValidateCredentials(authReq.UserName, authReq.Password) : _isAuthSuccess;
            //    }
            //}
            //else 
            //{
            //    //using (PrincipalContext pCtx = new PrincipalContext(ContextType.Machine))
            //    //{
            //    //    _validWindowsUser = _validWindowsUser == false ? pCtx.ValidateCredentials(authReq.UserName, authReq.Password) : _isAuthSuccess;
            //    //}
            //    //if (_validWindowsUser)//check for access only if valid windows user
            //    //{
            //        //check for sysadmin role if application security is turned on
            //    WebApplicationUser appUser = RepositoryHelper.GetWebApplicationUser(RestServiceConfiguration.SQLConnectInfo, authReq.UserName);
            //    if (appUser != null)
            //    {
            //        uType = appUser.IsSysAdmin == true ? UserType.Admin : UserType.User;
            //        _isAuthSuccess = true;
            //    }
            //    else
            //    {
            //        uType = UserType.None;
            //        _isAuthSuccess = false;
            //    }
            //    //}
            //    //else 
            //    //{
            //    //    uType = UserType.None;
            //    //    _isAuthSuccess = false;
            //    //}
  
            //}
            


            return new UserAuthenticationResponse()
            {
                IsAuthentic = _isAuthSuccess,
                UserName = authReq.UserName,
                UserType = uType.GetNameFromValue()
            };
        }

        public AppSecurityEnabledResponse IsAppSecurityEnabled() 
        {
            SetConnectionCredentiaslFromCWFHost();
            return new AppSecurityEnabledResponse()
            {
                IsSecurityEnabled = RepositoryHelper.IsApplicationSecurityEnabled(RestServiceConfiguration.SQLConnectInfo)
            };
        }
    }
}
