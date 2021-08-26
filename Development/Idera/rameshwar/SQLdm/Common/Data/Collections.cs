//------------------------------------------------------------------------------
// <copyright file="Collections.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Data
{
    using System.Collections;
    using System.Collections.Generic;

    public static class Collections
    {

        public static T[] ToArray<T>(ICollection<T> collection)
        {
            T[] result = new T[collection.Count];
            collection.CopyTo(result, 0);
            return result;
        }

        public static T[] ToArray<T>(ICollection<T> collection, object sync)
        {
            lock (sync)
            {
                return ToArray(collection);
            }
        }

        public static T[] ToArray<T>(ICollection collection)
        {
            T[] result = new T[collection.Count];
            collection.CopyTo(result, 0);
            return result;
        }

    }
}
