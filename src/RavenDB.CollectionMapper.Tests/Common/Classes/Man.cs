namespace RavenDB.CollectionMapper.Tests.Common.Classes
{
    class Man : Person
    {
        public override string Name { get; }
        public override int Age { get; }
        public override string Gender => "M";

        public Man(string name, int age)
        {
            Name = name;
            Age = age;
        }
    }
}
