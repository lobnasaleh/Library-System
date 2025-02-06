using LMS.Core.Entities;

using System.ComponentModel.DataAnnotations;

namespace LMS.Api.DTOs
{
    public class GenreCreateDTO
    {
        [Required]
        [StringLength(50, ErrorMessage = "Genre can not exceed 50 characters!")]
       // [UniqueName]
        public string Name { get; set; }

    }
}
