using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LMS.Api.DTOs
{
    public class BookUpdateDTO
    {
        [Required]
        [StringLength(50, ErrorMessage = "Name can not exceed 50 characters!")]
        public string Name { get; set; }
        [StringLength(500, ErrorMessage = "Description can not exceed 500 characters!")]
        public string? Description { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Author Name can not exceed 50 characters!")]
        public string Author { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Title can not exceed 50 characters!")]
        public string Title { get; set; }
        [Required]
        [MaxLength(17, ErrorMessage = "ISBN can not be longer than 17 characters")]
        public string ISBN { get; set; }
        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime CreatedAt { get; set; }
        public string? Image { get; set; }
        [Required]
        public bool IsAvailable { get; set; } = true;//available to borrow
    
    }
}
