using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestAssemblyWithModels
{
    public class Person
    {
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
        public virtual int Age { get; set; }
        public virtual DateTime DateOfBirth { get; set; }
    }
}
