using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using BBS.License;
using BBS.TracerX;

namespace Installer_form_application
{
    //SQLDM 10.1 (Barkha Khatri) -- added license screen for showing existing license info and getting new license keys
    public partial class LicenseScreen : Form
    {
        Form backScreenObject;
        LicenseSummary licenseSummary;
        const string CopyExistingLicenseInfoFormat = "Existing License Key(s) : {0} \n SQLdm Repository : {1} \n Total Licenses : {2} \n Monitored Instances : {3}";
        private Logger LOG = Logger.GetLogger("LicenseScreen");
        public LicenseScreen(Form backScreenObj)
        {
            InitializeComponent();
            MinimizeBox = false;
            MaximizeBox = false;
            backScreenObject = backScreenObj;
        }

        public LicenseScreen(Form backScreenObj,LicenseSummary license)
        {
            InitializeComponent();
            MinimizeBox = false;
            MaximizeBox = false;
            backScreenObject = backScreenObj;
            licenseSummary=license;
        }


        private void LicenseScreen_Load(object sender, EventArgs e)
        {
            string monitoredInstances, totalLicenses, sqldmRepository;
            totalLicenses = licenseSummary.LicensedServers.ToString();
            monitoredInstances = licenseSummary.MonitoredServers.ToString();
            string keys = String.Empty;
            foreach (var key in licenseSummary.CheckedKeys)
            {
                if (!string.IsNullOrEmpty(keys))
                    keys += ";";
                keys += key.KeyString;
            }
            sqldmRepository = licenseSummary.Repository;
            existingLicenseKeyTextbox.Text = keys;
            sqldmRepoTextbox.Text = sqldmRepository;
            totalLicensesTextbox.Text = totalLicenses;
            monitoredInstancesTextbox.Text = monitoredInstances;

        }


        
        private void back_Click(object sender, EventArgs e)
        {
            this.Hide();
            backScreenObject.Show();
        }

        private void next_Click(object sender, EventArgs e)
        {
            this.next.Enabled = false;
            this.back.Enabled = false;
            this.Cursor = Cursors.WaitCursor;

            string error;
            string keys=ValidateNewLicenseKeys(newLicenseKeyTextbox.Text, out error);

            this.next.Enabled = true;
            this.back.Enabled = true;
            this.Cursor = Cursors.Default;

            if (error.Length == 0 && keys!=null)
            {
                properties.ValidLicenseKeys = keys;
                this.Hide();
                Description nextScreen = new Description(this);
                nextScreen.Show();
            }
            else
            {
                LOG.ErrorFormat("LicenseScreen.next_Click()- Exception: {0}",error);
                MessageBox.Show(
                    error,
                    "License Key Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1,
                    0, "http://www.idera.com/licensing"
                );                
            }
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("Do you wish to cancel installation?", "Cancel", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2))
            {
                Application.Exit();
            }
        }

        private void copyExistingLicenseInfo_Click(object sender, EventArgs e)
        {
            Clipboard.SetText("Existing License Key(s) : " + existingLicenseKeyTextbox.Text+"\n"+
                              "SQLdm Repository : " + sqldmRepoTextbox.Text + "\n"+
                              "Total Licenses : " + totalLicensesTextbox.Text + "\n"+
                              "Monitored Instances : " + monitoredInstancesTextbox.Text);
        }

        public string ValidateNewLicenseKeys(string newLicenseKeysString,out string errorMessage)
        {
            try
            {
                errorMessage = string.Empty;
                string instance = properties.DMInstance;// session[INSTANCE];
                string database = properties.DMDBName;// session[REPOSITORY];
                int registeredServers = licenseSummary.MonitoredServers;
                string newLicenseKeyString = newLicenseKeysString;

                if (string.IsNullOrEmpty(newLicenseKeyString))
                {
                    errorMessage = "Please enter atleast one license key";
                    return null;
                }

                //Multiple keys will be seperated by semi colon (;)
                string[] delimiter = new string[] { "," };
                string[] newLicenseKeys = newLicenseKeyString.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                List<string> keyList = new List<string>();
                foreach (string key in newLicenseKeys)
                {
                    keyList.Add(key);
                }

                if (newLicenseKeys.Length == 0)
                {
                    errorMessage = "Please enter atleast one license key";
                    return null;
                }
                else
                {
                    LicenseSummary license = LicenseSummary.SummarizeKeys(
                          registeredServers,
                          instance,
                          keyList);
                    if (license.Status == LicenseStatus.OK)
                    {
                        if (license.IsTrial)
                            errorMessage = "A trial key can not be entered. Please enter a production key to continue.";
                        else
                        {
                            if (license.ProductVersion.Major < 9)
                                errorMessage = "The license key is for an older version of SQL diagnostic manager.  Please click on 'Help' to acquire a new license key.";
                            else
                                
                                foreach (var key in license.CheckedKeys)
                                {
                                    if (!key.IsValid)
                                    {
                                        string tempErrorMessage = "\nInvalid Key : " + key.KeyString + " : " + key.Comment;
                                        errorMessage += tempErrorMessage;
                                    }
                                        
                                    
                                }
                        }
                    }
                    else if (license.Status == LicenseStatus.CountExceeded)
                    {
                        errorMessage = "The number of currently monitored servers is greater than the number allowed by the specified key(s).";
                    }
                    else if (license.Status == LicenseStatus.Expired)
                    {
                        errorMessage = "The entered key has already expired.";
                    }
                    else if (license.Status == LicenseStatus.NoValidKeys)
                    {
                        errorMessage = "The specified license key(s) is invalid.";
                    }

                }
                if (errorMessage.Length > 0)
                    return null;
                else
                    return newLicenseKeysString;
            }
            catch (Exception ex)
            {
                errorMessage="Error occured in ValidateNewLicenseKeys . Error:" + ex.Message + "\n" + ex.StackTrace;
                return null;
            }
        }
    }

    
}