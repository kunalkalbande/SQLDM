using System;
using System.ComponentModel;
using System.Windows.Forms;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;
using Constants=Idera.SQLdm.Common.Constants;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    public partial class GenericFilterDialog : BaseDialog
    {
        public GenericFilterDialog(IUserFilter filterObject)
        {
            this.DialogHeader = "Filter Settings";
            InitializeComponent();
            Icon = Resources.FilterIcon;

            filterPropertiesGrid.SelectedObject = filterObject;
            AdaptFontSize();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (filterPropertiesGrid.SelectedObject is IUserFilter)
            {
                IUserFilter filterObject = (IUserFilter) filterPropertiesGrid.SelectedObject;
                string msg;
                if (!filterObject.Validate(out msg))
                {
                    msg = String.Format("{0}\n\nPlease revise your selections before continuing.", msg);
                    ApplicationMessageBox.ShowMessage(msg);
                    DialogResult = DialogResult.None;
                }
            }
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            if (filterPropertiesGrid.SelectedObject is IUserFilter)
            {
                IUserFilter filterObject = (IUserFilter) filterPropertiesGrid.SelectedObject;

                filterObject.ResetValues();
                filterPropertiesGrid.Refresh();
            }
        }

        private void GenericFilterDialog_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            if (e != null) e.Cancel = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.ViewFilters);
        }

        private void GenericFilterDialog_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            if (hlpevent != null) hlpevent.Handled = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.ViewFilters);
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