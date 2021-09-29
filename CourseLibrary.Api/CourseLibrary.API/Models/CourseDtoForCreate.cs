using CourseLibrary.API.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseLibrary.API.Models
{
    [CourseTitleMustHaveAuthorName("Exa")]
    public class CourseDtoForCreate : IValidatableObject
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }
        [MaxLength(1500)]
        public string Description { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var data = (CourseDtoForCreate)validationContext.ObjectInstance;
            if (data.Title == data.Description) 
            {
                yield return new ValidationResult("Title should be different from description",new string[] { "CourseDtoForCreate"});
            }
        }
    }
}
