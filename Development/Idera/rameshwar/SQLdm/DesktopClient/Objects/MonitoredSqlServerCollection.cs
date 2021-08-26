using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Idera.SQLdm.Common.Objects;

namespace Idera.SQLdm.DesktopClient.Objects
{
    using Wintellect.PowerCollections;

    internal sealed class MonitoredSqlServerCollection : KeyedCollection<int, MonitoredSqlServerWrapper>
    {
        public readonly object SyncRoot = new object();
        public event EventHandler<MonitoredSqlServerCollectionChangedEventArgs> Changed;

        public MonitoredSqlServerCollection()
            : base(null, 0)
        {
        }

        public MonitoredSqlServerWrapper this[string instanceName]
        {
            get
            {
                lock (SyncRoot)
                {
                    foreach (MonitoredSqlServerWrapper wrapper in Dictionary.Values)
                    {
                        if (wrapper.InstanceName.Equals(instanceName))
                            return wrapper;
                    }
                    return null;
                }
            }
        }

        /// <summary>
        /// Returns a read-only collection of instance id's.
        /// </summary>
        public ICollection<int> Keys
        {
            get
            {
                if (Dictionary != null)
                {
                    return Algorithms.ReadOnly(Dictionary.Keys);
                }
                else
                {
                    return null;
                }
            }
        }
 
        public IDictionary<int, MonitoredSqlServerWrapper> GetDictionary()
        {
            if (Dictionary != null)
            {
                return new Dictionary<int, MonitoredSqlServerWrapper>(Dictionary);
            }
            else
            {
                return null;
            }
        }

        private void OnChanged(MonitoredSqlServerCollectionChangedEventArgs e)
        {
            if (Changed != null)
            {
                Changed(this, e);
            }
        }

        protected override int GetKeyForItem(MonitoredSqlServerWrapper item)
        {
            return item.Id;
        }

        protected override void InsertItem(int index, MonitoredSqlServerWrapper newItem)
        {
            lock (SyncRoot)
            {
                base.InsertItem(index, newItem);
            }
        }

        protected override void SetItem(int index, MonitoredSqlServerWrapper newItem)
        {
            lock (SyncRoot)
            {
                base.SetItem(index, newItem);
                OnChanged(
                    new MonitoredSqlServerCollectionChangedEventArgs(KeyedCollectionChangeType.Replaced,
                                                                     new MonitoredSqlServerWrapper[] {newItem}));
            }
        }

        protected override void RemoveItem(int index)
        {
            lock (SyncRoot)
            {
                MonitoredSqlServerWrapper removedItem = Items[index];
                base.RemoveItem(index);
            }
        }

        protected override void ClearItems()
        {
            lock (SyncRoot)
            {
                if (base.Count > 0)
                {
                    base.ClearItems();
                    OnChanged(new MonitoredSqlServerCollectionChangedEventArgs(KeyedCollectionChangeType.Cleared, null));
                }
            }
        }

        public new void Add(MonitoredSqlServerWrapper newItem)
        {
            AddRange(new MonitoredSqlServerWrapper[] {newItem});
        }

        public new void Insert(int index, MonitoredSqlServerWrapper newItem)
        {
            base.Insert(index, newItem);
            OnChanged(
                new MonitoredSqlServerCollectionChangedEventArgs(KeyedCollectionChangeType.Added,
                                                                 new MonitoredSqlServerWrapper[] {newItem}));
        }

        public void AddRange(IList<MonitoredSqlServerWrapper> newItems)
        {
            if (newItems != null && newItems.Count > 0)
            {
                lock (SyncRoot)
                {
                    foreach (MonitoredSqlServerWrapper newItem in newItems)
                    {
                        if (!Contains(newItem.Id))
                        {
                            base.Add(newItem);
                        }
                        else
                        {

                        }
                    }

                    OnChanged(new MonitoredSqlServerCollectionChangedEventArgs(KeyedCollectionChangeType.Added, newItems));
                }
            }
        }

        public new void Remove(MonitoredSqlServerWrapper item)
        {
            RemoveRange(new MonitoredSqlServerWrapper[] { item });
        }

        public new void RemoveAt(int index)
        {
            MonitoredSqlServerWrapper removedItem = Items[index];
            base.RemoveAt(index);
            OnChanged(new MonitoredSqlServerCollectionChangedEventArgs(KeyedCollectionChangeType.Removed,
                                                                       new MonitoredSqlServerWrapper[] { removedItem }));
        }

        public void RemoveRange(ICollection<MonitoredSqlServerWrapper> items)
        {
            if (items != null && items.Count > 0)
            {
                lock (SyncRoot)
                {
                    foreach (MonitoredSqlServerWrapper item in items)
                    {
                        base.Remove(item);
                    }

                    OnChanged(new MonitoredSqlServerCollectionChangedEventArgs(KeyedCollectionChangeType.Removed, items));
                }
            }
        }

        public void UpdateRange(ICollection<MonitoredSqlServer> items)
        {
            if (items != null && items.Count > 0)
            {
                lock (SyncRoot)
                {
                    List<MonitoredSqlServerWrapper> updatedItems = new List<MonitoredSqlServerWrapper>();

                    foreach (MonitoredSqlServer item in items)
                    {
                        if (Contains(item.Id))
                        {
                            MonitoredSqlServerWrapper existingItem = Dictionary[item.Id];
                            existingItem.Instance = item;
                            updatedItems.Add(existingItem);
                        }
                    }

                    OnChanged(
                        new MonitoredSqlServerCollectionChangedEventArgs(KeyedCollectionChangeType.Replaced,
                                                                         updatedItems));
                }
            }
        }

        public void AddOrUpdate(MonitoredSqlServer newItem)
        {
            lock (SyncRoot)
            {
                int key = newItem.Id;

                if (Contains(key))
                {
                    MonitoredSqlServerWrapper existingItem = Dictionary[key];
                    existingItem.Instance = newItem;
                    OnChanged(
                        new MonitoredSqlServerCollectionChangedEventArgs(KeyedCollectionChangeType.Replaced,
                                                                         new MonitoredSqlServerWrapper[] {existingItem}));
                }
                else
                {
                    Add(new MonitoredSqlServerWrapper(newItem));
                }
            }
        }
    }

    internal class MonitoredSqlServerCollectionChangedEventArgs : EventArgs
    {
        public readonly KeyedCollectionChangeType ChangeType;
        public readonly ICollection<MonitoredSqlServerWrapper> Instances;

        public MonitoredSqlServerCollectionChangedEventArgs(
            KeyedCollectionChangeType change,
            ICollection<MonitoredSqlServerWrapper> instances)
        {
            ChangeType = change;
            Instances = instances;
        }
    }
}