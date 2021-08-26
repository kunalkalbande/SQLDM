

namespace Idera.SQLdm.PredictiveAnalyticsService.Classifiers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.IO;

    internal sealed class DataAttribute
    {
        public bool IsDiscrete;
        public bool Ignore;
        public int  NumDiscreteValues;

        public DataAttribute()
        {
        }

        public byte[] GetBytes()
        {
            byte[] buffer = new byte[SizeInBytes];

            using (MemoryStream stream = new MemoryStream(buffer))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(IsDiscrete);
                    writer.Write(Ignore);
                    writer.Write(NumDiscreteValues);
                }
            }

            return buffer;
        }

        public void InitFromBytes(byte[] buffer)
        {
            using (MemoryStream stream = new MemoryStream(buffer))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    IsDiscrete        = reader.ReadBoolean();
                    Ignore            = reader.ReadBoolean();
                    NumDiscreteValues = reader.ReadInt32();
                }
            }
        }

        public static int SizeInBytes
        {
            get
            {
                return sizeof(bool) * 2 + sizeof(int);
            }
        }
    }
}
