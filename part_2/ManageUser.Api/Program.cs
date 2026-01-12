using System.Reflection;
using ManageUser.Api.Mapping;
using ManageUser.Api.Utils;
using ManageUser.Infrastructure.DbContexts;
using ManageUser.Infrastructure.Repositories;
using ManageUser.Infrastructure.Services;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(
    (ctx, lc) =>
        lc
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Debug)
            .Enrich.FromLogContext()
            .ReadFrom.Configuration(builder.Configuration)
);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

var assemblyName = Assembly.GetExecutingAssembly().FullName;

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(connectionString,
        b => b.MigrationsAssembly(assemblyName));
});


builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetValue<string>("Redis:Configuration") ?? "localhost:6379";
});

// Add services to the container.

MapsterConfig.Register();
builder.Services.AddSingleton(TypeAdapterConfig.GlobalSettings);
builder.Services.AddScoped<IMapper, ServiceMapper>();

builder.Services.AddScoped<IRepository, Repository>();
builder.Services.AddScoped<IUsersService, UsersService>();


#region Configure_Cors

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AnyOrigin",
        builder =>
        {
            builder
                .SetIsOriginAllowed(origin =>
                {
                    return true;
                })
                .AllowAnyMethod()
                .AllowCredentials()
                .AllowAnyHeader();
        }
    );
});

#endregion


builder.Services.AddControllers();

// Configuration for invalid model state response customization

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = ApiResults<object>.InvalidModelStateResponse;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

try
{
    var app = builder.Build();

    // Auto Migrate Database

    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate(); 
    }

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        //app.MapOpenApi();
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    //// enabling cors with policy
    app.UseCors("AnyOrigin");

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "ManageUser.Api Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
