using CitiesManager.WebAPI.StartupExtensions;
using Serilog;

//
// Ovo je jednostavan primer app gde sam pokušao primenu Clean Arch. s tim da sam akcenat stavio na: Pafination + Filter + Sort
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


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHsts();                          // 18.06.2023.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
