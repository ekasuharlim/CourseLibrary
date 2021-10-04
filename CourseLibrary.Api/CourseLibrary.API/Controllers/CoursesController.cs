using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Models;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;
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
        [HttpGet("{courseId}")]
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

            return CreatedAtRoute("GetCourses",new { authorId = authorId}, this.mapper.Map<Course, CourseDto>(courseToSave));
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
                return NotFound("Invalid course");
            }
            this.mapper.Map<CourseDtoForUpdate, Course>(courseToUpdate, courseForAuthorFromRepo);            
            this.repo.UpdateCourse(courseForAuthorFromRepo);
            this.repo.Save();
            return NoContent();

        }

    }
}
