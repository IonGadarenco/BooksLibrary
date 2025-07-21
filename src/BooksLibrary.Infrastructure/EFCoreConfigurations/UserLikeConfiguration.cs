

using BooksLibrary.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace BooksLibrary.Infrastructure.EFCoreConfigurations
{
    public class UserLikeConfiguration : IEntityTypeConfiguration<UserLike>
    {
        public void Configure(EntityTypeBuilder<UserLike> builder)
        {
            
            builder.HasKey(ul => new { ul.UserId, ul.BookId });

            builder.HasOne(ul => ul.User)
                .WithMany(u => u.Likes)
                .HasForeignKey(ul => ul.UserId)
                .OnDelete(DeleteBehavior.Cascade); 

            builder.HasOne(ul => ul.Book)
                .WithMany(b => b.Likes)
                .HasForeignKey(ul => ul.BookId)
                .OnDelete(DeleteBehavior.Cascade); 
        }
    }
}
