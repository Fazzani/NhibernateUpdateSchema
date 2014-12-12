using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;

namespace TestAssemblyWithFluentAddressBook
{
    public class PhoneMap : ClassMap<Phone>
    {
        public PhoneMap()
        {
            Id(x => x.Id);
            Map(x => x.Number);
            References(x => x.Contact)
                .Not.Nullable()
                .Cascade.SaveUpdate();
        }
    }
}
