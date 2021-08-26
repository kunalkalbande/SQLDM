using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace PredictiveAnalyticsService.Math
{
    internal sealed class DiscreteDistribution : IDistribution
    {
        private double[] counts;
        private double   sum;

        public DiscreteDistribution(int numClasses)
        {
            counts = new double[numClasses];
            sum    = numClasses;

            for (int i = 0; i < numClasses; i++)
                counts[i] = 1; // laplace smoothing, assume at least 1
        }

        #region IDistribution Members

        public void AddValue(double data, double weight)
        {
            counts[(int)data] += weight;
            sum               += weight;
        }

        public double CalculateProbability(double data)
        {
            if (sum == 0)
                return 0;

            return (double)counts[(int)data] / sum;
        }

        public byte[] GetBytes()
        {
            byte[] buffer = new byte[this.SizeInBytes()];

            using (MemoryStream stream = new MemoryStream(buffer))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    // sum
                    writer.Write(sum);

                    // counts
                    for (int i = 0; i < counts.Length; i++)
                        writer.Write(counts[i]);
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
                    sum = reader.ReadDouble();

                    counts = new double[2]; // cheat - we know there are only 2 classes

                    for (int i = 0; i < counts.Length; i++)
                        counts[i] = reader.ReadDouble();
                }
            }
        }

        public int SizeInBytes()
        {
            return sizeof(double) +                 // sum
                   sizeof(double) * counts.Length;  // counts
        }

        public static int SizeInBytes(int numClasses)
        {
            return sizeof(double) +              // sum
                   sizeof(double) * numClasses;  // counts
        }

        #endregion
    }
}
