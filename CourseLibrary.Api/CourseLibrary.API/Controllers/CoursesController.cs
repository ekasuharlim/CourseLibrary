using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Models;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseLibrary.API.Controllers
{
    [ApiController]
    [Route("/api/authors/{authorid}/courses")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseLibraryRepository repo;
        private readonly IMapper mapper;

        public CoursesController(ICourseLibraryRepository repo, IMapper mapper) {
            this.repo = repo;
            this.mapper = mapper;
        }

        [HttpGet(Name = "GetCourses")]
        [HttpHead]
        public ActionResult<IEnumerable<CourseDto>> GetCoursesForAuthor(Guid authorId) {
            var authorFromRepo = this.repo.GetAuthor(authorId);

            if (!this.repo.AuthorExists(authorId)){
                return NotFound("Author not found");
            }
            var coursesFromRepo = this.repo.GetCourses(authorId);
            return Ok(this.mapper.Map<IEnumerable<Course>, IEnumerable<CourseDto>>(coursesFromRepo));
        }
        [HttpGet("{courseId}",Name = "GetCourseForAuthor")]
        [HttpHead("{courseId}")]
        public ActionResult<CourseDto> GetCourseForAuthor(Guid authorId, Guid courseId)
        {
            var authorFromRepo = this.repo.GetAuthor(authorId);

            if (authorFromRepo == null)
            {
                return NotFound("Author not found");
            }

            var courseFromRepo = this.repo.GetCourse(authorId, courseId);
            if (courseFromRepo == null) 
            {
                return NotFound("Course not found");
            }

            return Ok(this.mapper.Map<Course, CourseDto>(courseFromRepo));
        }

        [HttpPost]
        public ActionResult<CourseDto> CreateCourseForAuthor(Guid authorId, CourseDtoForCreate courseToCreate) 
        {
            if (!this.repo.AuthorExists(authorId)) 
            {
                return NotFound("Invalid author");
            }

            var courseToSave = this.mapper.Map<CourseDtoForCreate, Course>(courseToCreate);

            this.repo.AddCourse(authorId, courseToSave);
            this.repo.Save();

            return CreatedAtRoute("GetCourseForAuthor", new { authorId = authorId, courseId = courseToSave.Id }, this.mapper.Map<Course, CourseDto>(courseToSave));
        }

        [HttpPut("{courseId}")]
        public ActionResult UpdateCourseForAuthor(Guid authorId, Guid courseId, CourseDtoForUpdate courseToUpdate) 
        {
            if (!this.repo.AuthorExists(authorId))
            {
                return NotFound("Invalid author");
            }

            var courseForAuthorFromRepo = this.repo.GetCourse(authorId, courseId);
            if (courseForAuthorFromRepo == null) 
            {
                var courseToAdd = this.mapper.Map<CourseDtoForUpdate, Course>(courseToUpdate);
                courseToAdd.Id = courseId;
                this.repo.AddCourse(authorId, courseToAdd);
                this.repo.Save();
                var courseToReturn = this.mapper.Map<Course, CourseDto>(courseToAdd);
                return CreatedAtRoute("GetCourseForAuthor", new { authorId = authorId, courseId = courseId }, courseToReturn);
            }

            this.mapper.Map<CourseDtoForUpdate, Course>(courseToUpdate, courseForAuthorFromRepo);            
            this.repo.UpdateCourse(courseForAuthorFromRepo);
            this.repo.Save();
            return NoContent();

        }

        [HttpPatch("{courseId}")]
        public ActionResult UpdatePartialCourseForAuthor(Guid authorId, Guid courseId, JsonPatchDocument<CourseDtoForUpdate> patchDocument) 
        {
            if (!this.repo.AuthorExists(authorId))
            {
                return NotFound("Invalid author");
            }

            var courseForAuthorFromRepo = this.repo.GetCourse(authorId, courseId);
            if (courseForAuthorFromRepo == null)
            {
                return NotFound();
            }
            var courseToPatch = this.mapper.Map<Course, CourseDtoForUpdate>(courseForAuthorFromRepo);

            
            patchDocument.ApplyTo(courseToPatch,ModelState);
            if (!TryValidateModel(courseToPatch)) 
            {
                return ValidationProblem(ModelState);
            }

            this.mapper.Map<CourseDtoForUpdate, Course>(courseToPatch, courseForAuthorFromRepo);
            this.repo.UpdateCourse(courseForAuthorFromRepo);
            this.repo.Save();

            return NoContent();
        }

        [HttpDelete("{courseId}")]
        public ActionResult DeleteCourseFromAuthor(Guid authorId, Guid courseId) 
        {
            if (!this.repo.AuthorExists(authorId))
            {
                return NotFound("Invalid author");
            }

            var courseForAuthorFromRepo = this.repo.GetCourse(authorId, courseId);
            if (courseForAuthorFromRepo == null)
            {
                return NotFound();
            }
            this.repo.DeleteCourse(courseForAuthorFromRepo);
            this.repo.Save();
            return NoContent();
        }
        public override ActionResult ValidationProblem([ActionResultObjectValue] ModelStateDictionary modelStateDictionary)
        {
            var options = HttpContext.RequestServices.GetRequiredService<IOptions<ApiBehaviorOptions>>();
            return  (ActionResult)options.Value.InvalidModelStateResponseFactory(ControllerContext);
        }

    }
}
