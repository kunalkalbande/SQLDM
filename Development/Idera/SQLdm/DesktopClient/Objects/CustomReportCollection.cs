using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using Wintellect.PowerCollections;

namespace Idera.SQLdm.DesktopClient.Objects
{
    internal class CustomReportCollection : Dictionary<string, CustomReport>
    {
        public readonly object SyncRoot = new object();
        public event EventHandler<CustomReportCollectionChangedEventArgs> Changed;
        

        public CustomReportCollection()
            : base(StringComparer.CurrentCultureIgnoreCase)
        {
            Settings.Default.ActiveRepositoryConnectionChanged += Settings_ActiveRepositoryConnectionChanged;
        }

        private void Settings_ActiveRepositoryConnectionChanged(object sender, EventArgs e)
        {
            //ClearItems();
        }

        private void OnChanged(CustomReportCollectionChangedEventArgs e)
        {
            if (Changed != null)
            {
                Changed(this, e);
            }
        }

        protected string GetKeyForItem(CustomReport item)
        {
            return item.Name;
        }

        protected void InsertItem(string key, CustomReport newItem)
        {
            lock (SyncRoot)
            {
                Add(key, newItem);
            }
        }

        protected void SetItem(string key, CustomReport newItem)
        {
            lock (SyncRoot)
            {
                base[key] = newItem;
                OnChanged(
                    new CustomReportCollectionChangedEventArgs(KeyedCollectionChangeType.Replaced,
                                                      BuildChangeList(new CustomReport[] { newItem })));
            }
        }
        
        public void AddRange(ICollection<CustomReport> newItems)
        {
            if (newItems != null && newItems.Count > 0)
            {
                lock (SyncRoot)
                {
                    Dictionary<string, CustomReport> changeList = new Dictionary<string, CustomReport>();

                    foreach (CustomReport newItem in newItems)
                    {
                        if (!ContainsKey(newItem.Name))
                        {
                            Add(newItem.Name, newItem);
                            changeList.Add(newItem.Name, newItem);
                        }
                    }

                    OnChanged(new CustomReportCollectionChangedEventArgs(KeyedCollectionChangeType.Added, changeList));
                }
            }
        }

        public void RemoveRange(ICollection<CustomReport> items)
        {
            if (items != null && items.Count > 0)
            {
                lock (SyncRoot)
                {
                    Dictionary<string, CustomReport> changeList = new Dictionary<string, CustomReport>();

                    foreach (CustomReport item in items)
                    {
                        base.Remove(item.Name);
                        changeList.Add(item.Name, item);
                    }

                    OnChanged(new CustomReportCollectionChangedEventArgs(KeyedCollectionChangeType.Removed, changeList));
                }
            }
        }

        public void UpdateRange(ICollection<CustomReport> items)
        {
            if (items != null && items.Count > 0)
            {
                lock (SyncRoot)
                {
                    var updatedItems = new Dictionary<string, CustomReport>();

                    //go though all custom reports in the collection
                    foreach (var item in items)
                    {
                        //remove all instances of the changes key from the base
                        while (ContainsKey(item.Name))
                        {
                            Remove(this[item.Name].Name);
                        }
                        
                        //add the new one to the base
                        Add(item.Name, item);
                        
                        //add the updated item to updatedItems
                        if (!updatedItems.ContainsKey(item.Name))
                            updatedItems.Add(item.Name, item);
                    }

                    OnChanged(new CustomReportCollectionChangedEventArgs(KeyedCollectionChangeType.Replaced, updatedItems));
                }
            }
        }

        public void RemoveItem(string key)
        {
            lock (SyncRoot)
            {
                base.Remove(key);
                CustomReport deletereport = new CustomReport(key);
                Dictionary<string,CustomReport> Onereport = new Dictionary<string, CustomReport>();
                Onereport.Add(deletereport.Name, deletereport);
                OnChanged(new CustomReportCollectionChangedEventArgs(KeyedCollectionChangeType.Removed, Onereport));
            }
        }

        protected void ClearItems()
        {
            lock (SyncRoot)
            {
                base.Clear();;
                OnChanged(new CustomReportCollectionChangedEventArgs(KeyedCollectionChangeType.Cleared, null));
            }
        }

        public void Add(CustomReport newItem)
        {
            AddRange(new CustomReport[] { newItem });
        }

        public void Insert(string key, CustomReport newItem)
        {
            base.Add(key, newItem);
            OnChanged(
                new CustomReportCollectionChangedEventArgs(KeyedCollectionChangeType.Added, BuildChangeList(new CustomReport[] { newItem })));
        }


        public void AddOrUpdate(CustomReport newItem)
        {
            lock (SyncRoot)
            {
                if (base.ContainsKey(newItem.Name))
                {
                    CustomReport existingItem = base[newItem.Name];
                    base.Remove(existingItem.Name);
                    base.Add(newItem.Name, newItem);

                    OnChanged(
                        new CustomReportCollectionChangedEventArgs(KeyedCollectionChangeType.Replaced,
                                                          BuildChangeList(new CustomReport[] { newItem })));
                }
                else
                {
                    Add(newItem);
                }
            }
        }

        public bool TryGetCustomReport(string reportName, ref CustomReport report)
        {
            lock (SyncRoot)
            {
                if (ContainsKey(reportName))
                {
                    report = this[reportName];
                    return true;
                }
                return false;
            }
        }

        private static IDictionary<string, CustomReport> BuildChangeList(IEnumerable<CustomReport> changes)
        {
            Dictionary<string, CustomReport> changeList = new Dictionary<string, CustomReport>();

            if (changes != null)
            {
                foreach (CustomReport report in changes)
                {
                    changeList.Add(report.Name, report);
                }
            }

            return changeList;
        }
    }

    internal class CustomReportCollectionChangedEventArgs : EventArgs
    {
        public readonly KeyedCollectionChangeType ChangeType;
        public readonly IDictionary<string, CustomReport> Reports;

        public CustomReportCollectionChangedEventArgs(
            KeyedCollectionChangeType change,
            IDictionary<string, CustomReport> reports)
        {
            ChangeType = change;
            Reports = reports;
        }
    }
}