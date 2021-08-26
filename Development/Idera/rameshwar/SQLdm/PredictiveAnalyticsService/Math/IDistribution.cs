using System;
using System.Collections.Generic;
using System.Text;

namespace PredictiveAnalyticsService.Math
{
    interface IDistribution
    {
        void AddValue(double data, double weight);

        double CalculateProbability(double data);

        byte[] GetBytes();

        void InitFromBytes(byte[] buffer);
    }
}
