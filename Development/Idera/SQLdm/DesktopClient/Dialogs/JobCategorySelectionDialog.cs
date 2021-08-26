using Idera.SQLdm.DesktopClient.Helpers;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;
    using Infragistics.Win.UltraWinListView;

    public partial class JobCategorySelectionDialog : BaseDialog
    {
        private List<string> listItems = new List<string>();
        private List<string> selectedItems = new List<string>();

        public JobCategorySelectionDialog()
        {
            this.DialogHeader = "Excluded Job Categories";
            InitializeComponent();

            jobCategoriesListBox.Items.Clear();
            UpdateControls();
            AdaptFontSize();
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
            UltraListViewItem ulvi = jobCategoriesListBox.Items.Add(item,item);
            ulvi.CheckState = (selectedItems.Contains(item)) ?
                CheckState.Checked : CheckState.Unchecked;
        }

        protected void BuildList()
        {
            jobCategoriesListBox.Items.Clear();
            foreach (string item in listItems)
            {
                UltraListViewItem ulvi = jobCategoriesListBox.Items.Add(item, item);
                ulvi.CheckState = (selectedItems.Contains(item)) ?
                    CheckState.Checked : CheckState.Unchecked;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            selectedItems.Clear();
            foreach (UltraListViewItem checkedItem in jobCategoriesListBox.CheckedItems)
            {
                string value = (string) checkedItem.Value;
                selectedItems.Add(value);
            }
        }

        private void noneButton_Click(object sender, EventArgs e)
        {
            foreach (UltraListViewItem item in jobCategoriesListBox.Items)
                item.CheckState = CheckState.Unchecked;
        }

        private void UpdateControls()
        {
            noneButton.Enabled = jobCategoriesListBox.CheckedItems.Count > 0;
        }

        private void jobCategoriesListBox_ItemCheckStateChanged(object sender, ItemCheckStateChangedEventArgs e)
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