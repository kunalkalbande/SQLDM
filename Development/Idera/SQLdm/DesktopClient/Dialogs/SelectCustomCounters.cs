using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Idera.SQLdm.DesktopClient.Views.Administration;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    internal partial class SelectCustomCounters : BaseDialog
    {
        public SortedList<string, CustomCounter> availableCounters { get; private set; }
        public SortedList<string, CustomCounter> selectedCounters { get; private set; }

        public SelectCustomCounters(IEnumerable<CustomCounter> counters, IEnumerable<CustomCounter> selected, IEnumerable<int> linkedCounters )
        {
            InitializeComponent();

            availableCounters = new SortedList<string, CustomCounter>();
            foreach(var counter in counters)
            {
                if (linkedCounters != null)
                {
                    if (linkedCounters.Contains(counter.MetricID))
                        availableCounters.Add(counter.Name, counter);
                }
                else
                {
                    availableCounters.Add(counter.Name, counter);
                }
            }
            selectedCounters = new SortedList<string, CustomCounter>();

            Initialize(selected);
        }

        private void Initialize(IEnumerable<CustomCounter> selected)
        {
            availableCountersListBox.Items.Clear();
            if (availableCounters.Count > 0)
            {
                foreach (var counter in availableCounters)
                {
                    availableCountersListBox.Items.Add(counter.Key);
                }

                List<CustomCounter> list = selected.ToList();
                if (list.Count > 0)
                {
                    foreach (var counter in list)
                    {
                        if (availableCountersListBox.Items.Contains(counter.Name))
                        {
                            availableCountersListBox.SelectedItems.Add(counter.Name);
                        }
                    }
                    AddCounters();
                }
            }
            else
            {
                availableCountersLabel.BringToFront();
            }
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            addCountersButton.Enabled = (availableCountersListBox.SelectedItems.Count > 0);
            removeCountersButton.Enabled = (selectedCountersListBox.SelectedItems.Count > 0);
            okButton.Enabled = (selectedCounters.Count > 0);
        }

        private void AddCounters()
        {
            string[] selectedItems = new string[availableCountersListBox.SelectedItems.Count];
            availableCountersListBox.SelectedItems.CopyTo(selectedItems, 0);
            availableCountersListBox.SelectedItems.Clear();

            foreach (var item in selectedItems)
            {
                availableCountersListBox.Items.Remove(item);
                selectedCounters.Add(item, availableCounters[item]);
                selectedCountersListBox.Items.Add(item);
            }

            UpdateButtons();
        }

        private void RemoveCounters()
        {
            string[] selectedItems = new string[selectedCountersListBox.SelectedItems.Count];
            selectedCountersListBox.SelectedItems.CopyTo(selectedItems, 0);
            selectedCountersListBox.SelectedItems.Clear();

            foreach (var item in selectedItems)
            {
                selectedCountersListBox.Items.Remove(item);
                selectedCounters.Remove(item);
                availableCountersListBox.Items.Add(item);
            }

            UpdateButtons();
        }

        private void addCountersButton_Click(object sender, EventArgs e)
        {
            AddCounters();
        }

        private void removeCountersButton_Click(object sender, EventArgs e)
        {
            RemoveCounters();
        }

        private void availableCountersListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            UpdateButtons();
        }

        private void selectedCountersListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            UpdateButtons();
        }
    }
}
