using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


// SQLdm 10.1 (Srishti Purohit) -- class added to give structure to Instance Health coefficients
namespace Idera.SQLdm.Service.DataContracts.v1
{
    public class InstanceScaleFoctor
    {
        private double? instanceScaleFactor;
                
        public int SQLServerId { get; set; }

        public string InstanceName { get; set; }

        public bool IsActive { get; set; }

        public double? InstanceHealthScaleFactor
        {
            get { return instanceScaleFactor; }
            set { instanceScaleFactor = value; }
        }

        public InstanceScaleFoctor()
        {
            // Default init
            IsActive = true;
        }
        public bool IsInstanceHealthScaleFactorSet { get; set; }// Sqldm 10.1 (Pulkit Puri) -- for adding new member to class for implementaion of health index for instances

    }
}
