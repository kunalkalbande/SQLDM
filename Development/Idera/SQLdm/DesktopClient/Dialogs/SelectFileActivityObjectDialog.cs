using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Idera.SQLdm.DesktopClient.Helpers;
using Infragistics.Win.UltraWinGrid;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    internal partial class SelectFileActivityObjectDialog : BaseDialog
    {
        public enum ObjectType
        {
            Disk,
            Database,
            File
        }

        private readonly ObjectType objectType;
        private readonly List<string> availableObjects;
        private readonly List<string> selectedObjects;

        public SelectFileActivityObjectDialog(ObjectType objectType, IEnumerable<string> available, IEnumerable<string> selected)
        {
            this.DialogHeader = "Select Object";
            InitializeComponent();

            this.objectType = objectType;

            string type = Enum.GetName(typeof (ObjectType), objectType) + "s";
            this.Text = string.Format("Select {0}", type);
            infoLabel.Text = string.Format("Select {0} to display the associated file activity.", type);
            availableGroupBox.Text = string.Format("Available {0}", type);
            selectedGroupBox.Text = string.Format("Selected {0}", type);

            availableObjects = new List<string>(available);
            selectedObjects = new List<string>();

            Initialize(selected);
            AdaptFontSize();
        }

        private void Initialize(IEnumerable<string> selected)
        {
            availableObjectsListBox.Items.Clear();
            if (availableObjects.Count > 0)
            {
                foreach (var item in availableObjects)
                {
                    availableObjectsListBox.Items.Add(item);
                }

                List<string> list = selected.ToList();
                if (list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        if (availableObjectsListBox.Items.Contains(item))
                        {
                            availableObjectsListBox.SelectedItems.Add(item);
                        }
                    }
                    AddItems();
                }
            }
            else
            {
                availableStatusLabel.BringToFront();
            }
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            addButton.Enabled = (availableObjectsListBox.SelectedItems.Count > 0);
            removeButton.Enabled = (selectedObjectsListBox.SelectedItems.Count > 0);
            okButton.Enabled = (selectedObjects.Count > 0);
        }

        private void AddItems()
        {
            string[] selectedItems = new string[availableObjectsListBox.SelectedItems.Count];
            availableObjectsListBox.SelectedItems.CopyTo(selectedItems, 0);
            availableObjectsListBox.SelectedItems.Clear();

            foreach (var item in selectedItems)
            {
                availableObjectsListBox.Items.Remove(item);
                selectedObjects.Add(item);
                selectedObjectsListBox.Items.Add(item);
            }

            UpdateButtons();
        }

        private void RemoveItems()
        {
            string[] selectedItems = new string[selectedObjectsListBox.SelectedItems.Count];
            selectedObjectsListBox.SelectedItems.CopyTo(selectedItems, 0);
            selectedObjectsListBox.SelectedItems.Clear();

            foreach (var item in selectedItems)
            {
                selectedObjectsListBox.Items.Remove(item);
                selectedObjects.Remove(item);
                availableObjectsListBox.Items.Add(item);
            }

            UpdateButtons();
        }

        public List<string> SelectedObjects
        {
            get { return selectedObjects; }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            AddItems();
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            RemoveItems();
        }

        private void availableObjectsListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            UpdateButtons();
        }

        private void selectedObjectsListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            UpdateButtons();
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
