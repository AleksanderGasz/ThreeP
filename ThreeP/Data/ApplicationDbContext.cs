namespace ThreeP.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options)
{
    public DbSet<Settings> Settings { get; set; }
    public DbSet<Item> Items { get; set; }

    public DbSet<Set> Sets { get; set; }
    // public DbSet<SetItem> SetItems { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // USER
        builder.Entity<ApplicationUser>()
            .HasOne(x => x.Settings)
            .WithOne(x => x.User)
            .HasForeignKey<Settings>(x => x.UserId);

        //ITEMS
        builder.Entity<Item>().HasIndex(x => x.Name);
        builder.Entity<Item>()
            .HasOne(x => x.User)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.UserId);

        builder.Entity<Item>()
            .HasIndex(x => x.Name);

        // SETS
        builder.Entity<Set>().HasIndex(s => s.Name); // ZMIANA: szybkie wyszukiwanie po nazwie
        builder.Entity<Set>().HasMany(x => x.Items).WithMany(x => x.Sets);

        /*// SETITEMS
        builder.Entity<SetItem>().HasKey(x=>new {x.SetId, x.ItemId});

        builder.Entity<SetItem>()
            .HasOne(x => x.Set)
            .WithMany(x => x.SetsItems)
            .HasForeignKey(x => x.SetId);

        builder.Entity<SetItem>()
            .HasOne(x => x.Item)
            .WithMany(x => x.SetItems)
            .HasForeignKey(x => x.ItemId);*/
    }
}