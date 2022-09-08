using Microsoft.EntityFrameworkCore;
using FooMicroservice.Core.Entities;

namespace FooMicroservice.Core;

public class FooContext : DbContext
{
    public DbSet<FooEntity> Foos => Set<FooEntity>();

    public FooContext(DbContextOptions<FooContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FooContext).Assembly);
    }
}