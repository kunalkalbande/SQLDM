namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;
    using Helpers;
    using Common.Notification.Providers;
    using Common.Services;
    using Common.UI.Dialogs;
    using Infragistics.Win.Misc;

    public partial class ProgramDestinationDialog : BaseDialog
    {
        private ProgramDestination destination;

        private CaretData current;
        private CaretData pgmCaretData;

        public ProgramDestinationDialog()
        {
            this.DialogHeader = "Program Action";
            InitializeComponent();
            pgmCaretData = new CaretData();
            pgmCaretData.editor = programTextBox;

            // Auto scale fontsize.
            AdaptFontSize();
        }

        public ProgramDestination Destination
        {
            get { return destination;  }
            set { destination = value; }
        }

        private void ProgramDestinationDialog_Load(object sender, EventArgs e)
        {
            bodyButton.Enabled = false;
            if (destination != null)
            {
                descriptionTextBox.Text = destination.Description;
                programTextBox.Text = destination.Command;
                startTextBox.Text = destination.StartIn;
                UpdateControls();
            }
        }

        private void okButton_Click(object sender, EventArgs args)
        {
            if (!ValidateDestination())
            {
                DialogResult = DialogResult.None;
                return;
            }
            UpdateDestination(destination);
        }

        private void UpdateDestination(ProgramDestination dest)
        {
            dest.Description = descriptionTextBox.Text.Trim();
            dest.Command = programTextBox.Text.TrimStart();
            dest.StartIn = startTextBox.Text.Trim();
        } 

        private bool ValidateDestination()
        {
            IManagementService ms = ManagementServiceHelper.GetDefaultService();
            try
            {
                ProgramDestination pd = new ProgramDestination();
                UpdateDestination(pd);

                ms.ValidateDestination(pd);
            }
            catch (Exception e)
            {
                ApplicationMessageBox.ShowError(this,
                    "The program or start in directory does not appear to be valid.  Please correct the program or start in directory and try again.",
                    e);
                return false;
            }
            return true;
        }

        private void programTextBox_Leave(object sender, EventArgs e)
        {
            string programPath = programTextBox.Text.Trim();
            string startPath = startTextBox.Text.Trim();
            if (programPath.Length > 0 && startPath.Length == 0)
            {
                string path;
                string args;
                ProgramDestination.ParseCommandLine(programPath, out path, out args);
                path = path.Replace("\"", "");

                startTextBox.Text = Path.GetDirectoryName(path);
            }

            if (sender == programTextBox)
                current = pgmCaretData;
            else
                return;

            current.selectionStart = ((TextBox)sender).SelectionStart;
            current.selectionLength = ((TextBox)sender).SelectionLength;
        }

        private void testButton_Click(object sender, EventArgs args)
        {
            if (!ValidateDestination())
                return;

            IManagementService ms = ManagementServiceHelper.GetDefaultService();
            int rc = 0;
            try
            {
                ProgramDestination pd = new ProgramDestination();
                UpdateDestination(pd);

                rc = ms.TestAction(null, pd, null);
                ApplicationMessageBox.ShowInfo(this,
                    String.Format("Program test completed with return code '{0}'", rc));
            } catch (Exception e)
            {
                ApplicationMessageBox.ShowError(this,
                    "Attempting to run the program resulted in the following error:",
                    e);
                DialogResult = DialogResult.None;
            }
        }

        private void programTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void descriptionTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void UpdateControls()
        {
            string description = descriptionTextBox.Text.Trim();
            string programPath = programTextBox.Text.Trim();
            testButton.Enabled = programPath.Length > 0;
            okButton.Enabled = testButton.Enabled && description.Length > 0;
        }

        private void ultraToolbarsManager1_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {
            string varName;
            switch (e.Tool.Key)
            {
                case "Metric":
                    varName = "$(Metric)";
                    break;
                case "Severity":
                    varName = "$(Severity)";
                    break;

                case "Value":
                    varName = "$(Value)";
                    break;

                case "Instance Name":
                    varName = "$(Instance)";
                    break;

                case "Database":
                    varName = "$(Database)";
                    break;

                case "Table":
                    varName = "$(Table)";
                    break;

                case "Timestamp":
                    varName = "$(Timestamp)";
                    break;

                case "AlertText":
                    varName = "$(AlertText)";
                    break;

                case "AlertDescription":
                    varName = "$(Description)";
                    break;

                case "AlertSummary":
                    varName = "$(AlertSummary)";
                    break;

                case "Resource":
                    varName = "$(Resource)";
                    break;

                default:
                    return;
            }
            current.editor.Text = ReplaceText(current.editor.Text, current.selectionStart, current.selectionLength, varName);
            current.selectionStart += varName.Length;
            current.selectionLength = 0;
        }

        public string ReplaceText(string source, int start, int len, string newValue)
        {
            StringBuilder builder = new StringBuilder(source);
            if (len == 0)
            {
                builder.Insert(start, newValue);
                return builder.ToString();
            }
            string oldValue = source.Substring(start, len);
            builder.Replace(oldValue, newValue, start, len);
            return builder.ToString();
        }

        internal class CaretData
        {
            internal TextBox editor;
            internal int selectionStart;
            internal int selectionLength;
        }

        private void bodyButton_Click(object sender, EventArgs e)
        {
            string menu;
            if (sender == bodyButton)
            {
                menu = "BodyPopupMenu";
                current = pgmCaretData;
            }
            else
                return;

            UltraButton button = sender as UltraButton;
            if (button != null)
            {
                ultraToolbarsManager1.ShowPopup(menu, this.PointToScreen(new Point(button.Left, button.Bottom)));
            }
        }

        private void textBox_Enter(object sender, EventArgs e)
        {
            bodyButton.Enabled = sender == programTextBox;
        }

        /// <summary>
        /// Auto scale the fontsize for the control, acording the current DPI resolution that has applied
        /// on the OS.
        /// </summary>
        protected void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }
    }
}