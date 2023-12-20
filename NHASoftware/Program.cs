using AutoMapper;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.FeatureManagement;
using NHA.Helpers.ImageDataSourceTranslator;
using NHA.Website.Software.HangfireFilters;
using NHA.Website.Software.RequestDurationMiddleware;
using NHA.Website.Software.Services.CacheLoadingManager;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;
using NHA.Website.Software.Services.Social;
using NHA.Website.Software.Services.CacheGoblin;
using NHA.Website.Software.Services.Forums;
using NHA.Website.Software.Services.FileExtensionValidator;
using NHA.Website.Software.Services.CookieMonster;
using NHA.Website.Software.Services.AccessWarden;
using NHA.Website.Software.Services.Anime;
using NHA.Website.Software.Services.FriendSystem;
using NHA.Website.Software.Services.SendGrid;
using NHA.Website.Software.Services.SendGrid.Configuration;
using NHA.Helpers.HtmlStringCleaner;
using NHA.Website.Software.DBContext;
using NHA.Website.Software.Entities.Identity;
using NHA.Website.Software.Profiles;
using NHA.Website.Software.Services.Social.PostBuilderService;
using NHA.Website.Software.Services.Time;

//Creates instance of WebApplicationBuilder Class
var builder = WebApplication.CreateBuilder(args);

// gets the connectionString from Configuration.
var connectionString = builder.Configuration["ConnectionStrings:DefaultConnection"];

if (builder.Environment.IsProduction())
{
    var secretClient = new SecretClient(
        new Uri($"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/"),
        new DefaultAzureCredential());

    builder.Configuration.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());
}

if (builder.Environment.IsProduction())
{
    builder.Configuration.AddAzureAppConfiguration(options =>
    {
        options.Connect(builder.Configuration["ConnectionStrings:AppConfigurationConnection"]).UseFeatureFlags();
    });
}

var mapperConfig = new MapperConfiguration(mc =>
{
    //Automapper configuration.
    mc.AddProfile(new MappingProfile());
});
IMapper mapper = mapperConfig.CreateMapper();

#region ManageBuilderServices

//Application database config
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString!));
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
    options.ApiKey = builder.Configuration["SendGrid:ApiKey"]!;
    options.SenderEmail = builder.Configuration["SendGrid:SenderEmail"]!;
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
builder.Services.AddTransient<ITimeBender, TimeBender>();
builder.Services.AddTransient<IPostBuilder, PostBuilder>();


//Cookie service
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient<ICookieMonster, CookieMonster>();
builder.Services.AddSingleton(typeof(ICacheGoblin<>), typeof(CacheGoblin<>));
builder.Services.AddTransient<IFriendSystem, FriendSystem>();
builder.Services.AddTransient<IImageDataSourceTranslator, ImageDataSourceTranslator>();
builder.Services.AddSingleton<ICacheLoadingManager, CacheLoadingManager>();


builder.Services.AddHangfireServer();
builder.Services.AddMemoryCache();

#endregion

//Creates the Web application object by calling the WebBuilder.Build() method. All services should be added before here. 
var app = builder.Build();

var logger = app.Services.GetService<ILogger<Program>>();
logger!.LogTrace($"Web App starting environment is production - {app.Environment.IsProduction().ToString()}");
logger!.LogTrace($"Web App starting environment is development - {app.Environment.IsDevelopment().ToString()}");
logger!.LogTrace($"Web App builder starting environment is production - {builder.Environment.IsProduction().ToString()}");
logger!.LogTrace($"Web App builder environment is development - {builder.Environment.IsDevelopment().ToString()}");
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

if (app.Environment.IsProduction())
{
    app.UseAzureAppConfiguration();
}

//Custom Middleware to log request duration
app.UseRequestDurationMiddleware();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//Enables app cors property with the Binance api policy.
app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.GetService<IServiceScopeFactory>()!.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.Migrate();
}

//App Hang fire Configuration.

app.UseHangfireDashboard("/hangfire", new DashboardOptions()
{
    //Passes the authorization to app. 
    Authorization = new []{new MyAuthorizationFilter()}
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

//Used by postbuilder to access claims.current
app.Use(async (context, next) =>
{
    Thread.CurrentPrincipal = context.User;
    await next(context);
});

app.Run();



