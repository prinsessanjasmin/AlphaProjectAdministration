using Data.Contexts;
using Microsoft.EntityFrameworkCore;
using WebApp_MVC.Models;
using Business.Interfaces;
using Business.Services;
using WebApp_MVC.Controllers;
using Business.Models;
using Data.Entities;
using Microsoft.AspNetCore.Identity;



var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(x =>
    {
        x.Password.RequiredLength = 8;
        x.Password.RequireNonAlphanumeric = true;
        x.Password.RequireDigit = true;
        x.User.RequireUniqueEmail = true;
        x.Stores.ProtectPersonalData = true;
    })
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(x =>
    {
        x.LoginPath = "/auth/signin";
        x.LogoutPath = "/auth/signout";
        x.AccessDeniedPath = "/auth/denied";
        x.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        x.SlidingExpiration = true;
    });

builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IClientService, ClientService>();

builder.Services.AddScoped<SuccessResult>();
builder.Services.AddScoped<ErrorResult>();

builder.Services.AddScoped<ClientFormModel>(); 
builder.Services.AddScoped<LoginFormModel>();
builder.Services.AddScoped<MemberFormModel>();
builder.Services.AddScoped<ProjectFormModel>();
builder.Services.AddScoped<SignUpFormModel>();

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<AuthController>();
builder.Services.AddScoped<HomeController>();
builder.Services.AddScoped<ClientController>();
builder.Services.AddScoped<EmployeeController>();
builder.Services.AddScoped<ProjectController>();

builder.Services.AddScoped<AddProjectViewModel>();

var app = builder.Build();

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
