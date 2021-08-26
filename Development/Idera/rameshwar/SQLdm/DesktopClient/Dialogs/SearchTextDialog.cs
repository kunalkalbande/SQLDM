using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Idera.SQLdm.Common;
using Idera.SQLdm.DesktopClient.Helpers;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    public partial class SearchTextDialog : Form
    {
        #region fields

        private DataTable searchList;

        #endregion

        #region constructors

        public SearchTextDialog()
        {
            InitializeComponent();
            AdaptFontSize();
            searchList = new DataTable();
            searchList.Columns.Add(ColumnKey);
            searchList.Columns.Add(ColumnName);

            searchListComboBox.DataSource = searchList;
            searchListComboBox.DisplayMember = ColumnName;
        }

        #endregion

        #region properties

        public const string ColumnKey = "Key";
        public const string ColumnName = "Name";

        public event EventHandler FindNext;

        /// <summary>
        /// Use case matching when finding the SearchText value if true
        /// </summary>
        public bool MatchCase
        {
            get { return matchCaseCheckBox.Checked; }
        }

        public DataTable SearchList
        {
            get { return searchList; }
        }

        /// <summary>
        /// The column or field to search as selected from the SearchList by the user
        /// </summary>
        public string SearchIn
        {
            get { return (string)((DataRowView)searchListComboBox.SelectedItem)[ColumnKey]; }
        }

        /// <summary>
        /// The text to search for
        /// </summary>
        public string SearchText
        {
            get { return searchTextTextBox.Text; }
        }

        /// <summary>
        /// Search direction. Search back or up if true
        /// </summary>
        public bool SearchUp
        {
            get { return searchUpCheckBox.Checked; }
        }

        #endregion

        protected override void OnHelpButtonClicked(System.ComponentModel.CancelEventArgs e) {
            if (e != null) e.Cancel = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.SearchLogs);
        }

        protected override void OnHelpRequested(HelpEventArgs hevent) {
            if (hevent != null) hevent.Handled = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.SearchLogs);
        }

        private void findNextButton_Click(object sender, EventArgs e)
        {
            if (FindNext != null && SearchText.Length > 0)
            {
                FindNext(this, e);
            }

            //this is setup as the accept button, so hitting enter will trigger the search
            // however, the dialog should not close
            DialogResult = DialogResult.None;
        }

        private void SearchTextDialog_KeyDown(object sender, KeyEventArgs e)
        {
            // there is no cancel button, so simulate it manually
            if (e.KeyCode == Keys.Escape)
            {
                Hide();
            }
        }

        private void SearchTextDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Hide();
                e.Cancel = true;
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Hide();
        }

        /// <summary>
        /// Adapts the size of the font for this control in case of OS font changes.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Control);
        }
    }
}
