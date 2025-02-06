using LMS.Core.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LMS.Api.DTOs
{
    public class BorrowAndPenaltyDTO
    {
       
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        //bookname
        public string BookName { get; set; }
        public string Author { get; set; }
        public string UserName { get; set; }
        public decimal PenaltyAmount {  get; set; }
       

    }
}
