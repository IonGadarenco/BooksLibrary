
using BooksLibrary.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BooksLibrary.Infrastructure.EFCoreConfigurations
{
    public class AuthorConfiguration : IEntityTypeConfiguration<Author>
    {
        public void Configure(EntityTypeBuilder<Author> builder)
        {
            builder.Property(a => a.FullName).IsRequired().HasMaxLength(50);

            builder.HasMany(a => a.Books)
                   .WithMany(b => b.Authors);
        }
    }
}
