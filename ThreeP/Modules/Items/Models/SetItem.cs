namespace ThreeP.Modules.Items;

public class SetItem
{
    public Guid SetId { get; set; }
    public  Set Set { get; set; }

    public Guid ItemId { get; set; }
    public  Item Item { get; set; }
}