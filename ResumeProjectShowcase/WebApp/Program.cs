using Radzen;
using Serilog;
using WebApp.Components;
using Azure.Identity;
using ClassLibrary.Interfaces;
using ClassLibrary.Modules.OpenMeteoService;
using ClassLibrary.Modules.UnitsConverterService;
using ClassLibrary.Modules.GeocodingService;
using ClassLibrary.Modules.HumidAirPropertiesService;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));
var logger = builder.Services.BuildServiceProvider().GetRequiredService<Serilog.ILogger>();
try
{
    var azAppConfigConnection = builder.Configuration["AppConfig"];
    var azAppConfigEndpoint = Environment.GetEnvironmentVariable("AZURE_APPCONFIGURATION_ENDPOINT");
    if (!string.IsNullOrEmpty(azAppConfigConnection))
    {
        builder.Configuration.AddAzureAppConfiguration(options =>
        {
            options.Connect(azAppConfigConnection)
                .ConfigureRefresh(refresh =>
                {
                    // All configuration values will be refreshed if the sentinel key changes.
                    refresh.Register("TestApp:Settings:Sentinel", refreshAll: true);
                });
        });
    }
    else if (!string.IsNullOrEmpty(azAppConfigEndpoint))
    {
        builder.Configuration.AddAzureAppConfiguration(options =>
        {
            options.Connect(new Uri(azAppConfigEndpoint), new DefaultAzureCredential())
                .ConfigureRefresh(refresh =>
                {
                    // All configuration values will be refreshed if the sentinel key changes.
                    refresh.Register("TestApp:Settings:Sentinel", refreshAll: true);
                });
        });
    }
    else if (builder.Environment.IsDevelopment())
    {
        // Use the local.settings.json file for local development.
        builder.Configuration.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);
    }
    else
    {
        throw new Exception("No valid Azure App Configuration connection string or endpoint found.");
    }
    builder.Services.AddAzureAppConfiguration();
}
catch (Exception e)
{
    logger.Error(e, "Error while configuring Azure App Configuration");
    throw;
}

builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services
    .AddRadzenComponents();
builder.Services
    .AddHttpClient();



var openMeteoArchiveApiUrl = builder.Configuration["OpenMeteoArchiveApiUrl"];
if (!string.IsNullOrEmpty(openMeteoArchiveApiUrl))
{
    builder.Services.AddSingleton<IOpenMeteoService>(sp =>
    {
        var httpClient = sp.GetRequiredService<HttpClient>();
        return new OpenMeteoService(httpClient, openMeteoArchiveApiUrl, logger);
    });
}

builder.Services.AddTransient<IUnitsConverterService, UnitsConverterService>();
builder.Services.AddTransient<IHumidAirPropertiesService, HumidAirPropertiesService>();
builder.Services.AddTransient<IGeocodingService>(sp =>
{
    var httpClient = sp.GetRequiredService<HttpClient>();
    return new GeocodingService(httpClient);
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