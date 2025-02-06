using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LMS.Core.Entities
{
    public class BookGenre
    {

        //Navigation Properties
        public virtual Book? Book { get; set; }

        [ForeignKey(nameof(Book))]
        public int BookId { get; set; }
        public virtual Genre? Genre { get; set; }
        [ForeignKey(nameof(Genre))]
        public int GenreId { get; set; }
    }
}
