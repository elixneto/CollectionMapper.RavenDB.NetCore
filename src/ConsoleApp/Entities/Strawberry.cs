using CollectionMapper.RavenDB.Attributes;

namespace ConsoleApp.Entities;

[RavenCollectionAssignedFrom<Fruit>]
public class Strawberry : Fruit
{
    public new string Name { get; set; } = "Strawberry Inherited";
    public string Color { get; set; } = "Red";
    public string Size { get; set; } = "Medium";

    public override string ToString()
    {
        return $"Strawberry: {Name}, Color: {Color}, Size: {Size}";
    }
}