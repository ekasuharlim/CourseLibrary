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
using CourseLibrary.API.Models.ResourceParameter;

namespace CourseLibrary.API.Controllers
{
    [ApiController]
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
        [HttpHead]
        public ActionResult<IEnumerable<Author>> GetAuthors(
            [FromQuery] AuthorResourceParameter parameters)            
        {
            var authors = this.repo.GetAuthors(parameters);
            return Ok(mapper.Map<IEnumerable<AuthorDto>>(authors));            
        }

        [HttpGet("{authorId}", Name ="GetAuthor")]
        [HttpHead("{authorId}")]
        public ActionResult<AuthorDto> GetAuthor(Guid authorId)
        {
            var author = this.repo.GetAuthor(authorId);
            Console.WriteLine(author.Courses.Count());
            if (author == null) return NotFound();
            return Ok(mapper.Map<Author, AuthorDto>(author));            
        }

        [HttpPost]
        public ActionResult<AuthorDto> CreateAuthor(AuthorDtoForCreate author) 
        {
            if (author == null) 
            {
                return BadRequest();
            }
            var authorEntity = mapper.Map<AuthorDtoForCreate, Author>(author);
            this.repo.AddAuthor(authorEntity);
            this.repo.Save();

            var authorToReturn = mapper.Map<Author, AuthorDto>(authorEntity);

            return CreatedAtRoute("GetAuthor", new { AuthorId = authorToReturn.Id }, authorToReturn);
        }

    }
}
