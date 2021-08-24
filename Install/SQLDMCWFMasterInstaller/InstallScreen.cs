using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Installer_form_application
{
    public partial class InstallScreen : Form
    {
        Form screenObject;
        public InstallScreen(Form backScreen)
        {
            InitializeComponent();
            AcceptButton = buttonFinish;
            MinimizeBox = false;
            MaximizeBox = false;
            screenObject = backScreen;
        }

        private void InstallScreen_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void buttonFinish_Click(object sender, EventArgs e)
        {
            if (checkBoxLaunchApp.Checked)
            {                                                   
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.FileName = Path.Combine(properties.dmInstallationPath , "SQLdmDesktopClient.exe");
                process.Start();                
            }
            Application.Exit();
        }

        private void ConfigureAndLoadLabelDesc()
        {
            bool add = false;
            labelDesc.Text = "Congratulations!  ";

            if (properties.installDashboardOption)
            {
                labelDesc.Text += "IDERA Dashboard ";
                add = true;
            }

            if (properties.installSQLDMConsoleOption || properties.installSQLDMServiceAndRepositoryOption)
            {
                if (add)
                    labelDesc.Text += "and IDERA SQL Diagnostic Manager have been successfully installed.";
                else
                    labelDesc.Text += "IDERA SQL Diagnostic Manager has been successfully installed.";                                
            }
            else
                labelDesc.Text += "has been successfully installed.";
        }

        private void ConfigureAndLoadSQLdmLabel()
        {
            if(properties.installSQLDMConsoleOption)
                labelDesc.Text += "\n\n\nTo launch the SQL Diagnostic Manager:\n    \u2022 Go to Start Menu, Select Idera -> Idera SQL Diagnostic Manager Application";            
        }

        private void ConfigureAndLoadDashboardLabel()
        {
            string url;
            if (properties.remoteRegister)
                url = "https://" + properties.remoteHostname + ":" + properties.WebAppSSLPort;
            else
                url = "https://localhost:" + properties.WebAppSSLPort;

            if (properties.installDashboardOption || properties.remoteRegister)
                labelDesc.Text += "\n\n\nTo launch the IDERA Dashboard:\n    \u2022 Open the url -> " + url + ", from your browser";
        }

        private void ConfigureLaunchAppCheckBox()
        {
            if (properties.installSQLDMConsoleOption)
            {
                checkBoxLaunchApp.Visible = true;
                checkBoxLaunchApp.Checked = true;
            }
            else
            {
                checkBoxLaunchApp.Visible = false;
                checkBoxLaunchApp.Checked = false;
            }
        }

        private void InstallScreen_Load(object sender, EventArgs e)
        {
            ConfigureAndLoadLabelDesc();
            ConfigureAndLoadSQLdmLabel();
            ConfigureAndLoadDashboardLabel();
            ConfigureLaunchAppCheckBox();
        }
    }
}
