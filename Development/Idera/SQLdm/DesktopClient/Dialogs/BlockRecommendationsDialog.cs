using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using Microsoft.SqlServer.MessageBox;
using Microsoft.Win32;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Resources;
using Idera.SQLdm.Common;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.DesktopClient.Properties;
using Wintellect.PowerCollections;
using Idera.SQLdm.DesktopClient.Dialogs;

namespace Idera.SQLdm.StandardClient.Dialogs
{
    public partial class BlockRecommendationsDialog : BaseDialog
    {
        private int _instanceID;
        public bool isModified = false;

        private static List<string> blockedRecommendationID;
        private static List<int> blockedDatabaseIDList;
        private Dictionary<int, string> allDatabaseMasterINfo;

        public static List<int> BlockedDatabaseIDList
        {
            get { return blockedDatabaseIDList; }
            set { blockedDatabaseIDList = value; }
        }
        public static List<string> BlockedRecommendationID
        {
            get { return blockedRecommendationID; }
            set { blockedRecommendationID = value; }
        }
        public Dictionary<int, string> AllDatabaseMasterINfo
        {
            get { return allDatabaseMasterINfo; }
            set { allDatabaseMasterINfo = value; }
        }
        private BlockRecommendationsDialog()
        {
            this.DialogHeader = "Block Objects";
            InitializeComponent();

        }
        private BlockRecommendationsDialog(int instanceId)
            : this()
        {
            this.DialogHeader = "Block Objects";
            _instanceID = instanceId;
            Triple<Dictionary<int, string>, List<int>, List<string>> dataFromDB = RepositoryHelper.GetBlockedRecommendationDatabaseAnalysisConfiguration(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, _instanceID);
            if (dataFromDB != null)
            {
                AllDatabaseMasterINfo = dataFromDB.First == null ? new Dictionary<int, string>() : dataFromDB.First;
                blockedDatabaseIDList = dataFromDB.Second == null ? new List<int>() : dataFromDB.Second;
                blockedRecommendationID = dataFromDB.Third == null ? new List<string>() : dataFromDB.Third;
            }
        }
        public BlockRecommendationsDialog(int instanceId, IRecommendation r)
            : this(instanceId)
        {
            List<IRecommendation> l = new List<IRecommendation>(1);
            l.Add(r);
            BuildView(l);
        }

        public BlockRecommendationsDialog(int instanceId, IEnumerable<IRecommendation> r)
            : this(instanceId)
        {
            _instanceID = instanceId;
            BuildView(r);
        }

        private void BuildView(IEnumerable<IRecommendation> l)
        {
            btnBlockSelected.Enabled = false;
            lvRecommendationTypes.Items.Clear();
            lvDatabases.Items.Clear();
            if (null == l) return;
            List<string> dbs = new List<string>();
            List<string> ids = new List<string>();
            IList<IRecommendation> g;
            foreach (IRecommendation i in l)
            {
                if (null == i) continue;

                AddTypeToList(ids, i);

                AddDatabaseToList(dbs, i as IProvideDatabase);
                g = i as IList<IRecommendation>;
                if (null != g)
                {
                    foreach (IRecommendation r in g)
                    {
                        AddTypeToList(ids, r);
                        AddDatabaseToList(dbs, r as IProvideDatabase);
                    }
                }
            }
            ListViewItem item;
            foreach (string s in dbs)
            {
                item = new ListViewItem(s);
                item.Tag = GetDatabaseID(s);
                item.Checked = blockedDatabaseIDList.Contains(Convert.ToInt32(item.Tag));
                lvDatabases.Items.Add(item);
            }
            foreach (string id in ids)
            {
                item = new ListViewItem(MasterRecommendations.GetDescription(id));
                item.Tag = id;
                item.Checked = blockedRecommendationID.Contains(item.Tag.ToString());
                lvRecommendationTypes.Items.Add(item);
            }
            splitContainer1.Panel1Collapsed = lvRecommendationTypes.Items.Count <= 0;
            splitContainer1.Panel2Collapsed = lvDatabases.Items.Count <= 0;
        }
        private int GetDatabaseID(string databaseName)
        {
            int databaseID = 0;
            try
            {
                var ID = allDatabaseMasterINfo.FirstOrDefault(x => x.Value == databaseName).Key;
                databaseID = Convert.ToInt32(ID);
            }
            catch (Exception)
            {
                ApplicationMessageBox.ShowError(this.Owner, "Error while getting database records.");
            }
            return databaseID;
        }

        private void AddTypeToList(List<string> ids, IRecommendation i)
        {
            if (null == i) return;
            if (null == ids) return;
            if (!string.IsNullOrEmpty(i.ID)) if (!ids.Contains(i.ID)) ids.Add(i.ID);
        }

        private void AddDatabaseToList(List<string> dbs, IProvideDatabase ipd)
        {
            if (null != ipd)
            {
                if (!string.IsNullOrEmpty(ipd.Database))
                {
                    if (!dbs.Contains(ipd.Database)) dbs.Add(ipd.Database);
                }
            }
        }

        private void lvDatabases_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            UpdateButtonState();
        }

        private void lvRecommendationTypes_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            UpdateButtonState();
        }

        private void UpdateButtonState()
        {
            //if (null != lvDatabases.CheckedItems) if (lvDatabases.CheckedItems.Count != blockedDatabaseIDList.Count) enabled = true;
            //if (null != lvRecommendationTypes.CheckedItems) if (lvRecommendationTypes.CheckedItems.Count != blockedRecommendationID.Count) enabled = true;

            if (isModified != btnBlockSelected.Enabled) btnBlockSelected.Enabled = isModified;
            isModified = true;
        }

        private void btnBlockSelected_Click(object sender, EventArgs e)
        {
            //if (Idera.SQLdoctor.Common.License.LicenseManager.IsLimitedTrialMode)
            //{
            //    ApplicationMessageBox.ShowError(this, "Blocking is not allowed for trial.");
            //    return;
            //}
            try
            {
                blockedRecommendationID.Clear();
                blockedDatabaseIDList.Clear();
                if (null != lvRecommendationTypes.CheckedItems)
                {
                    foreach (ListViewItem item in lvRecommendationTypes.CheckedItems)
                    {
                        if (!blockedRecommendationID.Contains(item.Tag.ToString()))
                        {
                            blockedRecommendationID.Add(item.Tag.ToString());
                        }
                    }
                }
                if (null != lvDatabases.CheckedItems)
                {
                    foreach (ListViewItem item in lvDatabases.CheckedItems)
                    {
                        if (!blockedDatabaseIDList.Contains(Convert.ToInt32(item.Tag.ToString())))
                        {
                            blockedDatabaseIDList.Add(Convert.ToInt32(item.Tag.ToString()));
                        }
                    }
                }

                //if (blockedDatabaseIDList.Count > 0 || blockedRecommendationID.Count > 0)
                //{
                IManagementService managementService =
                            ManagementServiceHelper.GetDefaultService(
                                Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
                if (managementService != null)
                {
                    //recommendationsList = managementService.GetRecommendations(InstanceId, DateTime.Now.AddDays(2));
                    var result = managementService.BlockRecommendationDatabaseAnalysisConfiguration(_instanceID, blockedRecommendationID, blockedDatabaseIDList);
                    if (result != null && (bool)result)
                    {
                        ApplicationMessageBox.ShowInfo(this.Owner, "Recommendations/Databases blocked successfully.");
                    }
                    else
                    {
                        ApplicationMessageBox.ShowError(this.Owner, "Could not block Recommendations/Databases.");
                    }
                }
                //}
            }
            catch (Exception)
            {
                ApplicationMessageBox.ShowError(this.Owner, "Error while blocking Recommendations/Databases.");
            }
        }

        private void BlockDialog_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            //HelpTopics.ShowHelpTopic(HelpTopics.BlockObjects);
        }

    }
}