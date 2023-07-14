using CitiesManager.Core.Domain.RepositoryContracts;
using CitiesManager.Core.ServiceContracts;
using CitiesManager.Core.Services;
using CitiesManager.Infrastructure.Repositories;
using CitiesManager.WebAPI.DatabaseContext;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

// 12.07.2023. Valid
using FluentValidation.AspNetCore;                      // 12.07.2023.
using CitiesManager.Core.Helpers.Validators;            // 12.07.2023.    
using Microsoft.Extensions.DependencyInjection;         // 12.07.2023.
using System.Reflection;                                // 12.07.2023.
using Microsoft.AspNetCore.Mvc.ApplicationParts;        // 12.07.2023.    
using Microsoft.AspNetCore.Identity;
using CitiesManager.Core.DTO;
//using System.Reflection;                              // 12.07.2023.

namespace CitiesManager.WebAPI.StartupExtensions
{
    public static class ConfigureServicesExtension
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddTransient<ResponseHeaderActionFilter>();

            ////it adds controllers and views as services
            //services.AddControllersWithViews(options => {
            //    //options.Filters.Add<ResponseHeaderActionFilter>(5);

            //    var logger = services.BuildServiceProvider().GetRequiredService<ILogger<ResponseHeaderActionFilter>>();

            //    options.Filters.Add(new ResponseHeaderActionFilter(logger)
            //    {
            //        Key = "My-Key-From-Global",
            //        Value = "My-Value-From-Global",
            //        Order = 2
            //    });
            //});

           
            services.AddControllers();

            // 12.07.2023.
            // ovo ne radi !!!
            //services.AddControllers()
            //        .AddFluentValidation(fv =>
            //        {
            //            fv.RegisterValidatorsFromAssemblyContaining<CityAddValidator>();
            //            fv.RegisterValidatorsFromAssemblyContaining<CityUpdateValidator>();
            //        });


            //add services into IoC container
            // 04.07.2023. - pobrisao višak servise (svaki je bio za zasebnu CRUD operaciju)
            services.AddScoped<ICitiesRepository, CitiesRepository>();
            services.AddScoped<ICitiesServiceCRUD, CitiesServiceCRUD>();

            // 12.07.2023. - 13.07.2023. - može i AddTransient
            services.AddScoped<CityAddValidator>();
            services.AddScoped<CityUpdateValidator>();

            // 14.07.2023. Ver_2
            // Jedna klasična BO validacija koja nije ograničena samo na atribute...
            //services.AddScoped<CityBOValidaros>();
            //// ne znam zašto ovo ne radi ???
            ////services.AddScoped<ICityBOValidaros, CityBOValidaros>();



            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            //services.AddTransient<CitiesListActionFilter>();

            services.AddHttpLogging(options =>
            {
                options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestProperties | Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponsePropertiesAndHeaders;
            });

            // 12.07.2023. MJ valid - javlja gresku !!!
            //services.AddValidatorsFromAssembly(ApplicationAssemblyReference.Assembly);



            return services;
        }
    }
}
