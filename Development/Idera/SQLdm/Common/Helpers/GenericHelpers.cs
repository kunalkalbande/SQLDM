// ===============================
// AUTHOR       : CWF Team - Gowrish 
// PURPOSE      : Backend Isolation
// TICKET       : SQLDM-29086
// ===============================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace Idera.SQLdm.Common.Helpers
{
    public static class GenericHelpers
    {
        public static String ConcatStrings(params String[] values)
        {
            StringBuilder builder = new StringBuilder();
            foreach (String value in values)
            {
                builder.Append(value);
            }
            return builder.ToString();
        }

        public static void UpdateRegistryForAuthTokenAndRefreshToken(String authToken, String refreshToken)
        {
            if (!String.IsNullOrEmpty(authToken))
            {
                RegistryHelper.SetValueInRegistry(Constants.AuthTokenRegistryKey, authToken);
            }
            if (!String.IsNullOrEmpty(refreshToken))
            {
                RegistryHelper.SetValueInRegistry(Constants.RefreshTokenRegistryKey, refreshToken);
            }
        }
    }
}
