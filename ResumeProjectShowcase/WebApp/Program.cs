using Azure.Identity;
using ClassLibrary.Interfaces;
using ClassLibrary.Modules.GeocodingService;
using ClassLibrary.Modules.HumidAirPropertiesService;
using ClassLibrary.Modules.OpenMeteoService;
using ClassLibrary.Modules.UnitsConverterService;
using MudBlazor.Services;
using Serilog;
using WebApp.Components;


Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
                              .AddJsonFile("appsettings.json", optional: true)
                              .Build())
    .Enrich.FromLogContext()
    .CreateLogger();

try
{
    Log.Information("Starting web host");
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    // Configure Azure App Configuration
    var configuration = builder.Configuration;
    var azAppConfigConnection = configuration["AppConfig"];
    var azAppConfigEndpoint = Environment.GetEnvironmentVariable("AZURE_APPCONFIGURATION_ENDPOINT");

    if (!string.IsNullOrEmpty(azAppConfigConnection))
    {
        configuration.AddAzureAppConfiguration(options =>
        {
            options.Connect(azAppConfigConnection)
                   .ConfigureRefresh(refresh =>
                   {
                       refresh.Register("TestApp:Settings:Sentinel", refreshAll: true);
                   });
        });
    }
    else if (!string.IsNullOrEmpty(azAppConfigEndpoint))
    {
        configuration.AddAzureAppConfiguration(options =>
        {
            options.Connect(new Uri(azAppConfigEndpoint), new DefaultAzureCredential())
                   .ConfigureRefresh(refresh =>
                   {
                       refresh.Register("TestApp:Settings:Sentinel", refreshAll: true);
                   });
        });
    }
    else if (builder.Environment.IsDevelopment())
    {
        configuration.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);
    }
    else
    {
        throw new Exception("No valid Azure App Configuration connection string or endpoint found.");
    }
    builder.Services.AddAzureAppConfiguration();

    // Register Razor components and HttpClient.
    builder.Services
        .AddRazorComponents()
        .AddInteractiveServerComponents();
    builder.Services.AddHttpClient();

    // Register OpenMeteoService only if the URL is provided.
    var openMeteoArchiveApiUrl = configuration["OpenMeteoArchiveApiUrl"];
    if (!string.IsNullOrEmpty(openMeteoArchiveApiUrl))
    {
        builder.Services.AddSingleton<IOpenMeteoService>(sp =>
        {
            var httpClient = sp.GetRequiredService<HttpClient>();
            var logger = sp.GetRequiredService<ILogger<GeocodingService>>();
            return new OpenMeteoService(httpClient, openMeteoArchiveApiUrl, logger);
        });
    }
    else
    {
        Log.Warning("OpenMeteoArchiveApiUrl is not configured. OpenMeteoService will not be registered.");
    }

    builder.Services.AddMudServices();
    builder.Services.AddTransient<IUnitsConverterService, UnitsConverterService>();
    builder.Services.AddTransient<IHumidAirPropertiesService, HumidAirPropertiesService>();
    builder.Services.AddTransient<IGeocodingService>(sp =>
    {
        var httpClient = sp.GetRequiredService<HttpClient>();
        var logger = sp.GetRequiredService<ILogger<GeocodingService>>();
        return new GeocodingService(httpClient, logger);
    });

    builder.Services.AddServerSideBlazor().AddCircuitOptions(o =>
    {
        o.DetailedErrors = builder.Environment.IsDevelopment();
    });

    var app = builder.Build();

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error", createScopeForErrors: true);
        app.UseHsts();
    }

    app.UseAzureAppConfiguration();
    app.UseHttpsRedirection();
    app.UseStaticFiles(new StaticFileOptions
    {
        OnPrepareResponse = ctx =>
        {
            ctx.Context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.CacheControl] = "no-cache, no-store, must-revalidate";
            ctx.Context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Pragma] = "no-cache";
            ctx.Context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Expires] = "0";
        }
    });
    app.UseSerilogRequestLogging();
    app.UseAntiforgery();
    app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}