using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Views.Servers.Server;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    public partial class SqlTextDialog : BaseDialog
    {
        #region fields

        private int instanceId;
        private static string diagnoseQuery = null;
        private static string database = string.Empty;

        public static string DiagnoseQuery
        {
            get { return SqlTextDialog.diagnoseQuery; }
            set { SqlTextDialog.diagnoseQuery = value; }
        }

        public static string Database
        {
            get { return SqlTextDialog.database; }
            set { SqlTextDialog.database = value; }
        }

        #endregion
        #region constructors

        public SqlTextDialog(string text, int instaceID, bool isDiagnoseRequired)
        {
            this.DialogHeader = "SQL Text";
            InitializeComponent();
            //Passing instance ID which is required to diagnose query
            this.instanceId = instaceID;
            sqlTextTextBox.Text = text;
            //check if diagnose required
            diagnoseButton.Visible = isDiagnoseRequired;
            sqlTextTextBox.SelectionStart = 0;
            sqlTextTextBox.SelectionLength = 0;
            copySelectedButton.Enabled = false;
            AdaptFontSize();
        }

        public SqlTextDialog(string text,string DB, int instaceID, bool isDiagnoseRequired)
        {
            this.DialogHeader = "SQL Text";
            InitializeComponent();
            //Passing instance ID which is required to diagnose query
            this.instanceId = instaceID;
            sqlTextTextBox.Text = text;
            //check if diagnose required
            diagnoseButton.Visible = isDiagnoseRequired;
            sqlTextTextBox.SelectionStart = 0;
            sqlTextTextBox.SelectionLength = 0;
            copySelectedButton.Enabled = false;
            AdaptFontSize();
            database = DB;
        }

        /// <summary>
        /// SQLdm 9.1 (Vineet Kumar) (Community Integration) - Added this constructor to reuse this dialogue to show other messages as well with a customised header text
        /// </summary>
        /// <param name="text"></param>
        /// <param name="title"></param>
        public SqlTextDialog(string text, string title, bool isDiagnoseRequired)
        {
            this.DialogHeader = "SQL Text";
            InitializeComponent();
            this.Text = title;
            sqlTextTextBox.Text = text;
            //check if diagnose required
            diagnoseButton.Visible = isDiagnoseRequired;
            sqlTextTextBox.SelectionStart = 0;
            sqlTextTextBox.SelectionLength = 0;
            copySelectedButton.Enabled = false;
            AdaptFontSize();
        }

        #endregion

        #region events

        private void copyAllButton_Click(object sender, EventArgs e)
        {
            DataObject dataObject = new DataObject();
            if (!string.IsNullOrEmpty(sqlTextTextBox.Text))
            {
                dataObject.SetText(sqlTextTextBox.Text);
            }
            Clipboard.SetDataObject(dataObject, true);
        }

        #endregion

        private void copySelectedButton_Click(object sender, EventArgs e)
        {
            DataObject dataObject = new DataObject();
            dataObject.SetText(sqlTextTextBox.SelectedText);
            Clipboard.SetDataObject(dataObject, true);
        }

        private void sqlTextTextBox_MouseUp(object sender, MouseEventArgs e)
        {
            copySelectedButton.Enabled = sqlTextTextBox.SelectionLength > 0;
        }

        private void sqlTextTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            copySelectedButton.Enabled = sqlTextTextBox.SelectionLength > 0;
        }
        /// <summary>
        /// Run process to diagnose the query and show result on analysis UI.
        /// </summary>
        private void diagnoseButton_Click(object sender, EventArgs e)
        {
            diagnoseQuery = sqlTextTextBox.Text;
            ApplicationController.Default.ShowServerView(instanceId, ServerViews.Analysis, 11) ;
            //diagnoseRecommendationList = null;
        }
        /// <summary>
        /// Adapts the size of the font for this control in case of OS font changes.
        /// </summary>
        private void AdaptFontSize()
        {

            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }
    }
}