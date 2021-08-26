using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLdm.Service.Helpers
{
    internal static class BuisnessLogicHelper
    {
        #region Constants
        //SQLDM 10.1 (Pulkit Puri) health scale factor implementation
        public const double MAX_SCALE_FACTOR = 10.0;
        public const double MIN_SCALE_FACTOR = 0.0;
        #endregion

        #region HealthScaleFactors
        //SQLdm 10.1 (Pulkit Puri): To check validity of healthscalefactor
        public static bool IsHealthscaleFactorValid(double? HealthscaleFactor)
        {
            if (HealthscaleFactor > MAX_SCALE_FACTOR|| HealthscaleFactor < MIN_SCALE_FACTOR)
                return false;
            else
                return true;
        }
      #endregion
    }
}
