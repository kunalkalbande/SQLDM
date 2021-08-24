using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Installer_form_application
{
    public partial class EULA : Form
    {
        Form screenObject;

        public EULA(Form screenObj)
        {
            InitializeComponent();
            AcceptButton = buttonNext;
            CancelButton = buttonCancel;
            MinimizeBox = false;
            MaximizeBox = false;
            screenObject = screenObj;
        }

        private void EULA_Load(object sender, EventArgs e)
        {
            checkBoxAccept.Checked = properties.AGREETOLICENSE;

            string strLicenceAgreementDirectory = System.Reflection.Assembly.GetExecutingAssembly().Location;//Directory.GetCurrentDirectory();
            strLicenceAgreementDirectory = Directory.GetParent(strLicenceAgreementDirectory).FullName;
            strLicenceAgreementDirectory = Path.Combine( strLicenceAgreementDirectory , "../Documentation", "Idera - Software License Agreement.rtf");
            richTextBoxEULA.LoadFile(strLicenceAgreementDirectory);
        }

        private void EULA_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            properties.AGREETOLICENSE = checkBoxAccept.Checked;
            this.Hide();
            screenObject.Show();
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            properties.AGREETOLICENSE = checkBoxAccept.Checked;
            this.Hide();
            //Start:  SQLDM 10.1 - Praveen Suhalka - CWF Integration 
            InstallOption opt = new InstallOption(this);
            opt.Show();
            //End:  SQLDM 10.1 - Praveen Suhalka - CWF Integration 
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("Do you wish to cancel installation?", "Cancel", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2))
            {
                Application.Exit();
            }
        }

        private void checkBoxAccept_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxAccept.Checked)
            {
                buttonNext.Enabled = true;
            }
            else
            {
                buttonNext.Enabled = false;
            }
        }

    }
}
