using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;
using Wintellect.PowerCollections;
using System.Linq;

namespace Idera.SQLdm.DesktopClient.Objects
{
    internal class TagCollection : KeyedCollection<int, Tag>
    {
        public readonly object SyncRoot = new object();
        public event EventHandler<TagCollectionChangedEventArgs> Changed;
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("TagCollection");

        public TagCollection()
            : base(null, 0)
        {
        }

        /// <summary>
        /// Returns a read-only collection of tag id's.
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

        public IDictionary<int, Tag> GetDictionary()
        {
            if (Dictionary != null)
            {
                return new Dictionary<int, Tag>(Dictionary);
            }
            else
            {
                return null;
            }
        }

        private void OnChanged(TagCollectionChangedEventArgs e)
        {
            if (Changed != null)
            {
                Changed(this, e);
            }
        }

        protected override int GetKeyForItem(Tag item)
        {
            return item.Id;
        }

        protected override void InsertItem(int index, Tag newItem)
        {
            lock (SyncRoot)
            {
                base.InsertItem(index, newItem);
            }
        }

        protected override void SetItem(int index, Tag newItem)
        {
            lock (SyncRoot)
            {
                base.SetItem(index, newItem);
                OnChanged(
                    new TagCollectionChangedEventArgs(KeyedCollectionChangeType.Replaced,
                                                      BuildChangeList(new Tag[] { newItem })));
            }
        }

        protected override void RemoveItem(int index)
        {
            lock (SyncRoot)
            {
                base.RemoveItem(index);
            }
        }

        protected override void ClearItems()
        {
            lock (SyncRoot)
            {
                base.ClearItems();
                OnChanged(new TagCollectionChangedEventArgs(KeyedCollectionChangeType.Cleared, null));
            }
        }

        public new void Add(Tag newItem)
        {
            AddRange(new Tag[] { newItem });
        }

        public new void Insert(int index, Tag newItem)
        {
            base.Insert(index, newItem);
            OnChanged(
                new TagCollectionChangedEventArgs(KeyedCollectionChangeType.Added, BuildChangeList(new Tag[] { newItem })));
        }

        public void AddRange(ICollection<Tag> newItems)
        {
            if (newItems != null && newItems.Count > 0)
            {
                lock (SyncRoot)
                {
                    Dictionary<int, Tag> changeList = new Dictionary<int, Tag>();

                    foreach (Tag newItem in newItems)
                    {
                        if (!Contains(newItem.Id))
                        {
                            base.Add(newItem);
                            changeList.Add(newItem.Id, newItem);
                        }
                    }

                    OnChanged(new TagCollectionChangedEventArgs(KeyedCollectionChangeType.Added, changeList));
                }
            }
        }

        public new void Remove(Tag item)
        {
            RemoveRange(new Tag[] { item });
        }

        public new void RemoveAt(int index)
        {
            Tag removedItem = Items[index];
            base.RemoveAt(index);
            OnChanged(
                new TagCollectionChangedEventArgs(KeyedCollectionChangeType.Removed,
                                                  BuildChangeList(new Tag[] { removedItem })));
        }

        public void RemoveRange(ICollection<Tag> items)
        {
            if (items != null && items.Count > 0)
            {
                lock (SyncRoot)
                {
                    Dictionary<int, Tag> changeList = new Dictionary<int, Tag>();

                    foreach (Tag item in items)
                    {
                        base.Remove(item);
                        changeList.Add(item.Id, item);
                    }

                    OnChanged(new TagCollectionChangedEventArgs(KeyedCollectionChangeType.Removed, changeList));
                }
            }
        }
        
        public void UpdateRange(ICollection<Tag> items)
        {
            if (items != null && items.Count > 0)
            {
                lock (SyncRoot)
                {
                    Dictionary<int, Tag> updatedItems = new Dictionary<int, Tag>();

                    foreach (Tag item in items)
                    {
                        if (Contains(item.Id))
                        {
                            Tag existingItem = Dictionary[item.Id];
                            base.Remove(existingItem);
                            base.Add(item);
                            updatedItems.Add(item.Id, item);
                        }
                    }

                    OnChanged(new TagCollectionChangedEventArgs(KeyedCollectionChangeType.Replaced, updatedItems));
                }
            }
        }

        public void AddOrUpdate(Tag newItem)
        {
            lock (SyncRoot)
            {
                if (Contains(newItem.Id))
                {
                    Tag existingItem = Dictionary[newItem.Id];
                    base.Remove(existingItem);
                    base.Add(newItem);
                    OnChanged(
                        new TagCollectionChangedEventArgs(KeyedCollectionChangeType.Replaced,
                                                          BuildChangeList(new Tag[] { newItem })));
                }
                else
                {
                    Add(newItem);

                    // Setting context data for adding new Tag
                    AuditingEngine.SetContextData(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);
                }
            }
        }

        public bool TryGetTag(int tagId, ref Tag tag)
        {
            lock (SyncRoot)
            {
                if (Contains(tagId))
                {
                    tag = this[tagId];
                    return true;
                }
                return false;
            }
        }

        private static IDictionary<int, Tag> BuildChangeList(IEnumerable<Tag> changes)
        {
            Dictionary<int, Tag> changeList = new Dictionary<int, Tag>();

            if (changes != null)
            {
                foreach (Tag tag in changes)
                {
                    changeList.Add(tag.Id, tag);
                }
            }

            return changeList;
        }
        
    }

    internal class TagCollectionChangedEventArgs : EventArgs
    {
        public readonly KeyedCollectionChangeType ChangeType;
        public readonly IDictionary<int, Tag> Tags;

        public TagCollectionChangedEventArgs(
            KeyedCollectionChangeType change,
            IDictionary<int, Tag> tags)
        {
            ChangeType = change;
            Tags = tags;
        }
    }
}
