using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PluginCommon;
using CWFContracts = Idera.SQLdm.Common.CWFDataContracts;

namespace Idera.SQLdm.Service.Helpers.CWF
{
    /// <summary>
    /// SQLdm 9.0 (Gaurav Karwal): For translating the service data contract into CWF objects
    /// </summary>
    internal static class ObjectTranslator
    {
        /// <summary>
        /// SQLdm 9.0 (Gaurav Karwal): Translating third party alert list to CWF alert list
        /// </summary>
        /// <param name="thirdPartyAlertList"></param>
        /// <returns></returns>
        public static PluginCommon.Alerts TranslateToCWFAlertList(List<CWFContracts.Alert> thirdPartyAlertList) 
        {
            if (thirdPartyAlertList == null) throw new ArgumentNullException("the argument alert list was null");

            PluginCommon.Alerts responseAlertList = new Alerts();
            
            thirdPartyAlertList.ForEach(c => {
                responseAlertList.Add(new PluginCommon.Alert() 
                {
                    AlertCategory = c.AlertCategory,
                    Database  = c.Database,
                    Instance = c.Instance,
                    LastActiveTime = c.LastActiveTime,
                    Metric = c.Metric,
                    ProductId = c.ProductId,
                    Severity = c.Severity.Equals("Info") ? "Informational" : c.Severity,
                    StartTime = c.StartTime,
                    Summary = c.Summary,
                    Table = c.Table,
                    Value = c.Value
                });
            });
            return responseAlertList;
        }

        /// <summary>
        /// SQLdm 9.0 (Gaurav Karwal): Translating third party instance list to CWF instance list list
        /// </summary>
        /// <param name="thirdPartyInstanceList"></param>
        /// <returns></returns>
        public static PluginCommon.CreateInstances TranslateToCWFInstanceList(List<CWFContracts.Instance> thirdPartyInstanceList)
        {
            if (thirdPartyInstanceList == null) throw new ArgumentNullException("the argument instance list was null");

            PluginCommon.CreateInstances responseInstanceList = new CreateInstances();

            thirdPartyInstanceList.ForEach(c =>
            {
                responseInstanceList.Add(new PluginCommon.CreateInstance()
                {
                    Comments = c.Comments,
                    Edition = c.Edition,
                    Location = c.Location,
                    Name = c.Name,
                    Owner = c.Owner,
                    UtcFirstSeen = c.UtcFirstSeen.Value,
                    UtcLastSeen = c.UtcLastSeen,
                    Version = c.Version,
                    Status = (PluginCommon.InstanceStatus) c.InstanceStatus
                });
            });
            return responseInstanceList;
        }

        /// <summary>
        /// SQLdm 9.0 (Gaurav Karwal): Translating third party instance list to CWF instance list list
        /// </summary>
        /// <param name="thirdPartyInstanceList"></param>
        /// <returns></returns>
        public static PluginCommon.Widgets TranslateToCWFDashboardWidgetList(List<CWFContracts.DashboardWidget> thirdPartyDashboardWidgetList)
        {
            if (thirdPartyDashboardWidgetList == null) throw new ArgumentNullException("the argument instance list was null");

            PluginCommon.Widgets responseWidgetList = new Widgets();

            thirdPartyDashboardWidgetList.ForEach(c =>
            {
                responseWidgetList.Add(new PluginCommon.Widget()
                {
                    Name = c.Name,
                    NavigationLink = c.NavigationLink,
                    DataURI = c.DataURI,
                    Type = c.Type,
                    PackageURI = c.PackageURI,
                    Description = c.Description,
                    Version = c.Version,
                    Settings = c.Settings,
                    DefaultViews = c.DefaultViews,
                    Collapsed = c.Collapsed
                });
            });
            return responseWidgetList;
        }


        /// <summary>
        /// SQLdm 9.0 (Gaurav Karwal): Translating third party user list to CWF user list
        /// </summary>
        /// <param name="thirdPartyUserList"></param>
        /// <returns></returns>
        public static List<PluginCommon.CreateUser> TranslateToCWFUserList(List<CWFContracts.User> thirdPartyUserList)
        {
            if (thirdPartyUserList == null) throw new ArgumentNullException("the argument instance list was null");

            List<PluginCommon.CreateUser> responseUserList = new List<PluginCommon.CreateUser>();

            thirdPartyUserList.ForEach(c =>
            {
                responseUserList.Add(new PluginCommon.CreateUser()
                {
                   Account = c.Account,
                   SID = c.SID,
                   UserType = c.UserType
                });
            });
            return responseUserList;
        }

        /* CWF Team - Backend Isolation - SQLDM-29086
         * Created new Version of TranslateToCWFUserList to return PluginCommon.User instead of PluginCommon.CreateUser */
        public static List<PluginCommon.User> TranslateToCWFUserList1(List<CWFContracts.User> thirdPartyUserList)
        {
            if (thirdPartyUserList == null) throw new ArgumentNullException("the argument instance list was null");

            List<PluginCommon.User> responseUserList = new List<PluginCommon.User>();

            thirdPartyUserList.ForEach(c =>
            {
                responseUserList.Add(new PluginCommon.User()
                {
                    Account = c.Account,
                    SID = c.SID,
                    UserType = c.UserType
                });
            });
            return responseUserList;
        }

        /// <summary>
        /// SQLdm 9.0 (Abhishek Joshi): Translating the ProductInstance data contract (PluginCommon) to Instance data contract (CWFContracts)
        /// </summary>
        /// <param name="productInstancesList"></param>
        /// <returns></returns>
        public static List<CWFContracts.Instance> TranslateToCWFContract(PluginCommon.ProductInstances productInstancesList)
        {
            if (productInstancesList == null) throw new ArgumentNullException("the argument instance list was null");

            List<CWFContracts.Instance> responseInstanceList = new List<CWFContracts.Instance>();

            foreach (PluginCommon.ProductInstance instance in productInstancesList)
            {
                CWFContracts.Instance cwfInstance = new CWFContracts.Instance();
                
                cwfInstance.Name = instance.Name;
                cwfInstance.Comments = instance.Comments;
                cwfInstance.Edition = instance.Edition;
                cwfInstance.Location = instance.Location;
                cwfInstance.Owner = instance.Owner;

                cwfInstance.UtcFirstSeen = instance.UtcFirstSeen;
                cwfInstance.UtcLastSeen = instance.UtcLastSeen.ToString();
                    
                cwfInstance.Version = instance.Version;

                responseInstanceList.Add(cwfInstance);
            }
                
            return responseInstanceList;
        }


        public static List<Idera.SQLdm.Common.CWFDataContracts.GlobalTag> TranslateTags(PluginCommon.Tags tags)
        {
            List<Idera.SQLdm.Common.CWFDataContracts.GlobalTag> tagList = new List<Idera.SQLdm.Common.CWFDataContracts.GlobalTag>();

            foreach (PluginCommon.Tag tag in tags)
            {
                Idera.SQLdm.Common.CWFDataContracts.GlobalTag t = new Idera.SQLdm.Common.CWFDataContracts.GlobalTag();
                t.ID = tag.ID;
                t.Name = tag.Name;
                tagList.Add(t);
            }
            return tagList;
        }


    }
}
