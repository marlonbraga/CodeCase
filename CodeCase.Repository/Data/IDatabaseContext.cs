using CodeCase.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace CodeCase.Repository.Data
{
    public interface IDatabaseContext
    {
        DbSet<Entity> Entity { get; set; }
        DbSet<AnotherEntity> AnotherEntity { get; set; }
    }
}
