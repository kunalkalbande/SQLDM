using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// SQLdm 10.1 (Srishti Purohit) -- class added to give structure to Tag Health coefficients

namespace Idera.SQLdm.Service.DataContracts.v1
{
    public class TagScaleFactor
    {
        
        public int TagId { get; set; }

        public string TagName { get; set; }

        private double? tagScaleFactor;

        public double? TagHealthScaleFactor
        {
            get { return tagScaleFactor; }
            set { tagScaleFactor = value; }
        }

        public bool IsTagHealthScaleFactorSet { get; set; }// SQLdm (10.1) (Pulkit Puri)--For adding new member 
    }
}
