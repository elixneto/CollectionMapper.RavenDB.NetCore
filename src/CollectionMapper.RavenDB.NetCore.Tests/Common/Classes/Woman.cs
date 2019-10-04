namespace CollectionMapper.RavenDB.NetCore.Tests.Common.Classes
{
    class Woman : Person
    {
        public override string Name { get; }
        public override int Age { get; }
        public override string Gender => "F";

        public Woman(string name, int age)
        {
            Name = name;
            Age = age;
        }
    }
}
