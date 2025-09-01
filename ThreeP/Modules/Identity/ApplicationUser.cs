namespace Mac.Modules.Identity;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser<Guid>
{
    // NAV
    public UserSettings.Settings Settings { get; set; }
    public ICollection<Item> Items { get; set; }
}