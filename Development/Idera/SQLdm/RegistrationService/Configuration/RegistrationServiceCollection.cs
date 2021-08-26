//------------------------------------------------------------------------------
// <copyright file="RegistrationServiceCollection.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Configuration;

namespace Idera.SQLdm.RegistrationService.Configuration
{
    public class RegistrationServiceCollection : ConfigurationElementCollection
    {

        protected override string ElementName
        {
            get
            {
                return "RegistrationService";
            }
        }

        public new RegistrationServiceElement this[string instanceName]
        {
            get { return (RegistrationServiceElement)BaseGet(instanceName); }
            set
            {
                if (BaseGet(instanceName) != null)
                    BaseRemove(instanceName);
                BaseAdd(value);
            }
        }


        public RegistrationServiceElement this[int index]
        {
            get { return (RegistrationServiceElement)BaseGet(index); }
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
            return new RegistrationServiceElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((RegistrationServiceElement)element).InstanceName;
        }

        public void Add(RegistrationServiceElement element)
        {
            BaseAdd(element);
        }

        public void Clear()
        {
            BaseClear();
        }

        public void Remove(RegistrationServiceElement element)
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
