namespace Mac.Modules.Common;

public class BaseModel
{
    public Guid? Id { get; set; } = Guid.CreateVersion7();
    public bool IsActive { get; set; } = true;

    public DateTime CreateDate { get; set; } = DateTime.Now;
    public DateTime? ModifyDate { get; set; }
}

public class BaseModelWithUser : BaseModel
{
    public Guid? UserId { get; set; }
    public ApplicationUser? User { get; set; }
}