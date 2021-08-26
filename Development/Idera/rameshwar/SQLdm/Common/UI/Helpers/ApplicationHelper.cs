using System;
using System.Collections.Generic;
using System.Text;
using BBS.TracerX;
using System.IO;
using Idera.SQLdm.Common;

namespace Idera.SQLdm.Common.UI.Helpers
{
    internal static class ApplicationHelper
    {
        public static void ShowHelpTopic(string topic)
        {
            if (string.IsNullOrEmpty(topic))
            {
                Logger.Root.Error("ShowHelpTopic was passed a null or empty string.");
                topic = string.Empty;
            }

            IntPtr hWnd;
            Logger.Root.Info("Help Topic: ", Path.Combine(HelpTopics.HelpSubFolder, topic));

            if (!Idera.WebHelp.WebHelpLauncher.TryShowWebHelp(Path.Combine(HelpTopics.HelpSubFolder, topic), out hWnd, true))
            {
                Logger.Root.Info("Help request diverted to the Chm file");
                ShowHelpTopicChm(topic);
            }
        }

        public static void ShowHelpTopicChm(string topic)
        {
            if (string.IsNullOrEmpty(topic))
            {
                Logger.Root.Error("ShowHelpTopic was passed a null or empty string.");
            }
            else
            {
                // The dummy form is needed to prevent the help window from always being on top
                // of the application window.
                Logger.Root.Info("Showing help topic ", topic);
                System.Windows.Forms.Form dummy = new System.Windows.Forms.Form();
                System.Windows.Forms.Help.ShowHelp(dummy, Idera.SQLdm.Common.Constants.HelpFileName, System.Windows.Forms.HelpNavigator.TableOfContents);
                System.Windows.Forms.Help.ShowHelp(dummy, Idera.SQLdm.Common.Constants.HelpFileName, Path.Combine(HelpTopics.HelpSubFolder, topic));
            }
        }
    }
}
