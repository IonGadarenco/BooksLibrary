
using BooksLibrary.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BooksLibrary.Infrastructure.EFCoreConfigurations
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.Property(b => b.Title).IsRequired().HasMaxLength(140);
            builder.Property(b => b.Description);
            builder.Property(b => b.ISBN).IsRequired(false).HasMaxLength(20);
            builder.Property(b => b.TotalCopies).IsRequired();

            builder.HasIndex(b => b.ISBN).IsUnique();

            builder.HasMany(b => b.Reviews)
                .WithOne(r => r.Book)
                .HasForeignKey(r => r.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(b => b.Publisher)
                .WithMany(p => p.Books)
                .HasForeignKey(b => b.PublisherId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(b => b.Reservations)
                .WithOne(r => r.Book)
                .HasForeignKey(r => r.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(b => b.Loans)
                .WithOne(l => l.Book)
                .HasForeignKey(l => l.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(b => b.Authors)
                .WithMany(a => a.Books);

            builder.HasMany(b => b.Categories)
                .WithMany(c => c.Books);

            builder.HasMany(b => b.Likes)
                .WithOne(ul => ul.Book)
                .HasForeignKey(ul => ul.BookId);
        }
    }
}
