//------------------------------------------------------------------------------
// <copyright file="ScheduledCollectionSnapshotSendQueue.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.CollectionService.Monitoring
{
    using System;
    using System.Collections.Generic;
    using System.IO.Compression;
    using Idera.SQLdm.Common.Data;
    using Idera.SQLdm.Common.Services;
    using System.Runtime.Serialization;
    using System.IO;
    using Idera.SQLdm.CollectionService.Configuration;
    using System.Runtime.Serialization.Formatters.Binary;
    using Wintellect.PowerCollections;

    /// <summary>
    /// Scheduled collection enqueues all collected data to this queue.  A single thread
    /// is responsible for pulling data out of this queue and sending it to the management
    /// service.  If the connection to the management service is interrupted, that thread handles
    /// saving this data to prevent loss.
    /// </summary>
    [Serializable]
    public class ScheduledCollectionDataSendQueue : ISerializable
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("ScheduledCollectionDataSendQueue");
        private const int STREAM_TO_DISK_THRESHOLD = 5;

        private object queueLock;       
        private readonly Q<ScheduledCollectionDataMessageWrapper> queue;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ScheduledCollectionDataSendQueue"/> class.
        /// </summary>
        public ScheduledCollectionDataSendQueue()
        {
            queue = new Q<ScheduledCollectionDataMessageWrapper>(QMode.LIFO);
            queueLock = new object();
        }


        #region ISerializable Members

        public ScheduledCollectionDataSendQueue(SerializationInfo info, StreamingContext context)
        {
            queueLock = new object();
            try
            {
                object q = info.GetValue("queue", typeof(object));
                if (q is Q<ScheduledCollectionDataMessageWrapper>)
                    queue = (Q<ScheduledCollectionDataMessageWrapper>) q;
                else
                if (q is Queue<ScheduledCollectionDataMessageWrapper>)
                {
                    queue = new Q<ScheduledCollectionDataMessageWrapper>((Queue<ScheduledCollectionDataMessageWrapper>)q); 
                } 
            } catch (Exception e)
            {
                
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("queueLock", queueLock);
            info.AddValue("queue", queue);
        }

        #endregion

        public QMode QueueMode
        {
            get { return queue.Mode;  }
            set { queue.Mode = value; }
        }

        public int Count
        {
            get
            {
                lock (queueLock)
                {
                    return queue.Count;
                }
            }
        }

        /// <summary>
        /// Enqueues the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        public void Enqueue(ScheduledCollectionDataMessage data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            Enqueue(new ScheduledCollectionDataMessageWrapper(data));
        }

        public void Enqueue(ScheduledCollectionDataMessageWrapper wrapper)
        {
            using (LOG.VerboseCall("Enqueue"))
            {
                if (wrapper == null)
                    throw new ArgumentNullException("wrapper");

                lock (queueLock)
                {
                    queue.Enqueue(wrapper);
                    LOG.VerboseFormat("Scheduled collection queue contains {0} entries", queue.Count);
                }
            }
        }

//        private void PrepareEnqueue(ScheduledCollectionDataMessageWrapper message)
//        {
//            lock (queueLock)
//            {
//                if (queue.Count < STREAM_TO_DISK_THRESHOLD)
//                    return;       
//            }
//
//            message.Store();
//        }

        /// <summary>
        /// Dequeues this instance.
        /// </summary>
        /// <returns></returns>
        public ScheduledCollectionDataMessage Dequeue()
        {
            using (LOG.VerboseCall("Dequeue()"))
            {
                ScheduledCollectionDataMessage message = null;
                lock (queueLock)
                {
                    while (message == null && queue.Count > 0)
                    {
                        ScheduledCollectionDataMessageWrapper wrapper = queue.Dequeue();
                        try
                        {
                            message = wrapper.Message;
                        }
                        catch (Exception e)
                        {
                            LOG.Error("Unable to dequeue scheduled refresh snapshot (snapshot discarded):", e);
                        }
                    }
                }
                return message;
            }
        }

        public ScheduledCollectionDataMessage Dequeue(int mark)
        {
            using (LOG.VerboseCall("Dequeue(mark)"))
            {
                ScheduledCollectionDataMessage message = null;
                lock (queueLock)
                {
                    ScheduledCollectionDataMessageWrapper wrapper;
                    if (Count <= mark)
                        return Dequeue();

                    // someone added something since the last peek
                    int x = Count - mark;
                    LOG.VerboseFormat("Dequeue mark={0} count={1} index={2}", mark, Count, x);
                    wrapper = queue.Get(x);
                    try
                    {
                        message = wrapper.Message;
                    }
                    catch (Exception e)
                    {
                        LOG.Error("Unable to dequeue scheduled refresh snapshot (snapshot discarded):", e);
                    }
                }
                return message;
            }
        }


        /// <summary>
        /// Peeks this instance.
        /// </summary>
        /// <returns></returns>
        public ScheduledCollectionDataMessage Peek(out int mark)
        {
            ScheduledCollectionDataMessage message = null;
            Guid? messageKey = null;
            lock (queueLock)
            {
                while (message == null && queue.Count > 0)
                {
                    ScheduledCollectionDataMessageWrapper wrapper = queue.Peek();
                    try
                    {
                        message = wrapper.Message;
                    }
                    catch (Exception e)
                    {
                        LOG.Error("Unable to dequeue (peek) scheduled refresh snapshot (snapshot discarded):", e);
                        queue.Dequeue();
                    }
                }
                // the mark is the size of the queue
                mark = queue.Count;
            }
            return message;
        }

        [Serializable]
        public class ScheduledCollectionDataMessageWrapper : ISerializable
        {
            private static DateTime fileControl = DateTime.UtcNow.Date;
            private static int fileSequence = 0;
            private object sync;

            private DateTime created;
            private ScheduledCollectionDataMessage message;
            private string filename;

            public ScheduledCollectionDataMessageWrapper(ScheduledCollectionDataMessage message)
            {
                sync = new object();

                created = DateTime.Now;
                this.message = message;
                filename = null;
            }

            public ScheduledCollectionDataMessageWrapper(SerializationInfo info, StreamingContext context)
            {
                sync = new object();
                created = info.GetDateTime("created");
                filename = info.GetString("filename");
                if (filename == null)
                    message = (ScheduledCollectionDataMessage)info.GetValue("message", typeof(ScheduledCollectionDataMessage));                
            }

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                lock (sync)
                {
                    info.AddValue("created", created);
                    info.AddValue("message", message);
                    info.AddValue("filename", filename);
                }
            }

            private FileInfo GetNextFileName(string path)
            {
                DateTime now = DateTime.UtcNow.Date;
                if (now != fileControl)
                {
                    fileControl = now;
                    fileSequence = 0;
                }
                string name = null;
                do
                {
                    name = path + String.Format("\\{0:yyyyMMdd}{1:00000000}.bin", now, ++fileSequence);
                }
                while (File.Exists(name));

                return new FileInfo(name);
            }


            public void Store()
            {
                lock (sync)
                {
                    if (message == null)
                        return;

                    string dataPath = Path.Combine(CollectionServiceConfiguration.DataPath, "Queued");
                    if (!Directory.Exists(dataPath))
                        Directory.CreateDirectory(dataPath);

                    FileInfo file = GetNextFileName(dataPath);

                    BinaryFormatter formatter = new BinaryFormatter();
                    using (FileStream fileStream = file.Create())
                    {
                        DeflateStream deflater = new DeflateStream(fileStream, CompressionMode.Compress, true);
                        BufferedStream stream = new BufferedStream(deflater);
                        formatter.Serialize(stream, message);
                        stream.Close();
                        deflater.Close();
                    }
                    filename = file.FullName;
                    message = null;
                }
            }

            public ScheduledCollectionDataMessage Message
            {
                get
                {
                    lock (sync)
                    {
                        if (message == null)
                            Load();

                        return message;
                    }
                }
            }

            private void Load()
            {
                if (filename == null)
                    return;

                FileInfo file = new FileInfo(filename);
                if (file.Exists)
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    using (FileStream fileStream = new FileInfo(filename).OpenRead())
                    {
                        BufferedStream stream = new BufferedStream(fileStream);
                        DeflateStream inflater = new DeflateStream(stream, CompressionMode.Decompress);
                        message = (ScheduledCollectionDataMessage)formatter.Deserialize(inflater);
                        inflater.Close();
                        stream.Close();
                    }
                    file.Delete();
                }
                filename = null;
            }
        }


    }
}
