

using System.ComponentModel.DataAnnotations;

namespace LMS.Core.Entities
{
    public class Genre
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Genre can not exceed 50 characters!")]
       // [UniqueName] //sheltha 3shan kont ha ahtah a3ml check 3ala isdeleted kaman
        public string Name { get; set; }
        public bool IsDeleted { get; set; } = false;
        //Navigation Property
        public virtual ICollection<BookGenre>? BookGenres { get; set; }

    }
}
