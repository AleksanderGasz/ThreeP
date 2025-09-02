namespace ThreeP.Modules.Items;

public class Item:BaseModelWithUser
{
    [MaxLength(200)]
    public string Name { get; set; }
    [MaxLength(2000)]
    public string? Description { get; set; }
    public float? Weight { get; set; }
    
    //NAV
    // public ICollection<SetItem> SetItems { get; set; } = new HashSet<SetItem>();
    public ICollection<Set> Sets { get; set; }= new HashSet<Set>();
}