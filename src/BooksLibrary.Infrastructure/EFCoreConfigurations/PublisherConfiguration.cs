
using BooksLibrary.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BooksLibrary.Infrastructure.EFCoreConfigurations
{
    public class PublisherConfiguration : IEntityTypeConfiguration<Publisher>
    {
        public void Configure(EntityTypeBuilder<Publisher> builder)
        {
            builder.Property(p => p.FullName).IsRequired().HasMaxLength(50);
            builder.Property(p => p.Address).IsRequired().HasMaxLength(100);

            builder.HasMany(p => p.Books)
                .WithOne(b => b.Publisher)
                .HasForeignKey(b => b.PublisherId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
