namespace ThreeP.Modules.Trips;

public class TripItemStatus
{
    public Guid TripId { get; set; }  
    public Guid ItemId { get; set; }
    public bool IsPacked { get; set; }
    // NAV
    public Trip Trip { get; set; }
    public Item Item { get; set; }
}