using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Idera.SQLdm.DesktopClient.Helpers;
using Wintellect.PowerCollections;

namespace Idera.SQLdm.DesktopClient.Objects
{
    internal sealed class TableActionObject
    {
        #region types

        public enum TableAction
        {
            [Description("Rebuild indexes")]
            RebuildIndexes,
            [Description("Update statistics")]
            UpdateStatistics,
            None
        }

        public enum TableActionStatus
        {
            None,
            Cancelled,
            Scheduled,
            InProcess,
            Successful,
            Failed,
            Ongoing
        }

        #endregion

        #region fields

        #endregion

        #region constructors
        public TableActionObject(string database, int tableId, string tableName, decimal? percentFragmentation, bool selected)
        {
            UniqueIdentifier = new Pair<string, int>(database, tableId);
            DatabaseName = database;
            TableId = tableId;
            TableName = tableName;
            PercentFragmentation = percentFragmentation;
            Selected = selected;
        }

        #endregion

        #region properties

        public bool Selected;
        public string DatabaseName;
        public int TableId;
        public string TableName;
        public decimal? PercentFragmentation;

        public Pair<string, int> UniqueIdentifier;

        public DateTime? ActionTime = null;
        public TableAction Action = TableAction.None;
        public TableActionStatus Status = TableActionStatus.None;
        public Exception ActionError = null;

        public string FragmentationMessage
        {
            get
            {
                string message = null;

                switch (Status)
                {
                    case TableActionStatus.Scheduled:
                        message = "Rebuild pending";
                        break;
                    case TableActionStatus.InProcess:
                        message = "Rebuilding";
                        break;
                    case TableActionStatus.Successful:
                        message = PercentFragmentation.HasValue ?
                                        PercentFragmentation.Value.ToString("0.00%") :
                                        "Rebuilt";
                        break;
                    case TableActionStatus.Ongoing:
                        message = "Rebuild started";
                        break;
                    case TableActionStatus.Failed:
                        message = "Rebuild failed";
                        break;
                    case TableActionStatus.Cancelled:
                        message = PercentFragmentation.HasValue ?
                                        PercentFragmentation.Value.ToString("0.00%") :
                                        string.Empty;
                        break;
                    default:
                        message = string.Empty;
                        break;
                }

                return message;
            }
        }

        public string StatusMessage
        {
            get
            {
                string message = null;

                switch (Status)
                {
                    case TableActionStatus.Scheduled:
                        message = string.Concat(ApplicationHelper.GetEnumDescription(Action), " pending");
                        break;
                    case TableActionStatus.InProcess:
                        message = string.Concat(ApplicationHelper.GetEnumDescription(Action), " running");
                        break;
                    case TableActionStatus.Ongoing:
                        message = string.Format("A {0} has been started for this table, but the process is running for a long time and will not be monitored any further.",
                                                    ApplicationHelper.GetEnumDescription(Action));
                        break;
                    case TableActionStatus.Successful:
                        message = string.Format("{0} completed at {1}",
                                                ApplicationHelper.GetEnumDescription(Action),
                                                ActionTime);
                        break;
                    case TableActionStatus.Failed:
                        message = string.Format("{0} failed. {1}",
                                                ApplicationHelper.GetEnumDescription(Action),
                                                ActionError.Message);
                        break;
                    case TableActionStatus.Cancelled:
                        if (PercentFragmentation.HasValue && ActionTime.HasValue)
                        {
                            message = string.Format("{0} completed at {1}",
                                ApplicationHelper.GetEnumDescription(Action),
                                ActionTime);
                        }
                        else
                        {
                            message = string.Concat(ApplicationHelper.GetEnumDescription(Action), " cancelled");
                        }
                        break;
                    default:
                        message = null;
                        break;
                }

                return message;
            }
        }

        #endregion
    }
}