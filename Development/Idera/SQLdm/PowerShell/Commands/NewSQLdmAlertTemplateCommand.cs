using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Thresholds;
using Idera.SQLdm.PowerShell.Helpers;
using Idera.SQLdm.PowerShell.Objects;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Management.Automation;
using System.Net;
using System.Linq;

namespace Idera.SQLdm.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.New, "SQLdmAlertTemplate")]
    public class NewSQLdmAlertTemplateCommand : PSCmdlet
    {
        private int templateID = 0;
        private Dictionary<int, string> instanceIDs = new Dictionary<int, string>();
        private string sqlConnectionString;
        private string noMatchParam = string.Empty;
        private List<string> tagsFromDB = new List<string>();
        private List<string> validTagNames = new List<string>();

        private readonly List<string> selectedTags = new List<string>();

        [Parameter(Mandatory = true, Position = 0)]
        public string TemplateName
        {
            get { return Name; }
            set { Name = value; }
        }
        private string Name;

        [Parameter(Mandatory = true, Position = 1)]
        public string DataSource
        {
            get { return dataSourceName; }
            set { dataSourceName = value; }
        }
        private string dataSourceName;

        [Parameter(Mandatory = true, Position = 2)]
        public string RepositoryName
        {
            get { return databaseName; }
            set { databaseName = value; }
        }
        private string databaseName;

        [Parameter]
        public List<string> RepositoryInstances
        {
            get { return instanceName; }
            set { instanceName = value; }
        }
        private List<string> instanceName;

        [Parameter]
        public List<string> TagsNames
        {
            get { return _tagsName; }
            set { _tagsName = value; }
        }
        private List<string> _tagsName;

        [Credential]
        [Parameter]
        public PSCredential Credential
        {
            get { return credential; }
            set { credential = value; }
        }
        private PSCredential credential;

        List<int> excluded = new List<int>();

        public bool flag = false;

        protected override void BeginProcessing()
        {
            if (null != RepositoryInstances && 0 < RepositoryInstances.Count && null != TagsNames && 0 < TagsNames.Count)
            {
                // Both instances and tags are not allowed hence generate error and return
                WriteObject("Both instances and tags are not allowed");
                flag = true;
            }
            else if (null == RepositoryInstances && null == TagsNames)
            {
                // Atleast one of instances and tags are required
                WriteObject("Atleast one of instances and tags are required");
                flag = true;
            }
            else
            {
                SqlConnectionStringBuilder connectionString = new SqlConnectionStringBuilder();
                //SQLDM-30517. Update Null check for credentials.
                if (Credential != null && !String.IsNullOrEmpty(Credential.UserName))
                {
                    NetworkCredential creds = (NetworkCredential)Credential;
                    if (creds != null)
                    {
                        connectionString.UserID = creds.UserName;
                        connectionString.Password = creds.Password;
                        connectionString.IntegratedSecurity = false;
                    }
                }
                else
                    connectionString.IntegratedSecurity = true;

                connectionString.DataSource = dataSourceName;
                connectionString.InitialCatalog = databaseName;
                SQLdmAlertTemplateInfo templateinfo = new SQLdmAlertTemplateInfo(connectionString);

                sqlConnectionString = connectionString.ToString();
                if (!string.IsNullOrEmpty(sqlConnectionString))
                {
                    templateID = templateinfo.GetAlertTemplateById(connectionString, TemplateName);
                    if (null == instanceName)
                    {
                        ICollection<Tag> syncTags = templateinfo.GetTags(connectionString);
                        if (null != syncTags)
                            tagsFromDB = syncTags.Select(tag => tag.Name).ToList();


                        validTagNames = tagsFromDB.Intersect(_tagsName).ToList();
                        if (null != validTagNames && validTagNames.Count > 0)
                        {
                            instanceIDs = templateinfo.GetInstanceByTags(connectionString, validTagNames);
                        }
                        else
                        {
                            string errormsg = "string with space may be quoted with (single quote)\nstring without space should not be quoted with (single quote) and (double quote) ";
                            WriteObject(errormsg);
                        }
                        noMatchParam = "tags";
                    }
                    else
                    {
                        instanceIDs = templateinfo.GetInstanceId(connectionString, instanceName);
                        noMatchParam = "instances";
                    }

                    if (instanceIDs.Count == 0 || instanceIDs == null)
                    {
                        string tagsException = string.Format("No matching {0} found", noMatchParam);
                        GetThresholdMessage(tagsException);
                    }
                }
            }
        }

        protected override void ProcessRecord()
        {
            try
            {
                if (true != flag)
                {
                    IList<MetricThresholdEntry> thresholds = DataHelper.ManagementService.GetDefaultMetricThresholds(templateID);
                    if (thresholds.Count > 0 && null != thresholds)
                    {
                        if (instanceIDs.Count != 0 && instanceIDs != null)
                        {
                            IEnumerable<int> ids = instanceIDs.Select(instance => instance.Key).ToList();
                            DataHelper.ManagementService.AddAlertTemplate(templateID, ids);
                            DataHelper.ManagementService.ReplaceAlertConfiguration(thresholds, ids);

                            string messageThresholdCount = string.Format("Thresholds found successfully");
                            WriteObject(messageThresholdCount);

                            string alertMsg = string.Empty;
                            if (null != instanceName)
                            {
                                alertMsg = string.Format("Alerts updated successfully for instances: {0}", string.Join(", ", instanceIDs.Select(instance => instance.Value).ToArray()));
                            }
                            else if (null != validTagNames)
                            {
                                alertMsg = string.Format("Alerts updated successfully for tags: {0}", string.Join(", ", validTagNames.Select(tagNames => tagNames).ToArray()));
                            }
                            GetThresholdMessage(alertMsg);
                        }
                    }
                    else
                    {
                        string message = string.Format("Thresholds({0}) found", thresholds.Count);
                        throw new Exception(message);
                    }
                }
            }
            catch (Exception exception)
            {
                WriteObject(exception.Message);
            }
        }

        private void GetThresholdMessage(string message)
        {
            WriteObject(message);
        }

        protected override void StopProcessing()
        {
            base.StopProcessing();
        }
    }
}