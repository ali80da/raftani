/* RAFTANI - Email Client */

using Email.Web.Components;
using MudBlazor.Services;

// Ensure the required namespace for 'MapStaticAssets' is included

var builder = WebApplication.CreateBuilder(args);
{

    builder.Services.AddRazorComponents().AddInteractiveServerComponents().AddInteractiveWebAssemblyComponents();
    builder.Services.AddControllersWithViews();

    builder.Services.AddMudServices();


    builder.Services.AddHttpClient();

}
var app = builder.Build();
{

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseWebAssemblyDebugging();
    }
    else
    {
        app.UseExceptionHandler("/Error", createScopeForErrors: true);
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    // Redirect
    app.UseHttpsRedirection();

    app.UseStaticFiles();
    //app.MapStaticAssets(); // .NET 9.0+



    app.UseRouting();
    //app.UseSession();

    // Auth
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{Id?}");


    app.UseAntiforgery();

    app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode()
        .AddInteractiveWebAssemblyRenderMode()
        .AddAdditionalAssemblies(typeof(Email.Client._Imports).Assembly);

}
app.Run();