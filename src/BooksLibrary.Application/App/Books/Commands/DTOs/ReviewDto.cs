using System;
using System.Collections.Generic;
using System.Linq;


namespace BooksLibrary.Application.App.Books.Commands.DTOs
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public string UserName { get; set; } // Afișăm numele, nu ID-ul
        public DateTime CreatedAt { get; set; } // Presupunând că entitatea de bază `Entity` are o dată de creare
    }
}
