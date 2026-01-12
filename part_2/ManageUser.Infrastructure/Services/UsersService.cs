using Bogus;
using ManageUser.Infrastructure.DTOs;
using ManageUser.Infrastructure.Entites;
using ManageUser.Infrastructure.Repositories;
using Mapster;
using Microsoft.Extensions.Caching.Distributed;

namespace ManageUser.Infrastructure.Services;

public interface IUsersService
{
    Task CreateBulkUsersAsync(int count);
    Task CreateUserAsync(UserDto userDto);
    Task<List<UserDto>> FetchUsersAsync();
}

public class UsersService : IUsersService
{
    private readonly IRepository _repository;
    private readonly IDistributedCache _cache;

    public UsersService(IRepository repository, IDistributedCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task CreateUserAsync(UserDto userDto)
    {
        userDto.TimeStamp = DateTime.UtcNow;
        await _repository.AddAsync(userDto.Adapt<User>());
    }

    public async Task CreateBulkUsersAsync(int count)
    {
        var faker = new Faker<User>()
            .RuleFor(u => u.Name, f => f.Name.FullName())
            .RuleFor(u => u.Age, f => f.Random.Int(18, 80))
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.TimeStamp, f => DateTime.UtcNow);

        var users = faker.Generate(count);

        await _repository.AddRangeAsync(users);
    }

    public async Task<List<UserDto>> FetchUsersAsync()
    {
        var user = await _repository.GetAllAsync();

        return user.Adapt<List<UserDto>>();
    }
}
