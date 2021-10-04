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
    [CourseTitleNotTheSameWithDescription]
    public class CourseDtoForManipulation
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [MinLength(5)]
        public virtual string Description { get; set; }

    }
}
