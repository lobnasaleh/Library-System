
using System.ComponentModel.DataAnnotations;
using LMS.Api.DTOs;
using LMS.Core.Entities;
namespace LMS.Api.Validations
{
    public class StartAndDue:ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
           
            if (validationContext.ObjectInstance is null)
            {
                return new ValidationResult("Invalid object instance.");
            }
            var Borrow = (BorrowBookDTO)validationContext.ObjectInstance;

            var st= Borrow.StartDate;
            var e = Borrow.DueDate;
            if (st >= e)
            {
                return new ValidationResult("Due Date must be greater than start date");
            }

            // No need not retrieving anything from db
            // var context=(ApplicationDBContext)validationContext.GetService(typeof(ApplicationDBContext));



            return ValidationResult.Success;
        }
    }
}
