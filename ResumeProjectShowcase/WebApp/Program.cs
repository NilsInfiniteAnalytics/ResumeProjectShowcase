using Radzen;
using WebApp.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddRadzenComponents();

builder.Services.AddHttpClient();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

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

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
