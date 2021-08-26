using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Idera.SQLdm.DesktopClient.Helpers;

namespace Idera.SQLdm.DesktopClient.Controls
{
    /// <summary>
    /// Two listboxes with Add and Remove buttons to transfer items between them.
    /// Supports resizing by keeping the buttons centered between the listboxes and
    /// resizing the listboxes to keep the space between a constant width.
    /// </summary>
    internal partial class DualListSelectorControl : UserControl
    {

        /// <summary>
        /// Event fired when the user clicks either the add or remove buttons.
        /// The event will not fire if the lists are modified in code.
        /// </summary>
        public event EventHandler SelectionChanged;

        /// <summary>
        /// The list box containing the available items.
        /// </summary>
        public ListBox Available { get { return listBoxAvailable; } }

        /// <summary>
        /// Text above the Available list.
        /// </summary>
        public string AvailableLabel
        {
            get { return label3.Text; }
            set { label3.Text = value; }
        }

        /// <summary>
        /// The list box containing the selected items.
        /// </summary>
        public ListBox Selected { get { return listBoxSelected; } }

        /// <summary>
        /// Text above the Available list.
        /// </summary>
        public string SelectedLabel
        {
            get { return label1.Text; }
            set { label1.Text = value; }
        }

        // The width of the area between the two list boxes.
        // Initialized in the ctor and referenced in OnResize
        // to adjust the width of the list boxes.
        private int _middleWidth;

        // The initial distance the left edge of the add and remove buttons
        // and the right edge of panel1.
        private int _buttonOffset;

        public DualListSelectorControl()
        {
            InitializeComponent();
            if (AutoScaleSizeHelper.isScalingRequired)
                this.buttonRemove.Size = this.buttonAdd.Size;
            //Saurabh - SQLDM-30848 - UX-Modernization, PRD 4.2
            //if (AutoScaleSizeHelper.isScalingRequired && !isCustomCounter)
                //ScaleControlsAsPerResolution();
        }

        private void listBoxAvailable_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonAdd.Enabled = (listBoxAvailable.SelectedIndex != -1);
        }

        private void listBoxSelected_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonRemove.Enabled = CanRemoveItems();
        }

        private bool CanRemoveItems()
        {
            bool canRemoveItems = false;

            foreach (object selectedItem in listBoxSelected.SelectedItems)
            {
                if (!(selectedItem is ExtendedListItem))
                {
                    canRemoveItems = true;
                }
                else if (((ExtendedListItem)selectedItem).CanRemove)
                {
                    canRemoveItems = true;
                }

                if (canRemoveItems) break;
            }

            return canRemoveItems;
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            while (listBoxAvailable.SelectedItem != null)
            {
                listBoxSelected.Items.Add(listBoxAvailable.SelectedItem);
                listBoxAvailable.Items.Remove(listBoxAvailable.SelectedItem);
            }
            OnSelectionChanged();
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            System.Collections.ArrayList selectedItems = new System.Collections.ArrayList(listBoxSelected.SelectedItems);
            listBoxSelected.SuspendLayout();
            try
            {
                foreach (object selectedItem in selectedItems)
                {
                    if (!(selectedItem is ExtendedListItem) ||
                        ((ExtendedListItem)selectedItem).CanRemove)
                    {
                        listBoxAvailable.Items.Add(selectedItem);
                        listBoxSelected.Items.Remove(selectedItem);
                    }
                }
                listBoxSelected.SelectedItems.Clear();
            }
            finally
            {
                listBoxSelected.ResumeLayout();
            }

            OnSelectionChanged();
        }

        protected void OnSelectionChanged()
        {
            if (SelectionChanged != null)
                SelectionChanged(this, EventArgs.Empty);
        }

        private void listBoxAvailable_DoubleClick(object sender, EventArgs e)
        {
            while (listBoxAvailable.SelectedItem != null)
            {
                listBoxSelected.Items.Add(listBoxAvailable.SelectedItem);
                listBoxAvailable.Items.Remove(listBoxAvailable.SelectedItem);
            }

            OnSelectionChanged();
        }

        private void listBoxSelected_DoubleClick(object sender, EventArgs e)
        {
            if (listBoxSelected.SelectedItem is ExtendedListItem)
            {
                if (((ExtendedListItem)listBoxSelected.SelectedItem).CanRemove)
                {
                    listBoxAvailable.Items.Add(listBoxSelected.SelectedItem);
                    listBoxSelected.Items.Remove(listBoxSelected.SelectedItem);
                }
            }
            else
            {
                listBoxAvailable.Items.Add(listBoxSelected.SelectedItem);
                listBoxSelected.Items.Remove(listBoxSelected.SelectedItem);
            }

            OnSelectionChanged();
        }

        private void ListBoxDrawItem(object sender, DrawItemEventArgs e)
        {
            ListBox listBox = sender as ListBox;

            if (listBox != null)
            {
                e.DrawBackground();

                if (e.Index >= 0)
                {
                    Color foreColor;

                    if ((e.State & DrawItemState.Selected) > 0)
                    {
                        foreColor = Color.White;
                    }
                    else
                    {
                        foreColor = listBox.Items[e.Index] is ExtendedListItem
                                        ? ((ExtendedListItem)listBox.Items[e.Index]).TextColor
                                        : ForeColor;
                    }

                    using (Brush textBrush = new SolidBrush(foreColor))
                    {
                        e.Graphics.DrawString(listBox.Items[e.Index].ToString(), e.Font, textBrush, e.Bounds,
                                              StringFormat.GenericDefault);
                    }
                }

                e.DrawFocusRectangle();
            }
        }

        //Saurabh - SQLDM-30848 - UX-Modernization, PRD 4.2
        private void ListBoxMeasureItem(object sender, MeasureItemEventArgs e)
        {
            if(AutoScaleSizeHelper.isScalingRequired)
            {
                if(AutoScaleSizeHelper.isLargeSize)
                {
                    e.ItemHeight += 10;
                    return;
                }
                if (AutoScaleSizeHelper.isXLargeSize)
                {
                    e.ItemHeight += 12;
                    return;
                }
                if (AutoScaleSizeHelper.isXXLargeSize)
                {
                    e.ItemHeight += 15;
                    return;
                }
            }
        }
        //Saurabh - SQLDM-30848 - UX-Modernization, PRD 4.2
        protected void ScaleControlsAsPerResolution()
        {
            this.listBoxAvailable.DrawMode = DrawMode.OwnerDrawVariable;
            this.listBoxSelected.DrawMode = DrawMode.OwnerDrawVariable;
        }
    }

    internal class ExtendedListItem
    {
        public string Text;
        public object Tag;
        public Color TextColor = SystemColors.WindowText;
        public bool CanRemove = true;

        public ExtendedListItem(string text)
        {
            Text = text;
        }

        public ExtendedListItem(string text, object tag)
            : this(text)
        {
            Tag = tag;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
