using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CWFInstallerService
{
    public class CWFBaseException : Exception
    {
        public virtual string ErrorCode { get; set; }
        public virtual string ErrorMessage { get; set; }
        public CWFBaseException()
            : base()
        {
        }
        public CWFBaseException(string message, string code) : base(message)
        {
            this.ErrorCode = code;
            this.ErrorMessage = message;
        }
        public CWFBaseException(string message, Exception inner, string code)
            : base(message, inner)
        {
            this.ErrorCode = code;
            this.ErrorMessage = message;
        }

    }
    public class EmptyUserNameException : CWFBaseException
    {
        public override string ErrorCode
        {
            get
            {
                return "UserName_Empty";
            }
        }
        public override string ErrorMessage
        {
            get
            {
                return "User Name is blank. Please enter a user name.";
            }
        }
    }
    public class EmptyPasswordException : CWFBaseException
    {
        // Empty Password provided to CWF Installer Services
        public override string ErrorCode
        {
            get
            {
                return "Pswd_Empty";
            }
        }
        public override string ErrorMessage
        {
            get
            {
                return "Password is blank. Please enter a password.";
            }
        }
    }

    public class EmptyUsernameException : CWFBaseException
    {
        public override string ErrorCode
        {
            get
            {
                return "UserName_Empty";
            }
        }
        public override string ErrorMessage
        {
            get
            {
                return "User Name is blank. Please enter a user name.";
            }
        }
    }

    public class EmptyHostnameException : CWFBaseException
    {
        public override string ErrorCode
        {
            get
            {
                return "HostName_Empty";
            }
        }
        public override string ErrorMessage
        {
            get
            {
                return "The hostname is blank. Please enter a valid host name.";
            }
        }
    }

    public class InvalidPathException : CWFBaseException
    {
        public override string ErrorCode
        {
            get
            {
                return "INV_Path";
            }
        }
        public override string ErrorMessage
        {
            get
            {
                return "The directory path you entered is invalid. Please provide a valid directory path.";
            }
        }
    }

    public class InvalidFormatException : CWFBaseException
    {
        // Empty Password provided to CWF Installer Services
        public override string ErrorCode
        {
            get
            {
                return "INV_UserName";
            }
        }
        public override string ErrorMessage
        {
            get
            { 
                return "The User Name you entered is in the incorrect format. Please enter the User Name in the format: Domain\\UserAccount.";
            }
        }
    }
    public class InvalidCredentialsException : CWFBaseException
    {
        // Empty Password provided to CWF Installer Services
        public override string ErrorCode
        {
            get
            {
                return "INV_AcctCred";
            }
        }
        public override string ErrorMessage
        {
            get
            {
                return "The Idera Dashboard credentials you provided are invalid. Please enter valid credentials for the Idera Dashboard.";
            }
        }
    }
    public class InsufficientPermissionsException : CWFBaseException
    {
        // Empty Password provided to CWF Installer Services
        public override string ErrorCode
        {
            get
            {
                return "INS_PERMISSION_CWFACCT";
            }
        }
        public override string ErrorMessage
        {
            get
            {
                return "Provided CWF account has insufficient permission";
            }
        }
    }
    public class FailedToVerifyCredentialsException : CWFBaseException
    {
        public override string ErrorCode
        {
            get
            {
                return "CWFCred_FailedAuth";
            }
        }
        public override string ErrorMessage
        {
            get
            {
                return "The credentials you provided failed.  Please enter valid credentials for the Idera Dashboard.";
            }
        }
    }
    public class PortInUseException : CWFBaseException
    {
        int portInUse;
        public PortInUseException(int port)
        {
            portInUse = port;
        }
        public override string ErrorCode
        {
            get
            {
                return "Port_InUse";
            }
        }
        public override string ErrorMessage
        {
            get
            {
                return "The port number " + portInUse.ToString() + " you entered is already in use.  Please enter a different port.";
            }
        }
    }
    public class InvalidPortException : CWFBaseException
    {
        string invalidPort;
        public InvalidPortException(string port)
        {
            invalidPort = port;
        }
        public override string ErrorCode
        {
            get
            {
                return "Inv_Port";
            }
        }
        public override string ErrorMessage
        {
            get
            {
                return "The port number " + invalidPort + " you entered is invalid.  Please enter a different port.";
            }
        }
    }

    public class PortsNotUniqueException : CWFBaseException
    {
        string notUniquePort;
        List<int> portList;
        public PortsNotUniqueException(List<int> ports)
        {
            portList = ports;
            for (int i = 0; i < ports.Count; i++)
            {
                if (i > 0)
                {
                    notUniquePort += ", ";
                }
                notUniquePort += ports[i].ToString();
            }
        }
        public override string ErrorCode
        {
            get
            {
                return "Port_NotUnique";
            }
        }
        public override string ErrorMessage
        {
            get
            {
                if(portList.Count > 1)
                    return "The port numbers " + notUniquePort + " you entered are not unique.  Please enter different ports.";
                else
                    return "The port number " + notUniquePort + " you entered is not unique.  Please enter a different port.";
            }
        }
    }
    public class EmptySQLServerInstanceException : CWFBaseException
    {
        public override string ErrorCode
        {
            get
            {
                return "SQLInstance_Empty";
            }
        }
        public override string ErrorMessage
        {
            get
            {
                return "The SQL Instance Name is blank.  Please enter SQL Instance Name.";
            }
        }
    }
    public class EmptyDatabaseException : CWFBaseException
    {
        string productName = string.Empty;
        public EmptyDatabaseException(string product)
        {
            productName = product;
        }
        public override string ErrorCode
        {
            get
            {
                return "DB_Empty";
            }
        }
        public override string ErrorMessage
        {
            get
            {
                return "The Database Name is blank.  Please enter a database name for " + productName +" installation.";
            }
        }
    }
    public class InvalidDatabaseException : CWFBaseException
    {
        string productName;
        public InvalidDatabaseException(string product)
        {
            productName = product;
        }
        public override string ErrorCode
        {
            get
            {
                return "INV_DB";
            }
        }
        public override string ErrorMessage
        {
            get
            {
                return "The database name you entered is invalid. Please enter a valid database name for " + productName +" installation..";
            }
        }
    }
    public class EmptySQLUserNameException : CWFBaseException
    {
        public override string ErrorCode
        {
            get
            {
                return "SQLUserName_Empty";
            }
        }
        public override string ErrorMessage
        {
            get
            {
                return "The SQL user name you provided is blank. Please enter a SQL user name.";
            }
        }
    }
    public class EmptySQLPasswordException : CWFBaseException
    {
        public override string ErrorCode
        {
            get
            {
                return "SQLPswd_Empty";
            }
        }
        public override string ErrorMessage
        {
            get
            {
                return "The SQL password you provided is blank.  Please enter a SQL password.";
            }
        }
    }
    public class FailedToVerifySQLServerException : CWFBaseException
    {
        public override string ErrorCode
        {
            get
            {
                return "SQL_AuthFailed";
            }
        }
        public override string ErrorMessage
        {
            get
            {
                return "The SQL server verification failed.  Please enter valid credentials.";
            }
        }
    }
    public class UnSupportedSQLServerException : CWFBaseException
    {
        string version = string.Empty;
        public UnSupportedSQLServerException(string SQLVersion)
        {
            version = SQLVersion;
        }
        public override string ErrorCode
        {
            get
            {
                return "Unsupported_SQLVersion";
            }
        }
        public override string ErrorMessage
        {
            get
            {
                return "We do not support this version of SQL Server " + version;
            }
        }
    }
    public class FailedToVerifyUserPermissionsSQLServerException : CWFBaseException
    {
        public override string ErrorCode
        {
            get
            {
                return "Inv_SQLCredentials";
            }
        }
        public override string ErrorMessage
        {
            get
            {
                return "We are not able to verify the SQL User Credentials you provided.  Please enter valid SQL  User Credentials.";
            }
        }
    }
    public class FailedToVerifyDatabaseException : CWFBaseException
    {
        public override string ErrorCode
        {
            get
            {
                return "FailedVerf_DB";
            }
        }
        public override string ErrorMessage
        {
            get
            {
                return "We are not able to connect to the SQL Database.  Please check to see if the SQL Server is up.";
            }
        }
    }
    public class FailedToVerifyServiceAccountLoginException : CWFBaseException
    {
        public override string ErrorCode
        {
            get
            {
                return "";
            }
        }
        public override string ErrorMessage
        {
            get
            {
                return "";
            }
        }
    }
    public class FailedToCreateServiceAccountLoginException : CWFBaseException
    {
        public override string ErrorCode
        {
            get
            {
                return "";
            }
        }
        public override string ErrorMessage
        {
            get
            {
                return "";
            }
        }
    }
    public class FailedToAssignUserToDatabaseException : CWFBaseException
    {
        public override string ErrorCode
        {
            get
            {
                return "";
            }
        }
        public override string ErrorMessage
        {
            get
            {
                return "";
            }
        }
    }
    public class DiskSpaceNotAvailableException : CWFBaseException
    {
        public override string ErrorCode
        {
            get
            {
                return "DiskSpace_Unavail";
            }
        }
        public override string ErrorMessage
        {
            get
            {
                return "There is not enough disk space to install the product.  Please free up disk space or try to install in a different location.";
            }
        }
    }
    public class OSIncompatibleException : CWFBaseException
    {
        public override string ErrorCode
        {
            get
            {
                return "Invalid_OSVersion";
            }
        }
        public override string ErrorMessage
        {
            get
            {
                return "The current Operating System is not supported by this product.";
            }
        }
    }
    public class Net40NotInstalledException : CWFBaseException
    {
        public override string ErrorCode
        {
            get
            {
                return "Missing_Net4";
            }
        }
        public override string ErrorMessage
        {
            get
            {
                return ".NET Framework 4.0 must be installed prior to installation of this product.";
            }
        }
    }

    public class EmptyDashboardUrlException : CWFBaseException
    {
        public override string ErrorCode
        {
            get
            {
                return "INV_CWFURL";
            }
        }
        public override string ErrorMessage
        {
            get
            {
                return "The Idera Dashboard URL you provided is empty.  Please provide a valid Idera Dashboard URL";
            }
        }
    }

    public class InvalidDashboardUrlException : CWFBaseException
    {
        public override string ErrorCode
        {
            get
            {
                return "INV_CWFURL";
            }
        }
        public override string ErrorMessage
        {
            get
            {
                return "The Idera Dashboard URL you provided is invalid or not in correct format.  Please provide a valid Idera Dashboard URL.";
            }
        }
    }

    public class EmptyInstanceException : CWFBaseException
    {
        public override string ErrorCode
        {
            get
            {
                return "Instance_Empty";
            }
        }
        public override string ErrorMessage
        {
            get
            {
                return "The Instance Name is blank.  Please enter an Instance Name.";
            }
        }
    }

    public class InvalidInstanceNameException : CWFBaseException
    {
        public override string ErrorCode
        {
            get
            {
                return "INV_InstanceName";
            }
        }
        public override string ErrorMessage
        {
            get
            {
                return "The Instance Name you provided is invalid.  Please enter a valid Instance Name.";
            }
        }
    }

    public class VersionMismatchExcception : CWFBaseException
    {
        public override string ErrorCode
        {
            get
            {
                return "Version_MisMatch";
            }
        }
        public override string ErrorMessage
        {
            get
            {
                return "The Sample Product is already installed on the same Idera Dashboard.";
            }
        }
    }

    public class CannotUpgradeDashboard : CWFBaseException
    {
        public override string ErrorCode
        {
            get
            {
                return "Cannot_Upgrade";
            }
        }
        public override string ErrorMessage
        {
            get
            {
                return "The version of Idera Dashboard installed cannot support upgrade.  Please contact Idera Support.";
            }
        }
    }

    

    public class NewerDashboardExists : CWFBaseException
    {
        string version = string.Empty;
        public NewerDashboardExists(string productVersion)
        {
            version = productVersion;
        }
        public override string ErrorCode
        {
            get
            {
                return "NewerDashboardVersionExist";
            }
        }
        public override string ErrorMessage
        {
            get
            {
                return "Looks like you have a newer version of the Idera Dashboard installed and we can not install " + version + " over this newer version.";
            }
        }
    }

    
}
