using StackExchange.Redis;
using Testcontainers.Redis;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace CodeCase.IntegratedTest;

public class CacheFixture : IAsyncLifetime
{
    private readonly RedisContainer _redisContainer;
    public string ConnectionString { get; set; }
    public ConnectionMultiplexer ConnectionMultiplexer { get; set; }

    public CacheFixture()
    {
        _redisContainer = new RedisBuilder()
        .WithImage("redis:7.0")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _redisContainer.StartAsync();
        ConnectionString = _redisContainer.GetConnectionString();
        ConnectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(ConnectionString);
    }

    public async Task DisposeAsync()
    {
        if (_redisContainer != null)
        {
            await _redisContainer.StopAsync();
        }
    }
    public async Task ResetStateAsync()
    {
        var db = ConnectionMultiplexer.GetDatabase();
        var endPoint = ConnectionMultiplexer.GetEndPoints().First();
        var server = ConnectionMultiplexer.GetServer(endPoint);

        var keys = server.KeysAsync();

        await foreach (var key in keys)
        {
            await db.KeyDeleteAsync(key);
        }
    }
}