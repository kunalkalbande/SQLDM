using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.Common.CWFDataContracts
{
    public class Product
    {

        public int Id { get; set; }


        public string Name { get; set; }


        public string InstanceName { get; set; }


        public string ShortName { get; set; }


        public string Version { get; set; }


        public string RegisteringUser { get; set; }


        public string Location { get; set; }


        public string Status { get; set; }


        public DateTime? RegistrationDateTime { get; set; }


        public string ConnectionUser { get; set; }


        public string RestURL { get; set; }


        public string WebURL { get; set; }


        public string JarFile { get; set; }


        public string Description { get; set; }


        public string DefaultPage { get; set; }


        public Boolean IsLoadable { get; set; }


        public string ConnectionPassword { get; set; }


        public string RestFile { get; set; }

    }

    public class Products : List<Product>
    {
        public Products() { }
        public Products(IEnumerable<Product> products) : base(products) { }
    }
}
