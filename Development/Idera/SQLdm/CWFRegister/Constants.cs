using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLdm.CWFRegister
{
    class Constants
    {
        public static string REGISTRATION_PACKAGE_NAME = "CWFPackage";
        public static string REGISTRATION_PACKAGE_EXTENSION = "zip";
        public static string CWFRootURL = "http://{0}:9292";
        public static string CWFAPIStatusPath = "/IderaCoreServices/v1/Status";
        public static string CWFAPIIsAvailablePath = "/IderaCoreServices/v1/Products/IsAvailable";
        public static string CWFAPIProductsPath = "/IderaCoreServices/v1/Products";
        public static string CWF_BASE_URI = "/IderaCoreServices/v1";
        public static string PRODUCT_NAME = "SQLdm";//DO NOT CHANGE THE CASE: very important to keep it small case.
        public static string PRODUCT_SHORT_NAME = "SQLdm";//DO NOT CHANGE THE CASE: very important to keep it small case.
        public static string PRODUCT_WEB_UI_URI = "sqldm"; //DO NOT CHANGE THE CASE: very important to keep it small case.
        public static string PRODUCT_VERSION = string.Empty;
        public static string PRODUCT_STATUS = "Green";
        public static string PRODUCT_REST_API_PLUGIN = "SQLdmPlugin";
        public static string PRODUCT_REST_API_PLUGIN_EXTENSION = "dll";
        public static string PRODUCT_JAR_PLUGIN = "idera-sqldm_cwf_product_widgets-";
        public static string PRODUCT_JAR_PLUGIN_EXTENSION = "jar";
        public static string PRODUCT_WAR_PLUGIN = "idera-sqldm-";
        public static string PRODUCT_WAR_PLUGIN_EXTENSION = "war";
        public static string PRODUCT_VERSION_JAR = string.Empty;
        public static string CWF_PRODUCT_DETAIL_URI_PATTERN = "/Products/{0}?version={1}";
        public static string PRODUCT_DESCRIPTION = "SQL Diagnostic Manager";
        public static string PRODUCT_WEB_UI_BASE_PAGE = "home";
        public static string PRODUCT_INSTANCES_URI = "/GetAllProductInstances";
        public static string CWF_REGISTRATION_URI = "/RegisterProduct/";
        public static string CWF_ADDDASHBOARD_WIDGET_URI = "/AddDashboardWidgets";
        public static int NOT_REGISTERED = -1;
    }
}
