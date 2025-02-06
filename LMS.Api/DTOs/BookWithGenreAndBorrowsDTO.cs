namespace LMS.Api.DTOs
{
    public class BookWithGenreAndBorrowsDTO
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public List<string> genres { get; set; }
        public List<string> Borrowers { get; set; }

    }
}
