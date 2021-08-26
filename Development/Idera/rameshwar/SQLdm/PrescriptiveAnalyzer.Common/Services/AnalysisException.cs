using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdoctor.Common.Services
{
    [Serializable]
    public class AnalysisException : Exception
    {
        private Category area;

        public AnalysisException(Category area, string message) : base(message)
        {
            this.area = area;
        }

        public AnalysisException(Category area, string message, Exception innerException) : base(message, innerException)
        {
            this.area = area;
        }

        public AnalysisException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.area = (Category)info.GetValue("area", typeof(Category));
        }

        public override void  GetObjectData(SerializationInfo info, StreamingContext context)
        {
 	        base.GetObjectData(info, context);
            info.AddValue("area", area);
        }

        public Category Area
        {
            get { return area; }
            set { area = value; }
        }



    }
}
