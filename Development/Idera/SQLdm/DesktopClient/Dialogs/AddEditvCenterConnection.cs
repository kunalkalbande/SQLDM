using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web.Services.Protocols;
using System.Windows.Forms;

using Idera.SQLdm.Common.VMware;
using Idera.SQLdm.Common.HyperV;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.UI.Dialogs;
using Microsoft.VisualBasic.Logging;
using System.Runtime.InteropServices;
using Idera.SQLdm.Common.Helpers;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    public partial class AddEditvCenterConnection : BaseDialog
    {

        vCenterHosts workingHost = null;
        Dictionary<string, vCenterHosts> currentHosts;
        string oldName;
        string oldAddress;
        string oldUser;
        string oldPassword;
        private static readonly BBS.TracerX.Logger Log = BBS.TracerX.Logger.GetLogger(typeof(AddEditvCenterConnection));

        public AddEditvCenterConnection(vCenterHosts host, Dictionary<string, vCenterHosts> hosts)
        {
            this.DialogHeader = "Virtualization Host Configuration";
            InitializeComponent();

            if (host == null)
                workingHost = new vCenterHosts();
            else
            {
                oldName = host.vcName;
                oldAddress = host.vcAddress;
                oldUser = host.vcUser;
                oldPassword = host.vcPassword;
                workingHost = host;
            }

            currentHosts = hosts;
            if (currentHosts == null)
            {
                currentHosts = new Dictionary<string, vCenterHosts>();
            }
        }

        #region Properties

        #endregion

        #region Event Methods

        private void AddEditvCenterConnection_Load(object sender, EventArgs e)
        {
            vCenterHostsBindingSource.DataSource = workingHost;
            updateControls();

            txtHostName.Focus();
        }

        private void onTextChanged(object sender, EventArgs e)
        {
            updateControls();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            // do the test VM connection in background theath 
            EnableDisableControls(false);
            loadingCircle.Visible = loadingCircle.Active = true;
            this.statusMessage.Text = "Testing VM connection";
            
            bgWorker.RunWorkerAsync();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            workingHost.vcName = oldName;
            workingHost.vcAddress = oldAddress;
            workingHost.vcUser = oldUser;
            workingHost.vcPassword = oldPassword;

            Close();
        }

        private void AddEditvCenterConnection_HelpButtonClicked(object sender, System.ComponentModel.CancelEventArgs e)
        {
            showHelp();
        }

        private void AddEditvCenterConnection_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            showHelp();
        }

        #endregion

        #region Helper Methods

        private void validator_Validating(object sender, Infragistics.Win.Misc.ValidatingEventArgs e)
        {
            if (e.ValidationSettings.ValidationGroup == null)
                return;

            string value = e.Value == null ? null : e.Value.ToString().Trim();

            switch (e.ValidationSettings.ValidationGroup.Key)
            {
                case "vCenterName":
                    if (string.IsNullOrEmpty(value))
                    {
                        e.IsValid = false;
                        e.ValidationSettings.NotificationSettings.Text = "A name for this server is required.";
                        break;
                    }

                    if (txtHostName.Text != oldName)
                    {
                        bool exists = false;
                        string existingAddress = string.Empty;
                        foreach (vCenterHosts host in currentHosts.Values)
                        {
                            if (host.Equals(workingHost))
                                continue;

                            if (host.vcName == txtHostName.Text)
                            {
                                exists = true;
                                existingAddress = host.vcAddress;
                                break;
                            }
                        }
                        if (exists)
                        {
                            e.IsValid = false;
                            e.ValidationSettings.NotificationSettings.Text = string.Format("The server name must be unique. This name is already used for Virtualization Host address {0}.", existingAddress);
                            break;
                        }
                    }

                    e.IsValid = true;
                    break;
                case "vCenterAddress":
                    if (string.IsNullOrEmpty(value))
                    {
                        e.IsValid = false;
                        e.ValidationSettings.NotificationSettings.Text = "A valid address for this server is required.";
                        break;
                    }

                    vCenterHosts existingHost;
                    if (txtHostAddress.Text != oldAddress && currentHosts.TryGetValue(txtHostAddress.Text, out existingHost))
                    {
                        e.IsValid = false;
                        e.ValidationSettings.NotificationSettings.Text = string.Format("A unique address for this server is required. This address is already used for Virtualization Host {0}.", existingHost.vcName);
                        break;
                    }

                    e.IsValid = true;
                    break;
                case "vCenterInfo":
                    e.IsValid = !string.IsNullOrEmpty(value);
                    break;
                default:
                    e.IsValid = true;
                    break;
            }
        
        }

        private void updateControls()
        {
            btnTest.Enabled = 
                btnOK.Enabled = validator.Validate(true, false).IsValid;
            
        }

        private void showHelp()
        {
            ApplicationHelper.ShowHelpTopic(HelpTopics.vCenterConnection);
        }

        public void DisableInput(bool val)
        {
            txtHostName.Enabled =
                txtHostAddress.Enabled =
                txtHostUser.Enabled =
                txtHostPassword.Enabled =
                btnTest.Visible = !val;
            btnOK.Enabled = val;

        }

        #endregion

        /// <summary>
        /// This will executed in a separate thread created by the background worker to test VM connection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bgWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Boolean vceterConnectionFlag = true;
            ServiceUtil vimServiceUtil = new ServiceUtil(workingHost.vcAddress);
            HyperVService hyperVService = null;
            

            //SQLDM-29900
            if (!RegistryHelper.CheckMicrosoftRegistryValues("SchUseStrongCrypto", @"SOFTWARE\Microsoft\.NETFramework\v4.0.30319"))
            {
                RegistryHelper.UpdateMicrosoftRegistryValues("SchUseStrongCrypto", @"SOFTWARE\Microsoft\.NETFramework\v4.0.30319",1);
            }
            if (!RegistryHelper.CheckMicrosoftRegistryValues("SchUseStrongCrypto", @"SOFTWARE\Wow6432Node\Microsoft\.NETFramework\v4.0.30319"))
            {
                RegistryHelper.UpdateMicrosoftRegistryValues("SchUseStrongCrypto", @"SOFTWARE\Wow6432Node\Microsoft\.NETFramework\v4.0.30319", 1);
            }

            try
            {
                vimServiceUtil.Connect(workingHost.vcUser, workingHost.vcPassword);
                e.Result = workingHost.vcAddress;
            }
            catch (Exception exception)
            {
                Log.Debug( string.Format("Unable to connect to {0}", workingHost.vcAddress),
                    exception.ToString());
                e.Result = exception;
                vceterConnectionFlag = false;
            }

            try
            {
                if (vceterConnectionFlag == false)
                {
                    hyperVService = new HyperVService(workingHost.vcAddress);
                    hyperVService.Connect(workingHost.vcUser, workingHost.vcPassword);
                    e.Result = workingHost.vcAddress;
                }
            }
            catch (Exception exp)
            {
                Log.Debug(string.Format("Unable to connect to {0}", workingHost.vcAddress), exp.ToString());
                if (exp.GetType().Name == "COMException")
                {
                    Log.Error("The remote name could not be resolved: '" + workingHost.vcAddress + "'.", exp);
                    e.Result = new COMException("The remote name could not be resolved: '" + workingHost.vcAddress + "'.");
                }
                else if (exp.GetType().Name == "UnauthorizedAccessException")
                {
                    Log.Error("cannot complete login due to an incorrect username or password.", exp);
                    e.Result = new UnauthorizedAccessException("cannot complete login due to an incorrect username or password.");
                }
                else
                {
                    Log.Error("Unable to connect to the remote server.", exp);
                    e.Result = new Exception("Unable to connect to the remote server.");
                }
            }
            finally
            {
                
                try
                {
                    if (vceterConnectionFlag)
                    {
                        vimServiceUtil.Disconnect();
                    }
                    else if (hyperVService != null)
                    {
                        hyperVService.Disconnect();
                    }
                }
                catch (Exception exception)
                {
                    Log.Debug(string.Format("Unable to Disconect to {0}", workingHost.vcAddress),
                        exception.ToString());
                }
            }

        }

        /// <summary>
        ///  This is executed after the task is complete whatever the task has completed: 
        /// a) has been cancelled, b) with Exception c)test connection sucessfully
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bgWorker_RunWorkerCompleted(object sender, 
            System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            loadingCircle.Visible = loadingCircle.Active = false;
            EnableDisableControls(true);
            this.statusMessage.Text = String.Empty;

            if (DialogResult == DialogResult.Cancel || DialogResult == DialogResult.OK)
            {
                // User clicked the Close/ok button before test connection background 
                // operation completed.
                return;
            }
            
            if (e.Result is Exception)
            {
                ApplicationMessageBox.ShowError(this, string.Format("Unable to connect to {0}",
                    workingHost.vcAddress), (e.Result as Exception));
            }
            else 
            {
                //in case the test connection were succesfully
                ApplicationMessageBox.ShowMessage(string.Format("Successfully connected to {0}",
                    e.Result.ToString()));
            }
        }

        /// <summary>
        /// Disable the input text controls and ok button
        /// </summary>
        /// <param name="needEnable"></param>
        private void EnableDisableControls(bool needEnable )
        {
            txtHostName.Enabled = needEnable;
            txtHostAddress.Enabled = needEnable;
            txtHostUser.Enabled = needEnable;
            txtHostPassword.Enabled = needEnable;
            this.btnTest.Enabled = needEnable;
        }
        
    }
}
