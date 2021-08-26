using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using BBS.License;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.DesktopClient.Properties;
using Idera.SQLdm.DesktopClient.Helpers;
using Microsoft.SqlServer.MessageBox;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common;
using System.Security.Cryptography;
using BBS.TracerX;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.Common.Configuration;
using Microsoft.Win32;

namespace Idera.SQLdm.DesktopClient.Dialogs {
    using System.Data.SqlClient;

    internal partial class LicenseDialog : Form, IAuditable 
    {

        /// <summary>
        /// This static method displays the license dialog and returns the DialogResult.
        /// It returns DialogResult.Abort if the dialog could not be shown (e.g. could
        /// not get current licenses from server).
        /// Specify null for repositoryConnectionInfo to use the repository currently in global Settings.
        /// </summary>
        internal static DialogResult Display(IWin32Window parent, SqlConnectionInfo repositoryConnectionInfo) {
            using (Log.InfoCall()) {
                LicenseDialog dlg = null;
                try
                {
                    dlg = new LicenseDialog(repositoryConnectionInfo);
                }
                catch (Exception ex)
                {
                    ApplicationMessageBox.ShowError(parent,
                                                    "Unable to retrieve the default management service from the SQLDM Repository.  Please resolve the following error and try again.",
                                                    ex);
                    return DialogResult.Cancel;
                }
                using (dlg)
                {
                    return dlg.ShowDialog(parent);
                }
            }
        }

        internal static DialogResult Display(IWin32Window parent) {
            return Display(parent, null);
        }

        // The SQL connection and management service we use are not necessarily the ones in
        // the Settings since we may be in the middle of switching repositories.
        private readonly SqlConnectionInfo _repositoryConnectionInfo;
        private readonly IManagementService _managementService;

        private LicenseSummary _summary;
        private readonly string _totalLicensedFormatString;
        private readonly string _monitoredServersFormatString;

        private static readonly BBS.TracerX.Logger Log = BBS.TracerX.Logger.GetLogger(typeof(LicenseDialog));

        private LicenseDialog(SqlConnectionInfo repositoryConnectionInfo) {
            InitializeComponent();

            if (repositoryConnectionInfo == null) {
                // Use the repository connection and management service currenly used by
                // the rest of the application.
                Log.Debug("repositoryConnectionInfo is null, will use default from Settings.");
                _managementService = ManagementServiceHelper.IManagementService;
            } else {
                // This case means we are about to manage licenses for a repository other than
                // the one the rest of the application is currently using.  Remember the connection
                // info and get the IManagementService for the specified repository without changing
                // the default used by the rest of the app.
                Log.DebugFormat("repositoryConnectionInfo is NOT null.  Instance = {0}, database = {1}.", repositoryConnectionInfo.InstanceName, repositoryConnectionInfo.DatabaseName);
                _repositoryConnectionInfo = repositoryConnectionInfo;
                _managementService = ManagementServiceHelper.GetDefaultService(repositoryConnectionInfo, false);
            }

            _totalLicensedFormatString = labelLicensed.Text;
            _monitoredServersFormatString = labelMonitored.Text;
            GetCurrentLicensesAsync();
            AdaptFontSize();
        }

        // Get the current licenses from the Management Service and 
        // load them into the listview.
        // If there is an error gettig current keys from the DB, display
        // an error message and return null to indicate the operation failed.
        private void GetCurrentLicensesAsync() {
            using (Log.InfoCall()) {
                progressControl.Active = true;
                progressControl.Visible = true;
                bgWorker.RunWorkerAsync();
            }
        }
        
        // Perform an add or replace operation.
        // The only scenario where multiple keys are allowed (i.e. an ADD)
        // is when they are ALL permanent production keys.
        // Otherwise, the new key must replace ALL existing keys.
        private void AddOrReplaceKey(BBSLic lic, string key) 
        {
            using (Log.InfoCall()) 
            {
                LicenseKeyOperation keyOp = LicenseKeyOperation.Add;
                
                if (lic.IsTrial && !_summary.IsTrial) 
                {
                    ApplicationMessageBox.ShowError(this, "A trial key cannot be entered when a production key exists.");
                    return;
                }

                if (listView1.Items.Count > 0 && (lic.IsTrial || !lic.IsPermanent)) 
                {
                    // There can only be one such key.
                    keyOp = LicenseKeyOperation.Replace;
                }

                // Check for duplicate or incompatible keys.
                foreach (ListViewItem item in listView1.Items) 
                {
                    CheckedKey listObject = (CheckedKey)item.Tag;
                    if (LicenseSummary.KeysAreEqual(key, listObject.KeyString))
                    {
                        // duplicate keys
                        ApplicationMessageBox.ShowError(this, "The specified key is the same as an existing key.");
                        return;
                    }
                    else if (listObject.KeyObject == null || listObject.KeyObject.IsTrial || !listObject.KeyObject.IsPermanent)
                    {
                        keyOp = LicenseKeyOperation.Replace;
                    }
                }

                // If this key must replace the others, confirm that the user wants to do that. 
                if (keyOp == LicenseKeyOperation.Replace) {
                    ExceptionMessageBox emb = new ExceptionMessageBox(string.Format("Replace existing license key(s) with new key?\n\nKey: {0}", key));
                    emb.Caption = Application.ProductName;
                    emb.Buttons = ExceptionMessageBoxButtons.YesNo;
                    if (DialogResult.Yes != emb.Show(null)) {
                        Log.Info("User declined to replace existing licenses with the new one.");
                        return;
                    }
                }

                try 
                {
                    //to audit licenses replace
                    SetAuditableEntity(keyOp, key, lic.Limit1.ToString());

                    // Send the new license to the server.
                    Log.Info("KeyOp is " + keyOp);
                    LicenseSummary license = _managementService.SetLicenseKeys(keyOp, new string[] { key });
                    Log.Info(keyOp, " operation succeeded");
                    ShowLicense(license);
                    textBox1.Text = string.Empty;

                    if (!lic.IsTrial)
                    {
                        //We need to mark the desktop client machine as well as the management service machine
                        RegistryKey rk = null;
                        RegistryKey rks = null;

                        rk = Registry.LocalMachine;
                        rks = rk.CreateSubKey(@"Software\Idera\SQLdm");
                        rks.SetValue("ConfigInfo", 1, RegistryValueKind.DWord);

                        if (rks != null)
                            rks.Close();
                        rks = null;

                        if (rk != null)
                            rk.Close();
                        rk = null;
                    }
                   
                } 
                catch (Exception ex) 
                {
                    string msg = string.Format("The following error occurred while sending the new license key to Management Service.");
                    ApplicationMessageBox.ShowError(this, msg, ex);
                    return;
                }
            }
        }

        private static byte[] LicensePW() 
        {
                Process currentProcess = Process.GetCurrentProcess();
                string data = currentProcess.MachineName + currentProcess.Id;
                return BBSLic.GetHash(data);
        }

        private void textBox1_TextChanged(object sender, EventArgs e) {
            buttonAdd.Enabled = textBox1.Text != string.Empty;
        }

        // Load the key string into a BBSLic object and check the return code. 
        private BBSLic LoadKeyString(string key) {
            BBSLic lic = new BBSLic();
            LicErr licErr = lic.LoadKeyString(key);

            switch (licErr) {
                case LicErr.OK:
                    return lic;
                case LicErr.FutureKey:
                    ApplicationMessageBox.ShowError(this, string.Format("The specified key has a creation date of {0}.  Keys with future creation dates are not allowed.\n\nKey: {1}", lic.CreationDate, key));
                    return null;
                default:
                    Log.Info("BBSLic failed to parse license key.  Error: " + licErr);
                    ApplicationMessageBox.ShowError(this, "The specified license key is invalid.");
                    return null;
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e) {
            // First attempt to load the key into a new BBSLic object.
            using (Log.InfoCall()) {
                try {
                    this.Cursor = Cursors.WaitCursor;
                    string key = textBox1.Text.Trim();
                    Log.Info("User entered license key " + key);
                    BBSLic lic = LoadKeyString(key);

                    if (lic != null) {
                        // Key was loaded successfully.  Check for invalid properties.
                        if (lic.ProductID != Common.Constants.ProductId) 
                        {
                            ApplicationMessageBox.ShowError(this, string.Format("The specified key is for a different product.\n\nKey: {0}", key));
                            return;
                        }
                        //Start: SQL DM 9.0 (Vineet Kumar) (License Changes) -- Adding check for license key version. Entered key should be for >-9.0 version or it should be trial
                        else if (!lic.IsTrial && lic.ProductVersion.Major < 9)
                        {
                            ApplicationMessageBox.ShowError(this, string.Format("The license key is for an older version of SQL Diagnostic Manager.  Please visit the customer portal to acquire a new license key at " + Common.Constants.CustomerPortalLink + ". \n\nKey: {0}", key));
                            return;
                        }
                        //End: SQL DM 9.0 (Vineet Kumar) (License Changes) -- Adding check for license key version. Entered key should be for >-9.0 version or it should be trial
                        else if (lic.IsExpired) 
                        {
                            ApplicationMessageBox.ShowError(this, string.Format("The specified key has already expired.\n\nKey: {0}", key));
                            return;
                        } 
                        else if (!lic.CheckScopeHash(_summary.Repository)) 
                        {
                            ApplicationMessageBox.ShowError(this, string.Format("The specified key cannot be used with repository {0}.\n\nKey: {1}", _summary.Repository, key));
                            return;
                        } 
                        else if (!LicenseSummary.IsLicenseReasonable(lic)) 
                        {
                            ApplicationMessageBox.ShowError(this, string.Format("The specified key is invalid.\n\nKey: {0}", key));
                            return;
                        } 
                        else 
                        {
                            AddOrReplaceKey(lic, key);
                        }
                    }
                } 
                finally 
                {
                    this.Cursor = Cursors.Default;
                    this.UseWaitCursor = false;
                }
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e) {
            buttonRemove.Enabled = (listView1.SelectedItems.Count > 0);
        }

        private void buttonRemove_Click(object sender, EventArgs e) {
            using (Log.InfoCall()) {
                try {
                    this.Cursor = Cursors.WaitCursor;
                    List<string> keys = new List<string>();
                    Dictionary<String, String> licensesServerNumber = new Dictionary<string, string>();

                    foreach (ListViewItem item in listView1.SelectedItems) {
                        CheckedKey listObject = (CheckedKey)item.Tag;
                        keys.Add(listObject.KeyString);
                        licensesServerNumber.Add(listObject.KeyString, listObject.KeyObject.Limit1.ToString());
                    }

                    ApplicationMessageBox box = new ApplicationMessageBox();
                    box.Text = "Are you sure you want to remove the selected license key(s)?";
                    box.Buttons = ExceptionMessageBoxButtons.YesNo;
                    if (box.Show(this) == DialogResult.Yes) {
                        try {
                            //to audit licenses delete
                            foreach (var licenses in licensesServerNumber)
                            {
                                SetAuditableEntity(LicenseKeyOperation.Remove, licenses.Key, licenses.Value);
                            }

                            LicenseSummary license = _managementService.SetLicenseKeys(LicenseKeyOperation.Remove, keys);
                            Log.InfoFormat("Removed selected keys");
                            listView1.SelectedIndices.Clear();
                            ShowLicense(license);

                        } catch (Exception ex) {
                            string msg2 = string.Format("An error occurred while removing selected license keys via the Management Service.");
                            ApplicationMessageBox.ShowError(this, msg2, ex);
                        }
                    }
                } finally {
                    this.Cursor = Cursors.Default;
                }
            }
        }

        private void bgWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e) {
            if (System.Threading.Thread.CurrentThread.Name == null) System.Threading.Thread.CurrentThread.Name = "LicenseDialogWorker";
            using (Log.DebugCall()) {
                // This logs all the keys, checks them for errors, and adds up the limits.
                e.Result = RepositoryHelper.GetLicenseKeys(_repositoryConnectionInfo);
            }
        }

        private void bgWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e) {
            using (Log.DebugCall()) {
                listView1.Items.Clear();
                progressControl.Active = false;
                progressControl.Hide();

                if (DialogResult == DialogResult.Cancel) {
                    // User clicked the Close button before the background operation completed.
                    return;
                } else if (e.Error != null) {
                    ApplicationMessageBox.ShowError(this, e.Error);
                    Close();
                } else {
                    LicenseSummary license = (LicenseSummary)e.Result;
                    ShowLicense(license);
                }
            }
        }

        // Configure the dialog to show the specified license information.
        private void ShowLicense(LicenseSummary license) {
            _summary = license;

            if (_summary.LicensedServers == BBSLic.Unlimited)
                labelLicensed.Text = string.Format(_totalLicensedFormatString, "Unlimited");
            else
                labelLicensed.Text = string.Format(_totalLicensedFormatString, _summary.LicensedServers);

            labelMonitored.Text = string.Format(_monitoredServersFormatString, _summary.MonitoredServers);
            labelRepository.Text = string.Format(labelRepository.Text, _summary.Repository);

            labelRtvLicInfo.Visible = false;
            labelLicensed.Visible = labelMonitored.Visible = labelRepository.Visible = true;

            listView1.Items.Clear();
            foreach (CheckedKey key in _summary.CheckedKeys) {
                ListViewItem item = new ListViewItem(GetLicenseDisplayStrings(key));
                item.Tag = key;
                listView1.Items.Add(item);
            }

        }

        // Make a string[] from a BBSLic object.
        // Each element in the returned array corresponds to a column on the LicenseDialog's ListView.
        // No error checking is done here.
        private static string[] GetLicenseDisplayStrings(CheckedKey checkedKey) {
            string[] cols = new string[5];
            BBSLic keyObject = checkedKey.KeyObject;

            if (keyObject != null) {
                if (keyObject.IsTrial) {
                    cols[0] = Idera.SQLdm.DesktopClient.Properties.Resources.KeyType_Trial;
                } else {
                    cols[0] = Idera.SQLdm.DesktopClient.Properties.Resources.KeyType_Production;
                }

                if (keyObject.Limit1 == BBSLic.Unlimited) {
                    cols[1] = Idera.SQLdm.DesktopClient.Properties.Resources.KeyLimit_Unlimited;
                } else {
                    cols[1] = keyObject.Limit1.ToString();
                }

                if (keyObject.IsPermanent) {
                    cols[2] = Idera.SQLdm.DesktopClient.Properties.Resources.KeyExpires_Never;
                } else {
                    cols[2] = keyObject.ExpirationDate.ToShortDateString();
                }
            }

            if (!checkedKey.IsValid) cols[3] = checkedKey.Comment;

            cols[4] = checkedKey.KeyString;

            return cols;
        }

        private void LicenseDialog_HelpButtonClicked(object sender, System.ComponentModel.CancelEventArgs e) {
            if (e != null) e.Cancel = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.Licensing);
        }

        private void LicenseDialog_HelpRequested(object sender, HelpEventArgs hlpevent) {
            if (hlpevent != null) hlpevent.Handled = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.Licensing);

        }

        private void linkLabelManageServers_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ManageServersDialog msd = new ManageServersDialog();
            msd.ShowDialog(this);
        }

        private void buyNow_Click(object sender, EventArgs e)
        {
            ApplicationController.Default.LaunchProductPurchaseUrl();
        }

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }

        /// <summary>
        /// Sets an auditable entity to put it on the context
        /// </summary>
        /// <param name="keyOp"></param>
        /// <param name="key"></param>
        /// <param name="totalKeys"></param>
        private void SetAuditableEntity(LicenseKeyOperation keyOp, string key, string totalKeys)
        {
            AuditableEntity auditableEntity = GetAuditableEntity();
            auditableEntity.AddMetadataProperty("License Key", key);
            auditableEntity.AddMetadataProperty("Total number of servers that license supports", totalKeys == "-1" ? "Unlimited" : totalKeys);
            auditableEntity.AddMetadataProperty("License operations", keyOp.ToString());

            AuditingEngine.SetContextData(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);
            AuditingEngine.SetAuxiliarData("LicenseEntity", auditableEntity);
        }

        /// <summary>
        /// Get  an Auditable Entity from Tag
        /// </summary>
        /// <returns> Returns an Auditable Entity</returns>
        public AuditableEntity GetAuditableEntity()
        {

            AuditableEntity auditableEntity = new AuditableEntity();
            auditableEntity.Name = "License Key";

            return auditableEntity;
        }

        /// <summary>
        /// Get  an Auditable Entity from Tag
        /// </summary>
        /// <param name="oldValue"></param>
        /// <returns> Returns an Auditable Entity</returns>
        public AuditableEntity GetAuditableEntity(IAuditable oldValue)
        {
            return GetAuditableEntity();
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
                GetCurrentLicensesAsync();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(Common.Constants.CustomerPortalLink);
        }
    }
}
