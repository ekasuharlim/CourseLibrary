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
using CourseLibrary.API.Helpers;

namespace CourseLibrary.API.Controllers
{
    [ApiController]
    [Route("/api/authorscollection")]
    public class AuthorsCollectionController : ControllerBase
    {
        private readonly ICourseLibraryRepository repo;
        private readonly IMapper mapper;

        public AuthorsCollectionController(ICourseLibraryRepository repo, IMapper mapper)
        {
            this.repo = repo;
            this.mapper = mapper;
        }

        [HttpGet("({ids})",Name = "GetAuthorsCollection")]
        public ActionResult<IEnumerable<AuthorDto>> GetAuthorsCollection(
            [FromRoute]
            [ModelBinder(BinderType = typeof(ArrayModelBinder))]
            IEnumerable<Guid> ids) 
        {
            if (ids == null) 
            {
                return BadRequest();
            }

            var result = this.repo.GetAuthors(ids);
            if (result.Count() != ids.Count()) 
            {
                return NotFound();
            }

            return Ok(this.mapper.Map<IEnumerable<Author>, IEnumerable<AuthorDto>>(result));
        }

        [HttpPost]
        public ActionResult<IEnumerable<AuthorDto>> CreateAuthors(IEnumerable<AuthorDtoForCreate> authorsDto) 
        {
            var authors = this.mapper.Map<IEnumerable<AuthorDtoForCreate>, IEnumerable<Author>>(authorsDto);
            foreach (var author in authors) 
            {
                this.repo.AddAuthor(author);
            }
            this.repo.Save();
            var authorToReturn = this.mapper.Map<IEnumerable<Author>, IEnumerable<AuthorDto>>(authors);
            var listIds = string.Join(",",authorToReturn.Select(a => a.Id).ToArray());
            return CreatedAtRoute("GetAuthorsCollection",new { ids = listIds},authorToReturn);            
        }

    }
}
