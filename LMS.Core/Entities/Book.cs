using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LMS.Core.Entities
{
    [Index(nameof(IsDeleted),nameof(IsAvailable))]
    public class Book
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50,ErrorMessage ="Name can not exceed 50 characters!")]
        public string Name { get; set; }
        [StringLength(500,ErrorMessage ="Description can not exceed 500 characters!")]
        public string? Description { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Author Name can not exceed 50 characters!")]
        public string Author { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Title can not exceed 50 characters!")]
        public string Title { get; set; }
        [Required]
        [MaxLength(17,ErrorMessage="ISBN can not be longer than 17 characters")]
        public string ISBN { get; set; }
        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime CreatedAt { get; set; }
        public string? Image { get; set; }
        [Required]
        public bool IsAvailable { get; set; } = true;//available to borrow
        public bool IsDeleted { get; set; }=false;
        //Navigation property
        public virtual ICollection<Borrow>? Borrows { get; set; }
        public virtual ICollection<BookGenre>? BookGenres { get; set; }



    }
}
