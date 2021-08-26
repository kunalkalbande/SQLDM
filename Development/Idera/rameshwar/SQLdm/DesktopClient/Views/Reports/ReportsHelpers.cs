using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Views.Reports {
    internal static class ReportsHelpers {
        // Get the ListBox's selected items in a comma-separated list.
        public static string GetSelectedItemsAsCSVList(ListBox listbox) {
            string ret = string.Empty;
            foreach (object selected in listbox.SelectedItems) {
                ret += selected.ToString() + ", ";
            }

            // Remove the last ", ".
            if (ret.Length > 2) ret = ret.Substring(0, ret.Length - 2);
            return ret;
        }

        // Parse the comma-separated list and select the matching items in the ListBox.
        public static void SelectItemsInCSVList(ListBox listbox, string csv) {
            if (csv != null) {
                foreach (string parsed in csv.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)) {
                    string s = parsed.Trim();
                    foreach (object listed in listbox.Items) {
                        if (string.Compare(s, listed.ToString(), true) == 0) {
                            listbox.SelectedItems.Add(listed);
                            break;
                        }
                    }
                }
            }
        }

        // Get the ListView's selected items in a comma-separated list.
        public static string GetSelectedItemsAsCSVList(ListView listview) {
            string ret = string.Empty;
            foreach (ListViewItem item in listview.SelectedItems) {
                ret += item.Text + ", ";
            }

            // Remove the last ", ".
            if (ret.Length > 2) ret = ret.Substring(0, ret.Length - 2);
            return ret;
        }

        // Parse the comma-separated list and select the matching items in the ListView.
        public static void SelectItemsInCSVList(ListView listview, string csv) {
            if (csv != null) {
                foreach (string parsed in csv.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)) {
                    string s = parsed.Trim();
                    foreach (ListViewItem item in listview.Items) {
                        if (string.Compare(s, item.Text, true) == 0) {
                            item.Selected = true;
                            break;
                        }
                    }
                }
            }
        }
    }
}
