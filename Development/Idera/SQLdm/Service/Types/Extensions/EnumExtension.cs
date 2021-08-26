using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLdm.Service.Types.Extensions
{
    public static class EnumExtension
    {
        public static String GetNameFromValue(this Enum eff)
        {
            return Enum.GetName(eff.GetType(), eff);
        }
    }
}
