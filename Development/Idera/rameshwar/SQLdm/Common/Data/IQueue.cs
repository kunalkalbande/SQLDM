namespace Idera.SQLdm.Common.Data
{
    using System;
    using System.Collections.Generic;
    using Wintellect.PowerCollections;

    public interface IQueue<T>
    {
        int Count { get; }

        void Clear();

        T Dequeue();

        T[] Dequeue(int count);

        void Enqueue(T item);

        T Peek();

        T[] PeekAll();
    }

    [Serializable]
    public enum QMode
    {
        LIFO,
        FIFO
    }

    [Serializable]
    public class Q<T> : IQueue<T>
    {
        private delegate void _enqueue(T item);

        private QMode mode = QMode.FIFO;
        private Deque<T> dq = new Deque<T>();

        private _enqueue enqueueItem;

        public Q() : this(QMode.FIFO)
        {
            
        }

        public Q(QMode mode)
        {
            Mode = mode;
            SetQueueDelegates();
        }

        public Q(Queue<T> queue) : this(QMode.FIFO)
        {
            dq.AddManyToBack(queue.ToArray());
        }

        public Q(Stack<T> queue) : this(QMode.LIFO)
        {
            dq.AddManyToFront(queue.ToArray());
        }

        public QMode Mode
        {
            get { return mode; }
            set
            {
                if (mode != value)
                {
                    T[] items = dq.ToArray();
                    dq.Clear();
                    dq.AddManyToFront(items);
                    mode = value;
                }
                SetQueueDelegates();
            }
        }

        private void SetQueueDelegates()
        {
            if (mode == QMode.FIFO)
            {
                enqueueItem = dq.AddToBack;
            } else
            {
                enqueueItem = dq.AddToFront;
            }
        }

        public int Count
        {
            get { return dq.Count; }
        }

        public void Clear()
        {
            dq.Clear();
        }

        public T Get(int x)
        {
            return dq[x];
        }

        public T Dequeue()
        {
            return dq.RemoveFromFront();
        }

        public T[] Dequeue(int count)
        {
            T[] result;

            if (count > dq.Count)
                count = dq.Count;

            if (count == dq.Count)
            {
                result = dq.ToArray();
                dq.Clear();
            }
            else
            {
                result = new T[count];
                for (int i = 0; i < count; i++)
                {
                    result[i] = dq.GetAtFront();
                }
            }
            return result;
        }

        public bool Remove(T item)
        {
            return dq.Remove(item);
        }

        public void Enqueue(T item)
        {
            enqueueItem(item);
        }

        public T Peek()
        {
            return dq.GetAtFront();
        }

        public T[] PeekAll()
        {
            return dq.ToArray();
        }
    }

}
