using CitiesManager.Core.DTO;
using CitiesManager.WebAPI.Middleware;
using CitiesManager.WebAPI.StartupExtensions;
using Serilog;


//
// Ovo je jednostavan primer app gde sam pokušao primenu Clean Arch. s tim da sam akcenat stavio na: Pagination + Filter + Sort
// Nisam obuhvatio u ovom momentu Auth., Autent., JWT ... - to veæ od ranije imam rešeno
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

app.UseAuthorization();

app.MapControllers();


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

app.Run();

 