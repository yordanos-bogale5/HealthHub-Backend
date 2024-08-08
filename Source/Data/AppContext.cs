using Microsoft.EntityFrameworkCore;

public class AppContext : DbContext
{
  public AppContext(DbContextOptions options) : base(options) { }


  public DbSet<User> Users { get; set; }

}