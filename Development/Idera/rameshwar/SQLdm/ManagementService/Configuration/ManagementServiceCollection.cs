//------------------------------------------------------------------------------
// <copyright file="ManagementServiceCollection.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Configuration;

namespace Idera.SQLdm.ManagementService.Configuration
{
    public class ManagementServiceCollection : ConfigurationElementCollection
    {

        protected override string ElementName
        {
            get
            {
                return "ManagementService";
            }
        }

        public new ManagementServiceElement this[string instanceName]
        {
            get { return (ManagementServiceElement)BaseGet(instanceName); }
            set
            {
                if (BaseGet(instanceName) != null)
                    BaseRemove(instanceName);
                BaseAdd(value);
            }
        }


        public ManagementServiceElement this[int index]
        {
            get { return (ManagementServiceElement)BaseGet(index); }
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
            return new ManagementServiceElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ManagementServiceElement)element).InstanceName;
        }

        public void Add(ManagementServiceElement element)
        {
            BaseAdd(element);
        }

        public void Clear()
        {
            BaseClear();
        }

        public void Remove(ManagementServiceElement element)
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
