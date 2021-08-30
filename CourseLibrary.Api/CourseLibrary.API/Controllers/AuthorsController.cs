using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseLibrary.API.Controllers
{
    [Route("/api/authors")]
    public class AuthorsController : ControllerBase
    {
        private readonly ICourseLibraryRepository repo;

        public AuthorsController(ICourseLibraryRepository repo) 
        {
            this.repo = repo;
        }
        [HttpGet]
        public IActionResult GetAuthors() 
        {
            return  new JsonResult(this.repo.GetAuthors());
        }

        [HttpGet("{authorId}")]
        public IActionResult GetAuthors(Guid authorId)
        {
            var author = this.repo.GetAuthors().FirstOrDefault(a => a.Id == authorId);
            if (author != null)
            {
                return Ok(author);
            }
            else 
            {
                return NotFound();
            }
        }

    }
}
