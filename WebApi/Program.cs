using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System.Security.Cryptography.X509Certificates;
using WebApi;

var builder = WebApplication.CreateBuilder(args);

var con = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(configure =>
{
    configure.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var redisConnection = builder.Configuration.GetConnectionString("RedisConnection");
//if(builder.Environment.IsProduction()) redisConnection = builder.Configuration.GetConnectionString("RedisConnection");
/*builder.Services.AddStackExchangeRedisCache(options =>
{
    //options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
    options.InstanceName = "Redis";
    options.ConfigurationOptions = new ConfigurationOptions
    {
         //EndPoints ={$"{builder.Configuration.GetConnectionString("RedisConnection")}"},
         EndPoints = { { "localhost", 6379} },
         AbortOnConnectFail = false,
         Ssl = true
    };
});*/
/*var configurationOptions = new ConfigurationOptions
{
    EndPoints = { { "localhost", 6379 } },
    AbortOnConnectFail = false,
    Ssl = true,
};
var multiplexer = ConnectionMultiplexer.Connect(configurationOptions);
builder.Services.AddSingleton<IConnectionMultiplexer>(multiplexer);*/

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddDistributedMemoryCache();
builder.Services.AddScoped<IStudentService, StudentService>();
//builder.Services.AddScoped<IRedisCacheService, RedisCacheService>();
builder.Services.AddScoped<ICacheService,CacheService>(x=> new CacheService(redisConnection));

var app = builder.Build();

app.Logger.LogInformation(new string('=',120));
app.Logger.LogError("Redis : {0}", redisConnection);
app.Logger.LogError("Database Connection: {0}", con);
app.Logger.LogInformation(new string('=', 120));

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
