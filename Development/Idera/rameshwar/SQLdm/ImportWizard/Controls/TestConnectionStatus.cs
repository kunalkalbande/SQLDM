using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using System.Diagnostics;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.ImportWizard.Properties;

namespace Idera.SQLdm.ImportWizard.Controls
{
    public partial class TestConnectionStatus : UserControl
    {
        #region constants, types and members

        private enum _DisplayImage { Spin, OK, Failed };

        private Exception m_Error;

        #endregion

        #region Ctors

        public TestConnectionStatus()
        {
            InitializeComponent();
        }

        #endregion

        #region properties

        private _DisplayImage DisplayImage
        {
            set
            {
                switch (value)
                {
                    case _DisplayImage.Spin:
                        _spinner.Active = _spinner.Visible = true;
                        _statusImage.Visible = false;
                        break;
                    case _DisplayImage.OK:
                        _spinner.Active = _spinner.Visible = false;
                        _statusImage.Image = Resources.Check;
                        _statusImage.Visible = true;
                        break;
                    case _DisplayImage.Failed:
                        _spinner.Active = _spinner.Visible = false;
                        _statusImage.Image = Resources.Error;
                        _statusImage.Visible = true;
                        break;
                    default:
                        Debug.Assert(false);
                        break;
                }
            }
        }

        public bool HideStatus
        {
            get
            {
                return _pnl_Image.Visible;
            }
            set
            {
                _pnl_Image.Visible = _lnklbl_Status.Visible = !value;
            }
        }

        #endregion

        #region methods

        public void ShowInProgress()
        {
            m_Error = null;
            HideStatus = false;
            DisplayImage = _DisplayImage.Spin;
            _lnklbl_Status.Text = "Testing connection to SQLdm Repository...";
            _lnklbl_Status.LinkArea = new LinkArea(0, 0);
        }

        public void ShowSucceeded()
        {
            m_Error = null;
            HideStatus = false;
            DisplayImage = _DisplayImage.OK;
            _lnklbl_Status.Text = "Successfully connected to SQLdm Repository.";
            _lnklbl_Status.LinkArea = new LinkArea(0, 0);
        }

        public void ShowInvalidRepository()
        {
            m_Error = null;
            HideStatus = false;
            DisplayImage = _DisplayImage.Failed;
            _lnklbl_Status.Text = "The specified repository is not a valid SQLdm Repository.";
            _lnklbl_Status.LinkArea = new LinkArea(0, 0);
        }

        public void ShowNoPermission()
        {
            m_Error = new Exception("To import historical data you must connect to the SQLdm Repository as a member of the sysadmin role.");
            HideStatus = false;
            DisplayImage = _DisplayImage.Failed;
            _lnklbl_Status.Text = "No permission to import historical data, details.";
            _lnklbl_Status.LinkArea = new LinkArea(_lnklbl_Status.Text.Length - 8, 7);
        }

        public void ShowConnectionError(
                Exception e
            )
        {
            m_Error = e;
            HideStatus = false;
            DisplayImage = _DisplayImage.Failed;
            _lnklbl_Status.Text = "Failed to connect to the specified repository, details.";
            _lnklbl_Status.LinkArea = new LinkArea(_lnklbl_Status.Text.Length - 8, 7);
        }

        private void _lnklbl_Status_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (m_Error != null)
            {
                ApplicationMessageBox.ShowError(this, m_Error);
            }
        }

        #endregion
    }
}
