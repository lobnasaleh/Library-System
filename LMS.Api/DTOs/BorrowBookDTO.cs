using LMS.Core.Entities;

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using LMS.Api.Validations;

namespace LMS.Api.DTOs
{
    public class BorrowBookDTO
    {
        [Required]
        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        [StartAndDue]//custom validation
        public DateTime? DueDate { get; set; }
        [Required]
        public int? BookId { get; set; }
        //if BookId is missing in the JSON request, it will now be null instead of 0.
      //  The[Required] attribute will correctly detect null and return a validation error.
      
        
        /* sent in header
        * [Required]
        public string UserId { get; set; }*/

    }
}
