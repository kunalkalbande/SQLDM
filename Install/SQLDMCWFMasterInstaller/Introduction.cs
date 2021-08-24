using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CWFInstallerService;

// used for account validation
using System.DirectoryServices.AccountManagement;

// for reading registry values
using Microsoft.Win32;
using System.Threading;
using System.IO;

namespace Installer_form_application
{
    public partial class Introduction : Form
    {

        public const string DotNetVersion = ".NET Framework 2.0 not found.\n.NET Framework 2.0 and 4.0 must be installed prior to installation of this product.";

        public Introduction()
        {
            InitializeComponent();
#if TRIAL
            this.linkLabelGuide.TabStop = false;

            this.LabelHere.Size = new Size(383, 52);
            this.LabelHere.Text = "The SQL Diagnostic Manager setup Wizard allows you to install a trial version SQL Diagnostic Manager.";

            this.linkLabelGuide.LinkArea = new LinkArea(269, 6);
            this.linkLabelGuide.Location = new Point(198, 124);
            this.linkLabelGuide.Size = new Size(383, 100);
            this.linkLabelGuide.Text = "This trial does not include the IDERA Web Dashboard.  For a demonstration of SQL Diagnostic Manager with IDERA Web Dashboard, Please send an email to sales@idera.com or contact your IDERA account manager.\r\n\r\nIf you need assistance during the installation process, click <here> for installation instructions.";
            this.linkLabelGuide.Visible = true;

            this.label1.Visible = false;
            this.Text = string.Format("IDERA SQL Diagnostic Manager Trial Setup (version {0})", "11.1");
#else 
            this.Text = string.Format("IDERA SQL Diagnostic Manager Setup (version {0})", "11.1");

#endif //TRIAL

            MinimizeBox = false;
            MaximizeBox = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            EULA eula = new EULA(this);
            eula.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //if (IntPtr.Size == 4 && Environment.Is64BitOperatingSystem)
            //{
            //    MessageBox.Show("Please use a x64 installer to install the product", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    Application.Exit();
            //} 
            try
            {
                Validator.ValidateIfDotNet40FullInstalled();
                Validator.ValidateIfOperatingSystemCompatible();
            }
            catch (CWFBaseException ex)
            {
                MessageBox.Show(ex.ErrorMessage, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            /*
             * Removing this check.  It is no longer needed since DM is not dependent on .NET 2.0
            if (!ValidateIfDotNet20Installed())
            {
                MessageBox.Show(DotNetVersion);
                this.Close();
                Application.Exit();
                Environment.Exit(0);
            }
            */

        }

        private static bool ValidateIfDotNet20Installed()
        {
            // Opens the registry key for the .NET Framework entry.
            using (RegistryKey ndpKey =
                RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, "").
                OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\"))
            {
                foreach (string versionKeyName in ndpKey.GetSubKeyNames())
                {
                    if (versionKeyName.StartsWith("v"))
                    {

                        RegistryKey versionKey = ndpKey.OpenSubKey(versionKeyName);
                        string name = (string)versionKey.GetValue("Version", "");
                        string sp = versionKey.GetValue("SP", "").ToString();
                        string install = versionKey.GetValue("Install", "").ToString();

                        if (name != "")
                        {
                            if (install == "1")
                            {
                                if (name.Contains("2.0"))
                                {
                                    return true;
                                }

                            }

                            continue;
                        }
                        foreach (string subKeyName in versionKey.GetSubKeyNames())
                        {
                            RegistryKey subKey = versionKey.OpenSubKey(subKeyName);
                            name = (string)subKey.GetValue("Version", "");
                            if (name != "")
                                sp = subKey.GetValue("SP", "").ToString();
                            install = subKey.GetValue("Install", "").ToString();

                            if (name != "")
                            {
                                if (install == "1")
                                {
                                    if (name.Contains("2.0"))
                                    {
                                        return true;
                                    }

                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        private void Introduction_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("Do you wish to cancel installation?", "Cancel", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2))
            {
                Application.Exit();
            }
        }

        private void linkLabelGuide_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"http://wiki.idera.com/x/GQA1");
        }
    }
}
