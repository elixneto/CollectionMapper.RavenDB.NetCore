using CollectionMapper.RavenDB.Attributes;

namespace ConsoleApp.Entities;

[RavenCollectionAssignedFrom<Fruit>]
public class Banana : Fruit
{
    public new string Name { get; set; } = "Banana";
    public int Weight { get; set; } = 150;
    
    public override string ToString()
    {
        return $"Banana: {Name}, Weight: {Weight}g";
    }
}