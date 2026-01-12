using ManageUser.Infrastructure.DTOs;
using ManageUser.Infrastructure.Entites;
using Mapster;

namespace ManageUser.Api.Mapping;

public static class MapsterConfig
{
    public static void Register()
    {
        TypeAdapterConfig<User, UserDto>.NewConfig();
        TypeAdapterConfig<UserDto, User>.NewConfig();
    }
}