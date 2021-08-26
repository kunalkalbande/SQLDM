using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.Common.Analysis
{
    using System;
    using System.Xml.Serialization;
    using System.Runtime.Serialization;

    [Serializable]
    public class RecommendationCategory : ISerializable 
    {
        [XmlAttribute]
        public int CategoryId;
        [XmlAttribute]
        public string CategoryName;
        public RecommendationCategory()
        {
        }
        public RecommendationCategory(int id, string name)
        {
            this.CategoryId = id;
            this.CategoryName = name;
        }
        public RecommendationCategory(SerializationInfo info, StreamingContext context)
        {
            CategoryId = info.GetInt32("CategoryId");
            CategoryName = info.GetString("CategoryName");
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("CategoryId", CategoryId);
            info.AddValue("CategoryName", CategoryName);
        }
    }
}
