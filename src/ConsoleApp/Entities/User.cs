namespace ConsoleApp.Entities;

internal class User
{
    public string Id { get; set; }
    private string OtherStringId { get; set; } = Guid.NewGuid().ToString();
}