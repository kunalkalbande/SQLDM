using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Idera.SQLdm.Service.Configuration
{
    public class RestServiceCollection : ConfigurationElementCollection
    {

        protected override string ElementName
        {
            get
            {
                return "RestService";
            }
        }

        public new RestServiceElement this[string instanceName]
        {
            get { return (RestServiceElement)BaseGet(instanceName); }
            set
            {
                if (BaseGet(instanceName) != null)
                    BaseRemove(instanceName);
                BaseAdd(value);
            }
        }


        public RestServiceElement this[int index]
        {
            get { return (RestServiceElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);
                BaseAdd(index, value);
            }
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMap;
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new RestServiceElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((RestServiceElement)element).InstanceName;
        }

        public void Add(RestServiceElement element)
        {
            BaseAdd(element);
        }

        public void Clear()
        {
            BaseClear();
        }

        public void Remove(RestServiceElement element)
        {
            BaseRemove(element.InstanceName);
        }

        public void Remove(string instanceName)
        {
            BaseRemove(instanceName);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

    }
}
