using System;
using System.ComponentModel;
using System.Windows.Forms;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Services;
using System.Diagnostics;
using System.Threading;
using System.Runtime.Remoting;
using System.Collections;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace Idera.SQLdm.Common.UI.Dialogs.Config
{
    public partial class CollectionServiceConfigWizard : Form
    {
        private const string BROWSE_FO_MO = "<Browse for more...>";
        private delegate void ShowHelperDialog();
        delegate CollectionServiceConfigurationMessage GetCollectionServiceServiceConfiguration();

        private CollectionServiceConfigurationMessage configMessage;

        private object sync = new object();

        private ServiceInstance selectedInstance;
        private int selectedPort = 5167;
        private string selectedMachine; 
        
        public CollectionServiceConfigWizard()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.SQLdmConfigWizard;

            machineName.Text = Environment.MachineName;

            InitializePortList();
        }

        private void InitializePortList()
        {
            portList.Text = "";
            portList.Items.Clear();
            ServiceInstance instance = new ServiceInstance(machineName.Text, "Default", 5167);
            portList.Items.Add(instance);
            portList.Items.Add(BROWSE_FO_MO);
            selectedInstance = instance;
            portList.SelectedItem = instance;
        }


        private void selectServicePage_BeforeDisplay(object sender, EventArgs e)
        {
            if(backgroundWorker1.IsBusy)
                backgroundWorker1.CancelAsync();
        }

        private void connectWaitPage_BeforeDisplay(object sender, EventArgs e)
        {

        }

        private void settingsPage_BeforeDisplay(object sender, EventArgs e)
        {

        }
        
        private void settingsPage_BeforeMoveBack(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            wizard.SelectedPage = selectServicePage;
        }
       
        private void machineName_Leave(object sender, EventArgs e)
        {
            selectedMachine = machineName.Text.Trim();
        }

        private int GetSelectedPort()
        {
            if (selectedInstance != null)
                return selectedInstance.ServicePort;
            else
                return selectedPort;
        }

        private ICollectionServiceConfiguration GetService()
        {
            int port = GetSelectedPort();
            Debug.Print("selectedMachine=" + selectedMachine);
            Debug.Print("selectedPort=" + port.ToString());
            object[] args = new object[] { selectedMachine, port };
            Uri uri = new Uri(String.Format("tcp://{0}:{1}/Configuration", args));
            ICollectionServiceConfiguration service = RemotingHelper.GetObject<ICollectionServiceConfiguration>(uri.ToString());
            return service;
        }
        
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            ICollectionServiceConfiguration service = GetService();
            GetCollectionServiceServiceConfiguration asyncDelegate = new GetCollectionServiceServiceConfiguration(service.GetCollectionServiceConfiguration);

            IAsyncResult iar = asyncDelegate.BeginInvoke(null, null);
            while(!iar.IsCompleted)
            {
                if (backgroundWorker1.CancellationPending)
                {
                    Debug.Print("cancelled!");
                    e.Cancel = true;
                    e.Result = null;
                    return;                    
                }
                Thread.Sleep(700);
            }
            try
            {
                e.Result = asyncDelegate.EndInvoke(iar);
            }
            catch (Exception e1)
            {
                e.Result = e1;
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
                return;

            try
            {
                if (e.Error != null)
                    throw e.Error;

                if (e.Result is Exception)
                    throw ((Exception)e.Result);

                collectionServiceSource.Clear();
                configMessage = (CollectionServiceConfigurationMessage)e.Result;
                // add items to cboPort
                cboPort.Items.Clear();
                cboPort.Items.Add(configMessage.ManagementPort);
                cboPort.Items.Add(BROWSE_FO_MO);

                collectionServiceSource.Add(configMessage);
                wizard.GoNext();
            }
            catch (Exception e1)
            {
                ApplicationMessageBox.ShowError(this, e1);
                wizard.GoBack();
            }
        }

        private void connectWaitPage_AfterDisplay(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }

        private void CollectionServiceConfigWizard_Load(object sender, EventArgs e)
        {
            try
            {
                ConfigureRemoting();
            } catch (Exception)
            {
            }
        }

        internal void ConfigureRemoting()
        {
            RemotingConfiguration.CustomErrorsMode = CustomErrorsModes.Off;

            // register a server channel
            IDictionary properties = new System.Collections.Specialized.ListDictionary();

            // register a client channel
            properties = new System.Collections.Specialized.ListDictionary();
            properties["name"] = "tcp-client";
            properties["impersonationLevel"] = "None";
            properties["impersonate"] = false;
            properties["secure"] = true;

            BinaryClientFormatterSinkProvider clientSinkProvider = new BinaryClientFormatterSinkProvider();

            TcpClientChannel tcpClientChannel = new TcpClientChannel(properties, clientSinkProvider);
            ChannelServices.RegisterChannel(tcpClientChannel, true);

            // register the server channel
            foreach (IChannel channel in ChannelServices.RegisteredChannels)
            {
                Debug.Print("Registered channel: " + channel.ChannelName);
            }
        }

        private void wizard_Finish(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void wizard_Cancel(object sender, EventArgs e)
        {
            if (wizard.SelectedPage == this.connectWaitPage)
            {
                CancelConnect();
            }
            else
                Application.Exit();
        }
        
        private void CancelConnect()
        {
            wizard.GoBack();
        }

        private void settingsPage_BeforeMoveNext(object sender, CancelEventArgs e)
        {
            ICollectionServiceConfiguration service = GetService();
            collectionServiceSource.EndEdit();

            try
            {
                service.SetCollectionServiceConfiguration((CollectionServiceConfigurationMessage)collectionServiceSource.Current);
            }
            catch (Exception e1)
            {
                ApplicationMessageBox.ShowError(this, e1);
                e.Cancel = true;
            }
        }

        private void introductionPage_BeforeDisplay(object sender, EventArgs e)
        {

        }

        private void portList_SelectionChangeCommitted(object sender, EventArgs e)
        {
            object selectedItem = portList.SelectedItem;
            // the only string item in the list is the label to show the select dialog.
            if (selectedItem is string && selectedItem.Equals(BROWSE_FO_MO))
            {
                ShowHelperDialog method =
                    delegate()
                    {
                        ServiceBrowserDialog sbd =
                            new ServiceBrowserDialog(
                                ServiceBrowserDialog.ServiceType.Collection, machineName.Text);
                        if (sbd.ShowDialog(this) == DialogResult.OK)
                        {
                            ServiceInstance si = sbd.SelectedInstance;
                            if (!portList.Items.Contains(si))
                                portList.Items.Insert(0, si);
                            portList.SelectedItem = si;
                            selectedInstance = si;
                        }
                        else
                        {
                            SetDefaultItem(portList);
                            selectedInstance = portList.SelectedItem as ServiceInstance;
                        }
                    };
                BeginInvoke(method);
            }
            else
                if (selectedItem is ServiceInstance)
                    selectedInstance = selectedItem as ServiceInstance;
                else
                    selectedInstance = null;
        }

        private void portList_TextUpdate(object sender, EventArgs e)
        {
            // set list index to -1 to signify a typed in value
            portList.SelectedIndex = -1;

            // make sure they enter a numeric value
            int port;
            if (Int32.TryParse(portList.Text, out port))
            {
                selectedPort = port;
                selectServicePage.AllowMoveNext = portList.Text.Length > 0;
                foreach (object o in portList.Items)
                {
                    if (o is ServiceInstance && ((ServiceInstance)o).ServicePort == selectedPort)
                    {
                        selectedInstance = (ServiceInstance) o;
                        return;
                    }
                }
                selectedInstance = null;
            }
            else
                selectServicePage.AllowMoveNext = false;
        }

        private void SetDefaultItem(ComboBox cbo)
        {
            if (cbo.Items.Count > 1)
                cbo.SelectedIndex = 0;
            else
            {
                cbo.SelectedIndex = -1;
                cbo.SelectedText = "";
            }
        }

        private void cboPort_SelectionChangeCommitted(object sender, EventArgs e)
        {
            object selectedItem = cboPort.SelectedItem;
            // the only string item in the list is the label to show the select dialog.
            if (selectedItem is string && selectedItem.Equals(BROWSE_FO_MO))
            {
                ShowHelperDialog method =
                    delegate()
                    {
                        ServiceBrowserDialog sbd =
                            new ServiceBrowserDialog(
                                ServiceBrowserDialog.ServiceType.Management, serverName.Text);
                        if (sbd.ShowDialog(this) == DialogResult.OK)
                        {
                            ServiceInstance si = sbd.SelectedInstance;
                            if (!cboPort.Items.Contains(si.ServicePort))
                                cboPort.Items.Insert(0, si.ServicePort);
                            cboPort.SelectedItem = si.ServicePort;
                        }
                        else
                            cboPort.SelectedItem = configMessage.ManagementPort;
                    };
                BeginInvoke(method);
            }
            else
            if (selectedItem is string)
            {
                int i = configMessage.ManagementPort;
                Int32.TryParse((string)selectedItem, out i);
                configMessage.ManagementPort = i;
                cboPort.SelectedItem = i;
            }
            else
            if (selectedItem is int)
                configMessage.ManagementPort = (int)selectedItem;
            else
            {
                cboPort.SelectedItem = configMessage.ManagementPort;
            }
        }
    }
}