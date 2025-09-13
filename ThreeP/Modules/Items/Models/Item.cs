namespace ThreeP.Modules.Items;

public class Item : BaseModelWithUser
{
    [MaxLength(200)] public string Name { get; set; }
    [MaxLength(2000)] public string? Description { get; set; }
    public float? Weight { get; set; }

    //NAV
    // public ICollection<SetItem> SetItems { get; set; } = new HashSet<SetItem>();
    public ICollection<Set> Sets { get; set; } = new HashSet<Set>();
    
    public static ItemDto ToDto(Item item) => new ()
    {
        Id = item.Id,
        Name = item.Name,
        Description = item.Description,
        Weight = item.Weight,
        UserId = item.UserId
    };
}

public class ItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public float? Weight { get; set; }
    public Guid UserId { get; set; }



}