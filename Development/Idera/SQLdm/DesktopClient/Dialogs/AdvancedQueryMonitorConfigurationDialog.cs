using System;
using System.Windows.Forms;
using Idera.SQLdm.Common.Configuration;
using Wintellect.PowerCollections;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    using System.ComponentModel;
    using Common;

    public partial class AdvancedQueryMonitorConfigurationDialog : Form
    {
        private AdvancedQueryMonitorConfiguration configuration;
        private bool settingsChanged = false;

        public AdvancedQueryMonitorConfigurationDialog(AdvancedQueryMonitorConfiguration configuration)
        {
            InitializeComponent();
            this.configuration = configuration;

            if (configuration != null)
            {
                applicationsExcludeFilterTextBox.Text = configuration.ApplicationExcludeToString();
                databasesExcludeFilterTextBox.Text = configuration.DatabaseExcludeToString();
                sqlTextExcludeFilterTextBox.Text = configuration.SqlTextExcludeToString();
            }
        }

        private static void UpdateTextBoxConrols(Control parentControl, TextBoxBase textBox)
        {
            if (parentControl != null && textBox != null)
            {
                int lastLine = textBox.TextLength == 0
                                   ? 0
                                   : textBox.GetLineFromCharIndex(textBox.TextLength - 1);

                parentControl.Height = 75 + lastLine * TextRenderer.MeasureText(" ", textBox.Font).Height;
            }
        }

        private void applicationsExcludeFilterTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateTextBoxConrols(applicationsFilterPanel, applicationsExcludeFilterTextBox);
        }

        private void applicationsExcludeFilterTextBox_Resize(object sender, EventArgs e)
        {
            UpdateTextBoxConrols(applicationsFilterPanel, applicationsExcludeFilterTextBox);
        }

        private void databasesExcludeFilterTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateTextBoxConrols(databasesFilterPanel, databasesExcludeFilterTextBox);
        }

        private void databasesExcludeFilterTextBox_Resize(object sender, EventArgs e)
        {
            UpdateTextBoxConrols(databasesFilterPanel, databasesExcludeFilterTextBox);
        }

        private void sqlTextExcludeFilterTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateTextBoxConrols(sqlTextFilterPanel, sqlTextExcludeFilterTextBox);
        }

        private void sqlTextExcludeFilterTextBox_Resize(object sender, EventArgs e)
        {
            UpdateTextBoxConrols(sqlTextFilterPanel, sqlTextExcludeFilterTextBox);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (configuration == null)
            {
                configuration = new AdvancedQueryMonitorConfiguration();
            }

            ApplyApplicationFilterChanges();
            ApplyDatabaseFilterChanges();
            ApplySqlTextFilterChanges();

            if (!settingsChanged)
            {
                DialogResult = DialogResult.Cancel;
            }
        }

        private static bool SetChanged(string[] left, string[] right)
        {
            if (left == null || left.Length == 0)
            {
                return (right != null && right.Length > 0);
            }

            if (right == null || right.Length == 0 || left.Length != right.Length)
            {
                return true;
            }

            return !Algorithms.EqualSets(left, right);
        }

        private static void ParseFilterString(string filterString, out string[] matchFilters, out string[] likeFilters)
        {
            OrderedSet<string> matchSet = new OrderedSet<string>();
            OrderedSet<string> likeSet = new OrderedSet<string>();

            if (!string.IsNullOrEmpty(filterString))
            {
                string[] filters = filterString.Split(';');

                foreach (string untrimmedFilter in filters)
                {
                    string filter = untrimmedFilter.Trim();

                    if (filter.Length > 1 && filter[0] == '[' && filter[filter.Length - 1] == ']')
                    {
                        filter = filter.Remove(0, 1);
                        filter = filter.Remove(filter.Length - 1, 1);
                    }

                    if (filter.Contains("%") && !likeSet.Contains(filter))
                    {
                        likeSet.Add(filter);
                    }
                    else if (!matchSet.Contains(filter))
                    {
                        matchSet.Add(filter);
                    }
                }
            }

            matchFilters = matchSet.ToArray();
            likeFilters = likeSet.ToArray();
        }

        private void ApplyApplicationFilterChanges()
        {
            string[] matchFilters, likeFilters;
            ParseFilterString(applicationsExcludeFilterTextBox.Text, out matchFilters, out likeFilters);

            if (SetChanged(configuration.ApplicationExcludeLike, likeFilters))
            {
                configuration.ApplicationExcludeLike = likeFilters;
                settingsChanged = true;
            }

            if (SetChanged(configuration.ApplicationExcludeMatch, matchFilters))
            {
                configuration.ApplicationExcludeMatch = matchFilters;
                settingsChanged = true;
            }
        }

        private void ApplyDatabaseFilterChanges()
        {
            string[] matchFilters, likeFilters;
            ParseFilterString(databasesExcludeFilterTextBox.Text, out matchFilters, out likeFilters);

            if (SetChanged(configuration.DatabaseExcludeLike, likeFilters))
            {
                configuration.DatabaseExcludeLike = likeFilters;
                settingsChanged = true;
            }

            if (SetChanged(configuration.DatabaseExcludeMatch, matchFilters))
            {
                configuration.DatabaseExcludeMatch = matchFilters;
                settingsChanged = true;
            }
        }

        private void ApplySqlTextFilterChanges()
        {
            string[] matchFilters, likeFilters;
            ParseFilterString(sqlTextExcludeFilterTextBox.Text, out matchFilters, out likeFilters);

            if (SetChanged(configuration.SqlTextExcludeLike, likeFilters))
            {
                configuration.SqlTextExcludeLike = likeFilters;
                settingsChanged = true;
            }

            if (SetChanged(configuration.SqlTextExcludeMatch, matchFilters))
            {
                configuration.SqlTextExcludeMatch = matchFilters;
                settingsChanged = true;
            }
        }

        private void AdvancedQueryMonitorConfigurationDialog_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            if (e != null) e.Cancel = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.AdvancedQueryMonitorConfigurationDialog);
        }

        private void AdvancedQueryMonitorConfigurationDialog_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            if (hlpevent != null) hlpevent.Handled = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.AdvancedQueryMonitorConfigurationDialog);
        }

        private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Ignore ENTER and CTRL+ENTER
            e.Handled = e.KeyChar == 10 || e.KeyChar == 13;
        }
    }
}