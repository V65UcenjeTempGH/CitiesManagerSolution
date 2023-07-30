using CitiesManager.Core.Domain.RepositoryContracts;
using CitiesManager.Core.ServiceContracts;
using CitiesManager.Core.Services;
using CitiesManager.Infrastructure.Repositories;
using CitiesManager.WebAPI.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using CitiesManager.Core.Helpers.Validators;            // 12.07.2023.
using FluentValidation;
using CitiesManager.Core.DTO;                           // 15.07.2023.
using CitiesManager.WebAPI.Middleware;


namespace CitiesManager.WebAPI.StartupExtensions
{
    public static class ConfigureServicesExtension
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            //
            // I don't use it for now, ie for this example - this is just a IMiddleware async FluentValidation test !!!
            //
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

            // 28.07.2023.
            //services.AddControllers()
            //        .AddNewtonsoftJson(options =>
            //        {
            //            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            //            options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            //        });


            //add services into IoC container
            services.AddScoped<ICitiesRepository, CitiesRepository>();
            services.AddScoped<ICitiesServiceCRUD, CitiesServiceCRUD>();

            //
            // Register the validators and middleware
            //
            // Register the validators
            services.AddTransient<AbstractValidator<CityAddRequest>, CityAddValidator>();
            services.AddTransient<AbstractValidator<CityUpdateRequest>, CityUpdateValidator>();

            // Register the middleware
            services.AddTransient(typeof(ValidatorMiddleware<CityAddRequest>));
            services.AddTransient(typeof(ValidatorMiddleware<CityUpdateRequest>));
            //

            // Other service registrations...

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });


            services.AddHttpLogging(options =>
            {
                options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestProperties | Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponsePropertiesAndHeaders;
            });

            return services;
        }
    }
}
