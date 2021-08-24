using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace CWFInstallerService
{
    public static class Validator
    {

        public static void ValidateServiceCredentials(string serviceAccount, string servicePassword)
        {
            if (string.IsNullOrEmpty(serviceAccount)) throw new EmptyUserNameException();
            if (string.IsNullOrEmpty(servicePassword)) throw new EmptyPasswordException();

            // Extract domain and user info from the account.
            string domain = string.Empty;
            string user = string.Empty;
            if (serviceAccount.IndexOf(@"\") != -1)
            {
                string[] array = serviceAccount.Split('\\');
                domain = array[0];
                user = array[1];
            }
            if (string.IsNullOrEmpty(domain) || string.IsNullOrEmpty(user)) throw new InvalidFormatException();
            AccountHelper.IsUserLocalAdministrator(domain, user, servicePassword);
        }

        public static void ValidatePorts(List<int> ports)
        {
            // Check if all the ports are unique or not
            var duplicates = ports.GroupBy(item => item)
                .SelectMany(grp => grp.Skip(1).Take(1)).ToList();
            if (duplicates.Count > 0)
                throw new PortsNotUniqueException(duplicates);
            foreach(int port in ports)
            {
                // Validate each of port
                validatePort(port);
            }

        }

        public static void ValidateRemotePort(string hostname, List<int> ports)
        {
            var duplicates = ports.GroupBy(item => item)
                .SelectMany(grp => grp.Skip(1).Take(1)).ToList();
            if (duplicates.Count > 0)
                throw new PortsNotUniqueException(duplicates);
            foreach(int port in ports)
            {
                validateEachRemotePort(hostname, port);
            }
        }

        public static void validateEachRemotePort(string hostname, int port)
        {
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                try
                {
                    socket.Connect(hostname, port);
                    //throw new exception if not able to connect
                    throw new PortInUseException(port);
                }
                catch (SocketException ex)
                {
                    if (ex.SocketErrorCode != SocketError.ConnectionRefused)
                    {
                        //throw new exception
                        throw new PortInUseException(port);
                    }
                }
            }
        }

        public static void validatePort(int port)
        {
            // Check if the port is valid or not
            if (port < 1 || port > 65535) throw new InvalidPortException(port.ToString());

            // Check if the port is in use or not
            isPortInUse(port);
        }

        private static void isPortInUse(int port)
        {
            // Evaluate current system tcp connections. This is the same information provided
            // by the netstat command line application, just in .Net strongly-typed object
            // form.  We will look through the list, and if our port we would like to use
            // in our TcpClient is occupied, we will set inUse to true.

            // the following piece of code checks for ports which are in use but
            // is other than in LISTENING state
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

            foreach (TcpConnectionInformation tcpi in tcpConnInfoArray)
            {
                if (tcpi.LocalEndPoint.Port == port) throw new PortInUseException(port);
            }

            IPEndPoint[] tcpConnInfoArray1 = ipGlobalProperties.GetActiveTcpListeners();

            foreach (IPEndPoint endpoint in tcpConnInfoArray1)
            {
                if (endpoint.Port == port) throw new PortInUseException(port);
            }
        }

        public static void ValidateRepositoryConnection(string server, string database, bool isSQLAuth, string username,string password, bool is2FA, ref bool dbExists, string product)
        {
            RepositoryHelper.ValidateInput(server, database, isSQLAuth, username, password, product);
            RepositoryHelper.ValidateSQLInstance(server, isSQLAuth, username, password);
            RepositoryHelper.ValidateCurrentUserSQLPermissions(server, isSQLAuth, username, password);
            RepositoryHelper.checkIfRepositoryExists(server, database, isSQLAuth, username, password, ref dbExists);
        }

        public static void ValidateDiskSpaceForDashboard(string foldername)
        {
            ulong freeSpace = 0;
            GetAvailableDiskSpace(foldername, out freeSpace);
            double freeSpaceInMB = (freeSpace / 1024f) / 1024f;
            if (freeSpaceInMB < 500)
            {
                throw new DiskSpaceNotAvailableException();
            }


        }
  
            // Pinvoke for API function
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetDiskFreeSpaceEx(string lpDirectoryName,
        out ulong lpFreeBytesAvailable,
        out ulong lpTotalNumberOfBytes,
        out ulong lpTotalNumberOfFreeBytes);


        private static bool GetAvailableDiskSpace(string folderName, out ulong freespace)
        {
            freespace = 0;
            if (string.IsNullOrEmpty(folderName))
            {
                throw new ArgumentNullException("folderName");
            }

            if (!folderName.EndsWith("\\"))
            {
                folderName += '\\';
            }

            ulong free = 0, dummy1 = 0, dummy2 = 0;

            if (GetDiskFreeSpaceEx(folderName, out free, out dummy1, out dummy2))
            {
                freespace = free;
                return true;
            }
            else
            {
                return false;
            }
        }

        
        public static void ValidateIfDotNet40FullInstalled()
        {
            // Reference Link: http://thecodeventures.blogspot.in/2012/12/c-how-to-check-if-specific-version-of.html
            //To check if 4.0 Full exists or not
            object value = null;
            var item = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full").GetSubKeyNames();
            if( item == null) throw new Net40NotInstalledException();
            value = Registry.GetValue(
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full",
                    "Install",
                    null);
                    int installed = (int)value;
                    if(installed == 1) return;
            throw new Net40NotInstalledException();
        }

        public static void ValidateIfOperatingSystemCompatible()
        {
            if (!(Environment.OSVersion.Version.Major >= 6))
                throw new OSIncompatibleException();
        }

        public static void ValidateSQLServerInstance(string instanceName) 
        {
            if(string.IsNullOrEmpty(instanceName) || string.IsNullOrWhiteSpace(instanceName)) throw new EmptySQLServerInstanceException();
        }

        public static void ValidateDatabase(string databaseName, string product)
        {
            if (string.IsNullOrEmpty(databaseName) || string.IsNullOrWhiteSpace(databaseName)) throw new EmptyDatabaseException(product);
        }

        public static void CheckIfRemoteCredentialsAreNotEmpty(string username, string password, string hostname)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrWhiteSpace(username)) throw new EmptyUsernameException();
            if (string.IsNullOrEmpty(password) || string.IsNullOrWhiteSpace(password)) throw new EmptyPasswordException();
            if (string.IsNullOrEmpty(hostname) || string.IsNullOrWhiteSpace(hostname)) throw new EmptyHostnameException();
        }

        public static void checkIfPathIsValid(string path)
        {
            if (!Directory.Exists(path) || string.IsNullOrEmpty(path) || string.IsNullOrWhiteSpace(path)) throw new InvalidPathException();
        }

        public static void validateDashboardUrl(string url)
        {
            if (string.IsNullOrEmpty(url) || string.IsNullOrWhiteSpace(url)) throw new EmptyDashboardUrlException();
            Regex regex = new Regex(@"^http://.*:[0-9]*$");
            Match match = regex.Match(url);
            if (!match.Success)
            {
                throw new InvalidDashboardUrlException();
            }
        }

        public static void validateInstanceName(string instance)
        {
            if (string.IsNullOrEmpty(instance) || string.IsNullOrWhiteSpace(instance)) throw new EmptyInstanceException();
            Regex regex = new Regex(@"^[a-zA-Z0-9-]+$");
            Match match = regex.Match(instance);
            if (!match.Success)
            {
                throw new InvalidInstanceNameException();
            }
        }

        public static void validateSqlAuthCredentials(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrWhiteSpace(username)) throw new EmptySQLUserNameException();
            if (string.IsNullOrEmpty(password) || string.IsNullOrWhiteSpace(password)) throw new EmptySQLPasswordException();
        }

        public static void validateUpdateForDashboard(string version)
        {
            Version currentVersion = new Version(version);
            Version supportedVersion = new Version(Constants.leastSupportedDashboardVersion);
            Version newVersion = new Version(Constants.version);
            if (currentVersion < supportedVersion)
            {
                throw new CannotUpgradeDashboard();
            }
            if (currentVersion >= newVersion)
            {
                throw new NewerDashboardExists(Constants.version);
            }
        }

        public static void validatePath(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    return;
                }
                DirectoryInfo di = Directory.CreateDirectory(path);
                di.Delete();
            }
            catch (Exception e)
            {
                throw new InvalidPathException();
            }
        }
    }
}
