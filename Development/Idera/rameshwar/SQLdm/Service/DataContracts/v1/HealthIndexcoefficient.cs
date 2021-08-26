using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


// SQLdm 10.1 (Srishti Purohit) -- class added to give structure to global Health coefficients values
namespace Idera.SQLdm.Service.DataContracts.v1
{
    public class HealthIndexCoefficient
    {
        private double healthIndexCoefficientForCriticalAlert = Common.Constants.HEALTH_INDEX_COEFFICIENT_CRITICAL_ALERT;
        private double healthIndexCoefficientForWarningAlert = Common.Constants.HEALTH_INDEX_COEFFICIENT_WARNING_ALERT;
        private double healthIndexCoefficientForInformationalAlert = Common.Constants.HEALTH_INDEX_COEFFICIENT_INFO_ALERT;

        public double HealthIndexCoefficientForCriticalAlert
        {
            get { return healthIndexCoefficientForCriticalAlert; }
            set { healthIndexCoefficientForCriticalAlert = value; }
        }

        public double HealthIndexCoefficientForWarningAlert
        {
            get { return healthIndexCoefficientForWarningAlert; }
            set { healthIndexCoefficientForWarningAlert = value; }
        }

        public double HealthIndexCoefficientForInformationalAlert
        {
            get { return healthIndexCoefficientForInformationalAlert; }
            set { healthIndexCoefficientForInformationalAlert = value; }
        }

        //Sqldm 10.1 (Pulkit Puri) --for implementing healthindex for severity list
        public bool IsHealthIndexCoefficientForCriticalAlertSet{ get; set; }
        public bool IsHealthIndexCoefficientForWarningAlertSet { get; set; }
        public bool IsHealthIndexCoefficientForInformationalAlertSet{ get; set; }
        //Sqldm 10.1 (Pulkit Puri)

        public HealthIndexCoefficient()
        {
            // Default init
        }
    }

    public enum HealthIndexes
    {
        Critical = 1,
        Warning = 2,
        Informational = 3
    }
}
