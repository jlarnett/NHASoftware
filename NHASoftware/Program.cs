
using AutoMapper;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using NHASoftware.Profiles;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.FeatureManagement;
using NHA.Helpers.ImageDataSourceTranslator;
using NHASoftware.DBContext;
using NHASoftware.Entities.Identity;
using NHASoftware.HangfireFilters;
using NHASoftware.Services.Forums;
using NHASoftware.Services.AccessWarden;
using NHASoftware.Services.FileExtensionValidator;
using NHASoftware.Services.SendGrid;
using NHASoftware.Services.RepositoryPatternFoundationals;
using NHAHelpers.HtmlStringCleaner;
using NHASoftware.Services.Anime;
using NHASoftware.Services.CacheGoblin;
using NHASoftware.Services.CookieMonster;
using NHASoftware.Services.FriendSystem;
using NHASoftware.Services.SendGrid.Configuration;
using NHA.Website.Software.Services.Social;

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

builder.Configuration.AddAzureAppConfiguration(options =>
{
    options.Connect(builder.Configuration["ConnectionStrings:AppConfigurationConnection"]).UseFeatureFlags();
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
            builder.WithHeaders("Access-Control-Allow-Origin");
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
builder.Services.Configure<SendGridEmailSenderOptions>(options =>
{
    // Gets sendgrid secrets from azure key config / azure key vault. 
    options.ApiKey = builder.Configuration["SendGrid:ApiKey"];
    options.SenderEmail = builder.Configuration["SendGrid:SenderEmail"];
    options.SenderName = "NHA Industry";
});

//Setup for azure dynamic configurations / feature management
builder.Services.AddAzureAppConfiguration();
builder.Services.AddFeatureManagement();

builder.Services.AddTransient<IEmailSender, SendGridEmailSender>();
builder.Services.AddSingleton<IFileExtensionValidator, FileExtensionValidator>();
builder.Services.AddTransient<IWarden, AccessWarden>();
builder.Services.AddTransient<IHtmlStringCleaner, HtmlStringCleaner>();

//Setup for generic repository system
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddTransient<IForumPostRepository, ForumPostRepository>();
builder.Services.AddTransient<IForumCommentRepository, ForumCommentRepository>();
builder.Services.AddTransient<IForumTopicRepository, ForumTopicRepository>();
builder.Services.AddTransient<IForumSectionRepository, ForumSectionRepository>();
builder.Services.AddTransient<IAnimePageRepository, AnimePageRepository>();
builder.Services.AddTransient<IPostRepository, PostRepository>();
builder.Services.AddTransient<IUserLikeRepository, UserLikeRepository>();
builder.Services.AddTransient<IFriendRepository, FriendRepository>();
builder.Services.AddTransient<IFriendRequestRepository, FriendRequestRepository>();


//Cookie service
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient<ICookieMonster, CookieMonster>();
builder.Services.AddSingleton(typeof(ICacheGoblin<>), typeof(CacheGoblin<>));
builder.Services.AddTransient<IFriendSystem, FriendSystem>();
builder.Services.AddTransient<IImageDataSourceTranslator, ImageDataSourceTranslator>();


builder.Services.AddHangfireServer();

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

//Setup fp for azure dynamic app configurations / Feature flags
app.UseAzureAppConfiguration();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//Enables app cors property with the Binance api policy.
app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.GetService<IServiceScopeFactory>().CreateScope())
{
    scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.Migrate();
}

//App Hangfire Configuration.

app.UseHangfireDashboard("/hangfire", new DashboardOptions()
{
    //Passes the authorization to app. 
    Authorization = new []{new MyAuthorizationFilter()}
});

//app.UseHangfireServer();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();



