using FluentNHibernate.Mapping;
using TestAssemblyOnlyModels;

namespace TestAssemblyWithFluentMappings
{
    public class CompanyMap : ClassMap<Company>
    {
        public CompanyMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.Description);
        }
    }
}
