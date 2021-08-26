using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using Idera.SQLdm.Common.UI.Dialogs;
using TracerX;
using Idera.SQLdm.Common;
using Idera.SQLdm.DesktopClient.Presenters.GridEntries;
using Idera.SQLdm.DesktopClient.Properties;
using Idera.SQLdm.DesktopClient.Helpers;

namespace Idera.SQLdm.DesktopClient.Dialogs.Analysis
{
    public partial class ServerSettingsDialog : Form
    {
        private bool _loading = false;
        private int instnceID;
        private int includeDatabase;
        private string filterApplicationText;
        private List<int> blockedDatabase;
        private List<string> blockedRecommendation;
        private static readonly Logger Log = Logger.GetLogger("ServerSettingsDialog");
        public static List<DatabaseInformation> Databases = new List<DatabaseInformation>();

        public delegate void ConnectionSettingsChangedHandler(string instance);

        public enum ServerSettingsTab
        {
            Filters,
            BlockRecommendations,
            BlockDatabases
        }

        public ServerSettingsDialog(int instnceID, int filterDatabase, string filterApplication, List<int> databases, List<string> recommendations)
        {
            this.instnceID = instnceID;
            this.includeDatabase = filterDatabase;
            this.filterApplicationText = filterApplication;
            this.blockedDatabase = databases;
            this.blockedRecommendation = recommendations;
            //Moving code here from BlockedDatabaseTab and FilterDatabase tab to make only one DB call
            //Appending null check for Settings.Default.ActiveRepositoryConnection
            if (Settings.Default == null || Settings.Default.ActiveRepositoryConnection == null || Settings.Default.ActiveRepositoryConnection.ConnectionInfo == null)
            {
                Log.Error("No databases found because, Could not get Connection Info of active repository. Object 'Settings.Default.ActiveRepositoryConnection.ConnectionInfo' value is null.");
                Databases = new List<DatabaseInformation>();
            }
            else
                Databases = RepositoryHelper.GetDatabasesForInstance(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, this.instnceID);
            
            InitializeComponent();            
        }
        
        private void SelectTab(ServerSettingsTab t)
        {
            switch (t)
            {
                case ServerSettingsTab.Filters: { tabSettings.SelectedTab = tabFilters; break; }
                case ServerSettingsTab.BlockDatabases: { tabSettings.SelectedTab = tabBlockDatabases; break; }
                case ServerSettingsTab.BlockRecommendations: { tabSettings.SelectedTab = tabBlockRecommendations; break; }
            }
        }

        private void _serverComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading) return;
            if (IsModified())
            {
                DialogResult dr = ApplicationMessageBox.ShowQuestion(this, "Changes have been made.\n\nWould you like to save these changes before switching to a new server?", Microsoft.SqlServer.MessageBox.ExceptionMessageBoxButtons.YesNoCancel, Microsoft.SqlServer.MessageBox.ExceptionMessageBoxDefaultButton.Button1);
                switch (dr)
                {
                    case DialogResult.Yes:
                        {
                            if (!SaveChanges())
                            {
                                return;
                            }
                            break;
                        }
                    case DialogResult.No:
                        {
                            break;
                        }
                    case DialogResult.Cancel:
                        {
                            return;
                        }
                }
            }
        }


        private bool SaveChanges()
        {
            try
            {
                blockedRecommendations.SaveChanges();
            }
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError(this, ex);
                SelectTab(ServerSettingsTab.BlockRecommendations);
                Log.Info("Advance analysis configurations not saved.");
                return (false);
            }
            try
            {
                blockedDatabases.SaveChanges();
            }
            catch (ApplicationException ex)
            {
                Log.Info("Advance analysis configurations not saved.");
                return (false);
            }
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError(this, ex);
                SelectTab(ServerSettingsTab.BlockDatabases);
                Log.Info("Advance analysis configurations not saved.");
                return (false);
            }
            try
            {
                if (!filtersSettings.SaveChanges())
                {
                    SelectTab(ServerSettingsTab.Filters);
                    Log.Info("Advance analysis configurations not saved.");
                    return (false);
                }
            }
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError(this, ex);
                SelectTab(ServerSettingsTab.Filters);
                Log.Info("Advance analysis configurations not saved.");
                return (false);
            }
            okButton.Enabled = false;
            Log.Info("Advance analysis configurations saved.");
            return (true);
        }

        private void blockedRecommendations_SettingsChanged(object sender, EventArgs e)
        {
            OnSettingsChanged();
        }

        private void OnSettingsChanged()
        {
            okButton.Enabled = true;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (!SaveChanges()) DialogResult = DialogResult.None;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {

        }

        private void ServerSettingsDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (IsModified() && ((CloseReason.UserClosing == e.CloseReason) || (CloseReason.None == e.CloseReason)))
            //{
            //    DialogResult dr = ApplicationMessageBox.ShowQuestion(this, "Changes have been made.\n\nWould you like to save these changes before exiting?", Microsoft.SqlServer.MessageBox.ExceptionMessageBoxButtons.YesNoCancel, Microsoft.SqlServer.MessageBox.ExceptionMessageBoxDefaultButton.Button1);
            //    switch (dr)
            //    {
            //        case DialogResult.Yes: { if (!SaveChanges()) e.Cancel = true; break; }
            //        case DialogResult.No:{break;}
            //        case DialogResult.Cancel: { e.Cancel = true; break; }
            //    }
            //}
        }

        private bool IsModified()
        {
            return (blockedRecommendations.Modified || filtersSettings.Modified || blockedDatabases.Modified);
        }

        private void filtersSettings_SettingsChanged(object sender, EventArgs e)
        {
            OnSettingsChanged();
        }

        private void blockedDatabases_SettingsChanged(object sender, EventArgs e)
        {
            OnSettingsChanged();
        }
        private void ServerSettingsDialog_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            ShowHelp();
        }

        private void ServerSettingsDialog_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            ShowHelp();
        }

        private void ShowHelp()
        {
            ///Updated wiki link for server settings-- SQLdm 10.0 (Ankit Nagpal)
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.ServerSettings);
        }

    }
}