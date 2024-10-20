using CodeCase.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace CodeCase.Repository.Data;

public class DatabaseContext : DbContext, IDatabaseContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    public DbSet<Entity> Entity { get; set; }
    public DbSet<AnotherEntity> AnotherEntity { get; set; }
    public DbSet<EntityAnotherEntity> EntityAnotherEntity { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Entity>()
        .HasMany(e => e.AnotherEntity)
        .WithMany(e => e.Entity)
        .UsingEntity<EntityAnotherEntity>(
            l => l.HasOne<AnotherEntity>().WithMany(e => e.EntityAnotherEntity).OnDelete(DeleteBehavior.Restrict),
            r => r.HasOne<Entity>().WithMany(e => e.EntityAnotherEntity).OnDelete(DeleteBehavior.Restrict));
    }
}
