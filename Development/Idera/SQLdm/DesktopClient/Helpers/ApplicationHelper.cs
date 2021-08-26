using System.IO;
using System.Reflection;
using System.Security;
using System;
using System.ComponentModel;
using BBS.TracerX;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.DesktopClient.Helpers
{
    internal static class ApplicationHelper
    {
        #region Assembly Attribute Accessors

        public static string AssemblyTitle
        {
            get
            {
                // Get all Title attributes on this assembly
                object[] attributes =
                    Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                // If there is at leastCount one Title attribute
                if (attributes.Length > 0)
                {
                    // Select the first one
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    // If it is not an empty string, return it
                    if (titleAttribute.Title != "")
                        return titleAttribute.Title;
                }
                // If there was no Title attribute, or if the Title attribute was the empty string, return the .exe name
                return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public static string AssemblyVersion
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }

        public static string AssemblyDescription
        {
            get
            {
                // Get all Description attributes on this assembly
                object[] attributes =
                    Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                // If there aren't any Description attributes, return an empty string
                if (attributes.Length == 0)
                    return "";
                // If there is a Description attribute, return its value
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public static string AssemblyProduct
        {
            get
            {
                // Get all Product attributes on this assembly
                object[] attributes =
                    Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                // If there aren't any Product attributes, return an empty string
                if (attributes.Length == 0)
                    return "";
                // If there is a Product attribute, return its value
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public static string AssemblyCopyright
        {
            get
            {
                // Get all Copyright attributes on this assembly
                object[] attributes =
                    Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                // If there aren't any Copyright attributes, return an empty string
                if (attributes.Length == 0)
                    return "";
                // If there is a Copyright attribute, return its value
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public static string AssemblyCompany
        {
            get
            {
                // Get all Company attributes on this assembly
                object[] attributes =
                    Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                // If there aren't any Company attributes, return an empty string
                if (attributes.Length == 0)
                    return "";
                // If there is a Company attribute, return its value
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }

        #endregion

        #region Encryption Helpers

        public static SecureString ConvertStringToSecureString(string plaintext)
        {
            if (plaintext == null || plaintext.Length == 0)
            {
                return null;
            }
            else
            {
                SecureString secureString = new SecureString();
                char[] plaintextCharArray = plaintext.ToCharArray();

                foreach (char character in plaintextCharArray)
                {
                    secureString.AppendChar(character);
                }

                return secureString;
            }
        }

        #endregion

        #region Enum Helpers

        internal static string GetEnumDescription(object o)
        {
            System.Type otype = o.GetType();
            if (otype.IsEnum)
            {
                FieldInfo field = otype.GetField(Enum.GetName(otype, o));
                if (field != null)
                {
                    object[] attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), true);
                    if (attributes.Length > 0)
                        return ((DescriptionAttribute)attributes[0]).Description;
                }
            }
            return o.ToString();
        }


        #endregion

        public static void ShowHelpTopic(string topic)
        {
            if (string.IsNullOrWhiteSpace(topic))
            {
                Logger.Root.Error("ShowHelpTopic was passed a null or empty string.");
                topic = string.Empty;
            }
            string urlToOpen; //SQLdm 9.0 (Gaurav Karwal): calculating the path to open
            if (HelpTopics.AlertCLRStatus.Equals(topic))
            {
                // SQLdm 10.2.2 (Varun Chopra) SQLDM-19310 'Show Alert Help' link for 'CLR Enabled' metric takes you to the wrong link.
                urlToOpen = Path.Combine(HelpTopics.IderaWikiBaseURL, HelpTopics.Display, HelpTopics.Sqldm, HelpTopics.AlertCLRStatusUpdated);
            }
            else
            {
                urlToOpen = Path.Combine(HelpTopics.IderaWikiBaseURL, HelpTopics.IderaWikiSubfolder, topic);
            }
            //IntPtr hWnd; SQLdm 9.0 (Gaurav Karwal): Commented this code as in 9.0+ we will not have the WebHelp.exe
            Logger.Root.Info("Help Topic: ", urlToOpen);//SQLdm 9.0 (Gaurav Karwal): modified to use wiki
            System.Diagnostics.Process urlOpeningAgent = System.Diagnostics.Process.Start(urlToOpen); //SQLdm9.0 (Gaurav Karwal): opening the link in the default web browser
            
            //[START]: SQLdm 9.0 (Gaurav Karwal): Commented this code as in 9.0+ we will not have the WebHelp.exe
            //if (!Idera.WebHelp.WebHelpLauncher.TryShowWebHelp(Path.Combine(HelpTopics.HelpSubFolder, topic), out hWnd, true))
            //{
            //    Logger.Root.Info("Help request diverted to the Chm file");
            //    ShowHelpTopicChm(topic);
            //}
            //[END]: SQLdm 9.0 (Gaurav Karwal): Commented this code as in 9.0+ we will not have the WebHelp.exe
        }

        /// <summary>
        /// SQLdm 9.0 (Gaurav Karwal): This function is redundant after 9.0 as help has moved to wiki
        /// </summary>
        /// <param name="topic"></param>
        public static void ShowHelpTopicChm(string topic) {
            if (string.IsNullOrEmpty(topic)) {
                Logger.Root.Error("ShowHelpTopic was passed a null or empty string.");
            } else {
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
