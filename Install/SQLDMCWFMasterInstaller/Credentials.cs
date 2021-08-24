using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CWFInstallerService;
using System.Threading;
using System.IO;
using BBS.TracerX;
using System.Text.RegularExpressions;
using System.Net;

namespace Installer_form_application
{
    public partial class Credentials : Form
    {
        private Logger LOG = Logger.GetLogger("Credentials");
        Form screenObject;
        public Credentials(Form screenObj)
        { 
            InitializeComponent();
            AcceptButton = buttonNext;
            CancelButton = buttonCancel;
            MinimizeBox = false;
            MaximizeBox = false;
            screenObject = screenObj;
        }

        private void radioButtonNotRegister_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonNotRegister.Checked)
            {
                properties.notRegister = true;
                ConfigureUIForNoRegisteration();
            }
            else
            {
                properties.notRegister = false;
            }
        }

        private void Credentials_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("Do you wish to cancel installation?", "Cancel", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2))
            {
                Application.Exit();
            }
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            properties.remoteDashboardUrl = textBoxUrl.Text;
            properties.remoteDashboardServiceUsername = textBoxUsername.Text;
            properties.remoteDashboardServicePassword = textBoxPassword.Text;

            if (radioButtonRemote.Checked)
            {
                properties.remoteRegister = true;
                this.Cursor = Cursors.WaitCursor;
                this.buttonNext.Enabled = false;
                this.buttonBack.Enabled = false;
                backgroundWorkerRemote.RunWorkerAsync();
            }
            else
            {
                properties.notRegister = true;
                this.Hide();
                ServiceAccount nextScreen = new ServiceAccount(this);
                nextScreen.Show();
            }
        }
       
        private void buttonBack_Click(object sender, EventArgs e)
        {
            properties.remoteDashboardUrl = textBoxUrl.Text;
            properties.remoteHostname = textBoxUsername.Text;
            properties.remoteDashboardServicePassword = textBoxPassword.Text;
            properties.remoteRegister = radioButtonRemote.Checked;
            properties.notRegister = radioButtonNotRegister.Checked;
            this.Hide();
            screenObject.Show();
        }

        private void radioButtonRemote_CheckedChanged(object sender, EventArgs e)
        {            
            if (radioButtonRemote.Checked)
            {
                properties.remoteRegister = true;
                ConfigureUIForRemoteRegisteration();
                LoadUIForRemoteRegistration();
            } 
            else
                properties.remoteRegister = false;
        }

        private void LoadLabelsForDashbaordVersion()
        {
            if (properties.remoteDashbaordVersion != string.Empty)
            {
                if(IderaDashboardHelper.IsRemoteDashbaordVersionCompatible(properties.remoteDashbaordVersion))
                {
                    labelRemoteDashbaordVersion.Text = "Remote Dashboard Version: " + properties.remoteDashbaordVersion;
                    labelForVersionVerdict.Text = "Compatible version found on remote machine, installation can be proceeded.";
                    labelForVersionVerdict.ForeColor = Color.ForestGreen;                    
                }
                else
                {
                    labelRemoteDashbaordVersion.Text = "Remote Dashboard Version: " + properties.remoteDashbaordVersion;
                    labelForVersionVerdict.Text = "Incompatible version found on remote machine. \nPlease upgrade the dashboard to any version >= " + Properties.Settings.Default.RestrictedIDVersionForRemote;
                    labelForVersionVerdict.ForeColor = Color.Red;
                }
            }
            else
            {
                labelRemoteDashbaordVersion.Text = string.Empty;
                labelForVersionVerdict.Text = string.Empty;
            }
        }

        private void ConfigureLabelsForDashbaordVersion()
        {
            if (properties.remoteDashbaordVersion != string.Empty)
            {
                textBoxUrl.Enabled = false;
                textBoxUsername.Enabled = false;
                textBoxPassword.Enabled = false;
                labelRemoteDashbaordVersion.Visible = true;
                labelForVersionVerdict.Visible = true;
                buttonNext.Enabled = IderaDashboardHelper.IsRemoteDashbaordVersionCompatible(properties.remoteDashbaordVersion);
            }
            else
            {
                textBoxUrl.Enabled = true;
                textBoxUsername.Enabled = true;
                textBoxPassword.Enabled = true;
                labelRemoteDashbaordVersion.Visible = false;
                labelForVersionVerdict.Visible = false;
                buttonNext.Enabled = false;
            }
        }
        private void LoadUIForRemoteRegistration()
        {
            textBoxUrl.Text = properties.remoteDashboardUrl;
            textBoxUsername.Text = properties.remoteDashboardServiceUsername;
            textBoxPassword.Text = properties.remoteDashboardServicePassword;

            if (properties.remoteDashbaordVersion == string.Empty)   //test mode
                buttonTest.Text = "Test Version Compatibility";
            else //edit mode
            {
                buttonTest.Text = "Edit Credentials";                
            }
            LoadLabelsForDashbaordVersion();
        }

        private void ConfigureUIForRemoteRegisteration()
        {
            url.Visible = true;
            username.Visible = true;
            password.Visible = true;
            textBoxUrl.Visible = true;
            textBoxUsername.Visible = true;
            textBoxPassword.Visible = true;
            buttonTest.Visible = true;
            labelRemoteDashbaordVersion.Visible = false;
            labelForVersionVerdict.Visible = false;
            buttonNext.Enabled = false;  //compatible version check is rqd   
            ConfigureLabelsForDashbaordVersion();     
        }

        private void ConfigureUIForNoRegisteration()
        {
            url.Visible = false;
            username.Visible = false;
            password.Visible = false;
            textBoxUrl.Visible = false;
            textBoxUsername.Visible = false;
            textBoxPassword.Visible = false;
            buttonTest.Visible = false;
            labelRemoteDashbaordVersion.Visible = false;
            labelForVersionVerdict.Visible = false;                      
            buttonNext.Enabled = true;
        }

        private void Credentials_Load(object sender, EventArgs e)   //flow would come here only when services are getting installed
        {
            if (properties.notRegister)
            {
                radioButtonNotRegister.Checked = true;
            }
            else
            {
                radioButtonRemote.Checked = true;
            }
        }

        //private void getValuesFromDictionary(Dictionary<string, string> installDetails)
        //{
        //    if (properties.IDAccount == string.Empty)
        //        properties.IDAccount = installDetails.ContainsKey("Account") ? installDetails["Account"] : Environment.MachineName + @"\" + Environment.UserName;
        //    properties.IDServicePort = installDetails.ContainsKey("CorePort") ? installDetails["CorePort"] : "9292";
        //    if (installDetails.ContainsKey("InstallDir"))
        //    {
        //        properties.IDPath = installDetails["InstallDir"];
        //        MessageBox.Show("ID Path: " + properties.IDPath);
        //        Dictionary<string, string> configValues = new Dictionary<string, string>();
        //        try
        //        {
        //            configValues = InstallationHelper.getOldConfigValuesFromLocal(properties.IDPath);
        //        }
        //        catch
        //        {

        //        }

        //        MessageBox.Show("Properties got from config: "+configValues.Count.ToString());
        //        foreach (var i in configValues)
        //        {
        //            MessageBox.Show("From Config: "+i.Key+" -> "+i.Value);
        //        }

        //        if (configValues.ContainsKey("RepositoryHost")) { properties.IDInstance = configValues["RepositoryHost"]; }
        //        if (configValues.ContainsKey("RepositoryDatabase")) { properties.IDDBName = configValues["RepositoryDatabase"]; }
        //        if (configValues.ContainsKey("ServicePort")) { properties.IDServicePort = configValues["ServicePort"]; }
        //        if (configValues.ContainsKey("WebAppMonitorPort")) { properties.WebAppMonitorPort = configValues["WebAppMonitorPort"]; }
                
        //    }
        //    if (installDetails.ContainsKey("ProductVersion"))
        //    {
        //        properties.IDVersion = installDetails["ProductVersion"];
        //        Version versionCurrent = new Version(InstallationHelper.getCurrentVersion());
        //        Version versionExists = new Version(installDetails["ProductVersion"]);
        //        if (versionCurrent <= versionExists)
        //        {
        //            properties.installDashboard = false;
        //        }
        //        else
        //        {
        //            properties.installDashboard = true;
        //        }
        //    }
        //}

        private string ExtractDomainName(string url)
        {            
            Match match = Regex.Match(url, @"http://(.*):([0-9]+)", RegexOptions.IgnoreCase);
            if (match.Success)            
                return match.Groups[1].Value;            
            else           
                throw new Exception("The given url is in incorrect format, Expected format: \"http://Domain_Name:Port_Number\"");            
        }

        private string ExtractCoreServicePortNumber(string url)
        {            
            Match match = Regex.Match(url, @"http://(.*):([0-9]+)", RegexOptions.IgnoreCase);
            if (match.Success)
                return match.Groups[2].Value;
            else
                throw new Exception("The given url is in incorrect format, Expected format: \"http://Domain_Name:Port_Number\"");
        }

        //private void backgroundWorkerLocal_DoWork(object sender, DoWorkEventArgs e)
        //{           
        //    properties.localDashboardUrl = textBoxDashboardUrl.Text;
        //    properties.localDashboardServiceUsername = textBoxIDUserName.Text;
        //    properties.localDashboardServicePassword = textBoxIDPassword.Text;

        //    try
        //    {
        //        if (!IderaDashboardDetails.IsDashboardInstalledOnLocal) //only console is installed
        //            return; 

        //        if (textBoxDashboardUrl.Text != string.Empty && textBoxIDUserName.Text != string.Empty && textBoxIDPassword.Text != string.Empty)
        //        {                                    
        //            Validator.ValidateServiceCredentials(textBoxIDUserName.Text, textBoxIDPassword.Text);
        //            Validator.validateDashboardUrl(textBoxDashboardUrl.Text);
        //        }
        //        else
        //        {
        //            throw new Exception("Please complete all the manadatory fields.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        e.Result = ex.Message;
        //    }
        //}

        private void backgroundWorkerRemote_DoWork(object sender, DoWorkEventArgs e)
        {            
            try
            {
                if (textBoxUrl.Text != string.Empty && textBoxUsername.Text != string.Empty && textBoxPassword.Text != string.Empty)
                {
                    //Dictionary<string, string> installDetails;

                    properties.remoteHostname = ExtractDomainName(properties.remoteDashboardUrl);
                    properties.remoteCoreServicePortNumber = ExtractCoreServicePortNumber(properties.remoteDashboardUrl);
                    //installDetails = InstallationHelper.checkIfDashboardIsInstalledOnRemote(properties.remoteDashboardServiceUsername, properties.remoteDashboardServicePassword, properties.remoteHostname);
                    //if (!Convert.ToBoolean(installDetails["can_connect"]))                    
                    //    throw new Exception("Unable to connect to remote. Please check the credentials.");                    

                    //Validator.ValidateServiceCredentials(properties.remoteDashboardServiceUsername, properties.remoteDashboardServicePassword);
                    //Validator.validateDashboardUrl(properties.remoteDashboardUrl);                    
                }
                else
                {
                    throw new Exception("Please complete all the manadatory fields.");
                }                
            }
            catch (Exception ex)
            {
                e.Result = ex.Message;
            }
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.buttonNext.Enabled = true;
            this.buttonBack.Enabled = true;
            this.Cursor = Cursors.Default;

            if (e.Result != null  && e.Result.ToString() != string.Empty)
            {
                MessageBox.Show(e.Result.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }            

            this.Hide();
            ServiceAccount nextScreen = new ServiceAccount(this);
            nextScreen.Show();
        }

        private void backgroundWorkerFetchRemoteDetails_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                string url = textBoxUrl.Text + @"/IderaCoreServices/v1/Version";
                properties.remoteDashbaordVersion = IderaDashboardHelper.GetResponse(url, properties.remoteDashboardServiceUsername, properties.remoteDashboardServicePassword);
                properties.remoteDashbaordVersion = properties.remoteDashbaordVersion.Replace("\"", string.Empty).Trim();
                LOG.InfoFormat("backgroundWorkerFetchRemoteDetails_DoWork- url: {0},username: {1}, password: {2},remote dashboard version: {3}",
                    url,                    
                    properties.remoteDashboardServiceUsername,
                    properties.remoteDashboardServicePassword,
                    properties.remoteDashbaordVersion);
            }
            catch (Exception ex)
            {
               throw ex;
            }       
                 
            if (properties.remoteDashbaordVersion != string.Empty)           
                e.Result = true;            
            else            
                e.Result = false;             
        }

        private void backgroundWorkerFetchRemoteDetails_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Cursor = Cursors.Default;
            buttonTest.Enabled = true;
            textBoxUrl.Enabled = true;
            textBoxUsername.Enabled = true;
            textBoxPassword.Enabled = true;

            if (e.Error != null)
            {
                if (e.Error is WebException)
                {
                    WebException webEx = (WebException)e.Error;
                    if (webEx.Response != null && webEx.Response is HttpWebResponse && ((HttpWebResponse)webEx.Response).StatusCode == HttpStatusCode.Unauthorized)
                    {
                        LOG.ErrorFormat("backgroundWorkerFetchRemoteDetails_RunWorkerCompleted- Invalid Credentials");
                        MessageBox.Show("Unauthorized(401): Invalid Credentials", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;                        
                    }                    
                }
                LOG.ErrorFormat("backgroundWorkerFetchRemoteDetails_RunWorkerCompleted- Remote not reachable");
                MessageBox.Show("Remote dashboard is not reachable. It may be due to an incorrect format of remote dashboard url, Expected format: http://RemoteMachineName:CoreServicePortNumber", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);                                
                return;
            }

            if (e.Result != null)
            {
                ConfigureUIForRemoteRegisteration();
                LoadUIForRemoteRegistration();                 
            }
        }

        private void buttonTest_Click(object sender, EventArgs e)
        {
            if (properties.remoteDashbaordVersion != string.Empty)//edit mode
            {
                //properties.testRemoteButtonState = true;
                properties.remoteDashbaordVersion = string.Empty;
                ConfigureUIForRemoteRegisteration();
                LoadUIForRemoteRegistration();
            }
            else //Test mode
            {
                //To avoid clicking again and unwanted user interaction on the textboxes, when remote details are getting fetched
                buttonTest.Enabled = false; 
                textBoxUrl.Enabled = false;
                textBoxUsername.Enabled = false;
                textBoxPassword.Enabled = false;

                properties.remoteDashboardUrl = textBoxUrl.Text;
                properties.remoteDashboardServiceUsername = textBoxUsername.Text;
                properties.remoteDashboardServicePassword = textBoxPassword.Text;

                this.Cursor = Cursors.WaitCursor;                
                backgroundWorkerFetchRemoteDetails.RunWorkerAsync();
            }       
        }
    }
}
