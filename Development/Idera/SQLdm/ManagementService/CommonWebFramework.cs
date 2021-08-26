using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Security.Principal;
using System.Xml;
using BBS.TracerX;
using Idera.SQLdm.Common.Attributes;
using Idera.SQLdm.Common.Auditing;
using Microsoft.ApplicationBlocks.Data;
using Idera.SQLdm.ManagementService.Helpers;
using Idera.SQLdm.Common.Security.Encryption;

namespace Idera.SQLdm.ManagementService
{
    // SQLdm 9.0 (Abhishek Joshi) -CWF Integration --singleton class which contains the web frameworks registration information
    public class CommonWebFramework
    {

        private static CommonWebFramework instance = null;
        private static readonly Logger Log = Logger.GetLogger("CommonWebFramework");
        private static object locker = new object();

        #region fields

        private string _host;
        private string _port;
        private string _username;
        private string _password;
        private int _productId = 0;    // invalid productId
        private bool _isSaved = false; //tells us whether the CWF information is saved in the database yet or not
        private string _instanceName;
        private string _webURI;
        private string _restURI;
        private const string _webUIPort = "9291";
        #endregion

        #region constructors

        private CommonWebFramework()
        {

        }

        private CommonWebFramework(string cwfHost, string cwfPort, string cwfUser, string cwfPassword, string cwfProductInstance)
        {
            _host = cwfHost;
            _port = cwfPort;
            _username = cwfUser;
            _password = cwfPassword;
            _instanceName = cwfProductInstance;
            _isSaved = false;//since the new information has just arrived, the information is not yet saved
        }

        #endregion

        #region methods

        private static void Instantiate(ref CommonWebFramework instance)
        {
            Dictionary<string, string> frameworkDetails = RepositoryHelper.GetTheRegistrationInformation();

            if (frameworkDetails != null && frameworkDetails.Count > 0)
            {
                instance._isSaved = true;//cwf data is there in the repo
                instance._host = frameworkDetails["HostName"];
                instance._port = frameworkDetails["Port"];
                instance._username = frameworkDetails["UserName"];
                instance._password = frameworkDetails["Password"];
                if (frameworkDetails["ProductID"] != null && frameworkDetails["ProductID"] != string.Empty)
                {
                    int productId;
                    if (Int32.TryParse(frameworkDetails["ProductID"], out productId))
                    {
                        instance._productId = productId;
                    }
                    //= Convert.ToInt32(frameworkDetails["ProductID"]);
                }
                instance._instanceName = frameworkDetails["InstanceName"];
                
            }
            else
            {
                instance._isSaved = false;//cwf data is not there in the repo
            }
        }

        /// <summary>
        /// To get the instance of CWF from the database
        /// </summary>
        /// <returns></returns>
        public static CommonWebFramework GetInstance()
        {
            if(System.Threading.Monitor.TryEnter(locker,500))//handling multi-threaded calls
            {
                if (instance == null) instance = new CommonWebFramework(); 
            }
            
            Instantiate(ref instance);
            return instance;
        }

        /// <summary>
        /// To get the in-memory instance of this class without saving data to the repo
        /// </summary>
        /// <param name="cwfHost"></param>
        /// <param name="cwfPort"></param>
        /// <param name="cwfUser"></param>
        /// <param name="cwfPassword"></param>
        /// <returns></returns>
        public static CommonWebFramework GetInstance(string cwfHost, string cwfPort, string cwfUser, string cwfPassword, string cwfProductInstance)
        {
            if (System.Threading.Monitor.TryEnter(locker, 500)) //handling multi-threaded calls
            {
                if (instance == null) instance = new CommonWebFramework();
            }
            
            instance = new CommonWebFramework(cwfHost, cwfPort, cwfUser, cwfPassword, cwfProductInstance);
            return instance;
        }

        public void LoadCriticalInfo(string cwfHost, string cwfPort, string cwfUser, string cwfPassword, string cwfProductInstance)
        {
            this._host = cwfHost;
            this._port = cwfPort;
            this._username = cwfUser;
            this._password = cwfPassword;
            this._instanceName = cwfProductInstance;
            this._isSaved = false;//since the new information has just arrived, the information is not yet saved
            Log.Info("Loading critical info in memory:" + this.ToString());
        }

        /// <summary>
        ///SQLdm 9.0 (Gaurav Karwal: for saving the non persistent information into the memory
        /// </summary>
        /// <param name="registeredProductID">The product id which we receive after registration with the CWF</param>
        public void LoadNonPersistentInfo(int productID, string webURI, string RESTURI)
        {
            Log.Info("loading web uri as " + webURI + " and rest uri as " + RESTURI);
            if (productID == this._productId) //i know this will always be true but still, to be on a safer side
            {
                _webURI = webURI;
                _restURI = RESTURI;
            }
        }

        /// <summary>
        ///SQLdm 9.0 (Gaurav Karwal: for saving the information into the repo
        /// </summary>
        /// <param name="registeredProductID">The product id which we receive after registration with the CWF</param>
        public void Save(int registeredProductID, string webURI, string restURI,string registeredInstanceName)
        {
            try
            {
                RepositoryHelper.UpdateTheRegistrationInformation(this._host, this._port, this._username, this._password, registeredProductID, registeredInstanceName);
                _productId = registeredProductID;
                _webURI = webURI;
                _restURI = restURI;
                _isSaved = true;

            }
            catch (Exception ex)
            {
                Log.Error(ex.Message + ex.InnerException != null ? ex.InnerException.Message : "");
                //Management.LogWriteEventError(ex, Idera.SQLdm.Common.Messages.Status.ErrorRepositoryError, new string[] { });
                //_isSaved = false;
                //_productId = 0;
                //_webURI = string.Empty;
                //_restURI = string.Empty;
            }

        }

        /// <summary>
        /// SQLdm 9.0 (Gaurav Karwal): Refreshing the object from database
        /// </summary>
        public void Refresh()
        {
            if (_isSaved == true)
            {
                CommonWebFramework instance = this;
                CommonWebFramework.Instantiate(ref instance);
            }
            else
            {
                throw new Exception("The CWF information is not saved in the database");
            }
        }

        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("======START=====");
            sb.Length = 0;
            sb.AppendLine("BaseURL:" + this._host + ":" + this._port);
            sb.AppendLine("InstanceName:" + this._instanceName);
            sb.AppendLine("IsSaved:" + this._isSaved);
            //sb.AppendLine("password:" + );
            sb.AppendLine("REST URI:" + this._restURI);
            sb.AppendLine("Web URI:" + this._webURI);
            sb.AppendLine("username:" + this._username);
            sb.AppendLine("Product ID:" + this._productId.ToString());
            sb.AppendLine("======END=====");
            return sb.ToString();
        }
        #endregion

        #region properties

        public string WebUIURL
        {
            get
            {
                if (!string.IsNullOrEmpty(_host))
                {
                    return "https://" + _host + ":" + _webUIPort;
                }
                return string.Empty;
            }
        }
        public string RegisteredInstanceName
        {
            get { return _instanceName; }
        }

        public string Host
        {
            get { return _host; }

        }

        public string Port
        {
            get { return _port; }
        }

        public string UserName
        {
            get { return _username; }
        }

        public string Password
        {
            get { return _password; }
        }

        public int ProductID
        {
            get { return _productId; }
        }

        public bool IsSaved
        {
            get { return _isSaved; }
        }

        /// <summary>
        /// returns a decrypted password if its encrypted, else the password
        /// </summary>
        public string DecryptedPassword
        {
            get
            {
                string _retValue_ = _password;
                try
                {
                    _retValue_ = EncryptionHelper.QuickDecrypt(_password);

                }
                catch
                {

                }

                return _retValue_;
            }
        }

        public string BaseURL
        {
            get { return "http://" + _host + ":" + _port; }
        }

        public string RestURI
        {
            get { return _restURI; }
        }

        public string WebURI
        {
            get { return _webURI; }
        }
        #endregion

    }

}

