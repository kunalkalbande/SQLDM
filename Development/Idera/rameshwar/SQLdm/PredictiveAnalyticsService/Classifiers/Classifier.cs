using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;
using PredictiveAnalyticsService.Math;
using System.Runtime.Serialization;

namespace Idera.SQLdm.PredictiveAnalyticsService.Classifiers
{
    abstract class Classifier
    {
        protected DataTable       dataset;
        protected int             classIndex;
        protected DataAttribute[] attributes;
        protected int[]           classes;
        protected IDistribution[,]     attributeDistributions;
        protected DiscreteDistribution classDistributions;
        protected double          accuracy;

        public int ClassIndex
        {
            get { return classIndex; }
        }

        public bool IsAttributeDiscrete(int idx)
        {
            return attributes[idx].IsDiscrete;
        }

        public double Accuracy
        {
            get { return accuracy; }
        }

        public abstract void BuildModel(DataAttribute[] attributes, DataTable datasetTable, int classIndex, int[] classes);

        public abstract double ClassifyInput(DataRow dataRow);

        public abstract byte[] GetBytes();

        public abstract void InitFromBytes(byte[] bytes);        
    }
}
