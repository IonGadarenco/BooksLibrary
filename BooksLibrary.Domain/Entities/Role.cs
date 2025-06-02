

namespace BooksLibrary.Domain.Models
{
    public class Role : Entity
    {
        public string RoleName { get; set; }
        public ICollection<User> Users { get; set; } = new List<User>();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
