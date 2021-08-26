//------------------------------------------------------------------------------
// <copyright file="CollectionServiceCollection.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Configuration;

namespace Idera.SQLdm.CollectionService.Configuration
{
    public class CollectionServiceCollection : ConfigurationElementCollection
    {

        protected override string ElementName
        {
            get
            {
                return "CollectionService";
            }
        }

        public new CollectionServiceElement this[string instanceName]
        {
            get { return (CollectionServiceElement)BaseGet(instanceName); }
            set
            {
                if (BaseGet(instanceName) != null)
                    BaseRemove(instanceName);
                BaseAdd(value);
            }
        }


        public CollectionServiceElement this[int index]
        {
            get { return (CollectionServiceElement)BaseGet(index); }
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
            return new CollectionServiceElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CollectionServiceElement)element).InstanceName;
        }

        public void Add(CollectionServiceElement element)
        {
            BaseAdd(element);
        }

        public void Clear()
        {
            BaseClear();
        }

        public void Remove(CollectionServiceElement element)
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
