
using BooksLibrary.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BooksLibrary.Infrastructure.EFCoreConfigurations
{
    public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(EntityTypeBuilder<Reservation> builder)
        {
            builder.Property(r => r.ReservedAt).IsRequired();
            builder.Property(r => r.ExpiresAt).IsRequired();
            builder.Property(r => r.IsActive).IsRequired();

            builder.HasOne(r => r.User)
                .WithMany(u => u.Reservations)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.Book)
                .WithOne(b => b.Reservation)
                .HasForeignKey<Reservation>(r => r.BookId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
