using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Wintellect.PowerCollections;

namespace Idera.SQLdm.Common.Services
{
    public interface IPredictiveAnalytics
    {
        /// <summary>
        /// Returns a list of servers for which alerts have been generated.
        /// </summary>
        /// <returns></returns>
        Dictionary<int, List<int>> GetServerAlerts();

        /// <summary>
        /// Returns a table containing the servers that have predictive models stored in the repository.
        /// </summary>
        /// <returns></returns>
        DataTable GetPredictiveModelServers();

        /// <summary>
        /// Returns the predictive model (as a byte buffer) for the given server, etc.
        /// </summary>
        /// <param name="serverid"></param>
        /// <returns></returns>
        Dictionary<Triple<int, int, int>, byte[]> GetPredictiveModelsForServer(int serverid);

        /// <summary>
        /// Returns the input for a predictive model averaged over the last intervalMinutes starting
        /// at the cutoff datetime.
        /// </summary>
        /// <param name="serverid"></param>
        /// <param name="intervalMinutes"></param>
        /// <param name="cutoffDateTime"></param>
        /// <returns></returns>
        DataTable GetPredictiveModelInput(int serverid, int intervalMinutes, DateTime cutoffDateTime);

        /// <summary>
        /// Returns the training data to build a predictive model.
        /// </summary>
        /// <param name="serverid"></param>
        /// <param name="metricid"></param>
        /// <param name="severity"></param>
        /// <param name="timeframe"></param>
        /// <param name="cutoffDateTime"></param>
        /// <returns></returns>
        Pair<DataTable, List<DateTime>> GetPredictiveTrainingData(int serverid, int metricid, int severity, int timeframe, DateTime cutoffDateTime);

        /// <summary>
        /// Saves a predictive model to the repository.
        /// </summary>
        /// <param name="serverid"></param>
        /// <param name="metricid"></param>
        /// <param name="severity"></param>
        /// <param name="timeframe"></param>
        /// <param name="modelDataBuffer"></param>
        void SavePredictiveModel(int serverid, int metricid, int severity, int timeframe, byte[] modelDataBuffer);

        /// <summary>
        /// Saves a predictive model forecast to the repository.
        /// </summary>
        /// <param name="serverid"></param>
        /// <param name="metricid"></param>
        /// <param name="severity"></param>
        /// <param name="timeframe"></param>
        /// <param name="forecast"></param>
        /// <param name="accuracy"></param>
        /// <param name="expiration"></param>
        void SavePredictiveForecast(int serverid, int metricid, int severity, int timeframe, int forecast, double accuracy, DateTime expiration);

        /// <summary>
        /// Deletes forecasts that have expired.
        /// </summary>
        void GroomExpiredForecasts();
    }
}
