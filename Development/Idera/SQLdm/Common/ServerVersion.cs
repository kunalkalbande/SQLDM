//------------------------------------------------------------------------------
// <copyright file="ServerVersion.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Represents the version number of a SQL Server.
    /// </summary>
    [Serializable]
    public class ServerVersion
    {
        #region fields

        private int build;
        private int major;
        private int minor;
        private int revision;
        private string servicePack;
        private string version;
        private int masterDatabaseCompatibility;

        #endregion

        #region constructors

        public ServerVersion(string version)
        {
            if (version.ToString() == "?")
                return;

            if (version == null)
                throw new ArgumentNullException("version");

            this.version = version;

            string[] splitVersion = version.Split('.');

            if (splitVersion.Length < 3)
                throw new ArgumentException("version string in unrecognized format");

            try
            {
                Major = Convert.ToInt32(splitVersion[0]);
                Minor = Convert.ToInt32(splitVersion[1]);
                Build = Convert.ToInt32(splitVersion[2]);

                if (splitVersion.Length == 4)
                    Revision = Convert.ToInt32(splitVersion[3]);

                if (Major < 0)
                    throw new ArgumentOutOfRangeException("major");
                if (Minor < 0)
                    throw new ArgumentOutOfRangeException("minor");
                if (Build < 0)
                    throw new ArgumentOutOfRangeException("build");
                if (Revision < 0)
                    throw new ArgumentOutOfRangeException("revision");
            }
            catch (InvalidCastException exception)
            {
                throw new FormatException("Error parsing server version string", exception);
            }

            ServicePack = GetServicePack(major, minor, build, revision);
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the SQL Server build number.
        /// </summary>
        public int Build
        {
            get { return build; }
            private set { build = value; }
        }

        /// <summary>
        /// Gets the SQL Server major version number.
        /// </summary>
        public int Major
        {
            get { return major; }
            private set { major = value; }
        }

        /// <summary>
        /// Gets the SQL Server minor version number.
        /// </summary>
        public int Minor
        {
            get { return minor; }
            private set { minor = value; }
        }

        /// <summary>
        /// Gets the SQL Server revision number.
        /// </summary>
        public int Revision
        {
            get { return revision; }
            private set { revision = value; }
        }

        /// <summary>
        /// Gets the descriptive SQL Server service pack name.
        /// </summary>
        public string ServicePack
        {
            get { return servicePack; }
            private set { servicePack = value; }
        }

        /// <summary>
        /// Gets the string representation of the version number.
        /// </summary>
        public string Version
        {
            get { return version; }
            private set { version = value; }
        }

        public string DisplayVersion
        {
            get
            {
                StringBuilder buffer = new StringBuilder();

                buffer.AppendFormat("{0:D2}.{1:D2}.{2:D4}", Major, Minor, Build);
                if (Revision > 0)
                    buffer.AppendFormat(".{0:2}", Revision);

                return buffer.ToString();
            }
        }


        public int MasterDatabaseCompatibility
        {
            get { return masterDatabaseCompatibility; }
            internal set { masterDatabaseCompatibility = value; }
        }

        #endregion

        #region events

        #endregion

        #region methods

        /// <summary>
        /// Returns a string representation of the server version.
        /// </summary>
        /// <returns>The server version string.</returns>
        public override string ToString()
        {
            string ret;

            switch (Major)
            {
                case 7:
                    {
                        ret = "SQL Server 7";
                        break;
                    }
                case 8:
                    {
                        ret = "SQL Server 2000";
                        break;
                    }
                case 9:
                    {
                        ret = "SQL Server 2005";
                        break;
                    }
                // Add SQL Server 2008
                case 10:
                    {
                        ret = "SQL Server 2008";
                        if (Minor == 50)
                            ret += " R2";
                        break;
                    }
                case 11:
                    {
                        //9120 stands for a very early release of SQLServer 2014
                        //whose major version is still 11
                        if (Build >= 9120)
                        {
                            ret = "SQL Server 2014";    
                        }
                        else
                        {
                            ret = "SQL Server 2012";
                        }
                        
                        break;
                    }
                case 12:
                    {
                        ret = "SQL Server 2014";
                        break;
                    }

                case 13:// SQLdm 10.1 (Pulkit Puri) 
                    {
                        // SQLDM - 25388-- [10.1] When you right click on all servers in SQL DM and go to all server properties , SQL version of 2016 instance's is not updated.
                        ret = "SQL Server 2016";
                        break;
                    }
                // SQLdm 10.3 (Varun Chopra) SQL Server 2017 Support
                case 14:// SQLdm 10.3 (Varun Chopra) 
                {
                    ret = "SQL Server 2017";
                    break;
                }
                // SQLdm 10.5 (Sonali Dogra) SQL Server 2019 Support
                case 15:// SQLdm 10.5 (Sonali Dogra)
                {
                        ret = "SQL Server 2019";
                        break;
                }
                default:
                    {
                        return "SQL Server " + Major + "." + Minor;
                    }
            }

            if (ServicePack != String.Empty)
                ret = ret + " " + ServicePack;

            return ret;
        }

        /// <summary>
        /// Returns whether the SQL Server version is supported by SQLdm.
        /// </summary>
        /// <returns>Whether the version is supported.</returns>
        public bool IsSupported
        {
            get
            {
                if (Major == 8 && Build >= 194)
                    return true;

                if (Major == 9 && Build >= 1399)
                    return true;

                // Add SQL Server 2008
                if (Major == 10)
                    return true;

                // Add 2012
                if (Major >= 11)
                    return true;

                return false;
            }
        }

        /// <summary>
        /// Gets the service pack.
        /// </summary>
        /// <param name="major">The major.</param>
        /// <param name="minor">The minor.</param>
        /// <param name="build">The build.</param>
        /// <param name="revision">The revision.</param>
        /// <returns></returns>
        private static string GetServicePack(int major, int minor, int build, int revision)
        {
            Dictionary<int, string> releases = new Dictionary<int, string>();
            switch (major)
            {
                case 7:// SQL Server 7.0
                    {
                        if (minor == 0)
                        {
                            releases[623] = "";
                            releases[699] = "SP1";
                            releases[842] = "SP2";
                            releases[961] = "SP3";
                            releases[1063] = "SP4";
                        }
                        break;
                    }
                case 8:// SQL Server 2000
                    {
                        releases[194] = "";
                        releases[384] = "SP1";
                        releases[534] = "SP2";
                        releases[760] = "SP3";
                        releases[2039] = "SP4";

                        break;
                    }
                case 9:// SQL Server 2005
                    {
                        releases[1314] = "CTP";
                        releases[1399] = "";
                        releases[2047] = "SP1";
                        releases[3042] = "SP2";
                        releases[4035] = "SP3";
                        releases[5000] = "SP4";

                        break;
                    }
                case 10:// SQL Server 2008
                    {
                        switch (minor)
                        {
                            case 0:
                                releases[1600] = "";
                                releases[2531] = "SP1";
                                releases[4000] = "SP2";
                                releases[5500] = "SP3";
                                releases[6000] = "SP4";// DM Kit 1 Defect id - DE44596 (Biresh Kumar Mishra)
                                break;
                            case 50:
                                releases[1092] = "CTP";
                                releases[1600] = "";
                                releases[2500] = "SP1";
                                releases[4000] = "SP2";
                                releases[6000] = "SP3";// DM Kit 1 Defect id - DE44596 (Biresh Kumar Mishra)
                                break;
                        }
                        break;
                    }
                case 11:// SQL Server 2012
                    {
                        //9120 stands for a very early release of SQLServer 2014 
                        //whose major version is still 11
                        if (build >= 9120)
                        {
                            releases[9120] = "CTP1";
                            releases[9149] = "CTP2";
                        }
                        else
                        {
                            releases[1103] = "CTP1";
                            releases[1440] = "CTP3";
                            releases[1750] = "RC0";
                            releases[1913] = "RC1";
                            releases[2100] = "";    
                            releases[3000] = "SP1";
                            releases[5058] = "SP2";
                            releases[6020] = "SP3";// DM Kit 1 Defect id - DE44596 (Biresh Kumar Mishra)
                        }
                        break;
                    }
                case 12:// SQL Server 2014
                    {
                        releases[2000] = "RTM"; // DM Kit 1 Defect id - DE44596 (Biresh Kumar Mishra)
                        releases[2342] = "RTM CU1"; // DM Kit 1 Defect id - DE44596 (Biresh Kumar Mishra)
                        releases[2370] = "RTM CU2";
                        releases[2402] = "RTM CU3";
                        releases[2430] = "RTM CU4";
                        releases[2456] = "RTM CU5";
                        releases[2480] = "RTM CU6";
                        releases[2495] = "RTM CU7";
                        releases[2546] = "RTM CU8";
                        releases[2553] = "RTM CU9";
                        releases[2556] = "RTM CU10";
                        releases[2560] = "RTM CU11";
                        releases[2564] = "RTM CU12";
                        releases[2568] = "RTM CU13";
                        releases[2569] = "RTM CU14";
                        releases[4100] = "SP1";
                        releases[4416] = "SP1 CU1";
                        releases[4422] = "SP1 CU2";
                        releases[4427] = "SP1 CU3";
                        releases[4436] = "SP1 CU4";
                        releases[4438] = "SP1 CU5";
                        releases[4449] = "SP1 CU6";
                        releases[4459] = "SP1 CU7";
                        releases[4468] = "SP1 CU8";
                        releases[4474] = "SP1 CU9";
                        releases[5000] = "SP2";
                        releases[5511] = "SP2 CU1";
                        releases[5522] = "SP2 CU2";
						releases[5538] = "SP2 CU3";
                        releases[5540] = "SP2 CU4";
                        releases[5546] = "SP2 CU5";
                        releases[5552] = "SP2 CU6";
                        releases[5556] = "SP2 CU7";
                        releases[5557] = "SP2 CU8";
                        releases[5563] = "SP2 CU9";
                        releases[5571] = "SP2 CU10";
                        releases[5579] = "SP2 CU11";
						releases[5589] = "SP2 CU12";
                        releases[5590] = "SP2 CU13";
                        releases[5600] = "SP2 CU14";
                        releases[5605] = "SP2 CU15";
                        releases[5626] = "SP2 CU16";
                        releases[5632] = "SP2 CU17";
                        releases[5687] = "SP2 CU18";
                        
                        break;   
                    }
                case 13:// SQL Server 2016
                    {
                        releases[1601] = "RTM";
                        releases[2149] = "RTM CU1";
                        releases[2164] = "RTM CU2";
                        releases[2186] = "RTM CU3";
                        releases[4001] = "SP1";
                        break;
                    }
                // SQLdm 10.3 (Varun Chopra) SQL Server 2017 Support
                case 14:// SQL Server 2017
                {
                    releases[1] = "CTP";
                    releases[800] = "RC1";
                    releases[900] = "RC2";
                    break;
                }
				// SQLdm 10.5 (Sonali Dogra) SQL Server 2019 Support

                case 15:// SQL Server 2019
                {
                    releases[1000] = "CTP";
                    releases[1900] = "RC1";
                    break;
                }
                default:// SQL Server Unknown
                    {
                        break;
                    }
            }

            return GetSpString(build, releases);
            // If we've made it here, return the build.revision number
            // return revision == 0 ? String.Empty : revision.ToString();
        }

        private static string GetSpString(int target, Dictionary<int, string> releases)
        {
            if (releases.Count == 0)
                return "";

            List<int> builds = new List<int>(releases.Keys);
            builds.Sort();

            string ret = null;

            for (int i = 0; i < builds.Count; i++)
            {
                int build = builds[i];

                if (target < build)
                {
                    if (i > 0)
                    {
                        //If we've got a previous known release
                        ret = releases[builds[i - 1]] + "+";
                        break;
                    }
                    else
                    {
                        //Target build number is lower than anything we know about
                        ret = "";
                        break;
                    }
                }

                if (target == build)
                {
                    //An exact match for the target.  easy
                    ret = releases[build];
                    break;
                }
            }

            if (target > builds[builds.Count - 1])
            {
                //Is the target greater than the last build we know about?
                ret = releases[builds[builds.Count - 1]] + "+";
            }

            if (ret == "+")
            {
                //Special RTM empty string hot-fix case";
                ret = "RTM+";
            }

            return ret;
        }

        #endregion
    }
}
