using System;
using System.Collections.ObjectModel;

namespace Idera.SQLdm.DesktopClient.Objects
{
    [Serializable]
    internal class UserViewCollection : KeyedCollection<Guid, UserView>
    {
        [NonSerialized]
        public EventHandler<UserViewCollectionChangedEventArgs> Changed;

        public UserViewCollection() : base(null, 0)
        {
        }

        private void OnChanged(UserViewCollectionChangedEventArgs e)
        {
            if (Changed != null)
            {
                Changed(this, e);
            }
        }

        protected override Guid GetKeyForItem(UserView item)
        {
            return item.Id;
        }

        protected override void InsertItem(int index, UserView newItem)
        {
            UserView existingView;

            if (TryGetValue(newItem.Name, out existingView))
            {
                throw new ArgumentException("A user view with the same name already exists.");
            }
            else
            {
                base.InsertItem(index, newItem);
                OnChanged(new UserViewCollectionChangedEventArgs(KeyedCollectionChangeType.Added, newItem));
            }
        }

        protected override void SetItem(int index, UserView newItem)
        {
            UserView existingView;

            if (TryGetValue(newItem.Name, out existingView))
            {
                throw new ArgumentException("A user view with the same name already exists.");
            }
            else
            {
                base.SetItem(index, newItem);
                OnChanged(new UserViewCollectionChangedEventArgs(KeyedCollectionChangeType.Replaced, newItem));
            }
        }

        protected override void RemoveItem(int index)
        {
            UserView removedItem = Items[index];
            base.RemoveItem(index);
            OnChanged(new UserViewCollectionChangedEventArgs(KeyedCollectionChangeType.Removed, removedItem));
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            OnChanged(new UserViewCollectionChangedEventArgs(KeyedCollectionChangeType.Cleared, null));
        }

        public bool TryGetValue(string name, out UserView userView)
        {
            bool matchFound = false;
            userView = null;

            foreach (UserView existingView in Items)
            {
                if (string.Compare(existingView.Name, name, true) == 0)
                {
                    matchFound = true;
                    userView = existingView;
                    break;
                }
            }

            return matchFound;
        }
    }

    internal class UserViewCollectionChangedEventArgs : EventArgs
    {
        public readonly KeyedCollectionChangeType ChangeType;
        public readonly UserView UserView;

        public UserViewCollectionChangedEventArgs(
            KeyedCollectionChangeType change,
            UserView userView)
        {
            ChangeType = change;
            UserView = userView;
        }
    }

    internal enum KeyedCollectionChangeType
    {
        Added,
        Removed,
        Replaced,
        Cleared
    }
}