using EFCore.BulkExtensions;
using ManageUser.Infrastructure.DbContexts;
using ManageUser.Infrastructure.Entites;
using Microsoft.EntityFrameworkCore;

namespace ManageUser.Infrastructure.Repositories;

public interface IRepository
{
    Task AddAsync(User user);
    Task AddRangeAsync(IEnumerable<User> users);
    Task<List<User>> GetAllAsync();
}

public class Repository(AppDbContext context) : IRepository
{
    private readonly AppDbContext _context = context;

    public async Task AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task AddRangeAsync(IEnumerable<User> users)
    {
        //await _context.Users.AddRangeAsync(users);
        //await _context.SaveChangesAsync();

        var userList = users.Select(u =>
        {
            if (u.Id == Guid.Empty)
                u.Id = Guid.NewGuid();
            return u;
        }).ToList();

        // Use BulkInsert for high-performance inserts
        var bulkConfig = new BulkConfig
        {
            PreserveInsertOrder = true,  
            SetOutputIdentity = true,    
            BatchSize = 1000
        };

        await _context.BulkInsertAsync(userList, bulkConfig);
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _context.Users
            .AsNoTracking()
            .ToListAsync();
    }
}
