
using System.Configuration;
using AutoMapper;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NHASoftware.Data;
using NHASoftware.Profiles;

//Creates instance of WebApplicationBuilder Class
var builder = WebApplication.CreateBuilder(args);

// gets the connectionString from Configuration.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

//Automapper configuration.
var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
});

IMapper mapper = mapperConfig.CreateMapper();

#region ManageBuilderServices

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

#region CorsPolicy

//CORS policy setup. Allows calls to binace api ----------------------------------------------------------------------->

var  MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        builder =>
        {
            builder.WithOrigins("https://api.binance.com/");
            builder.WithHeaders("Access-Control-Allow-Headers");
        });
});

//CORS policy setup End ------------------------------------------------------------------------------------------------->

#endregion

builder.Services.AddSingleton(mapper);

//Hangfire service setup.
builder.Services.AddHangfire(options =>
{
    options.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"));
});

#endregion

//Creates the Webapplication object by calling the WebBuilder.Build() method. All services should be added before here. 
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//Enables app cors property with the Binance api policy.
app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.UseHangfireDashboard();
app.UseHangfireServer();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();



