using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NoviVovi.Infrastructure.Novels;

public class NovelMap : IEntityTypeConfiguration<NovelDbModel>
{
    public void Configure(EntityTypeBuilder<NovelDbModel> builder)
    {
        builder.ToTable("Novels");
        builder.HasKey(n => n.Id);
        builder.Property(n => n.Title).IsRequired().HasMaxLength(200);

        builder.HasMany(n => n.Slides)
            .WithOne()
            .HasForeignKey(s => s.NovelId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}