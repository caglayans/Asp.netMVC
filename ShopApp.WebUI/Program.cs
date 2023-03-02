using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using ShopApp.Business.Abstract;
using ShopApp.Business.Concrete;
using ShopApp.DataAccess.Abstract;
using ShopApp.DataAccess.Concrete.EFCore;
using ShopApp.WebUI.EmailServices;
using ShopApp.WebUI.Identity;
using System;
using System.Configuration;
using System.Data;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var provider = builder.Services.BuildServiceProvider();
var configuration = provider.GetRequiredService<IConfiguration>();


builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<ShopDbContext>(options =>
{
    var constring = builder.Configuration.GetConnectionString("MySqlCon");
    options.UseMySql(constring, ServerVersion.AutoDetect(constring));
    options.EnableSensitiveDataLogging();
});


builder.Services.AddDbContext<AppDbContext>(options =>
{
    var constring = builder.Configuration.GetConnectionString("MySqlCon");
    options.UseMySql(constring, ServerVersion.AutoDetect(constring));
});

builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();


builder.Services.Configure<IdentityOptions>(options =>
{
    //Password
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = true;

    //Lockout
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = true;
    options.SignIn.RequireConfirmedPhoneNumber = false;
} );

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/account/login";
    options.LogoutPath = "/account/logout";
    options.AccessDeniedPath = "/account/accessDenied";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(100);

    options.Cookie = new CookieBuilder
    {
        HttpOnly = true,
        Name=".ShopApp.Security.Cookie",
        SameSite=SameSiteMode.Strict
};
});


builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory()));
//builder.Services.AddScoped<IFileProvider>(sp => sp.GetRequiredService<IWebHostEnvironment>().ContentRootFileProvider);

builder.Services.AddScoped<IProductRepository, EFCoreProductRepository>();
builder.Services.AddScoped<IProductService, ProductManager>();
builder.Services.AddScoped<ICategoryRepository, EFCoreCategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryManager>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<ICartRepository, EFCoreCartRepository>();
builder.Services.AddScoped<ICartService, CartManager>();
builder.Services.AddScoped<IOrderRepository, EFCoreOrderRepository>();
builder.Services.AddScoped<IOrderService, OrderManager>();


builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    SeedDatabase.Seed(services);

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    await SeedIdentity.Seed(userManager, roleManager, configuration);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();