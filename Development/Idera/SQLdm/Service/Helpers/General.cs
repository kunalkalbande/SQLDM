using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLdm.Service.Helpers
{
    internal class GeneralHelper
    {
        /// <summary>
        /// Properly escapes a string containing single-quote characters for use in a SQL statement.
        /// </summary>
        public static string EscapeQuotes(string value)
        {
            if (value == null)
                return string.Empty;

            StringBuilder builder = new StringBuilder(value);

            builder.Replace("'", "''");

            return builder.ToString();
        }
    }
}
