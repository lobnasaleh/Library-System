
using LMS.Core.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Core.Entities
{
    public class ApplicationUser:IdentityUser
    {
        //el hagat ely 3ayza azwedha 3ala el user fel identity table

        [Required]
        [StringLength(50,ErrorMessage ="Name can not exceed 50 characters!")]
        public string Name { get; set; }
        public string? Image {  get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public Gender Gender { get; set; }
    
        /// /////
        
        public bool IsDeleted { get; set; } = false;

        //Navigation Properties
        public virtual ICollection<Borrow>? Borrows { get; set; }

    }

   
}
