// ===============================
// AUTHOR       : CWF Team - Gowrish 
// PURPOSE      : Backend Isolation
// TICKET       : SQLDM-29086
// ===============================
using Idera.SQLdm.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Constants = Idera.SQLdm.Common.Constants;

namespace Idera.SQLdm.Service.Helpers.CWF
{
    public class CWFConstants
    {
        public static String BaseCoreServieEndpointUrl = "/IderaCoreServices/V1/";
        public static String BaseDashboardServicesEndpointUrl = "/IderaDashboardServices/V1/";
    }

    public static class CWFApiEndoints
    {
        public static string GetPrincipal = "/authenticate/token/GetPrincipal";
        public static string SyncInstances = "/Instances?productId={0}";
        public static string RegisterInstances = "/Products/{0}/RegisterInstances";
        public static string SyncAlerts = "/Alerts";
        public static string SyncAlertsMetaData = "/Alerts/CreateUpdateAlertsMetadata";
        public static string SyncSecurity = "Products/{0}/SecurityConfigurations";
        public static string RegisterWidgets = "/Products/{0}/Widgets";
        public static string SyncLicenses = "/Products/{0}/Licenses";
        public static string GetTags = "/Products/{0}/Tags";
        public static string GetTag = "/Tag/{0}";
        public static string GetTagInstances = "/Tags/{0}/Instances";
        public static string SyncTags = "/Tags/Local";
        public static string SyncResourcesToTag = "/Tags/Local/Resources";
        public static string AddDatabases = "/Products/{0}/Databases";
        public static string EditUser = "/Users/{0}/Permissions";
        public static string Users = "/Users";
        public static string RefreshToken = "/Products/RefreshToken";
        public static string ConnectionCredentialsForInstance = "/Products/{0}/ConnectionCredentials";
        public static string AddWidget = "/Products/{0}/Widgets";
        public static string AssignInstancePermissions = "/Permissions/Instance";
        public static string GetProductInstances = "/Instances";
        public static string GetUser = "/User/{0}";
        public static string CreateUser = "/Users";
        public static string Roles = "/Roles";
        public static string UnregisterInstances = "/Products/{0}/UnregisterInstances";
        public static string GetConnectionCredentialsOfProductInstance = "/Products/{0}/ConnectionCredentials";
        public static string CreateUpdateAlertsMetadata = "/Alerts/CreateUpdateAlertsMetadata";
        public static string GetSQLDMProductsByInstanceName = "/Products/SQLdm";
        //SQLDM-29855
        public static string GetProducts = "/Products";
    }
}
