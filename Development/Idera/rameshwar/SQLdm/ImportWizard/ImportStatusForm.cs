using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using System.Diagnostics;
using System.Threading;
using Idera.SQLdm.Common;
using Wintellect.PowerCollections;
using BBS.TracerX;

using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.ImportWizard.Helpers;
using Idera.SQLdm.ImportWizard.Objects;
using Idera.SQLdm.ImportWizard.Properties;

namespace Idera.SQLdm.ImportWizard
{
    public partial class ImportStatusForm : Form
    {
        #region constants

        private const string NotifyIconText = "SQL diagnostic manager Import Wizard Status";

        #endregion

        #region members

        private ImportContext m_ImportContext;
        private BackgroundWorker m_BackgroundWorker;
        private bool m_IsCloseClicked;
        private readonly BBS.TracerX.Logger Log = BBS.TracerX.Logger.GetLogger("Status");

        #endregion

        #region ctors

        public ImportStatusForm (
                SqlConnectionInfo repository,
                DateTime importDate,
                List<SQLdm5x.MonitoredSqlServer> servers
            )
        {
            Debug.Assert(importDate != DateTime.MinValue && servers != null
                            && servers.Count > 0);

            // UI stuff
            InitializeComponent();
            MinimumSize = Size; // set min size to form size.
            _btn_Close.Visible = false; // hide the close button.
            _notifyIcon.Text = NotifyIconText;

            // Initialize import information.
            m_ImportContext = new ImportContext (repository, importDate, servers);

            // Create the background worker object.
            m_BackgroundWorker = new BackgroundWorker();
            m_BackgroundWorker.WorkerReportsProgress = true;
            m_BackgroundWorker.WorkerSupportsCancellation = true;
            m_BackgroundWorker.DoWork += new DoWorkEventHandler(m_BackgroundWorker_DoWork);
            m_BackgroundWorker.ProgressChanged += new ProgressChangedEventHandler(m_BackgroundWorker_ProgressChanged);
            m_BackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(m_BackgroundWorker_RunWorkerCompleted);

            // Start the background worker async.
            m_BackgroundWorker.RunWorkerAsync(m_ImportContext);
        }

        #endregion

        #region event methods

        private void changeCancelToClose()
        {
            // Hide the cancel button, and show the close button.
            _btn_Close.Location = _btn_Cancel.Location;
            _btn_Cancel.Visible = false;
            _btn_Close.Visible = true;

            // Disable the Hide and Notify button.
            _btn_HideAndNotify.Enabled = false;

            // Disable the notify icon cancel context menu item.
            _mi_Cancel.Enabled = false;
        }

        private void doShow()
        {
            Show();
            Activate();
        }

        private void doCancel()
        {
            // Show the status form.
            doShow();

            // Prompt the user to confirm cancel.
            DialogResult rc = ApplicationMessageBox.ShowQuestion(this, "Do you want to cancel historical data import?",
                                                                    Microsoft.SqlServer.MessageBox.ExceptionMessageBoxButtons.YesNo,
                                                                        Microsoft.SqlServer.MessageBox.ExceptionMessageBoxDefaultButton.Button2);
            if (rc == DialogResult.Yes)
            {
                // Change cancel to close button.
                changeCancelToClose();

                // Send cancel request to the background worker.
                m_BackgroundWorker.CancelAsync();
            }
        }

        private void doHelp()
        {
            doShow();
            Help.ShowHelp(FindForm(), Constants.HelpFileName, Path.Combine(HelpTopics.HelpSubFolder, HelpTopics.ImportWizardStatusPage));
        }

        private void ImportStatusForm_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            doHelp();
        }

        private void ImportStatusForm_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible == false)
            {
                _notifyIcon.ShowBalloonTip(1, Resources.ImportWizardTitle, Resources.NotifyClickHereForProgress,
                        ToolTipIcon.Info);
            }
        }

        private void ImportStatusForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // The form can only be closed if the Close button is clicked.
            // Otherwise just minimize the status form.
            if (e.CloseReason == CloseReason.UserClosing)
            {
                if (m_IsCloseClicked)
                {
                    DialogResult = DialogResult.Cancel;
                }
                else
                {
                    e.Cancel = true;
                    Hide();
                }
            }
        }

        private void _btn_HideAndNotify_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void _btn_Cancel_Click(object sender, EventArgs e)
        {
            doCancel();
        }

        private void _btn_Close_Click(object sender, EventArgs e)
        {
            // Set the closing flag, and call Close.
            m_IsCloseClicked = true;
            Close();
        }

        private void _btn_Help_Click(object sender, EventArgs e)
        {
            doHelp();
        }

        private void _notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            doShow();
        }

        private void _notifyIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            doShow();
        }

        private void _mi_Cancel_Click(object sender, EventArgs e)
        {
            doCancel();
        }

        private void _mi_Help_Click(object sender, EventArgs e)
        {
            doHelp();
        }

        private void _mi_OpenImportStatus_Click(object sender, EventArgs e)
        {
            doShow();
        }

        #endregion

        #region background worker methods

        void m_BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Change cancel to close button.
            changeCancelToClose();
        }

        void m_BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // Get percent completed and status.
            int p = e.ProgressPercentage;
            ImportStatus s = e.UserState as ImportStatus;
            Debug.Assert(s != null);

            // Set the notify icon text, based on state.
            switch (s.State)
            {
                case ImportStatus.ImportState.Start:
                    _notifyIcon.Text = NotifyIconText + " - Started";
                    break;
                case ImportStatus.ImportState.InProgress:
                    _notifyIcon.Text = NotifyIconText + " - " + p.ToString() + "%";
                    break;
                case ImportStatus.ImportState.End:
                    _notifyIcon.Text = NotifyIconText + " - Completed";
                    _notifyIcon.ShowBalloonTip(1000, "Idera SQL diagnostic manager Import Wizard", "Import has completed.", ToolTipIcon.Info);
                    doShow();
                    break;
                case ImportStatus.ImportState.Error:
                    _notifyIcon.Text = NotifyIconText + " - Error";
                    _notifyIcon.ShowBalloonTip(1000, "Idera SQL diagnostic manager Import Wizard", "Import has completed.", ToolTipIcon.Info);
                    doShow();
                    break;
                case ImportStatus.ImportState.Cancelled:
                    _notifyIcon.Text = NotifyIconText + " - Cancelled";
                    break;
                default:
                    Debug.Assert(false);
                    _notifyIcon.Text = NotifyIconText;
                    break;
            }

            _prgsBar.Value = p;
            _rtb_ImportStatus.AppendText(s.Status);
            _rtb_ImportStatus.ScrollToCaret();
        }

        void m_BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Do the casts.
            BackgroundWorker bgw = sender as BackgroundWorker;
            ImportContext ic = e.Argument as ImportContext;
            Debug.Assert(bgw != null && ic != null);

            // Start reading the historical data and moving it to the repository.
            //      bgw - cancellation pending flag.
            //      import context
            //      [out] cancelled or not flag
            //      [out] context of work done so far.
            Import importObject = new Import(bgw, ic);
            importObject.DoImport();
        }

        #endregion

    }
}