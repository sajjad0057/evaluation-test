using Bogus;
using ManageUser.Infrastructure.DTOs;
using ManageUser.Infrastructure.Entites;
using ManageUser.Infrastructure.Helpers;
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

public class UsersService(IRepository repository, IDistributedCache cache) : IUsersService
{
    private readonly IRepository _repository = repository;
    private readonly IDistributedCache _cache = cache;

    public async Task CreateUserAsync(UserDto userDto)
    {
        userDto.TimeStamp = DateTime.UtcNow;
        await _repository.AddAsync(userDto.Adapt<User>());

        await _cache.RemoveAsync("users_cache");
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

        await _cache.RemoveAsync("users_cache");
    }

    public async Task<List<UserDto>> FetchUsersAsync()
    {
        const string cacheKey = "users_cache";

        var cachedBytes = await _cache.GetAsync(cacheKey);

        if (cachedBytes != null)
        {
            return CacheCompressionHelper.Decompress<List<UserDto>>(cachedBytes);
        }

        var users = await _repository.GetAllAsync();
        var userDtos = users.Adapt<List<UserDto>>();

        var compressedData = CacheCompressionHelper.Compress(userDtos);
        await _cache.SetAsync(cacheKey, compressedData);

        return userDtos;
    }
}
