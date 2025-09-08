/*namespace ThreeP.Modules.Todo;

public class TodoList : BaseModelWithUser
{
    [MaxLength(200)] public string Name { get; set; } = string.Empty;
    [MaxLength(2000)] public string? Description { get; set; }

    // NAV
    public ICollection<TodoItem> TodoItems { get; set; } = [];
    // public Guid? TripId { get; set; }

    public Guid TripId { get; set; }
    public Trip Trip { get; set; }
}*/