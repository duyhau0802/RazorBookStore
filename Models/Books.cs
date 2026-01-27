using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RazorBookStore.Models
{
    public class Books
    {
        public int Id { get; set; }
        [Required]
        [DisplayName("Book Title")]
        public string Title { get; set; }

        [DisplayName("Book Descripton")]
        public string Descripton { get; set; }
        [Required]
        public string Author { get; set; }

        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

    }
}
