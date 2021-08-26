//------------------------------------------------------------------------------
// <copyright file="InstrumentedQueue.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using Idera.SQLdm.Common.Data;

namespace Idera.SQLdm.ManagementService.Health
{

//    moved to common...
//    public interface IQueue<T> 
//    {
//        int Count { get; }
//        
//        void Clear();
//
//        T Dequeue();
//        
//        void Enqueue(T item);
//
//        T Peek();
//    }
    
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
            throw new Exception("The method or operation is not implemented.");
        }

        public T[] PeekAll()
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
