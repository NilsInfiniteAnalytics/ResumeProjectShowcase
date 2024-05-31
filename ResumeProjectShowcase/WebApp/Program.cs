using Radzen;
using WebApp.Components;
using Azure.Identity;
using ClassLibrary.Interfaces;
using ClassLibrary.Modules.OpenMeteoService;

var builder = WebApplication.CreateBuilder(args);

// Add Azure App Configuration to the container.
var azAppConfigConnection = builder.Configuration["AppConfig"];
if (!string.IsNullOrEmpty(azAppConfigConnection))
{
    // Use the connection string if it is available.
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
else if (Uri.TryCreate(builder.Configuration["Endpoints:AppConfig"], UriKind.Absolute, out var endpoint))
{
    // Use Azure Active Directory authentication.
    // The identity of this app should be assigned 'App Configuration Data Reader' or 'App Configuration Data Owner' role in App Configuration.
    // For more information, please visit https://aka.ms/vs/azure-app-configuration/concept-enable-rbac
    builder.Configuration.AddAzureAppConfiguration(options =>
    {
        options.Connect(endpoint, new VisualStudioCredential())
        .ConfigureRefresh(refresh =>
        {
            // All configuration values will be refreshed if the sentinel key changes.
            refresh.Register("TestApp:Settings:Sentinel", refreshAll: true);
        });
    });
}
builder.Services.AddAzureAppConfiguration();

builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services
    .AddRadzenComponents();
builder.Services
    .AddHttpClient();

var openMeteoArchiveApiUrl = builder.Configuration["OpenMeteoArchiveApiUrl"];
if(!string.IsNullOrEmpty(openMeteoArchiveApiUrl))
{
    builder.Services.AddSingleton<IOpenMeteoService, OpenMeteoService>();
}

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
app.UseAntiforgery(); 
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.Run();