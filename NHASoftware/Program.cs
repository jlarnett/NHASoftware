
using AutoMapper;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using NHASoftware.Data;
using NHASoftware.Models;
using NHASoftware.Profiles;
using NHASoftware.Services;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

//Creates instance of WebApplicationBuilder Class
var builder = WebApplication.CreateBuilder(args);

// gets the connectionString from Configuration.
var connectionString = builder.Configuration["ConnectionStrings:DefaultConnection"];

builder.Host.ConfigureAppConfiguration((context, config) =>
{
    /************************************************************
     *  This is setup for Azure Key Vault. 
     ************************************************************/
    if (context.HostingEnvironment.IsProduction())
    {
        var builtConfig = config.Build();
        var secretClient = new SecretClient(
            new Uri($"https://{builtConfig["KeyVaultName"]}.vault.azure.net/"),
            new DefaultAzureCredential());

        config.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());
    }
});

var mapperConfig = new MapperConfiguration(mc =>
{
    //Automapper configuration.
    mc.AddProfile(new MappingProfile());
});
IMapper mapper = mapperConfig.CreateMapper();

#region ManageBuilderServices


//Application database config
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true).AddRoles<IdentityRole>()
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

//Send Grid service setup
builder.Services.Configure<NHASoftware.Configuration.SendGridEmailSenderOptions>(options =>
{
    // Gets sendgrid secrets from azure key config / azure key vault. 
    options.ApiKey = builder.Configuration["SendGrid:ApiKey"];
    options.SenderEmail = builder.Configuration["SendGrid:SenderEmail"];
    options.SenderName = "NHA Industry";
});

builder.Services.AddTransient<IEmailSender, SendGridEmailSender>();


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



//App Hangfire Configuration.

app.UseHangfireDashboard("/hangfire", new DashboardOptions()
{
    //Passes the authorization to app. 
    Authorization = new []{new MyAuthorizationFilter()}
});

app.UseHangfireServer();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();



