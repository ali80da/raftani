/* RAFTANI | ali80da - Email */

using Email.Server.Extensions.Containers;

var builder = WebApplication.CreateBuilder(args);
{



    // Add Services to The Container.

    builder.Services.AddControllers();
    // Configuring Swagger/OpenAPI
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();



    builder.Services.AddCustomCoreServices();

    // HSTS
    if (builder.Environment.IsProduction())
    {
        builder.Services.AddCustomHsts();
    }

}
var app = builder.Build();
{



    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.BackgroundColor = ConsoleColor.DarkBlue;

        app.UseSwagger();
        app.UseSwaggerUI();
    }
    else if (app.Environment.IsProduction())
    {
        app.UseExceptionHandler("/Error", createScopeForErrors: true);

        // The Default HSTS Value is 30 Days.
        app.UseHsts();
    }



    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

}
app.Run();