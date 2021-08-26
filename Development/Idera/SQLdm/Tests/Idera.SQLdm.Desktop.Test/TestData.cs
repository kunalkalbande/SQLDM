using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLdm.Desktop.Test
{
    internal static class TestData
    {
        internal static string GetRepositoryConnectionString()
        {
            return @"Data Source=FREEDOM\FOR_IDERA;Initial Catalog=SQLdmRepository85;Integrated Security=True;Asynchronous Processing=True;Connect Timeout=30;Encrypt=False;Application Name=""SQL Diagnostic Manager""";
        }
    }
}
