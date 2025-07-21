
using BooksLibrary.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BooksLibrary.Infrastructure.EFCoreConfigurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.Property(c => c.FullName).IsRequired().HasMaxLength(50);

            builder.HasMany(c => c.Books)
                   .WithMany(b => b.Categories);
        }
    }
}
