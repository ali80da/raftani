/* RAFTANI | ali80da - Email */

var builder = WebApplication.CreateBuilder(args);
{



    // Add Services to The Container.

    builder.Services.AddControllers();
    // Configuring Swagger/OpenAPI
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

}
var app = builder.Build();
{



    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

}
app.Run();