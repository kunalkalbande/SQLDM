using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Installer_form_application
{
    [Serializable]
    [CollectionDataContract]
    public class Products : List<Product>
    {
        public Products() { }
        public Products(IEnumerable<Product> products) : base(products) { }
    }

    [Serializable]
    [DataContract]
    public class Product
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2, EmitDefaultValue = false)]
        public string Name { get; set; }

        [DataMember(Order = 3)]
        public string InstanceName { get; set; }

        [DataMember(Order = 4)]
        public string ShortName { get; set; }

        [DataMember(Order = 5)]
        public string Version { get; set; }

        [DataMember(Order = 6)]
        public string RegisteringUser { get; set; }

        [DataMember(Order = 7)]
        public string Location { get; set; }

        [DataMember(Order = 8)]
        public string Status { get; set; }

        [DataMember(Order = 9)]
        public DateTime? RegistrationDateTime { get; set; }

        [DataMember(Order = 10)]
        public string ConnectionUser { get; set; }

        [DataMember(Order = 11)]
        public string RestURL { get; set; }

        [DataMember(Order = 12)]
        public string WebURL { get; set; }

        [DataMember(Order = 13)]
        public string JarFile { get; set; }

        [DataMember(Order = 14)]
        public string Description { get; set; }

        [DataMember(Order = 15)]
        public string DefaultPage { get; set; }

        [DataMember(Order = 16)]
        public Boolean IsLoadable { get; set; }

        [DataMember(Order = 17)]
        public string ConnectionPassword { get; set; }

        [DataMember(Order = 18)]
        public string RestFile { get; set; }

    }
}
