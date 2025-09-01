namespace ThreeP.Modules.Items;

public class SetItem
{
    public Guid  SetId { get; set; }
    public Guid  ItemId { get; set; }
    
    // NAV
    public required Set Set { get; set; } 
    public required Item Item { get; set; }
}