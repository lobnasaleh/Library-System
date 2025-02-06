using LMS.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace LMS.Api.DTOs
{
    public class BookWithGenreDTO
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public List<string> genres { get; set; }= new List<string>();
    }
}
