//------------------------------------------------------------------------------
// <copyright file="Product.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.DesktopClient.Helpers 
{
	using System;
	using System.Text;
	using System.Reflection;

	/// <summary>
	/// Represents the product portion of an Idera product license.
	/// </summary>
	internal sealed class LegacyProduct 
	{
		#region fields

		private DateTime	expirationDate;
		private int			licensedInstances;
		private string		name;
		private string		version;

		#endregion

		#region constructors

        internal LegacyProduct(string productString) 
		{
			string[] productParts = productString.Split('.');

			name = productParts[0];
			version = productParts[1];
			expirationDate = GetDateTimeFromProductDate(productParts[2]);
			licensedInstances = Convert.ToInt32(productParts[3]);
		}

		#region properties

		/// <summary>
		/// Expiration date
		/// </summary>
		public DateTime ExpirationDate 
		{
			get { return expirationDate; }
		}

		/// <summary>
		/// The number of licensed instances.
		/// </summary>
		public int LicensedInstances
		{
			get { return licensedInstances; }
		}

		/// <summary>
		/// Product name
		/// </summary>
		public string Name 
		{
			get { return name; }
		}

		/// <summary>
		/// Product version
		/// </summary>
		public string Version 
		{
			get { return version; }
		}

		#endregion

		#endregion

		#region methods

		public static string GetProductVersion() 
		{
			StringBuilder builder = new StringBuilder();

			// Get the assembly version using reflection
			Version version = Assembly.GetExecutingAssembly().GetName().Version;
			if (version.Major < 10) 
			{
				builder.Append("0");
			}
			builder.Append(version.Major);
			builder.Append(version.Minor);
			builder.Append("0");
			
			return builder.ToString();
		}

		public static string GetProductDate(DateTime date) 
		{
			StringBuilder productDate = new StringBuilder();
			productDate.Append(date.Year);
			if (date.Month < 10) 
			{
				productDate.Append("0");
			}
			productDate.Append(date.Month);
			if (date.Day < 10) 
			{
				productDate.Append("0");
			}
			productDate.Append(date.Day);
			return productDate.ToString();
		}

		private static DateTime GetDateTimeFromProductDate(string productDate) 
		{
			string year = productDate.Substring(0, 4);
			string month = productDate.Substring(4, 2);
			string day = productDate.Substring(6, 2);
			return new DateTime(Int32.Parse(year), Int32.Parse(month), Int32.Parse(day), 23, 59, 59);
		}

		public bool HasExpired() 
		{
			return expirationDate.CompareTo(DateTime.Now) < 0;
		}

		public bool IsValidVersion()
		{
			switch (version)
			{
				case "0310":
				case "0350":
				case "0360":
				case "0400":
				case "0410":
				case "0450":
				case "0460":
					return true;
				default:
					return false;
			}
		}

		public override string ToString() 
		{
			StringBuilder builder = new StringBuilder();
			builder.Append(name);
			builder.Append(".");
			builder.Append(version);
			builder.Append(".");
			builder.Append(GetProductDate(expirationDate));
			builder.Append(".");
			builder.Append(LicensedInstances);
			return builder.ToString();
		}	
	
		#endregion
	}
}
