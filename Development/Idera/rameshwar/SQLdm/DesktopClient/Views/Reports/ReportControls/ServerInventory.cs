using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using BBS.TracerX;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Infragistics.Win;
using Microsoft.Reporting.WinForms;

namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    public partial class ServerInventory : ReportContol
    {
        private static readonly Logger Log = Logger.GetLogger("Server Inventory");
        private List<ReportParameter> passthroughParameters = new List<ReportParameter>(1);

        protected class ServerInventoryParams
        {
            public string serverName;
            public string serverVersion;
            public string OSVersion;
            public Int32 processorCount;
            public Int64 physicalMemoryKB;
            public bool? IsClustered;
            public string owner;
            public Int32 tagId;

            public ServerInventoryParams()
            {
                serverName = "";
                serverVersion = "";
                OSVersion = "";
                processorCount = 0;
                physicalMemoryKB = 0;
                IsClustered = null;
                owner = "";
                tagId = 0;
            }
        }

        private enum ServerInventorySearchCriteria
        {
            selectSearchItem,
            ServerName,
            ServerVersion,
            OSVersion,
            NumberOfProcessors,
            PhysicalMemory,
            Clustered,
            Owner,
            ServerTag
        }

        readonly ValueListItem selectSearchItem = new ValueListItem(ServerInventorySearchCriteria.selectSearchItem, "<Select Search Item>");
        readonly ValueListItem searchServerName      = new ValueListItem(ServerInventorySearchCriteria.ServerName, "Server Name");
        readonly ValueListItem searchServerVersion = new ValueListItem(ServerInventorySearchCriteria.ServerVersion, "Server Version");
        readonly ValueListItem searchOSVersion = new ValueListItem(ServerInventorySearchCriteria.OSVersion, "OS Version");
        readonly ValueListItem searchNumProcessors = new ValueListItem(ServerInventorySearchCriteria.NumberOfProcessors, "Number of Processors");
        readonly ValueListItem searchPhysicalMemory = new ValueListItem(ServerInventorySearchCriteria.PhysicalMemory, "Physical Memory");
        readonly ValueListItem searchClustered = new ValueListItem(ServerInventorySearchCriteria.Clustered, "Is Clustered");
        readonly ValueListItem searchOwner = new ValueListItem(ServerInventorySearchCriteria.Owner, "Owner");
        readonly ValueListItem searchServerTag = new ValueListItem(ServerInventorySearchCriteria.ServerTag, "Server Tag");

        private bool blnsearch1ValueisNumeric;
        private bool blnsearch2ValueisNumeric;
        private bool blnsearch3ValueisNumeric;
        private bool blnsearch4ValueisNumeric;

        public ServerInventory()
        {
            InitializeComponent();
            base.AdaptFontSize();
        }

        public override void InitReport()
        {
            searchValue1Textbox.KeyPress += searchValueTextbox_onKeyPress;
            searchValue2Textbox.KeyPress += searchValueTextbox_onKeyPress;
            searchValue3Textbox.KeyPress += searchValueTextbox_onKeyPress;
            searchValue4Textbox.KeyPress += searchValueTextbox_onKeyPress;

            base.InitReport();
            InitializeReportViewer();
            
            InitGenericSearchCombo((ValueListItem)searchItem2Combo.SelectedItem,
                (ValueListItem)searchItem3Combo.SelectedItem,
                (ValueListItem)searchItem4Combo.SelectedItem,
                searchItem1Combo,"");

            State = UIState.ParmsNeeded;
            periodCombo.Visible = false;
            periodLabel.Visible = false;
            instanceCombo.Visible = false;
            instanceLabel.Visible = false;
            sampleSizeCombo.Visible = false;
            sampleLabel.Visible = false;
            ReportType = ReportTypes.ServerInventory;
        }

        #region Initialize combo boxes
        
        /// <summary>
        /// Add searchitems that have not previously been used in other combos
        /// </summary>
        private void InitGenericSearchCombo(ValueListItem selectedItem1, ValueListItem selectedItem2, ValueListItem selectedItem3, ComboBox combo, string strDefault)
        {
            InitGenericSearchCombo(selectedItem1, selectedItem2, selectedItem3, ref combo, strDefault, false);
        }

        /// <summary>
        /// Add searchitems that have not previously been used in other combos. Add search item text if required
        /// </summary>
        private void InitGenericSearchCombo(ValueListItem selectedItem1, ValueListItem selectedItem2, ValueListItem selectedItem3, ref ComboBox combo, string strDefault, bool addSelectSearchItem)
        {
            combo.Items.Clear();
            
            if (addSelectSearchItem) combo.Items.Add(selectSearchItem);

            //if none of the other combo boxes contains this search criteria
            if ((selectedItem1 != searchServerName) && (selectedItem2 != searchServerName) && (selectedItem3 != searchServerName))
            {
                combo.Items.Add(searchServerName);
            }

            if ((selectedItem1 != searchServerVersion) && (selectedItem2 != searchServerVersion) && (selectedItem3 != searchServerVersion))
            {
                combo.Items.Add(searchServerVersion);
            }

            if ((selectedItem1 != searchOSVersion) && (selectedItem2 != searchOSVersion) && (selectedItem3 != searchOSVersion))
            {
                combo.Items.Add(searchOSVersion);
            }

            if ((selectedItem1 != searchNumProcessors) && (selectedItem2 != searchNumProcessors) && (selectedItem3 != searchNumProcessors))
            {
                combo.Items.Add(searchNumProcessors);
            }

            if ((selectedItem1 != searchPhysicalMemory) && (selectedItem2 != searchPhysicalMemory) && (selectedItem3 != searchPhysicalMemory))
            {
                combo.Items.Add(searchPhysicalMemory);
            }

            if ((selectedItem1 != searchClustered) && (selectedItem2 != searchClustered) && (selectedItem3 != searchClustered))
            {
                combo.Items.Add(searchClustered);
            }

            if ((selectedItem1 != searchServerTag) && (selectedItem2 != searchServerTag) && (selectedItem3 != searchServerTag))
            {
                combo.Items.Add(searchServerTag);
            }

            if (strDefault != "")
            {
                for (int i = 0; i < combo.Items.Count; i++)
                {
                    if (strDefault.Equals(combo.Items[i].ToString()))
                    {
                        combo.SelectedIndex = i;
                        break;
                    }
                }
                if (combo.SelectedIndex==-1) combo.SelectedIndex = 0;
            }else
            {
                combo.SelectedIndex = 0;
            }
        }

#endregion

        protected override void SetReportParameters()
        {
            ServerInventoryParams reportParams = new ServerInventoryParams();

            base.SetReportParameters();

            reportParams = setServerInventoryParams(reportParams, (ValueListItem)searchItem1Combo.SelectedItem,
                                     searchValue1Textbox.Text, tagsCombo1, memoryUpDown1.Text, clusteredCombo1.Text);

            if (searchItem2Combo.SelectedItem != null) reportParams = 
                setServerInventoryParams(reportParams, (ValueListItem)searchItem2Combo.SelectedItem,
                                         searchValue2Textbox.Text, tagsCombo2, memoryUpDown2.Text, clusteredCombo2.Text);
            
            if (searchItem3Combo.SelectedItem != null) reportParams = 
                setServerInventoryParams(reportParams, (ValueListItem)searchItem3Combo.SelectedItem,
                searchValue3Textbox.Text, tagsCombo3, memoryUpDown3.Text, clusteredCombo3.Text);
            

            if (searchItem4Combo.SelectedItem != null) reportParams = 
                setServerInventoryParams(reportParams, (ValueListItem)searchItem4Combo.SelectedItem,
                searchValue4Textbox.Text, tagsCombo4, memoryUpDown4.Text, clusteredCombo4.Text);

            reportParameters.Add("serverInventory", reportParams);
            string selectedTag = null;
            //if a tag has been selected
            if(reportParams.tagId > 0)
            {
                if(tagsCombo1.SelectedItem != null)
                {
                    selectedTag = ((ValueListItem) tagsCombo1.SelectedItem).DisplayText;
                } 
                else if(tagsCombo2.SelectedItem != null)
                {
                    selectedTag = ((ValueListItem)tagsCombo2.SelectedItem).DisplayText;
                }
                else if(tagsCombo3.SelectedItem != null)
                {
                    selectedTag = ((ValueListItem)tagsCombo3.SelectedItem).DisplayText;    
                }else if (tagsCombo4.SelectedItem != null)
                {
                    selectedTag = ((ValueListItem)tagsCombo4.SelectedItem).DisplayText;
                }
            }
            reportParameters.Add("GUITag", selectedTag);
        }

        /// <summary>
        /// This method is returning the report parameters.  The names must match what is set in the RDL file.
        /// </summary>
        /// <returns></returns>
        public override Dictionary<string, object> GetReportParmeters()
        {
            ServerInventoryParams reportParams = new ServerInventoryParams();

            base.SetReportParameters();

            reportParams = setServerInventoryParams(reportParams, (ValueListItem)searchItem1Combo.SelectedItem,
                searchValue1Textbox.Text, tagsCombo1, memoryUpDown1.Text, clusteredCombo1.Text);

            if (searchItem2Combo.SelectedItem != null) reportParams =
                setServerInventoryParams(reportParams, (ValueListItem)searchItem2Combo.SelectedItem,
                searchValue2Textbox.Text, tagsCombo2, memoryUpDown2.Text, clusteredCombo2.Text);

            if (searchItem3Combo.SelectedItem != null) reportParams =
                setServerInventoryParams(reportParams, (ValueListItem)searchItem3Combo.SelectedItem,
                searchValue3Textbox.Text, tagsCombo3, memoryUpDown3.Text, clusteredCombo3.Text);


            if (searchItem4Combo.SelectedItem != null) reportParams =
                setServerInventoryParams(reportParams, (ValueListItem)searchItem4Combo.SelectedItem,
                searchValue4Textbox.Text, tagsCombo4, memoryUpDown4.Text, clusteredCombo4.Text);

            IList<int> servers = new List<int>(ApplicationModel.Default.AllInstances.Count);

            foreach (MonitoredSqlServer server in ApplicationModel.Default.AllInstances.Values)
            {
                servers.Add(server.Id);
            }
            reportParameters.Clear();

            if (String.IsNullOrEmpty(reportParams.serverName))
            {
                reportParameters.Add("ServerName", null);
            }
            else
            {
                reportParameters.Add("ServerName", reportParams.serverName);
            }

            if (String.IsNullOrEmpty(reportParams.serverVersion))
            {
                reportParameters.Add("ServerVersion", null);
            }
            else
            {
                reportParameters.Add("ServerVersion", reportParams.serverVersion);
            }
            if (String.IsNullOrEmpty(reportParams.OSVersion))
            {
                reportParameters.Add("OSVersion", null);
            }
            else
            {
                reportParameters.Add("OSVersion", reportParams.OSVersion);
            }
            reportParameters.Add("NumberOfProcessors", reportParams.processorCount.ToString());
            reportParameters.Add("PhysicalMemory", (reportParams.physicalMemoryKB /1024 / 1024).ToString());
            reportParameters.Add("Clustered", reportParams.IsClustered != null ? reportParams.IsClustered.ToString() : null);
            
            if (String.IsNullOrEmpty(reportParams.owner))
            {
                reportParameters.Add("Owner", null);
            }
            else
            {
                reportParameters.Add("Owner", reportParams.owner);
            }
            
            reportParameters.Add("SQLServerIDs", GetServerIdXml(servers));

            return reportParameters;
        }

        #region Backgroundworker
        override protected void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            ServerInventoryParams reportParams;
            if (System.Threading.Thread.CurrentThread.Name == null) System.Threading.Thread.CurrentThread.Name = "ReportControlWorker";

            using (Log.DebugCall())
            {
                WorkerData localReportData = (WorkerData)e.Argument;
                ReportDataSource dataSource;

                Log.Debug("localReportData.reportType = ", localReportData.reportType);
                localReportData.dataSources = new ReportDataSource[1];
                reportParams = (ServerInventoryParams)localReportData.reportParameters["serverInventory"];
                dataSource = new ReportDataSource("ServerInventory");

                IList<int> servers = new List<int>(ApplicationModel.Default.AllInstances.Count);

                foreach (MonitoredSqlServer server in ApplicationModel.Default.AllInstances.Values)
                {
                    servers.Add(server.Id);
                }

                passthroughParameters.Clear();
                passthroughParameters.Add(new ReportParameter("executionStart", DateTime.Now.ToString()));
                passthroughParameters.Add(new ReportParameter("ServerName", reportParams.serverName));
                passthroughParameters.Add(new ReportParameter("ServerVersion", reportParams.serverVersion==""?"":reportParams.serverVersion));
                passthroughParameters.Add(new ReportParameter("OSVersion", reportParams.OSVersion));
                passthroughParameters.Add(new ReportParameter("GUINumberOfProcessors", reportParams.processorCount.ToString()=="0"?"":reportParams.processorCount.ToString()));
                passthroughParameters.Add(new ReportParameter("GUIPhysicalMemory", reportParams.physicalMemoryKB.ToString() == "0" ? "" : (reportParams.physicalMemoryKB/1024/1024).ToString()));
                passthroughParameters.Add(new ReportParameter("GUIClustered", reportParams.IsClustered.ToString()));
                passthroughParameters.Add(new ReportParameter("GUIOwner", reportParams.owner));
                passthroughParameters.Add(new ReportParameter("GUITag", (localReportData.reportParameters["GUITag"] != null? localReportData.reportParameters["GUITag"].ToString():null)));

                dataSource.Value = RepositoryHelper.GetReportData("p_GetServerInventory",
                                                                  reportParams.serverName,
                                                                  reportParams.serverVersion,
                                                                  reportParams.OSVersion,
                                                                  reportParams.processorCount,
                                                                  reportParams.physicalMemoryKB,
                                                                  reportParams.IsClustered,
                                                                  reportParams.owner,
                                                                  reportParams.tagId,
                                                                  GetServerIdXml(servers));
                localReportData.dataSources[0] = dataSource;

                if (localReportData.cancelled)
                {
                    Log.Debug("reportData.cancelled = true.");
                    e.Cancel = true;
                }
                else
                {
                    e.Result = localReportData;
                }
            }
        }

        override protected void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            using (Log.DebugCall())
            {
                // Make sure this call is for the most recently requested report.
                Log.Debug("(reportData.bgWorker == sender) = ", reportData.bgWorker == sender);
                if (reportData.bgWorker == sender)
                {
                    // This event handler was called by the currently active report
                    if (reportData.cancelled)
                    {
                        Log.Debug("reportData.cancelled = true.");
                        return;
                    }
                    else if (e.Error != null)
                    {
                        if (e.Error.GetType() == typeof(System.Data.SqlClient.SqlException) &&
                            e.Error.Message.ToLower().Contains("msxmlsql.dll"))//
                        {
                            ApplicationMessageBox msgbox1 = new ApplicationMessageBox();
                            Exception msg = new Exception("An error occurred while retrieving data for the report.  It may be due to the problem described by the article available at http://support.microsoft.com/Default.aspx?kbid=918767", e.Error);
                            Log.Error("Showing message box: ", msg);
                            msgbox1.Message = msg;
                            msgbox1.SetCustomButtons("OK", "View Article");
                            msgbox1.Symbol = Microsoft.SqlServer.MessageBox.ExceptionMessageBoxSymbol.Error;
                            msgbox1.Show(this);
                            if (msgbox1.CustomDialogResult == Microsoft.SqlServer.MessageBox.ExceptionMessageBoxDialogResult.Button2)
                            {
                                Process.Start("http://support.microsoft.com/Default.aspx?kbid=918767");
                            }
                        }
                        else
                        {
                            ApplicationMessageBox.ShowError(this, "An error occurred while retrieving data for the report.  ",
                                            e.Error);
                        }

                        State = UIState.NoDataAcquired;
                    }
                    else
                    {
                        try
                        {
                            reportViewer.Reset();
                            reportViewer.LocalReport.EnableHyperlinks = true;

                            using (Stream stream = GetType().Assembly.GetManifestResourceStream(
                                "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.ServerInventory.rdl"))
                            {
                                reportViewer.LocalReport.LoadReportDefinition(stream);
                            }
                            foreach (ReportDataSource dataSource in reportData.dataSources)
                            {
                                reportViewer.LocalReport.DataSources.Add(dataSource);
                            }
                            
                            reportViewer.LocalReport.SetParameters(passthroughParameters);

                            reportViewer.RefreshReport();
                            reportViewer.LocalReport.DisplayName = "ServerInventory";
                            State = UIState.Rendered;
                        }
                        catch (Exception exception)
                        {
                            ApplicationMessageBox.ShowError(ParentForm, "An error occurred while refreshing the report.", exception);
                            State = UIState.ParmsNeeded;
                        }
                    }
                }
            }
        }
        #endregion

        #region Event Handlers

        public override void ResetFilterCriteria()
        {
            base.ResetFilterCriteria();
            searchItem2Combo.Items.Clear();
            searchItem3Combo.Items.Clear();
            searchItem4Combo.Items.Clear();
            InitGenericSearchCombo((ValueListItem)searchItem2Combo.SelectedItem, (ValueListItem)searchItem3Combo.SelectedItem, (ValueListItem)searchItem4Combo.SelectedItem, searchItem1Combo, "");
        }

        /// <summary>
        /// Common keypress event for the text boxes to prevent alphabetic input when being used for text input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void searchValueTextbox_onKeyPress(object sender, KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            string controlname = ((Control) sender).Name;
            bool blnSearchValueisNumeric = false;

            switch (controlname)
            {
                case "searchValue1Textbox":
                    blnSearchValueisNumeric = blnsearch1ValueisNumeric;
                    break;
                case "searchValue2Textbox":
                    blnSearchValueisNumeric = blnsearch2ValueisNumeric;
                    break;
                case "searchValue3Textbox":
                    blnSearchValueisNumeric = blnsearch3ValueisNumeric;
                    break;
                case "searchValue4Textbox":
                    blnSearchValueisNumeric = blnsearch4ValueisNumeric;
                    break;

            }
            if (!blnSearchValueisNumeric) return;

            NumberFormatInfo numberFormatInfo = CultureInfo.CurrentCulture.NumberFormat;
            string decimalSeparator = numberFormatInfo.NumberDecimalSeparator;
            
            string strText = ((TextBox)sender).Text;
            string keyInput = e.KeyChar.ToString();

            int intSelectedStart = ((TextBox)sender).SelectionStart;
            int intSelectedLength = ((TextBox)sender).SelectionLength;

            Regex rgx = new Regex("^\\d{1,5}([" + decimalSeparator + "]\\d{1,5}){0,2}$");
            
            int intCaretPosition = intSelectedStart + intSelectedLength;

            //if some text has been selected
            if (intSelectedLength > 0)
            {
                //take everything to the left
                string strLeft = strText.Substring(0, intSelectedStart);
                //take everything to the right
                string strRight = strText.Substring(intCaretPosition, strText.Length - intCaretPosition);

                if (rgx.IsMatch(strLeft + keyInput + strRight)) return;
            }
            else
            {   //if we are not at the start of the line
                if (rgx.IsMatch(strText.Insert(intSelectedStart,keyInput))) return;    
            }

            //if we have slipped past the regex it is because we are on a point between groups of numbers

            //Digits are fine as long as there are no more than 5 between points
            if (Char.IsDigit(e.KeyChar))
            {
                //get the number of chars since the beginning of the string or the last point
                int intChars = strText.IndexOf('.') == 0 ? strText.Length:strText.Length-strText.LastIndexOf('.');
                if (intChars <= 5) return;
            }
            
            // up to 3 Decimal separators are OK as long as they are not back to back
            if (keyInput.Equals(decimalSeparator))
            {
                //check if there are already 2 points
                if (strText.Trim().Length > 0)
                {
                    //first check the string for decimals. if there are not yet 2 decimals in the string 
                    //then potentially allow another one to go in
                    if ((Regex.Matches(strText, "[" + decimalSeparator + "]").Count) <= 2)
                    {
                        //if the last character is a not a point. cos we can't have back to back points
                        if ((strText.Substring(strText.Length - 1, 1) != decimalSeparator)) return;
                    }
                    //if we got here then there are too many points already 
                }
            }
            //group seperator and minus signs are not allowed
            //if(keyInput.Equals(groupSeparator) ||keyInput.Equals(negativeSign)) return;
            
            // Backspace key is OK
            if (e.KeyChar == '\b') return;

            // if we got here then this is not a valid key. 
            //Consume this invalid key and beep
            e.Handled = true;
        }
        /// <summary>
        /// Common eventhandler for handling the index changed of the search item combo boxes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void searchItemCombo_IndexChanged(object sender, EventArgs e)
        {
            ValueListItem[] otherSelectedSearchItems = new ValueListItem[3];
            ComboBox searchItemCombo = null;
            TextBox searchValueTextbox = null;
            ComboBox nextComboBox = null;
            ComboBox clusteredCombo = null;
            ComboBox tagsCombo = null;
            Label wildCardLabel = null;
            Label likeLabel = null;
            Label lessThanLabel = null;
            Panel memoryPanel = null;
            NumericUpDown updown = null;
            Label lblUpdown = null;

            switch(((Control) sender).Name)
            {
                case "searchItem2Combo":
                    otherSelectedSearchItems[0] = searchItem1Combo.SelectedItem as ValueListItem; 
                    otherSelectedSearchItems[1] = searchItem2Combo.SelectedItem as ValueListItem; 
                    otherSelectedSearchItems[2] = searchItem4Combo.SelectedItem as ValueListItem;
                    searchItemCombo = searchItem2Combo;
                    searchValueTextbox = searchValue2Textbox;
                    clusteredCombo = clusteredCombo2;
                    tagsCombo = tagsCombo2;
                    wildCardLabel = wildCardLabel2;
                    likeLabel = like2Label;
                    lessThanLabel = lessThanLabel2;
                    memoryPanel = memory2Panel;
                    blnsearch2ValueisNumeric = false;
                    nextComboBox = searchItem3Combo;
                    updown = memoryUpDown2;
                    lblUpdown = lblUpdown2;
                    break;
                case "searchItem3Combo":
                    otherSelectedSearchItems[0] = searchItem1Combo.SelectedItem as ValueListItem; 
                    otherSelectedSearchItems[1] = searchItem2Combo.SelectedItem as ValueListItem; 
                    otherSelectedSearchItems[2] = searchItem3Combo.SelectedItem as ValueListItem;
                    searchItemCombo = searchItem3Combo;
                    searchValueTextbox = searchValue3Textbox;
                    clusteredCombo = clusteredCombo3;
                    tagsCombo = tagsCombo3;
                    wildCardLabel = wildCardLabel3;
                    likeLabel = like3Label;
                    lessThanLabel = lessThanLabel3;
                    memoryPanel = memory3Panel;
                    blnsearch3ValueisNumeric = false;
                    nextComboBox = searchItem4Combo;
                    updown = memoryUpDown3;
                    lblUpdown = lblUpdown3;
                    break;
                case "searchItem4Combo":
                    otherSelectedSearchItems[0] = searchItem1Combo.SelectedItem as ValueListItem; 
                    otherSelectedSearchItems[1] = searchItem2Combo.SelectedItem as ValueListItem; 
                    otherSelectedSearchItems[2] = searchItem3Combo.SelectedItem as ValueListItem;
                    searchItemCombo = searchItem4Combo;
                    searchValueTextbox = searchValue4Textbox;
                    clusteredCombo = clusteredCombo4;
                    tagsCombo = tagsCombo4;
                    wildCardLabel = wildCardLabel4;
                    likeLabel = like4Label;
                    lessThanLabel = lessThanLabel4;
                    memoryPanel = memory4Panel;
                    blnsearch4ValueisNumeric = false;
                    updown = memoryUpDown4;
                    lblUpdown = lblUpdown4;
                    break;
                default:
                    otherSelectedSearchItems[0] = searchItem1Combo.SelectedItem as ValueListItem;
                    otherSelectedSearchItems[1] = searchItem3Combo.SelectedItem as ValueListItem;
                    otherSelectedSearchItems[2] = searchItem4Combo.SelectedItem as ValueListItem;
                    searchItemCombo = searchItem1Combo;
                    searchValueTextbox = searchValue1Textbox;
                    clusteredCombo = clusteredCombo1;
                    tagsCombo = tagsCombo1;
                    wildCardLabel = wildCardLabel1;
                    likeLabel = like1Label;
                    lessThanLabel = lessThanLabel1;
                    memoryPanel = memory1Panel;
                    blnsearch1ValueisNumeric = false;
                    nextComboBox = searchItem2Combo;
                    updown = memoryUpDown1;
                    lblUpdown = lblUpdown1;
                    break;
            }
            //This functions adds to this combo box whatever is not selected in the other combo boxes
            if (searchItemCombo != null && nextComboBox != null)
                InitGenericSearchCombo(otherSelectedSearchItems[0],
                                       otherSelectedSearchItems[1],
                                       otherSelectedSearchItems[2],
                                       ref nextComboBox, nextComboBox.Text, true);

            searchValueTextbox.Clear();

            searchValueTextbox.Show();

            if (searchItemCombo == null) return;

            wildCardLabel.Visible = searchItemCombo.SelectedItem != selectSearchItem;

            likeLabel.Hide();
            lessThanLabel.Hide();
            memoryPanel.Hide();
            clusteredCombo.Hide();
            tagsCombo.Hide();

            if ((searchItemCombo.SelectedItem == searchServerName) ||
                (searchItemCombo.SelectedItem == searchOSVersion) ||
                (searchItemCombo.SelectedItem == searchOwner))
            {
                likeLabel.Show();
            }

            if ((searchItemCombo.SelectedItem == searchPhysicalMemory) ||
                (searchItemCombo.SelectedItem == searchServerVersion) ||
                (searchItemCombo.SelectedItem == searchNumProcessors))
            {
                lessThanLabel.Show();
                wildCardLabel.Hide();

                switch (((Control) sender).Name)
                {
                    case "searchItem2Combo":
                        blnsearch2ValueisNumeric = true;
                        break;
                    case "searchItem3Combo":
                        blnsearch3ValueisNumeric = true;
                        break;
                    case "searchItem4Combo":
                        blnsearch4ValueisNumeric = true;
                        break;
                    default:
                        blnsearch1ValueisNumeric = true;
                        break;
                }
            }
            
            if (searchItemCombo.SelectedItem == searchPhysicalMemory || searchItemCombo.SelectedItem == searchNumProcessors)
            {
                searchValueTextbox.Hide();
                wildCardLabel.Hide();
                memoryPanel.Show();
                if(searchItemCombo.SelectedItem == searchPhysicalMemory)
                {
                    lblUpdown.Show();
                    updown.ReadOnly = false;

                    updown.DecimalPlaces = 1;
                    updown.Maximum = 128;
                    updown.Minimum = 1;
                    updown.Increment = 0.5m;
                    updown.Value = 1;
                }
                else
                {
                    updown.ReadOnly = false;
                    updown.DecimalPlaces = 0;
                    lblUpdown.Hide();
                    updown.Maximum = 128;
                    updown.Minimum = 1;
                    updown.Increment = 1;
                    updown.Value = 1;
                }
            }

            if (searchItemCombo.SelectedItem == searchClustered)
            {
                searchValueTextbox.Hide();
                wildCardLabel.Hide();
                clusteredCombo.Show();
                clusteredCombo.SelectedIndex = 0;
            }

            setupTagsCombo(searchItemCombo, tagsCombo, searchValueTextbox, wildCardLabel);

        }

        #endregion
        #region Functions
        /// <summary>
        /// Return available memory in KB
        /// </summary>
        /// <param name="txtAvailableMemory">Memory threshold in GB</param>
        /// <param name="ErrorDisplayText">Desciption of metric to display if there is an error in the function</param>
        /// <returns></returns>
        private long MemoryValueAdapter(string txtAvailableMemory, string ErrorDisplayText)
        {
            decimal memory = ValidateStringDecimal(txtAvailableMemory, ErrorDisplayText);
            if (memory < 0) memory = 0;
            return long.Parse(Math.Round(memory * 1024 * 1024).ToString());
        }

        private static void InitTagsCombo(ref ComboBox comboBox)
        {
            comboBox.Items.Clear();

            foreach (Tag tag in ApplicationModel.Default.Tags)
            {
                if (tag.Instances.Count > 0)
                {
                    comboBox.Items.Add(new ValueListItem(tag.Id, tag.Name));
                }
            }
        }
        /// <summary>
        /// Test that the string is indeed numeric
        /// </summary>
        /// <param name="strTest"></param>
        /// <returns></returns>
        private static bool IsNumeric(string strTest)
        {
            bool blnReturn = false;

            char[] chrTest = strTest.ToCharArray();
            for (int i = 0; i < chrTest.Length; i++)
            {

                if (!Char.IsNumber(chrTest[i]))
                {
                    blnReturn = false;
                    continue;
                }
                blnReturn = true;
            }
            return blnReturn;
        }
        /// <summary>
        /// Convert the string search value into a decimal
        /// </summary>
        /// <param name="searchValueTextBox"></param>
        /// <param name="Description"></param>
        /// <returns></returns>
        private decimal ValidateStringDecimal(string searchValueTextBox, string Description)
        {
            decimal returnValue = -1;
            if (!IsNumeric(searchValueTextBox))
            {
                ApplicationMessageBox.ShowInfo(ParentForm, "You are attempting to search on an invalid value for " + Description + ". Please check the search value and try again.");
            }
            else
            {
                returnValue = decimal.Parse(searchValueTextBox, NumberStyles.Number);
            }
            return returnValue;
        }

        /// <summary>
        /// Check if string contains a number then convert it
        /// </summary>
        /// <param name="searchValueTextBox"></param>
        /// <param name="Description"></param>
        /// <returns></returns>
        private long ValidateStringInt64(string searchValueTextBox, string Description)
        {
            long returnValue = -1;
            if (!IsNumeric(searchValueTextBox))
            {
                ApplicationMessageBox.ShowInfo(ParentForm, "You are attempting to search on an invalid value for " + Description + ". Please check the search value and try again.");
            }
            else
            {
                returnValue = long.Parse(searchValueTextBox, System.Globalization.NumberStyles.Number);
            }
            return returnValue;
        }

        /// <summary>
        /// Verify that the string contains a number and then convert it to a number
        /// </summary>
        /// <param name="searchValueTextBox"></param>
        /// <param name="Description"></param>
        /// <returns></returns>
        private int ValidateStringInt32(string searchValueTextBox, string Description)
        {
            int returnValue = -1;
            if(!IsNumeric(searchValueTextBox))
            {
                ApplicationMessageBox.ShowInfo(ParentForm, "You are attempting to search on an invalid number for " + Description + ". Please check the search value and try again.");
            }
            else
            {
                returnValue = int.Parse(searchValueTextBox);
            }
            return returnValue;
        }

        /// <summary>
        /// Pass in a tags combo to check for validity of the selection
        /// Ensure that it still exists and warn if it does not exist.
        /// If selection is invalid then set the combo to point at item 0
        /// </summary>
        /// <param name="_tagsCombo"></param>
        /// <returns></returns>
        public int ValidateSelectedTagID(ComboBox _tagsCombo)
        {
            //if we are searching on tag, ensure that we have some tags in the combo box
            if (_tagsCombo.Items.Count > 0)
            {
                ValueListItem selectedItem = (ValueListItem)_tagsCombo.SelectedItem;
                if (selectedItem.DataValue == null)
                {
                    return 0;
                }
                int intSelectedTagID = (int)(selectedItem.DataValue);

                //get the tag object
                Tag tag = GetTag(intSelectedTagID);
                if (tag == null)
                {
                    _tagsCombo.Items.Remove(_tagsCombo.SelectedItem);
                    _tagsCombo.SelectedIndex = 0;
                    ApplicationMessageBox.ShowInfo(ParentForm, "The selected tag no longer exists. Please select a tag.");
                }
                else
                {
                    return (int)((ValueListItem)_tagsCombo.SelectedItem).DataValue;
                }
            }
            else
            {
                ApplicationMessageBox.ShowInfo(ParentForm, "You do not have any tags defined. Please search again.");
            }
            return 0;
        }
        /// <summary>
        /// Populate the report params. Depending on the selected item in the combo the relevant parameter is extracted from a control
        /// </summary>
        /// <param name="parameters">This object is updated and passed back to the caller</param>
        /// <param name="comboSelectedItem">This doctates which parameter will be extracted from which control</param>
        /// <param name="searchValueText">search text box control</param>
        /// <param name="tagsCombo">tag combo control</param>
        /// <param name="memoryThresholdtext">memory updown control</param>
        /// <param name="clusteredCombotext"></param>
        /// <returns>ServerInventory params object with updated parameters</returns>
        private ServerInventoryParams setServerInventoryParams(ServerInventoryParams parameters, ValueListItem comboSelectedItem, string searchValueText, ComboBox tagsCombo, string memoryThresholdtext, string clusteredCombotext)
        {
            ServerInventoryParams returnValue = parameters;

            if (comboSelectedItem != null)
            {
                switch ((ServerInventorySearchCriteria) comboSelectedItem.DataValue)
                {
                    case ServerInventorySearchCriteria.selectSearchItem:
                        break;
                    case ServerInventorySearchCriteria.ServerName:
                        returnValue.serverName = searchValueText;
                        break;
                    case ServerInventorySearchCriteria.ServerTag:
                        returnValue.tagId = ValidateSelectedTagID(tagsCombo);
                        break;
                    case ServerInventorySearchCriteria.ServerVersion:
                        returnValue.serverVersion = searchValueText;
                        break;
                    case ServerInventorySearchCriteria.Owner:
                        returnValue.owner = searchValueText;
                        break;
                    case ServerInventorySearchCriteria.OSVersion:
                        returnValue.OSVersion = searchValueText;
                        break;
                    case ServerInventorySearchCriteria.NumberOfProcessors:
                        returnValue.processorCount =
                            ValidateStringInt32(memoryThresholdtext, searchNumProcessors.DisplayText);
                        break;
                    case ServerInventorySearchCriteria.PhysicalMemory:
                        returnValue.physicalMemoryKB = MemoryValueAdapter(memoryThresholdtext, searchPhysicalMemory.DisplayText);
                        break;
                    case ServerInventorySearchCriteria.Clustered:
                        if (!string.IsNullOrEmpty(clusteredCombotext))
                        {
                            returnValue.IsClustered = clusteredCombotext == "True" ? true : false;
                        }
                        break;
                }
            }
            return returnValue;
        }

        /// <summary>
        /// based on the filter on tags filter selection, poulate the tags combo box and select the first tag
        /// </summary>
        /// <param name="SearchItemCombo"></param>
        /// <param name="tagsCombo"></param>
        /// <param name="searchValueTextBox"></param>
        /// <param name="wildCardlabel"></param>
        private void setupTagsCombo(ComboBox SearchItemCombo, ComboBox tagsCombo, Control searchValueTextBox, Control wildCardlabel)
        {
            if (!(SearchItemCombo.SelectedItem is ValueListItem)) return;

            if (SearchItemCombo.SelectedItem != searchServerTag) return;

            InitTagsCombo(ref tagsCombo);
            searchValueTextBox.Hide();
            wildCardlabel.Hide();

            if (tagsCombo.Items.Count > 0)
            {
                searchValueTextBox.Hide();
                wildCardlabel.Hide();
                tagsCombo.Show();
                tagsCombo.SelectedIndex = 0;
            }
            else
            {
                tagsCombo.Items.Clear();
                ApplicationMessageBox.ShowInfo(ParentForm, "You do not have any tags defined. Please select an alternative search item.");
                SearchItemCombo.SelectedIndex = 0;
            }
        }

        #endregion

        override protected void reportViewer_Drillthrough(object sender, DrillthroughEventArgs e)
        {
            using (Log.DebugCall())
            {
                e.Cancel = true;
                Log.Debug("e.ReportPath = ", e.ReportPath);

                if (e.ReportPath == ReportsHelper.GetReportTitle(ReportTypes.ServerSummary))
                {
                    ApplicationController.Default.ShowReportsView(ReportTypes.ServerSummary, e);
                }
            }
        }
    }
}

