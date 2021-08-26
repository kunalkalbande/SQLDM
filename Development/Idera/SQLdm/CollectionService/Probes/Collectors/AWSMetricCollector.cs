

namespace Idera.SQLdm.CollectionService.Probes.Collectors
{
    using Amazon;
    using Amazon.CloudWatch;
    using Amazon.CloudWatch.Model;
    using Amazon.RDS;
    using Amazon.RDS.Model;
    using Amazon.Util;
    using BBS.TracerX;
    using Common;
    using Common.Configuration;
    using Common.Data;
    using Common.Objects;
    using Common.Services;
    using Common.Snapshots.Cloud;
    using Configuration;
    using Microsoft.ApplicationBlocks.Data;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    class AWSMetricCollector : BaseCollector
    {
        protected static Logger Log = Logger.GetLogger("AWS Metric Probe");

        private Dictionary<string, double> amazonRDSMetric = new Dictionary<string, double>();
        private string _instanceName;

        private const string WINDOWS_NET = "windows.net";
        private const string AZURE_COM = "azure.com";
        private const string AMAZONAWS_COM = "amazonaws.com";
        private const string GetAWSResourceDetailsStoredProcedure = "p_GetAWSResourceDetails";

        public AWSMetricCollector(string instanceName)
        {
            _instanceName = instanceName;
        }

        public override IAsyncResult BeginCollection(EventHandler<CollectorCompleteEventArgs> collectionCompleteCallback)
        {
            if (collectionCompleteCallback == null)
                throw new ArgumentNullException("collectionCompleteCallback");

            this.collectionCompleteCallback = collectionCompleteCallback;

            if (AsyncWaitHandle != null)
                throw new Exception("Attempt to call BeginCollection() twice on one instance");

            IAsyncResult queryResult = null;

            bool checkAwsCred = AWSMetricCollector.IsValidAWSCredentials(_instanceName);

            string[] metricnames = Enum.GetNames(typeof(CloudMetricList.AWSMetric));
            foreach (string metricname in metricnames)
            {
                double metricval = checkAwsCred == true ? GetAWSCloudWatchMetrics(_instanceName, metricname) : 0;
                amazonRDSMetric.Add(metricname, metricval);
            }
            Log.InfoFormat("AWS Collector completed with {0} collection.", "AWS CloudWatch Metric");

            collectionCompleteCallback(this, new CollectorCompleteEventArgs(amazonRDSMetric, 0, Result.Success));

            return queryResult;
        }

        public override IAsyncResult BeginCollectionNonQueryExecution(EventHandler<CollectorCompleteEventArgs> collectionNonQueryExecutionCompleteCallback)
        {
            throw new NotImplementedException();
        }

        public static AWSAccountProp GetAwsAccountPropDetails(string instanceName)
        {
            AWSAccountProp prop = new AWSAccountProp();

            ServiceCallProxy proxy = new ServiceCallProxy(typeof(IManagementService), CollectionServiceConfiguration.ManagementServiceUri.ToString() + "Management");
            IManagementService managementService = proxy.GetTransparentProxy() as IManagementService;

            prop = managementService.GetAWSResourcePrincipleDetails(instanceName);

            return prop;
        }

        public static string GetAWSInstanceId(string instanceName, AWSAccountProp awsAccount)
        {
            string instanceId = string.Empty;

            // Get current region
            RegionEndpoint currentRegion = RegionEndpoint.EnumerableAllRegions.Where(region => region.SystemName == awsAccount.aws_region_endpoint).FirstOrDefault();
            // Get RDS client
            AmazonRDSClient amazonRDSClient = new AmazonRDSClient(awsAccount.aws_access_key, awsAccount.aws_secret_key, currentRegion);
            // Generate RDS instance request
            DescribeDBInstancesRequest instanceRDSRequest = new DescribeDBInstancesRequest();
            DescribeDBInstancesResponse instanceRDSResponse = amazonRDSClient.DescribeDBInstances(instanceRDSRequest);
            // Get DNS name from fetched instance
            if (null != instanceRDSResponse.DBInstances && 0 < instanceRDSResponse.DBInstances.Count)
            {
                instanceId = instanceRDSResponse.DBInstances.Where(i => i.Endpoint != null && !string.IsNullOrWhiteSpace(i.Endpoint.Address) && string.Equals(i.Endpoint.Address, instanceName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault().DBInstanceIdentifier;
            }

            return instanceId;
        }

        public static bool IsValidAWSCredentials(string _instanceName)
        {
            bool isvalid = false;
            using (Log.InfoCall("IsValidAWSCredentials"))
            {

                try
                {
                    // Get account details for current cloud id
                    AWSAccountProp awsAccount = GetAwsAccountPropDetails(_instanceName);
                    if (null != awsAccount && !string.IsNullOrEmpty(awsAccount.aws_region_endpoint))
                    {
                        GetAWSInstanceId(_instanceName, awsAccount);
                        isvalid = true;
                    }
                }
                catch (Exception exception)
                {
                    Log.InfoFormat("Exception - AWS cloudwatch {0}", exception.Message);
                    isvalid = false;

                }
            }
            return isvalid;
        }

        public static double GetAWSCloudWatchMetrics(string _instanceName, string metricName)
        {
            double metricValue = 0;
            using (Log.InfoCall("GetAWSCloudWatchMetrics"))
            {
                Log.InfoFormat("AWS CloudWatch - Started reading {0} metric for instance {1} at {2}", metricName, _instanceName, DateTime.Now);
                try
                {
                    // Get account details for current cloud id
                    AWSAccountProp awsAccount = GetAwsAccountPropDetails(_instanceName);
                    if (null != awsAccount)
                    {
                        if (awsAccount.aws_access_key != "" && awsAccount.aws_secret_key != "" && awsAccount.aws_region_endpoint != "")
                        {
                            // Get current region
                            RegionEndpoint currentRegion = RegionEndpoint.EnumerableAllRegions.Where(region => region.SystemName == awsAccount.aws_region_endpoint).FirstOrDefault();
                            // Get AWS CloudWatch client
                            AmazonCloudWatchClient client = new AmazonCloudWatchClient(awsAccount.aws_access_key, awsAccount.aws_secret_key, currentRegion);
                            // Generate metric statistic request
                            GetMetricStatisticsRequest request = new GetMetricStatisticsRequest();
                            // Get statistics from AWS/RDS namespace
                            request.Namespace = "AWS/RDS";
                            // Set dimension for EC2 instance
                            request.Dimensions.Add(new Dimension()
                            {
                                Name = "DBInstanceIdentifier",
                                Value = GetAWSInstanceId(_instanceName, awsAccount)
                            });
                            // Get average statistics for current instance id
                            request.Statistics.Add("Average");
                            // Get statistics by day (86400 seconds for 1 day)
                            request.Period = 60; // (int)TimeSpan.FromDays(1).TotalSeconds;
                                                 // Set the metric name
                            request.MetricName = metricName;
                            // Define the time span, get data for last 1 day
                            DateTime currentTime = DateTime.UtcNow;
                            DateTime startTime = currentTime;
                            startTime = startTime.AddHours(-1);
                            // Define the meyric unit
                            switch (metricName)
                            {
                                case "CPUUtilization":
                                    // Get metric for last 1 hour
                                    request.Unit = "Percent";
                                    break;
                                case "ReadThroughput":
                                case "WriteThroughput":
                                    request.Unit = "Bytes/Second";
                                    break;
                                case "SwapUsage":
                                    // Get metric for last 1 hour
                                    request.Unit = "Bytes";
                                    break;
                                case "ReadLatency":
                                case "ReadLatencyLow":
                                case "WriteLatency":
                                case "WriteLatencyLow":
                                    // Get metric for last 1 hour
                                    request.Unit = "Seconds";
                                    break;
                                case "CPUCreditBalance":
                                case "CPUCreditBalanceHigh":
                                case "CPUCreditUsage":
                                case "DiskQueueDepth":
                                    // Get metric for last 1 hour
                                    request.Unit = "Count";
                                    break;
                                case "FreeableMemory":
                                    // Get metric for last 1 hour
                                    request.Unit = "Bytes";
                                    break;
                            }
                            // Set metric capturing start and end time
                            request.StartTimeUtc = Convert.ToDateTime(startTime.ToString(AWSSDKUtils.ISO8601DateFormat, CultureInfo.InvariantCulture.DateTimeFormat));
                            request.EndTimeUtc = Convert.ToDateTime(currentTime.ToString(AWSSDKUtils.ISO8601DateFormat, CultureInfo.InvariantCulture.DateTimeFormat));
                            // Get the metrics statistics response
                            GetMetricStatisticsResponse response = client.GetMetricStatistics(request);
                            if (response.Datapoints.Count > 0)
                            {
                                switch (metricName)
                                {
                                    case "CPUUtilization":
                                        metricValue = Convert.ToDouble(response.Datapoints[0].Average, CultureInfo.InvariantCulture);
                                        break;
                                    case "ReadThroughput":
                                    case "WriteThroughput":
                                        metricValue = Convert.ToDouble(response.Datapoints[0].Average, CultureInfo.InvariantCulture) / 1000000;
                                        break;
                                    case "SwapUsage":
                                        metricValue = Convert.ToDouble(response.Datapoints[0].Average, CultureInfo.InvariantCulture);
                                        break;
                                    case "ReadLatency":
                                    case "ReadLatencyLow":
                                    case "WriteLatency":
                                        metricValue = Convert.ToDouble(response.Datapoints[0].Average, CultureInfo.InvariantCulture) * 1000;
                                        break;
                                    case "WriteLatencyLow":
                                        metricValue = Convert.ToDouble(response.Datapoints[0].Average, CultureInfo.InvariantCulture) * 1000;
                                        break;
                                    case "CPUCreditBalance":
                                    case "CPUCreditBalanceHigh":
                                    case "CPUCreditUsage":
                                    case "DiskQueueDepth":
                                        metricValue = Convert.ToDouble(response.Datapoints[0].Average, CultureInfo.InvariantCulture);
                                        break;
                                    case "FreeableMemory":
                                        metricValue = Convert.ToDouble(response.Datapoints[0].Average, CultureInfo.InvariantCulture);
                                        break;
                                }
                            }
                        }
                        Log.InfoFormat("AWS cloudwatch - Found {0} metric value as {1} for instance {2} at {3}", metricName, metricValue, _instanceName, DateTime.Now);
                    }
                }
                catch (Exception exception)
                {
                    Log.InfoFormat("Exception - AWS cloudwatch {0}", exception.Message);
                }
                Log.InfoFormat("AWS cloudwatch - Completed reading {0} metric for instance {1} at {2}", metricName, _instanceName, DateTime.Now);
            }
            return metricValue;
        }
    }
}
