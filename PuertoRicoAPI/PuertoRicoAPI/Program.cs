using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using PuertoRicoAPI.Sockets;
using PuertoRicoAPI.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
     .LogTo(Console.WriteLine, LogLevel.None);
});

builder.Services.AddCors(options => options.AddPolicy(name: "UserOrigins",
    policy =>
    {
        policy.WithOrigins("http://192.168.1.110:4200", "http://localhost:4200")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();

        //WithOrigins("http://192.168.1.110:4200", "http://localhost:4200")//
    }));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseCors("UserOrigins");

app.UseHttpsRedirection();


app.UseRouting();

app.UseAuthorization();

// Register the UpdateHub SignalR hub

app.MapHub<UpdateHub>("/api/updateHub");

app.MapControllers();

app.Run();
