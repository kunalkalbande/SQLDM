using System;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.Snapshots;
using NUnit.Framework;
using System.Diagnostics;


namespace Idera.SQLdm.CoreServiceTest
{
    [TestFixture]
    class CoreServiceMethodsTest
    {
        MonitoredServerWorkload _workload;
        IManagementService _managementService;
        ICollectionService _collectionService;
        SqlConnectionInfo _sqlConnection = new SqlConnectionInfo();
        string _baseFolder = String.Empty;
        Process _collectionProcess;
        Process _managementProcess;

        [SetUp]
        public void Setup()
        {

            string currentFolder = Environment.CurrentDirectory;
            if (!String.IsNullOrWhiteSpace(currentFolder))
            {
                _baseFolder = currentFolder.Replace(@"\Idera\SQLdm\CoreServiceTest", String.Empty);

                //starting management service
                _collectionProcess = Process.Start(_baseFolder + @"\SQLdmManagementService.exe");

                //starting collection service
                _managementProcess = Process.Start(_baseFolder + @"\SQLdmCollectionService.exe");

                System.Threading.Thread.Sleep(50000); //wait for 50 second 


                while (ServiceInstallHelper.GetServiceStatus("SQLdmCollectionService") != ServiceState.Running)
                {
                    System.Threading.Thread.Sleep(1000);

                }

                _sqlConnection.InstanceName = "ACCOLITE-PC\\MSSQLSERVER12";
                _sqlConnection.DatabaseName = "SQLdmDatabase";
                _sqlConnection.UseIntegratedSecurity = true;

                _managementService = CoreServiceHelper.GetDefaultService<IManagementService>(_sqlConnection);
                _collectionService = CoreServiceHelper.GetDefaultService<ICollectionService>(_sqlConnection);


            }
            else
                throw new Exception("Could not find the CurrentDirectory , hence the services could not be run");
        }

        [Test]
        public void TestExpress()
        {
            try
            {
                Assert.IsNotNull(_managementService);
                Assert.IsNotNull(_collectionService);
                _managementService.ForceScheduledRefresh(22);
                //collectionService.StartMonitoringServer

                MonitoredSqlServer server = CoreServiceHelper.GetMonitoredSqlServer(_sqlConnection.ConnectionString,22);
                _workload = CoreServiceHelper.GetMonitoredServerWorkload(server, _sqlConnection.ConnectionString);
                _collectionService.StartMonitoringServer(_workload);
            }
            catch
            {
                throw;
            }
        }

        [Test]
        public void TestNonExpress()
        {
            try
            {
                Assert.IsNotNull(_managementService);
                Assert.IsNotNull(_collectionService);
                _managementService.ForceScheduledRefresh(21);
                //collectionService.StartMonitoringServer

                MonitoredSqlServer server=CoreServiceHelper.GetMonitoredSqlServer(_sqlConnection.ConnectionString, 21);
                _workload = CoreServiceHelper.GetMonitoredServerWorkload(server, _sqlConnection.ConnectionString);
                _collectionService.StartMonitoringServer(_workload);

            }
            catch
            {
                throw;
            }
        }


        [TearDown]
        public void TearDown()
        {

            //Stop the services if they are runing
            _collectionProcess.Kill();
            _managementProcess.Kill();

            //Clean up the objects
            _workload = null;
            _collectionProcess = null;
            _managementProcess = null;
            _managementService =null;
            _collectionService =null;
            _sqlConnection = null;
        }


    }
   
}

