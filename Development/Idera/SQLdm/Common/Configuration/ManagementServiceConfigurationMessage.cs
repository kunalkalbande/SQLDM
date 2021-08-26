//------------------------------------------------------------------------------
// <copyright file="ManagementServiceConfigurationMessage.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Runtime.Serialization;
using Idera.SQLdm.Common.Security.Encryption;

namespace Idera.SQLdm.Common.Configuration
{
    /// <summary>
    /// Management service configuration message.  The password is plain text and
    /// is serialized encrypted.  
    /// </summary>
    [Serializable]
    public class ManagementServiceConfigurationMessage : ISerializable
    {
        public string instanceName;
        public int    servicePort;
        public string repositoryHost;
        public string repositoryDatabase;
        public bool windowsAuthentication;
        public string repositoryUsername;
        public string repositoryPassword;

        public ManagementServiceConfigurationMessage()
        {
        }

        public string InstanceName
        {
            get { return instanceName; }
            set { instanceName = value; }
        }

        public string RepositoryHost
        {
            get { return repositoryHost; }
            set { repositoryHost = value; }
        }

        public string RepositoryDatabase
        {
            get { return repositoryDatabase; }
            set { repositoryDatabase = value; }
        }

        public string RepositoryUsername
        {
            get { return repositoryUsername; }
            set { repositoryUsername = value; }
        }

        public string RepositoryPassword
        {
            get { return repositoryPassword; }
            set { repositoryPassword = value; }
        }

        public int ServicePort
        {
            get { return servicePort; }
            set { servicePort = value; }
        }

        public bool WindowsAuthentication
        {
            get { return windowsAuthentication; }
            set { windowsAuthentication = value; }
        } 

        public ManagementServiceConfigurationMessage(SerializationInfo info, StreamingContext context)
        {
            InstanceName = info.GetString("instanceName");
            ServicePort = info.GetInt32("servicePort");
            RepositoryHost = info.GetString("repositoryHost");
            RepositoryDatabase = info.GetString("repositoryDatabase");
            WindowsAuthentication = info.GetBoolean("windowsAuthentication");
            RepositoryUsername = info.GetString("repositoryUsername");
            string encryptedPassword = info.GetString("encryptedPassword");
            SetRepositoryPassword(encryptedPassword);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("instanceName", InstanceName);
            info.AddValue("servicePort",ServicePort);
            info.AddValue("repositoryHost",RepositoryHost);
            info.AddValue("repositoryDatabase",RepositoryDatabase);
            info.AddValue("windowsAuthentication",WindowsAuthentication);
            info.AddValue("repositoryUsername",RepositoryUsername);
            string encryptedPassword = GetEncryptedPassword();
            info.AddValue("encryptedPassword",encryptedPassword);
        }

        private string GetEncryptedPassword()
        {
            return Cipher.EncryptPassword(InstanceName, RepositoryPassword);
        }
                
        private void SetRepositoryPassword(string encryptedPassword)
        {
            RepositoryPassword = Cipher.DecryptPassword(InstanceName, encryptedPassword);
        }
    }
}
