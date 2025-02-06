using LMS.Api.Validations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LMS.Api.DTOs
{
    public class BorrowedReturnedBookDTO
    {
       
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string BookName { get; set; }
    
    }
}
