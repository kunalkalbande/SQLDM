namespace Idera.SQLdm.DesktopClient.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;
    using Helpers;
    using Properties;
    using Wintellect.PowerCollections;

    public partial class SelectDrivesDialog : BaseDialog, IEqualityComparer<string>
    {
        private int serverId;
        private Set<string> selectedDrives;
        private bool ignoreCheckedChanged = false;
        
        public SelectDrivesDialog(int serverId, IEnumerable<string> selectedDrives)
        {
            this.DialogHeader = "Excluded Disk Drives";
            this.serverId = serverId;
            
            this.selectedDrives = new Set<string>(this);
            if (selectedDrives != null)
                this.selectedDrives.AddMany(selectedDrives);

            InitializeComponent();
            AdaptFontSize();
        }

        public IEnumerable<string> SelectedDrives
        {
            get
            {
                return selectedDrives;
            }
        }

        private void SelectDrivesDialog_Load(object sender, EventArgs e)
        {
            driveListBox.Items.Clear();
            foreach (string drive in SelectedDrives)
            {
                driveListBox.Items.Add(drive, true);
            }
            
            List<string> discovered =
                RepositoryHelper.GetDriveList(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, serverId);
            
            foreach (string drive in discovered)
            {
                if (!selectedDrives.Contains(drive))
                    driveListBox.Items.Add(drive, false);
            }
        }

        private void driveListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            bool enabled = false;

            if (driveListBox.CheckedItems.Count > 1)
                enabled = true;
            else
                enabled = e.NewValue == CheckState.Checked;

            clearButton.Enabled = enabled;
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            clearButton.Enabled = driveListBox.CheckedItems.Count > 0;
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            while (driveListBox.CheckedIndices.Count > 0)
            {
                int checkedX = driveListBox.CheckedIndices[0];
                driveListBox.SetItemChecked(checkedX, false);
            }
            ignoreCheckedChanged = true;
            selectAllDrivesCheckBox.Checked = false;
            ignoreCheckedChanged = false;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            selectedDrives.Clear();
            foreach (string drive in driveListBox.CheckedItems)
            {
                selectedDrives.Add(drive);
            }
        }

        #region IEqualityComparer<string> Members

        public bool Equals(string x, string y)
        {
            return String.Equals(x.ToUpper(), y.ToUpper());
        }

        public int GetHashCode(string obj)
        {
            return obj.ToUpper().GetHashCode();
        }

        #endregion

        private void selectAllDrivesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!ignoreCheckedChanged)
            {
                ignoreCheckedChanged = true;
                for (int i = 0; i < driveListBox.Items.Count; i++)
                {
                    driveListBox.SetItemChecked(i, selectAllDrivesCheckBox.Checked);
                }
                ignoreCheckedChanged = false;
            }
        }

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }
    }
}