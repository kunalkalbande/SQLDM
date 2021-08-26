//------------------------------------------------------------------------------
// <copyright file="LicenseManager.cs" company="BBS Technologies, Inc.">
//     Copyright (c) BBS Technologies, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.DesktopClient.Helpers
{
	using System;
	using Microsoft.Win32;
    using Idera.SQLdm.DesktopClient.Properties;

	/// <summary>
	/// Provides utility methods for managing the Idera product license.
	/// </summary>
	public sealed class LegacyLicenseManager
	{
        #region constants
        private const string _regHive = "IDEBT526";
        private const string _regKeyName = "360";
        private const string _regValName = "bdata";
        #endregion constants

		#region fields

		/// <summary>
		/// The lock object for checking licenses
		/// </summary>
		private static object licenseLock = new object();

		#endregion

		#region methods

		private static string DecryptLegacyLicenseKey(byte[] encryptedString)
		{
			if (encryptedString == null || encryptedString.Length == 0)
			{
				return null;
			}

			int errorCode;
			string decryptedString = NativeMethods.DecryptString(encryptedString, "BBS Technologies, Inc. - Data Management & Security Tools", encryptedString.Length, out errorCode);

			if (errorCode == 0)
			{
				return decryptedString;
			}
			else
			{
				return null;
			}
		}

		private static RegistryKey OpenIderaLicenseRegistryKey(bool writable)
		{
			RegistryKey microsoftRegistryKey = null;
			RegistryKey ideraRegistryKey = null;

			try
			{
				microsoftRegistryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft", writable);
				ideraRegistryKey = microsoftRegistryKey.OpenSubKey(_regHive, writable);

				if (ideraRegistryKey == null && writable)
				{
					ideraRegistryKey = microsoftRegistryKey.CreateSubKey(_regHive);
				}
			}
			finally
			{
				if (microsoftRegistryKey != null)
				{
					microsoftRegistryKey.Close();	
				}
			}

			return ideraRegistryKey;
		}

        public static LegacyLicense GetLegacyLicense()
		{
			LegacyLicense legacyLicense = null;
			RegistryKey sqldmRegistryKey = null;
			
			try
			{
				sqldmRegistryKey = OpenLegacySQLdmLicenseRegistryKey();

				if (sqldmRegistryKey != null)
				{
					lock (licenseLock)
					{
						byte[] encryptedBytes = (byte[])sqldmRegistryKey.GetValue(_regValName);

						if (encryptedBytes != null && encryptedBytes.Length > 0)
						{
							legacyLicense = new LegacyLicense(DecryptLegacyLicenseKey(encryptedBytes));
						}
					}
				}
			}
			catch {}
			finally
			{
				if (sqldmRegistryKey != null)
				{
					sqldmRegistryKey.Close();
				}
			}

			return legacyLicense;
		}

		private static RegistryKey OpenLegacySQLdmLicenseRegistryKey()
		{
			RegistryKey ideraRegistryKey = null;
			RegistryKey sqldmRegistryKey = null;

			try
			{
				ideraRegistryKey = OpenIderaLicenseRegistryKey(false);
                sqldmRegistryKey = ideraRegistryKey.OpenSubKey("501");
			}
			finally
			{
				if (ideraRegistryKey != null)
				{
					ideraRegistryKey.Close();
				}
			}

			return sqldmRegistryKey;
		}

		public static LegacyLicense GetLicense()
		{
			return GetLicense(false);
		}

        public static LegacyLicense GetLicense(bool generateTrial)
		{
			RegistryKey sqldmRegistryKey = null;
			
			try
			{
				sqldmRegistryKey = OpenSQLdmWebConsoleLicenseRegistryKey(generateTrial);

				if (sqldmRegistryKey != null)
				{
					lock (licenseLock)
					{
						string licenseString = (string)sqldmRegistryKey.GetValue(_regValName);

						if (licenseString != null)
						{
							return new LegacyLicense((string)sqldmRegistryKey.GetValue(_regValName));
						}
						else if (generateTrial)
						{
							string trialLicenseString = LegacyLicense.GenerateTrialLicenseString(Environment.MachineName);
							sqldmRegistryKey.SetValue(_regValName, trialLicenseString);
							sqldmRegistryKey.Flush();
							return new LegacyLicense(trialLicenseString);
						}
						else
						{
							return null;
						}
					}
				}
				else
				{
					return null;
				}
			}
			catch (Exception ex)
			{
				throw new ApplicationException(Resources.ExceptionUnableToGetLicense, ex);
			}
			finally
			{
				if (sqldmRegistryKey != null)
				{
					sqldmRegistryKey.Close();
				}
			}
		}

		private static RegistryKey OpenSQLdmWebConsoleLicenseRegistryKey(bool writable)
		{
			RegistryKey ideraRegistryKey = null;
			RegistryKey sqldmRegistryKey = null;

			try
			{
				ideraRegistryKey = OpenIderaLicenseRegistryKey(writable);
				sqldmRegistryKey = ideraRegistryKey.OpenSubKey(_regKeyName, writable);

				if (sqldmRegistryKey == null && writable)
				{
					sqldmRegistryKey = ideraRegistryKey.CreateSubKey(_regKeyName);
				}
			}
			finally
			{
				if (ideraRegistryKey != null)
				{
					ideraRegistryKey.Close();
				}
			}

			return sqldmRegistryKey;
		}

        public static LegacyLicense SetLicense(string licenseString)
		{
			LegacyLicense license = new LegacyLicense(licenseString);

			if (license.IsValid())
			{
				RegistryKey sqldmRegistryKey = null;
			
				try
				{
					sqldmRegistryKey = OpenSQLdmWebConsoleLicenseRegistryKey(true);

					if (sqldmRegistryKey != null)
					{
						lock (licenseLock)
						{
							sqldmRegistryKey.SetValue(_regValName, license.ToString());
							sqldmRegistryKey.Flush();
							return license;
						}
					}
					else
					{
                        throw new ApplicationException(Resources.ExceptionUnableToSetLicense);
					}
				}
				catch (Exception ex)
				{
                    throw new ApplicationException(Resources.ExceptionUnableToSetLicense, ex);
				}
				finally
				{
					if (sqldmRegistryKey != null)
					{
						sqldmRegistryKey.Close();
					}
				}
			}
			else
			{
                throw new ApplicationException(Resources.ExceptionInvalidLicense);
			}
		}

		#endregion
	}
}
