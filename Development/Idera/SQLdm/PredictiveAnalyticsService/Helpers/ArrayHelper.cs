using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.PredictiveAnalyticsService.Helpers
{
    static class ArrayHelper
    {
        public static string ArrayToString<T>(T[] array)
        {
            return ArrayToString<T>(array, ",");
        }

        public static string ArrayToString<T>(T[] array, string delimiter)
        {
            StringBuilder b = new StringBuilder();

            if (array == null || array.Length == 0)
                return string.Empty;

            b.Append(array[0]);

            for (int i = 1; i < array.Length; i++)
                b.Append(string.Format("{0}{1}", delimiter, array[i].ToString()));

            return b.ToString();
        }
    }
}
