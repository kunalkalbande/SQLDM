using System;
using System.IO;
using System.Windows.Forms;
using Idera.SQLdm.Common.UI.Dialogs;

namespace Idera.SQLdm.ImportWizard.Helpers
{
    internal static class HelpHelper
    {
        #region types

        public enum ImportWizardHelpTopic
        {
            IntroductionPage,
            SpecifyRepositoryPage,
            SelectSQLServersPage,
            SpecifyImportDatePage,
            FinishPage,
            ImportStatus
        };

        #endregion
    }
}
