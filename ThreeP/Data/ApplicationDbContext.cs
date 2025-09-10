namespace ThreeP.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options)
{
    public DbSet<Settings> Settings { get; set; }
    public DbSet<Item> Items { get; set; }

    public DbSet<Set> Sets { get; set; }

     // public DbSet<SetItem> SetItems { get; set; }
    public DbSet<Trip> Trips { get; set; }
    // public DbSet<TodoList> TodoLists { get; set; }
    // public DbSet<TodoItem> TodoItems { get; set; }


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
        // builder.Entity<Set>().HasMany(x => x.Items).WithMany(x => x.Sets);

        // SETITEMS
        /*builder.Entity<SetItem>().HasKey(x=>new {x.SetId, x.ItemId});

        builder.Entity<SetItem>()
            .HasOne(x => x.Set)
            .WithMany(x => x.SetItems)
            .HasForeignKey(x => x.SetId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<SetItem>()
            .HasOne(x => x.Item)
            .WithMany(x => x.SetItems)
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Cascade);*/

        // TRIPS
        builder.Entity<Trip>().HasOne(x => x.Set).WithMany(x => x.Trips).HasForeignKey(x => x.SetId);
        // builder.Entity<Trip>().HasOne(x => x.TodoList).WithOne(x => x.Trip).HasForeignKey<TodoList>(x => x.TripId);
        
        // TODOLIST
        /*builder.Entity<TodoList>()
            .HasMany(x => x.TodoItems)
            .WithOne(x => x.TodoList)
            .HasForeignKey(x => x.TodoListId);*/
        
        // TRIPITEMSTATUS
        /*builder.Entity<TripItemStatus>()
            .HasKey(x => new {  x.TripId, x.ItemId });
        
        builder.Entity<TripItemStatus>().HasOne(x=>x.Trip).WithMany(x=>x.TripItemsStatus).HasForeignKey(x=>x.TripId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<TripItemStatus>().HasOne(x=>x.Item).WithMany().HasForeignKey(x=>x.ItemId).OnDelete(DeleteBehavior.Restrict);*/
    }
}