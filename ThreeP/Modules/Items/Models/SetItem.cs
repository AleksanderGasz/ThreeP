namespace ThreeP.Modules.Items;

public class SetItem
{
    public Guid? SetId { get; set; }
    public required Set Set { get; set; }

    public Guid? ItemId { get; set; }
    public required Item Item { get; set; }
}