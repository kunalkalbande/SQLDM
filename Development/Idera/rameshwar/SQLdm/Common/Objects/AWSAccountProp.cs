using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLdm.Common.Objects
{
    [Serializable]
    public class AWSAccountProp
    {
        public AWSAccountProp()
        {
        }

        public string aws_access_key { get; set; }
        public string aws_secret_key { get; set; }
        public string aws_region_endpoint { get; set; }
    }
}
