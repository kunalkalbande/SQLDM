using Idera.SQLdm.Service.DataContracts.v1;
using Idera.SQLdm.Service.ServiceContracts.v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Idera.SQLdm.Service.Repository;

using Idera.SQLdm.Service.Core.Enums;
using Idera.SQLdm.Service.Core;
using Idera.SQLdm.Service.Configuration;
using Idera.SQLdm.Service.ServiceContracts.v1;

namespace Idera.SQLdm.Service.Web
{
    public partial class WebService : IGeneral
    {
        public void ForceCleanup()
        {
            using (_logX.InfoCall("ForceCleanup()"))
            {
                //LogProcessInfo();
                _logX.InfoFormat("Forced GC of {0} begin...", GC.MaxGeneration);
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                _logX.InfoFormat("Forced GC of {0} complete", GC.MaxGeneration);
                //LogProcessInfo();
                _logX.Info("WaitForPendingFinalizers begin...");
                GC.WaitForPendingFinalizers();
                _logX.Info("WaitForPendingFinalizers complete");
                //LogProcessInfo();
                _logX.InfoFormat("Post finalization GC of {0} begin...", GC.MaxGeneration);
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                _logX.InfoFormat("Post finalization GC of {0} complete", GC.MaxGeneration);
                //LogProcessInfo();
            }
        }
        
        public string GetVersion() {
            SetConnectionCredentiaslFromCWFHost();
            return (typeof(WebService).Assembly.GetName().Version.ToString()); 
        }

        public DataContracts.v1.GetServiceStatusResponse GetServiceStatus()
        {
            SetConnectionCredentiaslFromCWFHost();
            GetServiceStatusResponse response = new GetServiceStatusResponse();
            response.CanRun = true;
            response.ServiceStatuses = new List<ServiceStatus>();

            var state = RepositoryHelper.GetDatabaseState(RestServiceConfiguration.SQLConnectInfo);
            if (null == state)
            {
                throw new ApplicationException("The repository database is not available.");               
            }
            else if (!state.Online)
            {
                throw new ApplicationException(string.Format("The repository database is {0}.", state.StateDescription));               
            }

            //ToDo: Use Database state to find out other possible errors
            switch (state.State)
            {
                case 0: { break; }
                default: {
                    response.ServiceStatuses.Add(new ServiceStatus(ServiceState.RepositoryAccessError, new Exception(state.StateDescription)));
                    response.CanRun = false;
                    break;
                }
            }
            
            response.Version = RepositoryHelper.GetRepositoryVersion(RestServiceConfiguration.SQLConnectInfo);
            return response;
        }

        ///<summary>
        ///Author : Anshika Sharma
        ///Version : SQLdm 10.2
        ///Stores User Session Settings
        ///<param name="Settings"> User Session Settings as a dictionary having "Key" as name of setting and "Value" as value for that setting. </param>
        ///</summary>
        public string SetUserSessionSettings(Dictionary<string, string> Settings)
        {
            SetConnectionCredentiaslFromCWFHost();
            try
            {
                if (Settings != null)
                {
                   
                    String userName = RestServiceConfiguration.SQLConnectInfo.UserName;
                    _eventLog.WriteEntry("User Name is : " + userName);
                    string response = RepositoryHelper.SetUserSessionSettings(RestServiceConfiguration.SQLConnectInfo, userName, Settings);
                    return response;
                }
                else
                {
                    throw new ArgumentNullException();
                }
            }
            catch (Exception ex)
            {
                LOG.Error("Exception Occured : " + ex.Message);
                return "Nothing to save!";
            }

        }

        ///<summary>
        ///Author : Anshika Sharma
        ///Version : SQLdm 10.2
        ///Retrieves User Session Settings
        ///</summary>
        public Dictionary<string, string> GetUserSessionSettings()
        {
            SetConnectionCredentiaslFromCWFHost();
            try
            {
                String Username = RestServiceConfiguration.SQLConnectInfo.UserName;
                if (Username != null)
                {
                    Dictionary<string, string> response = RepositoryHelper.GetUserSessionSettings(RestServiceConfiguration.SQLConnectInfo, Username);
                    _eventLog.WriteEntry("User name is : " + Username);
                    return response;
                }
                else
                {
                    throw new ArgumentNullException();
                }
            }
            catch (Exception ex)
            {

                return null;
            }
        }
    }
}
