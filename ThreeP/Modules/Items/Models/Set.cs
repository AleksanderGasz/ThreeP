namespace ThreeP.Modules.Items;

public class Set:BaseModelWithUser
{
    [MaxLength(200)]
    public string? Name { get; set; }
    [MaxLength(2000)]
    public string? Description { get; set; }
    public float? Weight { get; set; }
    
    // NAV
    // public ICollection<SetItem> SetsItems { get; set; } = new HashSet<SetItem>();
    [JsonIgnore]
    public ICollection<Item> Items { get; set; } = new HashSet<Item>();

    public ICollection<Trip> Trips { get; set; }= new HashSet<Trip>();
    

}