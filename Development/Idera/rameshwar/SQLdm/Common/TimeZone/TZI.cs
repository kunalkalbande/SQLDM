//------------------------------------------------------------------------------
// <copyright file="TZI.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.TimeZone
{
    using System.Runtime.InteropServices;
    // MS KB Q115231
	/// <summary>
	/// For unpacking the TZI binary registry value that is contained in
	/// each TimeZone key in the registry.
	/// </summary>
	/// <remarks>This is a structure for reading the TZI binary key data that
	/// is stored in the registry for all of the various time-zones</remarks>
	[StructLayout(LayoutKind.Sequential,Pack=2)]
	internal struct TZI 
	{
		public int Bias;
		public int StandardBias;
		public int DaylightBias;
		public SystemTime StandardDate;
		public SystemTime DaylightDate;
	}
}
