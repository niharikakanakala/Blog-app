using System;
using System.ComponentModel.DataAnnotations;

namespace Blog.WebAPI.Models
{
    public class Article
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public string Author { get; set; }

        public DateTime PublicationDate { get; set; } = DateTime.Now;
       
        [Required]
        public string Tags { get; set; }  // Comma-separated tags
    }
}
