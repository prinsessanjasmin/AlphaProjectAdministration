using Data.Contexts;
using Microsoft.EntityFrameworkCore;
using WebApp_MVC.Models;
using Business.Interfaces;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<AddProjectViewModel>();
builder.Services.AddScoped<IEmployeeService, IEmployeeService>();

builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

// Add services to the container.


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
