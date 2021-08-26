//------------------------------------------------------------------------------
// <copyright file="InstrumentedCollection.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.ManagementService.Health
{
    using System.Collections;
    using System.Collections.Generic;

    public class InstrumentedCollection<T> : ICollection<T>
    {
        public delegate void SetCounterDelegate(int count);

        private SetCounterDelegate instrumentationMethod;
        private ICollection<T> collection;

        public InstrumentedCollection(ICollection<T> collection, SetCounterDelegate instrumentationMethod)
        {
            this.collection = collection;
            this.instrumentationMethod = instrumentationMethod;
        }

        public void Add(T item)
        {
            collection.Add(item);
            instrumentationMethod.Invoke(collection.Count);
        }

        public void Clear()
        {
            collection.Clear();
            instrumentationMethod.Invoke(0);
        }

        public bool Contains(T item)
        {
            return collection.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            collection.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return collection.Count; }
        }

        public bool IsReadOnly
        {
            get { return collection.IsReadOnly; }
        }

        public bool Remove(T item)
        {
            bool result = collection.Remove(item);
            instrumentationMethod.Invoke(collection.Count);
            return result;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)collection).GetEnumerator();
        }
    }
}
