using System.Collections.Generic;

namespace TestAssemblyWithFluentAddressBook
{
    public class Contact
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual List<Phone> Phones { get; set; }
    }
}
