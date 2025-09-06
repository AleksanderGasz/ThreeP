namespace ThreeP.Modules.Trips;

public class Trip : BaseModelWithUser
{
    [MaxLength(200)] public string Name { get; set; }
    [MaxLength(2000)] public string? Description { get; set; }

    // NAV
    public Guid? SetId { get; set; }
    public Set Set { get; set; }
}