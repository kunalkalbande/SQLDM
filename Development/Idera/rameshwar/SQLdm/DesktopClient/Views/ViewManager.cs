using System;
using System.Collections.Generic;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using Idera.SQLdm.DesktopClient.Views.Servers.Server;
using Idera.SQLdm.DesktopClient.Views.Servers.ServerGroup;

namespace Idera.SQLdm.DesktopClient.Views
{
    /// <summary>
    /// Caches existing views.
    /// </summary>
    internal sealed class ViewManager
    {
        private int mostFrequentlyUsedLimit = 9;
        private readonly Dictionary<object, ServerGroupView> cachedServerGroupViews = new Dictionary<object, ServerGroupView>();
        private readonly Dictionary<int, ServerViewContainer> cachedServerViews = new Dictionary<int, ServerViewContainer>();
        private readonly List<IView> mostFrequentlyUsedViews = new List<IView>();
        private static readonly BBS.TracerX.Logger StartUpTimeLog = BBS.TracerX.Logger.GetLogger(TextConstants.StartUpTimeLogName);

        public ViewManager()
        {
            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();
            Settings.Default.ActiveRepositoryConnectionChanging += Settings_ActiveRepositoryConnectionChanging;
            Settings.Default.ActiveRepositoryConnectionChanged += Settings_ActiveRepositoryConnectionChanged;

            if (Settings.Default.ActiveRepositoryConnection != null)
            {
                Settings.Default.ActiveRepositoryConnection.UserViews.Changed += UserViews_Changed;
            }

            ApplicationModel.Default.ActiveInstances.Changed += ActiveInstances_Changed;
            ApplicationModel.Default.Tags.Changed += Tags_Changed;
            mostFrequentlyUsedLimit = Settings.Default.CachedViewCount;

            //limit the count to between 1 and 9
            if (mostFrequentlyUsedLimit < 1)
                mostFrequentlyUsedLimit = 1;
            else if (mostFrequentlyUsedLimit > 9)
                mostFrequentlyUsedLimit = 9;
            stopWatch.Stop();
            StartUpTimeLog.DebugFormat("Time taken by ViewManager : {0}", stopWatch.ElapsedMilliseconds);
        }

        public void Dispose()
        {
            Settings.Default.ActiveRepositoryConnectionChanging -= Settings_ActiveRepositoryConnectionChanging;
            Settings.Default.ActiveRepositoryConnectionChanged -= Settings_ActiveRepositoryConnectionChanged;

            if (Settings.Default.ActiveRepositoryConnection != null)
            {
                Settings.Default.ActiveRepositoryConnection.UserViews.Changed -= UserViews_Changed;
            }

            ApplicationModel.Default.ActiveInstances.Changed -= ActiveInstances_Changed;
            ApplicationModel.Default.Tags.Changed -= Tags_Changed;

            Reset();
        }

        public int MostFrequentlyUsedLimit
        {
            get { return mostFrequentlyUsedLimit; }
            set
            {
                mostFrequentlyUsedLimit = value <= 0 ? 0 : value;
                UpdateMostFrequentlyUsedList();
            }
        }

        private void Reset()
        {
            foreach (ServerGroupView groupView in cachedServerGroupViews.Values)
            {
                if (groupView.Parent != null)
                {
                    groupView.Parent.Controls.Remove(groupView);
                }

                groupView.Dispose();
            }

            foreach (var serverView in cachedServerViews.Values)
            {
                serverView.Dispose();
            }
            
            cachedServerGroupViews.Clear();
            cachedServerViews.Clear();
            mostFrequentlyUsedViews.Clear();
        }

        private void Settings_ActiveRepositoryConnectionChanging(object sender, EventArgs e)
        {
            if (Settings.Default.ActiveRepositoryConnection != null)
            {
                Settings.Default.ActiveRepositoryConnection.UserViews.Changed -= UserViews_Changed;
            }
        }

        private void Settings_ActiveRepositoryConnectionChanged(object sender, EventArgs e)
        {
            if (Settings.Default.ActiveRepositoryConnection != null)
            {
                Settings.Default.ActiveRepositoryConnection.UserViews.Changed += UserViews_Changed;
                Reset();
            }
        }

        private void UserViews_Changed(object sender, UserViewCollectionChangedEventArgs e)
        {
            switch (e.ChangeType)
            {
                case KeyedCollectionChangeType.Removed:
                    ServerGroupView existingView;
                    if (cachedServerGroupViews.TryGetValue(e.UserView.Id, out existingView))
                    {
                        cachedServerGroupViews.Remove(e.UserView.Id);
                        mostFrequentlyUsedViews.Remove(existingView);

                        if (existingView.Parent != null)
                        {
                            existingView.Parent.Controls.Remove(existingView);
                        }

                        existingView.Dispose();
                    }
                    break;
            }
        }

        private void Tags_Changed(object sender, TagCollectionChangedEventArgs e)
        {
            switch (e.ChangeType)
            {
                case KeyedCollectionChangeType.Removed:
                    foreach (int tagId in e.Tags.Keys)
                    {
                        ServerGroupView existingView;
                        if (cachedServerGroupViews.TryGetValue(tagId, out existingView))
                        {
                            cachedServerGroupViews.Remove(tagId);
                            mostFrequentlyUsedViews.Remove(existingView);

                            if (existingView.Parent != null)
                            {
                                existingView.Parent.Controls.Remove(existingView);
                            }

                            existingView.Dispose();
                        }
                    }
                    break;
                case KeyedCollectionChangeType.Cleared:
                    List<int> idsToRemove = new List<int>();
                    foreach (object id in cachedServerGroupViews.Keys)
                    {
                        if (id is int)
                        {
                            int tagId = (int) id;
                            ServerGroupView view = cachedServerGroupViews[id];
                            mostFrequentlyUsedViews.Remove(view);

                            if (view.Parent != null)
                            {
                                view.Parent.Controls.Remove(view);
                            }

                            view.Dispose();
                            idsToRemove.Add(tagId);
                        }
                    }

                    foreach (int id in idsToRemove)
                    {
                        cachedServerGroupViews.Remove(id);
                    }
                    break;
            }
        }

        private void ActiveInstances_Changed(object sender, MonitoredSqlServerCollectionChangedEventArgs e)
        {
            switch (e.ChangeType)
            {
                case KeyedCollectionChangeType.Removed:
                    foreach (MonitoredSqlServerWrapper instance in e.Instances)
                    {
                        ServerViewContainer existingView;
                        if (cachedServerViews.TryGetValue(instance.Id, out existingView))
                        {
                            if (existingView == ApplicationController.Default.ActiveView)
                            {
                                existingView.CancelRefresh();
                                existingView.SaveSettings();
                            }

                            cachedServerViews.Remove(instance.Id);
                            mostFrequentlyUsedViews.Remove(existingView);

                            // dispose view should handle all cleanup in dispose
                            existingView.Dispose();
                        }
                    }
                    break;
                case KeyedCollectionChangeType.Cleared:
                    Reset();
                    break;
            }
        }

        private void AddMostFrequentlyUsedView(IView view)
        {
            if (view != null)
            {
                int currentIndex = mostFrequentlyUsedViews.IndexOf(view);

                if (currentIndex != -1)
                {
                    mostFrequentlyUsedViews.RemoveAt(currentIndex);
                }

                mostFrequentlyUsedViews.Insert(0, view);
                UpdateMostFrequentlyUsedList();
            }
        }

        private void UpdateMostFrequentlyUsedList()
        {
            while (mostFrequentlyUsedLimit < mostFrequentlyUsedViews.Count)
            {
                var oldestView = mostFrequentlyUsedViews[mostFrequentlyUsedViews.Count - 1];

                if (oldestView is ServerGroupView)
                {
                    var groupView = (ServerGroupView) oldestView;

                    if (groupView.View is UserView)
                    {
                        cachedServerGroupViews.Remove(((UserView)groupView.View).Id);
                    }
                    else if (groupView.View is int)
                    {
                        cachedServerGroupViews.Remove((int)groupView.View);
                    }
                }
                else if (oldestView is ServerViewContainer)
                {
                    cachedServerViews.Remove(((ServerViewContainer)oldestView).InstanceId);
                }

                mostFrequentlyUsedViews.RemoveAt(mostFrequentlyUsedViews.Count - 1);

                // dispose view should handle all cleanup in dispose
                oldestView.Dispose();
            }
        }

        public void CacheServerGroupView(object id, ServerGroupView view)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }

            cachedServerGroupViews.Add(id, view);
            AddMostFrequentlyUsedView(view);
        }

        public bool TryGetCachedServerGroupView(object id, out ServerGroupView view)
        {
            // Our assumption at this point is that if a cached view is being retrieved, 
            // it is going to be displayed. If that's the case, it should be moved to the 
            // head of the most frequently used list.

            bool isExisting = cachedServerGroupViews.TryGetValue(id, out view);

            if (isExisting)
            {
                AddMostFrequentlyUsedView(view);
            }

            return isExisting;
        }

        public void CacheServerView(int id, ServerViewContainer view)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }

            cachedServerViews.Add(id, view);
            AddMostFrequentlyUsedView(view);
        }

        public bool TryGetCachedServerView(int id, out ServerViewContainer view)
        {
            // Our assumption at this point is that if a cached view is being retrieved, 
            // it is going to be displayed. If that's the case, it should be moved to the 
            // head of the most frequently used list.

            bool isExisting = cachedServerViews.TryGetValue(id, out view);

            if (isExisting)
            {
                AddMostFrequentlyUsedView(view);
            }

            return isExisting;
        }

        public ServerViews GetServerViewForCachedServer(int id)
        {
            ServerViewContainer view = null;
            if(cachedServerViews.TryGetValue(id, out view))
            {
                if (view.CurrentServerView != null)
                    return view.CurrentServerView.Value;
            }
            return ServerViews.Overview;
        }
    }
}