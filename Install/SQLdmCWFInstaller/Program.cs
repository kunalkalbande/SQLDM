using System;
using System.Windows.Forms;

namespace SQLdmCWFInstaller
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            SQLdmCWFInstaller.Helpers.Logger _logger = SQLdmCWFInstaller.Helpers.Logger.GetLogger("SQL CWF Installer Logger");
            _logger.Info("launching");
            try
            {
                
                Application.EnableVisualStyles();
                
                Application.SetCompatibleTextRenderingDefault(false);
                
                Application.Run(new SQLdmInstallWizard());
                
            }
            catch (Exception ex)
            {
                _logger.Error("Main:",ex);
            }
        }
    }
}
