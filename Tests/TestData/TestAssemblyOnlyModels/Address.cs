using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestAssemblyOnlyModels
{
    public class Address
    {
        public virtual int Id { get; set; }
        public virtual string Line1 { get; set; }
        public virtual string Line2 { get; set; }
        public virtual string City { get; set; }
        public virtual string State { get; set; }
        public virtual string PostalCode { get; set; }
    }
}
