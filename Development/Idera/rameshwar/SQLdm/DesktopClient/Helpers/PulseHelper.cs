using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using Idera.SQLdm.DesktopClient.Properties;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.Newsfeed.Plugins.UI;

namespace Idera.SQLdm.DesktopClient.Helpers
{
    internal class PulseHelper
    {
        public static bool CheckPulseLogin(IWin32Window owner)
        {
            bool loggedIn = false;
            if (ApplicationModel.Default.IsPulseConfigured)
            {
                if (PulseController.Default.IsLoggedIn)
                {
                    loggedIn = true;
                }
                else
                {
                    if (string.IsNullOrEmpty(Settings.Default.PulseAccountName))
                    {
                        DialogResult result = ApplicationMessageBox.ShowQuestion(owner, "You have not specified a user for Newsfeed yet.\n\nDo you want to signup for Newsfeed now?", Microsoft.SqlServer.MessageBox.ExceptionMessageBoxButtons.YesNo);
                        if (result == DialogResult.Yes)
                        {
                            ApplicationController.Default.ShowPulseView();
                        }
                    }
                    else
                    {
                        if (Settings.Default.PulseAccountKeepLoggedIn)
                        {
                            loggedIn = PulseController.Default.LogIn(Settings.Default.PulseAccountName, true);
                        }
                        else
                        {
                            if (DialogResult.Yes == ApplicationMessageBox.ShowQuestion(owner, string.Format("The saved Newsfeed user '{0}' is not currently logged in.\n\nDo you want to login as '{0}' now?", Settings.Default.PulseAccountName), Microsoft.SqlServer.MessageBox.ExceptionMessageBoxButtons.YesNo))
                            {
                                loggedIn = PulseController.Default.LogIn(Settings.Default.PulseAccountName, false);
                            }
                        }
                    }
                }
            }
            else
            {
                if (DialogResult.Yes == ApplicationMessageBox.ShowQuestion(owner, "The IDERA Newsfeed is not currently configured for SQL Diagnostic Manager.\n\nDo you want to learn more about Newsfeed now?", Microsoft.SqlServer.MessageBox.ExceptionMessageBoxButtons.YesNo))
                {
                    ApplicationController.Default.ShowPulseView();
                }
            }

            return loggedIn;
        }
    }
}
