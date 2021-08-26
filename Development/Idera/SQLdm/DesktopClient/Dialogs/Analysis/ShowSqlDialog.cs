using System;
using System.Text;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ActiproSoftware.SyntaxEditor;
using Infragistics.Win.UltraWinToolbars;
using TracerX;
using Idera.SQLdm.Common.UI.Dialogs;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    public partial class ShowSqlDialog : BaseDialog
    {
        private static Logger LOG = Logger.GetLogger("ShowSqlDialog");

        public string TSQL { get; set; }

        public ShowSqlDialog()
        {
            this.DialogHeader = "T-SQL Viewer";
            InitializeComponent();
        }

        private void ShowSqlDialog_Load(object sender, EventArgs args)
        {
            try
            {
                System.Reflection.Assembly thisExe = System.Reflection.Assembly.GetExecutingAssembly();
                using (System.IO.Stream file = thisExe.GetManifestResourceStream("Idera.SQLdoctor.DesktopClient.ActiproSoftware.SQL.xml"))
                {
                    syntaxEditor1.Document.LoadLanguageFromXml(file, 0);
                }
            }
            catch (Exception e)
            {
                LOG.Warn("Failed to load SQL syntax definitions for viewer.  ", e);
            }
            if (!DesignMode)
            {
                syntaxEditor1.Text = TSQL;
            }
        }

        private void ShowSqlDialog_Shown(object sender, EventArgs e)
        {
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(syntaxEditor1.Document.Text);
            ApplicationMessageBox.ShowInfo(FindForm(), "Editor text copied to the clipboard");
        }

        private void ShowSqlDialog_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            //HelpTopics.ShowHelpTopic(HelpTopics.ShowMeTheProblem);
        }
    }
}
