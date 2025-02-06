using LMS.Core.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace LMS.Api.DTOs
{
    public class RegisterRequestDTO
    {
        [Required]
        [StringLength(50, ErrorMessage = "Name can not exceed 50 characters!")]
        public string Name { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Username can only contain letters, numbers, and underscores.")]
        [StringLength(50, ErrorMessage = "Username can not exceed 50 characters!")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string? Image { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public Gender Gender { get; set; }
    }
}
