using System.Data.Common;
using CodeCase.Repository.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Respawn;
using Testcontainers.MsSql;

namespace CodeCase.IntegratedTest;

public class DatabaseFixture : IAsyncLifetime
{
    private MsSqlContainer _container;
    public DbConnection DbConnection;
    public Respawner Respawner;

    public async Task InitializeAsync()
    {
        _container = new MsSqlBuilder().WithImage("mcr.microsoft.com/mssql/server:2022-latest").Build();
        await _container.StartAsync();
        var connectionStringWithMars = $"{_container.GetConnectionString()};MultipleActiveResultSets=true";
        DbConnection = new SqlConnection(connectionStringWithMars);
        var dbContextOptions = new DbContextOptionsBuilder<DatabaseContext>().UseSqlServer(DbConnection).Options;
        await using var dbContext = new DatabaseContext(dbContextOptions);
        await dbContext.Database.EnsureCreatedAsync();
        await dbContext.Database.OpenConnectionAsync();
        Respawner = await Respawner.CreateAsync(DbConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.SqlServer,
            WithReseed = true,
        });
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}