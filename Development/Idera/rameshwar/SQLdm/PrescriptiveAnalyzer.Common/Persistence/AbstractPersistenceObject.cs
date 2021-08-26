using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Persistence
{
    [Serializable]
    public abstract class AbstractPersistenceObject : ISerializable
    {
        public abstract string GetFileName();
        public abstract void GetObjectData(SerializationInfo info, StreamingContext context);
        public abstract void Save();
        
    }
}
