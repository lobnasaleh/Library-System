using LMS.Core.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Api.DTOs
{
    public class AssignGenreToBookDTO
    {
        public int BookId { get; set; }
        public int GenreId { get; set; }
    }
}
