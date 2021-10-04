using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseLibrary.API.Models
{
    public class CourseDtoForUpdate : CourseDtoForManipulation
    {
        [Required]
        public override string Description { get => base.Description; set => base.Description = value; }
    }
}
