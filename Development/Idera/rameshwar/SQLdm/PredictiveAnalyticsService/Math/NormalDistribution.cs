using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace PredictiveAnalyticsService.Math
{
    internal sealed class NormalDistribution : IDistribution
    {
        private double precision;
        private double precisionHalf;
        private double mean;
        private double stdev;
        private double stdevDefault;
        private double sumWeights;
        private double sumValues;
        private double sumValuesSquared;        

        public double Precision
        {
            get { return precision; }
        }

        public double Mean
        {
            get { return mean; }
        }

        public double Stdev
        {
            get { return stdev; }
        }

        public double WeightsSum
        {
            get { return sumWeights; }
        }        

        public NormalDistribution(double pre)
        {
            precision     = pre;
            precisionHalf = precision / 2;
            stdev         = precision / (2 * 3); // 3 stdev's within a single interval
            stdevDefault  = stdev;

            sumWeights       = 0;
            sumValues        = 0;
            sumValuesSquared = 0;
        }

        #region IDistribution Members

        public void AddValue(double data, double weight)
        {
            if (weight == 0)
                return;

            data = Round(data);

            sumWeights       += weight;
            sumValues        += data * weight;
            sumValuesSquared += data * data * weight;

            if (sumWeights == 0)
                return;

            mean = sumValues / sumWeights;

            double tempStdDev = System.Math.Sqrt(System.Math.Abs(sumValuesSquared - mean * sumValues) / sumWeights);

            if (tempStdDev > 1e-10)
                stdev = System.Math.Max(stdevDefault, tempStdDev);
        }

        public double CalculateProbability(double data)
        {
            data = Round(data);

            double zLower = (data - mean - precisionHalf) / stdev;
            double zUpper = (data - mean + precisionHalf) / stdev;
            double pLower = Utility.NormalProbability(zLower);
            double pUpper = Utility.NormalProbability(zUpper);

            return pUpper - pLower;
        }

        private double Round(double data)
        {
            return System.Math.Round(data / precision) * precision;
        }

        public byte[] GetBytes()
        {
            byte[] buffer = new byte[SizeInBytes];

            using (MemoryStream stream = new MemoryStream(buffer))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(precision);
                    writer.Write(precisionHalf);
                    writer.Write(mean);
                    writer.Write(stdev);
                    writer.Write(stdevDefault);
                    writer.Write(sumWeights);
                    writer.Write(sumValues);
                    writer.Write(sumValuesSquared);
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
                    precision        = reader.ReadDouble();
                    precisionHalf    = reader.ReadDouble();
                    mean             = reader.ReadDouble();
                    stdev            = reader.ReadDouble();
                    stdevDefault     = reader.ReadDouble();
                    sumWeights       = reader.ReadDouble();
                    sumValues        = reader.ReadDouble();
                    sumValuesSquared = reader.ReadDouble(); 
                }
            }
        }

        public static int SizeInBytes
        {
            get
            {
                return sizeof(double) * 8; // 8 fields of type double
            }
        }

        #endregion
    }
}
