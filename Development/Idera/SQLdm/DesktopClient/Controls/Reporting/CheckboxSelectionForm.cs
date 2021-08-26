using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using Idera.SQLdm.DesktopClient.Helpers;

namespace Idera.SQLdm.DesktopClient.Controls.Reporting {
    public partial class CheckboxSelectionForm : Form {
        public CheckboxSelectionForm() {
            InitializeComponent();
        }

        // Give the client full access to the CheckedListBox.
        public CheckedListBox TheListBox {
            get { return checkedListBox1; }
        }

        private Control _returnFocus;

        // Show the form right under the specified control and
        // make it the same width.
        public void ShowUnderControl(Control ctrl) {
            Point location = ctrl.PointToScreen(new Point(0, ctrl.Height-1));
            this.Location = location;
            Debug.Print("Location set to " + this.Location);
            this.Width = ctrl.Width;
            if (checkedListBox1.Items.Count > 1) {
                Rectangle loc1 = checkedListBox1.GetItemRectangle(0);
                Rectangle loc2 = checkedListBox1.GetItemRectangle(1);
                int delta = loc2.Y - loc1.Y;
                this.Height = delta * checkedListBox1.Items.Count + 5;
            } else {
                this.Height = 21;
            }

            // Don't allow this form to extend past the bottom of the screen.
            Rectangle screen = Screen.GetWorkingArea(this);
            if (this.Bottom > screen.Bottom) {
                this.Height -= (this.Bottom - screen.Bottom);
            }

            _returnFocus = ctrl;
            var owner = Program.MainWindow.GetWinformWindow();
            this.Show(owner);
        }

        // This generally called just before this form is disposed, so
        // copy the selected indices into a new array.
        public int[] GetCheckedIndices() {
            int[] checkedIndices = new int[checkedListBox1.CheckedIndices.Count];
            checkedListBox1.CheckedIndices.CopyTo(checkedIndices, 0);
            return checkedIndices;
        }

        // Show checkmarks on the specified items.
        public void SetCheckedIndices(int[] checkedItems) {
            if (checkedItems != null) {
                foreach (int i in checkedItems) {
                    checkedListBox1.SetItemChecked(i, true);
                }
            }
        }

        // Get a comma separated list of the selected items.
        public string GetCommaSeparatedList() {
            StringBuilder csv = new StringBuilder();
            foreach (object o in checkedListBox1.CheckedItems) {
                csv.Append(o.ToString());
                csv.Append(", ");
            }

            if (csv.Length > 2) csv.Length = csv.Length - 2;
            return csv.ToString();
        }

        // Close the form when the user clicks outside of it.
        protected override void OnDeactivate(EventArgs e) {
            base.OnDeactivate(e);
            this.Close();
            _returnFocus.Focus();
        }

    }
}