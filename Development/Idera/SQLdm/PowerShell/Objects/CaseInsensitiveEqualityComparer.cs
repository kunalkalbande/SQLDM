//------------------------------------------------------------------------------
// <copyright file="CaseInsensitiveEqualityComparer.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;


namespace Idera.SQLdm.PowerShell.Objects
{
    public class CaseInsensitiveEqualityComparer : IEqualityComparer<string>
    {
        private readonly StringComparer comparer = StringComparer.InvariantCultureIgnoreCase;

        public bool Equals(string x, string y)
        {
            return comparer.Equals(x, y);
        }

        public int GetHashCode(string obj)
        {
            return comparer.GetHashCode(obj);
        }
    }
}
