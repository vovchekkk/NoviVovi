using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NoviVovi.Infrastructure.Novels.Persistence;

public class NovelConfiguration : IEntityTypeConfiguration<NovelDbModel>
{
    public void Configure(EntityTypeBuilder<NovelDbModel> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).IsRequired().HasMaxLength(200);
        // Тут же настраиваете связи (HasMany, HasOne)
    }
}