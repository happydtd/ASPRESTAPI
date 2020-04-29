using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using Routine.Api.Data;
using Routine.Api.Helpers;
using Routine.Api.Services;

namespace Routine.Api
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
            services.AddHttpCacheHeaders(expires=> 
            {
                expires.MaxAge = 60;
                expires.CacheLocation = CacheLocation.Private;
            }, validation=>
            {
                validation.MustRevalidate = true;
            });
            //添加缓存器，configure中也要添加
            services.AddResponseCaching();
            //默认输出最高优先级是json,AddNewtonsoftJson（）也是输出json，但AddXmlDataContractSerializerFormatters输出是xml，所以要把这放在最后
            services.AddControllers(
                //support more input/output format
                setup=> { setup.ReturnHttpNotAcceptable = true;
                    //setup.OutputFormatters.Insert(0, new XmlDataContractSerializerOutputFormatter());
                    //setup.OutputFormatters.add(new XmlDataContractSerializerOutputFormatter());
                    setup.CacheProfiles.Add("120sCacheProfile", new CacheProfile
                    {
                        Duration = 120
                    });
                })
                .AddNewtonsoftJson(setup =>
                {
                    setup.SerializerSettings.ContractResolver = 
                    new CamelCasePropertyNamesContractResolver();
                })
                .AddXmlDataContractSerializerFormatters()
                .ConfigureApiBehaviorOptions(
                setup=>setup.InvalidModelStateResponseFactory = context =>
                {
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Type = "http://baidu.com",
                        Title = "something wrong",
                        Status = StatusCodes.Status422UnprocessableEntity,
                        Detail = "see detail",
                        Instance = context.HttpContext.Request.Path
                    };

                    problemDetails.Extensions.Add("traceId", context.HttpContext.TraceIdentifier);
                    return new UnprocessableEntityObjectResult(problemDetails)
                    {
                        ContentTypes = { "application/problem+json" }
                    };
                });
            //add support to vnd.company.hateoas+json
            services.Configure<MvcOptions>(config =>
            {
                var newtonSoftJsonOutputFormatter = config.OutputFormatters.OfType<NewtonsoftJsonOutputFormatter>()?.FirstOrDefault();
                if (newtonSoftJsonOutputFormatter != null)
                {
                    newtonSoftJsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.company.hateoas+json");
                }
            });
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddDbContext<RoutingDbContext>(optionsAction: option => { option.UseSqlite(connectionString: "Data Source=routine.db"); });
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddTransient<IPropertyMappingService, PropertyMappingService>();
            services.AddTransient<IPropertyCheckerService, PropertyCheckerService>();
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
                //return error message in RPOD
                app.UseExceptionHandler(appbuilder => {
                    appbuilder.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("Unexpected Error!");
                    });
                });
            }
            //添加缓存中间件, 微软这个做的不太好
            //app.UseResponseCaching();
            //缓存中间件要写这里
            app.UseHttpCacheHeaders();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
