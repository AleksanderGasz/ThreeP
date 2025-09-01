namespace ThreeP.Modules.Items;

public class Set:BaseModelWithUser
{
    [MaxLength(200)]
    public string? Name { get; set; }
    
    // NAV
    public ICollection<SetItem> SetsItems { get; set; } = new HashSet<SetItem>();

}