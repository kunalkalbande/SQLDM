//------------------------------------------------------------------------------
// <copyright file="License.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.DesktopClient.Helpers
{
	using System;
	using System.Text;
    using Idera.SQLdm.DesktopClient.Properties;

	/// <summary>
	/// Represents an Idera product license.
	/// </summary>
	public sealed class LegacyLicense
	{
		#region constants

		private const int TrialLicenseExpirationLimitInDays = 14;
		private const int TrialLicenseDefaultLicensedInstances = 15;
        private const string _productName = "SQLdm";

		#endregion

		#region fields

		private string	key;
        private LegacyProduct product;
		private string	productString;
		private string	server;

		#endregion

		#region constructors

        internal LegacyLicense(string licenseString) 
		{
			try
			{
				ParseLicense(licenseString);
			}
			catch (Exception ex)
			{
                throw new ApplicationException(Resources.ExceptionInvalidLicenseString, ex);
			}
		}

		#endregion

		#region properties

		public DateTime ExpirationDate
		{
			get { return product != null ? product.ExpirationDate : DateTime.MinValue; }
		}

		/// <summary>
		/// License key (generated hash)
		/// </summary>
		internal string Key 
		{
			get { return key; }
		}

		/// <summary>
		/// Product license applies to
		/// </summary>
        internal LegacyProduct Product 
		{
			get { return product; }
		}

		public int LicensedInstances
		{
			get { return product != null ? product.LicensedInstances : -1; }
		}

		/// <summary>
		/// Server license applies to
		/// </summary>
		internal string Server 
		{
			get { return server; }
		}

		#endregion

		#region methods

		public bool IsValid() 
		{
			// Check the product name
            if (String.Compare(product.Name, _productName, true) != 0)
			{
				return false;
			}		

			// Check the key to ensure that it is valid for this server and product
			// Also confirm that the license has not expired
			string hashedKey = GetHashKey(server, productString);
			return hashedKey.Equals(key) && !product.HasExpired() && product.IsValidVersion();
		}

		internal static string GenerateTrialLicenseString(string serverInstanceName) 
		{
			StringBuilder productString = new StringBuilder();
            productString.Append(_productName);
			productString.Append(".");
			productString.Append(LegacyProduct.GetProductVersion());
			productString.Append(".");
            productString.Append(LegacyProduct.GetProductDate(DateTime.Now.AddDays(TrialLicenseExpirationLimitInDays)));
			productString.Append(".");
			productString.Append(TrialLicenseDefaultLicensedInstances);

			StringBuilder licenseString = new StringBuilder();			
			licenseString.Append(serverInstanceName);
			licenseString.Append("&");
			licenseString.Append(productString.ToString());
			licenseString.Append("&");
			licenseString.Append(GetHashKey(serverInstanceName, productString.ToString()));
			
			return licenseString.ToString();
		}

		public bool IsTrialLicense() 
		{
			TimeSpan diff = product.ExpirationDate - DateTime.Now;
			return diff.Days <= TrialLicenseExpirationLimitInDays;
		}

		public string GetTrialLicenseExpirationMessage() 
		{
			DateTime endOfDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
			TimeSpan diff = product.ExpirationDate - endOfDay;

			if (diff.Days == 0)
			{
                return Resources.TrialLicenseWillExpireToday;
			}
			else if (diff.Days > 0)
			{
                return string.Format(Resources.TrialLicenseWillExpire, diff.Days, product.ExpirationDate.ToLongDateString());
			}
			else 
			{
                return Resources.TrialLicenseHasExpired;
			}
		}

		private void ParseLicense(string license) 
		{
			// Server
			server = license.Substring(0, license.IndexOf("&"));
			server = server.Substring(server.IndexOf("=") + 1);

			// Product
			string remaining = license.Substring(license.IndexOf("&") + 1);
			productString = remaining.Substring(0, remaining.IndexOf("&"));
			productString = productString.Substring(productString.IndexOf("=") + 1);
            product = new LegacyProduct(productString);

			// Key
			key = remaining.Substring(remaining.IndexOf("&") + 1);
			key = key.Substring(key.IndexOf("=") + 1);
		}

		private static string GetHashKey(string key, string saltPrecursor) 
		{
			int salt = GetSalt(key, saltPrecursor);

			string theBase = GetBase(salt);
			string strOut = "";

			char[] keyChars = key.ToCharArray();
			char[] baseChars = theBase.ToCharArray();
 
			for (int i = 0; i < keyChars.Length; i++) {

				char c = keyChars[i];
				int j = theBase.IndexOf(c);
 
				if (j >= 0) {

					j += 35;
 
					if (j >= 70) {
						j -= 70;
					}
 
					strOut += baseChars[j];

				} else {
					strOut += c;
				}

			}

			return strOut;
		}

		private static string GetBase(int salt) 
		{
 			if (salt < 2) {
				salt = 2;
			}
 
			if (salt > 40) {
				salt = 40;
			}
 
			string theBase = "";
 
			for (int i = 48; i <= 57; i++) {
				theBase += (char)i;
			}
			for (int i = 64; i <= 90; i++) {
				theBase += (char)i;
			}
			for (int i = 97; i <= 122; i++) {
				theBase += (char)i;
			}
			theBase += (char)35;
			theBase += (char)36;
			theBase += (char)37;
			theBase += (char)45;
			theBase += (char)46;
			theBase += (char)42;
			theBase += '^';
 
			string strMd = "";
 
			for (int i = 0; i < salt; i++) {
				for (int j = 0; j < theBase.Length; j++) {
					if ((j % salt) == i) {
						strMd += theBase.Substring(j, 1);
					}
				}
			}

			return strMd;
		}

		private static int GetSalt(string key, string saltPrecursor) 
		{
			int j = 0;
			int pId = 0;

			char[] dChars = key.ToCharArray();
			char[] pChars = saltPrecursor.ToCharArray();
 
			for (int i = 0; i < dChars.Length; i++) {
				j += dChars[i];
			}
 
			for (int i = 0; i < pChars.Length; i++) {
				pId += pChars[i];
			}
 
			int rtn = j * pId + pId;
			rtn = rtn % 40;
 
			if (rtn < 2) {
				rtn = 2;
			}
 
			return rtn;
		}

		public override string ToString() 
		{
			StringBuilder builder = new StringBuilder();
			builder.Append(server);
			builder.Append("&");
			builder.Append(product.ToString());
			builder.Append("&");
			builder.Append(key);
			return builder.ToString();
		}

		#endregion
	}
}
