

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Core.Entities
{
    public class Borrow
    {
        public int Id { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime StartDate { get; set; }
       
        [Required]
        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
     //   [StartAndDue]//custom validation
        
        public DateTime DueDate { get; set; }
        
        [DataType(DataType.Date)] //for ui date only picker
        [Column(TypeName = "date")]//saved as date only
        public DateTime? ReturnDate { get; set; }//lessa marag3sh el ketab

        //Navigation Properties
        public Book? Book { get; set; }

        [ForeignKey(nameof(Book))]
        public virtual int BookId { get; set; }

        public virtual ApplicationUser? User { get; set; }

        [ForeignKey(nameof(User))]
        [Required]
        public string UserId { get; set; }

        [NotMapped] // to tell EF to ignore this property
        public int PenaltyPerDay { get; private set; } = 100;

        [NotMapped] // to tell EF to ignore this property
        public decimal PenaltyAmount
        {
            get
            {
                if ((ReturnDate.HasValue && ReturnDate.Value > DueDate))
                {
                    TimeSpan overduePeriod = ReturnDate.Value - DueDate;//time span lazem teshtaghal 3ala datetime
                    int daysOverdue = (int)Math.Ceiling(overduePeriod.TotalDays);
                    return daysOverdue * PenaltyPerDay;
                }
                if (DateTime.Today > DueDate)
                {
                    TimeSpan overduePeriod = DateTime.Today - DueDate;//time span lazem teshtaghal 3ala datetime
                    int daysOverdue = (int)Math.Ceiling(overduePeriod.TotalDays);
                    return daysOverdue * PenaltyPerDay;
                }

                return 0;
            }

        }

      
    }
}
