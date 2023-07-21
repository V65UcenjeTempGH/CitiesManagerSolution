using CitiesManager.Core.Domain.RepositoryContracts;
using CitiesManager.Core.DTO;
using CitiesManager.Core.Helpers.Validators;
using CitiesManager.WebAPI.Middleware;
using CitiesManager.WebAPI.StartupExtensions;
using Microsoft.AspNetCore.Builder;
using Serilog;


//
// Ovo je jednostavan primer app gde sam pokušao primenu Clean Arch. s tim da sam akcenat stavio na: Pagination + Filter + Sort
// Nisam obuhvatio u ovom momentu Auth., Autent., JWT ...
// To kasnije, kada razrešim pitanje Paginacije kroz Clean Arch.
//
// Napomena: 
// Kao što je nekada kod modularnog programiranja bilo osnovno pitanje "kako da razbijem kod", ovde (OOP Clean Arch.) postavlja se pitanje "gde doðe kod" ???
//
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// 19.06.2023.
//Serilog
builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) => {
    loggerConfiguration
    .ReadFrom.Configuration(context.Configuration) //read configuration settings from built-in IConfiguration
    .ReadFrom.Services(services); //read out current app's services and make them available to serilog
});

// 19.06.2023. -
// UI Folder StartupExtensions -> ConfigureServicesExtension.cs ->                                             public static IServiceCollection ConfigureServices(...) -> return services;
builder.Services.ConfigureServices(builder.Configuration);


WebApplication app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHsts();                          // 18.06.2023.

app.UseHttpsRedirection();

// 20.07.2023.
// Aktiviraj generièki IMiddleware za POST metodu
app.UseWhen(context => context.Request.Method == HttpMethods.Post, appBuilder =>
{
    appBuilder.UseMiddleware<ValidatorMiddleware<CityAddRequest>>();
});

// Aktiviraj generièki IMiddleware za PUT metodu
app.UseWhen(context => context.Request.Method == HttpMethods.Put, appBuilder =>
{
    appBuilder.UseMiddleware<ValidatorMiddleware<CityUpdateRequest>>();
});

app.UseAuthorization();

app.MapControllers();

app.Run();

 