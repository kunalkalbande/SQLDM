using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using System.Diagnostics;
using Idera.SQLdm.Common.UI.Dialogs;

namespace Idera.SQLdm.ImportWizard.Controls
{
    public partial class LoadServersStatus : UserControl
    {
        #region types, constants and members

        private enum MessageType { None, Error, Warning };

        private const string NoServersAvailableWarningLinkText = "<No SQL Servers to import>";
        private const string NoServersAvailableWarningMsg = "To import data from your SQL Server instances, first register these instances with the new SQL diagnostic manager.";
        private const string ServerEnumErrorLinkText = "<Error loading SQL Servers>";
        private const string ServerEnumErrorMsg = "The Import Wizard encountered an error when retrieving the list of SQL Server registered with the new SQL diagnostic manager. Please verify the SQL diagnostic manager Repository location and connection credentials specified on the previous page.";

        private MessageType m_MessageType;
        private string m_Message;
        private Exception m_Exception;

        #endregion

        #region Ctors

        public LoadServersStatus()
        {
            InitializeComponent();
        }

        #endregion

        #region properties

        public bool HideStatus
        {
            set
            {
                if (value)
                {
                    this.SendToBack();
                    this.Visible = false;
                }
                else
                {
                    this.BringToFront();
                    this.Visible = true;
                }
            }
        }

        #endregion

        #region methods

        private void displaySpinner()
        {
            m_MessageType = MessageType.None;
            m_Message = null;
            m_Exception = null;

            _lnklbl_Status.Visible = false;
            _spinner.Active = _spinner.Visible = true;
            HideStatus = false;
        }

        private void displayLinkText(
                string linkText,
                MessageType type,
                string message,
                Exception exception
            )
        {
            _lnklbl_Status.Text = linkText;
            _lnklbl_Status.LinkArea = new LinkArea(1, _lnklbl_Status.Text.Length-2);

            m_MessageType = type;
            m_Message = message;
            m_Exception = exception;

            _spinner.Active = _spinner.Visible = false;
            _lnklbl_Status.Visible = true;
            HideStatus = false;
        }

        public void ShowInProgress()
        {
            displaySpinner();
        }

        public void ShowNoAvailableServers()
        {
            displayLinkText(NoServersAvailableWarningLinkText,MessageType.Warning, NoServersAvailableWarningMsg, null);
        }

        public void ShowError(
                Exception e
            )
        {
            displayLinkText(ServerEnumErrorLinkText, MessageType.Error, ServerEnumErrorMsg, e);
        }

        private void _lnklbl_Status_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            switch (m_MessageType)
            {
                case MessageType.None:
                    Debug.Assert(false);
                    break;

                case MessageType.Warning:
                    ApplicationMessageBox.ShowWarning(this, m_Message, m_Exception);
                    break;

                case MessageType.Error:
                    ApplicationMessageBox.ShowError(this, m_Message, m_Exception);
                    break;
            }
        }

        #endregion
    }
}
