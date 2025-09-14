namespace ThreeP.Modules.Trips;

public class Trip : BaseModelWithUser
{
    [MaxLength(200)] public string Name { get; set; }
    [MaxLength(2000)] public string? Description { get; set; }
    public DateTime? TripDate { get; set; }

    // NAV
    public Guid? SetId { get; set; }
    public Set Set { get; set; }

    // public TodoList TodoList { get; set; }
    // public ICollection<TripItemStatus> TripItemsStatus { get; set; } = new HashSet<TripItemStatus>();


    public static TripDto ToDto(Trip? trip) => new ()
    {
        Id = trip.Id,
        Name = trip.Name,
        Description = trip.Description,
        UserId = trip.UserId,
    };
}

public class TripDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public Guid UserId { get; set; }
    
    public static Trip ToModel(TripDto dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name,
        Description = dto.Description,
        UserId = dto.UserId
    };
}