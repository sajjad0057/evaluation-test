using ManageUser.Infrastructure.Entites;
using Microsoft.EntityFrameworkCore;

namespace ManageUser.Infrastructure.DbContexts;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
}