namespace Idera.SQLdm.Common.UI.Dialogs.Config
{
    using System;
    using System.Collections.Generic;
    using System.Management;
    using System.Threading;
    using System.Windows.Forms;
    using System.Runtime.InteropServices;

    public partial class ServiceBrowserDialog : Form
    {
        public enum ServiceType
        {
            Management,
            Collection
        }

        private ServiceType serviceType;
        private string machineName;
        private ServiceInstance selectedInstance;

        public ServiceBrowserDialog(ServiceType serviceType, string machineName)
        {
            InitializeComponent();
            this.Icon = Properties.Resources.SQLdmConfigWizard;

            this.serviceType = serviceType;
            this.machineName = machineName;
            this.Text = String.Format("Select {0} Service", serviceType.ToString());
            this.label_AvailInstancesOn.Text = "&Available instances on " + machineName + ": ";

            this.MinimumSize = this.Size;

            stackLayoutPanel1.ActiveControl = wmiProgressControl;
            wmiProgressControl.Active = true;
            backgroundWorker.RunWorkerAsync();
            serviceBindingSource.Sort = "InstanceName";
        }

        public ServiceInstance SelectedInstance
        {
            get { return selectedInstance; }
        }

        private void backgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            // allow the progress indicator to show to prevent flash
            Thread.Sleep(2000);

            List<ServiceInstance> list = new List<ServiceInstance>();
            e.Result = list;

            string path = String.Format(@"\\{0}\root\Idera\SQLdm\{1}Service", machineName, serviceType);
            string computerName = machineName;

            string query = String.Format("SELECT * FROM {0}ServiceConfiguration", serviceType);
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(path, query);

            foreach (ManagementObject mbo in searcher.Get())
            {
                ServiceInstance info = new ServiceInstance(
                    computerName,
                    mbo["InstanceName"].ToString(),
                    (int)mbo["ServicePort"]);

                list.Add(info);
            }
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            try
            {
                wmiProgressControl.Active = false;
                stackLayoutPanel1.ActiveControl = serviceListBox;

                if (e.Error != null)
                {
                    // Get the exception details.
                    string errMsg = string.Format("Error was encountered when retrieving a list of {0} Service instances running on {1}.", serviceType.ToString(), machineName);
                    COMException comException = e.Error as COMException;
                    if (comException != null)
                    {
                        switch (comException.ErrorCode)
                        {
                            case -2147023174: // Fire wall
                            case -2147023838: // WMI not running
                                errMsg += "  The WMI service may not be running, or the Windows Firewall is blocking WMI queries.";
                                break;

                            default:
                                break;
                        }
                    }
                    else
                    {
                        ManagementException mgmtException = e.Error as ManagementException;
                        if (mgmtException != null)
                        {
                            switch (mgmtException.ErrorCode)
                            {
                                case ManagementStatus.InvalidNamespace:
                                    errMsg += string.Format("  The {0} Service is not installed on this computer.", serviceType.ToString());
                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                    ApplicationMessageBox.ShowError(this, errMsg, e.Error);
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                }
                else
                {
                    List<ServiceInstance> services = e.Result as List<ServiceInstance>;
                    foreach (ServiceInstance service in services)
                    {
                        serviceBindingSource.Add(service);
                    }
                }
            } catch (Exception ex)
            {
                ApplicationMessageBox.ShowError(this, ex);
            }
        }

        private void serviceListBox_DoubleClick(object sender, EventArgs e)
        {
            if (selectedInstance != null)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void serviceListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            selectedInstance = serviceListBox.SelectedValue as ServiceInstance;
            okButton.Enabled = selectedInstance != null;
        }
    }
}