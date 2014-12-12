using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestAssemblyOnlyModels
{
    public class Company
    {
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
    }
}
