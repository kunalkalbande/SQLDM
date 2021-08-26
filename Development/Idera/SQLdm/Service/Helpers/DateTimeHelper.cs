using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Idera.SQLdm.Service.Helpers
{
    internal static class DateTimeHelper
    {
        internal static DateTime ConvertToUTC(DateTime dateTimeToConvert, string offsetInString) 
        {
            DateTime retValue = DateTime.MinValue;
            
            if (!dateTimeToConvert.Equals(DateTime.MinValue) && !string.IsNullOrWhiteSpace(offsetInString)) 
            {
                float numericOffsetInHours = 0;
                // SQLdm 10.1.3 (Varun Chopra) SQLDM-25316: Invariant culture to parse float
                if (float.TryParse(offsetInString, out numericOffsetInHours) ||
                    float.TryParse(offsetInString, NumberStyles.Float, CultureInfo.InvariantCulture,
                        out numericOffsetInHours))
                {
                    retValue = dateTimeToConvert.AddHours(Math.Round(numericOffsetInHours,2));
                }
            }
            return retValue;
        }

        internal static DateTime ConvertToLocal(DateTime dateTimeToConvert, string offsetInString)
        {
            DateTime retValue = DateTime.MinValue;

            if (!dateTimeToConvert.Equals(DateTime.MinValue) && !string.IsNullOrWhiteSpace(offsetInString))
            {
                float numericOffsetInHours = 0;
                // SQLdm 10.1.3 (Varun Chopra) SQLDM-25316: Invariant culture to parse float
                if (float.TryParse(offsetInString, out numericOffsetInHours) ||
                    float.TryParse(offsetInString, NumberStyles.Float, CultureInfo.InvariantCulture,
                        out numericOffsetInHours))
                {
                    retValue = dateTimeToConvert.AddHours(Math.Round(-numericOffsetInHours, 2));
                }
            }
            return retValue;
        }


    }
}
