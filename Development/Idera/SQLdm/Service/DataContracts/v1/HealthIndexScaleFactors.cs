using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

// SQLdm 10.1 (Srishti Purohit) -- class added to give structure to encapsulate Health coefficients for instance and tag
namespace Idera.SQLdm.Service.DataContracts.v1
{
    [DataContract]
    public class HealthIndexScaleFactors
    {
        private HealthIndexCoefficient healthIndexCoefficients = new HealthIndexCoefficient();
        private IList<InstanceScaleFoctor> instanceScaleFactorList = new List<InstanceScaleFoctor>();

        private IList<TagScaleFactor> tagScaleFactorList = new List<TagScaleFactor>();

        [DataMember]
        public HealthIndexCoefficient HealthIndexCoefficients
        {
            get
            {
                return healthIndexCoefficients;
            }
            set
            {
                healthIndexCoefficients = value;
            }
        }
        [DataMember]
        public IList<InstanceScaleFoctor> InstanceScaleFactorList
        {
            get
            {
                if (instanceScaleFactorList == null)
                    return new List<InstanceScaleFoctor>();
                return instanceScaleFactorList;
            }
            set
            {
                instanceScaleFactorList = value;
            }
        }
        [DataMember]
        public IList<TagScaleFactor> TagScaleFactorList
        {
            get
            {
                if (tagScaleFactorList == null)
                    return new List<TagScaleFactor>();
                return tagScaleFactorList;
            }
            set
            {
                tagScaleFactorList = value;
            }
        }
    }
}
