using Data.Contexts;
using Microsoft.EntityFrameworkCore;
using WebApp_MVC.Models;
using Business.Interfaces;
using Business.Services;
using WebApp_MVC.Controllers;
using Business.Models;
using Data.Entities;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Data.Repositories;
using Data.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using WebApp_MVC.Hubs;
using WebApp_MVC.Handlers;



var builder = WebApplication.CreateBuilder(args);

// In Program.cs
builder.Services.AddDbContextFactory<DataContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    
    )
    .ConfigureWarnings(warnings =>
        warnings.Log(RelationalEventId.PendingModelChangesWarning))
);

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(x =>
    {
        x.Password.RequiredLength = 8;
        x.Password.RequireNonAlphanumeric = true;
        x.Password.RequireDigit = true;
        x.User.RequireUniqueEmail = true;
        x.SignIn.RequireConfirmedEmail = false;
        x.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => !context.Request.Cookies.ContainsKey("cookieConsent");
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
});

builder.Services.ConfigureApplicationCookie(x =>
    {
        x.LoginPath = "/auth/signin";
        x.LogoutPath = "/auth/signout";
        x.AccessDeniedPath = "/auth/denied";
        x.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        x.SlidingExpiration = true;
        x.Cookie.HttpOnly = true;
        x.Cookie.SameSite = SameSiteMode.Lax; //Cookiehanteringen skapas av tredje part 
        x.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

builder.Services.AddAuthentication(x =>
{
    x.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
    .AddCookie()
    .AddGoogle(x =>
    {
        x.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? "";
        x.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? "";
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("Admins", policy => policy.RequireRole("Admin"))
    .AddPolicy("Managers", policy => policy.RequireRole("Admin", "Manager"));

var connectionString = builder.Configuration.GetConnectionString("AzureBlobStorage");
var containerName = "images";

builder.Services.AddScoped<IFileHandler>(_ => new AzureFileHandler(connectionString!, containerName)); 
builder.Services.AddTransient<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddTransient<IAddressRepository, AddressRepository>();
builder.Services.AddTransient<IClientRepository, ClientRepository>();
builder.Services.AddTransient<INotificationRepository, NotificationRepository>();
builder.Services.AddTransient<INotificationDismissedRepository, NotificationDismissedRepository>();

builder.Services.AddTransient<IProjectService, ProjectService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddTransient<IAddressService, AddressService>();
builder.Services.AddTransient<IClientService, ClientService>();
builder.Services.AddTransient<ISearchService, SearchService>();
builder.Services.AddTransient<INotificationService, NotificationService>();

builder.Services.AddScoped<ClientDto>(); 
builder.Services.AddScoped<ProjectDto>();
builder.Services.AddScoped<AppUserDto>();

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<HomeController>();
builder.Services.AddScoped<AuthController>();
builder.Services.AddScoped<BaseController>();
builder.Services.AddScoped<ClientController>();
builder.Services.AddScoped<EmployeeController>();
builder.Services.AddScoped<ProjectController>();
builder.Services.AddScoped<SearchController>();
builder.Services.AddScoped<CookiesController>();
builder.Services.AddScoped<NotificationsController>();

builder.Services.AddSignalR();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseStaticFiles();

app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();

await Data.Seed.DataSeeder.SeedData(app.Services);

app.MapHub<ChatHub>("/chathub");
app.MapHub<NotificationHub>("/notificationhub");

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();


