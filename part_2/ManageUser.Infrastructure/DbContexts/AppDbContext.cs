using ManageUser.Infrastructure.Entites;
using Microsoft.EntityFrameworkCore;

namespace ManageUser.Infrastructure.DbContexts;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();
}