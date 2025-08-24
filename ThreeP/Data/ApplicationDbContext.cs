namespace ThreeP.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options)
{
    
    public DbSet<Settings> Settings { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // USER
        builder.Entity<ApplicationUser>()
            .HasOne(x => x.Settings)
            .WithOne(x => x.User)
            .HasForeignKey<Settings>(x => x.UserId);
    }

}