/* RAFTANI - Email Client */

var builder = WebApplication.CreateBuilder(args);
{


    builder.Services.AddControllersWithViews();

}
var app = builder.Build();
{




    app.UseStaticFiles();
    app.UseRouting();
    //app.UseSession();

    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{Id?}");

}
app.Run();