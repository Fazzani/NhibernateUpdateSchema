using FluentNHibernate.Mapping;
using TestAssemblyOnlyModels;

namespace TestAssemblyWithFluentMappings
{
    public sealed class AddressMap : ClassMap<Address>
    {
        public AddressMap()
        {
            Id(x => x.Id);
            Map(x => x.Line1);
            Map(x => x.Line2);
            Map(x => x.City);
            Map(x => x.State);
            Map(x => x.PostalCode);
        }
    }
}
