using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Infragistics.Windows.Tiles;

namespace Idera.SQLdm.DesktopClient.Controls.ServerSummary
{
    public class ChartComponents : DataTemplateSelector
    {
        private static readonly ResourceDictionary Registry;
        private static readonly List<string> ComponentNames;

        static ChartComponents()
        {
            Registry = new ResourceDictionary()
                { Source = new Uri("/SQLdmDesktopClient;component/controls/serversummary/dashboardregistry.xaml", UriKind.Relative) };
            
            ComponentNames = new List<string>();
            foreach (DictionaryEntry item in Registry)
            {
                if (item.Value is DashboardComponentMetaData)
                {   // ensure key name is set in component metadata objects
                    ((DashboardComponentMetaData)item.Value).Key = item.Key.ToString();
                    // keep up with a list of component names
                    ComponentNames.Add(item.Key.ToString());
                }
            }
        }

        public static object GetResource(object key)
        {
            return Registry.Contains(key) ? Registry[key] : null;
        }

        public static List<DashboardComponentMetaData> GetRegisteredComponents()
        {
            return ComponentNames.Select(name => (DashboardComponentMetaData) Registry[name]).ToList();
        }
         
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;
            if (element != null && !(element.TemplatedParent is Tile))
            {
                DataTemplate template = Registry["defaultTileHeaderTemplate"] as DataTemplate;
                return template;
            }

            var instance = item as DashboardComponent;
            if (instance == null) return base.SelectTemplate(item, container); 
            if (String.IsNullOrEmpty(instance.ComponentKey) || !Registry.Contains(instance.ComponentKey))
                return Registry["EmptyTemplate"] as DataTemplate;
            var metadata = Registry[instance.ComponentKey] as DashboardComponentMetaData;
            if (metadata == null) return base.SelectTemplate(item, container);
            var result = Registry[metadata.TemplateKey] as DataTemplate;
            return  result ?? base.SelectTemplate(item, container);
        }
    }
}
