﻿using System.ComponentModel.DataAnnotations;

namespace BookstoreAdmin.Model
{
    public class Author
    {
        [Key]
        public int AuthorId { get; set; }
        public required string AuthorName { get; set; }
        public required string AuthorLastName { get; set; }
        public string AuthorBirthCountry { get; set; }
        public DateTime AuthorBirthDate { get; set; }
        public DateTime? AuthorDeathDate { get; set; }

        public List<Book> Books { get; set; }
    }
}