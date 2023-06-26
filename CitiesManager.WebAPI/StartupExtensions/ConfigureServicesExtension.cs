﻿using CitiesManager.Core.Domain.RepositoryContracts;
using CitiesManager.Core.ServiceContracts;
using CitiesManager.Core.Services;
using CitiesManager.Infrastructure.Repositories;
using CitiesManager.WebAPI.DatabaseContext;
using Microsoft.EntityFrameworkCore;

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

            //add services into IoC container
            services.AddScoped<ICitiesRepository, CitiesRepository>();

            services.AddScoped<ICitiesGetterService, CitiesGetterService>();
            services.AddScoped<ICitiesAdderService, CitiesAdderService>();
            services.AddScoped<ICitiesDeleterService, CitiesDeleterService>();
            services.AddScoped<ICitiesUpdaterService, CitiesUpdaterService>();
            //builder.Services.AddScoped<ICitiesSorterService, CitiesSorterService>();

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            //services.AddTransient<CitiesListActionFilter>();

            services.AddHttpLogging(options =>
            {
                options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestProperties | Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponsePropertiesAndHeaders;
            });

            return services;
        }
    }
}
