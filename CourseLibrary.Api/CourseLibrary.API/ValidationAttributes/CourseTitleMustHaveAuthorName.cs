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
        private readonly string errorMessage;

        public CourseTitleMustHaveAuthorName(string authorName) : base()
        {
            this.authorName = authorName;
        }
        public CourseTitleMustHaveAuthorName(string authorName, string errorMessage = "Title must contain author") : base(errorMessage)
        {
            this.authorName = authorName;
            this.errorMessage = errorMessage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var course = (CourseDtoForCreate) validationContext.ObjectInstance;
            if (!course.Title.Contains(this.authorName))
            {
                return new ValidationResult(this.errorMessage,new string[] { "CourseDtoForCreate" });
            }
            return ValidationResult.Success;
        }
    }
}
