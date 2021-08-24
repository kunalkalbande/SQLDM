using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Installer_form_application
{
    public partial class InstallationFailure : Form
    {
        public InstallationFailure(string error)
        {
            InitializeComponent();
            label1.Text = "Following is the output of installation: \n" + error;
        }

        private void InstallFailure_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();        
        }

        private void buttonFinish_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

     
    }
}
