using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FooMicroservice.Core.Entities;

public class FooEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class FooEntityConfiguration : IEntityTypeConfiguration<FooEntity>
{
    public void Configure(EntityTypeBuilder<FooEntity> builder)
    {
        builder.Property(e => e.Id)
            .ValueGeneratedNever();
    }
}