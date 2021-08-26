using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common
{
    public static class Constants
    {
        public const string ConnectionStringApplicationNamePrefix = "IDERA SQL doctor";
        public const string ConnectionStringSQLdmApplicationNamePrefix = "SQL diagnostic manager";

        public const int ProductId = 12;

        public const int DefaultConnectionTimeout = 60;
        public const int DefaultCommandTimeout = 300;
        public const string CheckupExternalEventName = ConnectionStringApplicationNamePrefix + " Checkup Event";
        public const string OneTimeAnalysisExternalEventName = ConnectionStringApplicationNamePrefix + " One Time Analysis Event";
        public static string ScheduledAnalaysisMailSlotName = "\\\\.\\mailslot\\" + ConnectionStringApplicationNamePrefix.Replace(' ', '_') + "\\AnalysisServiceNotifications";


        //        public const string SettingsTagName = "setting";
        //        public const string SettingsFileName = "sqltoolbox_settings.xml";
        //        public const string SettingsMetadataLastRefreshSuccess = "MetadataLastRefreshSuccess";
        //        public const string SettingsMetadataLastRefreshAttempt = "MetadataLastRefreshAttempt";
        //        public const string SettingsMetadataRefreshTimeOfDay = "MetadataRefreshTimeOfDay";
        //        public const string SettingsMetadataURL = "MetadataURL";
        public const string DefaultMetadataURL = "http://downloads.idera.com/sqltoolbox/productmetadata.xml";
        public const string DefaultBaseURL = "http://downloads.idera.com/";
        public const string DefaultPurchaseURL = "http://www.idera.com/Content/Show.aspx?PageID=2&PurchaseType=BuyNowAdd&AddProdID=32";
        //        public const string SettingsApplicationFirstLaunch = "ApplicationFirstLaunch";
        //        public const string SettingsConfirmedNewProducts = "ConfirmedNewProducts";
        //        public const string SettingsConfirmedUpdateProducts = "ConfirmedUpdatedProducts";
        public const string SettingsProxyServer = "ProxyServer";
        public const string SettingsProxyServerPort = "ProxyServerPort";
        public const string SettingsProxyUsername = "ProxyUsername";
        public const string SettingsProxyPassword = "ProxyPassword";



    }
}
