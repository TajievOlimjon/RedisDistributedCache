using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using WebApi;

var builder = WebApplication.CreateBuilder(args);

var con = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(configure =>
{
    configure.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var redis = builder.Configuration.GetConnectionString("RedisConnection");

/*builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
    options.InstanceName = "Redis";
});*/

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddScoped<IStudentService, StudentService>();
//builder.Services.AddScoped<IRedisCacheService, RedisCacheService>();
builder.Services.AddScoped<ICacheService, CacheService>();

var app = builder.Build();

try
{
    await using var scope = app.Services.CreateAsyncScope();
    var dataContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var log = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
    await dataContext.Database.MigrateAsync();
    await scope.DisposeAsync();
}
catch (Exception ex)
{
    app.Logger.LogError("Error: {ex.Message}", ex.Message);
}

if (app.Environment.IsDevelopment()||app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
