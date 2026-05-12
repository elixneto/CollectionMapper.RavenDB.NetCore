using CollectionMapper.RavenDB.Attributes;

namespace ConsoleApp.Entities;

[RavenCollection("AccounTX")]
internal class Account
{
    public string Name { get; set; }
    public decimal Ammount { get; set; }
}