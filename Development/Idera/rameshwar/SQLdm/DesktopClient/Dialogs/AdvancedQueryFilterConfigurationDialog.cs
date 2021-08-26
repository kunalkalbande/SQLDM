using System;
using System.Windows.Forms;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.DesktopClient.Helpers;
using Wintellect.PowerCollections;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    using System.ComponentModel;
    using Common;

    public partial class AdvancedQueryFilterConfigurationDialog : Form
    {
        private AdvancedQueryFilterConfiguration configuration;
        private bool settingsChanged = false;
        private string viewName = "Activity Monitor";

        public AdvancedQueryFilterConfigurationDialog(AdvancedQueryFilterConfiguration configuration, string viewName)
        {
            InitializeComponent();
            this.configuration = configuration;
            this.viewName = viewName;

            InitLabels();

            if (configuration != null)
            {
                applicationsExcludeFilterTextBox.Text = configuration.ApplicationExcludeToString();
                databasesExcludeFilterTextBox.Text = configuration.DatabaseExcludeToString();
                sqlTextExcludeFilterTextBox.Text = configuration.SqlTextExcludeToString();

                applicationsIncludeFilterTextBox.Text = configuration.ApplicationIncludeToString();
                databasesIncludeFilterTextBox.Text = configuration.DatabaseIncludeToString();
                sqlTextIncludeFilterTextBox.Text = configuration.SqlTextIncludeToString();

                chkExcludeDM.Checked = configuration.ExcludeDM;

                if (configuration.GetType() == typeof(AdvancedQueryMonitorConfiguration))
                {
                    rowcountPanel.Visible = false;
                }
                else
                {

                    if (configuration.Rowcount > 0)
                    {
                        rowcountUpDown.Enabled = true;
                        rowcountLimited.Checked = true;
                        rowcountUnlimited.Checked = false;
                        rowcountUpDown.Value = configuration.Rowcount;
                    }
                    else
                    {
                        rowcountUpDown.Enabled = false;
                        rowcountLimited.Checked = false;
                        rowcountUnlimited.Checked = true;
                    }
                    rowcountPanel.Visible = true;
                }
            }

            AdaptFontSize();
        }

        private void InitLabels()
        {
            office2007PropertyPage1.Text = String.Format(office2007PropertyPage1.Text, viewName);
            this.Text = String.Format(this.Text, viewName);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (configuration == null)
            {
                configuration = new AdvancedQueryFilterConfiguration();
            }

            ApplyApplicationFilterChanges();
            ApplyDatabaseFilterChanges();
            ApplySqlTextFilterChanges();
            configuration.ExcludeDM = chkExcludeDM.Checked;

            if (configuration.GetType() != typeof(AdvancedQueryMonitorConfiguration))
            {
                if (rowcountUnlimited.Checked == false && rowcountUpDown.Value > 0)
                {
                    configuration.Rowcount = (int)rowcountUpDown.Value;
                }
                else
                {
                    configuration.Rowcount = 0;
                }
            }

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

            //SQLdm 8.5 (Ankit Srivastava): for Inclusion filters
            ParseFilterString(applicationsIncludeFilterTextBox.Text, out matchFilters, out likeFilters);

            if (SetChanged(configuration.ApplicationIncludeLike, likeFilters))
            {
                configuration.ApplicationIncludeLike = likeFilters;
                settingsChanged = true;
            }

            if (SetChanged(configuration.ApplicationIncludeMatch, matchFilters))
            {
                configuration.ApplicationIncludeMatch = matchFilters;
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

            //SQLdm 8.5 (Ankit Srivastava): for Inclusion filters
            ParseFilterString(databasesIncludeFilterTextBox.Text, out matchFilters, out likeFilters);

            if (SetChanged(configuration.DatabaseIncludeLike, likeFilters))
            {
                configuration.DatabaseIncludeLike = likeFilters;
                settingsChanged = true;
            }

            if (SetChanged(configuration.DatabaseIncludeMatch, matchFilters))
            {
                configuration.DatabaseIncludeMatch = matchFilters;
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

            //SQLdm 8.5 (Ankit Srivastava): for Inclusion filters
            ParseFilterString(sqlTextIncludeFilterTextBox.Text, out matchFilters, out likeFilters);

            if (SetChanged(configuration.SqlTextIncludeLike, likeFilters))
            {
                configuration.SqlTextIncludeLike = likeFilters;
                settingsChanged = true;
            }

            if (SetChanged(configuration.SqlTextIncludeMatch, matchFilters)) // SQLdm 10.2 (Anshul Aggarwal) - Fixed Incorrect Comparison.
            {
                configuration.SqlTextIncludeMatch = matchFilters;
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

        private void rowcountLimited_CheckedChanged(object sender, EventArgs e)
        {
            if (rowcountLimited.Checked)
            {
                rowcountUpDown.Enabled = true;
            }
        }

        private void rowcountUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (configuration.GetType() != typeof(AdvancedQueryMonitorConfiguration))
            {
                if (rowcountUnlimited.Checked == false && rowcountUpDown.Value > 0)
                {
                    configuration.Rowcount = (int)rowcountUpDown.Value;
                }
            }
        }

        private void rowcountUnlimited_CheckedChanged(object sender, EventArgs e)
        {
            if (rowcountUnlimited.Checked)
            {
                rowcountUpDown.Enabled = false;
                configuration.Rowcount = 0;
            }
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