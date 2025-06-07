using System.ComponentModel.DataAnnotations;

namespace BooksLibrary.API
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        [StringLength(50)]
        public string Author { get; set; }

    }
}
