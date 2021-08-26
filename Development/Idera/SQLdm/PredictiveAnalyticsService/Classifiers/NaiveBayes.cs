using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;

using PredictiveAnalyticsService.Math;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Idera.SQLdm.PredictiveAnalyticsService.Classifiers
{    
    internal sealed class NaiveBayes : Classifier
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("NaiveBayes");

        public IDistribution[,] AttributeDistributions
        {
            get { return attributeDistributions; }
        }

        public NaiveBayes()
        {
        }

        #region Classifier Members

        public override double ClassifyInput(DataRow dataRow)
        {
            double[] distributions = GetDistributions(dataRow);

            double max = 0;
            int    indexOfMax = 0;

            for (int i = 0; i < distributions.Length; i++)
            {
                if (distributions[i] > max)
                {
                    max = distributions[i];
                    indexOfMax = i;
                }
            }

            if (max > 0)
                return indexOfMax; // class value, since they are zero based ints

            return -1;
        }

        public override void BuildModel(DataAttribute[] attributes, DataTable datasetTable, int classIndex, int[] classes)
        {
            if (attributes == null || attributes.Length == 0)
                throw new ApplicationException("Must provide attributes for dataset!");

            if (attributes.Length != datasetTable.Columns.Count)
                throw new ApplicationException("Must provide an attribute for each column in the dataset!");

            if (datasetTable == null || datasetTable.Rows.Count == 0)
                throw new ApplicationException("No data to build classifier!");

            if (classIndex >= datasetTable.Columns.Count)
                throw new ApplicationException("Class index column exceeds data set columns!");

            if(classes == null || classes.Length == 0)
                throw new ApplicationException("No classes provided!");

            this.classes    = classes;
            this.dataset    = datasetTable;
            this.classIndex = classIndex;
            this.attributes = new DataAttribute[attributes.Length];

            Array.Copy(attributes, this.attributes, attributes.Length);

            // instantiate arrays to hold the distributions
            attributeDistributions = new IDistribution[attributes.Length, classes.Length];
            classDistributions     = new DiscreteDistribution(classes.Length);

            double numPrecision = 0.01;
            double lastVal;
            double currentVal;
            double deltaSum;
            int    distrinct;
            int    firstNonNullRowIndex;

            for (int i = 0; i < attributes.Length; i++)
            {
                numPrecision = 0.01;

                // estimate the precision of the values for this attribute (column)
                if (i != classIndex)
                {                   
                    // sort by the column
                    datasetTable.DefaultView.Sort = string.Format("{0}", datasetTable.Columns[i].ColumnName);

                    firstNonNullRowIndex = 0;

                    // find first non-null row
                    for (int j = 0; j < datasetTable.DefaultView.Count; j++)
                    {
                        if (!Double.IsNaN((double)datasetTable.DefaultView[j][i]))
                        {
                            firstNonNullRowIndex = j;
                            break;
                        }
                    }

                    lastVal    = (double)datasetTable.DefaultView[firstNonNullRowIndex][i];
                    currentVal = 0;
                    deltaSum   = 0;
                    distrinct  = 0;

                    for (int j = firstNonNullRowIndex+1; j < datasetTable.DefaultView.Count; j++)
                    {
                        if (Double.IsNaN((double)datasetTable.DefaultView[j][i]))
                            continue;

                        currentVal = (double)datasetTable.DefaultView[j][i];

                        if (currentVal != lastVal)
                        {
                            deltaSum += currentVal - lastVal;
                            lastVal   = currentVal;
                            distrinct++;
                        }
                    }

                    if (distrinct > 0)
                        numPrecision = deltaSum / distrinct;
                    else
                        numPrecision = 0.01;
                }

                for (int j = 0; j < classes.Length; j++)
                {
                    if (attributes[i].IsDiscrete)
                        attributeDistributions[i, j] = new DiscreteDistribution(attributes[i].NumDiscreteValues);                        
                    else
                        attributeDistributions[i, j] = new NormalDistribution(numPrecision);
                }   
            }            

            // Update the classifier using each row of the dataset
            foreach (DataRow row in dataset.Rows)
                UpdateModel(row);

            // Calculate the accuracy over the training data
            UpdateAccuracy();
        }

        public override byte[] GetBytes()
        {
            int buffersize = 0;

            buffersize += sizeof(int);                      // version
            buffersize += sizeof(int);                      // class index
            buffersize += sizeof(int);                      // number of classes
            buffersize += sizeof(int) * classes.Length;     // classes
            buffersize += classDistributions.SizeInBytes(); // class distributions
            buffersize += sizeof(int);                      // number of attributes
            buffersize += DataAttribute.SizeInBytes * attributes.Length; // attributes
            buffersize += NormalDistribution.SizeInBytes * attributeDistributions.Length * classes.Length; // attribute distributions

            byte[] buffer = new byte[buffersize];

            using (MemoryStream stream = new MemoryStream(buffer))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    // save the version
                    writer.Write(1);

                    // write the accuracy
                    writer.Write(accuracy);

                    // save the class index
                    writer.Write(classIndex);

                    // save the number of classes
                    writer.Write(classes.Length);

                    // save the classes
                    for (int i = 0; i < classes.Length; i++)
                        writer.Write(classes[i]);

                    // save the class distributions
                    byte[] classDistBuffer = classDistributions.GetBytes();
                    writer.Write(classDistBuffer, 0, classDistBuffer.Length);

                    // save the number of attributes
                    writer.Write(attributes.Length);

                    // save the attributes
                    for (int i = 0; i < attributes.Length; i++)
                    {
                        byte[] attrBuffer = attributes[i].GetBytes();
                        writer.Write(attrBuffer, 0, attrBuffer.Length);
                    }

                    // save the attribute distributions
                    for (int i = 0; i < attributes.Length; i++)
                    {
                        for (int j = 0; j < classes.Length; j++)
                        {
                            byte[] attrDistBuffer = attributeDistributions[i, j].GetBytes();
                            writer.Write(attrDistBuffer, 0, attrDistBuffer.Length);
                        }
                    }
                }
            }

            return buffer;
        }

        public override void InitFromBytes(byte[] buffer)
        {
            using (MemoryStream stream = new MemoryStream(buffer))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    // read the version
                    int version = reader.ReadInt32();

                    // read the accuracy
                    accuracy = reader.ReadDouble();

                    // read the class index
                    classIndex = reader.ReadInt32();

                    // read the number of classes
                    int numclasses = reader.ReadInt32();
                    classes = new int[numclasses];

                    // read the classes
                    for (int i = 0; i < classes.Length; i++)
                        classes[i] = reader.ReadInt32();

                    // read the class distributions
                    // cheat - we always know the number of classes (for now it's always 2 => {0, 1})
                    byte[] classDistBuffer = new byte[DiscreteDistribution.SizeInBytes(2)];
                    reader.Read(classDistBuffer, 0, classDistBuffer.Length);
                    classDistributions = new DiscreteDistribution(numclasses);
                    classDistributions.InitFromBytes(classDistBuffer);

                    // read the number of attributes
                    int numAttributes = reader.ReadInt32();
                    attributes = new DataAttribute[numAttributes];

                    // read the attributes
                    byte[] attrBuffer;
                    for (int i = 0; i < attributes.Length; i++)
                    {
                        attrBuffer = new byte[DataAttribute.SizeInBytes];
                        reader.Read(attrBuffer, 0, attrBuffer.Length);
                        attributes[i] = new DataAttribute();
                        attributes[i].InitFromBytes(attrBuffer);
                    }

                    attributeDistributions = new IDistribution[numAttributes, numclasses];                    

                    // read the attribute distributions
                    byte[] attrDistBufferDiscrete = new byte[DiscreteDistribution.SizeInBytes(2)]; // cheat - we know the first is always (and the only) discrete distribution
                    byte[] attrDistBufferNormal   = new byte[NormalDistribution.SizeInBytes];

                    for (int i = 0; i < numAttributes; i++)
                    {
                        for (int j = 0; j < classes.Length; j++)
                        {                                                        
                            if (i == 0)
                            {
                                attributeDistributions[i, j] = new DiscreteDistribution(numclasses);
                                reader.Read(attrDistBufferDiscrete, 0, attrDistBufferDiscrete.Length);
                                attributeDistributions[i, j].InitFromBytes(attrDistBufferDiscrete);
                            }
                            else
                            {
                                attributeDistributions[i, j] = new NormalDistribution(0);
                                reader.Read(attrDistBufferNormal, 0, attrDistBufferNormal.Length);
                                attributeDistributions[i, j].InitFromBytes(attrDistBufferNormal);
                            }                            
                        }
                    }
                }
            }
        }

        #endregion

        private void UpdateModel(DataRow instanceRow)
        {
            int classvalue = (int)(double)instanceRow[classIndex];

            // update distribution of class
            classDistributions.AddValue(classvalue, 1); // can later weight this instance based on time (older data weighted less)

            for (int i = 0; i < attributes.Length; i++)
            {
                // skip NaN values and class value
                if (Double.IsNaN((double)instanceRow[i]) || i == classIndex)
                    continue;

                // update distribution of values for this class
                attributeDistributions[i, classvalue].AddValue((double)instanceRow[i], 1); // can later weight rows such that more recent data is weighted more heavily
            }
        }

        public double[] GetDistributions(DataRow row)
        {
            double[] probabilities = new double[classes.Length];

            // calculate the probabilities for each class (as observed in the data)
            for (int i = 0; i < classes.Length; i++)
                probabilities[i] = classDistributions.CalculateProbability(i);

            double probability    = 0;
            double maxprobability = 0;

            for (int i = 0; i < attributes.Length; i++)
            {
                // skip NaN values and class attribute
                if (Double.IsNaN((double)row[i]) || i == classIndex)
                    continue;

                probability    = 0;
                maxprobability = 0;

                for (int j = 0; j < classes.Length; j++)
                {
                    probability       = System.Math.Max(1e-75, attributeDistributions[i, j].CalculateProbability((double)row[i]));
                    probabilities[j] *= probability;

                    if (probabilities[j] > maxprobability)
                        maxprobability = probabilities[j];

                    // make sure we have a number
                    if (Double.IsNaN(probabilities[j]))
                    {
                        LOG.DebugFormat("Probability estimate returned NaN for column {0}.", i);

                        for (int z = 0; z < probabilities.Length; z++)
                            probabilities[j] = 0;

                        return probabilities;                        
                    }

                }

                // under flow check (? might not be necessary)
                if (maxprobability > 0 && maxprobability < 1e-75)
                {
                    for (int j = 0; j < classes.Length; j++)
                        probabilities[j] *= 1e75;
                }
            }

            // normalize probabilities
            Utility.Normalize(probabilities);

            return probabilities;
        }

        private void UpdateAccuracy()
        {
            double truePositives  = 0;
            double trueNegatives  = 0;
            double falsePositives = 0;
            double falseNegatives = 0;

            int prediction = 0;
            int actual     = 0;

            foreach (DataRow row in dataset.Rows)
            {
                prediction = (int)ClassifyInput(row);
                actual     = (int)(double)row[0];

                if (actual == 1 && prediction == 1)
                    truePositives++;

                else if (actual == 1 && prediction == 0)
                    falsePositives++;

                else if (actual == 0 && prediction == 1)
                    falseNegatives++;

                else if (actual == 0 && prediction == 0)
                    trueNegatives++;
            }

            accuracy = (truePositives + trueNegatives) / (truePositives + trueNegatives + falsePositives + falseNegatives);
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();

            b.Append("==================================================\r\n");
            b.Append("\r\n");
            b.Append("Classes:\r\n");

            // class info
            for (int i = 0; i < classes.Length; i++)
                b.AppendFormat("{0}: {1}\r\n", i, classDistributions.CalculateProbability(i));

            b.Append("==================================================\r\n");
            b.Append("\r\n");
            b.Append("Attributes:\r\n");

            // atributes
            for (int i = 1; i < attributes.Length; i++)
            {
                if (attributes[i].IsDiscrete)
                {
                    // name

                    // mean

                    // stdev

                    // weight sum

                    // precision
                }
                else
                {
                    // name
                    b.AppendFormat("Attribute: {0}\r\n", dataset.Columns[i].ColumnName);

                    // mean
                    b.Append("\tMean:     ");
                    for (int j = classes.Length-1; j >= 0; j--)
                        b.AppendFormat("\t{0:#.####}", ((NormalDistribution)attributeDistributions[i, j]).Mean);

                    b.Append("\r\n");

                    // stdev
                    b.AppendFormat("\tStdDev:   ");
                    for (int j = classes.Length - 1; j >= 0; j--)
                        b.AppendFormat("\t{0:#.####}", ((NormalDistribution)attributeDistributions[i, j]).Stdev);

                    b.Append("\r\n");

                    // weight sum
                    b.AppendFormat("\tWeight Sum:");
                    for (int j = classes.Length - 1; j >= 0; j--)
                        b.AppendFormat("\t{0:#.####}", ((NormalDistribution)attributeDistributions[i, j]).WeightsSum);

                    b.Append("\r\n");

                    // precision
                    b.AppendFormat("\tPrecision:");
                    for (int j = classes.Length - 1; j >= 0; j--)
                        b.AppendFormat("\t{0:#.####}", ((NormalDistribution)attributeDistributions[i, j]).Precision);

                    b.Append("\r\n");

                    b.Append("\r\n\r\n");
                }
            }

            return b.ToString();
        }
    }
}
