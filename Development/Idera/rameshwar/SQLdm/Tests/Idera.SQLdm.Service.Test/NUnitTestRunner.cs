using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Idera.SQLdm.Service.Test
{
    internal class NUnitTestRunner
    {
        const string DEFAULT_NUNIT_CONSOLE_RUNNER_PATH = "nunit-console.exe";
        string DEFAULT_NUNIT_CONSOLE_RESULT_FILE_NAME = "TestcaseExecutionResult_" + DateTime.Now.ToLongDateString();

        public string nunit_console_runner_path { get; set; }
        public string MyProperty { get; set; }

        public NUnitTestRunner(string console_runner_path) 
        {
            
        }
    
        

    }
}
