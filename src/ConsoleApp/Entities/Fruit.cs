namespace ConsoleApp.Entities;

public class Fruit
{
    public string Id { get; set; }
    public string Name { get; set; } = "Generic Fruit";
    
    public override string ToString()
    {
        return $"Fruit: {Name}";
    }
}