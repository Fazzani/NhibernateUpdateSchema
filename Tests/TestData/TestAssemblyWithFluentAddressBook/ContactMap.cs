using FluentNHibernate.Mapping;

namespace TestAssemblyWithFluentAddressBook
{
    public class ContactMap : ClassMap<Contact>
    {
        public ContactMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);
            HasMany(x => x.Phones)
                .Cascade.AllDeleteOrphan()
                .Inverse();
        }
    }
}
