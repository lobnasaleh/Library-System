
using LMS.Core.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace LMS.Api.Validations
{
    public class UniqueName:ValidationAttribute
    {
        protected override  ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return null;    
            }
            string name=value.ToString();

            //ma ansash a3melha bel dependecy injection haaa
           
            var context=(IGenreDuplicateChecker)validationContext.GetService(typeof(IGenreDuplicateChecker));
           bool notunique= context.IsDuplicateGenre(name).GetAwaiter().GetResult();

            if (notunique) {
                return new ValidationResult("This Genre already exists");
            }
           

           return ValidationResult.Success;
        }
    }
}
