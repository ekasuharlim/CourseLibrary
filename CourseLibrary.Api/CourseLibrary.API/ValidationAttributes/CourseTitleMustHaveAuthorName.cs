using CourseLibrary.API.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseLibrary.API.ValidationAttributes
{
    public class CourseTitleMustHaveAuthorName : ValidationAttribute
    {
        private readonly string authorName;

        public CourseTitleMustHaveAuthorName(string authorName) : base("Title must contain author : ")
        {
            this.authorName = authorName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var course = (CourseDtoForCreate) validationContext.ObjectInstance;
            if (!course.Title.Contains(this.authorName))
            {
                return new ValidationResult(this.ErrorMessageString + this.authorName,new string[] { "CourseDtoForCreate" });
            }
            return ValidationResult.Success;
        }
    }
}
