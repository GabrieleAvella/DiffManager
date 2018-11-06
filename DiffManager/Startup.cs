namespace DiffManager
{
    using DiffManager.Common;
    using DiffManager.Common.Interfaces;
    using DiffManager.DataContracts;
    using DiffManager.Domains.Contexts;
    using DiffManager.Domains.Repositories;
    using DiffManager.Domains.RepositoryInterfaces;
    using DiffManager.Domains.UnitOfWorks;
    using DiffManager.Models;
    using DiffManager.Services;
    using DiffManager.Services.Interfaces;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using Newtonsoft.Json.Serialization;

    using DifferenceForCreationServiceDto = Services.DataContracts.DifferenceForCreationDto;
    using DifferenceForUpdateServiceDto = Services.DataContracts.DifferenceForUpdateDto;
    using DifferenceServiceDto = Services.DataContracts.DifferenceDto;

    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(
                    setupAction =>
                        {
                            setupAction.ReturnHttpNotAcceptable = true;
                        })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(
                    options =>
                        {
                            options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                        });

            services.AddDbContext<DifferenceContext>(o => o.UseInMemoryDatabase("InMemoryDb"));

            services.AddScoped<IDifferenceAsyncRepository, DifferenceAsyncRepository>();
            services.AddScoped<IDifferenceUnitOfWork, DifferenceUnitOfWork>();

            services.AddScoped<IDiffsFinder, DiffsFinder>();
            services.AddScoped<IDiffService, DiffService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(
                    appBuilder =>
                    {
                        appBuilder.Run(
                            async context =>
                            {
                                var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                                if (exceptionHandlerFeature != null)
                                {
                                    var logger = loggerFactory.CreateLogger("Global exception logger");
                                    logger.LogError(
                                            500,
                                            exceptionHandlerFeature.Error,
                                            exceptionHandlerFeature.Error.Message);
                                }

                                context.Response.StatusCode = 500;
                                await context.Response.WriteAsync(
                                        "An unexpected fault happened. Try again later.");
                            });
                    });
            }

            AutoMapper.Mapper.Initialize(
                cfg =>
                    {
                        // Service data contracts to Entities
                        cfg.CreateMap<DifferenceForCreationServiceDto, Difference>();
                        cfg.CreateMap<DifferenceForUpdateServiceDto, Difference>();

                        // Service data contracts to Data contracts
                        cfg.CreateMap<DifferenceServiceDto, DifferenceDto>();
                        cfg.CreateMap<DifferenceServiceDto, DifferenceForUpdateDto>();

                        // Data contracts to Service data contracts
                        cfg.CreateMap<DifferenceForCreationDto, DifferenceForCreationServiceDto>();
                        cfg.CreateMap<DifferenceForUpdateDto, DifferenceForUpdateServiceDto>();

                        // Entities to Service data contracts
                        cfg.CreateMap<Difference, DifferenceServiceDto>();
                    });

            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            app.UseMvc();
        }
    }
}
