using AutoMapper;
using CourseLibrary.API.Models;
using CourseLibrary.API.Entities;
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
        private readonly IMapper mapper;

        public AuthorsController(ICourseLibraryRepository repo, IMapper mapper )
        {
            this.repo = repo;
            this.mapper = mapper;
        }
        [HttpGet]
        public ActionResult<IEnumerable<Author>> GetAuthors()
        {
            var authors = this.repo.GetAuthors();
            return Ok(mapper.Map<IEnumerable<AuthorDto>>(authors));
            
        }

        [HttpGet("{authorId}")]
        public ActionResult<AuthorDto> GetAuthor(Guid authorId)
        {
            var author = this.repo.GetAuthor(authorId);
            Console.WriteLine(author.Courses.Count());
            if (author == null) return NotFound();
            return Ok(mapper.Map<Author, AuthorDto>(author));
            
        }

    }
}
