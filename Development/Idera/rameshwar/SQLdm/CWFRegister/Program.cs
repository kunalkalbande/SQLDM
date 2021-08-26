using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CWFRegister.Configuration;

namespace Idera.SQLdm.CWFRegister
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            CWFRegisterConfiguration.InitLogging();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new CWFRegister());
        }
    }
}
