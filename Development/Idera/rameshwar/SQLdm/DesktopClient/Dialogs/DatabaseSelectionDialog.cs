using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinListView;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    using Helpers;

    public partial class DatabaseSelectionDialog : Form
    {
        private List<string> listItems = new List<string>();
        private List<string> selectedItems = new List<string>();

        public DatabaseSelectionDialog()
        {
            InitializeComponent();
            databaseListBox.DrawFilter = new HideFocusRectangleDrawFilter();

            databaseListBox.Items.Clear();
            UpdateControls();
            AdaptFontSize();
        }

        public string Comments
        {
            get { return commentsInformationBox.Text; }
            set
            {
                if (String.IsNullOrEmpty(value))
                    commentsInformationBox.Visible = false;
                else
                {
                    commentsInformationBox.Text = value;
                    commentsInformationBox.Visible = true;
                }
            }
        }

        /// <summary>
        /// Sets the items to show in the list.
        /// </summary>
        public IEnumerable<string> ListItems
        {
            get
            {
                return new List<string>(listItems);
            }
            set
            {
                listItems.Clear();

                if (value != null)
                {
                    listItems.AddRange(value);
                }

                BuildList();
            }
        }

        /// <summary>
        /// Sets the list of selected items
        /// </summary>
        public IList<string> SelectedItems
        {
            get { return selectedItems; }
            set
            {
                selectedItems.Clear();

                if (value != null)
                    selectedItems.AddRange(value);

                BuildList();
            }
        }

        /// <summary>
        /// Adds an item to the list control with the specified text.
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(string item)
        {
            if (listItems.Contains(item))
                return;
            listItems.Add(item);
            UltraListViewItem ulvi = databaseListBox.Items.Add(item, item);
            ulvi.CheckState = (selectedItems.Contains(item)) ?
                CheckState.Checked : CheckState.Unchecked;
        }

        protected void BuildList()
        {
            databaseListBox.Items.Clear();
            foreach (string item in listItems)
            {
                UltraListViewItem ulvi = databaseListBox.Items.Add(item, item);
                ulvi.CheckState = (selectedItems.Contains(item)) ?
                    CheckState.Checked : CheckState.Unchecked;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            selectedItems.Clear();
            foreach (UltraListViewItem checkedItem in databaseListBox.CheckedItems)
            {
                string value = (string)checkedItem.Value;
                selectedItems.Add(value);
            }
        }

        private void noneButton_Click(object sender, EventArgs e)
        {
            foreach (UltraListViewItem item in databaseListBox.Items)
                item.CheckState = CheckState.Unchecked;
        }

        private void UpdateControls()
        {
            noneButton.Enabled = databaseListBox.CheckedItems.Count > 0;
        }

        private void databaseListBox_ItemCheckStateChanged(object sender, ItemCheckStateChangedEventArgs e)
        {
            UpdateControls();
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