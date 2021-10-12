using CourseLibrary.API.DbContexts;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using System;
using System.Threading.Tasks;

namespace CourseLibrary.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(opt =>
            {
                opt.ReturnHttpNotAcceptable = true;
            })
            .AddNewtonsoftJson( option => 
            {
                option.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            })
            .AddXmlDataContractSerializerFormatters()            
            .ConfigureApiBehaviorOptions(options =>
              options.InvalidModelStateResponseFactory = context =>
              {
                  //main functionis to return custom ProblemDetails object
                  var problemDetailsFactory = context.HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();
                  var problemDetail = problemDetailsFactory.CreateValidationProblemDetails(context.HttpContext, context.ModelState);

                  problemDetail.Detail = "See the errors field for details.";
                  problemDetail.Instance = context.HttpContext.Request.Path;

                  var actionExecutingContext = context as ActionExecutingContext;

                  if (context.ModelState.ErrorCount > 0 && actionExecutingContext?.ActionArguments.Count == context.ActionDescriptor.Parameters.Count)
                  {
                      problemDetail.Type = "https://something";
                      problemDetail.Status = StatusCodes.Status422UnprocessableEntity;
                      problemDetail.Title = "One or more validation errors occured";

                      return new UnprocessableEntityObjectResult(problemDetail)
                      {
                          ContentTypes = { "application/problem+json" }
                      };
                  }

                  problemDetail.Status = StatusCodes.Status400BadRequest;
                  problemDetail.Title = "One or more errors on input occured";
                  return new BadRequestObjectResult(problemDetail)
                  {
                      ContentTypes = { "application/problem+json" }
                  };

              }
                );

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddScoped<ICourseLibraryRepository, CourseLibraryRepository>();

            services.AddDbContext<CourseLibraryContext>(options =>
            {
                options.UseSqlServer(
                    @"Server=(localdb)\mssqllocaldb;Database=CourseLibraryDB;Trusted_Connection=True;");
            });

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(ExceptionHandler);
                });
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private async Task ExceptionHandler(HttpContext context)
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("error at server");
        }
    }
}
