// -----------------------------------------------------------------------
// <copyright file="AuditService.cs" company="Idera">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Text;
using BBS.TracerX;
using Idera.SQLdm.Common.Services;
using Wintellect.PowerCollections;

namespace Idera.SQLdm.Common.Auditing
{
    /// <summary>
    /// Class in charge of processing the audit data and send it to the RepositoryHelper
    /// </summary>
    public class AuditingEngine
    {
        private static readonly Logger Log = Logger.GetLogger("Auditing Engine");
        private const String FullUserNameTemplate = "{0}\\{1}";
        private IManagementService _managementService;
        private static string sqlUser;
        private static Dictionary<int, string> headerTemplateList = null;
        
        private static AuditingEngine instance = null;
        private static readonly object padlock = new object();

        /// <summary>
        /// Singleton Instance
        /// </summary>
        public static AuditingEngine Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                        instance = new AuditingEngine();

                    return instance;
                }
            }
        }

        protected Dictionary<int, string> HeaderTemplateList
        {
            get
            {
                if(headerTemplateList == null || headerTemplateList.Count == 0)
                {
                    headerTemplateList = new Dictionary<int, string>();
                    if(_managementService != null)
                    {
                        try
                        {
                            headerTemplateList = _managementService.GetAuditHeaderTemplates();
                        }
                        catch (Exception e)
                        {
                            Log.ErrorFormat("Management Service not found for AuditingEngine.HeaderTemplateList. {0}", e);
                        }
                        
                    }
                }

                return headerTemplateList;
            }

            set { headerTemplateList = value; }
        }

        /// <summary>
        /// Reporistory User
        /// </summary>
        public string SQLUser
        {
            get { return sqlUser; }
            set { sqlUser = value ?? string.Empty; }
        }

        /// <summary>
        /// ManagementService instance
        /// </summary>
        public IManagementService ManagementService
        {
            get { return _managementService; }
            set { _managementService = value; }
        }

        /// <summary>
        /// Method that will call the RepositoryHelper once the AuditableEntity is full filled
        /// </summary>
        /// <param name="auditEntity"></param>
        /// <param name="actionType"></param>

        public void LogAction(AuditableEntity auditEntity, AuditableActionType actionType, params object[] list)
        {
            auditEntity.TimeStamp = DateTime.Now.ToUniversalTime();
            auditEntity.Workstation = Environment.MachineName;
            auditEntity.WorkstationUser = GetWorkstationUser();
            auditEntity.SqlUser = this.SQLUser;
            auditEntity.Header = ProcessHeaderTemplate(actionType, list);
            auditEntity.MetaData = ProcessMetaDataProperties(auditEntity);
            GetBodyFormat(auditEntity);
            try
            {
                this.ManagementService.LogAuditEvent(auditEntity, (short)actionType);
            }
            catch (Exception e)
            {
                Log.ErrorFormat("Management Service not found for AuditingEngine.LogAction. {0}", e);
            }
        }

        /// <summary>
        /// Sets AuditEntity Metadata to display text according to defined Body Format.
        
        //Change Summary:
        //Added "{0}" server for monitoring
        //-----------------------------------
        //Details:
        //Property: Value
        //Property: Value
        //Property: Value
        //Property: Value
        //-----------------------------------
             
        /// </summary>
        /// <param name="auditEntity"></param>
        protected void GetBodyFormat(AuditableEntity auditEntity)
        {
            StringBuilder result = new StringBuilder();
            result.AppendFormat("{0}{1}{1}", auditEntity.Header, Environment.NewLine);
            result.AppendFormat("Details:{0}", Environment.NewLine);
            result.AppendFormat("{0}",
                                string.IsNullOrEmpty(auditEntity.MetaData) ? string.Format("Not applicable{0}", Environment.NewLine) : auditEntity.MetaData);

            auditEntity.MetaData = result.ToString();
        }

        protected string ProcessHeaderTemplate(AuditableActionType actionType, params object[] list)
        {
            string result = string.Empty;

            if (!HeaderTemplateList.ContainsKey((int)actionType)) return result;

            if(list == null || list.Length == 0)
            {
                result = HeaderTemplateList[(int) actionType];
            }
            else
            {
                result = string.Format(HeaderTemplateList[(int)actionType], list);
            }

            return result;
        }

        /// <summary>
        /// Formats all MetaData properties
        /// </summary>
        /// <param name="auditEntity"></param>
        /// <returns></returns>
        protected string ProcessMetaDataProperties(AuditableEntity auditEntity)
        {
            if (auditEntity == null || auditEntity.MetadataProperties == null) return string.Empty;

            var result = new StringBuilder();
            string formatOutput = string.Empty;

            foreach (Pair<string, string> property in auditEntity.MetadataProperties)
            {
                formatOutput = string.IsNullOrEmpty(property.Second) ? "{0}{2}" : "{0}: {1}{2}";
                result.AppendFormat(formatOutput, property.First, property.Second, Environment.NewLine);
            }

            return result.ToString();
        }

        /// <summary>
        /// Returns the full user name logged in the machine.
        /// </summary>
        /// <returns>The full user name logged in the machine.</returns>
        public static String GetWorkstationUser()
        {
            return String.Format(FullUserNameTemplate, Environment.UserDomainName, Environment.UserName);
        }

        public static AuditContextData GetDefaultAuditableEntity(string sqlUser)
        {
            AuditContextData auditContext = new AuditContextData()
            {
                SqlUser = sqlUser,
                Workstation = Environment.MachineName,
                WorkstationUser = GetWorkstationUser()
            };

            return auditContext;
        }

        public static void SetContextData(string activeRepositoryUser)
        {
            AuditContextData auditContext = GetDefaultAuditableEntity(activeRepositoryUser);

            CallContext.SetData(AuditContextData.ContextName, auditContext);
        }

        public static void SetAuxiliarData(string contextName, object data)
        {
            CallContext.SetData(contextName, data);            
        }

        internal static void ClearAuxData(string dataName)
        {
            CallContext.FreeNamedDataSlot(dataName);
        }

        /// <summary>
        /// Acts with a Pop behavior. Whenever we get the object it gets removed from the context in order to start a leaking.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object PopAuxiliarData(string name)
        {
            return PopContextData(name);
        }

        /// <summary>
        /// Acts with a Pop behavior. Whenever we get the object it gets removed from the context in order to start a leaking.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object PopContextData(string name)
        {
            object contextData = CallContext.GetData(name);
            ClearAuxData(name);

            return contextData;
        }
    }
}
