using System.Reflection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Stl.DependencyInjection;
using Stl.Fusion.Bridge;
using Stl.Fusion.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Stl.Fusion;
using System;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Stl.Fusion.Server;
using Stl.Fusion.Server.Controllers;
using Stl.Fusion.Server.Authentication;
using Stl.Fusion.Blazor;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Templates.TodoApp.UI;
using Dipterv.Shared.Interfaces;
using Dipterv.Bll.Services;
using Dipterv.Shared.Interfaces.ComputeServices;
using Dipterv.Dal.Model;
using Microsoft.AspNetCore.Identity;
using Dipterv.Dal.DbContext;
using Microsoft.EntityFrameworkCore;
using Stl.Fusion.EntityFramework;
using Stl.Fusion.EntityFramework.Authentication;
using Dipterv.Bll.Mappings;

namespace Templates.TodoApp.Host;

public class Startup
{
    private IConfiguration Cfg { get; }
    private IWebHostEnvironment Env { get; }
    private HostSettings HostSettings { get; set; } = null!;
    public Startup(IConfiguration cfg, IWebHostEnvironment environment)
    {
        Cfg = cfg;
        Env = environment;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        
        services.AddSettings<HostSettings>();
#pragma warning disable ASP0000
        HostSettings = services.BuildServiceProvider().GetRequiredService<HostSettings>();
#pragma warning restore ASP0000





        // Fusion services
        services.AddSingleton(new Publisher.Options() { Id = HostSettings.PublisherId });
        var fusion = services.AddFusion();
        var fusionServer = fusion.AddWebServer();
        var fusionClient = fusion.AddRestEaseClient();
        var fusionAuth = fusion.AddAuthentication().AddServer(
            signInControllerSettingsFactory: _ => SignInController.DefaultSettings with
            {
                DefaultScheme = MicrosoftAccountDefaults.AuthenticationScheme,
                SignInPropertiesBuilder = (_, properties) =>
                {
                    properties.IsPersistent = true;
                }
            },
            serverAuthHelperSettingsFactory: _ => ServerAuthHelper.DefaultSettings with
            {
                NameClaimKeys = Array.Empty<string>(),
            });

        fusion.AddComputeService<IProductService, ProductService>();
        fusion.AddComputeService<IProductReviewService, ProductReviewService>();
        fusion.AddComputeService<IProductInventoryService, ProductInventoryService>();
        fusion.AddComputeService<IDateService, DateService>();
        fusion.AddComputeService<ISpecialOfferService, SpecialOfferService>();
        fusion.AddComputeService<ICustomerService, CustomerService>();
        fusion.AddComputeService<IOrderService, OrderService>();

        services.AddScoped<IAccountService, AccountService>();

        services.AddIdentity<ApplicationUser, IdentityRole<long>>(options =>
        {
            options.User.RequireUniqueEmail = true;

            options.Password.RequireDigit = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
        })
        .AddEntityFrameworkStores<FusionDbContext>()
        .AddDefaultTokenProviders();

        services.AddDbContextFactory<FusionDbContext>(dbContext => {
            dbContext.EnableSensitiveDataLogging();
        });

        services.AddScoped<FusionDbContext>(p => p.GetRequiredService<IDbContextFactory<FusionDbContext>>().CreateDbContext());

        services.AddDbContextServices<FusionDbContext>(dbContext =>
        {
            dbContext.AddEntityResolver<int, Product>();
            dbContext.AddEntityResolver<int, ProductInventory>();
            dbContext.AddEntityResolver<int, ProductReview>();
            dbContext.AddEntityResolver<int, Location>();
            dbContext.AddEntityResolver<int, WorkOrder>();
            dbContext.AddEntityResolver<int, SpecialOffer>();

            dbContext.AddOperations((_, o) => {
                o.UnconditionalWakeUpPeriod = TimeSpan.FromSeconds(1);
            });

            dbContext.AddAuthentication<DbSessionInfo<long>, DbUser<long>, long>();
        });

        services.AddAutoMapper(typeof(MappingProfile));


        // Shared services
        StartupHelper.ConfigureSharedServices(services);

        // ASP.NET Core authentication providers
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        }).AddCookie(options =>
        {
            options.LoginPath = "/signIn";
            options.LogoutPath = "/signOut";
            if (Env.IsDevelopment())
                options.Cookie.SecurePolicy = CookieSecurePolicy.None;
            // This controls the expiration time stored in the cookie itself
            options.ExpireTimeSpan = TimeSpan.FromDays(7);
            options.SlidingExpiration = true;
            // And this controls when the browser forgets the cookie
            options.Events.OnSigningIn = ctx =>
            {
                ctx.CookieOptions.Expires = DateTimeOffset.UtcNow.AddDays(28);
                return Task.CompletedTask;
            };
        });

        // Web
        services.AddRouting();
        services.AddMvc().AddApplicationPart(Assembly.GetExecutingAssembly());
        services.AddServerSideBlazor(o => o.DetailedErrors = true);
        fusionAuth.AddBlazor(o => { }); // Must follow services.AddServerSideBlazor()!
    }

    public void Configure(IApplicationBuilder app, ILogger<Startup> log)
    {


        if (Env.IsDevelopment()) {
            app.UseDeveloperExceptionPage();
            app.UseWebAssemblyDebugging();
        }
        else {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }
        app.UseHttpsRedirection();

        app.UseWebSockets(new WebSocketOptions() {
            KeepAliveInterval = TimeSpan.FromSeconds(30),
        });
        app.UseFusionSession();

        // Static + Swagger
        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles();

        // API controllers
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => {
            endpoints.MapBlazorHub();
            endpoints.MapFusionWebSocketServer();
            endpoints.MapControllers();
            endpoints.MapFallbackToPage("/_Host");
        });
    }
}
