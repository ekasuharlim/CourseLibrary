using CourseLibrary.API.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseLibrary.API.ValidationAttributes
{
    public class CourseTitleNotTheSameWithDescription : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var course = (CourseDtoForManipulation)validationContext.ObjectInstance;
            if (course.Title == course.Description) 
            {
                return new ValidationResult("Title must be different with description");
            }
            return ValidationResult.Success;
        }
    }
}
