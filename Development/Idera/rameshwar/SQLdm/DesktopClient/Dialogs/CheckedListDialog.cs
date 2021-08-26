using Idera.SQLdm.DesktopClient.Helpers;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;
    using Infragistics.Win.UltraWinListView;

    public partial class CheckedListDialog<ItemType> : Form
    {
        private List<Wrapper<ItemType>> listItems  = new List<Wrapper<ItemType>>();
        private List<ItemType> selectedItems = new List<ItemType>();

        public CheckedListDialog()
        {
            InitializeComponent();

            ultraListView1.Items.Clear();
            AdaptFontSize();
        }

        /// <summary>
        /// Sets the items to show in the list.
        /// </summary>
        public IEnumerable<ItemType> ListItems
        {
            get
            {
                List<ItemType> result = new List<ItemType>(listItems.Count);
                foreach (Wrapper<ItemType> item in listItems)
                    result.Add(item.Item);
                return result;
            }
            set
            {
                listItems.Clear();
                
                if (value != null)
                {
                    foreach (ItemType item in value)
                    {
                        Wrapper<ItemType> wrapper = new Wrapper<ItemType>(item.ToString(), item);
                        listItems.Add(wrapper);
                    }                
                }

                BuildList();
            }
        } 

        /// <summary>
        /// Sets the list of selected items
        /// </summary>
        public IList<ItemType> SelectedItems
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
        /// <param name="text"></param>
        /// <param name="item"></param>
        public void AddItem(string text, ItemType item, bool isChecked)
        {
            Wrapper<ItemType> wrapper = new Wrapper<ItemType>(text, item);
            if (listItems.Contains(wrapper))
                return;

            listItems.Add(wrapper);
            UltraListViewItem ulvi = ultraListView1.Items.Add(text, wrapper);
            ulvi.CheckState = isChecked ? CheckState.Checked : CheckState.Unchecked;
        }

        protected void BuildList()
        {
            ultraListView1.Items.Clear();
            foreach (Wrapper<ItemType> wrapper in listItems)
            {
                UltraListViewItem ulvi = ultraListView1.Items.Add(wrapper.Text, wrapper.Item);
                ulvi.CheckState = (selectedItems.Contains(wrapper.Item)) ?
                    CheckState.Checked : CheckState.Unchecked;
            }   
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            selectedItems.Clear();
            foreach (UltraListViewItem checkedItem in ultraListView1.CheckedItems)
            {
                Wrapper<ItemType> wrapper = (Wrapper<ItemType>)checkedItem.Value;
                selectedItems.Add(wrapper.Item);
            }
        }

        private void cbCheckitems_CheckedChanged(object sender, EventArgs e)
        {
            foreach (UltraListViewItem item in ultraListView1.Items)
            {
                item.CheckState = cbCheckitems.Checked ? CheckState.Checked : CheckState.Unchecked;
            }
        }        

        internal class Wrapper<T>
        {
            internal readonly string Text;
            internal readonly T Item;

            internal Wrapper(string text, T item)
            {
                Text = text;
                Item = item;
            }

            public override string ToString()
            {
                return Text ?? Item.ToString();
            } 
        }

        private void ultraListView1_ItemCheckStateChanged(object sender, ItemCheckStateChangedEventArgs e)
        {
            okButton.Enabled = ultraListView1.CheckedItems.Count > 0;
        }

        private void CheckedListDialog_Load(object sender, EventArgs e)
        {
            cbCheckitems.Checked = ultraListView1.CheckedItems.Count == ultraListView1.Items.Count;
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