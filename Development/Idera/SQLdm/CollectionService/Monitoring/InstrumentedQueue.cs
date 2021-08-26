//------------------------------------------------------------------------------
// <copyright file="InstrumentedQueue.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.CollectionService.Monitoring
{
    using System.Collections.Generic;
    using Idera.SQLdm.Common.Data;

    public class InstrumentedQueue<T> : IQueue<T>
    {
        public delegate void SetCounterDelegate(int itemsAdded, int queueLength);

        private SetCounterDelegate instrumentationMethod;
        private Queue<T> queue;

        public InstrumentedQueue(Queue<T> queue, SetCounterDelegate instrumentationMethod)
        {
            this.queue = queue;
            this.instrumentationMethod = instrumentationMethod;
        }

        public int Count
        {
            get { return queue.Count;  }
        }

        public void Clear()
        {
            queue.Clear();
            instrumentationMethod.Invoke(0, 0);
        }

        public T Dequeue()
        {
            T result = queue.Dequeue();
            instrumentationMethod.Invoke(0, queue.Count);
            return result;
        }

        public void Enqueue(T item)
        {
            queue.Enqueue(item);
            instrumentationMethod.Invoke(1, queue.Count);
        }

        public T Peek()
        {
            return queue.Peek();
        }

        public T[] Dequeue(int count)
        {
            throw new System.Exception("The method or operation is not implemented.");
        }

        public T[] PeekAll()
        {
            throw new System.Exception("The method or operation is not implemented.");
        }
    }
}