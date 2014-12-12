namespace TestAssemblyWithFluentAddressBook
{
    public class Phone
    {
        public virtual int Id { get; set; }
        public virtual string Number { get; set; }
        public virtual Contact Contact { get; set; }
    }
}
