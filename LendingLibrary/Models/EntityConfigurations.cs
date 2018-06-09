using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace LendingLibrary.Models
{
    public class ApplicationUserConfiguration : EntityTypeConfiguration<ApplicationUser>
    {
        public ApplicationUserConfiguration()
        {
            // Many to many between Friendship and ApplicationUser
            HasMany(u => u.Friendships).WithRequired(f => f.User).HasForeignKey(f => f.UserId).WillCascadeOnDelete(false);
            HasMany(u => u.Users).WithRequired(f => f.Friend).HasForeignKey(f => f.FriendId).WillCascadeOnDelete(false);
        }
    }

    public class BookConfiguration : EntityTypeConfiguration<Book>
    {
        public BookConfiguration()
        {
            // Book object required fields
            Property(b => b.Title).IsRequired();
            Property(b => b.Author).IsRequired();
        }
    }

    public class FriendshipConfiguration : EntityTypeConfiguration<Friendship>
    {
        public FriendshipConfiguration()
        {
            HasKey(u => new { u.UserId, u.FriendId });
            Property(u => u.UserId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(u => u.FriendId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
        }
    }
}
