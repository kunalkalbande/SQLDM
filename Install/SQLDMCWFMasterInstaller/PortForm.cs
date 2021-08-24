using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CWFInstallerService;
using BBS.TracerX;

namespace Installer_form_application
{
    public partial class PortForm : Form
    {
        Form screenObject;
        private Logger LOG = Logger.GetLogger("PortForm");
        public PortForm(Form screenObj)
        {
            InitializeComponent();
            AcceptButton = buttonNext;
            CancelButton = buttonCancel;
            MinimizeBox = false;
            MaximizeBox = false;
            screenObject = screenObj;
        }
               
        private void buttonBack_Click(object sender, EventArgs e)
        {
            properties.CoreServicesPort = textBoxCoreServicesPort.Text;
            properties.WebAppMonitorPort = textBoxWebAppMonitorPort.Text;
            properties.WebAppServicePort = textBoxWebAppServicePort.Text;
            properties.WebAppSSLPort = textBoxWebAppSSLPort.Text;
            this.Hide();
            screenObject.Show();
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            this.buttonNext.Enabled = false;
            this.buttonBack.Enabled = false;
            this.Cursor = Cursors.WaitCursor;
            backgroundWorker.RunWorkerAsync();   
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<int> ports = new List<int>();
            try
            {
                ports.Add(Int32.Parse(textBoxCoreServicesPort.Text));
                ports.Add(Int32.Parse(textBoxWebAppMonitorPort.Text));
                ports.Add(Int32.Parse(textBoxWebAppServicePort.Text));
                ports.Add(Int32.Parse(textBoxWebAppSSLPort.Text));

                if (properties.installDashboardOption)
                {
                    Validator.ValidatePorts(ports);
                }
            }
            catch (CWFBaseException ex)
            {
                e.Result = ex.ErrorMessage;
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

            if (e.Result != null)
            {
                LOG.ErrorFormat("backgroundWorker_RunWorkerCompleted()- Exception: {0}",e.Result.ToString());
                MessageBox.Show(e.Result.ToString(),"Invalid Port(s)",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }

            properties.CoreServicesPort = textBoxCoreServicesPort.Text;
            properties.WebAppMonitorPort = textBoxWebAppMonitorPort.Text;
            properties.WebAppServicePort = textBoxWebAppServicePort.Text;
            properties.WebAppSSLPort = textBoxWebAppSSLPort.Text;
            this.Hide();
            RepositoryDetails nextScreen = new RepositoryDetails(this);
            nextScreen.Show();
        }

        private void PortForm_FormClosing(object sender, FormClosingEventArgs e)
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

        private void PortForm_Load(object sender, EventArgs e)
        {            
            textBoxCoreServicesPort.Text = properties.CoreServicesPort;
            textBoxWebAppMonitorPort.Text = properties.WebAppMonitorPort;
            textBoxWebAppServicePort.Text = properties.WebAppServicePort;
            textBoxWebAppSSLPort.Text = properties.WebAppSSLPort;
        }
    }
}
