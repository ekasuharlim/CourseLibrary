using AutoMapper;
using CourseLibrary.API.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseLibrary.API.Profiles
{
    public class CourseProfile : Profile
    {
        public CourseProfile() 
        {
            CreateMap<Entities.Course, Models.CourseDto>();
            CreateMap<Models.CourseDtoForCreate,Entities.Course>();
            CreateMap<Models.CourseDtoForUpdate, Entities.Course>();
        }
        
    }
}
